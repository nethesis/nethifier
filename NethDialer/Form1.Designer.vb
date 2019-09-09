<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LBL_CHIAMATA = New System.Windows.Forms.Label()
        Me.LNK_CLEAR = New System.Windows.Forms.LinkLabel()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Nethifier (Dialer)"
        '
        'LBL_CHIAMATA
        '
        Me.LBL_CHIAMATA.AutoEllipsis = True
        Me.LBL_CHIAMATA.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LBL_CHIAMATA.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LBL_CHIAMATA.Location = New System.Drawing.Point(10, 27)
        Me.LBL_CHIAMATA.Name = "LBL_CHIAMATA"
        Me.LBL_CHIAMATA.Size = New System.Drawing.Size(258, 28)
        Me.LBL_CHIAMATA.TabIndex = 5
        '
        'LNK_CLEAR
        '
        Me.LNK_CLEAR.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LNK_CLEAR.AutoSize = True
        Me.LNK_CLEAR.LinkColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.LNK_CLEAR.Location = New System.Drawing.Point(254, 8)
        Me.LNK_CLEAR.Name = "LNK_CLEAR"
        Me.LNK_CLEAR.Size = New System.Drawing.Size(14, 13)
        Me.LNK_CLEAR.TabIndex = 4
        Me.LNK_CLEAR.TabStop = True
        Me.LNK_CLEAR.Text = "X"
        Me.LNK_CLEAR.Visible = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(275, 63)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.LBL_CHIAMATA)
        Me.Controls.Add(Me.LNK_CLEAR)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "Form1"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents LBL_CHIAMATA As System.Windows.Forms.Label
    Friend WithEvents LNK_CLEAR As System.Windows.Forms.LinkLabel

End Class
