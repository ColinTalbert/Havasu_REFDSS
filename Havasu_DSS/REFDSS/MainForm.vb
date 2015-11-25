Imports WeifenLuo.WinFormsUI.Docking
Imports System.Threading
Imports System.Xml
Imports System.IO
Imports System.Net

Imports Excel = Microsoft.Office.Interop.Excel
Imports Microsoft.Office
Imports System.Runtime.InteropServices

'The MainForm is the application shell itself.  It contains:
'   some toolbars
'   code to manage serializing and deserializing the gui components
'   code to handle adding gui components
'   code to handle the main application initialization.
'Additionally it contains the global pointers to the 'manager' objects which contain list of specific components and the logic for them to interact.
'The 'datamanager' is unique in that it doesn't have a gui component but instead contains logic for getting and setting info from the xml config file and interacting with the SQLite DB.

Public Class MainForm
    Public mainDockPanel As WeifenLuo.WinFormsUI.Docking.DockPanel
    Public mainMapManager As MapManager
    Public mainDataManager As DataManager
    Public mainGraphManager As GraphManager
    Public mainHydrographManager As HydrographManager

    Public HabitatGenerator As HabitatSuitibilityGeneratorForm
    Public chartFormater As ChartDisplayControl

    Public startDatePicker As New DateTimePicker
    Public endDatePicker As New DateTimePicker

    Public persistenceLength As Integer
    Public globalColorDialog As New ColorDialog


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub


    Private Sub MainForm_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Try
            InitializeContent()
        Catch ex As Exception
            MsgBox("We ran into a problem loading the previous content:" & vbCrLf & " error message:" & vbCrLf & ex.Message)
        End Try

        If My.Application.SplashScreen IsNot Nothing Then
            Dim splashScreenDispose As New My.MyApplication.DisposeDelegate(AddressOf My.Application.SplashScreen.Dispose)
            My.Application.SplashScreen.Invoke(splashScreenDispose)
            My.Application.SplashScreen = Nothing
            Me.Activate()
        End If

        'If Not mainDataManager.validConfig = "True" Then
        Me.Show()
        mainDataManager.haveShownChangedDataWarning = False
        'End If
    End Sub

    Private Sub InitializeContent()
        ' 


        resetDockPanel()

        mainDataManager = New DataManager(Me)
        If Me.mainDataManager.validConfig = "True" Then
            initializeFigures()
            chartFormater = New ChartDisplayControl
            'chartFormater.setGraphManager(mainGraphManager)
            'chartFormater.setHydrographManager(mainHydrographManager)

            initializeDatePickers()
            initializeUnits()
            restoreLayout()


            initializeViews()

            initializeScenarios()

            initializeTables()


            mainHydrographManager.updateCursor("select")
            'mainMapManager.ZoomFull()
        Else
            Me.Show()
            Me.TopMost = True
            MsgBox(mainDataManager.validConfig, MsgBoxStyle.SystemModal, "Missing inputs")
            Me.TopMost = False
        End If
    End Sub

    Private Sub resetDockPanel()
        ToolStripContainer1.ContentPanel.Controls.Clear()

        mainDockPanel = Nothing
        mainDockPanel = New WeifenLuo.WinFormsUI.Docking.DockPanel

        mainDockPanel.ActiveAutoHideContent = Nothing
        mainDockPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        mainDockPanel.Dock = System.Windows.Forms.DockStyle.Fill
        mainDockPanel.DockBackColor = System.Drawing.SystemColors.Control
        mainDockPanel.DockLeftPortion = 0.35R
        mainDockPanel.DockRightPortion = 0.65R
        mainDockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow
        mainDockPanel.Location = New System.Drawing.Point(0, 0)
        mainDockPanel.Name = "MainDockPanel"
        mainDockPanel.Size = New System.Drawing.Size(1362, 702)

        ToolStripContainer1.ContentPanel.Controls.Add(mainDockPanel)
    End Sub

    Private Sub restoreLayout()
        If mainDataManager.layoutStored Then
            mainMapManager = New MapManager(Me)
            mainHydrographManager = New HydrographManager(Me)

            mainGraphManager = New GraphManager(mainDockPanel, mainDataManager, Me)
            mainGraphManager.mainHydrographManager = mainHydrographManager

            mainDataManager.ClearRawDataForms()

            Dim strTmpOutFile As String = System.IO.Path.GetTempFileName()
            mainDataManager.saveLayoutXML(strTmpOutFile)

            Try
                mainDockPanel.LoadFromXml(strTmpOutFile, AddressOf ReloadContent)
                System.IO.File.Delete(strTmpOutFile)
                mainDataManager.loadPreviousGUISettings()
            Catch

            End Try

            

            mainHydrographManager.updateCursor("select")
        End If
    End Sub

    Private Sub MapToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles MapToolStripMenuItem.Click
        mainMapManager.addSegmentMapForm()
    End Sub

    Private Sub MapLegendToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MapLegendToolStripMenuItem.Click
        mainMapManager.addSegmentMapLegendForm()
    End Sub

    Private Sub GraphToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles GraphToolStripMenuItem.Click
        mainHydrographManager.addHydrograph()

    End Sub

    Private Sub RawDataToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles RawDataToolStripMenuItem.Click
        mainDataManager.addRawDataForm()
    End Sub

    Private Sub AddScenarioOASISreformaterxlsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles AddScenarioOASISreformaterxlsToolStripMenuItem.Click
        ' First create a FolderBrowserDialog object
        Dim FileBrowserDialog1 As New OpenFileDialog

        ' Then use the following code to create the Dialog window
        ' Change the .SelectedPath property to the default location
        With FileBrowserDialog1
            ' Desktop is the root folder in the dialog.
            If System.IO.File.Exists(My.Settings.ConfigXML) Then
                .InitialDirectory = System.IO.Path.GetDirectoryName(My.Settings.ConfigXML)
            ElseIf System.IO.File.Exists(My.Settings.InputDataDirectory) Then
                .InitialDirectory = My.Settings.InputDataDirectory
            Else
                .InitialDirectory = Environment.SpecialFolder.MyComputer
            End If

            ' Prompt the user with a custom message.
            .Title = "Select an input file"
            .CheckFileExists = True
            If .ShowDialog = DialogResult.OK Then
                'Get the user to enter a name for the scenario
                Dim strScenarioName As String = InputBox("Enter a name for this scenario")

                If strScenarioName = "" Then
                    MsgBox("Scenario name entered was blank", MsgBoxStyle.Critical, "Cannot add this as a scenario!")
                ElseIf mainDataManager.getScenarioNames().Contains(strScenarioName) Then
                    MsgBox("Scenario name entered already in DSS, please choose a different name", MsgBoxStyle.Critical, "Cannot add this as a scenario!")
                Else

                    Me.Cursor = Cursors.WaitCursor
                    mainDataManager.importScenario(.FileName, strScenarioName)
                    Me.Cursor = Cursors.Default

                    addScenario(strScenarioName)
                End If
            End If
        End With



    End Sub




    Private Sub OverviewMapToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs)
        mainMapManager.addOverviewForm(True)

    End Sub

    Private Sub HabitatSuitibilityGeneratorToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles HabitatSuitibilityGeneratorToolStripMenuItem.Click
        HabitatGenerator = HabitatSuitibilityGeneratorForm.Instance(Me)
        HabitatGenerator.Show(mainDockPanel, DockState.DockLeft)

    End Sub



    Private Sub SystemWideMetricsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SystemWideMetricsToolStripMenuItem.Click
        mainGraphManager.addSystemWideGraphForm()
    End Sub
    Private Sub ResultsGraphToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ResultsGraphToolStripMenuItem.Click
        mainGraphManager.addHabitatGraphForm()
    End Sub
    Private Sub AggregatedHabitatResultsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles AggregatedHabitatResultsToolStripMenuItem.Click
        mainGraphManager.addAggHabitatGraphForm()
    End Sub
    Private Sub FloToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles FloToolStripMenuItem.Click
        mainGraphManager.addflowVsHabitatGraphForm()
    End Sub

