using System;
using System.Runtime.CompilerServices;

namespace ConsoleApp2
{
    public class ExperimentalClass
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute()
        {
            Console.WriteLine("A.Execute");
        }

        public void ExecuteWithIntParam(int i)
        {
            Console.WriteLine($"A.ExecuteWithIntParam");
        }

        //public int ExecuteWithReturnInt()
        //{
        //    Console.WriteLine($"A.ExecuteWithReturnInt");
        //    return 42;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void ExecuteOld()
        //{
        //    Console.WriteLine("A.ExecuteOld");
        //}
    }
}
