using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPServer;
class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            string sendData = "787805010D0A";
            Byte[] sendDataBytes = ConvertHexStringToByteArray(sendData);

            //======  Protokol Numbers ======
            // Login Message 		0x01
            // Location Data 		0x12
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
            // Packet Length (1) 		: 0x1F(31) or 0x21(33)
            // Protocol Number (1)		: 0x12
            // Date Time (6) 			: 0x0B 0x08 0x1D 0x11 0x2E 0x10
            // Quantity satellite (1) 	: 0xCF
            // Latitude (4) 			: 0x02 0x7A 0xC7 0xEB
            // Longitude (4) 			: 0x0C 0x46 0x58 0x49
            // Speed (1) 				: 0x00
            // Course,Status/ACC AC (2) : 0x14 0x8F
            // LBS Information MCC (2)	: 0x01 0xCC
            // MNC (1)					: 0x00
            // LAC (2)					: 0x28 0x7D
            // Cell ID (3)				: 0x00 0x1F 0xB8
            // ACC+input2+ADC 0 or (2)	: 0x10 0xB6
            // Serial Number (2) 		: 0x00 0x03
            // Error Check (2)			: 0x80 0x81
            // Stop Bit (2)			    : 0x0D 0x0A


            //==========   LOCAL TEST  ===============
            /*
            Byte[] xx = {0x01 , 0x02};
            Byte[] yy = {0x04 , 0x05};
            Byte[] MyBytes = {0x01 , 0x02, 0x03 , 0x04, 0x05 , 0x06};
            String sData =  "78780D01012345678901234500018CDD0D0A"; //BitConverter.ToString(MyBytes).Replace("-","");
            // int k = Array.IndexOf<Byte[]>(MyBytes,xx,0,2);
            // int z = Array.IndexOf(MyBytes,yy);



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

            if(sData.Contains("7878") && sData.Contains("0D0A"))  {

            strDataLen = sData.Substring(4,2);
            strProtokolNoByte = sData.Substring(6,2);

                //LOGIN Msg
                if(strProtokolNoByte == "01") {
                    strTerminalIDBytes = sData.Substring(8,16);
                    strSerilNo =sData.Substring(24,4);
                    strErrorCheck = sData.Substring(28,4);

                    string sendData = "78780501" + strSerilNo + strErrorCheck + "0D0A";

                    Byte[] sendDataBytes = ConvertHexStringToByteArray(sendData);

                }

                //GPS Msg
                if(strProtokolNoByte == "12") {
                    strDateBytes = sData.Substring(8,12);
                    strQuantityOfSattelites =sData.Substring(20,2);
                    strLatBytes =sData.Substring(22,8);
                    strLongBytes =sData.Substring(30,8);
                }
            }
            */
            //================================================

            if (args is null)
            {
                Console.WriteLine("ERROR at Args!");
            }

            Console.WriteLine("SERVER Started.. OK..");

            //metot1
            TcpServer server = new TcpServer(6666);
            Console.ReadLine();

            //metot2
            // await new Program().StartListener();
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR at Args!");
            Console.WriteLine(ex.Message);
        }
    }

    public static byte[] ConvertHexStringToByteArray(string hexString)
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
}
