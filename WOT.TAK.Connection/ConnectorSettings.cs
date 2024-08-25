namespace WOT.TAK.Connection;

public record ConnectorSettings
{
    public Url ServerUrl { get; init; } = Url.Empty;

    public Port ServerPort { get; init; } = Port.Default;

    public bool CertificateVerification { get; init; } = true;

    public string CertificateAuthorityPath { get; init; } = string.Empty;

    public string CertificateAuthorityPassword { get; init; } = string.Empty;

    public string ClientCertificatePath { get; init; } = string.Empty;

    public string ClientCertificatePassword { get; init; } = string.Empty;

    public string Login { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}