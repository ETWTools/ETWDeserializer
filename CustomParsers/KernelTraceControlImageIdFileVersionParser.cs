namespace ETWDeserializer.CustomParsers
{
    using System;

    internal class KernelTraceControlImageIdFileVersionParser
    {
        private static readonly EventMetadata eventMetadata;

        private static readonly PropertyMetadata imageSize;

        private static readonly PropertyMetadata timeDateStamp;

        private static readonly PropertyMetadata origFileName;

        private static readonly PropertyMetadata fileDescription;

        private static readonly PropertyMetadata fileVersion;

        private static readonly PropertyMetadata binFileVersion;

        private static readonly PropertyMetadata verLanguage;

        private static readonly PropertyMetadata productName;

        private static readonly PropertyMetadata companyName;

        private static readonly PropertyMetadata productVersion;

        private static readonly PropertyMetadata fileId;

        private static readonly PropertyMetadata programId;

        static KernelTraceControlImageIdFileVersionParser()
        {
            imageSize = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_INT32, TDH_OUT_TYPE.TDH_OUTTYPE_UNSIGNEDINT, "ImageSize", false, false, 0, null);
            timeDateStamp = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_INT32, TDH_OUT_TYPE.TDH_OUTTYPE_UNSIGNEDINT, "TimeDateStamp", false, false, 0, null);
            origFileName = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "OrigFileName", false, false, 0, null);
            fileDescription = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "FileDescription", false, false, 0, null);
            fileVersion = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "FileVersion", false, false, 0, null);
            binFileVersion = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "BinFileVersion", false, false, 0, null);
            verLanguage = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "VerLanguage", false, false, 0, null);
            productName = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "ProductName", false, false, 0, null);
            companyName = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "CompanyName", false, false, 0, null);
            productVersion = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "ProductVersion", false, false, 0, null);
            fileId = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "FileId", false, false, 0, null);
            programId = new PropertyMetadata(TDH_IN_TYPE.TDH_INTYPE_UNICODESTRING, TDH_OUT_TYPE.TDH_OUTTYPE_STRING, "ProgramId", false, false, 0, null);
            eventMetadata = new EventMetadata(
                new Guid("b3e675d7-2554-4f18-830b-2762732560de"),
                64,
                0,
                "KernelTraceControl/ImageID/FileVersion",
                new[] { imageSize, timeDateStamp, origFileName, fileDescription, fileVersion, binFileVersion, verLanguage, productName, companyName, productVersion, fileId, programId });
        }

        public void Parse<T>(EventRecordReader reader, T writer, EventMetadata[] metadataArray, RuntimeEventMetadata runtimeMetadata) where T : IEtwWriter
        {
            writer.WriteEventBegin(eventMetadata, runtimeMetadata);
            
            writer.WritePropertyBegin(imageSize);
            writer.WriteUInt64(reader.ReadUInt32());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(timeDateStamp);
            writer.WriteUInt64(reader.ReadUInt32());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(origFileName);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(fileDescription);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(fileVersion);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(binFileVersion);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(verLanguage);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(productName);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(companyName);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(productVersion);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(fileId);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WritePropertyBegin(programId);
            writer.WriteUnicodeString(reader.ReadUnicodeString());
            writer.WritePropertyEnd();

            writer.WriteEventEnd();
        }
    }
}