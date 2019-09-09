<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FRM_NOTIFICATIONS
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
        Me.WEB_CONTAINER = New System.Windows.Forms.Panel()
        Me.LNK_CLEAR = New System.Windows.Forms.LinkLabel()
        Me.SuspendLayout()
        '
        'WEB_CONTAINER
        '
        Me.WEB_CONTAINER.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.WEB_CONTAINER.Location = New System.Drawing.Point(5, 5)
        Me.WEB_CONTAINER.Name = "WEB_CONTAINER"
        Me.WEB_CONTAINER.Size = New System.Drawing.Size(517, 117)
        Me.WEB_CONTAINER.TabIndex = 0
        '
        'LNK_CLEAR
        '
        Me.LNK_CLEAR.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LNK_CLEAR.AutoSize = True
        Me.LNK_CLEAR.LinkColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.LNK_CLEAR.Location = New System.Drawing.Point(470, 133)
        Me.LNK_CLEAR.Name = "LNK_CLEAR"
        Me.LNK_CLEAR.Size = New System.Drawing.Size(45, 13)
        Me.LNK_CLEAR.TabIndex = 1
        Me.LNK_CLEAR.TabStop = True
        Me.LNK_CLEAR.Text = "Clear All"
        '
        'FRM_NOTIFICATIONS
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Gainsboro
        Me.ClientSize = New System.Drawing.Size(527, 166)
        Me.ControlBox = False
        Me.Controls.Add(Me.LNK_CLEAR)
        Me.Controls.Add(Me.WEB_CONTAINER)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FRM_NOTIFICATIONS"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "FRM_NOTIFICATIONS"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents WEB_CONTAINER As System.Windows.Forms.Panel
    Friend WithEvents LNK_CLEAR As System.Windows.Forms.LinkLabel
End Class
