﻿using Ginseng.Mvc.Queries;
using Ginseng.Mvc.Queries.SelectLists;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Ginseng.Mvc
{
	public class CommonDropdowns
	{
		private CommonDropdowns()
		{
		}

		public IEnumerable<SelectListItem> Applications { get; set; }
		public IEnumerable<SelectListItem> Projects { get; set; }
		public IEnumerable<SelectListItem> AllActivities { get; set; }
		public IEnumerable<SelectListItem> Sizes { get; set; }
		public IEnumerable<SelectListItem> CloseReasons { get; set; }
		public IEnumerable<SelectListItem> Milestones { get; set; }
		public IEnumerable<SelectListItem> MyActivities { get; set; }

		public SelectList AppSelect(AllWorkItemsResult item)
		{
			return new SelectList(Applications, "Value", "Text", item.ApplicationId);
		}

		public SelectList ProjectSelect(AllWorkItemsResult item)
		{
			return new SelectList(Projects, "Value", "Text", item.ProjectId);
		}

		/// <summary>
		/// I don't think we'll offer an activity select because activity changes require hand-off
		/// </summary>
		public SelectList ActivitySelect(AllWorkItemsResult item)
		{
			return new SelectList(AllActivities, "Value", "Text", item.ActivityId);
		}

		public SelectList SizeSelect(AllWorkItemsResult item)
		{
			return new SelectList(Sizes, "Value", "Text", item.SizeId);
		}

		public SelectList CloseReasonSelect(AllWorkItemsResult item)
		{
			return new SelectList(CloseReasons, "Value", "Text", item.CloseReasonId);
		}

		public SelectList MilestoneSelect(AllWorkItemsResult item)
		{
			return new SelectList(Milestones, "Value", "Text", item.MilestoneId);
		}

		public static async Task<CommonDropdowns> FillAsync(SqlConnection connection, int orgId, int responsibilities)
		{
			var result = new CommonDropdowns();
			result.Applications = await new AppSelect() { OrgId = orgId }.ExecuteAsync(connection);
			result.Projects = await new ProjectSelect() { OrgId = orgId }.ExecuteAsync(connection);
			result.AllActivities = await new ActivitySelect() { OrgId = orgId }.ExecuteAsync(connection);
			result.MyActivities = await new ActivitySelect() { OrgId = orgId, Responsibilities = responsibilities }.ExecuteAsync(connection);
			result.Sizes = await new SizeSelect() { OrgId = orgId }.ExecuteAsync(connection);
			result.CloseReasons = await new CloseReasonSelect().ExecuteAsync(connection);
			result.Milestones = await new MilestoneSelect() { OrgId = orgId }.ExecuteAsync(connection);
			return result;
		}
	}
}