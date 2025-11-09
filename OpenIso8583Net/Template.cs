// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Template.cs" company="John Oxley">
//   2012
// </copyright>
// <summary>
//   A Template describing a message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenIso8583Net
{
    using OpenIso8583Net.Formatter;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A Template describing a message
    /// </summary>
    [Serializable]
    public class Template : Dictionary<int, IFieldDescriptor>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class. 
        /// </summary>
        public Template()
        {
            this.MsgTypeFormatter = Formatters.Ascii;
            this.BitmapFormatter = Formatters.Binary;
        }

        public Template(IFormatter msgTypeFormatter) : base()
        {
            this.MsgTypeFormatter = msgTypeFormatter;
            this.BitmapFormatter = Formatters.Binary;
        }

        protected Template(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            this.MsgTypeFormatter = Formatters.Ascii;
            this.BitmapFormatter = Formatters.Binary;
        }

        #endregion

        #region Public Properties


        /// <summary>
        /// Gets or sets the message type formatter
        /// </summary>
        public IFormatter MsgTypeFormatter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bitmap formatter
        /// </summary>
        public IFormatter BitmapFormatter { get; set; }

        /// <summary>
        /// gets or sets the bitmap length, default of 8 as per ISO8583
        /// </summary>
        public int BitmapLength { get; set; } = 8;


        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Describe the packing format of the template
        /// </summary>
        /// <returns>
        /// The packing of the template 
        /// </returns>
        public string DescribePacking()
        {
            var sb = new StringBuilder();

            foreach (var kvp in this)
            {
                var field = kvp.Key;
                var descriptor = kvp.Value;
                sb.AppendLine(descriptor.Display(string.Empty, field, string.Empty));
            }

            return sb.ToString();
        }



        #endregion
    }
}