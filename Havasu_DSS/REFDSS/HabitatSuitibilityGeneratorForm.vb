Imports System.Xml
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.IO
Imports System.Data.SQLite

Imports System.Diagnostics

Public Class HabitatSuitibilityGeneratorForm
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    Private parentMainForm As MainForm
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Private curFlow As String
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents btnRegenSpatialLayers As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtEquation As System.Windows.Forms.TextBox
    Friend WithEvents tcRegen As System.Windows.Forms.TabControl

    'Private chartdict As New Dictionary(Of String, Chart)
    'Private dgvdict As New Dictionary(Of String, DataGridView)
    Private fParser As New clsMathParser

    'Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
    Friend WithEvents cboLifestage As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cboSpecies As System.Windows.Forms.ComboBox
    Friend WithEvents lblSpecies As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Public spatialsenabled As Boolean = False

    Private curSpecies As String
    Private curLifestage As String
    Private curCovariate As String

    Public bLocked As Boolean = False
    Friend WithEvents btnRegenSingleSpecies As System.Windows.Forms.Button
    Dim progbar As habGenProgressBar

    Public globalColorDialog As ColorDialog

    Public AlertedUserToChangedHSC As Boolean = False

    Public Sub New(mf As MainForm)
        parentMainForm = mf
        Me.globalColorDialog = mf.globalColorDialog

        Me.InitializeComponent()
        progbar = New habGenProgressBar(mf)

        spatialsenabled = parentMainForm.mainDataManager.areOutputsProcessed()

        loadDisplaySelectors()
        updateHSCControl()
    End Sub

