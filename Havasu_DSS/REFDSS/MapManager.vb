Imports System.IO

Imports WeifenLuo.WinFormsUI.Docking

Public Class MapManager
    Private segmentMaps As New List(Of SegmentMapForm)
    Private segmentMapLegends As New List(Of SegmentMapLegendForm)
    Private overviewMaps As New List(Of OverviewMapForm)
    Public parentMainForm As MainForm

    Public Sub New(mf As MainForm)
        parentMainForm = mf
    End Sub



    Public Function addSegmentMapForm(Optional show As Boolean = True, Optional initaialRiver As String = "None")
        Dim segmentMap As New SegmentMapForm(Me)

        segmentMap.Text = "Habitat GIS Map " + CStr(segmentMaps.Count + 1)
        segmentMap.Tag = "Habitat GIS Map " + CStr(segmentMaps.Count + 1)

        If initaialRiver <> "None" Then
            segmentMap.curSegment = parentMainForm.mainDataManager.getSegmentName(initaialRiver)
            setCheckedDropdownItem(segmentMap.Segment.DropDownItems, segmentMap.curSegment)
            segmentMap.updateFlows()
            '
            segmentMap.loadBackgroundImage()
            segmentMap.loadHabitatGISData()
            'segmentMap.curFlow = segmentMap.FlowsToolStripMenuItem1.Text
            segmentMap.displayFlow(segmentMap.curFlow)
            segmentMap.AxMap1.ZoomToMaxExtents()
        End If

        segmentMaps.Add(segmentMap)

        If show Then
            segmentMap.Show(parentMainForm.MainDockPanel, DockState.Document)
        End If
        AddHandler segmentMap.AxMap1.ExtentsChanged, AddressOf segmentMap.syncExternalExtents
        Return segmentMap
    End Function

    Public Function addSegmentMapLegendForm(Optional show As Boolean = True, Optional initaialRiver As String = "None")
        Dim segmentMapLegend As New SegmentMapLegendForm(Me)

        segmentMapLegend.Tag = "GIS Map Legend " + CStr(segmentMapLegends.Count + 1)

        segmentMapLegends.Add(segmentMapLegend)

        If show Then
            segmentMapLegend.Show(parentMainForm.mainDockPanel, DockState.Document)
        End If

        Return segmentMapLegend
    End Function

    Public Sub removeSegmentMapForm(segmentMap As SegmentMapForm)
        segmentMaps.Remove(segmentMap)
        reOrderSegmentMaps()
    End Sub

    Public Sub reOrderSegmentMaps()
        Dim index As Integer = 1
        For Each segMap As SegmentMapForm In segmentMaps
            If Not segMap.bLocked Then
                segMap.Text = "Habitat GIS Map " + CStr(index)
            Else
                segMap.Text = "Habitat GIS Map " + CStr(index) + " (*)"
            End If
            index += 1
        Next
    End Sub
    Public Sub removeSegmentMapLegendForm(segmentMap As SegmentMapLegendForm)
        segmentMapLegends.Remove(segmentMap)
        reOrderSegmentMapLegends()
    End Sub

    Public Sub reOrderSegmentMapLegends()
        Dim index As Integer = 1
        For Each segMap As SegmentMapLegendForm In segmentMapLegends
            If Not segMap.bLocked Then
                segMap.Tag = "GIS Map Legend " + CStr(index)
            End If
            index += 1
        Next
    End Sub
    Public Function addOverviewForm(Optional show As Boolean = True)
        Dim OverviewMap As New OverviewMapForm(Me)
        overviewMaps.Add(OverviewMap)

        If show Then
            OverviewMap.Show(parentMainForm.mainDockPanel, DockState.DockLeft)
        End If

        Return OverviewMap
    End Function

    Public Sub removeOverviewMapForm(overviewMap As OverviewMapForm)
        overviewMaps.Remove(overviewMap)
    End Sub

    Public Sub syncExtents(segForm As SegmentMapForm, extents As MapWinGIS.Extents)

        For Each mapSegmentForm As SegmentMapForm In segmentMaps
            If segForm.curSegment = mapSegmentForm.curSegment And
                Not segForm.Equals(mapSegmentForm) And
                Not mapSegmentForm.bLocked Then
                mapSegmentForm.syncExtent(extents)
            End If
        Next

    End Sub

    Public Sub syncFlow(segment As String, scenario As String, flow As List(Of String))
        For Each mapSegmentForm As SegmentMapForm In segmentMaps
            If segment = mapSegmentForm.curSegment And
                scenario = mapSegmentForm.curScenario And
                Not mapSegmentForm.bLocked Then
                mapSegmentForm.displayFlow(flow)
            End If
        Next


    End Sub

    Public Sub UpdateCovariateSymbology()
        For Each mapSegmentForm As SegmentMapForm In segmentMaps
            If Not mapSegmentForm.bLocked Then
                mapSegmentForm.updateCovariateSymbology()
            End If
        Next
    End Sub


    Public Sub clearLayers()
        For Each mapSegmentForm As SegmentMapForm In segmentMaps
            mapSegmentForm.AxMap1.RemoveAllLayers()
        Next
    End Sub

    Public Sub redrawSegmentMaps()
        For Each mapSegmentForm As SegmentMapForm In segmentMaps
            mapSegmentForm.redrawMap()
        Next
    End Sub

    Public Sub updateSegmentTitles()
        For Each mapSegmentForm As SegmentMapForm In segmentMaps
            mapSegmentForm.updateTitle()
        Next
    End Sub

    Public Sub ZoomToPrev()
        For Each mapSegmentForm As SegmentMapForm In segmentMaps
            If Not mapSegmentForm.bLocked Then
                mapSegmentForm.AxMap1.ZoomToPrev()
                mapSegmentForm.AxMap1.Update()
            End If
        Next
    End Sub

    Public Sub ZoomFull()
        For Each mapSegmentForm As SegmentMapForm In segmentMaps
            If Not mapSegmentForm.bLocked Then
                mapSegmentForm.AxMap1.ZoomToMaxExtents()
                mapSegmentForm.AxMap1.Update()
            End If
        Next
    End Sub

    Public Sub ChangeCursor(cursor As MapWinGIS.tkCursorMode, sendMove As Boolean)
        For Each mapSegmentForm As SegmentMapForm In segmentMaps
            mapSegmentForm.AxMap1.CursorMode = cursor
            mapSegmentForm.AxMap1.SendMouseMove = sendMove
        Next
        For Each mapOverviewForm As OverviewMapForm In overviewMaps
            mapOverviewForm.AxMap1.CursorMode = cursor
            mapOverviewForm.AxMap1.SendMouseMove = sendMove
        Next
    End Sub

    Public Sub ChangeLableVisible(visible As Boolean)
        For Each mapSegmentForm As SegmentMapForm In segmentMaps
            If Not mapSegmentForm.bLocked Then
                mapSegmentForm.lblTitle.Visible = visible
            End If
        Next
    End Sub

    Public Function getPixelValuesMsg(projx As Double, projy As Double, strSegment As String, strTreatment As String, flow As String,
                                      Optional strSpecies As String = "", Optional strLifeStage As String = "")

        Dim msg As String = ""

        For Each strCovariate As String In parentMainForm.mainDataManager.getCovariateNames(strSegment, strTreatment)

            Dim strFileName As String
            If parentMainForm.mainMapManager.parentMainForm.mainDataManager.isCovariateSingleFlow(strCovariate) Then
                strFileName = getCovariateFileName("NA", strCovariate, strTreatment)
            Else
                strFileName = getCovariateFileName(flow, strCovariate, strTreatment)
            End If

            If parentMainForm.mainDataManager.isCovariateCategorical(strCovariate) Then
                msg += strCovariate + ":  " + parentMainForm.mainDataManager.getCategoricalValueLabel(strCovariate, Str(CDbl(getPixelValue(projx, projy, strFileName)))) + vbCrLf
            Else
                Dim tmpPixValue As Double = CDbl(getPixelValue(projx, projy, strFileName))
                If Not parentMainForm.mainDataManager.curUnits = "Metric" Then
                    tmpPixValue *= parentMainForm.mainDataManager.variableConversionFactor(strCovariate)
                End If

                Dim covariateLabel As String = strCovariate
                If strCovariate = "Depth" And tmpPixValue < 0 Then
                    tmpPixValue *= -1
                    covariateLabel = "Elevation above water surface"
                End If

                Dim stringFormat As String
                If tmpPixValue > -3 And tmpPixValue < 3 Then
                    stringFormat = "N2"
                ElseIf tmpPixValue > -10 And tmpPixValue < 10 Then
                    stringFormat = "N1"
                Else
                    stringFormat = "N0"
                End If

                msg += covariateLabel + ":  " + tmpPixValue.ToString(stringFormat) + " " + parentMainForm.mainDataManager.variableUnitsLabel(strCovariate, parentMainForm.mainDataManager.curUnits) + vbCrLf
            End If
        Next


        getPixelValuesMsg = msg
    End Function


    Public Function getPixelValue(projx As Double, projy As Double, strFileName As String)
        Dim g As New MapWinGIS.Grid
        g.Open(strFileName)
        Dim r, c As Integer
        g.ProjToCell(projx, projy, c, r)

        Dim v As Double = g.Value(c, r)
        g.Close()
        Return v
    End Function

    Public Function getCovariateFileName(flow, strCovariate, strTreatment)

        Dim d As String = ""
        Dim f As String = ""

        If flow = "NA" Then
            d = Path.Combine(My.Settings.InputDataDirectory, "Segments", strCovariate + ".tif")
        Else
            d = Path.Combine(My.Settings.InputDataDirectory, "Segments", strCovariate + "_" + flow + ".tif")
        End If

        'f = mainMapManager.parentMainForm.mainDataManager.getSegmentAbbrev(curSegment) + "_"
        'If flow <> "NA" Then
        '    f += Replace(flow, ".", "_") + "_"
        'End If

        'f += mainMapManager.parentMainForm.mainDataManager.getCovariateAbbrev(curCovariate)
        'd += mainMapManager.parentMainForm.mainDataManager.getTreatmentAbbrev(curSegment, curTreatment)
        'f += ".tif"

        Return d


    End Function
End Class
