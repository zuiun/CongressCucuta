using System.Text.Json;
using CongressCucuta.Converters;
using CongressCucuta.Core;

namespace CongressCucuta.Tests.Unit.Converters;

[TestClass]
public sealed class IDTypeJsonConverterTests {
    private static readonly JsonSerializerOptions _options = new () {
        Converters = { new IDTypeJsonConverter () },
    };

    [TestMethod]
    [DataRow ((byte) 0)]
    [DataRow ((byte) 1)]
    public void Read_Normal_ReturnsExpected (byte id) {
        string json = $"{id}";

        IDType expected = id;
        IDType actual = JsonSerializer.Deserialize<IDType> (json, _options);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, true)]
    [DataRow ((byte) 0, false)]
    public void ReadAsPropertyName_Normal_ReturnsExpected (byte key, bool value) {
        Dictionary<IDType, bool> dictionary = [];
        dictionary[key] = value;
        string json = $"{{ \"{key}\": {value.ToString ().ToLower ()} }}";

        Dictionary<IDType, bool> actual = JsonSerializer.Deserialize<Dictionary<IDType, bool>> (json, _options)!;

        Assert.IsTrue (actual.ContainsKey (key));
        Assert.AreEqual (value, actual[key]);
    }

    [TestMethod]
    [DataRow ((byte) 0)]
    [DataRow ((byte) 1)]
    public void Write_Normal_ReturnsExpected (byte id) {
        string expected = $"{id}";
        string actual = JsonSerializer.Serialize<IDType> (id, _options);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, true)]
    [DataRow ((byte) 0, false)]
    public void WriteAsPropertyName_Normal_ReturnsExpected (byte key, bool value) {
        Dictionary<IDType, bool> dictionary = [];
        dictionary[key] = value;

        string expected = $"{{\"{key}\":{value.ToString ().ToLower ()}}}";
        string actual = JsonSerializer.Serialize (dictionary, _options)!;

        Assert.AreEqual (expected, actual);
    }
}
