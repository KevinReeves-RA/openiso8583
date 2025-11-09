// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldDescriptor.cs" company="John Oxley">
//   2012
// </copyright>
// <summary>
//   A class describing a field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenIso8583Net
{
    using OpenIso8583Net.Exceptions;
    using OpenIso8583Net.FieldValidator;
    using OpenIso8583Net.Formatter;
    using OpenIso8583Net.LengthFormatters;
    using System;
    using System.Text;

    /// <summary>
    /// A class describing a field
    /// </summary>
    public class FieldDescriptor : IFieldDescriptor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDescriptor"/> class.
        /// </summary>
        /// <param name="lengthFormatter">
        /// The length formatter. 
        /// </param>
        /// <param name="validator">
        /// The validator. 
        /// </param>
        /// <param name="formatter">
        /// The formatter. 
        /// </param>
        /// <param name="adjuster">
        /// The adjuster. 
        /// </param>
        public FieldDescriptor(ILengthFormatter lengthFormatter, IFieldValidator validator, IFormatter formatter, Adjuster? adjuster, string displayName = "")
        {
            if (formatter is BinaryFormatter && validator is not HexFieldValidator)
            {
                throw new FieldDescriptorException("A Binary field must have a hex validator");
            }

            if (formatter is BcdFormatter && validator is not NumericFieldValidator)
            {
                throw new FieldDescriptorException("A BCD field must have a numeric validator");
            }

            this.LengthFormatter = lengthFormatter;
            this.Validator = validator;
            this.Formatter = formatter;
            this.DisplayName = displayName;
            Adjuster = adjuster;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the Field Adjuster (may be null)
        /// </summary>
        public Adjuster? Adjuster { get; private set; }

        /// <summary>
        ///   Gets the field formatter describing the field
        /// </summary>
        public virtual IFormatter Formatter { get; set; }

        /// <summary>
        ///   Gets the length formatter describing the field
        /// </summary>
        public virtual ILengthFormatter LengthFormatter { get; set; }

        /// <summary>
        ///   Gets the validator describing the field
        /// </summary>
        public virtual IFieldValidator Validator { get; private set; }

        /// <summary>
        /// The Field Display Name
        /// </summary>
        public virtual string DisplayName { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Create ASCII packed fixed length auto-padding alphanumeric field (AN)
        /// </summary>
        /// <param name="length">
        /// length of the field 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor AsciiAlphaNumeric(int length, string displayName = "")
        {
            var setAdjuster = new LambdaAdjuster(setLambda: value => value.PadRight(length, ' '));
            return Create(new FixedLengthFormatter(length), FieldValidators.Ansp, Formatters.Ascii, setAdjuster, displayName);
        }

        /// <summary>
        /// Create ASCII packed fixed length auto-padding amount field (AMOUNT)
        /// </summary>
        /// <param name="length">
        /// length of the field 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor AsciiAmount(int length, string displayName = "")
        {
            var setAdjuster = new LambdaAdjuster(setLambda: value => value.PadLeft(length, '0'));
            return Create(new FixedLengthFormatter(length), FieldValidators.Rev87AmountValidator, Formatters.Ascii, setAdjuster, displayName);
        }

        /// <summary>
        /// Create an ASCII fixed length field descriptor
        /// </summary>
        /// <param name="packedLength">
        /// The packed length of the field. For BCD fields, this is half the size of the field you want 
        /// </param>
        /// <param name="validator">
        /// Validator to use on the field 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor AsciiFixed(int packedLength, IFieldValidator validator, string displayName = "")
        {
            return Create(new FixedLengthFormatter(packedLength), validator, Formatters.Ascii, displayName);
        }

        /// <summary>
        /// Create ASCII packed variable length LL alphanumeric field (LL CHAR)
        /// </summary>
        /// <param name="maxLength">
        /// maximum field length 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor AsciiLlCharacter(int maxLength)
        {
            return AsciiVar(2, maxLength, FieldValidators.Ans);
        }

        /// <summary>
        /// Create ASCII packed variable length LL numeric field (LL NUM)
        /// </summary>
        /// <param name="maxLength">
        /// maximum field length 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor AsciiLlNumeric(int maxLength)
        {
            return AsciiVar(2, maxLength, FieldValidators.N);
        }

        /// <summary>
        /// Create ASCII packed variable length binary LLL field (LLL BINARY)
        /// </summary>
        /// <param name="packedLength">
        /// length of the field 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor AsciiLllBinary(int packedLength)
        {
            return Create(new VariableLengthFormatter(3, packedLength, Formatters.Ascii), FieldValidators.Hex, Formatters.Binary);
        }

        /// <summary>
        /// Create ASCII packed variable length LLL alphanumeric field (LLL CHAR)
        /// </summary>
        /// <param name="maxLength">
        /// maximum field length 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor AsciiLllCharacter(int maxLength)
        {
            return AsciiVar(3, maxLength, FieldValidators.Ans);
        }

        /// <summary>
        /// Create ASCII packed variable length LLL numeric field (LLL NUM)
        /// </summary>
        /// <param name="maxLength">
        /// maximum field length 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor AsciiLllNumeric(int maxLength)
        {
            return AsciiVar(3, maxLength, FieldValidators.N);
        }

        /// <summary>
        /// Create ASCII packed fixed length auto-padding numeric field (N)
        /// </summary>
        /// <param name="length">
        /// length of the field 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor AsciiNumeric(int length, string displayName = "")
        {
            var setAdjuster = new LambdaAdjuster(setLambda: value => value.PadLeft(length, '0'));
            return Create(new FixedLengthFormatter(length), FieldValidators.N, Formatters.Ascii, setAdjuster, displayName);
        }

        /// <summary>
        /// Create an ASCII variable length field descriptor
        /// </summary>
        /// <param name="lengthIndicator">
        /// length indicator 
        /// </param>
        /// <param name="maxLength">
        /// maximum length of the field 
        /// </param>
        /// <param name="validator">
        /// Validator to use on the field 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor AsciiVar(int lengthIndicator, int maxLength, IFieldValidator validator, string displayName = "")
        {
            return Create(new VariableLengthFormatter(lengthIndicator, maxLength, Formatters.Ascii), validator, Formatters.Ascii, displayName);
        }

        /// <summary>
        /// Create a Binary variable length field descriptor
        /// </summary>
        /// <param name="lengthIndicator">length indicator</param>
        /// <param name="maxLength">max length of field</param>
        /// <param name="validator">validator to use on the field</param>
        /// <returns>field descriptor</returns>
        public static IFieldDescriptor BinaryVar(int lengthIndicator, int maxLength, IFieldValidator validator, string displayName = "")
        {
            return new FieldDescriptor(new VariableLengthFormatter(lengthIndicator, maxLength, Formatters.Ascii), FieldValidators.Hex, Formatters.Binary, null, displayName);
        }

        /// <summary>
        /// The bcd fixed.
        /// </summary>
        /// <param name="length">
        /// The length. 
        /// </param>
        /// <returns>
        /// </returns>
        public static IFieldDescriptor BcdFixed(int length, string displayName = "")
        {
            return Create(new FixedLengthFormatter(length), FieldValidators.N, Formatters.Bcd, null, displayName);
        }

        /// <summary>
        /// The bcd var.
        /// </summary>
        /// <param name="lengthIndicator">
        /// The length indicator. 
        /// </param>
        /// <param name="maxLength">
        /// The max length. 
        /// </param>
        /// <param name="lengthFormatter">
        /// The length formatter. 
        /// </param>
        /// <returns>
        /// </returns>
        public static IFieldDescriptor BcdVar(int lengthIndicator, int maxLength, IFormatter lengthFormatter, string displayName = "")
        {
            return Create(new VariableLengthFormatter(lengthIndicator, maxLength, lengthFormatter), FieldValidators.N, Formatters.Bcd, null, displayName);
        }

        /// <summary>
        /// Create a binary fixed length field
        /// </summary>
        /// <param name="packedLength">
        /// length of the field 
        /// </param>
        /// <returns>
        /// field descriptor 
        /// </returns>
        public static IFieldDescriptor BinaryFixed(int packedLength, string displayName = "")
        {
            return Create(new FixedLengthFormatter(packedLength), FieldValidators.Hex, Formatters.Binary, displayName);
        }

        /// <summary>
        /// Create a field descriptor
        /// </summary>
        /// <param name="lengthFormatter">
        /// The length formatter. 
        /// </param>
        /// <param name="fieldValidator">
        /// The field validator. 
        /// </param>
        /// <param name="formatter">
        /// The formatter. 
        /// </param>
        /// <param name="adjuster">
        /// The adjuster. 
        /// </param>
        /// <returns>
        /// A field descriptor 
        /// </returns>
        public static IFieldDescriptor Create(ILengthFormatter lengthFormatter, IFieldValidator fieldValidator, IFormatter formatter, Adjuster? adjuster, string displayName)
        {
            return new FieldDescriptor(lengthFormatter, fieldValidator, formatter, adjuster, displayName);
        }

        /// <summary>
        /// Create a Field descriptor.
        /// </summary>
        /// <param name="lengthFormatter">
        /// The length formatter. 
        /// </param>
        /// <param name="fieldValidator">
        /// The field validator. 
        /// </param>
        /// <param name="formatter">
        /// The formatter. 
        /// </param>
        /// <returns>
        /// Field descriptor 
        /// </returns>
        public static IFieldDescriptor Create(ILengthFormatter lengthFormatter, IFieldValidator fieldValidator, IFormatter formatter, string displayName = "")
        {
            return new FieldDescriptor(lengthFormatter, fieldValidator, formatter, null, displayName);
        }

        /// <summary>
        /// Decorates IFieldDescriptor.Display method with a PCI-DSS PAN mask.
        /// </summary>
        /// <param name="decoratedFieldDescriptor">
        /// The decorated Field Descriptor. 
        /// </param>
        /// <returns>
        /// The pan mask. 
        /// </returns>
        public static IFieldDescriptor PanMask(IFieldDescriptor decoratedFieldDescriptor, string displayName)
        {
            return new PanMaskDecorator(decoratedFieldDescriptor, displayName);
        }

        /// <summary>
        /// Display the field, used in traces
        /// </summary>
        /// <param name="prefix">
        /// Prefix to display before the field 
        /// </param>
        /// <param name="fieldNumber">
        /// Field number 
        /// </param>
        /// <param name="value">
        /// field contents 
        /// </param>
        /// <returns>
        /// formatted string representing the field 
        /// </returns>
        public virtual string Display(string prefix, int fieldNumber, string value)
        {
            // always use StringBuffer's Append(string) to concatenate user data
            // Formatting methods like string.Format("{0}", data) may print garbage when the data contains format characters
            var fieldValue = value == null ? string.Empty : new StringBuilder().Append('[').Append(value).Append(']').ToString();

            return new StringBuilder().AppendFormat("{0}[{1,-6} {2,-7} {3,6} {4:d4}] {5:d3} '{7,-15}' {6}", prefix, this.LengthFormatter.Description, this.Validator.Description, this.LengthFormatter.MaxLength, this.Formatter.GetPackedLength(value == null ? 0 : value.Length), fieldNumber, fieldValue, DisplayName).ToString();
        }

        /// <summary>
        /// Get the packed length of the field, including a length header if necessary for the given value
        /// </summary>
        /// <param name="value">
        /// Value to calculate length for 
        /// </param>
        /// <returns>
        /// Packed length of the field, including length header 
        /// </returns>
        public virtual int GetPackedLength(string value)
        {
            return this.LengthFormatter.LengthOfLengthIndicator + this.Formatter.GetPackedLength(value.Length);
        }

        /// <summary>
        /// Packs the field into a byte[]
        /// </summary>
        /// <param name="fieldNumber">
        /// number of the field 
        /// </param>
        /// <param name="value">
        /// Value of the field to pack 
        /// </param>
        /// <returns>
        /// field data packed into a byte[] 
        /// </returns>
        public virtual byte[] Pack(int fieldNumber, string value)
        {
            if (!this.LengthFormatter.IsValidLength(this.Formatter.GetPackedLength(value.Length)))
            {
                throw new FieldLengthException(fieldNumber, "The field length is not valid");
            }

            if (!this.Validator.IsValid(value))
            {
                throw new FieldFormatException(fieldNumber, "Invalid value for field");
            }

            var lenOfLenInd = this.LengthFormatter.LengthOfLengthIndicator;
            var lengthOfField = this.Formatter.GetPackedLength(value.Length);
            var field = new byte[lenOfLenInd + lengthOfField];
            this.LengthFormatter.Pack(field, lengthOfField, 0);
            var fieldData = this.Formatter.GetBytes(value);
            Array.Copy(fieldData, 0, field, lenOfLenInd, lengthOfField);
            return field;
        }

        /// <summary>
        /// Unpack the field from the message
        /// </summary>
        /// <param name="fieldNumber">
        /// field number 
        /// </param>
        /// <param name="data">
        /// message data to unpack from 
        /// </param>
        /// <param name="offset">
        /// offset in the message to start 
        /// </param>
        /// <param name="newOffset">
        /// offset at the end of the field for the next field 
        /// </param>
        /// <returns>
        /// valud of the field 
        /// </returns>
        public virtual string Unpack(int fieldNumber, byte[] data, int offset, out int newOffset)
        {
            var lenOfLenInd = this.LengthFormatter.LengthOfLengthIndicator;
            var unpackedLengthOfField = this.LengthFormatter.GetLengthOfField(data, offset);
            var lengthOfField = unpackedLengthOfField;
            if (this.Formatter is BcdFormatter)
                lengthOfField = this.Formatter.GetPackedLength(lengthOfField);

            var fieldData = new byte[lengthOfField];
            Array.Copy(data, offset + lenOfLenInd, fieldData, 0, lengthOfField);
            newOffset = offset + lengthOfField + lenOfLenInd;
            var value = this.Formatter.GetString(fieldData);
            if (!this.Validator.IsValid(value))
                throw new FieldFormatException(fieldNumber, $"Invalid field format {Validator.Description} : {value}");

            var length = value.Length;
            if (this.Formatter is BcdFormatter)
            {
                length = this.Formatter.GetPackedLength(value.Length);

                // This is here because if the length of a BCD or binary field is odd, 
                // we need to strip the first character off which would've been padding
                if (unpackedLengthOfField % 2 != 0)
                    value = value.Substring(1);
            }

            if ((this.LengthFormatter is VariableLengthFormatter) && !this.LengthFormatter.IsValidLength(length))
                throw new FieldLengthException(fieldNumber, "Field is too long");

            return value;
        }


        #endregion

        #region Generic 
        public static IFieldDescriptor VarilableLength(int lengthIndicator, int maxLength, IFieldValidator validator, IFormatter formatter, string displayName = "")
        {
            return Create(new VariableLengthFormatter(lengthIndicator, maxLength, formatter), validator, formatter, displayName);
        }

        internal static IFieldDescriptor FixedLength(int packedLength, IFieldValidator validator, IFormatter formatter, string displayName = "")
        {
            return Create(new FixedLengthFormatter(packedLength), validator, formatter, displayName);
        }

        public static IFieldDescriptor BinaryVariableLength(int lengthIndicator, int maxLength, IFieldValidator validator, IFormatter formatter, string displayName = "")
        {
            return new FieldDescriptor(new VariableLengthFormatter(lengthIndicator, maxLength, formatter), FieldValidators.Hex, Formatters.Binary, null, displayName);
        }
        #endregion

    }
}