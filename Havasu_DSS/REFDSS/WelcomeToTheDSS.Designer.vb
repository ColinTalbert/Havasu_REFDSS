<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WelcomeToTheDSS
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WelcomeToTheDSS))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(19, 52)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(665, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "If this is the first time you have opened the application, you need to obtain the" & _
    " required inputs."
        '
        'LinkLabel1
        '
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Location = New System.Drawing.Point(439, 192)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(144, 13)
        Me.LinkLabel1.TabIndex = 1
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "WaterSmart on ScienceBase"
        Me.LinkLabel1.Visible = False
        '
        'Label2
        '
        Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(118, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(281, 20)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "This application needs initial data!"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(7, 400)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(332, 91)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = resources.GetString("Label3.Text")
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(252, 126)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(147, 23)
        Me.btnOK.TabIndex = 4
        Me.btnOK.Text = "Download Initial Data"
        Me.btnOK.UseVisualStyleBackColor = True
        Me.btnOK.Visible = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(615, 463)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(20, 92)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(313, 18)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "This can be accomplished in three ways:"
        Me.Label4.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(20, 128)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(233, 18)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "1) Auto download (recommended)"
        Me.Label5.Visible = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(20, 188)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(413, 18)
        Me.Label6.TabIndex = 8
        Me.Label6.Text = "2) Manual download from the following link (See user manual)"
        Me.Label6.Visible = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(20, 266)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(360, 18)
        Me.Label7.TabIndex = 9
        Me.Label7.Text = "3) Obtain the data from another user or the developer."
        Me.Label7.Visible = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(52, 151)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(446, 26)
        Me.Label8.TabIndex = 10
        Me.Label8.Text = "The application will download the required data as needed from USGS ScienceBase. " & _
    " " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Note: You must be connected to the internet on a reasonably fast connection f" & _
    "or this to work."
        Me.Label8.Visible = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(52, 207)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(493, 39)
        Me.Label9.TabIndex = 11
        Me.Label9.Text = resources.GetString("Label9.Text")
        Me.Label9.Visible = False
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(52, 286)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(325, 39)
        Me.Label10.TabIndex = 12
        Me.Label10.Text = "Obtain an existing set of inputs from another user or the developer.  " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.Label10.Visible = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(23, 313)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(612, 52)
        Me.Label11.TabIndex = 13
        Me.Label11.Text = resources.GetString("Label11.Text")
        Me.Label11.Visible = False
        '
        'WelcomeToTheDSS
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(702, 498)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.LinkLabel1)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "WelcomeToTheDSS"
        Me.Text = "Welcome to the Delaware WaterSmart DSS"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
End Class
