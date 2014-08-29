using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Cryptography
{
    public class CryptographyServices
    {
		public static string Decrypt(string encryptedText, string passKey, KeySize keySize = KeySize._256)
		{
			RijndaelManaged aesEncryption = new RijndaelManaged();
			aesEncryption.KeySize = (int)keySize;
			aesEncryption.BlockSize = 128;
			aesEncryption.Mode = CipherMode.ECB;
			aesEncryption.Padding = PaddingMode.ISO10126;
			byte[] KeyInBytes = Encoding.UTF8.GetBytes(_GenerateAPassKey(passKey));
			aesEncryption.Key = KeyInBytes;
			ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
			byte[] encryptedBytes = Convert.FromBase64CharArray(encryptedText.ToCharArray(), 0, encryptedText.Length);
			return ASCIIEncoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
		}

		private static string _GenerateAPassKey(string passKey, KeySize keySize = KeySize._256)
		{
			// Pass Phrase can be any string
			string passPhrase = passKey;
			// Salt Value can be any string(for simplicity use the same value as used for the pass phrase)
			string saltValue = passKey;
			// Hash Algorithm can be "SHA1 or MD5"
			string hashAlgorithm = "SHA1";
			// Password Iterations can be any number
			int passwordIterations = 2;
			// Convert Salt passphrase string to a Byte Array
			byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
			// Using System.Security.Cryptography.PasswordDeriveBytes to create the Key
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
			//When creating a Key Byte array from the base64 string the Key must have 32 dimensions.
			byte[] Key = pdb.GetBytes(((int)keySize) / 11);
			String KeyString = Convert.ToBase64String(Key);

			return KeyString;
		}

		public static string Encrypt(string plainStr, string passKey, KeySize keySize = KeySize._256)
		{
			RijndaelManaged aesEncryption = new RijndaelManaged();
			aesEncryption.KeySize = (int)keySize;
			aesEncryption.BlockSize = 128;
			aesEncryption.Mode = CipherMode.ECB;
			aesEncryption.Padding = PaddingMode.ISO10126;
			byte[] KeyInBytes = Encoding.UTF8.GetBytes(_GenerateAPassKey(passKey));
			aesEncryption.Key = KeyInBytes;
			byte[] plainText = ASCIIEncoding.UTF8.GetBytes(plainStr);
			ICryptoTransform crypto = aesEncryption.CreateEncryptor();
			byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
			return Convert.ToBase64String(cipherText);
		}
    }
}
