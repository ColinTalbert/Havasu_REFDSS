Imports System.IO
Imports System.Data.SQLite
Imports System.ComponentModel
Imports System.Threading


Public Class habGenProgressBar
    Public parentMainForm As MainForm
    Private fParser As New clsMathParser
    Private intNumThreads As Integer = 1

    Private bws As New List(Of BackgroundWorker)
    Private poolManager As New BackgroundWorker

    Public _speciesToProcess As List(Of String) = New List(Of String) _
        (New String() {"All"})
    Public _lifeStageToProcess As List(Of String) = New List(Of String) _
    (New String() {"All"})
    Public singleLS As Boolean = False

    Public Sub New(mf As MainForm)
        ' This call is required by the designer.
        InitializeComponent()
        parentMainForm = mf
        ' Add any initialization after the InitializeComponent() call.
        cboCoreCount.Items.Clear()
        For i As Integer = 1 To Environment.ProcessorCount
            cboCoreCount.Items.Add(Str(i))
        Next
        cboCoreCount.SelectedIndex = cboCoreCount.Items.Count - 2
        'intNumThreads = Int(cboCoreCount.SelectedItem)
    End Sub

    Public Sub runHabitatMaps()

        'poolManager.WorkerSupportsCancellation = True
        'AddHandler poolManager.DoWork, AddressOf poolManager_doWork
        'poolManager.RunWorkerAsync()
        poolManager_doWork()
    End Sub

    Private Sub poolManager_doWork()

        'This is the main processing subroutine.
        Dim startTime As DateTime = Now

        'Step 1:  Delete previous habitat layers
        parentMainForm.mainMapManager.clearLayers()

        'cleanDBTables()
        Dim outputDirSegments As String = My.Settings.OutputDataDirectory + "\Segments"

        'If System.IO.Directory.Exists(outputDirSegments) Then
        '    Try
        '        System.IO.Directory.Delete(outputDirSegments, True)
        '    Catch ex As Exception
        '    End Try
        'End If

        If Not System.IO.Directory.Exists(outputDirSegments) Then
            System.IO.Directory.CreateDirectory(outputDirSegments)
        End If


        'Step 2: Generate our actual habitat layers
        Dim curOutputFolder As String
        Dim toDoSpecies As String = _speciesToProcess(0)
        Dim toDoLS As String = _lifeStageToProcess(0)

        Me.pbTotalProgress.Maximum = getTodo("", "", toDoSpecies, toDoLS)

        For Each strSegment As String In parentMainForm.mainDataManager.getSegmentNames
            Me.lblSegment.Text = strSegment
            Me.pbSegmentProgress.Maximum = getTodo(strSegment, "", toDoSpecies, toDoLS)
            Me.pbSegmentProgress.Value = 0
            curOutputFolder = Path.Combine(outputDirSegments, strSegment)
            System.IO.Directory.CreateDirectory(curOutputFolder)

            For Each strTreatment As String In parentMainForm.mainDataManager.getTreatmentNames(strSegment)

                Me.lblTreatment.Text = strTreatment
                Me.pbTreatmentProgress.Maximum = getTodo(strSegment, strTreatment, toDoSpecies, toDoLS)
                Me.pbTreatmentProgress.Value = 0

                checkHabSegTable(strSegment, strTreatment)

                update_ProcessingHistory(strSegment, strTreatment, "started")

                Me.pbFlowProgress.Maximum = parentMainForm.mainDataManager.getFlows(strSegment, strTreatment).Count
                Me.pbFlowProgress.Value = 0


                Dim segFlows As List(Of String) = parentMainForm.mainDataManager.getFlows(strSegment, strTreatment)
                segFlows.Sort(Function(p1, p2) CDbl(p1) < CDbl(p2))

                'Dim dblSegFlows As List(Of Double)
                'dblSegFlows = segFlows.ConvertAll(Function(s As String) CDbl(s))
                'dblSegFlows.Sort()
                Dim lastFlow As String = "0"
                Dim lastVals As Dictionary(Of String, Double) = Nothing
                Dim rowVals As New Dictionary(Of String, Double)
                For Each strFlow As String In segFlows
                    rowVals = New Dictionary(Of String, Double)
                    rowVals = processOneFlow(strSegment, strTreatment, strFlow)

                    If lastFlow = "0" Then
                        lastVals = New Dictionary(Of String, Double)(rowVals)
                    End If
                    If rowVals.Keys.Count > 0 Then
                        addFlowRow(strSegment, strTreatment, lastFlow, strFlow, lastVals, rowVals)
                    End If
                    lastFlow = strFlow
                    lastVals = New Dictionary(Of String, Double)(rowVals)
                Next
                If rowVals.Keys.Count > 0 Then
                    addFlowRow(strSegment, strTreatment, lastFlow, "999999", rowVals, rowVals)
                End If
                update_ProcessingHistory(strSegment, strTreatment, "finished")
            Next
        Next

        parentMainForm.mainMapManager.redrawSegmentMaps()
        btnOK.Visible = True
        Label1.Visible = False
        lblFlowProgress.Visible = False
        lblSegProgress.Visible = False
        Label4.Visible = False
        Label7.Visible = False
        Label5.Visible = False
        Label6.Visible = False
        lblSegment.Visible = False
        lblTreatment.Visible = False
        lblFlow.Visible = False
        lblSpecies.Visible = False
        lblLifeStage.Visible = False
        pbSegmentProgress.Visible = False
        pbTreatmentProgress.Visible = False
        pbFlowProgress.Visible = False
        pbSpeciesProgress.Visible = False
        pbLifeStageProgress.Visible = False
        lblAllDone.Visible = True
        Dim stopTime As DateTime = Now
        Dim elapsedTime As TimeSpan = stopTime.Subtract(startTime)
        lblTimeElapsed.Text = "in " & String.Format("Elapsed Time : {0:00}:{1:00}:{2:00}", _
        CInt(elapsedTime.TotalHours), _
        CInt(elapsedTime.TotalMinutes) Mod 60, _
        CInt(elapsedTime.TotalSeconds) Mod 60)
        lblTimeElapsed.Visible = True

    End Sub

    Private Sub cleanDBTables()
        Dim strSQL As String = "SELECT name FROM sqlite_master WHERE type = 'table'"
        Dim SQLcommand As SQLiteCommand


        'SQL query to Create Table

        If parentMainForm.mainDataManager.mainSQLDBConnection.State = ConnectionState.Closed Then
            parentMainForm.mainDataManager.mainSQLDBConnection.Open()
        End If
        SQLcommand = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
        SQLcommand.CommandText = strSQL
        SQLcommand.ExecuteNonQuery()
        Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()

        Dim result As String
        While SQLreader.Read()
            result = SQLreader(0)
            If result.StartsWith("SegHab") Then
                strSQL = "DROP TABLE " + result

            End If
        End While

        parentMainForm.mainDataManager.mainSQLDBConnection.Close()
    End Sub


    'Private Sub backgroundWorker_doWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs)

    '    e.Result = processOneFlow(e.Argument(0), e.Argument(1), e.Argument(2))

    'End Sub
    'Private Sub backgroundWorker_RunWorkerCompleted(ByVal sender As System.Object, _
    '       ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)

    '    bws.Remove(sender)


    'End Sub


    Private Function processOneFlow(strSegment As String, strTreatment As String, strFlow As String) As Dictionary(Of String, Double)

        Me.lblFlow.Text = strFlow
        Dim rowVals As New Dictionary(Of String, Double)
        rowVals = New Dictionary(Of String, Double)


        Me.Label1.Text = "Reading in covariate grids for " + strSegment + " and " + strFlow
        'covariates_singleFlow.Clear()
        'loadPreCalcForScenario(strScenario, covariates_singleFlow)

        Dim flowCovariates As New Dictionary(Of String, Utilities.CovariateInfo)
        Me.Label1.Text = "Reading in input maps for:"
        Me.lblSpecies.Text = ""
        Me.lblLifeStage.Text = ""
        Application.DoEvents()



        Me.pbFlowProgress.Value = 0
        Me.Label1.Text = "Creating output maps for:"

        If Not singleLS Then
            _speciesToProcess = parentMainForm.mainDataManager.getSpeciesNames(strSegment, strTreatment)
        End If

        Dim strFileName As String = parentMainForm.mainDataManager.genCovariateFilename(strSegment, strTreatment, parentMainForm.mainDataManager.getCovariateNames(strSegment, strTreatment)(0), strFlow)
        Dim chunkIndices As List(Of chunkIndex) = Utilities.GetChunkIndices(strFileName, 5000)

        If singleLS Then
            Me.pbFlowProgress.Maximum = chunkIndices(0).chunksize * chunkIndices(0).chunksize / 10000 * chunkIndices.Count
            Me.pbSpeciesProgress.Maximum = chunkIndices(0).chunksize * chunkIndices(0).chunksize / 10000 * chunkIndices.Count
        End If

        For Each ci As chunkIndex In chunkIndices
            Dim chunk_dict As New Dictionary(Of String, Utilities.CovariateInfo)
            For Each covariate As String In parentMainForm.mainDataManager.getCovariateNames(strSegment, strTreatment)
                Me.Label1.Text = "Reading in input maps for: " & covariate
                Dim strFileName2 As String = parentMainForm.mainDataManager.genCovariateFilename(strSegment, strTreatment, covariate, strFlow)
                Dim covinfo As CovariateInfo = CovInfoFromPath(strFileName2, ci)
                covinfo.name = covariate
                If flowCovariates.ContainsKey(covariate) Then
                    flowCovariates(covariate) = covinfo
                Else
                    flowCovariates.Add(covariate, covinfo)
                End If
            Next

            For Each strSpecies As String In _speciesToProcess
                Me.lblSpecies.Text = strSpecies
                If Not singleLS Then
                    Me.pbSpeciesProgress.Maximum = parentMainForm.mainDataManager.getLifeStageNames(strSpecies).Count
                End If

                Me.pbSpeciesProgress.Value = 0
                If Not singleLS Then
                    _lifeStageToProcess = parentMainForm.mainDataManager.getLifeStageNames(strSpecies)
                End If
                For Each strLifeStage As String In _lifeStageToProcess
                    If parentMainForm.mainDataManager.checkSppsLifestageOccurance(strTreatment, _
                                            strSegment, strSpecies, strLifeStage) Then

                        Me.lblLifeStage.Text = strLifeStage
                        Dim outputFname As String = parentMainForm.mainDataManager.genOutputFname(strSegment, strTreatment, strFlow, strSpecies, strLifeStage)
                        If Not Directory.Exists(Path.GetDirectoryName(outputFname)) Then
                            Directory.CreateDirectory(Path.GetDirectoryName(outputFname))
                        End If
                        Me.pbLifeStageProgress.Value = 0
                        If Not singleLS Then
                            Me.pbFlowProgress.PerformStep()
                            Me.pbFlowProgress.PerformStep()
                            Me.pbSpeciesProgress.PerformStep()
                        End If

                        Me.pbTreatmentProgress.PerformStep()
                        Me.pbTotalProgress.PerformStep()
                        Me.pbSegmentProgress.PerformStep()
                        Application.DoEvents()

                        processOneOutput(strSpecies, strLifeStage, flowCovariates, outputFname, rowVals, ci)
                        GC.Collect()
                        Application.DoEvents()
                    End If
                Next
            Next


        Next





        'Next
        'If lastFlow = "0" Then
        '    lastVals = New Dictionary(Of String, Double)(rowVals)
        'End If
        'addFlowRow(strSegment, lastFlow, strFlow, lastVals, rowVals)
        'lastFlow = strFlow
        'lastVals = New Dictionary(Of String, Double)(rowVals)

        Return rowVals
    End Function

    Private Sub processOneOutput(ByRef strSpecies As String, ByRef strLifestage As String,
                                 ByRef covariatesDict As Dictionary(Of String, CovariateInfo),
                                 ByRef strOutfName As String, ByRef rowVals As Dictionary(Of String, Double),
                                 ByRef ci As chunkIndex)
        'get the hsc for this instance
        Dim hscVals As Dictionary(Of String, Double(,))
        hscVals = parentMainForm.mainDataManager.getHSCValues(strSpecies, strLifestage)

        'store the equation for this instance
        setFormula(strSpecies, strLifestage)

        'first reclassify all the pixels in our covariateDict to represent the current HSC
        'also store a list of which covariates are used in for this instance
        Dim usedCovariates As New List(Of String)
        For Each covariate As String In parentMainForm.mainDataManager.variableNames(strSpecies, strLifestage)
            covariatesDict(covariate).reclassifiedPixelValues = reclassifyPixelValues(hscVals(covariate), covariatesDict(covariate).pixelValues, covariatesDict(covariate).NoDataValue)
            covariatesDict(covariate).functionIndex = fParser.VarSymb(covariate)
            usedCovariates.Add(covariate)
        Next

        'generate an output from these reclassified values
        Dim fieldname As String = parentMainForm.mainDataManager.genFieldName(strSpecies, strLifestage)
        If rowVals.ContainsKey(fieldname) Then
            rowVals(fieldname) += createFinalGrid(covariatesDict, strOutfName, usedCovariates, ci)
        Else
            rowVals(fieldname) = createFinalGrid(covariatesDict, strOutfName, usedCovariates, ci)
        End If



    End Sub

    Private Function getTodo(Optional strSegment As String = "", Optional strTreatment As String = "", Optional strSpecies As String = "All", Optional strLS As String = "All") As Integer

        Dim firstSegName As String = parentMainForm.mainDataManager.getSegmentNames()(0)
        Dim firstTreatName As String = parentMainForm.mainDataManager.getTreatmentNames()(0)
        Dim firstCovariateName As String = parentMainForm.mainDataManager.getCovariateNames(firstSegName, firstTreatName)(0)
        Dim firstFlow As String = parentMainForm.mainDataManager.getFlows(firstSegName, firstTreatName)(0)
        Dim strFileName As String = parentMainForm.mainDataManager.genCovariateFilename(firstSegName,
                                                                                        firstTreatName,
                                                                                        firstCovariateName,
                                                                                        firstFlow)
        Dim chunkIndices As List(Of chunkIndex) = Utilities.GetChunkIndices(strFileName, 5000)

        Dim chunkCount As Integer = chunkIndices.Count



        Dim todo As Integer = 0

        For Each segment In parentMainForm.mainDataManager.getSegmentNames
            If strSegment = "" Or segment = strSegment Then
                For Each treatment As String In parentMainForm.mainDataManager.getTreatmentNames(segment)
                    If strTreatment = "" Or treatment = strTreatment Then
                        Dim flowcount As Integer = parentMainForm.mainDataManager.getFlows(segment, treatment).Count
                        For Each species In parentMainForm.mainDataManager.getSpeciesNames(segment, treatment)
                            If strSpecies = "All" Or strSpecies = species Then
                                For Each ls In parentMainForm.mainDataManager.getLifeStageNames(species)
                                    If strLS = "All" Or strLS = ls Then
                                        todo += flowcount * chunkCount
                                    End If
                                Next
                            End If
                        Next

                    End If
                Next
            End If
        Next
        Return todo
    End Function

    Private Function reclassifyPixelValues(ByRef hscVals(,) As Double, ByRef pixelVals() As Single, ByRef ndValue As Double) As Single()
        Dim outputpixels(pixelVals.Count - 1) As Single
        Dim i As Long = 0
        While i < pixelVals.Count

            outputpixels(i) = getPixelValue(hscVals, pixelVals(i), ndValue)
            i += 1

        End While

        Return outputpixels
    End Function

    Private Function getPixelValue(ByRef hscVals(,) As Double, ByVal pixelVal As Double, ByRef ndvalue As Double) As Single
        Dim i As Integer = 0
        If pixelVal = ndvalue Then
            Return ndvalue
        Else
            'Dim curHSC As Integer = hscVals(0, 0)
            If pixelVal <= hscVals(0, 0) Then
                Return (hscVals(1, 0))
            ElseIf pixelVal > hscVals(0, hscVals.Length / 2 - 1) Then
                Return (hscVals(1, hscVals.Length / 2 - 1))
            Else
                i = 0
                'Debug.Print(pixelVal)
                Do Until pixelVal <= hscVals(0, i)
                    i += 1
                Loop
                Return (hscVals(1, i - 1))
            End If

        End If
    End Function

    Private Function setFormula(ByVal strSpecies As String, ByVal strlifeStage As String) As Dictionary(Of String, Integer)


        Dim formula As String = parentMainForm.mainDataManager.getEquation(strSpecies, strlifeStage)
        Dim goodformula As Boolean = fParser.StoreExpression(formula)

        If Not goodformula Then

            MsgBox("There was a problem with your formula:" & vbCrLf & fParser.ErrorDescription)

        End If

    End Function

    Private Function createFinalGrid(ByRef outputCovariates2 As Dictionary(Of String, CovariateInfo),
                                ByRef outputFile As String, usedCovariates As List(Of String),
                                ByRef ci As chunkIndex) As Double


        Dim habTotal As Double = 0
        Dim pair As KeyValuePair(Of String, CovariateInfo)
        pair = outputCovariates2.First

        Dim NDValue As Double = 255 'outputCovariates2(pair.Key).NoDataValue


        Dim outputpixels(pair.Value.pixelValues.Count - 1) As Single


        Dim i As Long = 0
        Dim covariateCount As Integer = outputCovariates2.Keys.Count
        'Currently we are just multiplying each covariate together so this works
        'Dim allCovariates(outputCovariates2.Count - 1, pair.Value.Count - 1) As Single

        Dim outputPixel As Double

        Dim covIndex As Integer = 1


        Dim strOut As String = ""
        Me.pbLifeStageProgress.Maximum = pair.Value.pixelValues.Count / 10000 * ci.chunkCount

        Me.pbLifeStageProgress.Value = 0
        While i < pair.Value.pixelValues.Count
            outputPixel = -1
            'strOut = ""
            For Each covariate In usedCovariates
                If outputCovariates2(covariate).reclassifiedPixelValues(i) = outputCovariates2(covariate).NoDataValue Then
                    outputPixel = NDValue
                    Exit For
                Else
                    'strOut += "    " & p.Key & " " & p.Value.reclassifiedPixelValues(i)
                    fParser.VarValue(outputCovariates2(covariate).functionIndex) = outputCovariates2(covariate).reclassifiedPixelValues(i)
                End If
            Next

            If outputPixel <> NDValue Then
                outputPixel = fParser.Eval()
                habTotal += outputPixel
            Else
                outputPixel = NDValue
            End If

            outputpixels(i) = CInt(100 * outputPixel)
            i += 1
            If i Mod 10000 = 0 Then
                Me.pbLifeStageProgress.Value = i / 10000
                If singleLS Then
                    Me.pbFlowProgress.Value = i / 10000
                    Me.pbSpeciesProgress.Value = i / 10000
                End If
            End If
        End While

        Me.pbLifeStageProgress.Value = Me.pbLifeStageProgress.Maximum
        Application.DoEvents()
        ''option 1 
        Dim header As New MapWinGIS.GridHeader
        header.NodataValue = NDValue
        header.NumberCols = outputCovariates2(pair.Key).numCols
        header.NumberRows = outputCovariates2(pair.Key).numRows
        header.dX = outputCovariates2(pair.Key).dx
        header.dY = outputCovariates2(pair.Key).dy
        header.XllCenter = outputCovariates2(pair.Key).xllCenter
        header.YllCenter = outputCovariates2(pair.Key).yllCenter
        'header.Projection = "+proj=utm +zone=10 +datum=WGS84"

        'option 2
        ''header.CopyFrom(outputCovariates2(pair.Key).gridHeader)
        ''header = covariateMD(pair.Key)
        'If header.Projection = "" Then
        '    'Nasty hack to address the fact that mapwingis will not pull the projection info
        '    header.Projection = "+proj=utm +zone=10 +datum=WGS84 +units=m +no_defs"
        'End If



        ' covariateMD(pair.Key).Projection = "+proj=utm +zone=10 +datum=WGS84 +units=m +no_defs"
        Dim outputGrid As New MapWinGIS.Grid



        Dim addingTo As Boolean = File.Exists(outputFile)
        If Not addingTo Then
            outputGrid.CreateNew(outputFile, header, MapWinGIS.GridDataType.ByteDataType, 0)

        Else
            outputGrid.Open(outputFile)
        End If

        'outputGrid.PutFloatWindow(0, outputCovariates2(pair.Key).numRows - 1, 0, outputCovariates2(pair.Key).numCols - 1, outputpixels(0))
        outputGrid.PutFloatWindow(ci.startrow, ci.endrow, ci.startcol, ci.endcol, outputpixels(0))

        'option 3
        outputGrid.AssignNewProjection("+proj=utm +zone=11 +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs ")
        outputGrid.Save()
        outputGrid.Close()

        'Dim prjPath As String = Path.Combine(My.Settings.InputDataDirectory, "PreConstruction\template.prj")
        'System.IO.File.Copy(prjPath, outputFile.Replace(".img", ".prj"))

        Return habTotal

    End Function


