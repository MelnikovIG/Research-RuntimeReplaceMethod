using System;
using ConsoleApp2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicMethodCreatorTest
{
    [TestClass]
    public class DynamicMethodCreatorTest
    {
        public class A
        {
            public void Execute()
            {
                Console.WriteLine("Kek");
            }
        }

        [TestMethod]
        public void DynamicMethodCreatorTestCreate()
        {
            var targetMethod = typeof(A).GetMethod(nameof(A.Execute));
            var injectedMethod = DynamicMethodCreator.DynamicMethodCreator.CreateMethodSameAsOrigial(targetMethod);

            MethodReplacer.Replace(targetMethod, injectedMethod);

            new A().Execute();
        }
    }
}
