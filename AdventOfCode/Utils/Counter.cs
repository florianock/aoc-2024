using System.Collections;

namespace AdventOfCode.Utils;

public class Counter<TKey> : IEnumerable<KeyValuePair<TKey, long>> where TKey : class
{
    private readonly Dictionary<TKey, long> _counters;

    public Counter()
    {
        _counters = new Dictionary<TKey, long>();
    }

    public void Update(TKey id, long value = 1)
    {
        if (_counters.TryGetValue(id, out var count)) _counters[id] = count + value;
        else _counters.Add(id, value);
    }

    public IEnumerator<KeyValuePair<TKey, long>> GetEnumerator() => _counters.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}