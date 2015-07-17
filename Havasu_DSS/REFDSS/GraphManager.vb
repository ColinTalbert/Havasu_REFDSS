Imports WeifenLuo.WinFormsUI.Docking
Imports System.Windows.Forms.DataVisualization.Charting

Public Class GraphManager
    Private habitatGraphs As New List(Of HabitatGraphForm)
    Private AggHabitatGraphs As New List(Of aggHabitatGraphForm)
    Private flowVsHabitatGraphs As New List(Of FlowVsHabitatChartForm)
    Private SystemWideMetrics As New List(Of SystemWideMetricForm)
    Public mainDockPanel As DockPanel
    Public mainDataManager As DataManager
    Public mainHydrographManager As HydrographManager
    Private myMainForm As MainForm

    Public bShowLabels As Boolean = False
    Public globalColorDialog As ColorDialog

    Public Sub New(mdp As DockPanel, mdm As DataManager, mf As MainForm)
        mainDockPanel = mdp
        mainDataManager = mdm
        myMainForm = mf
        Me.globalColorDialog = mf.globalColorDialog
    End Sub


    Public Function addSystemWideGraphForm(Optional show As Boolean = True)
        Dim SystemWideMetric As New SystemWideMetricForm(Me, mainDataManager)
        SystemWideMetric.Text = "System Wide Metrics " + CStr(SystemWideMetrics.Count + 1)
        SystemWideMetric.SystemWideChart.Tag = "System Wide Metrics " + CStr(SystemWideMetrics.Count + 1)
        SystemWideMetrics.Add(SystemWideMetric)
        If show Then
            SystemWideMetric.Show(mainDockPanel, DockState.DockRight)
            MainForm.mainDataManager.applyDefaultChartSymbology("SystemWideMetrics", SystemWideMetric.SystemWideChart)
            SystemWideMetric.Show(mainDockPanel, DockState.DockRight)
            SystemWideMetric.loaddata()
        End If

        Return (SystemWideMetric)

    End Function

    Public Sub removeSystemWideGraph(SystemWideMetric As SystemWideMetricForm)
        SystemWideMetrics.Remove(SystemWideMetric)
        reOrderSystemWide()
    End Sub

    Public Sub reOrderSystemWide()
        Dim index As Integer = 1
        For Each systemWide As SystemWideMetricForm In SystemWideMetrics
            If Not systemWide.bLocked Then
                systemWide.Text = "System Wide Metrics " + CStr(index)
            Else
                systemWide.Text = "System Wide Metrics " + CStr(index) + " (*)"
            End If
            index += 1
        Next
    End Sub

    Public Function addflowVsHabitatGraphForm(Optional show As Boolean = True)
        Dim flowVsHabitatGraph As New FlowVsHabitatChartForm(Me, mainDataManager)
        flowVsHabitatGraph.Text = "Flow vs Habitat " + CStr(flowVsHabitatGraphs.Count + 1)
        flowVsHabitatGraph.HabitatChart.Tag = "Flow vs Habitat " + CStr(flowVsHabitatGraphs.Count + 1)
        flowVsHabitatGraphs.Add(flowVsHabitatGraph)
        If show Then
            flowVsHabitatGraph.curDisplayData = MainForm.mainDataManager.getDefaultDisplayData("FlowVsHabitatChartForm")
            MainForm.mainDataManager.applyDefaultChartSymbology("FlowVsHabitatChartForm", flowVsHabitatGraph.HabitatChart)
            flowVsHabitatGraph.Show(mainDockPanel, DockState.DockRight)
            flowVsHabitatGraph.loadData()
        End If
        Return flowVsHabitatGraph
    End Function

    Public Sub removeFlowVsHabitatChartForm(flowVsHabitat As FlowVsHabitatChartForm)
        flowVsHabitatGraphs.Remove(flowVsHabitat)
        reOrderHabitat()
    End Sub

    Public Sub reOrderFlowVsHabitat()
        Dim index As Integer = 1
        For Each flowVsHabitatGraph As FlowVsHabitatChartForm In flowVsHabitatGraphs
            If Not flowVsHabitatGraph.bLocked Then
                flowVsHabitatGraph.Text = "Habitat Area " + CStr(index)
            Else
                flowVsHabitatGraph.Text = "Habitat Area " + CStr(index) + " (*)"
            End If
            index += 1
        Next
    End Sub

    Public Function addHabitatGraphForm(Optional show As Boolean = True)
        Dim HabitatMetricGraph As New HabitatGraphForm(Me, mainDataManager)
        HabitatMetricGraph.Text = "Habitat Area " + CStr(habitatGraphs.Count + 1)
        HabitatMetricGraph.HabitatChart.Tag = "Habitat Area " + CStr(habitatGraphs.Count + 1)
        habitatGraphs.Add(HabitatMetricGraph)
        If show Then
            HabitatMetricGraph.curDisplayData = MainForm.mainDataManager.getDefaultDisplayData("HabitatGraph")
            HabitatMetricGraph.curDisplayData.startYear = mainDataManager.curStartYear
            HabitatMetricGraph.curDisplayData.endYear = mainDataManager.curEndYear
            MainForm.mainDataManager.applyDefaultChartSymbology("HabitatGraph", HabitatMetricGraph.HabitatChart)
            HabitatMetricGraph.Show(mainDockPanel, DockState.DockRight)
            HabitatMetricGraph.loadData()
        End If
        Return HabitatMetricGraph
    End Function

    Public Sub removeHabitatGraphForm(HabitatGraph As HabitatGraphForm)
        habitatGraphs.Remove(HabitatGraph)
        reOrderHabitat()
    End Sub

    Public Sub reOrderHabitat()
        Dim index As Integer = 1
        For Each systemWide As HabitatGraphForm In habitatGraphs
            If Not systemWide.bLocked Then
                systemWide.Text = "Habitat Area " + CStr(index)
            Else
                systemWide.Text = "Habitat Area " + CStr(index) + " (*)"
            End If
            index += 1
        Next
    End Sub

    Public Function addAggHabitatGraphForm(Optional show As Boolean = True)
        Dim AggHabitatMetricGraph As New aggHabitatGraphForm(Me, mainDataManager)
        AggHabitatMetricGraph.Text = "Aggregated Habitat Area " + CStr(AggHabitatGraphs.Count + 1)
        AggHabitatMetricGraph.HabitatChart.Tag = "Aggregated Habitat Area " + CStr(AggHabitatGraphs.Count + 1)
        AggHabitatGraphs.Add(AggHabitatMetricGraph)
        If show Then
            AggHabitatMetricGraph.curDisplayData = MainForm.mainDataManager.getDefaultDisplayData("AggHabitat")
            AggHabitatMetricGraph.curDisplayData.startYear = mainDataManager.curStartYear
            AggHabitatMetricGraph.curDisplayData.endYear = mainDataManager.curEndYear
            MainForm.mainDataManager.applyDefaultChartSymbology("AggHabitat", AggHabitatMetricGraph.HabitatChart)
            AggHabitatMetricGraph.Show(mainDockPanel, DockState.DockRight)
            AggHabitatMetricGraph.loadData()

        End If
        Return AggHabitatMetricGraph
    End Function

    Public Sub removeAggHabitatGraphForm(AggHabitatGraph As AggHabitatGraphForm)
        AggHabitatGraphs.Remove(AggHabitatGraph)
        reOrderAgg()
    End Sub

    Public Sub reOrderAgg()
        Dim index As Integer = 1
        For Each systemWide As aggHabitatGraphForm In AggHabitatGraphs
            If Not systemWide.bLocked Then
                systemWide.Text = "Aggregated Habitat Area " + CStr(index)
            Else
                systemWide.Text = "Aggregated Habitat Area " + CStr(index) + " (*)"
            End If
            index += 1
        Next
    End Sub

    Public Function getAllCharts(Optional getSystemWide As Boolean = True, Optional getHabitat As Boolean = True) As List(Of Chart)

        Dim returnCharts As New List(Of Chart)

        If getSystemWide Then
            For Each swm As SystemWideMetricForm In SystemWideMetrics
                returnCharts.Add(swm.SystemWideChart)
            Next
        End If

        If getHabitat Then
            For Each habChart As HabitatGraphForm In habitatGraphs
                returnCharts.Add(habChart.HabitatChart)
            Next
        End If

        Return returnCharts
    End Function


    Public Sub addStripLInes(startDay As Date, endDay As Date, title As String)
        mainHydrographManager.addStripLines(startDay, endDay, title)
    End Sub

    Public Sub syncExtents(ByRef chrt As Chart, oneWay As Boolean)

        If oneWay Then
            mainHydrographManager.syncExtents(chrt, False)
        End If

        For Each graphForm As HabitatGraphForm In habitatGraphs
            If Not graphForm.bLocked Then
                graphForm.syncExtent(chrt)
            End If
        Next



    End Sub

    Public Sub updateLoadData()
        For Each graphForm As HabitatGraphForm In habitatGraphs
            If Not graphForm.bLocked Then
                graphForm.curDisplayData.startYear = mainDataManager.curStartYear
                graphForm.curDisplayData.endYear = mainDataManager.curEndYear
                graphForm.loadData()
            End If
        Next

        For Each graphForm As aggHabitatGraphForm In AggHabitatGraphs
            If Not graphForm.bLocked Then
                graphForm.curDisplayData.startYear = mainDataManager.curStartYear
                graphForm.curDisplayData.endYear = mainDataManager.curEndYear
                graphForm.loadData()
            End If
        Next

        For Each graphForm As SystemWideMetricForm In SystemWideMetrics
            If Not graphForm.bLocked Then
                graphForm.loadData()
            End If
        Next

        For Each graphform As FlowVsHabitatChartForm In flowVsHabitatGraphs
            If Not graphform.bLocked Then
                graphform.loadData()
            End If
        Next

    End Sub

End Class
