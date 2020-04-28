using Autofac;
using System;
using Autofac.Features.ResolveAnything;

namespace NotRegistered
{
    class Program
    {
        interface ISpeakable
        {
            void Speak();
        }

        class Person : ISpeakable
        {
            public void Speak()
            {
                Console.WriteLine("Hello");
            }
        }

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // on-the-fly registration source
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            using var container = builder.Build();

            var person = container.Resolve<Person>();
            person.Speak();
        }
    }
}
