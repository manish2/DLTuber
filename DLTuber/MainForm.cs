using System;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;
using System.Web;
using System.Threading;
/// <summary>
/// Author: Manish Mallavarapu, Eric Lau
/// Last update: 4/4/2016, by Manish Mallavarapu
/// Version: 1
/// DLTuber uses Youtube Extractor, A DLL made by FlagBug
/// Found at https://github.com/flagbug/YoutubeExtractor 
/// </summary>
namespace DLTuber
{
    /// <summary>
    /// Start of program, Creates global variables and loads form
    /// </summary>
    public partial class MainForm : Form
    {
        private string title;
        private const string ERR_MSSG = "DLTuber has failed to sense an active internet connection," + 
                                  "you can still use DLTuber but some features may not be available";
        private InternetConnection conn;
        /// <summary>
        /// The constructor, program starts from here
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// On Form load, check for internet connection before continuing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new InternetConnection();
            if(!isConnected(conn))
            {
                MessageBox.Show(ERR_MSSG, "No Internet Connection",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                selectVideo.Enabled = false;
            }   
        }
        /// <summary>
        /// Returns true if internet is available
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool isConnected(InternetConnection c)
        {
            return c.connected() ? true : false; 
        }
        /// <summary>
        /// Returns true if URL is a valid YouTube URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool isValidUrl(string url)
        {            
            bool flag = false; 
            
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    flag = response.ResponseUri.ToString().Contains("youtube.com") ? true : false;
            }                
            catch(WebException e)
            {
                return flag; 
            }
            catch(UriFormatException e)
            {
                return flag; 
            }
            return flag; 
        }
        /// <summary>
        /// Loads thumbnail and video title onto pictureBox
        /// </summary>
        /// <param name="url"></param>
        private void loadThumbNail(string url)
        {
            try
            {
                title = GetTitle(url);
                if(vidTitle.InvokeRequired)
                {
                    vidTitle.BeginInvoke((MethodInvoker)delegate() { vidTitle.Text = "Title: " + title;}); 
                }
                else
                {
                    vidTitle.Text = "Title: " + title;
                }               
                videoThumbNail.Load("https://i.ytimg.com/vi/" + url + "/mqdefault.jpg");
            } 
            catch(WebException e)
            {
                videoThumbNail.Image = videoThumbNail.ErrorImage; 
            }
            
        }
        /// <summary>
        /// Checks if there is an open connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkConn(object sender, EventArgs e)
        {
            if(conn.connected())
            {
                selectVideo.Enabled = true;
            }
            else
            {
               selectVideo.Enabled = false;
               MessageBox.Show("DLTuber does not sense an internet connection", "No Internet Connection",
               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Opens File dialog to get desired save spot from user
        /// </summary>
        /// <param name="fType"></param>
        /// <returns></returns>
        private string openFileLocation(string fType)
        {
            string dir = " "; 
            switch(fType)
            {
                case "mp3":
                    fileLocationDialog.Filter = "mp3|*.mp3";
                    break;
                case "mp4":
                    fileLocationDialog.Filter = "mp4|*.mp4";
                    break;
                case "wav":
                    fileLocationDialog.Filter = "wav|*.wav";
                    break;
            }
            if(fileLocationDialog.ShowDialog() == DialogResult.OK)
            {        
                dir = fileLocationDialog.FileName; 
            }
            return dir; 
        }
       /// <summary>
       /// Gets the preferred format of video from the user
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void handleVideoClick(object sender, EventArgs e)
        {
            if (!conn.connected())
            {
                MessageBox.Show("DLTuber does not sense an internet connection, Please check your connection and try again"
                                 ,"No Internet Connection",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                selectVideo.Enabled = false;
            }
            else
            {
                selectVideo.Enabled = true;
                Button b = (Button)sender;
                string url = urlBox.Text;
                RunDownload(url); 
            }
        }
        /// <summary>
        /// Creates a new form to show download percentage 
        /// </summary>
        /// <param name="url"></param>
        private void RunDownload(string url)
        {
         
            if (isValidUrl(url))
            {
                loadThumbNail(url.Split('=')[1]);
                string dir;
                string title = GetTitle(url.Split('=')[1]);
                vidTitle.Text = "Title: " + title; 
                FormStatus childForm = new FormStatus();
                ProgressBar progBar = childForm.getProgressBar();
                childForm.setTitle(title); 
                if (mp3RadioBtn.Checked)
                {
                    dir = openFileLocation("mp3");
                    childForm.Show();
                    Thread audioThread = new Thread(() => Downloader.startDownloadAudioThread("mp3", url, dir, ref progBar,ref childForm));
                    audioThread.SetApartmentState(ApartmentState.STA);
                    audioThread.Start();         
                }
                else if (mp4RadioBtn.Checked)
                {
                    dir = openFileLocation("mp4");
                    childForm.Show();   
                    Thread videoThread = new Thread(() => Downloader.startDownloadVideoThread("mp3", url, dir, ref progBar,ref childForm));
                    videoThread.SetApartmentState(ApartmentState.STA);
                    videoThread.Start();             
                }
                else
                {
                    dir = openFileLocation("wav");
                }
            }
        }
 
        /// <summary>
        /// Gets Title of the video from the web through the URL 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetTitle(string url)  
        {
            WebClient client = new WebClient();
            return GetArgs(client.DownloadString("http://youtube.com/get_video_info?video_id=" + url), "title", '&');
        }

        private string GetArgs(string args, string key, char query)
        {
            int iqs = args.IndexOf(query);
            string querystring = null;

            if (iqs != -1)
            {
                querystring = (iqs < args.Length - 1) ? args.Substring(iqs + 1) : string.Empty;
                NameValueCollection nvcArgs = HttpUtility.ParseQueryString(querystring);
                return nvcArgs[key];
            }
            return "Not Available"; 
        }
    }
}
