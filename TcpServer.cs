using System.ComponentModel;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class TcpServer
    {
        private TcpListener _server;
        //ThreadSafeFileWriter  myWriter = new ThreadSafeFileWriter();
        MultiThreadFileWriter myWriter = new MultiThreadFileWriter();

        public TcpServer(int port)
        {
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();


            LoopClients();
        }

        public static Byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

        private async void LoopClients()
        {
            while (true)
            {
                try
                {
                // wait for client connection
                TcpClient newClient = await _server.AcceptTcpClientAsync();

                string _clientIP = IPAddress.Parse(((IPEndPoint)newClient.Client.RemoteEndPoint).Address.ToString()).ToString();

                Console.WriteLine(newClient.Client.Handle.ToString() +  " > " + _clientIP + " Connected !");
                myWriter.WriteLine(newClient.Client.Handle.ToString() +  " > " + _clientIP + " Connected !");

                // client found.. create a thread to handle communication
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("ERROR at LoopClients > " + ex.Message);
                    myWriter.WriteLine("ERROR at LoopClients > " + ex.Message);
                }
            }
        }

        public decimal DecodeCoordinate(byte[] bytes)
        {
            decimal minutes = (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
            minutes /= 30000;
            minutes /= 60;
            return minutes;
        }

        private static object myLocker = new object();

        ConcurrentDictionary<string, List<string>> myList = new ConcurrentDictionary<string, List<string>>();

        private async void HandleClient(object obj)
        {
            String _clientIP = "";
            string _myIMEI = "";
            try
            {
            // retrieve client from parameter passed to thread
            TcpClient client = (TcpClient)obj;

            //Boolean bClientConnected = true;
            String sData = null;

            _clientIP = IPAddress.Parse(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()).ToString();
            String strVoltageLevel = "XX";

            while (true)
            {
                //Yontem3 : Read Byte arrays..
                Byte[] data = new Byte[256];
                String strSerilNo = "";
                String strErrorCheck = "";
                String strDataLen = "";
                String strProtokolNoByte = "";
                String strTerminalIDBytes = "";
                String strDateBytes = "";
                String strQuantityOfSattelites = "";
                String strLatBytes = "";
                String strLongBytes = "";
                String strSpeedByte = "";
                String strACCStatus = "";
                String KontakStatu = "X";

                NetworkStream stream = client.GetStream();
                Int32 _numberOfBytesInTCP = stream.Read(data, 0, data.Length);
                Byte[] dataReal = new Byte[_numberOfBytesInTCP];
                Array.Copy(data,0,dataReal,0,_numberOfBytesInTCP);

                sData = BitConverter.ToString(dataReal).Replace("-","");

                if(_numberOfBytesInTCP <= 0)
                {
                    Console.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " Is Disconnected !");
                    myWriter.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " Is Disconnected ! ");
                    client.Close();
                    break;
                } else {
                    //OK.. Connection sağlandı.. MericY: Loglama icin tut..
                    //Console.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " > "+ sData);
                    //myWriter.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " > "+ sData);
                }


                if(sData.Contains("7878") && sData.Contains("0D0A"))  {

                strDataLen = sData.Substring(4,2);
                strProtokolNoByte = sData.Substring(6,2);

                    //LOGIN Msg
                    if(strProtokolNoByte == "01") {

                        //Console.WriteLine(_clientIP +  " > LOGIN DATA >> " +  sData);

                        strTerminalIDBytes = sData.Substring(8,16);
                        strSerilNo =sData.Substring(24,4);
                        strErrorCheck = sData.Substring(28,4);

                        myWriter.WriteLine(_clientIP +  " > IMEI >> " +  strTerminalIDBytes);

                        _myIMEI = strTerminalIDBytes;

                        string sendData = "78780501" + strSerilNo + strErrorCheck + "0D0A";
                        //Console.WriteLine(_clientIP +  " > SEND DATA >> " +  sendData);

                        //stream.Flush();
                        //NetworkStream tempStream = client.GetStream();
                        /*
                        Byte[] sendDataBytes = ConvertHexStringToByteArray(sendData);
                        stream.Write(sendDataBytes, 0, sendDataBytes.Length);
                        */
                    }


                     //INFO Msg :
                    if(strProtokolNoByte == "13") {
                        strVoltageLevel  = sData.Substring(12,2);
                        /*
                        myWriter.WriteLine(":::: INFO :::: ");
                        myWriter.WriteLine(sData);
                        myWriter.WriteLine(":: Voltage >> " + strVoltageLevel);
                        */
                    }

                    //GPS Msg
                    if(strProtokolNoByte == "22") {

                        myWriter.WriteLine(":::: GPS  :::: ");
                        myWriter.WriteLine(sData);

                        strDateBytes = sData.Substring(8,12);
                        strQuantityOfSattelites =sData.Substring(20,2);
                        strLatBytes =sData.Substring(22,8);
                        strLongBytes =sData.Substring(30,8);
                        strSpeedByte = sData.Substring(38,2);
                        strACCStatus = sData.Substring(60,2);

                        decimal latVal = DecodeCoordinate(ConvertHexStringToByteArray(strLatBytes));
                        decimal longVal = DecodeCoordinate(ConvertHexStringToByteArray(strLongBytes));
                        Int64 hiz = Int64.Parse(strSpeedByte, System.Globalization.NumberStyles.HexNumber);

                        latVal = Math.Round(latVal,5);
                        longVal = Math.Round(longVal,5);

                        //========================  Call API  ========================
                        var httpClientHandler = new HttpClientHandler();
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                        {
                            return true;
                        };

                        using (HttpClient httpClient = new HttpClient(httpClientHandler))
                        {
                            //var content = await client.GetStringAsync(GlobalVariables.WebAPIBaseUrl + "GetFTPInfo/DUBAI-1");
                            httpClient.BaseAddress = new Uri(GlobalVariables.WebAPIBaseUrl);
                            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                            if(!_myIMEI.Equals("")) {

                                if(strACCStatus == "01" ) {
                                    KontakStatu = "Y" ;
                                } else {
                                    KontakStatu = "N" ;
                                }

                                var url = "AracLog/WriteLog/" + _myIMEI+"/"+latVal+"/"+longVal+"/"+KontakStatu+"/"+hiz + "/" + strVoltageLevel;

                                //gecici log..
                                myWriter.WriteLine(_clientIP + " > URL ::: " + httpClient.BaseAddress + url);


                                HttpResponseMessage response = await httpClient.GetAsync(url);
                                response.EnsureSuccessStatusCode();
                                var resp = await response.Content.ReadAsStringAsync();
                                if(response.StatusCode != HttpStatusCode.OK) {
                                    myWriter.WriteLine(_clientIP +  " > API ERROR > : " + resp);
                                    myWriter.WriteLine(" URL : " + url);
                                }
                                //FTPBilgiler ftpInfo = JsonConvert.DeserializeObject<FTPBilgiler>(resp);
                            } else {
                                //gecici log..
                                myWriter.WriteLine("UYARI !!! " + _clientIP + " İçin IMEI Verisi Yok.." );
                            }
                        }
                        //============================================================
                    }



                    //ALARM Msg : GPS + LBS + Status(Voltage + SignalStrength)
                    if(strProtokolNoByte == "16") {
                        myWriter.WriteLine(":::: ALARM :::: ");
                        myWriter.WriteLine(sData);
                    }
                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(_clientIP + " ERROR ----- DISCONNECTED : " + ex.Message);
                myWriter.WriteLine(_clientIP +  " > ERROR ----- DISCONNECTED : " + ex.Message);
            }
        }
    }
}