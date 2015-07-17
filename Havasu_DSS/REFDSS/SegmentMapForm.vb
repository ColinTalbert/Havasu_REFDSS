Imports System.IO
Imports System.Xml

Public Class SegmentMapForm
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent


    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents lblTitle As System.Windows.Forms.Label

    Private mainMapManager As MapManager

    Public curScenario As String
    Public curSegment As String
    Public curTreatment As String = "2005"
    Public curSpecies As String
    Public curLifeStage As String
    Public curCovariate As String
    Public curFlow As New List(Of String)
    Public curSingleFlow As Boolean = False
    Public curSurveySpecies As String = "None"
    Public curSurveyYear As String = "All"

    Private curVisibleHandle As Integer = -1
    Public colorSchemes As New Dictionary(Of String, MapWinGIS.GridColorScheme)


    Private imgDict As New Dictionary(Of String, MWImageInfo)
    Private BackgroundInfo As New MWImageInfo
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Private components As System.ComponentModel.IContainer
    Friend WithEvents LockToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents Scenario As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Treatments As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Segment As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Species As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Lifestage As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Covariate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Flows As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AxMap1 As AxMapWinGIS.AxMap

    Public surveySep As New ToolStripSeparator
    Public surveySpecies As New ToolStripMenuItem
    Public surveyYears As New ToolStripMenuItem

    Public surveyPointsHndl As Integer = -1
    Public surveyPointsHndl2 As Integer = -1

    Public bLocked As Boolean = False

    Public Sub New(MM As MapManager)
        mainMapManager = MM
        Me.InitializeComponent()

        loadColorSchemes()
        LoadData()
        loadBackgroundImage()
        'loadHabitatGISData()
        ''loadGISData()
        'displayFlow(curFlow)

        AxMap1.SendMouseDown = True
        AxMap1.TrapRMouseDown = False
    End Sub

    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SegmentMapForm))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.AxMap1 = New AxMapWinGIS.AxMap()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.LockToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.Scenario = New System.Windows.Forms.ToolStripMenuItem()
        Me.Segment = New System.Windows.Forms.ToolStripMenuItem()
        Me.Treatments = New System.Windows.Forms.ToolStripMenuItem()
        Me.Species = New System.Windows.Forms.ToolStripMenuItem()
        Me.Lifestage = New System.Windows.Forms.ToolStripMenuItem()
        Me.Covariate = New System.Windows.Forms.ToolStripMenuItem()
        Me.Flows = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel1.SuspendLayout()
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        CType(Me.AxMap1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel1.Controls.Add(Me.ToolStripContainer1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(818, 744)
        Me.Panel1.TabIndex = 0
        '
        'ToolStripContainer1
        '
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.lblTitle)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.AxMap1)
        Me.ToolStripContainer1.ContentPanel.Size = New System.Drawing.Size(814, 715)
        Me.ToolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ToolStripContainer1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.Size = New System.Drawing.Size(814, 740)
        Me.ToolStripContainer1.TabIndex = 4
        Me.ToolStripContainer1.Text = "ToolStripContainer1"
        '
        'ToolStripContainer1.TopToolStripPanel
        '
        Me.ToolStripContainer1.TopToolStripPanel.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ToolStripContainer1.TopToolStripPanel.Padding = New System.Windows.Forms.Padding(5, 0, 5, 0)
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(3, 4)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(2, 18)
        Me.lblTitle.TabIndex = 1
        '
        'AxMap1
        '
        Me.AxMap1.AllowDrop = True
        Me.AxMap1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.AxMap1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AxMap1.Enabled = True
        Me.AxMap1.Location = New System.Drawing.Point(0, 0)
        Me.AxMap1.Name = "AxMap1"
        Me.AxMap1.OcxState = CType(resources.GetObject("AxMap1.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxMap1.Size = New System.Drawing.Size(814, 715)
        Me.AxMap1.TabIndex = 0
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LockToolStripMenuItem, Me.ToolStripSeparator1, Me.Scenario, Me.Segment, Me.Treatments, Me.Species, Me.Lifestage, Me.Covariate, Me.Flows})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(139, 186)
        '
        'LockToolStripMenuItem
        '
        Me.LockToolStripMenuItem.Name = "LockToolStripMenuItem"
        Me.LockToolStripMenuItem.Size = New System.Drawing.Size(138, 22)
        Me.LockToolStripMenuItem.Text = "Lock"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(135, 6)
        '
        'Scenario
        '
        Me.Scenario.Name = "Scenario"
        Me.Scenario.Size = New System.Drawing.Size(138, 22)
        Me.Scenario.Text = "Scenario"
        '
        'Segment
        '
        Me.Segment.Name = "Segment"
        Me.Segment.Size = New System.Drawing.Size(138, 22)
        Me.Segment.Text = "Segment"
        '
        'Treatments
        '
        Me.Treatments.Name = "Treatments"
        Me.Treatments.Size = New System.Drawing.Size(138, 22)
        Me.Treatments.Text = "Time Period"
        '
        'Species
        '
        Me.Species.Name = "Species"
        Me.Species.Size = New System.Drawing.Size(138, 22)
        Me.Species.Text = "Species"
        '
        'Lifestage
        '
        Me.Lifestage.Name = "Lifestage"
        Me.Lifestage.Size = New System.Drawing.Size(138, 22)
        Me.Lifestage.Text = "Lifestage"
        '
        'Covariate
        '
        Me.Covariate.Name = "Covariate"
        Me.Covariate.Size = New System.Drawing.Size(138, 22)
        Me.Covariate.Text = "Covariate"
        '
        'Flows
        '
        Me.Flows.Name = "Flows"
        Me.Flows.Size = New System.Drawing.Size(138, 22)
        Me.Flows.Text = "Flows"
        '
        'SegmentMapForm
        '
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(818, 744)
        Me.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "SegmentMapForm"
        Me.TabPageContextMenuStrip = Me.ContextMenuStrip1
        Me.Text = "Segment Map"
        Me.Panel1.ResumeLayout(False)
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.ContentPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        CType(Me.AxMap1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Private Sub loadColorSchemes()
        colorSchemes("Habitat") = mainMapManager.parentMainForm.mainDataManager.getColorScheme("SmartRiverConfig/SpeciesOfInterest/ColorScheme")
        For Each cov In mainMapManager.parentMainForm.mainDataManager.getCovariateNames("all", curTreatment)
            If mainMapManager.parentMainForm.mainDataManager.isCovariateCategorical(cov) Then
                colorSchemes(cov) = mainMapManager.parentMainForm.mainDataManager.getCategoricalColorScheme("SmartRiverConfig/Covariates/Covariate[Name='" & cov & "']/ColorScheme")
            Else
                If mainMapManager.parentMainForm.mainDataManager.curUnits = "metric" Then
                    colorSchemes(cov) = mainMapManager.parentMainForm.mainDataManager.getColorScheme("SmartRiverConfig/Covariates/Covariate[Name='" & cov & "']/ColorScheme")
                Else
                    colorSchemes(cov) = mainMapManager.parentMainForm.mainDataManager.getColorScheme("SmartRiverConfig/Covariates/Covariate[Name='" & cov & "']/ColorSchemeImperial")
                End If

            End If
Next
    End Sub

    Public Sub loadBackgroundImage()
        Dim backgroundfile As String = Path.Combine(My.Settings.InputDataDirectory, "Overview\HoJoTopockclip_nc1m.tif")
        If BackgroundInfo.filename <> backgroundfile Then
            If BackgroundInfo.handle <> -1 Then
                AxMap1.RemoveLayer(BackgroundInfo.handle)
                BackgroundInfo.image.Close()
            End If

            BackgroundInfo.image.UseTransparencyColor = False
            BackgroundInfo.image.TransparencyColor = Convert.ToUInt32(RGB(0, 0, 0))
            BackgroundInfo.image.UpsamplingMode = MapWinGIS.tkInterpolationMode.imBilinear
            BackgroundInfo.image.AllowHillshade = False

            BackgroundInfo.image.Open(backgroundfile, MapWinGIS.ImageType.USE_FILE_EXTENSION, False)

            BackgroundInfo.filename = backgroundfile
            BackgroundInfo.handle = AxMap1.AddLayer(BackgroundInfo.image, True)

        End If
    End Sub

    Public Sub loadHabitatGISData()
        Debug.Print("loadHabitatGISData()")
        Me.Cursor = Cursors.WaitCursor
        AxMap1.LockWindow(MapWinGIS.tkLockMode.lmLock)



        For Each imageInfo In imgDict.Values
            AxMap1.RemoveLayer(imageInfo.handle)
        Next

        If curSpecies <> "No species found" Then
            For Each flow In mainMapManager.parentMainForm.mainDataManager.getFlows(curSegment, curTreatment)
                Dim strFileName As String = mainMapManager.parentMainForm.mainDataManager.genOutputFname(curSegment, curTreatment, _
                                                                                                         flow, curSpecies, curLifeStage)
                addImage(strFileName, flow)
            Next
        End If

        AxMap1.LockWindow(MapWinGIS.tkLockMode.lmUnlock)
        Me.Cursor = Cursors.Default

    End Sub

    Private Sub loadCovariateGISData()
        Debug.Print("loadGISData()")
        Me.Cursor = Cursors.WaitCursor
        AxMap1.LockWindow(MapWinGIS.tkLockMode.lmLock)



        For Each imageInfo In imgDict.Values
            AxMap1.RemoveLayer(imageInfo.handle)
        Next

        'If singleFlow variable (ie categorical) load the layer
        'else load one file for each flow
        If mainMapManager.parentMainForm.mainDataManager.isCovariateSingleFlow(curCovariate) Then
            Dim strFileName As String = mainMapManager.getCovariateFileName("NA", curCovariate, curTreatment)
            addImage(strFileName, "NA")
        Else
            For Each flow In mainMapManager.parentMainForm.mainDataManager.getFlows(curSegment, curTreatment)
                Dim strFileName As String = mainMapManager.getCovariateFileName(flow, curCovariate, curTreatment)
                addImage(strFileName, flow)
            Next

        End If


        AxMap1.LockWindow(MapWinGIS.tkLockMode.lmUnlock)
        Me.Cursor = Cursors.Default


    End Sub

    Public Sub updateCovariateSymbology()
        If curCovariate <> "NA" Then
            colorSchemes(curCovariate) = mainMapManager.parentMainForm.mainDataManager.get_HSC_ColorScheme(curCovariate)
            loadCovariateGISData()
            displayFlow(curFlow)
        End If
    End Sub

    Private Sub addImage(strFileName, flow)
        Dim imgInfo As New MWImageInfo
        imgInfo.filename = strFileName


        If imgInfo.image.Open(imgInfo.filename, MapWinGIS.ImageType.USE_FILE_EXTENSION, False, ) Then
            'Push our coloring scheme in - normally this will have no effect for an image, but
            'iff tkRaster is rendering it, it will indeed get used and ought to be faster than bmp generation                    
            If curCovariate = "NA" Then
                imgInfo.image._pushSchemetkRaster(colorSchemes("Habitat"))
            Else
                imgInfo.image._pushSchemetkRaster(colorSchemes(curCovariate))
            End If
        End If


        imgInfo.image.UseTransparencyColor = True
        imgInfo.image.TransparencyColor = Convert.ToUInt32(RGB(0, 0, 0))
        imgInfo.image.UpsamplingMode = MapWinGIS.tkInterpolationMode.imNone
        imgInfo.image.DownsamplingMode = MapWinGIS.tkInterpolationMode.imNone
        imgInfo.image.AllowHillshade = False



        imgInfo.handle = AxMap1.AddLayer(imgInfo.image, False)
        imgInfo.flow = flow
        imgDict(flow) = imgInfo



    End Sub

    Public Sub LoadData()
        Debug.Print("loadData()")
        Scenario.DropDownItems.Clear()
        For Each scen In mainMapManager.parentMainForm.mainDataManager.getScenarioNames()
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = scen
            menuitem.Tag = "scenario"
            menuitem.CheckOnClick = +True
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click
            Scenario.DropDownItems.Add(menuitem)
        Next
        setCheckedDropdownItem(Scenario.DropDownItems, Scenario.DropDownItems.Item(0).Text)
        curScenario = Scenario.DropDownItems.Item(0).Text


        'add our segments to the DropDownItems
        Segment.DropDownItems.Clear()
        For Each seg In mainMapManager.parentMainForm.mainDataManager.getSegmentNames()
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = seg
            menuitem.Tag = "segment"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            Segment.DropDownItems.Add(menuitem)
        Next
        setCheckedDropdownItem(Segment.DropDownItems, Segment.DropDownItems.Item(0).Text)
        curSegment = Segment.DropDownItems.Item(0).Text

        'add our treatments to the DropDownItems
        Treatments.DropDownItems.Clear()
        For Each treat In mainMapManager.parentMainForm.mainDataManager.getTreatmentNames(curSegment)
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = treat
            menuitem.Tag = "treatment"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            Treatments.DropDownItems.Add(menuitem)
        Next
        setCheckedDropdownItem(Treatments.DropDownItems, Treatments.DropDownItems.Item(0).Text)
        curTreatment = Treatments.DropDownItems.Item(0).Text


        'add our species and lifestages to the form dropdowns
        Species.DropDownItems.Clear()
        For Each sps As String In mainMapManager.parentMainForm.mainDataManager.getSpeciesNames(curSegment, curTreatment)
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = sps
            menuitem.Tag = "species"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            Species.DropDownItems.Add(menuitem)
        Next
        If Species.DropDownItems.Count = 0 Then
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = "No species found"
            menuitem.Tag = "species"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            Species.DropDownItems.Add(menuitem)
        End If

        setCheckedDropdownItem(Species.DropDownItems, Species.DropDownItems.Item(0).Text)
        curSpecies = Species.DropDownItems.Item(0).Text


        updatelifestages()

        Covariate.DropDownItems.Clear()
        Covariate.ForeColor = Color.Gray
        For Each Cov As String In mainMapManager.parentMainForm.mainDataManager.getCovariateNames(curSegment, curTreatment)
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = Cov
            menuitem.Tag = "covariate"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            Covariate.DropDownItems.Add(menuitem)
        Next
        curCovariate = "NA"

        LoadSpeciesLocationData()

        updateFlows()

        updateTitle()
    End Sub

    Public Sub LoadSpeciesLocationData()

        ContextMenuStrip1.Items.Add(surveySep)
        ContextMenuStrip1.Items.Add(surveySpecies)
        surveySpecies.Text = "Bird Survey Points"

        Dim menuitemNone As New ToolStripMenuItem
        menuitemNone.Text = "None"
        menuitemNone.Checked = True
        menuitemNone.Tag = "surveySpecies"
        menuitemNone.CheckOnClick = True
        menuitemNone.DoubleClickEnabled = False
        surveySpecies.DropDownItems.Add(menuitemNone)
        AddHandler menuitemNone.Click, AddressOf ToolStripMenuItem_Click

        Dim sep As New ToolStripSeparator
        surveySpecies.DropDownItems.Add(sep)

        Dim menuitemAll As New ToolStripMenuItem
        menuitemAll.Text = "All"
        menuitemAll.Tag = "surveyYear"
        menuitemAll.Checked = True
        menuitemAll.Tag = "surveyYear"
        menuitemAll.CheckOnClick = True
        menuitemAll.DoubleClickEnabled = False
        surveyYears.DropDownItems.Add(menuitemAll)
        AddHandler menuitemAll.Click, AddressOf ToolStripMenuItem_Click

        Dim sep2 As New ToolStripSeparator
        surveyYears.DropDownItems.Add(sep2)

        ContextMenuStrip1.Items.Add(surveyYears)
        surveyYears.Text = "Bird Survey Years"

        For Each sp As String In mainMapManager.parentMainForm.mainDataManager.getSurveySpecies()
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = sp
            menuitem.Tag = "surveySpecies"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            If sp <> "---" Then
                surveySpecies.DropDownItems.Add(menuitem)
            End If

        Next

        For Each sp As String In mainMapManager.parentMainForm.mainDataManager.getSurveyYears()
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = sp
            menuitem.Tag = "surveyYear"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False


            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            surveyYears.DropDownItems.Add(menuitem)
        Next

    End Sub

    Private Sub updatelifestages()
        Debug.Print("updatelifestages()")
        Lifestage.DropDownItems.Clear()
        If curSpecies <> "No species found" Then


            For Each ls As String In mainMapManager.parentMainForm.mainDataManager.getLifeStageNames(curSpecies)
                Dim menuitem As New ToolStripMenuItem
                menuitem.Text = ls
                menuitem.Tag = "lifestage"
                menuitem.CheckOnClick = True
                menuitem.DoubleClickEnabled = False
                AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

                Lifestage.DropDownItems.Add(menuitem)
            Next
            setCheckedDropdownItem(Lifestage.DropDownItems, Lifestage.DropDownItems.Item(0).Text)
            curLifeStage = Lifestage.DropDownItems.Item(0).Text
        End If
    End Sub
    Public Sub updateSpecies()
        Debug.Print("updateSpecies()")
        Species.DropDownItems.Clear()
        For Each sps As String In mainMapManager.parentMainForm.mainDataManager.getSpeciesNames(curSegment, curTreatment)
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = sps
            menuitem.Tag = "species"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            Species.DropDownItems.Add(menuitem)
        Next
        setCheckedDropdownItem(Species.DropDownItems, Species.DropDownItems.Item(0).Text)
        curFlow.Clear()
        curFlow.Add(Flows.DropDownItems.Item(0).Text)
    End Sub

    Public Sub updateTreatments()
        Debug.Print("updateTreatments()")
        Treatments.DropDownItems.Clear()
        For Each treatment As String In mainMapManager.parentMainForm.mainDataManager.getTreatmentNames(curSegment)
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = treatment
            menuitem.Tag = "treatment"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            Treatments.DropDownItems.Add(menuitem)
        Next
        setCheckedDropdownItem(Treatments.DropDownItems, Treatments.DropDownItems.Item(0).Text)
        curTreatment = Treatments.DropDownItems.Item(0).Text
    End Sub

    Public Sub updateCovariates()
        Debug.Print("updateCovariates()")
        Covariate.DropDownItems.Clear()
        For Each cov As String In mainMapManager.parentMainForm.mainDataManager.getCovariateNames(curSegment, curTreatment)
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = cov
            menuitem.Tag = "covariate"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            Covariate.DropDownItems.Add(menuitem)
        Next
        setCheckedDropdownItem(Covariate.DropDownItems, Covariate.DropDownItems.Item(0).Text)

    End Sub

    Public Sub updateFlows()
        Debug.Print("updateFlows()")
        Flows.DropDownItems.Clear()
        For Each flow As String In mainMapManager.parentMainForm.mainDataManager.getFlows(curSegment, curTreatment)
            Dim menuitem As New ToolStripMenuItem

            If mainMapManager.parentMainForm.mainDataManager.curUnits = "metric" Then
                menuitem.Text = flow

            Else
                menuitem.Text = (CDbl(flow) * mainMapManager.parentMainForm.mainDataManager.getConversionFactor("MetersToFeet")).ToString("N1")
            End If

            menuitem.Tag = flow
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            Flows.DropDownItems.Add(menuitem)
        Next
        setCheckedDropdownItem(Flows.DropDownItems, Flows.DropDownItems.Item(0).Text)
        curFlow.Clear()
        curFlow.Add(Flows.DropDownItems.Item(0).Tag)

    End Sub





    Public Sub updateTitle()
        Debug.Print("updateTitle()")
        Dim title As String = ""
        title += "Scenario:    " + curScenario + vbCrLf
        'title += "Time Period:    " + curTreatment + vbCrLf
        'title += "Segment:    " + curSegment + vbCrLf
        If curCovariate = "NA" Then
            title += "Species:     " + curSpecies + vbCrLf
            title += "Lifestage:   " + curLifeStage + vbCrLf
        Else
            title += "Covariate:    " + curCovariate + vbCrLf
        End If

        If MainForm.mainDataManager.isCovariateSingleFlow(curCovariate) Then
            title += "WSE:            NA"
        ElseIf MainForm.mainDataManager.curUnits = "Metric" Then
            title += "WSE:            " + CDbl(curFlow(curFlow.Count - 1)).ToString("n1") + " meters"
        Else
            Dim val As Double = curFlow(curFlow.Count - 1) * MainForm.mainDataManager.getConversionFactor("MetersToFeet")
            title += "WSE:            " + val.ToString("n1") + " feet"
        End If

        lblTitle.Text = title
    End Sub

    Public Sub displayFlow(flowList As List(Of String))
        Debug.Print("displayFlow()")
        Dim flow As String
        Dim flow_ft As String

        If curSingleFlow Then
            flow = "NA"
        Else
            flow = flowList(0)
            flow = getClosestFlow(flow)
            curFlow.Add(flow)
        End If

        If Not mainMapManager.parentMainForm.mainDataManager.curUnits = "Metric" And Not curSingleFlow Then
            flow_ft = CDbl(flow) * mainMapManager.parentMainForm.mainDataManager.getConversionFactor("MetersToFeet")
            setCheckedDropdownItem(Flows.DropDownItems, flow)
        ElseIf Not curSingleFlow Then
            setCheckedDropdownItem(Flows.DropDownItems, flow)
        End If

        For Each f In mainMapManager.parentMainForm.mainDataManager.getFlows(curSegment, curTreatment)
            Try
                AxMap1.set_LayerVisible(imgDict(f).handle, False)
            Catch ex As Exception

            End Try

        Next

        Try
            AxMap1.set_LayerVisible(imgDict(flow).handle, True)
            'AxMap1.set_LayerVisible(2, True)
            curVisibleHandle = imgDict(flow).handle
            Debug.Print("showing " + curVisibleHandle)
        Catch ex As Exception

        End Try

        display_survey_points()
        updateTitle()
    End Sub

    Public Sub display_survey_points()

        If surveyPointsHndl2 <> -1 Then AxMap1.RemoveLayer(surveyPointsHndl2)
        If surveyPointsHndl <> -1 Then AxMap1.RemoveLayer(surveyPointsHndl)
        surveyPointsHndl2 = -1
        surveyPointsHndl = -1

        If curSurveySpecies = "None" Then


            Exit Sub
        End If


        Dim surveyDataTable As DataTable = mainMapManager.parentMainForm.mainDataManager.getSurveySpeciesData(curSurveySpecies, curSurveyYear, False)


        Dim sf As New MapWinGIS.Shapefile()
        Dim result As Boolean = sf.CreateNewWithShapeID("", MapWinGIS.ShpfileType.SHP_POLYGON)

        Dim fldx As New MapWinGIS.Field
        Dim fldy As New MapWinGIS.Field
        Dim fldArea As New MapWinGIS.Field

        sf.StartEditingTable()
        sf.EditInsertField(fldx, 0)
        sf.EditInsertField(fldy, 1)
        sf.EditInsertField(fldArea, 2)
        sf.StopEditingTable()

        Dim i As Integer = 0
        For Each pnt_row In surveyDataTable.rows
            Dim xCenter As Double = pnt_row(0)
            Dim yCenter As Double = pnt_row(1)


            Dim radius As Double = pnt_row(2)

            ' polygons must be clockwise
            Dim shp As New MapWinGIS.Shape()
            shp.Create(MapWinGIS.ShpfileType.SHP_POLYGON)

            For j As Integer = 0 To 36
                Dim pnt As New MapWinGIS.Point()
                pnt.x = xCenter + radius * Math.Cos(j * Math.PI / 18)
                pnt.y = yCenter - radius * Math.Sin(j * Math.PI / 18)
                shp.InsertPoint(pnt, j)
            Next
            sf.EditInsertShape(shp, i)

            sf.EditCellValue(0, i, xCenter.ToString())
            sf.EditCellValue(1, i, yCenter.ToString())
            sf.EditCellValue(2, i, Math.PI * radius * radius)

            i += 1
        Next

        Dim options As MapWinGIS.ShapeDrawingOptions = sf.DefaultDrawingOptions
        options.FillType = MapWinGIS.tkFillType.ftStandard
        options.FillColor = Convert.ToUInt32(RGB(248, 230, 99))
        options.FillTransparency = 150

        Try
            If surveyPointsHndl <> -1 Then AxMap1.RemoveLayer(surveyPointsHndl)
        Catch ex As Exception

        End Try

        surveyPointsHndl = AxMap1.AddLayer(sf, True)

        Dim surveyDataTable_absent As DataTable = mainMapManager.parentMainForm.mainDataManager.getSurveySpeciesData(curSurveySpecies, curSurveyYear, True)

        Dim sf2 As New MapWinGIS.Shapefile()
        Dim result2 As Boolean = sf2.CreateNewWithShapeID("", MapWinGIS.ShpfileType.SHP_POINT)

        Dim fldx2 As New MapWinGIS.Field
        Dim fldy2 As New MapWinGIS.Field

        sf2.StartEditingTable()
        sf2.EditInsertField(fldx2, 0)
        sf2.EditInsertField(fldy2, 1)
        sf2.StopEditingTable()

        i = 0
        For Each pnt_row In surveyDataTable_absent.Rows
            Dim xCenter As Double = pnt_row(0)
            Dim yCenter As Double = pnt_row(1)


            ' polygons must be clockwise
            Dim shp As New MapWinGIS.Shape()
            shp.Create(MapWinGIS.ShpfileType.SHP_POINT)
            Dim pnt As New MapWinGIS.Point()
            pnt.x = xCenter
            pnt.y = yCenter
            shp.InsertPoint(pnt, 0)
            sf2.EditInsertShape(shp, i)

            sf2.EditCellValue(0, i, xCenter.ToString())
            sf2.EditCellValue(1, i, yCenter.ToString())

            i += 1
        Next

        Dim success As Boolean = sf2.SaveAs("C:\temp_colin\junk\test.shp")
        Dim options2 As MapWinGIS.ShapeDrawingOptions = sf2.DefaultDrawingOptions
        options2.FillType = MapWinGIS.tkFillType.ftStandard
        options2.FillColor = Convert.ToUInt32(RGB(0, 0, 255))
        options2.FillTransparency = 150
        options2.PointShape = MapWinGIS.tkPointShapeType.ptShapeCross

        surveyPointsHndl2 = AxMap1.AddLayer(sf2, True)
    End Sub

    Public Sub syncExternalExtents(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Debug.Print("syncExternalExtents()")
        Dim extents As MapWinGIS.Extents
        extents = AxMap1.Extents

        If Not bLocked Then
            mainMapManager.syncExtents(Me, extents)
        End If

        addScaleBar(AxMap1)
    End Sub

    Public Sub syncExtent(extent As MapWinGIS.Extents)
        Debug.Print("syncExtent()")
        'This sub is called for each map by the MapManager for each of the child segment maps
        RemoveHandler AxMap1.ExtentsChanged, AddressOf syncExternalExtents
        AxMap1.Extents = extent
        addScaleBar(AxMap1)
        AddHandler AxMap1.ExtentsChanged, AddressOf syncExternalExtents
    End Sub

    Private Sub SegmentMapForm_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        mainMapManager.removeSegmentMapForm(Me)
    End Sub



    Private Sub ToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs)
        Debug.Print("ToolStripMenuItem_Click()")
        'function to make the checkboxes work like radio obtions
        'Also changes the instance flags and grays out options as needed.

        For Each MenuItem In sender.owner.items
            If Not MenuItem Is sender And Not TypeOf (MenuItem) Is ToolStripSeparator Then
                MenuItem.checked = False
            End If
        Next

        If sender.tag = "scenario" Then
            curScenario = sender.text
        ElseIf sender.tag = "segment" Then
            curSegment = sender.text
            updateTreatments()
            updateSpecies()
            updateFlows()
        ElseIf sender.tag = "treatment" Then
            curTreatment = sender.text
            updateCovariates()
            updateSpecies()
            updateFlows()
        ElseIf sender.tag = "species" Then
            curSpecies = sender.text
            updatelifestages()
            curLifeStage = getCheckedDropdownItem(Lifestage.DropDownItems)
            curCovariate = "NA"
            Covariate.ForeColor = Color.Gray
            Species.ForeColor = Color.Black
            Lifestage.ForeColor = Color.Black
            curSingleFlow = False
        ElseIf sender.tag = "lifestage" Then
            curSpecies = getCheckedDropdownItem(Species.DropDownItems)
            curLifeStage = sender.text
            curCovariate = "NA"
            Covariate.ForeColor = Color.Gray
            Species.ForeColor = Color.Black
            Lifestage.ForeColor = Color.Black
            curSingleFlow = False
        ElseIf sender.tag = "covariate" Then
            curCovariate = sender.text
            curSingleFlow = mainMapManager.parentMainForm.mainDataManager.isCovariateSingleFlow(curCovariate)
            curSpecies = "NA"
            curLifeStage = "NA"
            Covariate.ForeColor = Color.Black
            Species.ForeColor = Color.Gray
            Lifestage.ForeColor = Color.Gray
        ElseIf sender.tag = "surveySpecies" Then
            curSurveySpecies = sender.text
        ElseIf sender.tag = "surveyYear" Then
            curSurveyYear = sender.text
        Else
            'it must be a flow they clicked
            curFlow.Clear()
            curFlow.Add(sender.tag)
        End If

        If sender.tag = "segment" Or sender.tag = "treatment" Then
            loadBackgroundImage()
            If Lifestage.ForeColor = Color.Gray Then
                loadCovariateGISData()
            Else
                loadHabitatGISData()
            End If
        End If

        If sender.tag = "covariate" Then
            loadCovariateGISData()
        ElseIf sender.tag = "species" Or sender.tag = "lifestage" Then
            loadHabitatGISData()
        End If



        If bLocked Then
            displayFlow(curFlow)
        Else
            mainMapManager.syncFlow(curSegment, curScenario, curFlow)

        End If


        If sender.tag = "segment" Then
            AxMap1.ZoomToMaxExtents()
        End If

    End Sub


    Private Class MWImageInfo
        Public filename As String
        Public image As New MapWinGIS.Image
        Public handle As Integer = -1
        Public flow As String
    End Class



    Private Function getClosestFlow(ByVal flow) As String
        Debug.Print("getClosestFlow()")
        For Each item In Flows.DropDownItems
            If flow <= CDbl(item.tag) Then
                Return item.tag
            End If
        Next
        Return Flows.DropDownItems.Item(Flows.DropDownItems.Count - 1).Tag
    End Function


    Private Sub LockToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles LockToolStripMenuItem.Click
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

    Private Sub AxMap1_MouseDownEvent(sender As Object, e As AxMapWinGIS._DMapEvents_MouseDownEvent) Handles AxMap1.MouseDownEvent
        If e.button = 1 And AxMap1.CursorMode = MapWinGIS.tkCursorMode.cmSelection Then


            addScaleBar(AxMap1)

            Dim drawHndl As Integer
            drawHndl = AxMap1.NewDrawing(MapWinGIS.tkDrawReferenceList.dlScreenReferencedList)

            Dim ui As UInteger
            'ui = MapWinGIS.tkMapColor.Transparent
            ui = Convert.ToUInt32(RGB(255, 255, 255))

            AxMap1.DrawPoint(e.x, e.y, 10, ui)

            Dim projx, projy As Double
            AxMap1.PixelToProj(e.x, e.y, projx, projy)

            Dim msg As String = ""
            msg += CStr(projx) + " " + CStr(projy) + vbCrLf
            msg = mainMapManager.getPixelValuesMsg(projx, projy, curSegment, curTreatment, curFlow(curFlow.Count - 1), curSpecies, curLifeStage)

            AxMap1.AddDrawingLabel(drawHndl, msg, ui, e.x, e.y, MapWinGIS.tkHJustification.hjRaw)

            Dim drawlabel As MapWinGIS.Labels = AxMap1.get_DrawingLabels(drawHndl)
            drawlabel.AutoOffset = True
            drawlabel.FrameBackColor = ui
            drawlabel.FrameVisible = True
        End If

        'Dim ext As MapWinGIS.Extents = AxMap.Extents
        ''AxMap1.DrawLine(ext.xMin, ext.yMax, ext.xMax, ext.yMin, 10, ui)
        'Dim left As Integer = 10
        'Dim right As Integer = 110
        'Dim barHeight As Integer = AxMap.Height - 15
        'Dim crossbarHeight As Int16 = 5

        'AxMap.DrawLine(left, barHeight, right, barHeight, 2, ui)
        'AxMap.DrawLine(left, barHeight - crossbarHeight, left, barHeight + crossbarHeight, 2, ui)
        'AxMap.DrawLine(right, barHeight - crossbarHeight, right, barHeight + crossbarHeight, 2, ui)

        ''how long is it?
        'Dim projx, projy As Double
        'AxMap.PixelToProj(left, barHeight, projx, projy)
        'Dim projx2, projy2 As Double
        'AxMap.PixelToProj(right, barHeight, projx2, projy2)

        'Dim barWidth As Double = projx2 - projx
        'Dim barWidthLabel As String

        'If barWidth > 500 Then
        '    barWidthLabel = Format(barWidth / 1000, "0.0") + " km"
        'ElseIf barWidth > 25 Then
        '    barWidthLabel = Format(barWidth, "0") + " m"
        'ElseIf barWidth > 0.5 Then
        '    barWidthLabel = Format(barWidth, "0.0") + " m"
        'Else
        '    barWidthLabel = Format(barWidth * 100, "0") + " cm"
        'End If


        'AxMap.AddDrawingLabel(drawHndl, barWidthLabel, ui, left + 23, barHeight - 8, MapWinGIS.tkHJustification.hjLeft)
        ''If e.button = 1 And AxMap1.CursorMode = MapWinGIS.tkCursorMode.cmSelection Then
        ''    Dim projx, projy As Double
        ''    AxMap1.PixelToProj(e.x, e.y, projx, projy)

        ''    Dim msg As String = ""
        ''    msg += CStr(projx) + " " + CStr(projy) + vbCrLf

        ''    msg = mainMapManager.getPixelValuesMsg(projx, projy, curSegment, curTreatment, curFlow(0), curSpecies, curLifeStage)
        ''    'MsgBox(msg)

        ''End If

        'MsgBox("test")
    End Sub

    Private Sub addScaleBar(AxMap)

        AxMap1.ClearDrawings()
        Dim drawHndl As Integer
        drawHndl = AxMap1.NewDrawing(MapWinGIS.tkDrawReferenceList.dlScreenReferencedList)

        Dim ui As UInteger
        'ui = MapWinGIS.tkMapColor.Transparent
        ui = Convert.ToUInt32(RGB(0, 0, 0))

        Dim ext As MapWinGIS.Extents = AxMap.Extents
        'AxMap1.DrawLine(ext.xMin, ext.yMax, ext.xMax, ext.yMin, 10, ui)
        Dim left As Integer = 10
        Dim right As Integer = 110
        Dim barHeight As Integer = AxMap.Height - 15
        Dim crossbarHeight As Int16 = 5

        AxMap.DrawLine(left, barHeight, right, barHeight, 2, ui)
        AxMap.DrawLine(left, barHeight - crossbarHeight, left, barHeight + crossbarHeight, 2, ui)
        AxMap.DrawLine(right, barHeight - crossbarHeight, right, barHeight + crossbarHeight, 2, ui)

        'how long is it?
        Dim projx, projy As Double
        AxMap.PixelToProj(left, barHeight, projx, projy)
        Dim projx2, projy2 As Double
        AxMap.PixelToProj(right, barHeight, projx2, projy2)

        Dim barWidth As Double = projx2 - projx
        Dim barWidthLabel As String

        If barWidth > 500 Then
            If mainMapManager.parentMainForm.mainDataManager.curUnits = "Metric" Then
                barWidthLabel = Format(barWidth / 1000, "0.0") + " km"
            Else
                barWidthLabel = Format(barWidth / 1000 * mainMapManager.parentMainForm.mainDataManager.getConversionFactor("KilometersToMiles"), "0.0") + " mi"
            End If

        ElseIf barWidth > 25 Then
            If mainMapManager.parentMainForm.mainDataManager.curUnits = "Metric" Then
                barWidthLabel = Format(barWidth, "0") + " m"
            Else
                barWidthLabel = Format(barWidth * mainMapManager.parentMainForm.mainDataManager.getConversionFactor("MetersToFeet"), "0") + " ft"
            End If
        ElseIf barWidth > 0.5 Then
            If mainMapManager.parentMainForm.mainDataManager.curUnits = "Metric" Then
                barWidthLabel = Format(barWidth, "0.0") + " m"
            Else
                barWidthLabel = Format(barWidth * mainMapManager.parentMainForm.mainDataManager.getConversionFactor("MetersToFeet"), "0.0") + " ft"
            End If
        Else
            If mainMapManager.parentMainForm.mainDataManager.curUnits = "Metric" Then
                barWidthLabel = Format(barWidth * 100, "0") + " cm"
            Else
                barWidthLabel = Format(barWidth * mainMapManager.parentMainForm.mainDataManager.getConversionFactor("MetersToInches"), "0") + " in"
            End If

        End If


        AxMap.AddDrawingLabel(drawHndl, barWidthLabel, ui, left + 23, barHeight - 8, MapWinGIS.tkHJustification.hjLeft)


    End Sub

#Region "Serialization"


    Public Function saveToXMLNode() As XmlNode
        Dim outputNode As XmlNode
        outputNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("SmartRiverWidget")

        Dim nameNode As XmlNode
        nameNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("Name")
        nameNode.InnerText = Me.Text
        outputNode.AppendChild(nameNode)

        Dim lockedNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("Locked")
        lockedNode.InnerText = bLocked.ToString
        outputNode.AppendChild(lockedNode)

        Dim curScenarioNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("curScenario")
        curScenarioNode.InnerText = curScenario
        outputNode.AppendChild(curScenarioNode)

        Dim curSegmentNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("curSegment")
        curSegmentNode.InnerText = curSegment
        outputNode.AppendChild(curSegmentNode)

        Dim curTreatmentNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("curTreatment")
        curTreatmentNode.InnerText = curTreatment
        outputNode.AppendChild(curTreatmentNode)

        Dim curSpeciesNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("curSpecies")
        curSpeciesNode.InnerText = curSpecies
        outputNode.AppendChild(curSpeciesNode)

        Dim curLifeStageNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("curLifeStage")
        curLifeStageNode.InnerText = curLifeStage
        outputNode.AppendChild(curLifeStageNode)

        Dim curCovariateNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("curCovariate")
        curCovariateNode.InnerText = curCovariate
        outputNode.AppendChild(curCovariateNode)

        Dim curFlowNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("curFlow")
        curFlowNode.InnerText = CStr(curFlow(0))
        outputNode.AppendChild(curFlowNode)

        Dim curSingleFlowNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("curSingleFlow")
        curSingleFlowNode.InnerText = curSingleFlow.ToString
        outputNode.AppendChild(curSingleFlowNode)


        Dim xMinNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("xMin")
        Dim exts As MapWinGIS.Extents
        exts = AxMap1.Extents
        xMinNode.InnerText = CStr(exts.xMin)
        outputNode.AppendChild(xMinNode)

        Dim xMaxNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("xMax")
        xMaxNode.InnerText = CStr(exts.xMax)
        outputNode.AppendChild(xMaxNode)

        Dim yMinNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("yMin")
        yMinNode.InnerText = CStr(exts.yMin)
        outputNode.AppendChild(yMinNode)

        Dim yMaxNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("yMax")
        yMaxNode.InnerText = CStr(exts.yMax)
        outputNode.AppendChild(yMaxNode)

        Dim surveySpeciesNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("surveySpecies")
        surveySpeciesNode.InnerText = curSurveySpecies
        outputNode.AppendChild(surveySpeciesNode)

        Dim surveyYearNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("surveyYear")
        surveyYearNode.InnerText = CStr(curSurveyYear)
        outputNode.AppendChild(surveyYearNode)

        Return outputNode

    End Function

    Public Sub loadFromXmlNode(inNode As XmlNode)
        Try


            Dim dcNode As XmlNode
            dcNode = mainMapManager.parentMainForm.mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & "']")
            bLocked = CBool(dcNode.SelectSingleNode("Locked").InnerText)

            curTreatment = dcNode.SelectSingleNode("curTreatment").InnerText
            setCheckedDropdownItem(Treatments.DropDownItems, curTreatment)
            curScenario = dcNode.SelectSingleNode("curScenario").InnerText
            setCheckedDropdownItem(Scenario.DropDownItems, curScenario)
            curSegment = dcNode.SelectSingleNode("curSegment").InnerText
            setCheckedDropdownItem(Segment.DropDownItems, curSegment)

            Try
                curSurveySpecies = dcNode.SelectSingleNode("surveySpecies").InnerText
                setCheckedDropdownItem(surveySpecies.DropDownItems, curSurveySpecies)
                curSurveyYear = dcNode.SelectSingleNode("surveyYear").InnerText
                setCheckedDropdownItem(surveyYears.DropDownItems, curSurveyYear)
            Catch ex As Exception

            End Try

            


            updateFlows()
            updateCovariates()
            curSpecies = dcNode.SelectSingleNode("curSpecies").InnerText
            curLifeStage = dcNode.SelectSingleNode("curLifeStage").InnerText
            If curSpecies <> "NA" Then
                updatelifestages()
                Covariate.ForeColor = Color.Gray
                Species.ForeColor = Color.Black
                Lifestage.ForeColor = Color.Black
                setCheckedDropdownItem(Species.DropDownItems, curSpecies)
                setCheckedDropdownItem(Lifestage.DropDownItems, curLifeStage)
            Else
                Covariate.ForeColor = Color.Black
                Species.ForeColor = Color.Gray
                Lifestage.ForeColor = Color.Gray
            End If


            curCovariate = dcNode.SelectSingleNode("curCovariate").InnerText
            If curCovariate <> "NA" Then
                setCheckedDropdownItem(Covariate.DropDownItems, curCovariate)
            End If

            curFlow.Clear()
            curFlow.Add(dcNode.SelectSingleNode("curFlow").InnerText)
            setCheckedDropdownItem(Flows.DropDownItems, curFlow(0))
            curSingleFlow = CBool(dcNode.SelectSingleNode("curSingleFlow").InnerText)

            redrawMap()

            Dim prevExtent As New MapWinGIS.Extents
            Dim xMin, yMin, xMax, ymax As Double
            xMin = CDbl(dcNode.SelectSingleNode("xMin").InnerText)
            yMin = CDbl(dcNode.SelectSingleNode("yMin").InnerText)
            xMax = CDbl(dcNode.SelectSingleNode("xMax").InnerText)
            ymax = CDbl(dcNode.SelectSingleNode("yMax").InnerText)

            prevExtent.SetBounds(xMin, yMin, 0, xMax, ymax, 0)
            syncExtent(prevExtent)

        Catch ex As Exception

        End Try

    End Sub

    Public Sub redrawMap()
        AxMap1.RemoveAllLayers()
        BackgroundInfo.filename = ""
        loadBackgroundImage()
        If Lifestage.ForeColor = Color.Gray Then
            loadCovariateGISData()
        Else
            loadHabitatGISData()
        End If

        displayFlow(curFlow)
    End Sub


    Public Sub refreshAfterLoad()
        'MsgBox("not implemented")
    End Sub

#End Region

End Class


