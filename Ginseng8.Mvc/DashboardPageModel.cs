﻿using Ginseng.Models;
using Ginseng.Mvc.Queries;
using Ginseng.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Ginseng.Mvc
{
	[Authorize]
	public abstract class DashboardPageModel : AppPageModel
	{
		public DashboardPageModel(IConfiguration config) : base(config)
		{
		}

		public bool ShowLabelFilter { get; set; } = true;
		public IEnumerable<OpenWorkItemsResult> WorkItems { get; set; }
		public ILookup<int, Label> SelectedLabels { get; set; }
		public IEnumerable<IGrouping<int, Label>> LabelFilter { get; set; }
		public CommonDropdowns Dropdowns { get; set; }
		public ILookup<int, Comment> Comments { get; set; }
		public ILookup<int, ClosedWorkItemsResult> ClosedItems { get; set; }
		public IEnumerable<WorkDaysResult> WorkDays { get; set; }

		[BindProperty(SupportsGet = true)]
		public int? LabelId { get; set; }

		/// <summary>
		/// Implement this to get the query for the dashboard
		/// </summary>
		protected abstract OpenWorkItems GetQuery();

		protected virtual Func<ClosedWorkItemsResult, int> ClosedItemGrouping
		{
			get { return null; }
		}

		/// <summary>
		/// Override this to populate additional model properties during the OnGetAsync method
		/// </summary>
		protected virtual async Task OnGetInternalAsync(SqlConnection connection)
		{
			await Task.CompletedTask;
		}

		/// <summary>
		/// Override this to populate individual model properties that won't benefit from async execution
		/// </summary>
		protected virtual void OnGetInternal(SqlConnection connection)
		{
			// do nothing by default
		}

		/// <summary>
		/// Override this if you need to redirect from the dashboard page for reason
		/// </summary>
		protected virtual async Task<RedirectResult> GetRedirectAsync(SqlConnection connection)
		{
			return await Task.FromResult<RedirectResult>(null);
		}

		public async Task<IActionResult> OnGetAsync()
		{			
			using (var cn = Data.GetConnection())
			{
				var redirect = await GetRedirectAsync(cn);
				if (redirect != null) return redirect;

				var query = GetQuery();
				if (query != null)
				{
					WorkItems = await GetQuery().ExecuteAsync(cn);

					int[] itemIds = WorkItems.Select(wi => wi.Id).ToArray();
					var labelsInUse = await new LabelsInUse() { WorkItemIds = itemIds, OrgId = OrgId }.ExecuteAsync(cn);
					SelectedLabels = labelsInUse.ToLookup(row => row.WorkItemId);
					LabelFilter = labelsInUse.GroupBy(row => row.Id);
					
					var comments = await new Comments() { OrgId = OrgId, ObjectIds = itemIds }.ExecuteAsync(cn);
					Comments = comments.ToLookup(row => row.ObjectId);

					WorkDays = await new WorkDays() { OrgId = OrgId }.ExecuteAsync(cn);

					if (ClosedItemGrouping != null)
					{
						var closedItems = await new ClosedWorkItems() { OrgId = OrgId, AppId = CurrentOrgUser.CurrentAppId }.ExecuteAsync(cn);
						ClosedItems = closedItems.ToLookup(row => ClosedItemGrouping(row));
					}					
				}

				Dropdowns = await CommonDropdowns.FillAsync(cn, OrgId, CurrentOrgUser.Responsibilities);


				await OnGetInternalAsync(cn);

				OnGetInternal(cn);
			}

			return Page();
		}

		public LoadView GetLoadView(IGrouping<int, OpenWorkItemsResult> milestoneGrp, Func<WorkDaysResult, bool> workDayFilter = null)
		{
			int estimateHours = milestoneGrp.Sum(wi => wi.EstimateHours);

			if (milestoneGrp.First().MilestoneDate.HasValue)
			{
				DateTime milestoneDate = milestoneGrp.First().MilestoneDate.Value;
				return new LoadView()
				{
					EstimateHours = estimateHours,
					WorkHours = WorkDays.Where(wd => wd.Date <= milestoneDate && (workDayFilter?.Invoke(wd) ?? true)).Sum(wi => wi.Hours)
				};
			}
			else
			{
				return new LoadView()
				{
					EstimateHours = estimateHours
				};
			}			
		}
	}
}