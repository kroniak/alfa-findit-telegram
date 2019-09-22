namespace AlfaBot.Host.Utils
{
    internal static class Utils
    {
        /// <summary>
        /// Mask the mobile.
        /// Usage: MaskMobile("13456789876", 3, "****") => "134****9876"
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="startIndex"></param>
        /// <param name="mask"></param>
        /// <returns>Masked phone</returns>
        public static string MaskMobile(this string mobile, int startIndex, string mask)
        {
            if (string.IsNullOrEmpty(mobile))
                return string.Empty;

            var result = mobile;
            var maskLength = mask.Length;


            if (mobile.Length < startIndex) return result;

            result = mobile.Insert(startIndex, mask);
            result = result.Length >= startIndex + maskLength * 2
                ? result.Remove(startIndex + maskLength, maskLength)
                : result.Remove(startIndex + maskLength, result.Length - (startIndex + maskLength));

            return result;
        }
    }
}