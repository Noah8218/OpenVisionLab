# OpenVisionLab shared integration packages

- Contracts: `OpenVisionLab.Integration.Contracts` `0.2.0-alpha.3`
- Contracts SHA-256: `25CADF8BD6EDBC7E9C089BE6CE2286A7ADA5A335A3DEA5FBDBCCEF63343E4A24`
- TCP transport: `OpenVisionLab.Integration.Transport.Tcp` `0.1.0-alpha.3`
- TCP transport SHA-256: `5FBFE95358554D47A047305D589614832EAF775A05100B2B8E626D8DDDEC424F`
- Package source state: `clean` at Shared commit
  `f4743f3307d20a963b2197f2019713320b9859b9`

The contract package owns schema-v2 messages and validation. The TCP package
authenticates and synchronizes immutable transaction bytes between peer-local
exchange roots. Receiving bytes never acknowledges or runs an inspection.

These are fixed DEV package candidates copied from
`C:\Git\Shared\OpenVisionLab-Integration-Contracts`; they are not published
or released packages. The previous alpha.2 bytes remain beside these files and
are not overwritten. Every participating DEV consumer must use these exact
alpha.3 hashes.

This alpha.3 pair is a local DEV clean-source qualification candidate. Its
nuspecs are bound to the committed Shared source above.
