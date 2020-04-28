using System;
using Autofac;

namespace Decorators
{
    public class Program
    {
        public interface IReportingService
        {
            void Report();
        }

        public class ReportingService : IReportingService
        {
            public void Report()
            {
                Console.WriteLine("Reporting...");
            }
        }

        public class ReportingServiceWithLogging : IReportingService
        {
            /// <summary>
            /// The decorated service.
            /// </summary>
            private readonly IReportingService _reportingService;

            public ReportingServiceWithLogging(IReportingService reportingService)
            {
                _reportingService = reportingService ?? throw new ArgumentNullException(nameof(reportingService));
            }

            public void Report()
            {
                Console.WriteLine("Pre logging...");

                _reportingService.Report();

                Console.WriteLine("Post logging...");
            }
        }

        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ReportingService>()
                .Named<IReportingService>("reporting");

            // Decorating the ReportingService with the ReportingServiceWithLogging
            builder.RegisterDecorator<IReportingService>(
                (context, service) => new ReportingServiceWithLogging(service), "reporting");

            using var container = builder.Build();
            var reportingService = container.Resolve<IReportingService>();
            reportingService.Report();
        }
    }
}
