//using DevExpress.Mvvm.Native;



//using DevExpress.Pdf.Native.BouncyCastle.Utilities.Encoders;
//using DevExpress.DataAccess.Native.Sql.MasterDetail;

namespace iHeatOvenControl
{
    public static class TtcB2UpS_Cmd
    {
        //Head
        public const char Head = '@'; // 檔頭"@" 1 byte

        // 控制器編號
        public const string ControllerNumber = "01"; //控制器編號 2 byte

        //End Cod
        public const string End = "*\r"; // 結束 2 byte, ＣＲ（ 0DH ）
        //ACK
        //public const string Ack = 'ACK'; //應答正常之表示。
        //NAC
        //public const string Nac = "15"; //應答異常之表示。

        #region get data from controller
        public const string SignalAnalogData = "01"; //(30H,31H)：類比資料送信"要求"之格式
        public const string SignalDigitalData = "51"; //(35H,31H)：數位資料"要求"之格式
        public const string SignalRestStep = "80"; //(38H,30H)：剩餘段數 STEP ）返答"要求"之格式


        public const string SignalStepSetting = "20"; //(32H,30H)段數（STEP ）設定 讀出
        public const string SignalRepeatSetting = "21"; //循環（REPEAT ）設定 讀出
        public const string SignalOperationSetting = "22"; //(32H,32H)：運轉設定資料"要求"之格式
        public const string SignalTimeSetting = "24"; //時間信號（TIME SIGNAL ）設定 讀出
        public const string SignalConstValueSetting = "25"; //(32H,35H)：定値設定資料"要求"之格式
        public const string SignalPidZoneSetting = "28"; //ＰＩＤ區域（ZONE ）設定 讀出
        public const string SignalPidParameterSetting = "29"; //ＰＩＤ參數設定 讀出
        public const string SignalOnOffSystemSetting = "2B"; //ＯＮ／ＯＦＦＳＹＳＴＥＭ 讀出
        //get operation status
        public const int GetOperationStop=0;
        public const int GetOperationRun = 1;
        public const int GetOperationReady = 3;
        public const int GetOperationWait = 5;
        public const int GetOperationHold = 17;
        public const int GetOperationEnd = 32;
        #endregion


        #region set data to controller
        public const string SignalSetConstValue= "15"; //(31H,35H)：定値設定
        public const string SignalSetOperation = "53"; //５３’(35H,33H) 操作設定
        public const string  SignalSetStep = "10"; //段數（STEP ）設定

        #endregion
        //操作設定; go with " SignalSetOperation = "53""
        public const string OperationRun = "01";//RUN／運轉
        public const string OperationStop= "02";//STOP／停止
        public const string OperationHold = "03";//HOLD／保持
        public const string OperationAdvence = "04";//ADVENCE／跳段

        public const char OperationDo= '1';//執行（停止中以外無效）


    }
    public class TtcB2UpS_Protocol
    {

        #region FCS Computation

        public string Fcs(string data)
        {
            //‘＠０１０１’
            //40H,30H,31H,30H,31H
            //計算總和 40 XOR 30 XOR 31 XOR 30 XOR 31 =40
            byte sum = 0;
            for (var i = 0; i < data.Length; i++)
            {
                //16進制字串轉成16進制數值後加總
                sum ^= (byte)data[i];
            }

            //將得到的校驗碼轉成16進制字串
            return sum.ToString("X2");

        }

        #endregion

        #region check Response

        public bool CheckResponse(string response)
        {
            //remove FCS
            var subMsg = response.Substring(0, response.Length - 2);
            //LRC Computation
            var fcs = Fcs(subMsg);
            //Get LRC From Response
            var lrcFromResponse = response.Substring(response.Length - 2, 2);
            //compare two LRC
            return fcs.Equals(lrcFromResponse);
        }

        #endregion

    }


}
