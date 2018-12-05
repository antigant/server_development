using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomPlugin
{
    class General
    {
        public const int INTEGER_BYTE = 1; // Unsigned (0 to 255)
        public const int INTEGER_SBYTE = 2; // Signed (-128 to 127)
        public const int INTEGER_SHORT = 2; // Signed (-32,768 to 32,767)
        public const int INTEGER_USHORT = 2; // Unsigned (0 to 65,535)
        public const int INTEGER_INT = 4; // Singed (-2,147,483,648 to 2,147,483,647)
        public const int INTEGER_UINT = 4; // Unsigned (0 to 4,294,967,295)
        public const int INTEGER_LONG = 8; //  Signed (-9,223,372,036,854,775,808 to 9,223,372,036,854,775,807)
        public const int INTEGER_ULONG = 8; // Unsinged (0 to 18,446,744,073,709,551,615)
        public const int FLOAT_FLOAT = 4; // ±1.5e−45 to ±3.4e38  (Precision:7 digits)
        public const int FLOAT_DOUBLE = 8; // ±5.0e−324 to ±1.7e308 (Precision:15-16 digits)
        public const int FLOAT_DECIMAL = 16; // (-7.9 x 1028 to 7.9 x 1028) / (100 to 28) (Precision:28-29 digits)
        public const int CHARACTER_CHAR = 2;
        public const int OTHER_DATETIME = 8;
        public const int OTHER_BOOL = 1;

        public static string GetStringDataFromMessage(string message, string returnData)
        {
            string temp = returnData + "=";
            int pFrom = message.IndexOf(temp) + temp.Length;
            int pTo = message.LastIndexOf(",");

            // there's no comma at the end
            if (pTo - pFrom < 0)
            {
                pFrom = message.LastIndexOf(temp) + temp.Length;
                return message.Substring(pFrom);
            }

            return message.Substring(pFrom, pTo - pFrom);
        }
    }
}
