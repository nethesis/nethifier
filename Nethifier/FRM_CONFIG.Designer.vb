<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FRM_CONFIG
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FRM_CONFIG))
        Me.BUT_CONNECT = New System.Windows.Forms.Button()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.STRIP_STATUS = New System.Windows.Forms.ToolStripStatusLabel()
        Me.gbIpAddress = New System.Windows.Forms.GroupBox()
        Me.TXT_SERVER = New System.Windows.Forms.TextBox()
        Me.gbServerPort = New System.Windows.Forms.GroupBox()
        Me.TXT_PORT = New System.Windows.Forms.TextBox()
        Me.PingServer = New System.Windows.Forms.Timer(Me.components)
        Me.NOTIFY = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.CTX_MENU = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ConnectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.CallToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.LanguageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.INFO = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TABS = New System.Windows.Forms.TabControl()
        Me.TAB_001 = New System.Windows.Forms.TabPage()
        Me.STARTUP = New System.Windows.Forms.CheckBox()
        Me.LBL_NOTIFICA_CLOSE = New System.Windows.Forms.Label()
        Me.NUM_NOTIFICA = New System.Windows.Forms.NumericUpDown()
        Me.GRP_AUTH = New System.Windows.Forms.GroupBox()
        Me.LBL_PASSWORD = New System.Windows.Forms.Label()
        Me.LBL_USER = New System.Windows.Forms.Label()
        Me.TXT_PASSWORD = New System.Windows.Forms.TextBox()
        Me.TXT_USERNAME = New System.Windows.Forms.TextBox()
        Me.LBL_URL = New System.Windows.Forms.Label()
        Me.TXT_AUTHEN = New System.Windows.Forms.TextBox()
        Me.LBL_MODALITA = New System.Windows.Forms.Label()
        Me.CMB_MODE = New System.Windows.Forms.ComboBox()
        Me.AUTO_LOGIN = New System.Windows.Forms.CheckBox()
        Me.TAB_002 = New System.Windows.Forms.TabPage()
        Me.TXT_SERVER_MESSAGES = New System.Windows.Forms.TextBox()
        Me.TAB_003 = New System.Windows.Forms.TabPage()
        Me.TXT_ERROR_LOG = New System.Windows.Forms.TextBox()
        Me.TAB_004 = New System.Windows.Forms.TabPage()
        Me.LBL_RUN = New System.Windows.Forms.Label()
        Me.LBL_COMMAND = New System.Windows.Forms.Label()
        Me.PAN_CONTROLS = New System.Windows.Forms.Panel()
        Me.TAB_005 = New System.Windows.Forms.TabPage()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TXT_REDIAL_HOTKEY = New System.Windows.Forms.TextBox()
        Me.CMB_REDIAL_HOTKEY = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TXT_SPEEDDIAL_HOTKEY = New System.Windows.Forms.TextBox()
        Me.CMB_SPEEDDIAL_HOTKEY = New System.Windows.Forms.ComboBox()
        Me.TAB_006 = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.CHK_ANSWER = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.CHK_ONEKEY = New System.Windows.Forms.CheckBox()
        Me.CHK_SUONERIA = New System.Windows.Forms.CheckBox()
        Me.GRP_SUONERIA = New System.Windows.Forms.GroupBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CMB_DEVICE = New System.Windows.Forms.ComboBox()
        Me.CHK_SOUND_LOOP = New System.Windows.Forms.CheckBox()
        Me.BUT_TRY = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TXT_SOUND = New System.Windows.Forms.TextBox()
        Me.BUT_SFOGLIA = New System.Windows.Forms.Button()
        Me.BUT_SAVE = New System.Windows.Forms.Button()
        Me.FILE_DLG = New System.Windows.Forms.OpenFileDialog()
        Me.TMR_CONNECTION = New System.Windows.Forms.Timer(Me.components)
        Me.BUT_RESET_RUNWITH = New System.Windows.Forms.Button()
        Me.TIP = New System.Windows.Forms.ToolTip(Me.components)
        Me.TMR_ELAPSE = New System.Windows.Forms.Timer(Me.components)
        Me.TMR_ICONS = New System.Windows.Forms.Timer(Me.components)
        Me.TMR_DEVICE = New System.Windows.Forms.Timer(Me.components)
        Me.TAB_007 = New System.Windows.Forms.TabPage()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.GRP_PARAMS = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.CMB_PARAM = New System.Windows.Forms.ComboBox()
        Me.StatusStrip1.SuspendLayout()
        Me.gbIpAddress.SuspendLayout()
        Me.gbServerPort.SuspendLayout()
        Me.CTX_MENU.SuspendLayout()
        Me.TABS.SuspendLayout()
        Me.TAB_001.SuspendLayout()
        CType(Me.NUM_NOTIFICA, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GRP_AUTH.SuspendLayout()
        Me.TAB_002.SuspendLayout()
        Me.TAB_003.SuspendLayout()
        Me.TAB_004.SuspendLayout()
        Me.TAB_005.SuspendLayout()
        Me.TAB_006.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GRP_SUONERIA.SuspendLayout()
        Me.TAB_007.SuspendLayout()
        Me.GRP_PARAMS.SuspendLayout()
        Me.SuspendLayout()
        '
        'BUT_CONNECT
        '
        Me.BUT_CONNECT.Location = New System.Drawing.Point(317, 18)
        Me.BUT_CONNECT.Margin = New System.Windows.Forms.Padding(2)
        Me.BUT_CONNECT.Name = "BUT_CONNECT"
        Me.BUT_CONNECT.Size = New System.Drawing.Size(72, 38)
        Me.BUT_CONNECT.TabIndex = 2
        Me.BUT_CONNECT.UseVisualStyleBackColor = True
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(29, 17)
        Me.ToolStripStatusLabel1.Text = "Idle."
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.STRIP_STATUS})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 326)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(1, 0, 10, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(418, 22)
        Me.StatusStrip1.TabIndex = 4
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'STRIP_STATUS
        '
        Me.STRIP_STATUS.Name = "STRIP_STATUS"
        Me.STRIP_STATUS.Size = New System.Drawing.Size(0, 17)
        '
        'gbIpAddress
        '
        Me.gbIpAddress.Controls.Add(Me.TXT_SERVER)
        Me.gbIpAddress.Location = New System.Drawing.Point(8, 18)
        Me.gbIpAddress.Margin = New System.Windows.Forms.Padding(2)
        Me.gbIpAddress.Name = "gbIpAddress"
        Me.gbIpAddress.Padding = New System.Windows.Forms.Padding(2)
        Me.gbIpAddress.Size = New System.Drawing.Size(228, 38)
        Me.gbIpAddress.TabIndex = 9
        Me.gbIpAddress.TabStop = False
        '
        'TXT_SERVER
        '
        Me.TXT_SERVER.Location = New System.Drawing.Point(4, 15)
        Me.TXT_SERVER.Margin = New System.Windows.Forms.Padding(2)
        Me.TXT_SERVER.Name = "TXT_SERVER"
        Me.TXT_SERVER.Size = New System.Drawing.Size(219, 20)
        Me.TXT_SERVER.TabIndex = 0
        '
        'gbServerPort
        '
        Me.gbServerPort.Controls.Add(Me.TXT_PORT)
        Me.gbServerPort.Location = New System.Drawing.Point(239, 18)
        Me.gbServerPort.Margin = New System.Windows.Forms.Padding(2)
        Me.gbServerPort.Name = "gbServerPort"
        Me.gbServerPort.Padding = New System.Windows.Forms.Padding(2)
        Me.gbServerPort.Size = New System.Drawing.Size(74, 38)
        Me.gbServerPort.TabIndex = 10
        Me.gbServerPort.TabStop = False
        '
        'TXT_PORT
        '
        Me.TXT_PORT.Location = New System.Drawing.Point(4, 15)
        Me.TXT_PORT.Margin = New System.Windows.Forms.Padding(2)
        Me.TXT_PORT.Name = "TXT_PORT"
        Me.TXT_PORT.Size = New System.Drawing.Size(62, 20)
        Me.TXT_PORT.TabIndex = 1
        Me.TXT_PORT.Text = "8182"
        '
        'PingServer
        '
        Me.PingServer.Interval = 30000
        '
        'NOTIFY
        '
        Me.NOTIFY.ContextMenuStrip = Me.CTX_MENU
        Me.NOTIFY.Icon = CType(resources.GetObject("NOTIFY.Icon"), System.Drawing.Icon)
        Me.NOTIFY.Visible = True
        '
        'CTX_MENU
        '
        Me.CTX_MENU.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.CTX_MENU.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ConnectToolStripMenuItem, Me.ShowToolStripMenuItem, Me.ToolStripMenuItem2, Me.CallToolStripMenuItem, Me.ToolStripMenuItem3, Me.LanguageToolStripMenuItem, Me.INFO, Me.ToolStripMenuItem1, Me.ExitToolStripMenuItem})
        Me.CTX_MENU.Name = "CTX_MENU"
        Me.CTX_MENU.Size = New System.Drawing.Size(127, 154)
        '
        'ConnectToolStripMenuItem
        '
        Me.ConnectToolStripMenuItem.Name = "ConnectToolStripMenuItem"
        Me.ConnectToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.ConnectToolStripMenuItem.Text = "Connect"
        '
        'ShowToolStripMenuItem
        '
        Me.ShowToolStripMenuItem.Name = "ShowToolStripMenuItem"
        Me.ShowToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.ShowToolStripMenuItem.Text = "Show"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(123, 6)
        '
        'CallToolStripMenuItem
        '
        Me.CallToolStripMenuItem.Name = "CallToolStripMenuItem"
        Me.CallToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.CallToolStripMenuItem.Text = "Call"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(123, 6)
        '
        'LanguageToolStripMenuItem
        '
        Me.LanguageToolStripMenuItem.Name = "LanguageToolStripMenuItem"
        Me.LanguageToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.LanguageToolStripMenuItem.Text = "Language"
        '
        'INFO
        '
        Me.INFO.Name = "INFO"
        Me.INFO.Size = New System.Drawing.Size(126, 22)
        Me.INFO.Text = "Info"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(123, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'TABS
        '
        Me.TABS.Controls.Add(Me.TAB_001)
        Me.TABS.Controls.Add(Me.TAB_002)
        Me.TABS.Controls.Add(Me.TAB_003)
        Me.TABS.Controls.Add(Me.TAB_004)
        Me.TABS.Controls.Add(Me.TAB_005)
        Me.TABS.Controls.Add(Me.TAB_006)
        Me.TABS.Controls.Add(Me.TAB_007)
        Me.TABS.Location = New System.Drawing.Point(0, 1)
        Me.TABS.Name = "TABS"
        Me.TABS.SelectedIndex = 0
        Me.TABS.Size = New System.Drawing.Size(413, 291)
        Me.TABS.TabIndex = 9
        '
        'TAB_001
        '
        Me.TAB_001.Controls.Add(Me.STARTUP)
        Me.TAB_001.Controls.Add(Me.LBL_NOTIFICA_CLOSE)
        Me.TAB_001.Controls.Add(Me.NUM_NOTIFICA)
        Me.TAB_001.Controls.Add(Me.GRP_AUTH)
        Me.TAB_001.Controls.Add(Me.gbIpAddress)
        Me.TAB_001.Controls.Add(Me.AUTO_LOGIN)
        Me.TAB_001.Controls.Add(Me.BUT_CONNECT)
        Me.TAB_001.Controls.Add(Me.gbServerPort)
        Me.TAB_001.Location = New System.Drawing.Point(4, 22)
        Me.TAB_001.Name = "TAB_001"
        Me.TAB_001.Padding = New System.Windows.Forms.Padding(3)
        Me.TAB_001.Size = New System.Drawing.Size(405, 265)
        Me.TAB_001.TabIndex = 0
        Me.TAB_001.UseVisualStyleBackColor = True
        '
        'STARTUP
        '
        Me.STARTUP.AutoSize = True
        Me.STARTUP.Checked = True
        Me.STARTUP.CheckState = System.Windows.Forms.CheckState.Checked
        Me.STARTUP.Location = New System.Drawing.Point(17, 168)
        Me.STARTUP.Name = "STARTUP"
        Me.STARTUP.Size = New System.Drawing.Size(15, 14)
        Me.STARTUP.TabIndex = 15
        Me.STARTUP.UseVisualStyleBackColor = True
        Me.STARTUP.Visible = False
        '
        'LBL_NOTIFICA_CLOSE
        '
        Me.LBL_NOTIFICA_CLOSE.AutoSize = True
        Me.LBL_NOTIFICA_CLOSE.Location = New System.Drawing.Point(14, 215)
        Me.LBL_NOTIFICA_CLOSE.Name = "LBL_NOTIFICA_CLOSE"
        Me.LBL_NOTIFICA_CLOSE.Size = New System.Drawing.Size(61, 13)
        Me.LBL_NOTIFICA_CLOSE.TabIndex = 13
        Me.LBL_NOTIFICA_CLOSE.Text = "Close notify"
        Me.LBL_NOTIFICA_CLOSE.Visible = False
        '
        'NUM_NOTIFICA
        '
        Me.NUM_NOTIFICA.Location = New System.Drawing.Point(177, 212)
        Me.NUM_NOTIFICA.Maximum = New Decimal(New Integer() {60, 0, 0, 0})
        Me.NUM_NOTIFICA.Name = "NUM_NOTIFICA"
        Me.NUM_NOTIFICA.Size = New System.Drawing.Size(39, 20)
        Me.NUM_NOTIFICA.TabIndex = 8
        Me.TIP.SetToolTip(Me.NUM_NOTIFICA, "eddd" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "ddddd" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.NUM_NOTIFICA.Visible = False
        '
        'GRP_AUTH
        '
        Me.GRP_AUTH.Controls.Add(Me.LBL_PASSWORD)
        Me.GRP_AUTH.Controls.Add(Me.LBL_USER)
        Me.GRP_AUTH.Controls.Add(Me.TXT_PASSWORD)
        Me.GRP_AUTH.Controls.Add(Me.TXT_USERNAME)
        Me.GRP_AUTH.Controls.Add(Me.LBL_URL)
        Me.GRP_AUTH.Controls.Add(Me.TXT_AUTHEN)
        Me.GRP_AUTH.Controls.Add(Me.LBL_MODALITA)
        Me.GRP_AUTH.Controls.Add(Me.CMB_MODE)
        Me.GRP_AUTH.Location = New System.Drawing.Point(8, 63)
        Me.GRP_AUTH.Name = "GRP_AUTH"
        Me.GRP_AUTH.Size = New System.Drawing.Size(381, 89)
        Me.GRP_AUTH.TabIndex = 11
        Me.GRP_AUTH.TabStop = False
        '
        'LBL_PASSWORD
        '
        Me.LBL_PASSWORD.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LBL_PASSWORD.AutoSize = True
        Me.LBL_PASSWORD.Location = New System.Drawing.Point(6, 55)
        Me.LBL_PASSWORD.Name = "LBL_PASSWORD"
        Me.LBL_PASSWORD.Size = New System.Drawing.Size(0, 13)
        Me.LBL_PASSWORD.TabIndex = 22
        '
        'LBL_USER
        '
        Me.LBL_USER.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LBL_USER.AutoSize = True
        Me.LBL_USER.Location = New System.Drawing.Point(6, 26)
        Me.LBL_USER.Name = "LBL_USER"
        Me.LBL_USER.Size = New System.Drawing.Size(0, 13)
        Me.LBL_USER.TabIndex = 21
        '
        'TXT_PASSWORD
        '
        Me.TXT_PASSWORD.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TXT_PASSWORD.Location = New System.Drawing.Point(65, 52)
        Me.TXT_PASSWORD.Name = "TXT_PASSWORD"
        Me.TXT_PASSWORD.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TXT_PASSWORD.Size = New System.Drawing.Size(288, 20)
        Me.TXT_PASSWORD.TabIndex = 7
        '
        'TXT_USERNAME
        '
        Me.TXT_USERNAME.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TXT_USERNAME.Location = New System.Drawing.Point(65, 23)
        Me.TXT_USERNAME.Name = "TXT_USERNAME"
        Me.TXT_USERNAME.Size = New System.Drawing.Size(288, 20)
        Me.TXT_USERNAME.TabIndex = 6
        '
        'LBL_URL
        '
        Me.LBL_URL.AutoSize = True
        Me.LBL_URL.Location = New System.Drawing.Point(6, 56)
        Me.LBL_URL.Name = "LBL_URL"
        Me.LBL_URL.Size = New System.Drawing.Size(29, 13)
        Me.LBL_URL.TabIndex = 24
        Me.LBL_URL.Text = "URL"
        Me.LBL_URL.Visible = False
        '
        'TXT_AUTHEN
        '
        Me.TXT_AUTHEN.Location = New System.Drawing.Point(65, 53)
        Me.TXT_AUTHEN.Name = "TXT_AUTHEN"
        Me.TXT_AUTHEN.Size = New System.Drawing.Size(288, 20)
        Me.TXT_AUTHEN.TabIndex = 5
        Me.TXT_AUTHEN.Visible = False
        '
        'LBL_MODALITA
        '
        Me.LBL_MODALITA.AutoSize = True
        Me.LBL_MODALITA.Location = New System.Drawing.Point(6, 26)
        Me.LBL_MODALITA.Name = "LBL_MODALITA"
        Me.LBL_MODALITA.Size = New System.Drawing.Size(34, 13)
        Me.LBL_MODALITA.TabIndex = 20
        Me.LBL_MODALITA.Text = "Mode"
        Me.LBL_MODALITA.Visible = False
        '
        'CMB_MODE
        '
        Me.CMB_MODE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CMB_MODE.FormattingEnabled = True
        Me.CMB_MODE.Items.AddRange(New Object() {"HTTP"})
        Me.CMB_MODE.Location = New System.Drawing.Point(65, 23)
        Me.CMB_MODE.Name = "CMB_MODE"
        Me.CMB_MODE.Size = New System.Drawing.Size(157, 21)
        Me.CMB_MODE.TabIndex = 3
        Me.CMB_MODE.Visible = False
        '
        'AUTO_LOGIN
        '
        Me.AUTO_LOGIN.AutoSize = True
        Me.AUTO_LOGIN.Checked = True
        Me.AUTO_LOGIN.CheckState = System.Windows.Forms.CheckState.Checked
        Me.AUTO_LOGIN.Location = New System.Drawing.Point(17, 192)
        Me.AUTO_LOGIN.Name = "AUTO_LOGIN"
        Me.AUTO_LOGIN.Size = New System.Drawing.Size(15, 14)
        Me.AUTO_LOGIN.TabIndex = 4
        Me.AUTO_LOGIN.UseVisualStyleBackColor = True
        '
        'TAB_002
        '
        Me.TAB_002.Controls.Add(Me.TXT_SERVER_MESSAGES)
        Me.TAB_002.Location = New System.Drawing.Point(4, 22)
        Me.TAB_002.Name = "TAB_002"
        Me.TAB_002.Padding = New System.Windows.Forms.Padding(3)
        Me.TAB_002.Size = New System.Drawing.Size(405, 265)
        Me.TAB_002.TabIndex = 1
        Me.TAB_002.UseVisualStyleBackColor = True
        '
        'TXT_SERVER_MESSAGES
        '
        Me.TXT_SERVER_MESSAGES.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TXT_SERVER_MESSAGES.Location = New System.Drawing.Point(3, 3)
        Me.TXT_SERVER_MESSAGES.Multiline = True
        Me.TXT_SERVER_MESSAGES.Name = "TXT_SERVER_MESSAGES"
        Me.TXT_SERVER_MESSAGES.ReadOnly = True
        Me.TXT_SERVER_MESSAGES.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TXT_SERVER_MESSAGES.Size = New System.Drawing.Size(399, 259)
        Me.TXT_SERVER_MESSAGES.TabIndex = 0
        Me.TXT_SERVER_MESSAGES.WordWrap = False
        '
        'TAB_003
        '
        Me.TAB_003.Controls.Add(Me.TXT_ERROR_LOG)
        Me.TAB_003.Location = New System.Drawing.Point(4, 22)
        Me.TAB_003.Name = "TAB_003"
        Me.TAB_003.Padding = New System.Windows.Forms.Padding(3)
        Me.TAB_003.Size = New System.Drawing.Size(405, 265)
        Me.TAB_003.TabIndex = 2
        Me.TAB_003.UseVisualStyleBackColor = True
        '
        'TXT_ERROR_LOG
        '
        Me.TXT_ERROR_LOG.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TXT_ERROR_LOG.Location = New System.Drawing.Point(3, 3)
        Me.TXT_ERROR_LOG.Multiline = True
        Me.TXT_ERROR_LOG.Name = "TXT_ERROR_LOG"
        Me.TXT_ERROR_LOG.ReadOnly = True
        Me.TXT_ERROR_LOG.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TXT_ERROR_LOG.Size = New System.Drawing.Size(399, 259)
        Me.TXT_ERROR_LOG.TabIndex = 1
        Me.TXT_ERROR_LOG.WordWrap = False
        '
        'TAB_004
        '
        Me.TAB_004.Controls.Add(Me.LBL_RUN)
        Me.TAB_004.Controls.Add(Me.LBL_COMMAND)
        Me.TAB_004.Controls.Add(Me.PAN_CONTROLS)
        Me.TAB_004.Location = New System.Drawing.Point(4, 22)
        Me.TAB_004.Name = "TAB_004"
        Me.TAB_004.Size = New System.Drawing.Size(405, 265)
        Me.TAB_004.TabIndex = 3
        Me.TAB_004.UseVisualStyleBackColor = True
        '
        'LBL_RUN
        '
        Me.LBL_RUN.AutoSize = True
        Me.LBL_RUN.Location = New System.Drawing.Point(180, 11)
        Me.LBL_RUN.Name = "LBL_RUN"
        Me.LBL_RUN.Size = New System.Drawing.Size(0, 13)
        Me.LBL_RUN.TabIndex = 3
        '
        'LBL_COMMAND
        '
        Me.LBL_COMMAND.AutoSize = True
        Me.LBL_COMMAND.Location = New System.Drawing.Point(9, 11)
        Me.LBL_COMMAND.Name = "LBL_COMMAND"
        Me.LBL_COMMAND.Size = New System.Drawing.Size(0, 13)
        Me.LBL_COMMAND.TabIndex = 2
        '
        'PAN_CONTROLS
        '
        Me.PAN_CONTROLS.AutoScroll = True
        Me.PAN_CONTROLS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PAN_CONTROLS.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PAN_CONTROLS.Location = New System.Drawing.Point(0, 29)
        Me.PAN_CONTROLS.Margin = New System.Windows.Forms.Padding(3, 5, 3, 3)
        Me.PAN_CONTROLS.Name = "PAN_CONTROLS"
        Me.PAN_CONTROLS.Size = New System.Drawing.Size(405, 236)
        Me.PAN_CONTROLS.TabIndex = 1
        '
        'TAB_005
        '
        Me.TAB_005.Controls.Add(Me.Label2)
        Me.TAB_005.Controls.Add(Me.TXT_REDIAL_HOTKEY)
        Me.TAB_005.Controls.Add(Me.CMB_REDIAL_HOTKEY)
        Me.TAB_005.Controls.Add(Me.Label1)
        Me.TAB_005.Controls.Add(Me.TXT_SPEEDDIAL_HOTKEY)
        Me.TAB_005.Controls.Add(Me.CMB_SPEEDDIAL_HOTKEY)
        Me.TAB_005.Location = New System.Drawing.Point(4, 22)
        Me.TAB_005.Name = "TAB_005"
        Me.TAB_005.Size = New System.Drawing.Size(405, 265)
        Me.TAB_005.TabIndex = 4
        Me.TAB_005.Text = "Dialer Hotkey"
        Me.TAB_005.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(216, 45)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(129, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Ricomporre ultimo numero"
        '
        'TXT_REDIAL_HOTKEY
        '
        Me.TXT_REDIAL_HOTKEY.Location = New System.Drawing.Point(148, 42)
        Me.TXT_REDIAL_HOTKEY.Multiline = True
        Me.TXT_REDIAL_HOTKEY.Name = "TXT_REDIAL_HOTKEY"
        Me.TXT_REDIAL_HOTKEY.Size = New System.Drawing.Size(63, 21)
        Me.TXT_REDIAL_HOTKEY.TabIndex = 4
        '
        'CMB_REDIAL_HOTKEY
        '
        Me.CMB_REDIAL_HOTKEY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CMB_REDIAL_HOTKEY.FormattingEnabled = True
        Me.CMB_REDIAL_HOTKEY.Location = New System.Drawing.Point(16, 42)
        Me.CMB_REDIAL_HOTKEY.Name = "CMB_REDIAL_HOTKEY"
        Me.CMB_REDIAL_HOTKEY.Size = New System.Drawing.Size(109, 21)
        Me.CMB_REDIAL_HOTKEY.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(216, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(136, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Chiama numero selezionato"
        '
        'TXT_SPEEDDIAL_HOTKEY
        '
        Me.TXT_SPEEDDIAL_HOTKEY.Location = New System.Drawing.Point(148, 15)
        Me.TXT_SPEEDDIAL_HOTKEY.Multiline = True
        Me.TXT_SPEEDDIAL_HOTKEY.Name = "TXT_SPEEDDIAL_HOTKEY"
        Me.TXT_SPEEDDIAL_HOTKEY.Size = New System.Drawing.Size(63, 21)
        Me.TXT_SPEEDDIAL_HOTKEY.TabIndex = 1
        '
        'CMB_SPEEDDIAL_HOTKEY
        '
        Me.CMB_SPEEDDIAL_HOTKEY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CMB_SPEEDDIAL_HOTKEY.FormattingEnabled = True
        Me.CMB_SPEEDDIAL_HOTKEY.Location = New System.Drawing.Point(16, 15)
        Me.CMB_SPEEDDIAL_HOTKEY.Name = "CMB_SPEEDDIAL_HOTKEY"
        Me.CMB_SPEEDDIAL_HOTKEY.Size = New System.Drawing.Size(109, 21)
        Me.CMB_SPEEDDIAL_HOTKEY.TabIndex = 0
        '
        'TAB_006
        '
        Me.TAB_006.Controls.Add(Me.GroupBox1)
        Me.TAB_006.Controls.Add(Me.CHK_SUONERIA)
        Me.TAB_006.Controls.Add(Me.GRP_SUONERIA)
        Me.TAB_006.Location = New System.Drawing.Point(4, 22)
        Me.TAB_006.Name = "TAB_006"
        Me.TAB_006.Padding = New System.Windows.Forms.Padding(3)
        Me.TAB_006.Size = New System.Drawing.Size(405, 265)
        Me.TAB_006.TabIndex = 5
        Me.TAB_006.Text = "Ringtone"
        Me.TAB_006.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.CHK_ANSWER)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.ComboBox1)
        Me.GroupBox1.Controls.Add(Me.CHK_ONEKEY)
        Me.GroupBox1.Enabled = False
        Me.GroupBox1.Location = New System.Drawing.Point(6, 112)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(392, 71)
        Me.GroupBox1.TabIndex = 9
        Me.GroupBox1.TabStop = False
        '
        'CHK_ANSWER
        '
        Me.CHK_ANSWER.AutoSize = True
        Me.CHK_ANSWER.BackColor = System.Drawing.Color.White
        Me.CHK_ANSWER.Location = New System.Drawing.Point(6, -1)
        Me.CHK_ANSWER.Name = "CHK_ANSWER"
        Me.CHK_ANSWER.Size = New System.Drawing.Size(169, 17)
        Me.CHK_ANSWER.TabIndex = 9
        Me.CHK_ANSWER.Text = "Abilita risposta con tasto cuffia"
        Me.CHK_ANSWER.UseVisualStyleBackColor = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 25)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(41, 13)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "Device"
        '
        'ComboBox1
        '
        Me.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(53, 22)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(330, 21)
        Me.ComboBox1.TabIndex = 0
        '
        'CHK_ONEKEY
        '
        Me.CHK_ONEKEY.AutoSize = True
        Me.CHK_ONEKEY.Enabled = False
        Me.CHK_ONEKEY.Location = New System.Drawing.Point(53, 49)
        Me.CHK_ONEKEY.Name = "CHK_ONEKEY"
        Me.CHK_ONEKEY.Size = New System.Drawing.Size(308, 17)
        Me.CHK_ONEKEY.TabIndex = 6
        Me.CHK_ONEKEY.Text = "Usa un solo tasto per rispondere e per terminare la chiamata"
        Me.TIP.SetToolTip(Me.CHK_ONEKEY, "Togliendo il check, la suoneria sarà esguita una volta sola all'arrivo della chia" &
        "mata")
        Me.CHK_ONEKEY.UseVisualStyleBackColor = True
        '
        'CHK_SUONERIA
        '
        Me.CHK_SUONERIA.AutoSize = True
        Me.CHK_SUONERIA.Location = New System.Drawing.Point(12, 8)
        Me.CHK_SUONERIA.Name = "CHK_SUONERIA"
        Me.CHK_SUONERIA.Size = New System.Drawing.Size(97, 17)
        Me.CHK_SUONERIA.TabIndex = 8
        Me.CHK_SUONERIA.Text = "Abilita suoneria"
        Me.CHK_SUONERIA.UseVisualStyleBackColor = True
        '
        'GRP_SUONERIA
        '
        Me.GRP_SUONERIA.Controls.Add(Me.Label3)
        Me.GRP_SUONERIA.Controls.Add(Me.CMB_DEVICE)
        Me.GRP_SUONERIA.Controls.Add(Me.CHK_SOUND_LOOP)
        Me.GRP_SUONERIA.Controls.Add(Me.BUT_TRY)
        Me.GRP_SUONERIA.Controls.Add(Me.Label4)
        Me.GRP_SUONERIA.Controls.Add(Me.TXT_SOUND)
        Me.GRP_SUONERIA.Controls.Add(Me.BUT_SFOGLIA)
        Me.GRP_SUONERIA.Enabled = False
        Me.GRP_SUONERIA.Location = New System.Drawing.Point(6, 8)
        Me.GRP_SUONERIA.Name = "GRP_SUONERIA"
        Me.GRP_SUONERIA.Size = New System.Drawing.Size(392, 98)
        Me.GRP_SUONERIA.TabIndex = 7
        Me.GRP_SUONERIA.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 25)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(41, 13)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Device"
        '
        'CMB_DEVICE
        '
        Me.CMB_DEVICE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CMB_DEVICE.FormattingEnabled = True
        Me.CMB_DEVICE.Location = New System.Drawing.Point(53, 22)
        Me.CMB_DEVICE.Name = "CMB_DEVICE"
        Me.CMB_DEVICE.Size = New System.Drawing.Size(330, 21)
        Me.CMB_DEVICE.TabIndex = 0
        '
        'CHK_SOUND_LOOP
        '
        Me.CHK_SOUND_LOOP.AutoSize = True
        Me.CHK_SOUND_LOOP.Location = New System.Drawing.Point(53, 76)
        Me.CHK_SOUND_LOOP.Name = "CHK_SOUND_LOOP"
        Me.CHK_SOUND_LOOP.Size = New System.Drawing.Size(153, 17)
        Me.CHK_SOUND_LOOP.TabIndex = 6
        Me.CHK_SOUND_LOOP.Text = "Suona finchè non rispondo"
        Me.TIP.SetToolTip(Me.CHK_SOUND_LOOP, "Togliendo il check, la suoneria sarà esguita una volta sola all'arrivo della chia" &
        "mata")
        Me.CHK_SOUND_LOOP.UseVisualStyleBackColor = True
        '
        'BUT_TRY
        '
        Me.BUT_TRY.Location = New System.Drawing.Point(338, 48)
        Me.BUT_TRY.Name = "BUT_TRY"
        Me.BUT_TRY.Size = New System.Drawing.Size(45, 23)
        Me.BUT_TRY.TabIndex = 2
        Me.BUT_TRY.Text = "Prova"
        Me.BUT_TRY.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(24, 53)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(23, 13)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "File"
        '
        'TXT_SOUND
        '
        Me.TXT_SOUND.Location = New System.Drawing.Point(53, 50)
        Me.TXT_SOUND.Name = "TXT_SOUND"
        Me.TXT_SOUND.Size = New System.Drawing.Size(246, 20)
        Me.TXT_SOUND.TabIndex = 3
        '
        'BUT_SFOGLIA
        '
        Me.BUT_SFOGLIA.Location = New System.Drawing.Point(305, 48)
        Me.BUT_SFOGLIA.Name = "BUT_SFOGLIA"
        Me.BUT_SFOGLIA.Size = New System.Drawing.Size(26, 23)
        Me.BUT_SFOGLIA.TabIndex = 4
        Me.BUT_SFOGLIA.Text = "..."
        Me.BUT_SFOGLIA.UseVisualStyleBackColor = True
        '
        'BUT_SAVE
        '
        Me.BUT_SAVE.Location = New System.Drawing.Point(334, 297)
        Me.BUT_SAVE.Name = "BUT_SAVE"
        Me.BUT_SAVE.Size = New System.Drawing.Size(75, 23)
        Me.BUT_SAVE.TabIndex = 9
        Me.BUT_SAVE.UseVisualStyleBackColor = True
        '
        'FILE_DLG
        '
        Me.FILE_DLG.FileName = "OpenFileDialog1"
        '
        'TMR_CONNECTION
        '
        Me.TMR_CONNECTION.Interval = 1
        '
        'BUT_RESET_RUNWITH
        '
        Me.BUT_RESET_RUNWITH.Location = New System.Drawing.Point(4, 297)
        Me.BUT_RESET_RUNWITH.Name = "BUT_RESET_RUNWITH"
        Me.BUT_RESET_RUNWITH.Size = New System.Drawing.Size(75, 23)
        Me.BUT_RESET_RUNWITH.TabIndex = 10
        Me.BUT_RESET_RUNWITH.Text = "Reset"
        Me.BUT_RESET_RUNWITH.UseVisualStyleBackColor = True
        Me.BUT_RESET_RUNWITH.Visible = False
        '
        'TIP
        '
        Me.TIP.IsBalloon = True
        '
        'TMR_ELAPSE
        '
        Me.TMR_ELAPSE.Interval = 1000
        '
        'TMR_ICONS
        '
        Me.TMR_ICONS.Interval = 500
        Me.TMR_ICONS.Tag = "1"
        '
        'TMR_DEVICE
        '
        Me.TMR_DEVICE.Enabled = True
        Me.TMR_DEVICE.Interval = 1000
        '
        'TAB_007
        '
        Me.TAB_007.Controls.Add(Me.CheckBox1)
        Me.TAB_007.Controls.Add(Me.GRP_PARAMS)
        Me.TAB_007.Location = New System.Drawing.Point(4, 22)
        Me.TAB_007.Name = "TAB_007"
        Me.TAB_007.Padding = New System.Windows.Forms.Padding(3)
        Me.TAB_007.Size = New System.Drawing.Size(405, 265)
        Me.TAB_007.TabIndex = 6
        Me.TAB_007.Text = "ParamURL"
        Me.TAB_007.UseVisualStyleBackColor = True
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(9, 3)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(138, 17)
        Me.CheckBox1.TabIndex = 10
        Me.CheckBox1.Text = "Abilita URL Parametrico"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'GRP_PARAMS
        '
        Me.GRP_PARAMS.Controls.Add(Me.CMB_PARAM)
        Me.GRP_PARAMS.Controls.Add(Me.Label6)
        Me.GRP_PARAMS.Enabled = False
        Me.GRP_PARAMS.Location = New System.Drawing.Point(3, 3)
        Me.GRP_PARAMS.Name = "GRP_PARAMS"
        Me.GRP_PARAMS.Size = New System.Drawing.Size(392, 98)
        Me.GRP_PARAMS.TabIndex = 9
        Me.GRP_PARAMS.TabStop = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(6, 25)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(31, 13)
        Me.Label6.TabIndex = 1
        Me.Label6.Text = "Mod."
        '
        'CMB_PARAM
        '
        Me.CMB_PARAM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CMB_PARAM.FormattingEnabled = True
        Me.CMB_PARAM.Items.AddRange(New Object() {"ALLA RISPOSTA", "ALLO SQUILLO"})
        Me.CMB_PARAM.Location = New System.Drawing.Point(54, 17)
        Me.CMB_PARAM.Name = "CMB_PARAM"
        Me.CMB_PARAM.Size = New System.Drawing.Size(332, 21)
        Me.CMB_PARAM.TabIndex = 1
        '
        'FRM_CONFIG
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(418, 348)
        Me.Controls.Add(Me.BUT_RESET_RUNWITH)
        Me.Controls.Add(Me.BUT_SAVE)
        Me.Controls.Add(Me.TABS)
        Me.Controls.Add(Me.StatusStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FRM_CONFIG"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.gbIpAddress.ResumeLayout(False)
        Me.gbIpAddress.PerformLayout()
        Me.gbServerPort.ResumeLayout(False)
        Me.gbServerPort.PerformLayout()
        Me.CTX_MENU.ResumeLayout(False)
        Me.TABS.ResumeLayout(False)
        Me.TAB_001.ResumeLayout(False)
        Me.TAB_001.PerformLayout()
        CType(Me.NUM_NOTIFICA, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GRP_AUTH.ResumeLayout(False)
        Me.GRP_AUTH.PerformLayout()
        Me.TAB_002.ResumeLayout(False)
        Me.TAB_002.PerformLayout()
        Me.TAB_003.ResumeLayout(False)
        Me.TAB_003.PerformLayout()
        Me.TAB_004.ResumeLayout(False)
        Me.TAB_004.PerformLayout()
        Me.TAB_005.ResumeLayout(False)
        Me.TAB_005.PerformLayout()
        Me.TAB_006.ResumeLayout(False)
        Me.TAB_006.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GRP_SUONERIA.ResumeLayout(False)
        Me.GRP_SUONERIA.PerformLayout()
        Me.TAB_007.ResumeLayout(False)
        Me.TAB_007.PerformLayout()
        Me.GRP_PARAMS.ResumeLayout(False)
        Me.GRP_PARAMS.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BUT_CONNECT As System.Windows.Forms.Button
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents gbIpAddress As System.Windows.Forms.GroupBox
    Friend WithEvents TXT_SERVER As System.Windows.Forms.TextBox
    Friend WithEvents gbServerPort As System.Windows.Forms.GroupBox
    Friend WithEvents TXT_PORT As System.Windows.Forms.TextBox
    Friend WithEvents PingServer As System.Windows.Forms.Timer
    Friend WithEvents NOTIFY As System.Windows.Forms.NotifyIcon
    Friend WithEvents CTX_MENU As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ConnectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TABS As System.Windows.Forms.TabControl
    Friend WithEvents TAB_001 As System.Windows.Forms.TabPage
    Friend WithEvents GRP_AUTH As System.Windows.Forms.GroupBox
    Friend WithEvents LBL_PASSWORD As System.Windows.Forms.Label
    Friend WithEvents LBL_USER As System.Windows.Forms.Label
    Friend WithEvents LBL_MODALITA As System.Windows.Forms.Label
    Friend WithEvents AUTO_LOGIN As System.Windows.Forms.CheckBox
    Friend WithEvents CMB_MODE As System.Windows.Forms.ComboBox
    Friend WithEvents TXT_PASSWORD As System.Windows.Forms.TextBox
    Friend WithEvents TXT_USERNAME As System.Windows.Forms.TextBox
    Friend WithEvents BUT_SAVE As System.Windows.Forms.Button
    Friend WithEvents NUM_NOTIFICA As System.Windows.Forms.NumericUpDown
    Friend WithEvents LBL_NOTIFICA_CLOSE As System.Windows.Forms.Label
    Friend WithEvents LBL_URL As System.Windows.Forms.Label
    Friend WithEvents TXT_AUTHEN As System.Windows.Forms.TextBox
    Friend WithEvents STARTUP As System.Windows.Forms.CheckBox
    Friend WithEvents TAB_002 As System.Windows.Forms.TabPage
    Friend WithEvents TXT_SERVER_MESSAGES As System.Windows.Forms.TextBox
    Friend WithEvents TAB_003 As System.Windows.Forms.TabPage
    Friend WithEvents TXT_ERROR_LOG As System.Windows.Forms.TextBox
    Friend WithEvents TAB_004 As System.Windows.Forms.TabPage
    Friend WithEvents PAN_CONTROLS As System.Windows.Forms.Panel
    Friend WithEvents LBL_RUN As System.Windows.Forms.Label
    Friend WithEvents LBL_COMMAND As System.Windows.Forms.Label
    Friend WithEvents FILE_DLG As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents LanguageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TMR_CONNECTION As System.Windows.Forms.Timer
    Friend WithEvents BUT_RESET_RUNWITH As System.Windows.Forms.Button
    Friend WithEvents TIP As System.Windows.Forms.ToolTip
    Friend WithEvents STRIP_STATUS As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents TMR_ELAPSE As System.Windows.Forms.Timer
    Friend WithEvents TMR_ICONS As System.Windows.Forms.Timer
    Friend WithEvents INFO As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CallToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents TAB_005 As System.Windows.Forms.TabPage
    Friend WithEvents CMB_SPEEDDIAL_HOTKEY As System.Windows.Forms.ComboBox
    Friend WithEvents TXT_SPEEDDIAL_HOTKEY As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TXT_REDIAL_HOTKEY As System.Windows.Forms.TextBox
    Friend WithEvents CMB_REDIAL_HOTKEY As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TAB_006 As TabPage
    Friend WithEvents BUT_TRY As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents CMB_DEVICE As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents BUT_SFOGLIA As Button
    Friend WithEvents TXT_SOUND As TextBox
    Friend WithEvents CHK_SOUND_LOOP As CheckBox
    Friend WithEvents TMR_DEVICE As Timer
    Friend WithEvents CHK_SUONERIA As CheckBox
    Friend WithEvents GRP_SUONERIA As GroupBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents CHK_ANSWER As CheckBox
    Friend WithEvents Label5 As Label
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents CHK_ONEKEY As CheckBox
    Friend WithEvents TAB_007 As TabPage
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents GRP_PARAMS As GroupBox
    Friend WithEvents Label6 As Label
    Friend WithEvents CMB_PARAM As ComboBox
End Class
