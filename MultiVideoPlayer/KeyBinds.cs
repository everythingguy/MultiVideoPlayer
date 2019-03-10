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
    public partial class KeyBinds : Form
    {
        Dictionary<string, string> keybinds;
        DataTable db;

        public KeyBinds(Settings parent)
        {
            InitializeComponent();

            parentForm = parent;
        }

        private void KeyBinds_Load(object sender, EventArgs e)
        {
            Dock main = getDockForm();

            if(main != null)
            {
                keybinds = main.getKeyBinds();
                db = new DataTable();

                db.Columns.Add("Action", typeof(string));
                db.Columns.Add("Key", typeof(string));
                foreach(KeyValuePair<string, string> pair in keybinds)
                {
                    db.Rows.Add(pair.Key, pair.Value);
                }
                
                dataGridView1.DataSource = db;

                for(int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            } else
            {
                this.Close();
            }
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

        Settings parentForm;

        private void KeyBinds_FormClosing(object sender, FormClosingEventArgs e)
        {
            parentForm.enableBtn();
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 1)
            {
                dataGridView1.CurrentCell.ReadOnly = false;
            }
            else if(dataGridView1.CurrentCell.ColumnIndex == 0)
            {
                dataGridView1.CurrentCell.ReadOnly = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Dock main = getDockForm();
            main.setKeyBinds(keybinds);
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.UpdateCellValue(e.ColumnIndex, e.RowIndex);
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            keybinds[dataGridView1[0, e.RowIndex].Value.ToString()] = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
        }
    }
}
