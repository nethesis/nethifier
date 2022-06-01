Imports System.IO
Imports System.Web.Configuration
Imports System.Net.Mime
Imports Newtonsoft.Json
Imports Microsoft.Win32.SafeHandles
Imports System.Runtime.InteropServices
Imports System.Security.Principal
Imports System.ComponentModel
Imports Nethifier.Enums
Imports System.Text
Imports System.Net.Mail
Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security
Imports System.Security.Cryptography
Imports System.Security.Permissions
Imports Microsoft.Win32

Friend Class Config

    Private _IsSaved As Boolean
    Public ReadOnly Property IsSaved As Boolean
        Get
            Return _IsSaved
        End Get
    End Property

    Sub New(Path As String)
        'If (Path = "") Then
        Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName
        'End If
        _Path = Path & "\config.ini"
        SetProperties()
    End Sub

    Sub New()
        Dim P As String = ""
        Try
            If Not Debugger.IsAttached Then
                P = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName

                If Not IO.File.Exists(P & "\config.ini") Then
                    'Probably NOT yet Installed
                    'P = Application.StartupPath
                    P = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName
                End If
            Else
                'P = Application.StartupPath
                P = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName
            End If

            If Not IO.Directory.Exists(P) Then
                IO.Directory.CreateDirectory(P)
            End If
            _Path = P & "\config.ini"
        Catch ex As Exception
            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try

        SetProperties()
    End Sub

    Public USERNAME As String
    Public PASSWORD As String
    Public SERVER As String
    Public SERVER_PORT As Int32 = 8182
    Public AUTH_MODE As String
    Public AUTH_ADDRESS As String
    Public NOTIFY_TIMEOUT As Int32
    Public AUTO_LOGIN As Boolean = True
    Public PARAM_CHECK As Boolean = False
    Public PARAM_MODE As String
    Public POPUP_POS As Int32 = 0
    Public POPUP_INOLTRO As Boolean = False
    Public POPUP_NEW_CALL As Boolean = False
    Public POPUP_INTERNI As Boolean = False

    Public Commands As Hashtable = New Hashtable
    Public LANGUAGE As String = "default"

    Public HOTKEY_MOD_SPEED_DIAL As Int32 = 0
    Public HOTKEY_MOD_REDIAL As Int32 = 0
    Public MOD_SPEED_DIAL As Int32 = 0
    Public MOD_REDIAL As Int32 = 0

    Public SOUND_FILE As String
    Public SOUND_DEVICE As String 'Int32 = -1
    Public SOUND_LOOP As Boolean = True
    Public USE_RINGER As Boolean = True

    Private _Path As String = ""
    Public ReadOnly Property Path As String
        Get
            Return _Path
        End Get
    End Property

    Public Function Save() As Boolean
        Dim Stream As StreamWriter = Nothing
        Dim myPath As String = (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() & "\" & Application.ProductName & "\config.ini")
        If Not File.Exists(myPath) Then
            Stream = File.CreateText(myPath)
            Stream.Close()

            Me.HOTKEY_MOD_SPEED_DIAL = &H2 'CTRL
            Me.MOD_SPEED_DIAL = System.Windows.Forms.Keys.F11
        End If

        Dim Config As String = "" &
        "USERNAME=" & Me.USERNAME & vbCrLf &
        "PASSWORD=" & Me.PASSWORD & vbCrLf &
        "SERVER=" & Me.SERVER & vbCrLf &
        "SERVER_PORT=" & Me.SERVER_PORT & vbCrLf &
        "AUTH_MODE=" & Me.AUTH_MODE & vbCrLf &
        "AUTH_ADDRESS=" & Me.AUTH_ADDRESS & vbCrLf &
        "NOTIFY_TIMEOUT=" & Me.NOTIFY_TIMEOUT & vbCrLf &
        "AUTO_LOGIN=" & CStr(IIf(Me.AUTO_LOGIN, "1", "0")) & vbCrLf &
        "PARAM_CHECK=" & CStr(IIf(Me.PARAM_CHECK, "1", "0")) & vbCrLf &
        "PARAM_MODE=" & Me.PARAM_MODE & vbCrLf &
        "POPUP_POS=" & Me.POPUP_POS & vbCrLf &
        "POPUP_INOLTRO=" & CStr(IIf(Me.POPUP_INOLTRO, "1", "0")) & vbCrLf &
        "POPUP_INTERNI=" & CStr(IIf(Me.POPUP_INTERNI, "1", "0")) & vbCrLf &
        "LANGUAGE=" & Me.LANGUAGE & vbCrLf &
        "HOTKEY_MOD_SPEED_DIAL=" & Me.HOTKEY_MOD_SPEED_DIAL & vbCrLf &
        "HOTKEY_MOD_REDIAL=" & Me.HOTKEY_MOD_REDIAL & vbCrLf &
        "MOD_SPEED_DIAL=" & Me.MOD_SPEED_DIAL & vbCrLf &
        "MOD_REDIAL=" & Me.MOD_REDIAL & vbCrLf &
        "SOUND_FILE=" & Me.SOUND_FILE & vbCrLf &
        "SOUND_DEVICE=" & Me.SOUND_DEVICE & vbCrLf &
        "SOUND_LOOP=" & CStr(IIf(Me.SOUND_LOOP, "1", "0")) & vbCrLf &
        "USE_RINGER=" & CStr(IIf(Me.USE_RINGER, "1", "0")) & vbCrLf

        For Each Com As Command In Commands.Values
            If Trim(Com.Command) <> "" Then
                Config += "COMM_" & Com.Command & "|" & Com.RunWith & vbCrLf
            End If
        Next

        Dim Encrypt As CLS_SECURITY.Aes256Base64Encrypter = New CLS_SECURITY.Aes256Base64Encrypter

        Config = Encrypt.Encrypt(Config, Application.ProductName, True)

        File.WriteAllText(Path, Config)
        Return True
    End Function

    Private _Languages As Hashtable = New Hashtable
    ReadOnly Property Languages As Hashtable
        Get
            Return _Languages
        End Get
    End Property

    Private Sub SetProperties()
        If File.Exists(Path) Then
            Dim Decrypt As CLS_SECURITY.Aes256Base64Encrypter = New CLS_SECURITY.Aes256Base64Encrypter
            Dim Config As String = Decrypt.Decrypt(File.ReadAllText(Path), Application.ProductName, True)

            If Trim(Config) = "" Then
                Config = Decrypt.Decrypt(File.ReadAllText(Path), Application.ProductName)
            End If

            Dim Par As String() = Split(Config, vbCrLf) 'File.ReadAllLines(Path)

            Dim Key As String = ""
            Dim Val As String = ""
            Dim X As Integer = 0

            Dim SD As Boolean
            Dim RD As Boolean
            Dim HKSD As Boolean
            Dim HKRD As Boolean
            Dim P_POS As Boolean

            For I As Integer = 0 To Par.Length - 1
                Key = Trim(Par(I)) '.ToUpper
                If Key <> "" Then
                    Val = Trim(Key.Substring(Key.IndexOf("=", StringComparison.Ordinal) + 1))

                    If Key.StartsWith("USERNAME=") Then
                        Me.USERNAME = Val
                        X += 1
                    ElseIf Key.StartsWith("PASSWORD=") Then
                        Me.PASSWORD = Val
                        X += 1
                    ElseIf Key.StartsWith("SERVER=") Then
                        Me.SERVER = Val
                        X += 1
                    ElseIf Key.StartsWith("AUTH_MODE=") Then
                        Me.AUTH_MODE = Val
                        X += 1
                    ElseIf Key.StartsWith("AUTH_ADDRESS=") Then
                        Me.AUTH_ADDRESS = Val
                        X += 1
                    ElseIf Key.StartsWith("SERVER_PORT=") Then
                        If IsNumeric(Val) Then
                            Me.SERVER_PORT = CInt(Val)
                            X += 1
                        End If
                    ElseIf Key.StartsWith("AUTO_LOGIN=") Then
                        Me.AUTO_LOGIN = (Val = "1")
                        X += 1
                    ElseIf Key.StartsWith("PARAM_CHECK=") Then
                        Me.PARAM_CHECK = (Val = "1")
                        X += 1
                    ElseIf Key.StartsWith("PARAM_MODE=") Then
                        Me.PARAM_MODE = Val
                        X += 1
                    ElseIf Key.StartsWith("POPUP_POS=") Then
                        P_POS = True
                        Me.POPUP_POS = CInt(Val)
                        X += 1
                    ElseIf Key.StartsWith("POPUP_INOLTRO=") Then
                        Me.POPUP_INOLTRO = (Val = "1")
                    ElseIf Key.StartsWith("POPUP_INTERNI=") Then
                        Me.POPUP_INTERNI = (Val = "1")
                    ElseIf Key.StartsWith("LANGUAGE=") Then
                        Me.LANGUAGE = Val
                        X += 1
                    ElseIf Key.StartsWith("NOTIFY_TIMEOUT=") Then
                        If IsNumeric(Val) Then
                            Me.NOTIFY_TIMEOUT = CInt(Val)
                            X += 1
                        End If
                    ElseIf Key.StartsWith("HOTKEY_MOD_SPEED_DIAL=") Then
                        HKSD = True
                        If IsNumeric(Val) Then
                            Me.HOTKEY_MOD_SPEED_DIAL = CInt(Val)
                        End If
                    ElseIf Key.StartsWith("HOTKEY_MOD_REDIAL=") Then
                        HKRD = True
                        If IsNumeric(Val) Then
                            Me.HOTKEY_MOD_REDIAL = CInt(Val)
                        End If
                    ElseIf Key.StartsWith("MOD_SPEED_DIAL=") Then
                        SD = True
                        If IsNumeric(Val) Then
                            Me.MOD_SPEED_DIAL = CInt(Val)
                        End If
                    ElseIf Key.StartsWith("MOD_REDIAL=") Then
                        RD = True
                        If IsNumeric(Val) Then
                            Me.MOD_REDIAL = CInt(Val)
                        End If
                    ElseIf Key.StartsWith("SOUND_DEVICE=") Then
                        'If IsNumeric(Val) Then
                        '    Me.SOUND_DEVICE = CInt(Val)
                        'End If
                        Me.SOUND_DEVICE = Val
                    ElseIf Key.StartsWith("SOUND_LOOP=") Then
                        Me.SOUND_LOOP = (Val = "1")
                    ElseIf Key.StartsWith("USE_RINGER=") Then
                        Me.USE_RINGER = (Val = "1")
                    ElseIf Key.StartsWith("SOUND_FILE=") Then
                        Me.SOUND_FILE = Val
                    ElseIf Key.StartsWith("COMM_") Then
                        Dim Com As Command = New Command
                        Dim ComString As String() = Split(Key.Substring(5), "|")

                        If ComString.Length = 2 Then
                            With Com
                                .Command = ComString(0)
                                .RunWith = ComString(1)
                            End With
                            Me.Commands.Add(Com.Command.ToLower.Trim, Com)
                        End If
                    End If
                End If
            Next

            If Not (SD AndAlso RD AndAlso HKSD AndAlso HKRD) Then
                'Valore iniziale
                Me.MOD_REDIAL = 82 'R
                Me.MOD_SPEED_DIAL = 122 'F11
                Me.HOTKEY_MOD_REDIAL = &H2 'CTRL
                Me.HOTKEY_MOD_SPEED_DIAL = &H2 'CTRL
            End If

            If Not (P_POS) Then
                Me.POPUP_POS = 8
            End If

            _IsSaved = (X = 8)
        End If

        ''10 seconds
        'If Me.NOTIFY_TIMEOUT < 10 Then
        '    Me.NOTIFY_TIMEOUT = 10
        'End If

        Try
            Dim Langs As String() = IO.Directory.GetFiles(Application.StartupPath & "\languages")
            For L As Integer = 0 To Langs.Length - 1
                If Langs(L).EndsWith(".lan") Then
                    'Dim INFO As FileInfo = New FileInfo(Langs(L))
                    'Dim _L As String = INFO.Name.Substring(0, INFO.Name.Length - 4)

                    Dim _M As String() = Langs(L).Substring(0, Langs(L).Length - 4).Split(Convert.ToChar("\"))
                    Dim _L As String = _M(UBound(_M))
                    If Not Languages.ContainsKey(_L) Then
                        Languages.Add(_L, _L)
                    End If
                End If
            Next

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
End Class

Public Class Commands
    Private _Commands As New Hashtable

    Property Commands As Hashtable
        Get
            Return _Commands
        End Get
        Set(value As Hashtable)
            _Commands = value
        End Set
    End Property
    Public Overrides Function ToString() As String
        Return JsonConvert.SerializeObject(Me).ToLower
    End Function
End Class

Public Class Command
    Sub New(Command As String, RunWith As String)
        _Command = Command
        _RunWith = RunWith
    End Sub
    Sub New()

    End Sub
    Private _Command As String
    Property Command As String
        Get
            Return _Command
        End Get
        Set(value As String)
            _Command = value
        End Set
    End Property

    Private _RunWith As String
    Property RunWith As String
        Get
            Return _RunWith
        End Get
        Set(value As String)
            _RunWith = value
        End Set
    End Property
End Class

'Public Class ExceptionManager
'    Inherits TextFileManager

'    Sub New()
'        Try
'            Dim Path As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName
'            If Not IO.Directory.Exists(Path) Then
'                IO.Directory.CreateDirectory(Path)
'            End If
'            Me.Path = Path & "\exception.ini"


'        Catch ex As Exception
'            'MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
'            Me.Path = ""
'        End Try
'    End Sub

'    Overrides Sub Write(ByVal Ex As Exception)
'        Dim Msg As String = "" &
'        "************************************************************************************************" & vbCrLf &
'        Now.ToString & vbCrLf &
'        "************************************************************************************************" & vbCrLf &
'        Ex.Message & vbCrLf
'        If Not IsNothing(Ex.StackTrace) Then
'            Msg += "Stack Trace:" & vbCrLf &
'            Ex.StackTrace & vbCrLf
'        End If
'        If Not IsNothing(Ex.Source) Then
'            Msg += "Source:" & vbCrLf &
'            Ex.Source & vbCrLf
'        End If
'        Msg += "************************************************************************************************" & vbCrLf
'        MyBase.Write(New Exception(Msg))
'    End Sub
'End Class

'Public MustInherit Class TextFileManager
'    Private _Path As String
'    'Private Stream As StreamWriter

'    Overridable Property Path As String
'        Get
'            Return _Path
'        End Get
'        Set(value As String)
'            Try
'                _Path = value
'                Using Stream As StreamWriter = New StreamWriter(Path, True)
'                    'Just create text file / read
'                End Using
'            Catch ex As Exception
'                Throw New Exception(ex.Message, ex.InnerException)
'            End Try
'        End Set
'    End Property
'    Overridable Sub Delete()
'        Using Stream As StreamWriter = New StreamWriter(Path)
'            'Just create text file / read
'        End Using
'        'File.Delete(Path)
'    End Sub
'    Overridable Sub Write(Ex As Exception)
'        Write(Ex.Message, True)
'    End Sub
'    Overridable Sub Write(Msg As String, Optional NewLine As Boolean = True)
'        Using writer As StreamWriter = New StreamWriter(Me.Path, True)
'            With writer
'                If NewLine Then
'                    .Write(vbCrLf & Msg)
'                Else
'                    .Write(Msg)
'                End If
'            End With
'        End Using
'    End Sub
'    Overridable Function Read() As String
'        Dim ST As String = ""
'        Using reader As StreamReader = New StreamReader(Me.Path)
'            With reader
'                ST = .ReadToEnd()
'                .Close()
'                .Dispose()
'            End With
'        End Using
'        Return ST
'    End Function
'    Overridable Function ReadAllLines() As String()
'        Dim Lines As String() = System.IO.File.ReadAllLines(Me.Path, Encoding.Default)
'        Return Lines
'    End Function
'End Class

Public Class Login
    Private _Action As String
    Private _Username As String
    Private _Token As String

    Property Action As String
        Get
            Return _Action
        End Get
        Set(value As String)
            _Action = value
        End Set
    End Property

    Property Username As String
        Get
            Return _Username
        End Get
        Set(value As String)
            _Username = value
        End Set
    End Property

    Property Token As String
        Get
            Return _Token
        End Get
        Set(value As String)
            _Token = value
        End Set
    End Property

    ReadOnly Property Version As String
        Get
            Dim VersionInfo As Version = New Version
            With VersionInfo
                Return .Major & "." & .Minor & "." & .Build
                'Return "3.0.1"
            End With
        End Get
    End Property

    Sub New(Action As String, Username As String, Token As String)
        _Action = Action
        _Username = Username
        _Token = Token
    End Sub

    Public Overrides Function ToString() As String
        Return JsonConvert.SerializeObject(Me).ToLower
    End Function

End Class

Friend Class Helper

#Region "Helper Functions for Admin Privileges and Elevation Status"

    Shared Function IsApplicationRunning(Path As String, ByRef RunningProcess As Process) As Boolean
        For Each p As Process In Process.GetProcesses()
            Try
                If p.Modules(0).FileName.ToLower = Path.Trim.ToLower Then
                    RunningProcess = p
                    Return True
                End If
            Catch ex As Exception
            End Try
        Next
        Return False
    End Function

    Shared Function IsApplicationRunning(Path As String, CurrentProcess As Process, ByRef RunningProcess As Process) As Boolean
        For Each p As Process In Process.GetProcesses()
            Try
                If p.Modules(0).FileName.ToLower = Path.Trim.ToLower Then
                    If p.Id <> CurrentProcess.Id Then
                        RunningProcess = p
                        Return True
                    End If
                End If
            Catch ex As Exception
            End Try
        Next
        Return False
    End Function

    Shared Function IsRunAsAdmin() As Boolean
        Dim principal As New WindowsPrincipal(WindowsIdentity.GetCurrent)
        Return principal.IsInRole(WindowsBuiltInRole.Administrator)
    End Function

    Shared Function IsProcessElevated() As Boolean
        Dim fIsElevated As Boolean = False
        Dim hToken As SafeTokenHandle = Nothing
        Dim cbTokenElevation As Integer = 0
        Dim pTokenElevation As IntPtr = IntPtr.Zero

        Try
            ' Open the access token of the current process with TOKEN_QUERY.
            If (Not NativeMethods.OpenProcessToken(Process.GetCurrentProcess.Handle,
                NativeMethods.TOKEN_QUERY, hToken)) Then
                Throw New Win32Exception
            End If

            ' Allocate a buffer for the elevation information.
            cbTokenElevation = Marshal.SizeOf(GetType(TOKEN_ELEVATION))
            pTokenElevation = Marshal.AllocHGlobal(cbTokenElevation)
            If (pTokenElevation = IntPtr.Zero) Then
                Throw New Win32Exception
            End If

            ' Retrieve token elevation information.
            If (Not NativeMethods.GetTokenInformation(hToken,
                TOKEN_INFORMATION_CLASS.TokenElevation,
                pTokenElevation, cbTokenElevation, cbTokenElevation)) Then
                Throw New Win32Exception
            End If

            ' Marshal the TOKEN_ELEVATION struct from native to .NET
            Dim elevation As TOKEN_ELEVATION = DirectCast(Marshal.PtrToStructure(pTokenElevation, GetType(TOKEN_ELEVATION)), TOKEN_ELEVATION)
            fIsElevated = (elevation.TokenIsElevated <> 0)

        Finally
            ' Centralized cleanup for all allocated resources.
            If (Not hToken Is Nothing) Then
                hToken.Close()
                hToken = Nothing
            End If
            If (pTokenElevation <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pTokenElevation)
                pTokenElevation = IntPtr.Zero
                cbTokenElevation = 0
            End If
        End Try

        Return fIsElevated
    End Function

#End Region

    Shared Function GetImage(Name As String) As Bitmap
        Dim Bitmap As System.Drawing.Bitmap = Nothing
        '
        'Try
        '       Bitmap = New Bitmap(Application.StartupPath & "\resources\" & Name)
        'Catch ex As Exception
        'End Try
        '
        'Return Bitmap
        Return Bitmap
    End Function

    Shared Function GetIcon(Name As String) As Icon
        Dim Icon As System.Drawing.Icon = Nothing

        Try
            Icon = New Icon(Application.StartupPath & "\resources\" & Name & ".ico", 16, 16)
        Catch ex As Exception

        End Try

        Return Icon
    End Function

End Class

Friend Class Enums
    Friend Enum TOKEN_INFORMATION_CLASS
        TokenUser = 1
        TokenGroups
        TokenPrivileges
        TokenOwner
        TokenPrimaryGroup
        TokenDefaultDacl
        TokenSource
        TokenType
        TokenImpersonationLevel
        TokenStatistics
        TokenRestrictedSids
        TokenSessionId
        TokenGroupsAndPrivileges
        TokenSessionReference
        TokenSandBoxInert
        TokenAuditPolicy
        TokenOrigin
        TokenElevationType
        TokenLinkedToken
        TokenElevation
        TokenHasRestrictions
        TokenAccessInformation
        TokenVirtualizationAllowed
        TokenVirtualizationEnabled
        TokenIntegrityLevel
        TokenUIAccess
        TokenMandatoryPolicy
        TokenLogonSid
        MaxTokenInfoClass
    End Enum

    Friend Enum SECURITY_IMPERSONATION_LEVEL
        SecurityAnonymous = 0
        SecurityIdentification
        SecurityImpersonation
        SecurityDelegation
    End Enum

    Friend Enum TOKEN_ELEVATION_TYPE
        TokenElevationTypeDefault = 1
        TokenElevationTypeFull
        TokenElevationTypeLimited
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure SID_AND_ATTRIBUTES
        Public Sid As IntPtr
        Public Attributes As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure TOKEN_ELEVATION
        Public TokenIsElevated As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure TOKEN_MANDATORY_LABEL
        Public Label As SID_AND_ATTRIBUTES
    End Structure
End Class

Friend Class SafeTokenHandle
    Inherits SafeHandleZeroOrMinusOneIsInvalid

    Private Sub New()
        MyBase.New(True)
    End Sub

    Friend Sub New(ByVal handle As IntPtr)
        MyBase.New(True)
        MyBase.SetHandle(handle)
    End Sub

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Shared Function CloseHandle(ByVal handle As IntPtr) As Boolean
    End Function

    Protected Overrides Function ReleaseHandle() As Boolean
        Return SafeTokenHandle.CloseHandle(MyBase.handle)
    End Function

End Class

Friend Class NativeMethods

    ' Token Specific Access Rights

    Public Const STANDARD_RIGHTS_REQUIRED As UInt32 = &HF0000
    Public Const STANDARD_RIGHTS_READ As UInt32 = &H20000
    Public Const TOKEN_ASSIGN_PRIMARY As UInt32 = 1
    Public Const TOKEN_DUPLICATE As UInt32 = 2
    Public Const TOKEN_IMPERSONATE As UInt32 = 4
    Public Const TOKEN_QUERY As UInt32 = 8
    Public Const TOKEN_QUERY_SOURCE As UInt32 = &H10
    Public Const TOKEN_ADJUST_PRIVILEGES As UInt32 = &H20
    Public Const TOKEN_ADJUST_GROUPS As UInt32 = &H40
    Public Const TOKEN_ADJUST_DEFAULT As UInt32 = &H80
    Public Const TOKEN_ADJUST_SESSIONID As UInt32 = &H100
    Public Const TOKEN_READ As UInt32 = (STANDARD_RIGHTS_READ Or TOKEN_QUERY)
    Public Const TOKEN_ALL_ACCESS As UInt32 = (STANDARD_RIGHTS_REQUIRED Or
        TOKEN_ASSIGN_PRIMARY Or TOKEN_DUPLICATE Or TOKEN_IMPERSONATE Or
        TOKEN_QUERY Or TOKEN_QUERY_SOURCE Or TOKEN_ADJUST_PRIVILEGES Or
        TOKEN_ADJUST_GROUPS Or TOKEN_ADJUST_DEFAULT Or TOKEN_ADJUST_SESSIONID)


    Public Const ERROR_INSUFFICIENT_BUFFER As Int32 = 122


    ' Integrity Levels

    Public Const SECURITY_MANDATORY_UNTRUSTED_RID As Integer = 0
    Public Const SECURITY_MANDATORY_LOW_RID As Integer = &H1000
    Public Const SECURITY_MANDATORY_MEDIUM_RID As Integer = &H2000
    Public Const SECURITY_MANDATORY_HIGH_RID As Integer = &H3000
    Public Const SECURITY_MANDATORY_SYSTEM_RID As Integer = &H4000

    <DllImport("advapi32", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Shared Function OpenProcessToken(
        ByVal hProcess As IntPtr,
        ByVal desiredAccess As UInt32,
        <Out()> ByRef hToken As SafeTokenHandle) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Shared Function DuplicateToken(
        ByVal ExistingTokenHandle As SafeTokenHandle,
        ByVal ImpersonationLevel As SECURITY_IMPERSONATION_LEVEL,
        <Out()> ByRef DuplicateTokenHandle As SafeTokenHandle) _
        As Boolean
    End Function

    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Shared Function GetTokenInformation(
        ByVal hToken As SafeTokenHandle,
        ByVal tokenInfoClass As TOKEN_INFORMATION_CLASS,
        ByVal pTokenInfo As IntPtr,
        ByVal tokenInfoLength As Integer,
        <Out()> ByRef returnLength As Integer) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Public Const BCM_SETSHIELD As UInt32 = &H160C
    <DllImport("user32", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Shared Function SendMessage(
        ByVal hWnd As IntPtr,
        ByVal Msg As UInt32,
        ByVal wParam As Integer,
        ByVal lParam As IntPtr) _
        As Integer
    End Function

    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Shared Function GetSidSubAuthority(
        ByVal pSid As IntPtr,
        ByVal nSubAuthority As UInt32) _
        As IntPtr
    End Function

End Class

Friend Class LinkHelper
    <DllImport("shfolder.dll", CharSet:=CharSet.Auto)>
    Friend Shared Function SHGetFolderPath(hwndOwner As IntPtr, nFolder As Integer, hToken As IntPtr, dwFlags As Integer, lpszPath As StringBuilder) As Integer
    End Function

    <Flags()>
    Private Enum SLGP_FLAGS
        SLGP_SHORTPATH = &H1
        SLGP_UNCPRIORITY = &H2
        SLGP_RAWPATH = &H4
    End Enum

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure WIN32_FIND_DATAW
        Public dwFileAttributes As UInteger
        Public ftCreationTime As Long
        Public ftLastAccessTime As Long
        Public ftLastWriteTime As Long
        Public nFileSizeHigh As UInteger
        Public nFileSizeLow As UInteger
        Public dwReserved0 As UInteger
        Public dwReserved1 As UInteger
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)>
        Public cFileName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=14)>
        Public cAlternateFileName As String
    End Structure

    ''' <summary>The IShellLink interface allows Shell links to be created, modified, and resolved</summary>
    <ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")>
    Private Interface IShellLinkW
        ''' <summary>Retrieves the path and file name of a Shell link object</summary>
        Sub GetPath(<Out(), MarshalAs(UnmanagedType.LPWStr)> pszFile As StringBuilder, cchMaxPath As Integer, ByRef pfd As WIN32_FIND_DATAW, fFlags As SLGP_FLAGS)

    End Interface

    <ComImport(), Guid("0000010c-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IPersist
        <PreserveSig()>
        Sub GetClassID(ByRef pClassID As Guid)
    End Interface


    <ComImport(), Guid("0000010b-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IPersistFile
        Inherits IPersist
        Shadows Sub GetClassID(ByRef pClassID As Guid)
        <PreserveSig()>
        Function IsDirty() As Integer

        <PreserveSig()>
        Sub Load(<[In](), MarshalAs(UnmanagedType.LPWStr)> pszFileName As String, dwMode As UInteger)

        <PreserveSig()>
        Sub Save(<[In](), MarshalAs(UnmanagedType.LPWStr)> pszFileName As String, <[In](), MarshalAs(UnmanagedType.Bool)> fRemember As Boolean)

        <PreserveSig()>
        Sub SaveCompleted(<[In](), MarshalAs(UnmanagedType.LPWStr)> pszFileName As String)

        <PreserveSig()>
        Sub GetCurFile(<[In](), MarshalAs(UnmanagedType.LPWStr)> ppszFileName As String)
    End Interface

    Const STGM_READ As UInteger = 0
    Const MAX_PATH As Integer = 260

    ' CLSID_ShellLink from ShlGuid.h 
    <ComImport(), Guid("00021401-0000-0000-C000-000000000046")> Public Class ShellLink
    End Class


    Public Shared Function ResolveShortcut(filename As String) As String
        Dim link As New ShellLink()
        DirectCast(link, IPersistFile).Load(filename, STGM_READ)
        ' TODO: if I can get hold of the hwnd call resolve first. This handles moved and renamed files.  
        ' ((IShellLinkW)link).Resolve(hwnd, 0) 
        Dim sb As New StringBuilder(MAX_PATH)
        Dim data As New WIN32_FIND_DATAW()
        DirectCast(link, IShellLinkW).GetPath(sb, sb.Capacity, data, 0)
        Return sb.ToString()
    End Function

End Class

Friend Class Version
    Inherits TextFileManager

    Private _Major As String
    Private _Minor As String
    Private _Build As String
    Private _Copyright As String
    Private _CompanyName As String

    Sub New()

        'Try to check if we have permission to the application folder
        Dim HaveAccess As Boolean = False
        Try
            Dim Test As String = IO.Path.Combine(Application.StartupPath, "test.log")
            Dim Stream As StreamWriter = IO.File.CreateText(Test)
            Stream.Close()

            IO.File.Delete(Test)
            HaveAccess = True
        Catch ex As Exception
        End Try

        'If HaveAccess Then
        'Me.Path = IO.Path.Combine(Application.StartupPath, "version.inf")
        'Else
        'Me.Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName & "\version.inf"
        Me.Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName & "\config.ini"
        ''End If
        '
        Dim T() As String = Me.ReadAllLines

        If T.Length > 0 Then
            Dim Sec As CLS_SECURITY.Aes256Base64Encrypter = New CLS_SECURITY.Aes256Base64Encrypter
            Dim Content() As String = Split(Sec.Decrypt(T(0), Application.ProductName, True), vbCrLf)
            Dim Val As String = ""
            Dim Key As String = ""

            For I As Integer = 0 To Content.Length - 1
                Key = Content(I)
                Val = Trim(Key.Substring(Key.IndexOf("=", StringComparison.Ordinal) + 1))
                If Key.StartsWith("MAJOR=") Then
                    _Major = Val
                ElseIf Key.StartsWith("MINOR=") Then
                    _Minor = Val
                ElseIf Key.StartsWith("BUILD=") Then
                    _Build = Val
                ElseIf Key.StartsWith("COPYRIGHT=") Then
                    _Copyright = Val
                ElseIf Key.StartsWith("COMPANY=") Then
                    _CompanyName = Val
                End If
            Next

        End If
    End Sub

    ReadOnly Property Major As String
        Get
            'Return _Major
            Return "3"
        End Get
    End Property

    ReadOnly Property Minor As String
        Get
            'Return _Minor
            Return "0"
        End Get
    End Property

    ReadOnly Property Build As String
        Get
            'Return _Build
            Return "1"
        End Get
    End Property

    ReadOnly Property Copyright As String
        Get
            Return _Copyright
        End Get
    End Property

    ReadOnly Property CompanyName As String
        Get
            Return _CompanyName
        End Get
    End Property
End Class

Friend Class MessageHelper
    Inherits TextFileManager

    Sub New(Config As Config)

        Dim Lang As String = Config.LANGUAGE
        If Trim(Lang) = "" Then
            Lang = "default"
        End If

        Dim P As String = Application.StartupPath & "\Languages\" & Lang & ".lan"
        If Not IO.File.Exists(P) Then
            P = Environment.CurrentDirectory & "\Languages\" & Lang & ".lan"
        End If

        Me.Path = P
    End Sub

    Function GetMessage(CodeMessage As String, Optional Clear As Boolean = False) As String
        Dim Messages() As String = Me.ReadAllLines

        For I As Integer = 0 To Messages.Length - 1
            If Messages(I).ToLower.Trim.StartsWith(CodeMessage.Trim.ToLower & ": ") Then
                Return CleanMessage(Messages(I).Substring(CodeMessage.Trim.Length + 2))
            End If
        Next

        If Clear Then
            Return ""
        End If
        Return CodeMessage

    End Function

    Private Function CleanMessage(Msg As String) As String
        Msg = Msg.Replace("{USER_NAME}", System.Environment.UserName)
        Msg = Msg.Replace("{PROD_NAME}", Application.ProductName)

        Return Msg
    End Function
End Class

Friend Class UpdateInfo
    Public Version As Integer
    Public ExtendVersion As String
    Public Token As String
    Public ForceUpdate As Boolean
End Class

Friend Class Update

    Public Event OnStart()
    Public Event OnEnd(Reason As EndReason, ByVal DownloadedFileName As String, ByVal Exception As Exception)
    Public Event OnCancel(ByRef Cancel As Boolean)
    Public Event OnProgress(ByVal BytesToDownload As Long, ByVal BytesDowloadedSoFar As Long, ByVal BytesDownloaded As Integer)

    Private _InProgress As Boolean

    Private Msg As MessageHelper
    Private Config As Nethifier.Config

    Public Enum EndReason
        Cancel = 0
        Downloaded = 1
        [Error] = 2
    End Enum

    Private Userlogin As Login

    Sub New(LG As Login, Config As Nethifier.Config)
        Userlogin = LG
        Me.Config = Config
        Msg = New MessageHelper(Config)
    End Sub

    ReadOnly Property InProgress As Boolean
        Get
            Return _InProgress
        End Get
    End Property

    Public Shared Function UpdateURL() As String
        Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName & "\update.url"
    End Function
    Private Function GetUpdateURL() As String

        Dim Url As String = ""
        Dim Path As String = UpdateURL()
        If File.Exists(Path) Then
            Dim Decrypt As CLS_SECURITY.Aes256Base64Encrypter = New CLS_SECURITY.Aes256Base64Encrypter
            Url = Decrypt.Decrypt(File.ReadAllText(Path), Application.ProductName, True)
        End If
        Return Url

    End Function

    Private Function CheckAvailability() As UpdateInfo

        Dim Info As UpdateInfo = New UpdateInfo

        'Gestione Expired HTTPS certificate
        System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf CertificateHandler

        Dim s As WebRequest  'HttpWebRequest
        Dim enc As UTF8Encoding
        Dim postdata As String = ""
        Dim postdatabytes As Byte()

        Dim Url As String = GetUpdateURL()

        If Trim(Url) = "" Then
            Return Info
        End If

        Try
            s = WebRequest.Create(Url)
            enc = New System.Text.UTF8Encoding()

            If Not IsNothing(Userlogin) Then
                postdata = "username=" & Web.HttpUtility.UrlEncode(Userlogin.Username) & "&token=" & Web.HttpUtility.UrlEncode(Userlogin.Token) & "&"
            End If
            postdata += "version=" & Application.ProductVersion.Replace(".", "") & "&check=update"
            postdatabytes = enc.GetBytes(postdata)
            's.Headers.Add("checkavailabilty", UserInfo)

            s.Method = "POST"
            's.Timeout = 30
            s.ContentType = "application/x-www-form-urlencoded"
            s.ContentLength = postdatabytes.Length

            Using Stream = s.GetRequestStream()
                Stream.Write(postdatabytes, 0, postdatabytes.Length)
            End Using

            Dim Neth As String = "" '"Neth"
            Dim Resp As WebResponse = s.GetResponse
            If IsNumeric(Resp.Headers(Neth & "Version")) Then
                Info.Version = CInt(Resp.Headers(Neth & "Version"))
            End If
            Info.ExtendVersion = Trim("" & Resp.Headers(Neth & "ExtendVersion"))
            Info.Token = Trim("" & Resp.Headers(Neth & "Token"))
            Info.ForceUpdate = Trim("" & Resp.Headers(Neth & "ForcedUpdate")) = "1"

            'Dim dataStream As Stream = s.GetResponse.GetResponseStream()
            'Dim reader As New StreamReader(dataStream)
            '' Read the content.
            'NethVersion = reader.ReadToEnd()
            'Nonce = Trim("" & s.GetResponse.Headers("www-authenticate"))
        Catch ex As Exception
            'Dim DoExit As Boolean = True

            'If TypeOf ex Is WebException Then
            '    Dim wEx As WebException = DirectCast(ex, WebException)
            '    If wEx.Response IsNot Nothing Then

            '        'can use ex.Response.Status, .StatusDescription

            '        Dim Resp As HttpWebResponse = DirectCast(wEx.Response, HttpWebResponse)
            '        If Resp.StatusCode = HttpStatusCode.Unauthorized Then
            '            Nonce = wEx.Response.Headers.Item("www-authenticate")
            '        End If
            '    End If
            'End If
        End Try
        Return Info
    End Function
    Private Shared Function CertificateHandler(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal SSLerror As SslPolicyErrors) As Boolean
        Return True
    End Function

    Public Sub Update()
        Dim Info As UpdateInfo = Me.CheckAvailability()
        If Info.Version = 0 OrElse (Info.Version <= CInt(Application.ProductVersion.Replace(".", ""))) Then
            MessageBox.Show(Msg.GetMessage("AGG_001"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        Else
            If Not My.Application.CommandLineArgs.Contains("-update") Then
                If Not Info.ForceUpdate Then
                    Dim Ans As DialogResult = MessageBox.Show(Msg.GetMessage("AGG_002").Replace("#EXTEND_VERSION#", Info.ExtendVersion), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If Ans = DialogResult.No Then
                        Exit Sub
                    End If
                End If
            End If
        End If

        If Helper.IsRunAsAdmin Then
            Dim Update As New FRM_UPDATE(Userlogin, Info, Config)
            Update.ShowDialog()
            Exit Sub
        End If

        Dim proc As New ProcessStartInfo
        With proc
            .UseShellExecute = True
            .WorkingDirectory = Environment.CurrentDirectory
            .FileName = Application.ExecutablePath
            .Arguments = "-update"
            .Verb = "runas"
        End With

        Try
            Process.Start(proc)
            Application.ExitThread()
        Catch ex As Exception
            ' The user refused the elevation. 
            ' Do nothing and return directly ... 
            'MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub Download(ByVal AddressURL As String, ByVal FolderDestination As String)

        'Bug.SendReport()
        'Return True

        'Dim ExceptionManager As ExceptionManager = New ExceptionManager

        Dim URLReq As HttpWebRequest
        Dim URLRes As HttpWebResponse
        Dim FileStreamer As FileStream
        Dim bBuffer(999) As Byte

        Dim iBytesRead As Integer
        Dim sChunks As Stream

        Dim Url As String = GetUpdateURL()
        If Trim(Url) = "" Then
            RaiseEvent OnEnd(EndReason.Error, "", New Exception("Invalid target URL. No update available."))
            Exit Sub
        End If

        AddressURL = Url & "?" & AddressURL
        Dim UpdateName As String = Now.ToString("yyyyMMddhhmmss")

        Application.DoEvents()
        Try
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000
            URLReq = CType(WebRequest.Create(AddressURL), HttpWebRequest)
            URLRes = CType(URLReq.GetResponse, HttpWebResponse) 'Error occurs here!!
            sChunks = URLReq.GetResponse.GetResponseStream

            Dim ContentDisposition As String = URLRes.GetResponseHeader("Content-Disposition")

            If ContentDisposition <> "" AndAlso ContentDisposition.IndexOf("=", StringComparison.Ordinal) > -1 Then
                UpdateName += "_" & Split(ContentDisposition, "=")(1)
            Else
                UpdateName += ".exe"
            End If
            FileStreamer = New FileStream(IO.Path.Combine(FolderDestination, UpdateName), FileMode.Create)

            Dim DownloadedSoFar As Long
            Dim BytesToDownload As Long = URLRes.ContentLength

            If BytesToDownload <= 0 Then
                RaiseEvent OnEnd(EndReason.Error, "", New Exception("Corrupt or invalid file."))
                Exit Sub
            End If

            Do
                If _Cancel Then
                    Dim Cancel As Boolean
                    RaiseEvent OnCancel(Cancel)

                    If Cancel Then
                        _InProgress = False
                        RaiseEvent OnEnd(EndReason.Cancel, "", Nothing)
                        Exit Sub
                    End If
                End If
                _InProgress = True

                iBytesRead = sChunks.Read(bBuffer, 0, 1000)
                FileStreamer.Write(bBuffer, 0, iBytesRead)

                DownloadedSoFar += iBytesRead

                RaiseEvent OnProgress(BytesToDownload, DownloadedSoFar, iBytesRead)
                Application.DoEvents()
            Loop Until iBytesRead = 0

            sChunks.Close()
            FileStreamer.Close()
            URLRes.Close()

            RaiseEvent OnEnd(EndReason.Downloaded, UpdateName, Nothing)
        Catch ex As Exception
            RaiseEvent OnEnd(EndReason.Error, "", ex)
        End Try
    End Sub

    Private _Cancel As Boolean
    Public Sub Cancel()
        _Cancel = True
    End Sub

    Public Shared Function FormatFileSize(ByVal FileSizeBytes As Long) As String
        Dim sizeTypes() As String = {"b", "Kb", "Mb", "Gb"}
        Dim Len As Decimal = FileSizeBytes
        Dim sizeType As Integer = 0
        Do While Len > 1024
            Len = Decimal.Round(Len / 1024, 2)
            sizeType += 1
            If sizeType >= sizeTypes.Length - 1 Then Exit Do
        Loop

        Dim Resp As String = Len.ToString & " " & sizeTypes(sizeType)
        Return Resp
    End Function
End Class