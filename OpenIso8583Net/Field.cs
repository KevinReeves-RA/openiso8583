namespace OpenIso8583Net
{
    /// <summary>
    ///   Class representing a field
    /// </summary>
    public class Field : IField
    {
        /// <summary>
        ///   Field descriptor
        /// </summary>
        protected IFieldDescriptor _fieldDescriptor;

        /// <summary>
        ///   Creates a new instance of the Field class
        /// </summary>
        /// <param name = "fieldNumber">The field number that this object represents</param>
        /// <param name = "fieldDescriptor">Field descriptor</param>
        public Field(int fieldNumber, IFieldDescriptor fieldDescriptor)
        {
            FieldNumber = fieldNumber;
            _fieldDescriptor = fieldDescriptor;
        }

        #region IField Members

        private string _value = string.Empty;
        /// <summary>
        ///   The Value contained in the field
        /// </summary>
        /// adjustment is skipped if there is no Adjuster
        public string Value
        {
            get
            {
                return _fieldDescriptor.Adjuster == null ? _value : _fieldDescriptor.Adjuster.Get(_value);
            }
            set
            {
                _value = _fieldDescriptor.Adjuster == null ? value : _fieldDescriptor.Adjuster.Set(value);
            }
        }

        /// <summary>
        ///   Gets the field number that this field represents
        /// </summary>
        public int FieldNumber { get; private set; }

        /// <summary>
        ///   Gets the packed length of the field
        /// </summary>
        public int PackedLength
        {
            get { return _fieldDescriptor.GetPackedLength(Value); }
        }

        /// <summary>
        ///   Gets a representation of the field as a string
        /// </summary>
        /// <returns>String representation of the field</returns>
        public override string ToString()
        {
            return ToString(string.Empty);
        }

        /// <summary>
        ///   Gets a representation of the field as a string
        /// </summary>
        /// <param name = "prefix">Prefix to add onto the string</param>
        /// <returns>String representation of the field</returns>
        public string ToString(string prefix)
        {
            return _fieldDescriptor.Display(prefix, FieldNumber, Value);
        }

        /// <summary>
        ///   Unpacks the field from the message
        /// </summary>
        /// <param name = "msg">byte[] of the full message</param>
        /// <param name = "offset">offset indicating the start of the field</param>
        /// <returns>new offset in message to start unpacking the next field</returns>
        public int Unpack(byte[] msg, int offset)
        {
            int newOffset;
            Value = _fieldDescriptor.Unpack(FieldNumber, msg, offset, out newOffset);

            return newOffset;
        }

        /// <summary>
        ///   Returns a byte[] representation of the field
        /// </summary>
        /// <returns>byte[] representing the field</returns>
        public byte[] ToMsg()
        {
            return _fieldDescriptor.Pack(FieldNumber, Value);
        }

        #endregion
    }
}