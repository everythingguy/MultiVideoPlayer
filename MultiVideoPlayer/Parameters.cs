using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiVideoPlayer
{
    partial class Dock
    {
        Dictionary<string, string> parameters;

        private void readParameterSettings()
        {
            parameters = new Dictionary<string, string>();

            //FINISH
        }

        private void handleParameters(string[] parameters)
        {
            if(parameters == null)
            {
                readParameterSettings();
            }

            if(parameters != null)
            {
                //FINISH
            }
        }

        private void search(string query)
        {
            flowLayoutPanel1.SuspendLayout();

            List<string[]> specificVideos = new List<string[]>();

            foreach (string video in videoPaths)
            {
                string videoName = Path.GetFileNameWithoutExtension(video);
                string thumbnailPath = null;

                foreach(string dir in Properties.Settings.Default.directories)
                {
                    if(video.Contains(dir))
                    {
                        thumbnailPath = dir + "\\Thumbnails\\" + videoName + ".jpg";

                        if(!File.Exists(thumbnailPath))
                        {
                            thumbnailPath = null;
                        }
                        break;
                    }
                }

                if(thumbnailPath == null)
                {
                    //User is missing thumbnails ask them if they want to create them ONCE not a million times
                    continue;
                }

                string[] split = query.Split(' ');

                if (split.Length > 1)
                {
                    bool created = false;

                    foreach (string word in split)
                    {
                        if (videoName.Contains(word))
                        {
                            Console.WriteLine(thumbnailPath);

                            if (!removeBox(thumbnailPath))
                            {
                                flowLayoutPanel1.Controls.RemoveAt(flowLayoutPanel1.Controls.Count - 1);
                            }

                            createBox(thumbnailPath, video, true);
                            created = true;

                            break;
                        }
                    }

                    if(created)
                    {
                        continue;
                    }
                }

                if (videoName.Contains(query))
                {
                    string[] paths = { video, thumbnailPath };

                    specificVideos.Add(paths);

                    continue;
                }
            }

            foreach(string[] paths in specificVideos)
            {
                if (!removeBox(paths[1]))
                {
                    flowLayoutPanel1.Controls.RemoveAt(flowLayoutPanel1.Controls.Count - 1);
                }

                createBox(paths[1], paths[0], true);
            }

            flowLayoutPanel1.ResumeLayout();
        }
    }
}
