using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSSolutions.Websites.Models
{
	public class DataViewModel
	{
		public int TotalRow { get; set; }

		public int TotalPage
		{
			get
			{
				if (TotalRow <= PageSize)
				{
					return 1;
				}

				var count = TotalRow % PageSize;
				if ((count == 0))
				{
					return TotalRow / PageSize;
				}

				return ((TotalRow - count) / PageSize) + 1;
			}
		}

		public int PageSize { get; set; }

		public int PageIndex { get; set; }

		public bool Status { get; set; }

		public string Html { get; set; }

		public string Data { get; set; }
	}
}