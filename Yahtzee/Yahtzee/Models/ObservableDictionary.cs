using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

public class ObservableDictionary<TKey, TValue> :
    IDictionary<TKey, TValue>,
    INotifyCollectionChanged,
    INotifyPropertyChanged
{
    private readonly Dictionary<TKey, TValue> _dict = new();

    public event NotifyCollectionChangedEventHandler CollectionChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableDictionary()
    {
    }

    void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    void OnCollectionChanged(NotifyCollectionChangedEventArgs args) =>
        CollectionChanged?.Invoke(this, args);

    public TValue this[TKey key]
    {
        get => _dict[key];
        set
        {
            bool exists = _dict.ContainsKey(key);
            _dict[key] = value;

            if (exists)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace, value, key));
            else
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, value, key));

            OnPropertyChanged(nameof(Count));
        }
    }

    public ICollection<TKey> Keys => _dict.Keys;
    public ICollection<TValue> Values => _dict.Values;
    public int Count => _dict.Count;
    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        _dict.Add(key, value);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Add, value, key));
        OnPropertyChanged(nameof(Count));
    }

    public bool Remove(TKey key)
    {
        if (_dict.TryGetValue(key, out var value) && _dict.Remove(key))
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, value, key));
            OnPropertyChanged(nameof(Count));
            return true;
        }
        return false;
    }

    public bool ContainsKey(TKey key) => _dict.ContainsKey(key);
    public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);

    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public void Clear()
    {
        _dict.Clear();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(nameof(Count));
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) => _dict.ContainsKey(item.Key);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
        ((IDictionary<TKey, TValue>)_dict).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();
}