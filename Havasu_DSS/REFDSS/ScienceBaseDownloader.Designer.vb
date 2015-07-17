<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ScienceBaseDownloader
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ScienceBaseDownloader))
        Me.lblCurrentlyDownloading = New System.Windows.Forms.Label()
        Me.pbDownloading = New System.Windows.Forms.ProgressBar()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.pbExtracting = New System.Windows.Forms.ProgressBar()
        Me.lblExtracting = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblCurrentlyDownloading
        '
        Me.lblCurrentlyDownloading.AutoSize = True
        Me.lblCurrentlyDownloading.Location = New System.Drawing.Point(12, 9)
        Me.lblCurrentlyDownloading.Name = "lblCurrentlyDownloading"
        Me.lblCurrentlyDownloading.Size = New System.Drawing.Size(78, 13)
        Me.lblCurrentlyDownloading.TabIndex = 0
        Me.lblCurrentlyDownloading.Text = "Downloading:  "
        '
        'pbDownloading
        '
        Me.pbDownloading.Location = New System.Drawing.Point(15, 25)
        Me.pbDownloading.Name = "pbDownloading"
        Me.pbDownloading.Size = New System.Drawing.Size(828, 23)
        Me.pbDownloading.TabIndex = 1
        '
        'LinkLabel1
        '
        Me.LinkLabel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Location = New System.Drawing.Point(783, 142)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(101, 13)
        Me.LinkLabel1.TabIndex = 3
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "About ScienceBase"
        '
        'pbExtracting
        '
        Me.pbExtracting.Location = New System.Drawing.Point(17, 81)
        Me.pbExtracting.Name = "pbExtracting"
        Me.pbExtracting.Size = New System.Drawing.Size(828, 23)
        Me.pbExtracting.TabIndex = 5
        '
        'lblExtracting
        '
        Me.lblExtracting.AutoSize = True
        Me.lblExtracting.Location = New System.Drawing.Point(14, 65)
        Me.lblExtracting.Name = "lblExtracting"
        Me.lblExtracting.Size = New System.Drawing.Size(57, 13)
        Me.lblExtracting.TabIndex = 4
        Me.lblExtracting.Text = "Extracting:"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(420, 129)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 6
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'ScienceBaseDownloader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(896, 164)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.pbExtracting)
        Me.Controls.Add(Me.lblExtracting)
        Me.Controls.Add(Me.LinkLabel1)
        Me.Controls.Add(Me.pbDownloading)
        Me.Controls.Add(Me.lblCurrentlyDownloading)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ScienceBaseDownloader"
        Me.Text = "Download source data from ScienceBase"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblCurrentlyDownloading As System.Windows.Forms.Label
    Friend WithEvents pbDownloading As System.Windows.Forms.ProgressBar
    Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
    Friend WithEvents pbExtracting As System.Windows.Forms.ProgressBar
    Friend WithEvents lblExtracting As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
