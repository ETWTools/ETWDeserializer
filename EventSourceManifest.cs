namespace ETWDeserializer
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    internal sealed class EventSourceManifest
    {
        private readonly StringBuilder chunkBuilder = new StringBuilder();

        private readonly Guid providerGuid;

        private readonly byte format;

        private readonly byte majorVersion;

        private readonly byte minorVersion;

        private readonly byte magic;

        private readonly ushort totalChunks;

        private ushort chunksReceived;

        private instrumentationManifest manifest;

        public EventSourceManifest(Guid providerGuid, byte format, byte majorVersion, byte minorVersion, byte magic, ushort totalChunks)
        {
            this.providerGuid = providerGuid;
            this.format = format;
            this.majorVersion = majorVersion;
            this.minorVersion = minorVersion;
            this.magic = magic;
            this.totalChunks = totalChunks;
        }

        public void AddChunk(string schemaChunk)
        {
            this.chunkBuilder.Append(schemaChunk);
            ++this.chunksReceived;
        }

        public bool IsComplete {  get { return this.chunksReceived == this.totalChunks; } }

        public instrumentationManifest Schema
        {
            get
            {
                if (this.chunksReceived != this.totalChunks)
                {
                    throw new Exception("Schema is incomplete as not all chunks have been received");
                }

                if (this.manifest == null)
                {
                    string value = this.chunkBuilder.ToString();
                    var bytes = Encoding.ASCII.GetBytes(value);
                    using (var ms = new MemoryStream(bytes))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(instrumentationManifest));
                        this.manifest = (instrumentationManifest)serializer.Deserialize(ms);
                    }
                }

                return this.manifest;
            }
        }
        
        public Guid ProviderGuid { get { return this.providerGuid; } }

        public byte Format { get { return this.format; } }

        public byte MajorVersion { get { return this.majorVersion; } }

        public byte MinorVersion { get { return this.minorVersion; } }

        public byte Magic { get { return this.magic; } }

        public ushort TotalChunks { get { return this.totalChunks; } }

        public override bool Equals(object obj)
        {
            return obj is EventSourceManifest && this.Equals((EventSourceManifest)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.providerGuid.GetHashCode();
                hashCode = (hashCode * 397) ^ this.format.GetHashCode();
                hashCode = (hashCode * 397) ^ this.majorVersion.GetHashCode();
                hashCode = (hashCode * 397) ^ this.minorVersion.GetHashCode();
                hashCode = (hashCode * 397) ^ this.magic.GetHashCode();
                hashCode = (hashCode * 397) ^ this.totalChunks.GetHashCode();

                return hashCode;
            }
        }

        public bool Equals(EventSourceManifest other)
        {
            return this.providerGuid.Equals(other.providerGuid)
                   && this.format == other.format
                   && this.majorVersion == other.majorVersion
                   && this.minorVersion == other.minorVersion
                   && this.magic == other.magic
                   && this.totalChunks == other.totalChunks;
        }
    }
}