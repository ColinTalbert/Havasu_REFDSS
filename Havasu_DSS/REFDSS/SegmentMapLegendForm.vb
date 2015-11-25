Imports System.IO
Imports System.Xml

Public Class SegmentMapLegendForm
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents lblTitle As System.Windows.Forms.Label

    Private mainMapManager As MapManager
    Public curCovariate As String
    Public colorSchemes As New Dictionary(Of String, MapWinGIS.GridColorScheme)

    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Private components As System.ComponentModel.IContainer
    Friend WithEvents HabitatToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents dgv_legend As System.Windows.Forms.DataGridView
    Friend WithEvents Color As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Label As System.Windows.Forms.DataGridViewTextBoxColumn



    Public bLocked As Boolean = False

    Public Sub New(MM As MapManager)
        mainMapManager = MM
        Me.InitializeComponent()

        LoadData()

    End Sub

    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SegmentMapLegendForm))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.dgv_legend = New System.Windows.Forms.DataGridView()
        Me.Color = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Label = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.HabitatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.Panel1.SuspendLayout()
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        CType(Me.dgv_legend, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.dgv_legend)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.lblTitle)
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
        'dgv_legend
        '
        Me.dgv_legend.AllowUserToAddRows = False
        Me.dgv_legend.AllowUserToDeleteRows = False
        Me.dgv_legend.BackgroundColor = System.Drawing.Color.White
        Me.dgv_legend.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.dgv_legend.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgv_legend.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Color, Me.Label})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.White
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgv_legend.DefaultCellStyle = DataGridViewCellStyle2
        Me.dgv_legend.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgv_legend.GridColor = System.Drawing.Color.White
        Me.dgv_legend.Location = New System.Drawing.Point(0, 0)
        Me.dgv_legend.MinimumSize = New System.Drawing.Size(10, 10)
        Me.dgv_legend.MultiSelect = False
        Me.dgv_legend.Name = "dgv_legend"
        Me.dgv_legend.ReadOnly = True
        Me.dgv_legend.RowHeadersWidth = 5
        Me.dgv_legend.Size = New System.Drawing.Size(814, 715)
        Me.dgv_legend.TabIndex = 2
        '
        'Color
        '
        Me.Color.HeaderText = "Color"
        Me.Color.Name = "Color"
        Me.Color.ReadOnly = True
        Me.Color.Width = 40
        '
        'Label
        '
        Me.Label.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        Me.Label.DefaultCellStyle = DataGridViewCellStyle1
        Me.Label.HeaderText = "Label"
        Me.Label.Name = "Label"
        Me.Label.ReadOnly = True
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
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HabitatToolStripMenuItem, Me.ToolStripSeparator1})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(114, 32)
        '
        'HabitatToolStripMenuItem
        '
        Me.HabitatToolStripMenuItem.CheckOnClick = True
        Me.HabitatToolStripMenuItem.Name = "HabitatToolStripMenuItem"
        Me.HabitatToolStripMenuItem.Size = New System.Drawing.Size(113, 22)
        Me.HabitatToolStripMenuItem.Text = "Habitat"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(110, 6)
        Me.ToolStripSeparator1.Tag = "sep"
        '
        'SegmentMapLegendForm
        '
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(818, 744)
        Me.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "SegmentMapLegendForm"
        Me.TabPageContextMenuStrip = Me.ContextMenuStrip1
        Me.Text = "Segment Map"
        Me.Panel1.ResumeLayout(False)
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.ContentPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        CType(Me.dgv_legend, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Public Sub LoadData()
        Debug.Print("loadData()")
        updateCovariates()

        curCovariate = "habitat"

        DrawLegend()
    End Sub

    Public Sub updateCovariates()

        For i As Integer = ContextMenuStrip1.Items.Count - 1 To 0
            If ContextMenuStrip1.Items(i).Text <> "Habitat" And ContextMenuStrip1.Items(i).Tag <> "sep" Then
                ContextMenuStrip1.Items.RemoveAt(i)
            End If
        Next

        Dim curSegment As String = mainMapManager.parentMainForm.mainDataManager.getSegmentNames(0)
        Dim curTreatment As String = mainMapManager.parentMainForm.mainDataManager.getTreatmentNames()(0)
        For Each cov As String In mainMapManager.parentMainForm.mainDataManager.getCovariateNames(curSegment, curTreatment)
            Dim menuitem As New ToolStripMenuItem
            menuitem.Text = cov
            menuitem.Tag = "covariate"
            menuitem.CheckOnClick = True
            menuitem.DoubleClickEnabled = False
            AddHandler menuitem.Click, AddressOf ToolStripMenuItem_Click

            ContextMenuStrip1.Items.Add(menuitem)
        Next
        If curCovariate Is Nothing Then
            curCovariate = "habitat"
        End If
        setCheckedCovariate(curCovariate)
    End Sub


    Public Sub DrawLegend()
        Dim title As String = ""
        title += curCovariate ' + " map legend"

        lblTitle.Text = title
        Me.Text = title

        dgv_legend.Rows.Clear()

        Dim rowIndex As Integer = 1
        Dim breakNodes As XmlNodeList
        If curCovariate = "Habitat" Then
            breakNodes = MainForm.mainDataManager.habitatBreaks()
        Else
            breakNodes = MainForm.mainDataManager.covariateBreaks(curCovariate)
        End If
        dgv_legend.RowCount = breakNodes.Count + 1
        For Each breakNode As XmlNode In breakNodes
            Dim c As String = breakNode.SelectSingleNode("rgb").FirstChild.Value
            Dim l As String = breakNode.SelectSingleNode("label").FirstChild.Value
            Dim breakColor As Color = stringToColor(c)
            If breakColor.R = 0 And breakColor.G = 0 And breakColor.B = 0 Then
                breakColor = Drawing.Color.White
            End If


            dgv_legend.Rows(rowIndex).Cells(0).Style.BackColor = breakColor
            dgv_legend.Rows(rowIndex).Cells(1).Value = l
            rowIndex += 1
        Next

        'dgv_legend.Rows.RemoveAt(0)

    End Sub

    Private Sub setCheckedCovariate(strCovariate As String)

        For Each item In ContextMenuStrip1.Items
            If item.Tag <> "sep" Then
                item.Checked = item.Text.ToLower() = strCovariate.ToLower()
            End If
        Next
    End Sub

    Private Sub ToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles HabitatToolStripMenuItem.Click

        curCovariate = sender.text
        setCheckedCovariate(sender.text)

        DrawLegend()
    End Sub

    Private Sub SegmentMapForm_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        mainMapManager.removeSegmentMapLegendForm(Me)
    End Sub
#Region "Serialization"


    Public Function saveToXMLNode() As XmlNode
        Dim outputNode As XmlNode
        outputNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("SmartRiverWidget")

        Dim nameNode As XmlNode
        nameNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("Name")
        nameNode.InnerText = Me.Tag
        outputNode.AppendChild(nameNode)


        Dim curCovariateNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("curCovariate")
        curCovariateNode.InnerText = curCovariate
        outputNode.AppendChild(curCovariateNode)



        Return outputNode

    End Function

    Public Sub loadFromXmlNode(inNode As XmlNode)
        Try
            curCovariate = inNode.SelectSingleNode("curCovariate").InnerText
        Catch ex As Exception
            curCovariate = "habitat"
        End Try

        DrawLegend()


        setCheckedCovariate(curCovariate)

    End Sub



    Public Sub refreshAfterLoad()
        'MsgBox("not implemented")
    End Sub

#End Region
End Class


