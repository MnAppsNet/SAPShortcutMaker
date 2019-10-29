using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAP_Shortcut_Maker
{
    public partial class Edit : Form
    {
        private Settings controller;
        private int connectionIndex;
        public Edit(Settings Controller, int ConnectionIndex) //Constructor
        {
            controller = Controller;
            connectionIndex = ConnectionIndex;
            InitializeComponent();
        }
        private bool isNumber(string value)
        {
            return int.TryParse(value, out int n);
        }
        private bool FieldsOk()
        {
            return (
                 in_name.Text.Replace(" ", "") != ""
                 && in_server.Text.Replace(" ", "") != ""
                 && in_client.Text.Replace(" ", "") != ""
                 && in_instnum.Text.Replace(" ", "") != ""
                 && in_sysid.Text.Replace(" ", "") != ""
                 && in_username.Text.Replace(" ", "") != ""
                 && in_password.Text.Replace(" ", "") != ""
                 && in_lang.Text.Replace(" ", "") != ""
                 && isNumber(in_instnum.Text)
                 && isNumber(in_client.Text)
              );
        }
        private void LoadFields(object sender, EventArgs e)
        {
            if (connectionIndex < 0) this.Close();

            string connectionString = controller.controller.connections.GetItem(connectionIndex);

            in_name.Text = Connections.GetProperty(connectionString, Connections.Properties.Name);
            in_server.Text = Connections.GetProperty(connectionString, Connections.Properties.Server);
            in_client.Text = Connections.GetProperty(connectionString, Connections.Properties.Client);
            in_sysid.Text = Connections.GetProperty(connectionString, Connections.Properties.SystemID);
            in_instnum.Text = Connections.GetProperty(connectionString, Connections.Properties.InstanceNumber);
            in_username.Text = Connections.GetProperty(connectionString, Connections.Properties.Username);
            in_password.Text = Connections.GetProperty(connectionString, Connections.Properties.Password);
            in_lang.Text = Connections.GetProperty(connectionString, Connections.Properties.Language);
        }
        private string getConnectionString()
        {
            return in_name.Text + Program.Splitter +
                   in_server.Text + Program.Splitter +
                   in_client.Text + Program.Splitter +
                   in_sysid.Text + Program.Splitter +
                   in_instnum.Text + Program.Splitter +
                   in_username.Text + Program.Splitter +
                   Security.Encrypt(in_password.Text) + Program.Splitter +
                   in_lang.Text;
        }
        private void SaveButtonClick(object sender, EventArgs e)
        {
            if (!FieldsOk())
            {
                MessageBox.Show("Some of the fields are empty or doesn't contain valid values", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string connectionString = getConnectionString();
            if (connectionString.Replace(" ", "") != "")
            {
                controller.controller.UpdateConnection(connectionString, connectionIndex);
                controller.in_connections.Items[connectionIndex] = Connections.GetProperty(connectionString,Connections.Properties.Name);
                controller.controller.in_name.Items[connectionIndex] = Connections.GetProperty(connectionString, Connections.Properties.Name);

                this.Close();
            }
            else MessageBox.Show("Don't know how did you managed this but the connectionString is empty...", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
