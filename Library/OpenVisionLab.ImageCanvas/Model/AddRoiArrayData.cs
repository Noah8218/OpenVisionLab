namespace OpenVisionLab.ImageCanvas
{
	public class AddRoiArrayData
	{
		private int _rows;
		private int _columns;
		private float _rowSpacing;
		private float _columnSpacing;

		public int Rows
		{
			get { return _rows; }
			set
			{
				_rows = value;
			}
		}

		public int Columns
		{
			get { return _columns; }
			set
			{
				_columns = value;
			}
		}

		public float RowSpacing
		{
			get { return _rowSpacing; }
			set
			{
				_rowSpacing = value;
			}
		}

		public float ColumnSpacing
		{
			get { return _columnSpacing; }
			set
			{
				_columnSpacing = value;
			}
		}
	}
}
