namespace OpenVisionLab.ImageCanvas.Dialogs
{
	internal interface IImageCanvasDialogHost
	{
		string ShowOpenImageDialog(string initialDirectory);

		string ShowSaveImageDialog(string defaultFileName, string initialDirectory);
	}
}
