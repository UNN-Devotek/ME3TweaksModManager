# Updater Service SSH identity

The developer Updater Service still uses password SFTP and SSH on
`ftp.me3tweaks.com:22`. All three connections now require a trusted host-key
fingerprint **before password authentication**. Existing settings without a pin
must be configured before the next upload. There is no default production pin,
automatic first-use trust, or accept-all fallback.

1. Obtain the server's **SHA256 SSH host-key fingerprint** from the service
   administrator through an independently authenticated channel. The
   administrator can calculate it from the server's public host-key file using
   `ssh-keygen -lf <public-host-key-file> -E sha256`. A live connection or
   `ssh-keyscan` output alone is not a trusted source.
2. In **Updater Service settings**, enter the value in **Trusted SSH host key**,
   together with the existing username, password and storage paths. Use
   `SHA256:` followed by the case-sensitive Base64 digest. OpenSSH's unpadded
   form and the equivalent single-`=` padded form are accepted; surrounding
   whitespace is ignored. MD5, internal whitespace and malformed encodings fail.
3. Save to validate the server identity, credentials and both storage paths.
   Only a successful validation saves the fingerprint, using the existing
   `UpdaterService` / `HostKeyFingerprint` setting. It is public identity
   material, not a password. Password encryption remains unchanged.

A missing or invalid fingerprint fails before connecting. A different server key
refuses the SSH handshake before sending the password; the settings/error area
explains the refusal. SFTP and shell connections independently check the same pin,
including a new shell connection opened after a successful SFTP connection.
The SHA256 digest covers the complete SSH public-key blob, including its key
algorithm, and applies only to the fixed hostname and port above.

For a planned key rotation, obtain and verify the replacement fingerprint through
the authenticated administrator channel, then update the setting and save again.
The server must offer the verified key for both SFTP and SSH; negotiating another
host key intentionally fails. Do not copy an unexpected presented fingerprint
into settings merely to make an error disappear. Investigate unexpected changes.

Isolated synthetic SSH/SFTP tests establish the source trust boundary. A trusted
production fingerprint and Windows updater UI/server acceptance are still release
configuration/validation requirements; no production identity is shipped here.
