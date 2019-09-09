Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security.Principal
Imports System.ComponentModel
Imports System.Text

Friend Class NethDebugger

    Private _USE_NOTIFICATION_TIMEOUT As Boolean = False
    Private _SHOW_ERROR_LOG_TAB As Boolean = False
    Private _SHOW_SERVER_MESSAGES_TAB As Boolean = False
    Private _USE_DEBUG_TCP_SERVER As Boolean = False
    Private _USE_HTTP_AUTH As Boolean = False
    Private _DEBUG_TCP_SERVER_IP As String = ""
    Private _DEBUG_TCP_SERVER_PORT As String = ""
    Private _DEBUG_HTTP_SERVER_ADDRESS As String = ""
    Private _DEBUG_HTTP_SERVER_PORT As String = ""

    Private Path As String

    Public ReadOnly Property USE_NOTIFICATION_TIMEOUT As Boolean
        Get
            Return _USE_NOTIFICATION_TIMEOUT
        End Get
    End Property
    Public ReadOnly Property SHOW_ERROR_LOG_TAB As Boolean
        Get
            Return _SHOW_ERROR_LOG_TAB
        End Get
    End Property
    Public ReadOnly Property SHOW_SERVER_MESSAGES_TAB As Boolean
        Get
            Return _SHOW_SERVER_MESSAGES_TAB
        End Get
    End Property
    Public ReadOnly Property USE_DEBUG_TCP_SERVER As Boolean
        Get
            Return _USE_DEBUG_TCP_SERVER
        End Get
    End Property
    Public ReadOnly Property USE_HTTP_AUTH As Boolean
        Get
            Return _USE_HTTP_AUTH
        End Get
    End Property
    Public ReadOnly Property DEBUG_TCP_SERVER_IP As String
        Get
            Return _DEBUG_TCP_SERVER_IP
        End Get
    End Property
    Public ReadOnly Property DEBUG_TCP_SERVER_PORT As String
        Get
            Return _DEBUG_TCP_SERVER_PORT
        End Get
    End Property

    Public ReadOnly Property DEBUG_HTTP_SERVER_ADDRESS As String
        Get
            Return _DEBUG_HTTP_SERVER_ADDRESS
        End Get
    End Property

    Public ReadOnly Property DEBUG_HTTP_SERVER_PORT As String
        Get
            Return _DEBUG_HTTP_SERVER_PORT
        End Get
    End Property

    Public ReadOnly Property IsActive As Boolean
        Get
            Return IO.File.Exists(Path)
        End Get
    End Property

    Sub New()
        Path = Application.StartupPath & "\debug"
        SetProperties()
    End Sub

    Public Function Save() As Boolean
        Dim Stream As StreamWriter = Nothing
        If Not File.Exists(Path) Then
            Stream = File.CreateText(Path)
            Stream.Close()
        End If

        Dim Config As String = "" & _
        "USE_NOTIFICATION_TIMEOUT=" & CStr(IIf(Me._USE_NOTIFICATION_TIMEOUT, "1", "0")) & vbCrLf & _
        "SHOW_ERROR_LOG_TAB=" & CStr(IIf(Me._SHOW_ERROR_LOG_TAB, "1", "0")) & vbCrLf & _
        "SHOW_SERVER_MESSAGES_TAB=" & CStr(IIf(Me._SHOW_SERVER_MESSAGES_TAB, "1", "0")) & vbCrLf & _
        "USE_DEBUG_TCP_SERVER=" & CStr(IIf(Me._USE_DEBUG_TCP_SERVER, "1", "0")) & vbCrLf & _
        "USE_HTTP_AUTH=" & CStr(IIf(Me._USE_HTTP_AUTH, "1", "0")) & vbCrLf & _
        "DEBUG_TCP_SERVER_IP=" & Me._DEBUG_TCP_SERVER_IP & vbCrLf & _
        "DEBUG_TCP_SERVER_PORT=" & Me._DEBUG_TCP_SERVER_PORT & vbCrLf & _
        "DEBUG_HTTP_SERVER_ADDRESS=" & Me._DEBUG_HTTP_SERVER_ADDRESS & vbCrLf & _
        "DEBUG_HTTP_SERVER_PORT=" & Me._DEBUG_HTTP_SERVER_PORT & vbCrLf

        Dim Encrypt As CLS_SECURITY.Aes256Base64Encrypter = New CLS_SECURITY.Aes256Base64Encrypter
        Config = Encrypt.Encrypt(Config, Application.ProductName)

        File.WriteAllText(Path, Config)
        Return True
    End Function

    Private Sub SetProperties()
        If File.Exists(Path) Then
            Dim Decrypt As CLS_SECURITY.Aes256Base64Encrypter = New CLS_SECURITY.Aes256Base64Encrypter
            Dim Config As String = File.ReadAllText(Path)
            'Config = Decrypt.Decrypt(Config, Application.ProductName)
            Dim Par As String() = Split(Config, vbCrLf) '

            Dim Key As String = ""
            Dim Val As String = ""
            
            For I As Integer = 0 To Par.Length - 1
                Key = Trim(Par(I)) '.ToUpper
                If Key <> "" Then
                    Val = Trim(Key.Substring(Key.IndexOf("=", StringComparison.Ordinal) + 1))
                    If Key.StartsWith("USE_NOTIFICATION_TIMEOUT=") Then
                        Me._USE_NOTIFICATION_TIMEOUT = (Val = "1")
                    ElseIf Key.StartsWith("SHOW_ERROR_LOG_TAB=") Then
                        Me._SHOW_ERROR_LOG_TAB = (Val = "1")
                    ElseIf Key.StartsWith("SHOW_SERVER_MESSAGES_TAB=") Then
                        Me._SHOW_SERVER_MESSAGES_TAB = (Val = "1")
                    ElseIf Key.StartsWith("USE_DEBUG_TCP_SERVER=") Then
                        Me._USE_DEBUG_TCP_SERVER = (Val = "1")
                    ElseIf Key.StartsWith("USE_HTTP_AUTH=") Then
                        Me._USE_HTTP_AUTH = (Val = "1")
                    ElseIf Key.StartsWith("DEBUG_TCP_SERVER_IP=") Then
                        Me._DEBUG_TCP_SERVER_IP = Val
                    ElseIf Key.StartsWith("DEBUG_TCP_SERVER_PORT=") Then
                        Me._DEBUG_TCP_SERVER_PORT = Val
                    ElseIf Key.StartsWith("DEBUG_HTTP_SERVER_ADDRESS=") Then
                        Me._DEBUG_HTTP_SERVER_ADDRESS = Val
                    ElseIf Key.StartsWith("DEBUG_HTTP_SERVER_PORT=") Then
                        Me._DEBUG_HTTP_SERVER_PORT = Val
                    End If
                End If
            Next
        End If

    End Sub
End Class
