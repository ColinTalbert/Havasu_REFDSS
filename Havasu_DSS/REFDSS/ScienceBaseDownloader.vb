Imports System.Net
Imports Ionic.Zip
Imports System.IO

Public Class ScienceBaseDownloader


    Public parentMainForm As MainForm


    Private downloadzipfile As String
    Private rootDownloadFolder As String
    Private WithEvents WC As WebClient

    Dim nFilesCompleted As Integer
    Dim totalEntriesToProcess As Integer
    Dim _extractThread As System.Threading.Thread

    Dim _alreadyExtracting As Boolean = False
    Dim strSegment As String
    Dim strTreatment As String

    Public canceled As Boolean = False

    Public Sub New(mf As MainForm, seg As String, Optional treat As String = "None")

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        parentMainForm = mf
        strSegment = seg
        strTreatment = treat

        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        System.Diagnostics.Process.Start("https://www.sciencebase.gov/about/")
    End Sub

    Public Sub StartDownload()

        lblCurrentlyDownloading.Text = "Downloading InitialData.zip" 'data for:  " & strSegment ' & " " & strTreatment

        'Step one download the 


        Dim url As String
        If strSegment = "CoreData" Then
            'url = My.Settings.ScienceBaseCoreDataLink
            url = My.Settings.SB_Full
        Else
            url = parentMainForm.mainDataManager.getInputURL(strSegment, strTreatment)
        End If


        rootDownloadFolder = My.Settings.InputDataDirectory + "\" + "tempScienceBaseDownloads"
        If Not IO.Directory.Exists(rootDownloadFolder) Then
            IO.Directory.CreateDirectory(rootDownloadFolder)
        End If

        If strSegment = "CoreData" Then
            downloadzipfile = rootDownloadFolder + "\Full.zip"
        Else
            downloadzipfile = rootDownloadFolder + "\" + strSegment + parentMainForm.mainDataManager.getTreatmentAbbrev(strSegment, strTreatment) + ".zip"
        End If

        Dim jsonURL As String = url.Replace("https://www.sciencebase.gov/catalog/file/get", "https://www.sciencebase.gov/catalog/item")
        jsonURL = jsonURL.Replace("?name=Full.zip", "?format=json")


        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(jsonURL), HttpWebRequest)
        Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
        Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
        Dim jsonString = reader.ReadToEnd()
        Dim size As String = jsonString.Substring(jsonString.IndexOf("""size""") + 7)
        size = size.Substring(0, size.IndexOf("}"))
        pbDownloading.Maximum = CInt(size)
        reader.Close()
        response.Close()

        WC = New WebClient()
        WC.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)")
        WC.DownloadFileAsync(New Uri(url), downloadzipfile)
    End Sub

    Private Function getCoreURL() As String
        'Dim sourceString As String = New System.Net.WebClient().DownloadString(My.Settings.ScienceBaseLocatorURL)
        'Dim startKey As String = "<div id=""summaryBoxContent"" property=""description"">"
        'Dim endKey As String = "</div>"

        'sourceString = sourceString.Substring(sourceString.IndexOf(startKey) + Len(startKey))
        'Dim finalURL As String = sourceString.Substring(0, sourceString.IndexOf(endKey))
        'Return finalURL
    End Function

    Private Sub WC_DownloadProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs) Handles WC.DownloadProgressChanged
        pbDownloading.Value = e.BytesReceived

        If e.ProgressPercentage >= 100 Then
            lblCurrentlyDownloading.Text = "Finished downloading InitialData.zip"


            'We're done downloading the beast, unzip it into the right place
            Dim strOutDir As String
            Dim tmpOutFolder As String
            If strSegment = "CoreData" Then
                strOutDir = System.IO.Path.GetDirectoryName(My.Settings.InputDataDirectory)
                tmpOutFolder = rootDownloadFolder + "\CoreWorksace_tmp"
            Else
                strOutDir = My.Settings.InputDataDirectory + "\Segments\" + strSegment + parentMainForm.mainDataManager.getTreatmentAbbrev(strSegment, strTreatment)
                tmpOutFolder = rootDownloadFolder + "\" + strSegment + parentMainForm.mainDataManager.getTreatmentAbbrev(strSegment, strTreatment)
            End If

            If Not IO.Directory.Exists(tmpOutFolder) Then
                IO.Directory.CreateDirectory(tmpOutFolder)
            End If

            If Not _alreadyExtracting Then
                lblCurrentlyDownloading.Text = "Extracting..."
                Dim args(3) As Object
                args(0) = downloadzipfile
                args(1) = strOutDir
                'args(1) = tmpOutFolder
                args(2) = strOutDir
                _extractThread = New System.Threading.Thread(New System.Threading.ParameterizedThreadStart(AddressOf UnzipFile))
                _extractThread.Start(args)
                Me.Cursor = Cursors.WaitCursor
                _alreadyExtracting = True
            End If



        End If
    End Sub

    'Private Sub twoStageUnzip(ByVal args As Object())
    '    Dim extractCancelled As Boolean = False
    '    Dim zipToRead As String = args(0)
    '    Dim extractDir As String = args(1)
    '    Dim finalDir As String = args(2)
    '    Try
    '        Using zip As ZipFile = ZipFile.Read(zipToRead)
    '            totalEntriesToProcess = zip.Entries.Count
    '            SetProgressBarMax(zip.Entries.Count)
    '            AddHandler zip.ExtractProgress, New EventHandler(Of ExtractProgressEventArgs)(AddressOf Me.UnzipFile)
    '            zip.ExtractAll(extractDir, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
    '        End Using
    '    Catch ex1 As Exception
    '        MessageBox.Show(String.Format("There's been a problem extracting that zip file.  {0}", ex1.Message), "Error Extracting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
    '    End Try

    '    Try
    '        pbExtracting.Value = 0
    '        Dim secondZip As String = extractDir + "\" + whichOne + ".zip"
    '        Using zip As ZipFile = ZipFile.Read(secondZip)
    '            totalEntriesToProcess = zip.Entries.Count
    '            SetProgressBarMax(zip.Entries.Count)
    '            AddHandler zip.ExtractProgress, New EventHandler(Of ExtractProgressEventArgs)(AddressOf Me.zip_ExtractProgress)
    '            zip.ExtractAll(finalDir, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
    '        End Using
    '    Catch ex1 As Exception
    '        MessageBox.Show(String.Format("There's been a problem extracting that zip file.  {0}", ex1.Message), "Error Extracting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
    '    End Try


    '    Me.Cursor = Cursors.Default
    '    Me.Close()
    'End Sub

    Private Sub UnzipFile(ByVal args As Object())
        Dim extractCancelled As Boolean = False
        Dim zipToRead As String = args(0)
        Dim extractDir As String = args(1)
        Try
            Using zip As ZipFile = ZipFile.Read(zipToRead)
                totalEntriesToProcess = zip.Entries.Count
                SetProgressBarMax(zip.Entries.Count)
                AddHandler zip.ExtractProgress, New EventHandler(Of ExtractProgressEventArgs)(AddressOf Me.zip_ExtractProgress)
                zip.ExtractAll(extractDir, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
            End Using
        Catch ex1 As Exception
            MessageBox.Show(String.Format("There's been a problem extracting that zip file.  {0}", ex1.Message), "Error Extracting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End Try

        Me.Cursor = Cursors.Default
        System.IO.File.Delete(downloadzipfile)
        Me.Close()
    End Sub


    Private Sub SetProgressBarMax(ByVal n As Integer)
        If pbExtracting.InvokeRequired Then
            pbExtracting.Invoke(New Action(Of Integer)(AddressOf SetProgressBarMax), New Object() {n})
        Else
            pbExtracting.Maximum = n
            pbExtracting.Step = 1
        End If
    End Sub

    Private Sub zip_ExtractProgress(ByVal sender As Object, ByVal e As ExtractProgressEventArgs)
        If (e.EventType = Ionic.Zip.ZipProgressEventType.Extracting_AfterExtractEntry) Then
            StepEntryProgress(e)
        ElseIf (e.EventType = ZipProgressEventType.Extracting_BeforeExtractAll) Then
            'StepArchiveProgress(e)
        End If
    End Sub


    Private Sub StepEntryProgress(ByVal e As ZipProgressEventArgs)
        pbExtracting.PerformStep()
        System.Threading.Thread.Sleep(100)
        'set a label with status information
        nFilesCompleted = nFilesCompleted + 1
        lblExtracting.Text = String.Format("{0} of {1} files...({2})", nFilesCompleted, totalEntriesToProcess, e.CurrentEntry.FileName)
        Me.Update()
    End Sub











    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click
        canceled = True
        WC.CancelAsync()
        Me.Close()
    End Sub
End Class