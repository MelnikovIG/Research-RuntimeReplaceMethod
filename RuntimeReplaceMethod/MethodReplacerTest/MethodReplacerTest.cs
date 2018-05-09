using System;
using System.Threading.Tasks;
using ConsoleApp2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MethodReplacerTest
{
    [TestClass]
    public class MethodReplacerTest
    {
        public class A_RetVoid_ArgEmpty
        {
            public int State { get; set; }

            public void Execute()
            {
                Console.WriteLine("Execute");
            }

            public void ExecuteWithStateChange()
            {
                Console.WriteLine("ExecuteWithStateChange");
                State = 1;
            }
        }

        public class A_RetVoid_ArgInt
        {
            public int State { get; set; }

            public void Execute(int i)
            {
                Console.WriteLine($"Execute {i}");
            }

            public void ExecuteWithStateChange(int i)
            {
                Console.WriteLine($"ExecuteWithStateChange {i}");
                State = i;
            }
        }

        public class A_RetInt_ArgInt
        {
            public int State { get; set; }

            public int Execute(int i)
            {
                Console.WriteLine($"Execute {i}");
                return State;
            }

            public int ExecuteWithStateChange(int i)
            {
                Console.WriteLine($"ExecuteWithStateChange {i}");
                State = i;
                return State;
            }
        }

        public class A_RetInt_ArgInt_Async
        {
            public int State { get; set; }

            public async Task<int> Execute(int i)
            {
                await Task.Delay(1);
                Console.WriteLine($"Execute {i}");
                return State;
            }

            public async Task<int> ExecuteWithStateChange(int i)
            {
                await Task.Delay(1);
                Console.WriteLine($"ExecuteWithStateChange {i}");
                State = i;
                return State;
            }
        }

        [TestMethod]
        public void TestMethod_RetVoid_ArgEmpty()
        {
            //Assign

            var targetMethod = typeof(A_RetVoid_ArgEmpty).GetMethod(nameof(A_RetVoid_ArgEmpty.Execute));
            var injectedMethod = typeof(A_RetVoid_ArgEmpty).GetMethod(nameof(A_RetVoid_ArgEmpty.ExecuteWithStateChange));

            var a = new A_RetVoid_ArgEmpty();
            a.Execute();
            var oldState = a.State;

            //Act

            MethodReplacer.Replace(targetMethod, injectedMethod);
            a.Execute();

            //Assert

            var newState = a.State;
            Assert.AreEqual(oldState, 0);
            Assert.AreEqual(newState, 1);
        }

        [TestMethod]
        public void TestMethod_RetVoid_ArgInt()
        {
            //Assign

            var state = 4;
            var targetMethod = typeof(A_RetVoid_ArgInt).GetMethod(nameof(A_RetVoid_ArgInt.Execute));
            var injectedMethod = typeof(A_RetVoid_ArgInt).GetMethod(nameof(A_RetVoid_ArgInt.ExecuteWithStateChange));

            var a = new A_RetVoid_ArgInt();
            a.Execute(state);
            var oldState = a.State;

            //Act

            MethodReplacer.Replace(targetMethod, injectedMethod);
            a.Execute(state);

            //Assert

            var newState = a.State;
            Assert.AreEqual(oldState, 0);
            Assert.AreEqual(newState, state);
        }

        [TestMethod]
        public void TestMethod_RetInt_ArgInt()
        {
            //Assign

            var state = 4;
            var targetMethod = typeof(A_RetInt_ArgInt).GetMethod(nameof(A_RetInt_ArgInt.Execute));
            var injectedMethod = typeof(A_RetInt_ArgInt).GetMethod(nameof(A_RetInt_ArgInt.ExecuteWithStateChange));

            var a = new A_RetInt_ArgInt();
            var oldState = a.Execute(state);

            //Act

            MethodReplacer.Replace(targetMethod, injectedMethod);
            var newState = a.Execute(state);

            //Assert

            Assert.AreEqual(oldState, 0);
            Assert.AreEqual(newState, state);
        }

        [TestMethod]
        public async Task TestMethod_RetInt_ArgInt_Async()
        {
            //Assign

            var state = 4;
            var targetMethod = typeof(A_RetInt_ArgInt_Async).GetMethod(nameof(A_RetInt_ArgInt_Async.Execute));
            var injectedMethod = typeof(A_RetInt_ArgInt_Async).GetMethod(nameof(A_RetInt_ArgInt_Async.ExecuteWithStateChange));

            var a = new A_RetInt_ArgInt_Async();
            var oldState = await a.Execute(state);

            //Act

            MethodReplacer.Replace(targetMethod, injectedMethod);
            var newState = await a.Execute(state);

            //Assert

            Assert.AreEqual(oldState, 0);
            Assert.AreEqual(newState, state);
        }
    }
}
