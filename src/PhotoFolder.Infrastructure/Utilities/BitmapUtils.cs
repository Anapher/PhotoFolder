//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Globalization;
//using System.Text;

//namespace PhotoFolder.Infrastructure.Utilities
//{
//    public static class BitmapUtils
//    {
//        // This seems to cover all known Exif date strings
//        // Note that "    :  :     :  :  " is a valid date string according to the Exif spec (which means 'unknown date'): http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/datetimeoriginal.html
//        // Custom format reference: https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
//        private static readonly string[] _datePatterns =
//        {
//            "yyyy:MM:dd HH:mm:ss.fff",
//            "yyyy:MM:dd HH:mm:ss",
//            "yyyy:MM:dd HH:mm",
//            "yyyy-MM-dd HH:mm:ss",
//            "yyyy-MM-dd HH:mm",
//            "yyyy.MM.dd HH:mm:ss",
//            "yyyy.MM.dd HH:mm",
//            "yyyy-MM-ddTHH:mm:ss.fff",
//            "yyyy-MM-ddTHH:mm:ss.ff",
//            "yyyy-MM-ddTHH:mm:ss.f",
//            "yyyy-MM-ddTHH:mm:ss",
//            "yyyy-MM-ddTHH:mm.fff",
//            "yyyy-MM-ddTHH:mm.ff",
//            "yyyy-MM-ddTHH:mm.f",
//            "yyyy-MM-ddTHH:mm",
//            "yyyy:MM:dd",
//            "yyyy-MM-dd",
//            "yyyy-MM",
//            "yyyyMMdd", // as used in IPTC data
//            "yyyy"
//        };

//        public static bool TryParseDateTime(string value, out DateTimeOffset dateTime)
//        {
//            if (DateTimeOffset.TryParseExact(value, _datePatterns, null, DateTimeStyles.AllowWhiteSpaces, out dateTime))
//                return true;

//            dateTime = default;
//            return false;
//        }

//        public static bool TryParseDateTime(this PropertyItem propertyItem, out DateTimeOffset dateTime)
//        {
//            var str = Encoding.UTF8.GetString(propertyItem.Value);
//            return TryParseDateTime(str, out dateTime);
//        }

//        public static bool TryGetDateTime(this Bitmap bitmap, int propertyId, out DateTimeOffset dateTime)
//        {
//            try
//            {
//                var propertyItem = bitmap.GetPropertyItem(propertyId);
//                return propertyItem.TryParseDateTime(out dateTime);
//            }
//            catch (Exception)
//            {
//                dateTime = default;
//                return false;
//            }
//        }
//    }
//}
