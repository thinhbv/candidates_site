using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlGridFormRequest
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string SortColumn { get; set; }

        /// <summary>
        /// True: Sort from smallest to largest. For example, from A to Z.
        /// False: Sort from largest to smallest. For example, from Z to A.
        /// </summary>
        public bool SortDirection { get; set; }

        public string NodeId { get; set; }

        public int NodeLevel { get; set; }

        public IList<ControlGridFilter> Filters { get; set; }

        public static ControlGridFormRequest Create(HttpRequestBase request)
        {
            var result = new ControlGridFormRequest();

            var rows = request.Form["rows"];
            if (rows != null)
            {
                result.PageSize = Convert.ToInt32(rows);
            }

            var page = request.Form["page"];
            if (page != null)
            {
                result.PageIndex = Convert.ToInt32(page);
            }

            if (result.PageIndex < 1)
            {
                result.PageIndex = 1;
            }

            result.NodeId = request.Form["nodeid"];
            if (request.Form.AllKeys.Contains("n_level"))
            {
                var level = request.Form["n_level"];
                if (string.IsNullOrEmpty(level))
                {
                    result.NodeLevel = -1;
                }
                else
                {
                    result.NodeLevel = Convert.ToInt32(request.Form["n_level"]);
                }
            }
            else
            {
                result.NodeLevel = -1;
            }

            result.SortColumn = request.Form["sidx"];
            result.SortDirection = request.Form["sord"] == "asc";

            var isSearch = request.Form["_search"];
            if (isSearch == "true")
            {
                result.Filters = new List<ControlGridFilter>();

                var filters = JObject.Parse(request.Form["filters"]);
                var rules = (JArray)filters["rules"];
                foreach (var rule in rules)
                {
                    ControlGridFilterOperator @operator;
                    switch (rule["op"].Value<string>())
                    {
                        case "eq":
                            @operator = ControlGridFilterOperator.Equal;
                            break;

                        case "ne":
                            @operator = ControlGridFilterOperator.NotEqual;
                            break;

                        case "lt":
                            @operator = ControlGridFilterOperator.Less;
                            break;

                        case "le":
                            @operator = ControlGridFilterOperator.LessOrEqual;
                            break;

                        case "gt":
                            @operator = ControlGridFilterOperator.Greater;
                            break;

                        case "ge":
                            @operator = ControlGridFilterOperator.GreaterOrEqual;
                            break;

                        case "bw":
                            @operator = ControlGridFilterOperator.BeginsWith;
                            break;

                        case "bn":
                            @operator = ControlGridFilterOperator.NotBeginsWith;
                            break;

                        case "in":
                            @operator = ControlGridFilterOperator.IsIn;
                            break;

                        case "ni":
                            @operator = ControlGridFilterOperator.NotIsIn;
                            break;

                        case "ew":
                            @operator = ControlGridFilterOperator.EndsWith;
                            break;

                        case "en":
                            @operator = ControlGridFilterOperator.NotEndsWith;
                            break;

                        case "cn":
                            @operator = ControlGridFilterOperator.Contains;
                            break;

                        case "nc":
                            @operator = ControlGridFilterOperator.NotContains;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    result.Filters.Add(new ControlGridFilter
                                           {
                                               Name = rule["field"].Value<string>(),
                                               Operator = @operator,
                                               Value = rule["data"].Value<string>()
                                           });
                }
            }

            return result;
        }
    }

    public class ControlGridFilter
    {
        public string Name { get; set; }

        public ControlGridFilterOperator Operator { get; set; }

        public string Value { get; set; }
    }

    public enum ControlGridFilterOperator
    {
        Equal,
        NotEqual,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        BeginsWith,
        NotBeginsWith,
        IsIn,
        NotIsIn,
        EndsWith,
        NotEndsWith,
        Contains,
        NotContains,
    }
}