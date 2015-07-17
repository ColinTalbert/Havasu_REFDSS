Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SQLite
Imports System.Xml
Imports System.IO

Public Class HydrographGraphForm
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    Public WithEvents hydrographChart As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    'Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer

    Public mainHydrographManager As HydrographManager
    Public mainDataManager As DataManager
    Private segmentLookup As New Dictionary(Of String, String)

    Public bLocked As Boolean = False
    Public bAcceptedNovelDate As Boolean = False
    Public dataChanged As Boolean = False
    Private components As System.ComponentModel.IContainer
    Public bSelectMode As Boolean = False

    Public strSpecies As String
    Public curLifestage As String
    Public curPersistenceLength As Integer = 10


    Public curDisplayData As New chartDisplayData
    Public curCursorType As String = "zoom"

    Public chartType As String = "Hydrograph"

    Private myContextMenuStrip As GenericChartContextMenuStrip
    Private WithEvents SelectData As New System.Windows.Forms.ToolStripMenuItem("Select Data")
    Private hydroDatatable As New DataTable
    Friend WithEvents lblUniqueDate As System.Windows.Forms.Label

    Private WithEvents ViewData As New System.Windows.Forms.ToolStripMenuItem("View Tabular Data")


    Public Sub New(hgm As HydrographManager)
        mainHydrographManager = hgm
        mainDataManager = hgm.parentMainForm.mainDataManager
        Me.InitializeComponent()

        myContextMenuStrip = New GenericChartContextMenuStrip(hydrographChart, Me)
        Me.ContextMenuStrip = myContextMenuStrip
        'Add the custom select data menu strip item that only applies to hydrographCharts
        Me.ContextMenuStrip.Items.Insert(1, SelectData)
        myContextMenuStrip.Items.Insert(2, ViewData)

    End Sub

    Private Sub InitializeComponent()
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HydrographGraphForm))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblUniqueDate = New System.Windows.Forms.Label()
        Me.hydrographChart = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.Panel1.SuspendLayout()
        CType(Me.hydrographChart, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel1.Controls.Add(Me.lblUniqueDate)
        Me.Panel1.Controls.Add(Me.hydrographChart)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.ForeColor = System.Drawing.Color.DarkRed
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1242, 328)
        Me.Panel1.TabIndex = 0
        '
        'lblUniqueDate
        '
        Me.lblUniqueDate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblUniqueDate.AutoSize = True
        Me.lblUniqueDate.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUniqueDate.Location = New System.Drawing.Point(972, 257)
        Me.lblUniqueDate.Name = "lblUniqueDate"
        Me.lblUniqueDate.Size = New System.Drawing.Size(256, 60)
        Me.lblUniqueDate.TabIndex = 2
        Me.lblUniqueDate.Text = "***   Changed Data or Date ***" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "*** Update labels as needed***" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "***       Click t" & _
    "o accept     ***"
        Me.lblUniqueDate.Visible = False
        '
        'hydrographChart
        '
        Me.hydrographChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.HorizontalCenter
        Me.hydrographChart.BackSecondaryColor = System.Drawing.Color.White
        Me.hydrographChart.BorderlineWidth = 10
        Me.hydrographChart.BorderSkin.BackColor = System.Drawing.Color.White
        Me.hydrographChart.BorderSkin.BorderWidth = 2
        ChartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount
        ChartArea1.AxisX.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days
        ChartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days
        ChartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.WhiteSmoke
        ChartArea1.AxisX.MaximumAutoSize = 85.0!
        ChartArea1.AxisX2.MaximumAutoSize = 85.0!
        ChartArea1.AxisY.LabelAutoFitMinFontSize = 15
        ChartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.WhiteSmoke
        ChartArea1.AxisY.MaximumAutoSize = 85.0!
        ChartArea1.AxisY.TitleFont = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ChartArea1.AxisY2.MaximumAutoSize = 85.0!
        ChartArea1.CursorX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days
        ChartArea1.CursorX.IsUserEnabled = True
        ChartArea1.CursorX.IsUserSelectionEnabled = True
        ChartArea1.CursorY.LineColor = System.Drawing.Color.Transparent
        ChartArea1.Name = "Default"
        Me.hydrographChart.ChartAreas.Add(ChartArea1)
        Me.hydrographChart.Dock = System.Windows.Forms.DockStyle.Fill
        Legend1.Name = "Legend1"
        Me.hydrographChart.Legends.Add(Legend1)
        Me.hydrographChart.Location = New System.Drawing.Point(0, 0)
        Me.hydrographChart.Name = "hydrographChart"
        Series1.BorderWidth = 2
        Series1.ChartArea = "Default"
        Series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series1.Legend = "Legend1"
        Series1.Name = "Series1"
        Me.hydrographChart.Series.Add(Series1)
        Me.hydrographChart.Size = New System.Drawing.Size(1238, 324)
        Me.hydrographChart.TabIndex = 1
        Me.hydrographChart.Text = "HabitatChart"
        '
        'HydrographGraphForm
        '
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1242, 328)
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ForeColor = System.Drawing.Color.Black
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "HydrographGraphForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.TabText = ""
        Me.Text = "Hydrograph"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.hydrographChart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Private Sub hydrographChart_AxisViewChanged(sender As Object, e As System.Windows.Forms.DataVisualization.Charting.ViewEventArgs) Handles hydrographChart.AxisViewChanged
        If Not bLocked Then
            mainHydrographManager.syncExtents(Me.hydrographChart, True)
        End If

        showStripLineLabels()

    End Sub 'HabitatChart_AxisViewChanged 

    Public Sub syncExtent(chrt As Chart)
        'This sub is called for each map by the MapManager for each of the child segment maps
        RemoveHandler hydrographChart.AxisViewChanged, AddressOf hydrographChart_AxisViewChanged
        hydrographChart.ChartAreas(0).AxisX.ScaleView = chrt.ChartAreas(0).AxisX.ScaleView
        'hydrographChart.ChartAreas(0).AxisY.ScaleView = chrt.ChartAreas(0).AxisY.ScaleView

        AddHandler hydrographChart.AxisViewChanged, AddressOf hydrographChart_AxisViewChanged
    End Sub

    Private Sub HydrographGraphForm_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        mainHydrographManager.removeHydrograph(Me)
    End Sub

    Private Sub HydrographGraphForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.FormClosing
        mainHydrographManager.removeHydrograph(Me)
    End Sub

