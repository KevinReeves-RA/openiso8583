using System.Collections.Generic;
using System.Text;

namespace OpenIso8583Net
{
    /// <summary>
    ///   This class parses Mini statement data in field 48 in the response message
    /// </summary>
    public class MinistatementData : List<MinistatementLine>
    {
        public static MinistatementData Unpack(string data)
        {
            var res = new MinistatementData();
            res.FromMsg(data);
            return res;
        }

        /// <summary>
        ///   Parse the data out of the message
        /// </summary>
        /// <param name = "msg">Data to parse</param>
        public void FromMsg(string msg)
        {
            // Headers

            var lines = msg.Split('~');

            var header = lines[0];
            var headings = header.Split('|');

            // Through each line
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var parts = line.Split('|');
                if (parts.Length == 1) // If the line is empty, don't bother
                    continue;

                var msLine = new MinistatementLine();
                for (var j = 0; j < headings.Length; j++)
                {
                    var heading = headings[j];
                    if (j < parts.Length)
                    {
                        var data = parts[j];
                        if (string.IsNullOrEmpty(data))
                            data = null;

                        msLine.Add(heading, data!);
                    }
                    else
                        msLine.Add(heading, null!);
                }
                Add(msLine);
            }
        }

        /// <summary>
        /// Convert back to a message
        /// </summary>
        /// <returns></returns>
        public string ToMsg()
        {
            if (this.Count == 0)
                return null!;

            StringBuilder msg = new StringBuilder();
            msg.Append(string.Join('|', this[0].Keys));

            foreach (var line in this)
            {
                msg.Append('~');
                msg.Append(string.Join('|', line.Values));
            }
            return msg.ToString();
        }
    }
}