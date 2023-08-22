using System.Text;

namespace WOT.TAK.Connection;

public class EventReader
{
    public static String readFrom(Stream stream)
    {
        MemoryStream bytes = new MemoryStream();
        bool end = false;
        int open_tags = 0;
        bool started = false;
        int last = -1;
        while (!end)
        {
            int b = stream.ReadByte();
            if (b == -1)
            {
                return null;
            }
            bytes.WriteByte((byte)b);
            if (b == '<')
            {
                open_tags++;
                started = true;
            }
            if (b == '>' && last == '/') open_tags--;
            if (b == '/' && last == '<')
            {
                open_tags -= 2;
                do
                {
                    b = stream.ReadByte();
                    if (b == -1)
                    {
                        return null;
                    }
                    bytes.WriteByte((byte)b);
                } while (b != '>');
            }
            if (b == '?' && last == '<')
            {
                do
                {
                    b = stream.ReadByte();
                    if (b == -1)
                    {
                        return null;
                    }
                    bytes.WriteByte((byte)b);
                } while (b != '<');
            }
            if (open_tags == 0 && started)
            {
                end = true;
            }
            last = b;
        }
        return Encoding.UTF8.GetString(bytes.ToArray()).Trim();
    }
}