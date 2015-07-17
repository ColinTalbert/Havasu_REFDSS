Imports System.Windows.Forms.DataVisualization.Charting

Public Class SeriesFormater

    'Dim initialColor As Color
    'Dim initialWidth As Integer
    'Dim initialSytle As System.Windows.Forms.DataVisualization.Charting.ChartDashStyle
    'Dim initialMarker As System.Windows.Forms.DataVisualization.Charting.MarkerStyle
    Dim curSeries As DataVisualization.Charting.Series

    Private parentFormManager

    Public Sub New(s, mf)

        curSeries = s
        parentFormManager = mf
        ' This call is required by the designer.
        InitializeComponent()

        configureGUI()

        lblSeriesName.Text = s.Name

        ' Add any initialization after the InitializeComponent() call.


        'initialColor = curSeries.Color
        'initialWidth = curSeries.BorderWidth
        'initialSytle = curSeries.BorderDashStyle
        'initialMarker = curSeries.MarkerStyle

        If curSeries.LegendText <> "" Then
            txtLabel.Text = curSeries.LegendText
        Else
            txtLabel.Text = s.Name
        End If
    End Sub

    Private Sub configureGUI()

        btnColor.BackColor = curSeries.Color

        If curSeries.ChartType = SeriesChartType.Bar Or curSeries.ChartType = SeriesChartType.Column Then
            cboWidth.Items.Clear()
            cboWidth.Items.Add("0.2")
            cboWidth.Items.Add("0.4")
            cboWidth.Items.Add("0.6")
            cboWidth.Items.Add("0.8")
            cboWidth.Items.Add("1.0")
            If IsNothing(curSeries("PointWidth")) Then
                cboWidth.SelectedIndex = 3
            Else
                cboWidth.SelectedItem = curSeries("PointWidth")
            End If
            AddHandler cboWidth.SelectedIndexChanged, AddressOf cboWidth_SelectedIndexChanged

            cboStyle.Items.Clear()
            cboStyle.Items.Add("Default")
            cboStyle.Items.Add("Emboss")
            cboStyle.Items.Add("Cylinder")
            cboStyle.Items.Add("Wedge")
            cboStyle.Items.Add("LightToDark")
            cboStyle.SelectedItem = curSeries("DrawingStyle")

            cboMarkers.Items.Add("NA")
            cboMarkers.SelectedIndex = 0
            cboMarkers.Visible = False
        ElseIf curSeries.ChartType = SeriesChartType.FastLine Or curSeries.ChartType = SeriesChartType.Line Then
            cboWidth.Items.Clear()
            cboWidth.Items.Add("1")
            cboWidth.Items.Add("2")
            cboWidth.Items.Add("3")
            cboWidth.Items.Add("4")
            cboWidth.Items.Add("5")
            cboWidth.SelectedItem = CStr(curSeries.BorderWidth)

            cboStyle.Items.Clear()
            cboStyle.Items.Add(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid)
            cboStyle.Items.Add(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash)
            cboStyle.Items.Add(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDot)
            cboStyle.Items.Add(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot)
            cboStyle.Items.Add(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot)
            cboStyle.SelectedItem = curSeries.BorderDashStyle

            cboMarkers.Items.Add(System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None)
            cboMarkers.Items.Add(System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle)
            cboMarkers.Items.Add(System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Cross)
            cboMarkers.Items.Add(System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond)
            cboMarkers.Items.Add(System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Square)
            cboMarkers.Items.Add(System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star10)
            cboMarkers.Items.Add(System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4)
            cboMarkers.Items.Add(System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star5)
            cboMarkers.Items.Add(System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star6)
            cboMarkers.SelectedItem = curSeries.MarkerStyle
            AddHandler cboMarkers.SelectedIndexChanged, AddressOf cboMarkers_SelectedIndexChanged
        End If

        AddHandler cboStyle.SelectedIndexChanged, AddressOf cboStyle_SelectedIndexChanged
        AddHandler cboWidth.SelectedIndexChanged, AddressOf cboWidth_SelectedIndexChanged
    End Sub


    Private Sub btnColor_Click(sender As System.Object, e As System.EventArgs) Handles btnColor.Click
        parentFormManager.globalColorDialog.Color = btnColor.BackColor
        parentFormManager.globalColorDialog.ShowDialog()
        btnColor.BackColor = parentFormManager.globalColorDialog.Color
        curSeries.Color = btnColor.BackColor
    End Sub

    Private Sub cboWidth_SelectedIndexChanged(sender As System.Object, e As System.EventArgs)
        If curSeries.ChartType = SeriesChartType.Bar Or curSeries.ChartType = SeriesChartType.Column Then
            curSeries("PointWidth") = cboWidth.Text
        ElseIf curSeries.ChartType = SeriesChartType.FastLine Or curSeries.ChartType = SeriesChartType.Line Then
            curSeries.BorderWidth = cboWidth.Text
        End If


    End Sub




    'Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs)
    '    curSeries.Color = initialColor
    '    curSeries.BorderWidth = initialWidth
    '    Me.Close()
    'End Sub

    Private Sub cboStyle_SelectedIndexChanged(sender As System.Object, e As System.EventArgs)
        If curSeries.ChartType = SeriesChartType.Bar Or curSeries.ChartType = SeriesChartType.Column Then
            curSeries("DrawingStyle") = cboStyle.Text
        ElseIf curSeries.ChartType = SeriesChartType.FastLine Or curSeries.ChartType = SeriesChartType.Line Then
            curSeries.BorderDashStyle = cboStyle.SelectedItem
        End If
    End Sub

    Private Sub cboMarkers_SelectedIndexChanged(sender As System.Object, e As System.EventArgs)
        curSeries.MarkerStyle = cboMarkers.SelectedItem
    End Sub

    Private Sub txtLabel_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtLabel.Validated
        curSeries.LegendText = txtLabel.Text
    End Sub

    Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click
        Dim curSymbol As New seriesSymbol(CSng(cboWidth.Text), btnColor.BackColor, cboStyle.SelectedItem, cboMarkers.SelectedItem, txtLabel.Text)
        parentFormManager.maindatamanager.seriesSymbology(curSeries.Name) = curSymbol
        Me.Close()
    End Sub

End Class