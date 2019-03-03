﻿using Postulate.Base;
using Postulate.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Ginseng.Mvc.Queries
{
	public class WorkItemsResult
	{
		public int Id { get; set; }
		public int Number { get; set; }
		public string Title { get; set; }
		public int? OwnerUserId { get; set; }
		public int? BusinessPriority { get; set; }
		public int ApplicationId { get; set; }
		public string ApplicationName { get; set; }
		public int? ProjectId { get; set; }
		public string ProjectName { get; set; }
		public int? MilestoneId { get; set; }
		public string MilestoneName { get; set; }
		public DateTime? MilestoneDate { get; set; }
		public int? MilestoneDaysAway { get; set; }
		public int? CloseReasonId { get; set; }
		public string CloseReasonName { get; set; }
		public string OwnerName { get; set; }
		public string DeveloperName { get; set; }
		public string WorkItemSize { get; set; }
		public int? DeveloperUserId { get; set; }
		public int? DevelopmentPriority { get; set; }
		public int? EstimateDays { get; set; }
	}

	public class WorkItems : Query<WorkItemsResult>, ITestableQuery
	{
		public WorkItems() : base(
			@"SELECT 
				[wi].[Id],
				[wi].[Number],
				[wi].[Title],
				[wi].[OwnerUserId],
				[wi].[Priority] AS [BusinessPriority],
				[wi].[ApplicationId], [app].[Name] AS [ApplicationName],
				[wi].[ProjectId], [p].[Name] AS [ProjectName],
				[wi].[MilestoneId], [ms].[Name] AS [MilestoneName], [ms].[Date] AS [MilestoneDate], DATEDIFF(d, getdate(), [ms].[Date]) AS [MilestoneDaysAway],
				[wi].[CloseReasonId], [cr].[Name] AS [CloseReasonName],           
				COALESCE([owner_ou].[DisplayName], [ousr].[UserName]) AS [OwnerName],
				COALESCE([dev_ou].[DisplayName], [dusr].[UserName]) AS [DeveloperName],
				[sz].[Name] AS [WorkItemSize],
				[wid].[UserId] AS [DeveloperUserId],    
				[wid].[Priority] AS [DevelopmentPriority],
				COALESCE([wid].[EstimateHours], [sz].[EstimateHours]) AS [EstimateHours]
			FROM 
				[dbo].[WorkItem] [wi]
				INNER JOIN [dbo].[Application] [app] ON [wi].[ApplicationId]=[app].[Id]
				LEFT JOIN [dbo].[Project] [p] ON [wi].[ProjectId]=[p].[Id]
				LEFT JOIN [dbo].[Activity] [act] ON [wi].[ActivityId]=[act].[Id]
				LEFT JOIN [dbo].[Milestone] [ms] ON [wi].[MilestoneId]=[ms].[Id]
				LEFT JOIN [app].[CloseReason] [cr] ON [wi].[CloseReasonId]=[cr].[Id]
				LEFT JOIN [dbo].[WorkItemDevelopment] [wid] ON [wi].[Id]=[wid].[WorkItemId]
				LEFT JOIN [dbo].[OrganizationUser] [owner_ou] ON 
					[wi].[OrganizationId]=[owner_ou].[OrganizationId] AND
					[wi].[OwnerUserId]=[owner_ou].[UserId]
				LEFT JOIN [dbo].[AspNetUsers] [ousr] ON [wi].[OwnerUserId]=[ousr].[UserId]
				LEFT JOIN [dbo].[OrganizationUser] [dev_ou] ON
					[wi].[OrganizationId]=[dev_ou].[OrganizationId] AND
					[wid].[UserId]=[dev_ou].[UserId]
				LEFT JOIN [dbo].[AspNetUsers] [dusr] ON [wid].[UserId]=[dusr].[UserId]
				LEFT JOIN [dbo].[WorkItemSize] [sz] ON [wid].[SizeId]=[sz].[Id]
			WHERE
				[wi].[OrganizationId]=@orgId
			ORDER BY
				COALESCE([wid].[Priority], [wi].[Priority]), [wi].[Number]")
		{
		}

		public int OrgId { get; set; }

		public static IEnumerable<ITestableQuery> GetTestCases()
		{
			yield return new WorkItems() { OrgId = 0 };
		}

		public IEnumerable<dynamic> TestExecute(IDbConnection connection)
		{
			return TestExecuteHelper(connection);
		}
	}
}
