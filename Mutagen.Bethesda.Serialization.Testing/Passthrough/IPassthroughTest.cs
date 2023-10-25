using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Fallout4;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Serialization.Testing.Passthrough;

public interface IPassthroughTest
{
    Task JsonSerialize(
        IFallout4ModGetter modGetter,
        DirectoryPath dir,
        IWorkDropoff workDropoff, 
        IFileSystem fileSystem, 
        ICreateStream streamCreator);
    
    Task<IFallout4ModGetter> JsonDeserialize(
        DirectoryPath dir,
        ModKey modKey,
        IWorkDropoff workDropoff,
        IFileSystem fileSystem, 
        ICreateStream streamCreator);
    
    Task YamlSerialize(
        IFallout4ModGetter modGetter,
        DirectoryPath dir,
        IWorkDropoff workDropoff,
        IFileSystem fileSystem, 
        ICreateStream streamCreator);
    
    Task<IFallout4ModGetter> YamlDeserialize(
        DirectoryPath dir,
        ModKey modKey,
        IWorkDropoff workDropoff,
        IFileSystem fileSystem, 
        ICreateStream streamCreator);
}