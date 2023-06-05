using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TCPServer
{
    public class MultiThreadFileWriter
    {
        private static ConcurrentQueue<string> _textToWrite = new ConcurrentQueue<string>();
        private CancellationTokenSource _source = new CancellationTokenSource();
        private CancellationToken _token;

        //public static string lokalLogDosyasi = @"C:\OziTeknolojiApiLoglar\" + DateTime.Now.ToShortDateString().Replace(".", "") + "_ErrorReport.log";
        public static string lokalLogDosyasi = DateTime.Now.ToString("ddMMyyyy") + "_DailyLogReport.log";

        public MultiThreadFileWriter()
        {
            _token = _source.Token;
            // This is the task that will run
            // in the background and do the actual file writing
            Task.Run(WriteToFile, _token);
        }

        /// The public method where a thread can ask for a line
        /// to be written.
        public void WriteLine(string line)
        {
            line = DateTime.Now.ToString("ddMMyyyy_HHmmss") +  " -> "+  line;
            _textToWrite.Enqueue(line);
        }

        /// The actual file writer, running
        /// in the background.
        private async void WriteToFile()
        {
            while (true)
            {
                if (_token.IsCancellationRequested)
                {
                    return;
                }
                using (StreamWriter w = File.AppendText(lokalLogDosyasi))
                {
                    while (_textToWrite.TryDequeue(out string textLine))
                    {
                        await w.WriteLineAsync(textLine);
                    }
                    w.Flush();
                    Thread.Sleep(100);
                }
            }
        }
    }
}