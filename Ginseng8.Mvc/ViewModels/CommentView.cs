﻿using Ginseng.Models;
using System.Collections.Generic;

namespace Ginseng.Mvc.ViewModels
{
	public class CommentView
	{
		public int Id { get; set; }
		public IEnumerable<Comment> Comments { get; set; }
	}
}