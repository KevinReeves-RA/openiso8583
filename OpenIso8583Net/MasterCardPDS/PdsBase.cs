using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenIso8583Net.MasterCardPDS
{
    public abstract class PdsBase<T> where T : class, new()
    {
        public abstract string PdsId { get; }
        public abstract string PdsName { get; }

        public abstract string PdsValue { get; set; }

        public override string ToString()
        {
            string prefix = "                                       ";
            Type typ = this.GetType();
            PropertyInfo[] props = typ.GetProperties();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(prefix + "PDS " + PdsId + " : " + PdsName);
            sb.AppendLine(prefix + "    [" + PdsValue + "]");
            foreach (var prop in props.Where(w => w.Name != nameof(PdsId) && w.Name != nameof(PdsName) && w.Name != nameof(PdsValue)))
                sb.AppendLine(prefix + "        " + prop.Name + " : " + prop.GetValue(this)?.ToString());
            return sb.ToString();
        }

        public static T Parse(string value) { throw new NotImplementedException(); }
        public string ToMsg() { throw new NotImplementedException(); }
    }


}