#Region "saveAndReloadContent"

    Private Sub MainForm_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'Currently the application autosaves the config on closing.
        'Not sure if this is best
        mainDataManager.saveconfig()

    End Sub

    Private Function ReloadContent(ByVal persistString As String) As IDockContent
        'This function is passed to the Weifen Luo Dock manager so that appropriate
        'Widgets can be generated to recreate a previous configuration.
        If persistString = "frmConcept" Then
            Return Nothing
        ElseIf persistString.EndsWith(".SegmentMapForm") Then
            Return mainMapManager.addSegmentMapForm(False)
        ElseIf persistString.EndsWith(".SegmentMapLegendForm") Then
            Return mainMapManager.addSegmentMapLegendForm(False)
        ElseIf persistString.EndsWith(".HydrographGraphForm") Then
            Return mainHydrographManager.addHydrograph(False)
        ElseIf persistString.EndsWith(".OverviewMapForm") Then
            Return mainMapManager.addOverviewForm(False)
        ElseIf persistString.EndsWith(".HabitatSuitibilityGeneratorForm") Then
            HabitatGenerator = HabitatSuitibilityGeneratorForm.Instance(Me)
            Return HabitatGenerator
        ElseIf persistString.EndsWith(".SystemWideMetricForm") Then
            Return mainGraphManager.addSystemWideGraphForm(False)
        ElseIf persistString.EndsWith(".HabitatGraphForm") Then
            Return mainGraphManager.addHabitatGraphForm(False)
        ElseIf persistString.EndsWith(".aggHabitatGraphForm") Then
            Return mainGraphManager.addAggHabitatGraphForm(False)
        ElseIf persistString.EndsWith(".RawDataForm") Then
            Return mainDataManager.addRawDataForm(False)
        ElseIf persistString.EndsWith(".FlowVsHabitatChartForm") Then
            Return mainGraphManager.addflowVsHabitatGraphForm(False)
        ElseIf persistString.EndsWith(".FigureForm") Then
            Return mainDataManager.addFigureForm(show:=False)
        End If


    End Function




#End Region


