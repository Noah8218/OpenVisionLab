using System;
using OpenVisionLab.Integration.Contracts;

internal enum TwoDIntegrationConsumerAction
{
    QualityPass,
    QualityNg,
    ExecutionError,
    Cancelled
}

/// <summary>
/// Small host-side example for consuming a v2 2D result. The consumer branches
/// on the typed terminal pair and never infers quality from a Success boolean.
/// </summary>
internal static class TwoDIntegrationConsumerExample
{
    public static TwoDIntegrationConsumerAction Dispatch(IntegrationResultV2 result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return (result.Status, result.Outcome) switch
        {
            (IntegrationResultStatus.Completed, IntegrationInspectionOutcome.Pass)
                => TwoDIntegrationConsumerAction.QualityPass,
            (IntegrationResultStatus.Completed, IntegrationInspectionOutcome.Ng)
                => TwoDIntegrationConsumerAction.QualityNg,
            (IntegrationResultStatus.Failed, IntegrationInspectionOutcome.ExecutionError)
                => TwoDIntegrationConsumerAction.ExecutionError,
            (IntegrationResultStatus.Cancelled, IntegrationInspectionOutcome.Indeterminate)
                => TwoDIntegrationConsumerAction.Cancelled,
            _ => throw new IntegrationContractException(
                IntegrationErrorCode.InvalidState,
                $"Unsupported v2 result terminal pair: {result.Status}/{result.Outcome}.")
        };
    }

    public static void AssertCorrelation(
        IntegrationHandoffV2 handoff,
        IntegrationAcknowledgementV2 acknowledgement,
        IntegrationResultV2 result)
    {
        ArgumentNullException.ThrowIfNull(handoff);
        ArgumentNullException.ThrowIfNull(acknowledgement);
        ArgumentNullException.ThrowIfNull(result);

        if (acknowledgement.Status != IntegrationAcknowledgementStatus.Accepted)
        {
            throw new InvalidOperationException(
                "The consumer cannot handle a Result without an accepted acknowledgement.");
        }

        if (result.TransactionId != handoff.TransactionId
            || result.HandoffMessageId != handoff.MessageId
            || acknowledgement.TransactionId != handoff.TransactionId
            || acknowledgement.HandoffMessageId != handoff.MessageId)
        {
            throw new InvalidOperationException(
                "The v2 transaction, Handoff, or Acknowledgement identity does not match.");
        }

        IntegrationRunCorrelation expected =
            IntegrationRunCorrelation.FromContext(handoff.Context);
        if (result.Correlation != expected)
        {
            throw new InvalidOperationException(
                "The v2 Result correlation does not match the Handoff context.");
        }

        if (result.Producer != result.Correlation.ConsumerBuild)
        {
            throw new InvalidOperationException(
                "The v2 Result producer does not match the consumer build in its correlation.");
        }

        if (result.Status == IntegrationResultStatus.Completed
            && string.IsNullOrWhiteSpace(result.RunId))
        {
            throw new InvalidOperationException(
                "A completed v2 Result must retain its RunId for consumer correlation.");
        }
    }
}
