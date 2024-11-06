using System.Text.RegularExpressions;

namespace authentication_server.Common.Constants
{
    public class Regexp
    {
        public static readonly string Lowercase = "(?=.*[a-z])";  // At least one lowercase letter
        public static readonly string Uppercase = "(?=.*[A-Z])";  // At least one uppercase letter
        public static readonly string Digit = "(?=.*\\d)";        // At least one digit
        public static readonly string SpecialChar = "(?=.*[@$!%?&])";  // At least one special character
        public static readonly string MinLength = "[A-Za-z\\d@$!%?&]{8,}";  // Minimum length of 8 characters

        public static readonly Regex PasswordStrength = new(
            Lowercase + Uppercase + Digit + SpecialChar + MinLength,
            RegexOptions.Compiled
        );
    }
}
