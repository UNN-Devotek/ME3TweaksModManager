using System;
using System.Security.Cryptography;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace ME3TweaksModManager.modmanager.helpers
{
    /// <summary>
    /// Password clients for the updater service must authenticate its host key first.
    /// The operator supplies an independently verified SHA256 fingerprint, never TOFU.
    /// </summary>
    internal static class UpdaterServiceConnection
    {
        internal const string Host = "ftp.me3tweaks.com";
        internal const int Port = 22;

        internal static SftpClient CreateSftpClient(string username, string password, string fingerprint)
        {
            var expectedKeyHash = ParseFingerprint(fingerprint);
            var client = new SftpClient(Host, Port, username, password);
            ValidateHostKey(client, expectedKeyHash);
            return client;
        }

        internal static SshClient CreateSshClient(string username, string password, string fingerprint)
        {
            var expectedKeyHash = ParseFingerprint(fingerprint);
            var client = new SshClient(Host, Port, username, password);
            ValidateHostKey(client, expectedKeyHash);
            return client;
        }

        private static byte[] ParseFingerprint(string fingerprint)
        {
            const string prefix = "SHA256:";
            var value = fingerprint?.Trim();
            if (value != null && value.StartsWith(prefix, StringComparison.Ordinal))
            {
                var encodedHash = value.Substring(prefix.Length);
                // Accept OpenSSH's unpadded representation and its canonical padded equivalent.
                // Reject internal whitespace, alternate encodings, MD5 and case-folded Base64.
                if (encodedHash.Length == 43 || (encodedHash.Length == 44 && encodedHash.EndsWith("=", StringComparison.Ordinal)))
                {
                    Span<byte> hash = stackalloc byte[32];
                    var paddedHash = encodedHash.Length == 43 ? encodedHash + "=" : encodedHash;
                    if (Convert.TryFromBase64String(paddedHash, hash, out var written) && written == 32 &&
                        string.Equals(Convert.ToBase64String(hash), paddedHash, StringComparison.Ordinal))
                        return hash.ToArray();
                }
            }

            throw new ArgumentException("A trusted SHA256 SSH host-key fingerprint is required for " + Host +
                ":22. Obtain it from the service administrator through an authenticated channel and enter it in Updater Service settings (SHA256: followed by Base64).", nameof(fingerprint));
        }

        private static void ValidateHostKey(BaseClient client, byte[] expectedKeyHash)
        {
            client.HostKeyReceived += (_, e) =>
            {
                e.CanTrust = false;
                if (CryptographicOperations.FixedTimeEquals(expectedKeyHash, SHA256.HashData(e.HostKey)))
                {
                    e.CanTrust = true;
                    return;
                }

                throw new SshConnectionException("The SSH host key for " + Host +
                    ":22 does not match the trusted SHA256 fingerprint. Connection refused before password authentication. Verify any key rotation with the service administrator through an authenticated channel before changing the saved fingerprint.");
            };
        }
    }
}
