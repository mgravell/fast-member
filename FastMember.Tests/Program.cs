
using System;
using System.Diagnostics;
using System.ComponentModel;
using FastMember;
using System.Reflection;
namespace FastMemberTests
{
    public class Program
    {
        public string Value { get; set; }
        static void Main()
        {
            var obj = new Program();
            obj.Value = "abc";
            GC.KeepAlive(obj.Value);
            const int loop = 5000000;
            string last = null;
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                last = obj.Value;
                obj.Value = "abc";
            }
            watch.Stop();
            Console.WriteLine("Static C#: {0}ms", watch.ElapsedMilliseconds);
#if !NO_DYNAMIC
            dynamic dlr = obj;
            watch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                last = dlr.Value;
                dlr.Value = "abc";
            }
            watch.Stop();
            Console.WriteLine("Dynamic C#: {0}ms", watch.ElapsedMilliseconds);
#endif
            var prop = typeof (Program).GetProperty("Value");
            watch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                last = (string)prop.GetValue(obj, null);
                prop.SetValue(obj, "abc", null);
            }
            watch.Stop();
            Console.WriteLine("PropertyInfo: {0}ms", watch.ElapsedMilliseconds);

#if !DNXCORE50
            var descriptor = TypeDescriptor.GetProperties(obj)["Value"];
            watch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                last = (string)descriptor.GetValue(obj);
                descriptor.SetValue(obj, "abc");
            }
            watch.Stop();
            Console.WriteLine("PropertyDescriptor: {0}ms", watch.ElapsedMilliseconds);

            Hyper.ComponentModel.HyperTypeDescriptionProvider.Add(typeof(Program));
#endif
            //descriptor = TypeDescriptor.GetProperties(obj)["Value"];
            //watch = Stopwatch.StartNew();
            //for (int i = 0; i < loop; i++)
            //{
            //    last = (string)descriptor.GetValue(obj);
            //    descriptor.SetValue(obj, "abc");
            //}
            //watch.Stop();
            //Console.WriteLine("HyperPropertyDescriptor: {0}ms", watch.ElapsedMilliseconds);

            var accessor = TypeAccessor.Create(typeof (Program));
            watch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                last = (string)accessor[obj, "Value"];
                accessor[obj, "Value"] = "abc";
            }
            watch.Stop();
            Console.WriteLine("TypeAccessor.Create: {0}ms", watch.ElapsedMilliseconds);

            var wrapped = ObjectAccessor.Create(obj);
            watch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                last = (string)wrapped["Value"];
                wrapped["Value"] = "abc";
            }
            watch.Stop();
            Console.WriteLine("ObjectAccessor.Create: {0}ms", watch.ElapsedMilliseconds);


            object lastObj = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            watch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                lastObj = new Program();
            }
            watch.Stop();
            Console.WriteLine("c# new(): {0}ms", watch.ElapsedMilliseconds);

            Type type = typeof (Program);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            watch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                lastObj = Activator.CreateInstance(type);
            }
            watch.Stop();
            Console.WriteLine("Activator.CreateInstance: {0}ms", watch.ElapsedMilliseconds);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            watch = Stopwatch.StartNew();
            for (int i = 0; i < loop; i++)
            {
                lastObj = accessor.CreateNew();
            }
            watch.Stop();
            Console.WriteLine("TypeAccessor.CreateNew: {0}ms", watch.ElapsedMilliseconds);

            GC.KeepAlive(last);
            GC.KeepAlive(lastObj);



        }
    }
}
