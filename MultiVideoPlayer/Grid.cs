using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiVideoPlayer
{
    public class Grid
    {
        public int totalPlayers { get; }
        private Screen gridScreen;
        public int gridRows { get; }
        public int gridColumns { get; }
        public VideoPlayer[] Players;

        public Grid(int rows, int columns, bool secondaryScreen)
        {
            gridRows = rows;
            gridColumns = columns;
            totalPlayers = rows * columns;
            Players = new VideoPlayer[totalPlayers];

            getScreen(secondaryScreen);
        }

        public void closeAllPlayers(bool onlyWithReplacement = false)
        {
            for(int i = 0; i < Players.Length; i++)
            {
                if(Players[i] != null)
                {
                    if(onlyWithReplacement)
                    {
                        if(Players[i].replacement)
                        {
                            Players[i].Close();
                        }
                    }
                    else
                    {
                        Players[i].Close();
                    }
                }
            }
        }

        public void placePlayer(int screenNum, string filePath, bool autoReplace = false)
        {
            //autoReplace is true when the timer is replacing videos, which cant replace videos with replacement off
            double volume;
            bool muted = false;
            bool halfway = false;
            bool replace = true;

            if(autoReplace && !Players[screenNum - 1].replacement)
            {
                replace = false;
            }

            int[] RxC = getPlayerRowColumn(screenNum);
            int[] WH = getPlayerSize();
            Point topLeft = getPlayerLocation(RxC[0], RxC[1], WH[0], WH[1]);

            if(Properties.Settings.Default.startLowVolume)
            {
                volume = Properties.Settings.Default.lowVolumeLevel;
            }
            else
            {
                volume = 1;
            }

            if(Properties.Settings.Default.startAllMuted)
            {
                muted = true;
            }
            else if(screenNum > 1 && Properties.Settings.Default.startRestMuted)
            {
                muted = true;
            }

            if(Properties.Settings.Default.startHalfway)
            {
                halfway = true;
            }

            if (Players.Length >= screenNum && replace)
            {
                if(Players[screenNum -1] != null)
                {
                    Players[screenNum - 1].Close();
                }

                Players[screenNum - 1] = new VideoPlayer(screenNum, WH[0], WH[1], topLeft, RxC[0], RxC[1], volume.ToString(), muted, halfway, filePath, this);
            }
        }

        private int[] getPlayerRowColumn(int screenNum)
        {
            int playerRow = 1;
            int playerColumn = 1;

            int Counter = screenNum;
            if (Counter > totalPlayers)
            {
                Counter -= totalPlayers;
            }
            while (Counter >= gridRows)
            {
                if (gridColumns > gridRows && Counter < gridRows || Counter > gridRows)
                {
                    Counter -= gridColumns;
                    if (Counter >= 0)
                    {
                        playerRow += 1;
                    }
                }
                else if (gridRows > gridColumns)
                {
                    Counter -= gridColumns;
                    if (Counter >= 0)
                    {
                        playerRow += 1;
                    }
                }
                else if (Counter > gridRows && gridColumns <= gridRows)
                {
                    Counter -= gridColumns;
                    playerRow += 1;
                }
                else
                {
                    Counter = 0;
                }
            }

            Counter = 0;
            int column = 0;
            while (Counter < screenNum)
            {
                column += 1;

                if (column > gridColumns && gridColumns > gridRows)
                {
                    column = 1;
                }
                else if (column > gridColumns && gridColumns == gridRows)
                {
                    column = 1;
                }
                else if (column > gridColumns && gridRows > gridColumns)
                {
                    column = 1;
                }

                Counter = Counter + 1;

                playerColumn = column;
            }

            if (gridColumns > gridRows)
            {
                playerColumn += 1;
            }

            int[] RxC = {playerRow, playerColumn};

            return RxC;
        }

        private int[] getPlayerSize()
        {
            double width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / gridColumns;
            int realWidth = (int)width;
            double height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / gridRows;
            int realHeight = (int)height;

            int[] WH = { realWidth, realHeight };
            return WH;
        }

        private Point getPlayerLocation(int row, int column, int realWidth, int realHeight)
        {
            Rectangle WorkArea = gridScreen.WorkingArea;
            int previousScreen = column - 1;
            double widthTime = realWidth * previousScreen;
            double right = WorkArea.Left + widthTime;
            double previousRow = row - 1;
            double heightTime = realHeight * previousRow;
            double bottom = WorkArea.Top + heightTime;

            return new Point((int)right, (int)bottom);
        }

        //Determine which screen the grid is on
        private void getScreen(bool secondaryScreen)
        {
            if (secondaryScreen)
            {
                if (Screen.AllScreens.Length > 1)
                {
                    gridScreen = Screen.AllScreens[1];
                }
                else
                {
                    gridScreen = Screen.PrimaryScreen;
                }
            }
            else
            {
                gridScreen = Screen.PrimaryScreen;
            }
        }
    }
}
