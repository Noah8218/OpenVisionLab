# Build and run OpenVisionLab

[English README](../../README.md) · [한국어 README](../../README.ko.md) · [Development and checks](DEVELOPMENT.md)

## Published ZIP

The published prerelease is [v2.1.0-rc.3](https://github.com/Noah8218/OpenVisionLab/releases/tag/v2.1.0-rc.3).
It predates the `v2.2.0-dev.11` source workflow shown in the README.
Download `OpenVisionLab-win-x64-framework-dependent.zip`, extract it into a
new folder, and run `OpenVisionLab.exe`. Install the
[.NET 8 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0)
if it is not installed. The ZIP is framework-dependent, not an installer.

## Build from a fresh GitHub clone

### Recommended: Visual Studio 2022 Only

Requirements:

- Windows 10 or Windows 11 x64
- Visual Studio 2022 version 17.8 or later
- `.NET desktop development` workload selected in Visual Studio Installer
- Internet access for the first NuGet package restore

The workload installs MSBuild, the WPF development tools, and a compatible
.NET SDK. The build also calls Git, so run `git --version` in your development
terminal. If the command fails, install Git for Windows with PATH support and
restart Visual Studio. Check this even if GitHub Desktop is installed.

1. Open **Visual Studio Installer**.
2. Find Visual Studio 2022, select **Modify**, select
   **.NET desktop development**, and apply the change.
3. Start Visual Studio and select **Clone a repository**.
4. Enter `https://github.com/Noah8218/OpenVisionLab.git` and choose a short
   local path such as `C:\src\OpenVisionLab` or `D:\src\OpenVisionLab`.
5. Select branch `main`, open `OpenVisionLab.sln`, set `OpenVisionLab` as the
   startup project, select `Debug` and `Any CPU`, then choose
   **Build > Build Solution**.
6. Press **F5** or select the `OpenVisionLab` start button.

If Visual Studio is already installed without the workload, use
**Tools > Get Tools and Features** and add it. Visual Studio 2022 versions
earlier than 17.8 cannot build this `net8.0` solution.

### Command-Line Alternative

Requirements:

- Windows 10 or Windows 11 x64
- Git and Windows PowerShell
- .NET SDK `8.0.100` or later in the `8.x` line, or .NET SDK `9.x`
- .NET 8 Desktop Runtime when using an SDK `9.x` command-line installation
- Internet access for the first NuGet package restore

```powershell
New-Item -ItemType Directory -Force C:\src | Out-Null
Set-Location C:\src
git clone https://github.com/Noah8218/OpenVisionLab.git
cd OpenVisionLab
powershell -NoProfile -ExecutionPolicy Bypass -File tools\VerifySourceBuild.ps1
.\bin\Debug\OpenVisionLab.exe
```

Keep the checkout path short, such as `C:\src\OpenVisionLab` or
`D:\src\OpenVisionLab`, on Windows systems where long-path support is disabled.
The WPF/MSBuild release pipeline creates long temporary file names;
`VerifyReleaseCandidate.ps1` stops early with a clear path-length error when the
checkout cannot support them.

`VerifySourceBuild.ps1` performs the locked package restore, Debug and Release
solution builds, repository readiness check, and vendored DLL check. It does
not create an installer, run the public sample suite, or modify Recipe/Layer
state.

The repository includes the manifest-verified OpenVisionLab Vision SDK 3.0
runtime under `dll\OpenVisionLab-Vision-SDK`. A separate SDK source clone is
not required to build or run OpenVisionLab. [`sdk-manifest.json`](../../dll/OpenVisionLab-Vision-SDK/sdk-manifest.json) records the
exact SDK source commit and SHA-256 of every vendored SDK DLL.

If the script reports a missing SDK, add the Visual Studio workload described
above or install a supported SDK from the
[.NET download page](https://dotnet.microsoft.com/download). The repository
accepts `8.0.100` and later `8.x` SDKs, including `8.0.300`, and the `9.x` SDK
bundled with current Visual Studio 2022. SDK 10 is not qualified for this
Visual Studio 2022 source-build path. A standalone SDK 9 installation also
needs the [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
to run this `net8.0-windows` application; the Visual Studio workload installs
that runtime for the recommended path.

### Optional Clean Windows Sandbox Check

Windows Pro, Enterprise, or Education users who have Windows Sandbox enabled
can run an additional disposable-VM check:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\WindowsSandbox\InvokeSourceBuildSandbox.ps1
```

The host script archives the current committed source, maps only a new
task-specific evidence folder into Sandbox, downloads the pinned SDK inside
Sandbox, and runs the same source-build verifier. The Sandbox is temporary and
the launcher closes the instance after writing its result under
`artifacts\source_build_sandbox_*`. `sandbox-progress.txt` shows the current
download, extraction, restore, or build phase while it runs.
Networking is required for the SDK and NuGet downloads.

Windows Sandbox is optional. GitHub Actions already runs a clean Windows
checkout and a stricter superset of this build check. Do not enable Windows
features or change virtualization settings solely to build OpenVisionLab.

## Run a source build

Use a Windows development environment with WPF/Windows Desktop build support,
the Visual Studio workload or supported SDK described above, and the runtime
DLLs included under `dll/`.

From the repository root:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
.\bin\Debug\OpenVisionLab.exe
```

This is the normal source-build output. For fresh runtime evidence, use
[`tools/BuildCleanRuntime.ps1 -Mode Dev`](../../tools/BuildCleanRuntime.ps1)
and the new output directory it reports. Keep a retained `bin/Debug` workspace
separate from clean-runtime evidence.

## Workspace reference

![OpenVisionLab main workspace](../assets/demo/readme-workflows/main-workspace.png)

1. `Pipeline`: review the ordered Steps, Layer connections and final verdict.
2. `Tools`: select a Tool to configure and add to the Pipeline.
3. `Image viewer`: compare source, intermediate and output Layers in one, two or four panels.
4. `Tool panel`: confirm the input Layer, edit parameters, ROI and acceptance limits, then run the Tool. Changing a viewer Layer does not change the Tool input.
5. `Images` / `Results`: manage inspection images and review Step measurements, execution states and OK/NG reasons.

The application project and output settings are defined in
[`src/OpenVisionLab/OpenVisionLab.csproj`](../../src/OpenVisionLab/OpenVisionLab.csproj).
SDK selection is defined in [`global.json`](../../global.json); the source-build
verifier also checks its supported SDK range and .NET 8 Desktop Runtime.
