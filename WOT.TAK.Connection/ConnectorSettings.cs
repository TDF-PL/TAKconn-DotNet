namespace WOT.TAK.Connection;

public record ConnectorSettings
{
    public Url ServerUrl { get; init; } = Url.Empty;

    public Port ServerPort { get; init; } = Port.Default;

    public bool CertificateVerification { get; init; } = true;

    public string TrustStorePath { get; init; } = string.Empty;

    public string TrustStorePassword { get; init; } = string.Empty;

    public string KeyStorePath { get; init; } = string.Empty;

    public string KeyStorePassword { get; init; } = string.Empty;

    public string Login { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}