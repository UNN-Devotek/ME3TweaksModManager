using System;
using ME3TweaksModManager.modmanager.helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ME3TweaksModManager.Tests
{
    [TestClass]
    public class UpdaterServiceConnectionTests
    {
        // No connection, user settings, game data or GlobalTest.Init().
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("MD5:00:11:22:33")]
        [DataRow("SHA256:AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==")]
        [DataRow("SHA256:AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB")]
        [DataRow("SHA256:AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA ")]
        [DataRow("sha256:AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public void PasswordClientsRefuseMissingOrMalformedIdentity(string fingerprint)
        {
            Assert.Throws<ArgumentException>(() => UpdaterServiceConnection.CreateSftpClient("fixture", "synthetic", fingerprint));
            Assert.Throws<ArgumentException>(() => UpdaterServiceConnection.CreateSshClient("fixture", "synthetic", fingerprint));
        }

        [TestMethod]
        [DataRow("SHA256:AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        [DataRow(" SHA256:AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA= ")]
        public void ClientsBindTheConfiguredIdentityToTheFixedUpdaterEndpoint(string fingerprint)
        {
            using var sftp = UpdaterServiceConnection.CreateSftpClient("fixture", "synthetic", fingerprint);
            using var ssh = UpdaterServiceConnection.CreateSshClient("fixture", "synthetic", fingerprint);
            Assert.AreEqual("ftp.me3tweaks.com", sftp.ConnectionInfo.Host);
            Assert.AreEqual(22, sftp.ConnectionInfo.Port);
            Assert.AreEqual("ftp.me3tweaks.com", ssh.ConnectionInfo.Host);
            Assert.AreEqual(22, ssh.ConnectionInfo.Port);
        }
    }
}
