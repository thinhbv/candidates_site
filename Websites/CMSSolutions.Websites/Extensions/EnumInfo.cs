using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMSSolutions.Websites.Extensions
{
	public enum Round
	{
		[Display(Name = "Round 1")]
		Round1 = 1,

		[Display(Name = "Round 2")]
		Round2 = 2
	}

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

	public enum PositionType
	{
		[Display(Name = "Develop")]
		Develop = 0,

		[Display(Name = "QA")]
		QA = 1,

		[Display(Name = "Tech Lead")]
		TechLead = 2,

		[Display(Name = "Team Lead")]
		TeamLead = 3,

		[Display(Name = "BrSE")]
		BrSE = 4,

		[Display(Name = "PM")]
		PM = 5,

		[Display(Name = "Other")]
		Other = 5,
	}
}