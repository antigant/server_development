using System.Security.Cryptography;

namespace CustomPlugin
{
    class RNG
    {
        public string GenerateRandomCryptographicKey(int keyLength)
        {
            return System.Convert.ToBase64String(GenerateRandomCryptographicBytes(keyLength));
        }

        public byte[] GenerateRandomCryptographicBytes(int keyLength)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[keyLength];
            rng.GetBytes(bytes);
            return bytes;
        }
    }
}