#Region "initialization"


    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HabitatSuitibilityGeneratorForm))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.btnRegenSingleSpecies = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.lblSpecies = New System.Windows.Forms.Label()
        Me.cboLifestage = New System.Windows.Forms.ComboBox()
        Me.cboSpecies = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnRegenSpatialLayers = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtEquation = New System.Windows.Forms.TextBox()
        Me.tcRegen = New System.Windows.Forms.TabControl()
        Me.Panel1.SuspendLayout()
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel1.Controls.Add(Me.ToolStripContainer1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(910, 744)
        Me.Panel1.TabIndex = 0
        '
        'ToolStripContainer1
        '
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.btnRegenSingleSpecies)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.TableLayoutPanel1)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.btnRegenSpatialLayers)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.Label6)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.txtEquation)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.tcRegen)
        Me.ToolStripContainer1.ContentPanel.Size = New System.Drawing.Size(906, 715)
        Me.ToolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ToolStripContainer1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.Size = New System.Drawing.Size(906, 740)
        Me.ToolStripContainer1.TabIndex = 4
        Me.ToolStripContainer1.Text = "ToolStripContainer1"
        '
        'btnRegenSingleSpecies
        '
        Me.btnRegenSingleSpecies.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnRegenSingleSpecies.BackColor = System.Drawing.Color.Green
        Me.btnRegenSingleSpecies.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnRegenSingleSpecies.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRegenSingleSpecies.ForeColor = System.Drawing.Color.White
        Me.btnRegenSingleSpecies.Location = New System.Drawing.Point(10, 667)
        Me.btnRegenSingleSpecies.Name = "btnRegenSingleSpecies"
        Me.btnRegenSingleSpecies.Size = New System.Drawing.Size(241, 38)
        Me.btnRegenSingleSpecies.TabIndex = 36
        Me.btnRegenSingleSpecies.Text = "Process THIS species/life stage"
        Me.btnRegenSingleSpecies.UseVisualStyleBackColor = False
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 4
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.lblSpecies, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.cboLifestage, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.cboSpecies, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 2, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 8)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(906, 24)
        Me.TableLayoutPanel1.TabIndex = 35
        '
        'lblSpecies
        '
        Me.lblSpecies.AutoSize = True
        Me.lblSpecies.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpecies.Location = New System.Drawing.Point(3, 0)
        Me.lblSpecies.Name = "lblSpecies"
        Me.lblSpecies.Size = New System.Drawing.Size(58, 16)
        Me.lblSpecies.TabIndex = 31
        Me.lblSpecies.Text = "Species"
        '
        'cboLifestage
        '
        Me.cboLifestage.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboLifestage.FormattingEnabled = True
        Me.cboLifestage.Location = New System.Drawing.Point(536, 3)
        Me.cboLifestage.Name = "cboLifestage"
        Me.cboLifestage.Size = New System.Drawing.Size(367, 21)
        Me.cboLifestage.TabIndex = 34
        '
        'cboSpecies
        '
        Me.cboSpecies.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboSpecies.FormattingEnabled = True
        Me.cboSpecies.Location = New System.Drawing.Point(83, 3)
        Me.cboSpecies.Name = "cboSpecies"
        Me.cboSpecies.Size = New System.Drawing.Size(367, 21)
        Me.cboSpecies.TabIndex = 32
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(459, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(68, 16)
        Me.Label1.TabIndex = 33
        Me.Label1.Text = "Life Stage"
        '
        'btnRegenSpatialLayers
        '
        Me.btnRegenSpatialLayers.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnRegenSpatialLayers.BackColor = System.Drawing.Color.Green
        Me.btnRegenSpatialLayers.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnRegenSpatialLayers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRegenSpatialLayers.ForeColor = System.Drawing.Color.White
        Me.btnRegenSpatialLayers.Location = New System.Drawing.Point(264, 667)
        Me.btnRegenSpatialLayers.Name = "btnRegenSpatialLayers"
        Me.btnRegenSpatialLayers.Size = New System.Drawing.Size(167, 38)
        Me.btnRegenSpatialLayers.TabIndex = 30
        Me.btnRegenSpatialLayers.Text = "Process ALL species"
        Me.btnRegenSpatialLayers.UseVisualStyleBackColor = False
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(3, 603)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(345, 20)
        Me.Label6.TabIndex = 29
        Me.Label6.Text = "Equation for Composite Habitat Suitability"
        '
        'txtEquation
        '
        Me.txtEquation.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEquation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEquation.Location = New System.Drawing.Point(7, 626)
        Me.txtEquation.Name = "txtEquation"
        Me.txtEquation.Size = New System.Drawing.Size(896, 21)
        Me.txtEquation.TabIndex = 28
        '
        'tcRegen
        '
        Me.tcRegen.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tcRegen.Location = New System.Drawing.Point(0, 38)
        Me.tcRegen.Name = "tcRegen"
        Me.tcRegen.SelectedIndex = 0
        Me.tcRegen.Size = New System.Drawing.Size(906, 551)
        Me.tcRegen.TabIndex = 0
        '
        'HabitatSuitibilityGeneratorForm
        '
        Me.ClientSize = New System.Drawing.Size(910, 744)
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "HabitatSuitibilityGeneratorForm"
        Me.Text = "Habitat Suitability Generator"
        Me.Panel1.ResumeLayout(False)
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.ContentPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Private Shared anInstance As HabitatSuitibilityGeneratorForm
    Public Shared ReadOnly Property Instance(mf) As HabitatSuitibilityGeneratorForm
        Get
            If anInstance Is Nothing Then
                anInstance = New HabitatSuitibilityGeneratorForm(mf)
            End If
            Return anInstance
        End Get
    End Property
    Private Sub HabitatSuitibilityGeneratorForm_FormClosing(ByVal sender As Object, _
     ByVal e As System.Windows.Forms.FormClosingEventArgs) _
      Handles Me.FormClosing
        'release the instance of this form
        anInstance = Nothing

    End Sub
#End Region

    Public Sub loadDisplaySelectors()
        'add our species and lifestages to the form dropdowns
        cboSpecies.Items.Clear()
        For Each species As String In parentMainForm.mainDataManager.getSpeciesNames()
            cboSpecies.Items.Add(species)
        Next

        RemoveHandler cboSpecies.SelectedIndexChanged, AddressOf cboSpecies_SelectedIndexChanged
        cboSpecies.SelectedIndex = 0
        curSpecies = cboSpecies.Text
        AddHandler cboSpecies.SelectedIndexChanged, AddressOf cboSpecies_SelectedIndexChanged

        updateLifeStages(cboSpecies.Text)

    End Sub

    Private Sub updateLifeStages(strSpecies As String)
        RemoveHandler cboLifestage.SelectedIndexChanged, AddressOf cboLifeStage_SelectedIndexChanged

        cboLifestage.Items.Clear()
        For Each lifestage In parentMainForm.mainDataManager.getLifeStageNames(strSpecies)
            cboLifestage.Items.Add(lifestage)
        Next

        cboLifestage.SelectedIndex = 0
        curLifestage = cboLifestage.Text
        AddHandler cboLifestage.SelectedIndexChanged, AddressOf cboLifeStage_SelectedIndexChanged

    End Sub

#Region "LoadTabs"
    Public Sub updateHSCControl()
        'updates the HSCControl graphs to display a new species/lifestage
        RemoveHandler (tcRegen.SelectedIndexChanged), AddressOf tcRegen_SelectedIndexChanged
        tcRegen.TabPages.Clear()

        For Each strVariable In parentMainForm.mainDataManager.variableNames(curSpecies, curLifestage)
            Dim tp As TabPage = genTabPage(curSpecies, curLifestage, strVariable)
            tcRegen.TabPages.Add(tp)
        Next

        tcRegen.SelectedTab = tcRegen.TabPages(0)
        curCovariate = tcRegen.SelectedTab.Text

        AddHandler (tcRegen.SelectedIndexChanged), AddressOf tcRegen_SelectedIndexChanged

        updateEquation()
    End Sub

    Private Sub updateEquation()
        RemoveHandler txtEquation.Validated, AddressOf txtEquation_TextChanged

        txtEquation.Text = parentMainForm.mainDataManager.getEquation(curSpecies, curLifestage)

        AddHandler txtEquation.Validated, AddressOf txtEquation_TextChanged

    End Sub

    Private Function genTabPage(strSpecies, strLifeStage, strVariable) As TabPage
        'creates a new tabpage for a given species, lifestage and variable
        curCovariate = strVariable


        Dim tp As New TabPage

        tp.Name = strVariable
        tp.Text = strVariable

        Dim splitPanel As New Windows.Forms.SplitContainer
        splitPanel.Dock = DockStyle.Fill
        splitPanel.Orientation = Orientation.Horizontal

        Dim HabitatChart As Chart = makeChart(strSpecies, strLifeStage, strVariable)
        Dim dgv As DataGridView = makeDGV(strSpecies, strLifeStage, strVariable)




        updateChartFromDGV(HabitatChart, dgv, strVariable)

        splitPanel.Panel1.Controls.Add(HabitatChart)
        splitPanel.Panel2.Controls.Add(dgv)

        tp.Controls.Add(splitPanel)

        Return tp

    End Function

    Private Function makeChart(strSpecies, strLifeStage, strVariable) As Chart

        Dim strUnits As String = MainForm.mainDataManager.curUnits

        Dim varChart As New Windows.Forms.DataVisualization.Charting.Chart
        varChart.Name = strVariable
        varChart.Dock = DockStyle.Fill
        varChart.AntiAliasing = AntiAliasingStyles.All
        varChart.BackColor = Color.LightGray
        varChart.BorderSkin.BorderWidth = 2
        varChart.ChartAreas.Add("Default")
        varChart.ChartAreas("Default").AxisX.IsMarginVisible = True
        varChart.ChartAreas("Default").AxisY.Title = "HSC Value"

        Dim strUnitsLabel As String = parentMainForm.mainDataManager.variableUnitsLabel(strVariable, strUnits)
        If strUnitsLabel <> "Unitless" Then
            varChart.ChartAreas("Default").AxisX.Title = strVariable + " (" + strUnitsLabel + ")"
        Else
            varChart.ChartAreas("Default").AxisX.Title = strVariable
        End If

        Dim xmax As Double = parentMainForm.mainDataManager.variableValue(strSpecies, strLifeStage, strVariable, "XMax")
        Dim xmin As Double = parentMainForm.mainDataManager.variableValue(strSpecies, strLifeStage, strVariable, "XMin")

        If strUnits <> "Metric" Then
            xmax = xmax * parentMainForm.mainDataManager.variableConversionFactor(strVariable)
            xmin = xmin * parentMainForm.mainDataManager.variableConversionFactor(strVariable)
        End If

        xmax = CInt(xmax)
        xmin = CInt(xmin)

        Dim interval As Integer = (xmax - xmin) / 8
        varChart.ChartAreas("Default").AxisX.Interval = interval

        varChart.ChartAreas("Default").AxisX.Minimum = xmin
        varChart.ChartAreas("Default").AxisX.Maximum = xmax
        varChart.ChartAreas("Default").AxisY.Minimum = parentMainForm.mainDataManager.variableValue(strSpecies, strLifeStage, strVariable, "YMin")
        varChart.ChartAreas("Default").AxisY.Maximum = parentMainForm.mainDataManager.variableValue(strSpecies, strLifeStage, strVariable, "YMax")
        varChart.ChartAreas("Default").AxisX.ScrollBar.Enabled = True



        varChart.Series.Add(strVariable)

        If parentMainForm.mainDataManager.isCovariateCategorical(strVariable) Then
            varChart.Series(strVariable).ChartType = SeriesChartType.Column

            varChart.Series(strVariable).MarkerBorderColor = Color.Black
            varChart.Series(strVariable).BorderColor = Color.Black
            varChart.Series(strVariable).BorderWidth = 1
            varChart.ChartAreas("Default").AxisX.IsLabelAutoFit = True
            'varChart.ChartAreas("Default").AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep30
            varChart.ChartAreas("Default").AxisX.LabelStyle.Enabled = True
        Else
            varChart.Series(strVariable).ChartType = SeriesChartType.StepLine
            varChart.Series(strVariable).Color = Color.FromArgb(250, 102, 164, 139)
            varChart.Series(strVariable).MarkerStyle = MarkerStyle.Circle
            varChart.Series(strVariable).MarkerColor = Color.Yellow
            varChart.Series(strVariable).MarkerBorderWidth = 1

            varChart.Series(strVariable).MarkerBorderColor = Color.Black
            varChart.Series(strVariable).BorderColor = Color.Black
            varChart.Series(strVariable).BorderWidth = 5
            varChart.ChartAreas("Default").AxisX.IsLabelAutoFit = False
        End If



        'If not parentMainForm.mainDataManager.isCovariateCategorical(tcRegen.SelectedTab.Text) Then
        '    HabitatChart.ChartAreas("Default").AxisX.Minimum = variableNode.SelectSingleNode("XMin").FirstChild.Value
        '    HabitatChart.ChartAreas("Default").AxisX.Maximum = variableNode.SelectSingleNode("XMax").FirstChild.Value
        'Else

        'End If

        varChart.ChartAreas("Default").AxisX.ScaleView.Zoom(xmin, xmax)

        AddHandler varChart.MouseMove, AddressOf HabitatChart_MouseMove
        AddHandler varChart.MouseUp, AddressOf HabitatChart_MouseUp
        AddHandler varChart.MouseDown, AddressOf HabitatChart_MouseDown

        Return varChart
    End Function

    Private Function makeDGV(strSpecies, strLifeStage, strVariable) As DataGridView

        Dim dgv As New DataGridView
        'dgv.Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Bottom
        dgv.Dock = DockStyle.Fill

        Dim isCategorical As Boolean = parentMainForm.mainDataManager.isCovariateCategorical(strVariable)
        If isCategorical Then
            dgv.ColumnCount = 4
            dgv.Columns(3).HeaderText = "x value"
            dgv.Columns(3).Visible = False
            dgv.Columns(3).ValueType = GetType(System.Double)
        Else
            dgv.ColumnCount = 3
            dgv.Columns(0).ValueType = GetType(System.Double)
            dgv.Columns(0).DefaultCellStyle.Format = "n3"
        End If

        dgv.Columns(0).HeaderText = "covariate value (x)"
        dgv.Columns(1).HeaderText = "HSC Value (y)"
        dgv.Columns(1).DefaultCellStyle.Format = "n2"
        dgv.Columns(2).HeaderText = "Map Color"

        dgv.AutoSize = True
        'dgv.Columns(0).HeaderText = "Start"

        dgv.Columns(0).SortMode = DataGridViewColumnSortMode.Automatic

        dgv.Columns(1).SortMode = DataGridViewColumnSortMode.Automatic

        'Add nodes to the chart acording to our hsc data in the config
        Dim hscNodes As XmlNodeList
        hscNodes = parentMainForm.mainDataManager.variableBreaks(strSpecies, strLifeStage, strVariable)
        dgv.RowCount = hscNodes.Count + 1
        Dim rowIndex, lastValue As Integer
        rowIndex = 0
        lastValue = 0



        For Each hscNode In hscNodes
            Dim backColor As Color = stringToColor(hscNode.SelectSingleNode("rgb").FirstChild.Value)
            If backColor.R = 0 And backColor.G = 0 And backColor.B = 0 Then
                backColor = Color.White
            End If

            If isCategorical Then
                'Add a point to the series in the chart
                dgv.Rows(rowIndex).Cells(0).Value = hscNode.SelectSingleNode("label").FirstChild.Value.Trim()
                dgv.Rows(rowIndex).Cells(1).Value = Str(CDbl(hscNode.SelectSingleNode("yValue").FirstChild.Value)).Trim()

                dgv.Rows(rowIndex).Cells(2).Style.BackColor = backColor
                dgv.Rows(rowIndex).Cells(3).Value = CDbl(hscNode.SelectSingleNode("value").FirstChild.Value.Trim())

                ''dgv.Rows(rowIndex).Cells(0).Value = CDbl(Str(CDbl(hscNode.SelectSingleNode("max").FirstChild.Value)).Trim()).ToString("##0")
                'dgv.Rows(rowIndex).Cells(0).Value = parentMainForm.mainDataManager.getCategoricalValueLabel(strVariable, Str(CDbl(hscNode.SelectSingleNode("max").FirstChild.Value)).Trim())
                'dgv.Rows(rowIndex).Cells(1).Value = Str(CDbl(hscNode.SelectSingleNode("yValue").FirstChild.Value)).Trim()
                'dgv.Rows(rowIndex).Cells(2).Style.BackColor = stringToColor(hscNode.SelectSingleNode("rgb").FirstChild.Value)
                ''dgv.Rows(rowIndex).Cells(3).Value = Str(CDbl(hscNode.SelectSingleNode("max").FirstChild.Value)).Trim()



            Else
                'Add a point to the series in the chart
                If MainForm.mainDataManager.curUnits = "Metric" Then
                    dgv.Rows(rowIndex).Cells(0).Value = CDbl(hscNode.SelectSingleNode("min").FirstChild.Value)
                Else
                    dgv.Rows(rowIndex).Cells(0).Value = CDbl(hscNode.SelectSingleNode("min").FirstChild.Value) * parentMainForm.mainDataManager.variableConversionFactor(strVariable)
                End If

                dgv.Rows(rowIndex).Cells(1).Value = Str(CDbl(hscNode.SelectSingleNode("yValue").FirstChild.Value)).Trim()
                dgv.Rows(rowIndex).Cells(2).Style.BackColor = backColor
            End If

            rowIndex += 1
        Next

        AddHandler (dgv.CellValidating), AddressOf DGV_CellValidiating
        AddHandler (dgv.RowValidated), AddressOf DGV_CellEndEdit
        AddHandler (dgv.CellDoubleClick), AddressOf DGV_CellDoubleClick

        Return dgv

    End Function
#End Region


    Private Sub tcRegen_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tcRegen.SelectedIndexChanged

        curCovariate = sender.selectedtab.text

        updateEquation()

        If tcRegen.TabPages(tcRegen.SelectedIndex).Name = "Distance To Cover HSC" And Not spatialsenabled Then
        Else
            'loadGISData()
        End If

    End Sub

    Private Sub txtEquation_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim goodformula As Boolean = fParser.StoreExpression(txtEquation.Text)
        fParser.OpAssignExplicit = True
        If goodformula Then
            parentMainForm.mainDataManager.setEquation(curSpecies, curLifestage, txtEquation.Text)
        Else
            MsgBox("There was a problem with your formula:" & vbCrLf & fParser.ErrorDescription)
        End If

    End Sub

    Private Sub updateChartFromDGV(HabitatChart As Chart, dgv As DataGridView, strVariable As String)

        HabitatChart.Series(0).Points.Clear()
        Dim r As DataGridViewRow

        Dim valuesDict As New Dictionary(Of Double, Double)

        Dim i As Integer = 0
        For Each r In dgv.Rows
            If Not IsNothing(r.Cells(0).Value) And Not IsNothing(r.Cells(1).Value) Then

                If parentMainForm.mainDataManager.isCovariateCategorical(strVariable) Then
                    HabitatChart.Series(0).Points.AddXY(CDbl(Nothing), r.Cells(1).Value)
                    HabitatChart.Series(0).Points(i).AxisLabel = r.Cells(0).Value
                    i += 1
                Else
                    If valuesDict.ContainsKey(r.Cells(0).Value) Then
                        valuesDict(r.Cells(0).Value) = r.Cells(1).Value
                    Else
                        valuesDict.Add(r.Cells(0).Value, r.Cells(1).Value)
                    End If

                End If
            End If
        Next

        Dim keys As List(Of Double) = valuesDict.Keys.ToList
        keys.Sort()
        For Each key As Double In keys
            HabitatChart.Series(0).Points.AddXY(key, valuesDict.Item(key))
        Next

        HabitatChart.Invalidate()
        'saveHSCtoConfig(dgv)

    End Sub

    Private Sub updateDGVFromChart(HabitatChart As Chart, dgv As DataGridView, strVariable As String)


        'Dim HabitatChart As Chart = tcRegen.SelectedTab.Controls.Item(0).Controls.Item(0).Controls.Item(0)
        'Dim dgv As DataGridView = tcRegen.SelectedTab.Controls.Item(0).Controls.Item(1).Controls.Item(0)

        RemoveHandler (dgv.CellValidating), AddressOf DGV_CellValidiating
        RemoveHandler (dgv.RowValidated), AddressOf DGV_CellEndEdit

        Dim clrs(dgv.Rows.Count - 1) As Color
        Dim i As Integer = 0
        For Each row As DataGridViewRow In dgv.Rows
            clrs(i) = dgv.Rows(i).Cells(2).Style.BackColor
            i += 1
        Next

        Dim isCategorical As Boolean = parentMainForm.mainDataManager.isCovariateCategorical(tcRegen.SelectedTab.Text)
        dgv.Rows.Clear()
        dgv.RowCount = HabitatChart.Series(0).Points.Count + 1

        Dim valuesDict As New Dictionary(Of Double, Double)

        For Each p As DataPoint In HabitatChart.Series(0).Points
            If isCategorical And p.AxisLabel <> "NA" Then
                Dim max As String = parentMainForm.mainDataManager.getCategoricalValueFromLabel(tcRegen.SelectedTab.Text, p.AxisLabel)
                valuesDict.Item(max) = Math.Round(p.YValues(0), 3)
            Else
                If valuesDict.ContainsKey(Math.Round(p.XValue, 3)) Then
                    If Math.Round(p.YValues(0), 3) > valuesDict(Math.Round(p.XValue, 3)) Then
                        valuesDict.Item(Math.Round(p.XValue, 3)) = Math.Round(p.YValues(0), 3)
                    End If
                Else

                    valuesDict.Add(Math.Round(p.XValue, 3), Math.Round(p.YValues(0), 3))
                End If
            End If
        Next

        Dim keys As List(Of Double) = valuesDict.Keys.ToList
        keys.Sort()
        Dim rowIndex As Integer = 0
        Dim x As String
        Dim y As String
        For Each key As Double In keys
            'HabitatChart.Series(0).Points.AddXY(key, valuesDict.Item(key))
            'Add a point to the series in the chart
            x = Str(CDbl(key)).Trim()
            y = CDbl(valuesDict.Item(key)).ToString("##0.0##")

            If isCategorical Then
                dgv.Rows(rowIndex).Cells(0).Value = parentMainForm.mainDataManager.getCategoricalValueLabel(tcRegen.SelectedTab.Text, key)
                dgv.Rows(rowIndex).Cells(1).Value = CDbl(valuesDict.Item(key)).ToString("##0.0##")
                dgv.Rows(rowIndex).Cells(2).Style.BackColor = clrs(rowIndex)
                dgv.Rows(rowIndex).Cells(3).Value = CDbl(key).ToString("##0.0##")

                'dgv.Rows(rowIndex).Cells(0).Value = CDbl(key).ToString("##0")
                'dgv.Rows(rowIndex).Cells(1).Value = CDbl(valuesDict.Item(key)).ToString("##0.0##")
                'dgv.Rows(rowIndex).Cells(0).Value = parentMainForm.mainDataManager.getCategoricalValueLabel(tcRegen.SelectedTab.Text, key)
                'dgv.Rows(rowIndex).Cells(2).Style.BackColor = clrs(rowIndex)
            Else
                dgv.Rows(rowIndex).Cells(0).Value = CDbl(key).ToString("##0.0##")
                dgv.Rows(rowIndex).Cells(1).Value = CDbl(valuesDict.Item(key)).ToString("##0.0##")
                dgv.Rows(rowIndex).Cells(2).Style.BackColor = clrs(rowIndex)
            End If

            rowIndex += 1
        Next



        AddHandler (dgv.CellValidating), AddressOf DGV_CellValidiating
        AddHandler (dgv.RowValidated), AddressOf DGV_CellEndEdit

        updateChartFromDGV(HabitatChart, dgv, tcRegen.SelectedTab.Text)

    End Sub

#Region "DataGridViewControl"
    Private Sub DGV_CellValidiating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs)

        Dim dgv As DataGridView = tcRegen.SelectedTab.Controls.Item(0).Controls.Item(1).Controls.Item(0)

        Dim cell As DataGridViewCell = dgv.Item(e.ColumnIndex, e.RowIndex)

        If cell.IsInEditMode Then
            Dim c As Control = dgv.EditingControl

            c.Text = CleanInputNumber(c.Text)
            If c.Text = "" Then c.Text = "0"
            If c.Text.StartsWith(".") Then c.Text = "0" & c.Text
        End If


    End Sub

    Private Function CleanInputNumber(ByVal str As String) As String
        Return System.Text.RegularExpressions.Regex.Replace(str, "[^-1234567890.]", "")
    End Function

    Private Sub DGV_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        If e.ColumnIndex = 2 Then
            sender(e.ColumnIndex, e.RowIndex).value = ""
        Else
            Dim dgv As DataGridView = sender
            RemoveHandler (dgv.RowValidated), AddressOf DGV_CellEndEdit

            If parentMainForm.mainDataManager.isCovariateCategorical(tcRegen.SelectedTab.Text) Then
                sender.Sort(sender.columns(3), System.ComponentModel.ListSortDirection.Ascending)
            Else
                sender.Sort(sender.columns(0), System.ComponentModel.ListSortDirection.Ascending)
            End If

            AddHandler (dgv.RowValidated), AddressOf DGV_CellEndEdit
            updateChartFromDGV(tcRegen.SelectedTab.Controls.Item(0).Controls.Item(0).Controls.Item(0), sender, tcRegen.SelectedTab.Text)

        End If
        saveHSCtoConfig(sender)
    End Sub

    Private Sub DGV_CellDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        If sender.columns(e.ColumnIndex).Headertext = "Map Color" Then
            globalColorDialog.ShowDialog()

            sender(e.ColumnIndex, e.RowIndex).style.backcolor = globalColorDialog.Color

            sender.clearSelection()
            saveHSCtoConfig(sender)
            'loadGISData()
        End If
    End Sub



