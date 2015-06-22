using System.ComponentModel;
using System.Windows.Forms;

namespace LitePlacer
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Park_button = new System.Windows.Forms.Button();
            this.TestNeedleRecognition_button = new System.Windows.Forms.Button();
            this.textBoxSendtoTinyG = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.xpos_textBox = new System.Windows.Forms.TextBox();
            this.ypos_textBox = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.zpos_textBox = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.apos_textBox = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.SerialMonitor_richTextBox = new System.Windows.Forms.RichTextBox();
            this.Test1_button = new System.Windows.Forms.Button();
            this.Test2_button = new System.Windows.Forms.Button();
            this.Job_openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.Job_saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.OpticalHome_button = new System.Windows.Forms.Button();
            this.label97 = new System.Windows.Forms.Label();
            this.Test3_button = new System.Windows.Forms.Button();
            this.Test4_button = new System.Windows.Forms.Button();
            this.label124 = new System.Windows.Forms.Label();
            this.Test5_button = new System.Windows.Forms.Button();
            this.Test6_button = new System.Windows.Forms.Button();
            this.label145 = new System.Windows.Forms.Label();
            this.Tapes_tabPage = new System.Windows.Forms.TabPage();
            this.view_nextParts_button = new System.Windows.Forms.Button();
            this.pickup_next_button = new System.Windows.Forms.Button();
            this.label128 = new System.Windows.Forms.Label();
            this.tape_ViewComponents_button = new System.Windows.Forms.Button();
            this.TapeSet1_button = new System.Windows.Forms.Button();
            this.Tape_resetZs_button = new System.Windows.Forms.Button();
            this.Tape_GoToNext_button = new System.Windows.Forms.Button();
            this.label67 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.SetPartNo_button = new System.Windows.Forms.Button();
            this.NextPart_TextBox = new System.Windows.Forms.MaskedTextBox();
            this.TapeGoTo_button = new System.Windows.Forms.Button();
            this.TapeDown_button = new System.Windows.Forms.Button();
            this.TapeUp_button = new System.Windows.Forms.Button();
            this.DeleteTape_button = new System.Windows.Forms.Button();
            this.AddTape_button = new System.Windows.Forms.Button();
            this.label109 = new System.Windows.Forms.Label();
            this.Tapes_dataGridView = new System.Windows.Forms.DataGridView();
            this.SelectButtonColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OriginalTapeOrientation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.OriginalPartOrientation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.PartType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.HolePitch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PartPitch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HoleToPartSpacingX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.holeToPartSpacingYDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pickupZDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.placeZDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tapeObjBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label101 = new System.Windows.Forms.Label();
            this.label100 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.CameraSetupTest_button = new System.Windows.Forms.Button();
            this.GotoUpCamPosition_button = new System.Windows.Forms.Button();
            this.SetUpCamPosition_button = new System.Windows.Forms.Button();
            this.label99 = new System.Windows.Forms.Label();
            this.label98 = new System.Windows.Forms.Label();
            this.UpcamPositionY_textBox = new System.Windows.Forms.TextBox();
            this.UpcamPositionX_textBox = new System.Windows.Forms.TextBox();
            this.PickupCenterY_textBox = new System.Windows.Forms.TextBox();
            this.PickupCenterX_textBox = new System.Windows.Forms.TextBox();
            this.JigY_textBox = new System.Windows.Forms.TextBox();
            this.JigX_textBox = new System.Windows.Forms.TextBox();
            this.SetPickupCenter_button = new System.Windows.Forms.Button();
            this.SetPCB0_button = new System.Windows.Forms.Button();
            this.GotoPickupCenter_button = new System.Windows.Forms.Button();
            this.GotoPCB0_button = new System.Windows.Forms.Button();
            this.label95 = new System.Windows.Forms.Label();
            this.label96 = new System.Windows.Forms.Label();
            this.label93 = new System.Windows.Forms.Label();
            this.label94 = new System.Windows.Forms.Label();
            this.Snapshot_button = new System.Windows.Forms.Button();
            this.ImageTest_checkBox = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.zoffset_textbox = new System.Windows.Forms.TextBox();
            this.label130 = new System.Windows.Forms.Label();
            this.label131 = new System.Windows.Forms.Label();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label127 = new System.Windows.Forms.Label();
            this.fiducial_designator_regexp_textBox = new System.Windows.Forms.TextBox();
            this.button_setTemplate = new System.Windows.Forms.Button();
            this.label126 = new System.Windows.Forms.Label();
            this.fiducialTemlateMatch_textBox = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.cb_useTemplate = new System.Windows.Forms.CheckBox();
            this.label129 = new System.Windows.Forms.Label();
            this.calibMoveDistance_textBox = new System.Windows.Forms.TextBox();
            this.SlackMeasurement_label = new System.Windows.Forms.Label();
            this.button_camera_calibrate = new System.Windows.Forms.Button();
            this.DownCameraBoxYmmPerPixel_label = new System.Windows.Forms.Label();
            this.DownCameraBoxXmmPerPixel_label = new System.Windows.Forms.Label();
            this.label71 = new System.Windows.Forms.Label();
            this.DownCameraBoxX_textBox = new System.Windows.Forms.TextBox();
            this.label70 = new System.Windows.Forms.Label();
            this.DownCameraBoxY_textBox = new System.Windows.Forms.TextBox();
            this.DownCamera_Calibration_button = new System.Windows.Forms.Button();
            this.UpCameraBoxYmmPerPixel_label = new System.Windows.Forms.Label();
            this.UpCameraBoxXmmPerPixel_label = new System.Windows.Forms.Label();
            this.UpCameraBoxY_textBox = new System.Windows.Forms.TextBox();
            this.UpCameraBoxX_textBox = new System.Windows.Forms.TextBox();
            this.label106 = new System.Windows.Forms.Label();
            this.label105 = new System.Windows.Forms.Label();
            this.NeedleOffsetY_textBox = new System.Windows.Forms.TextBox();
            this.NeedleOffsetX_textBox = new System.Windows.Forms.TextBox();
            this.label149 = new System.Windows.Forms.Label();
            this.label148 = new System.Windows.Forms.Label();
            this.label146 = new System.Windows.Forms.Label();
            this.label143 = new System.Windows.Forms.Label();
            this.Offset2Method_button = new System.Windows.Forms.Button();
            this.NeedleOffset_label = new System.Windows.Forms.Label();
            this.label115 = new System.Windows.Forms.Label();
            this.ZUp_button = new System.Windows.Forms.Button();
            this.ZDown_button = new System.Windows.Forms.Button();
            this.tabPageBasicSetup = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabpage1 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label73 = new System.Windows.Forms.Label();
            this.xsv_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label74 = new System.Windows.Forms.Label();
            this.label75 = new System.Windows.Forms.Label();
            this.xjh_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label76 = new System.Windows.Forms.Label();
            this.Xmax_checkBox = new System.Windows.Forms.CheckBox();
            this.Xlim_checkBox = new System.Windows.Forms.CheckBox();
            this.Xhome_checkBox = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tr1_textBox = new System.Windows.Forms.TextBox();
            this.m1deg18_radioButton = new System.Windows.Forms.RadioButton();
            this.m1deg09_radioButton = new System.Windows.Forms.RadioButton();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.mi1_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.xvm_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.xjm_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.TestX_button = new System.Windows.Forms.Button();
            this.TestXY_button = new System.Windows.Forms.Button();
            this.TestYX_button = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label77 = new System.Windows.Forms.Label();
            this.ysv_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label78 = new System.Windows.Forms.Label();
            this.label79 = new System.Windows.Forms.Label();
            this.yjh_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label80 = new System.Windows.Forms.Label();
            this.Ymax_checkBox = new System.Windows.Forms.CheckBox();
            this.Ylim_checkBox = new System.Windows.Forms.CheckBox();
            this.Yhome_checkBox = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tr2_textBox = new System.Windows.Forms.TextBox();
            this.m2deg18_radioButton = new System.Windows.Forms.RadioButton();
            this.m2deg09_radioButton = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.mi2_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.yvm_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.yjm_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TestY_button = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label123 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label81 = new System.Windows.Forms.Label();
            this.zsv_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label82 = new System.Windows.Forms.Label();
            this.label83 = new System.Windows.Forms.Label();
            this.zjh_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label84 = new System.Windows.Forms.Label();
            this.Zmax_checkBox = new System.Windows.Forms.CheckBox();
            this.Zlim_checkBox = new System.Windows.Forms.CheckBox();
            this.Zhome_checkBox = new System.Windows.Forms.CheckBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.tr3_textBox = new System.Windows.Forms.TextBox();
            this.m3deg18_radioButton = new System.Windows.Forms.RadioButton();
            this.m3deg09_radioButton = new System.Windows.Forms.RadioButton();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.mi3_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.zvm_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.zjm_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.ZTestTravel_textBox = new System.Windows.Forms.TextBox();
            this.TestZ_button = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.tr4_textBox = new System.Windows.Forms.TextBox();
            this.m4deg18_radioButton = new System.Windows.Forms.RadioButton();
            this.m4deg09_radioButton = new System.Windows.Forms.RadioButton();
            this.label42 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.mi4_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.avm_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label49 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.ajm_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.TestA_button = new System.Windows.Forms.Button();
            this.VacuumRelease_textBox = new System.Windows.Forms.TextBox();
            this.label119 = new System.Windows.Forms.Label();
            this.VacuumTime_textBox = new System.Windows.Forms.TextBox();
            this.label118 = new System.Windows.Forms.Label();
            this.label90 = new System.Windows.Forms.Label();
            this.SquareCorrection_textBox = new System.Windows.Forms.TextBox();
            this.SmallMovement_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label87 = new System.Windows.Forms.Label();
            this.SlackCompensation_checkBox = new System.Windows.Forms.CheckBox();
            this.SizeYMax_textBox = new System.Windows.Forms.TextBox();
            this.SizeXMax_textBox = new System.Windows.Forms.TextBox();
            this.ParkLocationY_textBox = new System.Windows.Forms.TextBox();
            this.ParkLocationX_textBox = new System.Windows.Forms.TextBox();
            this.label113 = new System.Windows.Forms.Label();
            this.label102 = new System.Windows.Forms.Label();
            this.label107 = new System.Windows.Forms.Label();
            this.label92 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.Homebutton = new System.Windows.Forms.Button();
            this.HomeZ_button = new System.Windows.Forms.Button();
            this.HomeY_button = new System.Windows.Forms.Button();
            this.HomeXY_button = new System.Windows.Forms.Button();
            this.HomeX_button = new System.Windows.Forms.Button();
            this.BuiltInSettings_button = new System.Windows.Forms.Button();
            this.SaveSettings_button = new System.Windows.Forms.Button();
            this.DefaultSettings_button = new System.Windows.Forms.Button();
            this.buttonRefreshPortList = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxSerialPorts = new System.Windows.Forms.ComboBox();
            this.labelSerialPortStatus = new System.Windows.Forms.Label();
            this.Z_Backoff_label = new System.Windows.Forms.Label();
            this.label117 = new System.Windows.Forms.Label();
            this.Z0toPCB_BasicTab_label = new System.Windows.Forms.Label();
            this.label111 = new System.Windows.Forms.Label();
            this.Zlb_label = new System.Windows.Forms.Label();
            this.SetProbing_button = new System.Windows.Forms.Button();
            this.buttonConnectSerial = new System.Windows.Forms.Button();
            this.MotorPower_checkBox = new System.Windows.Forms.CheckBox();
            this.Vacuum_checkBox = new System.Windows.Forms.CheckBox();
            this.Pump_checkBox = new System.Windows.Forms.CheckBox();
            this.RunJob_tabPage = new System.Windows.Forms.TabPage();
            this.AbortPlacement_button = new System.Windows.Forms.Button();
            this.needle_calibration_test_button = new System.Windows.Forms.Button();
            this.PausePlacement_button = new System.Windows.Forms.Button();
            this.ChangeNeedle_button = new System.Windows.Forms.Button();
            this.ValidMeasurement_checkBox = new System.Windows.Forms.CheckBox();
            this.ReMeasure_button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ResetAllTapes_button = new System.Windows.Forms.Button();
            this.ResetOneTape_button = new System.Windows.Forms.Button();
            this.NewRow_button = new System.Windows.Forms.Button();
            this.PlaceThese_button = new System.Windows.Forms.Button();
            this.DeleteComponentGroup_button = new System.Windows.Forms.Button();
            this.Down_button = new System.Windows.Forms.Button();
            this.Up_button = new System.Windows.Forms.Button();
            this.PlaceAll_button = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.PlaceOne_button = new System.Windows.Forms.Button();
            this.ShowMachine_button = new System.Windows.Forms.Button();
            this.ShowNominal_button = new System.Windows.Forms.Button();
            this.JobOffsetY_textBox = new System.Windows.Forms.TextBox();
            this.JobOffsetX_textBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.MachineCoords_label = new System.Windows.Forms.Label();
            this.PlacedValue_label = new System.Windows.Forms.Label();
            this.PlacedRotation_label = new System.Windows.Forms.Label();
            this.PlacedY_label = new System.Windows.Forms.Label();
            this.PlacedX_label = new System.Windows.Forms.Label();
            this.PlacedComponent_label = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.label63 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.label89 = new System.Windows.Forms.Label();
            this.label88 = new System.Windows.Forms.Label();
            this.label86 = new System.Windows.Forms.Label();
            this.label85 = new System.Windows.Forms.Label();
            this.JobData_GridView = new System.Windows.Forms.DataGridView();
            this.countDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.componentListDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.componentTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.methodDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.methodParametersDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jobDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Bottom_checkBox = new System.Windows.Forms.CheckBox();
            this.CadData_GridView = new System.Windows.Forms.DataGridView();
            this.designatorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.footprintDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xnominalDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ynominalDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rotationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xmachineDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ymachineDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rotationmachineDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.methodDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isFiducialDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.physicalComponentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.MultiCalibrate_button = new System.Windows.Forms.Button();
            this.StopDemo_button = new System.Windows.Forms.Button();
            this.Demo_button = new System.Windows.Forms.Button();
            this.tabControlPages = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.CAD_openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TrueX_label = new System.Windows.Forms.Label();
            this.mechHome_button = new System.Windows.Forms.Button();
            this.OptHome_button = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCADFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadJobFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveJobFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Tapes_tabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tapes_dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tapeObjBindingSource)).BeginInit();
            this.groupBox12.SuspendLayout();
            this.tabPageBasicSetup.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabpage1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SmallMovement_numericUpDown)).BeginInit();
            this.RunJob_tabPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.JobData_GridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobDataBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CadData_GridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.physicalComponentBindingSource)).BeginInit();
            this.tabControlPages.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Park_button
            // 
            this.Park_button.Location = new System.Drawing.Point(543, 923);
            this.Park_button.Name = "Park_button";
            this.Park_button.Size = new System.Drawing.Size(75, 23);
            this.Park_button.TabIndex = 41;
            this.Park_button.Text = "Park";
            this.toolTip1.SetToolTip(this.Park_button, "Moves the machine to \"Park\" location,\r\ndefined at Basic Setup tab.");
            this.Park_button.UseVisualStyleBackColor = true;
            this.Park_button.Click += new System.EventHandler(this.Park_button_Click);
            // 
            // TestNeedleRecognition_button
            // 
            this.TestNeedleRecognition_button.Location = new System.Drawing.Point(16, 96);
            this.TestNeedleRecognition_button.Name = "TestNeedleRecognition_button";
            this.TestNeedleRecognition_button.Size = new System.Drawing.Size(110, 23);
            this.TestNeedleRecognition_button.TabIndex = 63;
            this.TestNeedleRecognition_button.Text = "Calibrate Needle";
            this.toolTip1.SetToolTip(this.TestNeedleRecognition_button, "Re-runs needle calibration routine.");
            this.TestNeedleRecognition_button.UseVisualStyleBackColor = true;
            this.TestNeedleRecognition_button.Click += new System.EventHandler(this.TestNeedleRecognition_button_Click);
            // 
            // textBoxSendtoTinyG
            // 
            this.textBoxSendtoTinyG.Location = new System.Drawing.Point(499, 609);
            this.textBoxSendtoTinyG.Name = "textBoxSendtoTinyG";
            this.textBoxSendtoTinyG.Size = new System.Drawing.Size(313, 20);
            this.textBoxSendtoTinyG.TabIndex = 8;
            this.toolTip1.SetToolTip(this.textBoxSendtoTinyG, "On enter, the text is sent directly to TinyG.");
            this.textBoxSendtoTinyG.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSendtoTinyG_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(424, 612);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Text to send:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(3, 762);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(24, 18);
            this.label14.TabIndex = 7;
            this.label14.Text = "X:";
            // 
            // xpos_textBox
            // 
            this.xpos_textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.xpos_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xpos_textBox.Location = new System.Drawing.Point(38, 761);
            this.xpos_textBox.Name = "xpos_textBox";
            this.xpos_textBox.ReadOnly = true;
            this.xpos_textBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.xpos_textBox.Size = new System.Drawing.Size(113, 19);
            this.xpos_textBox.TabIndex = 9;
            this.xpos_textBox.Text = "- - - -";
            this.xpos_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.xpos_textBox, "Current X position");
            // 
            // ypos_textBox
            // 
            this.ypos_textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ypos_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ypos_textBox.Location = new System.Drawing.Point(38, 788);
            this.ypos_textBox.Name = "ypos_textBox";
            this.ypos_textBox.ReadOnly = true;
            this.ypos_textBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ypos_textBox.Size = new System.Drawing.Size(113, 19);
            this.ypos_textBox.TabIndex = 11;
            this.ypos_textBox.Text = "- - - -";
            this.ypos_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.ypos_textBox, "Current Y position");
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(3, 789);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(23, 18);
            this.label17.TabIndex = 10;
            this.label17.Text = "Y:";
            // 
            // zpos_textBox
            // 
            this.zpos_textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.zpos_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zpos_textBox.Location = new System.Drawing.Point(38, 815);
            this.zpos_textBox.Name = "zpos_textBox";
            this.zpos_textBox.ReadOnly = true;
            this.zpos_textBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.zpos_textBox.Size = new System.Drawing.Size(113, 19);
            this.zpos_textBox.TabIndex = 13;
            this.zpos_textBox.Text = "- - - -";
            this.zpos_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.zpos_textBox, "Current Z position");
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(3, 816);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(23, 18);
            this.label18.TabIndex = 12;
            this.label18.Text = "Z:";
            // 
            // apos_textBox
            // 
            this.apos_textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.apos_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.apos_textBox.Location = new System.Drawing.Point(38, 842);
            this.apos_textBox.Name = "apos_textBox";
            this.apos_textBox.ReadOnly = true;
            this.apos_textBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.apos_textBox.Size = new System.Drawing.Size(113, 19);
            this.apos_textBox.TabIndex = 15;
            this.apos_textBox.Text = "- - - -";
            this.apos_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.apos_textBox, "Current A (rotation) position");
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(3, 843);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(23, 18);
            this.label19.TabIndex = 14;
            this.label19.Text = "A:";
            // 
            // SerialMonitor_richTextBox
            // 
            this.SerialMonitor_richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SerialMonitor_richTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SerialMonitor_richTextBox.Location = new System.Drawing.Point(499, 12);
            this.SerialMonitor_richTextBox.Name = "SerialMonitor_richTextBox";
            this.SerialMonitor_richTextBox.ReadOnly = true;
            this.SerialMonitor_richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.SerialMonitor_richTextBox.Size = new System.Drawing.Size(316, 574);
            this.SerialMonitor_richTextBox.TabIndex = 16;
            this.SerialMonitor_richTextBox.TabStop = false;
            this.SerialMonitor_richTextBox.Text = "";
            this.toolTip1.SetToolTip(this.SerialMonitor_richTextBox, "Shows the TinyG communication and diagnostic messages\r\n");
            // 
            // Test1_button
            // 
            this.Test1_button.Location = new System.Drawing.Point(543, 761);
            this.Test1_button.Name = "Test1_button";
            this.Test1_button.Size = new System.Drawing.Size(108, 23);
            this.Test1_button.TabIndex = 18;
            this.Test1_button.Text = "Test 1";
            this.Test1_button.UseVisualStyleBackColor = true;
            this.Test1_button.Click += new System.EventHandler(this.Test1_button_Click);
            // 
            // Test2_button
            // 
            this.Test2_button.Location = new System.Drawing.Point(543, 788);
            this.Test2_button.Name = "Test2_button";
            this.Test2_button.Size = new System.Drawing.Size(108, 23);
            this.Test2_button.TabIndex = 19;
            this.Test2_button.Text = "Test 2";
            this.Test2_button.UseVisualStyleBackColor = true;
            this.Test2_button.Click += new System.EventHandler(this.Test2_button_Click);
            // 
            // Job_openFileDialog
            // 
            this.Job_openFileDialog.Filter = "LitePlacer Job files (*.lpj)|*.lpj|All files (*.*)|*.*";
            this.Job_openFileDialog.ReadOnlyChecked = true;
            this.Job_openFileDialog.SupportMultiDottedExtensions = true;
            this.Job_openFileDialog.Title = "Job File to Load";
            // 
            // Job_saveFileDialog
            // 
            this.Job_saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            // 
            // OpticalHome_button
            // 
            this.OpticalHome_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpticalHome_button.Location = new System.Drawing.Point(104, 876);
            this.OpticalHome_button.Name = "OpticalHome_button";
            this.OpticalHome_button.Size = new System.Drawing.Size(75, 45);
            this.OpticalHome_button.TabIndex = 37;
            this.OpticalHome_button.Text = "Home";
            this.toolTip1.SetToolTip(this.OpticalHome_button, "Homes the machine\r\nFirst basic homing using limit swithces,\r\nthen optical homing " +
        "based on home mark.");
            this.OpticalHome_button.UseVisualStyleBackColor = true;
            this.OpticalHome_button.Click += new System.EventHandler(this.OpticalHome_button_Click);
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Location = new System.Drawing.Point(342, 794);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(53, 117);
            this.label97.TabIndex = 61;
            this.label97.Text = "Jogging: \r\nF5: <  \r\nF6: >\r\nF7: ^  \r\nF8: v\r\nF9: CCW \r\nF10: CW\r\nF11: Z^  \r\nF12: Z v" +
    "";
            this.toolTip1.SetToolTip(this.label97, resources.GetString("label97.ToolTip"));
            // 
            // Test3_button
            // 
            this.Test3_button.Location = new System.Drawing.Point(543, 815);
            this.Test3_button.Name = "Test3_button";
            this.Test3_button.Size = new System.Drawing.Size(108, 23);
            this.Test3_button.TabIndex = 66;
            this.Test3_button.Text = "Test 3";
            this.Test3_button.UseVisualStyleBackColor = true;
            this.Test3_button.Click += new System.EventHandler(this.Test3_button_Click);
            // 
            // Test4_button
            // 
            this.Test4_button.Location = new System.Drawing.Point(543, 842);
            this.Test4_button.Name = "Test4_button";
            this.Test4_button.Size = new System.Drawing.Size(108, 23);
            this.Test4_button.TabIndex = 52;
            this.Test4_button.Text = "Test 4";
            this.Test4_button.UseVisualStyleBackColor = true;
            this.Test4_button.Click += new System.EventHandler(this.Test4_button_Click);
            // 
            // label124
            // 
            this.label124.AutoSize = true;
            this.label124.Location = new System.Drawing.Point(401, 784);
            this.label124.Name = "label124";
            this.label124.Size = new System.Drawing.Size(52, 91);
            this.label124.TabIndex = 67;
            this.label124.Text = "\r\nAlt+Shift: \r\nAlt: \r\nAlt+Ctrl: \r\nShift: \r\nnone:\r\nCtrl:";
            this.label124.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Test5_button
            // 
            this.Test5_button.Location = new System.Drawing.Point(543, 869);
            this.Test5_button.Name = "Test5_button";
            this.Test5_button.Size = new System.Drawing.Size(108, 23);
            this.Test5_button.TabIndex = 68;
            this.Test5_button.Text = "Test 5";
            this.Test5_button.UseVisualStyleBackColor = true;
            this.Test5_button.Click += new System.EventHandler(this.Test5_button_Click);
            // 
            // Test6_button
            // 
            this.Test6_button.Location = new System.Drawing.Point(543, 895);
            this.Test6_button.Name = "Test6_button";
            this.Test6_button.Size = new System.Drawing.Size(108, 23);
            this.Test6_button.TabIndex = 69;
            this.Test6_button.Text = "Test 6";
            this.Test6_button.UseVisualStyleBackColor = true;
            this.Test6_button.Click += new System.EventHandler(this.Test6_button_Click);
            // 
            // label145
            // 
            this.label145.AutoSize = true;
            this.label145.Location = new System.Drawing.Point(450, 784);
            this.label145.Name = "label145";
            this.label145.Size = new System.Drawing.Size(91, 91);
            this.label145.TabIndex = 76;
            this.label145.Text = "\r\n100 mm / 90 deg.\r\n10 mm\r\n4 mm\r\n1 mm\r\n0.1 mm\r\n0.01 mm";
            // 
            // Tapes_tabPage
            // 
            this.Tapes_tabPage.Controls.Add(this.view_nextParts_button);
            this.Tapes_tabPage.Controls.Add(this.pickup_next_button);
            this.Tapes_tabPage.Controls.Add(this.label128);
            this.Tapes_tabPage.Controls.Add(this.tape_ViewComponents_button);
            this.Tapes_tabPage.Controls.Add(this.TapeSet1_button);
            this.Tapes_tabPage.Controls.Add(this.Tape_resetZs_button);
            this.Tapes_tabPage.Controls.Add(this.Tape_GoToNext_button);
            this.Tapes_tabPage.Controls.Add(this.label67);
            this.Tapes_tabPage.Controls.Add(this.label62);
            this.Tapes_tabPage.Controls.Add(this.SetPartNo_button);
            this.Tapes_tabPage.Controls.Add(this.NextPart_TextBox);
            this.Tapes_tabPage.Controls.Add(this.TapeGoTo_button);
            this.Tapes_tabPage.Controls.Add(this.TapeDown_button);
            this.Tapes_tabPage.Controls.Add(this.TapeUp_button);
            this.Tapes_tabPage.Controls.Add(this.DeleteTape_button);
            this.Tapes_tabPage.Controls.Add(this.AddTape_button);
            this.Tapes_tabPage.Controls.Add(this.label109);
            this.Tapes_tabPage.Controls.Add(this.Tapes_dataGridView);
            this.Tapes_tabPage.Location = new System.Drawing.Point(4, 22);
            this.Tapes_tabPage.Name = "Tapes_tabPage";
            this.Tapes_tabPage.Padding = new System.Windows.Forms.Padding(3);
            this.Tapes_tabPage.Size = new System.Drawing.Size(821, 690);
            this.Tapes_tabPage.TabIndex = 6;
            this.Tapes_tabPage.Text = "Tape Positions";
            this.Tapes_tabPage.UseVisualStyleBackColor = true;
            // 
            // view_nextParts_button
            // 
            this.view_nextParts_button.Location = new System.Drawing.Point(8, 515);
            this.view_nextParts_button.Name = "view_nextParts_button";
            this.view_nextParts_button.Size = new System.Drawing.Size(128, 25);
            this.view_nextParts_button.TabIndex = 77;
            this.view_nextParts_button.Text = "View Next Components";
            this.view_nextParts_button.UseVisualStyleBackColor = true;
            this.view_nextParts_button.Click += new System.EventHandler(this.view_nextParts_button_Click);
            // 
            // pickup_next_button
            // 
            this.pickup_next_button.Location = new System.Drawing.Point(740, 348);
            this.pickup_next_button.Name = "pickup_next_button";
            this.pickup_next_button.Size = new System.Drawing.Size(75, 22);
            this.pickup_next_button.TabIndex = 43;
            this.pickup_next_button.Text = "Pickup Next";
            this.pickup_next_button.UseVisualStyleBackColor = true;
            this.pickup_next_button.Click += new System.EventHandler(this.pickup_next_button_Click);
            // 
            // label128
            // 
            this.label128.AutoSize = true;
            this.label128.Location = new System.Drawing.Point(6, 558);
            this.label128.Name = "label128";
            this.label128.Size = new System.Drawing.Size(479, 52);
            this.label128.TabIndex = 42;
            this.label128.Text = resources.GetString("label128.Text");
            // 
            // tape_ViewComponents_button
            // 
            this.tape_ViewComponents_button.Location = new System.Drawing.Point(740, 319);
            this.tape_ViewComponents_button.Name = "tape_ViewComponents_button";
            this.tape_ViewComponents_button.Size = new System.Drawing.Size(75, 23);
            this.tape_ViewComponents_button.TabIndex = 41;
            this.tape_ViewComponents_button.Text = "ViewPickupLocation";
            this.toolTip1.SetToolTip(this.tape_ViewComponents_button, "Shows where the parts are to be picked up");
            this.tape_ViewComponents_button.UseVisualStyleBackColor = true;
            // 
            // TapeSet1_button
            // 
            this.TapeSet1_button.Location = new System.Drawing.Point(740, 164);
            this.TapeSet1_button.Name = "TapeSet1_button";
            this.TapeSet1_button.Size = new System.Drawing.Size(75, 23);
            this.TapeSet1_button.TabIndex = 40;
            this.TapeSet1_button.Text = "Cal@Hole 1";
            this.toolTip1.SetToolTip(this.TapeSet1_button, "Set this to first hole location");
            this.TapeSet1_button.UseVisualStyleBackColor = true;
            this.TapeSet1_button.Click += new System.EventHandler(this.TapeSet1_button_Click);
            // 
            // Tape_resetZs_button
            // 
            this.Tape_resetZs_button.Location = new System.Drawing.Point(740, 290);
            this.Tape_resetZs_button.Name = "Tape_resetZs_button";
            this.Tape_resetZs_button.Size = new System.Drawing.Size(75, 23);
            this.Tape_resetZs_button.TabIndex = 39;
            this.Tape_resetZs_button.Text = "Reset Z\'s";
            this.toolTip1.SetToolTip(this.Tape_resetZs_button, "Resets pickup and placement heights.");
            this.Tape_resetZs_button.UseVisualStyleBackColor = true;
            this.Tape_resetZs_button.Click += new System.EventHandler(this.Tape_resetZs_button_Click);
            // 
            // Tape_GoToNext_button
            // 
            this.Tape_GoToNext_button.Location = new System.Drawing.Point(740, 193);
            this.Tape_GoToNext_button.Name = "Tape_GoToNext_button";
            this.Tape_GoToNext_button.Size = new System.Drawing.Size(75, 23);
            this.Tape_GoToNext_button.TabIndex = 38;
            this.Tape_GoToNext_button.Text = "Go to next";
            this.toolTip1.SetToolTip(this.Tape_GoToNext_button, "Moves to hole of next part on the selected tape.");
            this.Tape_GoToNext_button.UseVisualStyleBackColor = true;
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(740, 267);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(39, 13);
            this.label67.TabIndex = 37;
            this.label67.Text = "Part #:";
            this.label67.Visible = false;
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(749, 248);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(66, 13);
            this.label62.TabIndex = 36;
            this.label62.Text = "for next part.";
            this.label62.Visible = false;
            // 
            // SetPartNo_button
            // 
            this.SetPartNo_button.Location = new System.Drawing.Point(740, 222);
            this.SetPartNo_button.Name = "SetPartNo_button";
            this.SetPartNo_button.Size = new System.Drawing.Size(75, 23);
            this.SetPartNo_button.TabIndex = 35;
            this.SetPartNo_button.Text = "Set this hole";
            this.SetPartNo_button.UseVisualStyleBackColor = true;
            this.SetPartNo_button.Visible = false;
            this.SetPartNo_button.Click += new System.EventHandler(this.SetPartNo_button_Click);
            // 
            // NextPart_TextBox
            // 
            this.NextPart_TextBox.Location = new System.Drawing.Point(783, 264);
            this.NextPart_TextBox.Mask = "999";
            this.NextPart_TextBox.Name = "NextPart_TextBox";
            this.NextPart_TextBox.PromptChar = ' ';
            this.NextPart_TextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.NextPart_TextBox.Size = new System.Drawing.Size(32, 20);
            this.NextPart_TextBox.TabIndex = 34;
            this.NextPart_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NextPart_TextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.NextPart_TextBox.Visible = false;
            // 
            // TapeGoTo_button
            // 
            this.TapeGoTo_button.Location = new System.Drawing.Point(740, 135);
            this.TapeGoTo_button.Name = "TapeGoTo_button";
            this.TapeGoTo_button.Size = new System.Drawing.Size(75, 23);
            this.TapeGoTo_button.TabIndex = 21;
            this.TapeGoTo_button.Text = "Go to hole 1";
            this.toolTip1.SetToolTip(this.TapeGoTo_button, "Moves the machine on hole 1 on the selected tape.");
            this.TapeGoTo_button.UseVisualStyleBackColor = true;
            this.TapeGoTo_button.Click += new System.EventHandler(this.TapeGoTo_button_Click);
            // 
            // TapeDown_button
            // 
            this.TapeDown_button.Location = new System.Drawing.Point(740, 106);
            this.TapeDown_button.Name = "TapeDown_button";
            this.TapeDown_button.Size = new System.Drawing.Size(75, 23);
            this.TapeDown_button.TabIndex = 20;
            this.TapeDown_button.Text = "Move Down";
            this.toolTip1.SetToolTip(this.TapeDown_button, "Moves the selected tape definition down on the table.");
            this.TapeDown_button.UseVisualStyleBackColor = true;
            this.TapeDown_button.Visible = false;
            this.TapeDown_button.Click += new System.EventHandler(this.TapeDown_button_Click);
            // 
            // TapeUp_button
            // 
            this.TapeUp_button.Location = new System.Drawing.Point(740, 77);
            this.TapeUp_button.Name = "TapeUp_button";
            this.TapeUp_button.Size = new System.Drawing.Size(75, 23);
            this.TapeUp_button.TabIndex = 19;
            this.TapeUp_button.Text = "Move Up";
            this.toolTip1.SetToolTip(this.TapeUp_button, "Moves the selected tape definition up on the table.");
            this.TapeUp_button.UseVisualStyleBackColor = true;
            this.TapeUp_button.Visible = false;
            this.TapeUp_button.Click += new System.EventHandler(this.TapeUp_button_Click);
            // 
            // DeleteTape_button
            // 
            this.DeleteTape_button.Location = new System.Drawing.Point(740, 48);
            this.DeleteTape_button.Name = "DeleteTape_button";
            this.DeleteTape_button.Size = new System.Drawing.Size(75, 23);
            this.DeleteTape_button.TabIndex = 18;
            this.DeleteTape_button.Text = "Delete";
            this.toolTip1.SetToolTip(this.DeleteTape_button, "Deletes the selected tape definition");
            this.DeleteTape_button.UseVisualStyleBackColor = true;
            this.DeleteTape_button.Click += new System.EventHandler(this.DeleteTape_button_Click);
            // 
            // AddTape_button
            // 
            this.AddTape_button.Location = new System.Drawing.Point(740, 19);
            this.AddTape_button.Name = "AddTape_button";
            this.AddTape_button.Size = new System.Drawing.Size(75, 23);
            this.AddTape_button.TabIndex = 17;
            this.AddTape_button.Text = "Add";
            this.toolTip1.SetToolTip(this.AddTape_button, "Adds a tape position to the table. \r\nCamera should be on hole 1");
            this.AddTape_button.UseVisualStyleBackColor = true;
            this.AddTape_button.Click += new System.EventHandler(this.AddTape_button_Click);
            // 
            // label109
            // 
            this.label109.AutoSize = true;
            this.label109.Location = new System.Drawing.Point(6, 3);
            this.label109.Name = "label109";
            this.label109.Size = new System.Drawing.Size(77, 13);
            this.label109.TabIndex = 16;
            this.label109.Text = "Tape Positions";
            // 
            // Tapes_dataGridView
            // 
            this.Tapes_dataGridView.AllowUserToAddRows = false;
            this.Tapes_dataGridView.AutoGenerateColumns = false;
            this.Tapes_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.Tapes_dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.Tapes_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tapes_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SelectButtonColumn,
            this.ID,
            this.OriginalTapeOrientation,
            this.OriginalPartOrientation,
            this.Type,
            this.PartType,
            this.HolePitch,
            this.PartPitch,
            this.HoleToPartSpacingX,
            this.holeToPartSpacingYDataGridViewTextBoxColumn,
            this.pickupZDataGridViewTextBoxColumn,
            this.placeZDataGridViewTextBoxColumn,
            this.bDataGridViewTextBoxColumn});
            this.Tapes_dataGridView.DataSource = this.tapeObjBindingSource;
            this.Tapes_dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Tapes_dataGridView.Location = new System.Drawing.Point(3, 19);
            this.Tapes_dataGridView.MultiSelect = false;
            this.Tapes_dataGridView.Name = "Tapes_dataGridView";
            this.Tapes_dataGridView.RowHeadersVisible = false;
            this.Tapes_dataGridView.RowHeadersWidth = 50;
            this.Tapes_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.Tapes_dataGridView.Size = new System.Drawing.Size(725, 480);
            this.Tapes_dataGridView.TabIndex = 15;
            this.Tapes_dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tapes_dataGridView_CellClick);
            this.Tapes_dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.EndEditModeForTapeSelection);
            // 
            // SelectButtonColumn
            // 
            this.SelectButtonColumn.HeaderText = "Select";
            this.SelectButtonColumn.Name = "SelectButtonColumn";
            this.SelectButtonColumn.Text = "Reset";
            this.SelectButtonColumn.Width = 43;
            // 
            // ID
            // 
            this.ID.DataPropertyName = "ID";
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.Width = 43;
            // 
            // OriginalTapeOrientation
            // 
            this.OriginalTapeOrientation.DataPropertyName = "OriginalTapeOrientation";
            this.OriginalTapeOrientation.HeaderText = "TapeOrient";
            this.OriginalTapeOrientation.Name = "OriginalTapeOrientation";
            this.OriginalTapeOrientation.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.OriginalTapeOrientation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.OriginalTapeOrientation.Width = 85;
            // 
            // OriginalPartOrientation
            // 
            this.OriginalPartOrientation.DataPropertyName = "OriginalPartOrientation";
            this.OriginalPartOrientation.HeaderText = "PartOrientation";
            this.OriginalPartOrientation.Name = "OriginalPartOrientation";
            this.OriginalPartOrientation.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.OriginalPartOrientation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.OriginalPartOrientation.Width = 102;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "Type";
            this.Type.HeaderText = "TapeType";
            this.Type.Name = "Type";
            this.Type.Width = 62;
            // 
            // PartType
            // 
            this.PartType.DataPropertyName = "PartType";
            this.PartType.HeaderText = "PartType";
            this.PartType.Name = "PartType";
            this.PartType.Width = 56;
            // 
            // HolePitch
            // 
            this.HolePitch.DataPropertyName = "HolePitch";
            this.HolePitch.HeaderText = "HolePitch";
            this.HolePitch.Name = "HolePitch";
            this.HolePitch.Width = 78;
            // 
            // PartPitch
            // 
            this.PartPitch.DataPropertyName = "PartPitch";
            this.PartPitch.HeaderText = "PartPitch";
            this.PartPitch.Name = "PartPitch";
            this.PartPitch.Width = 75;
            // 
            // HoleToPartSpacingX
            // 
            this.HoleToPartSpacingX.DataPropertyName = "HoleToPartSpacingX";
            this.HoleToPartSpacingX.HeaderText = "HoleToPartSpacingX";
            this.HoleToPartSpacingX.Name = "HoleToPartSpacingX";
            this.HoleToPartSpacingX.Width = 132;
            // 
            // holeToPartSpacingYDataGridViewTextBoxColumn
            // 
            this.holeToPartSpacingYDataGridViewTextBoxColumn.DataPropertyName = "HoleToPartSpacingY";
            this.holeToPartSpacingYDataGridViewTextBoxColumn.HeaderText = "HoleToPartSpacingY";
            this.holeToPartSpacingYDataGridViewTextBoxColumn.Name = "holeToPartSpacingYDataGridViewTextBoxColumn";
            this.holeToPartSpacingYDataGridViewTextBoxColumn.ReadOnly = true;
            this.holeToPartSpacingYDataGridViewTextBoxColumn.Width = 132;
            // 
            // pickupZDataGridViewTextBoxColumn
            // 
            this.pickupZDataGridViewTextBoxColumn.DataPropertyName = "PickupZ";
            this.pickupZDataGridViewTextBoxColumn.HeaderText = "PickupZ";
            this.pickupZDataGridViewTextBoxColumn.Name = "pickupZDataGridViewTextBoxColumn";
            this.pickupZDataGridViewTextBoxColumn.Width = 72;
            // 
            // placeZDataGridViewTextBoxColumn
            // 
            this.placeZDataGridViewTextBoxColumn.DataPropertyName = "PlaceZ";
            this.placeZDataGridViewTextBoxColumn.HeaderText = "PlaceZ";
            this.placeZDataGridViewTextBoxColumn.Name = "placeZDataGridViewTextBoxColumn";
            this.placeZDataGridViewTextBoxColumn.Width = 66;
            // 
            // bDataGridViewTextBoxColumn
            // 
            this.bDataGridViewTextBoxColumn.DataPropertyName = "b";
            this.bDataGridViewTextBoxColumn.HeaderText = "Slope";
            this.bDataGridViewTextBoxColumn.Name = "bDataGridViewTextBoxColumn";
            this.bDataGridViewTextBoxColumn.Width = 59;
            // 
            // tapeObjBindingSource
            // 
            this.tapeObjBindingSource.DataSource = typeof(LitePlacer.TapeObj);
            // 
            // label101
            // 
            this.label101.AutoSize = true;
            this.label101.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label101.Location = new System.Drawing.Point(194, 16);
            this.label101.Name = "label101";
            this.label101.Size = new System.Drawing.Size(53, 18);
            this.label101.TabIndex = 117;
            this.label101.Text = "Pickup";
            // 
            // label100
            // 
            this.label100.AutoSize = true;
            this.label100.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label100.Location = new System.Drawing.Point(93, 16);
            this.label100.Name = "label100";
            this.label100.Size = new System.Drawing.Size(73, 18);
            this.label100.TabIndex = 116;
            this.label100.Text = "PCB zero";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label55.Location = new System.Drawing.Point(3, 16);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(84, 18);
            this.label55.TabIndex = 115;
            this.label55.Text = "Up Camera";
            // 
            // CameraSetupTest_button
            // 
            this.CameraSetupTest_button.Location = new System.Drawing.Point(739, 836);
            this.CameraSetupTest_button.Name = "CameraSetupTest_button";
            this.CameraSetupTest_button.Size = new System.Drawing.Size(75, 23);
            this.CameraSetupTest_button.TabIndex = 114;
            this.CameraSetupTest_button.Text = "Test";
            this.CameraSetupTest_button.UseVisualStyleBackColor = true;
            this.CameraSetupTest_button.Visible = false;
            // 
            // GotoUpCamPosition_button
            // 
            this.GotoUpCamPosition_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GotoUpCamPosition_button.Location = new System.Drawing.Point(227, 762);
            this.GotoUpCamPosition_button.Name = "GotoUpCamPosition_button";
            this.GotoUpCamPosition_button.Size = new System.Drawing.Size(84, 23);
            this.GotoUpCamPosition_button.TabIndex = 79;
            this.GotoUpCamPosition_button.Text = "GoTo UpCamera";
            this.GotoUpCamPosition_button.UseVisualStyleBackColor = true;
            this.GotoUpCamPosition_button.Click += new System.EventHandler(this.GotoUpCamPosition_button_Click);
            // 
            // SetUpCamPosition_button
            // 
            this.SetUpCamPosition_button.Location = new System.Drawing.Point(21, 37);
            this.SetUpCamPosition_button.Name = "SetUpCamPosition_button";
            this.SetUpCamPosition_button.Size = new System.Drawing.Size(66, 23);
            this.SetUpCamPosition_button.TabIndex = 72;
            this.SetUpCamPosition_button.Text = "Set";
            this.toolTip1.SetToolTip(this.SetUpCamPosition_button, "Sets Up camera location");
            this.SetUpCamPosition_button.UseVisualStyleBackColor = true;
            this.SetUpCamPosition_button.Click += new System.EventHandler(this.SetUpCamPosition_button_Click);
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(18, 95);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(17, 13);
            this.label99.TabIndex = 71;
            this.label99.Text = "Y:";
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Location = new System.Drawing.Point(18, 69);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(17, 13);
            this.label98.TabIndex = 70;
            this.label98.Text = "X:";
            // 
            // UpcamPositionY_textBox
            // 
            this.UpcamPositionY_textBox.Location = new System.Drawing.Point(41, 92);
            this.UpcamPositionY_textBox.Name = "UpcamPositionY_textBox";
            this.UpcamPositionY_textBox.Size = new System.Drawing.Size(46, 20);
            this.UpcamPositionY_textBox.TabIndex = 69;
            this.UpcamPositionY_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UpcamPositionY_textBox_KeyPress);
            this.UpcamPositionY_textBox.Leave += new System.EventHandler(this.UpcamPositionY_textBox_Leave);
            // 
            // UpcamPositionX_textBox
            // 
            this.UpcamPositionX_textBox.Location = new System.Drawing.Point(41, 66);
            this.UpcamPositionX_textBox.Name = "UpcamPositionX_textBox";
            this.UpcamPositionX_textBox.Size = new System.Drawing.Size(46, 20);
            this.UpcamPositionX_textBox.TabIndex = 68;
            this.UpcamPositionX_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UpcamPositionX_textBox_KeyPress);
            this.UpcamPositionX_textBox.Leave += new System.EventHandler(this.UpcamPositionX_textBox_Leave);
            // 
            // PickupCenterY_textBox
            // 
            this.PickupCenterY_textBox.Location = new System.Drawing.Point(202, 92);
            this.PickupCenterY_textBox.Name = "PickupCenterY_textBox";
            this.PickupCenterY_textBox.Size = new System.Drawing.Size(46, 20);
            this.PickupCenterY_textBox.TabIndex = 39;
            this.PickupCenterY_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PickupCenterY_textBox_KeyPress);
            this.PickupCenterY_textBox.Leave += new System.EventHandler(this.PickupCenterY_textBox_Leave);
            // 
            // PickupCenterX_textBox
            // 
            this.PickupCenterX_textBox.Location = new System.Drawing.Point(202, 66);
            this.PickupCenterX_textBox.Name = "PickupCenterX_textBox";
            this.PickupCenterX_textBox.Size = new System.Drawing.Size(46, 20);
            this.PickupCenterX_textBox.TabIndex = 38;
            this.PickupCenterX_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PickupCenterX_textBox_KeyPress);
            this.PickupCenterX_textBox.Leave += new System.EventHandler(this.PickupCenterX_textBox_Leave);
            // 
            // JigY_textBox
            // 
            this.JigY_textBox.Location = new System.Drawing.Point(120, 92);
            this.JigY_textBox.Name = "JigY_textBox";
            this.JigY_textBox.Size = new System.Drawing.Size(46, 20);
            this.JigY_textBox.TabIndex = 35;
            this.JigY_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.JigY_textBox_KeyPress);
            this.JigY_textBox.Leave += new System.EventHandler(this.JigY_textBox_Leave);
            // 
            // JigX_textBox
            // 
            this.JigX_textBox.Location = new System.Drawing.Point(120, 66);
            this.JigX_textBox.Name = "JigX_textBox";
            this.JigX_textBox.Size = new System.Drawing.Size(46, 20);
            this.JigX_textBox.TabIndex = 34;
            this.JigX_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.JigX_textBox_KeyPress);
            this.JigX_textBox.Leave += new System.EventHandler(this.JigX_textBox_Leave);
            // 
            // SetPickupCenter_button
            // 
            this.SetPickupCenter_button.Location = new System.Drawing.Point(183, 37);
            this.SetPickupCenter_button.Name = "SetPickupCenter_button";
            this.SetPickupCenter_button.Size = new System.Drawing.Size(65, 23);
            this.SetPickupCenter_button.TabIndex = 44;
            this.SetPickupCenter_button.Text = "Set";
            this.toolTip1.SetToolTip(this.SetPickupCenter_button, "Sets manual pick up postion");
            this.SetPickupCenter_button.UseVisualStyleBackColor = true;
            this.SetPickupCenter_button.Click += new System.EventHandler(this.SetPickupCenter_button_Click);
            // 
            // SetPCB0_button
            // 
            this.SetPCB0_button.Location = new System.Drawing.Point(101, 37);
            this.SetPCB0_button.Name = "SetPCB0_button";
            this.SetPCB0_button.Size = new System.Drawing.Size(65, 23);
            this.SetPCB0_button.TabIndex = 43;
            this.SetPCB0_button.Text = "Set";
            this.toolTip1.SetToolTip(this.SetPCB0_button, "Sets PCB jig lower left location");
            this.SetPCB0_button.UseVisualStyleBackColor = true;
            this.SetPCB0_button.Click += new System.EventHandler(this.SetPCB0_button_Click);
            // 
            // GotoPickupCenter_button
            // 
            this.GotoPickupCenter_button.Location = new System.Drawing.Point(227, 818);
            this.GotoPickupCenter_button.Name = "GotoPickupCenter_button";
            this.GotoPickupCenter_button.Size = new System.Drawing.Size(84, 23);
            this.GotoPickupCenter_button.TabIndex = 41;
            this.GotoPickupCenter_button.Text = "GoTo Pickup";
            this.GotoPickupCenter_button.UseVisualStyleBackColor = true;
            this.GotoPickupCenter_button.Click += new System.EventHandler(this.GotoPickupCenter_button_Click);
            // 
            // GotoPCB0_button
            // 
            this.GotoPCB0_button.Location = new System.Drawing.Point(227, 790);
            this.GotoPCB0_button.Name = "GotoPCB0_button";
            this.GotoPCB0_button.Size = new System.Drawing.Size(84, 23);
            this.GotoPCB0_button.TabIndex = 40;
            this.GotoPCB0_button.Text = "GoTo PCB";
            this.GotoPCB0_button.UseVisualStyleBackColor = true;
            this.GotoPCB0_button.Click += new System.EventHandler(this.GotoPCB0_button_Click);
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(179, 95);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(17, 13);
            this.label95.TabIndex = 37;
            this.label95.Text = "Y:";
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Location = new System.Drawing.Point(179, 69);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(17, 13);
            this.label96.TabIndex = 36;
            this.label96.Text = "X:";
            // 
            // label93
            // 
            this.label93.AutoSize = true;
            this.label93.Location = new System.Drawing.Point(98, 95);
            this.label93.Name = "label93";
            this.label93.Size = new System.Drawing.Size(17, 13);
            this.label93.TabIndex = 33;
            this.label93.Text = "Y:";
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Location = new System.Drawing.Point(98, 69);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(17, 13);
            this.label94.TabIndex = 32;
            this.label94.Text = "X:";
            // 
            // Snapshot_button
            // 
            this.Snapshot_button.Location = new System.Drawing.Point(739, 784);
            this.Snapshot_button.Name = "Snapshot_button";
            this.Snapshot_button.Size = new System.Drawing.Size(75, 23);
            this.Snapshot_button.TabIndex = 30;
            this.Snapshot_button.Text = "Snapshot";
            this.Snapshot_button.UseVisualStyleBackColor = true;
            this.Snapshot_button.Visible = false;
            // 
            // ImageTest_checkBox
            // 
            this.ImageTest_checkBox.AutoSize = true;
            this.ImageTest_checkBox.Location = new System.Drawing.Point(739, 813);
            this.ImageTest_checkBox.Name = "ImageTest_checkBox";
            this.ImageTest_checkBox.Size = new System.Drawing.Size(75, 17);
            this.ImageTest_checkBox.TabIndex = 29;
            this.ImageTest_checkBox.Text = "For testing";
            this.ImageTest_checkBox.UseVisualStyleBackColor = true;
            this.ImageTest_checkBox.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(338, 412);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 134;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(253, 414);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 133;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // zoffset_textbox
            // 
            this.zoffset_textbox.Location = new System.Drawing.Point(35, 192);
            this.zoffset_textbox.Name = "zoffset_textbox";
            this.zoffset_textbox.Size = new System.Drawing.Size(36, 20);
            this.zoffset_textbox.TabIndex = 130;
            this.zoffset_textbox.Text = "0";
            this.toolTip1.SetToolTip(this.zoffset_textbox, "Nominal postion difference between\r\nthe needle tip and down camera image center.");
            this.zoffset_textbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.z_offset_textbox_keypress);
            // 
            // label130
            // 
            this.label130.AutoSize = true;
            this.label130.Location = new System.Drawing.Point(75, 194);
            this.label130.Name = "label130";
            this.label130.Size = new System.Drawing.Size(23, 13);
            this.label130.TabIndex = 132;
            this.label130.Text = "mm";
            // 
            // label131
            // 
            this.label131.AutoSize = true;
            this.label131.Location = new System.Drawing.Point(12, 192);
            this.label131.Name = "label131";
            this.label131.Size = new System.Drawing.Size(20, 13);
            this.label131.TabIndex = 131;
            this.label131.Text = "Z: ";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.label127);
            this.groupBox12.Controls.Add(this.fiducial_designator_regexp_textBox);
            this.groupBox12.Controls.Add(this.button_setTemplate);
            this.groupBox12.Controls.Add(this.label126);
            this.groupBox12.Controls.Add(this.fiducialTemlateMatch_textBox);
            this.groupBox12.Controls.Add(this.button3);
            this.groupBox12.Controls.Add(this.cb_useTemplate);
            this.groupBox12.Location = new System.Drawing.Point(3, 261);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(402, 114);
            this.groupBox12.TabIndex = 129;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Fiducial Settings";
            // 
            // label127
            // 
            this.label127.AutoSize = true;
            this.label127.Location = new System.Drawing.Point(109, 43);
            this.label127.Name = "label127";
            this.label127.Size = new System.Drawing.Size(65, 13);
            this.label127.TabIndex = 133;
            this.label127.Text = "Fid RegExp:";
            // 
            // fiducial_designator_regexp_textBox
            // 
            this.fiducial_designator_regexp_textBox.Location = new System.Drawing.Point(176, 39);
            this.fiducial_designator_regexp_textBox.Name = "fiducial_designator_regexp_textBox";
            this.fiducial_designator_regexp_textBox.Size = new System.Drawing.Size(63, 20);
            this.fiducial_designator_regexp_textBox.TabIndex = 132;
            this.fiducial_designator_regexp_textBox.Text = "^FID?";
            this.fiducial_designator_regexp_textBox.TextChanged += new System.EventHandler(this.fiducial_designator_regexp_textBox_TextChanged);
            // 
            // button_setTemplate
            // 
            this.button_setTemplate.Location = new System.Drawing.Point(98, 15);
            this.button_setTemplate.Name = "button_setTemplate";
            this.button_setTemplate.Size = new System.Drawing.Size(35, 23);
            this.button_setTemplate.TabIndex = 131;
            this.button_setTemplate.Text = "...";
            this.button_setTemplate.UseVisualStyleBackColor = true;
            this.button_setTemplate.Click += new System.EventHandler(this.button_setTemplate_Click);
            // 
            // label126
            // 
            this.label126.AutoSize = true;
            this.label126.Location = new System.Drawing.Point(7, 43);
            this.label126.Name = "label126";
            this.label126.Size = new System.Drawing.Size(60, 13);
            this.label126.TabIndex = 130;
            this.label126.Text = "Threshold :";
            // 
            // fiducialTemlateMatch_textBox
            // 
            this.fiducialTemlateMatch_textBox.Location = new System.Drawing.Point(70, 40);
            this.fiducialTemlateMatch_textBox.Name = "fiducialTemlateMatch_textBox";
            this.fiducialTemlateMatch_textBox.Size = new System.Drawing.Size(33, 20);
            this.fiducialTemlateMatch_textBox.TabIndex = 129;
            this.fiducialTemlateMatch_textBox.Text = "0.7";
            this.fiducialTemlateMatch_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fiducialTemlateMatch_textBox_KeyPress);
            this.fiducialTemlateMatch_textBox.Leave += new System.EventHandler(this.fiducialTemlateMatch_textBox_Leave);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(10, 66);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(150, 23);
            this.button3.TabIndex = 77;
            this.button3.Text = "Measure PCB Fiducials";
            this.toolTip1.SetToolTip(this.button3, "Re-measures PCB, convertign CAD data coordinates to \r\nmachine coordinates, based " +
        "on PCB fiducials.");
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.ReMeasure_button_Click);
            // 
            // cb_useTemplate
            // 
            this.cb_useTemplate.AutoSize = true;
            this.cb_useTemplate.Location = new System.Drawing.Point(6, 19);
            this.cb_useTemplate.Name = "cb_useTemplate";
            this.cb_useTemplate.Size = new System.Drawing.Size(92, 17);
            this.cb_useTemplate.TabIndex = 127;
            this.cb_useTemplate.Text = "Use Template";
            this.cb_useTemplate.UseVisualStyleBackColor = true;
            this.cb_useTemplate.CheckedChanged += new System.EventHandler(this.cb_useTemplate_CheckedChanged);
            // 
            // label129
            // 
            this.label129.AutoSize = true;
            this.label129.Location = new System.Drawing.Point(170, 131);
            this.label129.Name = "label129";
            this.label129.Size = new System.Drawing.Size(57, 26);
            this.label129.TabIndex = 146;
            this.label129.Text = "CalibMove\r\nDistance";
            // 
            // calibMoveDistance_textBox
            // 
            this.calibMoveDistance_textBox.Location = new System.Drawing.Point(173, 106);
            this.calibMoveDistance_textBox.Name = "calibMoveDistance_textBox";
            this.calibMoveDistance_textBox.Size = new System.Drawing.Size(46, 20);
            this.calibMoveDistance_textBox.TabIndex = 145;
            this.calibMoveDistance_textBox.Text = ".25";
            // 
            // SlackMeasurement_label
            // 
            this.SlackMeasurement_label.AutoSize = true;
            this.SlackMeasurement_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlackMeasurement_label.Location = new System.Drawing.Point(115, 215);
            this.SlackMeasurement_label.Name = "SlackMeasurement_label";
            this.SlackMeasurement_label.Size = new System.Drawing.Size(16, 13);
            this.SlackMeasurement_label.TabIndex = 142;
            this.SlackMeasurement_label.Text = "---";
            this.toolTip1.SetToolTip(this.SlackMeasurement_label, "Set the true size of the box on the image.");
            // 
            // button_camera_calibrate
            // 
            this.button_camera_calibrate.Location = new System.Drawing.Point(118, 103);
            this.button_camera_calibrate.Name = "button_camera_calibrate";
            this.button_camera_calibrate.Size = new System.Drawing.Size(46, 23);
            this.button_camera_calibrate.TabIndex = 140;
            this.button_camera_calibrate.Text = "Calib.";
            this.button_camera_calibrate.UseVisualStyleBackColor = true;
            // 
            // DownCameraBoxYmmPerPixel_label
            // 
            this.DownCameraBoxYmmPerPixel_label.AutoSize = true;
            this.DownCameraBoxYmmPerPixel_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCameraBoxYmmPerPixel_label.Location = new System.Drawing.Point(115, 192);
            this.DownCameraBoxYmmPerPixel_label.Name = "DownCameraBoxYmmPerPixel_label";
            this.DownCameraBoxYmmPerPixel_label.Size = new System.Drawing.Size(16, 13);
            this.DownCameraBoxYmmPerPixel_label.TabIndex = 27;
            this.DownCameraBoxYmmPerPixel_label.Text = "---";
            this.toolTip1.SetToolTip(this.DownCameraBoxYmmPerPixel_label, "Set the true size of the box on the image.");
            // 
            // DownCameraBoxXmmPerPixel_label
            // 
            this.DownCameraBoxXmmPerPixel_label.AutoSize = true;
            this.DownCameraBoxXmmPerPixel_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCameraBoxXmmPerPixel_label.Location = new System.Drawing.Point(115, 172);
            this.DownCameraBoxXmmPerPixel_label.Name = "DownCameraBoxXmmPerPixel_label";
            this.DownCameraBoxXmmPerPixel_label.Size = new System.Drawing.Size(16, 13);
            this.DownCameraBoxXmmPerPixel_label.TabIndex = 26;
            this.DownCameraBoxXmmPerPixel_label.Text = "---";
            this.toolTip1.SetToolTip(this.DownCameraBoxXmmPerPixel_label, "Set the true size of the box on the image.");
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label71.Location = new System.Drawing.Point(170, 74);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(23, 13);
            this.label71.TabIndex = 23;
            this.label71.Text = "mm";
            this.toolTip1.SetToolTip(this.label71, "Set the true size of the box on the image.");
            // 
            // DownCameraBoxX_textBox
            // 
            this.DownCameraBoxX_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCameraBoxX_textBox.Location = new System.Drawing.Point(118, 45);
            this.DownCameraBoxX_textBox.Name = "DownCameraBoxX_textBox";
            this.DownCameraBoxX_textBox.Size = new System.Drawing.Size(46, 20);
            this.DownCameraBoxX_textBox.TabIndex = 20;
            this.toolTip1.SetToolTip(this.DownCameraBoxX_textBox, "Set the true size of the box on the image.");
            this.DownCameraBoxX_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DownCameraBoxX_textBox_KeyPress);
            this.DownCameraBoxX_textBox.Leave += new System.EventHandler(this.DownCameraBoxX_textBox_Leave);
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label70.Location = new System.Drawing.Point(170, 48);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(23, 13);
            this.label70.TabIndex = 22;
            this.label70.Text = "mm";
            this.toolTip1.SetToolTip(this.label70, "Set the true size of the box on the image.");
            // 
            // DownCameraBoxY_textBox
            // 
            this.DownCameraBoxY_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCameraBoxY_textBox.Location = new System.Drawing.Point(118, 71);
            this.DownCameraBoxY_textBox.Name = "DownCameraBoxY_textBox";
            this.DownCameraBoxY_textBox.Size = new System.Drawing.Size(46, 20);
            this.DownCameraBoxY_textBox.TabIndex = 21;
            this.toolTip1.SetToolTip(this.DownCameraBoxY_textBox, "Set the true size of the box on the image.");
            this.DownCameraBoxY_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DownCameraBoxY_textBox_KeyPress);
            this.DownCameraBoxY_textBox.Leave += new System.EventHandler(this.DownCameraBoxY_textBox_Leave);
            // 
            // DownCamera_Calibration_button
            // 
            this.DownCamera_Calibration_button.Location = new System.Drawing.Point(53, 100);
            this.DownCamera_Calibration_button.Name = "DownCamera_Calibration_button";
            this.DownCamera_Calibration_button.Size = new System.Drawing.Size(46, 23);
            this.DownCamera_Calibration_button.TabIndex = 141;
            this.DownCamera_Calibration_button.Text = "Calib.";
            this.DownCamera_Calibration_button.UseVisualStyleBackColor = true;
            this.DownCamera_Calibration_button.Click += new System.EventHandler(this.DownCamera_Calibration_button_Click);
            // 
            // UpCameraBoxYmmPerPixel_label
            // 
            this.UpCameraBoxYmmPerPixel_label.AutoSize = true;
            this.UpCameraBoxYmmPerPixel_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpCameraBoxYmmPerPixel_label.Location = new System.Drawing.Point(11, 199);
            this.UpCameraBoxYmmPerPixel_label.Name = "UpCameraBoxYmmPerPixel_label";
            this.UpCameraBoxYmmPerPixel_label.Size = new System.Drawing.Size(16, 13);
            this.UpCameraBoxYmmPerPixel_label.TabIndex = 66;
            this.UpCameraBoxYmmPerPixel_label.Text = "---";
            this.toolTip1.SetToolTip(this.UpCameraBoxYmmPerPixel_label, "Set the true size of the box on the image.");
            // 
            // UpCameraBoxXmmPerPixel_label
            // 
            this.UpCameraBoxXmmPerPixel_label.AutoSize = true;
            this.UpCameraBoxXmmPerPixel_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpCameraBoxXmmPerPixel_label.Location = new System.Drawing.Point(11, 174);
            this.UpCameraBoxXmmPerPixel_label.Name = "UpCameraBoxXmmPerPixel_label";
            this.UpCameraBoxXmmPerPixel_label.Size = new System.Drawing.Size(16, 13);
            this.UpCameraBoxXmmPerPixel_label.TabIndex = 65;
            this.UpCameraBoxXmmPerPixel_label.Text = "---";
            this.toolTip1.SetToolTip(this.UpCameraBoxXmmPerPixel_label, "Set the true size of the box on the image.");
            // 
            // UpCameraBoxY_textBox
            // 
            this.UpCameraBoxY_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpCameraBoxY_textBox.Location = new System.Drawing.Point(53, 71);
            this.UpCameraBoxY_textBox.Name = "UpCameraBoxY_textBox";
            this.UpCameraBoxY_textBox.Size = new System.Drawing.Size(46, 20);
            this.UpCameraBoxY_textBox.TabIndex = 60;
            this.toolTip1.SetToolTip(this.UpCameraBoxY_textBox, "Set the true size of the box on the image.");
            this.UpCameraBoxY_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UpCameraBoxY_textBox_KeyPress);
            this.UpCameraBoxY_textBox.Leave += new System.EventHandler(this.UpCameraBoxY_textBox_Leave);
            // 
            // UpCameraBoxX_textBox
            // 
            this.UpCameraBoxX_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpCameraBoxX_textBox.Location = new System.Drawing.Point(53, 45);
            this.UpCameraBoxX_textBox.Name = "UpCameraBoxX_textBox";
            this.UpCameraBoxX_textBox.Size = new System.Drawing.Size(46, 20);
            this.UpCameraBoxX_textBox.TabIndex = 59;
            this.toolTip1.SetToolTip(this.UpCameraBoxX_textBox, "Set the true size of the box on the image.");
            this.UpCameraBoxX_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UpCameraBoxX_textBox_KeyPress);
            this.UpCameraBoxX_textBox.Leave += new System.EventHandler(this.UpCameraBoxX_textBox_Leave);
            // 
            // label106
            // 
            this.label106.AutoSize = true;
            this.label106.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label106.Location = new System.Drawing.Point(9, 48);
            this.label106.Name = "label106";
            this.label106.Size = new System.Drawing.Size(38, 13);
            this.label106.TabIndex = 57;
            this.label106.Text = "Box X:";
            this.toolTip1.SetToolTip(this.label106, "Set the true size of the box on the image.");
            // 
            // label105
            // 
            this.label105.AutoSize = true;
            this.label105.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label105.Location = new System.Drawing.Point(9, 74);
            this.label105.Name = "label105";
            this.label105.Size = new System.Drawing.Size(38, 13);
            this.label105.TabIndex = 58;
            this.label105.Text = "Box Y:";
            this.toolTip1.SetToolTip(this.label105, "Set the true size of the box on the image.");
            // 
            // NeedleOffsetY_textBox
            // 
            this.NeedleOffsetY_textBox.Location = new System.Drawing.Point(35, 167);
            this.NeedleOffsetY_textBox.Name = "NeedleOffsetY_textBox";
            this.NeedleOffsetY_textBox.Size = new System.Drawing.Size(36, 20);
            this.NeedleOffsetY_textBox.TabIndex = 86;
            this.NeedleOffsetY_textBox.Text = "6.99";
            this.toolTip1.SetToolTip(this.NeedleOffsetY_textBox, "Nominal postion difference between\r\nthe needle tip and down camera image center.");
            this.NeedleOffsetY_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NeedleOffsetY_textBox_KeyPress);
            this.NeedleOffsetY_textBox.Leave += new System.EventHandler(this.NeedleOffsetY_textBox_Leave);
            // 
            // NeedleOffsetX_textBox
            // 
            this.NeedleOffsetX_textBox.Location = new System.Drawing.Point(34, 144);
            this.NeedleOffsetX_textBox.Name = "NeedleOffsetX_textBox";
            this.NeedleOffsetX_textBox.Size = new System.Drawing.Size(37, 20);
            this.NeedleOffsetX_textBox.TabIndex = 85;
            this.NeedleOffsetX_textBox.Text = "42.88";
            this.toolTip1.SetToolTip(this.NeedleOffsetX_textBox, "Nominal postion difference between\r\nthe needle tip and down camera image center.");
            this.NeedleOffsetX_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NeedleOffsetX_textBox_KeyPress);
            this.NeedleOffsetX_textBox.Leave += new System.EventHandler(this.NeedleOffsetX_textBox_Leave);
            // 
            // label149
            // 
            this.label149.AutoSize = true;
            this.label149.Location = new System.Drawing.Point(75, 170);
            this.label149.Name = "label149";
            this.label149.Size = new System.Drawing.Size(23, 13);
            this.label149.TabIndex = 90;
            this.label149.Text = "mm";
            // 
            // label148
            // 
            this.label148.AutoSize = true;
            this.label148.Location = new System.Drawing.Point(75, 147);
            this.label148.Name = "label148";
            this.label148.Size = new System.Drawing.Size(23, 13);
            this.label148.TabIndex = 89;
            this.label148.Text = "mm";
            // 
            // label146
            // 
            this.label146.AutoSize = true;
            this.label146.Location = new System.Drawing.Point(12, 170);
            this.label146.Name = "label146";
            this.label146.Size = new System.Drawing.Size(17, 13);
            this.label146.TabIndex = 88;
            this.label146.Text = "Y:";
            // 
            // label143
            // 
            this.label143.AutoSize = true;
            this.label143.Location = new System.Drawing.Point(11, 146);
            this.label143.Name = "label143";
            this.label143.Size = new System.Drawing.Size(17, 13);
            this.label143.TabIndex = 87;
            this.label143.Text = "X:";
            // 
            // Offset2Method_button
            // 
            this.Offset2Method_button.Location = new System.Drawing.Point(9, 117);
            this.Offset2Method_button.Name = "Offset2Method_button";
            this.Offset2Method_button.Size = new System.Drawing.Size(108, 23);
            this.Offset2Method_button.TabIndex = 53;
            this.Offset2Method_button.Tag = "Runs the needle calibration routine";
            this.Offset2Method_button.Text = "Camera Offset";
            this.Offset2Method_button.UseVisualStyleBackColor = true;
            this.Offset2Method_button.Click += new System.EventHandler(this.Offset2Method_button_Click);
            // 
            // NeedleOffset_label
            // 
            this.NeedleOffset_label.AutoSize = true;
            this.NeedleOffset_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NeedleOffset_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NeedleOffset_label.Location = new System.Drawing.Point(3, 430);
            this.NeedleOffset_label.Name = "NeedleOffset_label";
            this.NeedleOffset_label.Size = new System.Drawing.Size(130, 22);
            this.NeedleOffset_label.TabIndex = 50;
            this.NeedleOffset_label.Text = "Instructions here";
            // 
            // label115
            // 
            this.label115.AutoSize = true;
            this.label115.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label115.Location = new System.Drawing.Point(264, 388);
            this.label115.Name = "label115";
            this.label115.Size = new System.Drawing.Size(56, 18);
            this.label115.TabIndex = 49;
            this.label115.Text = "Testing";
            // 
            // ZUp_button
            // 
            this.ZUp_button.Location = new System.Drawing.Point(227, 874);
            this.ZUp_button.Name = "ZUp_button";
            this.ZUp_button.Size = new System.Drawing.Size(84, 23);
            this.ZUp_button.TabIndex = 86;
            this.ZUp_button.Text = "Needle Up";
            this.toolTip1.SetToolTip(this.ZUp_button, "Takes needle up to Z0");
            this.ZUp_button.UseVisualStyleBackColor = true;
            this.ZUp_button.Click += new System.EventHandler(this.ZUp_button_Click);
            // 
            // ZDown_button
            // 
            this.ZDown_button.Location = new System.Drawing.Point(227, 902);
            this.ZDown_button.Name = "ZDown_button";
            this.ZDown_button.Size = new System.Drawing.Size(84, 23);
            this.ZDown_button.TabIndex = 85;
            this.ZDown_button.Text = "Needle Down";
            this.toolTip1.SetToolTip(this.ZDown_button, "Takes needle down to PCB level");
            this.ZDown_button.UseVisualStyleBackColor = true;
            this.ZDown_button.Click += new System.EventHandler(this.ZDown_button_Click);
            // 
            // tabPageBasicSetup
            // 
            this.tabPageBasicSetup.Controls.Add(this.groupBox6);
            this.tabPageBasicSetup.Controls.Add(this.tabControl1);
            this.tabPageBasicSetup.Controls.Add(this.VacuumRelease_textBox);
            this.tabPageBasicSetup.Controls.Add(this.label119);
            this.tabPageBasicSetup.Controls.Add(this.VacuumTime_textBox);
            this.tabPageBasicSetup.Controls.Add(this.label118);
            this.tabPageBasicSetup.Controls.Add(this.label90);
            this.tabPageBasicSetup.Controls.Add(this.SquareCorrection_textBox);
            this.tabPageBasicSetup.Controls.Add(this.SmallMovement_numericUpDown);
            this.tabPageBasicSetup.Controls.Add(this.label87);
            this.tabPageBasicSetup.Controls.Add(this.SlackCompensation_checkBox);
            this.tabPageBasicSetup.Controls.Add(this.SizeYMax_textBox);
            this.tabPageBasicSetup.Controls.Add(this.SizeXMax_textBox);
            this.tabPageBasicSetup.Controls.Add(this.ParkLocationY_textBox);
            this.tabPageBasicSetup.Controls.Add(this.ParkLocationX_textBox);
            this.tabPageBasicSetup.Controls.Add(this.label113);
            this.tabPageBasicSetup.Controls.Add(this.label102);
            this.tabPageBasicSetup.Controls.Add(this.label107);
            this.tabPageBasicSetup.Controls.Add(this.label92);
            this.tabPageBasicSetup.Controls.Add(this.label61);
            this.tabPageBasicSetup.Controls.Add(this.Homebutton);
            this.tabPageBasicSetup.Controls.Add(this.HomeZ_button);
            this.tabPageBasicSetup.Controls.Add(this.HomeY_button);
            this.tabPageBasicSetup.Controls.Add(this.SerialMonitor_richTextBox);
            this.tabPageBasicSetup.Controls.Add(this.HomeXY_button);
            this.tabPageBasicSetup.Controls.Add(this.HomeX_button);
            this.tabPageBasicSetup.Controls.Add(this.BuiltInSettings_button);
            this.tabPageBasicSetup.Controls.Add(this.SaveSettings_button);
            this.tabPageBasicSetup.Controls.Add(this.DefaultSettings_button);
            this.tabPageBasicSetup.Controls.Add(this.label4);
            this.tabPageBasicSetup.Controls.Add(this.buttonRefreshPortList);
            this.tabPageBasicSetup.Controls.Add(this.label2);
            this.tabPageBasicSetup.Controls.Add(this.comboBoxSerialPorts);
            this.tabPageBasicSetup.Controls.Add(this.textBoxSendtoTinyG);
            this.tabPageBasicSetup.Location = new System.Drawing.Point(4, 22);
            this.tabPageBasicSetup.Name = "tabPageBasicSetup";
            this.tabPageBasicSetup.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBasicSetup.Size = new System.Drawing.Size(821, 690);
            this.tabPageBasicSetup.TabIndex = 1;
            this.tabPageBasicSetup.Text = "Hardware Setup";
            this.tabPageBasicSetup.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.SetUpCamPosition_button);
            this.groupBox6.Controls.Add(this.label101);
            this.groupBox6.Controls.Add(this.label94);
            this.groupBox6.Controls.Add(this.label93);
            this.groupBox6.Controls.Add(this.label96);
            this.groupBox6.Controls.Add(this.label95);
            this.groupBox6.Controls.Add(this.label100);
            this.groupBox6.Controls.Add(this.SetPCB0_button);
            this.groupBox6.Controls.Add(this.SetPickupCenter_button);
            this.groupBox6.Controls.Add(this.label55);
            this.groupBox6.Controls.Add(this.JigX_textBox);
            this.groupBox6.Controls.Add(this.JigY_textBox);
            this.groupBox6.Controls.Add(this.PickupCenterX_textBox);
            this.groupBox6.Controls.Add(this.PickupCenterY_textBox);
            this.groupBox6.Controls.Add(this.UpcamPositionX_textBox);
            this.groupBox6.Controls.Add(this.label99);
            this.groupBox6.Controls.Add(this.UpcamPositionY_textBox);
            this.groupBox6.Controls.Add(this.label98);
            this.groupBox6.Location = new System.Drawing.Point(219, 452);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(259, 121);
            this.groupBox6.TabIndex = 90;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Special Locations";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabpage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(6, 17);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(268, 424);
            this.tabControl1.TabIndex = 89;
            // 
            // tabpage1
            // 
            this.tabpage1.Controls.Add(this.panel3);
            this.tabpage1.Controls.Add(this.TestX_button);
            this.tabpage1.Controls.Add(this.TestXY_button);
            this.tabpage1.Controls.Add(this.TestYX_button);
            this.tabpage1.Location = new System.Drawing.Point(4, 22);
            this.tabpage1.Name = "tabpage1";
            this.tabpage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabpage1.Size = new System.Drawing.Size(260, 398);
            this.tabpage1.TabIndex = 0;
            this.tabpage1.Text = "X";
            this.tabpage1.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label73);
            this.panel3.Controls.Add(this.xsv_maskedTextBox);
            this.panel3.Controls.Add(this.label74);
            this.panel3.Controls.Add(this.label75);
            this.panel3.Controls.Add(this.xjh_maskedTextBox);
            this.panel3.Controls.Add(this.label76);
            this.panel3.Controls.Add(this.Xmax_checkBox);
            this.panel3.Controls.Add(this.Xlim_checkBox);
            this.panel3.Controls.Add(this.Xhome_checkBox);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.label26);
            this.panel3.Controls.Add(this.xvm_maskedTextBox);
            this.panel3.Controls.Add(this.label27);
            this.panel3.Controls.Add(this.label28);
            this.panel3.Controls.Add(this.label29);
            this.panel3.Controls.Add(this.xjm_maskedTextBox);
            this.panel3.Controls.Add(this.label30);
            this.panel3.Location = new System.Drawing.Point(6, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(244, 350);
            this.panel3.TabIndex = 19;
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Location = new System.Drawing.Point(3, 189);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(78, 13);
            this.label73.TabIndex = 26;
            this.label73.Text = "Homing speed:";
            // 
            // xsv_maskedTextBox
            // 
            this.xsv_maskedTextBox.Location = new System.Drawing.Point(112, 186);
            this.xsv_maskedTextBox.Mask = "99999";
            this.xsv_maskedTextBox.Name = "xsv_maskedTextBox";
            this.xsv_maskedTextBox.PromptChar = ' ';
            this.xsv_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.xsv_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.xsv_maskedTextBox.TabIndex = 27;
            this.xsv_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.xsv_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.xsv_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.xsv_maskedTextBox_KeyPress);
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Location = new System.Drawing.Point(161, 189);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(44, 13);
            this.label74.TabIndex = 25;
            this.label74.Text = "mm/min";
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(4, 163);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(107, 13);
            this.label75.TabIndex = 23;
            this.label75.Text = "Homing acceleration:";
            // 
            // xjh_maskedTextBox
            // 
            this.xjh_maskedTextBox.Location = new System.Drawing.Point(112, 160);
            this.xjh_maskedTextBox.Mask = "99999";
            this.xjh_maskedTextBox.Name = "xjh_maskedTextBox";
            this.xjh_maskedTextBox.PromptChar = ' ';
            this.xjh_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.xjh_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.xjh_maskedTextBox.TabIndex = 24;
            this.xjh_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.xjh_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.xjh_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.xjh_maskedTextBox_KeyPress);
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(159, 163);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(80, 13);
            this.label76.TabIndex = 22;
            this.label76.Text = "10^6mm/min^3";
            // 
            // Xmax_checkBox
            // 
            this.Xmax_checkBox.AutoSize = true;
            this.Xmax_checkBox.Location = new System.Drawing.Point(7, 133);
            this.Xmax_checkBox.Name = "Xmax_checkBox";
            this.Xmax_checkBox.Size = new System.Drawing.Size(125, 17);
            this.Xmax_checkBox.TabIndex = 21;
            this.Xmax_checkBox.Text = "Max limit switch used";
            this.Xmax_checkBox.UseVisualStyleBackColor = true;
            this.Xmax_checkBox.Click += new System.EventHandler(this.Xmax_checkBox_Click);
            // 
            // Xlim_checkBox
            // 
            this.Xlim_checkBox.AutoSize = true;
            this.Xlim_checkBox.Location = new System.Drawing.Point(7, 110);
            this.Xlim_checkBox.Name = "Xlim_checkBox";
            this.Xlim_checkBox.Size = new System.Drawing.Size(122, 17);
            this.Xlim_checkBox.TabIndex = 20;
            this.Xlim_checkBox.Text = "Min limit switch used";
            this.Xlim_checkBox.UseVisualStyleBackColor = true;
            this.Xlim_checkBox.Click += new System.EventHandler(this.Xlim_checkBox_Click);
            // 
            // Xhome_checkBox
            // 
            this.Xhome_checkBox.AutoSize = true;
            this.Xhome_checkBox.Location = new System.Drawing.Point(7, 87);
            this.Xhome_checkBox.Name = "Xhome_checkBox";
            this.Xhome_checkBox.Size = new System.Drawing.Size(121, 17);
            this.Xhome_checkBox.TabIndex = 19;
            this.Xhome_checkBox.Text = "Homing switch used";
            this.Xhome_checkBox.UseVisualStyleBackColor = true;
            this.Xhome_checkBox.Click += new System.EventHandler(this.Xhome_checkBox_Click);
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.tr1_textBox);
            this.panel4.Controls.Add(this.m1deg18_radioButton);
            this.panel4.Controls.Add(this.m1deg09_radioButton);
            this.panel4.Controls.Add(this.label20);
            this.panel4.Controls.Add(this.label21);
            this.panel4.Controls.Add(this.label22);
            this.panel4.Controls.Add(this.label23);
            this.panel4.Controls.Add(this.mi1_maskedTextBox);
            this.panel4.Controls.Add(this.label24);
            this.panel4.Controls.Add(this.label25);
            this.panel4.Location = new System.Drawing.Point(3, 246);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(236, 99);
            this.panel4.TabIndex = 18;
            // 
            // tr1_textBox
            // 
            this.tr1_textBox.Location = new System.Drawing.Point(101, 63);
            this.tr1_textBox.Name = "tr1_textBox";
            this.tr1_textBox.Size = new System.Drawing.Size(54, 20);
            this.tr1_textBox.TabIndex = 19;
            this.tr1_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tr1_textBox_KeyPress);
            // 
            // m1deg18_radioButton
            // 
            this.m1deg18_radioButton.AutoSize = true;
            this.m1deg18_radioButton.Location = new System.Drawing.Point(153, 40);
            this.m1deg18_radioButton.Name = "m1deg18_radioButton";
            this.m1deg18_radioButton.Size = new System.Drawing.Size(64, 17);
            this.m1deg18_radioButton.TabIndex = 28;
            this.m1deg18_radioButton.TabStop = true;
            this.m1deg18_radioButton.Text = "1.8 deg.";
            this.m1deg18_radioButton.UseVisualStyleBackColor = true;
            this.m1deg18_radioButton.Click += new System.EventHandler(this.m1deg18_radioButton_Click);
            // 
            // m1deg09_radioButton
            // 
            this.m1deg09_radioButton.AutoSize = true;
            this.m1deg09_radioButton.Location = new System.Drawing.Point(91, 40);
            this.m1deg09_radioButton.Name = "m1deg09_radioButton";
            this.m1deg09_radioButton.Size = new System.Drawing.Size(64, 17);
            this.m1deg09_radioButton.TabIndex = 27;
            this.m1deg09_radioButton.TabStop = true;
            this.m1deg09_radioButton.Text = "0.9 deg.";
            this.m1deg09_radioButton.UseVisualStyleBackColor = true;
            this.m1deg09_radioButton.Click += new System.EventHandler(this.m1deg09_radioButton_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 68);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(100, 13);
            this.label20.TabIndex = 25;
            this.label20.Text = "Travel per rev. [1tr]:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(161, 68);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(23, 13);
            this.label21.TabIndex = 24;
            this.label21.Text = "mm";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 42);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(87, 13);
            this.label22.TabIndex = 22;
            this.label22.Text = "Step angle [1sa]:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 16);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(86, 13);
            this.label23.TabIndex = 19;
            this.label23.Text = "Microsteps [1mi]:";
            // 
            // mi1_maskedTextBox
            // 
            this.mi1_maskedTextBox.Location = new System.Drawing.Point(101, 14);
            this.mi1_maskedTextBox.Mask = "99999";
            this.mi1_maskedTextBox.Name = "mi1_maskedTextBox";
            this.mi1_maskedTextBox.PromptChar = ' ';
            this.mi1_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mi1_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.mi1_maskedTextBox.TabIndex = 20;
            this.mi1_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mi1_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mi1_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mi1_maskedTextBox_KeyPress);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(150, 16);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(55, 13);
            this.label24.TabIndex = 18;
            this.label24.Text = "[1, 2, 4, 8]";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(3, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(59, 16);
            this.label25.TabIndex = 15;
            this.label25.Text = "Motor1:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(4, 54);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(86, 13);
            this.label26.TabIndex = 16;
            this.label26.Text = "Speed [xvm, xfr]:";
            // 
            // xvm_maskedTextBox
            // 
            this.xvm_maskedTextBox.Location = new System.Drawing.Point(110, 51);
            this.xvm_maskedTextBox.Mask = "99999";
            this.xvm_maskedTextBox.Name = "xvm_maskedTextBox";
            this.xvm_maskedTextBox.PromptChar = ' ';
            this.xvm_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.xvm_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.xvm_maskedTextBox.TabIndex = 17;
            this.xvm_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.xvm_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.xvm_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.xvm_maskedTextBox_KeyPress);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(159, 54);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(65, 13);
            this.label27.TabIndex = 15;
            this.label27.Text = "000 mm/min";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(3, 4);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(26, 20);
            this.label28.TabIndex = 14;
            this.label28.Text = "X:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(3, 28);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(93, 13);
            this.label29.TabIndex = 12;
            this.label29.Text = "Acceleration [xjm]:";
            // 
            // xjm_maskedTextBox
            // 
            this.xjm_maskedTextBox.Location = new System.Drawing.Point(110, 25);
            this.xjm_maskedTextBox.Mask = "99999";
            this.xjm_maskedTextBox.Name = "xjm_maskedTextBox";
            this.xjm_maskedTextBox.PromptChar = ' ';
            this.xjm_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.xjm_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.xjm_maskedTextBox.TabIndex = 13;
            this.xjm_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.xjm_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.xjm_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.xjm_maskedTextBox_KeyPress);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(159, 28);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(80, 13);
            this.label30.TabIndex = 11;
            this.label30.Text = "10^6mm/min^3";
            // 
            // TestX_button
            // 
            this.TestX_button.Location = new System.Drawing.Point(6, 369);
            this.TestX_button.Name = "TestX_button";
            this.TestX_button.Size = new System.Drawing.Size(75, 23);
            this.TestX_button.TabIndex = 22;
            this.TestX_button.Text = "Test X";
            this.toolTip1.SetToolTip(this.TestX_button, "Makes some moves to test axis settings");
            this.TestX_button.UseVisualStyleBackColor = true;
            this.TestX_button.Click += new System.EventHandler(this.TestX_button_Click);
            // 
            // TestXY_button
            // 
            this.TestXY_button.Location = new System.Drawing.Point(87, 369);
            this.TestXY_button.Name = "TestXY_button";
            this.TestXY_button.Size = new System.Drawing.Size(75, 23);
            this.TestXY_button.TabIndex = 24;
            this.TestXY_button.Text = "Test XY";
            this.toolTip1.SetToolTip(this.TestXY_button, "Makes some moves to test axis settings");
            this.TestXY_button.UseVisualStyleBackColor = true;
            this.TestXY_button.Click += new System.EventHandler(this.TestXY_button_Click);
            // 
            // TestYX_button
            // 
            this.TestYX_button.Location = new System.Drawing.Point(169, 369);
            this.TestYX_button.Name = "TestYX_button";
            this.TestYX_button.Size = new System.Drawing.Size(75, 23);
            this.TestYX_button.TabIndex = 55;
            this.TestYX_button.Text = "Test YX";
            this.toolTip1.SetToolTip(this.TestYX_button, "Makes some moves to test axis settings");
            this.TestYX_button.UseVisualStyleBackColor = true;
            this.TestYX_button.Click += new System.EventHandler(this.TestYX_button_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Controls.Add(this.TestY_button);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(260, 398);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Y";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label77);
            this.panel1.Controls.Add(this.ysv_maskedTextBox);
            this.panel1.Controls.Add(this.label78);
            this.panel1.Controls.Add(this.label79);
            this.panel1.Controls.Add(this.yjh_maskedTextBox);
            this.panel1.Controls.Add(this.label80);
            this.panel1.Controls.Add(this.Ymax_checkBox);
            this.panel1.Controls.Add(this.Ylim_checkBox);
            this.panel1.Controls.Add(this.Yhome_checkBox);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.yvm_maskedTextBox);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.yjm_maskedTextBox);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(6, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(244, 350);
            this.panel1.TabIndex = 14;
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Location = new System.Drawing.Point(4, 189);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(78, 13);
            this.label77.TabIndex = 32;
            this.label77.Text = "Homing speed:";
            // 
            // ysv_maskedTextBox
            // 
            this.ysv_maskedTextBox.Location = new System.Drawing.Point(110, 186);
            this.ysv_maskedTextBox.Mask = "99999";
            this.ysv_maskedTextBox.Name = "ysv_maskedTextBox";
            this.ysv_maskedTextBox.PromptChar = ' ';
            this.ysv_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ysv_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.ysv_maskedTextBox.TabIndex = 33;
            this.ysv_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ysv_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.ysv_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ysv_maskedTextBox_KeyPress);
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(159, 189);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(44, 13);
            this.label78.TabIndex = 31;
            this.label78.Text = "mm/min";
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Location = new System.Drawing.Point(3, 163);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(107, 13);
            this.label79.TabIndex = 29;
            this.label79.Text = "Homing acceleration:";
            // 
            // yjh_maskedTextBox
            // 
            this.yjh_maskedTextBox.Location = new System.Drawing.Point(110, 160);
            this.yjh_maskedTextBox.Mask = "99999";
            this.yjh_maskedTextBox.Name = "yjh_maskedTextBox";
            this.yjh_maskedTextBox.PromptChar = ' ';
            this.yjh_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.yjh_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.yjh_maskedTextBox.TabIndex = 30;
            this.yjh_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.yjh_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.yjh_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.yjh_maskedTextBox_KeyPress);
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Location = new System.Drawing.Point(159, 163);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(80, 13);
            this.label80.TabIndex = 28;
            this.label80.Text = "10^6mm/min^3";
            // 
            // Ymax_checkBox
            // 
            this.Ymax_checkBox.AutoSize = true;
            this.Ymax_checkBox.Location = new System.Drawing.Point(7, 133);
            this.Ymax_checkBox.Name = "Ymax_checkBox";
            this.Ymax_checkBox.Size = new System.Drawing.Size(125, 17);
            this.Ymax_checkBox.TabIndex = 24;
            this.Ymax_checkBox.Text = "Max limit switch used";
            this.Ymax_checkBox.UseVisualStyleBackColor = true;
            this.Ymax_checkBox.Click += new System.EventHandler(this.Ymax_checkBox_Click);
            // 
            // Ylim_checkBox
            // 
            this.Ylim_checkBox.AutoSize = true;
            this.Ylim_checkBox.Location = new System.Drawing.Point(7, 110);
            this.Ylim_checkBox.Name = "Ylim_checkBox";
            this.Ylim_checkBox.Size = new System.Drawing.Size(122, 17);
            this.Ylim_checkBox.TabIndex = 23;
            this.Ylim_checkBox.Text = "Min limit switch used";
            this.Ylim_checkBox.UseVisualStyleBackColor = true;
            this.Ylim_checkBox.Click += new System.EventHandler(this.Ylim_checkBox_Click);
            // 
            // Yhome_checkBox
            // 
            this.Yhome_checkBox.AutoSize = true;
            this.Yhome_checkBox.Location = new System.Drawing.Point(7, 87);
            this.Yhome_checkBox.Name = "Yhome_checkBox";
            this.Yhome_checkBox.Size = new System.Drawing.Size(121, 17);
            this.Yhome_checkBox.TabIndex = 22;
            this.Yhome_checkBox.Text = "Homing switch used";
            this.Yhome_checkBox.UseVisualStyleBackColor = true;
            this.Yhome_checkBox.Click += new System.EventHandler(this.Yhome_checkBox_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.tr2_textBox);
            this.panel2.Controls.Add(this.m2deg18_radioButton);
            this.panel2.Controls.Add(this.m2deg09_radioButton);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.mi2_maskedTextBox);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Location = new System.Drawing.Point(3, 246);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(236, 99);
            this.panel2.TabIndex = 18;
            // 
            // tr2_textBox
            // 
            this.tr2_textBox.Location = new System.Drawing.Point(101, 63);
            this.tr2_textBox.Name = "tr2_textBox";
            this.tr2_textBox.Size = new System.Drawing.Size(54, 20);
            this.tr2_textBox.TabIndex = 29;
            this.tr2_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tr2_textBox_KeyPress);
            // 
            // m2deg18_radioButton
            // 
            this.m2deg18_radioButton.AutoSize = true;
            this.m2deg18_radioButton.Location = new System.Drawing.Point(153, 40);
            this.m2deg18_radioButton.Name = "m2deg18_radioButton";
            this.m2deg18_radioButton.Size = new System.Drawing.Size(64, 17);
            this.m2deg18_radioButton.TabIndex = 28;
            this.m2deg18_radioButton.TabStop = true;
            this.m2deg18_radioButton.Text = "1.8 deg.";
            this.m2deg18_radioButton.UseVisualStyleBackColor = true;
            this.m2deg18_radioButton.Click += new System.EventHandler(this.m2deg18_radioButton_Click);
            // 
            // m2deg09_radioButton
            // 
            this.m2deg09_radioButton.AutoSize = true;
            this.m2deg09_radioButton.Location = new System.Drawing.Point(91, 40);
            this.m2deg09_radioButton.Name = "m2deg09_radioButton";
            this.m2deg09_radioButton.Size = new System.Drawing.Size(64, 17);
            this.m2deg09_radioButton.TabIndex = 27;
            this.m2deg09_radioButton.TabStop = true;
            this.m2deg09_radioButton.Text = "0.9 deg.";
            this.m2deg09_radioButton.UseVisualStyleBackColor = true;
            this.m2deg09_radioButton.Click += new System.EventHandler(this.m2deg09_radioButton_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 68);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(100, 13);
            this.label15.TabIndex = 25;
            this.label15.Text = "Travel per rev. [2tr]:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(161, 68);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(23, 13);
            this.label16.TabIndex = 24;
            this.label16.Text = "mm";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 42);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(87, 13);
            this.label13.TabIndex = 22;
            this.label13.Text = "Step angle [2sa]:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(86, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Microsteps [2mi]:";
            // 
            // mi2_maskedTextBox
            // 
            this.mi2_maskedTextBox.Location = new System.Drawing.Point(101, 13);
            this.mi2_maskedTextBox.Mask = "99999";
            this.mi2_maskedTextBox.Name = "mi2_maskedTextBox";
            this.mi2_maskedTextBox.PromptChar = ' ';
            this.mi2_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mi2_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.mi2_maskedTextBox.TabIndex = 20;
            this.mi2_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mi2_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mi2_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mi2_maskedTextBox_KeyPress);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(150, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(55, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "[1, 2, 4, 8]";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 16);
            this.label10.TabIndex = 15;
            this.label10.Text = "Motor2:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(86, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Speed [yvm, yfr]:";
            // 
            // yvm_maskedTextBox
            // 
            this.yvm_maskedTextBox.Location = new System.Drawing.Point(110, 50);
            this.yvm_maskedTextBox.Mask = "99999";
            this.yvm_maskedTextBox.Name = "yvm_maskedTextBox";
            this.yvm_maskedTextBox.PromptChar = ' ';
            this.yvm_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.yvm_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.yvm_maskedTextBox.TabIndex = 17;
            this.yvm_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.yvm_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.yvm_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.yvm_maskedTextBox_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(159, 53);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "000 mm/min";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 20);
            this.label7.TabIndex = 14;
            this.label7.Text = "Y:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Acceleration [yjm]:";
            // 
            // yjm_maskedTextBox
            // 
            this.yjm_maskedTextBox.Location = new System.Drawing.Point(110, 25);
            this.yjm_maskedTextBox.Mask = "99999";
            this.yjm_maskedTextBox.Name = "yjm_maskedTextBox";
            this.yjm_maskedTextBox.PromptChar = ' ';
            this.yjm_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.yjm_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.yjm_maskedTextBox.TabIndex = 13;
            this.yjm_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.yjm_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.yjm_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.yjm_maskedTextBox_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(159, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "10^6mm/min^3";
            // 
            // TestY_button
            // 
            this.TestY_button.Location = new System.Drawing.Point(6, 368);
            this.TestY_button.Name = "TestY_button";
            this.TestY_button.Size = new System.Drawing.Size(75, 23);
            this.TestY_button.TabIndex = 23;
            this.TestY_button.Text = "Test Y";
            this.toolTip1.SetToolTip(this.TestY_button, "Makes some moves to test axis settings");
            this.TestY_button.UseVisualStyleBackColor = true;
            this.TestY_button.Click += new System.EventHandler(this.TestY_button_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label123);
            this.tabPage3.Controls.Add(this.panel5);
            this.tabPage3.Controls.Add(this.ZTestTravel_textBox);
            this.tabPage3.Controls.Add(this.TestZ_button);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(260, 398);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Z";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label123
            // 
            this.label123.AutoSize = true;
            this.label123.Location = new System.Drawing.Point(92, 365);
            this.label123.Name = "label123";
            this.label123.Size = new System.Drawing.Size(70, 13);
            this.label123.TabIndex = 83;
            this.label123.Text = "Z Test travel:";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.label81);
            this.panel5.Controls.Add(this.zsv_maskedTextBox);
            this.panel5.Controls.Add(this.label82);
            this.panel5.Controls.Add(this.label83);
            this.panel5.Controls.Add(this.zjh_maskedTextBox);
            this.panel5.Controls.Add(this.label84);
            this.panel5.Controls.Add(this.Zmax_checkBox);
            this.panel5.Controls.Add(this.Zlim_checkBox);
            this.panel5.Controls.Add(this.Zhome_checkBox);
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Controls.Add(this.label37);
            this.panel5.Controls.Add(this.zvm_maskedTextBox);
            this.panel5.Controls.Add(this.label38);
            this.panel5.Controls.Add(this.label39);
            this.panel5.Controls.Add(this.label40);
            this.panel5.Controls.Add(this.zjm_maskedTextBox);
            this.panel5.Controls.Add(this.label41);
            this.panel5.Location = new System.Drawing.Point(6, 6);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(244, 350);
            this.panel5.TabIndex = 20;
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Location = new System.Drawing.Point(3, 189);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(78, 13);
            this.label81.TabIndex = 32;
            this.label81.Text = "Homing speed:";
            // 
            // zsv_maskedTextBox
            // 
            this.zsv_maskedTextBox.Location = new System.Drawing.Point(110, 186);
            this.zsv_maskedTextBox.Mask = "99999";
            this.zsv_maskedTextBox.Name = "zsv_maskedTextBox";
            this.zsv_maskedTextBox.PromptChar = ' ';
            this.zsv_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.zsv_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.zsv_maskedTextBox.TabIndex = 33;
            this.zsv_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.zsv_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.zsv_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.zsv_maskedTextBox_KeyPress);
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.Location = new System.Drawing.Point(159, 189);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(44, 13);
            this.label82.TabIndex = 31;
            this.label82.Text = "mm/min";
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Location = new System.Drawing.Point(3, 163);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(107, 13);
            this.label83.TabIndex = 29;
            this.label83.Text = "Homing acceleration:";
            // 
            // zjh_maskedTextBox
            // 
            this.zjh_maskedTextBox.Location = new System.Drawing.Point(110, 160);
            this.zjh_maskedTextBox.Mask = "99999";
            this.zjh_maskedTextBox.Name = "zjh_maskedTextBox";
            this.zjh_maskedTextBox.PromptChar = ' ';
            this.zjh_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.zjh_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.zjh_maskedTextBox.TabIndex = 30;
            this.zjh_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.zjh_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.zjh_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.zjh_maskedTextBox_KeyPress);
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Location = new System.Drawing.Point(159, 163);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(80, 13);
            this.label84.TabIndex = 28;
            this.label84.Text = "10^6mm/min^3";
            // 
            // Zmax_checkBox
            // 
            this.Zmax_checkBox.AutoSize = true;
            this.Zmax_checkBox.Location = new System.Drawing.Point(7, 133);
            this.Zmax_checkBox.Name = "Zmax_checkBox";
            this.Zmax_checkBox.Size = new System.Drawing.Size(125, 17);
            this.Zmax_checkBox.TabIndex = 24;
            this.Zmax_checkBox.Text = "Max limit switch used";
            this.Zmax_checkBox.UseVisualStyleBackColor = true;
            this.Zmax_checkBox.Click += new System.EventHandler(this.Zmax_checkBox_Click);
            // 
            // Zlim_checkBox
            // 
            this.Zlim_checkBox.AutoSize = true;
            this.Zlim_checkBox.Location = new System.Drawing.Point(7, 110);
            this.Zlim_checkBox.Name = "Zlim_checkBox";
            this.Zlim_checkBox.Size = new System.Drawing.Size(122, 17);
            this.Zlim_checkBox.TabIndex = 23;
            this.Zlim_checkBox.Text = "Min limit switch used";
            this.Zlim_checkBox.UseVisualStyleBackColor = true;
            this.Zlim_checkBox.Click += new System.EventHandler(this.Zlim_checkBox_Click);
            // 
            // Zhome_checkBox
            // 
            this.Zhome_checkBox.AutoSize = true;
            this.Zhome_checkBox.Location = new System.Drawing.Point(7, 87);
            this.Zhome_checkBox.Name = "Zhome_checkBox";
            this.Zhome_checkBox.Size = new System.Drawing.Size(121, 17);
            this.Zhome_checkBox.TabIndex = 22;
            this.Zhome_checkBox.Text = "Homing switch used";
            this.Zhome_checkBox.UseVisualStyleBackColor = true;
            this.Zhome_checkBox.Click += new System.EventHandler(this.Zhome_checkBox_Click);
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.tr3_textBox);
            this.panel6.Controls.Add(this.m3deg18_radioButton);
            this.panel6.Controls.Add(this.m3deg09_radioButton);
            this.panel6.Controls.Add(this.label31);
            this.panel6.Controls.Add(this.label32);
            this.panel6.Controls.Add(this.label33);
            this.panel6.Controls.Add(this.mi3_maskedTextBox);
            this.panel6.Controls.Add(this.label34);
            this.panel6.Controls.Add(this.label35);
            this.panel6.Controls.Add(this.label36);
            this.panel6.Location = new System.Drawing.Point(3, 246);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(236, 99);
            this.panel6.TabIndex = 18;
            // 
            // tr3_textBox
            // 
            this.tr3_textBox.Location = new System.Drawing.Point(101, 65);
            this.tr3_textBox.Name = "tr3_textBox";
            this.tr3_textBox.Size = new System.Drawing.Size(54, 20);
            this.tr3_textBox.TabIndex = 21;
            this.tr3_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tr3_textBox_KeyPress);
            // 
            // m3deg18_radioButton
            // 
            this.m3deg18_radioButton.AutoSize = true;
            this.m3deg18_radioButton.Location = new System.Drawing.Point(153, 40);
            this.m3deg18_radioButton.Name = "m3deg18_radioButton";
            this.m3deg18_radioButton.Size = new System.Drawing.Size(64, 17);
            this.m3deg18_radioButton.TabIndex = 28;
            this.m3deg18_radioButton.TabStop = true;
            this.m3deg18_radioButton.Text = "1.8 deg.";
            this.m3deg18_radioButton.UseVisualStyleBackColor = true;
            this.m3deg18_radioButton.Click += new System.EventHandler(this.m3deg18_radioButton_Click);
            // 
            // m3deg09_radioButton
            // 
            this.m3deg09_radioButton.AutoSize = true;
            this.m3deg09_radioButton.Location = new System.Drawing.Point(91, 40);
            this.m3deg09_radioButton.Name = "m3deg09_radioButton";
            this.m3deg09_radioButton.Size = new System.Drawing.Size(64, 17);
            this.m3deg09_radioButton.TabIndex = 27;
            this.m3deg09_radioButton.TabStop = true;
            this.m3deg09_radioButton.Text = "0.9 deg.";
            this.m3deg09_radioButton.UseVisualStyleBackColor = true;
            this.m3deg09_radioButton.Click += new System.EventHandler(this.m3deg09_radioButton_Click);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(3, 68);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(100, 13);
            this.label31.TabIndex = 25;
            this.label31.Text = "Travel per rev. [3tr]:";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(161, 68);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(23, 13);
            this.label32.TabIndex = 24;
            this.label32.Text = "mm";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(3, 42);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(87, 13);
            this.label33.TabIndex = 22;
            this.label33.Text = "Step angle [3sa]:";
            // 
            // mi3_maskedTextBox
            // 
            this.mi3_maskedTextBox.Location = new System.Drawing.Point(101, 13);
            this.mi3_maskedTextBox.Mask = "99999";
            this.mi3_maskedTextBox.Name = "mi3_maskedTextBox";
            this.mi3_maskedTextBox.PromptChar = ' ';
            this.mi3_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mi3_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.mi3_maskedTextBox.TabIndex = 20;
            this.mi3_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mi3_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mi3_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mi3_maskedTextBox_KeyPress);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(3, 16);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(86, 13);
            this.label34.TabIndex = 19;
            this.label34.Text = "Microsteps [3mi]:";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(150, 16);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(55, 13);
            this.label35.TabIndex = 18;
            this.label35.Text = "[1, 2, 4, 8]";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.Location = new System.Drawing.Point(3, 0);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(59, 16);
            this.label36.TabIndex = 15;
            this.label36.Text = "Motor3:";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(3, 54);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(89, 13);
            this.label37.TabIndex = 16;
            this.label37.Text = "Speed [zvm, zvr]:";
            // 
            // zvm_maskedTextBox
            // 
            this.zvm_maskedTextBox.Location = new System.Drawing.Point(110, 50);
            this.zvm_maskedTextBox.Mask = "99999";
            this.zvm_maskedTextBox.Name = "zvm_maskedTextBox";
            this.zvm_maskedTextBox.PromptChar = ' ';
            this.zvm_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.zvm_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.zvm_maskedTextBox.TabIndex = 17;
            this.zvm_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.zvm_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.zvm_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.zvm_maskedTextBox_KeyPress);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(159, 54);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(44, 13);
            this.label38.TabIndex = 15;
            this.label38.Text = "mm/min";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(3, 4);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(25, 20);
            this.label39.TabIndex = 14;
            this.label39.Text = "Z:";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(3, 28);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(73, 13);
            this.label40.TabIndex = 12;
            this.label40.Text = "Acceler. [zjm]:";
            // 
            // zjm_maskedTextBox
            // 
            this.zjm_maskedTextBox.Location = new System.Drawing.Point(110, 25);
            this.zjm_maskedTextBox.Mask = "99999";
            this.zjm_maskedTextBox.Name = "zjm_maskedTextBox";
            this.zjm_maskedTextBox.PromptChar = ' ';
            this.zjm_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.zjm_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.zjm_maskedTextBox.TabIndex = 13;
            this.zjm_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.zjm_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.zjm_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.zjm_maskedTextBox_KeyPress);
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(159, 27);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(80, 13);
            this.label41.TabIndex = 11;
            this.label41.Text = "10^6mm/min^3";
            // 
            // ZTestTravel_textBox
            // 
            this.ZTestTravel_textBox.Location = new System.Drawing.Point(169, 362);
            this.ZTestTravel_textBox.Name = "ZTestTravel_textBox";
            this.ZTestTravel_textBox.Size = new System.Drawing.Size(75, 20);
            this.ZTestTravel_textBox.TabIndex = 82;
            this.ZTestTravel_textBox.TextChanged += new System.EventHandler(this.ZTestTravel_textBox_TextChanged);
            // 
            // TestZ_button
            // 
            this.TestZ_button.Location = new System.Drawing.Point(10, 362);
            this.TestZ_button.Name = "TestZ_button";
            this.TestZ_button.Size = new System.Drawing.Size(75, 23);
            this.TestZ_button.TabIndex = 34;
            this.TestZ_button.Text = "Test Z";
            this.toolTip1.SetToolTip(this.TestZ_button, "Makes some moves to test axis settings");
            this.TestZ_button.UseVisualStyleBackColor = true;
            this.TestZ_button.Click += new System.EventHandler(this.TestZ_button_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panel7);
            this.tabPage4.Controls.Add(this.TestA_button);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(260, 398);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "A";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.panel8);
            this.panel7.Controls.Add(this.label48);
            this.panel7.Controls.Add(this.avm_maskedTextBox);
            this.panel7.Controls.Add(this.label49);
            this.panel7.Controls.Add(this.label50);
            this.panel7.Controls.Add(this.label51);
            this.panel7.Controls.Add(this.ajm_maskedTextBox);
            this.panel7.Controls.Add(this.label52);
            this.panel7.Location = new System.Drawing.Point(6, 6);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(244, 350);
            this.panel7.TabIndex = 21;
            // 
            // panel8
            // 
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Controls.Add(this.tr4_textBox);
            this.panel8.Controls.Add(this.m4deg18_radioButton);
            this.panel8.Controls.Add(this.m4deg09_radioButton);
            this.panel8.Controls.Add(this.label42);
            this.panel8.Controls.Add(this.label43);
            this.panel8.Controls.Add(this.label44);
            this.panel8.Controls.Add(this.mi4_maskedTextBox);
            this.panel8.Controls.Add(this.label45);
            this.panel8.Controls.Add(this.label46);
            this.panel8.Controls.Add(this.label47);
            this.panel8.Location = new System.Drawing.Point(3, 246);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(236, 99);
            this.panel8.TabIndex = 18;
            // 
            // tr4_textBox
            // 
            this.tr4_textBox.Location = new System.Drawing.Point(101, 65);
            this.tr4_textBox.Name = "tr4_textBox";
            this.tr4_textBox.Size = new System.Drawing.Size(55, 20);
            this.tr4_textBox.TabIndex = 21;
            this.tr4_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tr4_textBox_KeyPress);
            // 
            // m4deg18_radioButton
            // 
            this.m4deg18_radioButton.AutoSize = true;
            this.m4deg18_radioButton.Location = new System.Drawing.Point(153, 40);
            this.m4deg18_radioButton.Name = "m4deg18_radioButton";
            this.m4deg18_radioButton.Size = new System.Drawing.Size(64, 17);
            this.m4deg18_radioButton.TabIndex = 28;
            this.m4deg18_radioButton.TabStop = true;
            this.m4deg18_radioButton.Text = "1.8 deg.";
            this.m4deg18_radioButton.UseVisualStyleBackColor = true;
            this.m4deg18_radioButton.Click += new System.EventHandler(this.m4deg18_radioButton_Click);
            // 
            // m4deg09_radioButton
            // 
            this.m4deg09_radioButton.AutoSize = true;
            this.m4deg09_radioButton.Location = new System.Drawing.Point(91, 40);
            this.m4deg09_radioButton.Name = "m4deg09_radioButton";
            this.m4deg09_radioButton.Size = new System.Drawing.Size(64, 17);
            this.m4deg09_radioButton.TabIndex = 27;
            this.m4deg09_radioButton.TabStop = true;
            this.m4deg09_radioButton.Text = "0.9 deg.";
            this.m4deg09_radioButton.UseVisualStyleBackColor = true;
            this.m4deg09_radioButton.Click += new System.EventHandler(this.m4deg09_radioButton_Click);
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(3, 68);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(100, 13);
            this.label42.TabIndex = 25;
            this.label42.Text = "Travel per rev. [4tr]:";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(162, 68);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(23, 13);
            this.label43.TabIndex = 24;
            this.label43.Text = "mm";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(3, 42);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(87, 13);
            this.label44.TabIndex = 22;
            this.label44.Text = "Step angle [4sa]:";
            // 
            // mi4_maskedTextBox
            // 
            this.mi4_maskedTextBox.Location = new System.Drawing.Point(101, 13);
            this.mi4_maskedTextBox.Mask = "99999";
            this.mi4_maskedTextBox.Name = "mi4_maskedTextBox";
            this.mi4_maskedTextBox.PromptChar = ' ';
            this.mi4_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mi4_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.mi4_maskedTextBox.TabIndex = 20;
            this.mi4_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mi4_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mi4_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mi4_maskedTextBox_KeyPress);
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(3, 16);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(86, 13);
            this.label45.TabIndex = 19;
            this.label45.Text = "Microsteps [4mi]:";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(150, 16);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(55, 13);
            this.label46.TabIndex = 18;
            this.label46.Text = "[1, 2, 4, 8]";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label47.Location = new System.Drawing.Point(3, 0);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(59, 16);
            this.label47.TabIndex = 15;
            this.label47.Text = "Motor4:";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(3, 53);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(91, 13);
            this.label48.TabIndex = 16;
            this.label48.Text = "Speed [avm, avr]:";
            // 
            // avm_maskedTextBox
            // 
            this.avm_maskedTextBox.Location = new System.Drawing.Point(105, 50);
            this.avm_maskedTextBox.Mask = "99999";
            this.avm_maskedTextBox.Name = "avm_maskedTextBox";
            this.avm_maskedTextBox.PromptChar = ' ';
            this.avm_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.avm_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.avm_maskedTextBox.TabIndex = 17;
            this.avm_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.avm_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.avm_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.avm_maskedTextBox_KeyPress);
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(154, 54);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(67, 13);
            this.label49.TabIndex = 15;
            this.label49.Text = "000 deg/min";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.Location = new System.Drawing.Point(3, 4);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(26, 20);
            this.label50.TabIndex = 14;
            this.label50.Text = "A:";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(3, 28);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(74, 13);
            this.label51.TabIndex = 12;
            this.label51.Text = "Acceler. [ajm]:";
            // 
            // ajm_maskedTextBox
            // 
            this.ajm_maskedTextBox.Location = new System.Drawing.Point(105, 25);
            this.ajm_maskedTextBox.Mask = "99999";
            this.ajm_maskedTextBox.Name = "ajm_maskedTextBox";
            this.ajm_maskedTextBox.PromptChar = ' ';
            this.ajm_maskedTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ajm_maskedTextBox.Size = new System.Drawing.Size(43, 20);
            this.ajm_maskedTextBox.TabIndex = 13;
            this.ajm_maskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ajm_maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.ajm_maskedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ajm_maskedTextBox_KeyPress);
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(151, 27);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(80, 13);
            this.label52.TabIndex = 11;
            this.label52.Text = "10^6mm/min^3";
            // 
            // TestA_button
            // 
            this.TestA_button.Location = new System.Drawing.Point(6, 362);
            this.TestA_button.Name = "TestA_button";
            this.TestA_button.Size = new System.Drawing.Size(75, 23);
            this.TestA_button.TabIndex = 35;
            this.TestA_button.Text = "Test A";
            this.toolTip1.SetToolTip(this.TestA_button, "Makes some moves to test axis settings");
            this.TestA_button.UseVisualStyleBackColor = true;
            this.TestA_button.Click += new System.EventHandler(this.TestA_button_Click);
            // 
            // VacuumRelease_textBox
            // 
            this.VacuumRelease_textBox.Location = new System.Drawing.Point(420, 382);
            this.VacuumRelease_textBox.Name = "VacuumRelease_textBox";
            this.VacuumRelease_textBox.Size = new System.Drawing.Size(58, 20);
            this.VacuumRelease_textBox.TabIndex = 81;
            this.toolTip1.SetToolTip(this.VacuumRelease_textBox, "If set to zero: For each mm of +Y movement, the \r\nmachine moves this much in X. S" +
        "et the value\r\nfor square movement.");
            this.VacuumRelease_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.VacuumRelease_textBox_KeyPress);
            this.VacuumRelease_textBox.Leave += new System.EventHandler(this.VacuumRelease_textBox_Leave);
            // 
            // label119
            // 
            this.label119.AutoSize = true;
            this.label119.Location = new System.Drawing.Point(280, 385);
            this.label119.Name = "label119";
            this.label119.Size = new System.Drawing.Size(130, 13);
            this.label119.TabIndex = 80;
            this.label119.Text = "Vacuum release time (ms):";
            // 
            // VacuumTime_textBox
            // 
            this.VacuumTime_textBox.Location = new System.Drawing.Point(420, 356);
            this.VacuumTime_textBox.Name = "VacuumTime_textBox";
            this.VacuumTime_textBox.Size = new System.Drawing.Size(58, 20);
            this.VacuumTime_textBox.TabIndex = 79;
            this.toolTip1.SetToolTip(this.VacuumTime_textBox, "If set to zero: For each mm of +Y movement, the \r\nmachine moves this much in X. S" +
        "et the value\r\nfor square movement.");
            this.VacuumTime_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.VacuumTime_textBox_KeyPress);
            this.VacuumTime_textBox.Leave += new System.EventHandler(this.VacuumTime_textBox_Leave);
            // 
            // label118
            // 
            this.label118.AutoSize = true;
            this.label118.Location = new System.Drawing.Point(280, 359);
            this.label118.Name = "label118";
            this.label118.Size = new System.Drawing.Size(128, 13);
            this.label118.TabIndex = 78;
            this.label118.Text = "Pickup vacuum time (ms):";
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(280, 324);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(116, 13);
            this.label90.TabIndex = 74;
            this.label90.Text = "Squareness correction:";
            // 
            // SquareCorrection_textBox
            // 
            this.SquareCorrection_textBox.Location = new System.Drawing.Point(402, 321);
            this.SquareCorrection_textBox.Name = "SquareCorrection_textBox";
            this.SquareCorrection_textBox.Size = new System.Drawing.Size(76, 20);
            this.SquareCorrection_textBox.TabIndex = 73;
            this.toolTip1.SetToolTip(this.SquareCorrection_textBox, "If set to zero: For each mm of +Y movement, the \r\nmachine moves this much in X. S" +
        "et the value\r\nfor square movement.");
            this.SquareCorrection_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SquareCorrection_textBox_KeyPress);
            this.SquareCorrection_textBox.Leave += new System.EventHandler(this.SquareCorrection_textBox_Leave);
            // 
            // SmallMovement_numericUpDown
            // 
            this.SmallMovement_numericUpDown.Location = new System.Drawing.Point(430, 295);
            this.SmallMovement_numericUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.SmallMovement_numericUpDown.Name = "SmallMovement_numericUpDown";
            this.SmallMovement_numericUpDown.Size = new System.Drawing.Size(48, 20);
            this.SmallMovement_numericUpDown.TabIndex = 72;
            this.toolTip1.SetToolTip(this.SmallMovement_numericUpDown, "To avoid jerkiness, small movements are done\r\nwith smaller speed. That speed is s" +
        "et here.");
            this.SmallMovement_numericUpDown.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.SmallMovement_numericUpDown.ValueChanged += new System.EventHandler(this.SmallMovement_numericUpDown_ValueChanged);
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Location = new System.Drawing.Point(323, 300);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(101, 13);
            this.label87.TabIndex = 71;
            this.label87.Text = "Small moves speed:";
            // 
            // SlackCompensation_checkBox
            // 
            this.SlackCompensation_checkBox.AutoSize = true;
            this.SlackCompensation_checkBox.Location = new System.Drawing.Point(355, 272);
            this.SlackCompensation_checkBox.Name = "SlackCompensation_checkBox";
            this.SlackCompensation_checkBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.SlackCompensation_checkBox.Size = new System.Drawing.Size(123, 17);
            this.SlackCompensation_checkBox.TabIndex = 57;
            this.SlackCompensation_checkBox.Text = "Slack Compensation";
            this.toolTip1.SetToolTip(this.SlackCompensation_checkBox, "All movements will go to position from same direction.\r\nIf there is slack in your" +
        " machine, the same side is loaded,\r\npresumably minimizing slack effects.");
            this.SlackCompensation_checkBox.UseVisualStyleBackColor = true;
            this.SlackCompensation_checkBox.Click += new System.EventHandler(this.SlackCompensation_checkBox_Click);
            // 
            // SizeYMax_textBox
            // 
            this.SizeYMax_textBox.Location = new System.Drawing.Point(402, 212);
            this.SizeYMax_textBox.Name = "SizeYMax_textBox";
            this.SizeYMax_textBox.Size = new System.Drawing.Size(76, 20);
            this.SizeYMax_textBox.TabIndex = 44;
            this.toolTip1.SetToolTip(this.SizeYMax_textBox, "Allows for custom machine size");
            this.SizeYMax_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SizeYMax_textBox_KeyPress);
            this.SizeYMax_textBox.Leave += new System.EventHandler(this.SizeYMax_textBox_Leave);
            // 
            // SizeXMax_textBox
            // 
            this.SizeXMax_textBox.Location = new System.Drawing.Point(402, 186);
            this.SizeXMax_textBox.Name = "SizeXMax_textBox";
            this.SizeXMax_textBox.Size = new System.Drawing.Size(76, 20);
            this.SizeXMax_textBox.TabIndex = 42;
            this.toolTip1.SetToolTip(this.SizeXMax_textBox, "Allows for custom machine size");
            this.SizeXMax_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SizeXMax_textBox_KeyPress);
            this.SizeXMax_textBox.Leave += new System.EventHandler(this.SizeXMax_textBox_Leave);
            // 
            // ParkLocationY_textBox
            // 
            this.ParkLocationY_textBox.Location = new System.Drawing.Point(402, 141);
            this.ParkLocationY_textBox.Name = "ParkLocationY_textBox";
            this.ParkLocationY_textBox.Size = new System.Drawing.Size(76, 20);
            this.ParkLocationY_textBox.TabIndex = 39;
            this.toolTip1.SetToolTip(this.ParkLocationY_textBox, "Define \"Park\" location, where machine\r\ngoes to make room for tape loadings etc.");
            this.ParkLocationY_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ParkLocationY_textBox_KeyPress);
            this.ParkLocationY_textBox.Leave += new System.EventHandler(this.ParkLocationY_textBox_Leave);
            // 
            // ParkLocationX_textBox
            // 
            this.ParkLocationX_textBox.Location = new System.Drawing.Point(402, 115);
            this.ParkLocationX_textBox.Name = "ParkLocationX_textBox";
            this.ParkLocationX_textBox.Size = new System.Drawing.Size(76, 20);
            this.ParkLocationX_textBox.TabIndex = 37;
            this.toolTip1.SetToolTip(this.ParkLocationX_textBox, "Define \"Park\" location, where machine\r\ngoes to make room for tape loadings etc.");
            this.ParkLocationX_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ParkLocationX_textBox_KeyPress);
            this.ParkLocationX_textBox.Leave += new System.EventHandler(this.ParkLocationX_textBox_Leave);
            // 
            // label113
            // 
            this.label113.AutoSize = true;
            this.label113.Location = new System.Drawing.Point(322, 172);
            this.label113.Name = "label113";
            this.label113.Size = new System.Drawing.Size(74, 13);
            this.label113.TabIndex = 48;
            this.label113.Text = "Machine Size:";
            // 
            // label102
            // 
            this.label102.AutoSize = true;
            this.label102.Location = new System.Drawing.Point(356, 215);
            this.label102.Name = "label102";
            this.label102.Size = new System.Drawing.Size(40, 13);
            this.label102.TabIndex = 45;
            this.label102.Text = "Max Y:";
            // 
            // label107
            // 
            this.label107.AutoSize = true;
            this.label107.Location = new System.Drawing.Point(356, 189);
            this.label107.Name = "label107";
            this.label107.Size = new System.Drawing.Size(40, 13);
            this.label107.TabIndex = 43;
            this.label107.Text = "Max X:";
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Location = new System.Drawing.Point(310, 144);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(86, 13);
            this.label92.TabIndex = 40;
            this.label92.Text = "Park Location Y:";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(310, 118);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(86, 13);
            this.label61.TabIndex = 38;
            this.label61.Text = "Park Location X:";
            // 
            // Homebutton
            // 
            this.Homebutton.Location = new System.Drawing.Point(89, 489);
            this.Homebutton.Name = "Homebutton";
            this.Homebutton.Size = new System.Drawing.Size(75, 23);
            this.Homebutton.TabIndex = 17;
            this.Homebutton.Text = "Home XYZ";
            this.toolTip1.SetToolTip(this.Homebutton, "Homes X, Y and Z axis, using limit switch only.");
            this.Homebutton.UseVisualStyleBackColor = true;
            this.Homebutton.Click += new System.EventHandler(this.Homebutton_Click);
            // 
            // HomeZ_button
            // 
            this.HomeZ_button.Location = new System.Drawing.Point(8, 503);
            this.HomeZ_button.Name = "HomeZ_button";
            this.HomeZ_button.Size = new System.Drawing.Size(75, 23);
            this.HomeZ_button.TabIndex = 33;
            this.HomeZ_button.Text = "Home Z";
            this.toolTip1.SetToolTip(this.HomeZ_button, "Homes Z axis, using limit switch only.");
            this.HomeZ_button.UseVisualStyleBackColor = true;
            this.HomeZ_button.Click += new System.EventHandler(this.HomeZ_button_Click);
            // 
            // HomeY_button
            // 
            this.HomeY_button.Location = new System.Drawing.Point(8, 474);
            this.HomeY_button.Name = "HomeY_button";
            this.HomeY_button.Size = new System.Drawing.Size(75, 23);
            this.HomeY_button.TabIndex = 32;
            this.HomeY_button.Text = "Home Y";
            this.toolTip1.SetToolTip(this.HomeY_button, "Homes Y axis, using limit switch only.");
            this.HomeY_button.UseVisualStyleBackColor = true;
            this.HomeY_button.Click += new System.EventHandler(this.HomeY_button_Click);
            // 
            // HomeXY_button
            // 
            this.HomeXY_button.Location = new System.Drawing.Point(89, 460);
            this.HomeXY_button.Name = "HomeXY_button";
            this.HomeXY_button.Size = new System.Drawing.Size(75, 23);
            this.HomeXY_button.TabIndex = 31;
            this.HomeXY_button.Text = "Home XY";
            this.toolTip1.SetToolTip(this.HomeXY_button, "Homes X and Y axis, using limit switch only.");
            this.HomeXY_button.UseVisualStyleBackColor = true;
            this.HomeXY_button.Click += new System.EventHandler(this.HomeXY_button_Click);
            // 
            // HomeX_button
            // 
            this.HomeX_button.Location = new System.Drawing.Point(8, 447);
            this.HomeX_button.Name = "HomeX_button";
            this.HomeX_button.Size = new System.Drawing.Size(75, 23);
            this.HomeX_button.TabIndex = 30;
            this.HomeX_button.Text = "Home X";
            this.toolTip1.SetToolTip(this.HomeX_button, "Homes X axis, using limit switch only.");
            this.HomeX_button.UseVisualStyleBackColor = true;
            this.HomeX_button.Click += new System.EventHandler(this.HomeX_button_Click);
            // 
            // BuiltInSettings_button
            // 
            this.BuiltInSettings_button.Location = new System.Drawing.Point(283, 12);
            this.BuiltInSettings_button.Name = "BuiltInSettings_button";
            this.BuiltInSettings_button.Size = new System.Drawing.Size(210, 23);
            this.BuiltInSettings_button.TabIndex = 29;
            this.BuiltInSettings_button.Text = "Reset all to Built-In Defaults";
            this.toolTip1.SetToolTip(this.BuiltInSettings_button, "Resets all settings to conservative defualt settings.\r\nALL YOUR SETTINGS MODIFICA" +
        "TIONS WILL BE LOST!");
            this.BuiltInSettings_button.UseVisualStyleBackColor = true;
            this.BuiltInSettings_button.Click += new System.EventHandler(this.BuiltInSettings_button_Click);
            // 
            // SaveSettings_button
            // 
            this.SaveSettings_button.Location = new System.Drawing.Point(283, 68);
            this.SaveSettings_button.Name = "SaveSettings_button";
            this.SaveSettings_button.Size = new System.Drawing.Size(210, 23);
            this.SaveSettings_button.TabIndex = 28;
            this.SaveSettings_button.Text = "Save Current Settings to User Defaults";
            this.toolTip1.SetToolTip(this.SaveSettings_button, "Saves these settings, so they can be retrieved by\r\nthe above button.");
            this.SaveSettings_button.UseVisualStyleBackColor = true;
            this.SaveSettings_button.Click += new System.EventHandler(this.SaveSettings_button_Click);
            // 
            // DefaultSettings_button
            // 
            this.DefaultSettings_button.Location = new System.Drawing.Point(283, 41);
            this.DefaultSettings_button.Name = "DefaultSettings_button";
            this.DefaultSettings_button.Size = new System.Drawing.Size(210, 21);
            this.DefaultSettings_button.TabIndex = 27;
            this.DefaultSettings_button.Text = "Load User Defaults to TinyG";
            this.toolTip1.SetToolTip(this.DefaultSettings_button, "Loads your saved settings back to system.");
            this.DefaultSettings_button.UseVisualStyleBackColor = true;
            this.DefaultSettings_button.Click += new System.EventHandler(this.DefaultSettings_button_Click);
            // 
            // buttonRefreshPortList
            // 
            this.buttonRefreshPortList.Location = new System.Drawing.Point(420, 415);
            this.buttonRefreshPortList.Name = "buttonRefreshPortList";
            this.buttonRefreshPortList.Size = new System.Drawing.Size(58, 23);
            this.buttonRefreshPortList.TabIndex = 4;
            this.buttonRefreshPortList.Text = "Refresh";
            this.toolTip1.SetToolTip(this.buttonRefreshPortList, "Re-scans the serial ports on this computer");
            this.buttonRefreshPortList.UseVisualStyleBackColor = true;
            this.buttonRefreshPortList.Click += new System.EventHandler(this.buttonRefreshPortList_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(277, 418);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Serial Port:";
            // 
            // comboBoxSerialPorts
            // 
            this.comboBoxSerialPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerialPorts.FormattingEnabled = true;
            this.comboBoxSerialPorts.Location = new System.Drawing.Point(339, 415);
            this.comboBoxSerialPorts.Name = "comboBoxSerialPorts";
            this.comboBoxSerialPorts.Size = new System.Drawing.Size(75, 21);
            this.comboBoxSerialPorts.TabIndex = 0;
            this.toolTip1.SetToolTip(this.comboBoxSerialPorts, "Serial port used by TinyG");
            // 
            // labelSerialPortStatus
            // 
            this.labelSerialPortStatus.AutoSize = true;
            this.labelSerialPortStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerialPortStatus.Location = new System.Drawing.Point(344, 766);
            this.labelSerialPortStatus.Name = "labelSerialPortStatus";
            this.labelSerialPortStatus.Size = new System.Drawing.Size(105, 18);
            this.labelSerialPortStatus.TabIndex = 3;
            this.labelSerialPortStatus.Text = "Not connected";
            this.toolTip1.SetToolTip(this.labelSerialPortStatus, "Connection status");
            // 
            // Z_Backoff_label
            // 
            this.Z_Backoff_label.AutoSize = true;
            this.Z_Backoff_label.Location = new System.Drawing.Point(76, 100);
            this.Z_Backoff_label.Name = "Z_Backoff_label";
            this.Z_Backoff_label.Size = new System.Drawing.Size(47, 13);
            this.Z_Backoff_label.TabIndex = 77;
            this.Z_Backoff_label.Text = "3.00 mm";
            // 
            // label117
            // 
            this.label117.AutoSize = true;
            this.label117.Location = new System.Drawing.Point(11, 99);
            this.label117.Name = "label117";
            this.label117.Size = new System.Drawing.Size(47, 13);
            this.label117.TabIndex = 76;
            this.label117.Text = "Backoff:";
            // 
            // Z0toPCB_BasicTab_label
            // 
            this.Z0toPCB_BasicTab_label.AutoSize = true;
            this.Z0toPCB_BasicTab_label.Location = new System.Drawing.Point(76, 83);
            this.Z0toPCB_BasicTab_label.Name = "Z0toPCB_BasicTab_label";
            this.Z0toPCB_BasicTab_label.Size = new System.Drawing.Size(53, 13);
            this.Z0toPCB_BasicTab_label.TabIndex = 53;
            this.Z0toPCB_BasicTab_label.Text = "37.00 mm";
            // 
            // label111
            // 
            this.label111.AutoSize = true;
            this.label111.Location = new System.Drawing.Point(11, 83);
            this.label111.Name = "label111";
            this.label111.Size = new System.Drawing.Size(59, 13);
            this.label111.TabIndex = 52;
            this.label111.Text = "Z0 to PCB:";
            // 
            // Zlb_label
            // 
            this.Zlb_label.AutoSize = true;
            this.Zlb_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Zlb_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Zlb_label.Location = new System.Drawing.Point(3, 388);
            this.Zlb_label.Name = "Zlb_label";
            this.Zlb_label.Size = new System.Drawing.Size(153, 38);
            this.Zlb_label.TabIndex = 51;
            this.Zlb_label.Text = "Probing instructions...\r\nLine 2";
            this.Zlb_label.Visible = false;
            // 
            // SetProbing_button
            // 
            this.SetProbing_button.Location = new System.Drawing.Point(9, 52);
            this.SetProbing_button.Name = "SetProbing_button";
            this.SetProbing_button.Size = new System.Drawing.Size(108, 23);
            this.SetProbing_button.TabIndex = 50;
            this.SetProbing_button.Text = "Height";
            this.toolTip1.SetToolTip(this.SetProbing_button, "Runs needle height calibration routine");
            this.SetProbing_button.UseVisualStyleBackColor = true;
            this.SetProbing_button.Click += new System.EventHandler(this.SetProbing_button_Click);
            // 
            // buttonConnectSerial
            // 
            this.buttonConnectSerial.Location = new System.Drawing.Point(453, 764);
            this.buttonConnectSerial.Name = "buttonConnectSerial";
            this.buttonConnectSerial.Size = new System.Drawing.Size(84, 23);
            this.buttonConnectSerial.TabIndex = 2;
            this.buttonConnectSerial.Text = "Connect";
            this.toolTip1.SetToolTip(this.buttonConnectSerial, "Try to connect to TinyG at port shown above");
            this.buttonConnectSerial.UseVisualStyleBackColor = true;
            this.buttonConnectSerial.Click += new System.EventHandler(this.buttonConnectSerial_Click);
            // 
            // MotorPower_checkBox
            // 
            this.MotorPower_checkBox.AutoSize = true;
            this.MotorPower_checkBox.Location = new System.Drawing.Point(410, 878);
            this.MotorPower_checkBox.Name = "MotorPower_checkBox";
            this.MotorPower_checkBox.Size = new System.Drawing.Size(86, 17);
            this.MotorPower_checkBox.TabIndex = 54;
            this.MotorPower_checkBox.Text = "Motor Power";
            this.toolTip1.SetToolTip(this.MotorPower_checkBox, "Motor power on/off \r\n(Motor power on holds machine position)");
            this.MotorPower_checkBox.UseVisualStyleBackColor = true;
            this.MotorPower_checkBox.Click += new System.EventHandler(this.MotorPower_checkBox_Click);
            // 
            // Vacuum_checkBox
            // 
            this.Vacuum_checkBox.AutoSize = true;
            this.Vacuum_checkBox.Location = new System.Drawing.Point(410, 913);
            this.Vacuum_checkBox.Name = "Vacuum_checkBox";
            this.Vacuum_checkBox.Size = new System.Drawing.Size(82, 17);
            this.Vacuum_checkBox.TabIndex = 26;
            this.Vacuum_checkBox.Text = "Vacuum On";
            this.toolTip1.SetToolTip(this.Vacuum_checkBox, "Valve control, vacuum on needle on/off");
            this.Vacuum_checkBox.UseVisualStyleBackColor = true;
            this.Vacuum_checkBox.Click += new System.EventHandler(this.Vacuum_checkBox_Click);
            // 
            // Pump_checkBox
            // 
            this.Pump_checkBox.AutoSize = true;
            this.Pump_checkBox.Location = new System.Drawing.Point(410, 896);
            this.Pump_checkBox.Name = "Pump_checkBox";
            this.Pump_checkBox.Size = new System.Drawing.Size(70, 17);
            this.Pump_checkBox.TabIndex = 25;
            this.Pump_checkBox.Text = "Pump On";
            this.toolTip1.SetToolTip(this.Pump_checkBox, "Vacuum pump on/off");
            this.Pump_checkBox.UseVisualStyleBackColor = true;
            this.Pump_checkBox.Click += new System.EventHandler(this.Pump_checkBox_Click);
            // 
            // RunJob_tabPage
            // 
            this.RunJob_tabPage.Controls.Add(this.AbortPlacement_button);
            this.RunJob_tabPage.Controls.Add(this.needle_calibration_test_button);
            this.RunJob_tabPage.Controls.Add(this.PausePlacement_button);
            this.RunJob_tabPage.Controls.Add(this.ChangeNeedle_button);
            this.RunJob_tabPage.Controls.Add(this.ValidMeasurement_checkBox);
            this.RunJob_tabPage.Controls.Add(this.ReMeasure_button);
            this.RunJob_tabPage.Controls.Add(this.groupBox2);
            this.RunJob_tabPage.Controls.Add(this.groupBox3);
            this.RunJob_tabPage.Controls.Add(this.JobOffsetY_textBox);
            this.RunJob_tabPage.Controls.Add(this.JobOffsetX_textBox);
            this.RunJob_tabPage.Controls.Add(this.groupBox1);
            this.RunJob_tabPage.Controls.Add(this.label89);
            this.RunJob_tabPage.Controls.Add(this.label88);
            this.RunJob_tabPage.Controls.Add(this.TestNeedleRecognition_button);
            this.RunJob_tabPage.Controls.Add(this.label86);
            this.RunJob_tabPage.Controls.Add(this.label85);
            this.RunJob_tabPage.Controls.Add(this.JobData_GridView);
            this.RunJob_tabPage.Controls.Add(this.Bottom_checkBox);
            this.RunJob_tabPage.Controls.Add(this.CadData_GridView);
            this.RunJob_tabPage.Location = new System.Drawing.Point(4, 22);
            this.RunJob_tabPage.Name = "RunJob_tabPage";
            this.RunJob_tabPage.Padding = new System.Windows.Forms.Padding(3);
            this.RunJob_tabPage.Size = new System.Drawing.Size(821, 690);
            this.RunJob_tabPage.TabIndex = 2;
            this.RunJob_tabPage.Text = "Run Job";
            this.RunJob_tabPage.UseVisualStyleBackColor = true;
            // 
            // AbortPlacement_button
            // 
            this.AbortPlacement_button.Location = new System.Drawing.Point(15, 5);
            this.AbortPlacement_button.Name = "AbortPlacement_button";
            this.AbortPlacement_button.Size = new System.Drawing.Size(109, 23);
            this.AbortPlacement_button.TabIndex = 36;
            this.AbortPlacement_button.Text = "Stop";
            this.toolTip1.SetToolTip(this.AbortPlacement_button, "Aborts the whole operation.");
            this.AbortPlacement_button.UseVisualStyleBackColor = true;
            this.AbortPlacement_button.Click += new System.EventHandler(this.AbortPlacement_button_Click);
            // 
            // needle_calibration_test_button
            // 
            this.needle_calibration_test_button.Location = new System.Drawing.Point(16, 123);
            this.needle_calibration_test_button.Name = "needle_calibration_test_button";
            this.needle_calibration_test_button.Size = new System.Drawing.Size(110, 23);
            this.needle_calibration_test_button.TabIndex = 76;
            this.needle_calibration_test_button.Text = "Needle Cal. Test";
            this.toolTip1.SetToolTip(this.needle_calibration_test_button, "Will see if the corrects to the needle are correct");
            this.needle_calibration_test_button.UseVisualStyleBackColor = true;
            this.needle_calibration_test_button.Click += new System.EventHandler(this.needle_calibration_test_button_Click);
            // 
            // PausePlacement_button
            // 
            this.PausePlacement_button.Location = new System.Drawing.Point(15, 34);
            this.PausePlacement_button.Name = "PausePlacement_button";
            this.PausePlacement_button.Size = new System.Drawing.Size(109, 23);
            this.PausePlacement_button.TabIndex = 35;
            this.PausePlacement_button.Text = "Pause";
            this.toolTip1.SetToolTip(this.PausePlacement_button, "Temporary pause");
            this.PausePlacement_button.UseVisualStyleBackColor = true;
            this.PausePlacement_button.Click += new System.EventHandler(this.PausePlacement_button_Click);
            // 
            // ChangeNeedle_button
            // 
            this.ChangeNeedle_button.Location = new System.Drawing.Point(16, 152);
            this.ChangeNeedle_button.Name = "ChangeNeedle_button";
            this.ChangeNeedle_button.Size = new System.Drawing.Size(110, 23);
            this.ChangeNeedle_button.TabIndex = 72;
            this.ChangeNeedle_button.Text = "Change Needle";
            this.ChangeNeedle_button.UseVisualStyleBackColor = true;
            this.ChangeNeedle_button.Click += new System.EventHandler(this.ChangeNeedle_button_Click);
            // 
            // ValidMeasurement_checkBox
            // 
            this.ValidMeasurement_checkBox.AutoSize = true;
            this.ValidMeasurement_checkBox.Location = new System.Drawing.Point(677, 268);
            this.ValidMeasurement_checkBox.Name = "ValidMeasurement_checkBox";
            this.ValidMeasurement_checkBox.Size = new System.Drawing.Size(138, 17);
            this.ValidMeasurement_checkBox.TabIndex = 71;
            this.ValidMeasurement_checkBox.Text = "Measurements are valid";
            this.ValidMeasurement_checkBox.UseVisualStyleBackColor = true;
            // 
            // ReMeasure_button
            // 
            this.ReMeasure_button.Location = new System.Drawing.Point(16, 68);
            this.ReMeasure_button.Name = "ReMeasure_button";
            this.ReMeasure_button.Size = new System.Drawing.Size(110, 23);
            this.ReMeasure_button.TabIndex = 48;
            this.ReMeasure_button.Text = "Re-measure";
            this.toolTip1.SetToolTip(this.ReMeasure_button, "Re-measures PCB, convertign CAD data coordinates to \r\nmachine coordinates, based " +
        "on PCB fiducials.");
            this.ReMeasure_button.UseVisualStyleBackColor = true;
            this.ReMeasure_button.Click += new System.EventHandler(this.ReMeasure_button_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ResetAllTapes_button);
            this.groupBox2.Controls.Add(this.ResetOneTape_button);
            this.groupBox2.Controls.Add(this.NewRow_button);
            this.groupBox2.Controls.Add(this.PlaceThese_button);
            this.groupBox2.Controls.Add(this.DeleteComponentGroup_button);
            this.groupBox2.Controls.Add(this.Down_button);
            this.groupBox2.Controls.Add(this.Up_button);
            this.groupBox2.Controls.Add(this.PlaceAll_button);
            this.groupBox2.Location = new System.Drawing.Point(10, 329);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(122, 353);
            this.groupBox2.TabIndex = 51;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Row Operations";
            // 
            // ResetAllTapes_button
            // 
            this.ResetAllTapes_button.Location = new System.Drawing.Point(6, 266);
            this.ResetAllTapes_button.Name = "ResetAllTapes_button";
            this.ResetAllTapes_button.Size = new System.Drawing.Size(109, 23);
            this.ResetAllTapes_button.TabIndex = 37;
            this.ResetAllTapes_button.Text = "Reset All Tapes";
            this.toolTip1.SetToolTip(this.ResetAllTapes_button, "Reset all tape loacations to 1");
            this.ResetAllTapes_button.UseVisualStyleBackColor = true;
            this.ResetAllTapes_button.Click += new System.EventHandler(this.ResetAllTapes_button_Click);
            // 
            // ResetOneTape_button
            // 
            this.ResetOneTape_button.Location = new System.Drawing.Point(5, 148);
            this.ResetOneTape_button.Name = "ResetOneTape_button";
            this.ResetOneTape_button.Size = new System.Drawing.Size(109, 23);
            this.ResetOneTape_button.TabIndex = 25;
            this.ResetOneTape_button.Text = "Reset Tape(s)";
            this.toolTip1.SetToolTip(this.ResetOneTape_button, "Resets the  tape pickup locations for \r\nthe components on selected rows to 1.");
            this.ResetOneTape_button.UseVisualStyleBackColor = true;
            this.ResetOneTape_button.Click += new System.EventHandler(this.ResetOneTape_button_Click);
            // 
            // NewRow_button
            // 
            this.NewRow_button.Location = new System.Drawing.Point(6, 90);
            this.NewRow_button.Name = "NewRow_button";
            this.NewRow_button.Size = new System.Drawing.Size(110, 23);
            this.NewRow_button.TabIndex = 22;
            this.NewRow_button.Text = "Add Row";
            this.toolTip1.SetToolTip(this.NewRow_button, "Adds a new row");
            this.NewRow_button.UseVisualStyleBackColor = true;
            this.NewRow_button.Click += new System.EventHandler(this.NewRow_button_Click);
            // 
            // PlaceThese_button
            // 
            this.PlaceThese_button.Location = new System.Drawing.Point(7, 324);
            this.PlaceThese_button.Name = "PlaceThese_button";
            this.PlaceThese_button.Size = new System.Drawing.Size(110, 23);
            this.PlaceThese_button.TabIndex = 17;
            this.PlaceThese_button.Text = "Place";
            this.toolTip1.SetToolTip(this.PlaceThese_button, "Does the \"Method\" operation on the components on\r\nthe selected rows.");
            this.PlaceThese_button.UseVisualStyleBackColor = true;
            this.PlaceThese_button.Click += new System.EventHandler(this.PlaceThese_button_Click);
            // 
            // DeleteComponentGroup_button
            // 
            this.DeleteComponentGroup_button.Location = new System.Drawing.Point(6, 119);
            this.DeleteComponentGroup_button.Name = "DeleteComponentGroup_button";
            this.DeleteComponentGroup_button.Size = new System.Drawing.Size(110, 23);
            this.DeleteComponentGroup_button.TabIndex = 16;
            this.DeleteComponentGroup_button.Text = "Delete Row(s)";
            this.toolTip1.SetToolTip(this.DeleteComponentGroup_button, "Deletes selected rows");
            this.DeleteComponentGroup_button.UseVisualStyleBackColor = true;
            this.DeleteComponentGroup_button.Click += new System.EventHandler(this.DeleteComponentGroup_button_Click);
            // 
            // Down_button
            // 
            this.Down_button.Location = new System.Drawing.Point(6, 61);
            this.Down_button.Name = "Down_button";
            this.Down_button.Size = new System.Drawing.Size(110, 23);
            this.Down_button.TabIndex = 15;
            this.Down_button.Text = "Move Down";
            this.toolTip1.SetToolTip(this.Down_button, "Moves selected row down");
            this.Down_button.UseVisualStyleBackColor = true;
            this.Down_button.Click += new System.EventHandler(this.Down_button_Click);
            // 
            // Up_button
            // 
            this.Up_button.Location = new System.Drawing.Point(6, 32);
            this.Up_button.Name = "Up_button";
            this.Up_button.Size = new System.Drawing.Size(110, 23);
            this.Up_button.TabIndex = 14;
            this.Up_button.Text = "Move Up";
            this.toolTip1.SetToolTip(this.Up_button, "Moves selected row up");
            this.Up_button.UseVisualStyleBackColor = true;
            this.Up_button.Click += new System.EventHandler(this.Up_button_Click);
            // 
            // PlaceAll_button
            // 
            this.PlaceAll_button.Location = new System.Drawing.Point(8, 295);
            this.PlaceAll_button.Name = "PlaceAll_button";
            this.PlaceAll_button.Size = new System.Drawing.Size(109, 23);
            this.PlaceAll_button.TabIndex = 20;
            this.PlaceAll_button.Text = "Place All";
            this.toolTip1.SetToolTip(this.PlaceAll_button, "Places all components on the Job Data table.");
            this.PlaceAll_button.UseVisualStyleBackColor = true;
            this.PlaceAll_button.Click += new System.EventHandler(this.PlaceAll_button_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.PlaceOne_button);
            this.groupBox3.Controls.Add(this.ShowMachine_button);
            this.groupBox3.Controls.Add(this.ShowNominal_button);
            this.groupBox3.Location = new System.Drawing.Point(10, 191);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(122, 118);
            this.groupBox3.TabIndex = 56;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Component Operations";
            // 
            // PlaceOne_button
            // 
            this.PlaceOne_button.Location = new System.Drawing.Point(5, 87);
            this.PlaceOne_button.Name = "PlaceOne_button";
            this.PlaceOne_button.Size = new System.Drawing.Size(110, 23);
            this.PlaceOne_button.TabIndex = 49;
            this.PlaceOne_button.Text = "Place";
            this.PlaceOne_button.UseVisualStyleBackColor = true;
            this.PlaceOne_button.Click += new System.EventHandler(this.PlaceOne_button_Click);
            // 
            // ShowMachine_button
            // 
            this.ShowMachine_button.Location = new System.Drawing.Point(5, 29);
            this.ShowMachine_button.Name = "ShowMachine_button";
            this.ShowMachine_button.Size = new System.Drawing.Size(110, 23);
            this.ShowMachine_button.TabIndex = 46;
            this.ShowMachine_button.Text = "Measured location";
            this.toolTip1.SetToolTip(this.ShowMachine_button, "Shows selected component measured location");
            this.ShowMachine_button.UseVisualStyleBackColor = true;
            this.ShowMachine_button.Click += new System.EventHandler(this.ShowMachine_button_Click);
            // 
            // ShowNominal_button
            // 
            this.ShowNominal_button.Location = new System.Drawing.Point(5, 58);
            this.ShowNominal_button.Name = "ShowNominal_button";
            this.ShowNominal_button.Size = new System.Drawing.Size(110, 23);
            this.ShowNominal_button.TabIndex = 45;
            this.ShowNominal_button.Text = "Nominal location";
            this.toolTip1.SetToolTip(this.ShowNominal_button, "Shows selected component location based on PCB zero and \r\nCAD data coordinates on" +
        "ly (no optical measurements).");
            this.ShowNominal_button.UseVisualStyleBackColor = true;
            this.ShowNominal_button.Click += new System.EventHandler(this.ShowNominal_button_Click);
            // 
            // JobOffsetY_textBox
            // 
            this.JobOffsetY_textBox.Location = new System.Drawing.Point(769, 6);
            this.JobOffsetY_textBox.Name = "JobOffsetY_textBox";
            this.JobOffsetY_textBox.Size = new System.Drawing.Size(43, 20);
            this.JobOffsetY_textBox.TabIndex = 39;
            this.JobOffsetY_textBox.Text = "0.0";
            this.JobOffsetY_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.JobOffsetY_textBox_KeyPress);
            this.JobOffsetY_textBox.Leave += new System.EventHandler(this.JobOffsetY_textBox_Leave);
            // 
            // JobOffsetX_textBox
            // 
            this.JobOffsetX_textBox.Location = new System.Drawing.Point(633, 4);
            this.JobOffsetX_textBox.Name = "JobOffsetX_textBox";
            this.JobOffsetX_textBox.Size = new System.Drawing.Size(43, 20);
            this.JobOffsetX_textBox.TabIndex = 37;
            this.JobOffsetX_textBox.Text = "0.0";
            this.JobOffsetX_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.JobOffsetX_textBox_KeyPress);
            this.JobOffsetX_textBox.Leave += new System.EventHandler(this.JobOffsetX_textBox_Leave);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.MachineCoords_label);
            this.groupBox1.Controls.Add(this.PlacedValue_label);
            this.groupBox1.Controls.Add(this.PlacedRotation_label);
            this.groupBox1.Controls.Add(this.PlacedY_label);
            this.groupBox1.Controls.Add(this.PlacedX_label);
            this.groupBox1.Controls.Add(this.PlacedComponent_label);
            this.groupBox1.Controls.Add(this.label66);
            this.groupBox1.Controls.Add(this.label65);
            this.groupBox1.Controls.Add(this.label64);
            this.groupBox1.Controls.Add(this.label63);
            this.groupBox1.Controls.Add(this.label58);
            this.groupBox1.Location = new System.Drawing.Point(625, 300);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 144);
            this.groupBox1.TabIndex = 50;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Placement Operations";
            // 
            // MachineCoords_label
            // 
            this.MachineCoords_label.AutoSize = true;
            this.MachineCoords_label.Location = new System.Drawing.Point(6, 121);
            this.MachineCoords_label.Name = "MachineCoords_label";
            this.MachineCoords_label.Size = new System.Drawing.Size(13, 13);
            this.MachineCoords_label.TabIndex = 34;
            this.MachineCoords_label.Text = "--";
            // 
            // PlacedValue_label
            // 
            this.PlacedValue_label.AutoSize = true;
            this.PlacedValue_label.Location = new System.Drawing.Point(95, 37);
            this.PlacedValue_label.Name = "PlacedValue_label";
            this.PlacedValue_label.Size = new System.Drawing.Size(13, 13);
            this.PlacedValue_label.TabIndex = 33;
            this.PlacedValue_label.Text = "--";
            // 
            // PlacedRotation_label
            // 
            this.PlacedRotation_label.AutoSize = true;
            this.PlacedRotation_label.Location = new System.Drawing.Point(95, 100);
            this.PlacedRotation_label.Name = "PlacedRotation_label";
            this.PlacedRotation_label.Size = new System.Drawing.Size(13, 13);
            this.PlacedRotation_label.TabIndex = 32;
            this.PlacedRotation_label.Text = "--";
            // 
            // PlacedY_label
            // 
            this.PlacedY_label.AutoSize = true;
            this.PlacedY_label.Location = new System.Drawing.Point(95, 79);
            this.PlacedY_label.Name = "PlacedY_label";
            this.PlacedY_label.Size = new System.Drawing.Size(13, 13);
            this.PlacedY_label.TabIndex = 31;
            this.PlacedY_label.Text = "--";
            // 
            // PlacedX_label
            // 
            this.PlacedX_label.AutoSize = true;
            this.PlacedX_label.Location = new System.Drawing.Point(95, 58);
            this.PlacedX_label.Name = "PlacedX_label";
            this.PlacedX_label.Size = new System.Drawing.Size(13, 13);
            this.PlacedX_label.TabIndex = 30;
            this.PlacedX_label.Text = "--";
            // 
            // PlacedComponent_label
            // 
            this.PlacedComponent_label.AutoSize = true;
            this.PlacedComponent_label.Location = new System.Drawing.Point(95, 16);
            this.PlacedComponent_label.Name = "PlacedComponent_label";
            this.PlacedComponent_label.Size = new System.Drawing.Size(13, 13);
            this.PlacedComponent_label.TabIndex = 29;
            this.PlacedComponent_label.Text = "--";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(6, 37);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(86, 13);
            this.label66.TabIndex = 28;
            this.label66.Text = "Value | Footprint:";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(6, 100);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(50, 13);
            this.label65.TabIndex = 27;
            this.label65.Text = "Rotation:";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(6, 79);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(62, 13);
            this.label64.TabIndex = 26;
            this.label64.Text = "Y (nominal):";
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(6, 58);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(62, 13);
            this.label63.TabIndex = 25;
            this.label63.Text = "X (nominal):";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(6, 16);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(69, 13);
            this.label58.TabIndex = 23;
            this.label58.Text = "Now placing:";
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label89.Location = new System.Drawing.Point(138, 277);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(78, 20);
            this.label89.TabIndex = 44;
            this.label89.Text = "Job Data:";
            this.toolTip1.SetToolTip(this.label89, "The placement operations are done according\r\nto Job Data specifications.");
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label88.Location = new System.Drawing.Point(138, 11);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(86, 20);
            this.label88.TabIndex = 43;
            this.label88.Text = "CAD Data:";
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(687, 12);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(68, 13);
            this.label86.TabIndex = 40;
            this.label86.Text = "Job Offset Y:";
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Location = new System.Drawing.Point(551, 10);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(68, 13);
            this.label85.TabIndex = 38;
            this.label85.Text = "Job Offset X:";
            // 
            // JobData_GridView
            // 
            this.JobData_GridView.AllowUserToAddRows = false;
            this.JobData_GridView.AutoGenerateColumns = false;
            this.JobData_GridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.JobData_GridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.JobData_GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.JobData_GridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.countDataGridViewTextBoxColumn,
            this.componentListDataGridViewTextBoxColumn,
            this.componentTypeDataGridViewTextBoxColumn,
            this.methodDataGridViewTextBoxColumn1,
            this.methodParametersDataGridViewTextBoxColumn});
            this.JobData_GridView.DataSource = this.jobDataBindingSource;
            this.JobData_GridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.JobData_GridView.Location = new System.Drawing.Point(138, 300);
            this.JobData_GridView.Name = "JobData_GridView";
            this.JobData_GridView.RowHeadersVisible = false;
            this.JobData_GridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.JobData_GridView.Size = new System.Drawing.Size(481, 382);
            this.JobData_GridView.TabIndex = 11;
            this.JobData_GridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.JobData_GridView_CellClick);
            // 
            // countDataGridViewTextBoxColumn
            // 
            this.countDataGridViewTextBoxColumn.DataPropertyName = "Count";
            this.countDataGridViewTextBoxColumn.HeaderText = "Count";
            this.countDataGridViewTextBoxColumn.Name = "countDataGridViewTextBoxColumn";
            this.countDataGridViewTextBoxColumn.ReadOnly = true;
            this.countDataGridViewTextBoxColumn.Width = 45;
            // 
            // componentListDataGridViewTextBoxColumn
            // 
            this.componentListDataGridViewTextBoxColumn.DataPropertyName = "ComponentList";
            this.componentListDataGridViewTextBoxColumn.HeaderText = "ComponentList";
            this.componentListDataGridViewTextBoxColumn.Name = "componentListDataGridViewTextBoxColumn";
            this.componentListDataGridViewTextBoxColumn.ReadOnly = true;
            this.componentListDataGridViewTextBoxColumn.Width = 150;
            // 
            // componentTypeDataGridViewTextBoxColumn
            // 
            this.componentTypeDataGridViewTextBoxColumn.DataPropertyName = "ComponentType";
            this.componentTypeDataGridViewTextBoxColumn.HeaderText = "ComponentType";
            this.componentTypeDataGridViewTextBoxColumn.Name = "componentTypeDataGridViewTextBoxColumn";
            this.componentTypeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // methodDataGridViewTextBoxColumn1
            // 
            this.methodDataGridViewTextBoxColumn1.DataPropertyName = "Method";
            this.methodDataGridViewTextBoxColumn1.HeaderText = "Method";
            this.methodDataGridViewTextBoxColumn1.Name = "methodDataGridViewTextBoxColumn1";
            this.methodDataGridViewTextBoxColumn1.Width = 75;
            // 
            // methodParametersDataGridViewTextBoxColumn
            // 
            this.methodParametersDataGridViewTextBoxColumn.DataPropertyName = "MethodParameters";
            this.methodParametersDataGridViewTextBoxColumn.HeaderText = "MethodParameters";
            this.methodParametersDataGridViewTextBoxColumn.Name = "methodParametersDataGridViewTextBoxColumn";
            // 
            // jobDataBindingSource
            // 
            this.jobDataBindingSource.DataSource = typeof(LitePlacer.JobData);
            // 
            // Bottom_checkBox
            // 
            this.Bottom_checkBox.AutoSize = true;
            this.Bottom_checkBox.Location = new System.Drawing.Point(612, 268);
            this.Bottom_checkBox.Name = "Bottom_checkBox";
            this.Bottom_checkBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Bottom_checkBox.Size = new System.Drawing.Size(59, 17);
            this.Bottom_checkBox.TabIndex = 8;
            this.Bottom_checkBox.Text = "Bottom";
            this.Bottom_checkBox.UseVisualStyleBackColor = true;
            // 
            // CadData_GridView
            // 
            this.CadData_GridView.AllowUserToAddRows = false;
            this.CadData_GridView.AllowUserToDeleteRows = false;
            this.CadData_GridView.AllowUserToResizeRows = false;
            this.CadData_GridView.AutoGenerateColumns = false;
            this.CadData_GridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.CadData_GridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.CadData_GridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.CadData_GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CadData_GridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.designatorDataGridViewTextBoxColumn,
            this.footprintDataGridViewTextBoxColumn,
            this.xnominalDataGridViewTextBoxColumn,
            this.ynominalDataGridViewTextBoxColumn,
            this.rotationDataGridViewTextBoxColumn,
            this.xmachineDataGridViewTextBoxColumn,
            this.ymachineDataGridViewTextBoxColumn,
            this.rotationmachineDataGridViewTextBoxColumn,
            this.methodDataGridViewTextBoxColumn,
            this.isFiducialDataGridViewCheckBoxColumn});
            this.CadData_GridView.DataSource = this.physicalComponentBindingSource;
            this.CadData_GridView.Location = new System.Drawing.Point(138, 34);
            this.CadData_GridView.Name = "CadData_GridView";
            this.CadData_GridView.RowHeadersVisible = false;
            this.CadData_GridView.RowHeadersWidth = 16;
            this.CadData_GridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.CadData_GridView.Size = new System.Drawing.Size(677, 228);
            this.CadData_GridView.TabIndex = 5;
            // 
            // designatorDataGridViewTextBoxColumn
            // 
            this.designatorDataGridViewTextBoxColumn.DataPropertyName = "Designator";
            this.designatorDataGridViewTextBoxColumn.HeaderText = "Designator";
            this.designatorDataGridViewTextBoxColumn.Name = "designatorDataGridViewTextBoxColumn";
            // 
            // footprintDataGridViewTextBoxColumn
            // 
            this.footprintDataGridViewTextBoxColumn.DataPropertyName = "Footprint";
            this.footprintDataGridViewTextBoxColumn.HeaderText = "Footprint";
            this.footprintDataGridViewTextBoxColumn.Name = "footprintDataGridViewTextBoxColumn";
            // 
            // xnominalDataGridViewTextBoxColumn
            // 
            this.xnominalDataGridViewTextBoxColumn.DataPropertyName = "X_nominal";
            this.xnominalDataGridViewTextBoxColumn.HeaderText = "X_nominal";
            this.xnominalDataGridViewTextBoxColumn.Name = "xnominalDataGridViewTextBoxColumn";
            // 
            // ynominalDataGridViewTextBoxColumn
            // 
            this.ynominalDataGridViewTextBoxColumn.DataPropertyName = "Y_nominal";
            this.ynominalDataGridViewTextBoxColumn.HeaderText = "Y_nominal";
            this.ynominalDataGridViewTextBoxColumn.Name = "ynominalDataGridViewTextBoxColumn";
            // 
            // rotationDataGridViewTextBoxColumn
            // 
            this.rotationDataGridViewTextBoxColumn.DataPropertyName = "Rotation";
            this.rotationDataGridViewTextBoxColumn.HeaderText = "Rotation";
            this.rotationDataGridViewTextBoxColumn.Name = "rotationDataGridViewTextBoxColumn";
            // 
            // xmachineDataGridViewTextBoxColumn
            // 
            this.xmachineDataGridViewTextBoxColumn.DataPropertyName = "X_machine";
            this.xmachineDataGridViewTextBoxColumn.HeaderText = "X_machine";
            this.xmachineDataGridViewTextBoxColumn.Name = "xmachineDataGridViewTextBoxColumn";
            // 
            // ymachineDataGridViewTextBoxColumn
            // 
            this.ymachineDataGridViewTextBoxColumn.DataPropertyName = "Y_machine";
            this.ymachineDataGridViewTextBoxColumn.HeaderText = "Y_machine";
            this.ymachineDataGridViewTextBoxColumn.Name = "ymachineDataGridViewTextBoxColumn";
            // 
            // rotationmachineDataGridViewTextBoxColumn
            // 
            this.rotationmachineDataGridViewTextBoxColumn.DataPropertyName = "Rotation_machine";
            this.rotationmachineDataGridViewTextBoxColumn.HeaderText = "Rotation_machine";
            this.rotationmachineDataGridViewTextBoxColumn.Name = "rotationmachineDataGridViewTextBoxColumn";
            // 
            // methodDataGridViewTextBoxColumn
            // 
            this.methodDataGridViewTextBoxColumn.DataPropertyName = "Method";
            this.methodDataGridViewTextBoxColumn.HeaderText = "Method";
            this.methodDataGridViewTextBoxColumn.Name = "methodDataGridViewTextBoxColumn";
            this.methodDataGridViewTextBoxColumn.Visible = false;
            // 
            // isFiducialDataGridViewCheckBoxColumn
            // 
            this.isFiducialDataGridViewCheckBoxColumn.DataPropertyName = "IsFiducial";
            this.isFiducialDataGridViewCheckBoxColumn.HeaderText = "IsFiducial";
            this.isFiducialDataGridViewCheckBoxColumn.Name = "isFiducialDataGridViewCheckBoxColumn";
            this.isFiducialDataGridViewCheckBoxColumn.ReadOnly = true;
            this.isFiducialDataGridViewCheckBoxColumn.Visible = false;
            // 
            // physicalComponentBindingSource
            // 
            this.physicalComponentBindingSource.DataSource = typeof(LitePlacer.PhysicalComponent);
            // 
            // MultiCalibrate_button
            // 
            this.MultiCalibrate_button.BackColor = System.Drawing.Color.Crimson;
            this.MultiCalibrate_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MultiCalibrate_button.Location = new System.Drawing.Point(677, 876);
            this.MultiCalibrate_button.Name = "MultiCalibrate_button";
            this.MultiCalibrate_button.Size = new System.Drawing.Size(129, 49);
            this.MultiCalibrate_button.TabIndex = 75;
            this.MultiCalibrate_button.Text = " Multi Calibration";
            this.toolTip1.SetToolTip(this.MultiCalibrate_button, "This will home, calibrate needle, and calibrate pcb (remeasure).");
            this.MultiCalibrate_button.UseVisualStyleBackColor = false;
            this.MultiCalibrate_button.Click += new System.EventHandler(this.MultiCalibrate_button_Click);
            // 
            // StopDemo_button
            // 
            this.StopDemo_button.Location = new System.Drawing.Point(657, 818);
            this.StopDemo_button.Name = "StopDemo_button";
            this.StopDemo_button.Size = new System.Drawing.Size(75, 23);
            this.StopDemo_button.TabIndex = 74;
            this.StopDemo_button.Text = "Stop Demo";
            this.StopDemo_button.UseVisualStyleBackColor = true;
            this.StopDemo_button.Visible = false;
            // 
            // Demo_button
            // 
            this.Demo_button.Location = new System.Drawing.Point(657, 784);
            this.Demo_button.Name = "Demo_button";
            this.Demo_button.Size = new System.Drawing.Size(75, 23);
            this.Demo_button.TabIndex = 73;
            this.Demo_button.Text = "Start Demo";
            this.Demo_button.UseVisualStyleBackColor = true;
            this.Demo_button.Visible = false;
            // 
            // tabControlPages
            // 
            this.tabControlPages.Controls.Add(this.RunJob_tabPage);
            this.tabControlPages.Controls.Add(this.tabPageBasicSetup);
            this.tabControlPages.Controls.Add(this.Tapes_tabPage);
            this.tabControlPages.Controls.Add(this.tabPage5);
            this.tabControlPages.Location = new System.Drawing.Point(6, 27);
            this.tabControlPages.Name = "tabControlPages";
            this.tabControlPages.SelectedIndex = 0;
            this.tabControlPages.Size = new System.Drawing.Size(829, 716);
            this.tabControlPages.TabIndex = 3;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox7);
            this.tabPage5.Controls.Add(this.groupBox12);
            this.tabPage5.Controls.Add(this.groupBox4);
            this.tabPage5.Controls.Add(this.button2);
            this.tabPage5.Controls.Add(this.Zlb_label);
            this.tabPage5.Controls.Add(this.button1);
            this.tabPage5.Controls.Add(this.label115);
            this.tabPage5.Controls.Add(this.NeedleOffset_label);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(821, 690);
            this.tabPage5.TabIndex = 7;
            this.tabPage5.Text = "Calibrations";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label3);
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Controls.Add(this.UpCameraBoxX_textBox);
            this.groupBox7.Controls.Add(this.label129);
            this.groupBox7.Controls.Add(this.label105);
            this.groupBox7.Controls.Add(this.DownCamera_Calibration_button);
            this.groupBox7.Controls.Add(this.SlackMeasurement_label);
            this.groupBox7.Controls.Add(this.calibMoveDistance_textBox);
            this.groupBox7.Controls.Add(this.label106);
            this.groupBox7.Controls.Add(this.UpCameraBoxY_textBox);
            this.groupBox7.Controls.Add(this.UpCameraBoxXmmPerPixel_label);
            this.groupBox7.Controls.Add(this.UpCameraBoxYmmPerPixel_label);
            this.groupBox7.Controls.Add(this.button_camera_calibrate);
            this.groupBox7.Controls.Add(this.DownCameraBoxX_textBox);
            this.groupBox7.Controls.Add(this.label70);
            this.groupBox7.Controls.Add(this.label71);
            this.groupBox7.Controls.Add(this.DownCameraBoxY_textBox);
            this.groupBox7.Controls.Add(this.DownCameraBoxXmmPerPixel_label);
            this.groupBox7.Controls.Add(this.DownCameraBoxYmmPerPixel_label);
            this.groupBox7.Location = new System.Drawing.Point(165, 14);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(240, 241);
            this.groupBox7.TabIndex = 148;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Camera mmPerPixel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 143;
            this.label3.Text = "DownCamera";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 142;
            this.label1.Text = "UpCamera";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button4);
            this.groupBox4.Controls.Add(this.SetProbing_button);
            this.groupBox4.Controls.Add(this.label117);
            this.groupBox4.Controls.Add(this.Z0toPCB_BasicTab_label);
            this.groupBox4.Controls.Add(this.label111);
            this.groupBox4.Controls.Add(this.Z_Backoff_label);
            this.groupBox4.Controls.Add(this.Offset2Method_button);
            this.groupBox4.Controls.Add(this.label143);
            this.groupBox4.Controls.Add(this.zoffset_textbox);
            this.groupBox4.Controls.Add(this.NeedleOffsetY_textBox);
            this.groupBox4.Controls.Add(this.NeedleOffsetX_textBox);
            this.groupBox4.Controls.Add(this.label130);
            this.groupBox4.Controls.Add(this.label149);
            this.groupBox4.Controls.Add(this.label148);
            this.groupBox4.Controls.Add(this.label146);
            this.groupBox4.Controls.Add(this.label131);
            this.groupBox4.Location = new System.Drawing.Point(5, 14);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(154, 241);
            this.groupBox4.TabIndex = 147;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Needle";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(9, 23);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(108, 23);
            this.button4.TabIndex = 78;
            this.button4.Text = "Rotational Offset";
            this.toolTip1.SetToolTip(this.button4, "Re-runs needle calibration routine.");
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.TestNeedleRecognition_button_Click);
            // 
            // CAD_openFileDialog
            // 
            this.CAD_openFileDialog.Filter = "CSV files (*.csv)|*.csv|KiCad files (*.pos)|*.pos|All files (*.*)|*.*";
            this.CAD_openFileDialog.ReadOnlyChecked = true;
            this.CAD_openFileDialog.SupportMultiDottedExtensions = true;
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 800;
            this.toolTip1.AutoPopDelay = 16000;
            this.toolTip1.InitialDelay = 800;
            this.toolTip1.ReshowDelay = 160;
            // 
            // TrueX_label
            // 
            this.TrueX_label.AutoSize = true;
            this.TrueX_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrueX_label.Location = new System.Drawing.Point(155, 769);
            this.TrueX_label.Name = "TrueX_label";
            this.TrueX_label.Size = new System.Drawing.Size(31, 9);
            this.TrueX_label.TabIndex = 77;
            this.TrueX_label.Text = "000.000";
            this.TrueX_label.Visible = false;
            // 
            // mechHome_button
            // 
            this.mechHome_button.Location = new System.Drawing.Point(9, 876);
            this.mechHome_button.Name = "mechHome_button";
            this.mechHome_button.Size = new System.Drawing.Size(93, 23);
            this.mechHome_button.TabIndex = 87;
            this.mechHome_button.Text = "Mech. Home";
            this.mechHome_button.UseVisualStyleBackColor = true;
            this.mechHome_button.Click += new System.EventHandler(this.mechHome_button_Click);
            // 
            // OptHome_button
            // 
            this.OptHome_button.Location = new System.Drawing.Point(8, 898);
            this.OptHome_button.Name = "OptHome_button";
            this.OptHome_button.Size = new System.Drawing.Size(94, 23);
            this.OptHome_button.TabIndex = 88;
            this.OptHome_button.Text = "Optical Home";
            this.OptHome_button.UseVisualStyleBackColor = true;
            this.OptHome_button.Click += new System.EventHandler(this.OptHome_button_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 949);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(839, 22);
            this.statusStrip1.TabIndex = 115;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(839, 24);
            this.menuStrip1.TabIndex = 116;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadCADFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.loadJobFileToolStripMenuItem,
            this.saveJobFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadCADFileToolStripMenuItem
            // 
            this.loadCADFileToolStripMenuItem.Name = "loadCADFileToolStripMenuItem";
            this.loadCADFileToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.loadCADFileToolStripMenuItem.Text = "Load Pick-n-Place File";
            this.loadCADFileToolStripMenuItem.Click += new System.EventHandler(this.loadCADFileToolStripMenuItem_Click);
            // 
            // loadJobFileToolStripMenuItem
            // 
            this.loadJobFileToolStripMenuItem.Name = "loadJobFileToolStripMenuItem";
            this.loadJobFileToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.loadJobFileToolStripMenuItem.Text = "Load Job File";
            this.loadJobFileToolStripMenuItem.Click += new System.EventHandler(this.loadJobFileToolStripMenuItem_Click);
            // 
            // saveJobFileToolStripMenuItem
            // 
            this.saveJobFileToolStripMenuItem.Name = "saveJobFileToolStripMenuItem";
            this.saveJobFileToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveJobFileToolStripMenuItem.Text = "Save Job File";
            this.saveJobFileToolStripMenuItem.Click += new System.EventHandler(this.saveJobFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(188, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(839, 971);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.CameraSetupTest_button);
            this.Controls.Add(this.Snapshot_button);
            this.Controls.Add(this.ImageTest_checkBox);
            this.Controls.Add(this.MultiCalibrate_button);
            this.Controls.Add(this.OptHome_button);
            this.Controls.Add(this.StopDemo_button);
            this.Controls.Add(this.mechHome_button);
            this.Controls.Add(this.Demo_button);
            this.Controls.Add(this.TrueX_label);
            this.Controls.Add(this.label145);
            this.Controls.Add(this.GotoUpCamPosition_button);
            this.Controls.Add(this.Test6_button);
            this.Controls.Add(this.label124);
            this.Controls.Add(this.label97);
            this.Controls.Add(this.Test5_button);
            this.Controls.Add(this.ZDown_button);
            this.Controls.Add(this.labelSerialPortStatus);
            this.Controls.Add(this.ZUp_button);
            this.Controls.Add(this.GotoPickupCenter_button);
            this.Controls.Add(this.GotoPCB0_button);
            this.Controls.Add(this.Test4_button);
            this.Controls.Add(this.Park_button);
            this.Controls.Add(this.Test3_button);
            this.Controls.Add(this.OpticalHome_button);
            this.Controls.Add(this.apos_textBox);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.zpos_textBox);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.ypos_textBox);
            this.Controls.Add(this.Test2_button);
            this.Controls.Add(this.MotorPower_checkBox);
            this.Controls.Add(this.Test1_button);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.xpos_textBox);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.tabControlPages);
            this.Controls.Add(this.Pump_checkBox);
            this.Controls.Add(this.Vacuum_checkBox);
            this.Controls.Add(this.buttonConnectSerial);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "LitePlacer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.Tapes_tabPage.ResumeLayout(false);
            this.Tapes_tabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tapes_dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tapeObjBindingSource)).EndInit();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.tabPageBasicSetup.ResumeLayout(false);
            this.tabPageBasicSetup.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabpage1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SmallMovement_numericUpDown)).EndInit();
            this.RunJob_tabPage.ResumeLayout(false);
            this.RunJob_tabPage.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.JobData_GridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobDataBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CadData_GridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.physicalComponentBindingSource)).EndInit();
            this.tabControlPages.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private Label label4;
        private TextBox textBoxSendtoTinyG;
        private Label label14;
        private TextBox xpos_textBox;
        private TextBox ypos_textBox;
        private Label label17;
        private TextBox zpos_textBox;
        private Label label18;
        private TextBox apos_textBox;
		private Label label19;
		private RichTextBox SerialMonitor_richTextBox;
        private Button Test1_button;
		private Button Test2_button;
		private OpenFileDialog Job_openFileDialog;
		private SaveFileDialog Job_saveFileDialog;
		private Button OpticalHome_button;
        private Button Park_button;
		private Button TestNeedleRecognition_button;
		private Label label97;
        private Button Test3_button;
		private Button Test4_button;
		private Label label124;
        private Button Test5_button;
        private Button Test6_button;
		private Label label145;
		private TabPage Tapes_tabPage;
		private Button AddTape_button;
		private Label label109;
        private DataGridView Tapes_dataGridView;
		private Label label101;
		private Label label100;
		private Label label55;
		private Button CameraSetupTest_button;
		private Button ZUp_button;
		private Button ZDown_button;
        private Label UpCameraBoxYmmPerPixel_label;
        private Label UpCameraBoxXmmPerPixel_label;
        private Label label105;
		private Label label106;
		private TextBox UpCameraBoxX_textBox;
        private TextBox UpCameraBoxY_textBox;
		private TextBox DownCameraBoxX_textBox;
		private TextBox DownCameraBoxY_textBox;
		private Label label70;
		private Label label71;
		private Label DownCameraBoxXmmPerPixel_label;
        private Label DownCameraBoxYmmPerPixel_label;
		private Button GotoUpCamPosition_button;
		private Button SetUpCamPosition_button;
		private Label label99;
		private Label label98;
		private TextBox UpcamPositionY_textBox;
		private TextBox UpcamPositionX_textBox;
		private TextBox NeedleOffsetY_textBox;
		private TextBox NeedleOffsetX_textBox;
		private TextBox PickupCenterY_textBox;
		private TextBox PickupCenterX_textBox;
		private TextBox JigY_textBox;
        private TextBox JigX_textBox;
		private Label label149;
		private Label label148;
		private Label label146;
        private Label label143;
		private Button Offset2Method_button;
		private Label NeedleOffset_label;
        private Label label115;
		private Button SetPickupCenter_button;
		private Button SetPCB0_button;
		private Button GotoPickupCenter_button;
		private Button GotoPCB0_button;
		private Label label95;
		private Label label96;
		private Label label93;
		private Label label94;
		private Button Snapshot_button;
        private CheckBox ImageTest_checkBox;
        private TabPage tabPageBasicSetup;
        private CheckBox SlackCompensation_checkBox;
        private TextBox SizeYMax_textBox;
		private TextBox SizeXMax_textBox;
		private TextBox ParkLocationY_textBox;
		private TextBox ParkLocationX_textBox;
        private Button TestYX_button;
		private CheckBox MotorPower_checkBox;
        private Label Z0toPCB_BasicTab_label;
		private Label label111;
        private Label Zlb_label;
        private Button SetProbing_button;
		private Label label113;
        private Label label102;
		private Label label107;
		private Label label92;
        private Label label61;
		private Button TestA_button;
		private Button Homebutton;
		private Button TestZ_button;
		private Button HomeZ_button;
		private Button HomeY_button;
		private Button HomeXY_button;
		private Button HomeX_button;
		private Button BuiltInSettings_button;
		private Button SaveSettings_button;
		private Button DefaultSettings_button;
		private CheckBox Vacuum_checkBox;
		private CheckBox Pump_checkBox;
		private Button TestXY_button;
		private Button TestY_button;
		private Button TestX_button;
		private Panel panel7;
		private Panel panel8;
		private TextBox tr4_textBox;
		private RadioButton m4deg18_radioButton;
		private RadioButton m4deg09_radioButton;
		private Label label42;
		private Label label43;
		private Label label44;
		private MaskedTextBox mi4_maskedTextBox;
		private Label label45;
		private Label label46;
		private Label label47;
		private Label label48;
		private MaskedTextBox avm_maskedTextBox;
		private Label label49;
		private Label label50;
		private Label label51;
		private MaskedTextBox ajm_maskedTextBox;
		private Label label52;
		private Panel panel3;
		private Label label73;
		private MaskedTextBox xsv_maskedTextBox;
		private Label label74;
		private Label label75;
		private MaskedTextBox xjh_maskedTextBox;
		private Label label76;
		private CheckBox Xmax_checkBox;
		private CheckBox Xlim_checkBox;
		private CheckBox Xhome_checkBox;
		private Panel panel4;
		private TextBox tr1_textBox;
		private RadioButton m1deg18_radioButton;
		private RadioButton m1deg09_radioButton;
		private Label label20;
		private Label label21;
		private Label label22;
		private Label label23;
		private MaskedTextBox mi1_maskedTextBox;
		private Label label24;
		private Label label25;
		private Label label26;
		private MaskedTextBox xvm_maskedTextBox;
		private Label label27;
		private Label label28;
		private Label label29;
		private MaskedTextBox xjm_maskedTextBox;
		private Label label30;
		private Panel panel5;
		private Label label81;
		private MaskedTextBox zsv_maskedTextBox;
		private Label label82;
		private Label label83;
		private MaskedTextBox zjh_maskedTextBox;
		private Label label84;
		private CheckBox Zmax_checkBox;
		private CheckBox Zlim_checkBox;
		private CheckBox Zhome_checkBox;
		private Panel panel6;
		private TextBox tr3_textBox;
		private RadioButton m3deg18_radioButton;
		private RadioButton m3deg09_radioButton;
		private Label label31;
		private Label label32;
		private Label label33;
		private MaskedTextBox mi3_maskedTextBox;
		private Label label34;
		private Label label35;
		private Label label36;
		private Label label37;
		private MaskedTextBox zvm_maskedTextBox;
		private Label label38;
		private Label label39;
		private Label label40;
		private MaskedTextBox zjm_maskedTextBox;
		private Label label41;
		private Panel panel1;
		private Label label77;
		private MaskedTextBox ysv_maskedTextBox;
		private Label label78;
		private Label label79;
		private MaskedTextBox yjh_maskedTextBox;
		private Label label80;
		private CheckBox Ymax_checkBox;
		private CheckBox Ylim_checkBox;
		private CheckBox Yhome_checkBox;
		private Panel panel2;
		private TextBox tr2_textBox;
		private RadioButton m2deg18_radioButton;
		private RadioButton m2deg09_radioButton;
		private Label label15;
		private Label label16;
		private Label label13;
		private Label label11;
		private MaskedTextBox mi2_maskedTextBox;
		private Label label12;
		private Label label10;
		private Label label8;
		private MaskedTextBox yvm_maskedTextBox;
		private Label label9;
		private Label label7;
		private Label label6;
		private MaskedTextBox yjm_maskedTextBox;
        private Label label5;
		private Button buttonRefreshPortList;
		private Label labelSerialPortStatus;
		private Button buttonConnectSerial;
		private Label label2;
		private ComboBox comboBoxSerialPorts;
        private TabPage RunJob_tabPage;
        private Button ReMeasure_button;
		private TextBox JobOffsetY_textBox;
		private TextBox JobOffsetX_textBox;
		private Button ShowNominal_button;
        private Button ShowMachine_button;
		private GroupBox groupBox2;
		private Button NewRow_button;
		private Button PlaceThese_button;
		private Button DeleteComponentGroup_button;
		private Button Down_button;
		private Button Up_button;
		private GroupBox groupBox1;
		private Button AbortPlacement_button;
		private Button PausePlacement_button;
		private Label MachineCoords_label;
		private Label PlacedValue_label;
		private Label PlacedRotation_label;
		private Label PlacedY_label;
		private Label PlacedX_label;
		private Label PlacedComponent_label;
		private Label label66;
		private Label label65;
		private Label label64;
		private Label label63;
		private Label label58;
		private Button PlaceAll_button;
		private Label label89;
		private Label label88;
		private Label label86;
        private Label label85;
		private DataGridView JobData_GridView;
		public CheckBox Bottom_checkBox;
        private DataGridView CadData_GridView;
		private TabControl tabControlPages;
		private Label label87;
		private NumericUpDown SmallMovement_numericUpDown;
        private GroupBox groupBox3;
        private OpenFileDialog CAD_openFileDialog;
		private Button DeleteTape_button;
		private Button TapeDown_button;
		private Button TapeUp_button;
        private Button TapeGoTo_button;
		private ToolTip toolTip1;
		private Button ResetOneTape_button;
        private Button ResetAllTapes_button;
		private Label label67;
		private Label label62;
		private Button SetPartNo_button;
        private MaskedTextBox NextPart_TextBox;
		private Label TrueX_label;
		private Label label90;
		private TextBox SquareCorrection_textBox;
        private Label Z_Backoff_label;
        private Label label117;
        private Button Tape_GoToNext_button;
        private Button Tape_resetZs_button;
        private Label label118;
        private TextBox VacuumTime_textBox;
        private TextBox VacuumRelease_textBox;
        private Label label119;
        private Button PlaceOne_button;
        private Button TapeSet1_button;
        private CheckBox ValidMeasurement_checkBox;
        private Button ChangeNeedle_button;
        private Label label123;
        private TextBox ZTestTravel_textBox;
        private Button Demo_button;
        private Button StopDemo_button;
        private Button button_camera_calibrate;
        private Label SlackMeasurement_label;
        private CheckBox cb_useTemplate;
        private GroupBox groupBox12;
        private Button button_setTemplate;
        private Label label126;
        private TextBox fiducialTemlateMatch_textBox;
        private Label label127;
        private TextBox fiducial_designator_regexp_textBox;
        private Button tape_ViewComponents_button;
        private Label label128;
        private Button pickup_next_button;
        private Button DownCamera_Calibration_button;
        private Label label129;
        private TextBox calibMoveDistance_textBox;
        private TextBox zoffset_textbox;
        private Label label130;
        private Label label131;
        private Button button1;
        private Button button2;
        private Button MultiCalibrate_button;
        private Button view_nextParts_button;
        private Button needle_calibration_test_button;
        private Button mechHome_button;
        private Button OptHome_button;
        private BindingSource physicalComponentBindingSource;
        private DataGridViewTextBoxColumn designatorDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn footprintDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn xnominalDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn ynominalDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn rotationDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn xmachineDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn ymachineDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn rotationmachineDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn methodDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn methodParameterDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn isFiducialDataGridViewCheckBoxColumn;
        private BindingSource jobDataBindingSource;
        private TabControl tabControl1;
        private TabPage tabpage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private Button button3;
        private Button button4;
        private GroupBox groupBox6;
        private GroupBox groupBox7;
        private Label label3;
        private Label label1;
        private GroupBox groupBox4;
        private DataGridViewTextBoxColumn countDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn componentListDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn componentTypeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn methodDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn methodParametersDataGridViewTextBoxColumn;

        private BindingSource tapeObjBindingSource;
        private DataGridViewButtonColumn SelectButtonColumn;
        private DataGridViewTextBoxColumn ID;
        private DataGridViewComboBoxColumn OriginalTapeOrientation;
        private DataGridViewComboBoxColumn OriginalPartOrientation;
        private DataGridViewComboBoxColumn Type;
        private DataGridViewComboBoxColumn PartType;
        private DataGridViewTextBoxColumn HolePitch;
        private DataGridViewTextBoxColumn PartPitch;
        private DataGridViewTextBoxColumn HoleToPartSpacingX;
        private DataGridViewTextBoxColumn holeToPartSpacingYDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn pickupZDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn placeZDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn bDataGridViewTextBoxColumn;
        private StatusStrip statusStrip1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadCADFileToolStripMenuItem;
        private ToolStripMenuItem loadJobFileToolStripMenuItem;
        private ToolStripMenuItem saveJobFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem quitToolStripMenuItem;
    }
}

