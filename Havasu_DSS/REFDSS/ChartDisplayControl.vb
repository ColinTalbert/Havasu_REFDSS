Imports System.Windows.Forms.DataVisualization.Charting

Public Class ChartDisplayControl

    Private childChart As Chart
    Private mainDataManager As DataManager
    Private chartName As String
    Private chartType As String
    Private AxesNames As New List(Of String)(New String() {"X", "Y", "Y2"})
    'Private AxesNames As New List(Of String)(New String() {"Y"})
    Private AxesTypes As New List(Of String)(New String() {"Major", "Minor"})
#Region "Top level functions"
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        For Each axesName As String In AxesNames
            For Each AxesType As String In AxesTypes
                Dim cntrl As ComboBox = getControlByName(axesName & AxesType & "GridLinesStyle")
                cntrl.Items.Clear()
                cntrl.Items.Add(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid)
                cntrl.Items.Add(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash)
                cntrl.Items.Add(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDot)
                cntrl.Items.Add(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot)
                cntrl.Items.Add(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot)

            Next
        Next

        addHandlers()


    End Sub

    Public Sub setChart(chrt As Chart, cName As String, mdm As DataManager, chrtType As String)
        childChart = chrt
        chartName = cName
        mainDataManager = mdm
        chartType = chrtType
        LoadChart()
    End Sub

    Private Sub LoadChart()
        'populate the gui with all of the current chart parameters
        If childChart.Legends(0).Docking = Docking.Bottom Then
            legendBottom.Checked = True
        ElseIf childChart.Legends(0).Docking = Docking.Top Then
            legendTop.Checked = True
        ElseIf childChart.Legends(0).Docking = Docking.Right Then
            legendRight.Checked = True
        ElseIf childChart.Legends(0).Docking = Docking.Left Then
            legendLeft.Checked = True
        End If
        If childChart.Legends(0).IsDockedInsideChartArea Then
            chkDockLegendInside.Checked = True
        End If

        If childChart.Titles.Count = 0 Then
            TitleNone.Checked = True
        Else
            txtTitle.Text = childChart.Titles(0).Text
            If childChart.Titles(0).Docking = Docking.Bottom Then
                titleBottom.Checked = True
            ElseIf childChart.Titles(0).Docking = Docking.Top Then
                titleTop.Checked = True
            ElseIf childChart.Titles(0).Docking = Docking.Right Then
                titleRight.Checked = True
            ElseIf childChart.Titles(0).Docking = Docking.Left Then
                titleLeft.Checked = True
            End If

            If childChart.Titles(0).IsDockedInsideChartArea Then
                chkDockTitelInside.Checked = True
            End If
            If Not childChart.Titles(0).Visible Then
                TitleNone.Checked = True
            End If
        End If

        For Each AxisName As String In AxesNames
            loadAxis(AxisName)
        Next


    End Sub

    Private Sub addHandlers()
        For Each AxisName As String In AxesNames
            AddHandler CType(getControlByName(AxisName & "AutoScale"), CheckBox).CheckedChanged, AddressOf AutoScale_CheckedChanged
            AddHandler CType(getControlByName("chkShow" & AxisName & "Title"), CheckBox).CheckedChanged, AddressOf showTitleCheckedChanged
            AddHandler CType(getControlByName(AxisName & "MinorInterval"), TextBox).Validated, AddressOf MinorInterval_TextChanged
            AddHandler CType(getControlByName(AxisName & "MinorTickMarksLength"), TextBox).Validated, AddressOf TickMarksLength_TextChanged
            For Each AxisType As String In AxesTypes
                AddHandler CType(getControlByName(AxisName & "Show" & AxisType & "GridLines"), CheckBox).CheckedChanged, AddressOf GroupCheckedChanged
                AddHandler CType(getControlByName(AxisName & "Show" & AxisType & "TickMarks"), CheckBox).CheckedChanged, AddressOf GroupCheckedChanged
                AddHandler CType(getControlByName(AxisName & AxisType & "GridLinesStyle"), ComboBox).SelectedIndexChanged, AddressOf GridLinesStyle_SelectedIndexChanged
                AddHandler CType(getControlByName(AxisName & "Show" & AxisType & "GridLines"), CheckBox).CheckedChanged, AddressOf GroupCheckedChanged
                AddHandler CType(getControlByName(AxisName & AxisType & "GridLinesColor"), Button).Click, AddressOf Color_Click
                AddHandler CType(getControlByName(AxisName & AxisType & "TickMarksColor"), Button).Click, AddressOf Color_Click

                AddHandler CType(getControlByName(AxisName & AxisType & "GridLinesWidth"), TextBox).Validated, AddressOf Width_TextChanged
                AddHandler CType(getControlByName(AxisName & AxisType & "TickMarksWidth"), TextBox).Validated, AddressOf Width_TextChanged

                AddHandler CType(getControlByName(AxisName & AxisType & "TickMarksLength"), TextBox).Validated, AddressOf TickMarksLength_TextChanged
            Next
        Next

    End Sub

    Private Sub loadAxis(strWhich As String)
        'Axes
        Dim ax As Axis = getAxisByName(strWhich)

        getControlByName("txt" & strWhich & "Title").Text = ax.Title
        getControlByName("chkShow" & strWhich & "Title").Checked = Not ax.Title = ""
        getControlByName("txt" & strWhich & "Title").Enabled = Not getControlByName("txt" & strWhich & "Title").Enabled
        getControlByName("txt" & strWhich & "Title").Enabled = getControlByName("chkShow" & strWhich & "Title").Checked

        getControlByName(strWhich & "Max").Text = ax.Maximum
        getControlByName(strWhich & "Min").Text = ax.Minimum
        getControlByName(strWhich & "Interval").Text = ax.MajorTickMark.Interval

        getControlByName(strWhich & "LabelsFormat").Text = ax.LabelStyle.Format

        If getControlByName(strWhich & "Interval").Text = "0" Then
            getControlByName(strWhich & "Interval").Text = CStr((CDbl(getControlByName(strWhich & "Max").Text) - CDbl(getControlByName(strWhich & "Min").Text)) / 7)
        End If

        'And finally gridLines
        For Each AxesType As String In AxesTypes
            Dim thisGrid As Grid = getGridByName(strWhich, AxesType)
            getControlByName(strWhich & "Show" & AxesType & "GridLines").checked = thisGrid.Enabled
            getControlByName(strWhich & AxesType & "GridLinesColor").backcolor = thisGrid.LineColor
            getControlByName(strWhich & AxesType & "GridLinesWidth").text = thisGrid.LineWidth
            Dim cntrl As ComboBox = getControlByName(strWhich & AxesType & "GridLinesStyle")
            cntrl.SelectedItem = getGridByName(strWhich, AxesType).LineDashStyle
        Next

        getControlByName(strWhich & "ShowMajorTickMarks").checked = ax.MajorTickMark.Enabled
        getControlByName(strWhich & "ShowMinorTickMarks").checked = ax.MinorTickMark.Enabled
        getControlByName(strWhich & "MajorTickMarksLength").text = ax.MajorTickMark.Size
        getControlByName(strWhich & "MinorTickMarksLength").text = ax.MinorTickMark.Size

        getControlByName(strWhich & "MajorTickMarksColor").backcolor = ax.MajorTickMark.LineColor
        getControlByName(strWhich & "MinorTickMarksColor").backcolor = ax.MajorTickMark.LineColor

        getControlByName(strWhich & "MinorInterval").text = ax.MajorTickMark.Interval / 5
    End Sub

    Private Function getAxisByName(strWhich) As Axis
        Dim axName = DirectCast([Enum].Parse(GetType(AxisName), strWhich), AxisName)
        Return Array.Find(childChart.ChartAreas(0).Axes, Function(s) s.AxisName = axName)

    End Function

    Private Function getGridByName(strAxes As String, strGridType As String) As Grid
        Dim ax As Axis = getAxisByName(strAxes)
        If strGridType.ToLower().Contains("major") Then
            Return ax.MajorGrid
        ElseIf strGridType.ToLower().Contains("minor") Then
            Return ax.MinorGrid
        End If
    End Function

    Private Function getTickMarksByName(strAxes As String, strTickMarkType As String) As TickMark
        Dim ax As Axis = getAxisByName(strAxes)
        If strTickMarkType.ToLower().Contains("major") Then
            Return ax.MajorTickMark
        ElseIf strTickMarkType.ToLower().Contains("minor") Then
            Return ax.MinorTickMark
        End If
    End Function

    Private Function getControlByName(strName As String)
        Return FindControlRecursive(Me, strName)

    End Function

    Public Shared Function FindControlRecursive(ByVal parent As Control, strName As String) As Control

        For Each child As Control In parent.Controls
            If child.Name.ToLower = strName.ToLower Then
                Return child
            Else
                Dim rtn As Control = FindControlRecursive(child, strName)
                If TypeOf (rtn) Is Control Then
                    Return rtn
                End If
            End If
        Next
        Return Nothing
    End Function

    Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub
