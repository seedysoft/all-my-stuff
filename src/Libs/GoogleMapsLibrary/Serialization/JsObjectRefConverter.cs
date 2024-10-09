//using GoogleMapsLibrary.Interfaces;
//using System.Text.Json;

//namespace GoogleMapsLibrary.Serialization;

//internal class JsObjectRefConverter<T> : JsonConverter<T>
//    where T : IJsObjectRef
//{
//    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

//    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
//    {
//        writer.WriteStartObject();

//        using JsonDocument doc = JsonSerializer.SerializeToDocument(new JsObjectRef1(value.Guid), typeof(JsObjectRef1), options);

//        foreach (JsonProperty prop in doc.RootElement.EnumerateObject())
//            prop.WriteTo(writer);

//        writer.WriteEndObject();
//    }
//}
