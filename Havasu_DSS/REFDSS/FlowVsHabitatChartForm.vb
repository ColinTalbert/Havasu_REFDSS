Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Data.SQLite
Imports System.Xml

Public Class FlowVsHabitatChartForm
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent
    Public WithEvents HabitatChart As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer

    Public mainGraphManager As GraphManager
    Public mainDataManager As DataManager
    Private curMetric As String

    Private components As System.ComponentModel.IContainer

    Public bLocked As Boolean = False
    Public bAcceptedNovelDate As Boolean = False
    Public dataChanged As Boolean = False

    Public curDisplayData As New chartDisplayData
    Public chartType As String = "FlowVsHabitatChartForm"


    Private WithEvents SelectData As New System.Windows.Forms.ToolStripMenuItem("Select Data")
    Private WithEvents ViewYearlyData As New System.Windows.Forms.ToolStripMenuItem("View Yearly Data Chart")
    Private WithEvents ViewData As New System.Windows.Forms.ToolStripMenuItem("View Tabular Data")
    Friend WithEvents lblUniqueDate As System.Windows.Forms.Label
    Private myContextMenuStrip As GenericChartContextMenuStrip

    Public Sub New(GM As GraphManager, DM As DataManager)
        mainGraphManager = GM
        mainDataManager = DM

        Me.InitializeComponent()
        loadData()

        myContextMenuStrip = New GenericChartContextMenuStrip(HabitatChart, Me)
        myContextMenuStrip.Items.Insert(1, SelectData)
        myContextMenuStrip.Items.Insert(2, ViewYearlyData)
        myContextMenuStrip.Items.Insert(3, ViewData)
        Me.ContextMenuStrip = myContextMenuStrip
    End Sub


    Private Sub InitializeComponent()
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Title1 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
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
        Me.lblUniqueDate.TabIndex = 5
        Me.lblUniqueDate.Text = "***   Changed Data or Date ***" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "*** Update labels as needed***" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "***       Click t" & _
    "o accept     ***"
        Me.lblUniqueDate.Visible = False
        '
        'HabitatChart
        '
        Me.HabitatChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.HorizontalCenter
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
        'FlowVsHabitatChartForm
        '
        Me.ClientSize = New System.Drawing.Size(818, 744)
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpButton = True
        Me.Name = "FlowVsHabitatChartForm"
        Me.ShowIcon = False
        Me.Text = "Result Graph"
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.ContentPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        CType(Me.HabitatChart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Public Sub loadData()

        HabitatChart.Series.Clear()

        Dim strSeries As String = ""


        Dim lSeries As New List(Of String)

        For Each outputTreatment As String In curDisplayData.treatments
            For Each outputRiver In curDisplayData.rivers
                For Each outputSegment In outputRiver.Value
                    For Each outputSpecies In curDisplayData.species
                        For Each outputLifestage In outputSpecies.Value
                            Dim bpseriesName As String
                            If mainDataManager.checkSppsLifestageOccurance(outputTreatment, _
                                       outputSegment, outputSpecies.Key, outputLifestage.name) Then

                                bpseriesName = "habitat vs discharge for: " & outputTreatment & " " & outputRiver.Key & " " & outputSegment & " " & outputSpecies.Key & " " & outputLifestage.name

                                Dim dt As DataTable
                                dt = mainDataManager.getFlowVsHabitat(outputRiver.Key, outputSegment, outputTreatment, outputSpecies.Key, outputLifestage.name)

                                Dim newSeries As New Series
                                newSeries.Name = bpseriesName
                                newSeries.Tag = bpseriesName
                                newSeries.ChartType = SeriesChartType.FastLine

                                mainDataManager.symbolizeSeries(bpseriesName, newSeries)

                                'If Not bRightAxis Then
                                '    hydrographChart.ChartAreas(0).AxisY2.TextOrientation = TextOrientation.Rotated270
                                '    newSeries.YAxisType = AxisType.Secondary
                                '    hydrographChart.ChartAreas(0).AxisY2.LabelStyle.Format = "#,###,###,"
                                '    hydrographChart.ChartAreas(0).AxisY2.Title = AxisLabel(seriesName)
                                '    'hydrographChart.ChartAreas(0).AxisY2.TitleFont = New Font(hydrographChart.ChartAreas(0).AxisY.TitleFont.FontFamily, CInt(mainDataManager.getFont("YAxisTitle")), FontStyle.Regular)

                                'End If
                                newSeries.Points.DataBindXY(dt.DefaultView, "discharge", dt.DefaultView, "habitat")
                                HabitatChart.Series.Add(newSeries)

                            End If
                        Next
                    Next
                Next

            Next
        Next

        If HabitatChart.Series.Count > 0 Then

            HabitatChart.ChartAreas(0).AxisY.Minimum = [Double].NaN
            HabitatChart.ChartAreas(0).AxisY.Maximum = [Double].NaN
            HabitatChart.ChartAreas(0).AxisY.Interval = [Double].NaN

            HabitatChart.ChartAreas(0).AxisX.Minimum = [Double].NaN
            HabitatChart.ChartAreas(0).AxisX.Maximum = [Double].NaN
            HabitatChart.ChartAreas(0).AxisX.Interval = [Double].NaN
            HabitatChart.ChartAreas(0).RecalculateAxesScale()
        End If

        addWarning(mainDataManager, dataChanged, bAcceptedNovelDate, HabitatChart, lblUniqueDate)
    End Sub


    'Private Sub habitatChart_getToolTipText(sender As Object, e As System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs) Handles HabitatChart.GetToolTipText
    '    Select Case e.HitTestResult.ChartElementType
    '        Case ChartElementType.Axis
    '            e.Text = e.HitTestResult.Axis.Name
    '        Case ChartElementType.DataPoint
    '            e.Text = "Data Point " + e.HitTestResult.PointIndex.ToString()
    '        Case ChartElementType.Gridlines
    '            e.Text = "Grid Lines"
    '        Case ChartElementType.LegendArea
    '            e.Text = "Legend Area"
    '        Case ChartElementType.LegendItem
    '            e.Text = "Legend Item"
    '        Case ChartElementType.PlottingArea
    '            e.Text = "Plotting Area"
    '        Case ChartElementType.StripLines
    '            e.Text = "Strip Lines"
    '        Case ChartElementType.TickMarks
    '            e.Text = "Tick Marks"
    '        Case ChartElementType.Title
    '            e.Text = "Title"
    '    End Select
    'End Sub
    Private Function getPercentValue(valList As List(Of Double), percentage As Double)
        If valList.Count = 0 Then
            Return -1
        Else
            Dim index As Integer = percentage * valList.Count / 100
            If index = valList.Count Then
                index -= 1
            End If
            Return valList(index)
        End If
    End Function

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

                sf.StartPosition = FormStartPosition.Manual
                sf.Location = New Point(e.X + Get_Control_Location(Me).X, e.Y + Get_Control_Location(Me).Y)
                sf.Show()

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

        AddHandler HabitatChart.AxisViewChanged, AddressOf habitatGraphChart_AxisViewChanged
    End Sub

    Private Sub HabitatGraphForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.FormClosing
        mainGraphManager.removeFlowVsHabitatChartForm(Me)
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
