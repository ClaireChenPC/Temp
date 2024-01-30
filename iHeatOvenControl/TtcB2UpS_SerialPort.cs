//using DevExpress.Charts.Native;
//using DevExpress.DataAccess.Native.Web;
//using DevExpress.PivotGrid.OLAP.Mdx;
//using iMotionControl;

using System.IO.Ports;
using System.Text;

//using static DevExpress.Utils.MVVM.Internal.ILReader;

namespace iHeatOvenControl
{
    public class TtcB2UpS_SerialPort
    {
        public SerialPort Rs232Port = new();

        public string Status = "Offline";
        public string Tx = "";
        public string Rx = "";
        public bool Connect = false;
        public bool DoThread = false;
        private TtcB2UpS_Protocol _protocol = new();

        #region check if port is open or close

        public bool IsPortOpen()
        {
            return Rs232Port.IsOpen;
        }

        #endregion

        #region Open / Close Procedures

        public bool Open(string portName)
        {
            if (Rs232Port.IsOpen)
            {
                Status = portName + " already opened";
                return false;
            }
            try
            {
                Rs232Port.PortName = portName;
                Rs232Port.BaudRate = 9600;
                Rs232Port.DataBits = 8;
                Rs232Port.Parity = Parity.Even;
                Rs232Port.StopBits = StopBits.One;
                Rs232Port.ReadTimeout = 1000;
                Rs232Port.WriteTimeout = 1000;
                Rs232Port.Open();
            }
            catch (Exception e)
            {
                Status = "Error opening " + portName + ": " + e.Message;
                return false;
            }

            Status = portName + " opened successfully";
            return true;
        }

        public bool Close()
        {
            //Ensure port is opened before attempting to close:
            if (Rs232Port.IsOpen)
            {
                try
                {
                    Rs232Port.Close();
                }
                catch (Exception err)
                {
                    Status = "Error closing " + Rs232Port.PortName + ": " + err.Message;
                    return false;
                }

                Status = Rs232Port.PortName + " closed successfully";
                return true;
            }

            Status = Rs232Port.PortName + " is not open";
            return false;
        }

        #endregion

        #region Write One Register

        public bool TransmitData(string singleNumber, string parmaterA, string parmaterB, ref string response)
        {
#if REMOVEBUS

#else
            if (!Rs232Port.IsOpen)
            {
                Status = "COM Port don't open";
                return false;
            }

            try
            {
                //Clear in/out buffers:
                Rs232Port.DiscardOutBuffer();
                Rs232Port.DiscardInBuffer();
#endif
                Tx = "";
                //Building cmd (for FCS)
                string cmd = TtcB2UpS_Cmd.Head+ TtcB2UpS_Cmd.ControllerNumber+ singleNumber+ parmaterA+ parmaterB;
                //FCS Commutation
                string fcs = _protocol.Fcs(cmd);
                //Console.WriteLine(fcs);
                //Building a complete cmd that can send out
                cmd =  cmd + fcs + TtcB2UpS_Cmd.End;
                Tx = cmd;
#if REMOVEBUS
            return true;
#else
            var buf = Encoding.ASCII.GetBytes(cmd);
                Rs232Port.Write(buf, 0, buf.Length);
                //WriteLine("Transmit = " + cmd);
                //get response
                response = Rs232Port.ReadLine();
                //WriteLine("Response = " + response);

                return true;
            }
            catch (Exception ex)
            {
                Status = "Error in write event:" + ex.Message;
                return false;
            }
#endif
        }

#endregion


        #region Read Registers

        public bool ReadRegister(string singleNumber, string parmaterA, string parmaterB, ref string subMsg)
        {
            var response = "";
            Rx = "";
            if (!TransmitData(singleNumber, parmaterA, parmaterB, ref response)) return false;
            if (response == "") return false;
            Rx = response;
            //Check FCS
            //remove * and CR to check FCS
            subMsg = response.Substring(0, response.Length - 2);
            if (!_protocol.CheckResponse(subMsg))
            {
                Status = "CRC error";
                return false;
            }

            Status = "Read successful";
            return true;
        }

        #endregion

