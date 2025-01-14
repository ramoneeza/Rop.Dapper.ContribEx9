namespace Rop.Dapper.ContribEx9;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PartialKeyAttribute : Attribute
{
    public int Order { get; set; }
    public PartialKeyAttribute(int order)
    {
        Order = order;
    }
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LeftJoinAttribute : Attribute
{
    public Type Type { get; }

    public LeftJoinAttribute(Type t)
    {
        Type = t;
    }
}
