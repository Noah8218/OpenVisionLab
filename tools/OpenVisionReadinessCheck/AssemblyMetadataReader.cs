using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace OpenVisionReadinessCheck;

internal static class AssemblyMetadataReader
{
    public static int Run(string[] args)
    {
        if (args.Length != 1 || string.IsNullOrWhiteSpace(args[0]))
        {
            Console.Error.WriteLine("Assembly metadata reader requires one assembly path.");
            return 2;
        }

        string assemblyPath = Path.GetFullPath(args[0]);
        if (!File.Exists(assemblyPath))
        {
            Console.Error.WriteLine($"Assembly metadata reader could not find: {assemblyPath}");
            return 2;
        }

        try
        {
            Dictionary<string, string> metadata = Read(assemblyPath);
            Console.WriteLine(JsonSerializer.Serialize(metadata));
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Assembly metadata reader failed: {exception.Message}");
            return 1;
        }
    }

    private static Dictionary<string, string> Read(string assemblyPath)
    {
        using FileStream stream = File.OpenRead(assemblyPath);
        using PEReader peReader = new PEReader(stream);
        if (!peReader.HasMetadata)
        {
            throw new InvalidDataException("The file does not contain CLI metadata.");
        }

        MetadataReader reader = peReader.GetMetadataReader();
        AssemblyDefinition assembly = reader.GetAssemblyDefinition();
        Dictionary<string, string> metadata = new(StringComparer.Ordinal);

        foreach (CustomAttributeHandle attributeHandle in assembly.GetCustomAttributes())
        {
            CustomAttribute attribute = reader.GetCustomAttribute(attributeHandle);
            if (!IsAssemblyMetadataAttribute(reader, attribute.Constructor))
            {
                continue;
            }

            BlobReader blobReader = reader.GetBlobReader(attribute.Value);
            if (blobReader.ReadUInt16() != 1)
            {
                throw new InvalidDataException("AssemblyMetadataAttribute has an invalid prolog.");
            }

            string name = blobReader.ReadSerializedString();
            string value = blobReader.ReadSerializedString();
            if (string.IsNullOrWhiteSpace(name) || value is null)
            {
                throw new InvalidDataException("AssemblyMetadataAttribute has an invalid name or value.");
            }

            metadata[name] = value;
        }

        return metadata;
    }

    private static bool IsAssemblyMetadataAttribute(MetadataReader reader, EntityHandle constructor)
    {
        EntityHandle declaringType = constructor.Kind switch
        {
            HandleKind.MemberReference => reader.GetMemberReference((MemberReferenceHandle)constructor).Parent,
            HandleKind.MethodDefinition => reader.GetMethodDefinition((MethodDefinitionHandle)constructor).GetDeclaringType(),
            _ => default
        };

        return declaringType.Kind switch
        {
            HandleKind.TypeReference => IsAssemblyMetadataType(reader.GetTypeReference((TypeReferenceHandle)declaringType), reader),
            HandleKind.TypeDefinition => IsAssemblyMetadataType(reader.GetTypeDefinition((TypeDefinitionHandle)declaringType), reader),
            _ => false
        };
    }

    private static bool IsAssemblyMetadataType(TypeReference type, MetadataReader reader)
    {
        return string.Equals(reader.GetString(type.Namespace), "System.Reflection", StringComparison.Ordinal)
            && string.Equals(reader.GetString(type.Name), "AssemblyMetadataAttribute", StringComparison.Ordinal);
    }

    private static bool IsAssemblyMetadataType(TypeDefinition type, MetadataReader reader)
    {
        return string.Equals(reader.GetString(type.Namespace), "System.Reflection", StringComparison.Ordinal)
            && string.Equals(reader.GetString(type.Name), "AssemblyMetadataAttribute", StringComparison.Ordinal);
    }
}
