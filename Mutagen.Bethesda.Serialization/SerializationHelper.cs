using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Serialization;

public static class SerializationHelper
{
    public static void WriteGroup<TWriteObject, TGroup, TObject>(
        TWriteObject writer,
        TGroup group,
        string? fieldName,
        ISerializationWriterKernel<TWriteObject> kernel,
        Write<TWriteObject, TGroup> groupWriter,
        Write<TWriteObject, TObject> itemWriter)
        where TGroup : class, IReadOnlyCollection<TObject>
    {
        if (group.Count == 0) return;
        kernel.WriteLoqui(writer, fieldName, group, (w, g, k) =>
        {
            groupWriter(w, g, k);
            foreach (var recordGetter in g)
            {
                itemWriter(w, recordGetter, k);
            }
        });
    }
}