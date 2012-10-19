using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Client
{
    public class GameDownloader
    {
        public GameDownloader(string downloadURI)
        {
            this.downloadURI = downloadURI;
        }

        public void BeginDownload(string savePath)
        {
            if (storedSavePath == null || storedSavePath != savePath)
                BeginFreshDownload(savePath);
            else
                Resume();
        }

        public void BeginFreshDownload(string savePath)
        {
            storedSavePath = savePath;          // TODO: Remove old file if it exists?

            string downloadDir = Path.GetDirectoryName(storedSavePath);
            if (!new DirectoryInfo(downloadDir).Exists)
                Directory.CreateDirectory(downloadDir);

            if (new FileInfo(storedSavePath).Exists)
                File.Delete(storedSavePath);

            Resume();
        }

        private void Resume()
        {
            bytesAlreadyDownloaded = TotalProgress;

            Uri uri = new Uri(downloadURI);
            webClient = new WebClient();
            webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);

            if (bytesAlreadyDownloaded > 0)
                webClient.Headers.Add("DLOffset", bytesAlreadyDownloaded.ToString());
            foreach (KeyValuePair<String, String> kvp in extraHeaders)
                webClient.Headers.Add(kvp.Key, kvp.Value);

            DownloadState = State.Connecting;
            string downloadPath = partFiles.Count == 0 ? storedSavePath : System.IO.Path.GetTempFileName();   // create a new temporary storing file if needed
            partFiles.Add(downloadPath);
            webClient.DownloadFileAsync(uri, downloadPath);
        }

        public void AbortDownload()
        {
            webClient.CancelAsync();
        }

        private void MergePartFiles()
        {
            if (partFiles.Count <= 1)
                return;
            FileStream fsOriginal = null, fsPart = null;
            int appended = 0;
            try
            {
                fsOriginal = File.Open(partFiles[0], FileMode.Append);
                for (int i = 1; i < partFiles.Count; i++)
                {
                    fsPart = File.Open(partFiles[i], FileMode.Open);
                    byte[] data = new byte[fsPart.Length];
                    fsPart.Read(data, 0, (int)data.Length);
                    fsOriginal.Write(data, 0, (int)data.Length);
                    fsPart.Close();
                    fsPart = null;
                    File.Delete(partFiles[i]);
                    appended++;
                }
            }
            finally
            {
                if (fsOriginal != null)
                    fsOriginal.Close();
                if (fsPart != null)
                    fsPart.Close();
            }
            while (appended-- > 0)
                partFiles.RemoveAt(1);
        }

        void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                DownloadState = State.Cancelled;
            else if (e.Error != null)
                DownloadState = State.Error;
            else
                DownloadState = State.Finished;

            MergePartFiles();
            if (DownloadState == State.Finished)
            {
                if (DownloadFinished != null)
                    DownloadFinished(this, null);
            }
            else
            {
                GameDownloadFailedEventHandler args;
                if (e.Cancelled)
                    args = new GameDownloadFailedEventHandler(DownloadFailureReason.Cancelled);
                else
                    args = new GameDownloadFailedEventHandler(DownloadFailureReason.Error,
                        e.Error != null ? e.Error.Message : "No specific reason found.");
                if (DownloadFailed != null)
                    DownloadFailed(this, args);
                if (!e.Cancelled && AutoResume)
                    Resume();
            }
        }

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadState = State.Downloading;
            if (DownloadProgressChanged != null)
                DownloadProgressChanged(this, new GameDownloaderProgressChangedEventArgs
                {
                    BytesReceived = e.BytesReceived + bytesAlreadyDownloaded,
                    TotalBytesToReceive = e.TotalBytesToReceive + bytesAlreadyDownloaded
                });
        }

        public long TotalProgress
        {
            get
            {
                long length = 0;
                for (int i = 0; i < partFiles.Count; i++)
                    length += new FileInfo(partFiles[i]).Length;
                return length;
            }
        }

        public void DeleteInstallerFiles()
        {
            for (int i = partFiles.Count - 1; i >= 0; i--)
            { 
                if (File.Exists(partFiles[i]))
                    File.Delete(partFiles[i]);
            }
        }

        // events
        public event EventHandler DownloadFinished;
        public event EventHandler<GameDownloadFailedEventHandler> DownloadFailed;
        public event EventHandler<GameDownloaderProgressChangedEventArgs> DownloadProgressChanged;

        // properties
        public bool AutoResume { get; set; }
        public State DownloadState { get; private set; }
        public Dictionary<String, string> ExtraHeaders { get { return extraHeaders; } }

        // members
        private long bytesAlreadyDownloaded = 0;
        private string storedSavePath;
        private string downloadURI;
        private List<string> partFiles = new List<string>();
        private Dictionary<String, String> extraHeaders = new Dictionary<string, string>();
        private WebClient webClient;

        // enums
        public enum DownloadReturnCode
        {
            OK,
            ServerError,
        }

        public enum DownloadFailureReason
        {
            Cancelled,
            Error
        }

        public enum State
        {
            Connecting,
            Downloading,
            Cancelled,
            Error,
            Finished
        }

        // event handler argument classes
        public class GameDownloadFailedEventHandler : EventArgs
        {
            public GameDownloadFailedEventHandler(DownloadFailureReason reason)
            {
                Reason = reason;
            }
            public GameDownloadFailedEventHandler(DownloadFailureReason reason, string message)
                : this(reason)
            {
                Message = message;
            }
            public DownloadFailureReason Reason { get; private set; }
            public string Message { get; private set; }
        }

        public class GameDownloaderProgressChangedEventArgs : EventArgs
        {
            public long BytesReceived { get; set; }
            public long TotalBytesToReceive { get; set; }
        }
    }
}
