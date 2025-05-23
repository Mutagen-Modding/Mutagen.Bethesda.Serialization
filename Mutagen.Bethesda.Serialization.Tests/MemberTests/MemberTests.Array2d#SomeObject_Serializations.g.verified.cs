﻿//HintName: SomeObject_Serializations.g.cs
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Serialization;
using Mutagen.Bethesda.Serialization.Exceptions;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Noggog;
using Noggog.IO;
using Noggog.WorkEngine;
using System.IO.Abstractions;
using System.Threading.Tasks;

#nullable enable

#pragma warning disable CS1998 // No awaits used
#pragma warning disable CS0618 // Obsolete

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

internal static class SomeObject_Serialization
{
    public static async Task Serialize<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        await SerializeFields<TKernel, TWriteObject>(
            writer: writer,
            item: item,
            kernel: kernel,
            metaData: metaData);
    }

    public static async Task SerializeFields<TKernel, TWriteObject>(
        TWriteObject writer,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter item,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        SerializationMetaData metaData)
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
        where TWriteObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        if (item.SomeArray is {} checkedSomeArray)
        {
            kernel.StartArray2dSection(writer, "SomeArray");
            for (int y = 0; y < checkedSomeArray.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray2 is {} checkedSomeArray2)
        {
            kernel.StartArray2dSection(writer, "SomeArray2");
            for (int y = 0; y < checkedSomeArray2.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray2.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray2[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray3 is {} checkedSomeArray3)
        {
            kernel.StartArray2dSection(writer, "SomeArray3");
            for (int y = 0; y < checkedSomeArray3.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray3.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray3[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray4 is {} checkedSomeArray4)
        {
            kernel.StartArray2dSection(writer, "SomeArray4");
            for (int y = 0; y < checkedSomeArray4.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray4.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray4[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray5 is {} checkedSomeArray5)
        {
            kernel.StartArray2dSection(writer, "SomeArray5");
            for (int y = 0; y < checkedSomeArray5.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray5.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray5[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray6 is {} checkedSomeArray6)
        {
            kernel.StartArray2dSection(writer, "SomeArray6");
            for (int y = 0; y < checkedSomeArray6.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray6.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteString(writer, null, checkedSomeArray6[x, y], default(string), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray7 is {} checkedSomeArray7)
        {
            kernel.StartArray2dSection(writer, "SomeArray7");
            for (int y = 0; y < checkedSomeArray7.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray7.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteFormKey(writer, null, checkedSomeArray7[x, y].FormKeyNullable, default(FormKey), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray8 is {} checkedSomeArray8)
        {
            kernel.StartArray2dSection(writer, "SomeArray8");
            for (int y = 0; y < checkedSomeArray8.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray8.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteFormKey(writer, null, checkedSomeArray8[x, y].FormKeyNullable, default(FormKey), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
        if (item.SomeArray9 is {} checkedSomeArray9)
        {
            kernel.StartArray2dSection(writer, "SomeArray9");
            for (int y = 0; y < checkedSomeArray9.Height; y++)
            {
                kernel.StartArray2dYSection(writer);
                for (int x = 0; x < checkedSomeArray9.Width; x++)
                {
                    kernel.StartArray2dXItem(writer);
                    kernel.WriteFormKey(writer, null, checkedSomeArray9[x, y].FormKeyNullable, default(FormKey), checkDefaults: false);
                    kernel.EndArray2dXItem(writer);
                }
                kernel.EndArray2dYSection(writer);
            }
            kernel.EndArray2dSection(writer);
        }
    }

    public static bool HasSerializationItems(
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObjectGetter? item,
        SerializationMetaData metaData)
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        if (item == null) return false;
        return true;
    }

    public static async Task<Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject> Deserialize<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        var obj = new Mutagen.Bethesda.Serialization.SourceGenerator.Tests.SomeObject();
        await DeserializeInto<TReadObject>(
            reader: reader,
            kernel: kernel,
            obj: obj,
            metaData: metaData);
        return obj;
    }

    public static async Task DeserializeSingleFieldInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData,
        string name)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        switch (name)
        {
            case "SomeArray":
                if (obj.SomeArray is {} checkedSomeArray)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray");
                            obj.SomeArray[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray2":
                if (obj.SomeArray2 is {} checkedSomeArray2)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray2");
                            obj.SomeArray2[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray3":
                if (obj.SomeArray3 is {} checkedSomeArray3)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray3");
                            obj.SomeArray3[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray4":
                obj.SomeArray4 = new Array2d<string>(SomeObject.SomeArray4FixedSize, default(string));
                if (obj.SomeArray4 is {} checkedSomeArray4)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray4");
                            obj.SomeArray4[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray5":
                obj.SomeArray5 = new Array2d<string>(SomeObject.SomeArray5FixedSize, default(string));
                if (obj.SomeArray5 is {} checkedSomeArray5)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray5");
                            obj.SomeArray5[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray6":
                obj.SomeArray6 = new Array2d<string>(SomeObject.SomeArray6FixedSize, default(string));
                if (obj.SomeArray6 is {} checkedSomeArray6)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = SerializationHelper.StripNull(kernel.ReadString(reader), name: "SomeArray6");
                            obj.SomeArray6[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray7":
                obj.SomeArray7 = new Array2d<IFormLink<ISurfaceBlockGetter>>(SomeObject.SomeArray7FixedSize, FormLink<ISurfaceBlockGetter>.Null);
                if (obj.SomeArray7 is {} checkedSomeArray7)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = kernel.ReadFormKey(reader).StripNull("SomeArray7").ToLink<ISurfaceBlockGetter>();
                            obj.SomeArray7[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray8":
                obj.SomeArray8 = new Array2d<IFormLink<ISurfaceBlockGetter>>(SomeObject.SomeArray8FixedSize, FormLink<ISurfaceBlockGetter>.Null);
                if (obj.SomeArray8 is {} checkedSomeArray8)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = kernel.ReadFormKey(reader).StripNull("SomeArray8").ToLink<ISurfaceBlockGetter>();
                            obj.SomeArray8[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            case "SomeArray9":
                obj.SomeArray9 = new Array2d<IFormLinkGetter<ISurfaceBlockGetter>>(SomeObject.SomeArray9FixedSize, FormLink<ISurfaceBlockGetter>.Null);
                if (obj.SomeArray9 is {} checkedSomeArray9)
                {
                    kernel.StartArray2dSection(reader);
                    int y = 0;
                    while (kernel.TryHasNextArray2dYSection(reader))
                    {
                        kernel.StartArray2dYSection(reader);
                        int x = 0;
                        while (kernel.TryHasNextArray2dXItem(reader))
                        {
                            kernel.StartArray2dXItem(reader);
                            var item = kernel.ReadFormKey(reader).StripNull("SomeArray9").ToLink<ISurfaceBlockGetter>();
                            obj.SomeArray9[x, y] = item;
                            kernel.EndArray2dXItem(reader);
                            x++;
                        }
                        kernel.EndArray2dYSection(reader);
                        y++;
                    }
                    kernel.EndArray2dSection(reader);
                }
                break;
            default:
                kernel.Skip(reader);
                break;
        }
    }
    
    public static async Task DeserializeInto<TReadObject>(
        TReadObject reader,
        ISerializationReaderKernel<TReadObject> kernel,
        Mutagen.Bethesda.Serialization.SourceGenerator.Tests.ISomeObject obj,
        SerializationMetaData metaData)
        where TReadObject : IContainStreamPackage
    {
        metaData.Cancel.ThrowIfCancellationRequested();
        while (kernel.TryGetNextField(reader, out var name))
        {
            await DeserializeSingleFieldInto<TReadObject>(
                reader: reader,
                kernel: kernel,
                obj: obj,
                metaData: metaData,
                name: name);
        }

    }

}

