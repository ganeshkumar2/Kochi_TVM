using CCNET;
using System;

namespace Kochi_TVM.BNR
{
    public class safeICCNET : Iccnet
    {

        public delegate void onCCNETError(object sender, CCNETErrorEvent e);
        public event onCCNETError OnError;

        private static safeICCNET instance = null;

        private safeICCNET()
        {
            //SearchAndConnect();
        }

        public static safeICCNET Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new safeICCNET();
                }
                return instance;
            }
        }

        public Answer DiagnosticCassette(int CassetteNumber)
        {

            try
            {
                return base.DiagnosticCassette(CassetteNumber);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

                Answer an = new Answer();
                an.Error = ex;

                return an;
            }
        }

        public override Answer Dispense(DispenseParameter[] dispenseParameter)
        {
            try
            {
                return base.Dispense(dispenseParameter);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

                Answer an = new Answer();
                an.Error = ex;

                return an;
            }
        }

        public override Answer Dispense(int BillType, int BillCount)
        {
            try
            {
                return base.Dispense(BillType, BillCount);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

                Answer an = new Answer();
                an.Error = ex;

                return an;
            }
        }

        public override Answer GetExtendedCassetteStatus(int CassetteNumber)
        {
            try
            {
                return base.GetExtendedCassetteStatus(CassetteNumber);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

                Answer an = new Answer();
                an.Error = ex;

                return an;
            }

        }

        public override Answer RunCommand(CCNETCommand Command)
        {
            try
            {
                return base.RunCommand(Command);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

                Answer an = new Answer();
                an.Error = ex;
                an.CcNetCommand = Command;

                return an;
            }

        }

        public override Answer RunCommand(CCNETCommand Command, bool IsThrowException)
        {
            return base.RunCommand(Command, IsThrowException);
        }

        public override Answer RunCommand(CCNETCommand Command, byte[] Data)
        {
            try
            {
                return base.RunCommand(Command, Data);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

                Answer an = new Answer();
                an.Error = ex;
                an.CcNetCommand = Command;
                an.SendedData = Data;
                return an;
            }
        }


        public override Answer RunCommand(CCNETCommand Command, CCNET_Sub_Command sub_command)
        {
            try
            {
                return base.RunCommand(Command, sub_command);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

                Answer an = new Answer();
                an.Error = ex;
                an.CcNetCommand = Command;

                an.CcNetSubCommand = sub_command;
                return an;
            }
        }

        public override Answer RunCommand(CCNETCommand Command, CCNET_Sub_Command sub_command, byte[] Data)
        {
            try
            {
                return base.RunCommand(Command, sub_command, Data);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

                Answer an = new Answer();
                an.Error = ex;
                an.CcNetCommand = Command;
                an.SendedData = Data;
                an.CcNetSubCommand = sub_command;
                return an;
            }
        }

        public override ComAnswer RunCommandCom(int Command)
        {
            try
            {
                return base.RunCommandCom(Command);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

                ComAnswer an = new ComAnswer();
                an.Error = ex;

                return an;
            }
        }

        public override ComAnswer RunCommandCom(int Command, byte[] Data)
        {
            try
            {
                return base.RunCommandCom(Command, Data);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

                ComAnswer an = new ComAnswer();
                an.Error = ex;

                an.SendedData = Data;

                return an;
            }
        }

        public override bool SearchAndConnect()
        {
            try
            {
                return base.SearchAndConnect(CCNET.Device.Bill_to_Bill);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }
                return false;
            }
        }


        public override void RunComandNonAnswer(CCNETCommand Command)
        {
            try
            {
                base.RunComandNonAnswer(Command);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

            }

        }

        public override void RunComandNonAnswer(CCNETCommand Command, byte[] Data)
        {
            try
            {
                base.RunComandNonAnswer(Command, Data);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

            }
        }

        public override void RunComandNonAnswer(CCNETCommand Command, CCNET_Sub_Command sub_command)
        {
            try
            {
                base.RunComandNonAnswer(Command, sub_command);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

            }
        }

        public override void RunComandNonAnswer(int Command)
        {
            try
            {
                base.RunComandNonAnswer(Command);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

            }
        }

        public override void RunComandNonAnswer(CCNETCommand Command, CCNET_Sub_Command sub_command, byte[] Data)
        {
            try
            {
                base.RunComandNonAnswer(Command, sub_command, Data);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

            }
        }

        public override void RunComandNonAnswer(int Command, byte[] Data)
        {
            try
            {
                base.RunComandNonAnswer(Command, Data);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

            }
        }

        public override void RunComandNonAnswer(int Command, int sub_command)
        {
            try
            {
                base.RunComandNonAnswer(Command, sub_command);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

            }
        }

        public override void RunComandNonAnswer(int Command, int sub_command, byte[] Data)
        {
            try
            {
                base.RunComandNonAnswer(Command, sub_command, Data);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(this, new CCNETErrorEvent(ex));
                }

            }
        }
    }

    public class CCNETErrorEvent : EventArgs
    {

        public CCNETErrorEvent(Exception exception)
        {
            CCNETException = exception;
        }

        public Exception CCNETException;

    }
}
