/*
 * Originally written by John Batte
 * Modifications, API changes and cleanups by Phil Crosby
 * http://codeproject.com/cs/library/downloader.asp
 * Modifications, by quifi
 */

using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Security;

namespace ColaFramework.Foundation.DownLoad
{
    /// <summary>
    /// Downloads and resumes files from HTTP, FTP, and File (file://) URLS
    /// </summary>
    public class FileDownloadHelper
    {
        // Block size to download is by default ???.
        private const int downloadBlockSize = 1024 * 32;

        // Determines whether the user has canceled or not.
        private bool canceled = false;

        public void Cancel()
        {
            this.canceled = true;
        }

        /// <summary>
        /// Timeout (ms)
        /// </summary>
        private Int32 m_timeout = 100;
        public Int32 Timeout
        {
            get { return m_timeout; }
            set { m_timeout = value; }
        }

        /// <summary>
        /// Progress update
        /// </summary>
        public event DownloadProgressHandler ProgressChanged;

        private IWebProxy proxy = null;

        /// <summary>
        /// Proxy to be used for http and ftp requests.
        /// </summary>
        public IWebProxy Proxy
        {
            get { return proxy; }
            set { proxy = value; }
        }

        /// <summary>
        /// Fired when progress reaches 100%.
        /// </summary>
        public event EventHandler DownloadComplete;

        private void OnDownloadComplete()
        {
            if (this.DownloadComplete != null)
                this.DownloadComplete(this, new EventArgs());
        }

        ///// <summary>
        ///// Begin downloading the file at the specified url, and save it to the current fileName.
        ///// </summary>
        //public void Download(string url)
        //{
        //    Download(url, "");
        //}
        /// <summary>
        /// Begin downloading the file at the specified url, and save it to the given fileName.
        /// </summary>
        public void Download(string url, string destFileName)
        {
            DownloadData data = null;
            this.canceled = false;

            try
            {
                // get download details                
                data = DownloadData.Create(url, destFileName, this.proxy, this.m_timeout);
                //// Find out the name of the file that the web server gave us.
                //string destFileName = Path.GetFileName(data.Response.ResponseUri.ToString());

                // create the download buffer
                byte[] buffer = new byte[downloadBlockSize];

                int readCount;

                // update how many bytes have already been read
                long totalDownloaded = data.StartPoint;

                bool gotCanceled = false;

                while ((int)(readCount = data.DownloadStream.Read(buffer, 0, downloadBlockSize)) > 0)
                {
                    // break on cancel
                    if (canceled)
                    {
                        gotCanceled = true;
                        data.Close();
                        break;
                    }

                    // update total bytes read
                    totalDownloaded += readCount;

                    // save block to end of file
                    SaveToFile(buffer, readCount, data.DownloadingToStream);

                    // send progress info
                    if (data.IsProgressKnown)
                        RaiseProgressChanged(totalDownloaded, data.FileSize);

                    // break on cancel
                    if (canceled)
                    {
                        gotCanceled = true;
                        data.Close();
                        break;
                    }
                }

                if (!gotCanceled)
                {
                    // stream could be incomplete
                    if (data.IsProgressKnown)
                    {
                        if (totalDownloaded < data.FileSize)
                            throw new WebException("date transfer not completed", WebExceptionStatus.ConnectionClosed);
                    }

                    RaiseProgressChanged(data.FileSize, data.FileSize);
                    data.CleanOnFinish();
                    OnDownloadComplete();
                }
            }
            catch (UriFormatException e)
            {
                throw new ArgumentException(
                    String.Format("Could not parse the URL \"{0}\" - it's either malformed or is an unknown protocol.", url), e);
            }
            finally
            {
                if (data != null)
                    data.Close();
            }
        }

        public static String MakeFingerPrintFilePath(String localFilePatch)
        {
            return localFilePatch + ".fp";
        }

        /// <summary>
        /// whether localFilePatch has downloading progressing information
        /// </summary>
        /// <param name="localFilePatch"></param>
        /// <returns></returns>
        public static Boolean IsFileInProgress(String localFilePatch)
        {
            return File.Exists(MakeFingerPrintFilePath(localFilePatch));
        }

