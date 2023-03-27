namespace OhneSize;

public record class DataItem(
    string Name,
    string ContentType,
    DateTimeOffset ModifiedAt
);

public record class DataItem<TData>(
    string Name,
    TData Data,
    string ContentType,
    DateTimeOffset ModifiedAt
) {
    public DataItem(string name, TData data, string contentType)
            : this(name, data, contentType, DateTimeOffset.UtcNow) {
    }

    public DataItem AsDataItem()
        => new DataItem(this.Name, this.ContentType, this.ModifiedAt);
}

public record class DataItemText(
    string Name,
    string Data,
    string ContentType,
    DateTimeOffset ModifiedAt
    ) : DataItem<string>(Name, Data, ContentType, ModifiedAt);

public record class DataItemBinary(
    string Name,
    byte[] Data,
    string ContentType,
    DateTimeOffset ModifiedAt
    ) : DataItem<byte[]>(Name, Data, ContentType, ModifiedAt);

