using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Cs.Extensions
{
    public class ManipulationExtensionException : Exception
    {
        public ManipulationExtensionException(Exception inner)
            : base("Manipulation Extension Error", inner)
        {
        }

        public ManipulationExtensionException(string message)
            : base(message)
        {
        }

        public ManipulationExtensionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public static class ManipulationExt
    {
        #region [ Binary Methods ]

        public static byte ReadFirstNBits(this byte value, int n)
        {
            try
            {
                int length = value.ToBinaryString(false).Length;
                if (n > length)
                    throw new ManipulationExtensionException(string.Format("Bits requested [{0}] is larger than available [{1}]", n, length));

                return (byte)(value >> (length - n));
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        public static byte ReadLastNBits(this byte value, int n)
        {
            try
            {
                int length = value.ToBinaryString(false).Length;
                if (n > length)
                    throw new ManipulationExtensionException(string.Format("Bits requested [{0}] is larger than available [{1}]", n, length));

                byte mask = (byte)((1 << n) - 1);
                return (byte)(value & mask);
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        #endregion

        #region [ List Methods ]

        public static List<TOut> ConvertTo<TOut, TIn>(this List<TIn> list)
        {
            try
            {
                return list.Cast<TOut>().ToList();
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        #endregion

        #region [ Array Methods ]

        /// <summary>
        /// Method performs the specified method on each value of the arrary and returns the new array
        /// Example double[] myarray = floatArray.ForEach(val => (double)val);
        /// </summary>
        /// <typeparam name="TOut">type - return type</typeparam>
        /// <typeparam name="TIn">type - input type</typeparam>
        /// <param name="array">array - array to run operation on</param>
        /// <param name="methodToRun">method - method to run</param>
        /// <returns>OutType[] - new array</returns>
        public static TOut[] ForEach<TOut, TIn>(this TIn[] array, Func<TIn, TOut> methodToRun)
        {
            try
            {
                List<TOut> list = new List<TOut>();
                foreach (TIn value in array)
                {
                    list.Add(methodToRun(value));
                }
                return list.ToArray();
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        /// <summary>
        /// Method converts the input type to the return type for each value of the array
        /// </summary>
        /// <typeparam name="OutType">type - return type</typeparam>
        /// <typeparam name="InType">type - input type</typeparam>
        /// <param name="array">array - array to convert</param>
        /// <returns>OutType[] - new array</returns>
        public static OutType[] ConvertTo<OutType, InType>(this InType[] array)
        {
            try
            {
                List<OutType> list = new List<OutType>();
                foreach (InType value in array)
                {
                    list.Add((OutType)Convert.ChangeType(value, typeof(OutType)));
                }
                return list.ToArray();
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        /// <summary>
        /// Mehtod retreives a sub array
        /// </summary>
        /// <typeparam name="T">type - array type</typeparam>
        /// <param name="array">array - parent array</param>
        /// <param name="index">int - start position</param>
        /// <param name="length">int - length of sub array</param>
        /// <returns>array - child array</returns>
        public static T[] SubArray<T>(this T[] array, int index, int length)
        {
            try
            {
                T[] subArray = new T[length];
                Array.Copy(array, index, subArray, 0, length);
                return subArray;
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        #endregion

        #region [ String Methods ]

        /// <summary>
        /// Method takes in any array type and outputs a deliminated string using the char input as deliminating value
        /// </summary>
        /// <typeparam name="T">Type - array type</typeparam>
        /// <param name="array">[] - array to join</param>
        /// <param name="joinChar">char - char to join items in array</param>
        /// <returns></returns>
        public static string Join<T>(this T[] array, char joinChar)
        {
            try
            {
                string str = "";
                foreach (T item in array)
                {
                    str += item.ToString() + joinChar;
                }

                return str.Substring(0, str.Length - 1);
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        /// <summary>
        /// Method appends the date and time to the end of the file name
        /// </summary>
        /// <param name="str">string - filename</param>
        /// <returns>string - filename with date and time</returns>
        public static string AppendDateTimeStamp(this String str)
        {
            try
            {
                string time = DateTime.Now.ToString("_hhmmss"); // includes leading zeros 
                string date = DateTime.Now.ToString("MM_dd_yyyy"); // includes leading zeros 
                date = date + time;

                int index = str.LastIndexOf('.');

                if (index == -1)
                    str = str + "_" + date;
                else
                {
                    string name = Path.GetFileNameWithoutExtension(str);
                    string ext = Path.GetExtension(str);

                    str = name + "_" + date;
                    str += "." + ext;
                }

                return str;
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        #endregion

        #region [ IEnumerable Methods ]

        /// <summary>
        /// Method removes the specified number of top and bottom values
        /// </summary>
        /// <param name="values">IEnumerable - values to modify</param>
        /// <param name="lowest">int - number of low vaules to remove</param>
        /// <param name="highest">int - number of high values to remove</param>
        /// <returns>IEnumerable - modified values</returns>
        public static IEnumerable<double> RemoveOutliers(this IEnumerable<double> values, int lowest, int highest)
        {
            try
            {
                List<double> list = values.ToList();
                List<double> orderedList = list.OrderBy(num => num).ToList();

                for (int i = 0; i < lowest; i++)
                {
                    list.Remove(orderedList[i]);
                }

                for (int i = 0; i < highest; i++)
                {
                    list.Remove(orderedList[(orderedList.Count - 1) - i]);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        /// <summary>
        /// Removes all empty elements
        /// </summary>
        /// <param name="values">IEnumerable - values to modify</param>
        /// <returns>IEnumerable - modified values</returns>
        public static IEnumerable<string> RemoveEmptyElements(this IEnumerable<string> values)
        {
            try
            {
                List<string> list = values.ToList();
                list.RemoveAll(s => s.Equals(""));

                return list;
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        /// <summary>
        /// Removes elements equal to the speficied string
        /// </summary>
        /// <param name="values">IEnumerable - values to modify</param>
        /// <param name="str">string - comparison string</param>
        /// <returns>IEnumerable - modified values</returns>
        public static IEnumerable<string> RemoveElementsEqualTo(this IEnumerable<string> values, string str)
        {
            try
            {
                List<string> list = values.ToList();
                list.RemoveAll(s => s.Equals(str));

                return list;
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        /// <summary>
        /// Removes elements containg the specified string
        /// </summary>
        /// <param name="values">IEnumerable - values to modify</param>
        /// <param name="str">string - comparison string</param>
        /// <returns>IEnumerable - modified values</returns>
        public static IEnumerable<string> RemoveElementsContaining(this IEnumerable<string> values, string str)
        {
            try
            {
                List<string> list = values.ToList();
                list.RemoveAll(s => s.Contains(str));

                return list;
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        /// <summary>
        /// Method keep only the values containing the specified string
        /// </summary>
        /// <param name="values">IEnumerable - values to modify</param>
        /// <param name="str">string - comparison string</param>
        /// <returns>IEnumerable - modified values</returns>
        public static IEnumerable<string> KeepOnlyElementsContaining(this IEnumerable<string> values, string str)
        {
            try
            {
                List<string> list = values.ToList();
                list.RemoveAll(s => !s.Contains(str));

                return list;
            }
            catch (Exception ex)
            {
                throw new ManipulationExtensionException(ex);
            }
        }

        #endregion
    }
}
