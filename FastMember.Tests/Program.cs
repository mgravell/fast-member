using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FastMember;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Exporters;

namespace FastMemberTests
{
    public class Program
    {
        public class FastMemberPerformance
        {
            public string Value { get; set; }

            private FastMemberPerformance obj;
            private dynamic dlr;
            private PropertyInfo prop;
            private PropertyDescriptor descriptor;

            private TypeAccessor accessor;
            private ObjectAccessor wrapped;

            private Type type;

            public static void Main(string[] args)
            {
                var summary = BenchmarkRunner.Run<FastMemberPerformance>(new Config());
                Console.WriteLine();
                // Display a summary to match the output of the original Performance test
                foreach (var report in summary.Reports.OrderBy(r => r.Benchmark.Target.MethodTitle))
                {
                    Console.WriteLine("{0}: {1:N2} ns", report.Benchmark.Target.MethodTitle, report.ResultStatistics.Median);
                }
                Console.WriteLine();
            }

            [Setup]
            public void Setup()
            {
                obj = new FastMemberPerformance();
                dlr = obj;
                prop = typeof(FastMemberPerformance).GetProperty("Value");
                descriptor = TypeDescriptor.GetProperties(obj)["Value"];

                // FastMember specific code
                accessor = FastMember.TypeAccessor.Create(typeof(FastMemberPerformance));
                wrapped = FastMember.ObjectAccessor.Create(obj);

                type = typeof(FastMemberPerformance);
            }

            [Benchmark(Description = "1. Static C#", Baseline = true)]
            public string StaticCSharp()
            {
                obj.Value = "abc";
                return obj.Value;
            }

            [Benchmark(Description = "2. Dynamic C#")]
            public string DynamicCSharp()
            {
                dlr.Value = "abc";
                return dlr.Value;
            }

            [Benchmark(Description = "3. PropertyInfo")]
            public string PropertyInfo()
            {
                prop.SetValue(obj, "abc", null);
                return (string)prop.GetValue(obj, null);
            }

            [Benchmark(Description = "4. PropertyDescriptor")]
            public string PropertyDescriptor()
            {
                descriptor.SetValue(obj, "abc");
                return (string)descriptor.GetValue(obj);
            }

            [Benchmark(Description = "5. TypeAccessor.Create")]
            public string TypeAccessor()
            {
                accessor[obj, "Value"] = "abc";
                return (string)accessor[obj, "Value"];
            }

            [Benchmark(Description = "6. ObjectAccessor.Create")]
            public string ObjectAccessor()
            {
                wrapped["Value"] = "abc";
                return (string)wrapped["Value"];
            }

            [Benchmark(Description = "7. c# new()")]
            public FastMemberPerformance CSharpNew()
            {
                return new FastMemberPerformance();
            }

            [Benchmark(Description = "8. Activator.CreateInstance")]
            public object ActivatorCreateInstance()
            {
                return Activator.CreateInstance(type);
            }

            [Benchmark(Description = "9. TypeAccessor.CreateNew")]
            public object TypeAccessorCreateNew()
            {
                return accessor.CreateNew();
            }
        }

        // BenchmarkDotNet settings (you can use the defaults, but these are tailored for this benchmark)
        private class Config : ManualConfig
        {
            public Config()
            {
                Add(Job.Default.WithLaunchCount(1));
                Add(PropertyColumn.Method);
                Add(StatisticColumn.Median, StatisticColumn.StdDev);
                Add(CsvExporter.Default, MarkdownExporter.Default, MarkdownExporter.GitHub);
                Add(new ConsoleLogger());
            }
        }
    }
}
