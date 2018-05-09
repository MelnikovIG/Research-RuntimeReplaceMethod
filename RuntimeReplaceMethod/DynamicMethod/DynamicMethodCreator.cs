using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMethodCreator
{
    public static class DynamicMethodCreator
    {
        public static MethodInfo CreateMethodSameAsOrigial(MethodInfo originalMethod)
        {
            var originalMethodInstructions = Mono.Reflection.Disassembler.GetInstructions(originalMethod).ToList();
            var returnType = originalMethod.ReturnType;
            var args = originalMethod.GetParameters().Select(x => x.ParameterType).ToArray();

            var methodName = $"{originalMethod.Name}_Dynamic_{Guid.NewGuid()}";
            var typeBuilder = CreateTypeBuilder();
            BuildMethod(typeBuilder, methodName, originalMethod);
            Type myType = typeBuilder.CreateType();
            MethodInfo dynamicMethod = myType.GetMethod(methodName);

            DumpMethod(originalMethod);
            DumpMethod(dynamicMethod);

            return dynamicMethod;
        }

        private static void BuildMethod(TypeBuilder typeBuilder, string methodName, MethodInfo originalMethod)
        {
            var originalMethodInstructions = Mono.Reflection.Disassembler.GetInstructions(originalMethod).ToList();
            var returnType = originalMethod.ReturnType;
            var args = originalMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            var attr = originalMethod.Attributes;

            MethodBuilder myMthdBld = typeBuilder.DefineMethod(
                                                methodName,
                                                attr,
                                                returnType,
                                                args);

            ILGenerator il = myMthdBld.GetILGenerator();

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
        }

        private static TypeBuilder CreateTypeBuilder()
        {
            AppDomain myDomain = AppDomain.CurrentDomain;
            AssemblyName asmName = new AssemblyName();
            asmName.Name = "MyDynamicAsm";

            AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(
                                           asmName,
                                           AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder myModule = myAsmBuilder.DefineDynamicModule("MyDynamicAsm",
                                                                      "MyDynamicAsm.dll");

            TypeBuilder myTypeBld = myModule.DefineType("MyDynamicType",
                                                        TypeAttributes.Public);

            return myTypeBld;
        }
        
        public static void DumpMethod(MethodInfo method)
        {
            Console.WriteLine($"====== method {method.Name} dump started ======");
            Console.WriteLine($"IsStatic: {method.IsStatic}");
            Console.WriteLine($"ParmsCount: {method.GetParameters().Count()}");
            Console.WriteLine($"ReturnType: {method.ReturnType.FullName}");
            Console.WriteLine($"====== method {method.Name} dump complete ======");
        }
    }
}
