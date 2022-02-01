Imports Nethifier.Helper
Imports Microsoft.Win32
Imports System.IO
Public NotInheritable Class FRM_SPLASH

    'NICK Multimedia 

    'TODO: This form can easily be set as the splash screen for the application by going to the "Application" tab
    '  of the Project Designer ("Properties" under the "Project" menu).

    'Private InstallInfo As CLS_INSTALLER.InstallationInfo = Nothing

    Private Msg As MessageHelper
    Private Config As Config

    Private TokenStream As FileStream

    Private Sub FRM_SPLASH_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Set up the dialog text at runtime according to the application's assembly information.  
        'Dim DebugPath As String = IO.Path.Combine(Application.StartupPath, "debug.log")
        Dim DebugPath As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).ToString(), "nethifier_debug.log")
        'My.Computer.FileSystem.WriteAllText(DebugPath, Format(Date.Now(), "yyyy/MM/dd HH:mm:ss") & "TEST" & vbCrLf, True)
        If IO.File.Exists(DebugPath) Then
            Dim arguments As String() = Environment.GetCommandLineArgs()
            My.Computer.FileSystem.WriteAllText(DebugPath, Format(Date.Now(), "yyyy/MM/dd HH:mm:ss") & "- Execution with args: " & String.Join(", ", arguments).Replace(Environment.NewLine, " ") & Environment.NewLine, True)
        End If

        Dim p() As Process
        p = Process.GetProcessesByName("Nethifier")
        If p.Length > 1 And My.Application.CommandLineArgs.Contains("-e") Then
            MsgBox("Un'altra istanza di Nethifier è già in esecuzione.")
            Application.Exit()
            End
        End If

        Dim RunningProcess As Process = Nothing

        Try
            Me.BUT_EXIT.Image = Nethifier.My.Resources.Resources.close
        Catch ex As Exception
            Console.WriteLine("No close img")
        End Try

        If Not My.Computer.FileSystem.FileExists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() & "\" & Application.ProductName & "\config.ini") And Not My.Application.CommandLineArgs.Contains("-e") Then
            RunAs("-e")
            Application.Exit()
        End If

        Opacity = 100

        Dim DoAdminExec As Boolean = False
        If Not My.Application.CommandLineArgs.Contains("-u") AndAlso Not My.Application.CommandLineArgs.Contains("-i") Then
            'Check if this is the first time we execute the program
            DoAdminExec = DoAdminCheck()
        Else
            If Not Helper.IsRunAsAdmin Then
                RunAs("-u")
                Exit Sub
            End If
        End If

        If Not DoAdminExec Then
            If Not My.Application.CommandLineArgs.Contains("-r") Then
                ReloadSettings()
            End If

            If Not Debugger.IsAttached Then
                BUT_INSTALL.Visible = True
                RunningProcess = Nothing
                If Helper.IsRunAsAdmin Then
                    Dim LM As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", True)
                    Dim APP_NAME As String = Application.ExecutablePath.Replace(Application.StartupPath & "\", "")

                    Dim Names As String() = LM.GetValueNames
                    Dim Found As Boolean = False
                    For I As Integer = 0 To Names.Length - 1
                        If Trim(Names(I)).ToLower = APP_NAME.ToLower Then
                            Found = True
                            Exit For
                        End If
                    Next
                    If Not Found Then
                        Registry.SetValue("HKEY_LOCAL_MACHINE\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", APP_NAME, "9999", RegistryValueKind.DWord)
                    End If
                End If

                If My.Application.CommandLineArgs.Contains("-i") Then
                    'Do Install

                    Dim Installer As New FRM_INSTALLER
                    Installer.Action = CLS_INSTALLER.Action.Install
                    Installer.Show()

                    Me.Dispose()
                    Exit Sub
                ElseIf My.Application.CommandLineArgs.Contains("-u") Then
                    'Do Uninstall
                    If Helper.IsApplicationRunning(Application.ExecutablePath, Process.GetCurrentProcess, RunningProcess) Then
                        If MessageBox.Show(Msg.GetMessage("INS_002"), Msg.GetMessage("INS_014"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                            RunningProcess.Kill()
                            Do
                                If Helper.IsApplicationRunning(Application.ExecutablePath, Process.GetCurrentProcess, RunningProcess) Then
                                    RunningProcess.Kill()
                                Else
                                    Exit Do
                                End If
                            Loop
                        Else
                            Application.Exit()
                            Exit Sub
                        End If
                    End If

                    If Helper.IsRunAsAdmin Then
                        Dim Uninstaller As New FRM_INSTALLER
                        Uninstaller.Action = CLS_INSTALLER.Action.Uninstall
                        Uninstaller.Show()

                        Try
                            Me.BUT_EXIT.Image.Dispose()
                        Catch ex As Exception
                            Console.WriteLine("No exit img")
                        End Try

                        Me.Dispose()
                    Else
                        Dim proc As New ProcessStartInfo
                        With proc
                            .UseShellExecute = True
                            .WorkingDirectory = Environment.CurrentDirectory
                            .FileName = Application.ExecutablePath
                            .Arguments = "-u"
                            .Verb = "runas"
                        End With

                        Try
                            Application.Exit()
                            Process.Start(proc)
                        Catch ex As Exception
                            ' The user refused the elevation. 
                            ' Do nothing and return directly ... 
                            MessageBox.Show(ex.Message)
                        End Try
                    End If
                    Exit Sub
                ElseIf My.Application.CommandLineArgs.Contains("-e") Or My.Application.CommandLineArgs.Contains("-R") Then

                    'Do Execute
                    Dim Config As New FRM_CONFIG
                    Config.Show()
                    Me.Dispose()

                    Exit Sub
                ElseIf My.Application.CommandLineArgs.Contains("-r") Then

                    'Reloaded
                    If Helper.IsRunAsAdmin Then
                        ReloadSettings()

                        'Do Execute
                        Dim Config As New FRM_CONFIG
                        Config.Show()
                        Me.Dispose()
                    End If
                    Exit Sub
                End If

                ' Get and display the process elevation information (IsProcessElevated)  
                ' and integrity level (GetProcessIntegrityLevel). The information is not  
                ' available on operating systems prior to Windows Vista. 
                If (Environment.OSVersion.Version.Major >= 6) Then
                    ' Running Windows Vista or later (major version >= 6).  

                    ' Update the Self-elevate button to show the UAC shield icon on  
                    ' the UI if the process is not elevated. 

                    Try
                        Me.BUT_INSTALL.FlatStyle = FlatStyle.System
                        Dim IntP As IntPtr
                        If Helper.IsProcessElevated Then
                            IntP = New IntPtr(0)
                        Else
                            IntP = New IntPtr(1)
                        End If
                        NativeMethods.SendMessage(Me.BUT_INSTALL.Handle, NativeMethods.BCM_SETSHIELD, 0, IntP)
                    Catch ex As Exception
                        MessageBox.Show(ex.Message, "An error occurred in IsProcessElevated", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                Else
                End If
            Else
                BUT_INSTALL.Visible = False
            End If

            'TODO: Customize the application's assembly information in the "Application" pane of the project 
            '  properties dialog (under the "Project" menu).

            'Application title
            If My.Application.Info.Title <> "" Then
                ApplicationTitle.Text = Application.ProductName  'My.Application.Info.Title
            Else
                'If the application title is missing, use the application name, without the extension
                ApplicationTitle.Text = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
            End If

            'Dim VersionInfo As Version = New Version
            'With VersionInfo
            ' 'Version.Text = String.Format("Version {0}", .Major & "." & .Minor & "." & .Build)
            'Version.Text = My.Application.Info.Version.ToString()
            'Copyright.Text = .Copyright
            'End With

            Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor)
            'Copyright info
            Copyright.Text = My.Application.Info.Copyright

        Else
            'MessageBox.Show("RELOADED")
            RunAs("-r")
        End If

    End Sub

    Private Sub BUT_INSTALL_Click(sender As Object, e As EventArgs) Handles BUT_INSTALL.Click
        If Helper.IsRunAsAdmin Then
            Dim Frm As New FRM_INSTALLER
            Frm.Messages = New MessageHelper(Config)
            Frm.Action = CLS_INSTALLER.Action.Install
            Frm.Show()

            Me.Dispose()
        Else
            RunAs("-i")
        End If

    End Sub

    Private Sub ReloadSettings()
        If (My.Application.CommandLineArgs.Contains("-r") And My.Computer.FileSystem.FileExists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() & "\" & Application.ProductName & "\config.ini")) Then
            Config = New Config(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName)
        Else
            Config = New Config
        End If
        Msg = New MessageHelper(Config)

        With CMB_LANGUAGES
            .Items.Clear()
            For Each L As String In Config.Languages.Values
                .Items.Add(L)
                If L.ToLower.Trim = Config.LANGUAGE.ToLower.Trim Then
                    .SelectedIndex = .Items.Count - 1
                End If
            Next
        End With
        ChangeCaption()
    End Sub

    Private Function DoAdminCheck() As Boolean
        'Do first execution check
        If My.Application.CommandLineArgs.Count = 0 Then
            RunAs("-m")
            Return False
        End If

        Dim ApplicationPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & Application.ProductName
        Dim DoAdminExec As Boolean = False

        'If Not Directory.Exists(ApplicationPath) Then
        'DoAdminExec = True
        'Directory.CreateDirectory(ApplicationPath)
        'End If
        'If Not Directory.Exists(ApplicationPath & "\languages") Then
        ' DoAdminExec = True
        'Directory.CreateDirectory(ApplicationPath & "\languages")
        'End If
        'If Not Directory.Exists(ApplicationPath & "\resources") Then
        'DoAdminExec = True
        'Directory.CreateDirectory(ApplicationPath & "\resources")
        'End If
        Return DoAdminExec
    End Function

    Private Sub RunAs(Arg As String)

    End Sub

    Private Sub BUT_RUN_Click(sender As Object, e As EventArgs) Handles BUT_RUN.Click
        'Do Execute
        'Dim Uninstaller As New FRM_INSTALLER
        'Uninstaller.Action = CLS_INSTALLER.Action.Uninstall
        'Uninstaller.Show()

        Dim Frm As New FRM_CONFIG
        Frm.Show()

        Me.Dispose()
    End Sub

    Private Sub BUT_EXIT_Click(sender As Object, e As EventArgs) Handles BUT_EXIT.Click
        Application.Exit()
    End Sub

    Private Sub CMB_LANGUAGES_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CMB_LANGUAGES.SelectedIndexChanged
        Config.LANGUAGE = CStr(CMB_LANGUAGES.SelectedItem)
        Config.Save()
        Msg = New MessageHelper(Config)

        ChangeCaption()
    End Sub

    Private Sub ChangeCaption()
        BUT_INSTALL.Text = Msg.GetMessage("INS_012")
        BUT_RUN.Text = Msg.GetMessage("INS_016")
        LBL_LANG.Text = Msg.GetMessage("INS_017")
    End Sub

    Private Sub MainLayoutPanel_Paint(sender As Object, e As PaintEventArgs) Handles MainLayoutPanel.Paint

    End Sub

    Private Sub ApplicationTitle_Click(sender As Object, e As EventArgs) Handles ApplicationTitle.Click

    End Sub
End Class
