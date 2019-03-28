﻿using Ginseng.Models;
using Postulate.Base;

namespace Ginseng.Mvc.Queries
{
	public class Comments : Query<Comment>
	{
		public Comments() : base(
			@"SELECT
				[c].*,
				COALESCE([ou].[DisplayName], [u].[UserName]) AS [DisplayName]
			FROM
				[dbo].[Comment] [c]
				INNER JOIN [dbo].[WorkItem] [wi] ON [c].[WorkItemId]=[wi].[Id]
				INNER JOIN [dbo].[AspNetUsers] [u] ON [c].[CreatedBy]=[u].[UserName]
				LEFT JOIN [dbo].[OrganizationUser] [ou] ON
					[wi].[OrganizationId]=[ou].[OrganizationId] AND
					[ou].[UserId]=[u].[UserId]
			WHERE
				[wi].[OrganizationId]=@orgId AND
				[c].[WorkItemId] IN @workItemIds
			ORDER BY
				[c].[DateCreated] DESC")
		{
		}

		public int OrgId { get; set; }
		public int[] WorkItemIds { get; set; }
	}
}