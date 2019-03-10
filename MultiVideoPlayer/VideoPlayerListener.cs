using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MultiVideoPlayer
{
    [ComVisible(true)]
    public class VideoPlayerListener
    {
        public VideoPlayerListener(VideoPlayer setPlayer, Dock setHost)
        {
            videoplayer = setPlayer;
            host = setHost;
        }

        private VideoPlayer videoplayer;

        private Dock host;

        public void log(string message)
        {
            if (message.Contains("Video Over"))
            {
                host.replacer(videoplayer.screenNum, videoplayer.parent);
                
                return;
            }

            if (message == "muted")
            {
                videoplayer.muteIS(true);
                return;
            }

            if (message == "unmuted")
            {
                videoplayer.muteIS(false);
                return;
            }

            if (message.Contains("Current Time:"))
            {
                try
                {
                    int pos = 0;
                    string convertThis = message.Replace("Current Time: ", "");
                    int dec = convertThis.IndexOf(".");
                    convertThis = convertThis.Substring(0, dec);

                    if (int.TryParse(convertThis, out pos))
                    {
                        videoplayer.currentPos = pos;
                    }
                    return;
                }
                catch
                {
                    return;
                }
            }
        }

    }
}
