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
    
    public partial class sp_SelStations_Result
    {
        public int recId { get; set; }
        public string descCode { get; set; }
        public string stationCode { get; set; }
        public string explanation { get; set; }
        public int stationTypeId { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string altitude { get; set; }
        public Nullable<int> mapRow { get; set; }
        public Nullable<int> mapColumn { get; set; }
        public Nullable<int> mapHereRow { get; set; }
        public Nullable<int> mapHereColumn { get; set; }
    }
}
