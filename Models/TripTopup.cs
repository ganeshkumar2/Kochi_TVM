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
    
    public partial class TripTopup
    {
        public long recId { get; set; }
        public Nullable<long> transactionId { get; set; }
        public string mifareId { get; set; }
        public Nullable<int> subType { get; set; }
        public Nullable<int> tripCount { get; set; }
        public Nullable<int> stationId { get; set; }
        public Nullable<int> salePointId { get; set; }
        public Nullable<System.DateTime> trxDT { get; set; }
        public Nullable<int> insUserId { get; set; }
        public System.DateTime insDT { get; set; }
        public Nullable<int> editUserId { get; set; }
        public System.DateTime editDT { get; set; }
        public bool isDeleted { get; set; }
        public Nullable<System.DateTime> syncDT { get; set; }
    }
}
