Public Class importScenarioProgressBar
    Public canceled As Boolean = False
    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click
        canceled = True
    End Sub
End Class