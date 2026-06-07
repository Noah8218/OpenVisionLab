using System;
using System.ComponentModel;

namespace OpenVisionLab.ImageCanvas
{
	public enum EnumImageCanvasItems
	{
		[Description("Load Image")] LoadImage,
		[Description("Load Swath Image")] LoadSwathImage,
		[Description("Save Image")] SaveImage,
		[Description("OnOff Group Move")] OnOffGroupMove,
		[Description("Clear Draw Result")] ClearDrawResult,
	}

	public enum EnumLayoutItems
	{
		[Description("Apply Image")] ApplyImage,
		[Description("Delete Layout")] Delete,
		[Description("Save Image")] ExportSave,
	}

	[Serializable]
	public enum EnumInspWindowType : int
	{
		Panel = 0, // Panel
		Module = 1, // Strip
		Unit = 2,
		SubUnit = 3,
		Align = 4,
		Inspection,
		Match,
		Masking,
		Reference,
		Thickness,
	}

	public enum EnumItemType : int
	{
		Window,
		Group,
	}

	public enum EnumAutoAlignDirection : int
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
	}

	public enum EnumFillMode : int
	{
		None,
		InFill,
		OutFill
	}

}
