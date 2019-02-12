using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace CustomPlugin
{
    public class EncryptPassword
    {
        // used for registration
        public HashWithSaltResult HashWithSalt(string password, int saltLength, HashAlgorithm hashAlgo)
        {
            RNG rng = new RNG();

            // save this salt value in db
            string salt = rng.GenerateRandomCryptographicKey(saltLength);

            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            List<byte> passwordWithSaltBytes = new List<byte>();
            passwordWithSaltBytes.AddRange(passwordBytes);
            passwordWithSaltBytes.AddRange(saltBytes);

            // final output (hashed password with salt), save this in db as well
            byte[] digestBytes = hashAlgo.ComputeHash(passwordWithSaltBytes.ToArray());

            return new HashWithSaltResult(salt, Convert.ToBase64String(digestBytes));
        }

        // used for login
        public string GetDigest(string password, string salt, HashAlgorithm hashAlgo)
        {
            if (password == "" || salt == "")
                return "NIL";

            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            List<byte> passwordWithSaltBytes = new List<byte>();
            passwordWithSaltBytes.AddRange(passwordBytes);
            passwordWithSaltBytes.AddRange(saltBytes);

            byte[] digestBytes = hashAlgo.ComputeHash(passwordWithSaltBytes.ToArray());

            return Convert.ToBase64String(digestBytes);
        }
    }
}