#Region "Data"

    Private Sub addSeries(scenario As String, segment As String)

        Dim segmentAbbrev As String = mainDataManager.getSegmentAbbrev(segment)

        ' Define the database query    
        Dim mySelectQuery As String = "SELECT Date, " + segmentAbbrev + " FROM scenario_" + scenario + ";"
        ' Create a database connection object using the connection string    
        mainDataManager.mainSQLDBConnection.Open()

        Dim dt As New DataTable
        Dim sqlda As New SQLite.SQLiteDataAdapter(mySelectQuery, mainDataManager.mainSQLDBConnection)
        sqlda.Fill(dt)

        Dim sqlcommand As SQLite.SQLiteCommand
        sqlcommand = mainDataManager.mainSQLDBConnection.CreateCommand
        sqlcommand.CommandText = mySelectQuery
        Dim sqlreader As SQLite.SQLiteDataReader = sqlcommand.ExecuteReader()

        Dim seriesName As String = scenario + "_" + segment

        hydrographChart.Series("Series1").Points.DataBindXY(dt.DefaultView, "Date", dt.DefaultView, segmentAbbrev)


        'hydrographChart.DataSource = dt
        'hydrographChart.Series("Series1").XValueMember = "Date"
        'hydrographChart.Series("Series1").YValueMembers = segmentAbbrev
        mainDataManager.mainSQLDBConnection.Close()

    End Sub

    Public Sub loadData()
        hydrographChart.Series.Clear()
        hydrographChart.ChartAreas(0).AxisX.StripLines.Clear()

        If mainDataManager.curStartYear > curDisplayData.startYear Then
            curDisplayData.startYear = mainDataManager.curStartYear
        End If
        If mainDataManager.curEndYear < curDisplayData.endYear Then
            curDisplayData.endYear = mainDataManager.curEndYear
        End If

        For Each scenario In curDisplayData.scenarios
            loadNonHabitatForScenario(scenario)
            loadHabitatForScenario(scenario)
        Next
        '    If item.checkstate = System.Windows.Forms.CheckState.Checked Then
        '        loadDataForScenario(item.text)
        '    End If

        'Next

        addWarning(mainDataManager, dataChanged, bAcceptedNovelDate, hydrographChart, lblUniqueDate)

    End Sub

    Private Sub loadNonHabitatForScenario(Scenario)


        Dim getRowListSQL As String = "SELECT * FROM scenario_UnitConversions"
        mainDataManager.mainSQLDBConnection.Open()
        Dim sqlDARowList As New SQLite.SQLiteDataAdapter(getRowListSQL, mainDataManager.mainSQLDBConnection)
        Dim RowListDatatable As New DataTable
        sqlDARowList.Fill(RowListDatatable)
        mainDataManager.mainSQLDBConnection.Close()

        ' Define the database query
        Dim mySelectSQL As String = "SELECT Date, "
        For Each row In RowListDatatable.Rows
            If mainDataManager.curUnits = "Metric" Then
                mySelectSQL += row(0) + ", "
            Else
                mySelectSQL += row(0) + " * " + mainDataManager.getConversionFactor(row(1)) + " as " + row(0) + ", "
            End If
        Next





        For Each river As String In mainDataManager.getRiverNames()
            Dim seg_abrevs As List(Of String) = mainDataManager.getSegmentAbrevsForRiver(river)
            Dim SQL_ave As String = "("
            SQL_ave += String.Join(" + ", seg_abrevs)
            If mainDataManager.curUnits = "Metric" Then
                SQL_ave += ")/" + CStr(seg_abrevs.Count)
            Else
                SQL_ave += " * " + mainDataManager.getConversionFactor("MetersToFeet") + ")/" + CStr(seg_abrevs.Count)
            End If

            SQL_ave += " as " + mainDataManager.getRiverAbbrev(river) + "_ave"
            mySelectSQL += SQL_ave

        Next
        mySelectSQL += " FROM scenario_" + Scenario
        mySelectSQL += " WHERE Date>=date('" + CStr(curDisplayData.startYear - 1) + "-10-01') and DATE<('" + CStr(curDisplayData.endYear) + "-10-01');"



        mainDataManager.mainSQLDBConnection.Open()
        hydroDatatable.Clear()
        Dim sqlDA As New SQLite.SQLiteDataAdapter(mySelectSQL, mainDataManager.mainSQLDBConnection)
        sqlDA.Fill(hydroDatatable)
        mainDataManager.mainSQLDBConnection.Close()

        For Each river In curDisplayData.rivers.Keys
            Dim segments As List(Of String) = curDisplayData.rivers(river)
            For Each seg In segments
                If seg <> "aggRiver" Then
                    Dim seriesName As String = Scenario + "  " + seg + "_Q"
                    Dim seriesTag As String = "Q|" + Scenario + "|" + seg
                    If curDisplayData.showHydro Then
                        loadHydroSeries(hydroDatatable, seriesName, mainDataManager.getSegmentAbbrev(seg), seriesTag, True)
                    End If
                Else 'segments is nothing, this is an average
                    Dim rivName As String = river.Replace("Aggregate across the ", "")
                    Dim rivAbbrev As String = mainDataManager.getRiverAbbrev(rivName)
                    loadHydroSeries(hydroDatatable, "Average of " + rivName + " segments", rivAbbrev + "_ave", rivAbbrev + "_ave", True)
                End If
            Next
        Next

        For Each item In curDisplayData.otherMetrics

            Dim seriesName As String = Scenario + "  " + item
            loadHydroSeries(hydroDatatable, seriesName, item, item, False)

        Next
    End Sub

    Private Sub loadHabitatForScenario(scenario)
        For Each sps In curDisplayData.species.Keys
            Dim lifestages As List(Of lifestage) = curDisplayData.species(sps)
            If IsNothing(lifestages) Then
                'We might eventually add something here
            Else
                For Each ls In curDisplayData.species(sps)
                    loadHabitatSeries(scenario, sps, ls)
                Next
            End If

        Next
    End Sub

    Private Sub loadDataForScenario(scenario As String)

        hydrographChart.ChartAreas(0).RecalculateAxesScale()
        hydrographChart.ChartAreas(0).AxisY2.Minimum = [Double].NaN
        hydrographChart.ChartAreas(0).AxisY2.Maximum = [Double].NaN
    End Sub

    Private Sub loadHydroSeries(ByRef dt As DataTable, seriesName As String,
                           xFieldName As String, seriesTag As String, bRightAxis As Boolean)
        Dim newSeries As New Series
        newSeries.Name = seriesName
        newSeries.Tag = seriesTag
        newSeries.ChartType = SeriesChartType.FastLine

        mainDataManager.symbolizeSeries(seriesName, newSeries)

        If Not bRightAxis Then
            hydrographChart.ChartAreas(0).AxisY2.TextOrientation = TextOrientation.Rotated270
            newSeries.YAxisType = AxisType.Secondary
            hydrographChart.ChartAreas(0).AxisY2.LabelStyle.Format = "#,###,###,"
            hydrographChart.ChartAreas(0).AxisY2.Title = AxisLabel(seriesName)
            'hydrographChart.ChartAreas(0).AxisY2.TitleFont = New Font(hydrographChart.ChartAreas(0).AxisY.TitleFont.FontFamily, CInt(mainDataManager.getFont("YAxisTitle")), FontStyle.Regular)

        End If
        newSeries.Points.DataBindXY(dt.DefaultView, "Date", dt.DefaultView, xFieldName)
        newSeries.ToolTip = "#SERIESNAME" & vbCrLf & "#VALX" & vbCrLf & "#VALY"


        Try
            hydrographChart.Series.Add(newSeries)
        Catch ex As Exception

        End Try


        SetChartTransparency(Me.hydrographChart, newSeries.Name)
    End Sub

    Private Sub loadHabitatSeries(strScenario As String, strSpecies As String, curLifeStage As lifestage)
        Dim curMetric As String = "Daily Habitat Series"

        For Each riv In curDisplayData.rivers.Keys
            Dim segs As List(Of String) = curDisplayData.rivers(riv)
            If IsNothing(segs) Then
                'Handle aggregated habitat
            Else
                For Each seg In segs
                    Dim treatments As List(Of String) = curDisplayData.treatments
                    For Each treatment As String In treatments
                        Dim curSegment As String = seg

                        Dim newSeries As New Series
                        newSeries.ToolTip = "#VAL"
                        Dim seriesName As String
                        seriesName = ""
                        If mainDataManager.getTreatmentNames().Count > 1 Then
                            seriesName += treatment + " "
                        End If
                        If mainDataManager.getScenarioNames().Count > 1 Then
                            seriesName += strScenario + " "
                        End If
                        If mainDataManager.getSegmentNames().Count > 1 Then
                            seriesName += seg + " "
                        End If
                        seriesName += strSpecies + " " + curLifeStage.name
                        newSeries.Name = seriesName
                        newSeries.Tag = seriesName

                        newSeries.ChartType = SeriesChartType.FastLine

                        mainDataManager.symbolizeSeries(seriesName, newSeries)

                        hydrographChart.Series.Add(newSeries)
                        Dim dTable As New DataTable
                        dTable = mainDataManager.getDailyHabitatData(strScenario, treatment, riv, seg, strSpecies, curLifeStage, curDisplayData.startYear, curDisplayData.endYear)


                        If dTable.Rows.Count > 0 Then
                            newSeries.Points.DataBindXY(dTable.DefaultView, "DateVal", dTable.DefaultView, "InterHabitat")
                            newSeries.EmptyPointStyle.MarkerStyle = MarkerStyle.None
                            newSeries.EmptyPointStyle.BorderWidth = 0
                            newSeries.YAxisType = AxisType.Secondary
                            hydrographChart.DataManipulator.InsertEmptyPoints(1, IntervalType.Months, newSeries)
                            hydrographChart.SuppressExceptions = True
                            hydrographChart.ChartAreas(0).RecalculateAxesScale()
                        End If

                        Dim hperiod As hydroperiod = curLifeStage.hydroPeriod
                        Dim intYearModifier As String = 0
                        If hperiod.startMonth * 100 + hperiod.startDay > hperiod.endMonth * 100 + hperiod.endDay Then
                            intYearModifier = 1
                        End If
                        Dim startdate As DateTime = DateSerial(2000, hperiod.startMonth, hperiod.startDay)
                        Dim enddate As DateTime = DateSerial(2000 + intYearModifier, hperiod.endMonth, hperiod.endDay)
                        addStripLines(startdate, enddate, curLifeStage.name + " hydroperiod")
                    Next
                Next
            End If
        Next
    End Sub

