using CongressCucuta.Core.Generators;

namespace CongressCucuta.Tests.Fakes;

public class FakeGenerator (params IEnumerable<int> values) : IGenerator {
    private readonly int[] _values = [.. values];
    private int _idx = 0;

    public int Choose (int maximum) {
        int value = _values[_idx];

        _idx = (_idx + 1) % _values.Length;
        return value;
    }

    public int Roll () {
        int value = _values[_idx];

        _idx = (_idx + 1) % _values.Length;
        return value;
    }
}
