using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiVideoPlayer
{
    public partial class Dock : Form
    {
        int screenNum = 1;

        Grid main;
        Grid secondary;

        List<string[]> thumbnails = new List<string[]>();
        List<string> videoPaths = new List<string>();
        List<string> queue = new List<string>();

        List<string> thumbnailBuffer = new List<string>();

        public Dock(string[] parameters = null)
        {
            InitializeComponent();

            flowLayoutPanel1.MouseWheel += flowLayoutPanel1_MouseWheel;

            main = new Grid(Properties.Settings.Default.rows, Properties.Settings.Default.columns, false);

            Rectangle screenArea;
            int y;
            if (Screen.AllScreens.Length > 1)
            {
                if(Properties.Settings.Default.dualScreen)
                {
                    secondary = new Grid(Properties.Settings.Default.rowsSecondary, Properties.Settings.Default.columnsSecondary, true);
                }

                screenArea = Screen.AllScreens[1].WorkingArea;
                y = screenArea.Y;
            }
            else
            {
                screenArea = Screen.PrimaryScreen.WorkingArea;
                y = 0;
            }

            this.Width = screenArea.Width / 4;
            this.Height = screenArea.Height + 40;

            this.Location = new Point(screenArea.Right - this.Width, y);

            this.TopMost = Properties.Settings.Default.topDock;

            handleParameters(parameters);
        }

        public void updateSettings()
        {
            if (main.gridRows != Properties.Settings.Default.rows || main.gridColumns != Properties.Settings.Default.columns)
            {
                main.closeAllPlayers();
                main = new Grid(Properties.Settings.Default.rows, Properties.Settings.Default.columns, false);
            }

            if (Properties.Settings.Default.dualScreen && secondary == null)
            {
                secondary = new Grid(Properties.Settings.Default.rowsSecondary, Properties.Settings.Default.columnsSecondary, true);
            }

            else if (Properties.Settings.Default.dualScreen && secondary.gridRows != Properties.Settings.Default.rowsSecondary || main.gridColumns != Properties.Settings.Default.columnsSecondary)
            {
                secondary.closeAllPlayers();
                secondary = new Grid(Properties.Settings.Default.rowsSecondary, Properties.Settings.Default.columnsSecondary, true);
            }

            else if (!Properties.Settings.Default.dualScreen)
            {
                if (secondary != null)
                {
                    secondary.closeAllPlayers();
                    secondary = null;
                }
            }

            foreach(VideoPlayer player in main.Players)
            {
                if(player != null && playerExists(player.screenNum))
                {
                    player.TopMost = Properties.Settings.Default.topPlayers;
                }
            }

            if (secondary != null)
            {
                foreach (VideoPlayer player in secondary.Players)
                {
                    if (player != null && playerExists(player.screenNum))
                    {
                        player.TopMost = Properties.Settings.Default.topPlayers;
                    }
                }
            }
        }

        private void Dock_Load(object sender, EventArgs e)
        {
            refresh();
        }

        Timer replacementTimer;

        private void refresh()
        {
            populateLst();
            readKeyFile();

            replacementTimer = new Timer();
            replacementTimer.Interval = Properties.Settings.Default.timerDur * 60000;
            replacementTimer.Tick += (sender, e) => autoReplaceAllPlayers();
        }

        private bool checkPopulateErrors()
        {
            List<string> dirs = Properties.Settings.Default.directories;

            if(dirs == null)
            {
                dirs = new List<string>();
            }

            bool hasFiles = false;
            int i = 0;

            if (dirs.Count > 0)
            {
                while(i < dirs.Count && !hasFiles)
                {
                    if(Directory.Exists(dirs[i]))
                    {
                        if(Directory.GetFiles(dirs[i]).Length > 0)
                        {
                            hasFiles = true;
                        }
                    }
                    i++;
                }
            }

            return hasFiles;
        }
        //search dir and all children dirs for videos until there are no children dirs left
        private void searchVideos(string dir, List<string> videoList, bool first = false)
        {
            if(first)
            {
                string[] videos = Directory.GetFiles(dir, "*.mp4");

                foreach(string video in videos)
                {
                    videoList.Add(video);
                }
            }

            string[] childrenDirs = Directory.GetDirectories(dir);

            foreach(string childDir in childrenDirs)
            {
                string[] videos = Directory.GetFiles(childDir, "*.mp4");

                foreach(string video in videos)
                {
                    videoList.Add(video);
                }

                string[] childsChildrenDirs = Directory.GetDirectories(childDir);

                foreach(string childsChildrenDir in childsChildrenDirs)
                {
                    searchVideos(childsChildrenDir, videoList);
                }
            }
        }

        public void populateLst()
        {
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                flowLayoutPanel1.Controls.Clear();
            }

            if (checkPopulateErrors())
            {
                screenNum = 1;
                thumbnails.Clear();
                videoPaths.Clear();
                queue.Clear();

                List<string> dirs = Properties.Settings.Default.directories;

                foreach (string dir in dirs)
                {
                    if (Directory.Exists(dir + "\\Thumbnails"))
                    {
                        string[] dirThumbnails = Directory.GetFiles(dir + "\\Thumbnails", "*.jpg");

                        if(dirThumbnails.Length == 0)
                        {
                            //ask user if they want to create thumbnails
                        }

                        searchVideos(dir, videoPaths, true);

                        thumbnails.Add(dirThumbnails);
                    }
                    else
                    {
                        //ask user if they want to create thumbnails
                    }
                }

                if (!Properties.Settings.Default.videosOrder)
                {
                    List<string[]> newList = new List<string[]>();

                    foreach (string[] dirThumbnail in thumbnails)
                    {
                        List<string> ranArray = new List<string>();
                        ranArray.AddRange(dirThumbnail);
                        ranArray.Shuffle();
                        newList.Add(ranArray.ToArray());
                    }

                    thumbnails = newList;
                }

                if (!Properties.Settings.Default.dirOrder)
                {
                    //combine all dirs into "one"
                    List<string[]> temp = new List<string[]>();
                    List<string> temp2 = new List<string>();

                    foreach(string[] dir in thumbnails)
                    {
                        foreach(string thumbnail in dir)
                        {
                            temp2.Add(thumbnail);
                        }
                    }

                    temp2.Shuffle();
                    temp.Add(temp2.ToArray());

                    thumbnails = temp;
                }

                flowLayoutPanel1.SuspendLayout();

                int count = 0;

                foreach (string[] dirThumbnail in thumbnails)
                {
                    foreach (string thumbnail in dirThumbnail)
                    {
                        string videoPath = null;
                        string thumbnailName = Path.GetFileNameWithoutExtension(thumbnail);

                        foreach (string videopath in videoPaths)
                        {
                            string videoName = Path.GetFileNameWithoutExtension(videopath);

                            if(videoName == thumbnailName)
                            {
                                videoPath = videopath;
                                break;
                            }
                        }

                        if (videoPath != null && File.Exists(videoPath))
                        {
                            if (count <= 100)
                            {

                                queue.Add(videoPath);

                                createBox(thumbnail, videoPath);
                                count++;

                            }
                            else
                            {
                                thumbnailBuffer.Add(thumbnail);
                            }
                        }
                    }
                }

                flowLayoutPanel1.ResumeLayout();
            }
        }

        private void createBox(string thumbnailPath, string videoPath, bool addtoTop = false)
        {
            PictureBox box = new PictureBox();
            box.ImageLocation = thumbnailPath;
            box.Size = new Size(450, 250);
            box.SizeMode = PictureBoxSizeMode.StretchImage;
            box.Margin = new Padding(10, 25, 0, 25);
            box.Click += (sender, e) => box_Click(sender, e, videoPath);
            flowLayoutPanel1.Controls.Add(box);

            if(addtoTop)
            {
                flowLayoutPanel1.Controls.SetChildIndex(box, 0);
            }
        }

        private bool removeBox(string thumbnailPath)
        {
            foreach (PictureBox box in flowLayoutPanel1.Controls.OfType<PictureBox>())
            {
                if (box.ImageLocation == thumbnailPath)
                {
                    flowLayoutPanel1.Controls.Remove(box);
                    return true;
                }
            }

            return false;
        }

        public void nextScreenNum()
        {
            if (screenNum < main.totalPlayers)
            {
                screenNum++;
            }
            else if(secondary != null && (screenNum - main.totalPlayers) < secondary.totalPlayers)
            {
                screenNum++;
            }
            else
            {
                screenNum = 1;
            }
        }

        private void overlayOpen(string pic, int x = 0, int y = 0)
        {
            overlay Overlay = new overlay(pic, x, y);
            Overlay.Show();

            System.Timers.Timer close = new System.Timers.Timer();
            close.Interval = 1200;
            close.Elapsed += (senderT, eT) => this.Invoke((MethodInvoker)delegate { Overlay.Close(); close.Stop(); close.Dispose(); });
            close.Start();
        }

        private void autoReplaceAllPlayers()
        {
            for(int i = 0; i < main.totalPlayers; i++)
            {
                if (queue.Count > 0)
                {
                    main.placePlayer(i + 1, queue[0], true);
                    queue.RemoveAt(0);
                }
            }

            if(secondary != null)
            {
                for(int i = 0; i < secondary.totalPlayers; i++)
                {
                    if(queue.Count > 0)
                    {
                        secondary.placePlayer(i + 1, queue[0], true);
                        queue.RemoveAt(0);
                    }
                }
            }
        }

        public void replacer(int screenNumber, Grid grid)
        {
            if (queue.Count > 0)
            {
                grid.placePlayer(screenNumber, queue[0]);
                queue.RemoveAt(0);
            }
        }

        private bool formExiting = false;
        private void Dock_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!formExiting)
            {
                formExiting = true;

                cleanupHTMLFiles();

                Application.Exit();
            }
        }

        private void cleanupHTMLFiles()
        {
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                if (Path.GetFileName(file).StartsWith("HTMLPage") && Path.GetFileName(file).EndsWith(".html"))
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        private void activateBuffer()
        {
            if (((flowLayoutPanel1.AutoScrollPosition.Y * -1) + 1079) > flowLayoutPanel1.VerticalScroll.Maximum - 2000 && thumbnailBuffer.Count > 0)
            {
                flowLayoutPanel1.SuspendLayout();

                for (int i = 0; i <= 20; i++)
                {
                    /*if (flowLayoutPanel1.Controls.Count > 0)
                    {
                        flowLayoutPanel1.Controls.RemoveAt(0);
                    }*/

                    string videoPath = null;
                    string thumbnailName = Path.GetFileNameWithoutExtension(thumbnailBuffer[0]);

                    foreach (string videopath in videoPaths)
                    {
                        string videoName = Path.GetFileNameWithoutExtension(videopath);

                        if (videoName == thumbnailName)
                        {
                            videoPath = videopath;
                            break;
                        }
                    }

                    if (File.Exists(videoPath))
                    {
                        queue.Add(videoPath);

                        createBox(thumbnailBuffer[0], videoPath);
                    }

                    thumbnailBuffer.RemoveAt(0);

                }

                flowLayoutPanel1.ResumeLayout();
            }
        }

        private void box_Click(object sender, EventArgs e, string videoPath)
        {
            boxPlacePlayer(videoPath);
        }

        private void boxPlacePlayer(string videoPath, bool next = true)
        {
            if (screenNum <= main.totalPlayers)
            {
                main.placePlayer(screenNum, videoPath);
            }
            else if (secondary != null)
            {
                secondary.placePlayer(screenNum - main.totalPlayers, videoPath);
            }

            if (next)
            {
                nextScreenNum();
            }
        }

        private void flowLayoutPanel1_MouseWheel(object sender, MouseEventArgs e)
        {
            activateBuffer();
        }

        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {
            activateBuffer();
        }
    }
}
