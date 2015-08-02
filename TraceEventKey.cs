namespace ETWDeserializer
{
    using System;

    internal struct TraceEventKey : IEquatable<TraceEventKey>
    {
        private readonly Guid ProviderId;

        private readonly ushort Id;

        private readonly byte Version;

        public TraceEventKey(Guid providerId, ushort id, byte version)
        {
            this.ProviderId = providerId;
            this.Id = id;
            this.Version = version;
        }

        public bool Equals(TraceEventKey other)
        {
            return this.ProviderId.Equals(other.ProviderId) && this.Id == other.Id && this.Version == other.Version;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TraceEventKey && Equals((TraceEventKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.ProviderId.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Id.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Version.GetHashCode();
                return hashCode;
            }
        }
    }
}