//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kochi_TVM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FareTable
    {
        public int recId { get; set; }
        public string fromValue { get; set; }
        public string toValue { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<byte> isActive { get; set; }
        public System.DateTime validityDateBegin { get; set; }
        public System.DateTime validityDateEnd { get; set; }
        public Nullable<int> insertUserId { get; set; }
        public System.DateTime insertDT { get; set; }
        public Nullable<int> editUserId { get; set; }
        public System.DateTime editDT { get; set; }
        public bool isDeleted { get; set; }
        public Nullable<int> version { get; set; }
        public Nullable<int> routeId { get; set; }
        public Nullable<int> fromId { get; set; }
        public Nullable<int> toId { get; set; }
    }
}
