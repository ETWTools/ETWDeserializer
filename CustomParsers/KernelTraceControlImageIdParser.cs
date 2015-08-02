namespace ETWDeserializer.CustomParsers
{
    using System;

    internal class KernelTraceControlImageIdParser
    {
        private static readonly EventMetadata eventMetadata;

        private static readonly PropertyMetadata imageBase;

        private static readonly PropertyMetadata imageSize;

        private static readonly PropertyMetadata timeDateStamp;

        private static readonly PropertyMetadata originalFileName;

        static KernelTraceControlImageIdParser()
        {
            imageBase = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_POINTER, TDH_OUT_TYPE.TDH_OUTTYPE_HEXINT64, "ImageBase", false, false, 0, null);
            imageSize = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UINT32, TDH_OUT_TYPE.TDH_OUTTYPE_UNSIGNEDINT, "ImageSize", false, false, 0, null);
            timeDateStamp = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UINT32, TDH_OUT_TYPE.TDH_OUTTYPE_UNSIGNEDINT, "TimeDateStamp", false, false, 0, null);
            originalFileName = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "OriginalFileName", false, false, 0, null);
            eventMetadata = new EventMetadata(
                new Guid("b3e675d7-2554-4f18-830b-2762732560de"),
                36,
                0,
                "KernelTraceControl/ImageID",
                new[] { imageBase, imageSize, timeDateStamp, originalFileName, });
        }

        public void Parse<T>(EventRecordReader reader, T writer, EventMetadata[] metadataArray, RuntimeEventMetadata runtimeMetadata) where T : IEtwWriter
        {
            writer.WriteEventBegin(eventMetadata, runtimeMetadata);

            writer.WritePropertyBegin(imageBase);
            writer.WritePointer(reader.ReadPointer());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(imageSize);
            writer.WriteUInt32(reader.ReadUInt32());
            writer.WritePropertyEnd();

            reader.ReadPointer();

            writer.WritePropertyBegin(timeDateStamp);
            writer.WriteUInt32(reader.ReadUInt32());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(originalFileName);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WriteEventEnd();
        }
    }
}