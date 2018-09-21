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
		[Display(Name = "Actived")]
		Actived=0,

		[Display(Name = "Blocked")]
		Blocked=1
	}
}