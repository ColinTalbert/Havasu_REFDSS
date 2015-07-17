Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Data.SQLite
Imports System.Xml

Public Class SystemWideMetricForm
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent
    Friend WithEvents SystemWideChart As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer

    Private mainGraphManager As GraphManager
    Public mainDataManager As DataManager
    Public ChartType As String = "SystemWideMetrics"
    Private components As System.ComponentModel.IContainer
    Private curMetric As String

    Private Metric As New System.Windows.Forms.ToolStripMenuItem
    Private Parameters As New System.Windows.Forms.ToolStripMenuItem

    Private myContextMenuStrip As GenericChartContextMenuStrip

    Public bLocked As Boolean = False

    Public Sub New(GM As GraphManager, DM As DataManager)
        mainGraphManager = GM
        mainDataManager = DM



        Me.InitializeComponent()

        myContextMenuStrip = New GenericChartContextMenuStrip(SystemWideChart, Me)
        Metric.Text = "Metric"
        Parameters.Text = "Parameters"
        Me.ContextMenuStrip = myContextMenuStrip
        Me.ContextMenuStrip.Items.Insert(0, Metric)
        Me.ContextMenuStrip.Items.Insert(1, Parameters)
        Dim tss As New ToolStripSeparator()
        Me.ContextMenuStrip.Items.Insert(2, tss)


        loadDisplaySelectors()
        loadData()
    End Sub


    Private Sub InitializeComponent()
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Title1 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SystemWideMetricForm))
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.SystemWideChart = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        CType(Me.SystemWideChart, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ToolStripContainer1
        '
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.SystemWideChart)
        Me.ToolStripContainer1.ContentPanel.Size = New System.Drawing.Size(818, 719)
        Me.ToolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ToolStripContainer1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.Size = New System.Drawing.Size(818, 744)
        Me.ToolStripContainer1.TabIndex = 0
        Me.ToolStripContainer1.Text = "ToolStripContainer1"
        '
        'SystemWideChart
        '
        ChartArea1.CursorX.IsUserEnabled = True
        ChartArea1.CursorX.IsUserSelectionEnabled = True
        ChartArea1.Name = "Default"
        Me.SystemWideChart.ChartAreas.Add(ChartArea1)
        Me.SystemWideChart.Dock = System.Windows.Forms.DockStyle.Fill
        Legend1.Name = "Legend1"
        Me.SystemWideChart.Legends.Add(Legend1)
        Me.SystemWideChart.Location = New System.Drawing.Point(0, 0)
        Me.SystemWideChart.Name = "SystemWideChart"
        Series1.ChartArea = "Default"
        Series1.Legend = "Legend1"
        Series1.Name = "Series1"
        Me.SystemWideChart.Series.Add(Series1)
        Me.SystemWideChart.Size = New System.Drawing.Size(818, 719)
        Me.SystemWideChart.TabIndex = 0
        Me.SystemWideChart.Text = "HabitatChart"
        Title1.DockedToChartArea = "Default"
        Title1.Name = "Title"
        Title1.Text = "Title"
        Me.SystemWideChart.Titles.Add(Title1)
        '
        'SystemWideMetricForm
        '
        Me.ClientSize = New System.Drawing.Size(818, 744)
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "SystemWideMetricForm"
        Me.Text = "System Wide Metrics"
        Me.TopMost = True
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        CType(Me.SystemWideChart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Public Sub loadDisplaySelectors()
        Debug.Print("loadDisplaySelectors()")
        Metric.DropDownItems.Clear()
        For Each scenario In mainDataManager.getSWMetrics()
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = scenario
            menuitem.Tag = "scenario"
            menuitem.CheckOnClick = +True
            AddHandler menuitem.Click, AddressOf MetricMenuItem_click
            Metric.DropDownItems.Add(menuitem)
        Next
        setCheckedDropdownItem(Metric.DropDownItems, Metric.DropDownItems.Item(0).Text)
        curMetric = Metric.DropDownItems.Item(0).Text

        loadMetricParameters()
    End Sub

    Private Sub MetricMenuItem_click(sender As System.Object, e As System.EventArgs)

        For Each MenuItem In sender.owner.items
            If Not MenuItem Is sender Then
                MenuItem.checked = False
            End If
        Next

        refreshData()

    End Sub

    Public Sub refreshData()
        curMetric = getCheckedDropdownItem(Metric.DropDownItems)
        loadMetricParameters()
        loadData()
    End Sub

    Private Sub loadMetricParameters()
        Debug.Print("loadMetricParameters()")
        Parameters.DropDownItems.Clear()
        For Each parameter In mainDataManager.getSWMetricParameters(curMetric)
            Dim LabelItem As New ToolStripMenuItem
            LabelItem.Text = parameter
            Parameters.DropDownItems.Add(LabelItem)
            Dim paramTxtBox As New ToolStripTextBox
            paramTxtBox.BackColor = Color.LightGray
            paramTxtBox.BorderStyle = BorderStyle.FixedSingle
            paramTxtBox.Alignment = ToolStripItemAlignment.Right
            paramTxtBox.Text = mainDataManager.getSWMetricParamVal(curMetric, parameter)
            Parameters.DropDownItems.Add(paramTxtBox)
            Dim sep As New ToolStripSeparator
            Parameters.DropDownItems.Add(sep)
        Next
    End Sub

    Public Sub loadData()
        SystemWideChart.Series.Clear()
        SystemWideChart.Titles("Title").Text = curMetric

        For Each outputSeries In mainDataManager.getSWMetricOuputSeries(curMetric)
            Dim newSeries As New Series
            newSeries.Name = outputSeries
            newSeries.ChartType = SeriesChartType.StackedColumn
            SystemWideChart.Series.Add(newSeries)
            SystemWideChart.Series(outputSeries).IsValueShownAsLabel = mainGraphManager.bShowLabels
        Next

        If mainDataManager.mainSQLDBConnection.State = ConnectionState.Closed Then
            mainDataManager.mainSQLDBConnection.Open()
        End If
        For Each scenario In mainDataManager.getScenarioNames
            'pull data from our db
            Dim SQLselect As String = mainDataManager.getSWMetricSQL(curMetric)
            For Each parameter In mainDataManager.getSWMetricParameters(curMetric)
                Dim variable As String = mainDataManager.getSWMetricParamVariableName(curMetric, parameter)
                SQLselect = SQLselect.Replace(variable, mainDataManager.getSWMetricParamVal(curMetric, parameter))
            Next
            SQLselect = SQLselect.Replace("{ScenarioTable}", "scenario_" + scenario)
            SQLselect = SQLselect.Replace(");", " WHERE Date>=date('" + CStr(mainDataManager.curStartYear - 1) + "-10-01') and DATE<date('" + CStr(mainDataManager.curEndYear) + "-10-01'));")

            Dim SQLcommand As SQLiteCommand
            SQLcommand = mainDataManager.mainSQLDBConnection.CreateCommand
            SQLcommand.CommandText = SQLselect
            Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()
            SQLreader.Read()

            For Each outputSeries In mainDataManager.getSWMetricOuputSeries(curMetric)
                'SystemWideChart.Series(outputSeries).Points.AddY(SQLreader(mainDataManager.getMetricSeriesField(curMetric, outputSeries)))
                SystemWideChart.Series(outputSeries).Points.AddXY(scenario, SQLreader(mainDataManager.getMetricSeriesField(curMetric, outputSeries)))
            Next

        Next

        mainDataManager.mainSQLDBConnection.Close()

        SystemWideChart.ChartAreas(0).RecalculateAxesScale()
    End Sub



    Private Sub displayLabels(visible)
        For Each outputSeries In mainDataManager.getSWMetricOuputSeries(curMetric)
            SystemWideChart.Series(outputSeries).IsValueShownAsLabel = visible
        Next
    End Sub



    Private Sub SystemWideMetricForm_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        mainGraphManager.removeSystemWideGraph(Me)
    End Sub

    Private Sub SystemWideMetricForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.FormClosing
        mainGraphManager.removeSystemWideGraph(Me)
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

        Dim curMetricNode As XmlNode = mainDataManager.config.CreateElement("curMetric")
        curMetricNode.InnerText = curMetric
        outputNode.AppendChild(curMetricNode)


        'Dim options As New Dictionary(Of String, ToolStripMenuItem) 

        'For Each toolStripMenuItem In myContextMenuStrip.Items
        '    Debug.Print(toolStripMenuItem.text)
        '    If toolStripMenuItem.text = "Metric" Then
        '        For Each Metric In toolStripMenuItem.Items

        '        Next

        '    End If

        'Next

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
        outputNode.AppendChild(mainDataManager.serializeChartSymbologyToXML(SystemWideChart))
        Return outputNode

    End Function

    Public Sub loadFromXmlNode(inNode As XmlNode)

        Dim dcNode As XmlNode
        dcNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & "']")
        bLocked = CBool(dcNode.SelectSingleNode("Locked").InnerText)
        curMetric = dcNode.SelectSingleNode("curMetric").InnerText


        Dim optionNodes As XmlNodeList = mainDataManager.config.SelectNodes("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & "']/*")
        For Each optionNode As XmlNode In optionNodes
            If optionNode.Name <> "Locked" And
                optionNode.Name <> "Name" Then
                Dim checkedoptions As String() = optionNode.InnerText.Split("|")
                For Each strOption In checkedoptions
                    Dim item As ToolStripMenuItem
                    item = myContextMenuStrip.Items(optionNode.Name.Replace(" ", "_"))
                    Dim ddItem As ToolStripMenuItem
                    If Not IsNothing(item) Then
                        ddItem = item.DropDownItems(strOption.Replace(" ", "_"))
                        If Not IsNothing(ddItem) Then
                            ddItem.Checked = True
                        Else
                            Debug.Print(strOption + " is nothing")
                        End If
                    Else
                        Debug.Print(optionNode.Name + " is nothing")
                    End If
                Next
            End If
        Next

        Dim chartSymbologyNode As XmlNode = dcNode.SelectSingleNode("Chart")
        If Not IsNothing(chartSymbologyNode) Then
            mainDataManager.symbolizeChartFromXML(SystemWideChart, chartSymbologyNode)
        End If


    End Sub

    Public Sub refreshAfterLoad()
        loadData()
    End Sub

#End Region

End Class
