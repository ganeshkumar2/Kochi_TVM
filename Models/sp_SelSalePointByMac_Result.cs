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
    
    public partial class sp_SelSalePointByMac_Result
    {
        public int recId { get; set; }
        public string salePointCode { get; set; }
        public Nullable<short> salePointTypeId { get; set; }
        public string macAddress { get; set; }
        public Nullable<byte> isActive { get; set; }
        public Nullable<int> insertUserId { get; set; }
        public System.DateTime insertDT { get; set; }
        public Nullable<int> editUserId { get; set; }
        public System.DateTime editDT { get; set; }
        public Nullable<int> stationId { get; set; }
        public bool isDeleted { get; set; }
        public string descCode { get; set; }
        public string ipAddress { get; set; }
        public Nullable<int> serialNumber { get; set; }
    }
}
