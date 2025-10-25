namespace ETWDeserializer
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    internal static class EventTraceOperandExpressionBuilder
    {
        public static void Build(
            ILGenerator il,
            IEventTraceOperand operand,
            Type eventRecordReaderType,
            Type eventRecordWriterType,
            Type eventMetadataArrayType,
            Type runtimeMetadataType)
        {
            new EventTraceOperandILEmitter().Build(il, operand, eventRecordReaderType, eventRecordWriterType, eventMetadataArrayType, runtimeMetadataType);
        }
    }

    internal sealed class EventTraceOperandILEmitter
    {
        public void Build(ILGenerator il, IEventTraceOperand operand, Type eventRecordReaderType, Type eventRecordWriterType, Type eventMetadataArrayType, Type runtimeMetadataType)
        {
            // Declare local variables for eventMetadata and properties
            // eventMetadata = eventMetadataTable[operand.EventMetadataTableIndex]
            // properties = eventMetadata.Properties
            var eventMetadata = il.DeclareLocal(typeof(EventMetadata));
            var properties = il.DeclareLocal(typeof(PropertyMetadata[]));

            // eventMetadata = eventMetadataTable[operand.EventMetadataTableIndex]
            il.Emit(OpCodes.Ldarg_2); // Load eventMetadataTable (parameter 2)
            il.Emit(OpCodes.Ldc_I4, operand.EventMetadataTableIndex);
            il.Emit(OpCodes.Ldelem_Ref);
            il.Emit(OpCodes.Stloc, eventMetadata);

            // properties = eventMetadata.Properties
            il.Emit(OpCodes.Ldloc, eventMetadata);
            var propertiesField = typeof(EventMetadata).GetField("Properties");
            il.Emit(OpCodes.Ldfld, propertiesField);
            il.Emit(OpCodes.Stloc, properties);

            // eventRecordWriter.WriteEventBegin(eventMetadata, runtimeMetadata)
            il.Emit(OpCodes.Ldarg_1); // Load eventRecordWriter (parameter 1)
            il.Emit(OpCodes.Ldloc, eventMetadata);
            il.Emit(OpCodes.Ldarg_3); // Load runtimeMetadata (parameter 3)
            var writeEventBeginMethod = eventRecordWriterType.GetMethod("WriteEventBegin", new[] { typeof(EventMetadata), runtimeMetadataType });
            il.Emit(OpCodes.Callvirt, writeEventBeginMethod);

            // Generate code for all properties
            var ilGenerator = new ILGenerator(il, eventRecordReaderType, eventRecordWriterType, properties);
            ilGenerator.CodeGenerate(operand.EventPropertyOperands);

            // eventRecordWriter.WriteEventEnd()
            il.Emit(OpCodes.Ldarg_1); // Load eventRecordWriter (parameter 1)
            var writeEventEndMethod = eventRecordWriterType.GetMethod("WriteEventEnd", Type.EmptyTypes);
            il.Emit(OpCodes.Callvirt, writeEventEndMethod);

            il.Emit(OpCodes.Ret);
        }

        private sealed class ILGenerator
        {
            private readonly Dictionary<IEventTracePropertyOperand, LocalBuilder> operandReferenceTable = new Dictionary<IEventTracePropertyOperand, LocalBuilder>();

            private readonly System.Reflection.Emit.ILGenerator il;

            private readonly Type eventRecordReaderType;

            private readonly Type eventRecordWriterType;

            private readonly LocalBuilder properties;

            public ILGenerator(System.Reflection.Emit.ILGenerator il, Type eventRecordReaderType, Type eventRecordWriterType, LocalBuilder properties)
            {
                this.il = il;
                this.eventRecordReaderType = eventRecordReaderType;
                this.eventRecordWriterType = eventRecordWriterType;
                this.properties = properties;
            }

            public void CodeGenerate(IEnumerable<IEventTracePropertyOperand> operands)
            {
                foreach (var operand in operands)
                {
                    var inType = operand.Metadata.InType;

                    // eventRecordWriter.WritePropertyBegin(properties[operand.PropertyIndex])
                    this.il.Emit(OpCodes.Ldarg_1); // Load eventRecordWriter (parameter 1)
                    this.il.Emit(OpCodes.Ldloc, this.properties);
                    this.il.Emit(OpCodes.Ldc_I4, operand.PropertyIndex);
                    this.il.Emit(OpCodes.Ldelem_Ref);
                    var writePropertyBeginMethod = this.eventRecordWriterType.GetMethod("WritePropertyBegin", new[] { typeof(PropertyMetadata) });
                    this.il.Emit(OpCodes.Callvirt, writePropertyBeginMethod);

                    // Handle arrays vs single values
                    if (operand.IsVariableArray || operand.IsFixedArray)
                    {
                        var loopVariable = this.il.DeclareLocal(typeof(int));
                        var endVariable = this.il.DeclareLocal(typeof(int));

                        // Store the array size
                        if (operand.IsVariableArray)
                        {
                            var arraySizeLocal = this.operandReferenceTable[operand.VariableArraySize];
                            this.il.Emit(OpCodes.Ldloc, arraySizeLocal);
                            ConvertIfNecessary(this.il, arraySizeLocal.LocalType, typeof(int));
                            this.il.Emit(OpCodes.Stloc, endVariable);
                        }
                        else
                        {
                            this.il.Emit(OpCodes.Ldc_I4, operand.FixedArraySize);
                            this.il.Emit(OpCodes.Stloc, endVariable);
                        }

                        // eventRecordWriter.WriteArrayBegin()
                        this.il.Emit(OpCodes.Ldarg_1);
                        var writeArrayBeginMethod = this.eventRecordWriterType.GetMethod("WriteArrayBegin", Type.EmptyTypes);
                        this.il.Emit(OpCodes.Callvirt, writeArrayBeginMethod);

                        // for (loopVariable = 0; loopVariable < endVariable; loopVariable++)
                        var loopStart = this.il.DefineLabel();
                        var loopCondition = this.il.DefineLabel();

                        // loopVariable = 0
                        this.il.Emit(OpCodes.Ldc_I4_0);
                        this.il.Emit(OpCodes.Stloc, loopVariable);
                        this.il.Emit(OpCodes.Br, loopCondition);

                        // Loop body start
                        this.il.MarkLabel(loopStart);

                        // Generate the code for each array element
                        this.EmitOperandCode(operand, inType);

                        // loopVariable++
                        this.il.Emit(OpCodes.Ldloc, loopVariable);
                        this.il.Emit(OpCodes.Ldc_I4_1);
                        this.il.Emit(OpCodes.Add);
                        this.il.Emit(OpCodes.Stloc, loopVariable);

                        // Loop condition: loopVariable < endVariable
                        this.il.MarkLabel(loopCondition);
                        this.il.Emit(OpCodes.Ldloc, loopVariable);
                        this.il.Emit(OpCodes.Ldloc, endVariable);
                        this.il.Emit(OpCodes.Blt, loopStart);

                        // eventRecordWriter.WriteArrayEnd()
                        this.il.Emit(OpCodes.Ldarg_1);
                        var writeArrayEndMethod = this.eventRecordWriterType.GetMethod("WriteArrayEnd", Type.EmptyTypes);
                        this.il.Emit(OpCodes.Callvirt, writeArrayEndMethod);
                    }
                    else
                    {
                        // Not an array, generate code once
                        this.EmitOperandCode(operand, inType);
                    }

                    // eventRecordWriter.WritePropertyEnd()
                    this.il.Emit(OpCodes.Ldarg_1);
                    var writePropertyEndMethod = this.eventRecordWriterType.GetMethod("WritePropertyEnd", Type.EmptyTypes);
                    this.il.Emit(OpCodes.Callvirt, writePropertyEndMethod);
                }
            }

            private void EmitOperandCode(IEventTracePropertyOperand operand, TDH_IN_TYPE inType)
            {
                // if struct, recurse
                if (operand.Children.Count > 0)
                {
                    // eventRecordWriter.WriteStructBegin()
                    this.il.Emit(OpCodes.Ldarg_1);
                    var writeStructBeginMethod = this.eventRecordWriterType.GetMethod("WriteStructBegin", Type.EmptyTypes);
                    this.il.Emit(OpCodes.Callvirt, writeStructBeginMethod);

                    // Recurse
                    this.CodeGenerate(operand.Children);

                    // eventRecordWriter.WriteStructEnd()
                    this.il.Emit(OpCodes.Ldarg_1);
                    var writeStructEndMethod = this.eventRecordWriterType.GetMethod("WriteStructEnd", Type.EmptyTypes);
                    this.il.Emit(OpCodes.Callvirt, writeStructEndMethod);
                }
                else
                {
                    // Read the value
                    if (operand.IsVariableLength || operand.IsFixedLength)
                    {
                        // eventRecordReader.ReadXXX(length)
                        this.il.Emit(OpCodes.Ldarg_0); // Load eventRecordReader (parameter 0)

                        if (operand.IsVariableLength)
                        {
                            // Load the variable length from operandReferenceTable
                            var lengthLocal = this.operandReferenceTable[operand.VariableLengthSize];
                            this.il.Emit(OpCodes.Ldloc, lengthLocal);
                            var readMethod = inType.ReadMethodInfo(this.eventRecordReaderType, lengthLocal.LocalType);
                            this.il.Emit(OpCodes.Callvirt, readMethod);
                        }
                        else
                        {
                            // Load the fixed length constant
                            this.il.Emit(OpCodes.Ldc_I4, operand.FixedLengthSize);
                            var readMethod = inType.ReadMethodInfo(this.eventRecordReaderType, typeof(int));
                            this.il.Emit(OpCodes.Callvirt, readMethod);
                        }
                    }
                    else
                    {
                        // eventRecordReader.ReadXXX()
                        this.il.Emit(OpCodes.Ldarg_0); // Load eventRecordReader (parameter 0)
                        var readMethod = inType.ReadMethodInfo(this.eventRecordReaderType);
                        this.il.Emit(OpCodes.Callvirt, readMethod);
                    }

                    // If this operand is referenced by other properties, save it to a local variable
                    if (operand.IsReferencedByOtherProperties)
                    {
                        var local = this.il.DeclareLocal(inType.CSharpType());
                        this.il.Emit(OpCodes.Stloc, local);
                        this.operandReferenceTable.Add(operand, local);

                        // eventRecordWriter.WriteXXX(local)
                        this.il.Emit(OpCodes.Ldarg_1); // Load eventRecordWriter (parameter 1)
                        this.il.Emit(OpCodes.Ldloc, local);
                        var writeMethod = inType.WriteMethodInfo(this.eventRecordWriterType, local.LocalType);
                        this.il.Emit(OpCodes.Callvirt, writeMethod);
                    }
                    else
                    {
                        // The read value is on the stack, write it directly
                        // eventRecordWriter.WriteXXX(readValue)
                        var readValue = this.il.DeclareLocal(inType.CSharpType());
                        this.il.Emit(OpCodes.Stloc, readValue);
                        this.il.Emit(OpCodes.Ldarg_1); // Load eventRecordWriter (parameter 1)
                        this.il.Emit(OpCodes.Ldloc, readValue);
                        var writeMethod = inType.WriteMethodInfo(this.eventRecordWriterType, inType.CSharpType());
                        this.il.Emit(OpCodes.Callvirt, writeMethod);
                    }
                }
            }

            private static void ConvertIfNecessary(System.Reflection.Emit.ILGenerator il, Type sourceType, Type targetType)
            {
                var sourceTypeCode = Type.GetTypeCode(sourceType);
                var targetTypeCode = Type.GetTypeCode(targetType);

                if (sourceTypeCode == targetTypeCode)
                {
                    return;
                }

                // Emit conversion instruction
                if (targetTypeCode == TypeCode.Int32)
                {
                    il.Emit(OpCodes.Conv_I4);
                }
                else if (targetTypeCode == TypeCode.Int64)
                {
                    il.Emit(OpCodes.Conv_I8);
                }
                else if (targetTypeCode == TypeCode.UInt32)
                {
                    il.Emit(OpCodes.Conv_U4);
                }
                else if (targetTypeCode == TypeCode.UInt64)
                {
                    il.Emit(OpCodes.Conv_U8);
                }
            }
        }
    }
}
