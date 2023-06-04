namespace TCPServer
{

    //The Mutex will let threads wait for the mutex handle in a queue until the mutex is released. 
    //So if 3 threads will write to the same file, they will wait in line nicely until they are granted access.

    //It is very important that the code releases the mutex as well, or the code will be locked forever.
    //Mutexes can be named, like in the example above. This locks the shared resource system wide. 
    //So if another process tries to access the same code, that process will also wait in line. 
    //Backslashes in mutex names are a reserved character and must be removed.



    public class ThreadSafeFileWriter
    {
        readonly string mutexReaderName = DateTime.Now.ToShortDateString().Replace(" ", "")
        .Replace(".", "").Replace(":", "").Replace("/", "")
        .Replace("\\", "").Replace("_", "").Replace("-", "")
        + "_ReaderMutex";

        readonly string mutexWriterName = DateTime.Now.ToShortDateString().Replace(" ", "")
        .Replace(".", "").Replace(":", "").Replace("/", "")
        .Replace("\\", "").Replace("_", "").Replace("-", "")
        +"_WriterrMutex";


        readonly string dailyLogFile = DateTime.Now.ToShortDateString().Replace(" ", "")
        .Replace(".", "").Replace(":", "").Replace("/", "")
        .Replace("\\", "").Replace("_", "").Replace("-", "")
        + "_Log.txt" ;


        readonly string dailyErrorLogFile = DateTime.Now.ToShortDateString().Replace(" ", "")
        .Replace(".", "").Replace(":", "").Replace("/", "")
        .Replace("\\", "").Replace("_", "").Replace("-", "")
        + "_ErrorLog.txt" ;


        public void WriteLogFile(string fileContents)
        {
            using (var mutex = new Mutex(false))
            {
                var hasHandle = false;
                try
                {
                    hasHandle = mutex.WaitOne(Timeout.Infinite, false);
                    if (File.Exists(dailyLogFile))
                    {
                        return;
                    }
                    File.WriteAllText(dailyLogFile, fileContents);
                }
                catch (Exception ex)
                {
                    if (File.Exists(dailyErrorLogFile))
                    {
                        return;
                    }
                    File.WriteAllText(dailyErrorLogFile, ex.Message);
                }
                finally
                {
                    if (hasHandle)
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
        }

        public string ReadLogFile()
        {
            // This block will be protected area
            using (var mutex = new Mutex(false))
            {
                var hasHandle = false;
                try
                {
                    // Wait for the muted to be available
                    hasHandle = mutex.WaitOne(Timeout.Infinite, false);
                    // Do the file read
                    if (!File.Exists(dailyLogFile))
                    {
                        return string.Empty;
                    }
                    return File.ReadAllText(dailyLogFile);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    // Very important! Release the mutex
                    // Or the code will be locked forever
                    if (hasHandle)
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
        }

    }
}