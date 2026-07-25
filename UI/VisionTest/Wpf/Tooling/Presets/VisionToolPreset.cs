using System;

namespace OpenVisionLab
{
    internal sealed class VisionToolPreset<TProperty>
    {
        private readonly Action<TProperty> apply;

        public VisionToolPreset(
            string id,
            string displayNameKey,
            string fallbackDisplayName,
            string descriptionKey,
            string fallbackDescription,
            Action<TProperty> apply)
        {
            Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("Preset id is required.", nameof(id)) : id;
            DisplayNameKey = displayNameKey ?? string.Empty;
            FallbackDisplayName = fallbackDisplayName ?? id;
            DescriptionKey = descriptionKey ?? string.Empty;
            FallbackDescription = fallbackDescription ?? string.Empty;
            this.apply = apply ?? throw new ArgumentNullException(nameof(apply));
        }

        public string Id { get; }

        public string DisplayNameKey { get; }

        public string FallbackDisplayName { get; }

        public string DescriptionKey { get; }

        public string FallbackDescription { get; }

        public string DisplayName => VisionToolVerificationText.T(DisplayNameKey, FallbackDisplayName);

        public string Description => VisionToolVerificationText.T(DescriptionKey, FallbackDescription);

        public void ApplyTo(TProperty property)
        {
            if (property == null)
            {
                return;
            }

            apply(property);
        }
    }
}
