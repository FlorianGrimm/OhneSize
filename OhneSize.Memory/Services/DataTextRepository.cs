namespace OhneSize.Services;

[Brimborium.Registrator.Singleton(Mode = Brimborium.Registrator.ServiceTypeMode.IncludeSelf)]
public class DataTextRepository : DataRepository<string, DataItemText> {
    protected override DataItemText CreateDataItem(string name, string data, string contentType, DateTimeOffset modifiedAt) {
        return new DataItemText(name, data, contentType, modifiedAt);
    }
}
