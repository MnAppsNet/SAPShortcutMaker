using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAP_Shortcut_Maker
{
    public partial class Settings : Form
    {
        public Main controller;
        public Settings(Main Controller)
        {
            controller = Controller;
            InitializeComponent();
        }

        private void SettingsLoad(object sender, EventArgs e)
        {
            in_sapshcut.Text = Properties.Settings.Default.sapshcut;
            foreach (string s in controller.connections.GetItems())
            {
                in_connections.Items.Add(Connections.GetProperty(s, Connections.Properties.Name));
            }
            if (in_connections.Items.Count > 0)
            {
                in_connections.SelectedIndex = 0;
                in_connections.Text = in_connections.Items[0].ToString();
            }
        }

        private void OnConnectionsTextChange(object sender, EventArgs e)
        {
            //Write protection, don't let user change the values
            if (!in_connections.Items.Contains(in_connections.Text))
            {
                if (in_connections.Items.Count > 0)
                {
                    in_connections.Text = in_connections.Items[0].ToString();
                    in_connections.SelectedIndex = 0;
                }
            }
        }

        private void deleteClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("Do you want to delete this connection?", "Delete Connection", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (in_connections.Items.Count > 0)
                {
                    int index = in_connections.SelectedIndex;
                    controller.connections.RemoveAt(index);
                    in_connections.Items.RemoveAt(index);
                    controller.in_name.Items.RemoveAt(index);
                    if (in_connections.Items.Count > 0)
                    {
                        in_connections.SelectedIndex = 0;
                        in_connections.Text = in_connections.Items[0].ToString();
                    }
                    else in_connections.Text = "";
                    if (controller.in_name.Items.Count > 0)
                    {
                        controller.in_name.SelectedIndex = 0;
                        controller.in_name.Text = in_connections.Items[0].ToString();
                    }
                    else controller.in_name.Text = "";

                    controller.SaveConnections();
                }
                else
                {
                    MessageBox.Show("No connection selected", "No connection found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            controller.in_name = in_connections;
        }

        private void editClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (in_connections.Items.Count > 0)
            {
                Edit edits = new Edit(this, in_connections.SelectedIndex);
                edits.Show();
            }
            else
            {
                MessageBox.Show("No connection selected", "No connection found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetSapshcutPath(object sender, EventArgs e)
        {
            Program.GetSapshcutPath();
            in_sapshcut.Text = Properties.Settings.Default.sapshcut;
        }
    }
}
