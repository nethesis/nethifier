Imports System.Runtime.InteropServices
Imports System.Linq
Imports Microsoft.Win32
Imports System.Net
Imports System.Web
Imports System.Text
Imports System.IO
Imports System.Media
Imports System.Net.Security
'Imports System.Collections.Specialized
Imports System.Diagnostics.Debug
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports Newtonsoft.Json
Imports IWshRuntimeLibrary
Imports Nethifier.Helper
Imports NAudio.Wave

Friend Class FRM_CONFIG

    Private IsLoggedIn As Boolean
    Public IsClosing As Boolean = False
    'Public WithEvents _Client As New TCP_CLIENT(AddressOf UpdateUI)
    Public WithEvents _Client As TCP_CLIENT

    Private Balloons As New Collection
    Private WithEvents Notifications As FRM_NOTIFICATIONS '= New FRM_NOTIFICATIONS
    Private Config As Config
    Private ExceptionManager As ExceptionManager
    Private Status As EnumStatus
    Private Msg As MessageHelper
    Private UserLogin As Login
    Private SoundDevices As ArrayList

    Private IsAutoConnecting As Boolean = False
    Private NethDebug As NethDebugger

    Private HotKeys As String() = {"-",
            "Ctlr",
            "Alt",
            "Shift"}
    ',
    '"Alt+Shift",
    '"Ctrl+Alt",
    '"Ctrl+Shift",
    '"Ctrl+Alt+Shift"}

    'H1 is MOD_ALT, H2 is MOD_CONTROL, H4 is MOD_SHIFT, H8 is MOD_WIN
    'Public Const MOD_ALT As Integer = &H1 + &H2 + &H4 + &H8
    Private HotKeysValues As Integer() = {0,
            &H2,
            &H1,
            &H4}
    ',
    '&H1 + &H4,
    '&H2 + &H1,
    '&H2 + &H4,
    '&H2 + &H1 + &H4}

    Private Enum EnumStatus
        Disconnected = 0
        Connected = 1
        Connecting = 2
        Disconnecting = 3
    End Enum

    Private Token As String = ""

    Private Function BytesToString(ByVal data() As Byte) As String
        Dim enc As New System.Text.UTF8Encoding()
        BytesToString = enc.GetString(data)
    End Function

    Private Function StrToByteArray(ByVal text As String) As Byte()
        Dim encoding As New System.Text.UTF8Encoding()
        StrToByteArray = encoding.GetBytes(text)
    End Function
    Private Function Mylog(ByVal context As String, ByVal text As String) As Int16
        If (InStr(text, ",") > 0) Then
            text = text.Substring(0, text.IndexOf(","))
        End If
        If (context = "UpdateUI") Then
            text = text.Replace("{", "").Replace("}", "")
        End If
        text = text.Replace(vbLf, " ").Replace(Environment.NewLine, " ")
        Dim DebugPath As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).ToString(), "nethifier_debug.log")
        If IO.File.Exists(DebugPath) Then
            My.Computer.FileSystem.WriteAllText(DebugPath, Format(Date.Now(), "yyyy/MM/dd HH:mm:ss") & "- " & context & " - " & text & Environment.NewLine, True)
        End If
        Debug.WriteLine(text)
        Return (0)
    End Function

    Public Sub UpdateUI(ByVal bytes() As Byte)

        'Handle call requested NICK

        If Me.InvokeRequired() Then
            ' InvokeRequired: We're running on the background thread. Invoke the delegate.
            Me.Invoke(_Client.ClientCallbackObject, bytes)
        Else
            ' We're on the main UI thread now.
            Dim dontReport As Boolean = False
            Dim RecievedMessage As String = BytesToString(bytes)

            Dim Message As String = ""
            For I As Integer = 0 To RecievedMessage.Length - 1
                If Asc(RecievedMessage.Substring(I, 1)) = 0 Then
                    Exit For
                Else
                    Message += RecievedMessage.Substring(I, 1)
                End If
            Next

            If Not Message.ToLower.StartsWith("{""ping"":""active""}") Then
                Mylog("UpdateUI", Message)
            End If


            'Dim MessagePart() As String
            Dim IsSystem As Boolean = Message.StartsWith("SYS:")

            If IsSystem Then

                    Message = Message.Substring(4)

                    If Message.StartsWith(vbCrLf & "ERR_MSG: ") Then
                        Dim M As String() = Split(Message.Substring((vbCrLf & "ERR_MSG: ").Length), "ERR_NUM: ")
                        TXT_ERROR_LOG.Text += Now.ToString("yyyy-MM-dd hh:mm:ss") & " - " & M(0)

                        '10054 - Server disconnected
                        '10060 - Server Response TimeOut
                        '10053 - Client disconnected
                        '11004 - Timeout expired

                        If IsNumeric(M(1)) AndAlso (CInt(M(1)) = 10054 OrElse CInt(M(1)) = 10060 OrElse (((CInt(M(1)) = 11004) OrElse CInt(M(1)) = 10053) AndAlso Status <> EnumStatus.Disconnected)) OrElse Status = EnumStatus.Connecting Then
                            If Status = EnumStatus.Connected AndAlso TryConnectionCount = 0 Then
                                ShowDisconnectedNotification()
                            End If

                            'Auto connect
                            If TryConnectionCount <= 5 Then
                                ActivateReconnection()
                            Else
                                DisableTimer()
                            End If
                        End If

                        If Status = EnumStatus.Connecting Then
                            BUT_CONNECT.Enabled = True

                            '2016/06/29
                            If CStr(BUT_CONNECT.Tag) = "FORCING_CONNECTION" Then
                                BUT_CONNECT.Tag = ""
                                Disconnect()
                                EnableProperties(True)
                            End If
                            '2016/06/29
                        Else
                            Disconnect()
                            EnableProperties(True)
                        End If
                    Else
                        'NOTIFY.Text = Message
                    End If
                Else
                    TXT_SERVER_MESSAGES.Text += Now.ToString("yyyy-MM-dd hh:mm:ss") & " - " & Message & vbCrLf
                End If

                If Message.EndsWith(vbLf) Then
                    Message = Message.Substring(0, Message.Length - vbLf.Length)
                End If
            If Message.ToLower.StartsWith("{""notification"":") Then
                'MessagePart = Split(Message.Substring("notify:".Length), "#")

                Try

                    Dim URL As String = ""
                    Dim WIDTH As Integer = 0
                    Dim HEIGHT As Integer = 0
                    Dim EXP As Integer = 0
                    Dim ID As String = ""
                    Dim ACTION As String = ""

                    Dim Notification As CLS_NOTIFICATION = New CLS_NOTIFICATION

                    JsonConvert.PopulateObject(Message, Notification)

                    With Notification.Notification
                        URL = .URL
                        ID = .ID
                        ACTION = .Action
                        EXP = .CloseTimeOut
                        HEIGHT = .Height
                        WIDTH = .Width

                        'Debug
                        If NethDebug.IsActive AndAlso NethDebug.USE_NOTIFICATION_TIMEOUT Then
                            'Riattivare quando avranno deciso cosa vogliono fare. 2014/05/05
                            If EXP > Config.NOTIFY_TIMEOUT AndAlso Config.NOTIFY_TIMEOUT > 0 Then
                                EXP = Config.NOTIFY_TIMEOUT
                            End If
                        End If
                        'Debug

                    End With

                    With Notifications
                        .UserLogin = UserLogin
                        Select Case UCase(ACTION)
                            Case Is = "OPEN"
                                .Commands = Config.Commands
                                If .AddNotification(ID:=ID, URL:=URL, Width:=WIDTH, Height:=HEIGHT, Expiration:=EXP) Then
                                    'NOTIFY.Icon = GetIcon("chat")
                                    NOTIFY.Icon = Nethifier.My.Resources.Resources.chat
                                    StartRinging()

                                Else
                                    _Client.SendBytes("{""error"":{""id"":""" & ID & """,""message"":""still active""}}")
                                End If
                            Case Is = "TERMINATE"
                                .CloseNotification(ID)

                                If .GetNotificationCount <= 0 Then
                                    'NOTIFY.Icon = GetIcon("online")
                                    'NOTIFY.Icon = (Resources.GetObject("NOTIFY.Icon"), System.Drawing.Icon)
                                    NOTIFY.Icon = Nethifier.My.Resources.Resources.online
                                End If
                        End Select
                    End With

                Catch ex As Exception
                    ExceptionManager.Write(ex)
                End Try

                'ElseIf Message.ToLower.StartsWith("{""reset"":") Then
                '    Do While True
                '        Dim E As Boolean = True
                '        For Each Com As Command In Config.Commands.Values
                '            Config.Commands.Remove(Com.Command)
                '            E = False
                '            Exit For
                '        Next

                '        If E Then
                '            Config.Save()
                '            Exit Do
                '        End If
                '    Loop
            ElseIf Message.ToLower.StartsWith("{""extenhangup"":") Then

                StopRinging()
                LED("available")

            ElseIf Message.ToLower.StartsWith("{""extenconnected"":") Then

                StopRinging()
                LED("busy")

            ElseIf Message.ToLower.StartsWith("{""ping"":""active""}") Then

                '

            ElseIf Message.ToLower.StartsWith("{""action"":""sendurl""") Then
                '   Messaggio ricevuto da Nethifier per l'esecuzione di una chiamata
                '{
                '  “action”: “sendurl”,
                '  “url”: “http://USER:PASSWORD@IP/servlet?number=NUMBER&outgoing_uri=INT@REMOTEHOST”
                '}
                LED("busy")
                Try
                    Dim URL As String = ""
                    Dim WIDTH As Integer = 0
                    Dim HEIGHT As Integer = 0
                    Dim EXP As Integer = 0
                    Dim ID As String = ""
                    Dim ACTION As String = ""
                    Dim UserName As String
                    Dim Password As String
                    Dim UserPwd As String
                    Dim SubUrl As String
                    Dim SubLen As Int32

                    Dim Request As CLS_SendUrl = New CLS_SendUrl

                    JsonConvert.PopulateObject(Message, Request)
                    'Request = JsonConvert.DeserializeObject(Message)

                    With Request
                        URL = Request.URL
                        ACTION = Request.Action
                    End With

                    SubLen = Convert.ToInt32(URL.StartsWith("http://")) * 7 + Convert.ToInt32(URL.StartsWith("https://")) * 8
                    SubUrl = URL.Substring(SubLen, URL.LastIndexOf("/") - SubLen)

                    UserPwd = SubUrl.Substring(0, SubUrl.LastIndexOf("@"))
                    UserName = UserPwd.Substring(0, UserPwd.IndexOf(":"))
                    Password = UserPwd.Substring(UserPwd.IndexOf(":") + 1)

                    Mylog("action:sendurl", URL.Replace(Password, "****").ToString())

                    Dim proc As New ProcessStartInfo
                    With proc
                        .UseShellExecute = True
                        .WorkingDirectory = Environment.CurrentDirectory
                        .FileName = "NethDialer.exe"
                        .Arguments = "-cloud2call=" & URL
                        '.Verb = "runas"
                    End With

                    Try
                        Process.Start(proc)
                    Catch ex As Exception
                        ' The user refused the elevation. 
                        ' Do nothing and return directly ... 
                        'MessageBox.Show(ex.Message)
                    Finally
                        'CALL_TIMER.Enabled = True
                    End Try
                    'CALL_TIMER.Enabled = True

                    'Dim webClient As New System.Net.WebClient
                    ''webClient.Headers.Clear()
                    ''webClient.CachePolicy = New System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)
                    'WebClient.Credentials = New NetworkCredential(UserName, Password)
                    ''webClient.Headers.Add("Accept-Encoding", "")
                    ''WebClient.Headers.Add("User-Agent", Guid.NewGuid().ToString)
                    ''WebClient.Encoding = System.Text.Encoding.UTF8
                    'If Not (webClient.IsBusy) Then
                    'Dim result As String
                    'Try
                    'result = WebClient.DownloadString(URL)
                    'Mylog("ResultCall", result.ToString())

                    'Catch e As WebException
                    'Mylog("ResultCall", e.Message.ToString())
                    'MsgBox(e.Message.ToString(), 0, "ResultCall")
                    'End Try
                    '   'webClient.Dispose()
                    'Else
                    'Mylog("ResultCall", "Webclient Busy")
                    'End If

                Catch ex As Exception
                    ExceptionManager.Write(ex)
                    Dim DebugPathE As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).ToString(), "nethifier_debug.log")
                    If IO.File.Exists(DebugPathE) Then
                        My.Computer.FileSystem.WriteAllText(DebugPathE, Format(Date.Now(), "yyyy/MM/dd HH:mm:ss") & "- UpdateUIEx: " & ex.Message.Replace(Environment.NewLine, " ") & Environment.NewLine, True)
                    End If
                End Try

            ElseIf Message.ToLower.StartsWith("{""action"":""debug""") Then
                Dim DebugPathE As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).ToString(), "nethifier_debug.log")
                My.Computer.FileSystem.WriteAllText(DebugPathE, Format(Date.Now(), "yyyy/MM/dd HH:mm:ss") & "- Enabled Debug " & Environment.NewLine, True)

            ElseIf Message.ToLower.StartsWith("{""action"":""debug-off""") Then
                        Dim DebugPathF As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).ToString(), "nethifier_debug.log")
                        If IO.File.Exists(DebugPathF) Then
                            My.Computer.FileSystem.WriteAllText(DebugPathF, Format(Date.Now(), "yyyy/MM/dd HH:mm:ss") & "- Disabled Debug " & Environment.NewLine, True)
                    Dim DebugPathG As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).ToString(), "nethifier_debug" & Format(Date.Now(), "yyyyMMddHHmmss") & ".log")
                    Rename(DebugPathF, DebugPathG)
                        End If

                    ElseIf Message.ToLower.StartsWith("{""action"":""reset"",") Then
                        Dim Action As CLS_ACTIONS = New CLS_ACTIONS
                        Dim Comm As New Commands

                        JsonConvert.PopulateObject(Message, Action)
                        JsonConvert.PopulateObject(Message.ToLower.Replace("{""action"":""reset"",""type"":""commands"",", "{"), Comm)

                        If Action.Action = "reset" Then
                            Do While True
                                Dim E As Boolean = True
                                For Each Com As Command In Config.Commands.Values
                                    Config.Commands.Remove(Com.Command)
                                    E = False
                                    Exit For
                                Next

                                If E Then
                                    Config.Save()
                                    Exit Do
                                End If
                            Loop

                            ReloadCommands(Comm)
                        End If

                    ElseIf Message.ToLower.StartsWith("{""commands"":") Then
                        Dim St As String() = Split(Message, vbLf)
                        Dim Comm As Commands = New Commands ' = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Commands)(Message)
                        JsonConvert.PopulateObject(St(0), Comm)

                        ReloadCommands(Comm)

                        If St.Length > 1 Then
                            Dim Colo As String = St(1).ToLower.Trim

                            If Colo.StartsWith("{""setcolorled"":") Then
                                LED(Colo.Substring(16).Replace("""}", "").Replace(":", "_"))
                            End If
                        End If

                    ElseIf Message.ToLower.StartsWith("{""setcolorled"":") Then
                        Dim M As String = Message

                        If M.IndexOf(vbLf) > 0 Then


                            Dim Colors As String() = Split(M, vbLf)
                            M = Colors(Colors.Length - 1).ToLower.Substring(16).Replace("""}", "")
                            LED(M.Replace(":", "_"))
                        Else
                            LED(M.ToLower.Substring(16).Replace("""}", "").Replace(":", "_"))
                        End If

                    ElseIf Message.StartsWith("{""ring"":") Then

                    Else
                        If Message = Msg.GetMessage("STAT_002") Then
                        Notifications.ClearNotifications()
                        Disconnect()
                        EnableProperties(True)

                    ElseIf Message = Msg.GetMessage("STAT_001") Then
                        UserLogin = New Login("login", TXT_USERNAME.Text, Token)
                        _Client.SendBytes(StrToByteArray(UserLogin.ToString))

                    ElseIf Message.ToLower.StartsWith("{""message"":""authe_ok""}") Then
                        IsAutoConnecting = False
                        IsLoggedIn = True
                        Notifications.TCPClient = _Client

                        PingServer.Enabled = True

                        Status = EnumStatus.Connected
                        EnableProperties(False)
                        If _ClickConnected Then
                            Me.Hide()
                        End If
                        _ClickConnected = False

                        'NOTIFY.Icon = GetIcon("online")
                        NOTIFY.Icon = Nethifier.My.Resources.Resources.online

                        With BUT_CONNECT
                            .Enabled = True
                            .Tag = "CONF_004"
                            .Text = Msg.GetMessage(CStr(.Tag))
                        End With

                        TryConnectionCount = 1
                        TMR_CONNECTION.Interval = 1

                        With TMR_ICONS
                            .Enabled = False
                            .Stop()
                        End With

                        Dim Icon As Integer = 0
                        If IsNumeric(Msg.GetMessage("TASK_006")) Then
                            Icon = CInt(Msg.GetMessage("TASK_006"))
                            If Not (Icon > 0 AndAlso Icon < 4) Then
                                Icon = 0
                            End If
                        End If

                        'CallToolStripMenuItem.Enabled = True
                        NOTIFY.ShowBalloonTip(20, Msg.GetMessage("TASK_004"), Msg.GetMessage("TASK_005"), CType(Icon, ToolTipIcon))

                        STRIP_STATUS.Text = Msg.GetMessage("STAT_001") & " [" & Now.ToString("hh:mm:ss") & "]"

                        LED("online")

                    ElseIf Message.Trim.ToLower = "authe_err" Then
                        Disconnect()
                        _Client.Disconnect()
                    End If

                    ' Display all other messages in the status strip.
                    'If Not dontReport Then Me.ToolStripStatusLabel1.Text = Message

                End If
            End If

        'IsAutoConnecting = False

    End Sub

    Private Sub ReloadCommands(Comm As Commands)

        Dim Hash As Hashtable = New Hashtable
        For Each K As Object In Comm.Commands
            With DirectCast(K, DictionaryEntry)
                Dim S As Object = .Value.ToString
                Dim C As Command = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Command)(S.ToString)
                Hash.Add(.Key.ToString.ToLower.Trim, C)
            End With
        Next

        'Sinc always?
        Do While True
            Dim IsFound As Boolean
            IsFound = False
            For Each Com As Command In Config.Commands.Values
                If Not Hash.ContainsKey(Com.Command.ToLower.Trim) Then
                    Config.Commands.Remove(Com.Command)
                    IsFound = True
                    Exit For
                End If
            Next
            If Not IsFound Then
                Exit Do
            End If
        Loop
        For Each Com As Command In Hash.Values
            If Not Config.Commands.ContainsKey(Com.Command) Then

                If Com.Command.ToLower = "url" Then
                    If _Browsers.Count > 0 Then
                        If _Browsers.ContainsKey("chrome.exe") Then
                            Com.RunWith = CStr(_Browsers("chrome.exe"))
                        ElseIf _Browsers.ContainsKey("firefox.exe") Then
                            Com.RunWith = CStr(_Browsers("firefox.exe"))
                        End If
                    End If
                End If

                Config.Commands.Add(Com.Command, Com)
            End If
        Next
        Config.Save()
        LoadCommands(Config.Commands, Config)

    End Sub

    Private Sub ShowDisconnectedNotification()

        Dim Icon As Integer = 0
        If IsNumeric(Msg.GetMessage("TASK_003")) Then
            Icon = CInt(Msg.GetMessage("TASK_003"))
            If Not (Icon > 0 AndAlso Icon < 4) Then
                Icon = 0
            End If
        End If
        NOTIFY.ShowBalloonTip(20, Msg.GetMessage("TASK_001"), Msg.GetMessage("TASK_002"), CType(Icon, ToolTipIcon))

    End Sub

    Private Sub ActivateReconnection()
        Dim Rand As New Random()
        Dim Num As Integer = Rand.Next(1, 10) * 1000  '(Rand.Next(1000, 10000))

        With TMR_CONNECTION
            If .Interval = 1 Then
                .Tag = CInt(4000)
            Else
                .Tag = CInt(.Tag) * 2
            End If

            _TIMER = (CInt(.Tag) + Num)

            .Interval = _TIMER
            .Enabled = True
            .Start()
        End With

        With TMR_ELAPSE
            .Enabled = True
            .Start()
        End With

        With TMR_ICONS
            .Enabled = False
            .Stop()
        End With
    End Sub

    Private Sub Disconnect(Optional IsStatus As Boolean = True)

        'DisableTimer()
        With TMR_ICONS
            .Enabled = False
            .Stop()
        End With

        IsLoggedIn = False
        Status = EnumStatus.Disconnected
        'NOTIFY.Icon = GetIcon("offline")
        NOTIFY.Icon = Nethifier.My.Resources.Resources.offline

        PingServer.Enabled = False

        With BUT_CONNECT
            .Tag = "CONF_001"
            .Text = Msg.GetMessage(CStr(.Tag))
        End With


        '2016-07-10
        UserLogin = Nothing
        'CallToolStripMenuItem.Enabled = False
        If Not Dialer Is Nothing Then
            If Not IsClosing Then
                Dialer.CloseDialer()
            End If
            Dialer = Nothing
        End If
        '2016-07-10

        If IsStatus Then
            STRIP_STATUS.Text = Msg.GetMessage("STAT_002") & " [" & Now.ToString("hh:mm:ss") & "]"
        End If

        StopRinging()
        LED("offline")
        'MessageBox.Show("OFFLINE")
    End Sub

    Private Sub EnableProperties(Enabled As Boolean, Optional IncludeButton As Boolean = False)
        TXT_PASSWORD.Enabled = Enabled
        TXT_PORT.Enabled = Enabled
        TXT_SERVER.Enabled = Enabled
        TXT_USERNAME.Enabled = Enabled
        TXT_AUTHEN.Enabled = Enabled
        CMB_MODE.Enabled = Enabled
        'NUM_NOTIFICA.Enabled = Enabled
        'AUTO_LOGIN.Enabled = Enabled
        STARTUP.Enabled = False
        'BUT_SAVE.Enabled = Enabled
    End Sub

    Private IsActivated As Boolean
    Private Sub FRM_CONFIG_Activated(sender As Object, e As EventArgs) Handles Me.Activated

        If IsActivated Then
            Exit Sub
        End If

        IsActivated = True

        If Config.AUTO_LOGIN Then
            PrepareReConnection()
        End If

    End Sub

    Private Sub FRM_CONFIG_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed

        If IsClosing Then
            If Not IsNothing(_Client) Then
                PrepareDisconnection()
            End If
        End If

    End Sub

    Private Sub FRM_CONFIG_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing

        If Not IsClosing Then
            Me.Hide()
            e.Cancel = True

        Else
            If Not IsNothing(Dialer) Then

                Dialer.CloseDialer()

            End If
        End If

    End Sub

    Private Sub FRM_CONFIG_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ExceptionManager = New ExceptionManager

        'Debug
        If NethDebug.IsActive Then
            If Not NethDebug.SHOW_ERROR_LOG_TAB Then
                TABS.TabPages.RemoveByKey("TAB_003")
            End If
            If Not NethDebug.SHOW_SERVER_MESSAGES_TAB Then
                TABS.TabPages.RemoveByKey("TAB_002")
            End If

            If NethDebug.USE_NOTIFICATION_TIMEOUT Then
                LBL_NOTIFICA_CLOSE.Visible = True
                NUM_NOTIFICA.Visible = True
            End If
        Else
            TABS.TabPages.RemoveAt(1)
            TABS.TabPages.RemoveAt(1)
        End If
        'Debug

        Me.Icon = Nethifier.My.Resources.Resources.panel

        'tmrPoll.Start()
        Me.Text = Application.ProductName

        CMB_MODE.SelectedIndex = 0

        ReadBrowsers()

        With Config
            TXT_PASSWORD.Text = .PASSWORD
            TXT_USERNAME.Text = .USERNAME
            TXT_SERVER.Text = .SERVER
            TXT_PORT.Text = .SERVER_PORT.ToString

            NUM_NOTIFICA.Value = .NOTIFY_TIMEOUT
            If NUM_NOTIFICA.Value < 10 Then
                NUM_NOTIFICA.Value = 0
                NUM_NOTIFICA.Tag = 10
            Else
                NUM_NOTIFICA.Tag = 0
            End If


            TXT_AUTHEN.Text = .AUTH_ADDRESS
            AUTO_LOGIN.Checked = .AUTO_LOGIN

            CMB_MODE.SelectedIndex = CMB_MODE.FindString(.AUTH_MODE)

            For i = 0 To HotKeys.Length - 1
                CMB_REDIAL_HOTKEY.Items.Add(HotKeys(i))
                If CInt(HotKeysValues(i)) = .HOTKEY_MOD_REDIAL Then
                    CMB_REDIAL_HOTKEY.SelectedIndex = i
                End If

                CMB_SPEEDDIAL_HOTKEY.Items.Add(HotKeys(i))
                If CInt(HotKeysValues(i)) = .HOTKEY_MOD_SPEED_DIAL Then
                    CMB_SPEEDDIAL_HOTKEY.SelectedIndex = i
                End If
            Next

            'LoadDial(CMB_SPEEDDIAL_HOTKEY, "PlayPause", .MOD_SPEED_DIAL, 256)
            LoadDial(CMB_SPEEDDIAL_HOTKEY, TXT_SPEEDDIAL_HOTKEY, .MOD_SPEED_DIAL, 100)
            LoadDial(CMB_REDIAL_HOTKEY, TXT_REDIAL_HOTKEY, .MOD_REDIAL, 200)
            RegisterHotKey(Me.Handle, 101, 2, 118)
            LoadCommands(.Commands, Config)

            TXT_SOUND.Text = .SOUND_FILE
            CHK_SOUND_LOOP.Checked = .SOUND_LOOP
            CHK_SUONERIA.Checked = .USE_RINGER
        End With

        'If Trim(TXT_SERVER.Text) = "" Then
        '    TXT_SERVER.Text = TCP_CLIENT.GetLocalIpAddress.ToString
        'End If

        Try
            'Dim APP_NAME As String = Application.ExecutablePath.Replace(Application.StartupPath & "\", "")
            'STARTUP.Checked = Not IsNothing(Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True).GetValue(APP_NAME))

            STARTUP.Checked = False
            Dim Files() As String = IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup))
            For I As Integer = 0 To Files.Length - 1
                If LinkHelper.ResolveShortcut(Files(I)).ToLower = Application.ExecutablePath.ToLower Then
                    'STARTUP.Checked = True
                    STARTUP.Checked = False
                    STARTUP.Enabled = False
                    Exit For
                End If
            Next

            If Not STARTUP.Checked Then
                Files = IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Startup))
                For I As Integer = 0 To Files.Length - 1
                    If LinkHelper.ResolveShortcut(Files(I)).ToLower = Application.ExecutablePath.ToLower Then
                        STARTUP.Checked = True
                        Exit For
                    End If
                Next
            End If

            LoadSoundDevices()

        Catch ex As Exception
            ExceptionManager.Write(ex)
        End Try

    End Sub

    Private Sub LoadSoundDevices()
        SoundDevices = New ArrayList

        CMB_DEVICE.Items.Clear()

        Dim waveOutDevices As Integer = NAudio.Wave.WaveOut.DeviceCount
        For waveOutDevice As Integer = 0 To waveOutDevices - 1

            Dim deviceInfo As NAudio.Wave.WaveOutCapabilities = NAudio.Wave.WaveOut.GetCapabilities(waveOutDevice)
            CMB_DEVICE.Items.Add("Device: " & Convert.ToString(deviceInfo.ProductName))
            SoundDevices.Add(deviceInfo.ProductGuid.ToString)

            If deviceInfo.ProductGuid.ToString() = Config.SOUND_DEVICE Then
                CMB_DEVICE.SelectedIndex = (CMB_DEVICE.Items.Count - 1)
            End If
        Next
        If CMB_DEVICE.SelectedIndex <= 0 AndAlso CMB_DEVICE.Items.Count > 0 Then
            CMB_DEVICE.SelectedIndex = 0
        End If

        TMR_DEVICE.Start()
    End Sub

    Private Sub LoadDial(C As ComboBox, T As TextBox, R As Integer, Code As Integer)

        If C.SelectedIndex < 0 Then
            C.SelectedIndex = 0
            T.Tag = 0
        Else
            If IsNumeric(R) AndAlso CInt(R) > 0 Then
                T.Tag = CInt(R)

                Dim V = [Enum].Parse(GetType(System.Windows.Forms.Keys), R.ToString)
                If IsNumeric(V.ToString) Then
                    V = Chr(CInt(V))
                End If
                T.Text = V.ToString.ToUpper.Replace("NUMPAD", "")
                RegisterHotKey(Me.Handle, Code, CInt(HotKeysValues(C.SelectedIndex)), R)
            End If
        End If
    End Sub

    Private Sub LoadCommands(Commands As Hashtable, Config As Config)

        PAN_CONTROLS.Controls.Clear()

        Dim Line As Integer = 0
        For Each Com As Command In Commands.Values
            Dim TXT As TextBox = New TextBox
            Dim BUT As Button = New Button
            Dim CMB As ComboBox = New ComboBox
            Dim BRW As ComboBox = New ComboBox

            With CMB
                .Name = "COM_" & Com.Command
                .DropDownStyle = ComboBoxStyle.DropDownList
                .Width = 75
                .Top = (Line * 25) + 2
                .Tag = Line
                .Left = 2
                .Items.Add(Com.Command)
                .SelectedIndex = 0
            End With

            With BUT
                .Name = Com.Command
                .Width = 50
                .Height = 24
                .Top = Line * 25
                .Text = "..."
            End With
            If Com.Command.Trim.ToLower = "url" Then
                With BRW
                    .Name = "BROWSER_" & Com.Command
                    .DropDownStyle = ComboBoxStyle.DropDown
                    .Width = 255
                    .Top = (Line * 25) + 2
                    .Tag = CMB
                    .Left = (CMB.Width + CMB.Left + 2)

                    Try
                        For Each K As String In _Browsers.Keys
                            .Items.Add(_Browsers(K))
                        Next
                    Catch ex As Exception
                        '
                    End Try

                    .Text = Com.RunWith
                End With
                With BUT
                    .Tag = BRW
                    .Left = (BRW.Width + BRW.Left + 2)
                End With
            Else
                With TXT
                    .Width = 255
                    .Top = (Line * 25) + 2
                    .Tag = CMB
                    .Left = (CMB.Width + CMB.Left + 2)
                    .Text = Com.RunWith
                End With
                With BUT
                    .Tag = TXT
                    .Left = (TXT.Width + TXT.Left + 2)
                End With
            End If

            AddHandler BUT.Click, AddressOf Sfoglia

            With PAN_CONTROLS.Controls
                .Add(BUT)
                .Add(CMB)
                If Com.Command.Trim.ToLower = "url" Then
                    .Add(BRW)
                Else
                    .Add(TXT)
                End If
            End With

            Line += 1
        Next
        PAN_CONTROLS.Tag = Line

        '2017-12-04
        'Recreate NOTIFICATION
        Notifications = New FRM_NOTIFICATIONS(Config)
    End Sub

    Private _ClickConnected As Boolean
    Private Sub BUT_CONNECT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BUT_CONNECT.Click, ConnectToolStripMenuItem.Click
        'L'autoconnessione funzionerà solo se la disconnessione è provocata senza intervento dell'utente

        If Not Me.IsLoggedIn AndAlso TMR_ELAPSE.Enabled Then
            InteruptReconnection()
        End If

        DisableTimer()

        '2016/06/29
        BUT_CONNECT.Tag = "FORCING_CONNECTION"
        '2016/06/29

        TryConnectionCount = 6

        ConnectToServer("TLS")

    End Sub

    Private Sub DisableTimer()
        With TMR_CONNECTION
            .Enabled = False
            .Stop()
        End With
        With TMR_ELAPSE
            .Enabled = False
            .Stop()
        End With
        With TMR_ICONS
            .Enabled = False
            .Stop()
        End With
        TMR_CONNECTION.Interval = 1
        IsAutoConnecting = False
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        If MessageBox.Show(Msg.GetMessage("CTX_007"), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            IsClosing = True

            Try
                LED("offline")
                Threading.Thread.Sleep(1000)

                If Not Dialer Is Nothing Then
                    Dialer.CloseDialer()
                    Dialer.Dispose()
                End If
                If Not LED_Proc Is Nothing Then
                    LED_Proc.Kill()
                End If
            Catch ex As Exception
                'MessageBox.Show(ex.Message)
            End Try

            Application.Exit()
        End If
    End Sub

    Private Sub ShowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowToolStripMenuItem.Click
        If ShowToolStripMenuItem.Text = Msg.GetMessage("CTX_003") Then
            Me.Show()
        Else
            Me.Hide()
        End If
    End Sub

    Private Sub CTX_MENU_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles CTX_MENU.Opening
        If Me.Visible Then
            ShowToolStripMenuItem.Text = Msg.GetMessage("CTX_002")
        Else
            ShowToolStripMenuItem.Text = Msg.GetMessage("CTX_003")
        End If

        CallToolStripMenuItem.Enabled = Not IsNothing(UserLogin)

        'With ConnectToolStripMenuItem
        '    If Not IsNothing(_Client) Then
        '        If _Client.isClientRunning Then
        '            .Text = Msg.GetMessage("CTX_005")
        '        Else
        '            .Text = Msg.GetMessage("CTX_004")
        '        End If
        '    Else
        '        .Text = Msg.GetMessage("CTX_004")
        '    End If
        'End With

    End Sub

    Private Sub ConnectToServer(Mode As String)
        Dim Nonce As String = ""
        Dim Username As String = TXT_USERNAME.Text.Trim
        Dim Password As String = TXT_PASSWORD.Text.Trim
        Dim HostName As String = TXT_SERVER.Text

        Try
            'If My.Computer.Network.IsAvailable Then
            '    'OK LAN CONNECTION
            '    If My.Computer.Network.Ping("www.google.com") Then
            '        'OK INTERNET CONNECTION AVAILABLE
            '    Else
            '        MessageBox.Show("No internet connection available")
            '        Exit Sub
            '    End If
            'Else
            '    MessageBox.Show("No connection available")
            '    Exit Sub
            'End If

            If Not Me.IsLoggedIn Then
                If Trim(HostName) = "" Then
                    Disconnect(False)

                    If IsAutoConnecting Then
                        Exit Sub
                    End If
                    MessageBox.Show(Msg.GetMessage("REQ_003"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    If Me.Visible Then
                        TXT_SERVER.Focus()
                    End If

                    Exit Sub
                End If
                If Trim(Username) = "" Then
                    Disconnect(False)

                    If IsAutoConnecting Then
                        Exit Sub
                    End If
                    MessageBox.Show(Msg.GetMessage("REQ_001"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    If Me.Visible Then
                        TXT_USERNAME.Focus()
                    End If

                    Exit Sub
                End If
                If Trim(Password) = "" Then
                    Disconnect(False)

                    If IsAutoConnecting Then
                        Exit Sub
                    End If
                    MessageBox.Show(Msg.GetMessage("REQ_002"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    If Me.Visible Then
                        TXT_PASSWORD.Focus()
                    End If

                    Exit Sub
                End If

                _Client = New TCP_CLIENT(AddressOf UpdateUI)
                Status = EnumStatus.Connecting

                'If Username = "" Then
                '    TXT_USERNAME.Focus()
                '    Exit Sub
                'End If
                'If Password = "" Then
                '    TXT_PASSWORD.Focus()
                '    Exit Sub
                'End If

                BUT_CONNECT.Enabled = False
                EnableProperties(False)

                Application.DoEvents()

                Dim AuthType As String = CStr(CMB_MODE.SelectedItem)

                'Debug
                If NethDebug.IsActive AndAlso Not NethDebug.USE_HTTP_AUTH Then
                    AuthType = "TCP"
                End If
                'Debug

                If AuthType = "HTTP" Then

                    '2014-05-13
                    With TMR_ICONS
                        If Not .Enabled Then
                            .Tag = 1
                            .Enabled = True
                            .Start()
                        End If
                    End With
                    '2014-05-13

                    For T As Integer = 1 To 5
                        DoHTTPConnect(Username, Password, Nonce, T)

                        If Trim(Nonce) <> "" Then
                            Exit For
                        End If
                    Next

                    If Nonce.ToLower.IndexOf("digest ", StringComparison.Ordinal) = 0 Then
                        Nonce = Nonce.Substring(7)
                    Else
                        Nonce = ""
                    End If

                    If Nonce.Trim <> "" Then
                        IsLoggedIn = True
                        'Proceed to TCP authentication only if 401 HTTP Status code is given and WWW-AUTHENTICATE is passed
                        TCPConnection(Username, Password, Nonce, Mode)

                        RegEdit(True)
                    Else
                        Disconnect()
                    End If

                Else

                    'Connection through TCP/IP
                    Me.IsLoggedIn = True

                    Nonce = "Digest 5031d941058935fa578f7f1225973b3ab240a387"
                    TCPConnection(Username, Password, Nonce, Mode)
                End If
            Else
                PrepareDisconnection()
                Disconnect()
            End If
        Catch ex As Exception

            If Not IsAutoConnecting Then
                DoShowError(ex)
            Else
                ActivateReconnection()

                'If TryConnectionCount >= 5 Then
                '    ShowDisconnectedNotification()
                '    TryConnectionCount = 0
                '    TMR_CONNECTION.Interval = 1
                '    IsAutoConnecting = False
                'Else
                '    ActivateReconnection()
                'End If

            End If

        End Try

    End Sub

    Private Sub DoHTTPConnect(Username As String, Password As String, ByRef Nonce As String, Attemp As Integer)
        'Gestione Expired HTTPS certificate
        System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf CertificateHandler

        Dim s As WebRequest  'HttpWebRequest
        Dim enc As UTF8Encoding
        Dim postdata As String
        Dim postdatabytes As Byte()

        Dim Url As String = "https://" & TXT_SERVER.Text & "/webrest/authentication/login" 'TXT_AUTHEN.Text

        'NICK!
        'Dim Url As String = "https://"
        'With NethDebug
        '    If .IsActive Then
        '        Url += .DEBUG_HTTP_SERVER_ADDRESS
        '        If .DEBUG_HTTP_SERVER_PORT <> "" Then
        '            Url += ":" & .DEBUG_HTTP_SERVER_PORT
        '        End If
        '    Else
        '        Url += TXT_SERVER.Text
        '    End If
        'End With
        'Url += "/webrest/authentication/login"

        'Url = TXT_AUTHEN.Text

        s = WebRequest.Create(Url)
        enc = New System.Text.UTF8Encoding()
        postdata = "username=" & HttpUtility.UrlEncode(Username) & "&password=" & HttpUtility.UrlEncode(Password)
        postdatabytes = enc.GetBytes(postdata)

        s.Method = "POST"
        's.Timeout = 30
        s.ContentType = "application/x-www-form-urlencoded"
        s.ContentLength = postdatabytes.Length

        Using Stream = s.GetRequestStream()
            Stream.Write(postdatabytes, 0, postdatabytes.Length)
        End Using

        ''Token
        Try
            Nonce = Trim("" & s.GetResponse.Headers("www-authenticate"))
        Catch ex As Exception
            Dim DoExit As Boolean = True

            If TypeOf ex Is WebException Then
                Dim wEx As WebException = DirectCast(ex, WebException)
                If wEx.Response IsNot Nothing Then

                    'can use ex.Response.Status, .StatusDescription

                    Dim Resp As HttpWebResponse = DirectCast(wEx.Response, HttpWebResponse)
                    If Resp.StatusCode = HttpStatusCode.Unauthorized Then
                        Nonce = wEx.Response.Headers.Item("www-authenticate")
                        DoExit = False
                    Else

                        'Forse sta riavviando il server
                        If Resp.StatusCode = HttpStatusCode.ServiceUnavailable AndAlso Attemp < 5 Then
                            'Retry connection for 5 times???
                            Application.DoEvents()
                            Threading.Thread.Sleep(2000)
                            Application.DoEvents()
                        Else
                            DoShowError(New Exception(Resp.StatusDescription & " (" & Resp.StatusCode & ")"))
                            If IsAutoConnecting Then
                                'Auto connect
                                If TryConnectionCount <= 5 Then
                                    ActivateReconnection()
                                Else
                                    InteruptReconnection()
                                End If
                            End If
                        End If

                    End If

                End If
            End If

            If DoExit Then
                Nonce = ""
                'Exit Sub
            End If
        End Try
    End Sub

    Private Sub PrepareReConnection()

        With TMR_ICONS
            .Tag = 1
            .Enabled = True
            .Start()
        End With

        'TMR_ICONS_Tick(Nothing, Nothing)

        IsAutoConnecting = True
        TryConnectionCount += 1
        ConnectToServer("TLS")

    End Sub

    Private Sub PrepareDisconnection()

        If Not (Status = EnumStatus.Connected AndAlso Me.IsLoggedIn) Then
            Exit Sub
        End If

        Status = EnumStatus.Disconnecting

        'Gestione Expired HTTPS certificate
        System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf CertificateHandler

        Try

            If NethDebug.IsActive AndAlso Not NethDebug.USE_HTTP_AUTH Then
                Dim s As WebRequest
                Dim enc As UTF8Encoding
                Dim postdatabytes As Byte()
                Dim Url As String = "https://" & TXT_SERVER.Text & "/webrest/authentication/logout" 'TXT_AUTHEN.Text

                s = WebRequest.Create(Url)
                enc = New System.Text.UTF8Encoding()
                postdatabytes = enc.GetBytes("")

                s.Headers.Add("Authorization", Config.USERNAME.ToLower & ":" & Me.Token.ToLower)
                s.Method = "POST"
                s.ContentType = "application/x-www-form-urlencoded"
                s.ContentLength = postdatabytes.Length
                Using Stream = s.GetRequestStream()
                    Stream.Write(postdatabytes, 0, postdatabytes.Length)
                End Using
                Dim Resp As WebResponse = s.GetResponse()
            End If

        Catch ex As Exception
            ExceptionManager.Write(ex)
        End Try

        If Not IsNothing(_Client) Then
            _Client.SendBytes(StrToByteArray("{""action"":""logout"",""username"":""" & Config.USERNAME.ToLower & """,""token"":""" & Me.Token.ToLower & """}"))
            _Client.Disconnect()
        End If
    End Sub

    Private Sub DoShowError(Ex As Exception)

        'If Me.Visible Then
        '    MessageBox.Show(Ex.Message, Msg.GetMessage("CONF_020"), MessageBoxButtons.OK, MessageBoxIcon.Error)
        'End If

        TXT_ERROR_LOG.Text += Now.ToString("yyyy-MM-dd hh:mm:ss") & " - " & Ex.Message & vbCrLf
        ExceptionManager.Write(Ex)

        BUT_CONNECT.Enabled = True
        EnableProperties(True, True)
    End Sub

    Private Sub TCPConnection(Username As String, Password As String, Nonce As String, Mode As String)
        If Me.IsLoggedIn Then
            If Status = EnumStatus.Connecting Then
                'Reimpostare la variabile, perchè dobbiamo risettarla dopo il successo del secondo login (TCP)
                IsLoggedIn = False

                Dim HMACSHA_1 As HMACSHA1 = New HMACSHA1(Encoding.UTF8.GetBytes(Password))
                Dim ByteToken As Byte() = HMACSHA_1.ComputeHash(Encoding.UTF8.GetBytes(Username & ":" & Password & ":" & Nonce))
                Token = Replace(BitConverter.ToString(ByteToken), "-", "")
                HMACSHA_1.Clear()

                If Me.Token <> "" Then
                    _ClickConnected = True

                    If NethDebug.IsActive AndAlso NethDebug.USE_DEBUG_TCP_SERVER Then
                        _Client.Connect(NethDebug.DEBUG_TCP_SERVER_IP.Trim, Convert.ToInt32(NethDebug.DEBUG_TCP_SERVER_PORT.Trim), Mode)
                    Else
                        _Client.Connect(Me.TXT_SERVER.Text.Trim, Convert.ToInt32(Me.TXT_PORT.Text.Trim), Mode)
                    End If

                    'Dim HMACSHA_2 As HMACSHA1 = New HMACSHA1(Encoding.ASCII.GetBytes(Password))
                    'Dim ByteToken2 As Byte() = HMACSHA_2.ComputeHash(Encoding.ASCII.GetBytes(Username & ":" & Password & ":" & Nonce))
                    'Token = GetBytesToHexadeciString(ByteToken2)
                    '_Client.SendBytes(StrToByteArray(Token))
                    'HMACSHA_2.Clear()
                End If
            End If
        End If
        'BUT_CONNECT.Enabled = True
        Application.DoEvents()
    End Sub

    Private Function GetBytesToHexadeciString(ByVal bytes As Byte()) As String
        Dim output As String = String.Empty
        Dim i As Integer = 0
        Do While i < bytes.Length
            output += bytes(i).ToString("X2")
            i += 1
        Loop
        Return output
    End Function

    Private Shared Function CertificateHandler(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal SSLerror As SslPolicyErrors) As Boolean
        Return True
    End Function

    Private Sub BUT_SAVE_Click(sender As Object, e As EventArgs) Handles BUT_SAVE.Click
        RegEdit(False)
        If My.Application.CommandLineArgs.Contains("-R") Then
            Application.Exit()
        End If

    End Sub

    Private Sub RegEdit(IsAutoSave As Boolean)

        Try
            Dim CNDX = CMB_MODE.SelectedIndex
            If CNDX < 0 Then
                CMB_MODE.SelectedIndex = 0
            End If
            With Config
                .AUTH_MODE = CStr(CMB_MODE.Items(CMB_MODE.SelectedIndex))
                .PASSWORD = TXT_PASSWORD.Text
                .USERNAME = TXT_USERNAME.Text
                .SERVER = TXT_SERVER.Text

                If Trim(.USERNAME) = "" Then
                    MessageBox.Show(Msg.GetMessage("REQ_001"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    TXT_USERNAME.Focus()
                    Exit Sub
                End If
                If Trim(.PASSWORD) = "" Then
                    MessageBox.Show(Msg.GetMessage("REQ_002"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    TXT_PASSWORD.Focus()
                    Exit Sub
                End If
                If .SERVER_PORT <= 0 Then
                    MessageBox.Show(Msg.GetMessage("REQ_003"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    TXT_PORT.Focus()
                    Exit Sub
                End If

                .SERVER_PORT = CInt(TXT_PORT.Text)
                .NOTIFY_TIMEOUT = CInt(NUM_NOTIFICA.Value)
                .AUTH_ADDRESS = TXT_AUTHEN.Text
                .AUTO_LOGIN = AUTO_LOGIN.Checked

                If CHK_SUONERIA.Checked Then
                    If Not IO.File.Exists(TXT_SOUND.Text) Then
                        Dim RingerPath As String = IO.Path.Combine(Application.StartupPath, "ringer.mp3")
                        If IO.File.Exists(RingerPath) Then
                            TXT_SOUND.Text = RingerPath
                        End If
                    End If
                Else
                    TXT_SOUND.Text = ""
                End If
                .SOUND_FILE = TXT_SOUND.Text
                .SOUND_LOOP = CHK_SOUND_LOOP.Checked
                .USE_RINGER = CHK_SUONERIA.Checked

                .HOTKEY_MOD_SPEED_DIAL = HotKeysValues(CMB_SPEEDDIAL_HOTKEY.SelectedIndex)
                .HOTKEY_MOD_REDIAL = HotKeysValues(CMB_REDIAL_HOTKEY.SelectedIndex)
                .MOD_SPEED_DIAL = CInt(TXT_SPEEDDIAL_HOTKEY.Tag)
                .MOD_REDIAL = CInt(TXT_REDIAL_HOTKEY.Tag)

                If CMB_DEVICE.SelectedIndex >= 0 Then
                    .SOUND_DEVICE = CStr(SoundDevices.Item(CMB_DEVICE.SelectedIndex))
                End If

                For Each Ctl As Control In PAN_CONTROLS.Controls
                    If TypeOf Ctl Is TextBox Then
                        Dim CommStr As String = DirectCast(Ctl.Tag, ComboBox).Text
                        If .Commands.ContainsKey(CommStr) Then
                            .Commands(CommStr) = New Command(CommStr, Ctl.Text)
                        Else
                            .Commands.Add(CommStr.Trim.ToLower, New Command(CommStr, Ctl.Text))
                        End If
                    ElseIf TypeOf Ctl Is ComboBox Then
                        If DirectCast(Ctl, ComboBox).Name.StartsWith("BROWSER_") Then
                            Dim CommStr As String = DirectCast(Ctl.Tag, ComboBox).Text
                            If .Commands.ContainsKey(CommStr) Then
                                .Commands(CommStr) = New Command(CommStr, Ctl.Text)
                            Else
                                .Commands.Add(CommStr.Trim.ToLower, New Command(CommStr, Ctl.Text))
                            End If
                        End If
                    End If
                Next
                .Save()

                'Unregister keyhook
                If IsNumeric(TXT_SPEEDDIAL_HOTKEY.Tag) Then
                    UnregisterHotKey(Me.Handle, 100)
                End If
                If IsNumeric(TXT_REDIAL_HOTKEY.Tag) Then
                    UnregisterHotKey(Me.Handle, 200)
                End If
                'Register keyhook
                LoadDial(CMB_SPEEDDIAL_HOTKEY, TXT_SPEEDDIAL_HOTKEY, .MOD_SPEED_DIAL, 100)
                LoadDial(CMB_REDIAL_HOTKEY, TXT_REDIAL_HOTKEY, .MOD_REDIAL, 200)
            End With

            If STARTUP.Checked Then
                'Registry.SetValue("HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run", APP_NAME, Application.ExecutablePath)
                Dim shell As WshShell = New WshShell
                Dim link As IWshShortcut
                Dim StartUpPath As String = Environment.GetFolderPath(Environment.SpecialFolder.Startup) & "\" & Application.ProductName & ".lnk"
                If Not IO.File.Exists(StartUpPath) Then
                    Try
                        link = DirectCast(shell.CreateShortcut(StartUpPath), IWshShortcut)
                        With link
                            .TargetPath = Application.ExecutablePath
                            .Arguments = "-e"
                            .Description = Application.ProductName
                            .WorkingDirectory = Application.StartupPath
                            .IconLocation = Application.ExecutablePath & ", 0"
                            .Save()
                        End With

                    Catch ex As Exception
                        ExceptionManager.Write(ex)
                    End Try
                End If
            Else
                Dim Files() As String = IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Startup))
                For I As Integer = 0 To Files.Length - 1
                    If LinkHelper.ResolveShortcut(Files(I)).ToLower = Application.ExecutablePath.ToLower Then
                        IO.File.Delete(Files(I))
                    End If
                Next
            End If

            If Not IsAutoSave Then
                MessageBox.Show(Msg.GetMessage("CONF_017"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If

        Catch ex As Exception

            If Not IsAutoSave Then
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            ExceptionManager.Write(ex)
        End Try

    End Sub

    Private Sub CMB_MODE_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CMB_MODE.SelectedIndexChanged
        If CMB_MODE.SelectedIndex = 0 Then
            LBL_URL.Text = Msg.GetMessage("CONF_012")
        Else
            LBL_URL.Text = Msg.GetMessage("CONF_013")
        End If
    End Sub

    Private LED_Proc As Process
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        Config = New Config
        Msg = New MessageHelper(Config)

        NethDebug = New NethDebugger

        ' Add any initialization after the InitializeComponent() call.
        SetCaptions()

        Try
            Dim Proc = New ProcessStartInfo
            With Proc
                .UseShellExecute = True
                .WorkingDirectory = Environment.CurrentDirectory
                '.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\" & Application.ProductName
                '.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\Nethesis\" & Application.ProductName
                '.WorkingDirectory = Application.StartupPath

                '.WorkingDirectory = Path.GetFileName(Application.StartupPath)
                .FileName = "NethLED.exe"

                Dim Args As String = ""
                For Each Arg In My.Application.CommandLineArgs
                    Args += Arg & " "
                Next

                .Arguments = Args
                '.Verb = "runas"
            End With
            If My.Computer.FileSystem.FileExists(Proc.WorkingDirectory + "/" + Proc.FileName) Then
                LED_Proc = Process.Start(Proc)
            Else
                Console.WriteLine(Proc.WorkingDirectory + "/" + Proc.FileName)
                'MsgBox("NethLED file not found.")
                Application.Exit()
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message + ":NetLED")
        Finally
        End Try

        NOTIFY.Icon = Nethifier.My.Resources.Resources.offline

    End Sub

    Private Sub SetCaptions()
        With BUT_CONNECT
            If IsNothing(.Tag) Then
                .Tag = "CONF_001"
            End If
            .Text = Msg.GetMessage(CStr(.Tag))
        End With

        Me.gbIpAddress.Text = Msg.GetMessage("CONF_003")
        Me.BUT_SAVE.Text = Msg.GetMessage("CONF_002")
        Me.STARTUP.Text = Msg.GetMessage("CONF_005")
        Me.AUTO_LOGIN.Text = Msg.GetMessage("CONF_006")
        gbServerPort.Text = Msg.GetMessage("CONF_007")

        LBL_NOTIFICA_CLOSE.Text = Msg.GetMessage("CONF_008")
        GRP_AUTH.Text = Msg.GetMessage("CONF_009")
        LBL_USER.Text = Msg.GetMessage("CONF_010")
        LBL_PASSWORD.Text = Msg.GetMessage("CONF_011")
        LBL_MODALITA.Text = Msg.GetMessage("CONF_014")
        LBL_URL.Text = Msg.GetMessage("CONF_012")
        LBL_RUN.Text = Msg.GetMessage("CONF_015")
        LBL_COMMAND.Text = Msg.GetMessage("CONF_016")

        TAB_001.Text = Msg.GetMessage("TAB_001")
        TAB_002.Text = Msg.GetMessage("TAB_002")
        TAB_003.Text = Msg.GetMessage("TAB_003")
        TAB_004.Text = Msg.GetMessage("TAB_004")

        ExitToolStripMenuItem.Text = Msg.GetMessage("CTX_001")
        LanguageToolStripMenuItem.Text = Msg.GetMessage("CTX_006")

        LanguageToolStripMenuItem.DropDownItems.Clear()
        For Each L As String In Config.Languages.Values
            Dim BUT As ToolStripMenuItem = CType(LanguageToolStripMenuItem.DropDownItems.Add(L), ToolStripMenuItem)
            BUT.Tag = L

            BUT.Checked = (L.ToLower.Trim = Config.LANGUAGE.ToLower.Trim)

            AddHandler BUT.Click, AddressOf LanguageSelect
        Next

        SetTooltip()
    End Sub

    Private Sub LanguageSelect(sender As Object, e As EventArgs)
        Config.LANGUAGE = CType(sender, ToolStripMenuItem).Tag.ToString
        Config.Save()
        Msg = New MessageHelper(Config)
        SetCaptions()
    End Sub

    Private Sub Notifications_VisibleChanged(sender As Object, e As EventArgs) Handles Notifications.VisibleChanged
        If Notifications.GetNotificationCount <= 0 Then
            'NOTIFY.Icon = GetIcon("online")
            NOTIFY.Icon = Nethifier.My.Resources.Resources.online

        End If
    End Sub

    Private Sub Sfoglia(sender As Object, e As EventArgs)

        With FILE_DLG
            If TypeOf DirectCast(sender, Button).Tag Is ComboBox Then
                .FileName = DirectCast(DirectCast(sender, Button).Tag, ComboBox).Text
            Else
                .FileName = DirectCast(DirectCast(sender, Button).Tag, TextBox).Text
            End If

            .CheckFileExists = True
            .Filter = "Executable|*.exe"
            If .ShowDialog() = Windows.Forms.DialogResult.OK Then
                If Trim(.FileName) <> "" Then
                    If TypeOf DirectCast(sender, Button).Tag Is ComboBox Then
                        DirectCast(DirectCast(sender, Button).Tag, ComboBox).Text = .FileName
                    Else
                        DirectCast(DirectCast(sender, Button).Tag, TextBox).Text = .FileName
                    End If
                End If
            End If
        End With
    End Sub

    Private TryConnectionCount As Integer = 0
    Private Sub TMR_CONNECTION_Tick(sender As Object, e As EventArgs) Handles TMR_CONNECTION.Tick
        With TMR_CONNECTION
            .Enabled = False
            .Stop()
        End With
        With TMR_ELAPSE
            .Enabled = False
            .Stop()
        End With

        If TryConnectionCount > 5 Then
            InteruptReconnection()
        Else
            PrepareReConnection()
        End If

    End Sub

    Private Sub InteruptReconnection()
        TryConnectionCount = 0
        TMR_CONNECTION.Interval = 1
        IsAutoConnecting = False

        With TMR_ICONS
            .Enabled = False
            .Stop()
        End With

        ShowDisconnectedNotification()
        Disconnect()

        BUT_CONNECT.Enabled = True

        EnableProperties(True)
    End Sub

    Private Sub NOTIFY_BalloonTipClicked(sender As Object, e As EventArgs) Handles NOTIFY.BalloonTipClicked
        Me.Show()
    End Sub

    Private Sub TABS_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TABS.SelectedIndexChanged
        BUT_RESET_RUNWITH.Visible = (TABS.SelectedTab.Name = TAB_004.Name)
        BUT_RESET_RUNWITH.Enabled = (Not IsNothing(_Client) AndAlso _Client.isClientRunning)
    End Sub

    Private Sub BUT_RESET_RUNWITH_Click(sender As Object, e As EventArgs) Handles BUT_RESET_RUNWITH.Click
        '_Client.SendBytes("")
        If IsNothing(_Client) Then
            Exit Sub
        End If

        If _Client.isClientRunning Then
            If MessageBox.Show(Msg.GetMessage("CONF_021"), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
                _Client.SendBytes(StrToByteArray("{""action"":""reset"",""username"":""" & Config.USERNAME.ToLower & """,""token"":""" & Me.Token.ToLower & """,""type"":""commands""}"))
            End If
        End If

    End Sub

    Private Sub NUM_NOTIFICA_ValueChanged(sender As Object, e As EventArgs) Handles NUM_NOTIFICA.ValueChanged

        With NUM_NOTIFICA
            If .Value < 10 Then
                If IsNumeric(.Tag) Then
                    If CDec(.Tag) = 0 Then
                        .Value = 0
                        .Tag = 10
                    Else
                        .Value = 10
                    End If
                Else
                    .Value = 0
                End If
            Else
                .Tag = 0
            End If

            If .Value = 0 Then
                TIP.Show(Msg.GetMessage("TIP_007", True), NUM_NOTIFICA)
            End If
        End With

    End Sub

    Private Sub PingServer_Tick(sender As Object, e As EventArgs) Handles PingServer.Tick
        If IsNothing(_Client) Then
            Exit Sub
        End If
        If _Client.isClientRunning Then
            If _Client.SendBytes("{""action"":""ping""}") Then
                'OK
            Else
                'Disconnected???
            End If
        End If
    End Sub

    Private Sub SetTooltip()
        TIP.SetToolTip(TXT_SERVER, Msg.GetMessage("TIP_001", True))
        TIP.SetToolTip(TXT_PORT, Msg.GetMessage("TIP_002", True))
        TIP.SetToolTip(TXT_USERNAME, Msg.GetMessage("TIP_003", True))
        TIP.SetToolTip(TXT_PASSWORD, Msg.GetMessage("TIP_004", True))
        TIP.SetToolTip(STARTUP, Msg.GetMessage("TIP_005", True))
        TIP.SetToolTip(AUTO_LOGIN, Msg.GetMessage("TIP_006", True))
        TIP.SetToolTip(NUM_NOTIFICA, Msg.GetMessage("TIP_007", True))
        TIP.SetToolTip(BUT_SAVE, Msg.GetMessage("TIP_008", True))
        TIP.SetToolTip(BUT_RESET_RUNWITH, Msg.GetMessage("TIP_009", True))
    End Sub

    Private _Browsers As New Hashtable
    Private Sub ReadBrowsers()
        Dim Browser As String() = {"chrome.exe", "firefox.exe", "iexplore.exe", "opera.exe", "safari.exe"}
        Dim B As String = ""
        For I As Integer = 0 To Browser.Length - 1
            B = ("" & CStr(Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" & Browser(I), "", ""))).Replace("""", "").ToLower
            If Trim(B) <> "" Then
                With _Browsers
                    .Add(Browser(I), B)
                End With
            End If
        Next
    End Sub

    Private _TIMER As Integer
    Private Sub TMR_ELAPSE_Tick(sender As Object, e As EventArgs) Handles TMR_ELAPSE.Tick

        _TIMER = (_TIMER - TMR_ELAPSE.Interval)

        Dim _T As Double = (_TIMER / 1000)
        Dim _S As Double = 0
        Dim _M As Double = 0
        Dim _H As Double = 0

        If _T > 59 Then
            _S = (_T Mod 60)
            'Convert to minute
            _T = (_T - _S)
            _T = (_T / 60)
            If _T > 59 Then
                _M = (_T Mod 60)
                'Convert to hour
                _T = (_T - _M)
                _H = (_T / 60)
            Else
                _M = _T
            End If
        Else
            _S = _T
        End If

        Dim Elapse As String = CStr(IIf(_H < 10, "0" & CInt(_H).ToString, CInt(_H).ToString).ToString & ":" & IIf(_M < 10, "0" & CInt(_M).ToString, CInt(_M).ToString).ToString & ":" & IIf(_S < 10, "0" & CInt(_S).ToString, CInt(_S).ToString).ToString)

        STRIP_STATUS.Text = Msg.GetMessage("STAT_003") & " " & Elapse & " - (" & TryConnectionCount & ")"
    End Sub

    Private Sub TMR_ICONS_Tick(sender As Object, e As EventArgs) Handles TMR_ICONS.Tick
        'NOTIFY.Icon = GetIcon("reconnect_" & TMR_ICONS.Tag.ToString) 
        Select Case TMR_ICONS.Tag.ToString
            Case "1", "5", "9"
                NOTIFY.Icon = Nethifier.My.Resources.Resources.reconnect_1
            Case "2", "6", "10"
                NOTIFY.Icon = Nethifier.My.Resources.Resources.reconnect_2
            Case "3", "7", "11"
                NOTIFY.Icon = Nethifier.My.Resources.Resources.reconnect_3
            Case "4", "8", "12"
                NOTIFY.Icon = Nethifier.My.Resources.Resources.reconnect_4
            Case Else
                InteruptReconnection()
                ConnectToServer("PLAIN")
        End Select

        Console.WriteLine(Status)

        TMR_ICONS.Tag = CInt(TMR_ICONS.Tag) + 1
        If CInt(TMR_ICONS.Tag) > 13 Then
            TMR_ICONS.Tag = 1
        End If
    End Sub

    Private Sub BUT_CONNECT_EnabledChanged(sender As Object, e As EventArgs) Handles BUT_CONNECT.EnabledChanged
        ConnectToolStripMenuItem.Enabled = BUT_CONNECT.Enabled
    End Sub

    Private Sub BUT_CONNECT_TextChanged(sender As Object, e As EventArgs) Handles BUT_CONNECT.TextChanged
        ConnectToolStripMenuItem.Text = BUT_CONNECT.Text
    End Sub

    Private Sub INFO_Click(sender As Object, e As EventArgs) Handles INFO.Click
        Dim INFORMATION As New FRM_ABOUT(UserLogin, Config)
        With INFORMATION
            .ShowDialog()
        End With
    End Sub

    Private Dialer As FRM_PHONECALLER
    Private Sub CallToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CallToolStripMenuItem.Click
        OpenDialer()
    End Sub

    Private Sub OpenDialer(Optional PhoneNumber As String = "")
        If IsNothing(UserLogin) Then
            Return
        End If

        If Dialer Is Nothing Then
            Dialer = New FRM_PHONECALLER(UserLogin, Config, NethDebug)
        End If

        With Dialer
            If PhoneNumber = "" Then
                'CALL
                .Show()
                .Location = New Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width - Dialer.Width, Screen.PrimaryScreen.WorkingArea.Size.Height - Dialer.Height)
                .Opacity = 1
                .Focus()
            Else
                'SPEEDDIAL
                .TXT_PHONE_NUMBER.Text = PhoneNumber
                .DoTelephoneCall()
            End If

        End With
    End Sub

    Private Shadows Sub KeyDown(sender As Object, e As KeyEventArgs) Handles TXT_SPEEDDIAL_HOTKEY.KeyDown, TXT_REDIAL_HOTKEY.KeyDown
        Dim T As TextBox = DirectCast(sender, TextBox)
        If e.KeyCode >= Keys.F9 AndAlso e.KeyCode <= Keys.F12 Then
            T.Text = e.KeyCode.ToString.ToUpper
            T.Tag = e.KeyCode
        ElseIf e.KeyCode >= Keys.F1 AndAlso e.KeyCode <= Keys.F8 Then
            MessageBox.Show("Selezionare tra F9 e F12", "Hotkey", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            If (e.KeyCode >= Keys.A AndAlso e.KeyCode <= Keys.Z) Then
                T.Text = Chr(e.KeyCode)
                T.Tag = e.KeyCode
            ElseIf (e.KeyCode >= Keys.NumPad0 AndAlso e.KeyCode <= Keys.NumPad9) Then
                T.Text = e.KeyCode.ToString.ToLower.Replace("numpad", "")
                T.Tag = e.KeyCode
            ElseIf (e.KeyCode = Keys.Delete OrElse e.KeyCode = Keys.Back) Then
                T.Text = ""
                T.Tag = 0
            End If
        End If

        e.SuppressKeyPress = True
    End Sub

    'FOR DIALER

    'FOR KEYBOARDHOOK
    'H1 is MOD_ALT, H2 is MOD_CONTROL, H4 is MOD_SHIFT, H8 is MOD_WIN
    'Public Const MOD_ALT As Integer = &H1 + &H2 + &H4 + &H8

    Public Const MOD_ALT As Integer = &H0
    Public Const WM_KEYDOWN As Integer = &H100
    Public Const WM_HOTKEY As Integer = &H312
    <DllImport("User32.dll")>
    Public Shared Function RegisterHotKey(ByVal hwnd As IntPtr,
                        ByVal id As Integer, ByVal fsModifiers As Integer,
                        ByVal vk As Integer) As Integer
    End Function
    <DllImport("User32.dll")>
    Public Shared Function UnregisterHotKey(ByVal hwnd As IntPtr,
                        ByVal id As Integer) As Integer
    End Function

    'FOR KEYBOARDHOOK

    'FOR COPYTEXT
    <DllImport("user32.dll")>
    Private Shared Sub keybd_event(bVk As Byte, bScan As Byte, dwFlags As UInteger, dwExtraInfo As UInteger)
    End Sub
    'FOR COPYTEXT
    Public Function DoHandleInCall(MyAction As String) As Boolean

        Dim Url As String = ""
        Url = "https://" & Config.SERVER & "/webrest/astproxy/" & MyAction.ToLower.ToString()

        Dim proc As New ProcessStartInfo
        With proc
            .UseShellExecute = True
            .WorkingDirectory = Environment.CurrentDirectory
            .FileName = "NethDialer.exe"
            .Arguments = "-username=" & UserLogin.Username & " -token=" & UserLogin.Token & "-phone=0 -url=" & Url
            '.Verb = "runas"
        End With

        Try
            Process.Start(proc)
        Catch ex As Exception
            ' The user refused the elevation. 
            ' Do nothing and return directly ... 
            'MessageBox.Show(ex.Message)
        Finally
            'CALL_TIMER.Enabled = True
        End Try

        Exit Function

        'Gestione Expired HTTPS certificate
        System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf CertificateHandler
        Try
            Dim s As WebRequest  'HttpWebRequest
            Dim enc As UTF8Encoding
            Dim postdata As String
            Dim postdatabytes As Byte()

            'Dim Url As String = "https://" & TXT_SERVER.Text & "/webrest/authentication/login" 'TXT_AUTHEN.Text
            'LBL_CHIAMATA.Text = "Connessione al server in corso..."
            Application.DoEvents()

            ExceptionManager.Write("Handling InCall " & MyAction.ToLower.ToString())

            Url = "https://"
            With NethDebug
                If .IsActive Then
                    Url += .DEBUG_HTTP_SERVER_ADDRESS
                    If .DEBUG_HTTP_SERVER_PORT <> "" Then
                        Url += ":" & .DEBUG_HTTP_SERVER_PORT
                    End If
                Else
                    Url += Config.SERVER
                End If
            End With
            Url += "/webrest/astproxy/" & MyAction.ToLower.ToString()

            ExceptionManager.Write("Creating request...")

            s = WebRequest.Create(Url)

            ExceptionManager.Write("Request created...[" & Url & "]")

            enc = New System.Text.UTF8Encoding()
            postdata = ""
            postdatabytes = enc.GetBytes(postdata)

            ExceptionManager.Write("Generating data...[" & postdata & "]")

            s.Method = "POST"
            's.Timeout = 30
            s.ContentType = "application/x-www-form-urlencoded"
            s.ContentLength = postdatabytes.Length

            ExceptionManager.Write("Request authorization...")
            s.Headers.Add("Authorization", UserLogin.Username & ":" & UserLogin.Token.ToLower)

            ExceptionManager.Write("Posting data...")
            Using Stream = s.GetRequestStream()
                Stream.Write(postdatabytes, 0, postdatabytes.Length)
            End Using
            ExceptionManager.Write("Data posted...")

            'LBL_CHIAMATA.Text = "Handling InCall " & MyAction.ToLower.ToString()
            Application.DoEvents()

            ''Token
            ExceptionManager.Write("Recieving response...")
            Dim Respo As String = Trim("" & s.GetResponse.Headers("www-authenticate"))
            ExceptionManager.Write("Response recieved...[" & Respo & "]")
            ExceptionManager.Write("Telephone is ringing, please pick it up...")
        Catch ex As Exception
            Opacity = 0
            Application.DoEvents()
            ex = New Exception("Impossibile gestire la chiamata." & vbCrLf & "REASON:" & ex.Message & vbCrLf & "Action:" & MyAction.ToLower.ToString())
            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            DoShowError(ex)
            Dim DebugPath As String = IO.Path.Combine(Application.StartupPath, "debug.log")
            If IO.File.Exists(DebugPath) Then
                My.Computer.FileSystem.WriteAllText(DebugPath, Format(Date.Now(), "yyyy/MM/dd HH:mm:ss") & "- DoCallEx: " & ex.Message, True)
            End If

        End Try
        'CALL_TIMER.Enabled = True

    End Function
    ' SPEEDDIAL
    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_HOTKEY Then
            Dim id As IntPtr = m.WParam
            Select Case (id.ToString)
                Case "100", "200"
                    Try
                        SendCtrlC()
                        Application.DoEvents()
                        System.Threading.Thread.Sleep(100)

                        Dim Msg As String = Clipboard.GetText
                        'Msg = Replace(Replace(Replace(Msg, "-", ""), "/", ""), " ", "")
                        Msg = Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Msg, "-", ""), "/", ""), " ", ""), "(", ""), ")", ""), "[", ""), "]", ""), "{", ""), "}", "")

                        If IsNumeric(Replace(Msg, "+", "")) Then

                            If id.ToInt32 = 100 Then
                                'Speed dial
                                OpenDialer(Msg)
                            Else
                                'Redial
                                OpenDialer(Msg)
                            End If
                        End If
                        Application.DoEvents()
                    Catch ex As Exception
                    Finally
                    End Try

                Case "101"
                    DoHandleInCall("answer")

            End Select
        End If
        MyBase.WndProc(m)
    End Sub

    Private Sub SendCtrlC()
        Dim KEYEVENTF_KEYUP As UInteger = 2
        Dim VK_CONTROL As Byte = &H11
        'SetForegroundWindow(hWnd)

        keybd_event(VK_CONTROL, 0, 0, 0)
        keybd_event(&H43, 0, 0, 0)
        'Send the C key (43 is "C")
        keybd_event(&H43, 0, KEYEVENTF_KEYUP, 0)
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0)
        ' 'Left Control Up
    End Sub


    Private WaveReader As NAudio.Wave.WaveFileReader = Nothing
    Private MP3Reader As Mp3FileReader
    Private Output As NAudio.Wave.DirectSoundOut = Nothing
    Private WithEvents Wave As NAudio.Wave.WaveOut
    Private WaveFile As String = "" '"C:\Projects\Source\Workspaces\Neth-Dev\Nethifier\bin\Debug\alarm.wav"

    Private Sub BUT_TRY_Click(sender As Object, e As EventArgs) Handles BUT_TRY.Click
        If BUT_TRY.Text = "Prova" Then
            BUT_TRY.Text = "Stop"
            StartRinging(False, True)
        Else
            BUT_TRY.Text = "Prova"
            StopRinging()
        End If
    End Sub

    Private Sub StartRinging(Optional IsLoop As Boolean = False, Optional Trial As Boolean = False)

        If CMB_DEVICE.SelectedIndex < 0 Then
            Return
        End If

        If Not IsLoop AndAlso Not Trial Then
            LED("incoming")
        End If

        If Not IO.File.Exists(TXT_SOUND.Text) Or Not CHK_SUONERIA.Checked Then
            BUT_TRY.Text = "Prova"
            Return
        End If

        TXT_SOUND.Enabled = False

        Try
            StopRinging()

            'MP3
            Dim FormatSound As String = TXT_SOUND.Text.Trim.ToLower

            Wave = New NAudio.Wave.WaveOut
            Wave.DeviceNumber = (CMB_DEVICE.SelectedIndex)

            If FormatSound.EndsWith(".mp3") Then
                MP3Reader = New Mp3FileReader(FormatSound)
                Wave.Init(MP3Reader)
            Else
                WaveReader = New NAudio.Wave.WaveFileReader(FormatSound)
                Wave.Init(WaveReader)
            End If

            Wave.Play()
        Catch ex As Exception

            '2018-03-08
            LoadSoundDevices()

            If CMB_DEVICE.SelectedIndex > -1 Then
                RegEdit(True)

                StartRinging(IsLoop, Trial)
            End If
            '2018-03-08

        End Try
    End Sub

    Private Sub StopRinging()
        TXT_SOUND.Enabled = True
        If Not IsNothing(Wave) Then
            If (Wave.PlaybackState = NAudio.Wave.PlaybackState.Playing) Then
                Wave.Stop()
                Wave.Dispose()
                Wave = Nothing
            End If
        End If
    End Sub

    Private Sub Wave_PlaybackStopped(sender As Object, e As StoppedEventArgs) Handles Wave.PlaybackStopped

        If Config.SOUND_LOOP Then
            StartRinging(True)
        End If

    End Sub

    Private Sub LED(command As String)
        Dim Path As String = Application.StartupPath
        If My.Application.CommandLineArgs.Contains("-e") Then
            Path = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName)
        End If
        Path = IO.Path.Combine(Path, "led_" & command)

        If IO.File.Exists(Path) Then
            IO.File.Delete(Path)
        End If
        Using Stream As StreamWriter = New StreamWriter(Path, True)
            'Just create text file / read
        End Using
    End Sub

    Private Sub BUT_SFOGLIA_Click(sender As Object, e As EventArgs) Handles BUT_SFOGLIA.Click
        With FILE_DLG
            .Filter = "MP3|*.mp3|Wave|*.wav"
            .DefaultExt = "*.mp3"
            .FileName = TXT_SOUND.Text
            .CheckFileExists = True
            If .ShowDialog() = DialogResult.OK Then
                TXT_SOUND.Text = .FileName
            End If
        End With
    End Sub

    Private Sub TMR_DEVICE_Tick(sender As Object, e As EventArgs) Handles TMR_DEVICE.Tick
        Dim waveOutDevices As Integer = NAudio.Wave.WaveOut.DeviceCount
        For waveOutDevice As Integer = 0 To waveOutDevices - 1
            Dim deviceInfo As NAudio.Wave.WaveOutCapabilities = NAudio.Wave.WaveOut.GetCapabilities(waveOutDevice)
            Dim IsFound As Boolean = False
            For I As Integer = 0 To SoundDevices.Count - 1
                If SoundDevices(I).ToString = deviceInfo.ProductGuid.ToString Then
                    IsFound = True
                End If
            Next
            If Not IsFound Then
                TMR_DEVICE.Stop()
                LoadSoundDevices()
                Exit Sub
            End If
        Next

        For I As Integer = 0 To SoundDevices.Count - 1
            Dim IsFound As Boolean = False
            For waveOutDevice As Integer = 0 To waveOutDevices - 1
                Dim deviceInfo As NAudio.Wave.WaveOutCapabilities = NAudio.Wave.WaveOut.GetCapabilities(waveOutDevice)
                If SoundDevices(I).ToString = deviceInfo.ProductGuid.ToString Then
                    IsFound = True
                End If
            Next

            If Not IsFound Then
                TMR_DEVICE.Stop()
                LoadSoundDevices()
                Exit Sub
            End If
        Next

    End Sub

    Private Sub CHK_SUONERIA_CheckedChanged(sender As Object, e As EventArgs) Handles CHK_SUONERIA.CheckedChanged
        GRP_SUONERIA.Enabled = CHK_SUONERIA.Checked

        If CHK_SUONERIA.Checked Then
            If Not IO.File.Exists(TXT_SOUND.Text) Then
                Dim RingerPath As String = IO.Path.Combine(Application.StartupPath, "ringer.mp3")
                If IO.File.Exists(RingerPath) Then
                    TXT_SOUND.Text = RingerPath
                End If
            End If
        End If
    End Sub

    'FOR DIALER
End Class
Public Class WbClnt
    Inherits System.Net.WebClient
    Protected Overrides Function GetWebRequest(ByVal uri As Uri) As WebRequest
        Dim w As WebRequest = MyBase.GetWebRequest(uri)
        'w.Timeout = 500000
        Threading.Thread.Sleep(5000)
        Return w
    End Function
End Class