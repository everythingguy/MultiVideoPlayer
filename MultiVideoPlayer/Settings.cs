using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiVideoPlayer
{
    public partial class Settings : Form
    {
        #region Main
        public Settings()
        {
            InitializeComponent();
        }

        private string password = "";
        private string secondIP = "";
        private bool mainPC = true;
        private bool startHalfway = false;
        private bool startRestMuted = true;
        private bool startAllMuted = false;
        private bool startLowVolume = true;
        private bool topDock = true;
        private bool topPlayers = true;
        private bool dirOrder = true;
        private bool videosOrder = true;
        private bool dualScreen = false;
        private int rows = 2;
        private int columns = 2;
        private int rowsSecondary = 2;
        private int columnsSecondary = 2;
        private double lowVolumeLevel = 0.2;
        private int halfwayLength = 0;
        private int timerDuration = 3;
        private List<string> directories = new List<string>();
        private void defaultSettings()
        {
            secondIP = "";
            mainPC = true;
            startHalfway = false;
            startRestMuted = true;
            startAllMuted = false;
            startLowVolume = true;
            topDock = true;
            topPlayers = true;
            dirOrder = true;
            videosOrder = true;
            dualScreen = false;
            rows = 2;
            columns = 2;
            rowsSecondary = 2;
            columnsSecondary = 2;
            lowVolumeLevel = 0.2;
            halfwayLength = 0;
            timerDuration = 3;
            saveSettings();
        }

        private void saveSettings()
        {
            Properties.Settings.Default.password = password;
            Properties.Settings.Default.secondIP = secondIP;
            Properties.Settings.Default.mainPC = mainPC;
            Properties.Settings.Default.startHalfway = startHalfway;
            Properties.Settings.Default.startRestMuted = startRestMuted;
            Properties.Settings.Default.startAllMuted = startAllMuted;
            Properties.Settings.Default.startLowVolume = startLowVolume;
            Properties.Settings.Default.lowVolumeLevel = lowVolumeLevel;
            Properties.Settings.Default.directories = directories;
            Properties.Settings.Default.halfwayLength = halfwayLength;
            Properties.Settings.Default.topDock = topDock;
            Properties.Settings.Default.topPlayers = topPlayers;
            Properties.Settings.Default.dirOrder = dirOrder;
            Properties.Settings.Default.videosOrder = videosOrder;
            Properties.Settings.Default.dualScreen = dualScreen;
            Properties.Settings.Default.rows = rows;
            Properties.Settings.Default.columns = columns;
            Properties.Settings.Default.rowsSecondary = rowsSecondary;
            Properties.Settings.Default.columnsSecondary = columnsSecondary;
            Properties.Settings.Default.timerDur = timerDuration;
            Properties.Settings.Default.Save();
        }

        private void updateForms()
        {
            Dock dock = getDockForm();

            dock.populateLst();
            dock.updateSettings();
            dock.TopMost = Properties.Settings.Default.topDock;
        }

        private Dock getDockForm()
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is Dock)
                {
                    return (Dock)form;
                }
            }

            Dock newForm = new Dock();
            newForm.Show();

            return newForm;
        }

        private void loadSettings()
        {
            password = Properties.Settings.Default.password;
            secondIP = Properties.Settings.Default.secondIP;
            mainPC = Properties.Settings.Default.mainPC;
            startHalfway = Properties.Settings.Default.startHalfway;
            startRestMuted = Properties.Settings.Default.startRestMuted;
            startAllMuted = Properties.Settings.Default.startAllMuted;
            startLowVolume = Properties.Settings.Default.startLowVolume;
            lowVolumeLevel = Properties.Settings.Default.lowVolumeLevel;
            halfwayLength = Properties.Settings.Default.halfwayLength;
            topPlayers = Properties.Settings.Default.topPlayers;
            topDock = Properties.Settings.Default.topDock;
            dirOrder = Properties.Settings.Default.dirOrder;
            videosOrder = Properties.Settings.Default.videosOrder;
            dualScreen = Properties.Settings.Default.dualScreen;
            rows = Properties.Settings.Default.rows;
            columns = Properties.Settings.Default.columns;
            rowsSecondary = Properties.Settings.Default.rowsSecondary;
            columnsSecondary = Properties.Settings.Default.columnsSecondary;
            timerDuration = Properties.Settings.Default.timerDur;

            if (Properties.Settings.Default.directories != null)
            {
                directories = Properties.Settings.Default.directories;
            }
        }

        private void displaySettings()
        {
            txtSecIP.Text = secondIP;
            setCheckState(chkMain, mainPC);
            setCheckState(chkhalfway, startHalfway);
            setCheckState(chkRestMuted, startRestMuted);
            setCheckState(chkAllMuted, startAllMuted);
            setCheckState(chkLowVolume, startLowVolume);
            setCheckState(chkTopDock, topDock);
            setCheckState(chkTopPlayers, topPlayers);
            setCheckState(chkVideoOrder, videosOrder);
            setCheckState(chkDirOrder, dirOrder);
            setCheckState(chkDualScreen, dualScreen);

            txtVolumeLevel.Text = lowVolumeLevel.ToString();
            txtHalfwayLength.Text = halfwayLength.ToString();
            txtRow.Text = rows.ToString();
            txtColumn.Text = columns.ToString();
            txtRowSecondary.Text = rowsSecondary.ToString();
            txtColumnSecondary.Text = columnsSecondary.ToString();
            txtTimerDur.Text = timerDuration.ToString();
        }

        private void getSettings()
        {
            IPAddress test;

            if (IPAddress.TryParse(txtSecIP.Text, out test))
            {
                secondIP = txtSecIP.Text;
            }

            mainPC = checkState(chkMain);
            startHalfway = checkState(chkhalfway);
            startRestMuted = checkState(chkRestMuted);
            startAllMuted = checkState(chkAllMuted);
            startLowVolume = checkState(chkLowVolume);
            topDock = checkState(chkTopDock);
            topPlayers = checkState(chkTopPlayers);
            videosOrder = checkState(chkVideoOrder);
            dirOrder = checkState(chkDirOrder);
            dualScreen = checkState(chkDualScreen);

            double volLevel;
            int halfLength;
            int timerDurTest;

            if (double.TryParse(txtVolumeLevel.Text, out volLevel))
            {
                lowVolumeLevel = volLevel;
            }

            if (int.TryParse(txtHalfwayLength.Text, out halfLength))
            {
                halfwayLength = halfLength;
            }

            if (int.TryParse(txtTimerDur.Text, out timerDurTest))
            {
                if (timerDurTest >= 1)
                {
                    timerDuration = timerDurTest;
                }
            }

            directories.Clear();
            foreach(string dir in lstDirectories.Items)
            {
                directories.Add(dir);
            }
        }

        private void setCheckState(CheckBox chk, bool state)
        {
            if (state)
            {
                chk.CheckState = CheckState.Checked;
            }
            else
            {
                chk.CheckState = CheckState.Unchecked;
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            loadSettings();
            displaySettings();

            if (directories.Count > 0)
            {
                foreach (string dir in directories)
                {
                    lstDirectories.Items.Add(dir);
                }
            }
        }

        private void btnPasswordSet_Click(object sender, EventArgs e)
        {
            string newPassword = txtPassword.Text;

            if (password.Length > 0)
            {
                string checkPassword = Microsoft.VisualBasic.Interaction.InputBox("Enter Password Again: ", "Password", "");

                if (checkPassword == newPassword)
                {

                    checkPassword = Microsoft.VisualBasic.Interaction.InputBox("Enter Previous Password: ", "Previous Password", "");

                    if (checkPassword == password)
                    {
                        password = newPassword;
                    }
                }
            }
        }

        private KeyBinds bindsMenu;

        private void btnOpenBinds_Click(object sender, EventArgs e)
        {
            if (bindsMenu == null || !bindsMenu.Visible)
            {
                bindsMenu = new KeyBinds(this);
                bindsMenu.Show();
                btnOpenBinds.Enabled = false;
            }
        }

        public void enableBtn()
        {
            btnOpenBinds.Enabled = true;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            getSettings();
            saveSettings();
            updateForms();
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to go back to the default settings?", "Default Settings", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                defaultSettings();

                displaySettings();
            }
        }

        private void txtSecIP_TextChanged(object sender, EventArgs e)
        {
            IPAddress address;

            if (IPAddress.TryParse(txtSecIP.Text, out address))
            {
                secondIP = txtSecIP.Text;
            }
        }

        private void txtVolumeChange_TextChanged(object sender, EventArgs e)
        {
            Double checker;

            if (Double.TryParse(txtVolumeLevel.Text, out checker))
            {
                if (checker >= 0 && checker < 1)
                {
                    lowVolumeLevel = checker;
                }
                else if (checker >= 1)
                {
                    checker *= 0.01;

                    if (checker >= 0 && checker <= 1)
                    {
                        lowVolumeLevel = checker;
                    }
                    else
                    {
                        MessageBox.Show("Invalid Numer. Number must be between 0 - 100");
                    }
                }
            }
        }

        private bool checkState(CheckBox chk)
        {
            if (chk.Checked)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Grid
        private void txtRow_TextChanged(object sender, EventArgs e)
        {
            TextBox senderTxt = (TextBox)sender;

            if (senderTxt.Name == "txtRow" || senderTxt.Name == "txtColumn")
            {
                gridDataUpdate(true);
            }
            else
            {
                gridDataUpdate(false);
            }
        }

        private void gridDataUpdate(bool main)
        {
            TextBox rowTxt;
            TextBox columnTxt;

            int row;
            int column;

            if (main)
            {
                rowTxt = txtRow;
                columnTxt = txtColumn;
            }
            else
            {
                rowTxt = txtRowSecondary;
                columnTxt = txtColumnSecondary;
            }

            if (Int32.TryParse(rowTxt.Text, out row) && Int32.TryParse(columnTxt.Text, out column))
            {
                //visual
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                for (int i = 0; i < column; i++)
                {
                    dataGridView1.Columns.Add(i.ToString(), (i + 1).ToString());
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                for (int i = 0; i < row; i++)
                {
                    dataGridView1.Rows.Add(1);
                    dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
                }

                dataGridView1.Refresh();

                //setting the values
                if (main)
                {
                    rows = row;
                    columns = column;
                }
                else
                {
                    rowsSecondary = row;
                    columnsSecondary = column;
                }
            }
            else
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Refresh();
            }
        }
        #endregion

        #region Directories
        private void btnAddDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string newDir = folderBrowserDialog1.SelectedPath;
                lstDirectories.Items.Add(newDir);
                directories.Add(newDir);
            }
        }

        private void btnRmDir_Click(object sender, EventArgs e)
        {
            if (lstDirectories.SelectedIndex >= 0)
            {
                lstDirectories.Items.RemoveAt(lstDirectories.SelectedIndex);
            }
        }

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            gridDataUpdate(true);
        }

        private void tabPage6_Enter(object sender, EventArgs e)
        {
            gridDataUpdate(false);
        }
        #endregion

        #region ThumbnailCreator
        private void tabPage4_Enter(object sender, EventArgs e)
        {
            lstThumbDirs.Items.Clear();

            foreach (string dir in lstDirectories.Items)
            {
                lstThumbDirs.Items.Add(dir);
            }
        }

        private void searchVideos(string dir, List<string> videoList, bool first = false)
        {
            if (first)
            {
                string[] videos = Directory.GetFiles(dir, "*.mp4");

                foreach (string video in videos)
                {
                    videoList.Add(video);
                }
            }

            string[] childrenDirs = Directory.GetDirectories(dir);

            foreach (string childDir in childrenDirs)
            {
                string[] videos = Directory.GetFiles(childDir, "*.mp4");

                foreach (string video in videos)
                {
                    videoList.Add(video);
                }

                string[] childsChildrenDirs = Directory.GetDirectories(childDir);

                foreach (string childsChildrenDir in childsChildrenDirs)
                {
                    searchVideos(childsChildrenDir, videoList);
                }
            }
        }

        private void validateThumbnails(string videoDir, bool overwrite, bool hide)
        {
            if (checkFFMPEG())
            {
                new Thread(() =>
                {
                    List<string> videos = new List<string>();

                    searchVideos(videoDir, videos, true);

                    foreach (string video in videos)
                    {
                        thumbnailCreator(video, videoDir, overwrite, hide);
                    }
                }).Start();
            }
        }

        private void btnCreateThumbnails_Click(object sender, EventArgs e)
        {
            foreach (string dir in lstThumbDirs.SelectedItems)
            {
                validateThumbnails(dir, checkState(chkThumbnailsOverwrite), checkState(chkThumbnailsHide));
            }
        }

        private void btnOtherThumbnails_Click(object sender, EventArgs e)
        {
            string dir = Microsoft.VisualBasic.Interaction.InputBox("Enter Video Directory: ", "Video Directory", "");

            validateThumbnails(dir, checkState(chkThumbnailsOverwrite), checkState(chkThumbnailsHide));
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            foreach (string dir in lstThumbDirs.Items)
            {
                validateThumbnails(dir, checkState(chkThumbnailsOverwrite), checkState(chkThumbnailsHide));
            }
        }

        int instanceNum = 0;

        private void thumbnailCreator(string videoPath, string startingDir, bool overwrite, bool hide)
        {
            if(!Directory.Exists(startingDir + "\\Thumbnails\\"))
            {
                Directory.CreateDirectory(startingDir + "\\Thumbnails\\");
            }

            FileInfo fileInfo = new FileInfo(videoPath);
            string thumbnailPath = startingDir + "\\Thumbnails\\" + fileInfo.Name.Replace(".mp4", ".jpg");

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Random rng = new Random();

            int minute = rng.Next(1, 11);
            int second = rng.Next(0, 60);

            string command;
            if (overwrite)
            {
                command = "/C ffmpeg -ss " + minute.ToString() + ":" + second.ToString("00") + " -i \"" + videoPath + "\" -vframes 1 -q:v 2 \"" + thumbnailPath + "\"";
            }
            else
            {
                command = "/C ffmpeg -ss " + minute.ToString() + ":" + second.ToString("00") + " -n -i \"" + videoPath + "\" -vframes 1 -q:v 2 \"" + thumbnailPath + "\"";
            }

            cmd.StartInfo.Arguments = command;
            cmd.Start();

            if (instanceNum < 11 && !hide)
            {
                instanceNum++;
            }
            else
            {
                cmd.WaitForExit();
                instanceNum = 0;
            }

            try
            {
                if (hide)
                {
                    cmd.WaitForExit();
                    File.SetAttributes(thumbnailPath, File.GetAttributes(thumbnailPath) | FileAttributes.Hidden);
                }
            }
            catch { }
        }
        #region checkFFMPEG
        private bool checkFFMPEG()
        {
            if (!ExistsOnPath("ffmpeg.exe"))
            {
                DialogResult answer = MessageBox.Show("Error ffmpeg is not installed on this computer. Whould you like to install it?", "FFMPEG is missing", MessageBoxButtons.YesNo);

                if (answer == DialogResult.Yes)
                {
                    ffmpegInstaller.installFFMPEG();
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        private static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }
        #endregion

        #endregion
    }
}
