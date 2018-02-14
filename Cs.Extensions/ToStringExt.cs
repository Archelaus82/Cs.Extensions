using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs.Extensions
{
    public static class ToStringExt
    {
        /// <summary>
        /// Methods returns the binary string representation of the byte value
        /// </summary>
        /// <param name="value">byte - value to modify</param>
        /// <param name="with_0x">bool - ture=0b00/flase=00</param>
        /// <returns>string - binary representation</returns>
        public static string ToBinaryString(this byte value, bool with_0b, int length = 0)
        {
            string binary = Convert.ToString(value, 2).PadLeft(length, '0');
            if (with_0b) return string.Format("0b{0}", binary);
            else return string.Format("{0}", binary);
        }

        /// <summary>
        /// Methods returns the hex string representation of the byte value
        /// </summary>
        /// <param name="value">byte - value to modify</param>
        /// <param name="with_0x">bool - ture=0x00/flase=00</param>
        /// <returns>string - hex representation</returns>
        public static string ToHexString(this byte value, bool with_0x, int length = 0)
        {
            if (with_0x) return string.Format("0x{0:X" + length + "}", value);
            else return string.Format("{0:X2}", value);
        }
    }
}
