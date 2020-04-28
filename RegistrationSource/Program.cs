using Autofac.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Autofac;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;

namespace RegistrationSource
{
    public class Program
    {
        public abstract class BaseHandler
        {
            public virtual string Handle(string message)
            {
                return $"Handled {message}";
            }
        }

        public class HandlerA : BaseHandler
        {
            public override string Handle(string message)
            {
                return $"Handled by A: {message}";
            }
        }

        public class HandlerB : BaseHandler
        {
            public override string Handle(string message)
            {
                return $"Handled by B: {message}";
            }
        }

        public interface IHandlerFactory
        {
            T GetHandler<T>() where T : BaseHandler;
        }

        public class HandlerFactory : IHandlerFactory
        {
            public T GetHandler<T>() where T : BaseHandler
            {
                return Activator.CreateInstance<T>();
            }
        }

        public class ConsumerA
        {
            private readonly HandlerA _handlerA;

            public ConsumerA(HandlerA handlerA)
            {
                _handlerA = handlerA ?? throw new ArgumentNullException(nameof(handlerA));
            }

            public void DoWork()
            {
                Console.WriteLine(_handlerA.Handle("ConsumerA"));
            }
        }

        public class ConsumerB
        {
            private readonly HandlerB _handlerB;

            public ConsumerB(HandlerB handlerB)
            {
                _handlerB = handlerB ?? throw new ArgumentNullException(nameof(handlerB)); ;
            }

            public void DoWork()
            {
                Console.WriteLine(_handlerB.Handle("ConsumerB"));
            }
        }

        public class HandlerRegistrationSource : IRegistrationSource
        {
            public bool IsAdapterForIndividualComponents => false;

            public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
            {
                if (!(service is IServiceWithType swt)
                    || swt.ServiceType == null
                    || !swt.ServiceType.IsAssignableTo<BaseHandler>())
                {
                    yield break;
                }

                yield return new ComponentRegistration(
                    Guid.NewGuid(),
                    new DelegateActivator(
                        swt.ServiceType,
                        (context, parameters) =>
                        {
                            var provider = context.Resolve<IHandlerFactory>();
                            var method = provider.GetType().GetMethod("GetHandler")?.MakeGenericMethod(swt.ServiceType);
                            return method.Invoke(provider, null);
                        }),
                    new CurrentScopeLifetime(),
                    InstanceSharing.None,
                    InstanceOwnership.OwnedByLifetimeScope,
                    new[] { service },
                    new ConcurrentDictionary<string, object>());
            }
        }

        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<HandlerFactory>()
                .As<IHandlerFactory>();

            builder.RegisterSource(new HandlerRegistrationSource());

            builder.RegisterType<ConsumerA>();
            builder.RegisterType<ConsumerB>();

            using var container = builder.Build();

            container.Resolve<ConsumerA>().DoWork();
            container.Resolve<ConsumerB>().DoWork();
        }
    }
}
