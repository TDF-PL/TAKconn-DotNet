namespace WOT.TAK.Connection;

public record Uid(Guid Id)
{
    public static Uid Random()
    {
        return new Uid(Guid.NewGuid());
    }

    public string AsString()
    {
        return this.Id.ToString();
    }

    public override string ToString()
    {
        return this.AsString();
    }
}