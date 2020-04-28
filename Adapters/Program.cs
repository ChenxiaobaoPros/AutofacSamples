using System;
using System.Collections.Generic;
using Autofac;

namespace Adapters
{
    public class Program
    {
        public interface ICommand
        {
            void Execute();
        }

        public class SaveCommand : ICommand
        {
            public void Execute()
            {
                Console.WriteLine("Saving a file");
            }
        }

        public class OpenCommand : ICommand
        {
            public void Execute()
            {
                Console.WriteLine("Opening a file");
            }
        }

        public class Button
        {
            private readonly ICommand _command;

            public Button(ICommand command)
            {
                _command = command ?? throw new ArgumentNullException(nameof(command));
            }

            public void Click()
            {
                _command.Execute();
            }
        }

        public class Editor
        {
            private readonly IEnumerable<Button> _buttons;

            public Editor(IEnumerable<Button> buttons)
            {
                _buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
            }

            public void ClickAll()
            {
                foreach (var button in _buttons)
                {
                    button.Click();
                }
            }
        }

        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SaveCommand>()
                .As<ICommand>();

            builder.RegisterType<OpenCommand>()
                .As<ICommand>();

            // Adapt all components implementing service ICommand to provide
            // Button using the provided adapter function.
            builder.RegisterAdapter<ICommand, Button>(command => new Button(command));

            builder.RegisterType<Button>();

            builder.RegisterType<Editor>();

            using var container = builder.Build();

            var editor = container.Resolve<Editor>();

            editor.ClickAll();
        }
    }
}