        #region get analog data 類比資料
        public bool GetAnalogData(ref string response)//‘＠０１０１’
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalAnalogData, "","",ref response);
        }
        #endregion

        #region get digital data 數位資料
        public bool GetDigitalData(ref string response)//‘＠0１5１’
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalDigitalData, "", "", ref response);
        }
        #endregion

        #region get rest step data 段數（STEP ）資料
        public bool GetRestStepData(ref string response)
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalRestStep, "", "", ref response);
        }
        #endregion


        #region get operation data 運轉設定資料
        public bool GetOperationData(ref string response)
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalOperationSetting, "", "", ref response);
        }
        #endregion

        #region get PID data 定値設定資料
        public bool GetPidData(ref string response)
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalPidZoneSetting, "", "", ref response);
        }
        #endregion

        #region get const data 定値設定資料
        public bool GetConstData(ref string response)
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalConstValueSetting, "", "", ref response);
        }
        #endregion

        #region 段數（STEP ）設定 讀出
        public bool GetStepSettingData(ref string response)
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalStepSetting, "", "", ref response);
        }
        #endregion

        #region 時間信號（ TIME SIGNAL 設定 讀出
        public bool GetTimeSignalSettingData(ref string response)
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalTimeSetting, "", "", ref response);
        }
        #endregion

        #region 循環（REPEAT ）設定 讀出
        public bool GetRepeatSettingData(ref string response)
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalRepeatSetting, "", "", ref response);
        }
        #endregion


        #region 時間信號（TIME SIGNAL ）設定 讀出
        public bool GetTimeSettingData(ref string response)
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalTimeSetting, "", "", ref response);
        }
        #endregion
        public const string SignalOnOffSystemSetting = "2B"; //ＯＮ／ＯＦＦＳＹＳＴＥＭ 讀出
        #region ＰＩＤ參數設定 讀出
        public bool GetPidParameterSettingData(ref string response)
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalPidParameterSetting, "", "", ref response);
        }
        #endregion


        #region ＯＮ／ＯＦＦＳＹＳＴＥＭ 讀出
        public bool GetOnOffSystemSettingData(ref string response)
        {
            return ReadRegister(TtcB2UpS_Cmd.SignalOnOffSystemSetting, "", "", ref response);
        }
        #endregion

        #region set step
        public bool SetStep(string pattern, string step, ref string response)
        {
            return TransmitData(TtcB2UpS_Cmd.SignalSetStep, pattern, step, ref response);
        }
        #endregion

        #region 定値設定

        public bool SetConstValue(string temp, string slope, ref string response)
        {
            string value = temp + "0000" + slope + "0000" + "0001" + "00" + '0' + '0';

            if (!TransmitData(TtcB2UpS_Cmd.SignalSetConstValue, value, "", ref response))
                return false;
            return true;
        }
        #endregion
        #region 運轉設定for initation

        public bool SetRunModeInit(ref string response)
        {
            //程式組編號(2B)+ (0:定値運轉、即時 啟 動)(1B)
            string value = "0A000000101210096015E";
            if (!TransmitData("12", value, "", ref response))
                return false;
            return true;

        }

        #endregion

        #region set operation mode
        //mode="01";//RUN／運轉
        //mode="02";//STOP／停止
        public bool SetOperationMode(string mode, ref string response)
        {

            if (!TransmitData(TtcB2UpS_Cmd.SignalSetOperation, mode + TtcB2UpS_Cmd.OperationDo, "", ref response))
                return false;
            //訊息返回（ ANSWER BACK) 操作設定完了通知之格式
            //1. Check FCS
            //remove * and CR to check FCS
            string subMsg = response.Substring(0, response.Length - 2);
            if (!_protocol.CheckResponse(subMsg))
            {
                Status = "CRC error";
                return false;
            }

            String A = response.Substring(7, 1);
            //Console.WriteLine(A.Length.ToString());
            //Convert.ToDecimal(A);
            //Console.WriteLine(Convert.ToInt16(A));
            //2. check 信號 and 控制編號 and "完了"(need to check)
            if ((response.Substring(3, 2) == "53") && (response.Substring(5, 2) == mode) /*&& (TtcB2UpS_Cmd.Ack)*/)
            {
                Status = "操作設定成功";
                return true;
            }
            Status = "操作設定失敗";
            return false;

        }

        #endregion

    }
}
