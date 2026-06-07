namespace OpenVisionLab
{
    partial class FormVision_Morphology
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
                        this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoGradient = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoHitMiss = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoBlackHat = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoTopHat = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoCross = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoErode = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoOpen = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoDilate = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbMorpH = new RJCodeUI_M1.RJControls.RJTextBox();
            this.rdoShapesCross = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoShapesRect = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoShapesEllipse = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.tbMorpW = new RJCodeUI_M1.RJControls.RJTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ibSource = new OpenVisionLab.VisionTestImageCanvas();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbLayerList2 = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnNewPanel_Desty = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.ibDestination = new OpenVisionLab.VisionTestImageCanvas();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnMorpRun = new RJCodeUI_M1.RJControls.RJButton();
            this.pnlClientArea.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.btnMorpRun);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(918, 613);
            // 
            // 
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox1.Controls.Add(this.rdoGradient);
            this.groupBox1.Controls.Add(this.rdoHitMiss);
            this.groupBox1.Controls.Add(this.rdoBlackHat);
            this.groupBox1.Controls.Add(this.rdoTopHat);
            this.groupBox1.Controls.Add(this.rdoCross);
            this.groupBox1.Controls.Add(this.rdoErode);
            this.groupBox1.Controls.Add(this.rdoOpen);
            this.groupBox1.Controls.Add(this.rdoDilate);
            this.groupBox1.Location = new System.Drawing.Point(424, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(488, 150);
            this.groupBox1.TabIndex = 2147;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operations";
            // 
            // rdoGradient
            // 
            this.rdoGradient.AutoSize = true;
            this.rdoGradient.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoGradient.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoGradient.Customizable = true;
            this.rdoGradient.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoGradient.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoGradient.Location = new System.Drawing.Point(94, 101);
            this.rdoGradient.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoGradient.Name = "rdoGradient";
            this.rdoGradient.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoGradient.Size = new System.Drawing.Size(90, 21);
            this.rdoGradient.TabIndex = 2151;
            this.rdoGradient.Text = "Gradient";
            this.rdoGradient.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoGradient.UseVisualStyleBackColor = true;
            this.rdoGradient.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // rdoHitMiss
            // 
            this.rdoHitMiss.AutoSize = true;
            this.rdoHitMiss.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoHitMiss.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoHitMiss.Customizable = true;
            this.rdoHitMiss.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoHitMiss.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoHitMiss.Location = new System.Drawing.Point(6, 101);
            this.rdoHitMiss.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoHitMiss.Name = "rdoHitMiss";
            this.rdoHitMiss.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoHitMiss.Size = new System.Drawing.Size(81, 21);
            this.rdoHitMiss.TabIndex = 2150;
            this.rdoHitMiss.Text = "HitMiss";
            this.rdoHitMiss.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoHitMiss.UseVisualStyleBackColor = true;
            this.rdoHitMiss.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // rdoBlackHat
            // 
            this.rdoBlackHat.AutoSize = true;
            this.rdoBlackHat.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoBlackHat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoBlackHat.Customizable = true;
            this.rdoBlackHat.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoBlackHat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoBlackHat.Location = new System.Drawing.Point(94, 74);
            this.rdoBlackHat.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoBlackHat.Name = "rdoBlackHat";
            this.rdoBlackHat.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoBlackHat.Size = new System.Drawing.Size(92, 21);
            this.rdoBlackHat.TabIndex = 2149;
            this.rdoBlackHat.Text = "BlackHat";
            this.rdoBlackHat.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoBlackHat.UseVisualStyleBackColor = true;
            this.rdoBlackHat.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // rdoTopHat
            // 
            this.rdoTopHat.AutoSize = true;
            this.rdoTopHat.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoTopHat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoTopHat.Customizable = true;
            this.rdoTopHat.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoTopHat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoTopHat.Location = new System.Drawing.Point(6, 74);
            this.rdoTopHat.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoTopHat.Name = "rdoTopHat";
            this.rdoTopHat.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoTopHat.Size = new System.Drawing.Size(82, 21);
            this.rdoTopHat.TabIndex = 2148;
            this.rdoTopHat.Text = "TopHat";
            this.rdoTopHat.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoTopHat.UseVisualStyleBackColor = true;
            this.rdoTopHat.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // rdoCross
            // 
            this.rdoCross.AutoSize = true;
            this.rdoCross.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoCross.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoCross.Customizable = true;
            this.rdoCross.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoCross.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoCross.Location = new System.Drawing.Point(94, 47);
            this.rdoCross.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoCross.Name = "rdoCross";
            this.rdoCross.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoCross.Size = new System.Drawing.Size(70, 21);
            this.rdoCross.TabIndex = 2147;
            this.rdoCross.Text = "Close";
            this.rdoCross.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoCross.UseVisualStyleBackColor = true;
            this.rdoCross.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // rdoErode
            // 
            this.rdoErode.AutoSize = true;
            this.rdoErode.Checked = true;
            this.rdoErode.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoErode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoErode.Customizable = true;
            this.rdoErode.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoErode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoErode.Location = new System.Drawing.Point(6, 20);
            this.rdoErode.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoErode.Name = "rdoErode";
            this.rdoErode.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoErode.Size = new System.Drawing.Size(72, 21);
            this.rdoErode.TabIndex = 2144;
            this.rdoErode.TabStop = true;
            this.rdoErode.Text = "Erode";
            this.rdoErode.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoErode.UseVisualStyleBackColor = true;
            this.rdoErode.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // rdoOpen
            // 
            this.rdoOpen.AutoSize = true;
            this.rdoOpen.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoOpen.Customizable = true;
            this.rdoOpen.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoOpen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoOpen.Location = new System.Drawing.Point(6, 47);
            this.rdoOpen.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoOpen.Name = "rdoOpen";
            this.rdoOpen.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoOpen.Size = new System.Drawing.Size(69, 21);
            this.rdoOpen.TabIndex = 2146;
            this.rdoOpen.Text = "Open";
            this.rdoOpen.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoOpen.UseVisualStyleBackColor = true;
            this.rdoOpen.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // rdoDilate
            // 
            this.rdoDilate.AutoSize = true;
            this.rdoDilate.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoDilate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoDilate.Customizable = true;
            this.rdoDilate.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoDilate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoDilate.Location = new System.Drawing.Point(94, 20);
            this.rdoDilate.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoDilate.Name = "rdoDilate";
            this.rdoDilate.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoDilate.Size = new System.Drawing.Size(72, 21);
            this.rdoDilate.TabIndex = 2145;
            this.rdoDilate.Text = "Dilate";
            this.rdoDilate.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoDilate.UseVisualStyleBackColor = true;
            this.rdoDilate.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tbMorpH);
            this.groupBox2.Controls.Add(this.rdoShapesCross);
            this.groupBox2.Controls.Add(this.rdoShapesRect);
            this.groupBox2.Controls.Add(this.rdoShapesEllipse);
            this.groupBox2.Controls.Add(this.tbMorpW);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(424, 224);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(488, 150);
            this.groupBox2.TabIndex = 2148;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Shapes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(93, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 25);
            this.label2.TabIndex = 2153;
            this.label2.Text = "Height";
            // 
            // tbMorpH
            // 
            this.tbMorpH._Customizable = true;
            this.tbMorpH.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbMorpH.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbMorpH.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbMorpH.BorderRadius = 3;
            this.tbMorpH.BorderSize = 1;
            this.tbMorpH.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbMorpH.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbMorpH.Location = new System.Drawing.Point(179, 58);
            this.tbMorpH.MultiLine = false;
            this.tbMorpH.Name = "tbMorpH";
            this.tbMorpH.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbMorpH.PasswordChar = false;
            this.tbMorpH.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbMorpH.PlaceHolderText = "3";
            this.tbMorpH.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbMorpH.Size = new System.Drawing.Size(45, 31);
            this.tbMorpH.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbMorpH.TabIndex = 2152;
            // 
            // rdoShapesCross
            // 
            this.rdoShapesCross.AutoSize = true;
            this.rdoShapesCross.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoShapesCross.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoShapesCross.Customizable = true;
            this.rdoShapesCross.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoShapesCross.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoShapesCross.Location = new System.Drawing.Point(6, 74);
            this.rdoShapesCross.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoShapesCross.Name = "rdoShapesCross";
            this.rdoShapesCross.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoShapesCross.Size = new System.Drawing.Size(71, 21);
            this.rdoShapesCross.TabIndex = 2148;
            this.rdoShapesCross.Text = "Cross";
            this.rdoShapesCross.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoShapesCross.UseVisualStyleBackColor = true;
            this.rdoShapesCross.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // rdoShapesRect
            // 
            this.rdoShapesRect.AutoSize = true;
            this.rdoShapesRect.Checked = true;
            this.rdoShapesRect.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoShapesRect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoShapesRect.Customizable = true;
            this.rdoShapesRect.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoShapesRect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoShapesRect.Location = new System.Drawing.Point(6, 20);
            this.rdoShapesRect.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoShapesRect.Name = "rdoShapesRect";
            this.rdoShapesRect.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoShapesRect.Size = new System.Drawing.Size(65, 21);
            this.rdoShapesRect.TabIndex = 2144;
            this.rdoShapesRect.TabStop = true;
            this.rdoShapesRect.Text = "Rect";
            this.rdoShapesRect.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoShapesRect.UseVisualStyleBackColor = true;
            this.rdoShapesRect.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // rdoShapesEllipse
            // 
            this.rdoShapesEllipse.AutoSize = true;
            this.rdoShapesEllipse.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoShapesEllipse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoShapesEllipse.Customizable = true;
            this.rdoShapesEllipse.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoShapesEllipse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoShapesEllipse.Location = new System.Drawing.Point(6, 47);
            this.rdoShapesEllipse.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoShapesEllipse.Name = "rdoShapesEllipse";
            this.rdoShapesEllipse.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoShapesEllipse.Size = new System.Drawing.Size(75, 21);
            this.rdoShapesEllipse.TabIndex = 2146;
            this.rdoShapesEllipse.Text = "Ellipse";
            this.rdoShapesEllipse.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoShapesEllipse.UseVisualStyleBackColor = true;
            this.rdoShapesEllipse.CheckedChanged += new System.EventHandler(this.OnShapes_CheckedChanged);
            // 
            // tbMorpW
            // 
            this.tbMorpW._Customizable = true;
            this.tbMorpW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbMorpW.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbMorpW.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbMorpW.BorderRadius = 3;
            this.tbMorpW.BorderSize = 1;
            this.tbMorpW.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbMorpW.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbMorpW.Location = new System.Drawing.Point(179, 14);
            this.tbMorpW.MultiLine = false;
            this.tbMorpW.Name = "tbMorpW";
            this.tbMorpW.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbMorpW.PasswordChar = false;
            this.tbMorpW.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbMorpW.PlaceHolderText = "3";
            this.tbMorpW.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbMorpW.Size = new System.Drawing.Size(45, 31);
            this.tbMorpW.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbMorpW.TabIndex = 2150;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(93, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 25);
            this.label1.TabIndex = 2149;
            this.label1.Text = "Width";
            // 
            // ibSource
            // 
            this.ibSource.Location = new System.Drawing.Point(8, 20);
            this.ibSource.Name = "ibSource";
            this.ibSource.Size = new System.Drawing.Size(374, 220);
            this.ibSource.TabIndex = 2149;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox3.Controls.Add(this.cbLayerList);
            this.groupBox3.Controls.Add(this.ibSource);
            this.groupBox3.Location = new System.Drawing.Point(16, 58);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(390, 285);
            this.groupBox3.TabIndex = 2154;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Input Layer";
            // 
            // cbLayerList
            // 
            this.cbLayerList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.BorderRadius = 3;
            this.cbLayerList.BorderSize = 2;
            this.cbLayerList.Customizable = false;
            this.cbLayerList.DataSource = null;
            this.cbLayerList.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbLayerList.Location = new System.Drawing.Point(8, 248);
            this.cbLayerList.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList.Name = "cbLayerList";
            this.cbLayerList.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList.SelectedIndex = -1;
            this.cbLayerList.Size = new System.Drawing.Size(374, 32);
            this.cbLayerList.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList.TabIndex = 2158;
            this.cbLayerList.Texts = "";
            this.cbLayerList.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox4.Controls.Add(this.cbLayerList2);
            this.groupBox4.Controls.Add(this.btnNewPanel_Desty);
            this.groupBox4.Controls.Add(this.ibDestination);
            this.groupBox4.Location = new System.Drawing.Point(16, 360);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(390, 285);
            this.groupBox4.TabIndex = 2155;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output Layer";
            // 
            // cbLayerList2
            // 
            this.cbLayerList2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList2.BorderRadius = 3;
            this.cbLayerList2.BorderSize = 2;
            this.cbLayerList2.Customizable = false;
            this.cbLayerList2.DataSource = null;
            this.cbLayerList2.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList2.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList2.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList2.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbLayerList2.Location = new System.Drawing.Point(8, 248);
            this.cbLayerList2.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList2.Name = "cbLayerList2";
            this.cbLayerList2.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList2.SelectedIndex = -1;
            this.cbLayerList2.Size = new System.Drawing.Size(338, 32);
            this.cbLayerList2.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList2.TabIndex = 2159;
            this.cbLayerList2.Texts = "";
            this.cbLayerList2.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList2_SelectedIndexChanged);
            // 
            // btnNewPanel_Desty
            // 
            this.btnNewPanel_Desty.BackColor = System.Drawing.Color.Transparent;
            this.btnNewPanel_Desty.BackIcon = true;
            this.btnNewPanel_Desty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewPanel_Desty.Customizable = true;
            this.btnNewPanel_Desty.DropdownMenu = null;
            this.btnNewPanel_Desty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel_Desty.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            this.btnNewPanel_Desty.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel_Desty.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNewPanel_Desty.IconSize = 30;
            this.btnNewPanel_Desty.Location = new System.Drawing.Point(354, 250);
            this.btnNewPanel_Desty.Name = "btnNewPanel_Desty";
            this.btnNewPanel_Desty.Size = new System.Drawing.Size(28, 28);
            this.btnNewPanel_Desty.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.btnNewPanel_Desty.TabIndex = 2157;
            this.btnNewPanel_Desty.TabStop = false;
            this.btnNewPanel_Desty.Click += new System.EventHandler(this.btnNewPanel_Desty_Click);
            // 
            // ibDestination
            // 
            this.ibDestination.Location = new System.Drawing.Point(8, 20);
            this.ibDestination.Name = "ibDestination";
            this.ibDestination.Size = new System.Drawing.Size(374, 220);
            this.ibDestination.TabIndex = 2149;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // 
            // btnMorpRun
            // 
            this.btnMorpRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.btnMorpRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(171)))));
            this.btnMorpRun.BorderRadius = 3;
            this.btnMorpRun.BorderSize = 1;
            this.btnMorpRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMorpRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnMorpRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(171)))));
            this.btnMorpRun.FlatAppearance.BorderSize = 1;
            this.btnMorpRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(241)))), ((int)(((byte)(247)))));
            this.btnMorpRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(246)))), ((int)(((byte)(251)))));
            this.btnMorpRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMorpRun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnMorpRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.btnMorpRun.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnMorpRun.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.btnMorpRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnMorpRun.IconSize = 1;
            this.btnMorpRun.Location = new System.Drawing.Point(423, 563);
            this.btnMorpRun.Name = "btnMorpRun";
            this.btnMorpRun.Size = new System.Drawing.Size(488, 40);
            this.btnMorpRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnMorpRun.TabIndex = 2153;
            this.btnMorpRun.Text = "Run";
            this.btnMorpRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnMorpRun.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.btnMorpRun.UseVisualStyleBackColor = false;
            this.btnMorpRun.Click += new System.EventHandler(this.btnMorpRun_Click);
            // 
            // FormVision_Morphology
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Morphology";
            this.ClientSize = new System.Drawing.Size(920, 655);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.Name = "FormVision_Morphology";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Morphology";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.pnlClientArea.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private RJCodeUI_M1.RJControls.RJRadioButton rdoOpen;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoDilate;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoErode;
        private System.Windows.Forms.GroupBox groupBox1;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoTopHat;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoCross;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoBlackHat;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoHitMiss;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoGradient;
        private System.Windows.Forms.GroupBox groupBox2;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoShapesCross;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoShapesRect;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoShapesEllipse;
        private System.Windows.Forms.Label label2;
        private RJCodeUI_M1.RJControls.RJTextBox tbMorpH;
        private RJCodeUI_M1.RJControls.RJTextBox tbMorpW;
        private System.Windows.Forms.Label label1;
        private VisionTestImageCanvas ibSource;
        private System.Windows.Forms.GroupBox groupBox4;
        private VisionTestImageCanvas ibDestination;
        private RJCodeUI_M1.RJControls.RJButton btnMorpRun;
        private System.Windows.Forms.GroupBox groupBox3;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel_Desty;
        private System.Windows.Forms.ToolTip toolTip1;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList2;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList;
    }
}



