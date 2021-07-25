using Kochi_TVM.Business;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.RptDispenser
{
    public class Dispenser
    {
        private static ILog log = LogManager.GetLogger(typeof(Dispenser).Name);

        #region Definations
        private string id = "Dispenser";
        private string name = "Dispenser";
        private string description = "CRT571 RPT Dispenser";
        private string comPort;


        RPTIssuer rpt = new RPTIssuer(TVMUtility.GetSamUnlockKey(Constants.SamCardType));
        private Dispenser_ErrorCodes errCode = Dispenser_ErrorCodes.Success;
        private string errDesc = String.Empty;

        private static Dispenser _instance = null;
        public static Dispenser Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Dispenser();
                return _instance;
            }
        }

        #endregion
        public Dispenser()
        {
            Init();
        }
        private bool Init()
        {
            bool result = false;

            try
            {
                rpt.crt571Port = Parameters.TVMConst.RPTDispenserPort;//ConfigurationManager.AppSettings["Dispenser"];

                var err = LIB_ERR.NO_ERROR;
                if (rpt.Init(DEVICE_TYPE.CRT571_DISPENSER, ref err))
                    if (err == LIB_ERR.NO_ERROR)
                        result = true;

                log.Debug("Debug MainPage -> rptDispenserStatus : " + err.ToString());

            }
            catch (Exception ex)
            {
            }

            return result;
        }

        public void Done()
        {
            rpt.Done();
        }

        public bool GetSAMSerialNo(ref byte[] samId, ref LIB_ERR errSam)
        {
            bool result = false;
            try
            {
                result = rpt.GetSAMSerialNo(ref samId, ref errSam);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public bool GetStackerStatus(ref DISP_STAT status)
        {
            bool result = false;
            try
            {
                result = rpt.GetStackerStatus(ref status);
            }
            catch (Exception ex)
            {

            }

            return result;
        }        
        public bool IsCardInRFCardOperationPosition()
        {
            bool result = false;
            try
            {
                result = rpt.IsCardInRFCardOperationPosition();
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public bool Move()
        {
            return rpt.MoveCardToRFCardPosition();
        }
        public bool Personalize(out byte[] uid, RPTPersoSector persoSector)
        {

            bool ret = false;

            byte[] cardUid = null;
            byte cardUidLen = 0;
            LIB_ERR err = LIB_ERR.NO_ERROR;

            ret = rpt.Do(persoSector, ref cardUid, ref cardUidLen, ref err);
            uid = new byte[cardUidLen];
            Array.Copy(cardUid, uid, uid.Length);

            return ret;
        }
        public bool Reject()
        {
            rpt.MoveCardtoTheGate();
            return true;
        }
        public bool Trash()
        {
            rpt.CaptureCardToErrorCardBin();
            return true;
        }

    }
}
