namespace ETWDeserializer.CustomParsers
{
    using System;

    internal class KernelTraceControlDbgIdParser
    {
        private static readonly EventMetadata eventMetadata;

        private static readonly PropertyMetadata imageBase;

        private static readonly PropertyMetadata processId;

        private static readonly PropertyMetadata guidSig;

        private static readonly PropertyMetadata age;

        private static readonly PropertyMetadata pdbFileName;

        static KernelTraceControlDbgIdParser()
        {
            imageBase = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_INT64, TDH_OUT_TYPE.TDH_OUTTYPE_HEXINT64, "ImageBase", false, false, 0, null);
            processId = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UINT32, TDH_OUT_TYPE.TDH_OUTTYPE_UNSIGNEDINT, "ProcessID", false, false, 0, null);
            guidSig = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_GUID, TDH_OUT_TYPE.TDH_OUTTYPE_GUID, "GuidSig", false, false, 0, null);
            age = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UINT32, TDH_OUT_TYPE.TDH_OUTTYPE_UNSIGNEDINT, "Age", false, false, 0, null);
            pdbFileName = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "PdbFileName", false, false, 0, null);
            eventMetadata = new EventMetadata(
                new Guid("b3e675d7-2554-4f18-830b-2762732560de"),
                36,
                0,
                "KernelTraceControl/ImageID/DbgID",
                new[] { imageBase, processId, guidSig, age, pdbFileName });
        }

        public void Parse<T>(EventRecordReader reader, T writer, EventMetadata[] metadataArray, RuntimeEventMetadata runtimeMetadata) where T : IEtwWriter
        {
            writer.WriteEventBegin(eventMetadata, runtimeMetadata);

            writer.WritePropertyBegin(imageBase);
            writer.WriteUInt64(reader.ReadUInt64());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(processId);
            writer.WriteUInt32(reader.ReadUInt32());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(guidSig);
            writer.WriteGuid(reader.ReadGuid());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(age);
            writer.WriteUInt32(reader.ReadUInt32());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(pdbFileName);
            writer.WriteAnsiString(reader.ReadAnsiString());
            writer.WritePropertyEnd();
            
            writer.WriteEventEnd();
        }
    }
}