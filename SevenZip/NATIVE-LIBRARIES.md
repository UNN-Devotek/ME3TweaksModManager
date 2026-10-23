# Bundled 7-Zip libraries

These libraries come from the unmodified full 7-Zip 26.05 Windows installers,
not the reduced-format Extra package. Keep the full codec set for mod imports,
NSIS executable inspection, compression and updater archive extraction.

Vendor release: https://github.com/ip7z/7zip/releases/tag/26.05
Security history: https://www.7-zip.org/history.txt
Source: https://github.com/ip7z/7zip/releases/download/26.05/7z2605-src.tar.xz

| Architecture | Installer SHA-256 | Extracted `7z.dll` SHA-256 |
| --- | --- | --- |
| x64 | `fd99e86f57d8f429ef5ed045bf98315af962e2309b0f1ba87a147a44c71647d0` | `78bf6b668d75801e3aa180b4ccb16e6d8395fba80dec8a065055bb6f8dfabb82` |
| x86 | `6435a470fe7c4769962dbed8a60c641ee21401c833ced06077c80e33e7845a5e` | `1c57100cc237db626259f68fe91c5a632a5128e0e01bab1452baa1104ff84311` |

`SevenZip/7z.dll` is x86. All `7z64.dll` copies and the legacy
`MassEffectModManagerCore/staticfiles/7z.dll` copy are x64. The application ships
`MassEffectModManagerCore/modmanager/native/7z64.dll`; test and developer tool
copies must use the same decoder. No active project embeds the legacy staticfiles
DLL, but it is refreshed to avoid reintroducing the old copy.

The vendor distribution license is preserved in
`MassEffectModManagerCore/modmanager/licenses/7-Zip.txt` and embedded in the app.
The replacement closes known decoder vulnerabilities including CVE-2024-11477
and CVE-2026-14266. Rebuild and validate on Windows before distribution; source
changes do not repair already distributed applications.
