namespace FileManager.Core;

public sealed class OutcomeResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public IReadOnlyList<T> PartialItems { get; }
    
    public int PartialCount => PartialItems.Count;
    public bool HasPartial => PartialCount > 0;
    
    public OutcomeResult(IReadOnlyList<T> items, IReadOnlyList<T> partialItems)
    {
        Items = items;
        PartialItems = partialItems;
    }
}