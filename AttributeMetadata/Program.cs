using System;
using System.ComponentModel.Composition;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using Autofac.Features.AttributeFilters;

namespace AttributeMetadata
{
    [MetadataAttribute]
    public class AgeMetadataAttribute : Attribute
    {
        public int Age { get; set; }

        public AgeMetadataAttribute(int age)
        {
            Age = age;
        }
    }

    public interface IArtwork
    {
        void Display();
    }

    [AgeMetadata(100)]
    public class CenturyArtwork : IArtwork
    {
        public void Display()
        {
            Console.WriteLine("Displaying a century-old piece");
        }
    }

    [AgeMetadata(1000)]
    public class MillennialArtwork : IArtwork
    {
        public void Display()
        {
            Console.WriteLine("Displaying a REALLY old piece of art");
        }
    }

    public class ArtDisplay
    {
        private readonly IArtwork _artwork;

        public ArtDisplay([MetadataFilter("Age", 100)]IArtwork artwork)
        {
            _artwork = artwork ?? throw new ArgumentNullException(nameof(artwork));
        }

        public void Display()
        {
            _artwork.Display();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // Scans all registrations with metadata
            builder.RegisterModule<AttributedMetadataModule>();

            builder.RegisterType<CenturyArtwork>()
                .As<IArtwork>();

            builder.RegisterType<MillennialArtwork>()
                .As<IArtwork>();

            // enables attribute filtering on constructor dependencies
            builder.RegisterType<ArtDisplay>()
                .WithAttributeFiltering();

            using var container = builder.Build();

            container.Resolve<ArtDisplay>().Display();
        }
    }
}
