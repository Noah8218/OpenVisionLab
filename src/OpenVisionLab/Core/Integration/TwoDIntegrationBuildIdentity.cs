#nullable enable

using System.Reflection;
using OpenVisionLab.Integration.Contracts;

namespace OpenVisionLab.Core.Integration;

internal static class TwoDIntegrationBuildIdentity
{
    private static readonly Assembly ApplicationAssembly =
        typeof(TwoDIntegrationBuildIdentity).Assembly;

    internal static IntegrationApplicationIdentity LoadQualifiedIdentity(
        string? manifestPath = null) =>
        IntegrationRuntimeBuildVerifier.LoadQualifiedIdentity(
            ApplicationAssembly,
            IntegrationApplicationIds.TwoDStudio,
            manifestPath);

    internal static IntegrationApplicationIdentity LoadQualifiedTargetIdentity(
        IntegrationApplicationIdentity expectedIdentity,
        string? manifestPath = null) =>
        IntegrationRuntimeBuildVerifier.LoadQualifiedTargetIdentity(
            ApplicationAssembly,
            expectedIdentity,
            manifestPath);
}
