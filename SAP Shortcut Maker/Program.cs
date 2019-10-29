using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SAP_Shortcut_Maker
{
    static class Program
    {
        public const char Splitter = ((char)007);
        private const int minimumArgumentStringLength = 5;

        [STAThread]
        private static void Main(string[] args)
        {
            if (args != null && args.Length == minimumArgumentStringLength)
            {
                string name = args[0];
                string server = args[1];
                string client = args[2];
                string sysid = args[3];
                string insnum = args[4];

                Connections connections = new Connections();

                string connectionString = connections.Find(item =>
                Connections.GetProperty(item, Connections.Properties.Name) == name &&
                Connections.GetProperty(item, Connections.Properties.Server) == server &&
                Connections.GetProperty(item, Connections.Properties.Client) == client &&
                Connections.GetProperty(item, Connections.Properties.SystemID) == sysid &&
                Connections.GetProperty(item, Connections.Properties.InstanceNumber) == insnum
                ) ;


                if (connectionString.Length > Connections.MinimumConnectionStringLength)
                    Connect(connectionString);
                else
                    MessageBox.Show("Didn't found the connection", "Connection not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Application.Exit();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            }
        private static string connectionArgsTemplate = "-max -guiparm=\"{0} {3}\" -system={2} -client={1} -user={4} -pw={5} -l={6}";
        public static void Connect(string connectionString)
        {
            Execute(
                string.Format(connectionArgsTemplate,
                connectionString.Split(Program.Splitter).GetValue(1).ToString(),                   //Server
                connectionString.Split(Program.Splitter).GetValue(2).ToString(),                   //Client
                connectionString.Split(Program.Splitter).GetValue(3).ToString(),                   //System ID
                connectionString.Split(Program.Splitter).GetValue(4).ToString(),                   //Instance Number
                connectionString.Split(Program.Splitter).GetValue(5).ToString(),                   //Username
                Security.Decrypt(connectionString.Split(Program.Splitter).GetValue(6).ToString()), //Password
                connectionString.Split(Program.Splitter).GetValue(7).ToString()                    //language
                )
            );

        }
        public static void Execute(string args)
        {
            if (!System.IO.File.Exists(Properties.Settings.Default.sapshcut))
            {
                MessageBox.Show("Couldn't find Sapshcut please give it's path.\n It's inside the SAP instalation folder folder and in FrontEnd\\SAPgui\\ subfolder","Sapshcut didn't found",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                GetSapshcutPath();
            }

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = Properties.Settings.Default.sapshcut;
            startInfo.Arguments = args;
            process.StartInfo = startInfo;
            process.Start();
        }
        public static void GetSapshcutPath()
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Title = "Select sapshcut executable";
            OFD.Filter = "sapshcut.exe|sapshcut.exe|*.exe|*.exe";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                System.Diagnostics.FileVersionInfo FVI = System.Diagnostics.FileVersionInfo.GetVersionInfo(OFD.FileName);
                if (FVI.FileDescription == "SAP GUI Shortcut") {
                    Properties.Settings.Default.sapshcut = OFD.FileName;
                    Properties.Settings.Default.Save();
                    }
                else
                    MessageBox.Show("This doesn't seems to be sapshcut", "Invalid executable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
