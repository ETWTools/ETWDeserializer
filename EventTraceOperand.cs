namespace ETWDeserializer
{
    using System.Collections.Generic;

    internal sealed class EventTraceOperand : IEventTraceOperand
    {
        internal EventTraceOperand(EventMetadata metadata, int eventMetadataTableIndex, IEnumerable<IEventTracePropertyOperand> operands)
        {
            this.Metadata = metadata;
            this.EventMetadataTableIndex = eventMetadataTableIndex;
            this.EventPropertyOperands = operands;
        }

        public int EventMetadataTableIndex { get; private set; }

        public EventMetadata Metadata { get; private set; }

        public IEnumerable<IEventTracePropertyOperand> EventPropertyOperands { get; private set; }
    }
}