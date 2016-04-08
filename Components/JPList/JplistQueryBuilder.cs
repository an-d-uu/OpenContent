﻿using Satrabel.OpenContent.Components.Datasource.search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satrabel.OpenContent.Components.JPList
{
    public class JplistQueryBuilder
    {
        public static Select MergeJpListQuery(Select select, List<StatusDTO> statuses)
        {
            var query = select.Query;
            foreach (StatusDTO status in statuses)
            {
                switch (status.action)
                {
                    case "paging":
                        {
                            int number = 100000;
                            //  string value (it could be number or "all")
                            int.TryParse(status.data.number, out number);
                            select.PageSize = number;
                            select.PageIndex = status.data.currentPage;
                            break;
                        }
                    case "filter":
                        {
                            if (status.type == "textbox" && status.data != null && !string.IsNullOrEmpty(status.name) && !string.IsNullOrEmpty(status.data.value))
                            {
                                query.AddRule(new FilterRule()
                                {
                                    Field = status.name,
                                    FieldOperator = OperatorEnum.START_WITH,
                                    Value = new StringRuleValue(status.data.value),
                                });
                            }
                            else if ((status.type == "checkbox-group-filter" || status.type == "button-filter-group")
                                        && status.data != null && !string.IsNullOrEmpty(status.name))
                            {
                                if (status.data.filterType == "pathGroup" && status.data.pathGroup != null && status.data.pathGroup.Count > 0)
                                {
                                    query.AddRule(new FilterRule()
                                    {
                                        Field = status.name,
                                        FieldOperator = OperatorEnum.IN,
                                        MultiValue = status.data.pathGroup.Select(s => new StringRuleValue(s)),
                                    });
                                }
                            }
                            else if (status.type == "filter-select" && status.data != null && !string.IsNullOrEmpty(status.name))
                            {
                                if (status.data.filterType == "path" && status.data.path != null)
                                {
                                    query.AddRule(new FilterRule()
                                    {
                                        Field = status.name,
                                        Value = new StringRuleValue(status.data.path),
                                    });
                                }
                            }
                            break;
                        }

                    case "sort":
                        {
                            select.Sort.Clear();
                            select.Sort.Add(new SortRule()
                            {
                                Field = status.data.path,
                                Descending = status.data.order == "desc",
                                //FieldType = FieldTypeEnum.
                            });
                            break;
                        }
                }
            }
            return select;
        }
    }
}