#End Region


#Region "Legend"
    Private Sub LegendPlacementChanged(sender As System.Object, e As System.EventArgs) Handles legendTop.CheckedChanged, legendRight.CheckedChanged, legendLeft.CheckedChanged, legendBottom.CheckedChanged

        childChart.Legends(0).Enabled = True
        If legendBottom.Checked = True Then
            childChart.Legends(0).Docking = Docking.Bottom
        ElseIf legendTop.Checked = True Then
            childChart.Legends(0).Docking = Docking.Top
        ElseIf legendRight.Checked = True Then
            childChart.Legends(0).Docking = Docking.Right
        ElseIf legendLeft.Checked = True Then
            childChart.Legends(0).Docking = Docking.Left
        ElseIf legendNone.Checked Then
            childChart.Legends(0).Enabled = False
        End If
    End Sub

    Private Sub btnChangeFont_Click(sender As System.Object, e As System.EventArgs) Handles btnChangeFont.Click
        getNewFont(Me.childChart.Legends(0))
    End Sub
#End Region

#Region "Axes"
    Private Sub btnChangeAxisTitleFont_Click(sender As System.Object, e As System.EventArgs) Handles btnChangeXAxisTitleFont.Click, btnChangeYAxisTitleFont.Click, btnChangeY2AxisTitleFont.Click
        Dim ax As Axis = getAxisByName(sender.tag)
        getNewFont(ax)
    End Sub

    Private Sub AutoScale_CheckedChanged(sender As System.Object, e As System.EventArgs)

        updateAxis(sender.tag, sender.checked)

    End Sub

    Private Sub updateAxis(strTag As String, auto As Boolean)
        getControlByName(strTag & "Min").Enabled = Not auto
        getControlByName(strTag & "Max").Enabled = Not auto
        getControlByName(strTag & "Interval").Enabled = Not auto
        getControlByName(strTag & "minlbl").Enabled = Not auto
        getControlByName(strTag & "maxlbl").Enabled = Not auto
        getControlByName(strTag & "Intervallbl").Enabled = Not auto

        Dim ax As Axis = getAxisByName(strTag)
        If auto Then
            ax.Maximum = [Double].NaN
            ax.Minimum = [Double].NaN
            ax.Interval = [Double].NaN
            childChart.ChartAreas(0).RecalculateAxesScale()
        Else
            ax.Maximum = getControlByName(strTag & "Max").Text
            ax.Minimum = getControlByName(strTag & "Min").Text
            ax.Interval = getControlByName(strTag & "Interval").Text
        End If
    End Sub

    Private Sub manualAxesBreaks_Validated(sender As System.Object, e As System.EventArgs) Handles YMax.Validated, YMin.Validated, YInterval.Validated, XMax.Validated, XMin.Validated, XInterval.Validated, Y2Max.Validated, Y2Min.Validated, Y2Interval.Validated
        updateAxis(sender.tag, False)
    End Sub

    Private Sub btnChangeAxisLabelsFont_Click(sender As System.Object, e As System.EventArgs) Handles btnChangeXAxisLabelsFont.Click, btnChangeYAxisLabelsFont.Click, btnChangeY2AxisLabelsFont.Click
        Dim ax As Axis = getAxisByName(sender.tag)
        getNewFont(ax.LabelStyle)
    End Sub

    Private Sub txtYTitle_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtXTitle.TextChanged, txtYTitle.TextChanged, txtY2Title.TextChanged
        Dim ax As Axis = getAxisByName(sender.tag)
        ax.Title = getControlByName("txt" & sender.tag & "Title").Text
    End Sub

    Private Sub showTitleCheckedChanged(sender As System.Object, e As System.EventArgs)
        Dim ax As Axis = getAxisByName(sender.tag)
        getControlByName("txt" & sender.tag & "Title").Enabled = sender.Checked
        If Not sender.Checked Then
            ax.Title = ""
        Else
            ax.Title = getControlByName("txt" & sender.tag & "Title").Text
        End If
    End Sub

    Private Sub YLog_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles YLog.CheckedChanged, XLog.CheckedChanged, Y2Log.CheckedChanged
        Dim ax As Axis = getAxisByName(sender.tag)
        ax.ScaleView.ZoomReset(0)
        ax.IsLogarithmic = sender.Checked
    End Sub
