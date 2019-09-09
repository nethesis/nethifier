<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FRM_UPDATE
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
        Me.components = New System.ComponentModel.Container()
        Me.PROGRESS = New System.Windows.Forms.ProgressBar()
        Me.BUT_CANCEL = New System.Windows.Forms.Button()
        Me.TMR = New System.Windows.Forms.Timer(Me.components)
        Me.LBL_DOWNLOAD = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'PROGRESS
        '
        Me.PROGRESS.Location = New System.Drawing.Point(12, 32)
        Me.PROGRESS.Name = "PROGRESS"
        Me.PROGRESS.Size = New System.Drawing.Size(262, 19)
        Me.PROGRESS.TabIndex = 2
        '
        'BUT_CANCEL
        '
        Me.BUT_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BUT_CANCEL.Location = New System.Drawing.Point(103, 64)
        Me.BUT_CANCEL.Name = "BUT_CANCEL"
        Me.BUT_CANCEL.Size = New System.Drawing.Size(75, 23)
        Me.BUT_CANCEL.TabIndex = 3
        Me.BUT_CANCEL.Text = "Annulla"
        Me.BUT_CANCEL.UseVisualStyleBackColor = True
        '
        'TMR
        '
        Me.TMR.Enabled = True
        Me.TMR.Interval = 5000
        '
        'LBL_DOWNLOAD
        '
        Me.LBL_DOWNLOAD.AutoSize = True
        Me.LBL_DOWNLOAD.Location = New System.Drawing.Point(12, 13)
        Me.LBL_DOWNLOAD.Name = "LBL_DOWNLOAD"
        Me.LBL_DOWNLOAD.Size = New System.Drawing.Size(0, 13)
        Me.LBL_DOWNLOAD.TabIndex = 4
        '
        'FRM_UPDATE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BUT_CANCEL
        Me.ClientSize = New System.Drawing.Size(289, 104)
        Me.ControlBox = False
        Me.Controls.Add(Me.LBL_DOWNLOAD)
        Me.Controls.Add(Me.BUT_CANCEL)
        Me.Controls.Add(Me.PROGRESS)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "FRM_UPDATE"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Aggiornamento...."
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PROGRESS As System.Windows.Forms.ProgressBar
    Friend WithEvents BUT_CANCEL As System.Windows.Forms.Button
    Friend WithEvents TMR As System.Windows.Forms.Timer
    Friend WithEvents LBL_DOWNLOAD As System.Windows.Forms.Label
End Class
