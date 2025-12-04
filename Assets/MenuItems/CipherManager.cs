using System.Text;
using UnityEngine;

namespace Systems.Security
{
    public static class CipherManager
    {
        private const int AlphabetLength = 33;
        private const char LatinBase = 'A';
        private const char CyrillicBase = 'À';

        public static string Encrypt(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            input = input.ToUpper();
            char encryptionKey = GenerateRandomKey();

            var sb = new StringBuilder(input.Length + 1);

            foreach (var c in input)
            {
                sb.Append(EncryptChar(c, encryptionKey));
            }

            sb.Append(encryptionKey);

            return sb.ToString();
        }

        public static string Decrypt(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            int lastIndex = input.Length - 1;
            char encryptionKey = input[lastIndex];
            string content = input.Substring(0, lastIndex);

            var sb = new StringBuilder(content.Length);

            foreach (var c in content)
            {
                sb.Append(DecryptChar(c, encryptionKey));
            }

            return sb.ToString();
        }

        private static char EncryptChar(char c, char key)
        {
            int shift = key - LatinBase;
            return (char)((c + shift - CyrillicBase) % AlphabetLength + LatinBase);
        }

        private static char DecryptChar(char c, char key)
        {
            int shift = key - LatinBase;
            int decryptedValue = (c - shift - LatinBase + AlphabetLength) % AlphabetLength + CyrillicBase;
            return (char)decryptedValue;
        }

        private static char GenerateRandomKey()
        {
            int randomInt = Random.Range(0, 36);

            if (randomInt < 26)
            {
                return (char)(LatinBase + randomInt);
            }

            return (char)('0' + randomInt - 26);
        }
    }
}