#End Region

#Region "grid lines"
    Private Sub GroupCheckedChanged(sender As System.Object, e As System.EventArgs)
        For Each cntrl In sender.parent.controls
            If Not ReferenceEquals(cntrl, sender) Then
                Debug.Print(cntrl.name)
                cntrl.enabled = sender.checked
            End If
        Next

        If sender.name.contains("GridLines") Then
            Dim grd As Grid = getGridByName(sender.tag, sender.name)
            grd.Enabled = sender.checked
        ElseIf sender.name.contains("TickMarks") Then
            Dim tckMarks As TickMark = getTickMarksByName(sender.tag, sender.name)
            tckMarks.Enabled = sender.checked
        End If

    End Sub

    Private Sub GridLinesStyle_SelectedIndexChanged(sender As System.Object, e As System.EventArgs)
        Dim grd As Grid = getGridByName(sender.tag, sender.name)
        grd.LineDashStyle = sender.SelectedItem
    End Sub

    Private Sub Color_Click(sender As System.Object, e As System.EventArgs)

        Dim clrDlg As New ColorDialog
        clrDlg.Color = sender.backcolor
        clrDlg.ShowDialog()

        sender.backcolor = clrDlg.Color
        If sender.name.contains("GridLines") Then
            Dim grd As Grid = getGridByName(sender.tag, sender.name)
            grd.LineColor = sender.backcolor
        ElseIf sender.name.contains("TickMarks") Then
            Dim tckMarks As TickMark = getTickMarksByName(sender.tag, sender.name)
            tckMarks.LineColor = clrDlg.Color
        End If

    End Sub

    Private Sub TickMarksLength_TextChanged(sender As System.Object, e As System.EventArgs)
        Dim tckMarks As TickMark = getTickMarksByName(sender.tag, sender.name)
        Try
            tckMarks.Size = CSng(sender.text)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Width_TextChanged(sender As System.Object, e As System.EventArgs)
        If sender.name.contains("GridLines") Then
            Dim grd As Grid = getGridByName(sender.tag, sender.name)
            grd.LineWidth = CSng(sender.text)
        ElseIf sender.name.contains("TickMarks") Then
            Dim tckMarks As TickMark = getTickMarksByName(sender.tag, sender.name)
            tckMarks.LineWidth = CSng(sender.text)
        End If
    End Sub

    Private Sub MinorInterval_TextChanged(sender As System.Object, e As System.EventArgs)
        Try
            Dim ax As Axis = getAxisByName(sender.tag)
            ax.MinorTickMark.Interval = CSng(sender.text)
            ax.MinorGrid.Interval = CSng(sender.text)
        Catch ex As Exception

        End Try

    End Sub

