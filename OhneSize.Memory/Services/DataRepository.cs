using System.Diagnostics.CodeAnalysis;

namespace OhneSize.Services;

public abstract class DataRepository<TData, TDataItem>
    where TDataItem : DataItem<TData> {

    public DataRepository() {
        this.ListItem = new List<DataItem<TData>>();
        this.NameComparer = StringComparer.OrdinalIgnoreCase;
    }

    protected StringComparer NameComparer;

    abstract protected TDataItem CreateDataItem(string name, TData data, string contentType, DateTimeOffset ModifiedAt);

    public List<DataItem<TData>> ListItem { get; }

    public List<T> GetAll<T>(Func<DataItem<TData>, bool>? predicate, Func<DataItem<TData>, T> selector) {
        lock (this.ListItem) {
            IEnumerable<DataItem<TData>> listItemFiltered
                = (predicate is null)
                ? this.ListItem
                : this.ListItem.Where(predicate)
                ;
            return listItemFiltered.Select(selector).ToList();
        }
    }
    public List<DataItem> GetAllDataItem(Func<DataItem<TData>, bool>? predicate) {
        lock (this.ListItem) {
            IEnumerable<DataItem<TData>> listItemFiltered
                = (predicate is null)
                ? this.ListItem
                : this.ListItem.Where(predicate)
                ;
            return listItemFiltered.Select((item) => item.AsDataItem()).ToList();
        }
    }

    public virtual bool TryGetByName(string name, [MaybeNullWhen(false)] out DataItem<TData> value, out int index) {
        lock (this.ListItem) {
            return this.TryGetByNameInternal(name, out value, out index);
        }
    }

    public virtual bool TryGetByNameInternal(string name, [MaybeNullWhen(false)] out DataItem<TData> value, out int index) {
        var idx = 0;
        foreach (var item in this.ListItem) {
            var result = this.NameComparer.Compare(item.Name, name);
            if (result < 0) {
                idx++;
            } else if (result == 0) {
                value = item;
                index = idx;
                return true;
            } else {
                value = default;
                index = ~idx;
                return false;
            }
        }
        value = default;
        index = ~idx;
        return false;
    }

    public virtual (bool added, DataItem<TData>? dataItem) Add(string name, TData data, string contentType) {
        lock (this.ListItem) {
            return this.AddInternal(name, data, contentType);
        }
    }

    public virtual (bool added, DataItem<TData>? dataItem) AddInternal(string name, TData data, string contentType) {
        bool found = this.TryGetByNameInternal(name, out var _, out var index);
        if (found) {
            return (false, default);
        } else {
            var idx = ~index;
            var dataItem = new DataItem<TData>(name, data, contentType, DateTimeOffset.UtcNow);
            this.ListItem.Insert(idx, dataItem);
            return (true, dataItem);
        }
    }

    public virtual bool Update(string name, TData data, string contentType) {
        lock (this.ListItem) {
            return this.UpdateInternal(name, data, contentType);
        }
    }

    public virtual bool UpdateInternal(string name, TData data, string contentType) {
        bool found = this.TryGetByNameInternal(name, out var _, out var index);
        if (found) {
            return false;
        } else {
            var idx = ~index;
            var dataItem = new DataItem<TData>(name, data, contentType, DateTimeOffset.UtcNow);
            this.ListItem.Insert(idx, dataItem);
            return true;
        }
    }

    public virtual (bool added, DataItem<TData>? dataItem) Upsert(string name, TData data, string contentType) {
        lock (this.ListItem) {
            return this.UpsertInternal(name, data, contentType);
        }
    }

    public virtual (bool added, DataItem<TData>? dataItem) UpsertInternal(string name, TData data, string contentType) {
        bool found = this.TryGetByNameInternal(name, out var _, out var index);
        var dataItem = new DataItem<TData>(name, data, contentType, DateTimeOffset.UtcNow);
        if (found) {
            this.ListItem[index] = dataItem;
            return (false, dataItem);
        } else {
            var idx = ~index;
            this.ListItem.Insert(idx, dataItem);
            return (true, dataItem);
        }
    }

    public virtual bool Delete(string name) {
        lock (this.ListItem) {
            return this.DeleteInternal(name);
        }
    }

    public virtual bool DeleteInternal(string name) {
        bool found = this.TryGetByNameInternal(name, out var _, out var index);
        if (found && (0 <= index)) {
            this.ListItem.RemoveAt(index);
            return true;
        } else {
            return false;
        }
    }

    public virtual void Clear() {
        lock (this.ListItem) {
            this.ListItem.Clear();
        }
    }

}
