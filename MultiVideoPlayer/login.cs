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
    public partial class login : Form
    {
        bool auth = false;
        string[] parameters;

        public login()
        {
            InitializeComponent();
        }

        private bool formExiting = false;
        private void login_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!formExiting)
            {
                formExiting = true;
                if (Application.OpenForms.Count == 1)
                {
                    Application.Exit();
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (auth)
            {
                parameters = txtPassword.Text.Split(' ');
                new Dock(parameters).Show();
                this.Close();
            }
            else
            {
                string passwordParameters = txtPassword.Text;
                string[] split = passwordParameters.Split(' ');

                string password = split[0];

                if (password == Properties.Settings.Default.password)
                {
                    authSuccess();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void login_Load(object sender, EventArgs e)
        {
            if(Properties.Settings.Default.password.Length == 0)
            {
                authSuccess();
            }
        }

        private void authSuccess()
        {
            auth = true;
            this.Text = "Parameters";
            label1.Text = "Parameters: ";
            btnSubmit.Text = "Start";
        }
    }
}
