namespace WOT.TAK.Connection;

public record Url(string Value)
{
    public static Url Empty => new(string.Empty);

    public string AsString()
    {
        return this.Value;
    }

    public override string ToString()
    {
        return this.AsString();
    }

    public static Url Of(string value)
    {
        return new Url(value);
    }
}