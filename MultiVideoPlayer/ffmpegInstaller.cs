using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiVideoPlayer
{
    static class ffmpegInstaller
    {
        public static void installFFMPEG()
        {
            string downloadLink;
            string installPath;

            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            MessageBox.Show("Select where you would like to install ffmpeg");
            DialogResult result = folderDialog.ShowDialog();

            if (result == DialogResult.OK && Directory.Exists(folderDialog.SelectedPath))
            {
                installPath = folderDialog.SelectedPath;
            }
            else
            {
                return;
            }

            WebBrowser browser = new WebBrowser();
            WebClient downloader = new WebClient();
            browser.ScriptErrorsSuppressed = true;

            if (Environment.Is64BitOperatingSystem)
            {
                downloadLink = "https://ffmpeg.zeranoe.com/builds/win64/static/";
            }
            else
            {
                downloadLink = "https://ffmpeg.zeranoe.com/builds/win32/static/";
            }

            browser.DocumentCompleted += (sender, e) =>
            {
                HtmlElement newestRow = browser.Document.GetElementsByTagName("tr")[2];
                HtmlElement downloadLinkLocation = newestRow.GetElementsByTagName("a")[0];
                downloadLink += downloadLinkLocation.InnerHtml;
                downloader.DownloadFile(downloadLink, "download.zip");
                System.IO.Compression.ZipFile.ExtractToDirectory("download.zip", Environment.CurrentDirectory);
                try
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(downloadLinkLocation.InnerHtml.Replace(".zip", ""), installPath + "\\ffmpeg");
                }
                catch { }
                File.Delete("download.zip");
                //add installation to path
                string pathvar = System.Environment.GetEnvironmentVariable("PATH");
                var value = pathvar + ";" + installPath + "\\ffmpeg\\bin";
                var target = EnvironmentVariableTarget.Machine;
                System.Environment.SetEnvironmentVariable("PATH", value, target);
            };

            browser.Navigate(downloadLink);
        }
    }
}
