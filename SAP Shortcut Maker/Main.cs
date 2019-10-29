using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAP_Shortcut_Maker
{
    public partial class Main : Form
    {
        #region --- Private Variables ---
        public Connections connections;
        #endregion -------------------------
        public Main() //Constructor
        {
            InitializeComponent();
            connections = new Connections();
            foreach(string s in connections.GetItems())
            {
                in_name.Items.Add(Connections.GetProperty(s, Connections.Properties.Name));
            }
        }
        private static void createShortcut(string args)
        {
            SaveFileDialog OFD = new SaveFileDialog();
            OFD.Filter = "Shortcut|*.lnk";
            OFD.Title = "Choose a location to save the shortcut";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                string link = OFD.FileName + ((!OFD.FileName.EndsWith(".lnk"))?".lnk":"");
                var shell = new IWshRuntimeLibrary.WshShell();
                var shortcut = shell.CreateShortcut(link) as IWshRuntimeLibrary.IWshShortcut;
                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.WorkingDirectory = Application.StartupPath;
                shortcut.Arguments = args;
                shortcut.Save();
            }
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
        public void UpdateConnection(string connectionString, int index)
        {
            connections.SetItem(index,connectionString);
            SaveConnections();

        }
        #region --- handle saved connections ---
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
        private string addConnection()
        {
            string connectionString = Encoding.UTF8.GetString(Encoding.Default.GetBytes(getConnectionString()));
            if (!connections.Exists(connectionString))
            {
                connections.Add(connectionString);
                in_name.Items.Add(connectionString.Split(Program.Splitter).GetValue(0).ToString());
                SaveConnections();
            }
            return connectionString;
        }
        private void loadConnection(int index)
        {
            if (index < 0) return;

            in_name.Text = connections.GetProperty(index, Connections.Properties.Name);
            in_server.Text = connections.GetProperty(index, Connections.Properties.Server) ;
            in_client.Text = connections.GetProperty(index, Connections.Properties.Client);
            in_sysid.Text = connections.GetProperty(index, Connections.Properties.SystemID);
            in_instnum.Text = connections.GetProperty(index, Connections.Properties.InstanceNumber);
            in_username.Text = connections.GetProperty(index, Connections.Properties.Username);
            in_password.Text = connections.GetProperty(index, Connections.Properties.Password);
            in_lang.Text = connections.GetProperty(index, Connections.Properties.Language);

        }
        public void SaveConnections()
        {
            connections.SaveItems();
        }
        #endregion ------------------

        #region --- events ---
        private void connectButtonClick(object sender, EventArgs e) // On click of the Connect Button
        {
            if (!FieldsOk())
            {
                MessageBox.Show("Some of the fields are empty or doesn't contain valid values","Warning!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            string connectionString = addConnection();
            if (connectionString.Replace(" ", "") != "")
            {
                Program.Connect(connectionString);
            }
            else MessageBox.Show("Don't know how did you managed this but the connectionString is empty...", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void createButtonClick(object sender, EventArgs e) // On click of the Create Button
        {
            if (!FieldsOk())
            {
                MessageBox.Show("Some of the fields are empty or doesn't contain valid values", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string connectionString = addConnection();
            if (connectionString.Replace(" ", "") != "")
            {
                createShortcut("\"" + Connections.GetProperty(connectionString, Connections.Properties.Name) + "\"" +
                   " \"" + Connections.GetProperty(connectionString, Connections.Properties.Server) + "\"" +
                   " \"" + Connections.GetProperty(connectionString, Connections.Properties.Client) + "\"" +
                   " \"" + Connections.GetProperty(connectionString, Connections.Properties.SystemID) + "\"" +
                   " \"" + Connections.GetProperty(connectionString, Connections.Properties.InstanceNumber) + "\""
                   );
            }
            else MessageBox.Show("Don't know how did you managed this but the connectionString is empty...", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void ConnectionNameChanged(object sender, EventArgs e) // On index change of connection name combobox
        {
            loadConnection(in_name.SelectedIndex);
        }
        private void topMostCheckBoxChange(object sender, EventArgs e) // On Change of the TopMost checkbox
        {
            this.TopMost = checkBox1.Checked;
        }
        private void settingsButtonClick(object sender, EventArgs e)
        {
            Settings settings = new Settings(this);
            settings.Show();
        }
        #endregion ------------
    }
}
