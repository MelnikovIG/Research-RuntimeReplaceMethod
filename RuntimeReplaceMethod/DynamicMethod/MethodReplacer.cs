using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ConsoleApp2
{
    public static class MethodReplacer
    {
        public static unsafe void Replace(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            var isInjectedMethodDynamic = methodToInject is DynamicMethod;

            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            if (!isInjectedMethodDynamic)
            {
                RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);
            }
            else
            {
                var dynamicMethodHandle = GetDynamicHandle(methodToInject as DynamicMethod);
                RuntimeHelpers.PrepareMethod(dynamicMethodHandle);
            }

            unsafe
            {
                if (IntPtr.Size == 4)
                {
                    int* inj = (int*)methodToInject.MethodHandle.Value.ToPointer() + 2;
                    int* tar = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
#if DEBUG
                    Console.WriteLine("\nVersion x86 Debug\n");

                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;

                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    Console.WriteLine("\nVersion x86 Release\n");
                    *tar = *inj;
#endif
                }
                else
                {

                    long* inj = isInjectedMethodDynamic
                        ? (long*)GetDynamicHandle(methodToInject as DynamicMethod).Value.ToPointer() + 1
                        : (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
#if DEBUG
                    Console.WriteLine("\nVersion x64 Debug\n");
                    //byte* injInst = (byte*)*inj;
                    //byte* tarInst = (byte*)*tar;
                    byte* injInst = (byte*)GetDynamicHandle(methodToInject as DynamicMethod).GetFunctionPointer();
                    byte* tarInst = (byte*)methodToReplace.MethodHandle.GetFunctionPointer();

                    long* injSrc = (long*)(injInst + 1);
                    long* tarSrc = (long*)(tarInst + 1);

                    *tarSrc = (((long)injInst + 5) + *injSrc) - ((long)tarInst + 5);
#else
                    Console.WriteLine("\nVersion x64 Release\n");
                    *tar = *inj;
#endif
                }
            }
        }

        private static RuntimeMethodHandle GetDynamicHandle(DynamicMethod dynamicMethod)
        {
            var descr = typeof(DynamicMethod)
                .GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic);

            var res = (RuntimeMethodHandle)descr.Invoke(dynamicMethod, null);
            return res;
        }
    }
}