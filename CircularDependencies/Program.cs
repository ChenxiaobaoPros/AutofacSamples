using System;
using Autofac;

namespace CircularDependencies
{
    public class Program
    {
        #region Property Dependency

        public class ParentWithProperty
        {
            public ChildWithProperty Child { get; set; }

            public override string ToString()
            {
                return "Parent";
            }
        }

        public class ChildWithProperty
        {
            public ParentWithProperty Parent { get; set; }

            public override string ToString()
            {
                return "Child";
            }
        }

        #endregion

        #region Constructor Dependency

        public class ParentWithConstructor
        {
            public ChildWithProperty1 Child { get; }

            public ParentWithConstructor(ChildWithProperty1 child)
            {
                Child = child ?? throw new ArgumentNullException(nameof(child));
            }

            public override string ToString()
            {
                return $"{this.GetType().Name} with a {Child.GetType().Name}";
            }
        }

        public class ChildWithProperty1
        {
            public ParentWithConstructor Parent { get; set; }

            public override string ToString()
            {
                return "Child";
            }
        }

        #endregion

        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // Auto-wired properties with circular dependencies
            builder.RegisterType<ParentWithProperty>()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<ChildWithProperty>()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<ParentWithConstructor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ChildWithProperty1>()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            using var container = builder.Build();

            var parentWithProperty = container.Resolve<ParentWithProperty>();
            Console.WriteLine(parentWithProperty.Child);

            var parentWithConstructor = container.Resolve<ParentWithConstructor>();
            Console.WriteLine(parentWithConstructor.Child.Parent);
        }
    }
}
