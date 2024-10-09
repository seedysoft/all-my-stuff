//using GoogleMapsLibrary.Interfaces;

//namespace GoogleMapsLibrary;

//internal class JsObjectRef1 : IJsObjectRef
//{
//    public Guid Guid { get; private set; }
//    public string GuidString => Guid.ToString();

//    public JsObjectRef1(Guid guid) => Guid = guid;
    
//    [JsonConstructor]
//    public JsObjectRef1(string guidString) => Guid = new Guid(guidString);

//    public override bool Equals(object? obj) => obj is JsObjectRef other && other.Guid == Guid;

//    public override int GetHashCode() => Guid.GetHashCode();
//}