        ///// <summary>
        ///// Download a file from a list or URLs. If downloading from one of the URLs fails,
        ///// another URL is tried.
        ///// </summary>
        //public void Download(List<string> urlList)
        //{
        //    this.Download(urlList, "");
        //}
        /// <summary>
        /// Download a file from a list or URLs. If downloading from one of the URLs fails,
        /// another URL is tried.
        /// </summary>
        public void Download(List<string> urlList, string destFileName)
        {
            // validate input
            if (urlList == null)
                throw new ArgumentException("Url list not specified.");

            if (urlList.Count == 0)
                throw new ArgumentException("Url list empty.");

            // try each url in the list.
            // if one succeeds, we are done.
            // if any fail, move to the next.
            Exception ex = null;
            foreach (string s in urlList)
            {
                ex = null;
                try
                {
                    Download(s, destFileName);
                }
                catch (Exception e)
                {
                    ex = e;
                }
                // If we got through that without an exception, we found a good url
                if (ex == null)
                    break;
            }
            if (ex != null)
                throw ex;
        }

        ///// <summary>
        ///// Asynchronously download a file from the url.
        ///// </summary>
        //public void AsyncDownload(string url)
        //{
        //    System.Threading.ThreadPool.QueueUserWorkItem(
        //        new System.Threading.WaitCallback(this.WaitCallbackMethod), new string[] { url, "" });
        //}
        /// <summary>
        /// Asynchronously download a file from the url to the destination fileName.
        /// </summary>
        public void AsyncDownload(string url, string destFileName)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(this.WaitCallbackMethod), new string[] { url, destFileName });
        }
        /// <summary>
        /// Asynchronously download a file from a list or URLs. If downloading from one of the URLs fails,
        /// another URL is tried.
        /// </summary>
        public void AsyncDownload(List<string> urlList, string destFileName)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(this.WaitCallbackMethod), new object[] { urlList, destFileName });
        }
        /// <summary>
        /// Asynchronously download a file from a list or URLs. If downloading from one of the URLs fails,
        /// another URL is tried.
        /// </summary>
        public void AsyncDownload(List<string> urlList)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(this.WaitCallbackMethod), new object[] { urlList, "" });
        }
        /// <summary>
        /// A WaitCallback used by the AsyncDownload methods.
        /// </summary>
        private void WaitCallbackMethod(object data)
        {
            // Can either be a string array of two strings (url and dest fileName),
            // or an object array containing a list<string> and a dest fileName
            if (data is string[])
            {
                String[] strings = data as String[];
                this.Download(strings[0], strings[1]);
            }
            else
            {
                Object[] list = data as Object[];
                List<String> urlList = list[0] as List<String>;
                String destFileName = list[1] as string;
                this.Download(urlList, destFileName);
            }
        }
        private void SaveToFile(byte[] buffer, int count, FileStream f)
        {
            try
            {
                f.Write(buffer, 0, count);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(
                    String.Format("Error trying to save file \"{0}\": {1}", f.Name, e.Message), e);
            }
        }
        private void RaiseProgressChanged(long current, long target)
        {
            if (this.ProgressChanged != null)
                this.ProgressChanged(this, new DownloadEventArgs(target, current));
        }
    }

    /// <summary>
    /// Constains the connection to the file server and other statistics about a file
    /// that's downloading.
    /// </summary>
    class DownloadData
    {
        private WebResponse response;

        private Stream stream;
        private String downloadTo;
        private FileStream downloadToStream = null;
        private long size;
        private long start;

        private struct FingerPrint
        {
            public String timeStamp;
            public Int64 fileSize;
        }
        private static FingerPrint LoadFingerPrint(String destFileName)
        {
            String fingerPrintFileName = FileDownloadHelper.MakeFingerPrintFilePath(destFileName);

            if (!File.Exists(fingerPrintFileName))  //记录文件尚未创建
                return new FingerPrint { timeStamp = "", fileSize = 0 };

            try
            {
                SecurityElement xmlDoc = SecurityElement.FromString(File.ReadAllText(fingerPrintFileName));
                String timeStamp = xmlDoc.Attributes["time_stamp"].ToString();
                Int64 fileSize = Int64.Parse(xmlDoc.Attributes["file_size"].ToString());
                return new FingerPrint { timeStamp = timeStamp, fileSize = fileSize };
            }
            catch (IOException)
            {
                return new FingerPrint { timeStamp = "", fileSize = 0 };
            }
            catch (System.IO.IsolatedStorage.IsolatedStorageException)
            {
                return new FingerPrint { timeStamp = "", fileSize = 0 };
            }
            catch (XmlSyntaxException)
            {
                return new FingerPrint { timeStamp = "", fileSize = 0 };
            }
            catch (FormatException)
            {
                return new FingerPrint { timeStamp = "", fileSize = 0 };
            }
            catch (NullReferenceException)
            {
                return new FingerPrint { timeStamp = "", fileSize = 0 };
            }
        }

        private static void SaveFingerPrint(String destFileName, FingerPrint fingerPrint)
        {
            String fingerPrintFileName = FileDownloadHelper.MakeFingerPrintFilePath(destFileName);

            SecurityElement finger_print = new SecurityElement("finger_print");

            finger_print.AddAttribute("time_stamp", fingerPrint.timeStamp);
            finger_print.AddAttribute("file_size", fingerPrint.fileSize.ToString());

            File.WriteAllText(fingerPrintFileName, finger_print.ToString());
        }

        private static void DeleteFingerPrint(String destFileName)
        {
            File.Delete(FileDownloadHelper.MakeFingerPrintFilePath(destFileName));
        }

        private static void DeleteDestFile(String destFileName)
        {
            File.Delete(FileDownloadHelper.MakeFingerPrintFilePath(destFileName));

            File.Delete(destFileName);
        }

        private Int32 timeout = 0;
        private IWebProxy proxy = null;

        public static DownloadData Create(string url, string destFileName)
        {
            return Create(url, destFileName, null, 0);
        }

        public static DownloadData Create(string url, string destFileName, IWebProxy proxy, Int32 timeout)
        {

            // This is what we will return
            DownloadData downloadData = new DownloadData();
            downloadData.proxy = proxy;
            downloadData.timeout = timeout;
            downloadData.downloadTo = destFileName;

            long urlSize = downloadData.GetFileSize(url);
            downloadData.size = urlSize;

            WebRequest req = downloadData.GetRequest(url);
            try
            {
                downloadData.response = (WebResponse)req.GetResponse();
            }
            catch (Exception e)
            {
                throw new ArgumentException(String.Format(
                    "Error downloading \"{0}\": {1}", url, e.Message), e);
            }

            String lastModified = downloadData.response.Headers["Last-Modified"] ?? "";

            // Check to make sure the response isn't an error. If it is this method
            // will throw exceptions.
            ValidateResponse(downloadData.response, url);


            // Take the name of the file given to use from the web server.
            //String fileName = System.IO.Path.GetFileName(downloadData.response.ResponseUri.ToString());

            //String downloadTo = Path.Combine(destFileName, fileName);

            String downloadTo = destFileName;

            FingerPrint fingerPrint = LoadFingerPrint(downloadTo);
            // If we don't know how big the file is supposed to be,
            // we can't resume, so delete what we already have if something is on disk already.
            if (!downloadData.IsProgressKnown
                || fingerPrint.timeStamp != lastModified
                || fingerPrint.fileSize != urlSize)
            {
                DeleteDestFile(downloadTo);
            }

            SaveFingerPrint(downloadTo, new FingerPrint { timeStamp = lastModified, fileSize = urlSize });

            if (downloadData.IsProgressKnown && File.Exists(downloadTo))
            {
                // We only support resuming on http requests
                if (!(downloadData.Response is HttpWebResponse))
                {
                    DeleteDestFile(downloadTo);
                }
                else
                {
                    // Try and start where the file on disk left off
                    downloadData.start = new FileInfo(downloadTo).Length;

                    if (downloadData.start < urlSize)
                    {
                        // Try and resume by creating a new request with a new start position
                        downloadData.response.Close();
                        req = downloadData.GetRequest(url);
                        ((HttpWebRequest)req).AddRange((int)downloadData.start);
                        downloadData.response = req.GetResponse();

                        if (((HttpWebResponse)downloadData.Response).StatusCode != HttpStatusCode.PartialContent)
                        {
                            // They didn't support our resume request. 
                            DeleteDestFile(downloadTo);
                            downloadData.start = 0;
                        }
                    }
                }
            }

            downloadData.downloadToStream = File.Open(downloadTo, FileMode.Append, FileAccess.Write);

            return downloadData;
        }

        // Used by the factory method
        private DownloadData()
        {
        }

        private DownloadData(WebResponse response, long size, long start)
        {
            this.response = response;
            this.size = size;
            this.start = start;
            this.stream = null;
        }

        /// <summary>
        /// Checks whether a WebResponse is an error.
        /// </summary>
        /// <param name="response"></param>
        private static void ValidateResponse(WebResponse response, string url)
        {
            if (response is HttpWebResponse)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)response;

                // If it's an HTML page, it's probably an error page. Comment this
                // out to enable downloading of HTML pages.
                if (/*httpResponse.ContentType.Contains("text/html") || */httpResponse.StatusCode == HttpStatusCode.NotFound)	//httpResponse.ContentType could be null
                {
                    throw new ArgumentException(
                        String.Format("Could not download \"{0}\" - a web page was returned from the web server.",
                        url));
                }
            }
            else if (response is FtpWebResponse)
            {
                FtpWebResponse ftpResponse = (FtpWebResponse)response;
                if (ftpResponse.StatusCode == FtpStatusCode.ConnectionClosed)
                    throw new ArgumentException(
                        String.Format("Could not download \"{0}\" - FTP server closed the connection.", url));
            }
            // FileWebResponse doesn't have a status code to check.
        }

        /// <summary>
        /// Checks the file size of a remote file. If size is -1, then the file size
        /// could not be determined.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="progressKnown"></param>
        /// <returns></returns>
        private long GetFileSize(string url)
        {
            WebResponse response = null;
            long size = -1;
            try
            {
                response = GetRequest(url).GetResponse();
                if (response != null)
                    size = response.ContentLength;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            return size;
        }

        private WebRequest GetRequest(string url)
        {
            //WebProxy proxy = WebProxy.GetDefaultProxy();
            WebRequest request = WebRequest.Create(url);
            if (timeout != 0)
                request.Timeout = timeout;

            if (request is HttpWebRequest)
            {
                request.Credentials = CredentialCache.DefaultCredentials;
                //Uri result = request.Proxy.GetProxy(new Uri("http://www.google.com"));
            }

            if (this.proxy != null)
            {
                request.Proxy = this.proxy;
            }

            return request;
        }

        public void Close()
        {
            this.response.Close();
            if (this.downloadToStream != null)
            {
                this.downloadToStream.Dispose();
                this.downloadToStream = null;
            }
        }

        public void CleanOnFinish()
        {
            DeleteFingerPrint(downloadTo);
        }

        #region Properties
        public WebResponse Response
        {
            get { return response; }
            set { response = value; }
        }
        public Stream DownloadStream
        {
            get
            {
                if (this.start == this.size)
                    return Stream.Null;
                if (this.stream == null)
                    this.stream = this.response.GetResponseStream();
                return this.stream;
            }
        }
        public FileStream DownloadingToStream
        {
            get
            {
                return this.downloadToStream;
            }
        }
        public long FileSize
        {
            get
            {
                return this.size;
            }
        }
        public long StartPoint
        {
            get
            {
                return this.start;
            }
        }
        public bool IsProgressKnown
        {
            get
            {
                // If the size of the remote url is -1, that means we
                // couldn't determine it, and so we don't know
                // progress information.
                return this.size > -1;
            }
        }
        #endregion
    }

    /// <summary>
    /// Progress of a downloading file.
    /// </summary>
    public class DownloadEventArgs : EventArgs
    {
        private int percentDone;
        private string downloadState;
        private long totalFileSize;

        public long TotalFileSize
        {
            get { return totalFileSize; }
            set { totalFileSize = value; }
        }
        private long currentFileSize;

        public long CurrentFileSize
        {
            get { return currentFileSize; }
            set { currentFileSize = value; }
        }

        public DownloadEventArgs(long totalFileSize, long currentFileSize)
        {
            this.totalFileSize = totalFileSize;
            this.currentFileSize = currentFileSize;

            this.percentDone = (int)((((double)currentFileSize) / totalFileSize) * 100);
        }

        public DownloadEventArgs(string state)
        {
            this.downloadState = state;
        }

        public DownloadEventArgs(int percentDone, string state)
        {
            this.percentDone = percentDone;
            this.downloadState = state;
        }

        public int PercentDone
        {
            get
            {
                return this.percentDone;
            }
        }

        public string DownloadState
        {
            get
            {
                return this.downloadState;
            }
        }
    }
    public delegate void DownloadProgressHandler(object sender, DownloadEventArgs e);
}