using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using YoutubeExtractor;
/// <summary>
/// Author: Manish Mallavarapu, Eric Lau 
/// Updated: 04/04/2016, by Manish Mallavarapu
/// </summary>
namespace DLTuber
{
    /// <summary>
    /// This class Downloads YouTube videos in the specified formats
    /// </summary>
    class Downloader
    {
        private static string v_;
        private static string url_;
        private static string dir_;
        private static ProgressBar progBar_;
        private static AudioDownloader audioDownloader_;
        private static BackgroundWorker audiodl;
        private static FormStatus childForm_; 
        private static BackgroundWorker videodl;
        private static bool isAudio_;
        private static bool isVideo_; 
        /// <summary>
        /// Private constructor to prevent instantiation
        /// Singleton pattern
        /// </summary>
        private Downloader()
        {
            
        }
        /// <summary>
        /// Creates a background worker to download in video format
        /// </summary>
        /// <param name="v"></param>
        /// <param name="url"></param>
        /// <param name="dir"></param>
        /// <param name="progressBar"></param>
        public static void startDownloadVideoThread(string v, string url, string dir,ref ProgressBar progressBar,ref FormStatus childForm)
        {
            isVideo_ = true;
            isAudio_ = false;
            v_ = v;
            url_ = url;
            dir_ = dir;
            progBar_ = progressBar;
            childForm_ = childForm;
            videodl = new BackgroundWorker();
            videodl.DoWork += bw_downloadVideo;
            videodl.ProgressChanged += bw_updateProgressBar;
            videodl.WorkerReportsProgress = true;
            videodl.WorkerSupportsCancellation = true;
            Button cancelButton = childForm.Controls.Find("cancelButton", false)[0] as Button;
            cancelButton.Click += cancelDownload;
            if (!videodl.IsBusy)
            {
                videodl.RunWorkerAsync();
            }
            
        }
        /// <summary>
        /// Creates a background worker to download audio 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="url"></param>
        /// <param name="dir"></param>
        /// <param name="progressBar"></param>
        public static void startDownloadAudioThread(string v, string url, string dir,ref ProgressBar progressBar,ref FormStatus childForm)
        {
            isVideo_ = false;
            isAudio_ = true; 
            v_ = v;
            url_ = url;
            dir_ = dir;
            childForm_ = childForm;
            progBar_ = progressBar; 
            audiodl = new BackgroundWorker();
            audiodl.DoWork += bw_downloadAudio;
            audiodl.ProgressChanged += bw_updateProgressBar;
            audiodl.WorkerReportsProgress = true;
            audiodl.WorkerSupportsCancellation = true;
            Button cancelButton = childForm.Controls.Find("cancelButton",false)[0] as Button;
            cancelButton.Click += cancelDownload;
            if (!audiodl.IsBusy)
            {
                audiodl.RunWorkerAsync();
            }
        }
        private static void cancelDownload(object sender, EventArgs e)
        {
            if (isVideo_)
            {
                videodl.CancelAsync();
            } 
            else if(isAudio_)
            {
                audiodl.CancelAsync(); 
            }
            childForm_.Close();
        }
        private static void bw_downloadAudio(object send, DoWorkEventArgs e)
        {
            isVideo_ = false;
            isAudio_ = true; 
            BackgroundWorker worker = send as BackgroundWorker;
            if (dir_ != " ")
            {
                //Parameter for video type
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url_);
                VideoInfo video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).First();
                
                if (video.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                }
                audioDownloader_ = new AudioDownloader(video, dir_);
                audioDownloader_.AudioExtractionProgressChanged += (sender, args) => progBar_.Invoke((Action)(() => { worker.ReportProgress((int)(85 + args.ProgressPercentage * 0.15)); }));
                try
                {
                    audioDownloader_.Execute();
                }
                catch (WebException we)
                {
                    MessageBox.Show("The video returned an error, please try again later",
                                     we.Response.ToString(),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private static void bw_downloadVideo(object send, DoWorkEventArgs e)
        {
            BackgroundWorker worker = send as BackgroundWorker;
            if (dir_ != " ")
            {
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url_);
                //Select the first .mp4 video with 360p resolution
                VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 );
                //If the video has a decrypted signature, decipher it
                if (video.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                }
                /*
                  Create the video downloader.
                  The first argument is the video to download.
                  The second argument is the path to save the video file.
                 */
                var videoDownloader = new VideoDownloader(video, dir_);
                // Register the ProgressChanged event and print the current progress
                videoDownloader.DownloadProgressChanged += (sender, args) => progBar_.Invoke((Action)(() => {worker.ReportProgress((int)(args.ProgressPercentage)); }));
                /*
                  Execute the video downloader.
                  For GUI applications note, that this method runs synchronously.
                 */
                try
                {
                    videoDownloader.Execute();
                }
                catch (WebException we)
                {
                    MessageBox.Show("The video returned an error, please try again later",
                                     we.Response.ToString(),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        } 
        private static void bw_updateProgressBar(object send, ProgressChangedEventArgs e)
        {
            progBar_.Invoke(new Action(() => { progBar_.Value = e.ProgressPercentage; })); 
        }
        /// <summary>
        /// Downloads the YouTube video to a .mp4 format
        /// </summary>
        /// <param name="v"></param>
        /// <param name="url"></param>
        /// <param name="dir"></param>
        /// <param name="progressBar1"></param>
        private static void downloadVideo(ref string v, ref string url, ref string dir, ProgressBar progressBar1)
        {

            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url);
            //Select the first .mp4 video with 360p resolution
            VideoInfo video = videoInfos
                .First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);

            //If the video has a decrypted signature, decipher it
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

           /*
             Create the video downloader.
             The first argument is the video to download.
             The second argument is the path to save the video file.
            */
            var videoDownloader = new VideoDownloader(video, dir);
            // Register the ProgressChanged event and print the current progress
            videoDownloader.DownloadProgressChanged += (sender, args) => progressBar1.Invoke((Action)(() => { progressBar1.Value = (int)(args.ProgressPercentage); }));
            videoDownloader.DownloadFinished += (sender, args) => completedDownload();
            /*
              Execute the video downloader.
              For GUI applications note, that this method runs synchronously.
             */
            try
            {
                videoDownloader.Execute();
            }
            catch (WebException e)
            {
                MessageBox.Show("The video returned an error, please try again later",
                                 e.Response.ToString(),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        ///  Downloads the YouTube video to a .mp3 format
        /// </summary>
        /// <param name="type"></param>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <param name="progressBar1"></param>
        /// <param name="progressBar"></param>
        private static void downloadAudio(ref string type, ref string url, ref string path,ProgressBar progressBar)
        {
            if (path != " ")
            {
                //Parameter for video type
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url);
                
                VideoInfo video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).First();
                if (video.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                }
                var audioDownloader = new AudioDownloader(video, path);    
                audioDownloader.DownloadProgressChanged += (sender, args) => progressBar.Invoke((Action)(() => { progressBar.Value = (int)(args.ProgressPercentage * 0.85); }));
                audioDownloader.AudioExtractionProgressChanged += (sender, args) => progressBar.Invoke((Action)(() => { progressBar.Value = (int)(85 + args.ProgressPercentage * 0.15); }));
                audioDownloader.DownloadFinished += (sender, args) => completedDownload();
                audioDownloader.Execute();
            }
        }
        private static void completedDownload()
        {
            ((Label)childForm_.Controls.Find("statusLabel", false)[0]).Text = "Done";
        }
       
    }
}
