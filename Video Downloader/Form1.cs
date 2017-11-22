using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExtractor;

namespace Video_Downloader
{
    public partial class Form1 : Form
    {
      
        private VideoDownloader downloader;
        private string _savedPath;
        private bool abort = false;
        private bool downloaded = false;

        public Form1()
        {
            InitializeComponent();
        }

        private bool InputIsValid(string url)
        {
            bool result = false;
            if (!String.IsNullOrEmpty(url) && url.Length < 44)
            {
                Uri uriResult;
                result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
            return result;
        }

        private void Download_Click(object sender, EventArgs e)
        {
            abort = false;
            if (InputIsValid(txtUrl.Text))
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(txtUrl.Text);
               
                    VideoInfo video = videos.First(p => (p.VideoType == VideoType.Mp4) || (p.VideoType == VideoType.Flash) && p.Resolution == Convert.ToInt32(cbResolution.Text));
                    if (video.RequiresDecryption)
                        DownloadUrlResolver.DecryptDownloadUrl(video);

                    _savedPath = Path.Combine(DefinePath() + "\\", video.Title + video.VideoExtension);
                    if (abort)
                    {
                        return;
                    }
                    downloader = new VideoDownloader(video, _savedPath);
                     downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
                    Thread thread = new Thread(() => { try { downloader.Execute(); } catch(Exception exc) { MessageBox.Show("URL IS NOT VALID OR YOU HAVE NO RIGHTS"); } }) { IsBackground = true };
                    thread.Start();
                
                
            }
            else CleanTextBox();

        }

        private void CleanTextBox()
        {
            MessageBox.Show("URL IS NOT VALID");
            txtUrl.Clear();
        }
     
        private string DefinePath()
        {
            string savePath = null;
            string dummyFileName = "Save Here";

            SaveFileDialog sf = new SaveFileDialog();
            // Feed the dummy name to the save dialog
            sf.FileName = dummyFileName;

            if (sf.ShowDialog() == DialogResult.OK)
            {
                // Now here's our save folder
                savePath = Path.GetDirectoryName(sf.FileName);
               
            }
            else abort = true;
            
            return savePath;
        }

        private void Downloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                progressBar.Value = (int)e.ProgressPercentage;
                Percentage.Text = Convert.ToString((int)e.ProgressPercentage) + " %";
                Download.Enabled = false;
                progressBar.Update();
  
                if (progressBar.Value == 100)
                {
                    
                    MessageBox.Show("Download Completed");
                    if (MessageBoxButtons.OK==0)
                    {
                        OnDownloadComplete();
                    }
                }

            }));
        }

        private void OnDownloadComplete()
        {
            downloaded = true;
            Download.Enabled = true;
            Play.Visible = true;
            OpenFile.Visible = true;
            progressBar.Value = 0;
            Percentage.Text = "0 %";
            txtUrl.Clear();
            LatestMedia media = new LatestMedia(_savedPath);
            lblName.Text = media.GetName();
            lblSize.Text = String.Format("{0:0.00}", media.GetSize().ToString()) + " MB";
            lblLocation.Text = media.GetPath();
            lblExt.Text = media.GetExtension();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            cbResolution.SelectedIndex = 0;
        }

        private void openFile_click(object sender, EventArgs e)
        {
            if (downloaded)
            System.Diagnostics.Process.Start(_savedPath);
         
        }
        private void openFolder_click(object sender, EventArgs e)
        {
            if (downloaded)
                System.Diagnostics.Process.Start("explorer.exe",Path.GetDirectoryName(_savedPath));
        }

        private void Download_EnabledChanged(object sender, EventArgs e)
        {
            if (!Download.Enabled)
                Download.BackColor = Color.LightGray;
            else Download.BackColor = Color.ForestGreen;
        }
    }
}
