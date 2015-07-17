<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SelectDisplayData
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Aggregate across all")
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("By river or segment")
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.cboBaseline = New System.Windows.Forms.ComboBox()
        Me.lblBaseline = New System.Windows.Forms.Label()
        Me.lblDailyFlow = New System.Windows.Forms.Label()
        Me.pnlDailyFlow = New System.Windows.Forms.Panel()
        Me.chkShowHydro = New System.Windows.Forms.CheckBox()
        Me.tvTreatment = New System.Windows.Forms.TreeView()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tvScenario = New System.Windows.Forms.TreeView()
        Me.tvRivers = New System.Windows.Forms.TreeView()
        Me.lblScenario = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.tcDisplays = New System.Windows.Forms.TabControl()
        Me.tpSpeciesHabitat = New System.Windows.Forms.TabPage()
        Me.pnlParameters = New System.Windows.Forms.Panel()
        Me.gbHydroperiod = New System.Windows.Forms.GroupBox()
        Me.btnSaveHydroPeriod = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtEndDay = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtEndMonth = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtStartDay = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtStartMonth = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.tvSpecies = New System.Windows.Forms.TreeView()
        Me.tpOtherMetrics = New System.Windows.Forms.TabPage()
        Me.tvOtherMetrics = New System.Windows.Forms.TreeView()
        Me.tpDisplayMetrics = New System.Windows.Forms.TabPage()
        Me.tvDisplayMetrics = New System.Windows.Forms.TreeView()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnMakeDefault = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.rbDaily = New System.Windows.Forms.RadioButton()
        Me.rbYearly = New System.Windows.Forms.RadioButton()
        Me.rbEntire = New System.Windows.Forms.RadioButton()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.pnlInterval = New System.Windows.Forms.Panel()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.pnlLimitTimePeriod = New System.Windows.Forms.Panel()
        Me.cboEnd = New System.Windows.Forms.ComboBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.cboStart = New System.Windows.Forms.ComboBox()
        Me.Label13 = New System.Windows.Forms.Label()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.pnlDailyFlow.SuspendLayout()
        Me.tcDisplays.SuspendLayout()
        Me.tpSpeciesHabitat.SuspendLayout()
        Me.pnlParameters.SuspendLayout()
        Me.gbHydroperiod.SuspendLayout()
        Me.tpOtherMetrics.SuspendLayout()
        Me.tpDisplayMetrics.SuspendLayout()
        Me.pnlInterval.SuspendLayout()
        Me.pnlLimitTimePeriod.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer1.Location = New System.Drawing.Point(1, 2)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboBaseline)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lblBaseline)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lblDailyFlow)
        Me.SplitContainer1.Panel1.Controls.Add(Me.pnlDailyFlow)
        Me.SplitContainer1.Panel1.Controls.Add(Me.tvTreatment)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label4)
        Me.SplitContainer1.Panel1.Controls.Add(Me.tvScenario)
        Me.SplitContainer1.Panel1.Controls.Add(Me.tvRivers)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lblScenario)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.tcDisplays)
        Me.SplitContainer1.Size = New System.Drawing.Size(958, 606)
        Me.SplitContainer1.SplitterDistance = 314
        Me.SplitContainer1.TabIndex = 0
        '
        'cboBaseline
        '
        Me.cboBaseline.FormattingEnabled = True
        Me.cboBaseline.Location = New System.Drawing.Point(88, 103)
        Me.cboBaseline.Name = "cboBaseline"
        Me.cboBaseline.Size = New System.Drawing.Size(212, 21)
        Me.cboBaseline.TabIndex = 16
        '
        'lblBaseline
        '
        Me.lblBaseline.AutoSize = True
        Me.lblBaseline.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBaseline.Location = New System.Drawing.Point(9, 104)
        Me.lblBaseline.Name = "lblBaseline"
        Me.lblBaseline.Size = New System.Drawing.Size(73, 16)
        Me.lblBaseline.TabIndex = 15
        Me.lblBaseline.Text = "Baseline:"
        '
        'lblDailyFlow
        '
        Me.lblDailyFlow.AutoSize = True
        Me.lblDailyFlow.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDailyFlow.Location = New System.Drawing.Point(16, 531)
        Me.lblDailyFlow.Name = "lblDailyFlow"
        Me.lblDailyFlow.Size = New System.Drawing.Size(105, 16)
        Me.lblDailyFlow.TabIndex = 14
        Me.lblDailyFlow.Text = "Daily Flow (Q)"
        '
        'pnlDailyFlow
        '
        Me.pnlDailyFlow.BackColor = System.Drawing.Color.White
        Me.pnlDailyFlow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlDailyFlow.Controls.Add(Me.chkShowHydro)
        Me.pnlDailyFlow.Location = New System.Drawing.Point(12, 550)
        Me.pnlDailyFlow.Name = "pnlDailyFlow"
        Me.pnlDailyFlow.Size = New System.Drawing.Size(288, 33)
        Me.pnlDailyFlow.TabIndex = 13
        '
        'chkShowHydro
        '
        Me.chkShowHydro.AutoSize = True
        Me.chkShowHydro.Checked = True
        Me.chkShowHydro.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkShowHydro.Location = New System.Drawing.Point(7, 8)
        Me.chkShowHydro.Name = "chkShowHydro"
        Me.chkShowHydro.Size = New System.Drawing.Size(158, 17)
        Me.chkShowHydro.TabIndex = 3
        Me.chkShowHydro.Text = "Include daily flow values (Q)"
        Me.chkShowHydro.UseVisualStyleBackColor = True
        '
        'tvTreatment
        '
        Me.tvTreatment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tvTreatment.CheckBoxes = True
        Me.tvTreatment.Location = New System.Drawing.Point(12, 165)
        Me.tvTreatment.Name = "tvTreatment"
        Me.tvTreatment.Size = New System.Drawing.Size(288, 45)
        Me.tvTreatment.TabIndex = 12
        Me.tvTreatment.Visible = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(11, 146)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(93, 16)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Time Period"
        Me.Label4.Visible = False
        '
        'tvScenario
        '
        Me.tvScenario.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tvScenario.CheckBoxes = True
        Me.tvScenario.FullRowSelect = True
        Me.tvScenario.HideSelection = False
        Me.tvScenario.Location = New System.Drawing.Point(12, 24)
        Me.tvScenario.Name = "tvScenario"
        Me.tvScenario.Size = New System.Drawing.Size(288, 77)
        Me.tvScenario.TabIndex = 10
        '
        'tvRivers
        '
        Me.tvRivers.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tvRivers.CheckBoxes = True
        Me.tvRivers.Location = New System.Drawing.Point(12, 246)
        Me.tvRivers.Name = "tvRivers"
        TreeNode3.Name = "aggAll"
        TreeNode3.Text = "Aggregate across all"
        TreeNode4.Name = "AggByRiver"
        TreeNode4.Text = "By river or segment"
        Me.tvRivers.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode3, TreeNode4})
        Me.tvRivers.Size = New System.Drawing.Size(288, 278)
        Me.tvRivers.TabIndex = 9
        '
        'lblScenario
        '
        Me.lblScenario.AutoSize = True
        Me.lblScenario.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblScenario.Location = New System.Drawing.Point(9, 5)
        Me.lblScenario.Name = "lblScenario"
        Me.lblScenario.Size = New System.Drawing.Size(70, 16)
        Me.lblScenario.TabIndex = 7
        Me.lblScenario.Text = "Scenario"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(10, 227)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(111, 16)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "River/Segment"
        '
        'tcDisplays
        '
        Me.tcDisplays.Controls.Add(Me.tpSpeciesHabitat)
        Me.tcDisplays.Controls.Add(Me.tpOtherMetrics)
        Me.tcDisplays.Controls.Add(Me.tpDisplayMetrics)
        Me.tcDisplays.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcDisplays.Location = New System.Drawing.Point(0, 0)
        Me.tcDisplays.Name = "tcDisplays"
        Me.tcDisplays.SelectedIndex = 0
        Me.tcDisplays.Size = New System.Drawing.Size(636, 602)
        Me.tcDisplays.TabIndex = 0
        '
        'tpSpeciesHabitat
        '
        Me.tpSpeciesHabitat.Controls.Add(Me.pnlParameters)
        Me.tpSpeciesHabitat.Controls.Add(Me.tvSpecies)
        Me.tpSpeciesHabitat.Location = New System.Drawing.Point(4, 22)
        Me.tpSpeciesHabitat.Name = "tpSpeciesHabitat"
        Me.tpSpeciesHabitat.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSpeciesHabitat.Size = New System.Drawing.Size(628, 576)
        Me.tpSpeciesHabitat.TabIndex = 0
        Me.tpSpeciesHabitat.Text = "Species Habitat"
        Me.tpSpeciesHabitat.UseVisualStyleBackColor = True
        '
        'pnlParameters
        '
        Me.pnlParameters.Controls.Add(Me.gbHydroperiod)
        Me.pnlParameters.Controls.Add(Me.Label5)
        Me.pnlParameters.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlParameters.Location = New System.Drawing.Point(351, 3)
        Me.pnlParameters.Name = "pnlParameters"
        Me.pnlParameters.Size = New System.Drawing.Size(274, 570)
        Me.pnlParameters.TabIndex = 1
        '
        'gbHydroperiod
        '
        Me.gbHydroperiod.Controls.Add(Me.btnSaveHydroPeriod)
        Me.gbHydroperiod.Controls.Add(Me.Label10)
        Me.gbHydroperiod.Controls.Add(Me.Label8)
        Me.gbHydroperiod.Controls.Add(Me.txtEndDay)
        Me.gbHydroperiod.Controls.Add(Me.Label9)
        Me.gbHydroperiod.Controls.Add(Me.txtEndMonth)
        Me.gbHydroperiod.Controls.Add(Me.Label7)
        Me.gbHydroperiod.Controls.Add(Me.txtStartDay)
        Me.gbHydroperiod.Controls.Add(Me.Label6)
        Me.gbHydroperiod.Controls.Add(Me.txtStartMonth)
        Me.gbHydroperiod.Location = New System.Drawing.Point(9, 29)
        Me.gbHydroperiod.Name = "gbHydroperiod"
        Me.gbHydroperiod.Size = New System.Drawing.Size(262, 219)
        Me.gbHydroperiod.TabIndex = 1
        Me.gbHydroperiod.TabStop = False
        Me.gbHydroperiod.Text = "Hydroperiod"
        '
        'btnSaveHydroPeriod
        '
        Me.btnSaveHydroPeriod.Enabled = False
        Me.btnSaveHydroPeriod.Location = New System.Drawing.Point(39, 192)
        Me.btnSaveHydroPeriod.Name = "btnSaveHydroPeriod"
        Me.btnSaveHydroPeriod.Size = New System.Drawing.Size(185, 23)
        Me.btnSaveHydroPeriod.TabIndex = 9
        Me.btnSaveHydroPeriod.Text = "Save Hydroperiod Changes"
        Me.btnSaveHydroPeriod.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Enabled = False
        Me.Label10.Location = New System.Drawing.Point(10, 137)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(214, 52)
        Me.Label10.TabIndex = 8
        Me.Label10.Text = "*** Note these changes are only temporary " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "      and only affect the current cha" & _
    "rt." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "  To make them global and permanent click " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "  the button below" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Enabled = False
        Me.Label8.Location = New System.Drawing.Point(7, 102)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(51, 13)
        Me.Label8.TabIndex = 7
        Me.Label8.Text = "End Day:"
        '
        'txtEndDay
        '
        Me.txtEndDay.Enabled = False
        Me.txtEndDay.Location = New System.Drawing.Point(78, 99)
        Me.txtEndDay.Name = "txtEndDay"
        Me.txtEndDay.Size = New System.Drawing.Size(62, 20)
        Me.txtEndDay.TabIndex = 6
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Enabled = False
        Me.Label9.Location = New System.Drawing.Point(7, 80)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(62, 13)
        Me.Label9.TabIndex = 5
        Me.Label9.Text = "End Month:"
        '
        'txtEndMonth
        '
        Me.txtEndMonth.Enabled = False
        Me.txtEndMonth.Location = New System.Drawing.Point(78, 77)
        Me.txtEndMonth.Name = "txtEndMonth"
        Me.txtEndMonth.Size = New System.Drawing.Size(62, 20)
        Me.txtEndMonth.TabIndex = 4
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Enabled = False
        Me.Label7.Location = New System.Drawing.Point(7, 47)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(54, 13)
        Me.Label7.TabIndex = 3
        Me.Label7.Text = "Start Day:"
        '
        'txtStartDay
        '
        Me.txtStartDay.Enabled = False
        Me.txtStartDay.Location = New System.Drawing.Point(78, 44)
        Me.txtStartDay.Name = "txtStartDay"
        Me.txtStartDay.Size = New System.Drawing.Size(62, 20)
        Me.txtStartDay.TabIndex = 2
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Enabled = False
        Me.Label6.Location = New System.Drawing.Point(7, 25)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(65, 13)
        Me.Label6.TabIndex = 1
        Me.Label6.Text = "Start Month:"
        '
        'txtStartMonth
        '
        Me.txtStartMonth.Enabled = False
        Me.txtStartMonth.Location = New System.Drawing.Point(78, 22)
        Me.txtStartMonth.Name = "txtStartMonth"
        Me.txtStartMonth.Size = New System.Drawing.Size(62, 20)
        Me.txtStartMonth.TabIndex = 0
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(5, 5)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(101, 20)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Parameters"
        '
        'tvSpecies
        '
        Me.tvSpecies.CheckBoxes = True
        Me.tvSpecies.Dock = System.Windows.Forms.DockStyle.Left
        Me.tvSpecies.HideSelection = False
        Me.tvSpecies.Location = New System.Drawing.Point(3, 3)
        Me.tvSpecies.Name = "tvSpecies"
        Me.tvSpecies.Size = New System.Drawing.Size(274, 570)
        Me.tvSpecies.TabIndex = 0
        '
        'tpOtherMetrics
        '
        Me.tpOtherMetrics.Controls.Add(Me.tvOtherMetrics)
        Me.tpOtherMetrics.Location = New System.Drawing.Point(4, 22)
        Me.tpOtherMetrics.Name = "tpOtherMetrics"
        Me.tpOtherMetrics.Padding = New System.Windows.Forms.Padding(3)
        Me.tpOtherMetrics.Size = New System.Drawing.Size(628, 576)
        Me.tpOtherMetrics.TabIndex = 1
        Me.tpOtherMetrics.Text = "Other Metrics"
        Me.tpOtherMetrics.UseVisualStyleBackColor = True
        '
        'tvOtherMetrics
        '
        Me.tvOtherMetrics.CheckBoxes = True
        Me.tvOtherMetrics.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvOtherMetrics.HideSelection = False
        Me.tvOtherMetrics.Location = New System.Drawing.Point(3, 3)
        Me.tvOtherMetrics.Name = "tvOtherMetrics"
        Me.tvOtherMetrics.Size = New System.Drawing.Size(622, 570)
        Me.tvOtherMetrics.TabIndex = 0
        '
        'tpDisplayMetrics
        '
        Me.tpDisplayMetrics.Controls.Add(Me.tvDisplayMetrics)
        Me.tpDisplayMetrics.Location = New System.Drawing.Point(4, 22)
        Me.tpDisplayMetrics.Name = "tpDisplayMetrics"
        Me.tpDisplayMetrics.Size = New System.Drawing.Size(628, 576)
        Me.tpDisplayMetrics.TabIndex = 2
        Me.tpDisplayMetrics.Text = "DisplayMetrics"
        Me.tpDisplayMetrics.UseVisualStyleBackColor = True
        '
        'tvDisplayMetrics
        '
        Me.tvDisplayMetrics.CheckBoxes = True
        Me.tvDisplayMetrics.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvDisplayMetrics.Location = New System.Drawing.Point(0, 0)
        Me.tvDisplayMetrics.Name = "tvDisplayMetrics"
        Me.tvDisplayMetrics.Size = New System.Drawing.Size(628, 576)
        Me.tvDisplayMetrics.TabIndex = 0
        '
        'btnOK
        '
        Me.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnOK.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnOK.Location = New System.Drawing.Point(683, 614)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 1
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnCancel.Location = New System.Drawing.Point(764, 614)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'btnMakeDefault
        '
        Me.btnMakeDefault.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnMakeDefault.Location = New System.Drawing.Point(867, 614)
        Me.btnMakeDefault.Name = "btnMakeDefault"
        Me.btnMakeDefault.Size = New System.Drawing.Size(81, 23)
        Me.btnMakeDefault.TabIndex = 6
        Me.btnMakeDefault.Text = "Make default"
        Me.ToolTip1.SetToolTip(Me.btnMakeDefault, "Clicking this saves the current selection to be used as the default data " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "for th" & _
        "e type of chart, data view.")
        Me.btnMakeDefault.UseVisualStyleBackColor = True
        '
        'rbDaily
        '
        Me.rbDaily.AutoSize = True
        Me.rbDaily.Checked = True
        Me.rbDaily.Location = New System.Drawing.Point(66, 2)
        Me.rbDaily.Name = "rbDaily"
        Me.rbDaily.Size = New System.Drawing.Size(48, 17)
        Me.rbDaily.TabIndex = 7
        Me.rbDaily.TabStop = True
        Me.rbDaily.Text = "Daily"
        Me.rbDaily.UseVisualStyleBackColor = True
        '
        'rbYearly
        '
        Me.rbYearly.AutoSize = True
        Me.rbYearly.Location = New System.Drawing.Point(120, 2)
        Me.rbYearly.Name = "rbYearly"
        Me.rbYearly.Size = New System.Drawing.Size(54, 17)
        Me.rbYearly.TabIndex = 8
        Me.rbYearly.TabStop = True
        Me.rbYearly.Text = "Yearly"
        Me.rbYearly.UseVisualStyleBackColor = True
        '
        'rbEntire
        '
        Me.rbEntire.AutoSize = True
        Me.rbEntire.Location = New System.Drawing.Point(180, 2)
        Me.rbEntire.Name = "rbEntire"
        Me.rbEntire.Size = New System.Drawing.Size(84, 17)
        Me.rbEntire.TabIndex = 9
        Me.rbEntire.Text = "Entire period"
        Me.rbEntire.UseVisualStyleBackColor = True
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(3, 3)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(103, 13)
        Me.Label11.TabIndex = 11
        Me.Label11.Text = "Limit time period:"
        '
        'pnlInterval
        '
        Me.pnlInterval.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.pnlInterval.BackColor = System.Drawing.Color.White
        Me.pnlInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlInterval.Controls.Add(Me.Label12)
        Me.pnlInterval.Controls.Add(Me.rbYearly)
        Me.pnlInterval.Controls.Add(Me.rbDaily)
        Me.pnlInterval.Controls.Add(Me.rbEntire)
        Me.pnlInterval.Location = New System.Drawing.Point(12, 615)
        Me.pnlInterval.Name = "pnlInterval"
        Me.pnlInterval.Size = New System.Drawing.Size(266, 22)
        Me.pnlInterval.TabIndex = 12
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(3, 4)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(54, 13)
        Me.Label12.TabIndex = 10
        Me.Label12.Text = "Interval:"
        '
        'pnlLimitTimePeriod
        '
        Me.pnlLimitTimePeriod.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.pnlLimitTimePeriod.BackColor = System.Drawing.Color.White
        Me.pnlLimitTimePeriod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlLimitTimePeriod.Controls.Add(Me.cboEnd)
        Me.pnlLimitTimePeriod.Controls.Add(Me.Label14)
        Me.pnlLimitTimePeriod.Controls.Add(Me.cboStart)
        Me.pnlLimitTimePeriod.Controls.Add(Me.Label13)
        Me.pnlLimitTimePeriod.Controls.Add(Me.Label11)
        Me.pnlLimitTimePeriod.Location = New System.Drawing.Point(284, 615)
        Me.pnlLimitTimePeriod.Name = "pnlLimitTimePeriod"
        Me.pnlLimitTimePeriod.Size = New System.Drawing.Size(376, 22)
        Me.pnlLimitTimePeriod.TabIndex = 13
        Me.pnlLimitTimePeriod.Visible = False
        '
        'cboEnd
        '
        Me.cboEnd.FormattingEnabled = True
        Me.cboEnd.Location = New System.Drawing.Point(314, 0)
        Me.cboEnd.Name = "cboEnd"
        Me.cboEnd.Size = New System.Drawing.Size(53, 21)
        Me.cboEnd.TabIndex = 15
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(248, 2)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(66, 13)
        Me.Label14.TabIndex = 14
        Me.Label14.Text = "End = 9/30/"
        '
        'cboStart
        '
        Me.cboStart.FormattingEnabled = True
        Me.cboStart.Location = New System.Drawing.Point(179, 0)
        Me.cboStart.Name = "cboStart"
        Me.cboStart.Size = New System.Drawing.Size(53, 21)
        Me.cboStart.TabIndex = 13
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(113, 2)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(69, 13)
        Me.Label13.TabIndex = 12
        Me.Label13.Text = "Start = 10/1/"
        '
        'SelectDisplayData
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(960, 644)
        Me.Controls.Add(Me.pnlLimitTimePeriod)
        Me.Controls.Add(Me.pnlInterval)
        Me.Controls.Add(Me.btnMakeDefault)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "SelectDisplayData"
        Me.ShowIcon = False
        Me.Text = "Select data for display"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.pnlDailyFlow.ResumeLayout(False)
        Me.pnlDailyFlow.PerformLayout()
        Me.tcDisplays.ResumeLayout(False)
        Me.tpSpeciesHabitat.ResumeLayout(False)
        Me.pnlParameters.ResumeLayout(False)
        Me.pnlParameters.PerformLayout()
        Me.gbHydroperiod.ResumeLayout(False)
        Me.gbHydroperiod.PerformLayout()
        Me.tpOtherMetrics.ResumeLayout(False)
        Me.tpDisplayMetrics.ResumeLayout(False)
        Me.pnlInterval.ResumeLayout(False)
        Me.pnlInterval.PerformLayout()
        Me.pnlLimitTimePeriod.ResumeLayout(False)
        Me.pnlLimitTimePeriod.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents chkShowHydro As System.Windows.Forms.CheckBox
    Friend WithEvents tcDisplays As System.Windows.Forms.TabControl
    Friend WithEvents tpSpeciesHabitat As System.Windows.Forms.TabPage
    Friend WithEvents tpOtherMetrics As System.Windows.Forms.TabPage
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblScenario As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents tvRivers As System.Windows.Forms.TreeView
    Friend WithEvents tvSpecies As System.Windows.Forms.TreeView
    Friend WithEvents tvOtherMetrics As System.Windows.Forms.TreeView
    Friend WithEvents tvScenario As System.Windows.Forms.TreeView
    Friend WithEvents tvTreatment As System.Windows.Forms.TreeView
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents pnlParameters As System.Windows.Forms.Panel
    Friend WithEvents gbHydroperiod As System.Windows.Forms.GroupBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtStartMonth As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtEndDay As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtEndMonth As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtStartDay As System.Windows.Forms.TextBox
    Friend WithEvents btnSaveHydroPeriod As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents tpDisplayMetrics As System.Windows.Forms.TabPage
    Friend WithEvents tvDisplayMetrics As System.Windows.Forms.TreeView
    Friend WithEvents btnMakeDefault As System.Windows.Forms.Button
    Friend WithEvents lblDailyFlow As System.Windows.Forms.Label
    Friend WithEvents pnlDailyFlow As System.Windows.Forms.Panel
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents rbDaily As System.Windows.Forms.RadioButton
    Friend WithEvents rbYearly As System.Windows.Forms.RadioButton
    Friend WithEvents rbEntire As System.Windows.Forms.RadioButton
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents pnlInterval As System.Windows.Forms.Panel
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents pnlLimitTimePeriod As System.Windows.Forms.Panel
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents cboEnd As System.Windows.Forms.ComboBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents cboStart As System.Windows.Forms.ComboBox
    Friend WithEvents cboBaseline As System.Windows.Forms.ComboBox
    Friend WithEvents lblBaseline As System.Windows.Forms.Label
End Class
