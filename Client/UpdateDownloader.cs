using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Microsoft.Win32;
using System.Threading;

namespace Client
{
    public partial class UpdateDownloader : Form
    {
        #region Windows Forms

        public UpdateDownloader()
        {
            InitializeComponent();
        }

        public UpdateDownloader(string installationDirectory) : this()
        {
            this.installationDirectory = installationDirectory;
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (downloadComplete)
            {
                this.DialogResult = DialogResult.OK;
                Close();
                var process = System.Diagnostics.Process.Start(executePath, "/SP- /silent /noicons \"/dir=" + InstallationDirectory + "\"");
                SaveInstallerPath(executePath);
            }
            else
            {
                downloader.BeginDownload(downloadPath);
                SetDownloadingGUIState();
            }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            var state = downloader.DownloadState;
            if (downloadComplete || state == GameDownloader.State.Cancelled || state == GameDownloader.State.Error)
            {
                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    System.Threading.Thread.Sleep(3000);
                    DeleteInstaller();
                });
                this.DialogResult = DialogResult.Ignore;
            }
            else
            {
                downloader.AbortDownload();
                SetAbortedGUIState();
            }
        }

        private void SetFinishedGUIState()
        {
            btnInstall.Enabled = true;
            btnAbort.Text = "Close without installing";
        }

        private void SetDownloadingGUIState()
        {
            btnAbort.Text = "Abort";
            btnInstall.Text = "Install";
            btnInstall.Enabled = false;
        }

        private void SetAbortedGUIState()
        {
            btnInstall.Text = "Resume";
            btnInstall.Enabled = true;
        }

        #endregion

        #region IO

        private void SaveInstallerPath(string path)
        {
            using (StreamWriter sw = new StreamWriter(tmpInstallPathFile, true))
            {
                sw.WriteLine(path);
            }
        }

        public static void ClearOldInstallers()
        {
            if (File.Exists(tmpInstallPathFile))
            {

                using (StreamReader sr = new StreamReader(tmpInstallPathFile))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (System.IO.File.Exists(s))
                            System.IO.File.Delete(s);
                    }
                }
                File.Delete(tmpInstallPathFile);
            }
        }

        private void DeleteInstaller()
        {
            downloader.DeleteInstallerFiles();
            //if (File.Exists(downloadPath))
            //    File.Delete(downloadPath);
            if (File.Exists(executePath))
                File.Delete(executePath);
        }

        #endregion

        public void SendLatestVersionRequest()
        {
            string versionLookupURI = Program.Settings.VersionLookupURI.Trim();
            if (string.IsNullOrEmpty(versionLookupURI) ||
                string.IsNullOrEmpty(Program.Settings.DownloadHost) ||
                string.IsNullOrEmpty(Program.Settings.DownloadKey) ||
                string.IsNullOrEmpty(Program.Settings.DownloadURI))
            {
                latestVersion = Program.GameVersion;
                versionInfoReceived.Set();
                return;
            }

            //try
            //{
                WebRequest webRequest = WebRequest.Create(versionLookupURI);
                RequestState requestState = new RequestState { Request = webRequest, UpdateDownloader = this };
                webRequest.BeginGetResponse(RespCallback, requestState);

                //HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                //Stream receiveStream = response.GetResponseStream();
                //StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                //string s = readStream.ReadToEnd();
                //response.Close();
                //return new Version(s.Trim());
            //}
            //return null;
        }

        private static void RespCallback(IAsyncResult asynchronousResult)
        {
            RequestState requestState = (RequestState)asynchronousResult.AsyncState;
            UpdateDownloader updateDownloader = requestState.UpdateDownloader;

            updateDownloader.versionInfoReceived.Set();

            try
            {
                WebResponse response = requestState.Request.EndGetResponse(asynchronousResult);
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string versionString = sr.ReadLine();
                Version version = new Version(versionString.Trim());
                updateDownloader.latestVersion = version;
            }
            catch (WebException ex)
            {
                Graphics.Application.Log("Failed to find latest available version: " + ex.Message);
            }
            catch (Exception ex)
            {
                Graphics.Application.Log("Couldn't parse latest available version: " + ex.Message);
            }
        }

        class RequestState
        {
            public WebRequest Request;
            public UpdateDownloader UpdateDownloader;
        }

        private Version latestVersion;
        ManualResetEvent versionInfoReceived = new ManualResetEvent(false);
        float maxWaitTime = 3f;

        //public static bool TryToFindUpdate(string installationDirectory)
        //{
        //    string versionLookupURI = Program.Settings.VersionLookupURI.Trim();
        //    if (string.IsNullOrEmpty(versionLookupURI) || 
        //        string.IsNullOrEmpty(Program.Settings.DownloadHost) ||
        //        string.IsNullOrEmpty(Program.Settings.DownloadKey) ||
        //        string.IsNullOrEmpty(Program.Settings.DownloadURI))
        //        return false;

        //    Graphics.Application.Log("Looking up latest version from " + versionLookupURI);
        //    Version latestVersion = RemoteVersion(versionLookupURI);
        //    Graphics.Application.Log("Latest version is " + latestVersion);

        //    if (latestVersion != null && latestVersion > new Version(0, 0, 0, 0))
        //    {
        //        Version currentVersion = Program.GameVersion;
        //        if (currentVersion < latestVersion)
        //        {
        //            Graphics.Application.Log("Latest version is newer than current version, updating");
        //            UpdateDownloader updateDownloader = new UpdateDownloader(installationDirectory);
        //            bool downloading = updateDownloader.DownloadGame(Program.Settings.DownloadURI);
        //            if (downloading)
        //            {
        //                var result = updateDownloader.ShowDialog();
        //                return result == DialogResult.OK;
        //            }
        //        }
        //    }
        //    return false;
        //}


        public bool NewVersionAvailable
        {
            get
            {
                if (!versionInfoReceived.WaitOne((int)(1000f * maxWaitTime), false))
                    return false;
                return Program.GameVersion < latestVersion;
            }
        }

        public bool TryToUpdate()
        {
            if (!DownloadGame(Program.Settings.DownloadURI))
                return false;
            var result = ShowDialog();
            return result == DialogResult.OK;
        }

        public bool TryToUpdateBackground()
        {
            return DownloadGame(Program.Settings.DownloadURI);
        }

        private bool DownloadGame(string downloadURI)
        {
            downloader = new GameDownloader(downloadURI);
            downloader.DownloadFinished += new EventHandler(downloader_DownloadFinished);
            downloader.DownloadProgressChanged += new EventHandler<GameDownloader.GameDownloaderProgressChangedEventArgs>(downloader_DownloadProgressChanged);
            downloader.DownloadFailed += new EventHandler<GameDownloader.GameDownloadFailedEventHandler>(downloader_DownloadFailed);
            downloadPath = System.IO.Path.GetTempFileName();
            downloader.BeginFreshDownload(downloadPath);

            return true;        // TODO
        }

        void downloader_DownloadFinished(object sender, EventArgs e)
        {
            if (new System.IO.FileInfo(downloadPath).Exists)
            {
                string newPath = downloadPath.Substring(0, downloadPath.LastIndexOf('.')) + ".exe";
                File.Move(downloadPath, newPath);

                downloadComplete = true;
                SetFinishedGUIState();
                this.executePath = newPath;
            }
            else
                lblDownloadText.Text = "Error opening installer.";
        }

        void downloader_DownloadProgressChanged(object sender, GameDownloader.GameDownloaderProgressChangedEventArgs e)
        {
            DateTime now = DateTime.Now;
            if ((now - lastGUIUpdate).TotalMilliseconds > 33 || e.TotalBytesToReceive == e.BytesReceived)
            {
                lastGUIUpdate = now;
                progressBar.Minimum = 0;
                progressBar.Maximum = (int)e.TotalBytesToReceive;
                progressBar.Value = (int)e.BytesReceived;
                lastProgressString = String.Format("{0:0.0} of {1:0.0} kB ({2:0.0}%)",
                    e.BytesReceived / 1024f,
                    e.TotalBytesToReceive / 1024f,
                    100f * e.BytesReceived / e.TotalBytesToReceive);
                lblDownloadText.Text = String.Format("Downloading: {0}", lastProgressString);
            }
        }

        void downloader_DownloadFailed(object sender, GameDownloader.GameDownloadFailedEventHandler e)
        {
            if (e.Reason == GameDownloader.DownloadFailureReason.Cancelled)
                lblDownloadText.Text = "Paused: " + lastProgressString;
            else
                lblDownloadText.Text = "Error getting installer: " + e.Message; 
        }

        private static void IncorrectKeyMessage(string message)
        {
            System.Windows.Forms.MessageBox.Show(message, "Failed to update game", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //private static Version RemoteVersion(string url)
        //{
        //    try
        //    {
        //        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
        //        HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
        //        Stream receiveStream = response.GetResponseStream();
        //        StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
        //        string s = readStream.ReadToEnd();
        //        response.Close();
        //        return new Version(s.Trim());
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return null;
        //}

        public string InstallationDirectory
        {
            get { return installationDirectory; }
            set
            {
                installationDirectory = value;
                if (!string.IsNullOrEmpty(installationDirectory) && installationDirectory.Length > 0 && !installationDirectory.EndsWith("\\"))
                    installationDirectory += "\\";
            }
        }


        private DateTime lastGUIUpdate = DateTime.MinValue;

        private bool downloadComplete = false;
        private string lastProgressString = "";

        private string executePath, installationDirectory;
        private string downloadPath;
        private GameDownloader downloader;

        private static readonly string tmpInstallPathFile = "ins.tmp";
    }
}