#Region "DBSaveResults"
    Private Sub checkHabSegTable(segment, treatment)
        If Not HabSegTableExists(segment, treatment) Then
            'clear previous progress
            update_ProcessingHistory(segment, treatment, "undone")

            addHabSegTable(segment, treatment)
        End If

        checkHabSegTableSpecies(segment, treatment)
    End Sub
    Private Sub update_ProcessingHistory(segment As String, treatment As String, status As String)
        'Check if this row already exists
        Dim rowExists As Boolean = False
        Dim strSQLCheck As String = "select * from ProcessingHistory where segmenttreatment = '" + segment + treatment + "'"
        Dim sqlcommand As SQLite.SQLiteCommand
        sqlcommand = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
        sqlcommand.CommandText = strSQLCheck
        Dim checkData As New DataTable
        Try
            Dim sqlda As New SQLite.SQLiteDataAdapter(strSQLCheck, parentMainForm.mainDataManager.mainSQLDBConnection)
            sqlda.Fill(checkData)
        Catch ex As Exception
            'If My.Settings.verbose Then
            '    MsgBox(ex.ToString)
            'End If
        End Try

        rowExists = checkData.Rows.Count > 0



        If rowExists Then
            Dim strSQL As String = "UPDATE ProcessingHistory SET status='" + status + "', "
            strSQL += "dateprocessed='" + DateTime.Now.ToString("yyyyMMddHHmmssff") + "' "
            strSQL += " where segmenttreatment = '" + segment + treatment + "'"
            Dim SQLcommand2 As SQLiteCommand
            If parentMainForm.mainDataManager.mainSQLDBConnection.State = ConnectionState.Closed Then
                parentMainForm.mainDataManager.mainSQLDBConnection.Open()
            End If
            SQLcommand2 = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
            SQLcommand2.CommandText = strSQL
            SQLcommand2.ExecuteNonQuery()
            SQLcommand2.Dispose()
            parentMainForm.mainDataManager.mainSQLDBConnection.Close()
        Else
            Dim strSQL As String = "INSERT INTO ProcessingHistory Values('"
            strSQL += segment + treatment + "', '"
            strSQL += status + "', '"
            strSQL += DateTime.Now.ToString("yyyyMMddHHmmssff") + "')"

            Dim SQLcommand3 As SQLiteCommand
            If parentMainForm.mainDataManager.mainSQLDBConnection.State = ConnectionState.Closed Then
                parentMainForm.mainDataManager.mainSQLDBConnection.Open()
            End If
            SQLcommand3 = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
            SQLcommand3.CommandText = strSQL
            SQLcommand3.ExecuteNonQuery()
            SQLcommand3.Dispose()
            parentMainForm.mainDataManager.mainSQLDBConnection.Close()
        End If
    End Sub
    Private Function HabSegTableExists(segment, treatment) As Boolean

        ' Specify restriction to get table definition schema
        ' For reference on GetSchema see:
        ' http://msdn2.microsoft.com/en-us/library/ms254934(VS.80).aspx

        If parentMainForm.mainDataManager.mainSQLDBConnection.State = ConnectionState.Closed Then
            parentMainForm.mainDataManager.mainSQLDBConnection.Open()
        End If
        Dim strTableName As String = parentMainForm.mainDataManager.genHabLookupTableName(segment, treatment)
        Dim restrictions(3) As String
        restrictions(2) = strTableName
        'parentMainForm.mainDataManager.mainSQLDBConnection.Open()
        Dim dbTbl As DataTable = parentMainForm.mainDataManager.mainSQLDBConnection.GetSchema("Tables", restrictions)

        Dim exists As Boolean = Not dbTbl.Rows.Count = 0

        dbTbl.Dispose()
        parentMainForm.mainDataManager.mainSQLDBConnection.Close()
        Return exists

    End Function

    Private Sub checkHabSegTableSpecies(segment, treatment)
        If parentMainForm.mainDataManager.mainSQLDBConnection.State = ConnectionState.Closed Then
            parentMainForm.mainDataManager.mainSQLDBConnection.Open()
        End If
        Dim strSQL As String = ""
        Dim strTableName As String = parentMainForm.mainDataManager.genHabLookupTableName(segment, treatment)
        For Each species In parentMainForm.mainDataManager.getSpeciesNames(segment, treatment)
            For Each lifestage In parentMainForm.mainDataManager.getLifeStageNames(species)
                Try
                    strSQL = "alter TABLE " + strTableName + " add '" + parentMainForm.mainDataManager.genFieldName(species, lifestage) + "_lv'" + " DOUBLE"
                    Dim SQLcommand As SQLiteCommand
                    SQLcommand = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
                    SQLcommand.CommandText = strSQL
                    SQLcommand.ExecuteNonQuery()
                    SQLcommand.Dispose()

                    strSQL = "alter TABLE " + strTableName + " add '" + parentMainForm.mainDataManager.genFieldName(species, lifestage) + "_hv'" + " DOUBLE"
                    Dim SQLcommand2 As SQLiteCommand
                    SQLcommand2 = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
                    SQLcommand2.CommandText = strSQL
                    SQLcommand2.ExecuteNonQuery()
                    SQLcommand2.Dispose()
                Catch ex As Exception

                End Try
            Next

        Next
        parentMainForm.mainDataManager.mainSQLDBConnection.Close()
    End Sub

    Private Sub addHabSegTable(segment, treatment)
        Dim strSQL As String
        Dim strTableName As String = parentMainForm.mainDataManager.genHabLookupTableName(segment, treatment)
        ''Drop the table if it exists
        strSQL = "DROP TABlE IF EXISTS""" + strTableName + """"
        Dim SQLcommand As SQLiteCommand
        If parentMainForm.mainDataManager.mainSQLDBConnection.State = ConnectionState.Closed Then
            parentMainForm.mainDataManager.mainSQLDBConnection.Open()
        End If
        SQLcommand = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
        SQLcommand.CommandText = strSQL
        SQLcommand.ExecuteNonQuery()
        SQLcommand.Dispose()

        'Add a new table
        strSQL = "CREATE TABLE """ + strTableName + """ (""" + "LowFlow" + """ DOUBLE,""" + "HighFlow""" + " DOUBLE"
        For Each species In parentMainForm.mainDataManager.getSpeciesNames(segment, treatment)
            Debug.Print(species)
            For Each lifestage In parentMainForm.mainDataManager.getLifeStageNames(species)
                Debug.Print(lifestage)
                strSQL += ", """ + parentMainForm.mainDataManager.genFieldName(species, lifestage) + "_lv""" + " DOUBLE"
                strSQL += ", """ + parentMainForm.mainDataManager.genFieldName(species, lifestage) + "_hv""" + " DOUBLE"
            Next

        Next
        strSQL += ")"
        SQLcommand = New SQLiteCommand

        SQLcommand = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
        SQLcommand.CommandText = strSQL
        SQLcommand.ExecuteNonQuery()
        SQLcommand.Dispose()
        parentMainForm.mainDataManager.mainSQLDBConnection.Close()
    End Sub

    Private Sub addFlowRow(segment As String, treatment As String, lowFlow As Double, highflow As Double,
                           lowVals As Dictionary(Of String, Double), highVals As Dictionary(Of String, Double))
        Dim strTableName As String = parentMainForm.mainDataManager.genHabLookupTableName(segment, treatment)

        'Check if this row already exists
        Dim rowExists As Boolean = False
        Dim strSQLCheck As String = "select * from " + strTableName + " where lowFlow = " + Str(lowFlow) + " and highflow = " + Str(highflow)
        Dim sqlcommand As SQLite.SQLiteCommand
        sqlcommand = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
        sqlcommand.CommandText = strSQLCheck
        Dim checkData As New DataTable
        Try
            Dim sqlda As New SQLite.SQLiteDataAdapter(strSQLCheck, parentMainForm.mainDataManager.mainSQLDBConnection)
            sqlda.Fill(checkData)
        Catch ex As Exception
            If My.Settings.verbose Then
                MsgBox(ex.ToString)
            End If
        End Try

        rowExists = checkData.Rows.Count > 0



        If rowExists Then
            Dim strSQL As String = "UPDATE " + strTableName + " SET  "


            For Each key In lowVals.Keys
                strSQL += key + "_lv=" + CStr(lowVals(key)) + ", "
                strSQL += key + "_hv=" + CStr(highVals(key)) + ", "
            Next
            strSQL = strSQL.Substring(0, strSQL.Length - 2)
            strSQL += " WHERE lowFlow = " + CStr(lowFlow) + " and " + "highFlow = " + CStr(highflow)

            Dim SQLcommand2 As SQLiteCommand
            If parentMainForm.mainDataManager.mainSQLDBConnection.State = ConnectionState.Closed Then
                parentMainForm.mainDataManager.mainSQLDBConnection.Open()
            End If
            SQLcommand2 = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
            SQLcommand2.CommandText = strSQL
            SQLcommand2.ExecuteNonQuery()
            SQLcommand2.Dispose()
            parentMainForm.mainDataManager.mainSQLDBConnection.Close()
        Else
            Dim strSQL As String = "INSERT INTO " + strTableName
            Dim strCols As String = " (LowFlow, HighFlow"
            Dim strVals As String = " VALUES (" + CStr(lowFlow) + ", " + CStr(highflow)

            For Each key In lowVals.Keys
                strCols += ", " + key + "_lv"
                strVals += ", " + CStr(lowVals(key))
                strCols += ", " + key + "_hv"
                strVals += ", " + CStr(highVals(key))
            Next

            strCols += ") " + vbCrLf
            strVals += ")"
            strSQL += strCols + strVals

            Dim SQLcommand3 As SQLiteCommand
            If parentMainForm.mainDataManager.mainSQLDBConnection.State = ConnectionState.Closed Then
                parentMainForm.mainDataManager.mainSQLDBConnection.Open()
            End If
            SQLcommand3 = parentMainForm.mainDataManager.mainSQLDBConnection.CreateCommand
            SQLcommand3.CommandText = strSQL
            SQLcommand3.ExecuteNonQuery()
            SQLcommand3.Dispose()
            parentMainForm.mainDataManager.mainSQLDBConnection.Close()
        End If
       

    End Sub


#End Region

    Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub

End Class