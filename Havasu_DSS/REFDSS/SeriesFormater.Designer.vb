<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SeriesFormater
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
        Me.btnOK = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnColor = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cboWidth = New System.Windows.Forms.ComboBox()
        Me.cboStyle = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cboMarkers = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtLabel = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblSeriesName = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnOK
        '
        Me.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.btnOK.Location = New System.Drawing.Point(174, 223)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(81, 56)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(31, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Color"
        '
        'btnColor
        '
        Me.btnColor.Location = New System.Drawing.Point(113, 49)
        Me.btnColor.Name = "btnColor"
        Me.btnColor.Size = New System.Drawing.Size(100, 23)
        Me.btnColor.TabIndex = 3
        Me.btnColor.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(77, 89)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(35, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Width"
        '
        'cboWidth
        '
        Me.cboWidth.FormattingEnabled = True
        Me.cboWidth.Items.AddRange(New Object() {"1", "2", "3", "4", "5"})
        Me.cboWidth.Location = New System.Drawing.Point(113, 85)
        Me.cboWidth.Name = "cboWidth"
        Me.cboWidth.Size = New System.Drawing.Size(100, 21)
        Me.cboWidth.TabIndex = 5
        Me.cboWidth.Text = "1"
        '
        'cboStyle
        '
        Me.cboStyle.FormattingEnabled = True
        Me.cboStyle.Location = New System.Drawing.Point(113, 119)
        Me.cboStyle.Name = "cboStyle"
        Me.cboStyle.Size = New System.Drawing.Size(100, 21)
        Me.cboStyle.TabIndex = 7
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(77, 122)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(30, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Style"
        '
        'cboMarkers
        '
        Me.cboMarkers.FormattingEnabled = True
        Me.cboMarkers.Location = New System.Drawing.Point(113, 153)
        Me.cboMarkers.Name = "cboMarkers"
        Me.cboMarkers.Size = New System.Drawing.Size(100, 21)
        Me.cboMarkers.TabIndex = 9
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(67, 155)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(45, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Markers"
        '
        'txtLabel
        '
        Me.txtLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLabel.Location = New System.Drawing.Point(62, 188)
        Me.txtLabel.Name = "txtLabel"
        Me.txtLabel.Size = New System.Drawing.Size(343, 20)
        Me.txtLabel.TabIndex = 10
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(20, 192)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(33, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "Label"
        '
        'lblSeriesName
        '
        Me.lblSeriesName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSeriesName.AutoSize = True
        Me.lblSeriesName.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSeriesName.Location = New System.Drawing.Point(13, 13)
        Me.lblSeriesName.Name = "lblSeriesName"
        Me.lblSeriesName.Size = New System.Drawing.Size(49, 16)
        Me.lblSeriesName.TabIndex = 12
        Me.lblSeriesName.Text = "Label6"
        '
        'SeriesFormater
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(420, 258)
        Me.Controls.Add(Me.lblSeriesName)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.txtLabel)
        Me.Controls.Add(Me.cboMarkers)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.cboStyle)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.cboWidth)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnColor)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnOK)
        Me.Name = "SeriesFormater"
        Me.ShowIcon = False
        Me.Text = "Format Series"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnColor As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cboWidth As System.Windows.Forms.ComboBox
    Friend WithEvents cboStyle As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cboMarkers As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtLabel As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblSeriesName As System.Windows.Forms.Label
End Class
