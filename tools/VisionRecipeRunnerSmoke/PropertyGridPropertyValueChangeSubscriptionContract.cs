using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class PropertyGridPropertyValueChangeSubscriptionContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl20_property_grid_value_change_subscription_contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        Directory.CreateDirectory(evidenceDirectory);

        string adapterPath = Path.Combine(
            repositoryRoot,
            "src",
            "Libraries",
            "WpfPropertyGridBridge",
            "WpfPropertyGridAdapter.cs");
        string subscriptionPath = Path.Combine(
            repositoryRoot,
            "src",
            "Libraries",
            "WpfPropertyGridBridge",
            "PropertyGridPropertyValueChangeSubscription.cs");
        string adapter = File.ReadAllText(adapterPath);
        string subscription = File.ReadAllText(subscriptionPath);

        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        Check(
            "subscription owner is a concrete internal type",
            subscription.Contains(
                "internal sealed class PropertyGridPropertyValueChangeSubscription",
                StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "subscription owner keeps value, notification and snapshot registries",
            subscription.Contains("valueChangedHandlers", StringComparison.Ordinal)
                && subscription.Contains("propertyChangedHandlers", StringComparison.Ordinal)
                && subscription.Contains("lastValues", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "attach owns both source event subscriptions",
            subscription.Contains("propertyItem.ValueChanged += valueChangedHandler", StringComparison.Ordinal)
                && subscription.Contains("notifyPropertyChanged.PropertyChanged += propertyChangedHandler", StringComparison.Ordinal)
                && subscription.Contains("lastValues[propertyItem] = ReadPropertyItemValue(propertyItem)", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "detach unsubscribes and clears every owned registry",
            subscription.Contains("notifyPropertyChanged.PropertyChanged -= handler.Value", StringComparison.Ordinal)
                && subscription.Contains("handler.Key.ValueChanged -= handler.Value", StringComparison.Ordinal)
                && subscription.Contains("propertyChangedHandlers.Clear()", StringComparison.Ordinal)
                && subscription.Contains("valueChangedHandlers.Clear()", StringComparison.Ordinal)
                && subscription.Contains("lastValues.Clear()", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "property notification keeps the existing equality guard and old/new values",
            subscription.Contains("string.Equals(e.PropertyName, \"PropertyValue\"", StringComparison.Ordinal)
                && subscription.Contains("object.Equals(oldValue, newValue)", StringComparison.Ordinal)
                && subscription.Contains("propertyValueChanged(propertyItem, oldValue, newValue)", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "adapter composes the subscription with its existing event projection",
            adapter.Contains(
                "private readonly PropertyGridPropertyValueChangeSubscription propertyValueChangeSubscription",
                StringComparison.Ordinal)
                && adapter.Contains(
                    "new PropertyGridPropertyValueChangeSubscription(",
                    StringComparison.Ordinal)
                && adapter.Contains(
                    "(property, oldValue, newValue) => RaisePropertyValueChanged(property, oldValue, newValue)",
                    StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "selected-object replacement detaches before attaching new property items",
            Count(adapter, "propertyValueChangeSubscription.Detach();") == 2
                && Count(adapter, "propertyValueChangeSubscription.Attach(innerPropertyGrid.Properties);") == 2,
            passed,
            failed);
        Check(
            "adapter no longer owns the old handler registries or registration methods",
            !adapter.Contains("propertyValueChangedHandlers", StringComparison.Ordinal)
                && !adapter.Contains("propertyItemPropertyChangedHandlers", StringComparison.Ordinal)
                && !adapter.Contains("propertyItemLastValues", StringComparison.Ordinal)
                && !adapter.Contains("RegisterPropertyValueChangedHandlers", StringComparison.Ordinal)
                && !adapter.Contains("UnregisterPropertyValueChangedHandlers", StringComparison.Ordinal),
            passed,
            failed);

        string outputPath = Path.Combine(
            evidenceDirectory,
            "property-grid-value-change-subscription-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-20 PropertyGrid property-value change subscription owner",
                "RepositoryRoot: " + repositoryRoot,
                "EvidenceDirectory: " + evidenceDirectory
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in passed)
        {
            Console.WriteLine("PASS|" + item);
        }

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(
            "CONTRACT|property-grid-value-change-subscription|passed="
            + passed.Count
            + "|failed="
            + failed.Count);
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static string ResolveRepositoryRoot()
    {
        foreach (string start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            DirectoryInfo? current = new DirectoryInfo(Path.GetFullPath(start));
            while (current != null)
            {
                string adapterPath = Path.Combine(
                    current.FullName,
                    "src",
                    "Libraries",
                    "WpfPropertyGridBridge",
                    "WpfPropertyGridAdapter.cs");
                if (File.Exists(adapterPath))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new DirectoryNotFoundException("OpenVisionLab repository root could not be located.");
    }

    private static int Count(string text, string value)
    {
        int count = 0;
        int index = 0;
        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }

    private static void Check(
        string name,
        bool condition,
        List<string> passed,
        List<string> failed)
    {
        if (condition)
        {
            passed.Add(name);
        }
        else
        {
            failed.Add(name);
        }
    }
}
