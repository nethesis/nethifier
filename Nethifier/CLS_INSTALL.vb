Imports IWshRuntimeLibrary
Imports Microsoft.Win32
Imports System.IO
Imports System.Security.Permissions
Imports System.Net.Mail
Imports System.Net

Friend Class CLS_INSTALLER
    Private StartUpPath As String
    Private StartMenuPath As String
    Private DesktopPath As String
    Private ApplicationPath As String
    Private ProgramFileFolder As String

    Private Msg As MessageHelper

    Friend Enum Action
        Install = 0
        Uninstall = 1
    End Enum

    'Private _Msg As MessageHelper
    'Property Messages As MessageHelper
    '    Get
    '        Return _Msg
    '    End Get
    '    Set(value As MessageHelper)
    '        _Msg = value
    '    End Set
    'End Property

    Private Function CreateFolders(AllUsers As Boolean, Optional ProgramFolder As String = "", Optional UserDataFolder As String = "") As Boolean

        If AllUsers Then
            StartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)
            StartUpPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup)
            DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
        Else
            StartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)
            StartUpPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup)
            DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
        End If

        'MessageBox.Show(StartUpPath)

        ProgramFileFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        ApplicationPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)

        StartMenuPath += "\" & Application.ProductName
        ApplicationPath += "\" & Application.ProductName
        ProgramFileFolder += "\" & Application.ProductName

        If ProgramFolder.Trim <> "" Then
            Try
                If ProgramFolder.ToLower.Trim <> ProgramFileFolder.ToLower.Trim Then
                    'Create folder
                    If Not Directory.Exists(ProgramFolder) Then
                        If MessageBox.Show(Msg.GetMessage("INS_018").Replace("{PROD_FOLDER}", ProgramFolder), Msg.GetMessage("INS_012"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            Directory.CreateDirectory(ProgramFolder)
                            ProgramFileFolder = ProgramFolder
                        Else
                            Return False
                        End If
                    End If
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message, Msg.GetMessage("INS_012"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try
        End If

        ProgramFolder = ProgramFileFolder

        Try
            If Not Directory.Exists(StartMenuPath) Then
                Directory.CreateDirectory(StartMenuPath)
            End If
            If Not Directory.Exists(ApplicationPath) Then
                Directory.CreateDirectory(ApplicationPath)
            End If
            If Not Directory.Exists(ProgramFileFolder) Then
                Directory.CreateDirectory(ProgramFileFolder)
            End If
            If Not Directory.Exists(ProgramFileFolder & "\languages") Then
                Directory.CreateDirectory(ProgramFileFolder & "\languages")
            End If
            If Not Directory.Exists(ProgramFileFolder & "\resources") Then
                Directory.CreateDirectory(ProgramFileFolder & "\resources")
            End If
            If Not Directory.Exists(ProgramFileFolder & "\update") Then
                Directory.CreateDirectory(ProgramFileFolder & "\update")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Msg.GetMessage("INS_012"))
            Return False
        End Try

        Return True
    End Function

    Private Function DeleteFiles(Path As String, ByRef SuccedLog As String, ByRef FailedLog As String, ByVal DeleteConfig As Boolean) As Boolean

        Dim Proceed As Boolean = True

        Try
            Dim Dir() As String = Directory.GetDirectories(Path)
            Dim Files() As String = Directory.GetFiles(Path)

            For I As Integer = 0 To Dir.Length - 1
                If Not DeleteFiles(Dir(I), SuccedLog, FailedLog, DeleteConfig) Then
                    Proceed = False
                End If
            Next

            Dim IgnoreFiles = New Hashtable
            Dim SP As String = Application.StartupPath.ToLower

            With IgnoreFiles
                .Add(SP & "nethifier.exe", "")
                .Add(SP & "nethutilities.dll", "")
            End With

            For I As Integer = 0 To Files.Length - 1
                If Not IgnoreFiles.ContainsKey(Files(I).ToLower.Trim) Then
                    If Files(I).ToLower <> Application.ExecutablePath.ToLower Then
                        Try
                            If Not DeleteConfig AndAlso Files(I).Trim.ToLower.EndsWith("\config.ini") Then
                                'ByPass
                            Else
                                IO.File.Delete(Files(I))
                                SuccedLog += "File deleted: " & Files(I) & vbCrLf
                            End If
                        Catch ex As Exception
                            FailedLog += "**********************************************" & vbCrLf
                            FailedLog += "Can't delete this file:" & Files(I) & vbCrLf
                            FailedLog += "Reason: " & ex.Message & vbCrLf
                            FailedLog += "**********************************************" & vbCrLf

                            Proceed = False
                        End Try
                    End If
                End If
            Next

            Try
                If IO.Directory.GetFiles(Path).Length = 0 Then
                    IO.Directory.Delete(Path)
                    SuccedLog += "Directory deleted: " & Path & vbCrLf
                Else
                    SuccedLog += "This directory can be deleted manually: " & Path & vbCrLf
                End If
            Catch ex As Exception
                FailedLog += "**********************************************" & vbCrLf
                FailedLog += "Can't delete this folder:" & Path & vbCrLf
                FailedLog += "Reason: " & ex.Message & vbCrLf
                FailedLog += "**********************************************" & vbCrLf

                Proceed = False
            End Try
        Catch ex As Exception
            MessageBox.Show(ex.Message, Msg_INS_014, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Proceed = False
        End Try

        Return Proceed
    End Function

    Private Msg_INS_019 As String = ""
    Private Msg_INS_014 As String = ""

    Public Function Uninstall(DeleteConfig As Boolean) As Boolean

        Try
            Dim Conf As Config = New Config
            Msg = New MessageHelper(Conf)

            Msg_INS_019 = Msg.GetMessage("INS_019")
            Msg_INS_014 = Msg.GetMessage("INS_014")

            Dim Users As String() = IO.File.ReadAllLines(Application.StartupPath & "\users.ini")

            Dim SuccedLog As String = ""
            Dim FailedLog As String = ""
            Dim Proceed As Boolean = True
            Dim Path As String = ""

            For I As Integer = 0 To Users.Length - 1
                If Trim(Users(I)) <> "" Then
                    If Trim(Users(I)).ToLower = System.Environment.UserName.ToLower Then
                        Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName
                        If IO.File.Exists(Path & "\installation.log") Then
                            Dim Log As InstallationInfo = New InstallationInfo(Path)
                            Dim Val As String() = Log.ReadAllLines

                            For X = 0 To Val.Length - 1
                                Dim F As String = Val(X)
                                If IO.Directory.Exists(F) Then
                                    DeleteFiles(F, SuccedLog, FailedLog, DeleteConfig)
                                ElseIf IO.File.Exists(F) Then
                                    IO.File.Delete(F)
                                End If
                            Next
                        End If
                        DeleteFiles(Path, SuccedLog, FailedLog, DeleteConfig)
                    Else
                        Proceed = False
                    End If
                End If
            Next

            If Proceed Then
                If Not DeleteConfig Then
                    Conf.Save()
                End If

                Path = Application.StartupPath
                If IO.File.Exists(Path & "\installation.log") Then
                    Dim Log As InstallationInfo = New InstallationInfo(Path)
                    Dim Val As String() = Log.ReadAllLines
                    For X = 0 To Val.Length - 1
                        Dim F As String = Val(X)
                        If IO.Directory.Exists(F) Then
                            DeleteFiles(F, SuccedLog, FailedLog, DeleteConfig)
                        ElseIf IO.File.Exists(F) Then
                            IO.File.Delete(F)
                        End If
                    Next
                End If


                ''2017/04/09 Jomar
                'If Not DeleteFiles(Path, SuccedLog, FailedLog, DeleteConfig) Then
                '    If Trim(FailedLog) <> "" Then
                '        MessageBox.Show(FailedLog, Msg_INS_014, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '        Return False
                '    End If
                'End If
                ''2017/04/09 Jomar
                UninstallBatchFile.CreateBatch(Application.StartupPath)


                Dim RegKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Nethesis\" & Application.ProductName)
                If Not IsNothing(RegKey) Then

                    Registry.LocalMachine.DeleteSubKey("Software\Microsoft\Nethesis\" & Application.ProductName)
                    RegKey = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Nethesis")
                    If RegKey.SubKeyCount <= 0 Then
                        Registry.LocalMachine.DeleteSubKey("Software\Microsoft\Nethesis")
                    End If
                End If
            Else
                MessageBox.Show(Msg_INS_019, Msg_INS_014, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Msg_INS_014, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        Return True
    End Function

    Public Function Install(AllUsers As Boolean, WindowsStartup As Boolean, DesktopShortcut As Boolean, Optional ByRef ProgramFolder As String = "", Optional ByRef UserAppFolder As String = "") As Boolean

        Dim Config As New Config
        Msg = New MessageHelper(Config)

        If Not CreateFolders(AllUsers, ProgramFolder) Then
            Return False
        End If

        Dim InstallationInfo As String = ""

        Dim shell As WshShell = New WshShell
        Dim ExePath As String = ""
        Dim ExeName As String = Application.ExecutablePath.Replace(Application.StartupPath & "\", "")

        'Copy Files to Program Folder
        If ExeName.ToLower <> Application.ProductName.ToLower & ".exe" Then
            ExeName = Application.ProductName & ".exe"
        End If

        ExePath = ProgramFileFolder & "\" & ExeName

        Dim RunningProcess As Process = Nothing

        If Helper.IsApplicationRunning(ExePath, RunningProcess) Then
            If MessageBox.Show(Msg.GetMessage("INS_001"), Msg.GetMessage("INS_012"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                RunningProcess.Kill()
                Do
                    If Helper.IsApplicationRunning(ExePath, RunningProcess) Then
                        RunningProcess.Kill()
                    Else
                        Exit Do
                    End If
                Loop
            Else
                Application.Exit()
                Exit Function
            End If
        End If

        IO.File.Copy(Application.ExecutablePath, ExePath, True)
        IO.File.Copy(Application.StartupPath & "\HidLibrary.dll", ProgramFileFolder & "\HidLibrary.dll", True)
        IO.File.Copy(Application.StartupPath & "\LuxaforSharp.dll", ProgramFileFolder & "\LuxaforSharp.dll", True)
        IO.File.Copy(Application.StartupPath & "\NAudio.dll", ProgramFileFolder & "\NAudio.dll", True)
        IO.File.Copy(Application.StartupPath & "\NethLED.exe", ProgramFileFolder & "\NethLED.exe", True)
        IO.File.Copy(Application.StartupPath & "\NethDialer.exe", ProgramFileFolder & "\NethDialer.exe", True)
        IO.File.Copy(Application.StartupPath & "\NethUtilities.dll", ProgramFileFolder & "\NethUtilities.dll", True)
        IO.File.Copy(Application.StartupPath & "\Newtonsoft.Json.dll", ProgramFileFolder & "\Newtonsoft.Json.dll", True)
        IO.File.Copy(Application.StartupPath & "\ringer.mp3", ProgramFileFolder & "\ringer.mp3", True)
        '
        IO.File.Copy(Application.StartupPath & "\Version.inf", ProgramFileFolder & "\Version.inf", True)

        Dim UpdateURL As String = IO.Path.Combine(Application.StartupPath, "Update.url")
        If IO.File.Exists(UpdateURL) Then
            IO.File.Copy(UpdateURL, IO.Path.Combine(ProgramFileFolder, "Update.url"), True)
        End If
        'IO.File.Copy(Application.StartupPath & "\Update.url", ProgramFileFolder & "\Update.url", True)

        UserAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName '& "\Languages\"

        Dim UserLanguages As String = UserAppFolder & "\languages"
        Dim UserResources As String = UserAppFolder & "\resources"
        If Not Directory.Exists(UserAppFolder) Then
            Directory.CreateDirectory(UserAppFolder)
        End If
        If Not Directory.Exists(UserLanguages) Then
            Directory.CreateDirectory(UserLanguages)
        End If
        If Not Directory.Exists(UserResources) Then
            Directory.CreateDirectory(UserResources)
        End If

        'Dim Info As InstallationInfo = New InstallationInfo(ProgramFileFolder)
        'Info.Delete()
        'Info.Write(InstallationInfo)

        InstallationInfo = ""

        Dim Lang As String() = Directory.GetFiles(Application.StartupPath & "\languages")
        For I As Integer = 0 To Lang.Length - 1
            Dim X As Integer = Lang(I).LastIndexOf("\", StringComparison.Ordinal)
            IO.File.Copy(Lang(I), ProgramFileFolder & "\languages" & Lang(I).Substring(X, Lang(I).Length - X), True)
            'NO NEED TO CREATE 
            'IO.File.Copy(Lang(I), UserLanguages & Lang(I).Substring(X, Lang(I).Length - X), True)
        Next

        Dim Res As String() = Directory.GetFiles(Application.StartupPath & "\resources")
        For I As Integer = 0 To Res.Length - 1
            Dim X As Integer = Res(I).LastIndexOf("\", StringComparison.Ordinal)
            IO.File.Copy(Res(I), ProgramFileFolder & "\resources" & Res(I).Substring(X, Res(I).Length - X), True)
            'NO NEED TO CREATE 
            'IO.File.Copy(Res(I), UserResources & Res(I).Substring(X, Res(I).Length - X), True)
        Next

        If Not IO.File.Exists(StartMenuPath & "\" & Application.ProductName & ".lnk") Then
            Try
                With DirectCast(shell.CreateShortcut(StartMenuPath & "\" & Application.ProductName & ".lnk"), IWshShortcut)
                    .TargetPath = ExePath
                    .Arguments = "-e"
                    .Description = Application.ProductName
                    .WorkingDirectory = ProgramFileFolder
                    .IconLocation = ExePath & ", 0"
                    .Save()
                End With
            Catch ex As Exception
                MessageBox.Show(ex.Message, Msg.GetMessage("INS_012"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try
        End If

        If Not IO.File.Exists(StartMenuPath & "\Uninstall.lnk") Then
            Try
                With DirectCast(shell.CreateShortcut(StartMenuPath & "\Uninstall.lnk"), IWshShortcut)
                    .TargetPath = ExePath
                    .Arguments = "-u"
                    .Description = "Uninstall " & Application.ProductName
                    .WorkingDirectory = ProgramFileFolder
                    .IconLocation = ProgramFileFolder & "\resources\uninstall.ico"
                    .Save()
                End With
            Catch ex As Exception
                MessageBox.Show(ex.Message, Msg.GetMessage("INS_012"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try
        End If
        InstallationInfo += StartMenuPath & vbCrLf

        If WindowsStartup Then
            If Not IO.File.Exists(StartUpPath & "\" & Application.ProductName & ".lnk") Then
                Try
                    With DirectCast(shell.CreateShortcut(StartUpPath & "\" & Application.ProductName & ".lnk"), IWshShortcut)
                        .TargetPath = ExePath
                        .Arguments = "-e"
                        .Description = Application.ProductName
                        .WorkingDirectory = ProgramFileFolder
                        .IconLocation = ExePath & ", 0"
                        .Save()
                    End With
                Catch ex As Exception
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End Try
            End If
            InstallationInfo += StartUpPath & "\" & Application.ProductName & ".lnk" & vbCrLf
        End If

        If DesktopShortcut Then
            'Desktop Link
            If Not IO.File.Exists(DesktopPath & "\" & Application.ProductName & ".lnk") Then
                Try
                    With DirectCast(shell.CreateShortcut(DesktopPath & "\" & Application.ProductName & ".lnk"), IWshShortcut)
                        .TargetPath = ExePath
                        .Arguments = "-e"
                        .Description = Application.ProductName
                        .WorkingDirectory = ProgramFileFolder
                        .IconLocation = ExePath & ", 0"
                        .Save()
                    End With
                Catch ex As Exception
                    MessageBox.Show(ex.Message, Msg.GetMessage("INS_012"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End Try
            End If
            InstallationInfo += DesktopPath & "\" & Application.ProductName & ".lnk" & vbCrLf
        End If

        'InstallationInfo += ApplicationPath & vbCrLf
        Dim Info As InstallationInfo
        If Not AllUsers Then
            Info = New InstallationInfo(UserAppFolder)
        Else
            Info = New InstallationInfo(ProgramFileFolder)
        End If
        Info.Delete()
        Info.Write(InstallationInfo)

        Dim User As UsersInfo = New UsersInfo(ProgramFileFolder)
        User.Write(Environment.UserName)

        'Create first time Config
        Dim DefaultLanguage As String = Config.LANGUAGE
        Config = New Config(ApplicationPath)
        Config.LANGUAGE = DefaultLanguage
        Config.Save()

        Dim RegKey As RegistryKey = Registry.LocalMachine.CreateSubKey("Software\Microsoft\Nethesis\" & Application.ProductName)
        Dim SerialKey As String = "NETH-" & Now.Ticks & "-" & Application.ProductVersion

        With RegKey
            .SetValue("Version", Application.ProductVersion)
            .SetValue("Date Installed", Now.ToLongDateString & " " & Now.ToLongTimeString)
            .SetValue("Path", ProgramFileFolder)
            .SetValue("Serial Key", SerialKey)
            .SetValue("Culture", Application.CurrentCulture.ToString)
        End With

        Return True

        'Process.Start(Path)
        'Environment.Is64BitOperatingSystem 
    End Function

    Public Class InstallationInfo
        Inherits TextFileManager

        Sub New(Path As String)
            Me.Path = Path & "\installation.log"
        End Sub
    End Class

    Public Class UsersInfo
        Inherits TextFileManager

        Sub New(Path As String)
            Me.Path = Path & "\users.ini"
        End Sub
    End Class
End Class
