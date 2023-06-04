using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class TcpServer
    {
        private TcpListener _server;

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

                // client found.. create a thread to handle communication
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("ERROR at LoopClients > " + ex.Message);
                }
            }
        }

        private void HandleClient(object obj)
        {
            String _clientIP = "";
            try
            {
                // retrieve client from parameter passed to thread
            TcpClient client = (TcpClient)obj;

            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);

            // you could use the NetworkStream to read and write,
            // but there is no forcing flush, even when requested

            //Boolean bClientConnected = true;
            String sData = null;

            _clientIP = IPAddress.Parse(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()).ToString();

            while (true)
            {
                //metot1 : reads from stream
                // sData = sReader.ReadLine();
                // if(sData != null)
                // {
                //     Console.WriteLine(_clientIP +" > " + sData);
                // } else {
                //     break;
                // }

                //metot2 : read string from bytes
                // Byte[] data = new Byte[256];
                // NetworkStream stream = client.GetStream();
                // Int32 bytes = stream.Read(data, 0, data.Length);
                // sData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                // if(bytes > 0)
                // {
                //     Console.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " > "+ sData);
                // } else {
                //     Console.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " Is Disconnected !");
                //     break;
                // }

                //Yontem3 : Read Byte arrays..
                Byte[] data = new Byte[256];


                //======  Protokol Numbers ======
                // Login Message 		0x01
                // Location Data 		0x22
                // Status information 	0x13
                // String information 	0x15
                // Alarm data 			0x16

                // ====== login msg     : 78 78 0D 01 01 23 45 67 89 01 23 45 00 01 8C DD 0D 0A
                // Start Bit (2)		: 0x78 0x78
                // Packet Length (1)	: 0x0D
                // Protocol Number (1)	: 0x01
                // Terminal ID (8)		: 0x01 0x23 0x45 0x67 0x89 0x01 0x23 0x45
                // Info Serial Nor (2)	: 0x00 0x01
                // Error Check (2)		: 0x8C 0xDD
                // Stop Bit (2)		    : 0x0D 0x0

                // ====== GPS msg 	        :
                // Start Bit (2) 			: 0x78 0x78
                // Packet Length (1) 		: 0x22
                // Protocol Number (1)		: 0x22
                // Date Time (6) 			: 0x0B 0x08 0x1D 0x11 0x2E 0x10
                // Quantity satellite (1) 	: 0xCF
                // Latitude (4) 			: 0x02 0x7A 0xC7 0xEB
                // Longitude (4) 			: 0x0C 0x46 0x58 0x49
                // Speed (1) 				: 0x00
                // Course,Status/ACC AC (2): 0x14 0x8F
                // LBS Information MCC (2)	: 0x01 0xCC
                // MNC (1)					: 0x00
                // LAC (2)					: 0x28 0x7D
                // Cell ID (3)				: 0x00 0x1F 0xB8
                // ACC+input2+ADC 0 or (2)	: 0x10 0xB6
                // Serial Number (2) 		: 0x00 0x03
                // Error Check (2)			: 0x80 0x81
                // Stop Bit (2)			    : 0x0D 0x0A


                String strStartBits = "";
                String strStoptBits = "";

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

                /*
                Byte[] StartBits = {0x78 , 0x78};
                Byte[] StoptBits = {0x0D , 0x0A};
                Byte[] DataLenByte = {0x00};
                Byte[] ProtokolNoByte = {0x00};
                Byte[] TerminalIDBytes = new Byte[8];
                Byte[] DateBytes = new Byte[6];
                Byte[] LatBytes = new Byte[4];
                Byte[] LongBytes = new Byte[4];
                Byte[] SpeedByte = {0x00};

                Array.Copy(dataReal,0,StartBits,0,2);
                Array.Copy(dataReal,2,DataLenByte,0,1);
                Array.Copy(dataReal,3,StartBits,0,1);
                Array.Copy(dataReal,0,StartBits,0,2);
                */

                NetworkStream stream = client.GetStream();
                Int32 _numberOfBytesInTCP = stream.Read(data, 0, data.Length);
                Byte[] dataReal = new Byte[_numberOfBytesInTCP];
                Array.Copy(data,0,dataReal,0,_numberOfBytesInTCP);

                sData = BitConverter.ToString(dataReal).Replace("-","");

                if(_numberOfBytesInTCP > 0)
                {
                    //MericY: Loglama icin tut..
                    Console.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " > "+ sData);
                } else {
                    Console.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " Is Disconnected !");
                    break;
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

                        Console.WriteLine("===================================");
                        Console.WriteLine(_clientIP +  " > IMEI >> " +  strTerminalIDBytes);
                        Console.WriteLine("===================================");

                        string sendData = "78780501" + strSerilNo + strErrorCheck + "0D0A";
                        //Console.WriteLine(_clientIP +  " > SEND DATA >> " +  sendData);

                        //stream.Flush();
                        //NetworkStream tempStream = client.GetStream();
                        Byte[] sendDataBytes = ConvertHexStringToByteArray(sendData);
                        stream.Write(sendDataBytes, 0, sendDataBytes.Length);
                    }

                    //GPS Msg
                    if(strProtokolNoByte == "22") {
                        strDateBytes = sData.Substring(8,12);
                        strQuantityOfSattelites =sData.Substring(20,2);
                        strLatBytes =sData.Substring(22,8);
                        strLongBytes =sData.Substring(30,8);
                        strSpeedByte = sData.Substring(38,2);

                        Console.WriteLine(_clientIP +  " > Date: " +  strDateBytes+  "  Lat: " +  strLatBytes+  " Lon: " +  strLongBytes+  " Speed: " +  strSpeedByte);

                        long n1 = Int64.Parse(strLatBytes, NumberStyles.HexNumber);
                        double XXXintVal1 = n1/30000;
                        double res1 = Math.Round(XXXintVal1/60,7);
                        Console.WriteLine(_clientIP + " > LAT : " + res1);

                        long n2 = Int64.Parse(strLongBytes, NumberStyles.HexNumber);
                        double XXXintVal2 = n2/30000;
                        double res2 = Math.Round(XXXintVal2/60,7);
                        Console.WriteLine(_clientIP + " > LONG : " + res2);

                        Console.WriteLine(_clientIP + " > Hiz bilgisi : " + strSpeedByte);
                    }
                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(_clientIP + " DISCONNECTED ! ");
            }
        }
    }
}