#End Region

#Region "symbology"

    Private Function AxisLabel(strTitle As String) As String
        If strTitle.EndsWith("_Target") Or strTitle.EndsWith("Diversion_Q") Then
            Return "Millions of gallons per day"
        ElseIf strTitle.EndsWith("_Release") Then
            Return "Release (cfs)"
        ElseIf strTitle.EndsWith("_Storage") Then
            Return "Storage volume, in billions of gallons"
        ElseIf strTitle.EndsWith("_Spill") Then
            Return "Spill (cfs)"
        ElseIf strTitle.EndsWith("_Q") Then
            Return "Discharge (cfs)"
        ElseIf strTitle.EndsWith("Drought_Condition") Then
            Return "Drought Condition"
        Else
            Return strTitle
        End If
    End Function

    Public Sub addStripLines(StartDay As Date, EndDay As Date, title As String)

        removeStripLine(title)

        Dim strpColor As Color
        If title = "spawning hydroperiod" Then
            strpColor = Color.LightPink
        ElseIf title = "incubation hydroperiod" Then
            strpColor = Color.LightYellow
        ElseIf title = "juvenile hydroperiod" Then
            strpColor = Color.LightCyan
        ElseIf title = "adult hydroperiod" Then
            strpColor = Color.LightGreen
        ElseIf title = "NA hydroperiod" Then
            strpColor = Color.LightGray
        ElseIf title = "breeding hydroperiod" Then
            strpColor = Color.FromArgb(227, 182, 46)
        ElseIf title = "nesting hydroperiod" Then
            strpColor = Color.FromArgb(117, 186, 202)
        ElseIf title = "molting hydroperiod" Then
            strpColor = Color.FromArgb(218, 122, 146)
        Else
            strpColor = Color.Fuchsia
        End If


        Dim hydroperiodLength As Integer = DateDiff(DateInterval.Day, StartDay, EndDay)
        Dim modStartDay As DateTime = StartDay '.AddDays(-1)
        Dim hydrostart As Integer = DateDiff(DateInterval.Day, DateSerial(Year(StartDay), 1, 1), modStartDay)

        Dim stripLine As New StripLine()
        stripLine.BackColor = strpColor
        stripLine.StripWidth = hydroperiodLength
        stripLine.StripWidthType = DateTimeIntervalType.Days

        stripLine.IntervalOffset = hydrostart
        stripLine.IntervalOffsetType = DateTimeIntervalType.Days

        stripLine.Interval = 1
        stripLine.IntervalType = DateTimeIntervalType.Years

        stripLine.TextLineAlignment = StringAlignment.Far
        stripLine.ForeColor = Color.DarkRed
        stripLine.ToolTip = title

        stripLine.Tag = title
        'stripLine.BackHatchStyle = ChartHatchStyle.Percent50


        'hydrographChart.ChartAreas(0).AxisX.StripLines.Clear()
        hydrographChart.ChartAreas(0).AxisX.StripLines.Add(stripLine)

        showStripLineLabels()
    End Sub

    Public Sub removeStripLine(strTag As String)
        Dim removeme As StripLine = Nothing
        For Each strplines As StripLine In hydrographChart.ChartAreas(0).AxisX.StripLines

            If strplines.Tag = strTag Then
                removeme = strplines
            End If

        Next

        If Not removeme Is Nothing Then
            hydrographChart.ChartAreas(0).AxisX.StripLines.Remove(removeme)
        End If

    End Sub

    Public Sub showStripLineLabels()
        'If zoomed in then label our striplines
        If hydrographChart.ChartAreas(0).AxisX.ScaleView.Size < 1825 Then
            For Each strpline In hydrographChart.ChartAreas(0).AxisX.StripLines
                strpline.Text = strpline.Tag
            Next
        Else
            For Each strpline In hydrographChart.ChartAreas(0).AxisX.StripLines
                strpline.Text = ""
            Next
        End If
    End Sub


