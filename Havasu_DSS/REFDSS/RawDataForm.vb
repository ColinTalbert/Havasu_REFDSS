Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Data.SQLite
Imports System.Xml

Public Class RawDataForm
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    Private mainGraphManager As GraphManager
    Public mainDataManager As DataManager
    Private curMetric As String

    Private components As System.ComponentModel.IContainer

    Public bLocked As Boolean = False

    Public curDisplayData As New chartDisplayData
    Public chartType As String = "RawDataForm"
    Public dataChanged As Boolean = False

    Private WithEvents SelectData As New System.Windows.Forms.ToolStripMenuItem("Select Data")
    Private WithEvents CopySelected As New System.Windows.Forms.ToolStripMenuItem("Copy selected Ctrl+C")
    Private WithEvents CopyAll As New System.Windows.Forms.ToolStripMenuItem("Copy all data")
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents dgvRawData As System.Windows.Forms.DataGridView
    Private myContextMenuStrip As ContextMenuStrip
    Private OrigMetricNames As New Dictionary(Of String, String)

    Public Sub New(DM As DataManager)
        mainDataManager = DM

        Me.InitializeComponent()
        ' loadData()

        myContextMenuStrip = New ContextMenuStrip
        myContextMenuStrip.Items.Insert(0, SelectData)
        myContextMenuStrip.Items.Insert(1, CopySelected)
        myContextMenuStrip.Items.Insert(2, CopyAll)
        Me.ContextMenuStrip = myContextMenuStrip
    End Sub


    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RawDataForm))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.dgvRawData = New System.Windows.Forms.DataGridView()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.dgvRawData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(818, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(81, 20)
        Me.SaveToolStripMenuItem.Text = "Save as CSV"
        '
        'dgvRawData
        '
        Me.dgvRawData.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        Me.dgvRawData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle1.Format = "N2"
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvRawData.DefaultCellStyle = DataGridViewCellStyle1
        Me.dgvRawData.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvRawData.Location = New System.Drawing.Point(0, 24)
        Me.dgvRawData.Name = "dgvRawData"
        Me.dgvRawData.Size = New System.Drawing.Size(818, 720)
        Me.dgvRawData.TabIndex = 1
        '
        'RawDataForm
        '
        Me.ClientSize = New System.Drawing.Size(818, 744)
        Me.Controls.Add(Me.dgvRawData)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "RawDataForm"
        Me.Text = "Data view"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.dgvRawData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Public Sub loadData()
        Me.Cursor = Cursors.WaitCursor
        Dim dt As New DataTable
        If curDisplayData.interval = "daily" Then
            dt = mainDataManager.getDailyData(curDisplayData)
        ElseIf curDisplayData.interval = "single" Then
            dt = mainDataManager.getSingleData(curDisplayData)
            For Each row As DataRow In dt.Rows
                row(0) = readableSingleColumnName(row(0))
            Next row
        ElseIf curDisplayData.interval = "yearly" Then
            dt = mainDataManager.getYearlyScenarioData(curDisplayData)
        End If



        dgvRawData.DataSource = Nothing
        dgvRawData.DataSource = dt
        Me.Cursor = Cursors.Default


        If dgvRawData.Columns.Count > 0 Then
            If curDisplayData.interval = "daily" Then
                dgvRawData.Columns(0).DefaultCellStyle.Format = "d"
            Else
                dgvRawData.Columns(0).DefaultCellStyle.Format = "N2"
            End If
            formatRows()
        End If

        For i As Integer = 1 To dgvRawData.ColumnCount - 1
            dgvRawData.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        Next

        dgvRawData.Refresh()
    End Sub

    'Private Sub dataGridView1_CellFormatting(ByVal sender As Object, _
    'ByVal e As DataGridViewCellFormattingEventArgs) _
    'Handles dgvRawData.CellFormatting

    '    If e.ColumnIndex = dgvRawData.Columns.Count - 1 Then
    '        'the first if waits until all the values for the row are populated
    '        ' If the column is the Artist column, check the 
    '        ' value. 
    '        Dim dgvr As DataGridViewRow = dgvRawData.Rows(e.RowIndex)
    '        If Me.curDisplayData.interval = "single" Then
    '            formatSingleRow(dgvr)
    '        Else
    '            For Each c As DataGridViewCell In dgvr.Cells
    '                c.Style.BackColor = Color.Ivory
    '            Next
    '        End If

    '    End If
    'End Sub

    Private Class metricGroup
        Public baselineval As Double
        Public allZeros As Boolean
    End Class
    Private Sub formatRows()
        If curDisplayData.interval = "single" Then
            For Each dgvr As DataGridViewRow In dgvRawData.Rows
                formatSingleRow(dgvr)
            Next
        ElseIf curDisplayData.interval = "yearly" Then
            For Each dgvr As DataGridViewRow In dgvRawData.Rows
                formatYearRow(dgvr)
            Next
        Else
        End If

    End Sub
    Private Sub formatSingleRow(dgvr As DataGridViewRow)
        Try
            'get the baseline(s) value(s) and check if the row is all zeros
            Dim baseline As Double
            Dim allZeros As Boolean = True
            For Each c As DataGridViewCell In dgvr.Cells
                Dim col As DataGridViewTextBoxColumn = dgvRawData.Columns(c.ColumnIndex)
                Dim words As String() = col.HeaderText.Split("_")
                Dim group As String = Join(words.Skip(1).Take(words.Count - 1).ToArray, "_")
                If words(0) <> "metric" Then
                    If CDbl(c.Value) > 0 Then
                        allZeros = False
                    End If
                End If
                If col.HeaderText = curDisplayData.baseline Then
                    baseline = CDbl(c.Value)
                End If
            Next

            For Each c As DataGridViewCell In dgvr.Cells
                Dim col As DataGridViewTextBoxColumn = dgvRawData.Columns(c.ColumnIndex)
                If allZeros Then
                    c.Style.BackColor = Color.Gray
                ElseIf col.HeaderText <> "metric" Then
                    Dim delta As Double = ((CDbl(c.Value) - baseline) / baseline)
                    If delta >= 0.1 Then
                        c.Style.BackColor = Color.LightGreen
                    ElseIf delta <= -0.1 Then
                        c.Style.BackColor = Color.LightPink
                    Else
                        c.Style.BackColor = Color.Ivory
                    End If
                End If

            Next
        Catch ex As Exception

        End Try
        Debug.Print("now")
    End Sub

    Private Sub formatYearRow(dgvr As DataGridViewRow)

        Dim groupsDict As New Dictionary(Of String, metricGroup)
        For Each c As DataGridViewCell In dgvr.Cells
            Dim col As DataGridViewTextBoxColumn = dgvRawData.Columns(c.ColumnIndex)
            Dim words As String() = col.HeaderText.Split("_")
            Dim group As String = Join(words.Skip(1).Take(words.Count - 1).ToArray, "_")
            If Not words(0) = "WaterYear" Then
                If Not groupsDict.ContainsKey(group) Then
                    Dim newgroup As New metricGroup
                    newgroup.baselineval = 0
                    newgroup.allZeros = True
                    groupsDict.Add(group, newgroup)
                End If

                Try
                    If CDbl(c.Value) > 0 Then
                        groupsDict(group).allZeros = False
                    End If
                Catch ex As Exception
                    groupsDict(group).allZeros = True
                End Try

                Try
                    If words(0) = curDisplayData.baseline Then
                        groupsDict(group).baselineval = CDbl(c.Value)
                    End If
                Catch ex As Exception
                    groupsDict(group).baselineval = 0
                End Try

            End If
        Next

        'now format the cells relative to these baselines
        For Each c As DataGridViewCell In dgvr.Cells
            Dim col As DataGridViewTextBoxColumn = dgvRawData.Columns(c.ColumnIndex)
            Dim words As String() = col.HeaderText.Split("_")
            Dim group As String = Join(words.Skip(1).Take(words.Count - 1).ToArray, "_")

            If words(0) = "WaterYear" Then
                c.Style.BackColor = Color.Ivory
            Else
                Try
                    Dim delta As Double = ((c.Value - groupsDict(group).baselineval) / groupsDict(group).baselineval)
                    If delta >= 0.1 Then
                        c.Style.BackColor = Color.LightGreen
                    ElseIf delta <= -0.1 Then
                        c.Style.BackColor = Color.LightPink
                    Else
                        c.Style.BackColor = Color.Ivory
                    End If
                Catch ex As Exception
                    c.Style.BackColor = Color.DarkGray
                End Try


            End If
        Next
    End Sub

    Private Sub formatDayRow(dgvr As DataGridViewRow)
        For Each c As DataGridViewCell In dgvr.Cells
            c.Style.BackColor = Color.Ivory
        Next
    End Sub

    Public Function readableSingleColumnName(oldname As String)
        Try
            Dim words As String() = oldname.Split("_")
            Dim river As String
            Try
                river = mainDataManager.getRiverName(words(1))
                If river = "Delaware" Then
                    river = "Delaware main stem"
                End If
            Catch ex As Exception
                river = words(1)
            End Try

            Dim segment As String
            Try
                segment = mainDataManager.getSegmentName(words(2)).Replace("_", "")
            Catch ex As Exception
                segment = words(2)
            End Try


            Dim species As String = mainDataManager.getSpeciesName(words(3))
            Dim lifestage As String = mainDataManager.getLifeStageName(words(3), words(4))
            Dim treatment As String = words(0)

            Dim prettyReturn As String
            If river.Contains("agg") Then
                prettyReturn = ("Basin wide summary" & " " & species & " " & lifestage & " (" & treatment & ")" & " ha")
            ElseIf segment = "aggRiver" Then
                prettyReturn = (river & " " & species & " " & lifestage & " (" & treatment & ")" & " ha")
            Else
                prettyReturn = (segment & " " & species & " " & lifestage & " (" & treatment & ")" & " ha")
            End If

            If words(5) <> "meanBottom25" Then
                prettyReturn += " " & words(5)
            End If
            Return prettyReturn

        Catch ex As Exception
            Return oldname
        End Try

    End Function

    Private Sub updateMetricNames()
        OrigMetricNames.Clear()

        For Each row As DataGridViewRow In dgvRawData.Rows
            If Not row.Cells(0).Value Is Nothing Then
                OrigMetricNames.Add(CStr(row.Index), row.Cells(0).Value)
                If mainDataManager.dgvNames.ContainsKey(row.Cells(0).Value) Then
                    row.Cells(0).Value = mainDataManager.dgvNames(row.Cells(0).Value)
                End If
            End If
        Next
    End Sub

    'Private Function formatDataGridYearly()
    '    'This works for daily as well but is too slow
    '    Dim groupsDict As New Dictionary(Of String, Integer)
    '    For Each col As DataGridViewTextBoxColumn In dgvRawData.Columns
    '        col.Tag = "none"
    '        If col.HeaderText <> "year" Then

    '            Dim words As String() = col.HeaderText.Split("_")
    '            Dim group As String = Join(words.Skip(1).Take(words.Count - 1).ToArray, "_")
    '            col.Tag = group
    '            If words.Length > 3 Then
    '                If words(0) = curDisplayData.baseline Then
    '                    groupsDict.Add(col.Tag, col.Index)
    '                End If

    '            End If

    '        End If
    '    Next
    '    Dim baseval As Double
    '    dgvRawData.DefaultCellStyle.Format = "n2"
    '    Dim allZero As Boolean
    '    For Each row As DataGridViewRow In dgvRawData.Rows
    '        
    '        For i = 0 To row.Cells.Count - 1
    '            If row.Cells(i).Value > 0 Then
    '                allZero = False
    '            End If
    '            If groupsDict.ContainsKey(dgvRawData.Columns(i).Tag) Then
    '                baseval = row.Cells(groupsDict(dgvRawData.Columns(i).Tag)).Value
    '                Dim delta As Double = ((row.Cells(i).Value - baseval) / baseval)
    '                If delta >= 0.1 Then
    '                    row.Cells(i).Style.BackColor = Color.LightGreen
    '                ElseIf delta <= -0.1 Then
    '                    row.Cells(i).Style.BackColor = Color.LightPink
    '                Else
    '                    row.Cells(i).Style.BackColor = Color.Ivory
    '                End If
    '            End If
    '        Next
    '        If allZero Then
    '            For i = 0 To row.Cells.Count - 1
    '                row.Cells(i).Style.BackColor = Color.Gray
    '            Next
    '        End If
    '    Next

    'End Function

    'Private Function formatDataGridDefault()
    '    For Each row As DataGridViewRow In dgvRawData.Rows
    '        For i = 0 To row.Cells.Count - 1
    '            row.Cells(i).Style.BackColor = Color.Ivory
    '        Next
    '    Next
    'End Function

    'Private Function formatDataGridSingle()
    '    Dim baselineCol As Integer = 0
    '    For Each col As DataGridViewTextBoxColumn In dgvRawData.Columns
    '        If col.HeaderText = curDisplayData.baseline Then
    '            baselineCol = dgvRawData.Columns.IndexOf(col)
    '        End If
    '    Next

    '    Dim baseval As Double
    '    Dim allZero As Boolean
    '    For Each row As DataGridViewRow In dgvRawData.Rows
    '        allZero = True
    '        For i = 1 To row.Cells.Count - 1
    '            If row.Cells(i).Value > 0 Then
    '                allZero = False
    '            End If
    '            baseval = row.Cells(baselineCol).Value
    '            Dim delta As Double = ((row.Cells(i).Value - baseval) / baseval)
    '            If delta >= 0.1 Then
    '                row.Cells(i).Style.BackColor = Color.LightGreen
    '            ElseIf delta <= -0.1 Then
    '                row.Cells(i).Style.BackColor = Color.LightPink
    '            End If
    '        Next
    '        If allZero Then
    '            For i = 0 To row.Cells.Count - 1
    '                row.Cells(i).Style.BackColor = Color.Gray
    '            Next
    '        End If
    '    Next
    'End Function

    Private Function getPercentValue(valList As List(Of Double), percentage As Double)
        If valList.Count = 0 Then
            Return -1
        Else
            Dim index As Integer = percentage * valList.Count / 100
            Return valList(index)
        End If
    End Function

    Private Sub HabitatGraphForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.FormClosing
        mainDataManager.removeRawDataForm(Me)
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
        'outputNode.AppendChild(mainDataManager.serializeChartSymbologyToXML(HabitatChart))

        Return outputNode

    End Function

    Public Sub loadFromXmlNode(inNode As XmlNode)

        Dim dcNode As XmlNode
        dcNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & "']")
        If dcNode Is Nothing Then
            dcNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & " (*)" & "']")
            Me.Text = Me.Text + " (*)"
        End If
        'bLocked = CBool(dcNode.SelectSingleNode("Locked").InnerText)
        'Me.myContextMenuStrip.setLockState(bLocked)

        curDisplayData = mainDataManager.deserializeChartDisplayDataFromXML(dcNode.SelectSingleNode("chartDisplayData"))
        'Dim chartSymbologyNode As XmlNode = dcNode.SelectSingleNode("Chart")
        'If Not IsNothing(chartSymbologyNode) Then
        '    mainDataManager.symbolizeChartFromXML(HabitatChart, chartSymbologyNode)
        'End If


    End Sub

    Private Sub SelectData_Click(sender As System.Object, e As System.EventArgs) Handles SelectData.Click
        Dim selData As New SelectDisplayData(Me, chartType)
        selData.Show()
    End Sub

    Private Sub CopyData_Click(sender As System.Object, e As System.EventArgs) Handles CopySelected.Click
        copyData()
    End Sub

    Private Sub CopyAllData_Click(sender As System.Object, e As System.EventArgs) Handles CopyAll.Click
        copyData(True)
    End Sub
    Private Sub copyData(Optional copyAll As Boolean = False)
        If Me.dgvRawData.GetCellCount( _
            DataGridViewElementStates.Selected) > 0 Then

            Try
                If copyAll Then
                    Me.dgvRawData.SelectAll()
                End If

                ' Add the selection to the clipboard.
                Clipboard.SetDataObject( _
                    Me.dgvRawData.GetClipboardContent())


            Catch ex As System.Runtime.InteropServices.ExternalException

            End Try

        End If
    End Sub


    Public Sub refreshAfterLoad()
        loadData()
    End Sub

