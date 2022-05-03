Imports NAudio.Wave

Public Class FRM_NOTIFICATIONS
    Private WithEvents DOCUMENT As HtmlDocument

    Friend Event BalloonClosed(Sender As FRM_NOTIFICATIONS)
    Friend BalloonID As Long

    Private TimerCollection As Hashtable = New Hashtable

    Private _CLIENT As TCP_CLIENT

    Friend Property TCPClient As TCP_CLIENT
        Get
            Return _CLIENT
        End Get
        Set(value As TCP_CLIENT)
            _CLIENT = value
        End Set
    End Property

    Private _UserLogin As Login
    Friend Property UserLogin As Login
        Get
            Return _UserLogin
        End Get
        Set(value As Login)
            _UserLogin = value
        End Set
    End Property
    Private _CONFIG As Config
    Friend Property CONFIG As Config
        Get
            Return _CONFIG
        End Get
        Set(value As Config)
            _CONFIG = value
        End Set
    End Property
    Private Sub WEB_BROWSER_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs)
        'Dim TimeOut As String = DOCUMENT.GetElementById("wait").

        With DirectCast(sender, WebBrowser)
            DOCUMENT = .Document
            DOCUMENT.Window.Name = .Name

            Dim Head As HtmlElement
            Dim Script As HtmlElement

            Head = DOCUMENT.GetElementsByTagName("head")(0)
            Script = DOCUMENT.CreateElement("script")
            Script.SetAttribute("text", "$userInfo=" & UserLogin.ToString) 'function _invoker(){eval(arguments[0])}
            Head.AppendChild(Script)

            Dim META As HtmlElement = DOCUMENT.CreateElement("meta")
            META.OuterHtml = "<meta http-equiv=""cache-control"" content=""no-cache"">"
            Head.AppendChild(META)

            META = DOCUMENT.CreateElement("meta")
            META.OuterHtml = "<meta http-equiv=""pragma"" content=""no-cache"">"
            Head.AppendChild(META)
        End With

        AddHandler DOCUMENT.Click, AddressOf DOCUMENT_Click
        AddHandler DOCUMENT.MouseLeave, AddressOf DOCUMENT_MouseLeave
        AddHandler DOCUMENT.MouseOver, AddressOf DOCUMENT_MouseOver

        Dim Expiration As Integer = 0
        If IsNumeric(DirectCast(sender, WebBrowser).Tag) Then
            Expiration = CInt(DirectCast(sender, WebBrowser).Tag)
            Expiration = (Expiration * 1000)
        End If

        If Expiration > 0 Then
            Dim TMR As New Timer
            AddHandler TMR.Tick, AddressOf TMR_CLOSE_Tick
            TMR.Interval = Expiration
            TMR.Tag = sender

            TimerCollection.Add(DOCUMENT.Window.Name, TMR)
            TMR.Start()
        Else
            TimerCollection.Add(DOCUMENT.Window.Name, sender)
        End If

        Dim H As Integer
        For Each Notify As Control In WEB_CONTAINER.Controls
            H += Notify.Height
        Next

        If Screen.PrimaryScreen.WorkingArea.Height < (H + 50) Then
            'Cosa bisogna fare???
        End If
        Me.Location = New Point(Screen.PrimaryScreen.WorkingArea.Width - Width, Screen.PrimaryScreen.WorkingArea.Height - ((H + 50) + 0)) '+ (2 * WEB_CONTAINER.Controls.Count)
        If _CONFIG.POPUP_POS = 1 Then Location = New Point(0, 0)
        If _CONFIG.POPUP_POS = 2 Then Location = New Point(0, Screen.PrimaryScreen.WorkingArea.Height - ((H + 50) + 0))
        If _CONFIG.POPUP_POS = 4 Then Location = New Point(Screen.PrimaryScreen.WorkingArea.Width - Width, 0)
        Me.Height = (H + 50)

        If Not Me.Visible Then
            Me.Show()
        End If

    End Sub

    Private Sub WEB_BROWSER_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
        If e.KeyCode = Keys.F5 Then
            e.IsInputKey = True
        End If
    End Sub

    Private Sub WEB_BROWSER_NewWindow(sender As Object, e As System.ComponentModel.CancelEventArgs)
        'Me.Close()
    End Sub

    Private Sub BUT_CHIUDI_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub DOCUMENT_Click(sender As Object, e As HtmlElementEventArgs)
        DOCUMENT = DirectCast(sender, HtmlDocument)

        Dim Element As HtmlElement = DOCUMENT.ActiveElement
        Dim ARG As String = Element.GetAttribute("arg")
        Dim CMD As String = Element.GetAttribute("cmd")
        Dim CLOSE As String = Element.GetAttribute("close")
        Dim RESPONSE As String = Element.GetAttribute("response")

        'URL = ""

        If Me.Commands.ContainsKey(CMD.ToLower.Trim) Then
            Dim Com As Command = DirectCast(Me.Commands(CMD.Trim.ToLower), Command)
            If IO.File.Exists(Com.RunWith) Then
                Process.Start("""" & Com.RunWith & """", ARG)
            End If
        End If

        'Dim ChromePath As String = CStr(Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "Path", "NOT_FOUND"))
        'If Trim(URL) <> "" Then
        '    If Trim(CMD) <> "" Then
        '        URL += "?" & CMD
        '    End If
        '    If IO.Directory.Exists(ChromePath) Then
        '        Process.Start(ChromePath & "\chrome.exe", URL)
        '    Else
        '        'Use default browser
        '        Process.Start(URL)
        '    End If
        'End If

        If RESPONSE <> "" Then

            SendData("{""response"":{""id"":""" & DOCUMENT.Window.Name & """,""response"":""" & RESPONSE & """}}")

        End If

        If CLOSE = "1" Then

            For Each Notify As Control In WEB_CONTAINER.Controls
                If TypeOf Notify Is WebBrowser Then
                    If Object.Equals(DirectCast(Notify, WebBrowser).Document, DOCUMENT) Then
                        WEB_CONTAINER.Controls.Remove(Notify)
                    End If
                End If
            Next
            'Me.Close()
        End If
    End Sub

    'Private Sub FRM_BALLOON_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
    '    RaiseEvent BalloonClosed(Me)
    'End Sub

    Private Sub TMR_CLOSE_Tick(sender As Object, e As EventArgs)
        DirectCast(sender, Timer).Stop()
        WEB_CONTAINER.Controls.Remove(DirectCast(DirectCast(sender, Timer).Tag, WebBrowser))
    End Sub

    Private Sub FRM_BALLOON_Load(sender As Object, e As EventArgs) Handles Me.Load
        'WEB_BROWSER.        
    End Sub

    'Private _Notifications As Collection = New Collection
    'ReadOnly Property Notifications As Collection
    '    Get
    '        Return _Notifications
    '    End Get
    'End Property

    Private _Commands As Hashtable = New Hashtable
    Property Commands As Hashtable
        Get
            Return _Commands
        End Get
        Set(value As Hashtable)
            _Commands = value
        End Set
    End Property

    Public Sub ClearNotifications()
        Do While WEB_CONTAINER.Controls.Count > 0
            WEB_CONTAINER.Controls.Remove(WEB_CONTAINER.Controls(0))
        Loop
    End Sub

    Public Function AddNotification(ID As String, URL As String, Width As Integer, Height As Integer, Expiration As Integer) As Boolean

        If TimerCollection.ContainsKey(ID) Then
            Return False
        End If

        Dim Notification As New WebBrowser
        Me.Width = Width

        With Notification
            AddHandler .PreviewKeyDown, AddressOf WEB_BROWSER_PreviewKeyDown
            AddHandler .DocumentCompleted, AddressOf WEB_BROWSER_DocumentCompleted
            .Width = WEB_CONTAINER.Width
            .Height = Height
            .Navigate(URL, "_self", Nothing, "www-authenticate: " & UserLogin.Username & ":" & UserLogin.Token & Chr(10) & Chr(13))

            '.Url = New Uri(URL)
            .AllowWebBrowserDrop = False
            '.AllowNavigation = True

            .WebBrowserShortcutsEnabled = False 'this line will disable F5

            .IsWebBrowserContextMenuEnabled = False
            .Dock = System.Windows.Forms.DockStyle.Top
            .ScrollBarsEnabled = False
            .ScriptErrorsSuppressed = True
            .Tag = Expiration
            .Name = ID
        End With

        If WEB_CONTAINER.Controls.Count > 0 Then
            Dim Pan As Panel = New Panel
            Pan.Height = 3
            Pan.Dock = DockStyle.Top
            WEB_CONTAINER.Controls.Add(Pan)
        End If

        WEB_CONTAINER.Controls.Add(Notification)
        Return True
    End Function

    Public Sub CloseNotification(ID As String)
        For Each Notify As Control In WEB_CONTAINER.Controls
            If TypeOf Notify Is WebBrowser Then
                If DirectCast(Notify, WebBrowser).Document.Window.Name.Trim.ToLower = ID.Trim.ToLower Then
                    WEB_CONTAINER.Controls.Remove(Notify)
                    Exit Sub
                End If
            End If
        Next
    End Sub

    Public Function GetNotificationCount() As Integer
        Dim NotiCount As Integer = 0
        For Each Notify As Control In WEB_CONTAINER.Controls
            If TypeOf Notify Is WebBrowser Then
                NotiCount += 1
            End If
        Next
        Return NotiCount
    End Function

    Private Sub LNK_CLEAR_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LNK_CLEAR.LinkClicked
        ClearNotifications()
    End Sub

    'Private Sub WEB_CONTAINER_ControlAdded(sender As Object, e As ControlEventArgs) Handles WEB_CONTAINER.ControlAdded
    '    _Notifications.Add(e.Control)
    'End Sub

    Private Sub WEB_CONTAINER_ControlRemoved(sender As Object, e As ControlEventArgs) Handles WEB_CONTAINER.ControlRemoved

        For Each K As String In TimerCollection.Keys

            If TypeOf TimerCollection(K) Is Timer Then
                Dim TMR As Timer = DirectCast(TimerCollection(K), Timer)
                If Object.Equals(TMR.Tag, e.Control) Then
                    TimerCollection.Remove(K)
                    Exit For
                End If
            ElseIf TypeOf TimerCollection(K) Is WebBrowser Then
                If Object.Equals(TimerCollection(K), e.Control) Then
                    TimerCollection.Remove(K)
                    Exit For
                End If
            End If

        Next

        Me.Height -= e.Control.Height
        Dim H As Integer = 50
        For Each Notify As Control In WEB_CONTAINER.Controls
            H += Notify.Height
        Next
        Me.Location = New Point(Screen.PrimaryScreen.WorkingArea.Width - Width, Screen.PrimaryScreen.WorkingArea.Height - (H + 0)) '(2 * WEB_CONTAINER.Controls.Count)

        If TypeOf e.Control Is WebBrowser Then
            SendData("{""id"":""" & DirectCast(e.Control, WebBrowser).Document.Window.Name & """,""action"":""closed""}")
        End If

        ''2017-12-04
        'StopRinging()

        If WEB_CONTAINER.Controls.Count = 0 Then
            Me.Hide()
        Else
            If TypeOf WEB_CONTAINER.Controls.Item(0) Is Panel Then
                WEB_CONTAINER.Controls.RemoveAt(0)
            Else
                Dim IsPanel As Boolean = False
                For Each Ctl As Control In WEB_CONTAINER.Controls
                    If TypeOf Ctl Is Panel Then
                        If IsPanel Then
                            WEB_CONTAINER.Controls.Remove(Ctl)
                            Exit For
                        End If
                        IsPanel = True
                    Else
                        IsPanel = False
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub SendData(Data As String)
        If Not IsNothing(TCPClient) Then
            If TCPClient.isClientRunning Then
                TCPClient.SendBytes(Data)
            End If
        End If
    End Sub

    Private Sub DOCUMENT_MouseLeave(sender As Object, e As HtmlElementEventArgs)
        If TimerCollection.Count <= 0 Then
            Exit Sub
        End If

        DOCUMENT = DirectCast(sender, HtmlDocument)

        Dim TMR As Object = TimerCollection(DOCUMENT.Window.Name)

        If Not IsNothing(TMR) Then
            If TypeOf TMR Is Timer Then
                DirectCast(TMR, Timer).Start()
            End If
        End If

    End Sub

    Private Sub DOCUMENT_MouseOver(sender As Object, e As HtmlElementEventArgs)
        If TimerCollection.Count <= 0 Then
            Exit Sub
        End If

        DOCUMENT = DirectCast(sender, HtmlDocument)

        Dim TMR As Object = TimerCollection(DOCUMENT.Window.Name)

        If Not IsNothing(TMR) Then
            If TypeOf TMR Is Timer Then
                DirectCast(TMR, Timer).Stop()
            End If
        End If

    End Sub


    '2017-12-04
    Private WaveReader As WaveFileReader = Nothing
    Private Output As DirectSoundOut = Nothing
    Private WithEvents Wave As WaveOut
    Private WaveFile As String = "" '"C:\Projects\Source\Workspaces\Neth-Dev\Nethifier\bin\Debug\alarm.wav"

    'Public Sub New()

    '    ' This call is required by the designer.
    '    InitializeComponent()

    '    ' Add any initialization after the InitializeComponent() call.

    'End Sub

    Friend Sub New(Config As Config)

        _CONFIG = Config

        WaveFile = _CONFIG.SOUND_FILE
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub StartRinging()

        If Not IO.File.Exists(WaveFile) Then
            Return
        End If

        StopRinging()

        Wave = New WaveOut
        Wave.DeviceNumber = 0
        WaveReader = New WaveFileReader(WaveFile)
        Wave.Init(WaveReader)
        Wave.Play()
    End Sub

    Private Sub StopRinging()
        If Not IsNothing(Wave) Then
            If (Wave.PlaybackState = NAudio.Wave.PlaybackState.Playing) Then
                Wave.Stop()
                Wave.Dispose()
                Wave = Nothing
            End If
        End If
    End Sub

    Private Sub Wave_PlaybackStopped(sender As Object, e As StoppedEventArgs) Handles Wave.PlaybackStopped
        StartRinging()
    End Sub

    Private Sub WEB_CONTAINER_ControlAdded(sender As Object, e As ControlEventArgs) Handles WEB_CONTAINER.ControlAdded
        'StartRinging()
    End Sub
End Class