﻿using Ginseng.Models.Conventions;
using Ginseng.Models.Interfaces;
using Postulate.Base;
using Postulate.Base.Attributes;
using Postulate.Base.Interfaces;
using Postulate.SqlServer.IntKey;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Threading.Tasks;

namespace Ginseng.Models
{
	/// <summary>
	/// A software product managed by an organization (or team)
	/// </summary>
	public class Application : BaseTable, IOrgSpecific, IFindRelated<int>, ISelectable
	{
		[References(typeof(Organization))]
		[PrimaryKey]
		public int OrganizationId { get; set; }

		[PrimaryKey]
		[MaxLength(50)]
		public string Name { get; set; }

		[MaxLength(255)]
		public string Description { get; set; }

		/// <summary>
		/// App's live URL, as applicable
		/// </summary>
		[MaxLength(255)]
		public string Url { get; set; }

        /// <summary>
        /// Where do we email invoices?
        /// </summary>
        [MaxLength(100)]
        public string InvoiceEmail { get; set; }            

        /// <summary>
        /// Show this app on the New Items dashbord page
        /// </summary>
        [DefaultExpression("1")]
        public bool AllowNewItems { get; set; }

        [References(typeof(Team))]
        public int? TeamId { get; set; }

		public bool IsActive { get; set; } = true;
        
        public Organization Organization { get; set; }
        public Team Team { get; set; }

        [NotMapped]
        public bool Selected { get; set; }

        [NotMapped]
        public int LabelId { get; set; }

        public override bool Equals(object obj)
        {
            var test = obj as Application;
            return (test != null) ? test.Id == Id : false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void FindRelated(IDbConnection connection, CommandProvider<int> commandProvider)
        {
            Organization = commandProvider.Find<Organization>(connection, OrganizationId);            
        }

        public async Task FindRelatedAsync(IDbConnection connection, CommandProvider<int> commandProvider)
        {
            Organization = await commandProvider.FindAsync<Organization>(connection, OrganizationId);            
        }

        public async Task<int> GetOrgIdAsync(IDbConnection connection)
        {
            var org = await connection.FindAsync<Organization>(OrganizationId);
            return org.Id;
        }
    }
}