Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Data.SQLite
Imports System.Xml

Public Class HabitatGraphForm
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent
    Public WithEvents HabitatChart As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer

    Public mainGraphManager As GraphManager
    Public mainDataManager As DataManager

    Private components As System.ComponentModel.IContainer

    Public bLocked As Boolean = False
    Public bAcceptedNovelDate As Boolean = False
    Public dataChanged As Boolean = False

    Public curDisplayData As New chartDisplayData
    Public chartType As String = "HabitatGraph"

    Private WithEvents SelectData As New System.Windows.Forms.ToolStripMenuItem("Select Data")
    Private WithEvents ViewAggChart As New System.Windows.Forms.ToolStripMenuItem("View Aggregated Data Chart")
    Private WithEvents ViewData As New System.Windows.Forms.ToolStripMenuItem("View Tabular Data")
    Friend WithEvents lblUniqueDate As System.Windows.Forms.Label

    Private myContextMenuStrip As GenericChartContextMenuStrip

    Public Sub New(GM As GraphManager, DM As DataManager)
        mainGraphManager = GM
        mainDataManager = DM

        Me.InitializeComponent()
        'loadDisplaySelectors()
        loadData()

        myContextMenuStrip = New GenericChartContextMenuStrip(HabitatChart, Me)
        myContextMenuStrip.Items.Insert(1, SelectData)
        myContextMenuStrip.Items.Insert(2, ViewAggChart)
        myContextMenuStrip.Items.Insert(3, ViewData)
        Me.ContextMenuStrip = myContextMenuStrip
    End Sub


    Private Sub InitializeComponent()
        Dim CalloutAnnotation1 As System.Windows.Forms.DataVisualization.Charting.CalloutAnnotation = New System.Windows.Forms.DataVisualization.Charting.CalloutAnnotation()
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Title1 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HabitatGraphForm))
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.lblUniqueDate = New System.Windows.Forms.Label()
        Me.HabitatChart = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        CType(Me.HabitatChart, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ToolStripContainer1
        '
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.lblUniqueDate)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.HabitatChart)
        Me.ToolStripContainer1.ContentPanel.Size = New System.Drawing.Size(818, 719)
        Me.ToolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ToolStripContainer1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.Size = New System.Drawing.Size(818, 744)
        Me.ToolStripContainer1.TabIndex = 0
        Me.ToolStripContainer1.Text = "ToolStripContainer1"
        '
        'lblUniqueDate
        '
        Me.lblUniqueDate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblUniqueDate.AutoSize = True
        Me.lblUniqueDate.BackColor = System.Drawing.Color.White
        Me.lblUniqueDate.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUniqueDate.ForeColor = System.Drawing.Color.DarkRed
        Me.lblUniqueDate.Location = New System.Drawing.Point(550, 650)
        Me.lblUniqueDate.Name = "lblUniqueDate"
        Me.lblUniqueDate.Size = New System.Drawing.Size(256, 60)
        Me.lblUniqueDate.TabIndex = 3
        Me.lblUniqueDate.Text = "***   Changed Data or Date ***" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "*** Update labels as needed***" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "***       Click t" & _
    "o accept     ***"
        Me.lblUniqueDate.Visible = False
        '
        'HabitatChart
        '
        CalloutAnnotation1.AnchorAlignment = System.Drawing.ContentAlignment.TopLeft
        CalloutAnnotation1.AnchorOffsetX = 0.0R
        CalloutAnnotation1.AnchorOffsetY = 0.0R
        CalloutAnnotation1.AnchorX = 0.0R
        CalloutAnnotation1.AxisXName = "Default\rX"
        CalloutAnnotation1.BackColor = System.Drawing.Color.Transparent
        CalloutAnnotation1.Font = New System.Drawing.Font("Microsoft Sans Serif", 72.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        CalloutAnnotation1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(20, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        CalloutAnnotation1.LineColor = System.Drawing.Color.Transparent
        CalloutAnnotation1.Name = "DataChanged"
        CalloutAnnotation1.Text = "Changed Data"
        CalloutAnnotation1.Visible = False
        CalloutAnnotation1.YAxisName = "Default\rY"
        Me.HabitatChart.Annotations.Add(CalloutAnnotation1)
        Me.HabitatChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.HorizontalCenter
        ChartArea1.AxisX.Interval = 1.0R
        ChartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount
        ChartArea1.AxisX.IntervalOffset = 1.0R
        ChartArea1.AxisX.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Years
        ChartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Years
        ChartArea1.AxisX.LabelStyle.Format = "yyyy"
        ChartArea1.CursorX.IsUserEnabled = True
        ChartArea1.CursorX.IsUserSelectionEnabled = True
        ChartArea1.Name = "Default"
        Me.HabitatChart.ChartAreas.Add(ChartArea1)
        Me.HabitatChart.Dock = System.Windows.Forms.DockStyle.Fill
        Legend1.Name = "Legend1"
        Me.HabitatChart.Legends.Add(Legend1)
        Me.HabitatChart.Location = New System.Drawing.Point(0, 0)
        Me.HabitatChart.MinimumSize = New System.Drawing.Size(10, 10)
        Me.HabitatChart.Name = "HabitatChart"
        Me.HabitatChart.Size = New System.Drawing.Size(818, 719)
        Me.HabitatChart.TabIndex = 0
        Me.HabitatChart.Text = "HabitatChart"
        Title1.DockedToChartArea = "Default"
        Title1.Name = "Title"
        Title1.Text = "Title"
        Me.HabitatChart.Titles.Add(Title1)
        '
        'HabitatGraphForm
        '
        Me.ClientSize = New System.Drawing.Size(818, 744)
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "HabitatGraphForm"
        Me.Text = "Result Graph"
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.ContentPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        CType(Me.HabitatChart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Public Sub refreshDataDisplay()

    End Sub

    Public Sub loadData()

        HabitatChart.Series.Clear()
        'HabitatChart.Titles("Title").Text = "Habitat " + outputSpecies + " " + outputlifestage + " at " + curSegment

        If mainDataManager.curStartYear > curDisplayData.startYear Then
            curDisplayData.startYear = mainDataManager.curStartYear
        End If
        If mainDataManager.curEndYear < curDisplayData.endYear Then
            curDisplayData.endYear = mainDataManager.curEndYear
        End If

        mainDataManager.mainSQLDBConnection.Open()
        For Each outputScenario In curDisplayData.scenarios
            For Each outputTreatment As String In curDisplayData.treatments
                For Each outputRiver In curDisplayData.rivers
                    For Each outputSegment In outputRiver.Value
                        For Each outputSpecies In curDisplayData.species
                            For Each outputLifestage In outputSpecies.Value
                                For Each outputMetric In curDisplayData.displayMetrics
                                    Dim newSeries As New Series
                                    newSeries.Name = outputScenario & " " & outputTreatment & " " & outputRiver.Key & " " & outputSegment & " " & outputSpecies.Key & " " & outputLifestage.name & " " & outputMetric

                                    If mainDataManager.getHabitatMetricGraphType(outputMetric) = "Bar" Then
                                        newSeries.ChartType = SeriesChartType.Column
                                    Else
                                        newSeries.ChartType = SeriesChartType.Line
                                    End If

                                    HabitatChart.Series.Add(newSeries)

                                    Dim yrlyData As DataTable
                                    If outputRiver.Key = "aggAll" Then
                                        yrlyData = mainDataManager.getBasinWideHabitat(outputScenario, outputTreatment, outputRiver.Key, _
                                                                                                  outputSegment, outputSpecies.Key, outputLifestage.name, outputMetric, _
                                                                                                  outputLifestage.hydroPeriod, curDisplayData.startYear, curDisplayData.endYear)
                                    ElseIf outputSegment = "aggRiver" Then
                                        yrlyData = mainDataManager.getYearlyRiverHabitat(outputScenario, outputTreatment, outputRiver.Key, _
                                                                                                  outputSegment, outputSpecies.Key, outputLifestage.name, outputMetric, _
                                                                                                  outputLifestage.hydroPeriod, curDisplayData.startYear, curDisplayData.endYear)

                                    Else
                                        yrlyData = mainDataManager.getYearlySegmentHabitat(outputScenario, outputTreatment, outputRiver.Key, _
                                                                                                  outputSegment, outputSpecies.Key, outputLifestage.name, outputMetric, _
                                                                                                  outputLifestage.hydroPeriod, curDisplayData.startYear, curDisplayData.endYear)
                                    End If

                                    Dim dt As Date
                                    For Each r In yrlyData.Rows
                                        dt = DateSerial(r("WaterYear"), 1, 1)
                                        newSeries.Points.AddXY(dt, r("InterHabitat"))
                                    Next

                                    mainDataManager.symbolizeSeries(newSeries.Name, newSeries)


                                Next
                            Next
                        Next

                    Next
                Next
            Next




        Next

        mainDataManager.mainSQLDBConnection.Close()

        HabitatChart.ChartAreas(0).AxisY.Minimum = [Double].NaN
        HabitatChart.ChartAreas(0).AxisY.Maximum = [Double].NaN
        HabitatChart.ChartAreas(0).AxisY.Interval = [Double].NaN
        HabitatChart.ChartAreas(0).AxisX.Minimum = [Double].NaN
        HabitatChart.ChartAreas(0).AxisX.Maximum = [Double].NaN
        HabitatChart.ChartAreas(0).RecalculateAxesScale()

        addWarning(mainDataManager, dataChanged, bAcceptedNovelDate, HabitatChart, lblUniqueDate)

    End Sub


    Private Sub chart1_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles HabitatChart.MouseDown
        Dim result As HitTestResult = HabitatChart.HitTest(e.X, e.Y)
        If Not (result Is Nothing) And Not (result.Object Is Nothing) Then
            ' When user hits the LegendItem
            If TypeOf result.Object Is LegendItem Then
                ' Legend item result
                Dim legendItem As LegendItem = CType(result.Object, LegendItem)

                ' series item selected
                Dim selectedSeries As Series = HabitatChart.Series(legendItem.SeriesName)
                Dim sf As New SeriesFormater(selectedSeries, mainGraphManager)
                'sf.cboWidth.SelectedText = 4

                sf.StartPosition = FormStartPosition.Manual
                sf.Location = New Point(e.X + Get_Control_Location(Me).X, e.Y + Get_Control_Location(Me).Y)
                'sf.Location = Me.PointToScreen(New Point(e.X, e.Y))
                sf.Show()
                'If Not (selectedSeries Is Nothing) Then
                '    If selectedSeries.Enabled Then
                '        selectedSeries.Enabled = False

                '    Else
                '        selectedSeries.Enabled = True
                '    End If
                'End If
            End If
        End If
    End Sub


    Private Sub habitatGraphChart_AxisViewChanged(sender As Object, e As System.Windows.Forms.DataVisualization.Charting.ViewEventArgs) Handles HabitatChart.AxisViewChanged
        If Not bLocked Then
            mainGraphManager.syncExtents(Me.HabitatChart, True)
        End If

    End Sub 'HabitatChart_AxisViewChanged 

    Public Sub syncExtent(chrt As Chart)
        'This sub is called for each map by the MapManager for each of the child segment maps
        RemoveHandler HabitatChart.AxisViewChanged, AddressOf habitatGraphChart_AxisViewChanged
        HabitatChart.ChartAreas(0).AxisX.ScaleView = chrt.ChartAreas(0).AxisX.ScaleView
        'HabitatChart.ChartAreas(0).AxisY.ScaleView = e.ChartArea.AxisY.ScaleView

        AddHandler HabitatChart.AxisViewChanged, AddressOf habitatGraphChart_AxisViewChanged
    End Sub

    Private Sub HabitatGraphForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.FormClosing
        mainGraphManager.removeHabitatGraphForm(Me)
    End Sub


    Private Sub LockToolStripMenuItem_Click_1(sender As System.Object, e As System.EventArgs)
        If sender.text = "Lock" Then
            sender.text = "Unlock"
            Me.Text = Me.Text + " (*)"
            bLocked = True
        Else
            sender.text = "Lock"
            Me.Text = Me.Text.Substring(0, Len(Me.Text) - 4)
            bLocked = False
        End If
    End Sub

#Region "Serialization"


    Public Function saveToXMLNode() As XmlNode
        Dim outputNode As XmlNode
        outputNode = mainDataManager.config.CreateElement("SmartRiverWidget")

        Dim nameNode As XmlNode
        nameNode = mainDataManager.config.CreateElement("Name")
        nameNode.InnerText = Me.Text
        outputNode.AppendChild(nameNode)

        Dim lockedNode As XmlNode = mainDataManager.config.CreateElement("Locked")
        lockedNode.InnerText = bLocked.ToString
        outputNode.AppendChild(lockedNode)

        'Dim options As New Dictionary(Of String, ToolStripMenuItem)

        'For Each toolStripMenuItem In ContextMenuStrip1.Items
        '    Debug.Print(toolStripMenuItem.text)
        '    If toolStripMenuItem.text <> "lock" And
        '        toolStripMenuItem.text <> "Format Chart" And
        '        toolStripMenuItem.text <> "" Then
        '        options.Add(toolStripMenuItem.text, toolStripMenuItem)
        '    End If

        'Next
        ''options.Add("scenarios", curhydrographForm.ScenarioContextMenu)
        ''options.Add("segments", curhydrographForm.SegmentToolStripMenuItem1)
        ''options.Add("otherMetrics", curhydrographForm.OtherMetricToolStripMenuItem)

        'For Each key As String In options.Keys
        '    Dim checkedOptions As New List(Of String)
        '    For Each item In options(key).DropDownItems
        '        If item.checkstate = System.Windows.Forms.CheckState.Checked Then
        '            checkedOptions.Add(item.text)
        '        End If
        '    Next
        '    Dim checkedOptionNode As XmlNode = mainDataManager.config.CreateElement(key.Replace(" ", "_"))
        '    checkedOptionNode.InnerText = String.Join("|", checkedOptions)
        '    outputNode.AppendChild(checkedOptionNode)
        'Next

        outputNode.AppendChild(mainDataManager.serializeChartDisplayDataToXML(curDisplayData))
        outputNode.AppendChild(mainDataManager.serializeChartSymbologyToXML(HabitatChart))

        Return outputNode

    End Function

    Public Sub loadFromXmlNode(inNode As XmlNode)

        Dim dcNode As XmlNode
        dcNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & "']")
        If dcNode Is Nothing Then
            dcNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & " (*)" & "']")
            Me.Text = Me.Text + " (*)"
        End If
        bLocked = CBool(dcNode.SelectSingleNode("Locked").InnerText)
        Me.myContextMenuStrip.setLockState(bLocked)

        curDisplayData = mainDataManager.deserializeChartDisplayDataFromXML(dcNode.SelectSingleNode("chartDisplayData"))
        Dim chartSymbologyNode As XmlNode = dcNode.SelectSingleNode("Chart")
        If Not IsNothing(chartSymbologyNode) Then
            mainDataManager.symbolizeChartFromXML(HabitatChart, chartSymbologyNode)
        End If


    End Sub

    Private Sub SelectData_Click(sender As System.Object, e As System.EventArgs) Handles SelectData.Click
        Dim selData As New SelectDisplayData(Me, chartType)
        selData.Show()
    End Sub

    Private Sub ViewData_Click(sender As System.Object, e As System.EventArgs) Handles ViewData.Click
        Dim selData As HabitatGraphForm = mainDataManager.addRawDataForm(True)
        selData.curDisplayData = Me.curDisplayData
        selData.loadData()
    End Sub

    Private Sub ViewYearlyChart_Click(sender As System.Object, e As System.EventArgs) Handles ViewAggChart.Click
        Dim habChart As aggHabitatGraphForm = mainGraphManager.addAggHabitatGraphForm(True)

        habChart.curDisplayData = Me.curDisplayData
        habChart.curDisplayData.interval = "single"
        habChart.loadData()
    End Sub

    Public Sub refreshAfterLoad()
        loadData()
    End Sub

#End Region


    Private Sub lblUniqueDate_Click(sender As System.Object, e As System.EventArgs) Handles lblUniqueDate.Click
        lblUniqueDate.Visible = False
        HabitatChart.BorderlineDashStyle = ChartDashStyle.NotSet
        bAcceptedNovelDate = True
    End Sub
End Class
