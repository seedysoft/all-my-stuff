namespace Seedysoft.Libs.Core.Entities;

public abstract record MasterFilesBase(string Value)
{
    public static implicit operator string?(MasterFilesBase t) => t?.Value;

    public override string ToString() => Value;
}
