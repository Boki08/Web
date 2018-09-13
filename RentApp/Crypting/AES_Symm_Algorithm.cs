using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;

namespace RentApp.Crypting
{
	public class AES_Symm_Algorithm
	{
		/// <summary>
		/// Function that encrypts the plaintext from inFile and stores cipher text to outFile
		/// </summary>
		/// <param name="inFile"> filepath where plaintext is stored </param>
		/// <param name="outFile"> filepath where cipher text is expected to be stored </param>
		/// <param name="secretKey"> symmetric encryption key </param>
		public static void EncryptFile(string inFile, string outFile, string secretKey)
		{
			byte[] header = null;	//image header (54 byte) should not be encrypted
			byte[] body = null;     //image body to be encrypted

            Formatter.Decompose(File.ReadAllBytes(inFile),out header, out body);

            //DESCryptoServiceProvider DEScsp = new DESCryptoServiceProvider();
            AesCryptoServiceProvider AEScsp = new AesCryptoServiceProvider();

            AEScsp.Key = ASCIIEncoding.ASCII.GetBytes(secretKey);
            AEScsp.Mode = CipherMode.ECB;
            AEScsp.Padding = PaddingMode.None;

            ICryptoTransform encryptor = AEScsp.CreateEncryptor();

            MemoryStream mStream = new MemoryStream();

            CryptoStream crypto = new CryptoStream(mStream,encryptor, CryptoStreamMode.Write);

            //encription...

            crypto.Write(body,0,body.Length);


            byte[] encrypted = mStream.ToArray();

            //	public static void Compose(byte[] header, byte[] body, int outputLenght, string outFile)
            Formatter.Compose(header, encrypted, header.Length + encrypted.Length,outFile);
            //
        }


		/// <summary>
		/// Function that decrypts the cipher text from inFile and stores as plaintext to outFile
		/// </summary>
		/// <param name="inFile"> filepath where cipher text is stored </param>
		/// <param name="outFile"> filepath where plain text is expected to be stored </param>
		/// <param name="secretKey"> symmetric encryption key </param>
		public static void DecryptFile(string inFile, string outFile, string secretKey)
		{
			byte[] header = null;		//image header (54 byte) should not be decrypted
			byte[] body = null;			//image body to be decrypted

			/// Formatter.Decompose();			
			
			//DESCryptoServiceProvider desCrypto = new DESCryptoServiceProvider();
            //AesCryptoServiceProvider aesCrypto = new AesCryptoServiceProvider();
            /// desCrypto.Padding = PaddingMode.None;

            /// ICryptoTransform desDecrypt = desCrypto.CreateDecryptor();
            /// CryptoStream cryptoStream

            /// output = header + decrypted_body
            /// Formatter.Compose();		
            /// 

            Formatter.Decompose(File.ReadAllBytes(inFile), out header, out body);

            //DESCryptoServiceProvider DEScsp = new DESCryptoServiceProvider();
            AesCryptoServiceProvider Aescsp = new AesCryptoServiceProvider();

            Aescsp.Key = ASCIIEncoding.ASCII.GetBytes(secretKey);
            Aescsp.Mode = CipherMode.ECB;
            Aescsp.Padding = PaddingMode.None;

            ICryptoTransform decryptor = Aescsp.CreateDecryptor();

            MemoryStream mStream = new MemoryStream();

            CryptoStream crypto = new CryptoStream(mStream, decryptor, CryptoStreamMode.Write);

            //encription...

            crypto.Write(body, 0, body.Length);


            byte[] decrypted = mStream.ToArray();

            //	public static void Compose(byte[] header, byte[] body, int outputLenght, string outFile)
            Formatter.Compose(header, decrypted, header.Length + decrypted.Length, outFile);
        }
	}
}
