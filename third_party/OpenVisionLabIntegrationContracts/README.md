# OpenVisionLab shared integration packages

- Contracts: `OpenVisionLab.Integration.Contracts` `0.2.0-alpha.4`
- Contracts SHA-256: `F08D6A4775440F9D2C83D5ED9647A3410096FADFA87C58717C6F3B7814727BD6`
- TCP transport: `OpenVisionLab.Integration.Transport.Tcp` `0.1.0-alpha.4`
- TCP transport SHA-256: `FE768D3DC8CD6F4AAA59AB01854CAE7560259762C6D94B3DE3BF125023136AE4`
- Package nuspec repository commit:
  `f4743f3307d20a963b2197f2019713320b9859b9`

The contract package owns schema-v2 messages and validation. The TCP package
authenticates and synchronizes immutable transaction bytes between peer-local
exchange roots. Receiving bytes never acknowledges or runs an inspection.

These repository-local dependencies use the exact alpha.4 hashes above.
The previous alpha.1–alpha.3 files remain available for compatibility.
Restore from the local feed configured by this repository; another producer
checkout is not required.

The hashes above identify the bundled alpha.4 bytes checked on 2026-09-27.
The nuspec commit is package metadata, not independent proof that the producer
checkout was clean. New PCs restore from this repository-local feed; the
producer source checkout is not a build prerequisite.