#End Region

    Private Sub cboSpecies_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        curSpecies = cboSpecies.Text
        updateLifeStages(curSpecies)
        updateHSCControl()
        updateEquation()
        parentMainForm.mainMapManager.UpdateCovariateSymbology()
        'loadGISData()
    End Sub

    Private Sub cboLifeStage_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        curLifestage = cboLifestage.Text
        updateHSCControl()
        updateEquation()
        parentMainForm.mainMapManager.UpdateCovariateSymbology()
        'loadGISData()
    End Sub


#Region "RegenChartControl"
    Private selectedDataPoint As DataPoint
    Private Sub HabitatChart_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim HabitatChart As Chart
        HabitatChart = tcRegen.SelectedTab.Controls.Item(0).Controls.Item(0).Controls.Item(0)

        ' Call Hit Test Method
        Dim hitResult As HitTestResult = getCloseHitResult(HabitatChart, e.X, e.Y)

        ' Initialize currently selected data point
        selectedDataPoint = Nothing
        If hitResult.ChartElementType = ChartElementType.DataPoint Then
            selectedDataPoint = CType(hitResult.Object, DataPoint)

            ' Show point value as label
            selectedDataPoint.IsValueShownAsLabel = True

            ' Set cursor shape
            If parentMainForm.mainDataManager.isCovariateCategorical(tcRegen.SelectedTab.Text) Then
                HabitatChart.Cursor = Cursors.SizeNS
            Else
                HabitatChart.Cursor = Cursors.SizeAll
            End If

        End If
    End Sub 'HabitatChart_MouseDown

    Private Sub HabitatChart_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim HabitatChart As Chart
        HabitatChart = tcRegen.SelectedTab.Controls.Item(0).Controls.Item(0).Controls.Item(0)

        ' Check if data point selected
        If Not (selectedDataPoint Is Nothing) Then
            ' Mouse coordinates should not be outside of the chart 
            Dim coordinateX As Integer = e.X
            If coordinateX < 0 Then
                coordinateX = 0
            End If
            If coordinateX > HabitatChart.Size.Width - 1 Then
                coordinateX = HabitatChart.Size.Width - 1
            End If

            Dim coordinateY As Integer = e.Y
            If coordinateY < 0 Then
                coordinateY = 0
            End If
            If coordinateY > HabitatChart.Size.Height - 10 Then
                coordinateY = HabitatChart.Size.Height - 10
            End If

            ' Calculate new Y value from current cursor position
            Dim yValue As Double = HabitatChart.ChartAreas("Default").AxisY.PixelPositionToValue(coordinateY)
            yValue = Math.Min(yValue, HabitatChart.ChartAreas("Default").AxisY.Maximum)
            yValue = Math.Max(yValue, HabitatChart.ChartAreas("Default").AxisY.Minimum)

            ' Update selected point Y value
            selectedDataPoint.YValues(0) = yValue

            If Not parentMainForm.mainDataManager.isCovariateCategorical(tcRegen.SelectedTab.Text) Then
                'Calculate new x value from current cursor position
                Dim xValue As Double = HabitatChart.ChartAreas("Default").AxisX.PixelPositionToValue(coordinateX)
                xValue = Math.Min(xValue, HabitatChart.ChartAreas("Default").AxisX.Maximum)
                xValue = Math.Max(xValue, HabitatChart.ChartAreas("Default").AxisX.Minimum)
                selectedDataPoint.XValue = xValue
            End If

            ' Update selected point Y value
            selectedDataPoint.YValues(0) = yValue


            ' Invalidate chart
            HabitatChart.Invalidate()
        Else
            ' Set different shape of cursor over the data points
            Dim hitResult As HitTestResult = getCloseHitResult(HabitatChart, e.X, e.Y)

            If hitResult.ChartElementType = ChartElementType.DataPoint Then
                HabitatChart.Cursor = Cursors.Hand
                hitResult.Object.MarkerSize = 15

            Else
                HabitatChart.Cursor = Cursors.Default
                For Each point As DataPoint In HabitatChart.Series(0).Points
                    point.MarkerSize = 8
                Next
            End If
        End If


    End Sub 'HabitatChart_MouseMove

    Private Function getCloseHitResult(ByVal mChart As Chart, ByVal x As Integer, ByVal y As Integer)
        Dim hitResult As HitTestResult = Nothing
        Dim i, j As Integer

        Dim exitFor As Boolean = False

        For i = -10 To 10
            For j = -10 To 10
                hitResult = mChart.HitTest(x + i, y + j)
                If hitResult.ChartElementType = ChartElementType.DataPoint Then
                    exitFor = True
                    Exit For
                End If
            Next
            If exitFor Then Exit For
        Next

        Return hitResult

    End Function

    Private Sub HabitatChart_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim HabitatChart As Chart
        HabitatChart = tcRegen.SelectedTab.Controls.Item(0).Controls.Item(0).Controls.Item(0)

        ' Initialize currently selected data point
        If Not (selectedDataPoint Is Nothing) Then
            ' Hide point label
            selectedDataPoint.IsValueShownAsLabel = False

            ' reset selected object
            selectedDataPoint = Nothing

            ' Invalidate chart
            HabitatChart.Invalidate()

            ' Reset cursor style
            HabitatChart.Cursor = Cursors.Default

            updateDGVFromChart(sender, tcRegen.SelectedTab.Controls.Item(0).Controls.Item(1).Controls.Item(0), sender.parent.text)
            saveHSCtoConfig(tcRegen.SelectedTab.Controls.Item(0).Controls.Item(1).Controls.Item(0))
        End If


    End Sub 'HabitatChart_MouseUp