#End Region

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

        Dim acceptedDateChanged As XmlNode = mainDataManager.config.CreateElement("NewDateAccepted")
        acceptedDateChanged.InnerText = bAcceptedNovelDate.ToString
        outputNode.AppendChild(acceptedDateChanged)

        outputNode.AppendChild(mainDataManager.serializeChartDisplayDataToXML(curDisplayData))
        outputNode.AppendChild(mainDataManager.serializeChartSymbologyToXML(hydrographChart))

        Return outputNode

    End Function

    Public Sub loadFromXmlNode(inNode As XmlNode)

        Dim dcNode As XmlNode
        dcNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & "']")
        If dcNode Is Nothing Then
            dcNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & " (*)" & "']")
            Me.Text = Me.Text & " (*)"
        End If
        bLocked = CBool(dcNode.SelectSingleNode("Locked").InnerText)
        Try
            bAcceptedNovelDate = CBool(dcNode.SelectSingleNode("NewDateAccepted").InnerText)
        Catch ex As Exception
            bAcceptedNovelDate = False
        End Try

        Me.myContextMenuStrip.setLockState(bLocked)

        curDisplayData = mainDataManager.deserializeChartDisplayDataFromXML(dcNode.SelectSingleNode("chartDisplayData"))
        Dim chartSymbologyNode As XmlNode = dcNode.SelectSingleNode("Chart")
        If Not IsNothing(chartSymbologyNode) Then
            mainDataManager.symbolizeChartFromXML(hydrographChart, chartSymbologyNode)
        End If

    End Sub

    Public Sub refreshAfterLoad()
        loadData()
    End Sub

