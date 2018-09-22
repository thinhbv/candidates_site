using System;
using System.ComponentModel.DataAnnotations;

namespace CMSSolutions.Data.QueryBuilder
{
    [Serializable]
    public enum ComparisonOperator
    {
        [Display(Name = "Equal to")]
        EqualTo = 0,

        [Display(Name = "Ends with")]
        EndsWith,

        [Display(Name = "Greater than")]
        GreaterThan,

        [Display(Name = "Greater than or equal to")]
        GreaterThanOrEqualTo,

        In,

        [Display(Name = "Is empty")]
        IsEmpty,

        [Display(Name = "Is not empty")]
        IsNotEmpty,

        [Display(Name = "Is not null")]
        IsNotNull,

        [Display(Name = "Is not null or empty")]
        IsNotNullOrEmpty,

        [Display(Name = "Is null")]
        IsNull,

        [Display(Name = "Is null or empty")]
        IsNullOrEmpty,

        [Display(Name = "Less than")]
        LessThan,

        [Display(Name = "Less than or equal to")]
        LessThanOrEqualTo,

        [Display(Name = "Contains")]
        Like,

        [Display(Name = "Not ending with")]
        NotEndsWith,

        [Display(Name = "Not equal to")]
        NotEqualTo,

        [Display(Name = "Not contains")]
        NotLike,

        [Display(Name = "Not starting with")]
        NotStartsWith,

        [Display(Name = "Starts with")]
        StartsWith
    }
}