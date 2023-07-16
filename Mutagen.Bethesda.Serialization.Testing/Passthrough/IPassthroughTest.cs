using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Testing.Passthrough;

public interface IPassthroughTest
{
    Task JsonSerialize(
        ISkyrimModGetter modGetter,
        DirectoryPath dir,
        IWorkDropoff workDropoff, 
        IFileSystem fileSystem, 
        ICreateStream streamCreator);
    
    Task<ISkyrimModGetter> JsonDeserialize(
        DirectoryPath dir,
        ModKey modKey,
        IWorkDropoff workDropoff,
        IFileSystem fileSystem, 
        ICreateStream streamCreator);
    
    Task YamlSerialize(
        ISkyrimModGetter modGetter,
        DirectoryPath dir,
        IWorkDropoff workDropoff,
        IFileSystem fileSystem, 
        ICreateStream streamCreator);
    
    Task<ISkyrimModGetter> YamlDeserialize(
        DirectoryPath dir,
        ModKey modKey,
        IWorkDropoff workDropoff,
        IFileSystem fileSystem, 
        ICreateStream streamCreator);
}