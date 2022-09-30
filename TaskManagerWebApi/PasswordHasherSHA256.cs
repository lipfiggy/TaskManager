using System.Security.Cryptography;
using System.Text;

namespace TaskManagerWebApi
{
    public class PasswordHasherSHA256 : IPasswordHasher
    {
        public string GetHashOfAPassword(string password)
        {
            byte[] passwordInBytes = Encoding.Default.GetBytes(password);
            return Convert.ToBase64String(SHA256.Create().ComputeHash(passwordInBytes));
        }
    }
}
