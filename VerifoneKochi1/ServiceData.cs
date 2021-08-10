using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.VerifoneKochi1
{
    [Serializable]
    public class ServiceData
    {
        public ServiceData()
        {

        }

        public string PassCounter { get; set; } = string.Empty;

        public string PassLimit { get; set; } = string.Empty;

        public string PassExpiryDate { get; set; } = string.Empty;

        public string PassEffectiveDate { get; set; } = string.Empty;

        public string PassType { get; set; } = string.Empty;

        public string LastTerminalId { get; set; } = string.Empty;

        public string LastStan { get; set; } = string.Empty;

        public string LastBatch { get; set; } = string.Empty;

        public string LastCity { get; set; } = string.Empty;

        public string LastTapInDt { get; set; } = string.Empty;

        public string LastFareAmount { get; set; } = string.Empty;

        public string LastVehicleType { get; set; } = string.Empty;

        public string LastFareType { get; set; } = string.Empty;

        public string LastTransactionType { get; set; } = string.Empty;

        public string FromLineCode { get; set; } = string.Empty;

        public string FromStageCode { get; set; } = string.Empty;

        public string FromStationId { get; set; } = string.Empty;

        public string FromStationShortCode { get; set; } = string.Empty;

        public string ToLineCode { get; set; } = string.Empty;

        public string ToStageCode { get; set; } = string.Empty;

        public string ToStationId { get; set; } = string.Empty;

        public string ToStationShortCode { get; set; } = string.Empty;

        public string LastRouteNo { get; set; } = string.Empty;

        public string PassValidityVol { get; set; } = string.Empty;

        public string Rfu { get; set; } = string.Empty;

        public string PanToken { get; set; } = string.Empty;

        public string Balance { get; set; } = string.Empty;

        //Yeni kart haritasına göre gelen alanlardır
        public string PassCounterMsb { get; set; } = string.Empty;

        public string PassLimitMsb { get; set; } = string.Empty;

        public string PassProductCode { get; set; } = string.Empty;

        public string LoadStationId { get; set; } = string.Empty;

        public string LoadSalePointId { get; set; } = string.Empty;

        public ResponseResult RespResult { get; set; } = new ResponseResult();
    }
    public class ResponseResult
    {
        private string ReturnDescriptionField = string.Empty;
        private string ErrorDetailsField = string.Empty;
        private int ReturnCodeField = 0;
        private string ResultField = string.Empty;

        public string ReturnDescription
        {
            get
            {
                return ReturnDescriptionField;
            }
            set
            {
                ReturnDescriptionField = value;
            }
        }
        public string ErrorDetails
        {
            get
            {
                return ErrorDetailsField;
            }
            set
            {
                ErrorDetailsField = value;
            }
        }
        public int ReturnCode
        {
            get
            {
                return ReturnCodeField;
            }
            set
            {
                ReturnCodeField = value;
            }
        }
        public string Result
        {
            get
            {
                return ResultField;
            }
            set
            {
                ResultField = value;
            }
        }
    }

    public class ResponseTopUp
    {
        public string PanTokenNo { get; set; } = string.Empty;

        public string Rrn { get; set; } = string.Empty;

        public string Amt { get; set; } = string.Empty;

        public ResponseResult RespResult { get; set; } = new ResponseResult();
    }
    public class ResponseUpdateService
    {
        public string PanTokenNo { get; set; } = string.Empty;

        public ResponseResult RespResult { get; set; } = new ResponseResult();
    }
    public class ServiceDataMultiple
    {
        public ResponseResult RespResult { get; set; } = null;
        public ServiceData[] ServiceDatas { get; set; } = null;

        public ServiceDataMultiple()
        {
            RespResult = new ResponseResult();
            ServiceDatas = new ServiceData[2];
        }
    }
}
