namespace LitePlacer
{
    partial class TapeEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TapeEditForm));
            this.TapeEditOK_button = new System.Windows.Forms.Button();
            this.ID_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TapeOrientation_comboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TapeRotation_comboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TapeOffsetX_textBox = new System.Windows.Forms.TextBox();
            this.TapePitch_textBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Nozzle_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.TapeWidth_comboBox = new System.Windows.Forms.ComboBox();
            this.TapeOffsetY_textBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.Capacity_textBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.Type_comboBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.NextPart_textBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.FirstY_textBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.FirstX_textBox = new System.Windows.Forms.TextBox();
            this.LastY_label = new System.Windows.Forms.Label();
            this.LastY_textBox = new System.Windows.Forms.TextBox();
            this.LastX_label = new System.Windows.Forms.Label();
            this.LastX_textBox = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.PlacementZ_textBox = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.PickupZ_textBox = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.TrayID_textBox = new System.Windows.Forms.TextBox();
            this.GetFirstPosition_button = new System.Windows.Forms.Button();
            this.GetLastPosition_button = new System.Windows.Forms.Button();
            this.GetPickupZ_button = new System.Windows.Forms.Button();
            this.GetPlacementZ_button = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.RotationDirect_textBox = new System.Windows.Forms.TextBox();
            this.TapeEditCancel_button = new System.Windows.Forms.Button();
            this.ResetPickupZ_button = new System.Windows.Forms.Button();
            this.ResetPlacementZ_button = new System.Windows.Forms.Button();
            this.ResetTrayID_button = new System.Windows.Forms.Button();
            this.CoordinatesForParts_checkBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Nozzle_numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // TapeEditOK_button
            // 
            this.TapeEditOK_button.Location = new System.Drawing.Point(124, 419);
            this.TapeEditOK_button.Name = "TapeEditOK_button";
            this.TapeEditOK_button.Size = new System.Drawing.Size(75, 23);
            this.TapeEditOK_button.TabIndex = 0;
            this.TapeEditOK_button.Text = "OK";
            this.TapeEditOK_button.UseVisualStyleBackColor = true;
            this.TapeEditOK_button.Click += new System.EventHandler(this.TapeEditOK_button_Click);
            // 
            // ID_textBox
            // 
            this.ID_textBox.Location = new System.Drawing.Point(16, 26);
            this.ID_textBox.Name = "ID_textBox";
            this.ID_textBox.Size = new System.Drawing.Size(100, 20);
            this.ID_textBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.ID_textBox, "Name of the tape (ex: 0805, 10k)");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "ID";
            this.toolTip1.SetToolTip(this.label1, "Name of the tape (ex: 0805, 10k)");
            // 
            // TapeOrientation_comboBox
            // 
            this.TapeOrientation_comboBox.FormattingEnabled = true;
            this.TapeOrientation_comboBox.Items.AddRange(new object[] {
            "+X",
            "-X",
            "+Y",
            "-Y"});
            this.TapeOrientation_comboBox.Location = new System.Drawing.Point(122, 25);
            this.TapeOrientation_comboBox.Name = "TapeOrientation_comboBox";
            this.TapeOrientation_comboBox.Size = new System.Drawing.Size(61, 21);
            this.TapeOrientation_comboBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.TapeOrientation_comboBox, "Which way the part count increases.\r\n+Y: Tape holes on right\r\n-Y: Tape holes on l" +
        "eft\r\n+X: Tape holes down\r\n-X: Tape holes up");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(119, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Orientation";
            this.toolTip1.SetToolTip(this.label2, "Which way the part count increases.\r\n+Y: Tape holes on right\r\n-Y: Tape holes on l" +
        "eft\r\n+X: Tape holes down\r\n-X: Tape holes up");
            // 
            // TapeRotation_comboBox
            // 
            this.TapeRotation_comboBox.FormattingEnabled = true;
            this.TapeRotation_comboBox.Items.AddRange(new object[] {
            "0deg.",
            "90deg.",
            "180deg.",
            "270deg."});
            this.TapeRotation_comboBox.Location = new System.Drawing.Point(189, 26);
            this.TapeRotation_comboBox.Name = "TapeRotation_comboBox";
            this.TapeRotation_comboBox.Size = new System.Drawing.Size(61, 21);
            this.TapeRotation_comboBox.TabIndex = 5;
            this.toolTip1.SetToolTip(this.TapeRotation_comboBox, "Manufactures put parts on tapes rotated in any which way.\r\nPart on a tape in +Y o" +
        "rientation (holes to right) and with 0 deg.\r\nrotation in the CAD data, is rotate" +
        "d this much when placed.");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(186, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Rotation";
            this.toolTip1.SetToolTip(this.label3, "Manufactures put parts on tapes rotated in any which way.\r\nPart on a tape in +Y o" +
        "rientation (holes to right) and with 0 deg.\r\nrotation in the CAD data, is rotate" +
        "d this much when placed.");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(255, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Nozzle";
            this.toolTip1.SetToolTip(this.label4, "Manufactures put parts on tapes rotated in any which way.\r\nPart on a tape in +Y o" +
        "rientation (holes to right) and with 0 deg.\r\nrotation in the CAD data, is rotate" +
        "d this much when placed.");
            // 
            // TapeOffsetX_textBox
            // 
            this.TapeOffsetX_textBox.Location = new System.Drawing.Point(73, 116);
            this.TapeOffsetX_textBox.Name = "TapeOffsetX_textBox";
            this.TapeOffsetX_textBox.Size = new System.Drawing.Size(48, 20);
            this.TapeOffsetX_textBox.TabIndex = 9;
            this.toolTip1.SetToolTip(this.TapeOffsetX_textBox, "Automatically set if you use standard tape, selected above.\r\nIf you have a custom" +
        " part holder or somethign like that,\r\nyou can set a non-standard measures here.");
            // 
            // TapePitch_textBox
            // 
            this.TapePitch_textBox.Location = new System.Drawing.Point(19, 116);
            this.TapePitch_textBox.Name = "TapePitch_textBox";
            this.TapePitch_textBox.Size = new System.Drawing.Size(48, 20);
            this.TapePitch_textBox.TabIndex = 10;
            this.toolTip1.SetToolTip(this.TapePitch_textBox, "Automatically set if you use standard tape, selected above.\r\nIf you have a custom" +
        " part holder or somethign like that,\r\nyou can set a non-standard measures here.");
            this.TapePitch_textBox.TextChanged += new System.EventHandler(this.TapePitch_textBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(70, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Offset X";
            this.toolTip1.SetToolTip(this.label5, "Automatically set if you use standard tape, selected above.\r\nIf you have a custom" +
        " part holder or somethign like that,\r\nyou can set a non-standard measures here.");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Part Pitch";
            this.toolTip1.SetToolTip(this.label6, "Automatically set if you use standard tape, selected above.\r\nIf you have a custom" +
        " part holder or somethign like that,\r\nyou can set a non-standard measures here.");
            // 
            // Nozzle_numericUpDown
            // 
            this.Nozzle_numericUpDown.Location = new System.Drawing.Point(256, 26);
            this.Nozzle_numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Nozzle_numericUpDown.Name = "Nozzle_numericUpDown";
            this.Nozzle_numericUpDown.Size = new System.Drawing.Size(37, 20);
            this.Nozzle_numericUpDown.TabIndex = 7;
            this.toolTip1.SetToolTip(this.Nozzle_numericUpDown, "Which nozzle to use for these parts");
            this.Nozzle_numericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // TapeWidth_comboBox
            // 
            this.TapeWidth_comboBox.DropDownWidth = 170;
            this.TapeWidth_comboBox.FormattingEnabled = true;
            this.TapeWidth_comboBox.Items.AddRange(new object[] {
            "8/2mm",
            "8/4mm",
            "12/4mm",
            "12/8mm",
            "16/4mm",
            "16/8mm",
            "16/12mm",
            "24/4mm",
            "24/8mm",
            "24/12mm",
            "24/16mm",
            "24/20mm",
            "32/4mm",
            "32/8mm",
            "32/12mm",
            "32/16mm",
            "32/20mm",
            "32/24mm",
            "32/28mm",
            "32/32mm",
            "custom"});
            this.TapeWidth_comboBox.Location = new System.Drawing.Point(16, 73);
            this.TapeWidth_comboBox.Name = "TapeWidth_comboBox";
            this.TapeWidth_comboBox.Size = new System.Drawing.Size(77, 21);
            this.TapeWidth_comboBox.TabIndex = 11;
            this.toolTip1.SetToolTip(this.TapeWidth_comboBox, "Select a standard size");
            this.TapeWidth_comboBox.SelectedIndexChanged += new System.EventHandler(this.TapeWidth_comboBox_SelectedIndexChanged);
            // 
            // TapeOffsetY_textBox
            // 
            this.TapeOffsetY_textBox.Location = new System.Drawing.Point(127, 116);
            this.TapeOffsetY_textBox.Name = "TapeOffsetY_textBox";
            this.TapeOffsetY_textBox.Size = new System.Drawing.Size(48, 20);
            this.TapeOffsetY_textBox.TabIndex = 14;
            this.toolTip1.SetToolTip(this.TapeOffsetY_textBox, "Automatically set if you use standard tape, selected above.\r\nIf you have a custom" +
        " part holder or somethign like that,\r\nyou can set a non-standard measures here.");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(124, 100);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Offset Y";
            this.toolTip1.SetToolTip(this.label7, "Automatically set if you use standard tape, selected above.\r\nIf you have a custom" +
        " part holder or somethign like that,\r\nyou can set a non-standard measures here.");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Tape Width";
            this.toolTip1.SetToolTip(this.label8, "Select a standard size");
            // 
            // Capacity_textBox
            // 
            this.Capacity_textBox.Location = new System.Drawing.Point(379, 25);
            this.Capacity_textBox.Name = "Capacity_textBox";
            this.Capacity_textBox.Size = new System.Drawing.Size(48, 20);
            this.Capacity_textBox.TabIndex = 20;
            this.toolTip1.SetToolTip(this.Capacity_textBox, "How many parts on a strip\r\n0: You are prompted to place a part at the location\r\n<" +
        "0: Infinite (feeder)");
            this.Capacity_textBox.Visible = false;
            this.Capacity_textBox.TextChanged += new System.EventHandler(this.Capacity_textBox_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(376, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Capacity";
            this.toolTip1.SetToolTip(this.label10, "How many parts on a strip\r\n0: You are prompted to place a part at the location\r\n<" +
        "0: Infinite (feeder)");
            this.label10.Visible = false;
            // 
            // Type_comboBox
            // 
            this.Type_comboBox.FormattingEnabled = true;
            this.Type_comboBox.Items.AddRange(new object[] {
            "Paper (White)",
            "Black Plastic",
            "Clear Plastic"});
            this.Type_comboBox.Location = new System.Drawing.Point(122, 73);
            this.Type_comboBox.Name = "Type_comboBox";
            this.Type_comboBox.Size = new System.Drawing.Size(114, 21);
            this.Type_comboBox.TabIndex = 22;
            this.toolTip1.SetToolTip(this.Type_comboBox, "What optical filter set is used");
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(119, 57);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(31, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Type";
            this.toolTip1.SetToolTip(this.label11, "What optical filter set is used");
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(310, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(50, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Next part";
            this.toolTip1.SetToolTip(this.label12, "Number of next part used for placement");
            // 
            // NextPart_textBox
            // 
            this.NextPart_textBox.Location = new System.Drawing.Point(313, 25);
            this.NextPart_textBox.Name = "NextPart_textBox";
            this.NextPart_textBox.Size = new System.Drawing.Size(48, 20);
            this.NextPart_textBox.TabIndex = 24;
            this.toolTip1.SetToolTip(this.NextPart_textBox, "Number of next part used for placement");
            this.NextPart_textBox.TextChanged += new System.EventHandler(this.NextPart_textBox_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(67, 154);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(36, 13);
            this.label13.TabIndex = 29;
            this.label13.Text = "First Y";
            this.toolTip1.SetToolTip(this.label13, "Location of first hole/alingment mark/part");
            // 
            // FirstY_textBox
            // 
            this.FirstY_textBox.Location = new System.Drawing.Point(70, 170);
            this.FirstY_textBox.Name = "FirstY_textBox";
            this.FirstY_textBox.Size = new System.Drawing.Size(48, 20);
            this.FirstY_textBox.TabIndex = 28;
            this.toolTip1.SetToolTip(this.FirstY_textBox, "Location of first hole/alingment mark/part");
            this.FirstY_textBox.TextChanged += new System.EventHandler(this.FirstY_textBox_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(13, 154);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(36, 13);
            this.label14.TabIndex = 27;
            this.label14.Text = "First X";
            this.toolTip1.SetToolTip(this.label14, "Location of first hole/alingment mark/part");
            // 
            // FirstX_textBox
            // 
            this.FirstX_textBox.Location = new System.Drawing.Point(16, 170);
            this.FirstX_textBox.Name = "FirstX_textBox";
            this.FirstX_textBox.Size = new System.Drawing.Size(48, 20);
            this.FirstX_textBox.TabIndex = 26;
            this.toolTip1.SetToolTip(this.FirstX_textBox, "Location of first hole/alingment mark/part");
            this.FirstX_textBox.TextChanged += new System.EventHandler(this.FirstX_textBox_TextChanged);
            // 
            // LastY_label
            // 
            this.LastY_label.AutoSize = true;
            this.LastY_label.Location = new System.Drawing.Point(67, 261);
            this.LastY_label.Name = "LastY_label";
            this.LastY_label.Size = new System.Drawing.Size(37, 13);
            this.LastY_label.TabIndex = 33;
            this.LastY_label.Text = "Last Y";
            this.toolTip1.SetToolTip(this.LastY_label, "Location of last hole/alingment mark/part");
            // 
            // LastY_textBox
            // 
            this.LastY_textBox.Enabled = false;
            this.LastY_textBox.Location = new System.Drawing.Point(70, 277);
            this.LastY_textBox.Name = "LastY_textBox";
            this.LastY_textBox.Size = new System.Drawing.Size(48, 20);
            this.LastY_textBox.TabIndex = 32;
            this.toolTip1.SetToolTip(this.LastY_textBox, "Location of last hole/alingment mark/part");
            this.LastY_textBox.TextChanged += new System.EventHandler(this.LastY_textBox_TextChanged);
            // 
            // LastX_label
            // 
            this.LastX_label.AutoSize = true;
            this.LastX_label.Location = new System.Drawing.Point(13, 261);
            this.LastX_label.Name = "LastX_label";
            this.LastX_label.Size = new System.Drawing.Size(37, 13);
            this.LastX_label.TabIndex = 31;
            this.LastX_label.Text = "Last X";
            this.toolTip1.SetToolTip(this.LastX_label, "Location of last hole/alingment mark/part");
            // 
            // LastX_textBox
            // 
            this.LastX_textBox.Enabled = false;
            this.LastX_textBox.Location = new System.Drawing.Point(16, 277);
            this.LastX_textBox.Name = "LastX_textBox";
            this.LastX_textBox.Size = new System.Drawing.Size(48, 20);
            this.LastX_textBox.TabIndex = 30;
            this.toolTip1.SetToolTip(this.LastX_textBox, "Location of last hole/alingment mark/part");
            this.LastX_textBox.TextChanged += new System.EventHandler(this.LastX_textBox_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(364, 154);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(67, 13);
            this.label17.TabIndex = 42;
            this.label17.Text = "Placement Z";
            this.toolTip1.SetToolTip(this.label17, "Z values used in pickup and placement.\r\nIf reset, values are measured when first " +
        "needed.\r\n");
            // 
            // PlacementZ_textBox
            // 
            this.PlacementZ_textBox.Location = new System.Drawing.Point(367, 170);
            this.PlacementZ_textBox.Name = "PlacementZ_textBox";
            this.PlacementZ_textBox.Size = new System.Drawing.Size(48, 20);
            this.PlacementZ_textBox.TabIndex = 41;
            this.toolTip1.SetToolTip(this.PlacementZ_textBox, "Z values used in pickup and placement.\r\nIf reset, values are measured when first " +
        "needed.\r\n");
            this.PlacementZ_textBox.TextChanged += new System.EventHandler(this.PlacementZ_textBox_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(310, 154);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 13);
            this.label18.TabIndex = 40;
            this.label18.Text = "Pickup Z";
            this.toolTip1.SetToolTip(this.label18, "Z values used in pickup and placement.\r\nIf reset, values are measured when first " +
        "needed.\r\n");
            // 
            // PickupZ_textBox
            // 
            this.PickupZ_textBox.Location = new System.Drawing.Point(313, 170);
            this.PickupZ_textBox.Name = "PickupZ_textBox";
            this.PickupZ_textBox.Size = new System.Drawing.Size(48, 20);
            this.PickupZ_textBox.TabIndex = 39;
            this.toolTip1.SetToolTip(this.PickupZ_textBox, "Z values used in pickup and placement.\r\nIf reset, values are measured when first " +
        "needed.\r\n");
            this.PickupZ_textBox.TextChanged += new System.EventHandler(this.PickupZ_textBox_TextChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(310, 57);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(42, 13);
            this.label19.TabIndex = 48;
            this.label19.Text = "Tray ID";
            this.toolTip1.SetToolTip(this.label19, "Name of tray (--: not part of a tray)\r\n\r\nTray is a collection of tapes, usually o" +
        "n holders of some sort.\r\npeople use these to quickly set up the machine to popul" +
        "ate\r\ncertain baords they do often.");
            // 
            // TrayID_textBox
            // 
            this.TrayID_textBox.Location = new System.Drawing.Point(313, 73);
            this.TrayID_textBox.Name = "TrayID_textBox";
            this.TrayID_textBox.Size = new System.Drawing.Size(100, 20);
            this.TrayID_textBox.TabIndex = 47;
            this.toolTip1.SetToolTip(this.TrayID_textBox, "Name of the tape (ex: 0805, 10k)");
            // 
            // GetFirstPosition_button
            // 
            this.GetFirstPosition_button.Location = new System.Drawing.Point(48, 196);
            this.GetFirstPosition_button.Name = "GetFirstPosition_button";
            this.GetFirstPosition_button.Size = new System.Drawing.Size(102, 23);
            this.GetFirstPosition_button.TabIndex = 36;
            this.GetFirstPosition_button.Text = "Get current ";
            this.toolTip1.SetToolTip(this.GetFirstPosition_button, "move current X and Y position to the boxes");
            this.GetFirstPosition_button.UseVisualStyleBackColor = true;
            this.GetFirstPosition_button.Click += new System.EventHandler(this.GetFirstPosition_button_Click);
            // 
            // GetLastPosition_button
            // 
            this.GetLastPosition_button.Enabled = false;
            this.GetLastPosition_button.Location = new System.Drawing.Point(16, 303);
            this.GetLastPosition_button.Name = "GetLastPosition_button";
            this.GetLastPosition_button.Size = new System.Drawing.Size(102, 23);
            this.GetLastPosition_button.TabIndex = 38;
            this.GetLastPosition_button.Text = "Get current ";
            this.toolTip1.SetToolTip(this.GetLastPosition_button, "move current X and Y position to the boxes");
            this.GetLastPosition_button.UseVisualStyleBackColor = true;
            this.GetLastPosition_button.Click += new System.EventHandler(this.GetLastPosition_button_Click);
            // 
            // GetPickupZ_button
            // 
            this.GetPickupZ_button.Location = new System.Drawing.Point(313, 225);
            this.GetPickupZ_button.Name = "GetPickupZ_button";
            this.GetPickupZ_button.Size = new System.Drawing.Size(47, 23);
            this.GetPickupZ_button.TabIndex = 44;
            this.GetPickupZ_button.Text = "Get";
            this.toolTip1.SetToolTip(this.GetPickupZ_button, "move current Z position to the boxes");
            this.GetPickupZ_button.UseVisualStyleBackColor = true;
            this.GetPickupZ_button.Click += new System.EventHandler(this.GetPickupZ_button_Click);
            // 
            // GetPlacementZ_button
            // 
            this.GetPlacementZ_button.Location = new System.Drawing.Point(368, 225);
            this.GetPlacementZ_button.Name = "GetPlacementZ_button";
            this.GetPlacementZ_button.Size = new System.Drawing.Size(47, 23);
            this.GetPlacementZ_button.TabIndex = 46;
            this.GetPlacementZ_button.Text = "Get";
            this.toolTip1.SetToolTip(this.GetPlacementZ_button, "move current Z position to the boxes");
            this.GetPlacementZ_button.UseVisualStyleBackColor = true;
            this.GetPlacementZ_button.Click += new System.EventHandler(this.GetPlacementZ_button_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(124, 154);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 13);
            this.label9.TabIndex = 52;
            this.label9.Text = "Rotation";
            this.toolTip1.SetToolTip(this.label9, "Location of last hole/alingment mark/part");
            // 
            // RotationDirect_textBox
            // 
            this.RotationDirect_textBox.Location = new System.Drawing.Point(124, 170);
            this.RotationDirect_textBox.Name = "RotationDirect_textBox";
            this.RotationDirect_textBox.Size = new System.Drawing.Size(48, 20);
            this.RotationDirect_textBox.TabIndex = 51;
            this.toolTip1.SetToolTip(this.RotationDirect_textBox, "Location of last hole/alingment mark/part");
            this.RotationDirect_textBox.TextChanged += new System.EventHandler(this.ACorrection_textBox_TextChanged);
            // 
            // TapeEditCancel_button
            // 
            this.TapeEditCancel_button.Location = new System.Drawing.Point(259, 419);
            this.TapeEditCancel_button.Name = "TapeEditCancel_button";
            this.TapeEditCancel_button.Size = new System.Drawing.Size(75, 23);
            this.TapeEditCancel_button.TabIndex = 16;
            this.TapeEditCancel_button.Text = "Cancel";
            this.TapeEditCancel_button.UseVisualStyleBackColor = true;
            this.TapeEditCancel_button.Click += new System.EventHandler(this.TapeEditCancel_button_Click);
            // 
            // ResetPickupZ_button
            // 
            this.ResetPickupZ_button.Location = new System.Drawing.Point(313, 196);
            this.ResetPickupZ_button.Name = "ResetPickupZ_button";
            this.ResetPickupZ_button.Size = new System.Drawing.Size(48, 23);
            this.ResetPickupZ_button.TabIndex = 43;
            this.ResetPickupZ_button.Text = "Reset";
            this.ResetPickupZ_button.UseVisualStyleBackColor = true;
            this.ResetPickupZ_button.Click += new System.EventHandler(this.ResetPickupZ_button_Click);
            // 
            // ResetPlacementZ_button
            // 
            this.ResetPlacementZ_button.Location = new System.Drawing.Point(368, 196);
            this.ResetPlacementZ_button.Name = "ResetPlacementZ_button";
            this.ResetPlacementZ_button.Size = new System.Drawing.Size(48, 23);
            this.ResetPlacementZ_button.TabIndex = 45;
            this.ResetPlacementZ_button.Text = "Reset";
            this.ResetPlacementZ_button.UseVisualStyleBackColor = true;
            this.ResetPlacementZ_button.Click += new System.EventHandler(this.ResetPlacementZ_button_Click);
            // 
            // ResetTrayID_button
            // 
            this.ResetTrayID_button.Location = new System.Drawing.Point(313, 99);
            this.ResetTrayID_button.Name = "ResetTrayID_button";
            this.ResetTrayID_button.Size = new System.Drawing.Size(48, 23);
            this.ResetTrayID_button.TabIndex = 49;
            this.ResetTrayID_button.Text = "Reset";
            this.ResetTrayID_button.UseVisualStyleBackColor = true;
            this.ResetTrayID_button.Click += new System.EventHandler(this.ResetTrayID_button_Click);
            // 
            // CoordinatesForParts_checkBox
            // 
            this.CoordinatesForParts_checkBox.AutoSize = true;
            this.CoordinatesForParts_checkBox.Location = new System.Drawing.Point(16, 241);
            this.CoordinatesForParts_checkBox.Name = "CoordinatesForParts_checkBox";
            this.CoordinatesForParts_checkBox.Size = new System.Drawing.Size(139, 17);
            this.CoordinatesForParts_checkBox.TabIndex = 50;
            this.CoordinatesForParts_checkBox.Text = "Use coordinates directly";
            this.CoordinatesForParts_checkBox.UseVisualStyleBackColor = true;
            this.CoordinatesForParts_checkBox.CheckedChanged += new System.EventHandler(this.CoordinatesForParts_checkBox_CheckedChanged);
            // 
            // TapeEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 464);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.RotationDirect_textBox);
            this.Controls.Add(this.CoordinatesForParts_checkBox);
            this.Controls.Add(this.ResetTrayID_button);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.TrayID_textBox);
            this.Controls.Add(this.GetPlacementZ_button);
            this.Controls.Add(this.ResetPlacementZ_button);
            this.Controls.Add(this.GetPickupZ_button);
            this.Controls.Add(this.ResetPickupZ_button);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.PlacementZ_textBox);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.PickupZ_textBox);
            this.Controls.Add(this.GetLastPosition_button);
            this.Controls.Add(this.GetFirstPosition_button);
            this.Controls.Add(this.LastY_label);
            this.Controls.Add(this.LastY_textBox);
            this.Controls.Add(this.LastX_label);
            this.Controls.Add(this.LastX_textBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.FirstY_textBox);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.FirstX_textBox);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.NextPart_textBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.Type_comboBox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.Capacity_textBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.TapeEditCancel_button);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.TapeOffsetY_textBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TapeWidth_comboBox);
            this.Controls.Add(this.TapePitch_textBox);
            this.Controls.Add(this.TapeOffsetX_textBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Nozzle_numericUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TapeRotation_comboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TapeOrientation_comboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ID_textBox);
            this.Controls.Add(this.TapeEditOK_button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TapeEditForm";
            this.Text = "Edit Tape Parameters";
            this.Load += new System.EventHandler(this.TapeEditForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Nozzle_numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TapeEditOK_button;
        private System.Windows.Forms.TextBox ID_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox TapeOrientation_comboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox TapeRotation_comboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown Nozzle_numericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TapeOffsetX_textBox;
        private System.Windows.Forms.TextBox TapePitch_textBox;
        private System.Windows.Forms.ComboBox TapeWidth_comboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TapeOffsetY_textBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button TapeEditCancel_button;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox Capacity_textBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox Type_comboBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox NextPart_textBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox FirstY_textBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox FirstX_textBox;
        private System.Windows.Forms.Label LastY_label;
        private System.Windows.Forms.TextBox LastY_textBox;
        private System.Windows.Forms.Label LastX_label;
        private System.Windows.Forms.TextBox LastX_textBox;
        private System.Windows.Forms.Button GetFirstPosition_button;
        private System.Windows.Forms.Button GetLastPosition_button;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox PlacementZ_textBox;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox PickupZ_textBox;
        private System.Windows.Forms.Button GetPickupZ_button;
        private System.Windows.Forms.Button ResetPickupZ_button;
        private System.Windows.Forms.Button GetPlacementZ_button;
        private System.Windows.Forms.Button ResetPlacementZ_button;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox TrayID_textBox;
        private System.Windows.Forms.Button ResetTrayID_button;
        private System.Windows.Forms.CheckBox CoordinatesForParts_checkBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox RotationDirect_textBox;
    }
}