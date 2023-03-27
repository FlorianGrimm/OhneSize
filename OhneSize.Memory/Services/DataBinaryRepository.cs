namespace OhneSize.Services;

[Brimborium.Registrator.Singleton]
public class DataBinaryRepository : DataRepository<byte[], DataItemBinary> {
    protected override DataItemBinary CreateDataItem(string name, byte[] data, string contentType, DateTimeOffset modifiedAt) {
        return new DataItemBinary(name, data, contentType, modifiedAt);
    }
}
