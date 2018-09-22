using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMSSolutions.Websites.Extensions
{
	public enum LevelType
	{
		[Display(Name = "Skill")]
		None=0,

		[Display(Name = "Main Skill")]
		Main = 1
	}

	public enum CandidateStatus
	{
		[Display(Name = "New")]
		New = 0,

		[Display(Name = "Blocked")]
		Blocked=1,

		[Display(Name = "Interviewing")]
		Interview = 2,

		[Display(Name = "Pass")]
		Pass = 3,

		[Display(Name = "Failed")]
		Failed = 4,
	}
}