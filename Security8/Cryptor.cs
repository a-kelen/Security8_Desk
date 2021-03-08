using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Security8
{
    public class Cryptor
    {
        static string IV = "qo1lc3sjd8zpt9cx";
        static string Key = "owejehavhelkjhfjsjghrydkg57k7jf3";
        AesCryptoServiceProvider aes;

        public Cryptor()
        {
             aes = new AesCryptoServiceProvider();
           

            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            aes.IV = ASCIIEncoding.ASCII.GetBytes(IV);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
        }

        
        public string Encrypt(string text)
        {
            ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] encryptedBytes = transform.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(text), 0, text.Length);

            string str = Convert.ToBase64String(encryptedBytes);

            return str;
        }

        public string Decrypt(string file)
        {
            var info = new FileInfo(file);
            string text = TextReader.getText(file);
            ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] encryptedBytes = Convert.FromBase64String(text);
            byte[] decryptedBytes = transform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            string str = ASCIIEncoding.ASCII.GetString(decryptedBytes);
            return str;
        }
    }
}