#Region "Map Segment toolbar"



    Private Sub btnPrevExtent_Click(sender As System.Object, e As System.EventArgs) Handles btnPrevExtent.Click
        mainMapManager.ZoomToPrev()
    End Sub

    Private Sub tsbZoomIn_Click(sender As System.Object, e As System.EventArgs) Handles tsbPan.Click, tsbZoomIn.Click, tsbZoomOut.Click, tspIdentify.Click, tsbMapSelect.Click

        If sender.Name = "tsbZoomIn" Then
            mainMapManager.ChangeCursor(MapWinGIS.tkCursorMode.cmZoomIn, False)
        ElseIf sender.Name = "tsbZoomOut" Then
            mainMapManager.ChangeCursor(MapWinGIS.tkCursorMode.cmZoomOut, False)
        ElseIf sender.Name = "tsbPan" Then
            mainMapManager.ChangeCursor(MapWinGIS.tkCursorMode.cmPan, False)
        ElseIf sender.Name = "tspIdentify" Then
            mainMapManager.ChangeCursor(MapWinGIS.tkCursorMode.cmNone, True)
        ElseIf sender.name = "tsbMapSelect" Then
            mainMapManager.ChangeCursor(MapWinGIS.tkCursorMode.cmSelection, False)
        End If
        tsbZoomIn.Checked = False
        tsbZoomOut.Checked = False
        tsbPan.Checked = False
        tspIdentify.Checked = False
        tsbMapSelect.Checked = False
        sender.checked = True
    End Sub

    Private Sub tsbZoomFull_Click(sender As System.Object, e As System.EventArgs) Handles tsbZoomFull.Click
        mainMapManager.ZoomFull()
    End Sub

    Private Sub txbTitle_Click(sender As System.Object, e As System.EventArgs) Handles txbTitle.Click
        mainMapManager.ChangeLableVisible(txbTitle.Checked)
    End Sub

    Private Sub tspAddSegmentMap_Click(sender As System.Object, e As System.EventArgs)
        mainMapManager.addSegmentMapForm()
    End Sub

#End Region
#Region "hydrograph toolbar"
    Private Sub tsbAddHydrograph_Click(sender As System.Object, e As System.EventArgs)
        mainHydrographManager.addHydrograph()
    End Sub
#End Region

    Private Sub tsbZoomCursor_Click(sender As System.Object, e As System.EventArgs) Handles tsbZoomCursor.Click
        mainHydrographManager.updateCursor("zoom")
        tsbSelectFlowCursor.Checked = Not tsbZoomCursor.Checked
    End Sub

    Private Sub tsbSelect_Click(sender As System.Object, e As System.EventArgs) Handles tsbSelectFlowCursor.Click
        mainHydrographManager.updateCursor("select")
        tsbZoomCursor.Checked = Not tsbSelectFlowCursor.Checked
    End Sub


    Private Sub tsbZoomFullHydro_Click(sender As System.Object, e As System.EventArgs) Handles tsbZoomFullHydro.Click
        mainHydrographManager.zoomfull()
    End Sub


#Region "Chart toolbar"
    Private Sub formatChart_Click(sender As System.Object, e As System.EventArgs)
        chartFormater.Show()
    End Sub
#End Region

#Region "Animation"
    Dim playing As Boolean = False
    Dim pause As Boolean = False
    Dim waitseconds As Double = 100
    Private Sub tsbPlay_Click(sender As System.Object, e As System.EventArgs) Handles btnPlay.Click

        If pause = False And playing = False Then
            playing = True
            btnPlay.Image = My.Resources.media_playback_pause_2
            Try
                While True
                    mainHydrographManager.incrementCursors()
                    Thread.Sleep(waitseconds)
                    Application.DoEvents()
                    If pause = True Then
                        pause = False
                        playing = False
                        btnPlay.Image = My.Resources.media_playback_start_2
                        Exit While
                    End If
                End While

                ''While chartTime.ChartAreas("Default").CursorX.Position <= chartTime.ChartAreas("Default").AxisX.Maximum
                ''    waitseconds = tbPlaySpeed.Maximum - tbPlaySpeed.Value
                ''    chartTime.ChartAreas("Default").CursorX.Position += 1
                ''    CursorUpdate(chartTime.ChartAreas("Default").CursorX.Position)
                ''    Thread.Sleep(waitseconds)
                ''    Application.DoEvents()
                'If pause = True Then
                '    pause = False
                '    playing = False
                '    btnPlay.Image = My.Resources.media_playback_start_2
                '    Exit While
                'End If
                'End While
                'chartTime.ChartAreas("Default").CursorX.Position = chartTime.ChartAreas("Default").AxisX.Minimum + 1
                'CursorUpdate(chartTime.ChartAreas("Default").CursorX.Position)
            Catch ex As Exception
                pause = False
                playing = False
                btnPlay.Image = My.Resources.media_playback_start_2
                Exit Sub
            End Try

            pause = False
            playing = False
            btnPlay.Image = My.Resources.media_playback_start_2
            'chartTime.ChartAreas("Default").CursorX.Position = chartTime.ChartAreas("Default").AxisX.Minimum
            'CursorUpdate(chartTime.ChartAreas("Default").CursorX.Position)
        Else
            pause = True

        End If
    End Sub

    Private Sub cboPlaySpeed_TextChanged(sender As Object, e As System.EventArgs) Handles cboPlaySpeed.TextChanged
        Select Case cboPlaySpeed.Text
            Case "very slow"
                waitseconds = 400
            Case "slow"
                waitseconds = 300
            Case "normal"
                waitseconds = 200
            Case "fast"
                waitseconds = 100
            Case "very fast"
                waitseconds = 0
        End Select
    End Sub

