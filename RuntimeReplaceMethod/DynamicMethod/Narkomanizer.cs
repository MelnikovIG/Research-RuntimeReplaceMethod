using System.Linq;
using System.Reflection;

namespace ConsoleApp2
{
    public static class Narkomanizer
    {
        public static void Narkomanize<T>()
        {
            var methods = typeof(T).GetMethods(BindingFlags.Public/* | BindingFlags.NonPublic*/ | BindingFlags.Instance | BindingFlags.Static);

            //var newMethod = methods.First(x => x.Name == "ExecuteOld");

            foreach (var methodInfo in methods)
            {
                if (methodInfo.Name == "Execute" || methodInfo.Name == "ExecuteWithIntParam" || methodInfo.Name == "ExecuteWithReturnInt")
                {
                    var newMethod = MethodMerger.Merge(methodInfo);
                    MethodReplacer.Replace(methodInfo, newMethod);
                }

                //Console.WriteLine(methodInfo.Name);
            }
        }

    }
}
