using System.Globalization;

namespace WOT.TAK.Connection;

public record Port(int Value)
{
    public static Port Default => new(8089);

    public int AsInt()
    {
        return this.Value;
    }

    public override string ToString()
    {
        return this.AsInt().ToString(CultureInfo.InvariantCulture);
    }

    public static Port Of(int value)
    {
        return new Port(value);
    }
}