Imports System.IO
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Xml.Serialization
Imports System.Data.SQLite
Imports System.Xml
Imports WeifenLuo.WinFormsUI.Docking

Module Utilities
    Public Sub insertBreak(ByVal high As Double, ByVal low As Double, ByVal hColor As UInteger,
        ByVal lColor As UInteger, ByVal sLabel As String, ByRef sch As MapWinGIS.GridColorScheme)
        'helper function for loadColorScheme sub
        Dim break As New MapWinGIS.GridColorBreak
        break.LowValue = low
        break.HighValue = high + 0.000000001
        break.LowColor = lColor
        break.HighColor = hColor
        break.Caption = sLabel
        'Dim ui As UInteger
        'ui = MapWinGIS.tkMapColor.Transparent
        'ui = Convert.ToUInt32(RGB(255, 255, 255))
        sch.InsertBreak(break)
    End Sub

    Public Function stringToColor(ByVal strColor As String) As Color
        Dim strRGB() As String = strColor.Split(",")
        Dim clr As Color
        clr = Color.FromArgb(Int(strRGB(0)), Int(strRGB(1)), Int(strRGB(2)))

        Return clr
    End Function
    Public Function colorToString(ByVal clr As Color) As String
        Dim colorstring As String
        colorstring = Str(clr.R) + ","
        colorstring += Str(clr.G) + ","
        colorstring += Str(clr.B)

        Return colorstring
    End Function

    Public Function getCheckedDropdownItem(dropdownitems As ToolStripItemCollection) As String
        'returns the checked item in a toolstrip dropdown list
        For Each item In dropdownitems
            If item.checked Then Return item.text
        Next

        Return Nothing
    End Function

    Public Sub setCheckedDropdownItem(dropdownitems As ToolStripItemCollection, checkText As String)
        For Each item In dropdownitems
            If Not TypeOf (item) Is ToolStripSeparator Then
                If item.text = checkText Or item.tag = checkText Then
                    item.checkstate = System.Windows.Forms.CheckState.Checked
                ElseIf Not TypeOf (item) Is ToolStripSeparator And item.checkstate = System.Windows.Forms.CheckState.Checked Then
                    item.checkstate = System.Windows.Forms.CheckState.Unchecked
                End If
            End If
        Next
    End Sub

    Public Function cms_to_cfs(cms As Double) As Double

        Dim conversionFactor As Double = 35.3145
        Return cms * conversionFactor

    End Function

    Public Function cleanUpSQL(rawSQL As String, valuesDict As Dictionary(Of String, String)) As String

        Dim cleanSQL As String = rawSQL
        For Each key As String In valuesDict.Keys
            cleanSQL = cleanSQL.Replace(key, valuesDict(key))
        Next

        Return cleanSQL

    End Function

    Public Function Get_Control_Location(ByVal control As Control) As Point
        Try
            If control.Name = "MainForm" Then
                Return control.Location
            End If

            Return control.Location + Get_Control_Location(control.Parent)
        Catch ex As Exception
            Dim zeroPoint As New Point(0, 0)
        End Try


    End Function

    Public Function MakeRelativePath(fromPath As String, toPath As String) As String
        If String.IsNullOrEmpty(fromPath) Then
            Throw New ArgumentNullException("fromPath")
        End If
        If String.IsNullOrEmpty(toPath) Then
            Throw New ArgumentNullException("toPath")
        End If

        Dim fromUri As New Uri(fromPath)
        Dim toUri As New Uri(toPath)

        Dim relativeUri As Uri = fromUri.MakeRelativeUri(toUri)
        Dim relativePath As String = Uri.UnescapeDataString(relativeUri.ToString())

        Return relativePath.Replace("/"c, Path.DirectorySeparatorChar)
    End Function




    Public Function MakeAbsolutePathFromRelative(fromPath As String, toPath As String) As String
        Dim strRelativePath As String

        fromPath = returnParentFolder(fromPath)
        Dim curParentFolder As String = returnParentFolder(My.Settings.ConfigXML)


        strRelativePath = MakeRelativePath(fromPath, toPath)
        Return Path.GetFullPath(Path.Combine(curParentFolder, strRelativePath))

    End Function

    Public Function returnParentFolder(sPath As String) As String
        If Path.GetExtension(sPath) <> "" Then
            sPath = Path.GetDirectoryName(sPath)
        End If
        If Not sPath.EndsWith("\") Then
            sPath += "\"
        End If

        Return sPath
    End Function

    Public Class CovariateInfo
        Public filePath As String
        Public name As String
        Public pixelValues() As Single
        Public reclassifiedPixelValues() As Single
        Public gridHeader As MapWinGIS.GridHeader
        Public NoDataValue As Double
        Public numRows As Double
        Public numCols As Double
        Public dx As Double
        Public dy As Double
        Public xllCenter As Double
        Public yllCenter As Double
        Public functionIndex As Integer

    End Class

    Public Class chunkIndex
        Public chunksize As Integer
        Public startcol As Integer
        Public startrow As Integer

        Public endrow As Integer
        Public endcol As Integer

        Public pixwidth As Integer
        Public pixheight As Integer

        Public chunkCount As Integer
    End Class
    Public Function GetChunkIndices(ByVal strPath As String, ByVal intChunkSize As Integer) As List(Of chunkIndex)

        Dim result As New List(Of chunkIndex)


        Dim covariateGrid As New MapWinGIS.Grid

        covariateGrid = New MapWinGIS.Grid
        covariateGrid.Open(strPath)


        Dim totalNumRows As Integer = covariateGrid.Header.NumberRows
        Dim totalNumCols As Integer = covariateGrid.Header.NumberCols

        Dim numRows As Integer = intChunkSize
        Dim numCols As Integer = intChunkSize

        Dim i As Integer = 0
        Dim j As Integer = 0

        While i < totalNumRows
            If i + intChunkSize < totalNumRows Then
                numRows = intChunkSize
            Else
                numRows = totalNumRows - i - 1
            End If
            j = 0
            While j < totalNumCols
                If j + intChunkSize < totalNumCols Then
                    numCols = intChunkSize
                Else
                    numCols = totalNumCols - j - 1
                End If

                Dim ci As New chunkIndex
                ci.chunksize = intChunkSize
                ci.startrow = i
                ci.startcol = j
                ci.pixheight = numRows
                ci.pixwidth = numCols
                ci.endcol = ci.startcol + ci.pixwidth
                ci.endrow = ci.startrow + ci.pixheight
                ci.chunkCount = Math.Ceiling(totalNumCols / intChunkSize) * Math.Ceiling(totalNumRows / intChunkSize)

                result.Add(ci)

                j += intChunkSize
            End While

            i += intChunkSize
        End While

        GetChunkIndices = result

    End Function

    Public Function CovInfoFromPath(ByVal strPath As String, ci As chunkIndex) As CovariateInfo
        Dim covariateGrid As New MapWinGIS.Grid

        Dim covInfo As New CovariateInfo
        covInfo.filePath = strPath

        covariateGrid = New MapWinGIS.Grid
        covariateGrid.Open(strPath)

        covInfo.NoDataValue = covariateGrid.Header.NodataValue
        covInfo.numCols = covariateGrid.Header.NumberCols
        covInfo.numRows = covariateGrid.Header.NumberRows
        covInfo.dx = covariateGrid.Header.dX
        covInfo.dy = covariateGrid.Header.dY
        covInfo.xllCenter = covariateGrid.Header.XllCenter
        covInfo.yllCenter = covariateGrid.Header.YllCenter



        Dim rows, cols As Integer
        rows = covariateGrid.Header.NumberRows - 1
        cols = covariateGrid.Header.NumberCols - 1
        'Dim covariateVals(((rows + 1) * (cols + 1)) - 1) As Single
        'covariateGrid.GetFloatWindow(0, rows, 0, cols, covariateVals(0))
        Dim chunksize As Integer = 5000
        Dim covariateVals(((ci.pixwidth + 1) * (ci.pixheight + 1)) - 1) As Single
        covariateGrid.GetFloatWindow(ci.startrow, ci.endrow, ci.startcol, ci.endcol, covariateVals(0))

        covInfo.pixelValues = covariateVals
        CovInfoFromPath = covInfo
    End Function

    Public Class GenericChartContextMenuStrip
        Inherits System.Windows.Forms.ContextMenuStrip

        Private mChart As Chart
        Private mParentForm
        Private WithEvents lockChart As New System.Windows.Forms.ToolStripMenuItem("Lock chart")
        Private WithEvents ChangeSymbology As New System.Windows.Forms.ToolStripMenuItem("Format Chart")
        Private WithEvents CloneChart As New System.Windows.Forms.ToolStripMenuItem("Clone Chart")
        Private WithEvents PrintSetup As New System.Windows.Forms.ToolStripMenuItem("Print setup")
        Private WithEvents PrintPreview As New System.Windows.Forms.ToolStripMenuItem("Print Preview")
        Private WithEvents PrintChart As New System.Windows.Forms.ToolStripMenuItem("Print")

        Private WithEvents CopyChart As New System.Windows.Forms.ToolStripMenuItem("Copy")
        Private WithEvents SaveChart As New System.Windows.Forms.ToolStripMenuItem("Save")

        Public Sub New(myChart, myParentForm)
            mChart = myChart
            mParentForm = myParentForm
            Me.Items.Add(lockChart)
            Me.Items.Add(ChangeSymbology)
            Me.Items.Add(CloneChart)
            Dim tss As New ToolStripSeparator()
            Me.Items.Add(tss)
            Me.Items.Add(PrintSetup)
            Me.Items.Add(PrintPreview)
            Me.Items.Add(PrintChart)
            Me.Items.Add(CopyChart)
            Me.Items.Add(SaveChart)

        End Sub

        Public Sub setLockState(locked As Boolean)
            If locked Then
                lockChart.Text = "Unlock chart"
            Else
                lockChart.Text = "Lock chart"
            End If
        End Sub

        Private Sub lockClicked(sender As System.Object, e As System.EventArgs) Handles lockChart.Click
            If sender.text = "Lock chart" Then
                sender.text = "Unlock chart"
                mParentForm.Text = mParentForm.Text + " (*)"
                mParentForm.bLocked = True
            Else
                sender.text = "Lock chart"
                mParentForm.Text = mParentForm.Text.Substring(0, Len(mParentForm.Text) - 4)
                mParentForm.bLocked = False
            End If
        End Sub

        Private Sub ChangeSymbology_Click(sender As System.Object, e As System.EventArgs) Handles ChangeSymbology.Click
            Dim chartFormater As New ChartDisplayControl
            chartFormater.setChart(mChart, mParentForm.Text, mParentForm.mainDataManager, mParentForm.ChartType)
            chartFormater.Show()
        End Sub
        Private Sub CloneChart_Click(sender As System.Object, e As System.EventArgs) Handles CloneChart.Click
            If Object.ReferenceEquals(mParentForm.GetType(), GetType(aggHabitatGraphForm)) Then
                Dim aggHab As aggHabitatGraphForm = mParentForm.mainGraphManager.addAggHabitatGraphForm(True)
                aggHab.curDisplayData = mParentForm.curDisplayData
                Dim chartXML As XmlNode = mParentForm.maindatamanager.serializeChartSymbologyToXML(mParentForm.HabitatChart)
                mParentForm.maindatamanager.symbolizeChartFromXML(aggHab.HabitatChart, chartXML)
                aggHab.loadData()
            ElseIf Object.ReferenceEquals(mParentForm.GetType(), GetType(HydrographGraphForm)) Then
                Dim hydro As HydrographGraphForm = mParentForm.mainHydrographManager.addHydrograph(True)
                hydro.curDisplayData = mParentForm.curDisplayData
                Dim chartXML As XmlNode = mParentForm.maindatamanager.serializeChartSymbologyToXML(mParentForm.hydrographChart)
                mParentForm.maindatamanager.symbolizeChartFromXML(hydro.hydrographChart, chartXML)
                hydro.loadData()
            ElseIf Object.ReferenceEquals(mParentForm.GetType(), GetType(HabitatGraphForm)) Then
                Dim hab As HabitatGraphForm = mParentForm.mainGraphManager.addHabitatGraphForm(True)
                hab.curDisplayData = mParentForm.curDisplayData
                Dim chartXML As XmlNode = mParentForm.maindatamanager.serializeChartSymbologyToXML(mParentForm.habitatChart)
                mParentForm.maindatamanager.symbolizeChartFromXML(hab.HabitatChart, chartXML)
                hab.loadData()
            ElseIf Object.ReferenceEquals(mParentForm.GetType(), GetType(FlowVsHabitatChartForm)) Then
                Dim fvh As FlowVsHabitatChartForm = mParentForm.mainGraphManager.addflowVsHabitatGraphForm(True)
                fvh.curDisplayData = mParentForm.curDisplayData
                Dim chartXML As XmlNode = mParentForm.maindatamanager.serializeChartSymbologyToXML(mParentForm.habitatChart)
                mParentForm.maindatamanager.symbolizeChartFromXML(fvh.HabitatChart, chartXML)
                fvh.loadData()
            End If


        End Sub


        Private Sub PrintSetup_Click(sender As System.Object, e As System.EventArgs) Handles PrintSetup.Click
            mChart.Printing.PageSetup()
        End Sub

        Private Sub PrintChart_Click(sender As System.Object, e As System.EventArgs) Handles PrintChart.Click
            mChart.Printing.Print(True)
        End Sub

        Private Sub PrintPreview_Click(sender As System.Object, e As System.EventArgs) Handles PrintPreview.Click
            mChart.Printing.PrintPreview()
        End Sub

        Private Sub CopyChart_Click(sender As System.Object, e As System.EventArgs) Handles CopyChart.Click
            'Create a memory stream to save the chart image    
            Dim stream As New System.IO.MemoryStream()

            ' Save the chart image to the stream    
            mChart.SaveImage(stream, System.Drawing.Imaging.ImageFormat.Png)

            ' Create a bitmap using the stream    
            Dim bmp As New Bitmap(stream)

            ' Save the bitmap to the clipboard    
            Clipboard.SetDataObject(bmp)

        End Sub

        Private Sub SaveChart_Click(sender As System.Object, e As System.EventArgs) Handles SaveChart.Click
            ' Create a new save file dialog
            Dim saveFileDialog1 As New SaveFileDialog()

            ' Sets the current file name filter string, which determines 
            ' the choices that appear in the "Save as file type" or 
            ' "Files of type" box in the dialog box.
            saveFileDialog1.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|EMF (*.emf)|*.emf|PNG (*.png)|*.png|SVG (*.svg)|*.svg|GIF (*.gif)|*.gif|TIFF (*.tif)|*.tif"
            saveFileDialog1.FilterIndex = 2
            saveFileDialog1.RestoreDirectory = True

            ' Set image file format
            If saveFileDialog1.ShowDialog() = DialogResult.OK Then
                Dim format As ChartImageFormat = ChartImageFormat.Bmp

                If saveFileDialog1.FileName.EndsWith("bmp") Then
                    format = ChartImageFormat.Bmp
                Else
                    If saveFileDialog1.FileName.EndsWith("jpg") Then
                        format = ChartImageFormat.Jpeg
                    Else
                        If saveFileDialog1.FileName.EndsWith("emf") Then
                            format = ChartImageFormat.Emf
                        Else
                            If saveFileDialog1.FileName.EndsWith("gif") Then
                                format = ChartImageFormat.Gif
                            Else
                                If saveFileDialog1.FileName.EndsWith("png") Then
                                    format = ChartImageFormat.Png
                                Else
                                    If saveFileDialog1.FileName.EndsWith("tif") Then
                                        format = ChartImageFormat.Tiff
                                    End If
                                End If ' Save image
                            End If
                        End If
                    End If
                End If
                mChart.SaveImage(saveFileDialog1.FileName, format)
            End If
        End Sub
    End Class


    Public Function getApplicationSetting(strWhich As String) As String

        If System.IO.File.Exists(My.Settings.ConfigXML) Then
            Dim config As New Xml.XmlDocument
            config.Load(My.Settings.ConfigXML)
            Return config.SelectSingleNode("SmartRiverConfig/Settings/Application/" & strWhich).FirstChild.InnerText
        Else
            Return My.Settings.Item(strWhich)
        End If
    End Function


    Public Sub addTableCol(ByRef ds As DataSet, ByRef mainTable As DataTable, tempTable As DataTable, inColName As String, newColName As String)
        'mainTable.Columns.Add(newColName, GetType(System.Decimal))
        'Dim i As Integer
        'For Each r In mainTable.Rows
        '    r(newColName) = tempTable.Rows(i)(inColName)
        '    i += 1
        'Next

        'Dim Result = From mt In mainTable.AsEnumerable() _
        '    Join tt In tempTable.AsEnumerable() _
        '    On mt.Item("date") Equals tt.Item("date")
        '    Select New DataRow( _
        '      .ShelfID = mt.Field(Of Date)("date"), _
        '      .ShelfLocation = mt.Field(Of Decimal)(1), _
        '      .ShelfClassification = mt.Field(Of Decimal)(2), _
        '       .BookName = tt.Field(Of Decimal)("InterHabitat"))

        'Dim test As DataTable = Result.CopyToDataTable

        ds.Tables.Add(tempTable)
        Dim obj_PrimaryClmn(1) As System.Data.DataColumn
        obj_PrimaryClmn(0) = tempTable.Columns(0)
        tempTable.PrimaryKey = obj_PrimaryClmn

        ds.Relations.Add(tempTable.TableName, mainTable.Columns("DateVal"), tempTable.Columns("DateVal"), False)
        'ds.Relations.Add(tempTable.TableName, tempTable.Columns("DateVal"), mainTable.Columns("DateVal"))
        'mainTable.Columns.Add(newColName, GetType(System.Decimal), tempTable.TableName + "." + inColName)
        ds.EnforceConstraints = False
        mainTable.Merge(tempTable, False)
        'mainTable.Merge(tempTable)
        mainTable.Columns("InterHabitat").ColumnName = newColName
    End Sub

    'Public Function MergeDataTables(ByVal dtb1 As DataTable, ByVal dtb2 As DataTable, ByVal dtb1MatchField As String, ByVal dtb2MatchField As String) As DataTable
    '    Dim dtbOutput As DataTable = dtb1.Copy
    '    Dim lstSkipFields As New List(Of String)
    '    For Each dcl As DataColumn In dtb2.Columns
    '        Try
    '            dtbOutput.Columns.Add(dcl.ColumnName, dcl.DataType)
    '        Catch ex As DuplicateNameException
    '            lstSkipFields.Add(dcl.ColumnName)
    '        End Try
    '    Next dcl
    '    'Merge dtb2 records that match existing records in dtb1
    '    Dim dtb2Temp As DataTable = dtb2.Copy
    '    For int2 As Integer = dtb2Temp.Rows.Count - 1 To 0 Step -1
    '        Dim drw2 As DataRow = dtb2Temp.Rows(int2)
    '        Dim o2 As Object = drw2(dtb2MatchField)
    '        For Each drw1 As DataRow In dtbOutput.Rows
    '            Dim o1 As Object = drw1(dtb1MatchField)
    '            If o1.ToString = o2.ToString Then
    '                For Each dcl As DataColumn In dtb2Temp.Columns
    '                    If Not lstSkipFields.Contains(dcl.ColumnName) Then
    '                        drw1(dcl.ColumnName) = drw2(dcl.ColumnName)
    '                    End If
    '                Next dcl
    '                dtb2Temp.Rows.Remove(drw2)
    '            End If
    '        Next drw1
    '    Next int2
    '    'add rows that weren't in dtb1
    '    For Each drw2 As DataRow In dtb2Temp.Rows
    '        Dim drw1 As DataRow = dtbOutput.NewRow
    '        For Each dcl As DataColumn In dtb2Temp.Columns
    '            drw1(dcl.ColumnName) = drw2(dcl.ColumnName)
    '        Next dcl
    '        dtbOutput.Rows.Add(drw1)
    '    Next drw2
    '    Return dtbOutput
    'End Function


    Public Sub sumTableCol(ByRef mainTable As DataTable, tempTable As DataTable, inColName As String, newColName As String)
        Dim i As Integer
        For Each r In mainTable.Rows
            r(newColName) = r(newColName) + tempTable.Rows(i)(inColName)
            i += 1
        Next

    End Sub

    Public Sub addWarning(mainDataManager, dataChanged, bAcceptedNovelDate, thisChart, lblUniqueDate)
        If Not mainDataManager.haveShownChangedDataWarning And _
            dataChanged = True Then
            Dim msg As String
            msg = "You have changed the data being displayed in this chart."
            msg += vbCrLf + vbCrLf + "The chart title, axis labels, etc. "
            msg += "DO NOT AUTOMATICALLY UPDATE to reflect these changed data."
            msg += vbCrLf + vbCrLf + "You must manually update these (if needed) by"
            msg += vbCrLf + "right clicking in the chart and selecting 'Format Chart'."
            msg += vbCrLf + "The form that appears has items for changing these labels."
            msg += vbCrLf + vbCrLf + "This message will not be shown again."
            MsgBox(msg, MsgBoxStyle.Information, "Update Chart Labels Warning")
            mainDataManager.haveShownChangedDataWarning = True
        End If


        Try
            If dataChanged And _
                Not bAcceptedNovelDate Then

                thisChart.BorderlineWidth = 7
                thisChart.BorderlineDashStyle = ChartDashStyle.Solid
                thisChart.BorderlineColor = Color.DarkRed
                lblUniqueDate.Visible = True

            Else
                thisChart.BorderlineDashStyle = ChartDashStyle.NotSet
                lblUniqueDate.Visible = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    '<SQLiteFunction(Name:="aveBottom25", Arguments:=-1, FuncType:=FunctionType.Aggregate)> _
    ' Aggregate user-defined function.  Arguments = -1 means any number of arguments is acceptable
    ''' </summary>
    <SQLiteFunction(Name:="aveBottom25", Arguments:=-1, FuncType:=FunctionType.Aggregate)> _
    Class test25
        Inherits SQLiteFunction
        ': SQLiteFunction
        Public Overrides Sub [Step](args As Object(), nStep As Integer, ByRef contextData As Object)
            If contextData Is Nothing Then
                contextData = New List(Of Double)
            End If
            contextData.Add(CDbl(args(0)))
        End Sub

        Public Overrides Function Final(contextData As Object) As Object
            contextData.sort()
            contextData = contextData.GetRange(0, contextData.count / 4)
            Return Convert.ToString(Enumerable.Average(contextData))
        End Function
    End Class

    Public Function isSessionDirectory(strDir As String) As String
        'Checks wether the specified directory is a valid REFDSS session directory
        'To pass it must have a config.xml file at the root, a sqliteDB called REF_data.sqlite, and a folder called Outputs
        Dim validSD As Boolean = True
        If Not Directory.Exists(strDir) Then
            Return "Directory doesn't exist"
        End If
        If Not File.Exists(Path.Combine(strDir, "config.xml")) Then
            Return "There is no 'config.xml' file in " + strDir
        End If
        If Not File.Exists(Path.Combine(strDir, "REFDSS_data.sqlite")) Then
            Return "There is no 'REFDSS_data.sqlite' file in " + strDir
        End If
        'Check if the sqlite DB is legit
        Try
            Dim mainSQLDBConnection As New SQLite.SQLiteConnection()
            Dim connectionstring As String = "Data Source=" & Path.Combine(strDir, "REFDSS_data.sqlite") & ";"
            mainSQLDBConnection.ConnectionString = connectionstring
            If mainSQLDBConnection.State = ConnectionState.Closed Then
                mainSQLDBConnection.Open()
            End If
            Dim SQLcommand As SQLiteCommand
            SQLcommand = mainSQLDBConnection.CreateCommand
            SQLcommand.CommandText = "SELECT * FROM sqlite_master WHERE type='table';"
            SQLcommand.ExecuteNonQuery()
            mainSQLDBConnection.Close()
        Catch ex As Exception
            Return "'REF_data.sqlite' is invalid"
        End Try

        Try
            Dim config As New XmlDocument
            config.Load(Path.Combine(strDir, "config.xml"))
        Catch ex As Exception
            Return "'config.xml' is invalid"
        End Try

        'If all of the above are good but no Outputs directory then we'll go ahead and make one
        If Not Directory.Exists(Path.Combine(strDir, "Outputs")) Then
            MkDir(Path.Combine(strDir, "Outputs"))
        End If

        'We passed all tests!
        Return "True"
    End Function

End Module
