using BackendOperacionesFroward.Controllers.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BackendOperacionesFroward
{
    public static class AuxiliarMethodsSecurity
    {
        public static string GetHash256(string input)
        {
            SHA256 sha256Hash = SHA256.Create();

            byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString());

            return sBuilder.ToString();
        }

        public static string DecodeFrom64(string encodedData)
        {
            UTF8Encoding encoder = new();
            Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new(decoded_char);
            return result;
        }

        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
            }
            catch (Exception)
            {
                throw new ErrorHttpResponseException(StringError.BASE64_ERROR);
            }
        }
    }
}
