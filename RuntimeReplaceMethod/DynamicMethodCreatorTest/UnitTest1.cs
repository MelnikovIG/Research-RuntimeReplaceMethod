using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicMethodCreatorTest
{
    [TestClass]
    public class UnitTest1
    {
        public class A
        {
            public void Execute()
            {

            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            var targetMethod = typeof(A).GetMethod(nameof(A.Execute));
            var dynamicMethod = DynamicMethodCreator.DynamicMethodCreator.CreateMethodSameAsOrigial(targetMethod);
        }
    }
}
