namespace ETWDeserializer.CustomParsers
{
    using System;

    /// <summary>
    /// Much of this knowledge has been acquired thanks to:
    /// https://github.com/etwtools/semantic-logging/blob/d57a4750b8699c71f34933ecc1bae629b8d4c93c/source/Src/TraceEvent1.2.7/KernelTraceEventParser.cs
    /// https://github.com/etwtools/semantic-logging/blob/d57a4750b8699c71f34933ecc1bae629b8d4c93c/source/Src/TraceEvent1.2.7/SymbolEventParser.cs
    /// </summary>

    internal static class CustomParserGuids
    {
        public static Guid KernelTraceControlGuid = new Guid("b3e675d7-2554-4f18-830b-2762732560de");

        public static Guid KernelStackWalkGuid = new Guid("def2fe46-7bd6-4b80-bd94-f57fe20d0ce3");
    }
}