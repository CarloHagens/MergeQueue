using System.Text;

namespace MergeQueue.Api.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToHexString(this IEnumerable<byte> bytes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var @byte in bytes)
            {
                stringBuilder.Append(@byte.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}
