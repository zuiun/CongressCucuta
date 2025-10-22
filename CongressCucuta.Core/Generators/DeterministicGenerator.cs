namespace CongressCucuta.Core.Generators;

public class DeterministicGenerator (List<int> values) : IGenerator {
    private readonly List<int> _values = values;
    private int _idx = 0;

    public int Choose (int maximum) {
        int value = _values[_idx];

        _idx = (_idx + 1) % _values.Count;
        return value;
    }

    public int Roll () {
        int value = _values[_idx];

        _idx = (_idx + 1) % _values.Count;
        return value;
    }
}
