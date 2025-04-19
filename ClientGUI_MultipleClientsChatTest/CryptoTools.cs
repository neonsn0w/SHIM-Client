using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClientGUI_MultipleClientsChatTest
{
    internal class CryptoTools
    {
        private const string PublicKeyFile = "publicKey.xml";
        private const string PrivateKeyFile = "privateKey.xml";

        public static void GenerateAndSaveKeys()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    // Export keys to XML strings
                    string publicKey = rsa.ToXmlString(false); // Public key only
                    string privateKey = rsa.ToXmlString(true); // Public and private key

                    // Save keys to files
                    File.WriteAllText(PublicKeyFile, publicKey);
                    File.WriteAllText(PrivateKeyFile, privateKey);

                    Console.WriteLine("Keys generated and saved to files.");
                }
                finally
                {
                    rsa.PersistKeyInCsp = false; // Do not persist keys in the container
                }
            }
        }

        public static byte[] Encrypt(string message, string publicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey); // Load the public key
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                return rsa.Encrypt(messageBytes, false); // Encrypt with PKCS#1 v1.5 padding
            }
        }

        public static string Decrypt(byte[] encryptedMessage, string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey); // Load the private key
                byte[] decryptedBytes = rsa.Decrypt(encryptedMessage, false); // Decrypt with PKCS#1 v1.5 padding
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        public static (string publicKey, string privateKey) LoadKeys()
        {
            if (!File.Exists(PublicKeyFile) || !File.Exists(PrivateKeyFile))
            {
                throw new FileNotFoundException("Key files not found. Please generate the keys first.");
            }

            // Read keys from files
            string publicKey = File.ReadAllText(PublicKeyFile);
            string privateKey = File.ReadAllText(PrivateKeyFile);

            return (publicKey, privateKey);
        }
    }
}
