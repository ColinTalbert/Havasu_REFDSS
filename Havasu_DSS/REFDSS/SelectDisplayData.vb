Imports System.Data.SQLite
Imports System.Xml
Imports System.Xml.Serialization

Public Class SelectDisplayData

    Public mainParentForm
    Public chartType As String
    Public stepInterval = "daily" 'or "yearly" or "single"
    Private HydroEventsOff As Boolean = False

    Public Sub New(hg, chrtType)
        mainParentForm = hg
        chartType = chrtType
        ' This call is required by the designer.
        InitializeComponent()



        ' Add any initialization after the InitializeComponent() call.
        configureForParent(hg.curDisplayData)
        populateDataOptionGui()

        loadChartDisplayData(hg.curDisplayData)
    End Sub


    Public Sub configureForParent(cdd As chartDisplayData)
        'show or hide the particular components required for this type of parent form
        If TypeOf mainParentForm Is HydrographGraphForm Then
            tcDisplays.TabPages.Remove(tpDisplayMetrics)
            pnlInterval.Visible = False
        ElseIf TypeOf mainParentForm Is HabitatGraphForm Or _
            TypeOf mainParentForm Is FlowVsHabitatChartForm Then
            tcDisplays.TabPages.Remove(tpOtherMetrics)
            pnlInterval.Visible = False
        ElseIf TypeOf mainParentForm Is aggHabitatGraphForm Then
            tcDisplays.TabPages.Remove(tpOtherMetrics)
            rbDaily.Visible = False
            pnlInterval.Visible = True
        ElseIf TypeOf mainParentForm Is RawDataForm Then
            pnlInterval.Visible = True
        End If

        If cdd.interval = "daily" Then
            tpOtherMetrics.Visible = True
            tpDisplayMetrics.Visible = False
            pnlDailyFlow.Visible = True
            lblDailyFlow.Visible = True
        Else
            tpOtherMetrics.Visible = False
            tpDisplayMetrics.Visible = True
            pnlDailyFlow.Visible = False
            lblDailyFlow.Visible = False
        End If

        If TypeOf mainParentForm Is FlowVsHabitatChartForm Then
            tvScenario.Visible = False
            lblBaseline.Visible = False
            cboBaseline.Visible = False
            lblScenario.Visible = False
            pnlLimitTimePeriod.Visible = False
            tcDisplays.TabPages.Remove(tpDisplayMetrics)
            pnlParameters.Visible = False
        End If

    End Sub


    Public Sub populateDataOptionGui()
        'Options for Scenarios
        tvScenario.Nodes.Clear()

        For Each scen In mainParentForm.mainDataManager.getScenarioNames()
            tvScenario.Nodes.Add(scen)
            cboBaseline.Items.Add(scen)
        Next

        'Options for treatments
        tvTreatment.Nodes.Clear()

        For Each treatment In mainParentForm.mainDataManager.getTreatmentNames
            tvTreatment.Nodes.Add(treatment)
        Next


        'Options For Rivers
        tvRivers.Nodes("AggByRiver").Nodes.Clear()
        For Each riv In mainParentForm.mainDataManager.getRiverNames()

            tvRivers.Nodes("AggByRiver").Nodes.Add(riv, "Aggregate across the " + riv)
            For Each seg In mainParentForm.mainDataManager.getSegmentNamesForRiver(riv)
                tvRivers.Nodes("AggByRiver").Nodes(riv).Nodes.Add(seg)
            Next
        Next
        tvRivers.Nodes("AggByRiver").ExpandAll()


        tvOtherMetrics.Nodes.Clear()
        For Each otherMetric In mainParentForm.maindatamanager.getOtherMetrics()
            tvOtherMetrics.Nodes.Add(otherMetric)
        Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Hard wired for havasu :(
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ''Now add all the fields that are not segment flows to the OtherVars menulist dropdown items
        'Dim segmentAbbrevs As List(Of String) = mainParentForm.mainDataManager.getSegmentAbbrevs()

        'Dim strsql As String = "pragma table_info(scenario_" + tvScenario.Nodes(0).Text + ");"

        'mainParentForm.mainDataManager.mainSQLDBConnection.Open()
        'Dim SQLcommand As SQLiteCommand
        'SQLcommand = mainParentForm.mainDataManager.mainSQLDBConnection.CreateCommand
        'SQLcommand.CommandText = strsql
        'Dim SQLreader As SQLiteDataReader = SQLcommand.ExecuteReader()

        'While SQLreader.Read()
        '    If Not segmentAbbrevs.Contains(SQLreader(1)) And
        '        Not SQLreader(1) = "Date" Then
        '        tvOtherMetrics.Nodes.Add(SQLreader(1))
        '    End If

        'End While
        'SQLcommand.Dispose()
        'mainParentForm.mainDataManager.mainSQLDBConnection.Close()
        'tvOtherMetrics.Nodes.Add("Monthly sum evapotraspiration")
        'tvOtherMetrics.Nodes.Add("Monthly change in marsh volume")

        'tvDisplayMetrics.Nodes.Clear()
        For Each strDisplayMetric In MainForm.mainDataManager.getHabitatMetrics()
            tvDisplayMetrics.Nodes.Add(strDisplayMetric, strDisplayMetric)
        Next

        'add our species and lifestages to the form dropdowns
        tvSpecies.Nodes.Clear()
        For Each sps As String In mainParentForm.mainDataManager.getSpeciesNames()
            tvSpecies.Nodes.Add(sps, sps)
            For Each ls As String In mainParentForm.mainDataManager.getLifeStageNames(sps)
                tvSpecies.Nodes(sps).Nodes.Add(ls, ls)
                tvSpecies.Nodes(sps).Nodes.Find(ls, True)(0).Tag = mainParentForm.mainDataManager.getHydroperiod(sps, ls).getAsString()
            Next

        Next
        tvSpecies.ExpandAll()


    End Sub

    Public Sub loadChartDisplayData(cdd As chartDisplayData)
        'configures gui to have checked the passed data
        For Each item As TreeNode In tvScenario.Nodes
            item.Checked = cdd.scenarios.Contains(item.Text)
        Next
        cboBaseline.Text = cdd.baseline

        For Each item As TreeNode In tvTreatment.Nodes
            item.Checked = cdd.treatments.Contains(item.Text)
        Next

        tvRivers.Nodes(0).Checked = cdd.rivers.ContainsKey("aggAll")
        For Each n As TreeNode In tvRivers.Nodes("AggByRiver").Nodes
            n.Checked = cdd.rivers.Item(n.Name).Contains("aggRiver")
            For Each segNode As TreeNode In n.Nodes
                If cdd.rivers.ContainsKey(n.Name) Then
                    segNode.Checked = cdd.rivers(n.Name).Contains(segNode.Text)
                Else
                    segNode.Checked = False
                End If
            Next

        Next

        chkShowHydro.Checked = cdd.showHydro


        For Each speciesNode As TreeNode In tvSpecies.Nodes

            For Each lifestageNode As TreeNode In tvSpecies.Nodes(speciesNode.Text).Nodes
                If cdd.species.ContainsKey(speciesNode.Name) Then
                    Dim newls As lifestage = cdd.species(speciesNode.Text).Find(lifestage.FindPredicateByName(lifestageNode.Text))
                    If Not IsNothing(newls) Then
                        lifestageNode.Checked = True
                        lifestageNode.Tag = newls.hydroPeriod.getAsString
                    Else
                        lifestageNode.Checked = False
                    End If

                Else
                    lifestageNode.Checked = False
                End If
            Next

        Next

        If TypeOf mainParentForm Is HydrographGraphForm Or _
           TypeOf mainParentForm Is RawDataForm Then
            For Each item As TreeNode In tvOtherMetrics.Nodes
                item.Checked = cdd.otherMetrics.Contains(item.Text)
            Next
        End If

        If TypeOf mainParentForm Is HabitatGraphForm Or _
           TypeOf mainParentForm Is aggHabitatGraphForm Or _
           TypeOf mainParentForm Is RawDataForm Then
            For Each item As TreeNode In tvDisplayMetrics.Nodes
                item.Checked = cdd.displayMetrics.Contains(item.Text)
            Next
            rbYearly.Checked = (cdd.interval = "yearly")
        End If



        cboStart.Items.Clear()
        cboEnd.Items.Clear()

        If cdd.startYear < mainParentForm.mainDataManager.curStartYear Or _
           cdd.startYear > mainParentForm.mainDataManager.curEndYear Then
            cdd.startYear = mainParentForm.mainDataManager.curStartYear
        End If
        If cdd.endYear > mainParentForm.mainDataManager.curEndYear Or _
            cdd.endYear < mainParentForm.mainDataManager.curStartYear Then
            cdd.endYear = mainParentForm.mainDataManager.curEndYear
        End If


        cboStart.Text = cdd.startYear - 1
        cboEnd.Text = cdd.endYear


        For i As Integer = mainParentForm.mainDataManager.curStartYear - 1 To mainParentForm.mainDataManager.curEndYear
            cboStart.Items.Add(CStr(i))
            cboEnd.Items.Add(CStr(i))
        Next

        If cdd.interval = "daily" Then
            rbDaily.Checked = True
        ElseIf cdd.interval = "yearly" Then
            rbYearly.Checked = True
        Else
            rbEntire.Checked = True
        End If

    End Sub

    Public Function saveChartDisplayData() As chartDisplayData
        Dim cdd As New chartDisplayData
        cdd.scenarios.Clear()
        For Each item As TreeNode In tvScenario.Nodes
            If item.Text = cboBaseline.Text Then
                If item.Checked Then
                    cdd.scenarios.Insert(0, item.Text)
                End If
            Else
                If item.Checked Then
                    cdd.scenarios.Add(item.Text)
                End If
            End If

        Next

        cdd.baseline = cboBaseline.Text

        cdd.treatments.Clear()
        For Each item As TreeNode In tvTreatment.Nodes
            If item.Checked Then
                cdd.treatments.Add(item.Text)
            End If
        Next

        cdd.rivers.Clear()
        Dim NAlist As New List(Of String)
        NAlist.Add("aggRiver")
        If tvRivers.Nodes(0).Checked Then
            
            cdd.rivers.Add("aggAll", NAlist)
        End If

        For Each n As TreeNode In tvRivers.Nodes("AggByRiver").Nodes
            Dim listSegs As New List(Of String)
            If n.Checked Then
                listSegs.Add("aggRiver")
            End If

            For Each segNode As TreeNode In n.Nodes
                If segNode.Checked Then
                    listSegs.Add(segNode.Text)
                End If

            Next
            cdd.rivers.Add(n.Name, listSegs)

        Next

        cdd.showHydro = chkShowHydro.Checked

        cdd.species.Clear()
        For Each speciesNode As TreeNode In tvSpecies.Nodes
            Dim lifestageList As New List(Of lifestage)
            For Each lifestageNode As TreeNode In tvSpecies.Nodes(speciesNode.Text).Nodes
                If lifestageNode.Checked Then
                    Dim newls As New lifestage

                    newls.name = lifestageNode.Text
                    newls.hydroPeriod.setFromString(lifestageNode.Tag)
                    lifestageList.Add(newls)
                End If
            Next
            If lifestageList.Count > 0 Then
                cdd.species.Add(speciesNode.Text, lifestageList)
            End If
        Next

        If TypeOf mainParentForm Is HydrographGraphForm Or _
            TypeOf mainParentForm Is RawDataForm Then
            cdd.otherMetrics.Clear()
            For Each item As TreeNode In tvOtherMetrics.Nodes
                If item.Checked Then
                    cdd.otherMetrics.Add(item.Text)
                End If
            Next
        End If
        If TypeOf mainParentForm Is HabitatGraphForm Or _
            TypeOf mainParentForm Is aggHabitatGraphForm Or _
            TypeOf mainParentForm Is RawDataForm Then
            cdd.displayMetrics.Clear()
            For Each item As TreeNode In tvDisplayMetrics.Nodes
                If item.Checked Then
                    cdd.displayMetrics.Add(item.Text)
                End If
            Next
        End If

        cdd.startYear = cboStart.Text + 1
        cdd.endYear = cboEnd.Text

        If rbDaily.Checked Then
            cdd.interval = "daily"
        ElseIf rbYearly.Checked Then
            cdd.interval = "yearly"
        Else
            cdd.interval = "single"
        End If

        Try
            mainParentForm.bAcceptedNovelDate = False
        Catch ex As Exception

        End Try

        Return cdd

    End Function

    Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click
        Me.Cursor = Cursors.WaitCursor

        Dim newCDD As chartDisplayData = saveChartDisplayData()
        Dim strNewCDD As String = mainParentForm.mainDataManager.serializeChartDisplayDataToXML(newCDD).innerText
        Dim strOldCDD As String = mainParentForm.mainDataManager.serializeChartDisplayDataToXML(mainParentForm.curDisplayData).innerText

        If strNewCDD <> strOldCDD Then
            mainParentForm.dataChanged = True
        Else
            mainParentForm.dataChanged = False
        End If

        mainParentForm.curDisplayData = newCDD
        mainParentForm.loadData()
        Me.Cursor = Cursors.Default
        Me.Close()
    End Sub



    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub tvSpecies_AfterSelect(sender As Object, e As System.Windows.Forms.TreeViewEventArgs) Handles tvSpecies.AfterSelect
        loadLifeStageHydroPeriod()

    End Sub

    Private Sub tv_Click(sender As Object, e As System.EventArgs) Handles tvScenario.Click, tvOtherMetrics.Click, tvRivers.Click, tvTreatment.Click
        clearHydroperiodControls()
    End Sub

    Private Sub clearHydroperiodControls()
        HydroEventsOff = True
        For Each cntrl As Control In gbHydroperiod.Controls
            If TypeOf cntrl Is TextBox Then
                HydroEventsOff = True
                cntrl.Text = ""
            End If
            cntrl.Enabled = False
        Next
        HydroEventsOff = False
    End Sub

    Private Sub loadLifeStageHydroPeriod()
        For Each cntrl As Control In gbHydroperiod.Controls
            cntrl.Enabled = True
        Next

        Dim ls As New lifestage
        ls.name = tvSpecies.SelectedNode.Text
        Try
            HydroEventsOff = True
            ls.hydroPeriod.setFromString(tvSpecies.SelectedNode.Tag)
            txtStartMonth.Text = ls.hydroPeriod.startMonth
            txtStartDay.Text = ls.hydroPeriod.startDay
            txtEndMonth.Text = ls.hydroPeriod.endMonth
            txtEndDay.Text = ls.hydroPeriod.endDay
        Catch ex As Exception
            clearHydroperiodControls()
        Finally
            HydroEventsOff = False
        End Try

    End Sub
    Private Sub tvSpecies_Click(sender As Object, e As System.EventArgs) Handles tvSpecies.Click
        If Not IsNothing(tvSpecies.SelectedNode) Then
            loadLifeStageHydroPeriod()
        End If
    End Sub

    Private Sub txtStartMonth_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtStartMonth.Validated, txtStartDay.Validated, txtEndDay.Validated, txtEndMonth.Validated
        If Not HydroEventsOff Then
            tvSpecies.SelectedNode.Tag = txtStartMonth.Text & "|" & txtStartDay.Text & "|" & txtEndMonth.Text & "|" & txtEndDay.Text
            Debug.Print(tvSpecies.SelectedNode.Tag)
        End If

    End Sub

    Private Sub btnSaveHydroPeriod_Click(sender As System.Object, e As System.EventArgs) Handles btnSaveHydroPeriod.Click
        Dim ls As String = tvSpecies.SelectedNode.Text
        Dim sps As String = tvSpecies.SelectedNode.Parent.Text

        mainParentForm.mainDataManager.setHydroperiod(sps, ls, tvSpecies.SelectedNode.Tag)

    End Sub


    Private Sub btnMakeDefault_Click(sender As System.Object, e As System.EventArgs) Handles btnMakeDefault.Click
        mainParentForm.curDisplayData = saveChartDisplayData()
        mainParentForm.mainDataManager.setDefaultChartData(chartType, mainParentForm.mainDataManager.serializeChartDisplayDataToXML(mainParentForm.curDisplayData))
    End Sub



    Private Sub rbDaily_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDaily.CheckedChanged, rbEntire.CheckedChanged, rbYearly.CheckedChanged
        If rbDaily.Checked Then
            tpOtherMetrics.Visible = True
            tpDisplayMetrics.Visible = False
            pnlDailyFlow.Visible = True
            lblDailyFlow.Visible = True
        Else
            tpOtherMetrics.Visible = False
            tpDisplayMetrics.Visible = True
            pnlDailyFlow.Visible = False
            lblDailyFlow.Visible = False

            Dim cdd As chartDisplayData = mainParentForm.curDisplayData
            If cdd.displayMetrics.Count = 0 Then
                tvDisplayMetrics.Nodes(1).Checked = True

            End If
        End If

    End Sub

End Class

Public Class chartDisplayData

    Public showHydro As Boolean = False
    Public scenarios As New List(Of String)
    Public rivers As New SerializableDictionary(Of String, List(Of String))
    Public treatments As New List(Of String)
    Public species As New SerializableDictionary(Of String, List(Of lifestage))
    'for hydrograph only
    Public otherMetrics As New List(Of String)
    'for habitatGraphForm only
    Public displayMetrics As New List(Of String)
    Public startYear As Integer
    Public endYear As Integer
    Public interval As String
    Public baseline As String

    Public Sub New()

        'Set defaults
        'showHydro = True
        'scenarios.Add("aggAll")
        'rivers.Add("aggAll", Nothing)
        'species.Add("aggAll", Nothing)


    End Sub




End Class

Public Class lifestage
    Implements IComparable(Of lifestage)
    Public name As String
    Public hydroPeriod As New hydroperiod

    Public Function compareTo(ByVal other As lifestage) As Integer Implements System.IComparable(Of lifestage).CompareTo
        If name = other.name Then
            Return 0
        Else
            Return String.Compare(name, other.name)
        End If

    End Function

    Public Shared Function FindPredicateByName(ByVal lsname As String) As Predicate(Of lifestage)
        Return Function(ls2 As lifestage) lsname = ls2.name
    End Function
End Class

Public Class hydroperiod
    Public startMonth As Integer
    Public startDay As Integer
    Public endMonth As Integer
    Public endDay As Integer

    Public Sub setFromString(strHydroperiod As String)
        'string format is:
        'startMonth|startDay|endMonth|endDay
        If Not IsNothing(strHydroperiod) Then
            startMonth = strHydroperiod.Split("|")(0)
            startDay = strHydroperiod.Split("|")(1)
            endMonth = strHydroperiod.Split("|")(2)
            endDay = strHydroperiod.Split("|")(3)
        End If


    End Sub

    Public Function getAsString() As String
        'string format is:
        'startMonth|startDay|endMonth|endDay
        Return startMonth & "|" & startDay & "|" & endMonth & "|" & endDay
    End Function
End Class

#Region " IXmlSerializable Members "
<XmlRoot("dictionary")> _
Public Class SerializableDictionary(Of TKey, TValue)
    Inherits Dictionary(Of TKey, TValue)
    Implements System.Xml.Serialization.IXmlSerializable

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements IXmlSerializable.GetSchema
        Return Nothing
    End Function

    Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements IXmlSerializable.ReadXml

        Dim keySerializer As XmlSerializer = New XmlSerializer(GetType(TKey))
        Dim valueSerializer As XmlSerializer = New XmlSerializer(GetType(TValue))

        Dim wasEmpty As Boolean = reader.IsEmptyElement
        reader.Read()

        If (wasEmpty) Then Return

        While (reader.NodeType <> System.Xml.XmlNodeType.EndElement)

            reader.ReadStartElement("item")

            reader.ReadStartElement("key")

            Dim key As TKey = CType(keySerializer.Deserialize(reader), TKey)

            reader.ReadEndElement()

            reader.ReadStartElement("value")

            Dim value As TValue = CType(valueSerializer.Deserialize(reader), TValue)

            reader.ReadEndElement()

            Me.Add(key, value)

            reader.ReadEndElement()

            reader.MoveToContent()

        End While

        reader.ReadEndElement()

    End Sub

    Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements IXmlSerializable.WriteXml

        Dim keySerializer As New XmlSerializer(GetType(TKey))
        Dim valueSerializer As New XmlSerializer(GetType(TValue))

        For Each key As TKey In Me.Keys

            writer.WriteStartElement("item")

            writer.WriteStartElement("key")

            keySerializer.Serialize(writer, key)

            writer.WriteEndElement()

            writer.WriteStartElement("value")

            Dim value As TValue = Me(key)

            valueSerializer.Serialize(writer, value)

            writer.WriteEndElement()

            writer.WriteEndElement()

        Next key

    End Sub

End Class

#End Region
