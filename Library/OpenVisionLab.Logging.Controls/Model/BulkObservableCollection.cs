using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenVisionLab.Logging.Controls.Model
{
    public class BulkObservableCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }
    }
}