#End Region

    Private Sub getNewFont(FontObjectToChange)
        Dim fontDlg As New FontDialog
        Try
            fontDlg.Font = FontObjectToChange.font
            fontDlg.Color = FontObjectToChange.forecolor
        Catch ex As Exception
            Try
                fontDlg.Font = FontObjectToChange.titlefont
                fontDlg.Color = FontObjectToChange.titleforecolor
            Catch ex2 As Exception
                fontDlg.Font = FontObjectToChange.font
                'fontDlg.Color = FontObjectToChange.FontColor
            End Try
            
        End Try

        fontDlg.ShowColor = True
        fontDlg.ShowDialog()
        Try
            FontObjectToChange.font = fontDlg.Font
            FontObjectToChange.forecolor = fontDlg.Color
        Catch ex As Exception
            Try
                FontObjectToChange.titlefont = fontDlg.Font
                FontObjectToChange.titleforecolor = fontDlg.Color
            Catch ex2 As Exception
                FontObjectToChange.font = fontDlg.Font
                'FontObjectToChange.FontColor = fontDlg.Color
            End Try
        End Try

    End Sub



    Private Sub btnMakeDefault_Click(sender As System.Object, e As System.EventArgs) Handles btnMakeDefault.Click
        mainDataManager.setDefaultChartSymbology(chartType, mainDataManager.serializeChartSymbologyToXML(childChart))
    End Sub

    Private Sub btnTitleFont_Click(sender As System.Object, e As System.EventArgs) Handles btnTitleFont.Click
        getNewFont(Me.childChart.Titles(0))
    End Sub

    Private Sub txtTitle_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtTitle.Validated
        If childChart.Titles.Count = 0 Then
            childChart.Titles.Add(txtTitle.Text)
        Else
            childChart.Titles(0).Text = txtTitle.Text
        End If

    End Sub

    Private Sub titleTop_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles titleTop.CheckedChanged, titleBottom.CheckedChanged, titleLeft.CheckedChanged, titleRight.CheckedChanged, TitleNone.CheckedChanged
        If Not childChart.Titles.Count = 0 Then


            childChart.Titles(0).Visible = True
            If titleBottom.Checked = True Then
                childChart.Titles(0).Docking = Docking.Bottom
            ElseIf titleTop.Checked = True Then
                childChart.Titles(0).Docking = Docking.Top
            ElseIf titleRight.Checked = True Then
                childChart.Titles(0).Docking = Docking.Right
            ElseIf titleLeft.Checked = True Then
                childChart.Titles(0).Docking = Docking.Left
            ElseIf TitleNone.Checked Then
                childChart.Titles(0).Visible = False
            End If

        End If
    End Sub

    Private Sub LabelsFormat_TextChanged(sender As System.Object, e As System.EventArgs) Handles XLabelsFormat.Validated, YLabelsFormat.Validated, Y2LabelsFormat.Validated
        Dim ax As Axis = getAxisByName(sender.tag)
        ax.LabelStyle.Format = getControlByName(sender.tag & "LabelsFormat").Text
    End Sub


    Private Sub chkDockInside_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkDockTitelInside.CheckedChanged
        childChart.Titles(0).IsDockedInsideChartArea = chkDockTitelInside.Checked
        childChart.Titles(0).DockedToChartArea = "Default"
    End Sub

    Private Sub chkDockLegendInside_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkDockLegendInside.CheckedChanged
        childChart.Legends(0).IsDockedInsideChartArea = chkDockLegendInside.Checked
        childChart.Legends(0).DockedToChartArea = "Default"
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.Text = "Auto" Then
            childChart.ChartAreas("Default").AxisX.IntervalType = DateTimeIntervalType.Auto
        ElseIf ComboBox1.Text = "Days" Then
            childChart.ChartAreas("Default").AxisX.IntervalType = DateTimeIntervalType.Days
        ElseIf ComboBox1.Text = "Weeks" Then
            childChart.ChartAreas("Default").AxisX.IntervalType = DateTimeIntervalType.Weeks
        ElseIf ComboBox1.Text = "Months" Then
            childChart.ChartAreas("Default").AxisX.IntervalType = DateTimeIntervalType.Months
        ElseIf ComboBox1.Text = "Years" Then
            childChart.ChartAreas("Default").AxisX.IntervalType = DateTimeIntervalType.Years
        End If
    End Sub


    Private Sub XAutoFitLabels_CheckedChanged(sender As System.Object, e As System.EventArgs)
        Dim ax As Axis = getAxisByName(sender.tag)
        ax.IsLabelAutoFit = sender.checked
    End Sub

    Private Sub btnChangeSeriesLabels_Click(sender As System.Object, e As System.EventArgs) Handles btnChangeSeriesLabels.Click
        Dim fontDlg As New FontDialog
        fontDlg.Font = childChart.Series(0).Font

        fontDlg.ShowColor = False
        fontDlg.ShowDialog()
        For Each s As Series In childChart.Series
            s.Font = fontDlg.Font
        Next
    End Sub
End Class