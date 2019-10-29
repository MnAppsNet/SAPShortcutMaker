using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace SAP_Shortcut_Maker
{
    //Implimentation Of Encrytpion And Decription Algorithms Here
    public static class Security
    {
        //____________________________________________________//          <<<
        //             Half of the encryption key             //        <<<
        //                You must change that                //      <<<
        //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV  //    <<<  ================================================
        private const string HalfKey = "t1S6Id(4kR*3f(0_4.?"; //  <<<    ================================================
        private const int GeneratedKeyLength = 12;            //    <<<  ================================================
        //ΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛΛ //      <<<
        //          You need to change both of these          //        <<<
        //               For your own security                //          <<<
        //____________________________________________________//    

        private static string hardwareID()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == "")
                {
                    //Get only the first CPU's ID
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + System.IO.Path.GetPathRoot(Environment.SystemDirectory).Replace(@"\","") + @"""");
            dsk.Get();
            string volumeSerial = dsk["VolumeSerialNumber"].ToString();

            return cpuInfo + volumeSerial;
        }
        private static string createKey()
        {
            Random R = new Random(DateTime.Now.Second + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Millisecond);
            string key = "";
            for ( int i = 0; i < GeneratedKeyLength; i++)
            {
                key += (char)R.Next(33, 136);
            }
            return key;
        }

        public static string Encrypt(string textData) 
        {
            string hid = hardwareID();
            string OtherHalf = createKey();
            string Encryptionkey = HalfKey + hid + OtherHalf;
            RijndaelManaged objrij = new RijndaelManaged();
            //set the mode for operation of the algorithm
            objrij.Mode = CipherMode.ECB;
            //set the padding mode used in the algorithm.
            objrij.Padding = PaddingMode.ISO10126;
            //set the size, in bits, for the secret key.
            objrij.KeySize = 0x80;
            //set the block size in bits for the cryptographic operation.
            objrij.BlockSize = 0x80;
            //set the symmetric key that is used for encryption & decryption.
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            //set the initialization vector (IV) for the symmetric algorithm
            byte[] EncryptionkeyBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);
            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;
            //Creates symmetric AES object with the current key and initialization vector IV.
            ICryptoTransform objtransform = objrij.CreateEncryptor();
            byte[] textDataByte = Encoding.UTF8.GetBytes(textData);
            //Final transform the test string.
            string encrypted = Convert.ToBase64String(objtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(encrypted + OtherHalf));
        }

        public static string Decrypt(string EncryptedText)
        {
            string Encryptionkey = HalfKey + hardwareID();
            RijndaelManaged objrij = new RijndaelManaged();
            objrij.Mode = CipherMode.ECB;
            objrij.Padding = PaddingMode.ISO10126;
            objrij.KeySize = 0x80;
            objrij.BlockSize = 0x80;
            string encrypted = Encoding.UTF8.GetString(Convert.FromBase64String(EncryptedText));
            Encryptionkey += encrypted.Substring(encrypted.Length - GeneratedKeyLength, GeneratedKeyLength);
            encrypted = encrypted.Substring(0, encrypted.Length - GeneratedKeyLength);
            byte[] encryptedTextByte = Convert.FromBase64String(encrypted);
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            byte[] EncryptionkeyBytes = new byte[0x10];
            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);
            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;
            byte[] TextByte = objrij.CreateDecryptor().TransformFinalBlock(encryptedTextByte, 0, encryptedTextByte.Length);
            return Encoding.UTF8.GetString(TextByte); 
        }

    }
}