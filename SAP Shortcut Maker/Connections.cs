using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAP_Shortcut_Maker
{
    public class Connections
    {
        public const int MinimumConnectionStringLength = 15;
        private string saveFile = System.IO.Directory.GetCurrentDirectory() + @"\connections";
        private List<string> items;
        public enum Properties
        {
            Name = 0,
            Server = 1,
            Client = 2,
            SystemID = 3,
            InstanceNumber = 4,
            Username = 5,
            Password = 6,
            Language = 7
        }

        public Connections() //Constructor
        {
            items = new List<string>();
            LoadItems();
        }
        #region --- File Handling ---
        public void LoadItems()
        {
            if (System.IO.File.Exists(saveFile))
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(saveFile, Encoding.UTF8);
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > MinimumConnectionStringLength)
                    {
                        if (line.Split(Program.Splitter).Length == 8)
                        {
                            items.Add(line);
                        }
                    }
                }
                reader.Close();
                reader.Dispose();
            }
        }
        public void SaveItems()
        {
            if (System.IO.File.Exists(saveFile))
            {
                try
                {
                    System.IO.File.SetAttributes(saveFile, System.IO.FileAttributes.Normal);
                }
                catch { }
            }
            System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFile, false, Encoding.UTF8);
            foreach (string s in items)
            {
                writer.WriteLine(s);
            }
            writer.Close();
            writer.Dispose();
            if (System.IO.File.Exists(saveFile))
            {
                try
                {
                    System.IO.File.SetAttributes(saveFile, System.IO.FileAttributes.Hidden | System.IO.FileAttributes.ReadOnly);
                }
                catch { }
            }
        }
        #endregion
        #region --- Connection items Handling ---
        public string GetProperty(int itemIndex, Properties property)
        {
            string ret = GetItem(itemIndex).Split(Program.Splitter).GetValue((int)property).ToString();
            return (property == Properties.Password) ? Security.Decrypt(ret) : ret;
        }
        public static string GetProperty(string connectionString, Properties property)
        {
            string ret = connectionString.Split(Program.Splitter).GetValue((int)property).ToString();
            return (property == Properties.Password) ? Security.Decrypt(ret) : ret;
        }
        public List<string> GetItems()
        {
            return items;
        }
        public string GetItem(int itemIndex)
        {
            return items[itemIndex];
        }
        public void SetItem(int itemIndex, string value)
        {
            items[itemIndex] = value;
        }
        public void Add(string item)
        {
            items.Add(item);
        }
        public string Find(Predicate<string> match)
        {
            return items.Find(match);
        }
        public int FindIndex(Predicate<string> match)
        {
            return items.FindIndex(match);
        }
        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }
        public bool Exists(string item)
        {
            return items.Exists( itm =>
                GetProperty(item,Properties.Name) == GetProperty(itm,Properties.Name) &&
                GetProperty(item, Properties.Server) == GetProperty(itm, Properties.Server) &&
                GetProperty(item, Properties.Client) == GetProperty(itm, Properties.Client) &&
                GetProperty(item, Properties.SystemID) == GetProperty(itm, Properties.SystemID) &&
                GetProperty(item, Properties.InstanceNumber) == GetProperty(itm, Properties.InstanceNumber) &&
                GetProperty(item, Properties.Username) == GetProperty(itm, Properties.Username) &&
                GetProperty(item, Properties.Password) == GetProperty(itm, Properties.Password) &&
                GetProperty(item, Properties.Language) == GetProperty(itm, Properties.Language)
                );
        }
        #endregion
    }
}
