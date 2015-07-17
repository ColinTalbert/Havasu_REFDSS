Imports WeifenLuo.WinFormsUI.Docking
Imports System.Windows.Forms.DataVisualization.Charting

Public Class HydrographManager

    Private hydrographForms As New List(Of HydrographGraphForm)
    Public mainDataManager As DataManager
    Public parentMainForm As MainForm
    Public globalColorDialog As ColorDialog


    Public Sub New(mf As MainForm)
        parentMainForm = mf
        mainDataManager = mf.mainDataManager
        Me.globalColorDialog = mf.globalColorDialog
    End Sub

    Public Function addHydrograph(Optional show As Boolean = True)
        Dim hydrograph As New HydrographGraphForm(Me)
        hydrograph.Text = "Hydrograph " + CStr(hydrographForms.Count + 1)
        hydrograph.hydrographChart.Tag = hydrograph.Text
        hydrographForms.Add(hydrograph)
        If show Then
            'set this hydrograph to the default symbology and data
            hydrograph.curDisplayData = MainForm.mainDataManager.getDefaultDisplayData("Hydrograph")
            hydrograph.curDisplayData.startYear = mainDataManager.curStartYear
            hydrograph.curDisplayData.endYear = mainDataManager.curEndYear
            MainForm.mainDataManager.applyDefaultChartSymbology("Hydrograph", hydrograph.hydrographChart)
            hydrograph.Show(parentMainForm.mainDockPanel, DockState.DockBottom)
            hydrograph.loadData()
        End If

        Return hydrograph
    End Function

    Public Sub removeHydrograph(hydrographForm As HydrographGraphForm)
        hydrographForms.Remove(hydrographForm)
        reOrderHydrographs()
    End Sub

    Public Sub reOrderHydrographs()
        Dim index As Integer = 1
        For Each hydroForm As HydrographGraphForm In hydrographForms
            If Not hydroForm.bLocked Then
                hydroForm.Text = "Hydrograph " + CStr(index)
            Else
                hydroForm.Text = "Hydrograph " + CStr(index) + " (*)"
            End If
            index += 1
        Next


    End Sub


    Public Sub syncExtents(ByRef chrt As Chart, oneWay As Boolean)

        If oneWay Then
            parentMainForm.mainGraphManager.syncExtents(chrt, False)
        End If

        For Each hydrographForm As HydrographGraphForm In hydrographForms
            If Not hydrographForm.bLocked Then
                hydrographForm.syncExtent(chrt)
            End If
        Next


    End Sub

    Public Sub syncGraphExtents(chrt As Chart)

        parentMainForm.mainGraphManager.syncExtents(chrt, True)

    End Sub

    Public Sub updateLoadData()
        For Each hydrographForm As HydrographGraphForm In hydrographForms
            If Not hydrographForm.bLocked Then
                hydrographForm.curDisplayData.startYear = mainDataManager.curStartYear
                hydrographForm.curDisplayData.endYear = mainDataManager.curEndYear
                hydrographForm.loadData()
            End If
        Next
    End Sub

    Public Sub syncCursor(position As Integer)

        For Each hydrographForm As HydrographGraphForm In hydrographForms
            If Not hydrographForm.bLocked Then
                hydrographForm.SyncCursor(position)
            End If
        Next

        'mainGraphmanager.syncExtents(e)

    End Sub

    Public Sub addStripLines(startDay As Date, endDay As Date, title As String)
        For Each hydrographForm As HydrographGraphForm In hydrographForms
            If Not hydrographForm.bLocked Then
                hydrographForm.addStripLines(startDay, endDay, title)
            End If
        Next
    End Sub

    Public Sub updateLogrithmic(isLog As Boolean)
        For Each hydrographForm As HydrographGraphForm In hydrographForms
            If Not hydrographForm.bLocked Then
                hydrographForm.hydrographChart.ChartAreas(0).AxisY.IsLogarithmic = isLog
                hydrographForm.hydrographChart.ChartAreas(0).AxisY2.IsLogarithmic = isLog
                'hydrographForm.hydrographChart.ChartAreas(0).RecalculateAxesScale()
            End If
        Next
    End Sub

    Public Sub updateCursor(cursorType As String)
        For Each hydrographForm As HydrographGraphForm In hydrographForms
            hydrographForm.updateCursor(cursorType)
        Next
    End Sub

    Public Sub zoomfull()
        For Each hydrographForm As HydrographGraphForm In hydrographForms
            If Not hydrographForm.bLocked Then
                hydrographForm.hydrographChart.ChartAreas(0).AxisX.ScaleView.ZoomReset(1000)
                hydrographForm.hydrographChart.ChartAreas(0).AxisY.ScaleView.ZoomReset(1000)
                hydrographForm.hydrographChart.ChartAreas(0).RecalculateAxesScale()
            End If
        Next
    End Sub

    Public Sub incrementCursors()

        Dim lastHF As HydrographGraphForm = Nothing

        For Each hydrographForm As HydrographGraphForm In hydrographForms
            If Not hydrographForm.bLocked Then
                hydrographForm.incrementCursor()
                lastHF = hydrographForm
            End If
        Next

        parentMainForm.mainGraphManager.syncExtents(lastHF.hydrographChart, True)


    End Sub

    Public Function getAllCharts() As List(Of Chart)

        Dim returnCharts As New List(Of Chart)




        Return returnCharts
    End Function

    'Public Sub addSegment(strSegment As String)
    '    For Each hydro As HydrographGraphForm In hydrographForms
    '        For Each item As ToolStripMenuItem In hydro.Section.DropDownItems
    '            If item.Text = strSegment Then
    '                item.CheckState = System.Windows.Forms.CheckState.Checked
    '            End If
    '        Next



    '        'Dim item As ToolStripMenuItem = hydro.SegmentToolStripMenuItem1.DropDownItems(strSegment)
    '        'setCheckedDropdownItem(hydro.SegmentToolStripMenuItem1.DropDownItems, strSegment)
    '            hydro.loadData()
    '    Next
    'End Sub

End Class


