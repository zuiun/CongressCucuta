using System.Text.Json;
using CongressCucuta.Converters;

namespace CongressCucuta.Tests.Unit.Fakes;

internal static class FakeJsonSerializerOptions {
    public static readonly JsonSerializerOptions Options = new () {
        Converters = { new IDTypeJsonConverter () },
    };
}
