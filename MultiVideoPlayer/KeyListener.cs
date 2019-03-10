using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiVideoPlayer
{
    partial class Dock
    {
        private Dictionary<string, string> keyBinds = new Dictionary<string, string>();

        public Dictionary<string, string> getKeyBinds()
        {
            return new Dictionary<string, string>(keyBinds);
        }

        public void setKeyBinds(Dictionary<string, string> newDictionary)
        {
            keyBinds = new Dictionary<string, string>(newDictionary);

            saveNewKeyFile();
        }

        private void createKeyFile()
        {
            File.Copy("defaultKeyBinds.csv", "userKeyBinds.csv");
        }

        public void readKeyFile()
        {
            if(!File.Exists("userKeyBinds.csv"))
            {
                createKeyFile();
            }

            string[] lines = File.ReadAllLines("userKeyBinds.csv");

            foreach(string line in lines)
            {
                string[] split = line.Split(',');
                keyBinds[split[0].Trim()] = split[1].Trim();
            }
        }

        private void saveNewKeyFile()
        {
            using (var stream = File.Create("userKeyBinds.csv")) { }
                
            StreamWriter wr = new StreamWriter("userKeyBinds.csv");

            foreach(KeyValuePair<string, string> pair in keyBinds)
            {
                wr.WriteLine(pair.Key + ", " + pair.Value);
            }

            wr.Flush();
            wr.Dispose();
        }

        private void changeKeyFile(string key, string value)
        {
            keyBinds[key] = value;

            string[] lines = File.ReadAllLines("userKeyBinds.csv");

            for(int i = 0; i < lines.Length; i++)
            {
                string[] split = lines[i].Split(',');

                if(split[0] == key)
                {
                    lines[i] = key + ", " + value;
                    File.WriteAllLines("userKeyBinds.csv", lines);
                    return;
                }
            }
        }

        public void Dock_KeyDown(object sender, KeyEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Alt) && ModifierKeys.HasFlag(Keys.Shift))
            {
                //Ctrl, Shift and Alt pressed
                if(e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
                {
                    keyEvents(e, "ctrl + alt + shift");
                }
            }

            else
            {
                if (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Alt))
                {
                    //Ctrl and Alt pressed
                    if(e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.Menu)
                    {
                        keyEvents(e, "ctrl + alt");
                    }
                }

                else if (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Shift))
                {
                    //Ctrl and Shift pressed
                    if(e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey)
                    {
                        keyEvents(e, "ctrl + shift");
                    }
                }

                else if (ModifierKeys.HasFlag(Keys.Shift) && ModifierKeys.HasFlag(Keys.Alt))
                {
                    //Alt and Shift pressed
                    if(e.KeyCode != Keys.Menu && e.KeyCode != Keys.ShiftKey)
                    {
                        keyEvents(e, "alt + shift");
                    }
                }

                else
                {
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        //Ctrl is pressed
                        if (e.KeyCode != Keys.ControlKey)
                        {
                            keyEvents(e, "ctrl");
                        }
                    }

                    else if (ModifierKeys.HasFlag(Keys.Alt))
                    {
                        //Alt is pressed
                        if (e.KeyCode != Keys.Menu)
                        {
                            keyEvents(e, "alt");
                        }
                    }

                    else if (ModifierKeys.HasFlag(Keys.Shift))
                    {
                        //Shift is pressed
                        if (e.KeyCode != Keys.ShiftKey)
                        {
                            keyEvents(e, "shift");
                        }
                    }

                    else
                    {
                        //Single Key
                        keyEvents(e);
                    }
                }
            }
        }

        private bool hasInteger(string input)
        {
            return input.Any(c => char.IsDigit(c));
        }

        private bool bindMatchKey(string combo, string key)
        {
            if(keyBinds.ContainsKey(key) && combo.ToLower() == keyBinds[key].ToLower())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool playerExists(int testNum = 0)
        {
            if(secondary != null && testNum > (main.totalPlayers + secondary.totalPlayers))
            {
                return false;
            }
            else if(secondary == null && testNum > main.totalPlayers)
            {
                return false;
            }

            if (testNum <= 0)
            {
                testNum = screenNum;
            }

            if(testNum <= main.totalPlayers 
                && main.Players[testNum - 1] != null 
                && !main.Players[testNum - 1].IsDisposed)
            {
                return true;
            }
            else if(secondary != null
                && (testNum - main.totalPlayers) <= secondary.totalPlayers 
                && secondary.Players[testNum - main.totalPlayers - 1] != null 
                && !secondary.Players[testNum - main.totalPlayers - 1].IsDisposed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        Settings settings;
        bool swapToggle = false;
        int swapFirst = 0;
        int swapLast = 0;

        private void keyEvents(KeyEventArgs e, string combo = "")
        {
            string keyCombo;
            string keyCode = e.KeyCode.ToString();

            //for number keys, gets rid of the D in front of the number because D isn't in the key of the dictionary
            if (keyCode.Length == 2 && keyCode.Substring(0, 1) == "D" && hasInteger(keyCode))
            {
                keyCode = keyCode.Substring(1, 1);
            }

            if (combo.Length > 0)
            {
                keyCombo = combo + " + " + keyCode;
            }
            else
            {
                keyCombo = keyCode;
            }

            //swap
            if (bindMatchKey(keyCombo, "swap") || swapToggle)
            {
                if (swapToggle)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        //player
                        if (bindMatchKey(keyCombo, "player" + i))
                        {
                            screenNum = i;
                        }
                    }

                    if (swapFirst == 0)
                    {
                        swapFirst = screenNum;
                    }
                    else
                    {
                        swapLast = screenNum;

                        VideoPlayer player1;
                        VideoPlayer player2;

                        if(swapFirst <= main.totalPlayers)
                        {
                            player1 = main.Players[swapFirst - 1];
                        }
                        else
                        {
                            player1 = secondary.Players[swapFirst - main.totalPlayers - 1];
                        }

                        if (swapLast <= main.totalPlayers)
                        {
                            player2 = main.Players[swapLast - 1];
                        }
                        else
                        {
                            player2 = secondary.Players[swapLast - main.totalPlayers - 1];
                        }

                        string tempPath = player1.videoPath;
                        int tempStartPos = player1.currentPos;
                        player1.videoChange(player2.videoPath, player2.currentPos);
                        player2.videoChange(tempPath, tempStartPos);

                        swapToggle = false;
                        swapFirst = 0;
                        swapLast = 0;
                    }
                }
                else
                {
                    swapToggle = true;
                }
                return;
            }

            //exit
            if (bindMatchKey(keyCombo, "exit"))
            {
                Application.Exit();
                return;
            }

            //refresh
            if (bindMatchKey(keyCombo, "refresh"))
            {
                refresh();
                return;
            }

            //search
            if(bindMatchKey(keyCombo, "search"))
            {
                string result = Microsoft.VisualBasic.Interaction.InputBox("Enter Query: ", "Search");

                if(result.Length > 0)
                {
                    search(result);
                }

                return;
            }

            //openSettings
            if (bindMatchKey(keyCombo, "openSettings"))
            {
                if (settings == null || !settings.Visible)
                {
                    settings = new Settings();
                    settings.Show();
                } else
                {
                    settings.Focus();
                }
                return;
            }

            //mute
            if (bindMatchKey(keyCombo, "mute"))
            {
                if (playerExists())
                {
                    if (screenNum <= main.totalPlayers)
                    {
                        main.Players[screenNum - 1].toggleMute();
                    }
                    else if(secondary != null)
                    {
                        secondary.Players[screenNum - main.totalPlayers - 1].toggleMute();
                    }
                }

                return;
            }

            //muteAll
            if (bindMatchKey(keyCombo, "muteAll"))
            {
                for(int i = 1; i <= main.totalPlayers; i++)
                {
                    if (playerExists(i))
                    {
                        main.Players[i - 1].mute();
                    }
                }

                if (secondary != null)
                {
                    for (int i = 1; i <= secondary.totalPlayers; i++)
                    {
                        if(playerExists(i + main.totalPlayers))
                        {
                            secondary.Players[i - 1].mute();
                        }
                    }
                }

                return;
            }

            //volumeUp
            if (bindMatchKey(keyCombo, "volumeUp"))
            {
                if(playerExists())
                {
                    if (screenNum <= main.totalPlayers)
                    {
                        main.Players[screenNum - 1].volumeUp();
                    }
                    else if (secondary != null)
                    {
                        secondary.Players[screenNum - main.totalPlayers - 1].volumeUp();
                    }
                }

                return;
            }

            //volumeDown
            if (bindMatchKey(keyCombo, "volumeDown"))
            {
                if (playerExists())
                {
                    if (screenNum <= main.totalPlayers)
                    {
                        main.Players[screenNum - 1].volumeDown();
                    }
                    else if (secondary != null)
                    {
                        secondary.Players[screenNum - main.totalPlayers - 1].volumeDown();
                    }
                }
                return;
            }

            //fastFroward
            if (bindMatchKey(keyCombo, "fastFroward"))
            {
                if (playerExists())
                {
                    if (screenNum <= main.totalPlayers)
                    {
                        main.Players[screenNum - 1].fastForward();
                    }
                    else if (secondary != null)
                    {
                        secondary.Players[screenNum - main.totalPlayers - 1].fastForward();
                    }
                }
                return;
            }

            //rewind
            if (bindMatchKey(keyCombo, "rewind"))
            {
                if (playerExists())
                {
                    if (screenNum <= main.totalPlayers)
                    {
                        main.Players[screenNum - 1].rewind();
                    }
                    else if (secondary != null)
                    {
                        secondary.Players[screenNum - main.totalPlayers - 1].rewind();
                    }
                }
                return;
            }

            //replaceStay
            if (bindMatchKey(keyCombo, "replaceStay"))
            {
                string nextVideo = queue[0];
                queue.RemoveAt(0);

                boxPlacePlayer(nextVideo, false);
                return;
            }

            //replaceNext
            if (bindMatchKey(keyCombo, "replaceNext"))
            {
                string nextVideo = queue[0];
                queue.RemoveAt(0);

                boxPlacePlayer(nextVideo);
                return;
            }

            //replaceToggle
            if (bindMatchKey(keyCombo, "replaceToggle"))
            {
                if(playerExists() && replacementTimer != null)
                {
                    Grid grid;
                    int screenNumber;

                    if (screenNum <= main.totalPlayers)
                    {
                        grid = main;
                        screenNumber = screenNum;
                    }
                    else
                    {
                        grid = secondary;
                        screenNumber = screenNum - main.totalPlayers;
                    }

                    grid.Players[screenNumber - 1].replacement = !grid.Players[screenNumber - 1].replacement;

                    //top left point of player
                    Point location = grid.Players[screenNumber - 1].Location;

                    //middle top of player
                    int x = grid.Players[screenNumber - 1].Width / 2;
                    x += location.X;

                    int y = grid.Players[screenNumber - 1].Height / 2;
                    y += location.Y;

                    //center by subtracting half of the overlay's width and height
                    x -= 279;
                    y -= 254;

                    if (grid.Players[screenNumber - 1].replacement)
                    {
                        overlayOpen("Images/clock-on.png", x, y);
                    }
                    else
                    {
                        overlayOpen("Images/clock-off.png", x, y);
                    }
                }

                return;
            }
            
            //hide
            if (bindMatchKey(keyCombo, "hide"))
            {
                if (playerExists())
                {
                    if (screenNum <= main.totalPlayers)
                    {
                        main.Players[screenNum - 1].toggleHide();
                    }
                    else if (secondary != null)
                    {
                        secondary.Players[screenNum - main.totalPlayers - 1].toggleHide();
                    }
                }
                return;
            }

            //hideAll
            if (bindMatchKey(keyCombo, "hideAll"))
            {
                for(int i = 1; i <= main.totalPlayers; i++)
                {
                    if (playerExists(i))
                    {
                        main.Players[i - 1].toggleHide();
                    }
                }

                if (secondary != null)
                {
                    for (int i = 1; i <= secondary.totalPlayers; i++)
                    {
                        if(playerExists(i + main.totalPlayers))
                        {
                            secondary.Players[i - 1].toggleHide();
                        }
                    }
                }

                return;
            }

            //pauseTimer
            if (bindMatchKey(keyCombo, "pauseTimer"))
            {
                if (replacementTimer != null)
                {
                    replacementTimer.Enabled = !replacementTimer.Enabled;


                    if (replacementTimer.Enabled)
                    {
                        //clock on
                        overlayOpen("Images/clock-on.png");
                    }
                    else
                    {
                        //clock off
                        overlayOpen("Images/clock-off.png");
                    }
                }

                return;
            }

            //pause
            if (bindMatchKey(keyCombo, "pause"))
            {
                if (playerExists())
                {
                    if (screenNum <= main.totalPlayers)
                    {
                        main.Players[screenNum - 1].togglePause();
                    }
                    else if (secondary != null)
                    {
                        secondary.Players[screenNum - main.totalPlayers - 1].togglePause();
                    }
                }
                return;
            }

            //pauseAll
            if (bindMatchKey(keyCombo, "pauseAll"))
            {
                for(int i = 1; i <= main.totalPlayers; i++)
                {
                    if (playerExists(i))
                    {
                        main.Players[i - 1].pause();
                    }
                }


                if(secondary != null)
                {
                    for(int i = 1; i <= secondary.totalPlayers; i++)
                    {
                        if(playerExists(i + main.totalPlayers))
                        {
                            secondary.Players[i - 1].pause();
                        }
                    }
                }
                return;
            }

            //deleteVideo
            if (bindMatchKey(keyCombo, "deleteVideo"))
            {
                return;
            }


            for (int i = 1; i <= 10; i++)
            {
                //player
                if (bindMatchKey(keyCombo, "player" + i))
                {
                    screenNum = i;
                    return;
                }

                //remotePlayer
                if (bindMatchKey(keyCombo, "remotePlayer" + i))
                {
                    //FINISH
                    return;
                }
            }
        }
    }
    
}
