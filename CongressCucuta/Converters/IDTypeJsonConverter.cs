using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using CongressCucuta.Core;

namespace CongressCucuta.Converters;

public class IDTypeJsonConverter : JsonConverter<IDType> {
    public override IDType Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        byte id = reader.GetByte ();

        return new (id);
    }

    public override IDType ReadAsPropertyName (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        string property = reader.GetString ()!;
        byte id = byte.Parse (property);

        return new (id);
    }

    public override void Write (Utf8JsonWriter writer, IDType value, JsonSerializerOptions options) {
        writer.WriteNumberValue (value.ID);
    }

    public override void WriteAsPropertyName (Utf8JsonWriter writer, [DisallowNull] IDType value, JsonSerializerOptions options) {
        writer.WritePropertyName (value.ID.ToString ());
    }
}
