Imports Microsoft.Win32.SafeHandles
Imports System.Runtime.InteropServices
Imports System.Security.Principal
Imports System.ComponentModel

Public Class FRM_INSTALLER

    Friend Action As CLS_INSTALLER.Action

    Private Msg As MessageHelper
    Friend Property Messages As MessageHelper
        Get
            Return Msg
        End Get
        Set(value As MessageHelper)
            Msg = value
        End Set
    End Property

    Private Sub FRM_INSTALLER_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = Application.ProductName
        Me.Width = 453

        Msg = New MessageHelper(New Config)

        CHK_ALL_USERS.Text = Msg.GetMessage("INS_009")
        CHK_DESKTOP.Text = Msg.GetMessage("INS_010")
        CHK_STARTUP.Text = Msg.GetMessage("INS_011")
        CHK_ELIMINA_CONFIG.Text = Msg.GetMessage("INS_015")
        CHK_RUN_AFTER_INSTALL.Text = Msg.GetMessage("INS_020")

        BUT_INSTALL.Text = Msg.GetMessage("INS_012")
        BUT_CANCEL.Text = Msg.GetMessage("INS_013")

        If Action = CLS_INSTALLER.Action.Install Then
            BUT_INSTALL.Text = Msg.GetMessage("INS_012")

            TXT_FOLDER.Text = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\" & Application.ProductName
            LBL_PROGRAMMA.Text = Msg.GetMessage("INS_008")

            PAN_INSTALL.Visible = True
            PAN_UNINSTALL.Visible = False

            Me.Height = 217
        Else
            BUT_INSTALL.Text = Msg.GetMessage("INS_014")

            PAN_INSTALL.Visible = False
            PAN_UNINSTALL.Visible = True

            Me.Height = 118
        End If

        If My.Application.CommandLineArgs.Contains("-m") Then
            CHK_ALL_USERS.Enabled = False
        End If

    End Sub

    Private Sub BUT_INSTALL_Click(sender As Object, e As EventArgs) Handles BUT_INSTALL.Click

        Dim Installer As New CLS_INSTALLER

        If Action = CLS_INSTALLER.Action.Install Then

            Dim UserAppFolder As String = ""
            Dim ProgFolder As String = TXT_FOLDER.Text

            If Installer.Install(CHK_ALL_USERS.Checked, CHK_STARTUP.Checked, CHK_DESKTOP.Checked, ProgFolder, UserAppFolder) Then
                MessageBox.Show(Msg.GetMessage("INS_003"), Msg.GetMessage("INS_012"), MessageBoxButtons.OK, MessageBoxIcon.Information)
                'Application.Exit()

                If CHK_RUN_AFTER_INSTALL.Checked Then
                    'Do Execute right away
                    'Dim Path As String = TXT_FOLDER.Text
                    'If Not Path.EndsWith("\") Then
                    '    Path += "\"
                    'End If
                    'Path += Application.ExecutablePath.Replace(Application.StartupPath & "\", "")

                    Dim proc As New ProcessStartInfo
                    With proc
                        .UseShellExecute = True
                        .WorkingDirectory = ProgFolder 'Environment.CurrentDirectory
                        .FileName = Application.ExecutablePath.Replace(Application.StartupPath & "\", "") 'Path
                        '.Arguments = "-e"
                        .Arguments = "-r"
                        '.Verb = "runas"
                    End With

                    Try
                        Application.Exit()
                        Process.Start(proc)
                    Catch ex As Exception
                        ' The user refused the elevation. 
                        ' Do nothing and return directly ... 
                        MessageBox.Show(ex.Message)
                    End Try
                Else
                    Application.Exit()
                End If

                Exit Sub
            Else
                MessageBox.Show(Msg.GetMessage("INS_004"), Msg.GetMessage("INS_012"), MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            Dim Msg005 As String = Msg.GetMessage("INS_005")
            Dim Msg006 As String = Msg.GetMessage("INS_006")
            Dim Msg014 As String = Msg.GetMessage("INS_014")

            If Installer.Uninstall(CHK_ELIMINA_CONFIG.Checked) Then
                MessageBox.Show(Msg005, "Uninstall", MessageBoxButtons.OK, MessageBoxIcon.Information)
                'Process.Start("cmd.exe", "/C choice /C Y /N /D Y /T 3 & Del """ + Application.ExecutablePath & """")
                Process.Start(UninstallBatchFile.Path)


                'Process.Start("cmd.exe", "/C choice /C Y /N /D Y /T 3 & RD """ + Application.StartupPath & """")
                ''For XP
                'Process.Start("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del " & Application.ExecutablePath)
                Application.Exit()
            Else
                MessageBox.Show(Msg006, Msg014, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If

    End Sub

    Private Sub BUT_CANCEL_Click(sender As Object, e As EventArgs) Handles BUT_CANCEL.Click
        Application.Exit()
    End Sub

    Private Sub BUT_FOLDER_Click(sender As Object, e As EventArgs) Handles BUT_FOLDER.Click
        With FOLDER
            .Description = Msg.GetMessage("INS_007")
            .ShowNewFolderButton = True
            If .ShowDialog() = Windows.Forms.DialogResult.OK Then
                TXT_FOLDER.Text = .SelectedPath()
            End If
        End With
    End Sub

End Class
