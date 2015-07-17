Public Class WelcomeToTheDSS

    Public currentMainForm = Nothing

    Private Sub LinkLabel1_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        System.Diagnostics.Process.Start(getApplicationSetting("ScienceBaseLink"))
    End Sub


    Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click
        Dim folderBrowserDialog1 As New FolderBrowserDialog

        folderBrowserDialog1.Description = "Select the directory to save core data inputs into"
        If folderBrowserDialog1.ShowDialog = DialogResult.OK Then
            My.Settings.InputDataDirectory = folderBrowserDialog1.SelectedPath + "\Inputs"
            My.Settings.SessionDirectory = folderBrowserDialog1.SelectedPath + "\DefaultSessionDirectory"
            My.Settings.OutputDataDirectory = folderBrowserDialog1.SelectedPath + "\DefaultSessionDirectory\Outputs"
            My.Settings.ConfigXML = folderBrowserDialog1.SelectedPath + "\DefaultSessionDirectory\config.xml"
            My.Settings.SQliteDB = folderBrowserDialog1.SelectedPath + "\DefaultSessionDirectory\REFDSS_data.sqlite"
            My.Settings.Save()

            Dim x As New ScienceBaseDownloader(Me.currentMainForm, "CoreData")
            x.StartDownload()
            x.ShowDialog()

            If x.canceled Then
                'do nothing
            Else
                Me.Close()
            End If



        Else
            Exit Sub
        End If


    End Sub

    Private Sub WelcomeToTheDSS_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.Text = getApplicationSetting("WelcomeTitle")
        Label2.Text = "This appears to be the first time you have opened the " & vbCrLf & getApplicationSetting("ApplicationTitle")
    End Sub


    Private Sub Label4_Click(sender As System.Object, e As System.EventArgs) Handles Label4.Click

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
End Class