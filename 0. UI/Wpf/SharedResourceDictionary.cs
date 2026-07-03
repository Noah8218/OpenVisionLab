using System;
using System.Collections.Generic;
using System.Windows;

namespace OpenVisionLab
{
    public sealed class SharedResourceDictionary : ResourceDictionary
    {
        private static readonly object SyncRoot = new object();
        private static readonly Dictionary<Uri, ResourceDictionary> SharedDictionaries = new Dictionary<Uri, ResourceDictionary>();
        private Uri source;

        public new Uri Source
        {
            get => source;
            set
            {
                source = value;
                if (value == null)
                {
                    return;
                }

                ResourceDictionary sharedDictionary;
                lock (SyncRoot)
                {
                    if (SharedDictionaries.TryGetValue(value, out sharedDictionary))
                    {
                        MergedDictionaries.Add(sharedDictionary);
                        return;
                    }

                    // Tool windows repeatedly load the same theme dictionaries; cache the first parsed dictionary
                    // so subsequent tool views keep standalone XAML resources without paying the parse cost again.
                    base.Source = value;
                    SharedDictionaries[value] = this;
                }
            }
        }
    }
}
