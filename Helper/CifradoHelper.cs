namespace Fintech.API.Helpers
{
	using System;
	using System.Security.Cryptography;
	using System.Text;
	using System.IO;

	class CifradoHelper
	{
		private byte[] keybytes = Encoding.UTF8.GetBytes("RfUjXn2r5u8x/A?D");
		private byte[] iv = Encoding.UTF8.GetBytes("RfUjXn2r5u8x/A?D");

		public string DecryptStringAES(string cipherText)
		{
			var encrypted = Convert.FromBase64String(cipherText);
			var decriptedFromJavascript = DecryptStringFromBytes(encrypted, this.keybytes, this.iv);
			return string.Format(decriptedFromJavascript);
		}

		public string EncryptStringAES(string cipherText)
		{
			//var encrypted = Convert.FromBase64String(cipherText);
			var EncryptFromJavascript = EncryptStringToBytes(cipherText, this.keybytes, this.iv);
			//return string.Format(decriptedFromJavascript);
			return Convert.ToBase64String(EncryptFromJavascript);
		}

		private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
		{
			// Check arguments.  
			if (plainText == null || plainText.Length <= 0)
			{
				throw new ArgumentNullException("plainText");
			}
			if (key == null || key.Length <= 0)
			{
				throw new ArgumentNullException("key");
			}
			if (iv == null || iv.Length <= 0)
			{
				throw new ArgumentNullException("iv");
			}
			byte[] encrypted;
			// Create a RijndaelManaged object  
			// with the specified key and IV.  
			using (var rijAlg = new RijndaelManaged())
			{
				rijAlg.Mode = CipherMode.CBC;
				rijAlg.Padding = PaddingMode.PKCS7;
				rijAlg.FeedbackSize = 128;

				rijAlg.Key = key;
				rijAlg.IV = iv;

				// Create a decrytor to perform the stream transform.  
				var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

				// Create the streams used for encryption.  
				using (var msEncrypt = new MemoryStream())
				{
					using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (var swEncrypt = new StreamWriter(csEncrypt))
						{
							//Write all data to the stream.  
							swEncrypt.Write(plainText);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}
			// Return the encrypted bytes from the memory stream.  
			return encrypted;
		}

		private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
		{
			// Check arguments.  
			if (cipherText == null || cipherText.Length <= 0)
			{
				throw new ArgumentNullException("cipherText");
			}
			if (key == null || key.Length <= 0)
			{
				throw new ArgumentNullException("key");
			}
			if (iv == null || iv.Length <= 0)
			{
				throw new ArgumentNullException("iv");
			}

			// Declare the string used to hold  
			// the decrypted text.  
			string plaintext = null;

			// Create an RijndaelManaged object  
			// with the specified key and IV.  
			using (var rijAlg = new RijndaelManaged())
			{
				//Settings  
				rijAlg.Mode = CipherMode.CBC;
				rijAlg.Padding = PaddingMode.PKCS7;
				rijAlg.FeedbackSize = 128;

				rijAlg.Key = key;
				rijAlg.IV = iv;

				// Create a decrytor to perform the stream transform.  
				var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

				try
				{
					// Create the streams used for decryption.  
					using (var msDecrypt = new MemoryStream(cipherText))
					{
						using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
						{

							using (var srDecrypt = new StreamReader(csDecrypt))
							{
								// Read the decrypted bytes from the decrypting stream  
								// and place them in a string.  
								plaintext = srDecrypt.ReadToEnd();

							}

						}
					}
				}
				catch
				{
					plaintext = "keyError";
				}
			}

			return plaintext;
		}
	}
}