#End Region

    Private Sub saveHSCtoConfig(dgv As DataGridView)
        If Not AlertedUserToChangedHSC Then
            Dim msg As String
            msg = "You are about to change the stored Habitat Suitability Curve (HSC) values!"
            msg += vbCrLf + vbCrLf + "After you are done changing these values you will"
            msg += vbCrLf + " need to recalculate the habitat outputs."

            msg += vbCrLf + vbCrLf + "You can always return to the original values by selecting:"
            msg += vbCrLf + "'Reset all Defaults!' from the File menu."
            msg += vbCrLf + vbCrLf + "Are you sure you want to make this change?"
            Dim ans As VariantType = MsgBox(msg, MsgBoxStyle.YesNo, "Change HSC Values?")
            If ans = vbYes Then
                AlertedUserToChangedHSC = True
            Else
                Exit Sub
            End If
        End If


        parentMainForm.mainDataManager.removeBreakNodes(curSpecies, curLifestage, curCovariate)


        Dim isCategorical As Boolean = parentMainForm.mainDataManager.isCovariateCategorical(curCovariate)

        If isCategorical Then
            Dim rowIndex As Integer = 0
            While rowIndex < dgv.Rows.Count - 1
                Dim backColor As Color = dgv.Rows(rowIndex).Cells(2).Style.BackColor
                If backColor.R = 255 And backColor.G = 255 And backColor.B = 255 Then
                    backColor = Color.Black
                End If



                parentMainForm.mainDataManager.addCategoricalBreakNode(curSpecies, curLifestage, curCovariate,
                                           dgv.Rows(rowIndex).Cells(3).Value, dgv.Rows(rowIndex).Cells(1).Value,
                                           colorToString(backColor), dgv.Rows(rowIndex).Cells(0).Value)
                rowIndex += 1
            End While

        Else
            Dim rowIndex As Integer = 0
            Dim max As Double
            Dim curVal As Double
            While rowIndex < dgv.Rows.Count - 1
                Dim clr As Color = dgv.Rows(rowIndex).Cells(2).Style.BackColor

                If clr = Color.White Then
                    clr = Color.Black
                End If

                Try
                    curVal = dgv.Rows(rowIndex).Cells(0).Value
                    max = dgv.Rows(rowIndex + 1).Cells(0).Value

                    If MainForm.mainDataManager.curUnits <> "Metric" Then
                        max = max / parentMainForm.mainDataManager.variableConversionFactor(curCovariate)
                        curVal = curVal / parentMainForm.mainDataManager.variableConversionFactor(curCovariate)
                    End If

                Catch ex As Exception
                    max = "999999"
                End Try
                Dim backColor As Color = dgv.Rows(rowIndex).Cells(2).Style.BackColor
                If backColor.R = 255 And backColor.G = 255 And backColor.B = 255 Then
                    backColor = Color.Black
                End If

                parentMainForm.mainDataManager.addBreakNode(curSpecies, curLifestage, curCovariate,
                                                       curVal, max, dgv.Rows(rowIndex).Cells(1).Value,
                                                       colorToString(backColor))

                rowIndex += 1
            End While
        End If
        parentMainForm.mainMapManager.UpdateCovariateSymbology()

    End Sub

    Private Sub btnRegenSpatialLayers_Click(sender As System.Object, e As System.EventArgs) Handles btnRegenSpatialLayers.Click
        parentMainForm.mainDataManager.config.Save(My.Settings.ConfigXML)
        regenAll()
    End Sub

    Public Sub regenAll()
        Dim _speciesToProcess As List(Of String) = New List(Of String) _
        (New String() {"All"})
        Dim _lifeStageToProcess As List(Of String) = New List(Of String) _
        (New String() {"All"})
        progbar = New habGenProgressBar(parentMainForm)
        progbar._lifeStageToProcess = _lifeStageToProcess
        progbar._speciesToProcess = _speciesToProcess
        progbar.singleLS = False
        genspatialLayers()
    End Sub

    Private Sub btnRegenSingleSpecies_Click(sender As System.Object, e As System.EventArgs) Handles btnRegenSingleSpecies.Click
        parentMainForm.mainDataManager.config.Save(My.Settings.ConfigXML)

        Dim _speciesToProcess As List(Of String) = New List(Of String) _
        (New String() {cboSpecies.Text})
        Dim _lifeStageToProcess As List(Of String) = New List(Of String) _
        (New String() {cboLifestage.Text})
        progbar = New habGenProgressBar(parentMainForm)
        progbar._lifeStageToProcess = _lifeStageToProcess
        progbar._speciesToProcess = _speciesToProcess
        progbar.singleLS = True
        genspatialLayers()

    End Sub



    Private Sub genspatialLayers()
        Me.Cursor = Cursors.WaitCursor

        progbar.Show()
        progbar.runHabitatMaps()
        'lblGenSpatialDetails.Visible = True
        'lbGenSpatial.Visible = True
        'pbGenSpatial.Visible = True
        'lblLoadingData.Visible = False
        'pbLoadingData.Visible = False





        ' ''Step 2: Generate all of the 'preCalculate' layers that we will need
        ' ''   These are input covariates that are generated dynamically form a single input
        ' ''   In our case these are distance to shore
        'Dim covariates_singleFlow As New Dictionary(Of String, CovariateInfo)
        'genPreCalcs(covariates_singleFlow)
        'progbar.runHabitatMaps()





        spatialsenabled = True
        Me.Cursor = Cursors.Default
    End Sub








