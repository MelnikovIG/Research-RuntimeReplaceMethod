using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            //new ExperimentalClass().Execute();
            //new ExperimentalClass().ExecuteWithIntParam(1);

            Narkomanizer.Narkomanize<ExperimentalClass>();

            new ExperimentalClass().Execute();
            new ExperimentalClass().ExecuteWithIntParam(1);
            Console.ReadKey();
        }

    }
}
