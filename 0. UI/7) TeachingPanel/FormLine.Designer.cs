using Cyotek.Windows.Forms;

namespace KtemVisionSystem
{
    partial class FormLine
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLine));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdoEdgeR = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoEdgeL = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rjLabel6 = new RJCodeUI_M1.RJControls.RJLabel();
            this.dgvLine_R = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.btnIntersectionTest = new RJCodeUI_M1.RJControls.RJButton();
            this.btnLineTest = new RJCodeUI_M1.RJControls.RJButton();
            this.rjLabel16 = new RJCodeUI_M1.RJControls.RJLabel();
            this.dgvLine_L = new RJCodeUI_M1.RJControls.RJDataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLine_R)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLine_L)).BeginInit();
            this.SuspendLayout();
            // 
            // timePixelData
            // 
            this.timePixelData.Enabled = true;
            this.timePixelData.Interval = 10;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.panel1.Controls.Add(this.rdoEdgeR);
            this.panel1.Controls.Add(this.rdoEdgeL);
            this.panel1.Controls.Add(this.rjLabel6);
            this.panel1.Controls.Add(this.dgvLine_R);
            this.panel1.Controls.Add(this.btnIntersectionTest);
            this.panel1.Controls.Add(this.btnLineTest);
            this.panel1.Controls.Add(this.rjLabel16);
            this.panel1.Controls.Add(this.dgvLine_L);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(612, 531);
            this.panel1.TabIndex = 1952;
            // 
            // rdoEdgeR
            // 
            this.rdoEdgeR.AutoSize = true;
            this.rdoEdgeR.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoEdgeR.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoEdgeR.Customizable = true;
            this.rdoEdgeR.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoEdgeR.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoEdgeR.Location = new System.Drawing.Point(521, 28);
            this.rdoEdgeR.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoEdgeR.Name = "rdoEdgeR";
            this.rdoEdgeR.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoEdgeR.Size = new System.Drawing.Size(87, 21);
            this.rdoEdgeR.TabIndex = 2153;
            this.rdoEdgeR.Text = "Edge(R)";
            this.rdoEdgeR.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoEdgeR.UseVisualStyleBackColor = true;
            // 
            // rdoEdgeL
            // 
            this.rdoEdgeL.AutoSize = true;
            this.rdoEdgeL.Checked = true;
            this.rdoEdgeL.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoEdgeL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoEdgeL.Customizable = true;
            this.rdoEdgeL.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoEdgeL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoEdgeL.Location = new System.Drawing.Point(435, 28);
            this.rdoEdgeL.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoEdgeL.Name = "rdoEdgeL";
            this.rdoEdgeL.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoEdgeL.Size = new System.Drawing.Size(86, 21);
            this.rdoEdgeL.TabIndex = 2152;
            this.rdoEdgeL.TabStop = true;
            this.rdoEdgeL.Text = "Edge(L)";
            this.rdoEdgeL.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoEdgeL.UseVisualStyleBackColor = true;
            // 
            // rjLabel6
            // 
            this.rjLabel6.AutoSize = true;
            this.rjLabel6.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel6.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel6.LinkLabel = false;
            this.rjLabel6.Location = new System.Drawing.Point(178, 6);
            this.rjLabel6.Name = "rjLabel6";
            this.rjLabel6.Size = new System.Drawing.Size(68, 18);
            this.rjLabel6.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel6.TabIndex = 2146;
            this.rjLabel6.Text = "Edge(R)";
            // 
            // dgvLine_R
            // 
            this.dgvLine_R.AllowUserToResizeRows = false;
            this.dgvLine_R.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.dgvLine_R.AlternatingRowsColorApply = false;
            this.dgvLine_R.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLine_R.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvLine_R.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvLine_R.BorderRadius = 13;
            this.dgvLine_R.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLine_R.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLine_R.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.dgvLine_R.ColumnHeaderFont = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvLine_R.ColumnHeaderHeight = 40;
            this.dgvLine_R.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLine_R.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLine_R.ColumnHeadersHeight = 40;
            this.dgvLine_R.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLine_R.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.dgvLine_R.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLine_R.Customizable = false;
            this.dgvLine_R.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvLine_R.EnableHeadersVisualStyles = false;
            this.dgvLine_R.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvLine_R.Location = new System.Drawing.Point(181, 27);
            this.dgvLine_R.Name = "dgvLine_R";
            this.dgvLine_R.ReadOnly = true;
            this.dgvLine_R.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvLine_R.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.dgvLine_R.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLine_R.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLine_R.RowHeadersVisible = false;
            this.dgvLine_R.RowHeadersWidth = 30;
            this.dgvLine_R.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvLine_R.RowHeight = 40;
            this.dgvLine_R.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gray;
            this.dgvLine_R.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvLine_R.RowsTextColor = System.Drawing.Color.Gray;
            this.dgvLine_R.RowTemplate.Height = 40;
            this.dgvLine_R.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.dgvLine_R.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLine_R.SelectionTextColor = System.Drawing.Color.Gray;
            this.dgvLine_R.Size = new System.Drawing.Size(190, 180);
            this.dgvLine_R.TabIndex = 2150;
            this.dgvLine_R.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLineL_CellClick);
            // 
            // btnIntersectionTest
            // 
            this.btnIntersectionTest.BackColor = System.Drawing.Color.White;
            this.btnIntersectionTest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnIntersectionTest.BorderRadius = 15;
            this.btnIntersectionTest.BorderSize = 3;
            this.btnIntersectionTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIntersectionTest.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnIntersectionTest.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnIntersectionTest.FlatAppearance.BorderSize = 3;
            this.btnIntersectionTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnIntersectionTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnIntersectionTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIntersectionTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIntersectionTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnIntersectionTest.IconChar = FontAwesome.Sharp.IconChar.CheckSquare;
            this.btnIntersectionTest.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnIntersectionTest.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnIntersectionTest.IconSize = 80;
            this.btnIntersectionTest.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnIntersectionTest.Location = new System.Drawing.Point(450, 167);
            this.btnIntersectionTest.Name = "btnIntersectionTest";
            this.btnIntersectionTest.Size = new System.Drawing.Size(150, 112);
            this.btnIntersectionTest.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnIntersectionTest.TabIndex = 2148;
            this.btnIntersectionTest.Text = "교점 검사";
            this.btnIntersectionTest.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnIntersectionTest.UseVisualStyleBackColor = false;
            this.btnIntersectionTest.Click += new System.EventHandler(this.btnIntersectionTest_Click);
            // 
            // btnLineTest
            // 
            this.btnLineTest.BackColor = System.Drawing.Color.White;
            this.btnLineTest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLineTest.BorderRadius = 15;
            this.btnLineTest.BorderSize = 3;
            this.btnLineTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLineTest.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnLineTest.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnLineTest.FlatAppearance.BorderSize = 3;
            this.btnLineTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnLineTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnLineTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLineTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLineTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLineTest.IconChar = FontAwesome.Sharp.IconChar.CheckSquare;
            this.btnLineTest.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLineTest.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLineTest.IconSize = 80;
            this.btnLineTest.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnLineTest.Location = new System.Drawing.Point(450, 55);
            this.btnLineTest.Name = "btnLineTest";
            this.btnLineTest.Size = new System.Drawing.Size(150, 112);
            this.btnLineTest.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnLineTest.TabIndex = 2147;
            this.btnLineTest.Text = "라인 검사";
            this.btnLineTest.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnLineTest.UseVisualStyleBackColor = false;
            this.btnLineTest.Click += new System.EventHandler(this.btnLineTest_Click);
            // 
            // rjLabel16
            // 
            this.rjLabel16.AutoSize = true;
            this.rjLabel16.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel16.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel16.LinkLabel = false;
            this.rjLabel16.Location = new System.Drawing.Point(2, 6);
            this.rjLabel16.Name = "rjLabel16";
            this.rjLabel16.Size = new System.Drawing.Size(66, 18);
            this.rjLabel16.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel16.TabIndex = 2151;
            this.rjLabel16.Text = "Edge(L)";
            // 
            // dgvLine_L
            // 
            this.dgvLine_L.AllowUserToResizeRows = false;
            this.dgvLine_L.AlternatingRowsColor = System.Drawing.Color.Empty;
            this.dgvLine_L.AlternatingRowsColorApply = false;
            this.dgvLine_L.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLine_L.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvLine_L.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvLine_L.BorderRadius = 13;
            this.dgvLine_L.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLine_L.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLine_L.ColumnHeaderColor = System.Drawing.Color.MediumPurple;
            this.dgvLine_L.ColumnHeaderFont = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvLine_L.ColumnHeaderHeight = 40;
            this.dgvLine_L.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLine_L.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvLine_L.ColumnHeadersHeight = 40;
            this.dgvLine_L.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLine_L.ColumnHeaderTextColor = System.Drawing.Color.White;
            this.dgvLine_L.ColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLine_L.Customizable = false;
            this.dgvLine_L.DgvBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.dgvLine_L.EnableHeadersVisualStyles = false;
            this.dgvLine_L.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvLine_L.Location = new System.Drawing.Point(2, 27);
            this.dgvLine_L.Name = "dgvLine_L";
            this.dgvLine_L.ReadOnly = true;
            this.dgvLine_L.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvLine_L.RowHeaderColor = System.Drawing.Color.WhiteSmoke;
            this.dgvLine_L.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLine_L.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvLine_L.RowHeadersVisible = false;
            this.dgvLine_L.RowHeadersWidth = 30;
            this.dgvLine_L.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvLine_L.RowHeight = 40;
            this.dgvLine_L.RowsColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Gray;
            this.dgvLine_L.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvLine_L.RowsTextColor = System.Drawing.Color.Gray;
            this.dgvLine_L.RowTemplate.Height = 40;
            this.dgvLine_L.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(199)))), ((int)(((byte)(241)))));
            this.dgvLine_L.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLine_L.SelectionTextColor = System.Drawing.Color.Gray;
            this.dgvLine_L.Size = new System.Drawing.Size(190, 180);
            this.dgvLine_L.TabIndex = 2149;
            this.dgvLine_L.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLineL_CellClick);
            // 
            // FormLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(612, 531);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormLine";
            this.Text = "Line";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLine_R)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLine_L)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private System.Windows.Forms.Panel panel1;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoEdgeR;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoEdgeL;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel6;
        private RJCodeUI_M1.RJControls.RJDataGridView dgvLine_R;
        private RJCodeUI_M1.RJControls.RJButton btnIntersectionTest;
        private RJCodeUI_M1.RJControls.RJButton btnLineTest;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel16;
        private RJCodeUI_M1.RJControls.RJDataGridView dgvLine_L;
    }
}