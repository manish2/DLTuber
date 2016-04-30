using System.Windows.Forms;
using System.Net.NetworkInformation;
using System;
/// <summary>
/// Author: Manish Mallavarapu, Eric Lau
/// Updated: 04/04/2016, by Manish Mallavarapu 
/// </summary>
namespace DLTuber
{
    public partial class FormStatus : Form
    {
        public bool isCanceled = false; 
        public FormStatus()
        {
            InitializeComponent();
            NetworkChange.NetworkAvailabilityChanged += AvailabilityChanged;
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }
        public ProgressBar getProgressBar()
        {
            return statusBar; 
        }
        public void setTitle(string title)
        {
            vidTitle.Text = "Title: " + title;
        }
        private void AvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            MessageBox.Show("A problem occured while downloading your video, please check the internet and try again"
                            ,"No Internet Connection",MessageBoxButtons.OK
                            , MessageBoxIcon.Error);
        }
    }
}
