using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Mono.Reflection;

namespace ConsoleApp2
{
    public class MethodMerger
    {
        public static void Begin()
        {
            Console.WriteLine("//---------------------------");
            Console.WriteLine($"Start injected {DateTime.Now.Ticks}");
        }

        public static void End()
        {
            Console.WriteLine($"End injected {DateTime.Now.Ticks}");
            Console.WriteLine("//---------------------------");
        }

        public static DynamicMethod Merge(MethodInfo originalMethod)
        {
            var methodBegin = typeof(MethodMerger).GetMethod(nameof(Begin), BindingFlags.Public/* | BindingFlags.NonPublic*/ | BindingFlags.Instance | BindingFlags.Static);

            var methodEnd = typeof(MethodMerger).GetMethod(nameof(End), BindingFlags.Public/* | BindingFlags.NonPublic*/ | BindingFlags.Instance | BindingFlags.Static);

            var beginMethodInstructions = Mono.Reflection.Disassembler.GetInstructions(methodBegin).ToList();
            beginMethodInstructions.RemoveAt(beginMethodInstructions.Count - 1); //удалим возврат

            var endMethodInstructions = Mono.Reflection.Disassembler.GetInstructions(methodEnd);

            var originalMethodInstructions = Mono.Reflection.Disassembler.GetInstructions(originalMethod).ToList();
            originalMethodInstructions.RemoveAt(originalMethodInstructions.Count - 1); //удалим возврат

            var returnType = originalMethod.ReturnType;
            var args = originalMethod.GetParameters().Select(x => x.ParameterType).ToArray();

            DynamicMethod dynamicMethod = new DynamicMethod(
                "DynamicMethod",
                returnType,
                args,
                typeof(MethodMerger).Module);

            ILGenerator il = dynamicMethod.GetILGenerator();

            il.Emit(OpCodes.Call, methodBegin);

            foreach (var instruction in originalMethodInstructions)
            {
                if (instruction.Operand == null)
                {
                    il.Emit(instruction.OpCode);
                }
                else if (instruction.Operand is string strOper)
                {
                    il.Emit(instruction.OpCode, strOper);
                }
                else if (instruction.Operand is MethodInfo methodOper)
                {
                    il.Emit(instruction.OpCode, methodOper);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_S)
                {
                    il.Emit(instruction.OpCode, (sbyte)instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Br_S)
                {
                    //Label targetInstruction = il.DefineLabel();
                    //il.Emit(OpCodes.Br_S, targetInstruction);
                    //il.MarkLabel(targetInstruction);
                    throw new NotImplementedException();
                }
                else if (instruction.OpCode == OpCodes.Box)
                {
                    Type type = instruction.Operand as Type;
                    il.Emit(OpCodes.Box, type);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            il.Emit(OpCodes.Call, methodEnd);
            il.Emit(OpCodes.Ret);

            //dynamicMethod.Invoke(new MethodMerger(), null); // call for test
            return dynamicMethod;
        }
    }
}

