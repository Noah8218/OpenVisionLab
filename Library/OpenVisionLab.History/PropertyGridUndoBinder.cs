using System;
using OpenVisionLab.PropertyGrid;

namespace OpenVisionLab.History
{
    public sealed class PropertyGridUndoBinder : IDisposable
    {
        private readonly IPropertyGridView propertyGrid;
        private readonly UndoRedoManager history;
        private readonly Action afterApply;
        private bool disposed;

        public PropertyGridUndoBinder(IPropertyGridView propertyGrid, UndoRedoManager history, Action afterApply)
        {
            this.propertyGrid = propertyGrid ?? throw new ArgumentNullException(nameof(propertyGrid));
            this.history = history ?? throw new ArgumentNullException(nameof(history));
            this.afterApply = afterApply;

            propertyGrid.PropertyValueChanged += OnPropertyValueChanged;
            propertyGrid.SelectedObjectsChanged += OnSelectedObjectsChanged;
        }

        public void Clear()
        {
            history.Clear();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            propertyGrid.PropertyValueChanged -= OnPropertyValueChanged;
            propertyGrid.SelectedObjectsChanged -= OnSelectedObjectsChanged;
        }

        private void OnPropertyValueChanged(object sender, PropertyGridPropertyValueChangedEventArgs e)
        {
            if (history.IsReplaying || e == null)
            {
                return;
            }

            if (!PropertyChangeCommand.CanCreate(e.TargetObject, e.PropertyName, e.OldValue, e.NewValue))
            {
                return;
            }

            history.Execute(new PropertyChangeCommand(e.TargetObject, e.PropertyName, e.OldValue, e.NewValue, afterApply));
        }

        private void OnSelectedObjectsChanged(object sender, EventArgs e)
        {
            if (!history.IsReplaying)
            {
                history.Clear();
            }
        }
    }
}
