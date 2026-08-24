using System.Security.Cryptography;
using System.Text;

namespace SchurkoPortfolio.Core.Identity
{
    public static class StringExtensions
    {
        /// <summary>
        /// Computes the SHA-256 hash of the input string and returns it as a lowercase hex string.
        /// Returns empty string when input is null or empty.
        /// </summary>
        public static string ToSha256(this string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);

            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        // Backwards-compatibility: some code may call the misspelled ToShe256()
        public static string ToShe256(this string? input) => ToSha256(input);
    }
}
