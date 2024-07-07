using System.Text;

namespace WOT.TAK.Connection;

internal static class EventReader
{
    public static async Task<string?> ReadFromAsync(Stream stream, CancellationToken cancellationToken)
    {
        var bytes = new MemoryStream();
        var end = false;
        var openTags = 0;
        var started = false;
        var last = -1;

        while (!end)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var b = await stream.ReadByteAsync(cancellationToken).ConfigureAwait(false);
            if (b == -1)
            {
                return null;
            }

            await bytes.WriteByteAsync((byte)b, cancellationToken).ConfigureAwait(false);

            switch (b)
            {
                case '<':
                    openTags++;
                    started = true;
                    break;
                case '>' when last == '/':
                    openTags--;
                    break;
                case '/' when last == '<':
                    openTags -= 2;
                    do
                    {
                        b = await stream.ReadByteAsync(cancellationToken).ConfigureAwait(false);
                        if (b == -1)
                        {
                            return null;
                        }

                        await bytes.WriteByteAsync((byte)b, cancellationToken).ConfigureAwait(false);
                    }
                    while (b != '>');
                    break;
            }

            if (b == '?' && last == '<')
            {
                do
                {
                    b = await stream.ReadByteAsync(cancellationToken).ConfigureAwait(false);
                    if (b == -1)
                    {
                        return null;
                    }

                    await bytes.WriteByteAsync((byte)b, cancellationToken).ConfigureAwait(false);
                }
                while (b != '<');
            }

            if (openTags == 0 && started)
            {
                end = true;
            }

            last = b;
        }

        return Encoding.UTF8.GetString(bytes.ToArray()).Trim();
    }

    private static async Task<int> ReadByteAsync(this Stream stream, CancellationToken cancellationToken)
    {
        var buffer = new byte[1];
        var bytesRead = await stream.ReadAsync(buffer, 0, 1, cancellationToken).ConfigureAwait(false);
        return bytesRead == 0 ? -1 : buffer[0];
    }

    private static async Task WriteByteAsync(this MemoryStream stream, byte value, CancellationToken cancellationToken)
    {
        var buffer = new[] { value };
        await stream.WriteAsync(buffer, 0, 1, cancellationToken).ConfigureAwait(false);
    }
}