#End Region


    Private Sub SaveToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        Dim FileBrowserDialog1 As New SaveFileDialog
        ' Then use the following code to create the Dialog window
        ' Change the .SelectedPath property to the default location
        With FileBrowserDialog1
            ' Desktop is the root folder in the dialog.
            If System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(My.Settings.ConfigXML)) Then
                .InitialDirectory = System.IO.Path.GetDirectoryName(My.Settings.ConfigXML)
            Else
                .InitialDirectory = Environment.SpecialFolder.MyComputer
            End If
            .Title = "Save data to CSV file"
            .AddExtension = True
            .DefaultExt = ".csv"
            .CheckFileExists = False
            If .ShowDialog = DialogResult.OK Then
                DataTable2CSV(dgvRawData.DataSource, FileBrowserDialog1.FileName)
            Else

                Exit Sub

            End If
        End With
    End Sub


#Region "DataTable2CSV"
    'from http://www.devx.com/vb2themax/Tip/19703
    Sub DataTable2CSV(ByVal table As DataTable, ByVal filename As String)
        DataTable2CSV(table, filename, ",")
    End Sub
    Sub DataTable2CSV(ByVal table As DataTable, ByVal filename As String, _
        ByVal sepChar As String)
        Dim writer As System.IO.StreamWriter = Nothing
        Try
            writer = New System.IO.StreamWriter(filename)

            ' first write a line with the columns name
            Dim sep As String = ""
            Dim builder As New System.Text.StringBuilder
            For Each col As DataColumn In table.Columns
                builder.Append(sep).Append(col.ColumnName)
                sep = sepChar
            Next
            writer.WriteLine(builder.ToString())

            ' then write all the rows
            For Each row As DataRow In table.Rows
                sep = ""
                builder = New System.Text.StringBuilder

                For Each col As DataColumn In table.Columns
                    builder.Append(sep).Append(row(col.ColumnName))
                    sep = sepChar
                Next
                writer.WriteLine(builder.ToString())
            Next
        Finally
            If Not writer Is Nothing Then writer.Close()
        End Try
    End Sub
#End Region

    Private Sub dgvRawData_CellEdited(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvRawData.CellValueChanged
        If curDisplayData.interval = "single" And e.ColumnIndex = 0 And Not dgvRawData.CurrentCell Is Nothing Then
            Dim oldVal As String = OrigMetricNames(CStr(e.RowIndex))

            Dim newVal As String
            newVal = dgvRawData.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
            If oldVal <> newVal Then
                If mainDataManager.dgvNames.ContainsKey(oldVal) Then
                    mainDataManager.dgvNames(oldVal) = newVal
                Else
                    mainDataManager.dgvNames.Add(oldVal, newVal)
                End If

            End If
        End If

    End Sub

    Private Sub dgvRawData_VisibleChanged(sender As Object, e As System.EventArgs) Handles dgvRawData.VisibleChanged
        formatRows()
    End Sub
End Class