#End Region





    Private Sub ToolStripLabel5_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripLabel5.Click
        tsAnalysisDateRange.Items.Add(New ToolStripControlHost(New DateTimePicker))

    End Sub

    Private Sub addView(strViewName As String)
        'create a menu item for this view for the main select list
        Dim menuitem As New ToolStripMenuItem
        menuitem.Name = strViewName
        menuitem.Text = strViewName
        menuitem.Tag = "SmartRiverConfig/GUIConfigs/BuiltIn/GUIConfig[Name='" & strViewName & "']"
        menuitem.CheckOnClick = False
        AddHandler menuitem.Click, AddressOf ViewClick
        tsmViews.DropDownItems.Insert(tsmViews.DropDownItems.Count - 3, menuitem)

        'also create another menu item for the delete view list
        Dim menuitem2 As New ToolStripMenuItem
        menuitem2.Name = strViewName
        menuitem2.Text = strViewName
        tsmRemoveView.DropDownItems.Add(menuitem2)
    End Sub

    Private Sub initializeViews()
        tsmRemoveView.DropDownItems.Clear()
        For index As Integer = 0 To tsmViews.DropDownItems.Count - 4
            tsmViews.DropDownItems.RemoveAt(0)
        Next

        For Each n As XmlNode In mainDataManager.config.SelectNodes("SmartRiverConfig/GUIConfigs/BuiltIn/GUIConfig")
            addView(n.SelectSingleNode("Name").FirstChild.Value)
        Next
    End Sub
    Private Sub initializeTables()
        Dim Tier1TablesDname As String = Path.Combine(My.Settings.InputDataDirectory, "Tier1", "Tables")

        TablesTSM.DropDownItems.Clear()

        Dim TablesInfoReader As New Microsoft.VisualBasic.
                      FileIO.TextFieldParser(
                        Path.Combine(Tier1TablesDname, "TableInfo.csv"))
        TablesInfoReader.TextFieldType = FileIO.FieldType.Delimited
        TablesInfoReader.SetDelimiters(",")

        Dim headerRow As String() = TablesInfoReader.ReadFields()
        Dim currentRow As String()
        While Not TablesInfoReader.EndOfData
            Try
                currentRow = TablesInfoReader.ReadFields()
                addTable(currentRow(0), currentRow(1), currentRow(2))

            Catch ex As Microsoft.VisualBasic.
                        FileIO.MalformedLineException
                MsgBox("Line " & ex.Message &
                "is not valid and will be skipped.")
            End Try
        End While

    End Sub

    Private Sub addTable(strTableFName As String, strSheetName As String, strTableCaption As String)
        'create a menu item for this view for the main select list
        Dim menuitem As New ToolStripMenuItem
        menuitem.Name = strSheetName.Trim()
        menuitem.Text = strTableCaption
        menuitem.Tag = strTableFName + "||||" + strSheetName
        menuitem.CheckOnClick = False
        AddHandler menuitem.Click, AddressOf TableClick
        TablesTSM.DropDownItems.Add(menuitem)
    End Sub

    Private Sub addFigure(strFigureName As String, strFigureCaption As String)
        'create a menu item for this view for the main select list
        Dim menuitem As New ToolStripMenuItem
        menuitem.Name = strFigureName
        menuitem.Text = strFigureName + ":   " + strFigureCaption
        menuitem.Tag = strFigureName
        menuitem.CheckOnClick = False
        AddHandler menuitem.Click, AddressOf FigureClick
        FiguresTSM.DropDownItems.Add(menuitem)
    End Sub
    Private Sub initializeFigures()
        Dim Tier1FigureDname As String = Path.Combine(My.Settings.InputDataDirectory, "Tier1", "Figures")

        FiguresTSM.DropDownItems.Clear()


        Dim FigureInfoReader As New Microsoft.VisualBasic.
                      FileIO.TextFieldParser(
                        Path.Combine(Tier1FigureDname, "FigureInfo.csv"))
        FigureInfoReader.TextFieldType = FileIO.FieldType.Delimited
        FigureInfoReader.SetDelimiters(",")

        Dim headerRow As String() = FigureInfoReader.ReadFields()
        Dim currentRow As String()
        While Not FigureInfoReader.EndOfData
            Try
                currentRow = FigureInfoReader.ReadFields()
                addFigure(Path.GetFileNameWithoutExtension(currentRow(0)), currentRow(1))

            Catch ex As Microsoft.VisualBasic.
                        FileIO.MalformedLineException
                MsgBox("Line " & ex.Message &
                "is not valid and will be skipped.")
            End Try
        End While

    End Sub

    Private Sub addScenario(strScenario As String)
        'also create another menu item for the delete view list
        Dim menuitem2 As New ToolStripMenuItem
        menuitem2.Name = strScenario
        menuitem2.Text = strScenario
        menuitem2.CheckOnClick = False
        tsmRemoveScenarios.DropDownItems.Add(menuitem2)
    End Sub

    Private Sub initializeScenarios()

        For Each n As XmlNode In mainDataManager.config.SelectNodes("SmartRiverConfig/Scenarios/Scenario")
            addScenario(n.SelectSingleNode("fullName").FirstChild.Value)
        Next
    End Sub

    Private Sub ViewClick(sender As System.Object, e As System.EventArgs)

        Me.Cursor = Cursors.WaitCursor

        Dim newView As XmlNode = mainDataManager.config.SelectSingleNode(sender.tag)

        Dim GUIConfig As XmlNode
        GUIConfig = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig")
        If Not IsNothing(GUIConfig) Then
            GUIConfig.ParentNode.RemoveChild(GUIConfig)
        End If

        mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current").AppendChild(newView.Clone)

        mainDataManager.storeCurrentSeriesSymbology()
        mainDataManager.storeCurrentDGVNames()

        mainDataManager.config.Save(My.Settings.ConfigXML)
        'InitializeContent()
        resetDockPanel()
        restoreLayout()

        For Each mmu In tsmViews.DropDownItems
            If TypeOf (mmu) Is ToolStripMenuItem Then
                mmu.Checked = False
            End If
        Next
        sender.checked = True
        Me.Cursor = Cursors.Default

    End Sub

    Private Sub SaveCurrentView_Click(sender As System.Object, e As System.EventArgs) Handles SaveCurrentView.Click
        'Get a name for this new view
        Dim name As String = InputBox("Enter a name for this view:", "View name")
        If Not name = "" Then 'if name = "" they either clicked cancel or forgot to enter a name
            'Save the view to our config xml
            mainDataManager.saveconfig()

            Dim xpath As String = "SmartRiverConfig/GUIConfigs/BuiltIn/GUIConfig[Name='" & name & "']"
            Dim newView As XmlNode = mainDataManager.config.SelectSingleNode(xpath)
            If Not IsNothing(newView) Then
                newView.ParentNode.RemoveChild(newView)
            End If
            mainDataManager.config.Save(My.Settings.ConfigXML)
            Dim CurrentView As XmlNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig")

            mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/BuiltIn").AppendChild(CurrentView.Clone)
            newView = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/BuiltIn").ChildNodes.Item(mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/BuiltIn").ChildNodes.Count - 1)
            Dim viewNameNode As XmlNode = mainDataManager.config.CreateElement("Name")
            viewNameNode.InnerText = name
            newView.InsertBefore(viewNameNode, newView.ChildNodes.Item(0))
            mainDataManager.config.Save(My.Settings.ConfigXML)

            'add this new view to our views dropdown list
            addView(name)
        End If
    End Sub

    Private Sub tsmRemoveView_Click(sender As System.Object, ByVal e As ToolStripItemClickedEventArgs) Handles tsmRemoveView.DropDownItemClicked

        Dim toRemove As String = e.ClickedItem.Text
        tsmViews.DropDownItems.RemoveByKey(toRemove)
        tsmRemoveView.DropDownItems.RemoveByKey(toRemove)

        Dim toRemoveNode As XmlNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/BuiltIn/GUIConfig[Name='" & toRemove & "']")

        toRemoveNode.ParentNode.RemoveChild(toRemoveNode)
        mainDataManager.config.Save(My.Settings.ConfigXML)
    End Sub

    Private Sub tsmRemoveScenario_Click(sender As System.Object, ByVal e As ToolStripItemClickedEventArgs) Handles tsmRemoveScenarios.DropDownItemClicked

        Dim toRemove As String = e.ClickedItem.Text

        If MsgBox("Are you sure you want to delete the scenario titled: " + toRemove, MsgBoxStyle.YesNoCancel, "Confirm delete scenario") = MsgBoxResult.Yes Then
            tsmRemoveScenarios.DropDownItems.RemoveByKey(toRemove)

            Dim strScenarioAbbrev As String = mainDataManager.getScenarioAbbrev(toRemove)
            mainDataManager.removeScenario(strScenarioAbbrev, False)
        End If
    End Sub

    Private Sub initializeDatePickers()

        txtPeriodStart.Items.Clear()
        txtPeriodEnd.Items.Clear()

        mainDataManager.curStartYear = mainDataManager.getCurStartYear()
        mainDataManager.curEndYear = mainDataManager.getCurEndYear()

        txtPeriodStart.Text = mainDataManager.curStartYear - 1
        txtPeriodEnd.Text = mainDataManager.curEndYear

        For i As Integer = CInt(mainDataManager.getStartDate.Year()) To CInt(mainDataManager.getEndDate.Year())
            txtPeriodStart.Items.Add(CStr(i))
            txtPeriodEnd.Items.Add(CStr(i))
        Next

    End Sub

    Private Sub initializeUnits()
        mainDataManager.curUnits = mainDataManager.getStoredUnits()
        MetricToolStripMenuItem.Checked = mainDataManager.curUnits = "Metric"
        ImperialToolStripMenuItem.Checked = mainDataManager.curUnits = "Imperial"
    End Sub

    Private Sub validateStartDate()
        Dim badDate As Boolean = False
        Try
            Dim startYear As Integer = CInt(txtPeriodStart.Text) + 1
            Dim endYear As Integer = CInt(txtPeriodEnd.Text)
            If startYear > endYear Then badDate = True
            If startYear < mainDataManager.getStartDate.Year - 1 Then badDate = True
        Catch ex As Exception
            badDate = True
        End Try

        If badDate Then
            MsgBox("The date entered is either: not in YYYY format,  not in our period of record, or greater than/equal to  the specified end date", MsgBoxStyle.Exclamation)
            txtPeriodStart.Text = txtPeriodStart.Items(0)
        Else
            mainDataManager.curStartYear = CInt(txtPeriodStart.Text) + 1
        End If
    End Sub
    Private Sub validateEndDate()
        Dim badDate As Boolean = False
        Try
            Dim startYear As Integer = CInt(txtPeriodStart.Text) + 1
            Dim endYear As Integer = CInt(txtPeriodEnd.Text)
            If startYear > endYear Then badDate = True
            If endYear > mainDataManager.getEndDate.Year Then badDate = True
        Catch ex As Exception
            badDate = True
        End Try

        If badDate Then
            MsgBox("The date entered is either: not in YYYY format,  not in our period of record, or less than/equal to  the specified begining date", MsgBoxStyle.Exclamation)
            txtPeriodEnd.Text = txtPeriodEnd.Items(txtPeriodEnd.Items.Count - 1)
        Else
            mainDataManager.curEndYear = CInt(txtPeriodEnd.Text)
        End If
    End Sub
    Private Sub updateTimePeriod()
        mainHydrographManager.updateLoadData()
        mainGraphManager.updateLoadData()
        mainDataManager.updateLoadData()
    End Sub

    Private Sub txtPeriodStart_TextUpdate(sender As Object, e As System.EventArgs) Handles txtPeriodStart.Validated

        validateStartDate()
        updateTimePeriod()

    End Sub

    Private Sub txtPeriodEnd_TextUpdate(sender As Object, e As System.EventArgs) Handles txtPeriodEnd.Validated
        validateEndDate()
        updateTimePeriod()
    End Sub

    Private Sub txtPeriodEnd_indexChanged(sender As Object, e As System.EventArgs) Handles txtPeriodEnd.SelectedIndexChanged
        validateEndDate()
        updateTimePeriod()
    End Sub

    Private Sub txtPeriodstart_indexChanged(sender As Object, e As System.EventArgs) Handles txtPeriodStart.SelectedIndexChanged
        validateStartDate()
        updateTimePeriod()
    End Sub



    Private Sub ChangeInputDataDirectoryToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ChangeInputDataDirectoryToolStripMenuItem.Click
        Dim folderBrowserDialog1 As New FolderBrowserDialog
        folderBrowserDialog1.Description = "Select the directory with SmartRiverGIS data inputs"
        folderBrowserDialog1.SelectedPath = Path.GetDirectoryName(My.Settings.ConfigXML)
        If folderBrowserDialog1.ShowDialog = DialogResult.OK Then
            Try
                My.Settings.InputDataDirectory = folderBrowserDialog1.SelectedPath
                mainDataManager.setSetting("InputDataDirectory", folderBrowserDialog1.SelectedPath)
                mainDataManager.config.Save(My.Settings.ConfigXML)
                InitializeContent()
            Catch ex As Exception
                'they don't have a sessionDirectory or a confib.xml yet.  We'll save these latter.
            End Try
            
        Else
            Exit Sub
        End If
        My.Settings.Save()
    End Sub

    Private Sub ChangeSessionDirectory_Click(sender As System.Object, e As System.EventArgs) Handles ChangeSessionDirectory.Click
        Dim folderBrowserDialog1 As New FolderBrowserDialog
        folderBrowserDialog1.Description = "Select an existing REFDSS Session Directory"
        folderBrowserDialog1.SelectedPath = My.Settings.SessionDirectory
        If folderBrowserDialog1.ShowDialog = DialogResult.OK Then
            Dim isValidSessionDir As String = isSessionDirectory(folderBrowserDialog1.SelectedPath)

            If isValidSessionDir = "True" Then
                mainDataManager.saveconfig()
                My.Settings.SessionDirectory = folderBrowserDialog1.SelectedPath
                InitializeContent()
                My.Settings.Save()
            Else
                Dim msg As String
                msg = "The selected directory doesn't appear to be a valid REFDSSS directory"
                msg += vbCrLf + "Specifically it: " + isValidSessionDir
                MsgBox(msg, MsgBoxStyle.Exclamation, "Invalid Session Directory Selected")
                Exit Sub
            End If

        End If

    End Sub

    Private Sub SaveCurrentSession_Click(sender As System.Object, e As System.EventArgs) Handles SaveCurrentSession.Click
        Dim folderBrowserDialog1 As New FolderBrowserDialog
        folderBrowserDialog1.Description = "Select a new (empty) directory"
        folderBrowserDialog1.SelectedPath = My.Settings.SessionDirectory
        If folderBrowserDialog1.ShowDialog = DialogResult.OK Then
            If Directory.GetFiles(folderBrowserDialog1.SelectedPath).Count() > 0 Or _
                Directory.GetDirectories(folderBrowserDialog1.SelectedPath).Count() > 0 Then
                Dim msg As String = "The selected directory is not empty" + vbCrLf
                msg += "Please select an empty directory to create a copy of the current session directory in"
                Exit Sub
            End If

            mainDataManager.saveconfig()

            FileCopy(My.Settings.ConfigXML, Path.Combine(folderBrowserDialog1.SelectedPath, "config.xml"))
            FileCopy(My.Settings.SQliteDB, Path.Combine(folderBrowserDialog1.SelectedPath, "REFDSS_data.sqlite"))
            MkDir(Path.Combine(folderBrowserDialog1.SelectedPath, "Outputs"))

            My.Settings.SessionDirectory = folderBrowserDialog1.SelectedPath
            InitializeContent()

            My.Settings.Save()

        End If
    End Sub

    Private Sub CreateNewDefaultSession_Click(sender As System.Object, e As System.EventArgs) Handles CreateNewDefaultSession.Click
        Dim folderBrowserDialog1 As New FolderBrowserDialog
        folderBrowserDialog1.Description = "Select a new (empty) directory"
        folderBrowserDialog1.SelectedPath = My.Settings.SessionDirectory
        If folderBrowserDialog1.ShowDialog = DialogResult.OK Then
            If Directory.GetFiles(folderBrowserDialog1.SelectedPath).Count() > 0 Or _
                Directory.GetDirectories(folderBrowserDialog1.SelectedPath).Count() > 0 Then
                Dim msg As String = "The selected directory is not empty" + vbCrLf
                msg += "Please select an empty directory to create a copy of the current session directory in"
                Exit Sub
            End If

            mainDataManager.saveconfig()

            FileCopy(Path.Combine(My.Settings.InputDataDirectory, "config.xml"), Path.Combine(folderBrowserDialog1.SelectedPath, "config.xml"))
            FileCopy(Path.Combine(My.Settings.InputDataDirectory, "REFDSS_data.sqlite"), Path.Combine(folderBrowserDialog1.SelectedPath, "REFDSS_data.sqlite"))
            MkDir(Path.Combine(folderBrowserDialog1.SelectedPath, "Outputs"))

            My.Settings.SessionDirectory = folderBrowserDialog1.SelectedPath
            InitializeContent()

            My.Settings.Save()

        End If
    End Sub



    Private Sub txtPersistenceLength_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtPersistenceLength.TextChanged
        persistenceLength = CInt(txtPersistenceLength.Text)
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As System.Object, e As System.EventArgs) Handles tsmOverviewMap.Click
        mainMapManager.addOverviewForm()
    End Sub

    Private Sub ResetAllDefaultsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ResetAllDefaultsToolStripMenuItem.Click
        Dim ans As VariantType = MsgBox("Are you sure you want to continue? " & vbCrLf & "All HSC, habitat equations, subsequent results will be lost?", MsgBoxStyle.YesNo, "Restore defaults?")
        If ans = vbNo Then
            Exit Sub
        End If

        Dim parentNode As XmlNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/SpeciesOfInterest")
        For Each strSpecies As String In mainDataManager.getSpeciesNames
            Dim speciesNode As XmlNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpecies & "']")
            Dim replacementNode As XmlNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/backup/SmartRiverConfig/SpeciesOfInterest/Species[Name='" & strSpecies & "']")
            Dim newReplacementNode As XmlNode = replacementNode.Clone()
            parentNode.AppendChild(newReplacementNode)
            parentNode.ReplaceChild(newReplacementNode, speciesNode)
        Next
        mainDataManager.config.Save(My.Settings.ConfigXML)

        HabitatGenerator = HabitatSuitibilityGeneratorForm.Instance(Me)
        HabitatGenerator.loadDisplaySelectors()
        HabitatGenerator.updateHSCControl()

        Dim msg As String = "All HSC and equation values have been reset." & vbCrLf
        msg += "If you have generated habitat layers from non-default HSCs or equations you will need to regenerate these with these reset default values." & vbCrLf
        msg += "Would you like to regenerate all of these species\lifestage output now?"
        msg += vbCrLf & vbCrLf & "Note: This processing can take several hours!"

        Dim ans2 As VariantType = MsgBox(msg, MsgBoxStyle.YesNo, "Restore defaults?")
        If ans2 = vbYes Then
            HabitatGenerator.regenAll()
        End If


    End Sub


    Private Sub SaveScenariosDBAsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SaveScenariosDBAsToolStripMenuItem.Click
        Dim FileBrowserDialog1 As New SaveFileDialog
        ' Then use the following code to create the Dialog window
        ' Change the .SelectedPath property to the default location
        With FileBrowserDialog1
            ' Desktop is the root folder in the dialog.
            If System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(My.Settings.SQliteDB)) Then
                .InitialDirectory = System.IO.Path.GetDirectoryName(My.Settings.SQliteDB)
            Else
                .InitialDirectory = Environment.SpecialFolder.MyComputer
            End If
            .Title = "Scenario/flows database (REFDSS_data.sqlite)"
            .AddExtension = True
            .DefaultExt = ".sqlite"
            .CheckFileExists = False
            If .ShowDialog = DialogResult.OK Then
                File.Copy(My.Settings.SQliteDB, FileBrowserDialog1.FileName)
                My.Settings.SQliteDB = FileBrowserDialog1.FileName
                mainDataManager.setSetting("SQLiteDB", My.Settings.SQliteDB)
                Exit Sub

            End If
        End With

        mainDataManager.saveconfig()
        My.Settings.Save()
    End Sub

    Private Sub OpenProjectFileconfigxmlToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles OpenProjectFileconfigxmlToolStripMenuItem.Click
        Dim FileBrowserDialog1 As New OpenFileDialog
        ' Then use the following code to create the Dialog window
        ' Change the .SelectedPath property to the default location
        With FileBrowserDialog1
            ' Desktop is the root folder in the dialog.
            If System.IO.File.Exists(System.IO.Path.GetDirectoryName(My.Settings.ConfigXML)) Then
                .InitialDirectory = System.IO.Path.GetDirectoryName(My.Settings.ConfigXML)
            Else
                .InitialDirectory = Environment.SpecialFolder.MyComputer
            End If
            .Title = "Select a project settings file (config.xml)"
            .CheckFileExists = True
            If .ShowDialog = DialogResult.OK Then
                My.Settings.ConfigXML = FileBrowserDialog1.FileName

                Try
                    mainDataManager.config.Load(My.Settings.ConfigXML)
                Catch ex As Exception
                    Dim msg As String
                    msg = "There appears to be a problem with the specified configuration file:" & vbCrLf
                    msg += My.Settings.ConfigXML
                    msg += vbCrLf & "Please navigate to a valid version of this file (Delaware_config.xml) using the file menu, 'Open' option."
                    MsgBox(msg)
                    Exit Sub
                End Try

                My.Settings.Save()
            Else
                Exit Sub

            End If
        End With

        InitializeContent()
    End Sub

    Private Sub SaveProjectConfigToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SaveProjectConfigToolStripMenuItem.Click
        mainDataManager.saveconfig()
    End Sub

    Private Sub SaveAsProjectFileconfigxmlToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SaveAsProjectFileconfigxmlToolStripMenuItem.Click
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
            .Title = "Select a project settings file (config.xml)"
            .AddExtension = True
            .DefaultExt = ".xml"
            .CheckFileExists = False
            If .ShowDialog = DialogResult.OK Then
                My.Settings.ConfigXML = FileBrowserDialog1.FileName
            Else
                Exit Sub
            End If
        End With

        mainDataManager.saveconfig()
        My.Settings.Save()
    End Sub

    Private Sub ChangeScenariosflowsDelewareDSSdatasqliteToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ChangeScenariosflowsDelewareDSSdatasqliteToolStripMenuItem.Click
        Dim FileBrowserDialog1 As New OpenFileDialog
        ' Then use the following code to create the Dialog window
        ' Change the .SelectedPath property to the default location
        With FileBrowserDialog1
            ' Desktop is the root folder in the dialog.
            If System.IO.File.Exists(System.IO.Path.GetDirectoryName(My.Settings.SQliteDB)) Then
                .InitialDirectory = System.IO.Path.GetDirectoryName(My.Settings.SQliteDB)
            Else
                .InitialDirectory = Environment.SpecialFolder.MyComputer
            End If
            .Title = "Select a project flow/habitat database (*_data.sqlite)"
            .CheckFileExists = True
            If .ShowDialog = DialogResult.OK Then
                My.Settings.SQliteDB = FileBrowserDialog1.FileName
                mainDataManager.setSetting("SQLiteDB", FileBrowserDialog1.FileName)
                mainDataManager.saveconfig()
            Else
                Exit Sub

            End If
        End With

        InitializeContent()
    End Sub

    Private Sub MetricToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MetricToolStripMenuItem.Click
        MetricToolStripMenuItem.Checked = True
        ImperialToolStripMenuItem.Checked = False
        mainDataManager.curUnits = "Metric"

        updateContents()
    End Sub

    Private Sub ImperialToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImperialToolStripMenuItem.Click
        MetricToolStripMenuItem.Checked = False
        ImperialToolStripMenuItem.Checked = True
        mainDataManager.curUnits = "Imperial"

        updateContents()
    End Sub

    Private Sub updateContents()
        If Not HabitatGenerator Is Nothing Then
            HabitatGenerator.updateHSCControl()
        End If

        mainMapManager.updateSegmentTitles()
        mainGraphManager.updateLoadData()
        mainHydrographManager.updateLoadData()
    End Sub

    Private Sub TableClick(sender As Object, e As EventArgs)
        Dim strTableFname = sender.Tag.Split(New String() {"||||"}, StringSplitOptions.RemoveEmptyEntries)(0)
        Dim strSheetName = sender.Tag.Split(New String() {"||||"}, StringSplitOptions.RemoveEmptyEntries)(1)

        OpenExcel(Path.Combine(My.Settings.InputDataDirectory, "Tier1\Tables", strTableFname), strSheetName)
    End Sub
    Public Sub OpenExcel(ByVal FileName As String, Optional ByVal SheetName As String = "")
        If IO.File.Exists(FileName) Then
            Dim Proceed As Boolean = False
            Dim xlApp As Excel.Application = Nothing
            Dim xlWorkBooks As Excel.Workbooks = Nothing
            Dim xlWorkBook As Excel.Workbook = Nothing
            Dim xlWorkSheet As Excel.Worksheet = Nothing
            Dim xlWorkSheets As Excel.Sheets = Nothing
            Dim xlCells As Excel.Range = Nothing
            xlApp = New Excel.Application
            xlApp.DisplayAlerts = False
            xlWorkBooks = xlApp.Workbooks
            xlWorkBook = xlWorkBooks.Open(FileName)
            xlApp.Visible = True
            xlWorkSheets = xlWorkBook.Sheets
            For x As Integer = 1 To xlWorkSheets.Count
                xlWorkSheet = CType(xlWorkSheets(x), Excel.Worksheet)
                If xlWorkSheet.Name = SheetName Then
                    Console.WriteLine(SheetName)
                    Proceed = True
                    Exit For
                End If
                Runtime.InteropServices.Marshal.FinalReleaseComObject(xlWorkSheet)
                xlWorkSheet = Nothing
            Next
            If Proceed Then
                xlWorkSheet.Activate()
            End If
        Else
            MessageBox.Show("'" & FileName & "' not located. Try one of the write examples first.")
        End If
    End Sub

    Public Sub ReleaseComObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch ex As Exception
            obj = Nothing
        End Try
    End Sub

    Private Sub FigureClick(sender As Object, e As EventArgs)
        mainDataManager.addFigureForm(sender.text)
    End Sub


    Private Sub GuayReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GuayReportToolStripMenuItem.Click
        System.Diagnostics.Process.Start(Path.Combine(My.Settings.InputDataDirectory, "GuayReport\Topock Water Resources Guide-Final 2012 (Guay).pdf"))
    End Sub
End Class