namespace Fintech.API.Helpers
{
	using System;
	using System.Security.Cryptography;
	using System.Text;
	using System.IO;

	class CifradoHelper
	{
		private string encryptionKey = "TmF Cifrado DAtOs SensiBLes"; //Puede ser cualquier texto para que se utilice como una llave
		private byte[] salt = Encoding.ASCII.GetBytes("dG1GR3JvdVBMZWd2SVQ="); //Es un texto en base 64, el cual se convierne en un byte[] el cual puede ser por medio de un formato hexadecimal 
		//Ivan Medvedev -> SXZhbiBNZWR2ZWRldg== -> 4976616e204d65647665646576 ->{ 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }
		//tmFGrouPLegvIT -> dG1GR3JvdVBMZWd2SVQ= -> 746d4647726f75504c6567764954 ->{ 0x74, 0x6d, 0x46, 0x47, 0x72, 0x6f, 0x75, 0x50, 0x4c, 0x65, 0x67, 0x76, 0x49, 0x54 }

		public string DecryptStringAES(string cipherText)
		{
			//var encrypted = Convert.FromBase64String(cipherText);
			var decriptedFromJavascript = DecryptStringFromBytes(cipherText, this.encryptionKey, this.salt);
			return string.Format(decriptedFromJavascript);
		}

		public string EncryptStringAES(string cipherText)
		{
			var EncryptFromJavascript = EncryptStringToBytes(cipherText, this.encryptionKey, this.salt);
			return Convert.ToBase64String(EncryptFromJavascript);
		}

		private static byte[] EncryptStringToBytes(string plainText, string encryptionKey, byte[] salt)
		{
			// Check arguments.  
			if (plainText == null || plainText.Length <= 0)
			{
				throw new ArgumentNullException("plainText");
			}
			if (encryptionKey == null || encryptionKey.Length <= 0)
			{
				throw new ArgumentNullException("key");
			}
			if (salt == null || salt.Length <= 0)
			{
				throw new ArgumentNullException("iv");
			}

			byte[] encrypted;
			var clearText = plainText; //"123456";
			var EncryptionKey = encryptionKey;//"secret key string";

			var clearBytes = Encoding.Unicode.GetBytes(clearText);
			using (System.Security.Cryptography.Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x74, 0x6d, 0x46, 0x47, 0x72, 0x6f, 0x75, 0x50, 0x4c, 0x65, 0x67, 0x76, 0x49, 0x54 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);

				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(clearBytes, 0, clearBytes.Length);
						cs.Close();
					}
					clearText = Convert.ToBase64String(ms.ToArray());
					encrypted = ms.ToArray();
				}
			}
			//Console.WriteLine(clearText);
			return encrypted;
		}

		private static string DecryptStringFromBytes(string cipherText, string encryptionKey, byte[] salt)
		{
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (encryptionKey == null || encryptionKey.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (salt == null || salt.Length <= 0)
            {
                throw new ArgumentNullException("iv");
            }

			string encrypted;
			string EncryptionKey = encryptionKey;//"secret key string";
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x74, 0x6d, 0x46, 0x47, 0x72, 0x6f, 0x75, 0x50, 0x4c, 0x65, 0x67, 0x76, 0x49, 0x54 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);

				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(cipherBytes, 0, cipherBytes.Length);
						cs.Close();
					}
					var cipherTextT = Encoding.Unicode.GetString(ms.ToArray());
					encrypted = Encoding.Unicode.GetString(ms.ToArray());
				}
			}
			return encrypted;	
		}
	}
}