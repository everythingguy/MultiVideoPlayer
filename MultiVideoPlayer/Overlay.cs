using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiVideoPlayer
{
    public partial class overlay : Form
    {

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private const int WS_EX_TOPMOST = 0x00000008;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= WS_EX_TOPMOST;
                return createParams;
            }
        }

        public overlay(string pic, int x = 0, int y = 0)
        {
            InitializeComponent();
            if(x != 0 && y != 0)
            {
                this.Location = new Point(x, y);
            } else
            {
                this.StartPosition = FormStartPosition.CenterScreen;
            }
            this.TransparencyKey = pictureBox1.BackColor;
            pictureBox1.ImageLocation = pic;

            this.CreateHandle();
        }
    }
}
