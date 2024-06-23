namespace Seedysoft.Libs.TuyaDeviceControl.Exceptions;

public class TypeException : Exception
{
    public TypeException() { }

    public TypeException(string? message) : base(message) { }

    public TypeException(string? message, Exception? innerException) : base(message, innerException) { }
}
