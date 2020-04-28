using System;
using Autofac;

namespace AutofacSamples
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

            using (var container = builder.Build())
            {

            }
        }
    }
}
