namespace ETWDeserializer.CustomParsers
{
    using System;

    internal class KernelStackWalkEventParser
    {
        private static readonly EventMetadata eventMetadata;

        private static readonly PropertyMetadata stacksPropertyMetadata;

        private static readonly PropertyMetadata stackProcessMetadata;

        private static readonly PropertyMetadata stackThreadMetadata;

        private static readonly PropertyMetadata eventTimeStampMetadata;

        static KernelStackWalkEventParser()
        {
            eventTimeStampMetadata = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_INT64, TDH_OUT_TYPE.TDH_OUTTYPE_UNSIGNEDLONG, "EventTimeStamp", false, false, 0, null);
            stackProcessMetadata = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_INT32, TDH_OUT_TYPE.TDH_OUTTYPE_UNSIGNEDINT, "StackProcess", false, false, 0, null);
            stackThreadMetadata = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_INT32, TDH_OUT_TYPE.TDH_OUTTYPE_UNSIGNEDINT, "StackThread", false, false, 0, null);
            stacksPropertyMetadata = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_POINTER, TDH_OUT_TYPE.TDH_OUTTYPE_HEXINT64, "Stacks", false, false, 0, null);
            eventMetadata = new EventMetadata(
                new Guid("def2fe46-7bd6-4b80-bd94-f57fe20d0ce3"),
                32, 
                0,
                "Kernel/Stack/StackWalk",
                new[] { eventTimeStampMetadata, stackProcessMetadata, stackThreadMetadata, stacksPropertyMetadata });
        }

        public void Parse<T>(EventRecordReader reader, T writer, EventMetadata[] metadataArray, RuntimeEventMetadata runtimeMetadata) where T : IEtwWriter
        {
            writer.WriteEventBegin(eventMetadata, runtimeMetadata);

            int pointerSize = (runtimeMetadata.Flags & Etw.EVENT_HEADER_FLAG_32_BIT_HEADER) != 0 ? 4 : 8;
            int numberOfStacks = (runtimeMetadata.UserDataLength - 16) / pointerSize;

            writer.WritePropertyBegin(eventTimeStampMetadata);
            writer.WriteUInt64(reader.ReadUInt64());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(stackProcessMetadata);
            writer.WriteUInt32(reader.ReadUInt32());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(stackThreadMetadata);
            writer.WriteUInt32(reader.ReadUInt32());

            writer.WritePropertyBegin(stacksPropertyMetadata);

            writer.WriteArrayBegin();
            for (int i = 0; i < numberOfStacks; ++i)
            {
                writer.WritePointer(reader.ReadPointer());
            }
            writer.WriteArrayEnd();

            writer.WritePropertyEnd();
            writer.WriteEventEnd();
        }
    }
}