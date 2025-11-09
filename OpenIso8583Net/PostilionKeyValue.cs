using System.Collections.Generic;
using System.Text;

namespace OpenIso8583Net
{
    /// <summary>
    /// unpack the 127.22 data from it's Postilion key value pair
    /// </summary>
    /// <remarks>
    /// The physical layout of the field is as follows:
    ///     * 1 byte length indicator of the key length indicator
    ///     * Length indicator of the key
    ///     * Key
    ///     * 1 byte length indicator of the value length indicator
    ///     * Length indicator of the value
    ///     * Value
    /// 
    /// repeat until nothing left
    /// </remarks>
    public static class PostilionKeyValue
    {
        /// <summary>
        /// Parses the Postilion Key Value pair from the string provided
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseData(string data)
        {
            var res = new Dictionary<string, string>();

            string remainingData = data;
            while (remainingData.Length > 0)
            {
                int keyBytes = int.Parse(remainingData[..1]);
                int keyLength = int.Parse(remainingData.Substring(1, keyBytes));
                string key = remainingData.Substring(1 + keyBytes, keyLength);

                remainingData = remainingData[(1 + keyBytes + keyLength)..];

                int valBytes = int.Parse(remainingData[..1]);
                int valLength = int.Parse(remainingData.Substring(1, valBytes));
                string val = remainingData.Substring(1 + valBytes, valLength);

                remainingData = remainingData[(1 + valBytes + valLength)..];

                res.Add(key, val);
            }

            return res;
        }

        /// <summary>
        /// Packs a dictionary&lt;string,string&gt; into the Postilion packed key value format for adding to the 127.22 fields
        /// </summary>
        public static string ToMsg(this Dictionary<string, string> data)
        {
            var sb = new StringBuilder();
            foreach (var key in data.Keys)
            {
                // the lengths of the key and value as a string
                var keyLen = key.Length.ToString();
                var valLen = data[key].Length.ToString();

                sb.Append(keyLen.Length); // number of digits representing the length of the key
                                          // e.g. if the length of the key is 100, this will have a value of 3 as the length is 3 digits long
                                          // or if the length of the key string is 18, this will have a value of 2 as the length is 2 digits long
                sb.Append(keyLen);        // the key length in digits (as per above example 100 or 18)
                sb.Append(key);           // the key 

                // put together as per the key above, just using the value instead
                sb.Append(valLen.Length); // number of digits representing the length of the value
                sb.Append(valLen);        // the value length
                sb.Append(data[key]);     // the value
            }
            return sb.ToString();
        }

    }
}
