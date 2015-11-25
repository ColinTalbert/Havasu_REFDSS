Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Data.SQLite
Imports System.Xml
Imports System.IO
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Xml.Serialization
Imports WeifenLuo.WinFormsUI.Docking
Imports System.Text.RegularExpressions
Imports System.Net
Imports System.Text
Imports Ionic.Zip


Public Class DataManager
    Public validConfig As String = "True"

    Private RawDataForms As New List(Of RawDataForm)
    Private FigureForms As New List(Of FigureForm)

    Public config As New XmlDocument
    Public inputsFolder As String
    Public parentMainForm As MainForm
    Public mainSQLDBConnection As New SQLite.SQLiteConnection()

    Public defaultSeriesColors As New Queue(Of Color)
    Public seriesSymbology As New Dictionary(Of String, seriesSymbol)
    Public dgvNames As New Dictionary(Of String, String)

    Public curStartYear As Integer
    Public curEndYear As Integer

    Public curUnits As String = "Metric"

    Public processMissingData As String = "unknown"
    Public downloadMissingInputs As String = "unknown"

    Public haveShownChangedDataWarning As Boolean = True

    Public Sub New(mf As MainForm)

        parentMainForm = mf

        loadCustomSeriesColors()


        loadData()
    End Sub

    Public Sub ClearRawDataForms()
        RawDataForms.Clear()
    End Sub

    Public Function addRawDataForm(Optional show As Boolean = True)
        'Generate a new raw data form and add it to our queue
        'This form loads with the default values
        Dim dataForm As New RawDataForm(Me)
        dataForm.Text = "Data " + CStr(RawDataForms.Count + 1)
        RawDataForms.Add(dataForm)
        If show Then
            'set this hydrograph to the default symbology and data
            dataForm.curDisplayData = parentMainForm.mainDataManager.getDefaultDisplayData("RawDataForm")
            dataForm.curDisplayData.startYear = parentMainForm.mainDataManager.curStartYear
            dataForm.Show(parentMainForm.mainDockPanel, DockState.DockBottom)

            dataForm.loadData()
        End If
        Return dataForm
    End Function

    Public Sub removeRawDataForm(dataForm As RawDataForm)
        'remove an existing RawDataForm from our Queue
        RawDataForms.Remove(dataForm)
        reOrderRawDataForms()
    End Sub

    Public Sub reOrderRawDataForms()
        'Update the label numbering on our RawDataForms so they remain sequential
        Dim index As Integer = 1
        For Each dataForm As RawDataForm In RawDataForms
            If Not dataForm.bLocked Then
                dataForm.Text = "Data " + CStr(index)
            Else
                dataForm.Text = "Data " + CStr(index) + " (*)"
            End If
            index += 1
        Next


    End Sub

    Public Function addFigureForm(Optional strWhich As String = "", Optional show As Boolean = True)
        'Generate a new raw data form and add it to our queue
        'This form loads with the default values
        Dim FigureForm As New FigureForm(Me)


        If show Then
            FigureForm.Text = strWhich
            'set this hydrograph to the default symbology and data
            FigureForm.Show(parentMainForm.mainDockPanel, DockState.Float)

            FigureForm.loadData()
        End If
        FigureForms.Add(FigureForm)
        FigureForm.Tag = "FigureForm:" + CStr(FigureForms.Count)
        Return FigureForm
    End Function

    Public Sub removeFigureForm(dataForm As FigureForm)
        'remove an existing RawDataForm from our Queue
        FigureForms.Remove(dataForm)
        reOrderFigureForms()
    End Sub

    Public Sub reOrderFigureForms()
        'Update the label numbering on our RawDataForms so they remain sequential
        Dim index As Integer = 1
        For Each FigureForm As FigureForm In FigureForms
            FigureForm.Tag = "FigureForm:" + CStr(index)
            index += 1
        Next
    End Sub

    Public Sub updateLoadData()
        For Each dataForm As RawDataForm In RawDataForms
            If Not dataForm.bLocked Then
                dataForm.curDisplayData.startYear = curStartYear
                dataForm.curDisplayData.endYear = curEndYear
                dataForm.loadData()
            End If
        Next

        For Each figForm As FigureForm In FigureForms
            figForm.loadData()
        Next


    End Sub



    Private Sub loadData()
        Me.validConfig = validateInputs()
    End Sub

    Public Sub saveconfig()
        If Me.validConfig = "True" Then
            Dim strTmpOutFile As String = My.Settings.OutputDataDirectory + "\tmpLayout.xml"
            parentMainForm.mainDockPanel.SaveAsXml(strTmpOutFile)

            Dim LayoutConfig As New XmlDocument
            LayoutConfig.Load(strTmpOutFile)

            Dim rootNode As XmlNode = LayoutConfig.SelectSingleNode("DockPanel")

            storeCurrentSeriesSymbology()
            storeCurrentDGVNames()

            storeCurrentGUISettings()

            Dim GUIConfig As XmlNode
            GUIConfig = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig")

            Dim layoutConfigNode As XmlNode
            layoutConfigNode = config.ImportNode(rootNode, True)

            GUIConfig.AppendChild(layoutConfigNode)

            setSetting("OutputDataDirectory", My.Settings.OutputDataDirectory)
            setSetting("InputDataDirectory", My.Settings.InputDataDirectory)
            setSetting("SQLiteDB", My.Settings.SQliteDB)
            config.Save(My.Settings.ConfigXML)
            My.Settings.Save()
        End If
    End Sub

    Public Function layoutStored() As Boolean
        Dim dockPanel As XmlNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/DockPanel")
        Return Not IsNothing(dockPanel)
    End Function

    Public Sub saveLayoutXML(strPath As String)
        Dim LayoutConfig As New XmlDocument
        Dim dockPanel As XmlNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/DockPanel")


        Dim objWriter As New StreamWriter(strPath, False, System.Text.Encoding.UTF8)
        objWriter.Write("<?xml version=""1.0"" encoding=""utf-8""?>")
        objWriter.Write("<!--DockPanel configuration file. Author: Weifen Luo, all rights reserved.-->")
        objWriter.Write("<!--DockPanel configuration file. Author: Weifen Luo, all rights reserved.-->")
        objWriter.Write(dockPanel.OuterXml)
        objWriter.Close()

    End Sub

    Public Sub storeCurrentGUISettings()

        Dim GUIConfig As XmlNode
        GUIConfig = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig")
        If Not IsNothing(GUIConfig) Then
            GUIConfig.ParentNode.RemoveChild(GUIConfig)
        End If


        GUIConfig = config.CreateElement("GUIConfig")


        'Loop through each of our widgets and store it's specifics in it's node
        Dim dc_typed As Object
        For Each dc In parentMainForm.mainDockPanel.Contents
            dc_typed = (getTypedDC(dc))
            GUIConfig.AppendChild(dc_typed.saveToXMLNode())
        Next

        Dim globalTimePeriod As XmlNode
        globalTimePeriod = config.CreateElement("GlobalTimePeriod")
        Dim startYearNode As XmlNode
        startYearNode = config.CreateElement("StartYear")
        startYearNode.InnerText = CStr(curStartYear)
        Dim endYearNode As XmlNode
        endYearNode = config.CreateElement("EndYear")
        endYearNode.InnerText = CStr(curEndYear)
        globalTimePeriod.AppendChild(startYearNode)
        globalTimePeriod.AppendChild(endYearNode)
        GUIConfig.AppendChild(globalTimePeriod)

        Dim units As XmlNode
        units = config.CreateElement("curUnits")
        units.InnerText = curUnits
        GUIConfig.AppendChild(units)

        config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current").AppendChild(GUIConfig)

    End Sub

    Public Function serializeFontToXML(fnt As Font, Optional clr As Color = Nothing) As XmlNode
        Dim fontNode As XmlNode
        fontNode = config.CreateElement("Font")

        Dim fontNameNode As XmlNode = config.CreateElement("Name")
        fontNameNode.InnerText = fnt.Name
        fontNode.AppendChild(fontNameNode)

        Dim fontSizeNode As XmlNode = config.CreateElement("Size")
        fontSizeNode.InnerText = CStr(fnt.Size)
        fontNode.AppendChild(fontSizeNode)

        Dim fontBoldNode As XmlNode = config.CreateElement("Style")
        fontBoldNode.InnerText = CStr(fnt.Style)
        fontNode.AppendChild(fontBoldNode)

        Dim fontColorNode As XmlNode = config.CreateElement("Color")
        If IsNothing(clr) Then
            clr = stringToColor("255,255,255")

        End If
        fontColorNode.InnerText = colorToString(clr)
        fontNode.AppendChild(fontColorNode)

        'Dim fontItalicNode As XmlNode = config.CreateElement("Italic")
        'fontItalicNode.InnerText = CStr(fnt.Italic)
        'fontNode.AppendChild(fontItalicNode)

        Return fontNode

    End Function

    Public Function deserializeFont(xmlFontNode As XmlNode) As Font
        'note that the color variable must be deserialized separatately
        Dim name As String = xmlFontNode.SelectSingleNode("Name").InnerText
        Dim size As Single = CSng(xmlFontNode.SelectSingleNode("Size").InnerText)
        Dim fStyle As FontStyle = FontStyle.Bold
        Dim outFont As New Font(name, size, fStyle)
        Return outFont
    End Function

    Public Function serializeLegendToXML(lgnd As Legend) As XmlNode

        Dim legendNode As XmlNode = config.CreateElement("Legend")
        legendNode.AppendChild(serializeFontToXML(lgnd.Font, lgnd.ForeColor))
        Dim legendPlacement As XmlNode = config.CreateElement("placement")

        If lgnd.Docking = Docking.Bottom Then
            legendPlacement.InnerText = "bottom"
        ElseIf lgnd.Docking = Docking.Top Then
            legendPlacement.InnerText = "top"
        ElseIf lgnd.Docking = Docking.Right Then
            legendPlacement.InnerText = "right"
        ElseIf lgnd.Docking = Docking.Left Then
            legendPlacement.InnerText = "left"
        ElseIf lgnd.InsideChartArea = "Default" Then
            legendPlacement.InnerText = "inside"
        ElseIf lgnd.Enabled = False Then
            legendPlacement.InnerText = "node"
        End If
        legendNode.AppendChild(legendPlacement)
        Return legendNode
    End Function

    Public Function serializeSingleAxisToXML(ax As Axis, nodeName As String) As XmlNode
        Dim axisNode As XmlNode = config.CreateElement(nodeName)

        Dim axisTitleNode As XmlNode = config.CreateElement("AxisTitle")
        Dim axisTitleTextNode As XmlNode = config.CreateElement("TitleText")
        axisTitleTextNode.InnerText = ax.Title
        axisTitleNode.AppendChild(axisTitleTextNode)
        axisTitleNode.AppendChild(serializeFontToXML(ax.TitleFont, ax.TitleForeColor))
        axisNode.AppendChild(axisTitleNode)

        Dim axisScaleNode As XmlNode = config.CreateElement("AxisScale")
        Dim scaleLogrithmicNode As XmlNode = config.CreateElement("Logrithmic")
        scaleLogrithmicNode.InnerText = CStr(ax.IsLogarithmic)
        axisScaleNode.AppendChild(scaleLogrithmicNode)
        Dim autoscaleNode As XmlNode = config.CreateElement("Autoscale")
        autoscaleNode.InnerText = CStr(ax.Maximum = [Double].NaN)
        axisScaleNode.AppendChild(autoscaleNode)
        Dim MaxNode As XmlNode = config.CreateElement("Max")
        MaxNode.InnerText = CStr(ax.Maximum)
        axisScaleNode.AppendChild(MaxNode)
        Dim MinNode As XmlNode = config.CreateElement("Min")
        MinNode.InnerText = CStr(ax.Minimum)
        axisScaleNode.AppendChild(MinNode)
        Dim IntervalNode As XmlNode = config.CreateElement("Interval")
        IntervalNode.InnerText = CStr(ax.Interval)
        axisScaleNode.AppendChild(IntervalNode)


        axisScaleNode.AppendChild(serializeFontToXML(ax.LabelStyle.Font, ax.LabelStyle.ForeColor))
        axisNode.AppendChild(axisScaleNode)

        Return axisNode

    End Function

    Public Function serializeAllAxesToXML(chrtArea As ChartArea) As XmlNode
        Dim axesNode As XmlNode = config.CreateElement("Axes")
        axesNode.AppendChild(serializeSingleAxisToXML(chrtArea.AxisX, "XAxis"))
        axesNode.AppendChild(serializeSingleAxisToXML(chrtArea.AxisY, "YAxis"))
        axesNode.AppendChild(serializeSingleAxisToXML(chrtArea.AxisY2, "Y2Axis"))
        Return axesNode
    End Function

    Public Function serializeChartSymbologyToXML(chrt As Chart) As XmlNode
        Dim chartSym As XmlNode = config.CreateElement("ChartSymbology")

        chrt.Serializer.Content = SerializationContents.All
        chrt.Serializer.NonSerializableContent = "Chart.Series, *.StripLines"
        'chrt.Serializer.SerializableContent = "Legend.*"

        Dim w As New StringWriter
        chrt.Serializer.Save(w)
        'chartSym.InnerXml = w.ToString


        Dim xdoc As New XmlDocument
        xdoc.LoadXml(w.ToString)

        Dim returnNode As XmlNode = config.ImportNode(xdoc.ChildNodes(1), True)
        Return returnNode

    End Function

    Public Sub symbolizeChartFromXML(chrt As Chart, chartSymbologyNode As XmlNode)

        Dim sReader As New StringReader(chartSymbologyNode.OuterXml)
        chrt.Serializer.Load(sReader)

    End Sub

    Public Function serializeChartDisplayDataToXML(cdd As chartDisplayData) As XmlNode

        Dim x As New XmlSerializer(cdd.GetType)
        Dim w As New StringWriter
        x.Serialize(w, cdd)

        Dim xdoc As New XmlDocument
        xdoc.LoadXml(w.ToString)

        Dim returnNode As XmlNode = config.ImportNode(xdoc.ChildNodes(1), True)
        Return returnNode

    End Function

    Public Function deserializeChartDisplayDataFromXML(cddNode As XmlNode) As chartDisplayData
        Dim x As New XmlSerializer(GetType(chartDisplayData))
        Dim sReader As New StringReader(cddNode.OuterXml)

        Dim outCdd As New chartDisplayData
        outCdd = x.Deserialize(sReader)
        sReader.Close()
        Return (outCdd)
    End Function

    Public Function serializeRectangleToXML(rect As Rectangle) As XmlNode

        Dim x As New XmlSerializer(rect.GetType)
        Dim w As New StringWriter
        x.Serialize(w, rect)

        Dim xdoc As New XmlDocument
        xdoc.LoadXml(w.ToString)

        Dim returnNode As XmlNode = config.ImportNode(xdoc.ChildNodes(1), True)
        Return returnNode

    End Function

    Public Function deserializeRectFromXML(rectNode As XmlNode) As Rectangle
        Dim x As New XmlSerializer(GetType(Rectangle))
        Dim sReader As New StringReader(rectNode.OuterXml)

        Dim outRect As New Rectangle
        outRect = x.Deserialize(sReader)
        sReader.Close()
        Return (outRect)
    End Function

    Public Function getDefaultDisplayData(strWhich As String) As chartDisplayData
        Dim defaultXMLNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/DefaultComponents/" & strWhich & "/chartDisplayData")
        Return deserializeChartDisplayDataFromXML(defaultXMLNode)
    End Function

    Public Sub applyDefaultChartSymbology(strWhich As String, chrt As Chart)
        Dim defaultXMLNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/DefaultComponents/" & strWhich & "/Chart")
        symbolizeChartFromXML(chrt, defaultXMLNode)
    End Sub

    Public Sub setDefaultChartSymbology(strWhich As String, symboNode As XmlNode)
        Dim defaultXMLNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/DefaultComponents/" & strWhich & "/Chart")
        defaultXMLNode.ParentNode.ReplaceChild(symboNode, defaultXMLNode)
        config.Save(My.Settings.ConfigXML)
    End Sub

    Public Sub setDefaultChartData(strWhich As String, symboNode As XmlNode)
        Dim defaultXMLNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/DefaultComponents/" & strWhich & "/chartDisplayData")
        defaultXMLNode.ParentNode.ReplaceChild(symboNode, defaultXMLNode)
        config.Save(My.Settings.ConfigXML)
    End Sub

    Public Sub loadPreviousGUISettings()

        'Store the current dictionary of seriesSymbology
        Dim chartSymNodes As XmlNodeList
        chartSymNodes = config.SelectNodes("SmartRiverConfig/GUIConfigs/SeriesSymbology/Series")
        seriesSymbology.Clear()
        For Each series As XmlNode In chartSymNodes
            Dim strName As String = series.SelectSingleNode("Name").InnerText
            Dim curColor As Color = stringToColor(series.SelectSingleNode("Color").InnerText)
            Dim curWidth As String = series.SelectSingleNode("Width").InnerText

            Dim lineStyleNode As XmlNode = series.SelectSingleNode("LineStyle")
            Dim curLineStyle As String
            If IsNothing(lineStyleNode) Then
                curLineStyle = "5"
            Else
                curLineStyle = series.SelectSingleNode("LineStyle").InnerText
            End If
            Dim testNode As XmlNode = series.SelectSingleNode("LegendText")
            Dim curLabel As String
            If IsNothing(testNode) Then
                curLabel = strName
            Else
                curLabel = testNode.InnerText
            End If

            Dim serSymbol As New seriesSymbol(curWidth, curColor, curLineStyle, , curLabel)

            seriesSymbology.Add(strName, serSymbol)
        Next

        'Store the current dictionary of dgvNames
        Dim DGVNameNodes As XmlNodeList
        DGVNameNodes = config.SelectNodes("SmartRiverConfig/GUIConfigs/DGVNames/DGVName")
        dgvNames.Clear()
        For Each dgvName As XmlNode In DGVNameNodes
            Dim strOrigName As String = dgvName.SelectSingleNode("OriginalName").InnerText
            Dim strNewgName As String = dgvName.SelectSingleNode("NewName").InnerText

            dgvNames.Add(strOrigName, strNewgName)
        Next

        Dim dc_typed As Object
        For Each dc In parentMainForm.mainDockPanel.Contents
            dc_typed = (getTypedDC(dc))
            Dim curNode As XmlNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & dc_typed.Tag & "']")
            dc_typed.loadFromXMLNode(curNode)
            Try
                dc_typed.refreshAfterLoad()
            Catch ex As Exception

            End Try

        Next
    End Sub

    Private Function getTypedDC(ByRef DC As WeifenLuo.WinFormsUI.Docking.DockContent) As Object
        Dim dc_typed As Object
        If TypeOf DC Is HydrographGraphForm Then
            dc_typed = CType(DC, HydrographGraphForm)
        ElseIf TypeOf DC Is SegmentMapForm Then
            dc_typed = CType(DC, SegmentMapForm)
        ElseIf TypeOf DC Is OverviewMapForm Then
            dc_typed = CType(DC, OverviewMapForm)
        ElseIf TypeOf DC Is HabitatGraphForm Then
            dc_typed = CType(DC, HabitatGraphForm)
        ElseIf TypeOf DC Is SystemWideMetricForm Then
            dc_typed = CType(DC, SystemWideMetricForm)
        ElseIf TypeOf DC Is aggHabitatGraphForm Then
            dc_typed = CType(DC, aggHabitatGraphForm)
        ElseIf TypeOf DC Is HabitatSuitibilityGeneratorForm Then
            dc_typed = CType(DC, HabitatSuitibilityGeneratorForm)
        ElseIf TypeOf DC Is RawDataForm Then
            dc_typed = CType(DC, RawDataForm)
        ElseIf TypeOf DC Is FlowVsHabitatChartForm Then
            dc_typed = CType(DC, FlowVsHabitatChartForm)
        ElseIf TypeOf DC Is SegmentMapLegendForm Then
            dc_typed = CType(DC, SegmentMapLegendForm)
        ElseIf TypeOf DC Is FigureForm Then
            dc_typed = CType(DC, FigureForm)
        Else
            MsgBox("Add new type to 'getTypedDC' in DataManager")
        End If

        Return dc_typed
    End Function

    Public Function validateInputs() As String
        Try
            'Formost we need a valid session Directory
            'This is the first time a user has run the application. They must download and unzip the input data from ScienceBase 
            If My.Settings.SessionDirectory = "NA" Or Not isSessionDirectory(My.Settings.SessionDirectory) = "True" Then
                'This is the first time a user has run the application. They must download and unzip the input data from ScienceBase 
                Dim newWelcomeForm As New WelcomeToTheDSS
                Try
                    newWelcomeForm.Text = getApplicationSetting("WelcomeTitle")
                    newWelcomeForm.currentMainForm = Me.parentMainForm
                Catch ex As Exception
                    If My.Settings.verbose Then
                        MsgBox(ex.ToString)
                    End If
                End Try

                newWelcomeForm.ShowDialog()
                If Not isSessionDirectory(My.Settings.SessionDirectory) = "True" Then
                    'good luck soldier
                    Return "After obtaining default data please navigate to a valid session directory using the file menu, 'Open Session Directory' option."
                End If

            End If

            LoadSessionDirectory(My.Settings.SessionDirectory)

            'We must also check that the schema/data in the config.xml and sqlite db match what this version of the app is expecting.
            'If not download them from ScienceBase and 
            CheckAndUpdata_db()

            If Not System.IO.Directory.Exists(My.Settings.InputDataDirectory) Then

                Return "The specified input data directory:" & vbCrLf & _
                                    My.Settings.InputDataDirectory & vbCrLf & _
                                    "does not exist." & vbCrLf & vbCrLf & _
                                    "Please Navigate to a valid inputs directory using the file menu, 'Change Input Data Directory' option."
            End If


            'Outputs folder exists but is it populated?
            If Not areOutputsProcessed() Then
                Dim msg As String
                msg = "The geospatial habitat outputs don't appear to have been generated yet "
                msg += "in this output directory. Viewing of these layers will be disabled until they are generated"
                msg += " but the remaining functionality will be available."
                msg += vbCrLf & vbCrLf & "If you click 'Yes' below they will be generated automatically"
                msg += " but note that this process can take several hours."
                msg += vbCrLf & "If you click 'No', you will be able to view the inputs, change HSC values, view the output metrics, and regenerate the layers latter (if at all)."
                msg += vbCrLf & vbCrLf & "Would you like to generate them now?"
                Dim ans As VariantType = MsgBox(msg, MsgBoxStyle.YesNo, "Process Habitat Maps?")
                If ans = vbYes Then
                    Dim progbar As New habGenProgressBar(parentMainForm)
                    Dim _speciesToProcess As List(Of String) = New List(Of String) _
                    (New String() {"All"})
                    Dim _lifeStageToProcess As List(Of String) = New List(Of String) _
                    (New String() {"All"})

                    progbar._lifeStageToProcess = _lifeStageToProcess
                    progbar._speciesToProcess = _speciesToProcess
                    progbar.singleLS = False
                    parentMainForm.Cursor = Cursors.WaitCursor
                    parentMainForm.mainMapManager = New MapManager(parentMainForm)
                    parentMainForm.mainDataManager = Me
                    progbar.Show()
                    progbar.runHabitatMaps()
                    parentMainForm.Cursor = Cursors.Default

                End If
            End If
        Catch ex As Exception
            Return "Untrapped problem encountered validiating inputs" & vbCrLf & vbCrLf & "error message:" & ex.Message
        End Try

        Return "True"
    End Function

    Public Sub CheckAndUpdata_db()
        ' ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ''db portion not needed yet
        ' ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Dim curDB_version As Integer

        'Dim check_sql As String = "SELECT name FROM sqlite_master WHERE type='table' AND name='db_version'"
        'If mainSQLDBConnection.State = ConnectionState.Closed Then
        '    mainSQLDBConnection.Open()
        'End If

        'Dim SurveyDatatable As New DataTable
        'Dim sqlDA As New SQLite.SQLiteDataAdapter(check_sql, mainSQLDBConnection)
        'sqlDA.Fill(SurveyDatatable)


        'If SurveyDatatable.Rows.Count = 0 Then
        '    curDB_version = -1
        'Else
        '    Dim strSQL As String = "SELECT max(Version) as curVersion from 'db_version'"
        '    SurveyDatatable.Clear()
        '    Dim sqlDA2 As New SQLite.SQLiteDataAdapter(strSQL, mainSQLDBConnection)
        '    sqlDA2.Fill(SurveyDatatable)

        '    curDB_version = SurveyDatatable.Rows(0)("curVersion")
        'End If
        'mainSQLDBConnection.Close()

        'If My.Settings.db_version > curDB_version Then
        '    'dang it we need to update our xml to have some stuff we'll be expecting...
        '    Dim rootDownloadFolder As String = My.Settings.InputDataDirectory + "\" + "tempScienceBaseDownloads"
        '    If Not IO.Directory.Exists(rootDownloadFolder) Then
        '        IO.Directory.CreateDirectory(rootDownloadFolder)
        '    End If

        '    Dim url As String = My.Settings.db_SB

        '    Dim WC As WebClient = New WebClient()
        '    WC.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)")
        '    WC.DownloadFile(New Uri(url), rootDownloadFolder + "\REFDSS_data.sqlite")

        '    updateDBTo1(rootDownloadFolder + "\REFDSS_data.sqlite")
        'End If

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'XML config portion...
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim xpath As String = "SmartRiverConfig/ConfigVersion"
        Dim n As XmlNode = config.SelectSingleNode(xpath)

        Dim curXML_version As Integer
        If n Is Nothing Then
            curXML_version = -1
        Else
            curXML_version = CInt(n.FirstChild.Value)
        End If

        If curXML_version < 3 Then
            'dang it, they have some older version of the data nothing short of 
            Dim msg As String
            msg = "The current data are from a previous version of the Havasu REFDSS"
            msg += "and need to be replaced with the latest version to be compatible with this version of the application."
            msg += "Would you like to overwrite the current inputs and session data?" + vbCrLf + vbCrLf
            msg += vbCrLf & vbCrLf & "If you click 'Yes' previous data will be deleted and the newest versions will be downloaded and updated automatically" + vbCrLf + vbCrLf
            msg += vbCrLf & "If you click 'No', you will not be able to use the current version of this application" + vbCrLf + vbCrLf

            Dim ans As VariantType = MsgBox(msg, MsgBoxStyle.YesNo, "Overwrite current data?")
            If ans = vbYes Then
                msg = "Are you sure that it's ok delete the contents of:" + vbCrLf
                msg += My.Settings.InputDataDirectory + vbCrLf
                msg += My.Settings.SessionDirectory + vbCrLf

                Dim doublecheck_ans As VariantType = MsgBox(msg, MsgBoxStyle.YesNo, "Overwrite current data?")

                If doublecheck_ans = vbYes Then

                    mainSQLDBConnection.Close()
                    mainSQLDBConnection.Dispose()
                    GC.Collect()
                    GC.WaitForPendingFinalizers()

                    If System.IO.Directory.Exists(My.Settings.InputDataDirectory) Then
                        System.IO.Directory.Delete(My.Settings.InputDataDirectory, True)
                    End If
                    If System.IO.Directory.Exists(My.Settings.SessionDirectory) Then
                        System.IO.Directory.Delete(My.Settings.SessionDirectory, True)
                    End If


                    Dim x As New ScienceBaseDownloader(Me.parentMainForm, "CoreData")
                    x.StartDownload()
                    x.ShowDialog()

                    If x.canceled Then
                        'do nothing
                    End If

                    mainSQLDBConnection = New SQLite.SQLiteConnection()
                    LoadSessionDirectory(My.Settings.SessionDirectory)
                End If
            End If
        End If


        '    Dim rootDownloadFolder As String = My.Settings.InputDataDirectory + "\" + "tempScienceBaseDownloads"
        '    If Not IO.Directory.Exists(rootDownloadFolder) Then
        '        IO.Directory.CreateDirectory(rootDownloadFolder)
        '    End If

        '    Dim url As String = My.Settings.xml_SB

        '    Dim WC As WebClient = New WebClient()
        '    WC.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)")
        '    WC.DownloadFile(New Uri(url), rootDownloadFolder + "\config.xml")

        '    updateConfigTo1(rootDownloadFolder + "\config.xml")
        'End If

        'If curXML_version < 2 Then
        '    updateConfigTo2(My.Settings.ConfigXML)
        'End If

        'If Not Directory.Exists(My.Settings.InputDataDirectory + "\Tier1") Then
        '    DownloadTier1()
        'End If

    End Sub

    Public Sub LoadSessionDirectory(strSessionDir)
        If isSessionDirectory(strSessionDir) Then

            My.Settings.SessionDirectory = strSessionDir
            parentMainForm.Text = "Havasu REFDSS - " + strSessionDir
            My.Settings.ConfigXML = Path.Combine(My.Settings.SessionDirectory, "config.xml")
            config.Load(My.Settings.ConfigXML)

            My.Settings.OutputDataDirectory = Path.Combine(My.Settings.SessionDirectory, "Outputs")
            setSetting("OutputDataDirectory", Path.Combine(My.Settings.SessionDirectory, "Outputs"))

            My.Settings.SQliteDB = Path.Combine(My.Settings.SessionDirectory, "REFDSS_data.sqlite")
            Dim connectionstring As String = "Data Source=" & My.Settings.SQliteDB & ";"
            mainSQLDBConnection.ConnectionString = connectionstring
            setSetting("SQLiteDB", My.Settings.SQliteDB)


            'Let's check for an inputs directory at the same level as our sessionDir
            If System.IO.Directory.Exists(Path.Combine(Directory.GetParent(My.Settings.SessionDirectory).FullName, "inputs")) Then
                My.Settings.InputDataDirectory = Path.Combine(Directory.GetParent(My.Settings.SessionDirectory).FullName, "inputs")
                setSetting("InputDataDirectory", My.Settings.InputDataDirectory)
            End If

            'Save all the changes to both the config.xml and application.settings
            config.Save(My.Settings.ConfigXML)
            My.Settings.Save()
        End If
    End Sub


    Public Function findRelativeFile(fname As String)
        'First is fname exists use it directly
        If System.IO.File.Exists(fname) Or System.IO.Directory.Exists(fname) Then
            Return fname
        End If

        Dim configHome As String = returnParentFolder(My.Settings.ConfigXML)
        Dim inputHome As String = returnParentFolder(My.Settings.InputDataDirectory)
        Dim outputHome As String = returnParentFolder(My.Settings.OutputDataDirectory)
        Dim dbHome As String = returnParentFolder(My.Settings.SQliteDB)

        Try
            'next see if the fname is a child of any of the four root spaces
            If System.IO.File.Exists(Path.Combine(configHome, fname)) Or _
                System.IO.Directory.Exists(Path.Combine(configHome, fname)) Then
                Return Path.Combine(configHome, fname)
            ElseIf System.IO.File.Exists(Path.Combine(inputHome, fname)) Or _
                System.IO.Directory.Exists(Path.Combine(inputHome, fname)) Then
                Return Path.Combine(inputHome, fname)
            ElseIf System.IO.File.Exists(Path.Combine(outputHome, fname)) Or _
                System.IO.Directory.Exists(Path.Combine(outputHome, fname)) Then
                Return Path.Combine(outputHome, fname)
            ElseIf System.IO.File.Exists(Path.Combine(dbHome, fname)) Or _
                System.IO.Directory.Exists(Path.Combine(dbHome, fname)) Then
                Return Path.Combine(dbHome, fname)
            End If
        Catch ex As Exception

        End Try

        Try
            Dim justname1 As String = Path.GetFileName(fname)
            If justname1 = String.Empty Then
                justname1 = Path.GetDirectoryName(fname)
            End If
            'next see if the fname is a child of any of the four root spaces
            If System.IO.File.Exists(Path.Combine(configHome, justname1)) Or _
                System.IO.Directory.Exists(Path.Combine(configHome, justname1)) Then
                Return Path.Combine(configHome, justname1)
            ElseIf System.IO.File.Exists(Path.Combine(inputHome, justname1)) Or _
                System.IO.Directory.Exists(Path.Combine(inputHome, justname1)) Then
                Return Path.Combine(inputHome, justname1)
            ElseIf System.IO.File.Exists(Path.Combine(outputHome, justname1)) Or _
                System.IO.Directory.Exists(Path.Combine(outputHome, justname1)) Then
                Return Path.Combine(outputHome, justname1)
            ElseIf System.IO.File.Exists(Path.Combine(dbHome, justname1)) Or _
                System.IO.Directory.Exists(Path.Combine(dbHome, justname1)) Then
                Return Path.Combine(dbHome, justname1)
            End If
        Catch ex As Exception

        End Try

        'We failed to find the file at all.  
        Return "could not find the file or directory"


    End Function

    Public Sub removeBreakNodes(strSpecies As String, strLifeStage As String, strVariable As String)

        Dim BreakNodes As XmlNodeList
        BreakNodes = config.SelectNodes("SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpecies & "']/Lifestages/Lifestage[Name='" & strLifeStage & "']/HSC/Variable[Name='" & strVariable & "']/Break")
        For Each BreakNode As XmlNode In BreakNodes
            BreakNode.ParentNode.RemoveChild(BreakNode)
        Next
    End Sub

    Public Sub addBreakNode(strSpecies As String, strLifeStage As String, strVariable As String,
                                     min As String, max As String, yVal As String, strColor As String)
        Dim variableNode As XmlNode
        variableNode = config.SelectSingleNode("SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpecies & "']/Lifestages/Lifestage[Name='" & strLifeStage & "']/HSC/Variable[Name='" & strVariable & "']")

        If CDbl(max) < CDbl(min) Then
            max = "999999"
        End If

        Dim breakNode, minNode, maxNode, yValNode, colorNode As XmlNode
        breakNode = config.CreateElement("Break")
        minNode = config.CreateElement("min")
        minNode.InnerText = min
        maxNode = config.CreateElement("max")
        maxNode.InnerText = max
        yValNode = config.CreateElement("yValue")
        yValNode.InnerText = yVal
        colorNode = config.CreateElement("rgb")
        colorNode.InnerText = strColor

        breakNode.InsertAfter(minNode, breakNode.LastChild)
        breakNode.InsertAfter(maxNode, breakNode.LastChild)
        breakNode.InsertAfter(yValNode, breakNode.LastChild)
        breakNode.InsertAfter(colorNode, breakNode.LastChild)
        variableNode.InsertAfter(breakNode, variableNode.LastChild)


    End Sub

    Public Sub addCategoricalBreakNode(strSpecies As String, strLifeStage As String, strVariable As String,
                                 value As String, yVal As String, strColor As String, strlabel As String)
        Dim variableNode As XmlNode
        variableNode = config.SelectSingleNode("SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpecies & "']/Lifestages/Lifestage[Name='" & strLifeStage & "']/HSC/Variable[Name='" & strVariable & "']")


        Dim breakNode, valueNode, yValNode, colorNode, labelNode As XmlNode

        breakNode = config.CreateElement("Break")
        valueNode = config.CreateElement("value")
        valueNode.InnerText = value
        yValNode = config.CreateElement("yValue")
        yValNode.InnerText = yVal
        colorNode = config.CreateElement("rgb")
        colorNode.InnerText = strColor
        labelNode = config.CreateElement("label")
        labelNode.InnerText = strlabel

        breakNode.InsertAfter(valueNode, breakNode.LastChild)
        breakNode.InsertAfter(yValNode, breakNode.LastChild)
        breakNode.InsertAfter(colorNode, breakNode.LastChild)
        breakNode.InsertAfter(labelNode, breakNode.LastChild)
        variableNode.InsertAfter(breakNode, variableNode.LastChild)


    End Sub

    Public Sub importScenario(strFile As String, strScenarioName As String)
        'loads a scenario from an xls reformater into our sqlite db.
        Dim pattern As String = "[^a-zA-Z0-9_]"
        Dim replacement As String = ""
        Dim rgx As New Regex(pattern)
        Dim strAbbrev As String = rgx.Replace(strScenarioName, replacement)

        Dim progBar As New importScenarioProgressBar
        progBar.ProgressBar1.Minimum = 1
        progBar.Show()
        Application.DoEvents()

        'First open the xls reformater book and read in our data
        progBar.lblProgress.Text = "Opening data source: " & strFile
        Dim xlApp As New Excel.Application
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet

        xlWorkBook = xlApp.Workbooks.Open(strFile)
        xlWorkSheet = xlWorkBook.Worksheets("wse")

        Dim lastRow As Excel.Range
        lastRow = xlWorkSheet.Range("C" & xlWorkSheet.Rows.Count).End(Excel.XlDirection.xlUp)

        Dim r As Excel.Range = xlWorkSheet.Range(xlWorkSheet.Range("A2"), lastRow)

        Dim array(,) As Object = r.Value(Excel.XlRangeValueDataType.xlRangeValueDefault)

        xlWorkBook.Close()
        xlApp.Quit()

        releaseObject(xlApp)
        releaseObject(xlWorkBook)
        releaseObject(xlWorkSheet)
        Application.DoEvents()

        'second remove the table if it exists in our DB
        removeScenario(strAbbrev, True)

        'third add a table to the db to hold this scenario
        'The table name will be scenario_[name here]
        Dim strSQL As String
        strSQL = "CREATE  TABLE 'main'.'scenario_" & strAbbrev & "' ('Date' DATETIME UNIQUE"

        strSQL += ", '" & "topock1" & "' FLOAT"
        strSQL += ", '" & "ET_AF" & "' FLOAT"
        strSQL += ")"
        strSQL = strSQL.Replace("'", Chr(34))
        Dim SQLcommand As SQLiteCommand


        'SQL query to Create Table

        If mainSQLDBConnection.State = ConnectionState.Closed Then
            mainSQLDBConnection.Open()
        End If
        SQLcommand = mainSQLDBConnection.CreateCommand
        SQLcommand.CommandText = strSQL
        SQLcommand.ExecuteNonQuery()

        'now loop through the rows inserting each into the db

        Dim curDate As Date

        strSQL = ""
        Dim batch As Integer = 0

        Dim rowsToImport As ULong = array.GetUpperBound(0)


        progBar.ProgressBar1.Maximum = rowsToImport
        progBar.lblProgress.Text = "Importing data to database"

        For row As Integer = 1 To rowsToImport
            curDate = array(row, 1)

            strSQL += "INSERT INTO scenario_" & strAbbrev & " VALUES ('"
            strSQL += curDate.ToString("yyyy'-'MM'-'dd") + "'"

            For col = 2 To 3
                strSQL += ", " & array(row, col)

            Next
            strSQL += " );" & vbCrLf

            If batch > 1000 Or row = rowsToImport Then
                Debug.Print(Str(row))
                SQLcommand.CommandText = strSQL
                SQLcommand.ExecuteNonQuery()
                Debug.Print("pushed")
                strSQL = ""
                batch = 0
                progBar.lblProgress.Text = "Imported " & Str(row) & " out of " & rowsToImport & " rows into database"
            Else
                batch += 1
                progBar.ProgressBar1.Value = row
                Application.DoEvents()
            End If
            If progBar.canceled Then
                Exit For
            End If
        Next

        If Not progBar.canceled Then
            'Now add the node to our XML
            Dim scenariosNode, scenarioNode, fullName As XmlNode, abbrev As XmlNode

            scenariosNode = config.SelectSingleNode("SmartRiverConfig/Scenarios")
            scenarioNode = config.CreateElement("Scenario")
            fullName = config.CreateElement("fullName")
            fullName.InnerText = strScenarioName
            scenarioNode.InsertAfter(fullName, scenarioNode.LastChild)
            abbrev = config.CreateElement("Abbrev")

            abbrev.InnerText = strAbbrev
            scenarioNode.InsertAfter(abbrev, scenarioNode.LastChild)
            scenariosNode.InsertAfter(scenarioNode, scenariosNode.LastChild)
            config.Save(My.Settings.ConfigXML)
        Else
            SQLcommand.CommandText = "DROP TABLE 'main'.'scenario_" & strAbbrev & "';"
            SQLcommand.ExecuteNonQuery()
        End If

        SQLcommand.Dispose()
        mainSQLDBConnection.Close()
        progBar.Close()
    End Sub

    Public Sub removeScenario(strSenarioName As String, ignore As Boolean)



        Dim scenarioNames As List(Of String) = getScenarioNames()
        If scenarioNames.Count < 2 And Not ignore Then
            MsgBox("The DSS needs at least one scenario!  You cannot delete the last one.  Please add another scenario before deleting this one.", MsgBoxStyle.Critical, "Cannot delete last scenario")
            Exit Sub
        End If


        Dim strSQL As String
        strSQL = "DROP  TABLE if exists 'main'.'scenario_" & strSenarioName & "'"

        Dim SQLcommand As SQLiteCommand


        'SQL query to Create Table

        If mainSQLDBConnection.State = ConnectionState.Closed Then
            mainSQLDBConnection.Open()
        End If
        SQLcommand = mainSQLDBConnection.CreateCommand
        SQLcommand.CommandText = strSQL
        SQLcommand.ExecuteNonQuery()


        mainSQLDBConnection.Close()

        Dim replacementScenario As String = getScenarioAbbrev(scenarioNames(0))
        Dim i As Integer = 0
        While replacementScenario = strSenarioName
            replacementScenario = getScenarioAbbrev(scenarioNames(i))
            i += 1
        End While

        'second remove the scenario node from our xml if it's there.
        Dim scenarioNode As XmlNode
        Try
            scenarioNode = config.SelectSingleNode("SmartRiverConfig/Scenarios/Scenario[Abbrev='" & strSenarioName & "']")
            If Not scenarioNode Is Nothing Then
                scenarioNode.ParentNode.RemoveChild(scenarioNode)
            End If

            config.Save(My.Settings.ConfigXML)
        Catch ex As Exception
            If My.Settings.verbose Then
                MsgBox(ex.ToString)
            End If
        End Try

        'replace the baseline with our replacement if needed

        Dim scenarioRefs As XmlNodeList = config.SelectNodes("//curScenario")
        For Each scenarioRef As XmlNode In scenarioRefs
            If scenarioRef.FirstChild.InnerText = strSenarioName Then
                scenarioRef.FirstChild.InnerText = replacementScenario
            End If
        Next

        scenarioRefs = config.SelectNodes("//baseline")
        For Each scenarioRef As XmlNode In scenarioRefs
            If scenarioRef.FirstChild.InnerText = strSenarioName Then
                scenarioRef.FirstChild.InnerText = replacementScenario
            End If
        Next

        scenarioRefs = config.SelectNodes("//string")
        For Each scenarioRef As XmlNode In scenarioRefs
            If scenarioRef.FirstChild.InnerText = strSenarioName And scenarioRef.ParentNode.Name = "scenarios" Then
                scenarioRef.FirstChild.InnerText = replacementScenario
            End If
        Next

        config.Save(My.Settings.ConfigXML)
    End Sub

    Private Sub releaseObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch ex As Exception
            obj = Nothing
        Finally
            GC.Collect()
        End Try
    End Sub

#Region "retrieveData"
    Public Function getInputURL(strsegment As String, strTreatment As String) As String

        Dim xpath As String = "SmartRiverConfig/Settings/ScienceBaseUrls/" & strsegment & getTreatmentAbbrev(strsegment, strTreatment)
        Dim n As XmlNode = config.SelectSingleNode(xpath)

        If n Is Nothing Then
            Return "0"
        Else
            Return n.FirstChild.Value
        End If


    End Function


    Public Function getScenarioNames() As List(Of String)
        Dim return_scenarios As New List(Of String)

        Dim scenarioNodes As XmlNodeList
        scenarioNodes = config.SelectNodes("SmartRiverConfig/Scenarios/Scenario")
        For Each scenarioNode In scenarioNodes
            return_scenarios.Add(scenarioNode.SelectSingleNode("fullName").FirstChild.Value)
        Next
        Return return_scenarios


    End Function

    Public Function getRiverNames() As List(Of String)
        Dim return_sections As New List(Of String)

        Dim sectionsNodes As XmlNodeList
        sectionsNodes = config.SelectNodes("SmartRiverConfig/Rivers/River")
        For Each sectionNode In sectionsNodes
            return_sections.Add(sectionNode.SelectSingleNode("Name").FirstChild.Value)
        Next
        Return return_sections


    End Function

    Public Function getSegmentNames() As List(Of String)
        Dim return_segments As New List(Of String)

        Dim segmentNodes As XmlNodeList
        segmentNodes = config.SelectNodes("SmartRiverConfig/Segments/Segment")
        For Each segmentNode In segmentNodes
            return_segments.Add(segmentNode.SelectSingleNode("SegmentName").FirstChild.Value)
        Next
        Return return_segments


    End Function

    Public Function getSegmentAbbrevs() As List(Of String)
        Dim return_segments As New List(Of String)

        Dim segmentNodes As XmlNodeList
        segmentNodes = config.SelectNodes("SmartRiverConfig/Segments/Segment")
        For Each segmentNode In segmentNodes
            return_segments.Add(segmentNode.SelectSingleNode("SegmentAbbrev").FirstChild.Value)
        Next
        Return return_segments
    End Function

    'Public Function getSegmentLengths(Optional lengthType As String = "reach") As Dictionary(Of String, Double)
    '    Dim return_segments As New Dictionary(Of String, Double)

    '    Dim segmentNodes As XmlNodeList
    '    segmentNodes = config.SelectNodes("SmartRiverConfig/Segments/Segment")
    '    For Each segmentNode In segmentNodes
    '        return_segments.Add(segmentNode.SelectSingleNode("SegmentAbbrev").FirstChild.Value,
    '                            CDbl(segmentNode.SelectSingleNode(lengthType & "Km").FirstChild.Value))
    '    Next
    '    Return return_segments
    'End Function

    Public Function getReachLength(segmentName As String) As String
        Dim xpath As String = "SmartRiverConfig/Segments/Segment[SegmentName='" & segmentName & "']/reachKm"
        Dim n As XmlNode = config.SelectSingleNode(xpath)

        If n Is Nothing Then
            Return "0"
        Else
            Return n.FirstChild.Value
        End If

    End Function
    Public Function getSegmentLength(segmentName As String) As String
        Dim xpath As String
        xpath = "SmartRiverConfig/Segments/Segment[SegmentName='" & segmentName & "']/segmentKm"
        Return config.SelectSingleNode(xpath).FirstChild.Value

    End Function

    Public Function getFlowNormalizer(segmentName As String) As String
        Dim xpath As String
        xpath = "SmartRiverConfig/Segments/Segment[SegmentName='" & segmentName & "']/drainageMi2"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function


    Public Function getTreatmentNames(Optional strSegment As String = "all") As List(Of String)

        Dim return_treatments As New List(Of String)

        Dim segmentNodes As XmlNodeList
        If strSegment = "all" Then
            For Each strSeg As String In parentMainForm.mainDataManager.getSegmentNames()
                segmentNodes = config.SelectNodes("SmartRiverConfig/Segments/Segment[SegmentName='" & strSeg & "']/Treatments/Treatment")
                For Each segmentNode In segmentNodes
                    return_treatments.Add(segmentNode.SelectSingleNode("fullName").FirstChild.Value)
                Next
            Next
        Else
            segmentNodes = config.SelectNodes("SmartRiverConfig/Segments/Segment[SegmentName='" & strSegment & "']/Treatments/Treatment")
            For Each segmentNode In segmentNodes
                return_treatments.Add(segmentNode.SelectSingleNode("fullName").FirstChild.Value)
            Next
        End If

        Return return_treatments.Distinct().ToList()

    End Function

    Public Function getSegmentNamesForRiver(strRiver As String) As List(Of String)
        Dim return_segments As New List(Of String)

        Dim segmentNodes As XmlNodeList
        segmentNodes = config.SelectNodes("SmartRiverConfig/Segments/Segment[River='" & strRiver & "']")
        For Each segmentNode In segmentNodes
            return_segments.Add(segmentNode.SelectSingleNode("SegmentName").FirstChild.Value)
        Next
        Return return_segments


    End Function

    Public Function getSegmentAbrevsForRiver(strRiver As String) As List(Of String)
        Dim return_segments As New List(Of String)

        Dim segmentNodes As XmlNodeList
        segmentNodes = config.SelectNodes("SmartRiverConfig/Segments/Segment[River='" & strRiver & "']")
        For Each segmentNode In segmentNodes
            return_segments.Add(segmentNode.SelectSingleNode("SegmentAbbrev").FirstChild.Value)
        Next
        Return return_segments


    End Function

    Public Function getSpeciesNames(Optional curSegment As String = "All", Optional curTreatment As String = "2005") As List(Of String)
        Dim return_species As New List(Of String)

        Dim speciesNodes As XmlNodeList
        If curSegment = "All" Then
            speciesNodes = config.SelectNodes("SmartRiverConfig/SpeciesOfInterest/Species")
        Else
            speciesNodes = config.SelectNodes("SmartRiverConfig/Segments/Segment[SegmentName='" & curSegment & "']/Treatments/Treatment[fullName='" & curTreatment & "']/SpeciesOfInterest/Species")
        End If

        For Each speciesNode In speciesNodes
            If curSegment = "All" Then
                return_species.Add(speciesNode.SelectSingleNode("Name").FirstChild.Value)
            Else
                return_species.Add(speciesNode.FirstChild.Value)
            End If

        Next
        Return return_species
    End Function

    Public Function getCovariateNames(strSegment, strTreatment) As List(Of String)
        Dim return_covariates As New List(Of String)

        Dim covariateNodes As XmlNodeList
        If strSegment <> "all" Then
            covariateNodes = config.SelectNodes("SmartRiverConfig/Segments/Segment[SegmentName='" & strSegment & "']/Treatments/Treatment[fullName='" & strTreatment & "']/Covariates/Covariate")
        Else
            covariateNodes = config.SelectNodes("SmartRiverConfig/Covariates/Covariate")
        End If
        For Each covariateNode In covariateNodes
            If strSegment <> "all" Then
                return_covariates.Add(covariateNode.FirstChild.Value)
            Else
                return_covariates.Add(covariateNode.SelectSingleNode("Name").FirstChild.Value)
            End If

        Next
        Return return_covariates

    End Function

    Public Function getLifeStageNames(strSpeciesName As String) As List(Of String)
        Dim return_lifeStages As New List(Of String)

        Dim LSNodes As XmlNodeList
        LSNodes = config.SelectNodes("SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpeciesName & "']/Lifestages/Lifestage")
        For Each LSNode In LSNodes
            return_lifeStages.Add(LSNode.SelectSingleNode("Name").FirstChild.Value)
        Next
        Return return_lifeStages

    End Function

    Public Function variableNames(strSpeciesName As String, strLifeStageName As String) As List(Of String)
        Dim return_variableNames As New List(Of String)

        Dim variableNodes As XmlNodeList
        variableNodes = config.SelectNodes("SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpeciesName & "']/Lifestages/Lifestage[Name='" & strLifeStageName & "']/HSC/Variable")
        For Each variableNode In variableNodes
            return_variableNames.Add(variableNode.SelectSingleNode("Name").FirstChild.Value)
        Next
        Return return_variableNames
    End Function

    Public Function variableValue(strSpeciesName As String, strLifeStageName As String,
                                  strVariable As String, strAttribute As String) As String

        Dim xpath As String
        xpath = "SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpeciesName & "']"
        xpath += "/Lifestages/Lifestage[Name='" & strLifeStageName & "']/HSC/Variable[Name='" & strVariable & "']"
        xpath += "/" & strAttribute
        Return config.SelectSingleNode(xpath).FirstChild.Value

    End Function

    Public Function variableUnitsLabel(strVariable As String, strUnits As String) As String

        Dim xpath As String
        If strUnits = "Metric" Then
            xpath = "SmartRiverConfig/Covariates/Covariate[Name='" & strVariable & "']/Units"
        Else
            xpath = "SmartRiverConfig/Covariates/Covariate[Name='" & strVariable & "']/ImperialUnits"
        End If
        Return config.SelectSingleNode(xpath).FirstChild.Value

    End Function

    Public Function getConversionFactor(strConversionName As String) As String
        Dim xpath As String
        xpath = "SmartRiverConfig/Units/" + strConversionName
        Return CDbl(config.SelectSingleNode(xpath).FirstChild.Value)
    End Function

    Public Function variableConversionFactor(strVariable As String) As Double

        Dim xpath As String
        xpath = "SmartRiverConfig/Covariates/Covariate[Name='" & strVariable & "']/UnitConversion"
        Dim strUnitConversion As String = config.SelectSingleNode(xpath).FirstChild.Value

        If strUnitConversion = "None" Then
            Return 1.0
        Else
            Return getConversionFactor(strUnitConversion)
        End If


    End Function

    Public Function isCovariateCategorical(strCovariate As String) As Boolean
        Dim xpath As String
        xpath = "SmartRiverConfig/Covariates/Covariate[Name='" & strCovariate & "']/Categorical"
        Return config.SelectSingleNode(xpath).FirstChild.Value = "True"
    End Function

    Public Function getCategoricalValueLabel(strCovariate As String, strValue As String) As String
        Dim xpath As String = "SmartRiverConfig/Covariates/Covariate[Name='" & strCovariate & "']/ColorScheme/Break"
        Dim BreakNodes As XmlNodeList
        BreakNodes = config.SelectNodes(xpath)

        For Each breaknode As XmlNode In BreakNodes
            If CDbl(breaknode.SelectSingleNode("value").FirstChild.Value) >= CDbl(strValue) Then

                Return breaknode.SelectSingleNode("label").FirstChild.Value
            End If
        Next

        Return "NA"
    End Function

    Public Function getCategoricalValueFromLabel(strCovariate As String, strLabel As String) As String
        Dim xpath As String

        xpath = "SmartRiverConfig/Covariates/Covariate[Name='" & strCovariate & "']/ColorScheme/Break[label='" & strLabel & "']/value"

        Dim n As XmlNode = config.SelectSingleNode(xpath)
        Return n.FirstChild.Value
    End Function

    Public Function variableBreaks(strSpeciesName As String, strLifeStageName As String, strVariable As String) As XmlNodeList

        Dim BreakNodes As XmlNodeList
        BreakNodes = config.SelectNodes("SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpeciesName & "']/Lifestages/Lifestage[Name='" & strLifeStageName & "']/HSC/Variable[Name='" & strVariable & "']/Break")
        Return BreakNodes

    End Function



    Public Function habitatBreaks() As XmlNodeList

        Dim XPath As String = "SmartRiverConfig/SpeciesOfInterest/ColorScheme/Break"
        Dim BreakNodes As XmlNodeList
        BreakNodes = config.SelectNodes(XPath)
        Return BreakNodes

    End Function

    Public Function covariateBreaks(strCovariate As String) As XmlNodeList

        Dim XPath As String
        If curUnits = "metric" Then
            XPath = "SmartRiverConfig/Covariates/Covariate[Name='" & strCovariate & "']/ColorScheme/Break"
        Else
            XPath = "SmartRiverConfig/Covariates/Covariate[Name='" & strCovariate & "']/ColorSchemeImperial/Break"
        End If


        Dim BreakNodes As XmlNodeList
        BreakNodes = config.SelectNodes(XPath)
        Return BreakNodes

    End Function

    Public Function getHSCValues(ByVal strSpeciesname As String, ByVal strLifestageName As String) As Dictionary(Of String, Double(,))
        Dim outputDict As New Dictionary(Of String, Double(,))
        Dim variablesNodes As XmlNodeList
        variablesNodes = config.SelectNodes("SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpeciesname & "']/Lifestages/Lifestage[Name='" & strLifestageName & "']/HSC/Variable")
        Dim variableName As String
        Dim variableHSC(,) As Double

        For Each variableNode As XmlNode In variablesNodes
            variableName = variableNode.SelectSingleNode("Name").FirstChild.Value
            variableHSC = getParameterHSCValues(strSpeciesname, strLifestageName, variableName)
            outputDict.Add(variableName, variableHSC)
        Next

        Return outputDict
    End Function

    Public Function getParameterHSCValues(ByRef strSpeciesName As String, ByRef strLifestageName As String, ByRef strVariable As String) As Double(,)
        Dim HSCNodes As XmlNodeList
        HSCNodes = config.SelectNodes("SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpeciesName & "']/Lifestages/Lifestage[Name='" & strLifestageName & "']/HSC/Variable[Name='" & strVariable & "']/Break")

        Dim outputArray(1, HSCNodes.Count - 1) As Double
        Dim i As Integer = 0
        For Each HSCNode As XmlNode In HSCNodes
            If isCovariateCategorical(strVariable) Then
                outputArray(0, i) = CDbl(HSCNode.SelectSingleNode("value").FirstChild.Value) - 0.0001
                outputArray(1, i) = CDbl(HSCNode.SelectSingleNode("yValue").FirstChild.Value)
            Else
                outputArray(0, i) = CDbl(HSCNode.SelectSingleNode("min").FirstChild.Value)
                outputArray(1, i) = CDbl(HSCNode.SelectSingleNode("yValue").FirstChild.Value)
            End If

            i += 1
        Next

        Return outputArray
    End Function


    Public Function getEquation(strSpecies As String, strLifeStage As String)
        Dim xpath As String = "SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpecies & "']/Lifestages/Lifestage[Name='" & strLifeStage & "']/HSC/Equation/formula"
        Return config.SelectSingleNode(xpath).FirstChild.Value

    End Function
    Public Function setEquation(strSpecies As String, strLifeStage As String, newEquation As String)
        Dim xpath As String = "SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpecies & "']/Lifestages/Lifestage[Name='" & strLifeStage & "']/HSC/Equation/formula"
        Dim node As XmlNode
        node = config.SelectSingleNode(xpath)
        node.FirstChild.Value = newEquation

    End Function

    Public Function getFlows(strSegment As String, strTreatment As String) As List(Of String)


        Dim return_flows As New List(Of String)
        Dim xpath As String = "SmartRiverConfig/Segments/Segment[SegmentName='" & strSegment & "']/Treatments/Treatment[fullName='" & strTreatment & "']/Flows/Flow"

        Dim flowNodes As XmlNodeList
        flowNodes = config.SelectNodes(xpath)
        For Each flowNode In flowNodes
            return_flows.Add(flowNode.SelectSingleNode("cfs").FirstChild.Value)
        Next
        Return return_flows

    End Function

    Public Function getCategoricalColorScheme(xpath As String) As MapWinGIS.GridColorScheme
        Dim variableNodes As XmlNodeList
        Dim variableNode As XmlNode

        variableNodes = config.SelectNodes(xpath + "/Break")
        Dim numBreaks As Integer
        numBreaks = variableNodes.Count

        Dim mwColorScheme As New MapWinGIS.GridColorScheme
        Dim uColor As UInt32
        Dim slabel As String

        Dim maxVal As Double = -9999999
        Dim minVal As Double = 9999999

        For Each variableNode In variableNodes
            Dim m As Double = CDbl(variableNode.SelectSingleNode("value").FirstChild.Value)
            minVal = m - 0.01
            maxVal = m + 0.01

            Dim clr As Color = stringToColor(variableNode.SelectSingleNode("rgb").FirstChild.Value)
            uColor = Convert.ToUInt32(RGB(clr.R, clr.G, clr.B))

            slabel = minVal.ToString("f") & " - " & maxVal.ToString("f")
            'slabel = slabel.PadRight(20) & "HSC = " & y.ToString("f")


            Utilities.insertBreak(maxVal, minVal, uColor, uColor, slabel, mwColorScheme)
        Next

        'mwColorScheme.NoDataColor = Convert.ToUInt32(RGB(255, 0, 0))

        Return mwColorScheme


    End Function

    Public Function getColorScheme(xpath As String) As MapWinGIS.GridColorScheme
        Dim variableNodes As XmlNodeList
        Dim variableNode As XmlNode

        variableNodes = config.SelectNodes(xpath + "/Break")
        Dim numBreaks As Integer
        numBreaks = variableNodes.Count

        Dim mwColorScheme As New MapWinGIS.GridColorScheme
        Dim uColor As UInt32
        Dim slabel As String

        Dim maxVal As Double = -9999999
        Dim minVal As Double = 9999999
        For Each variableNode In variableNodes
            Dim m As Double = CDbl(variableNode.SelectSingleNode("max").FirstChild.Value)
            If variableNode.SelectSingleNode("max").FirstChild.Value > maxVal Then
                maxVal = variableNode.SelectSingleNode("max").FirstChild.Value
            End If
            If variableNode.SelectSingleNode("max").FirstChild.Value < minVal Then
                minVal = variableNode.SelectSingleNode("max").FirstChild.Value
            End If
        Next


        Dim min As Double = minVal
        Dim max As Double

        'slabel = " < min (i.e NoData, dry land, etc)"
        'uColor = Convert.ToUInt32(RGB(0, 0, 0))
        'insertBreak(min - 0.0000001, -999999, uColor, uColor, slabel, colorScheme)


        For Each variableNode In variableNodes
            'Dim x, y As Double
            If min = CDbl(variableNode.SelectSingleNode("max").FirstChild.Value) Then
                min = -999999
            End If
            max = variableNode.SelectSingleNode("max").FirstChild.Value + 0.00000001


            Dim clr As Color = stringToColor(variableNode.SelectSingleNode("rgb").FirstChild.Value)
            uColor = Convert.ToUInt32(RGB(clr.R, clr.G, clr.B))


            'If c.strVariable = "Depth" Then
            '    uColor = Convert.ToUInt32(RGB(Val, Val, 255))

            'ElseIf c.strVariable = "Velocity" Then
            '    uColor = Convert.ToUInt32(RGB(255, Val, Val))
            'Else
            '    uColor = Convert.ToUInt32(RGB(Val, 255, Val))
            'End If

            slabel = min.ToString("f") & " - " & max.ToString("f")
            'slabel = slabel.PadRight(20) & "HSC = " & y.ToString("f")


            If max = maxVal Then max = 9999
            Utilities.insertBreak(max, min, uColor, uColor, slabel, mwColorScheme)
            'y = variableNode.SelectSingleNode("yValue").FirstChild.Value
            min = max - 0.00000001
        Next

        'mwColorScheme.NoDataColor = Convert.ToUInt32(RGB(255, 0, 0))

        Return mwColorScheme


    End Function

    Public Function get_HSC_ColorScheme(curCovariate As String) As MapWinGIS.GridColorScheme
        Dim curSpecies As String = parentMainForm.HabitatGenerator.cboSpecies.Text
        Dim curLifeStage As String = parentMainForm.HabitatGenerator.cboLifestage.Text

        Dim xpath As String
        xpath = "SmartRiverConfig/SpeciesOfInterest/Species[Name='" & curSpecies & "']/Lifestages/Lifestage[Name='" & curLifeStage & "']/HSC/Variable[Name='" & curCovariate & "']"

        If isCovariateCategorical(curCovariate) Then
            Return getCategoricalColorScheme(xpath)
        Else
            Return getColorScheme(xpath)
        End If


    End Function

    Public Function getScenarioAbbrev(strScenario As String) As String
        Dim xpath As String = "SmartRiverConfig/Scenarios/Scenario[fullName='" & strScenario & "']/Abbrev"
        Return config.SelectSingleNode(xpath).FirstChild.Value

    End Function

    Public Function getSegmentAbbrev(strSegment As String) As String
        Dim xpath As String = "SmartRiverConfig/Segments/Segment[SegmentName='" & strSegment & "']/SegmentAbbrev"
        Dim n As XmlNode = config.SelectSingleNode(xpath)

        If n Is Nothing Then
            Return strSegment
        Else
            Return n.FirstChild.Value
        End If


    End Function

    Public Function getTreatmentAbbrev(strSegment As String, strTreatment As String) As String
        Dim xpath As String = "SmartRiverConfig/Segments/Segment[SegmentName='" & strSegment & "']/Treatments/Treatment[fullName='" & strTreatment & "']/TreatAbbrev"
        Dim n As XmlNode = config.SelectSingleNode(xpath)

        If n Is Nothing Then
            Return ""
        Else
            Return n.InnerText
        End If


    End Function

    Public Function getRiverAbbrev(strRiver As String) As String
        Dim xpath As String = "SmartRiverConfig/Rivers/River[Name='" & strRiver & "']/Abbrev"
        Dim n As XmlNode = config.SelectSingleNode(xpath)

        If n Is Nothing Then
            Return strRiver
        Else
            Return n.FirstChild.Value
        End If

    End Function

    Public Function getRiverName(strRiverAbrev As String) As String
        Dim xpath As String = "SmartRiverConfig/Rivers/River[Abbrev='" & strRiverAbrev & "']/Name"
        Return config.SelectSingleNode(xpath).FirstChild.Value

    End Function

    Public Function getSegmentName(strSegmentAbrev As String) As String
        Dim xpath As String = "SmartRiverConfig/Segments/Segment[SegmentAbbrev='" & strSegmentAbrev & "']/SegmentName"
        Return config.SelectSingleNode(xpath).FirstChild.Value

    End Function

    Public Function getCovariateAbbrev(strCovariate As String) As String
        Dim xpath As String = "SmartRiverConfig/Covariates/Covariate[Name='" & strCovariate & "']/fileAbbrev"
        Return config.SelectSingleNode(xpath).FirstChild.Value

    End Function

    Public Function isCovariateSingleFlow(strCovariate As String) As Boolean

        If strCovariate = "NA" Then
            Return False
        End If

        Dim xpath As String = "SmartRiverConfig/Covariates/Covariate[Name='" & strCovariate & "']/SingleFlow"
        Dim ans As String = config.SelectSingleNode(xpath).FirstChild.Value
        Return ans.ToLower = "true"

    End Function

    Public Function getSpeciesAbbrev(strSpeciesName) As String
        Dim xpath As String = "SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpeciesName & "']/fileAbbrev"
        Return config.SelectSingleNode(xpath).FirstChild.Value

    End Function

    Public Function getSpeciesName(strSpeciesAbbrev) As String
        Dim xpath As String = "SmartRiverConfig/SpeciesOfInterest/Species[fileAbbrev='" & strSpeciesAbbrev & "']/Name"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

    Public Function getLifeStageAbbrev(strSpeciesName, strLifeStageName) As String
        Dim xpath As String = "SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpeciesName & "']/Lifestages/Lifestage[Name='" & strLifeStageName & "']/fileAbbrev"

        Dim node As XmlNode
        Try
            node = config.SelectSingleNode(xpath).FirstChild
        Catch ex As Exception
            If Not getSpeciesNames().Contains(strSpeciesName) Then
                strSpeciesName = getSpeciesNames()(0)
            End If
            strLifeStageName = getLifeStageNames(strSpeciesName)(0)
            xpath = "SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpeciesName & "']/Lifestages/Lifestage[Name='" & strLifeStageName & "']/fileAbbrev"
            node = config.SelectSingleNode(xpath).FirstChild
        End Try

        If Not IsNothing(node) Then
            Return node.Value
        Else
            Return ""
        End If


    End Function

    Public Function getLifeStageName(strSpeciesabbrev, strLifeStageabbrev) As String
        Dim xpath As String = "SmartRiverConfig/SpeciesOfInterest/Species[fileAbbrev='" & strSpeciesabbrev & "']/Lifestages/Lifestage[fileAbbrev='" & strLifeStageabbrev & "']/Name"
        Dim node As XmlNode = config.SelectSingleNode(xpath).FirstChild
        If Not IsNothing(node) Then
            Return node.Value
        Else
            Return ""
        End If


    End Function

    Public Function checkSppsLifestageOccurance(strTreatment As String, _
                                        strSegment As String, strSpecies As String, strLifeStage As String) As Boolean

        Dim occurSpecies As New List(Of String)
        occurSpecies = getSpeciesNames(strSegment, strTreatment)
        Return occurSpecies.Contains(strSpecies)

    End Function

#End Region

#Region "FileNames"
    Public Function genCovariateFilename(strSegment As String, strTreatment As String, strCovariate As String, strFlow As String)

        If isCovariateSingleFlow(strCovariate) Then
            Return Path.Combine(My.Settings.InputDataDirectory, "Segments", strCovariate + ".tif")
        Else
            Return Path.Combine(My.Settings.InputDataDirectory, "Segments", strCovariate + "_" + strFlow + ".tif")
        End If



        'Dim strPath As String
        'Dim strTreatmentAbrev As String = getTreatmentAbbrev(strSegment, strTreatment)

        'strPath = Path.Combine(My.Settings.InputDataDirectory, "segments", strSegment) & strTreatmentAbrev
        'Dim strFname As String
        'strFname = getSegmentAbbrev(strSegment) + color"_"
        'If Not isCovariateSingleFlow(strCovariate) Then
        '    strFname += strFlow + "_"
        'End If

        'strFname += getCovariateAbbrev(strCovariate) + ".tif"
        'Return Path.Combine(strPath, strFname)

    End Function

    Public Function genOutputFname(strSegment As String, strTreatment As String, strFlow As String,
                                    strSpecies As String, strLifestage As String)
        Dim strPath As String
        strPath = Path.Combine(My.Settings.OutputDataDirectory, "segments", strSegment)

        Dim strFname As String
        strFname = getSegmentAbbrev(strSegment) + "_"
        strFname += getSpeciesAbbrev(strSpecies) + "_"
        If strLifestage <> "NA" Then
            strFname += getLifeStageAbbrev(strSpecies, strLifestage) + "_"
        End If

        Dim strTreatmentAbbrev As String = getTreatmentAbbrev(strSegment, strTreatment)
        strFname += strFlow + strTreatmentAbbrev + ".tif"
        Return Path.Combine(strPath, strFname)

    End Function

    Public Function genFieldName(strSpecies As String, strLifeStage As String) As String
        Return getSpeciesAbbrev(strSpecies) + "_" + getLifeStageAbbrev(strSpecies, strLifeStage)
    End Function

    Public Function genHabLookupTableName(strSegment As String, strTreatment As String) As String
        Return "SegHab_" + getSegmentAbbrev(strSegment) + getTreatmentAbbrev(strSegment, strTreatment)
    End Function

#End Region

#Region "SystemWideMetrics"
    Public Function getSWMetrics() As List(Of String)
        Dim return_Metrics As New List(Of String)

        Dim MetricNodes As XmlNodeList
        MetricNodes = config.SelectNodes("SmartRiverConfig/SystemWideMetrics/Metric")
        For Each metricNode In MetricNodes
            return_Metrics.Add(metricNode.SelectSingleNode("Name").FirstChild.Value)
        Next
        Return return_Metrics
    End Function

    Public Function getSWMetricParameters(strMetric) As List(Of String)
        Dim return_Parameters As New List(Of String)

        Dim ParameterNodes As XmlNodeList
        ParameterNodes = config.SelectNodes("SmartRiverConfig/SystemWideMetrics/Metric[Name='" & strMetric & "']/Parameter")
        For Each ParameterNode In ParameterNodes
            return_Parameters.Add(ParameterNode.SelectSingleNode("Label").FirstChild.Value)
        Next
        Return return_Parameters
    End Function

    Public Function getSWMetricParamVal(strMetric, strParam) As String
        Dim xpath As String = "SmartRiverConfig/SystemWideMetrics/Metric[Name='" & strMetric & "']/Parameter[Label='" & strParam & "']/CurrentValue"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

    Public Function getSWMetricParamVariableName(strMetric, strParam) As String
        Dim xpath As String = "SmartRiverConfig/SystemWideMetrics/Metric[Name='" & strMetric & "']/Parameter[Label='" & strParam & "']/Variable"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

    Public Function getSWMetricSQL(strMetric) As String
        Dim xpath As String = "SmartRiverConfig/SystemWideMetrics/Metric[Name='" & strMetric & "']/SQL"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

    Public Function getSWMetricOuputSeries(strMetric) As List(Of String)
        Dim return_Series As New List(Of String)

        Dim OutputSeriesNodes As XmlNodeList
        OutputSeriesNodes = config.SelectNodes("SmartRiverConfig/SystemWideMetrics/Metric[Name='" & strMetric & "']/OutputSeries")
        For Each SeriesNode In OutputSeriesNodes
            return_Series.Add(SeriesNode.SelectSingleNode("Name").FirstChild.Value)
        Next
        Return return_Series
    End Function

    Public Function getMetricSeriesField(strMetric, strOutputSeries) As String
        Dim xpath As String = "SmartRiverConfig/SystemWideMetrics/Metric[Name='" & strMetric & "']/OutputSeries[Name='" & strOutputSeries & "']/Field"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function
#End Region

#Region "HabitatMetrics"
    Public Function getHydroperiod(strSpecies As String, strLifeStage As String) As hydroperiod
        Dim hydroP As New hydroperiod

        Dim xpath As String = "SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpecies & "']/Lifestages/Lifestage[Name='" & strLifeStage & "']/hydroperiod"
        hydroP.setFromString(config.SelectSingleNode(xpath).FirstChild.Value)

        Return hydroP
    End Function

    Public Sub setHydroperiod(strSpecies As String, strLifeStage As String, hydroperiod As String)

        Dim xpath As String = "SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpecies & "']/Lifestages/Lifestage[Name='" & strLifeStage & "']/hydroperiod"
        Dim node As XmlNode
        node = config.SelectSingleNode(xpath)
        node.FirstChild.Value = hydroperiod

    End Sub


    Public Function getHabitatMetrics() As List(Of String)
        Dim return_Metrics As New List(Of String)

        Dim MetricNodes As XmlNodeList
        MetricNodes = config.SelectNodes("SmartRiverConfig/HabitatMetrics/Metric")
        For Each metricNode In MetricNodes
            return_Metrics.Add(metricNode.SelectSingleNode("Name").FirstChild.Value)
        Next
        Return return_Metrics
    End Function

    Public Function getOtherMetrics() As List(Of String)
        Dim return_Metrics As New List(Of String)

        Dim MetricNodes As XmlNodeList
        MetricNodes = config.SelectNodes("SmartRiverConfig/OtherMetrics/Metric")
        For Each metricNode In MetricNodes
            return_Metrics.Add(metricNode.SelectSingleNode("Name").FirstChild.Value)
        Next
        Return return_Metrics
    End Function

    Public Function getHabitatMetricSQL(strMetric) As String
        Dim xpath As String = "SmartRiverConfig/HabitatMetrics/Metric[Name='" & strMetric & "']/SQL"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

    Public Function getOtherMetricSQL(strMetric) As String
        Dim xpath As String = "SmartRiverConfig/OtherMetrics/Metric[Name='" & strMetric & "']/SQL"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

    Public Function getHabitatMetricAbrev(strMetric) As String
        Dim xpath As String = "SmartRiverConfig/HabitatMetrics/Metric[Name='" & strMetric & "']/Abrev"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

    Public Function getOtherMetricAbrev(strMetric) As String
        Dim xpath As String = "SmartRiverConfig/OtherMetrics/Metric[Name='" & strMetric & "']/Abrev"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

    Public Function getHydrographMetricSQL(strMetric) As String
        Dim xpath As String = "SmartRiverConfig/HydrographMetrics/Metric[Name='" & strMetric & "']/SQL"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

    Public Function getHabitatMetricGraphType(strMetric) As String

        Dim xpath As String = "SmartRiverConfig/HabitatMetrics/Metric[Name='" & strMetric & "']/GraphType"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function



    Public Function getFlowVsHabitatSQL() As String
        Dim xpath As String = "SmartRiverConfig/FlowVsHabitat/Metric/SQL"
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

#End Region

#Region "Flow Data"
    Public Function getStartDate() As Date
        Dim strSQL As String
        strSQL = "SELECT min(Date) FROM scenario_"
        strSQL += getScenarioAbbrev(getScenarioNames(0))
        strSQL += " WHERE strftime('%m', Date) = '10'"

        Dim SQLcommand As SQLiteCommand


        'SQL query to Create Table
        If mainSQLDBConnection.State = ConnectionState.Closed Then
            mainSQLDBConnection.Open()
        End If

        SQLcommand = mainSQLDBConnection.CreateCommand
        SQLcommand.CommandText = strSQL
        SQLcommand.ExecuteNonQuery()
        Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()

        Dim result As Date
        While SQLreader.Read()
            result = SQLreader(0)
        End While

        mainSQLDBConnection.Close()
        Return result
    End Function

    Public Function getEndDate() As Date
        Dim strSQL As String
        strSQL = "SELECT max(Date) FROM scenario_"
        strSQL += getScenarioAbbrev(getScenarioNames(0))

        Dim SQLcommand As SQLiteCommand


        'SQL query to Create Table
        If mainSQLDBConnection.State = ConnectionState.Closed Then
            mainSQLDBConnection.Open()
        End If
        SQLcommand = mainSQLDBConnection.CreateCommand
        SQLcommand.CommandText = strSQL
        SQLcommand.ExecuteNonQuery()
        Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()

        Dim result As Date
        While SQLreader.Read()
            result = SQLreader(0)
        End While

        mainSQLDBConnection.Close()
        Return result

    End Function

    Public Function getCurStartYear() As Integer
        Dim xpath As String = "SmartRiverConfig/GUIConfigs/Current/GUIConfig/GlobalTimePeriod/StartYear"
        Dim node As XmlNode
        node = config.SelectSingleNode(xpath)
        Return CInt(node.InnerText)
    End Function

    Public Function getCurEndYear() As Integer
        Dim xpath As String = "SmartRiverConfig/GUIConfigs/Current/GUIConfig/GlobalTimePeriod/EndYear"
        Dim node As XmlNode
        node = config.SelectSingleNode(xpath)
        Return CInt(node.InnerText)
    End Function

    Public Function getStoredUnits() As String
        Dim xpath As String = "SmartRiverConfig/GUIConfigs/Current/GUIConfig/curUnits"
        Dim node As XmlNode
        Try
            node = config.SelectSingleNode(xpath)
            Return node.InnerText
        Catch ex As Exception
            Return "Metric"
        End Try

    End Function

#End Region

#Region "calc yearly habitat"
    'yearly habitat can be aggregated from individual sections to a whole river to all the rivers combined.
    'When you do this you go from m2/km of stream to m2 of habitat
    Public Function getBasinWideHabitat(strScenario As String, strTreatment As String, strRiv As String, _
                                    strSegment As String, strSpecies As String, strLifeStage As String, _
                                    strMetric As String, hperiod As hydroperiod, _
                                    Optional strStartYear As String = "NA", Optional strEndYear As String = "NA",
                                    Optional groupByYear As Boolean = True) _
                                As DataTable
        Dim totalBasinResults As New Dictionary(Of Integer, Double)
        For Each strRiver In getRiverNames()

            Dim rivDataTable As DataTable = getYearlyRiverHabitat(strScenario, strTreatment, strRiver, _
                                    strSegment, strSpecies, strLifeStage, _
                                    strMetric, hperiod, _
                                    strStartYear, strEndYear, groupByYear)
            For Each row In rivDataTable.Rows
                If totalBasinResults.ContainsKey(row("WaterYear")) Then
                    totalBasinResults(row("WaterYear")) = totalBasinResults(row("WaterYear")) + row("InterHabitat")
                Else
                    totalBasinResults.Add(row("WaterYear"), row("InterHabitat"))
                End If
            Next

        Next

        Dim outputTable As New DataTable
        outputTable.Columns.Add("WaterYear", GetType(Integer))
        outputTable.Columns.Add("InterHabitat", GetType(Double))
        For Each k In totalBasinResults.Keys
            outputTable.Rows.Add(k, totalBasinResults(k))
        Next

        outputTable.DefaultView.Sort = "InterHabitat ASC"
        Return outputTable

    End Function
    Public Function check_speciesLSProcessed(strSpecies As String, strLifeStage As String,
                                    outputSegment As String, outputTreatment As String) As Boolean
        'See if a particular species/ls has been processed
        Dim checkData As New DataTable

        Dim speciesAbrev As String = getSpeciesAbbrev(strSpecies)
        Dim lsAbrev As String = getLifeStageAbbrev(strSpecies, strLifeStage)
        Dim lowValField As String = speciesAbrev + "_" + lsAbrev + "_lv"
        Dim segmentHabTable As String = genHabLookupTableName(outputSegment, outputTreatment)

        Dim strSQL As String = "select sum(" + lowValField + ") from " + segmentHabTable

        Dim SQLcommand As SQLiteCommand

        'SQL query to Create Table
        If mainSQLDBConnection.State = ConnectionState.Closed Then
            mainSQLDBConnection.Open()
        End If
        SQLcommand = mainSQLDBConnection.CreateCommand
        SQLcommand.CommandText = strSQL
        SQLcommand.ExecuteNonQuery()
        Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()

        Dim processed As Boolean
        While SQLreader.Read()
            Try
                processed = SQLreader(0) > 0
            Catch ex As Exception
                processed = False
            End Try


        End While

        mainSQLDBConnection.Close()



        ''"select * from ProcessingHistory where speciespipelifestage = '" + speciesAbrev + "_" + lsAbrev + "'"


        'Dim sqlcommand As SQLite.SQLiteCommand
        'sqlcommand = mainSQLDBConnection.CreateCommand
        'sqlcommand.CommandText = strSQL
        'Try
        '    Dim sqlda As New SQLite.SQLiteDataAdapter(strSQL, mainSQLDBConnection)
        '    sqlda.Fill(checkData)
        'Catch ex As Exception
        '    'If My.Settings.verbose Then
        '    '    MsgBox(ex.ToString)
        '    'End If
        'End Try

        'Dim processed As Boolean = False
        'If checkData.Rows.Count > 0 Then
        '    Dim r As DataRow = checkData.Rows(0)
        '    If r("status") = "processed" Then
        '        processed = True
        '    End If
        'End If

        If Not processed And _
            processMissingData = "unknown" Then

            Dim ans As VariantType = MsgBox("One or more of the species you're displaying have not been processed. Do you want to process them now?", MsgBoxStyle.YesNo, "Process data")

            If ans = vbYes Then
                processMissingData = "True"
            Else
                processMissingData = "False"
            End If
        End If

        If Not processed And processMissingData = "True" Then
            Dim _speciesToProcess As List(Of String) = New List(Of String) _
                (New String() {strSpecies})
            Dim _lifeStageToProcess As List(Of String) = New List(Of String) _
                 (New String() {strLifeStage})

            Dim progbar As New habGenProgressBar(Me.parentMainForm)

            progbar._lifeStageToProcess = _lifeStageToProcess
            progbar._speciesToProcess = _speciesToProcess
            progbar.singleLS = True

            Me.parentMainForm.Cursor = Cursors.WaitCursor


            progbar.runHabitatMaps()
            progbar.ShowDialog()
            Me.parentMainForm.Cursor = Cursors.Default
            processed = True
        End If

        Return processed
    End Function
    Public Function getYearlyRiverHabitat(strScenario As String, strTreatment As String, strRiv As String, _
                                    strSegment As String, strSpecies As String, strLifeStage As String, _
                                    strMetric As String, hperiod As hydroperiod, _
                                    Optional strStartYear As String = "NA", Optional strEndYear As String = "NA",
                                    Optional groupByYear As Boolean = True) _
                    As DataTable
        Dim indvSegments As New Dictionary(Of Integer, Double)

        Dim totalData As New DataTable

        For Each strSegment In getSegmentNamesForRiver(strRiv)
            Dim segDataTable As DataTable = getYearlySegmentHabitat(strScenario, strTreatment, strRiv, _
                                    strSegment, strSpecies, strLifeStage, _
                                    strMetric, hperiod, _
                                    strStartYear, strEndYear, groupByYear)

            totalData.Merge(segDataTable, True, MissingSchemaAction.Add)
            For Each row In segDataTable.Rows
                If indvSegments.ContainsKey(row("WaterYear")) Then
                    indvSegments(row("WaterYear")) = indvSegments(row("WaterYear")) + (row("InterHabitat"))
                Else
                    indvSegments.Add(row("WaterYear"), row("InterHabitat"))
                End If
            Next

        Next
        Dim outputTable As New DataTable
        outputTable.Columns.Add("WaterYear", GetType(Integer))
        outputTable.Columns.Add("InterHabitat", GetType(Double))
        For Each k In indvSegments.Keys
            outputTable.Rows.Add(k, indvSegments(k))
        Next

        outputTable.DefaultView.Sort = "InterHabitat ASC"
        Return outputTable
    End Function
    Public Function getYearlySegmentHabitat(strScenario As String, strTreatment As String, strRiver As String, _
                                    strSegment As String, strSpecies As String, strLifeStage As String, _
                                    strMetric As String, hperiod As hydroperiod, _
                                    Optional strStartYear As String = "NA", Optional strEndYear As String = "NA",
                                    Optional groupByYear As Boolean = True) _
                                As DataTable
        Dim dTable As New DataTable
        If checkSppsLifestageOccurance(strTreatment, _
                            strSegment, strSpecies, strLifeStage) Then
            If check_speciesLSProcessed(strSpecies, strLifeStage, strSegment, strTreatment) Then



                Dim habLookupTable As String = genHabLookupTableName(strSegment, strTreatment)
                Dim scenarioTable As String = "scenario_" + getScenarioAbbrev(strScenario)
                Dim curScenarioAbbrev As String = getSegmentAbbrev(strSegment)
                Dim lowValField As String = getSpeciesAbbrev(strSpecies) + "_" + getLifeStageAbbrev(strSpecies, strLifeStage) + "_lv"
                Dim highValField As String = getSpeciesAbbrev(strSpecies) + "_" + getLifeStageAbbrev(strSpecies, strLifeStage) + "_hv"
                Dim normValue As String = getReachLength(strSegment)
                Dim reachLength As String = getSegmentLength(strSegment)


                Dim strSQL As String = getHabitatMetricSQL(strMetric)
                Dim SQLCleanupDict As New Dictionary(Of String, String)
                SQLCleanupDict.Add("{scenariotable}", scenarioTable)
                SQLCleanupDict.Add("{lookuptable}", habLookupTable)
                SQLCleanupDict.Add("{scenarioabbrev}", curScenarioAbbrev)
                SQLCleanupDict.Add("{lowvalue}", lowValField)
                SQLCleanupDict.Add("{highvalue}", highValField)
                SQLCleanupDict.Add("{startmonth}", hperiod.startMonth.ToString("D2"))
                SQLCleanupDict.Add("{endmonth}", hperiod.endMonth.ToString("D2"))
                SQLCleanupDict.Add("{startday}", hperiod.startDay.ToString("D2"))
                SQLCleanupDict.Add("{endday}", hperiod.endDay.ToString("D2"))
                SQLCleanupDict.Add("{normalization}", normValue)
                SQLCleanupDict.Add("{reachLength}", reachLength)
                SQLCleanupDict.Add("{startyearlimit}", strStartYear)
                SQLCleanupDict.Add("{endyearlimit}", strEndYear)

                If curUnits = "Metric" Then
                    SQLCleanupDict.Add("{discharge_conversion}", "1.0")
                    SQLCleanupDict.Add("{habitat_conversion}", "1.0")
                Else
                    SQLCleanupDict.Add("{discharge_conversion}", getConversionFactor("MetersToFeet"))
                    SQLCleanupDict.Add("{habitat_conversion}", getConversionFactor("HectaresToAcres"))
                End If

                Dim dateRange As String = ""
                Dim startDateNum As Integer = hperiod.startMonth * 100 + hperiod.startDay
                Dim endDateNum As Integer = hperiod.endMonth * 100 + hperiod.endDay

                If startDateNum < endDateNum Then
                    dateRange = "(dayInt BETWEEN " + Str(startDateNum) + " AND " + Str(endDateNum) + ")"
                Else
                    dateRange = "(dayInt BETWEEN 0 AND " + Str(endDateNum) + " OR dayInt BETWEEN " + Str(startDateNum) + " AND 9999)"
                End If
                SQLCleanupDict.Add("{dateRange}", dateRange)


                strSQL = cleanUpSQL(strSQL, SQLCleanupDict)

                If groupByYear Then
                    strSQL += vbCrLf & "GROUP BY WaterYear"
                End If

                If mainSQLDBConnection.State = ConnectionState.Closed Then
                    mainSQLDBConnection.Open()
                End If
                Dim sqlcommand As SQLite.SQLiteCommand
                sqlcommand = mainSQLDBConnection.CreateCommand
                sqlcommand.CommandText = strSQL


                Dim sqlda As New SQLite.SQLiteDataAdapter(strSQL, mainSQLDBConnection)
                sqlda.Fill(dTable)
                mainSQLDBConnection.Close()
            End If
        End If
        Return dTable

    End Function

    Public Function getYearlyScenarioData(cdd As chartDisplayData) As DataTable
        Dim allData As New DataTable
        Dim totalResults As New Dictionary(Of Integer, List(Of String))

        Dim addedYearCol As Boolean = False


        For Each outputTreatment As String In cdd.treatments
            For Each outputRiver In cdd.rivers
                For Each outputSegment In outputRiver.Value
                    For Each outputSpecies In cdd.species
                        For Each outputLifestage In outputSpecies.Value
                            For Each outputMetric In cdd.displayMetrics
                                For Each outputScenario In cdd.scenarios
                                    Dim seriesName As String = outputScenario & "_" & outputTreatment & "_" & _
                                        getRiverAbbrev(outputRiver.Key) & "_" & getSegmentAbbrev(outputSegment) & "_" & _
                                        getSpeciesAbbrev(outputSpecies.Key) & "_" & getLifeStageAbbrev(outputSpecies.Key, outputLifestage.name) & "_" & _
                                        getHabitatMetricAbrev(outputMetric)
                                    Dim yrlyData As DataTable
                                    If outputRiver.Key = "aggAll" Then
                                        yrlyData = getBasinWideHabitat(outputScenario, outputTreatment, outputRiver.Key, _
                                                                                                  outputSegment, outputSpecies.Key, outputLifestage.name, outputMetric, _
                                                                                                  outputLifestage.hydroPeriod, cdd.startYear, cdd.endYear, cdd.interval = "yearly")
                                    ElseIf outputSegment = "aggRiver" Then
                                        yrlyData = getYearlyRiverHabitat(outputScenario, outputTreatment, outputRiver.Key, _
                                                                                                  outputSegment, outputSpecies.Key, outputLifestage.name, outputMetric, _
                                                                                                  outputLifestage.hydroPeriod, cdd.startYear, cdd.endYear, cdd.interval = "yearly")

                                    Else
                                        yrlyData = getYearlySegmentHabitat(outputScenario, outputTreatment, outputRiver.Key, _
                                                                                                  outputSegment, outputSpecies.Key, outputLifestage.name, outputMetric, _
                                                                                                  outputLifestage.hydroPeriod, cdd.startYear, cdd.endYear, cdd.interval = "yearly")
                                    End If

                                    If Not addedYearCol Then
                                        allData.Columns.Add("WaterYear")
                                        For Each row In yrlyData.Rows
                                            totalResults.Add(row("WaterYear"), New List(Of String))
                                        Next
                                        addedYearCol = True
                                    End If
                                    allData.Columns.Add(seriesName, GetType(System.Decimal))

                                    If yrlyData.Rows.Count > 0 Then
                                        For Each row In yrlyData.Rows
                                            If totalResults.ContainsKey(CInt(row("WaterYear"))) Then
                                                'totalResults.Add(row("WaterYear"), New List(Of String))
                                                totalResults(CInt(row("WaterYear"))).Add(CDbl(row("InterHabitat")))
                                            End If
                                        Next
                                    Else
                                        For Each wy As Integer In totalResults.Keys
                                            totalResults(CInt(wy)).Add(-9999.0)
                                        Next

                                    End If
                                Next
                            Next
                        Next

                    Next
                Next
            Next




        Next

        Dim i As Integer
        For Each k In totalResults.Keys
            i = 1
            Dim r As DataRow = allData.NewRow
            r(0) = k
            For Each item In totalResults(k)
                If item <> "-9999" Then
                    r(i) = item
                End If
                i += 1
            Next
            allData.Rows.Add(r)

        Next

        Return allData
    End Function

    Public Function getSingleData(cdd As chartDisplayData) As DataTable
        Dim allData As New DataTable
        allData.Columns.Add("metric")
        For Each scenario As String In cdd.scenarios
            allData.Columns.Add(scenario, GetType(Double))
        Next

        Dim totalResults As New Dictionary(Of Integer, List(Of String))
        For Each outputTreatment As String In cdd.treatments
            For Each outputRiver In cdd.rivers
                For Each outputSegment In outputRiver.Value
                    For Each outputSpecies In cdd.species
                        For Each outputLifestage In outputSpecies.Value
                            For Each outputMetric In cdd.displayMetrics
                                Try
                                    Dim r As DataRow = allData.NewRow
                                    Dim seriesName As String = outputTreatment & "_" & _
                                            getRiverAbbrev(outputRiver.Key) & "_" & getSegmentAbbrev(outputSegment) & "_" & _
                                            getSpeciesAbbrev(outputSpecies.Key) & "_" & getLifeStageAbbrev(outputSpecies.Key, outputLifestage.name) & "_" & _
                                            getHabitatMetricAbrev(outputMetric)
                                    r(0) = seriesName
                                    Dim i As Integer = 1
                                    For Each outputScenario In cdd.scenarios

                                        Dim yrlyData As DataTable
                                        If outputRiver.Key = "aggAll" Then
                                            yrlyData = getBasinWideHabitat(outputScenario, outputTreatment, outputRiver.Key, _
                                                                                                      outputSegment, outputSpecies.Key, outputLifestage.name, outputMetric, _
                                                                                                      outputLifestage.hydroPeriod, cdd.startYear, cdd.endYear, cdd.interval = "yearly")
                                        ElseIf outputSegment = "aggRiver" Then
                                            yrlyData = getYearlyRiverHabitat(outputScenario, outputTreatment, outputRiver.Key, _
                                                                                                      outputSegment, outputSpecies.Key, outputLifestage.name, outputMetric, _
                                                                                                      outputLifestage.hydroPeriod, cdd.startYear, cdd.endYear, cdd.interval = "yearly")

                                        Else
                                            yrlyData = getYearlySegmentHabitat(outputScenario, outputTreatment, outputRiver.Key, _
                                                                                                      outputSegment, outputSpecies.Key, outputLifestage.name, outputMetric, _
                                                                                                      outputLifestage.hydroPeriod, cdd.startYear, cdd.endYear, cdd.interval = "yearly")
                                        End If
                                        If yrlyData.Rows.Count > 0 Then
                                            r(i) = CDbl(yrlyData.Rows(0)("InterHabitat"))
                                        Else
                                            r(i) = 0
                                        End If
                                        i += 1
                                    Next
                                    allData.Rows.Add(r)
                                Catch ex As Exception

                                End Try
                            Next
                        Next

                    Next
                Next
            Next
        Next

        Return allData
    End Function

    Public Function getDailyData(cdd As chartDisplayData) As DataTable

        Dim selectSQL As String = "SELECT "
        Dim firstScenario As String
        Dim FromSQL As String = "    FROM "


        Dim hydroDatatable As New DataTable
        For Each scenario In cdd.scenarios
            If FromSQL = "    FROM " Then
                firstScenario = "scenario_" + scenario
                FromSQL += firstScenario + vbCrLf
                selectSQL += "scenario_" + scenario + ".Date as DateVal, "
            Else
                FromSQL += "    Inner JOIN scenario_" + scenario + vbCrLf
                FromSQL += "        ON " + firstScenario + ".Date=scenario_" + scenario + ".Date"
            End If
        Next

        Dim colName As String
        If cdd.showHydro Then
            For Each scenario In cdd.scenarios
                For Each outputRiver In cdd.rivers
                    For Each outputSegment In outputRiver.Value
                        colName = getScenarioAbbrev(scenario) + "_" + _
                            getRiverAbbrev(outputRiver.Key) + "_" + getSegmentAbbrev(outputSegment)
                        colName.Replace(" ", "")
                        colName.Replace("aggRiver", "aveSegs")
                        If IsNumeric(colName.Substring(0, 1)) Then
                            colName = "n" + colName
                        End If

                        If outputSegment = "aggRiver" Then
                            Dim seg_abrevs As List(Of String) = getSegmentAbrevsForRiver(outputRiver.Key)
                            Dim SQL_ave As String = "("
                            For Each seg_abrev In seg_abrevs
                                SQL_ave += "scenario_" + scenario + "." + seg_abrev + " + "
                            Next
                            SQL_ave = SQL_ave.Substring(0, Len(SQL_ave) - 2)
                            SQL_ave += ")/" + CStr(seg_abrevs.Count)
                            SQL_ave += " as " + colName
                            selectSQL += SQL_ave + ", "
                        Else
                            selectSQL += "scenario_" + scenario + "." + getSegmentAbbrev(outputSegment)

                            If curUnits <> "Metric" Then
                                selectSQL += " * " + getConversionFactor("MetersToFeet")
                            End If
                            selectSQL += " as " + colName + ", "
                        End If


                    Next
                Next
            Next
        End If

        'For Each scenario In cdd.scenarios
        '    For Each item In cdd.otherMetrics
        '        colName = scenario + "_" + item
        '        selectSQL += "scenario_" + scenario + "." + item + " as " + colName + ", "
        '    Next
        'Next
        selectSQL = selectSQL.Substring(0, Len(selectSQL) - 2)

        Dim strSQL As String
        strSQL = selectSQL + vbCrLf + FromSQL
        strSQL += " WHERE DateVal>=date('" + CStr(cdd.startYear - 1) + "-10-01') and DateVal<date('" + CStr(cdd.endYear) + "-10-01');"
        If mainSQLDBConnection.State = ConnectionState.Closed Then
            mainSQLDBConnection.Open()
        End If
        Dim sqlDA As New SQLite.SQLiteDataAdapter(strSQL, mainSQLDBConnection)
        sqlDA.Fill(hydroDatatable)
        mainSQLDBConnection.Close()

        Dim ds As New DataSet()
        ds.Tables.Add(hydroDatatable)

        Dim obj_PrimaryClmn(1) As System.Data.DataColumn
        obj_PrimaryClmn(0) = hydroDatatable.Columns(0)
        hydroDatatable.PrimaryKey = obj_PrimaryClmn

        'The above SQL call has returned a table with all flow and otherMetics (drought days, etc)
        'to this we must add all of the daily habitat data
        'Due to the complexity of the SQL statements required for this we must do these SQL opperations individually
        'and then append the daily result back to a new column on our working table.

        If cdd.treatments.Count = 0 Then
            cdd.treatments.Add(getTreatmentNames()(0))
        End If

        For Each outputScenario In cdd.scenarios
            For Each outputTreatment As String In cdd.treatments
                For Each outputRiver In cdd.rivers
                    For Each outputSegment In outputRiver.Value
                        For Each outputSpecies In cdd.species
                            For Each outputLifestage In outputSpecies.Value
                                If checkSppsLifestageOccurance(outputTreatment, _
                    outputSegment, outputSpecies.Key, outputLifestage.name) Then


                                    If check_speciesLSProcessed(outputSpecies.Key, outputLifestage.name, outputSegment, outputTreatment) Then
                                        Dim habColName As String = outputScenario & "_" & outputTreatment & "_" & _
                                                                     getRiverAbbrev(outputRiver.Key) & "_" & getSegmentAbbrev(outputSegment) & "_" & _
                                                                     getSpeciesAbbrev(outputSpecies.Key) & "_" & getLifeStageAbbrev(outputSpecies.Key, outputLifestage.name)
                                        Dim habTable As New DataTable
                                        habTable = getDailyHabitatData(outputScenario, outputTreatment, outputRiver.Key, outputSegment,
                                                                        outputSpecies.Key, outputLifestage, cdd.startYear, cdd.endYear)
                                        addTableCol(ds, hydroDatatable, habTable, "InterHabitat", habColName)
                                    End If
                                End If
                            Next
                        Next '
                    Next
                Next
            Next
        Next

        Return hydroDatatable
    End Function

    Public Function getDailyHabitatData(Scenario As String, Treatment As String, River As String,
                                        Segment As String, Species As String, objLifeStage As lifestage,
                                        startYear As Integer, endYear As Integer) As DataTable
        Dim dailyHabTable As New DataTable

        If Segment = "aggRiver" Then
            Dim tmpHabTable As DataTable
            Dim firstSeg As Boolean = True
            For Each seg In getSegmentNamesForRiver(River)
                tmpHabTable = getDailyHabitatDataSegment(Scenario, Treatment, River, _
                                        seg, Species, objLifeStage, startYear, endYear)
                If firstSeg Then
                    dailyHabTable = tmpHabTable.Copy()
                    firstSeg = False
                Else

                End If

            Next

        Else
            dailyHabTable = getDailyHabitatDataSegment(Scenario, Treatment, River, _
                                        Segment, Species, objLifeStage, startYear, endYear)

        End If

        Return dailyHabTable
    End Function

    Public Function getDailyHabitatDataSegment(Scenario As String, Treatment As String, River As String,
                                        Segment As String, Species As String, objLifeStage As lifestage,
                                        startYear As Integer, endYear As Integer) As DataTable

        If startYear > endYear Then
            startYear = parentMainForm.mainDataManager.curStartYear
            endYear = parentMainForm.mainDataManager.curEndYear
        End If

        Dim dailyHabTable As New DataTable
        If checkSppsLifestageOccurance(Treatment, _
                            Segment, Species, objLifeStage.name) Then
            If check_speciesLSProcessed(Species, objLifeStage.name, Segment, Treatment) Then



                Dim habLookupTable As String = genHabLookupTableName(Segment, Treatment)
                Dim scenarioTable As String = "scenario_" + getScenarioAbbrev(Scenario)
                Dim curScenarioAbbrev As String = getSegmentAbbrev(Segment)
                Dim lowValField As String = getSpeciesAbbrev(Species) + "_" + getLifeStageAbbrev(Species, objLifeStage.name) + "_lv"
                Dim highValField As String = getSpeciesAbbrev(Species) + "_" + getLifeStageAbbrev(Species, objLifeStage.name) + "_hv"
                Dim normValue As String = getReachLength(Segment)
                Dim repReachLength As String = getSegmentLength(Segment)

                Dim hperiod As hydroperiod = objLifeStage.hydroPeriod
                Dim strSQL As String = getHydrographMetricSQL("Daily Habitat Series")
                Dim SQLCleanupDict As New Dictionary(Of String, String)
                SQLCleanupDict.Add("{scenariotable}", scenarioTable)
                SQLCleanupDict.Add("{lookuptable}", habLookupTable)
                SQLCleanupDict.Add("{scenarioabbrev}", curScenarioAbbrev)
                SQLCleanupDict.Add("{lowvalue}", lowValField)
                SQLCleanupDict.Add("{highvalue}", highValField)
                SQLCleanupDict.Add("{startmonth}", hperiod.startMonth.ToString("D2"))
                SQLCleanupDict.Add("{endmonth}", hperiod.endMonth.ToString("D2"))
                SQLCleanupDict.Add("{startday}", hperiod.startDay.ToString("D2"))
                SQLCleanupDict.Add("{endday}", hperiod.endDay.ToString("D2"))
                SQLCleanupDict.Add("{normalization}", normValue)
                SQLCleanupDict.Add("{startyearlimit}", startYear)
                SQLCleanupDict.Add("{endyearlimit}", endYear)
                SQLCleanupDict.Add("{reachLength}", repReachLength)

                If curUnits = "Metric" Then
                    SQLCleanupDict.Add("{discharge_conversion}", "1.0")
                    SQLCleanupDict.Add("{habitat_conversion}", "1.0")
                Else
                    SQLCleanupDict.Add("{discharge_conversion}", getConversionFactor("MetersToFeet"))
                    SQLCleanupDict.Add("{habitat_conversion}", getConversionFactor("HectaresToAcres"))
                End If

                Dim dateRange As String = ""
                Dim startDateNum As Integer = hperiod.startMonth * 100 + hperiod.startDay
                Dim endDateNum As Integer = hperiod.endMonth * 100 + hperiod.endDay

                If startDateNum < endDateNum Then
                    dateRange = "(dayInt BETWEEN " + Str(startDateNum) + " AND " + Str(endDateNum) + ")"
                Else
                    dateRange = "(dayInt BETWEEN 0 AND " + Str(endDateNum) + " OR dayInt BETWEEN " + Str(startDateNum) + " AND 9999)"
                End If
                SQLCleanupDict.Add("{dateRange}", dateRange)

                strSQL = cleanUpSQL(strSQL, SQLCleanupDict)

                Dim sqlcommand As SQLite.SQLiteCommand
                sqlcommand = mainSQLDBConnection.CreateCommand
                sqlcommand.CommandText = strSQL
                Try
                    Dim sqlda As New SQLite.SQLiteDataAdapter(strSQL, mainSQLDBConnection)
                    sqlda.Fill(dailyHabTable)
                Catch ex As Exception
                    If My.Settings.verbose Then
                        MsgBox(ex.ToString)
                    End If
                End Try

            End If
        End If

        Return dailyHabTable
    End Function
#End Region

    Public Function getFlowVsHabitat(River As String, Segment As String, Treatment As String, Species As String, _LifeStage As String)
        Dim flowVsHabTable As New DataTable
        Dim habLookupTable As String = genHabLookupTableName(Segment, Treatment)
        Dim highValField As String = getSpeciesAbbrev(Species) + "_" + getLifeStageAbbrev(Species, _LifeStage) + "_hv"
        Dim normValue As String = getReachLength(Segment)
        Dim flowNormalizer As String = getFlowNormalizer(Segment)


        Dim SQLCleanupDict As New Dictionary(Of String, String)
        SQLCleanupDict.Add("{lookuptable}", habLookupTable)
        SQLCleanupDict.Add("{highvalue}", highValField)
        SQLCleanupDict.Add("{norm}", normValue)
        SQLCleanupDict.Add("{flownormalizer}", flowNormalizer)


        If curUnits = "Metric" Then
            SQLCleanupDict.Add("{discharge_conversion}", "1.0")
            SQLCleanupDict.Add("{habitat_conversion}", "1.0")
        Else
            SQLCleanupDict.Add("{discharge_conversion}", getConversionFactor("MetersToFeet"))
            SQLCleanupDict.Add("{habitat_conversion}", getConversionFactor("HectaresToAcres"))
        End If

        Dim strSQL As String = getFlowVsHabitatSQL()
        strSQL = cleanUpSQL(strSQL, SQLCleanupDict)

        Dim sqlcommand As SQLite.SQLiteCommand
        sqlcommand = mainSQLDBConnection.CreateCommand
        sqlcommand.CommandText = strSQL
        Try
            Dim sqlda As New SQLite.SQLiteDataAdapter(strSQL, mainSQLDBConnection)
            sqlda.Fill(flowVsHabTable)
        Catch ex As Exception
            If My.Settings.verbose Then
                MsgBox(ex.ToString)
            End If
        End Try



        Return flowVsHabTable
    End Function

    Public Function areOutputsProcessed() As Boolean
        'Outputs folder exists but is it populated?
        Dim segmentsDir As String = Path.Combine(My.Settings.OutputDataDirectory, "Segments")
        Dim outputsDone As Boolean = False
        If Not System.IO.Directory.Exists(segmentsDir) Then
            outputsDone = False
        Else
            Dim folder As New DirectoryInfo(segmentsDir)
            If folder.Exists And folder.GetFileSystemInfos().Count > 0 Then
                outputsDone = True
            End If
        End If
        Return outputsDone
    End Function

#Region "SpeciesLocation"
    Public Function getSurveySpecies() As List(Of String)


        Dim strSQL As String = "SELECT DISTINCT Species FROM bird_survey_data"

        Dim results As New List(Of String)
        'SQL query to Create Table
        If mainSQLDBConnection.State = ConnectionState.Closed Then
            mainSQLDBConnection.Open()
        End If

        Dim SurveyDatatable As New DataTable
        Dim sqlDA As New SQLite.SQLiteDataAdapter(strSQL, mainSQLDBConnection)
        sqlDA.Fill(SurveyDatatable)
        mainSQLDBConnection.Close()

        Dim ds As New DataSet()
        ds.Tables.Add(SurveyDatatable)

        Dim obj_PrimaryClmn(1) As System.Data.DataColumn
        obj_PrimaryClmn(0) = SurveyDatatable.Columns(0)
        SurveyDatatable.PrimaryKey = obj_PrimaryClmn

        'The above SQL call has returned a table with a list of our survey species

        For Each row In SurveyDatatable.Rows
            results.Add(row(0))
        Next

        Return results


    End Function

    Public Function getSurveyYears()
        Dim strSQL As String = "SELECT DISTINCT Year FROM bird_survey_data"

        Dim results As New List(Of String)
        'SQL query to Create Table
        If mainSQLDBConnection.State = ConnectionState.Closed Then
            mainSQLDBConnection.Open()
        End If

        Dim SurveyDatatable As New DataTable
        Dim sqlDA As New SQLite.SQLiteDataAdapter(strSQL, mainSQLDBConnection)
        sqlDA.Fill(SurveyDatatable)
        mainSQLDBConnection.Close()

        Dim ds As New DataSet()
        ds.Tables.Add(SurveyDatatable)

        Dim obj_PrimaryClmn(1) As System.Data.DataColumn
        obj_PrimaryClmn(0) = SurveyDatatable.Columns(0)
        SurveyDatatable.PrimaryKey = obj_PrimaryClmn

        'The above SQL call has returned a table with a list of our survey species

        For Each row In SurveyDatatable.Rows
            results.Add(row(0))
        Next

        Return results
    End Function

    Public Function getSurveySpeciesData(Optional strSpecies As String = "None", Optional strYear As String = "All", Optional Absence As Boolean = False) As DataTable
        Dim strSQL As String = "SELECT x_utm, y_utm, display_dist from bird_survey_data where"
        If strYear <> "All" Then
            strSQL += " year=" + CStr(strYear) + " AND"
        End If

        If Absence Then
            strSQL += " Species <> '" + strSpecies + "'"
        Else
            strSQL += " Species = '" + strSpecies + "'"
        End If
        strSQL = strSQL.Replace("'", Chr(34))

        Dim results As New List(Of String)
        'SQL query to Create Table
        If mainSQLDBConnection.State = ConnectionState.Closed Then
            mainSQLDBConnection.Open()
        End If

        Dim SurveyDatatable As New DataTable
        Dim sqlDA As New SQLite.SQLiteDataAdapter(strSQL, mainSQLDBConnection)
        sqlDA.Fill(SurveyDatatable)
        mainSQLDBConnection.Close()

        Return SurveyDatatable

    End Function
#End Region

#Region "symbology"
    Public Sub symbolizeSeries(seriesName As String, newSeries As Series)
        If seriesSymbology.ContainsKey(seriesName) Then
            If newSeries.ChartType = SeriesChartType.Bar Or newSeries.ChartType = SeriesChartType.Column Then
                newSeries("PointWidth") = seriesSymbology(seriesName).curWidth
            ElseIf newSeries.ChartType = SeriesChartType.FastLine Or newSeries.ChartType = SeriesChartType.Line Then
                newSeries.BorderWidth = seriesSymbology(seriesName).curWidth
                newSeries.BorderDashStyle = DirectCast([Enum].Parse(GetType(ChartDashStyle), seriesSymbology(seriesName).curStyle), ChartDashStyle)
            End If
            newSeries.Color = seriesSymbology(seriesName).curColor
            newSeries.LegendText = seriesSymbology(seriesName).curLegendText
        Else
            Dim nextColor As Color = defaultSeriesColors.Dequeue
            Dim curSymbol As seriesSymbol
            If newSeries.ChartType = SeriesChartType.Bar Or newSeries.ChartType = SeriesChartType.Column Then
                curSymbol = New seriesSymbol(0.8, nextColor, "Default")
            ElseIf newSeries.ChartType = SeriesChartType.FastLine Or newSeries.ChartType = SeriesChartType.Line Then
                curSymbol = New seriesSymbol(1, nextColor, "Solid")
            Else
                curSymbol = New seriesSymbol(1, nextColor, "Solid")
            End If

            newSeries.Color = nextColor

            'newSeries.Label = seriesName
            seriesSymbology.Add(seriesName, curSymbol)
            'Stick this color back on the end of the queue for reuse
            defaultSeriesColors.Enqueue(nextColor)
        End If


    End Sub

    Public Sub loadCustomSeriesColors()
        'page 21 of the cartographers toolkit
        For Each c In {Color.FromArgb(117, 186, 202),
                       Color.FromArgb(218, 122, 146),
                       Color.FromArgb(227, 182, 46),
                       Color.FromArgb(148, 216, 45),
                       Color.FromArgb(116, 109, 163),
                       Color.FromArgb(182, 240, 252),
                       Color.FromArgb(134, 151, 71),
                       Color.FromArgb(158, 123, 88),
                       Color.FromArgb(28, 131, 234),
                       Color.FromArgb(223, 1, 74),
                       Color.FromArgb(255, 139, 63),
                       Color.FromArgb(254, 227, 246),
                        Color.FromArgb(255, 139, 63),
                        Color.FromArgb(254, 90, 60),
                       Color.DarkSlateGray}
            defaultSeriesColors.Enqueue(c)
        Next

        'Convert colors to integers
        Dim colorBlue As Integer
        Dim colorGreen As Integer
        Dim colorRed As Integer
        Dim iMyCustomColor As Integer
        Dim iMyCustomColors(defaultSeriesColors.Count - 1) As Integer

        For index = 0 To defaultSeriesColors.Count - 1
            'cast to integer
            colorBlue = defaultSeriesColors(index).B
            colorGreen = defaultSeriesColors(index).G
            colorRed = defaultSeriesColors(index).R

            'shift the bits
            iMyCustomColor = colorBlue << 16 Or colorGreen << 8 Or colorRed

            iMyCustomColors(index) = iMyCustomColor
        Next

        parentMainForm.globalColorDialog.CustomColors = iMyCustomColors

    End Sub

    Public Sub storeCurrentSeriesSymbology()

        Dim GUIConfig As XmlNode
        GUIConfig = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/SeriesSymbology")
        If Not IsNothing(GUIConfig) Then
            GUIConfig.ParentNode.RemoveChild(GUIConfig)
        End If

        'Store the current dictionary of seriesSymbology
        Dim chartSymNode As XmlNode
        chartSymNode = config.CreateElement("SeriesSymbology")
        For Each seriesName As String In seriesSymbology.Keys
            Dim seriesSymNode As XmlNode = config.CreateElement("Series")
            Dim seriesNameNode As XmlNode = config.CreateElement("Name")
            seriesNameNode.InnerText = seriesName
            seriesSymNode.AppendChild(seriesNameNode)
            Dim seriesColorNode As XmlNode = config.CreateElement("Color")
            Dim curColor As Color = seriesSymbology(seriesName).curColor
            seriesColorNode.InnerText = colorToString(curColor)
            seriesSymNode.AppendChild(seriesColorNode)
            Dim seriesWidthNode As XmlNode = config.CreateElement("Width")
            seriesWidthNode.InnerText = seriesSymbology(seriesName).curWidth
            seriesSymNode.AppendChild(seriesWidthNode)
            Dim seriesStyleNode As XmlNode = config.CreateElement("LineStyle")
            seriesStyleNode.InnerText = seriesSymbology(seriesName).curStyle
            seriesSymNode.AppendChild(seriesStyleNode)
            Dim seriesLabelTextNode As XmlNode = config.CreateElement("LegendText")
            seriesLabelTextNode.InnerText = seriesSymbology(seriesName).curLegendText
            seriesSymNode.AppendChild(seriesLabelTextNode)
            chartSymNode.AppendChild(seriesSymNode)
        Next
        config.SelectSingleNode("SmartRiverConfig/GUIConfigs").AppendChild(chartSymNode)
    End Sub

    Public Sub storeCurrentDGVNames()
        Dim GUIConfig As XmlNode
        GUIConfig = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/DGVNames")
        If Not IsNothing(GUIConfig) Then
            GUIConfig.ParentNode.RemoveChild(GUIConfig)
        End If

        'Store the current dictionary of seriesSymbology
        Dim DGVNamesNode As XmlNode
        DGVNamesNode = config.CreateElement("DGVNames")

        For Each DGVName As String In dgvNames.Keys
            Dim dgvNameNode As XmlNode = config.CreateElement("DGVName")
            Dim KeyNode As XmlNode = config.CreateElement("OriginalName")
            Dim ValNode As XmlNode = config.CreateElement("NewName")
            KeyNode.InnerText = DGVName
            ValNode.InnerText = dgvNames(DGVName)

            dgvNameNode.AppendChild(KeyNode)
            dgvNameNode.AppendChild(ValNode)
            DGVNamesNode.AppendChild(dgvNameNode)
        Next

        config.SelectSingleNode("SmartRiverConfig/GUIConfigs").AppendChild(DGVNamesNode)
    End Sub

#End Region

#Region "Settings"
    Public Function getSetting(setting As String) As String
        Dim xpath As String = "SmartRiverConfig/Settings/" & setting
        Return config.SelectSingleNode(xpath).FirstChild.Value
    End Function

    Public Sub setSetting(setting As String, newValue As String)
        Try
            Dim xpath As String = "SmartRiverConfig/Settings/" & setting
            Dim node As XmlNode
            node = config.SelectSingleNode(xpath)
            node.FirstChild.Value = newValue
            config.Save(My.Settings.ConfigXML)
        Catch
        End Try

    End Sub
#End Region

#Region "DBandConfigUpdaters"
    Public Sub updateDBTo1(strNewDBName)
        Dim updateSQLDBConnection As New SQLite.SQLiteConnection()
        Dim connectionstring As String = "Data Source=" & strNewDBName & ";"
        updateSQLDBConnection.ConnectionString = connectionstring

        If mainSQLDBConnection.State = ConnectionState.Closed Then
            mainSQLDBConnection.Open()
        End If

        Dim sb As New StringBuilder("ATTACH DATABASE '")
        sb.Append(strNewDBName)
        sb.Append("' AS updateDB;")
        Dim c As New SQLiteCommand(sb.ToString, mainSQLDBConnection)
        c.ExecuteNonQuery()
        c.Dispose()

        Dim c2 As New SQLiteCommand("create table main.db_version as select * from updateDB.db_version;", mainSQLDBConnection)
        c2.ExecuteNonQuery()
        c2.Dispose()

        Dim c3 As New SQLiteCommand("create table main.MarshVolLookup as select * from updateDB.MarshVolLookup;", mainSQLDBConnection)
        c3.ExecuteNonQuery()
        c3.Dispose()



    End Sub


    Public Sub updateConfigTo1(strNewConfigFname)
        Dim NewConfig As New XmlDocument
        NewConfig.Load(strNewConfigFname)

        'Dim FlowNodes As XmlNodeList
        'FlowNodes = config.SelectNodes("SmartRiverConfig/Segments/Segment/Treatments/Treatment/Flows/Flow")
        'For Each FlowNode As XmlNode In FlowNodes
        '    Dim flow As String = FlowNode.SelectSingleNode("cfs").InnerText()
        '    Dim newFlowNode As XmlNode = NewConfig.SelectSingleNode("SmartRiverConfig/Segments/Segment/Treatments/Treatment/Flows/Flow[cfs='" & flow & "']")

        '    Dim newAFnode As XmlNode = config.CreateElement("af")
        '    newAFnode.InnerText = newFlowNode.SelectSingleNode("af").InnerText()
        '    FlowNode.AppendChild(newAFnode)

        'Next

        'Grab existing nodes to use to specify where to insert the new content
        Dim rootNode As XmlNode = config.SelectSingleNode("SmartRiverConfig")
        Dim RiversNode As XmlNode = config.SelectSingleNode("SmartRiverConfig/Rivers")
        Dim HabMetricsNode As XmlNode = config.SelectSingleNode("SmartRiverConfig/HabitatMetrics")

        'Create ConfigVersion node
        Dim newVersionNode As XmlNode = config.CreateElement("ConfigVersion")
        newVersionNode.InnerText = "1"
        rootNode.InsertBefore(newVersionNode, RiversNode)

        'Copy OtherMetricsNode
        Dim newOtherMetricsNode As XmlNode = config.CreateElement("OtherMetrics")
        Dim importOtherMetricsNode As XmlNode = NewConfig.SelectSingleNode("SmartRiverConfig/OtherMetrics")
        newOtherMetricsNode.InnerXml = importOtherMetricsNode.InnerXml
        rootNode.InsertAfter(newOtherMetricsNode, HabMetricsNode)

        'Add in the new water storage view
        Dim importViewNode As XmlNode = NewConfig.SelectSingleNode("SmartRiverConfig/GUIConfigs/BuiltIn/GUIConfig[Name='Water Storage']")
        Dim newViewNode As XmlNode = config.CreateElement("GUIConfig")
        newViewNode.InnerXml = importViewNode.InnerXml
        Dim BuiltinNode As XmlNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/BuiltIn")
        BuiltinNode.AppendChild(newViewNode)

        'Update the series symbology
        Dim updatedSeriesNodes As XmlNodeList
        updatedSeriesNodes = NewConfig.SelectNodes("SmartRiverConfig/GUIConfigs/UpdateSeriesSymbology/Series")
        Dim oldSeriesRootNode As XmlNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/SeriesSymbology")
        For Each updatedSeriesNode As XmlNode In updatedSeriesNodes
            Dim seriesName As String = updatedSeriesNode.SelectSingleNode("Name").InnerText
            Try
                Dim oldSeriesNode As XmlNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/SeriesSymbology/Series[Name='" + seriesName + "']")
                oldSeriesRootNode.RemoveChild(oldSeriesNode)
            Catch ex As Exception

            End Try

            Dim newSeriesNode As XmlNode = config.CreateElement("Series")
            newSeriesNode.InnerXml = updatedSeriesNode.InnerXml
            oldSeriesRootNode.AppendChild(newSeriesNode)
        Next

        Dim curInnerXML = config.InnerXml
        curInnerXML = curInnerXML.Replace("<Name>molting</Name>", "<Name>breedingForaging</Name>")
        curInnerXML = curInnerXML.Replace("<name>molting</name>", "<name>breedingForaging</name>")
        config.InnerXml = curInnerXML

        config.Save(My.Settings.ConfigXML)


    End Sub

    Public Sub updateConfigTo2(strNewConfigFname)
        Dim NewConfig As New XmlDocument
        NewConfig.Load(strNewConfigFname)

        'Grab existing nodes to use to specify where to insert the new content
        Dim rootNode As XmlNode = config.SelectSingleNode("SmartRiverConfig")
        Dim RiversNode As XmlNode = config.SelectSingleNode("SmartRiverConfig/Rivers")
        Dim HabMetricsNode As XmlNode = config.SelectSingleNode("SmartRiverConfig/HabitatMetrics")

        'Create ConfigVersion node
        Dim newVersionNode As XmlNode = config.SelectSingleNode("SmartRiverConfig/ConfigVersion")
        newVersionNode.InnerText = "2"

        'Replace the corrupt raw data default.
        Dim newRawData As XmlNode = config.SelectSingleNode("SmartRiverConfig/GUIConfigs/DefaultComponents/RawDataForm")
        newRawData.InnerXml = "<chartDisplayData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><showHydro>true</showHydro><scenarios><string>2011_low</string><string>Average</string><string>MaxHabitat</string><string>2014_high</string></scenarios><rivers><item><key><string>Topock</string></key><value><ArrayOfString><string>Topock_1</string></ArrayOfString></value></item></rivers><treatments><string>hist</string></treatments><species><item><key><string>Yuma clapper rail</string></key><value><ArrayOfLifestage><lifestage><name>breedingForaging</name><hydroPeriod><startMonth>3</startMonth><startDay>15</startDay><endMonth>7</endMonth><endDay>30</endDay></hydroPeriod></lifestage><lifestage><name>breedingNesting</name><hydroPeriod><startMonth>3</startMonth><startDay>15</startDay><endMonth>7</endMonth><endDay>30</endDay></hydroPeriod></lifestage></ArrayOfLifestage></value></item><item><key><string>Southwestern willow flycatcher</string></key><value><ArrayOfLifestage><lifestage><name>breeding</name><hydroPeriod><startMonth>5</startMonth><startDay>1</startDay><endMonth>8</endMonth><endDay>31</endDay></hydroPeriod></lifestage></ArrayOfLifestage></value></item></species><otherMetrics><string>TotalWaterRequired</string></otherMetrics><displayMetrics><string>Average of bottom 25% of habitat</string></displayMetrics><startYear>2006</startYear><endYear>2099</endYear><interval>single</interval><baseline>2011_low</baseline></chartDisplayData>"

        Dim curInnerXML = config.InnerXml
        curInnerXML = curInnerXML.Replace("<Name>molting</Name>", "<Name>breedingForaging</Name>")
        curInnerXML = curInnerXML.Replace("<string>Rev7</string>", "")
        curInnerXML = curInnerXML.Replace("<string>FFMP</string>", "")
        curInnerXML = curInnerXML.Replace("<treatments><string>2005</string></treatments>", "<treatments><string>hist</string></treatments>")
        config.InnerXml = curInnerXML

        config.Save(My.Settings.ConfigXML)


    End Sub

    Public Sub DownloadTier1()

        'Download the new Tier1 Data
        Dim rootDownloadFolder As String = My.Settings.InputDataDirectory + "\" + "tempScienceBaseDownloads"
        If Not IO.Directory.Exists(rootDownloadFolder) Then
            IO.Directory.CreateDirectory(rootDownloadFolder)
        End If

        Dim url As String = "https://www.sciencebase.gov/catalog/file/get/561e6cd0e4b0cdb063e59d38?name=Tier1.zip"

        Dim WC As WebClient = New WebClient()
        WC.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)")
        WC.DownloadFile(New Uri(url), rootDownloadFolder + "\tier1.zip")

        'Unzip these into our inputs folder

        Dim args(3) As Object
        args(0) = rootDownloadFolder + "\tier1.zip"
        args(1) = My.Settings.InputDataDirectory
        'args(1) = tmpOutFolder
        args(2) = My.Settings.InputDataDirectory

        UnzipFile(args)

    End Sub

    Private Sub UnzipFile(ByVal args As Object())
        Dim extractCancelled As Boolean = False
        Dim zipToRead As String = args(0)
        Dim extractDir As String = args(1)
        Try
            Using zip As ZipFile = ZipFile.Read(zipToRead)

                zip.ExtractAll(extractDir, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
            End Using
        Catch ex1 As Exception
            MessageBox.Show(String.Format("There's been a problem extracting that zip file.  {0}", ex1.Message), "Error Extracting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End Try

    End Sub
#End Region

End Class

Public Class seriesSymbol
    Public curColor As Color
    Public curWidth As Single
    Public curStyle As String
    Public curMarker As String
    Public curLegendText As String

    Public Sub New(w As Single, c As Color,
                   Optional s As String = "Solid",
                   Optional m As String = "None",
                   Optional label As String = "")
        curColor = c
        curWidth = w
        curStyle = s
        curMarker = m
        curLegendText = label
    End Sub


End Class
