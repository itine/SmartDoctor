namespace SmartDoctor.Helper
{
    public class SecurePasswordHasher
    {
        public static string HashPassword(string password)
        {
            return password;
            //var salt = BCryptHelper.GenerateSalt(4);
            //return BCryptHelper.HashPassword(password, salt);
        }
        public static bool ValidatePassword(string password, string correctHash)
        {
            return password == correctHash;
            //return BCryptHelper.CheckPassword(password, correctHash);
        }
    }
}
