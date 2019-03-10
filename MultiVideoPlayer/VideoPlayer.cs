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
using System.Xml;
using System.Xml.Linq;

namespace MultiVideoPlayer
{
    public partial class VideoPlayer : Form
    {
        public int screenNum { get; }
        private int inRow;
        private int inColumn;
        private string volume;
        private bool muted;
        private bool startHalfway;
        public string videoPath { get; set; }
        private string path;
        private bool pauseBool = false;
        public bool replacement = true;
        private int startTime = 0;
        public Grid parent { get; }

        protected override CreateParams CreateParams
        {
            get
            {
                //Hide form from alt tab
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }


        public VideoPlayer(int screenNumSetter, int w, int h, Point XY, int row, int column, string volumeSetter, bool mutedSetter, bool halfwaySetter, string filenameSetter, Grid parentSetter)
        {
            InitializeComponent();

            screenNum = screenNumSetter;
            inRow = row;
            inColumn = column;
            volume = volumeSetter;
            muted = mutedSetter;
            startHalfway = halfwaySetter;
            videoPath = filenameSetter;
            parent = parentSetter;

            this.Width = w;
            this.Height = h;

            this.Location = XY;

            this.Show();
        }

        private void VideoPlayer_Load(object sender, EventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;

            webBrowser1.ObjectForScripting = new VideoPlayerListener(this, getDockForm());

            startVideo();

            refresher.Interval = 1000;
            refresher.Tick += (senderT, eT) => videoPos();
            refresher.Start();
        }

        private void startVideo()
        {
            string htmlFilename = createHTML();
            loadHTML(htmlFilename);
        }

        private string createHTML()
        {
            Random VideoNum = new Random();
            int randomVideoNum = VideoNum.Next(1, 10000);
            string HTMLPageName = String.Format("HTMLPage{0}.html", randomVideoNum.ToString());
            int halfwayLength = Properties.Settings.Default.halfwayLength * 60;

            string curDir = Directory.GetCurrentDirectory();
            path = string.Format("{0}/{1}", curDir, HTMLPageName);
            string mute = "muted";
            string classname = "muted";
            string time = "0";

            bool isNumeric = decimal.TryParse(volume, out decimal n);

            if (isNumeric == false)
            {
                volume = "0.10";
            }

            string halfway = "this.duration / 2";
            if (startHalfway == false)
            {
                halfway = "0";
            }

            if(startTime != 0)
            {
                halfway = startTime.ToString();
            }

            if (muted == false)
            {
                mute = "class";
                classname = "unmuted";
            }

            string javascript = "document.addEventListener('keydown', function(e) { e.preventDefault(); }); var video = document.getElementById('player'); video.volume = " + volume + "; video.addEventListener('loadedmetadata', function() { if(this.duration > " + halfwayLength + ") { this.currentTime = " + halfway + " + " + time + "; } else if (" + halfway + " > 0) { this.currentTime = this.duration / 4 + " + time + " } }, false); function videoPos() { external.log(\"Current Time: \" + video.currentTime); } function forward() { video.currentTime = video.currentTime + 60; } function rewind() { video.currentTime = video.currentTime - 60; } function videoEnd() { external.log('Video Over'); } function volUp() { if(video.volume < 1) { video.muted = false; if(video.volume + .1 <= 1) { video.volume = video.volume + .1; } else { video.volume = 1; } } } function volDown() { video.muted = false; if(video.volume >= .1) { video.volume = video.volume - .1; } else { video.volume = 0; } } function mute() { video.muted = !video.muted; } function pause() { if(video.paused || video.ended) { video.play(); } else { video.pause(); } } video.onvolumechange = function() { if(video.volume == 0) { external.log('muted'); } else if(video.muted == false) { external.log('unmuted'); } };";

            var xDocument = new XDocument(
                new XDocumentType("html", null, null, null),
                new XElement("html", new XAttribute("lang", "en"),
                new XElement("head",
                new XElement("meta", new XAttribute("charset", "uft-8")),
                new XElement("title", "Video"),
                new XElement("style",

      @"html {
			margin: 0px;
			height: 100%;
			width: 100%;
		}

		body {
			margin: 0%;
			min-height: 100%;
			width: 100%;
		}

		video {
			position: absolute;
			min-width: 100%;
			max-width: 100%;
			min-height: 100%;
			max-height: 100%
		}")
                ),
                new XElement("body",
                new XElement("video", new XAttribute("id", "player"), new XAttribute("src", videoPath), new XAttribute("controls", "controls"), new XAttribute("tabindex", "0"), new XAttribute("autoplay", "autoplay"), new XAttribute(mute, classname), new XAttribute("onended", "videoEnd();")),
                new XElement("script", "//", new XCData(Environment.NewLine + javascript + "//")))
                ));

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                IndentChars = "\t"
            };
            using (var writer = XmlWriter.Create(path, settings))
            {
                xDocument.WriteTo(writer);
            }

            return HTMLPageName;
        }

        private void loadHTML(string HTMLPageName)
        {
            string curDir = Directory.GetCurrentDirectory();
            string url = string.Format("file:///{0}/{1}", curDir, HTMLPageName);
            url = url.Replace(" ", "%20");
            url = url.Replace("\\", "/");
            webBrowser1.Navigate(url);
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

        Keys lastKey = new Keys();

        private void webBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //to fix double key problem
            if (lastKey != e.KeyCode)
            {
                Dock main = getDockForm();

                KeyEventArgs args = new KeyEventArgs(e.KeyCode);

                //send key to dock where key listeners is
                main.Dock_KeyDown(sender, args);

                lastKey = e.KeyCode;
                //focus dock so the user can press the same key twice in a row
                main.Focus();
            }
        }

        public void videoChange(string newVideoPath, int startPos)
        {
            videoPath = newVideoPath;
            startTime = startPos;

            startVideo();
        }

        public int currentPos = -1;
        Timer refresher = new Timer();
        private void videoPos()
        {
            try
            {
                webBrowser1.Document.InvokeScript("videoPos");
            }
            catch { }
        }

        private void VideoPlayer_Activated(object sender, EventArgs e)
        {
            //if the user selects the player clear lastKey so the user can use that key
            lastKey = new Keys();
        }

        public void toggleHide()
        {
            if(this.Visible)
            {
                this.hide();
            }
            else
            {
                this.Show();
            }
        }

        public void hide()
        {
            if(this.Visible)
            {
                this.Hide();
            }
        }

        public void toggleMute()
        {
            if (webBrowser1.Document != null)
            {
                webBrowser1.Document.InvokeScript("mute");
                muted = !muted;
            }
        }

        public void mute()
        {
            if(!muted)
            {
                toggleMute();
            }
        }

        public void muteIS(bool state)
        {
            muted = state;
        }

        public void fastForward()
        {
            if (webBrowser1.Document != null)
            {
                webBrowser1.Document.InvokeScript("forward");
            }
        }

        public void rewind()
        {
            if (webBrowser1.Document != null)
            {
                webBrowser1.Document.InvokeScript("rewind");
            }
        }

        public void volumeUp()
        {
            if (webBrowser1.Document != null)
            {
                webBrowser1.Document.InvokeScript("volUp");
            }
        }

        public void volumeDown()
        {
            if (webBrowser1.Document != null)
            {
                webBrowser1.Document.InvokeScript("volDown");
            }
        }

        public void togglePause()
        {
            if (webBrowser1.Document != null)
            {
                webBrowser1.Document.InvokeScript("pause");
            }

            pauseBool = !pauseBool;
        }

        public void pause()
        {
            if(!pauseBool)
            {
                togglePause();
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }
    }
}
