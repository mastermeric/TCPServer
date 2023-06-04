namespace TCPServer
{
    public class ThreadSafeFileWriter
    {
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
            using (var mutex = new Mutex(false, dailyLogFile))
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
            using (var mutex = new Mutex(false, dailyLogFile))
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

        public string ReadErrorFile(string filePathAndName)
        {
            // This block will be protected area
            using (var mutex = new Mutex(false, filePathAndName.Replace("\\", "")))
            {
                var hasHandle = false;
                try
                {
                // Wait for the muted to be available
                hasHandle = mutex.WaitOne(Timeout.Infinite, false);
                // Do the file read
                if (!File.Exists(filePathAndName))
                    return string.Empty;
                return File.ReadAllText(filePathAndName);
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
                    mutex.ReleaseMutex();
                }
            }
        }

    }
}