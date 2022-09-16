namespace Mutagen.Bethesda.Serialization;

public static class SerializationHelper
{
    public static void WriteGroup<TKernel, TWriteObject, TGroup, TObject>(
        TWriteObject writer,
        TGroup group,
        string? fieldName,
        MutagenSerializationWriterKernel<TKernel, TWriteObject> kernel,
        Write<TKernel, TWriteObject, TGroup> groupWriter,
        Write<TKernel, TWriteObject, TObject> itemWriter)
        where TGroup : class, IReadOnlyCollection<TObject>
        where TKernel : ISerializationWriterKernel<TWriteObject>, new()
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