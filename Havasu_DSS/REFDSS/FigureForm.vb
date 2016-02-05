Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Data.SQLite
Imports System.Xml
Imports System.IO
Imports zpblib


Public Class FigureForm
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    Private mainGraphManager As GraphManager
    Public mainDataManager As DataManager
    Private curMetric As String

    Private components As System.ComponentModel.IContainer

    Public bLocked As Boolean = False

    Public previousRect As Rectangle = Nothing
    Public chartType As String = "FigureForm"
    Public dataChanged As Boolean = False

    Private WithEvents SelectData As New System.Windows.Forms.ToolStripMenuItem("Select Data")
    Private WithEvents CopySelected As New System.Windows.Forms.ToolStripMenuItem("Copy selected Ctrl+C")
    Private WithEvents CopyAll As New System.Windows.Forms.ToolStripMenuItem("Copy all data")
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private myContextMenuStrip As ContextMenuStrip
    Friend WithEvents BackToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ForwardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ZoomPictureBox1 As ZPBlib.ZoomPictureBox

    Private Tier1FigureDname As String

    Private OrigMetricNames As New Dictionary(Of String, String)

    Public Sub New(DM As DataManager)
        mainDataManager = DM

        Me.InitializeComponent()
        ' loadData()

        Tier1FigureDname = Path.Combine(My.Settings.InputDataDirectory, "Tier1", "Figures")


    End Sub


    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FigureForm))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ForwardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ZoomPictureBox1 = New ZPBlib.ZoomPictureBox()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveToolStripMenuItem, Me.BackToolStripMenuItem, Me.ForwardToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(818, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(43, 20)
        Me.SaveToolStripMenuItem.Text = "Save"
        '
        'BackToolStripMenuItem
        '
        Me.BackToolStripMenuItem.Image = Global.Havasu_DSS.My.Resources.Resources.go_previous_21
        Me.BackToolStripMenuItem.Name = "BackToolStripMenuItem"
        Me.BackToolStripMenuItem.Size = New System.Drawing.Size(60, 20)
        Me.BackToolStripMenuItem.Text = "back"
        Me.BackToolStripMenuItem.Visible = False
        '
        'ForwardToolStripMenuItem
        '
        Me.ForwardToolStripMenuItem.Image = Global.Havasu_DSS.My.Resources.Resources.go_next_2
        Me.ForwardToolStripMenuItem.Name = "ForwardToolStripMenuItem"
        Me.ForwardToolStripMenuItem.Size = New System.Drawing.Size(76, 20)
        Me.ForwardToolStripMenuItem.Text = "forward"
        Me.ForwardToolStripMenuItem.Visible = False
        '
        'ZoomPictureBox1
        '
        Me.ZoomPictureBox1.BackColor = System.Drawing.SystemColors.AppWorkspace
        Me.ZoomPictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ZoomPictureBox1.EnableMouseDragging = True
        Me.ZoomPictureBox1.EnableMouseWheelZooming = True
        Me.ZoomPictureBox1.Image = Nothing
        Me.ZoomPictureBox1.ImageBounds = New System.Drawing.Rectangle(0, 0, 0, 0)
        Me.ZoomPictureBox1.ImagePosition = New System.Drawing.Point(0, 0)
        Me.ZoomPictureBox1.Location = New System.Drawing.Point(0, 24)
        Me.ZoomPictureBox1.MaximumZoomFactor = 64.0R
        Me.ZoomPictureBox1.MinimumImageHeight = 10
        Me.ZoomPictureBox1.MinimumImageWidth = 10
        Me.ZoomPictureBox1.MinimumSize = New System.Drawing.Size(10, 10)
        Me.ZoomPictureBox1.MouseWheelDivisor = 4000
        Me.ZoomPictureBox1.Name = "ZoomPictureBox1"
        Me.ZoomPictureBox1.Size = New System.Drawing.Size(818, 720)
        Me.ZoomPictureBox1.TabIndex = 1
        Me.ZoomPictureBox1.ZoomFactor = 0.0R
        '
        'FigureForm
        '
        Me.ClientSize = New System.Drawing.Size(818, 744)
        Me.Controls.Add(Me.ZoomPictureBox1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FigureForm"
        Me.Text = "Figure viewer"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Public Sub loadData()
        Try
            Me.ZoomPictureBox1.Image() = New Bitmap(Path.Combine(Tier1FigureDname, Me.Text.Split(":")(0) & ".jpg"))
        Catch ex As Exception

        End Try

    End Sub

    

#Region "Serialization"


    Public Function saveToXMLNode() As XmlNode
        Dim outputNode As XmlNode
        outputNode = mainDataManager.config.CreateElement("SmartRiverWidget")

        Dim nameNode As XmlNode
        nameNode = mainDataManager.config.CreateElement("Name")
        nameNode.InnerText = Me.Text
        outputNode.AppendChild(nameNode)

        Dim tagNode As XmlNode
        tagNode = mainDataManager.config.CreateElement("Tag")
        tagNode.InnerText = Me.Tag
        outputNode.AppendChild(tagNode)

        Dim strImageBounds As String = Me.ZoomPictureBox1.ImageBounds.ToString
        Dim boundsNode As XmlNode
        boundsNode = mainDataManager.config.CreateElement("bounds")

        Dim rectNode As XmlNode = mainDataManager.serializeRectangleToXML(Me.ZoomPictureBox1.ImageBounds)
        boundsNode.AppendChild(rectNode)
        outputNode.AppendChild(boundsNode)



        'Dim lockedNode As XmlNode = mainDataManager.config.CreateElement("Locked")
        'lockedNode.InnerText = bLocked.ToString
        'outputNode.AppendChild(lockedNode)

        'Dim options As New Dictionary(Of String, ToolStripMenuItem)

        'For Each toolStripMenuItem In ContextMenuStrip1.Items
        '    Debug.Print(toolStripMenuItem.text)
        '    If toolStripMenuItem.text <> "lock" And
        '        toolStripMenuItem.text <> "Format Chart" And
        '        toolStripMenuItem.text <> "" Then
        '        options.Add(toolStripMenuItem.text, toolStripMenuItem)
        '    End If

        'Next
        ''options.Add("scenarios", curhydrographForm.ScenarioContextMenu)
        ''options.Add("segments", curhydrographForm.SegmentToolStripMenuItem1)
        ''options.Add("otherMetrics", curhydrographForm.OtherMetricToolStripMenuItem)

        'For Each key As String In options.Keys
        '    Dim checkedOptions As New List(Of String)
        '    For Each item In options(key).DropDownItems
        '        If item.checkstate = System.Windows.Forms.CheckState.Checked Then
        '            checkedOptions.Add(item.text)
        '        End If
        '    Next
        '    Dim checkedOptionNode As XmlNode = mainDataManager.config.CreateElement(key.Replace(" ", "_"))
        '    checkedOptionNode.InnerText = String.Join("|", checkedOptions)
        '    outputNode.AppendChild(checkedOptionNode)
        'Next

        'outputNode.AppendChild(mainDataManager.serializeChartDisplayDataToXML(curDisplayData))
        'outputNode.AppendChild(mainDataManager.serializeChartSymbologyToXML(HabitatChart))

        Return outputNode

    End Function

    Public Sub loadFromXmlNode(inNode As XmlNode)

        Dim dcNode As XmlNode
        dcNode = mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Tag='" & Me.Tag & "']")


        If dcNode Is Nothing Then

            Me.Text = Me.mainDataManager.parentMainForm.FiguresTSM.DropDownItems(0).Text()
        Else
            Dim nameNode As XmlNode = dcNode.SelectSingleNode("Name")
            Me.Text = nameNode.InnerText
            previousRect = mainDataManager.deserializeRectFromXML(dcNode.SelectSingleNode("bounds").SelectSingleNode("Rectangle"))
        End If


        'bLocked = CBool(dcNode.SelectSingleNode("Locked").InnerText)
        'Me.myContextMenuStrip.setLockState(bLocked)

        'curDisplayData = mainDataManager.deserializeChartDisplayDataFromXML(dcNode.SelectSingleNode("chartDisplayData"))
        'Dim chartSymbologyNode As XmlNode = dcNode.SelectSingleNode("Chart")
        'If Not IsNothing(chartSymbologyNode) Then
        '    mainDataManager.symbolizeChartFromXML(HabitatChart, chartSymbologyNode)
        'End If


    End Sub





    Public Sub refreshAfterLoad()
        loadData()

        If Not previousRect = Nothing Then
            Me.ZoomPictureBox1.SetZoomBounds(previousRect)
        End If
    End Sub

#End Region


    Private Sub SaveToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SaveToolStripMenuItem.Click
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
            .Title = "Save data to jpeg file"
            .AddExtension = True
            .DefaultExt = ".jpg"
            .CheckFileExists = False
            If .ShowDialog = DialogResult.OK Then
                File.Copy(Path.Combine(Tier1FigureDname, Me.Text.Split(":")(0) & ".jpg"), FileBrowserDialog1.FileName, True)
            Else

                Exit Sub

            End If
        End With
    End Sub


    Private Sub FigureForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Me.mainDataManager.removeFigureForm(Me)

    End Sub
End Class
