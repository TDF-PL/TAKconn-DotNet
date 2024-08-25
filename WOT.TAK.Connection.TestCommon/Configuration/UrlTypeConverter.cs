using System.ComponentModel;
using System.Globalization;

namespace WOT.TAK.Connection.TestCommon.Configuration;

public class UrlTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        => value is string str ? new Url(str) : base.ConvertFrom(context, culture, value);
}

public class PortTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string)
        || sourceType == typeof(int)
        || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        => value switch
        {
            int i => new Port(i),
            string str => new Port(int.Parse(str, culture)),
            _ => base.ConvertFrom(context, culture, value)
        };
}