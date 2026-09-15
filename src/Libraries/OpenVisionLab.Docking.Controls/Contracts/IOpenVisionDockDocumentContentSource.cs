using System.Collections.Generic;

namespace OpenVisionLab.Docking.Controls
{
    public interface IOpenVisionDockDocumentContentSource
    {
        string SelectedDocumentId { get; }

        List<string> GetDocumentIds();

        object UpdateDocumentContent(string documentId, object currentContent);
    }
}
