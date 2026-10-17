namespace OpenVisionLab
{
    [System.Xml.Serialization.XmlRoot("CData")]
    public class DataState
    {
        private long savedRevision;
        private string savedContentHash = string.Empty;

        public long SavedRevision
        {
            get => savedRevision;
            set { if (value < 0) { throw new System.ArgumentOutOfRangeException(nameof(value)); } savedRevision = value; }
        }
        public string SavedContentHash
        {
            get => savedContentHash;
            set
            {
                string hash = value ?? string.Empty;
                if (hash.Length != 0 && (hash.Length != 64 || !System.Linq.Enumerable.All(hash, System.Uri.IsHexDigit))) { throw new System.ArgumentException("Saved recipe content hash must be SHA-256 hexadecimal.", nameof(value)); }
                savedContentHash = hash.ToUpperInvariant();
            }
        }
        public bool ShouldSerializeSavedRevision() => SavedRevision != 0;
        public bool ShouldSerializeSavedContentHash() => !string.IsNullOrEmpty(SavedContentHash);

        public string Description { get; set; } = string.Empty;
        public bool ShouldSerializeDescription() => !string.IsNullOrEmpty(Description);

        public DataState LoadConfig(string recipeName)
        {
            return RecipeDataStorage.Load(recipeName, this);
        }

        public void SaveConfig(string recipeName)
        {
            RecipeDataStorage.Save(recipeName, this);
        }
    }
}
