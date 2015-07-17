Imports System.Xml

Public Class OverviewMapForm
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent


    Friend WithEvents AxMap1 As AxMapWinGIS.AxMap
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents DockPanel1 As WeifenLuo.WinFormsUI.Docking.DockPanel

    Private mainMapManager As MapManager
    Private mainHydroManager As HydrographManager
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Private components As System.ComponentModel.IContainer

    Public bLocked As Boolean = False

    Public Sub New(mm As MapManager)
        Me.InitializeComponent()

        mainMapManager = mm
        loadData()
    End Sub


    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim DockPanelSkin1 As WeifenLuo.WinFormsUI.Docking.DockPanelSkin = New WeifenLuo.WinFormsUI.Docking.DockPanelSkin()
        Dim AutoHideStripSkin1 As WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin = New WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin()
        Dim DockPanelGradient1 As WeifenLuo.WinFormsUI.Docking.DockPanelGradient = New WeifenLuo.WinFormsUI.Docking.DockPanelGradient()
        Dim TabGradient1 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim DockPaneStripSkin1 As WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin = New WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin()
        Dim DockPaneStripGradient1 As WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient = New WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient()
        Dim TabGradient2 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim DockPanelGradient2 As WeifenLuo.WinFormsUI.Docking.DockPanelGradient = New WeifenLuo.WinFormsUI.Docking.DockPanelGradient()
        Dim TabGradient3 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim DockPaneStripToolWindowGradient1 As WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient = New WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient()
        Dim TabGradient4 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim TabGradient5 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim DockPanelGradient3 As WeifenLuo.WinFormsUI.Docking.DockPanelGradient = New WeifenLuo.WinFormsUI.Docking.DockPanelGradient()
        Dim TabGradient6 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim TabGradient7 As WeifenLuo.WinFormsUI.Docking.TabGradient = New WeifenLuo.WinFormsUI.Docking.TabGradient()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(OverviewMapForm))
        Me.DockPanel1 = New WeifenLuo.WinFormsUI.Docking.DockPanel()
        Me.AxMap1 = New AxMapWinGIS.AxMap()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        CType(Me.AxMap1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DockPanel1
        '
        Me.DockPanel1.ActiveAutoHideContent = Nothing
        Me.DockPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DockPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DockPanel1.DockBackColor = System.Drawing.SystemColors.Control
        Me.DockPanel1.Location = New System.Drawing.Point(0, 0)
        Me.DockPanel1.Name = "DockPanel1"
        Me.DockPanel1.Size = New System.Drawing.Size(792, 654)
        DockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight
        DockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight
        AutoHideStripSkin1.DockStripGradient = DockPanelGradient1
        TabGradient1.EndColor = System.Drawing.SystemColors.Control
        TabGradient1.StartColor = System.Drawing.SystemColors.Control
        TabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark
        AutoHideStripSkin1.TabGradient = TabGradient1
        AutoHideStripSkin1.TextFont = New System.Drawing.Font("Segoe UI", 9.0!)
        DockPanelSkin1.AutoHideStripSkin = AutoHideStripSkin1
        TabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight
        TabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight
        TabGradient2.TextColor = System.Drawing.SystemColors.ControlText
        DockPaneStripGradient1.ActiveTabGradient = TabGradient2
        DockPanelGradient2.EndColor = System.Drawing.SystemColors.Control
        DockPanelGradient2.StartColor = System.Drawing.SystemColors.Control
        DockPaneStripGradient1.DockStripGradient = DockPanelGradient2
        TabGradient3.EndColor = System.Drawing.SystemColors.ControlLight
        TabGradient3.StartColor = System.Drawing.SystemColors.ControlLight
        TabGradient3.TextColor = System.Drawing.SystemColors.ControlText
        DockPaneStripGradient1.InactiveTabGradient = TabGradient3
        DockPaneStripSkin1.DocumentGradient = DockPaneStripGradient1
        DockPaneStripSkin1.TextFont = New System.Drawing.Font("Segoe UI", 9.0!)
        TabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption
        TabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical
        TabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption
        TabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText
        DockPaneStripToolWindowGradient1.ActiveCaptionGradient = TabGradient4
        TabGradient5.EndColor = System.Drawing.SystemColors.Control
        TabGradient5.StartColor = System.Drawing.SystemColors.Control
        TabGradient5.TextColor = System.Drawing.SystemColors.ControlText
        DockPaneStripToolWindowGradient1.ActiveTabGradient = TabGradient5
        DockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight
        DockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight
        DockPaneStripToolWindowGradient1.DockStripGradient = DockPanelGradient3
        TabGradient6.EndColor = System.Drawing.SystemColors.InactiveCaption
        TabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical
        TabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption
        TabGradient6.TextColor = System.Drawing.SystemColors.InactiveCaptionText
        DockPaneStripToolWindowGradient1.InactiveCaptionGradient = TabGradient6
        TabGradient7.EndColor = System.Drawing.Color.Transparent
        TabGradient7.StartColor = System.Drawing.Color.Transparent
        TabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark
        DockPaneStripToolWindowGradient1.InactiveTabGradient = TabGradient7
        DockPaneStripSkin1.ToolWindowGradient = DockPaneStripToolWindowGradient1
        DockPanelSkin1.DockPaneStripSkin = DockPaneStripSkin1
        Me.DockPanel1.Skin = DockPanelSkin1
        Me.DockPanel1.TabIndex = 0
        '
        'AxMap1
        '
        Me.AxMap1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AxMap1.Enabled = True
        Me.AxMap1.Location = New System.Drawing.Point(0, 0)
        Me.AxMap1.Name = "AxMap1"
        Me.AxMap1.OcxState = CType(resources.GetObject("AxMap1.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxMap1.Size = New System.Drawing.Size(788, 650)
        Me.AxMap1.TabIndex = 2
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel1.Controls.Add(Me.AxMap1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(792, 654)
        Me.Panel1.TabIndex = 4
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(61, 4)
        '
        'OverviewMapForm
        '
        Me.ClientSize = New System.Drawing.Size(792, 654)
        Me.ContextMenuStrip = Me.ContextMenuStrip1
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.DockPanel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "OverviewMapForm"
        Me.Text = "Overview Map"
        Me.TopMost = True
        CType(Me.AxMap1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub


    Private Sub OverviewMapForm_FormClosing(ByVal sender As Object, _
     ByVal e As System.Windows.Forms.FormClosingEventArgs) _
      Handles Me.FormClosing
        'release the instance of this form
        'anInstance = Nothing
        mainMapManager.removeOverviewMapForm(Me)
    End Sub


    Private shpSegs As New MapWinGIS.Shapefile
    Private hndSegs As Integer
    Private highlightCircle As Integer

    Private Sub loadData()

        Dim segs As String = My.Settings.InputDataDirectory + "\Overview\Study_segments_proj.shp"
        Dim rivs As String = My.Settings.InputDataDirectory + "\\Overview\MajorRivers.shp"
        Dim waterBodies As String = My.Settings.InputDataDirectory + "\\Overview\OtherWaterbodies.shp"
        Dim mainRes As String = My.Settings.InputDataDirectory + "\Overview\Resevoirs.shp"
        Dim rivAreas As String = My.Settings.InputDataDirectory + "\Overview\RiverPolys.shp"
        Dim background As String = My.Settings.InputDataDirectory + "\Overview\ned100m_east72.jpg"
        Dim states As String = My.Settings.InputDataDirectory + "\Overview\states.shp"

        Dim shpRivs As New MapWinGIS.Shapefile
        Dim shpWaterBodies As New MapWinGIS.Shapefile
        Dim shpMainRes As New MapWinGIS.Shapefile
        Dim shpRivAreas As New MapWinGIS.Shapefile
        Dim shpStates As New MapWinGIS.Shapefile
        Dim imgBackground As New MapWinGIS.Image

        shpSegs.Open(segs)
        shpStates.Open(states)
        shpRivs.Open(rivs)
        shpWaterBodies.Open(waterBodies)
        shpMainRes.Open(mainRes)
        shpRivAreas.Open(rivAreas)
        imgBackground.UpsamplingMode = MapWinGIS.tkInterpolationMode.imNone
        imgBackground.TransparencyPercent = 0.5
        imgBackground.Open(background, MapWinGIS.ImageType.USE_FILE_EXTENSION, False)

        Dim hndRivs, hndWaterBodies, hndMainRes, hndRivAreas, hndBackground, hndStates As Integer

        hndBackground = AxMap1.AddLayer(imgBackground, True)

        hndStates = AxMap1.AddLayer(shpStates, True)
        AxMap1.set_ShapeLayerLineColor(hndStates, Convert.ToUInt32(RGB(125, 125, 125)))
        AxMap1.set_ShapeLayerLineWidth(hndStates, 8)
        AxMap1.set_ShapeLayerDrawFill(hndStates, False)

        hndRivs = AxMap1.AddLayer(shpRivs, True)
        hndRivAreas = AxMap1.AddLayer(shpRivAreas, True)
        hndWaterBodies = AxMap1.AddLayer(shpWaterBodies, True)
        hndMainRes = AxMap1.AddLayer(shpMainRes, True)
        hndSegs = AxMap1.AddLayer(shpSegs, True)



        AxMap1.set_ShapeLayerLineColor(hndRivs, Convert.ToUInt32(RGB(50, 50, 255)))
        AxMap1.set_ShapeLayerLineWidth(hndRivs, 3)

        AxMap1.set_ShapeLayerLineColor(hndSegs, Convert.ToUInt32(RGB(255, 0, 0)))
        'AxMap1.set_ShapeLayerDrawFill(hndSegs, True)
        AxMap1.set_ShapeLayerLineWidth(hndSegs, 4)
        AxMap1.set_ShapeLayerDrawFill(hndSegs, False)




        AxMap1.set_ShapeLayerFillColor(hndMainRes, Convert.ToUInt32(RGB(100, 100, 255)))
        AxMap1.set_ShapeLayerLineColor(hndMainRes, Convert.ToUInt32(RGB(50, 50, 255)))
        AxMap1.set_ShapeLayerLineWidth(hndMainRes, 3)

        AxMap1.set_ShapeLayerFillColor(hndRivAreas, Convert.ToUInt32(RGB(100, 100, 255)))
        AxMap1.set_ShapeLayerLineColor(hndRivAreas, Convert.ToUInt32(RGB(50, 50, 255)))
        AxMap1.set_ShapeLayerLineWidth(hndRivAreas, 3)





        'Set the labels
        Dim labeltext As String
        Dim labelcolor As UInt32 = Convert.ToUInt32(RGB(255, 0, 0))
        Dim x, y As Double
        Dim fieldNum As Integer = 2
        For shapenum = 0 To shpSegs.NumShapes - 1
            labeltext = shpSegs.CellValue(fieldNum, shapenum)
            x = shpSegs.Shape(shapenum).Point(0).x
            y = shpSegs.Shape(shapenum).Point(0).y
            'AxMap1.AddLabel(hndSegs, labeltext, Convert.ToUInt32(RGB(255, 0, 0)), x, y, MapWinGIS.tkHustification.hjRight)
        Next
        'AxMap1.set_LayerLabelsScale(hndSegs, True)
        'AxMap1.set_LayerLabelsOffset(hndSegs, -15)
        'AxMap1.set_LayerLabelsShadow(hndSegs, True)
        'AxMap1.set_LayerLabelsShadowColor(hndSegs, Convert.ToUInt32(RGB(255, 255, 255)))
        'AxMap1.set_UseLabelCollision(hndSegs, True)


        'Set the extents
        Dim exts As New MapWinGIS.Extents
        'exts.SetBounds(xMin, yMin, 0, xMax, YMax, 0)
        'exts.SetBounds(-75.2, 41.6, 0, -74.6, 42.2, 0)
        AxMap1.SetBounds(-75.2, 41.8, -74.8, 42.6)
        AxMap1.MapResizeBehavior = MapWinGIS.tkResizeBehavior.rbModern
        'AxMap1.Extents = exts
        AxMap1.SendMouseUp = True
        AxMap1.Redraw()

    End Sub



    Private Sub AxMap1_MouseUpEvent(sender As Object, e As AxMapWinGIS._DMapEvents_MouseUpEvent) Handles AxMap1.MouseUpEvent
        Dim ext As New MapWinGIS.Extents
        Dim TL, BR As MapWinGIS.Point
        Dim shapes() As Integer = Nothing
        Dim shp As Integer
        Dim Buffer As Integer
        TL = New MapWinGIS.Point 'Top Left Projected
        BR = New MapWinGIS.Point 'Bottom Right Projected
        Buffer = 10 'For points and lines give a 5 pixel buffer for selecting the shapes
        'For polygons, select shapes exactly
        If get_myClass() = "Polygon" Then Buffer = 0
        AxMap1.PixelToProj(e.x - Buffer, e.y + Buffer, TL.x, TL.y)
        AxMap1.PixelToProj(e.x + Buffer, e.y - Buffer, BR.x, BR.y)
        ext = New MapWinGIS.Extents
        ext.SetBounds(TL.x, BR.y, 0, BR.x, TL.y, 0)

        AxMap1.set_ShapeLayerLineColor(hndSegs, Convert.ToUInt32(RGB(255, 0, 0)))

        If shpSegs.SelectShapes(ext, 0.0, MapWinGIS.SelectMode.INTERSECTION, shapes) Then
            'We found some shapes
            For shp = 0 To shapes.GetUpperBound(0)
                Dim segAbrev As String = shpSegs.CellValue(2, shapes(shp))
                Debug.Print(segAbrev)
                Dim segName As String = mainMapManager.parentMainForm.mainDataManager.getSegmentName(segAbrev)

                mainMapManager.addSegmentMapForm(True, segAbrev)
                'mainMapManager.parentMainForm.mainHydrographManager.addSegment(segName)
                'Do something with shapes(shp)
                'AxMap1.set_ShapeLineColor(hndSegs, shapes(shp), System.Convert.ToUInt32(RGB(255, 255, 0)))
                'AxMap1.set_ShapeFillColor(hndSegs, shapes(shp), System.Convert.ToUInt32(RGB(255, 255, 0)))

                highlightCircle = AxMap1.NewDrawing(MapWinGIS.tkDrawReferenceList.dlScreenReferencedList)
                For i As Integer = 1 To 15
                    AxMap1.DrawCircle(e.x, e.y, i, System.Convert.ToUInt32(RGB(255, 255, 0)), True)
                    AxMap1.Refresh()
                Next
                Threading.Thread.Sleep(50)
                AxMap1.ClearDrawing(highlightCircle)
            Next shp
        End If
        AxMap1.Redraw()
    End Sub

    Private Function get_myClass() As String
        With shpSegs
            If .ShapefileType = MapWinGIS.ShpfileType.SHP_MULTIPOINT Or _
               .ShapefileType = MapWinGIS.ShpfileType.SHP_MULTIPOINTM Or _
               .ShapefileType = MapWinGIS.ShpfileType.SHP_MULTIPOINTZ Or _
               .ShapefileType = MapWinGIS.ShpfileType.SHP_POINT Or _
               .ShapefileType = MapWinGIS.ShpfileType.SHP_POINTM Or _
               .ShapefileType = MapWinGIS.ShpfileType.SHP_POINTZ Then
                get_myClass = "Point"
                Exit Function
            End If
            If .ShapefileType = MapWinGIS.ShpfileType.SHP_POLYLINE Or _
               .ShapefileType = MapWinGIS.ShpfileType.SHP_POLYLINEM Or _
               .ShapefileType = MapWinGIS.ShpfileType.SHP_POLYLINEZ Then
                get_myClass = "Line"
                Exit Function
            End If
        End With
        get_myClass = "Polygon"
    End Function

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

        Dim options As New Dictionary(Of String, ToolStripMenuItem)

        For Each toolStripMenuItem In ContextMenuStrip1.Items
            Debug.Print(toolStripMenuItem.text)
            If toolStripMenuItem.text <> "lock" And
                toolStripMenuItem.text <> "Format Chart" And
                toolStripMenuItem.text <> "" Then
                options.Add(toolStripMenuItem.text, toolStripMenuItem)
            End If

        Next
        'options.Add("scenarios", curhydrographForm.ScenarioContextMenu)
        'options.Add("segments", curhydrographForm.SegmentToolStripMenuItem1)
        'options.Add("otherMetrics", curhydrographForm.OtherMetricToolStripMenuItem)

        For Each key As String In options.Keys
            Dim checkedOptions As New List(Of String)
            For Each item In options(key).DropDownItems
                If item.checkstate = System.Windows.Forms.CheckState.Checked Then
                    checkedOptions.Add(item.text)
                End If
            Next
            Dim checkedOptionNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement(key.Replace(" ", "_"))
            checkedOptionNode.InnerText = String.Join("|", checkedOptions)
            outputNode.AppendChild(checkedOptionNode)
        Next

        Dim xMinNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("xMin")
        xMinNode.InnerText = CStr(AxMap1.Extents.xMin)
        outputNode.AppendChild(xMinNode)

        Dim xMaxNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("xMax")
        xMaxNode.InnerText = CStr(AxMap1.Extents.xMax)
        outputNode.AppendChild(xMaxNode)

        Dim yMinNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("yMin")
        yMinNode.InnerText = CStr(AxMap1.Extents.yMin)
        outputNode.AppendChild(yMinNode)

        Dim yMaxNode As XmlNode = mainMapManager.parentMainForm.mainDataManager.config.CreateElement("yMax")
        yMaxNode.InnerText = CStr(AxMap1.Extents.yMax)
        outputNode.AppendChild(yMaxNode)


        Return outputNode

    End Function

    Public Sub loadFromXmlNode(inNode As XmlNode)

        Dim dcNode As XmlNode
        dcNode = mainMapManager.parentMainForm.mainDataManager.config.SelectSingleNode("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & "']")
        bLocked = CBool(dcNode.SelectSingleNode("Locked").InnerText)

        Dim optionNodes As XmlNodeList = mainMapManager.parentMainForm.mainDataManager.config.SelectNodes("SmartRiverConfig/GUIConfigs/Current/GUIConfig/SmartRiverWidget[Name='" & Me.Text & "']/*")
        For Each optionNode As XmlNode In optionNodes
            If optionNode.Name <> "Locked" And
                optionNode.Name <> "Name" Then
                Dim checkedoptions As String() = optionNode.InnerText.Split("|")
                For Each strOption In checkedoptions
                    Dim item As ToolStripMenuItem
                    item = ContextMenuStrip1.Items(optionNode.Name.Replace(" ", "_"))
                    Dim ddItem As ToolStripMenuItem
                    If Not IsNothing(item) Then
                        ddItem = item.DropDownItems(strOption.Replace(" ", "_"))
                        If Not IsNothing(ddItem) Then
                            ddItem.Checked = True
                        Else
                            Debug.Print(strOption + " is nothing")
                        End If
                    Else
                        Debug.Print(optionNode.Name + " is nothing")
                    End If
                Next
            End If
        Next

        Try
            Dim prevExtent As New MapWinGIS.Extents
            Dim xMin, yMin, xMax, ymax As Double
            xMin = CDbl(dcNode.SelectSingleNode("xMin").InnerText)
            yMin = CDbl(dcNode.SelectSingleNode("yMin").InnerText)
            xMax = CDbl(dcNode.SelectSingleNode("xMax").InnerText)
            ymax = CDbl(dcNode.SelectSingleNode("yMax").InnerText)

            prevExtent.SetBounds(xMin, yMin, 0, xMax, ymax, 0)
            AxMap1.Extents = prevExtent
        Catch ex As Exception

        End Try

    End Sub

    Public Sub refreshAfterLoad()
        'MsgBox("not implemented")
    End Sub


#End Region

End Class
