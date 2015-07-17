<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class habGenProgressBar
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(habGenProgressBar))
        Me.pbTotalProgress = New System.Windows.Forms.ProgressBar()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.pbSegmentProgress = New System.Windows.Forms.ProgressBar()
        Me.lblFlowProgress = New System.Windows.Forms.Label()
        Me.pbFlowProgress = New System.Windows.Forms.ProgressBar()
        Me.lblSegProgress = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblFlow = New System.Windows.Forms.Label()
        Me.lblLifeStage = New System.Windows.Forms.Label()
        Me.lblSpecies = New System.Windows.Forms.Label()
        Me.lblTreatment = New System.Windows.Forms.Label()
        Me.lblSegment = New System.Windows.Forms.Label()
        Me.pbTreatmentProgress = New System.Windows.Forms.ProgressBar()
        Me.pbSpeciesProgress = New System.Windows.Forms.ProgressBar()
        Me.pbLifeStageProgress = New System.Windows.Forms.ProgressBar()
        Me.cboCoreCount = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.lblAllDone = New System.Windows.Forms.Label()
        Me.lblTimeElapsed = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'pbTotalProgress
        '
        Me.pbTotalProgress.Location = New System.Drawing.Point(30, 196)
        Me.pbTotalProgress.Name = "pbTotalProgress"
        Me.pbTotalProgress.Size = New System.Drawing.Size(724, 23)
        Me.pbTotalProgress.Step = 1
        Me.pbTotalProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.pbTotalProgress.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(30, 19)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(138, 16)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Creating output for:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(30, 176)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Total Progress"
        '
        'pbSegmentProgress
        '
        Me.pbSegmentProgress.Location = New System.Drawing.Point(344, 41)
        Me.pbSegmentProgress.Name = "pbSegmentProgress"
        Me.pbSegmentProgress.Size = New System.Drawing.Size(376, 13)
        Me.pbSegmentProgress.Step = 1
        Me.pbSegmentProgress.TabIndex = 3
        Me.pbSegmentProgress.Visible = False
        '
        'lblFlowProgress
        '
        Me.lblFlowProgress.AutoSize = True
        Me.lblFlowProgress.Location = New System.Drawing.Point(520, 22)
        Me.lblFlowProgress.Name = "lblFlowProgress"
        Me.lblFlowProgress.Size = New System.Drawing.Size(48, 13)
        Me.lblFlowProgress.TabIndex = 5
        Me.lblFlowProgress.Text = "Progress"
        '
        'pbFlowProgress
        '
        Me.pbFlowProgress.Location = New System.Drawing.Point(344, 83)
        Me.pbFlowProgress.Name = "pbFlowProgress"
        Me.pbFlowProgress.Size = New System.Drawing.Size(376, 13)
        Me.pbFlowProgress.Step = 1
        Me.pbFlowProgress.TabIndex = 6
        '
        'lblSegProgress
        '
        Me.lblSegProgress.AutoSize = True
        Me.lblSegProgress.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSegProgress.Location = New System.Drawing.Point(100, 41)
        Me.lblSegProgress.Name = "lblSegProgress"
        Me.lblSegProgress.Size = New System.Drawing.Size(60, 13)
        Me.lblSegProgress.TabIndex = 7
        Me.lblSegProgress.Text = "Segment:"
        Me.lblSegProgress.Visible = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(82, 62)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(78, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Time Period:"
        Me.Label4.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(104, 104)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(56, 13)
        Me.Label5.TabIndex = 9
        Me.Label5.Text = "Species:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(93, 125)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(67, 13)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "Life stage:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(123, 83)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(39, 13)
        Me.Label7.TabIndex = 11
        Me.Label7.Text = "SWE:"
        '
        'lblFlow
        '
        Me.lblFlow.AutoSize = True
        Me.lblFlow.Location = New System.Drawing.Point(161, 83)
        Me.lblFlow.Name = "lblFlow"
        Me.lblFlow.Size = New System.Drawing.Size(0, 13)
        Me.lblFlow.TabIndex = 16
        '
        'lblLifeStage
        '
        Me.lblLifeStage.AutoSize = True
        Me.lblLifeStage.Location = New System.Drawing.Point(161, 125)
        Me.lblLifeStage.Name = "lblLifeStage"
        Me.lblLifeStage.Size = New System.Drawing.Size(0, 13)
        Me.lblLifeStage.TabIndex = 15
        '
        'lblSpecies
        '
        Me.lblSpecies.AutoSize = True
        Me.lblSpecies.Location = New System.Drawing.Point(161, 104)
        Me.lblSpecies.Name = "lblSpecies"
        Me.lblSpecies.Size = New System.Drawing.Size(0, 13)
        Me.lblSpecies.TabIndex = 14
        '
        'lblTreatment
        '
        Me.lblTreatment.AutoSize = True
        Me.lblTreatment.Location = New System.Drawing.Point(161, 62)
        Me.lblTreatment.Name = "lblTreatment"
        Me.lblTreatment.Size = New System.Drawing.Size(0, 13)
        Me.lblTreatment.TabIndex = 13
        Me.lblTreatment.Visible = False
        '
        'lblSegment
        '
        Me.lblSegment.AutoSize = True
        Me.lblSegment.Location = New System.Drawing.Point(161, 41)
        Me.lblSegment.Name = "lblSegment"
        Me.lblSegment.Size = New System.Drawing.Size(0, 13)
        Me.lblSegment.TabIndex = 12
        Me.lblSegment.Visible = False
        '
        'pbTreatmentProgress
        '
        Me.pbTreatmentProgress.Location = New System.Drawing.Point(344, 62)
        Me.pbTreatmentProgress.Name = "pbTreatmentProgress"
        Me.pbTreatmentProgress.Size = New System.Drawing.Size(376, 13)
        Me.pbTreatmentProgress.Step = 1
        Me.pbTreatmentProgress.TabIndex = 17
        Me.pbTreatmentProgress.Visible = False
        '
        'pbSpeciesProgress
        '
        Me.pbSpeciesProgress.Location = New System.Drawing.Point(344, 104)
        Me.pbSpeciesProgress.Name = "pbSpeciesProgress"
        Me.pbSpeciesProgress.Size = New System.Drawing.Size(376, 13)
        Me.pbSpeciesProgress.Step = 1
        Me.pbSpeciesProgress.TabIndex = 18
        '
        'pbLifeStageProgress
        '
        Me.pbLifeStageProgress.Location = New System.Drawing.Point(344, 123)
        Me.pbLifeStageProgress.Name = "pbLifeStageProgress"
        Me.pbLifeStageProgress.Size = New System.Drawing.Size(376, 13)
        Me.pbLifeStageProgress.Step = 1
        Me.pbLifeStageProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.pbLifeStageProgress.TabIndex = 19
        '
        'cboCoreCount
        '
        Me.cboCoreCount.FormattingEnabled = True
        Me.cboCoreCount.Location = New System.Drawing.Point(703, 151)
        Me.cboCoreCount.Name = "cboCoreCount"
        Me.cboCoreCount.Size = New System.Drawing.Size(66, 21)
        Me.cboCoreCount.TabIndex = 22
        Me.cboCoreCount.Visible = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(603, 154)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(94, 13)
        Me.Label8.TabIndex = 23
        Me.Label8.Text = "Processors to use:"
        Me.Label8.Visible = False
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(331, 242)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 24
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        Me.btnOK.Visible = False
        '
        'lblAllDone
        '
        Me.lblAllDone.AutoSize = True
        Me.lblAllDone.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAllDone.Location = New System.Drawing.Point(244, 62)
        Me.lblAllDone.Name = "lblAllDone"
        Me.lblAllDone.Size = New System.Drawing.Size(263, 25)
        Me.lblAllDone.TabIndex = 25
        Me.lblAllDone.Text = "All processing complete"
        Me.lblAllDone.Visible = False
        '
        'lblTimeElapsed
        '
        Me.lblTimeElapsed.AutoSize = True
        Me.lblTimeElapsed.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTimeElapsed.Location = New System.Drawing.Point(245, 87)
        Me.lblTimeElapsed.Name = "lblTimeElapsed"
        Me.lblTimeElapsed.Size = New System.Drawing.Size(199, 20)
        Me.lblTimeElapsed.TabIndex = 26
        Me.lblTimeElapsed.Text = "All processing complete"
        Me.lblTimeElapsed.Visible = False
        '
        'habGenProgressBar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(778, 294)
        Me.Controls.Add(Me.lblTimeElapsed)
        Me.Controls.Add(Me.lblAllDone)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.cboCoreCount)
        Me.Controls.Add(Me.pbLifeStageProgress)
        Me.Controls.Add(Me.pbSpeciesProgress)
        Me.Controls.Add(Me.pbTreatmentProgress)
        Me.Controls.Add(Me.lblFlow)
        Me.Controls.Add(Me.lblLifeStage)
        Me.Controls.Add(Me.lblSpecies)
        Me.Controls.Add(Me.lblTreatment)
        Me.Controls.Add(Me.lblSegment)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblSegProgress)
        Me.Controls.Add(Me.pbFlowProgress)
        Me.Controls.Add(Me.lblFlowProgress)
        Me.Controls.Add(Me.pbSegmentProgress)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.pbTotalProgress)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "habGenProgressBar"
        Me.Text = "Habitat Generation Progress"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pbTotalProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents pbSegmentProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents lblFlowProgress As System.Windows.Forms.Label
    Friend WithEvents pbFlowProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents lblSegProgress As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblFlow As System.Windows.Forms.Label
    Friend WithEvents lblLifeStage As System.Windows.Forms.Label
    Friend WithEvents lblSpecies As System.Windows.Forms.Label
    Friend WithEvents lblTreatment As System.Windows.Forms.Label
    Friend WithEvents lblSegment As System.Windows.Forms.Label
    Friend WithEvents pbTreatmentProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents pbSpeciesProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents pbLifeStageProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents cboCoreCount As System.Windows.Forms.ComboBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents lblAllDone As System.Windows.Forms.Label
    Friend WithEvents lblTimeElapsed As System.Windows.Forms.Label
End Class
