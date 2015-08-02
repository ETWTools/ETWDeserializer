namespace ETWDeserializer
{
    using System.Collections.Generic;

    internal sealed class EventTracePropertyOperand : IEventTracePropertyOperand
    {
        public EventTracePropertyOperand(PropertyMetadata metadata, int propertyIndex, bool isVariableArray, bool isFixedArray, bool isVariableLength, bool isFixedLength, bool isWbemXml)
        {
            this.Metadata = metadata;
            this.PropertyIndex = propertyIndex;
            this.IsVariableArray = isVariableArray;
            this.IsFixedArray = isFixedArray;
            this.IsVariableLength = isVariableLength;
            this.IsFixedLength = isFixedLength;
            this.IsWbemXMLFragment = isWbemXml;
            this.Children = new List<IEventTracePropertyOperand>();
        }

        public PropertyMetadata Metadata { get; private set; }

        public int PropertyIndex { get; private set; }

        public bool IsVariableArray { get; private set; }

        public IEventTracePropertyOperand VariableArraySize { get; private set; }

        public bool IsVariableLength { get; private set; }

        public IEventTracePropertyOperand VariableLengthSize { get; private set; }

        public bool IsFixedArray { get; private set; }

        public int FixedArraySize { get; private set; }

        public bool IsFixedLength { get; private set; }

        public int FixedLengthSize { get; private set; }

        public bool IsWbemXMLFragment { get; private set; }

        public bool IsReferencedByOtherProperties { get; set; }

        public List<IEventTracePropertyOperand> Children { get; private set; }

        public void SetFixedArraySize(int fixedArraySize)
        {
            this.FixedArraySize = fixedArraySize;
        }

        public void SetVariableArraySize(IEventTracePropertyOperand reference)
        {
            this.VariableArraySize = reference;
        }

        public void SetFixedLengthSize(int fixedLengthSize)
        {
            this.FixedLengthSize = fixedLengthSize;
        }

        public void SetVariableLengthSize(IEventTracePropertyOperand reference)
        {
            this.VariableLengthSize = reference;
        }
    }
}