#End Region

#Region "Chart interaction"

    Private Sub HydrographGraphForm_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Right Then
            Me.incrementCursor()
        ElseIf e.KeyCode = Keys.Left Then
            Me.incrementCursor(True)
        End If
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keydata As Keys) As Boolean

        If keydata = Keys.Right Or keydata = Keys.Left Or keydata = Keys.Up Or keydata = Keys.Down Then
            OnKeyDown(New KeyEventArgs(keydata))
            ProcessCmdKey = True
        Else
            ProcessCmdKey = MyBase.ProcessCmdKey(msg, keydata)
        End If
    End Function

    Private Sub chart1_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles hydrographChart.MouseDown
        Dim result As HitTestResult = hydrographChart.HitTest(e.X, e.Y)
        If Not (result Is Nothing) And Not (result.Object Is Nothing) Then
            ' When user hits the LegendItem
            If TypeOf result.Object Is LegendItem Then
                ' Legend item result
                Dim legendItem As LegendItem = CType(result.Object, LegendItem)

                ' series item selected
                Dim selectedSeries As Series = hydrographChart.Series(legendItem.SeriesName)
                Dim sf As New SeriesFormater(selectedSeries, mainHydrographManager)
                sf.btnColor.BackColor = mainDataManager.seriesSymbology(selectedSeries.Name).curColor
                If mainDataManager.seriesSymbology(selectedSeries.Name).curWidth > 5 Then
                    mainDataManager.seriesSymbology(selectedSeries.Name).curWidth = 5
                End If
                sf.cboWidth.SelectedIndex = mainDataManager.seriesSymbology(selectedSeries.Name).curWidth - 1
                sf.cboStyle.Text = mainDataManager.seriesSymbology(selectedSeries.Name).curStyle.ToString
                sf.cboMarkers.Text = mainDataManager.seriesSymbology(selectedSeries.Name).curMarker.ToString
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

    Private Sub SelectData_Click(sender As System.Object, e As System.EventArgs) Handles SelectData.Click
        Dim selData As New SelectDisplayData(Me, chartType)
        selData.Show()
    End Sub

    Public Sub SyncCursor(position As Integer)
        RemoveHandler hydrographChart.CursorPositionChanged, AddressOf hydrographChart_CursorPositionChanged
        hydrographChart.ChartAreas("Default").CursorX.Position = position
        AddHandler hydrographChart.CursorPositionChanged, AddressOf hydrographChart_CursorPositionChanged
    End Sub

    Public Sub updateCursor(cursorType As String)
        'Change the currently selected cursor type either zoom or select
        curCursorType = cursorType
        Dim zoomable As Boolean = cursorType = "zoom"
        bSelectMode = Not zoomable

        hydrographChart.ChartAreas(0).AxisX.ScaleView.Zoomable = zoomable
        hydrographChart.ChartAreas(0).AxisY.ScaleView.Zoomable = zoomable

        hydrographChart.ChartAreas(0).CursorY.IsUserSelectionEnabled = zoomable
        hydrographChart.ChartAreas(0).CursorX.IsUserSelectionEnabled = zoomable
        hydrographChart.ChartAreas(0).CursorY.IsUserEnabled = zoomable
        hydrographChart.ChartAreas(0).CursorX.IsUserEnabled = Not zoomable


        If zoomable Then
            hydrographChart.ChartAreas(0).CursorX.LineWidth = 1
            hydrographChart.ChartAreas(0).CursorX.SelectionColor = Color.LightGray
        Else
            hydrographChart.ChartAreas(0).CursorX.LineWidth = 3
            hydrographChart.ChartAreas(0).CursorX.SelectionColor = Color.Transparent
            If Not hydrographChart.ChartAreas(0).CursorX.Position > hydrographChart.ChartAreas(0).AxisX.ScaleView.ViewMinimum And
                    hydrographChart.ChartAreas(0).CursorX.Position > hydrographChart.ChartAreas(0).AxisX.ScaleView.ViewMaximum Then
                hydrographChart.ChartAreas(0).CursorX.Position = CInt(((hydrographChart.ChartAreas(0).AxisX.ScaleView.ViewMaximum - hydrographChart.ChartAreas(0).AxisX.ScaleView.ViewMinimum) / 2) + hydrographChart.ChartAreas(0).AxisX.ScaleView.ViewMinimum)

            End If
        End If
    End Sub

    'Private Sub Chart1_GetToolTipText(sender As Object, e As System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs) Handles hydrographChart.GetToolTipText

    '    ' Check selected chart element and set tooltip text
    '    Select Case e.HitTestResult.ChartElementType
    '        Case ChartElementType.DataPoint
    '            Dim cursorPoint As DataPoint = e.HitTestResult.Series.Points.FindByValue(e.X, "X")
    '            e.Text = "Data Point " & e.X & ", " & e.Y
    '    End Select

    'End Sub 'Chart1_GetToolTipText


    Private Sub hydrographChart_CursorPositionChanged(sender As Object, e As System.Windows.Forms.DataVisualization.Charting.CursorEventArgs) Handles hydrographChart.CursorPositionChanged, hydrographChart.CursorPositionChanging
        'send out a signal to other panels to update their display based on the currently cursor positions flow
        If e.Axis.Name = "X axis" And bSelectMode Then
            For Each Series In hydrographChart.Series
                If Series.Tag.startswith("Q|") And Not Double.IsNaN(e.NewPosition) Then
                    Dim cursorPoint As DataPoint = Series.Points.FindByValue(e.NewPosition, "X")
                    Dim tag As String() = Series.Tag.split("|")
                    If Not cursorPoint Is Nothing Then
                        Dim curFlows As New List(Of String)
                        Dim i As Integer = 0
                        Try
                            Do Until i = curPersistenceLength
                                cursorPoint = Series.Points.FindByValue(e.NewPosition - i, "X")
                                If mainDataManager.curUnits <> "metric" Then
                                    curFlows.Add(CDbl(cursorPoint.YValues.First) / mainDataManager.getConversionFactor("MetersToFeet"))
                                Else
                                    curFlows.Add(cursorPoint.YValues.First)
                                End If
                                i += 1
                            Loop





                            mainHydrographManager.parentMainForm.mainMapManager.syncFlow(tag(2), tag(1), curFlows)
                            mainHydrographManager.syncCursor(e.NewPosition)
                        Catch ex As Exception

                        End Try
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub SetChartTransparency(chart As Chart, Seriesname As String)
        Dim setTransparent As Boolean = True
        Dim numberOfPoints As Integer = 100
        chart.ApplyPaletteColors()
        For Each point As DataPoint In chart.Series(Seriesname).Points
            If setTransparent Then
                point.Color = Color.FromArgb(0, point.Color)
            Else
                point.Color = Color.FromArgb(255, point.Color)
            End If
            numberOfPoints = numberOfPoints - 1
            If numberOfPoints = 0 Then
                numberOfPoints = 100
                setTransparent = Not setTransparent
            End If
        Next
    End Sub


    Public Sub incrementCursor(Optional IncrementLeft As Boolean = False)

        If IncrementLeft Then
            If hydrographChart.ChartAreas("Default").CursorX.Position >= hydrographChart.ChartAreas("Default").AxisX.Minimum Then
                Dim offset As Double = (hydrographChart.ChartAreas("Default").CursorX.Position - hydrographChart.ChartAreas("Default").AxisX.ScaleView.Position)
                hydrographChart.ChartAreas("Default").CursorX.Position -= 1

                hydrographChart.ChartAreas("Default").AxisX.ScaleView.Scroll(hydrographChart.ChartAreas("Default").CursorX.Position - offset)
                '(hydrographChart.ChartAreas("Default").CursorX.Position - hydrographChart.ChartAreas("Default").AxisX.ScaleView.Position))
            Else
                hydrographChart.ChartAreas("Default").CursorX.Position = hydrographChart.ChartAreas("Default").AxisX.Maximum
                hydrographChart.ChartAreas("Default").AxisX.ScaleView.Scroll(hydrographChart.ChartAreas("Default").CursorX.Position)
            End If
        Else
            If hydrographChart.ChartAreas("Default").CursorX.Position <= hydrographChart.ChartAreas("Default").AxisX.Maximum Then
                Dim offset As Double = (hydrographChart.ChartAreas("Default").CursorX.Position - hydrographChart.ChartAreas("Default").AxisX.ScaleView.Position)
                hydrographChart.ChartAreas("Default").CursorX.Position += 1

                hydrographChart.ChartAreas("Default").AxisX.ScaleView.Scroll(hydrographChart.ChartAreas("Default").CursorX.Position - offset)
                '(hydrographChart.ChartAreas("Default").CursorX.Position - hydrographChart.ChartAreas("Default").AxisX.ScaleView.Position))
            Else
                hydrographChart.ChartAreas("Default").CursorX.Position = hydrographChart.ChartAreas("Default").AxisX.Minimum
                hydrographChart.ChartAreas("Default").AxisX.ScaleView.Scroll(hydrographChart.ChartAreas("Default").CursorX.Position)
            End If
        End If



        For Each Series In hydrographChart.Series
            Dim tags As String() = Series.Tag.split("|")

            If Series.Tag.startswith("Q|") And Not Double.IsNaN(hydrographChart.ChartAreas("Default").CursorX.Position) Then
                Dim cursorPoint As DataPoint = Series.Points.FindByValue(hydrographChart.ChartAreas("Default").CursorX.Position, "X")
                If Not cursorPoint Is Nothing Then
                    Dim tag As String() = Series.Tag.split("|")

                    Dim curFlows As New List(Of String)
                    Dim i As Integer = 0
                    Do Until i = curPersistenceLength
                        curFlows.Add(cursorPoint.YValues.First)
                        i += 1
                    Loop
                    mainHydrographManager.parentMainForm.mainMapManager.syncFlow(tag(2), tag(1), curFlows)
                End If
            End If


            'If mainDataManager.getSegmentNames().Contains(Series.Tag) And _
            '    Not Double.IsNaN(hydrographChart.ChartAreas("Default").CursorX.Position) Then
            '    Dim cursorPoint As DataPoint = Series.Points.FindByValue(hydrographChart.ChartAreas("Default").CursorX.Position, "X")
            '    mainHydrographManager.parentMainForm.mainMapManager.syncFlow(Series.Tag, "test", cursorPoint.YValues.First)
            'End If
        Next

    End Sub

#End Region

    Private Sub ViewData_Click(sender As System.Object, e As System.EventArgs) Handles ViewData.Click
        Dim selData As RawDataForm = mainDataManager.addRawDataForm(True)
        Me.curDisplayData.interval = "daily"
        selData.curDisplayData = Me.curDisplayData
        selData.loadData()
    End Sub

    Private Sub lblUniqueDate_Click(sender As System.Object, e As System.EventArgs) Handles lblUniqueDate.Click
        lblUniqueDate.Visible = False
        hydrographChart.BorderlineDashStyle = ChartDashStyle.NotSet
        bAcceptedNovelDate = True
    End Sub


End Class
