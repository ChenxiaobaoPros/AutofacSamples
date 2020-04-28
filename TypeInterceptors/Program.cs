using System;
using System.IO;
using System.Linq;
using System.Xml;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;

namespace TypeInterceptors
{
    public class CallLogger : IInterceptor
    {
        private readonly TextWriter _writer;

        public CallLogger(TextWriter writer)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;

            _writer.WriteLine("Calling method '{0}' with args '{1}'",
                methodName,
                string.Join(",",
                    invocation.Arguments.Select(a => (a ?? string.Empty).ToString())
                )
            );

            invocation.Proceed();

            _writer.WriteLine("Done calling '{0}', result was '{1}'",
                methodName,
                invocation.ReturnValue
            );
        }
    }

    public interface IAudit
    {
        int Start(DateTime reportDate);
    }

    // Intercepts CallLogger
    [Intercept(typeof(CallLogger))]
    public class Audit : IAudit
    {
        public virtual int Start(DateTime reportDate)
        {
            Console.WriteLine($"Starting report on {reportDate}");
            return new Random().Next();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.Register(c => new CallLogger(Console.Out))
                .As<IInterceptor>()
                .AsSelf();

            builder.RegisterType<Audit>()
                .As<IAudit>()
                .EnableInterfaceInterceptors();

            using var container = builder.Build();

            var audit = container.Resolve<IAudit>();
            audit.Start(DateTime.Now);
        }
    }
}
