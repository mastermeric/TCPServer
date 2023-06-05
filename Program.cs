using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPServer;
class Program
{
    public static decimal DecodeLatitude(byte[] bytes)
    {
        decimal minutes = (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
        minutes /= 30000;
        minutes /= 60;
        return minutes;
    }

    public static Byte[] ConvertHexStringToByteArray1(string hexString)
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
    static async Task Main(string[] args)
    {
        try
        {
            Byte[] aa = ConvertHexStringToByteArray1("0465B9D6");
            Byte[] bb = ConvertHexStringToByteArray1("031927D7");
            decimal aaa = DecodeLatitude(new byte[] {0x04,0x65,0xB9,0xD6});
            decimal bbb = DecodeLatitude(new byte[] {0x03,0x19,0x27,0xD7});

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