#Region "Serialization"


    Public Function saveToXMLNode() As XmlNode
        Dim outputNode As XmlNode
        outputNode = parentMainForm.mainDataManager.config.CreateElement("SmartRiverWidget")

        Dim nameNode As XmlNode
        nameNode = parentMainForm.mainDataManager.config.CreateElement("Name")
        nameNode.InnerText = Me.Text
        outputNode.AppendChild(nameNode)

        Dim lockedNode As XmlNode = parentMainForm.mainDataManager.config.CreateElement("Locked")
        lockedNode.InnerText = bLocked.ToString
        outputNode.AppendChild(lockedNode)

        Dim curSpeciesNode As XmlNode = parentMainForm.mainDataManager.config.CreateElement("curSpecies")
        curSpeciesNode.InnerText = curSpecies
        outputNode.AppendChild(curSpeciesNode)

        Dim curLSNode As XmlNode = parentMainForm.mainDataManager.config.CreateElement("curLifestage")
        curLSNode.InnerText = curLifestage
        outputNode.AppendChild(curLSNode)

        Return outputNode

    End Function

    Public Sub loadFromXmlNode(inNode As XmlNode)

        Dim dcNode As XmlNode
        dcNode = parentMainForm.mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & "']")
        curSpecies = dcNode.SelectSingleNode("curSpecies").InnerText
        cboSpecies.Text = curSpecies
        curLifestage = dcNode.SelectSingleNode("curLifestage").InnerText
        cboLifestage.Text = curLifestage

        'Dim optionNodes As XmlNodeList = parentMainForm.mainDataManager.config.SelectNodes("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & "']/*")
        'For Each optionNode As XmlNode In optionNodes
        '    If optionNode.Name <> "Locked" And
        '        optionNode.Name <> "Name" Then
        '        Dim checkedoptions As String() = optionNode.InnerText.Split("|")
        '        For Each strOption In checkedoptions
        '            Dim item As ToolStripMenuItem
        '            item = ContextMenuStrip1.Items(optionNode.Name.Replace(" ", "_"))
        '            Dim ddItem As ToolStripMenuItem
        '            If Not IsNothing(item) Then
        '                ddItem = item.DropDownItems(strOption.Replace(" ", "_"))
        '                If Not IsNothing(ddItem) Then
        '                    ddItem.Checked = True
        '                Else
        '                    Debug.Print(strOption + " is nothing")
        '                End If
        '            Else
        '                Debug.Print(optionNode.Name + " is nothing")
        '            End If
        '        Next
        '    End If
        'Next



    End Sub

    Public Sub refreshAfterLoad()

    End Sub

#End Region




End Class
