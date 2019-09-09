<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FRM_INSTALLER
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FRM_INSTALLER))
        Me.BUT_INSTALL = New System.Windows.Forms.Button()
        Me.BUT_CANCEL = New System.Windows.Forms.Button()
        Me.CHK_ALL_USERS = New System.Windows.Forms.CheckBox()
        Me.CHK_DESKTOP = New System.Windows.Forms.CheckBox()
        Me.BUT_FOLDER = New System.Windows.Forms.Button()
        Me.FOLDER = New System.Windows.Forms.FolderBrowserDialog()
        Me.TXT_FOLDER = New System.Windows.Forms.TextBox()
        Me.LBL_PROGRAMMA = New System.Windows.Forms.Label()
        Me.PAN_INSTALL = New System.Windows.Forms.Panel()
        Me.CHK_RUN_AFTER_INSTALL = New System.Windows.Forms.CheckBox()
        Me.CHK_STARTUP = New System.Windows.Forms.CheckBox()
        Me.PAN_UNINSTALL = New System.Windows.Forms.Panel()
        Me.CHK_ELIMINA_CONFIG = New System.Windows.Forms.CheckBox()
        Me.PAN_INSTALL.SuspendLayout()
        Me.PAN_UNINSTALL.SuspendLayout()
        Me.SuspendLayout()
        '
        'BUT_INSTALL
        '
        Me.BUT_INSTALL.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BUT_INSTALL.Location = New System.Drawing.Point(256, 180)
        Me.BUT_INSTALL.Name = "BUT_INSTALL"
        Me.BUT_INSTALL.Size = New System.Drawing.Size(89, 25)
        Me.BUT_INSTALL.TabIndex = 5
        Me.BUT_INSTALL.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.BUT_INSTALL.UseVisualStyleBackColor = True
        '
        'BUT_CANCEL
        '
        Me.BUT_CANCEL.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BUT_CANCEL.Location = New System.Drawing.Point(351, 180)
        Me.BUT_CANCEL.Name = "BUT_CANCEL"
        Me.BUT_CANCEL.Size = New System.Drawing.Size(89, 25)
        Me.BUT_CANCEL.TabIndex = 6
        Me.BUT_CANCEL.UseVisualStyleBackColor = True
        '
        'CHK_ALL_USERS
        '
        Me.CHK_ALL_USERS.AutoSize = True
        Me.CHK_ALL_USERS.Location = New System.Drawing.Point(18, 13)
        Me.CHK_ALL_USERS.Name = "CHK_ALL_USERS"
        Me.CHK_ALL_USERS.Size = New System.Drawing.Size(15, 14)
        Me.CHK_ALL_USERS.TabIndex = 0
        Me.CHK_ALL_USERS.UseVisualStyleBackColor = True
        '
        'CHK_DESKTOP
        '
        Me.CHK_DESKTOP.AutoSize = True
        Me.CHK_DESKTOP.Checked = True
        Me.CHK_DESKTOP.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CHK_DESKTOP.Location = New System.Drawing.Point(18, 37)
        Me.CHK_DESKTOP.Name = "CHK_DESKTOP"
        Me.CHK_DESKTOP.Size = New System.Drawing.Size(15, 14)
        Me.CHK_DESKTOP.TabIndex = 1
        Me.CHK_DESKTOP.UseVisualStyleBackColor = True
        '
        'BUT_FOLDER
        '
        Me.BUT_FOLDER.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.BUT_FOLDER.Image = CType(resources.GetObject("BUT_FOLDER.Image"), System.Drawing.Image)
        Me.BUT_FOLDER.Location = New System.Drawing.Point(378, 128)
        Me.BUT_FOLDER.Name = "BUT_FOLDER"
        Me.BUT_FOLDER.Size = New System.Drawing.Size(30, 20)
        Me.BUT_FOLDER.TabIndex = 4
        Me.BUT_FOLDER.UseVisualStyleBackColor = True
        '
        'TXT_FOLDER
        '
        Me.TXT_FOLDER.Location = New System.Drawing.Point(18, 128)
        Me.TXT_FOLDER.Name = "TXT_FOLDER"
        Me.TXT_FOLDER.Size = New System.Drawing.Size(358, 20)
        Me.TXT_FOLDER.TabIndex = 2
        Me.TXT_FOLDER.TabStop = False
        '
        'LBL_PROGRAMMA
        '
        Me.LBL_PROGRAMMA.AutoSize = True
        Me.LBL_PROGRAMMA.Location = New System.Drawing.Point(18, 110)
        Me.LBL_PROGRAMMA.Name = "LBL_PROGRAMMA"
        Me.LBL_PROGRAMMA.Size = New System.Drawing.Size(0, 13)
        Me.LBL_PROGRAMMA.TabIndex = 6
        '
        'PAN_INSTALL
        '
        Me.PAN_INSTALL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PAN_INSTALL.Controls.Add(Me.CHK_RUN_AFTER_INSTALL)
        Me.PAN_INSTALL.Controls.Add(Me.CHK_STARTUP)
        Me.PAN_INSTALL.Controls.Add(Me.CHK_ALL_USERS)
        Me.PAN_INSTALL.Controls.Add(Me.LBL_PROGRAMMA)
        Me.PAN_INSTALL.Controls.Add(Me.CHK_DESKTOP)
        Me.PAN_INSTALL.Controls.Add(Me.TXT_FOLDER)
        Me.PAN_INSTALL.Controls.Add(Me.BUT_FOLDER)
        Me.PAN_INSTALL.Location = New System.Drawing.Point(12, 12)
        Me.PAN_INSTALL.Name = "PAN_INSTALL"
        Me.PAN_INSTALL.Size = New System.Drawing.Size(428, 162)
        Me.PAN_INSTALL.TabIndex = 7
        '
        'CHK_RUN_AFTER_INSTALL
        '
        Me.CHK_RUN_AFTER_INSTALL.AutoSize = True
        Me.CHK_RUN_AFTER_INSTALL.Checked = True
        Me.CHK_RUN_AFTER_INSTALL.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CHK_RUN_AFTER_INSTALL.Location = New System.Drawing.Point(18, 82)
        Me.CHK_RUN_AFTER_INSTALL.Name = "CHK_RUN_AFTER_INSTALL"
        Me.CHK_RUN_AFTER_INSTALL.Size = New System.Drawing.Size(15, 14)
        Me.CHK_RUN_AFTER_INSTALL.TabIndex = 11
        Me.CHK_RUN_AFTER_INSTALL.UseVisualStyleBackColor = True
        '
        'CHK_STARTUP
        '
        Me.CHK_STARTUP.AutoSize = True
        Me.CHK_STARTUP.Checked = True
        Me.CHK_STARTUP.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CHK_STARTUP.Location = New System.Drawing.Point(18, 60)
        Me.CHK_STARTUP.Name = "CHK_STARTUP"
        Me.CHK_STARTUP.Size = New System.Drawing.Size(15, 14)
        Me.CHK_STARTUP.TabIndex = 10
        Me.CHK_STARTUP.UseVisualStyleBackColor = True
        '
        'PAN_UNINSTALL
        '
        Me.PAN_UNINSTALL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PAN_UNINSTALL.Controls.Add(Me.CHK_ELIMINA_CONFIG)
        Me.PAN_UNINSTALL.Location = New System.Drawing.Point(12, 12)
        Me.PAN_UNINSTALL.Name = "PAN_UNINSTALL"
        Me.PAN_UNINSTALL.Size = New System.Drawing.Size(428, 59)
        Me.PAN_UNINSTALL.TabIndex = 8
        '
        'CHK_ELIMINA_CONFIG
        '
        Me.CHK_ELIMINA_CONFIG.AutoSize = True
        Me.CHK_ELIMINA_CONFIG.Location = New System.Drawing.Point(18, 19)
        Me.CHK_ELIMINA_CONFIG.Name = "CHK_ELIMINA_CONFIG"
        Me.CHK_ELIMINA_CONFIG.Size = New System.Drawing.Size(15, 14)
        Me.CHK_ELIMINA_CONFIG.TabIndex = 1
        Me.CHK_ELIMINA_CONFIG.UseVisualStyleBackColor = True
        '
        'FRM_INSTALLER
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(453, 217)
        Me.ControlBox = False
        Me.Controls.Add(Me.PAN_INSTALL)
        Me.Controls.Add(Me.PAN_UNINSTALL)
        Me.Controls.Add(Me.BUT_CANCEL)
        Me.Controls.Add(Me.BUT_INSTALL)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FRM_INSTALLER"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FRM_INSTALLER"
        Me.TopMost = True
        Me.PAN_INSTALL.ResumeLayout(False)
        Me.PAN_INSTALL.PerformLayout()
        Me.PAN_UNINSTALL.ResumeLayout(False)
        Me.PAN_UNINSTALL.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BUT_INSTALL As System.Windows.Forms.Button
    Friend WithEvents BUT_CANCEL As System.Windows.Forms.Button
    Friend WithEvents CHK_ALL_USERS As System.Windows.Forms.CheckBox
    Friend WithEvents CHK_DESKTOP As System.Windows.Forms.CheckBox
    Friend WithEvents BUT_FOLDER As System.Windows.Forms.Button
    Friend WithEvents FOLDER As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents TXT_FOLDER As System.Windows.Forms.TextBox
    Friend WithEvents LBL_PROGRAMMA As System.Windows.Forms.Label
    Friend WithEvents PAN_INSTALL As System.Windows.Forms.Panel
    Friend WithEvents PAN_UNINSTALL As System.Windows.Forms.Panel
    Friend WithEvents CHK_ELIMINA_CONFIG As System.Windows.Forms.CheckBox
    Friend WithEvents CHK_STARTUP As System.Windows.Forms.CheckBox
    Friend WithEvents CHK_RUN_AFTER_INSTALL As System.Windows.Forms.CheckBox
End Class
