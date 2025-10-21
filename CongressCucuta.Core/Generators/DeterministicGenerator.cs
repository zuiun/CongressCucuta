namespace CongressCucuta.Core.Generators;

public class DeterministicGenerator (List<int> values) : IGenerator {
    private readonly List<int> _values = values;
    private int _idx = 0;

    public int Choose (int maximum) {
        if (_idx < _values.Count) {
            int value = _values[_idx];

            ++_idx;
            return value;
        } else {
            return -1;
        }
    }

    public int Roll () {
        if (_idx < _values.Count) {
            int value = _values[_idx];

            ++_idx;
            return value;
        } else {
            return -1;
        }
    }
}
