using System;
using System.ComponentModel;
using System.Linq;
using Autofac;
using Autofac.Extras.AggregateService;

namespace AggregateServices
{
    public interface IService1 { }
    public interface IService2 { }
    public interface IService3 { }
    public interface IService4 { }

    public class ConcreteService1 : IService1 { }
    public class ConcreteService2 : IService2 { }
    public class ConcreteService3 : IService3 { }

    public class ConcreteService4 : IService4
    {
        private string _name;

        public ConcreteService4(string name)
        {
            _name = name ?? throw new ArgumentNullException();
        }

    }

    /// <summary>
    /// The aggregate service from Autofac depends on Castle.Core
    /// </summary>
    public interface IAggregateService
    {
        IService1 Service1 { get; }
        IService2 Service2 { get; }
        IService3 Service3 { get; }
        IService4 Service4 { get; }
    }

    public class Consumer
    {
        private readonly IAggregateService _aggregateService;

        public Consumer(IAggregateService aggregateService)
        {
            _aggregateService = aggregateService ?? throw new ArgumentNullException(nameof(aggregateService));
        }

        public void Consume()
        {
            var aggServiceProperties = _aggregateService.GetType().GetProperties().Select(p => p.Name);

            Console.WriteLine($"'{this._aggregateService.GetType().Name}' has '{string.Join(", ", aggServiceProperties)}'");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // depends on Castle.Core
            builder.RegisterAggregateService<IAggregateService>();

            // register assembly types which names' starts with 'Concrete'
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => t.Name.StartsWith("Concrete"))
                .Except<ConcreteService4>()
                .AsImplementedInterfaces();

            // register concrete service with Named Parameter
            builder.RegisterType<ConcreteService4>()
                .As<IService4>()
                .WithParameter("name", $"{nameof(ConcreteService4)}");

            builder.RegisterType<Consumer>();

            using var container = builder.Build();

            var consumer = container.Resolve<Consumer>();

            consumer.Consume();
        }
    }
}
