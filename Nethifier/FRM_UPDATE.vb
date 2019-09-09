Friend Class FRM_UPDATE

    Private WithEvents NethUpdate As Nethifier.Update
    Private UserLogin As Login
    Private Info As UpdateInfo

    Private Config As Nethifier.Config
    Private UpdateDestination As String = ""

    Private Msg As Nethifier.MessageHelper

    Sub New(LG As Login, Info As UpdateInfo, Config As Nethifier.Config)

        ' This call is required by the designer.
        InitializeComponent()

        UserLogin = LG
        Me.Info = Info
        Me.Config = Config

        Msg = New MessageHelper(Config)
        ' Add any initialization after the InitializeComponent() call.
        NethUpdate = New Nethifier.Update(UserLogin, Config)
    End Sub

    Private Sub BUT_CANCEL_Click(sender As Object, e As EventArgs) Handles BUT_CANCEL.Click
        If NethUpdate.InProgress Then
            NethUpdate.Cancel()
        End If
    End Sub

    Private Sub TMR_Tick(sender As Object, e As EventArgs) Handles TMR.Tick
        'Do Update
        TMR.Stop()
        If Helper.IsRunAsAdmin Then
            Dim RegKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\Microsoft\Nethesis\" & Application.ProductName)

            If IsNothing(RegKey) Then
                MessageBox.Show(Msg.GetMessage("AGG_003"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Me.Dispose()
                Exit Sub
            End If

            UpdateDestination = IO.Path.Combine(RegKey.GetValue("Path").ToString, "update")
            NethUpdate.Download("token=" & Info.Token & "&check=download&download=" & Info.Version.ToString, UpdateDestination)
        Else
            Me.Dispose()
        End If
        'End If
    End Sub

    Private Sub FRM_UPDATE_Load(sender As Object, e As EventArgs) Handles Me.Load
        LBL_DOWNLOAD.Text = Msg.GetMessage("AGG_007")

        Application.DoEvents()

        NethUpdate = New Nethifier.Update(UserLogin, Config)
    End Sub

    Private Sub NethUpdate_OnCancel(ByRef Cancel As Boolean) Handles NethUpdate.OnCancel
        If MessageBox.Show(Msg.GetMessage("AGG_004"), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Cancel = True
        End If
    End Sub

    Private Sub NethUpdate_OnEnd(Reason As Nethifier.Update.EndReason, ByVal DownloadedFileName As String, ByVal Exception As Exception) Handles NethUpdate.OnEnd

        Dim ExceptionManager As ExceptionManager = New ExceptionManager

        If Reason = Nethifier.Update.EndReason.Downloaded Then

            LBL_DOWNLOAD.Text = Msg.GetMessage("AGG_005")

            Try
                Dim proc As New ProcessStartInfo
                With proc
                    .UseShellExecute = True
                    .WorkingDirectory = UpdateDestination
                    .FileName = DownloadedFileName
                    '.Arguments = "-u"
                    .Verb = "runas"
                End With

                Application.ExitThread()
                Process.Start(proc)

                Me.Dispose()
            Catch ex As Exception
                Me.Dispose()

                ExceptionManager.Write(ex)
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        ElseIf Reason = Nethifier.Update.EndReason.Error Then
            Me.Dispose()

            If Not IsNothing(Exception) Then
                ExceptionManager.Write(Exception)
                MessageBox.Show(Exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            'Application.ExitThread()
            Application.Exit()
        End If
    End Sub

    Private Sub NethUpdate_OnProgress(ByVal BytesToDownload As Long, ByVal BytesDowloadedSoFar As Long, ByVal BytesDownloaded As Integer) Handles NethUpdate.OnProgress
        PROGRESS.Maximum = CInt(BytesToDownload)
        PROGRESS.Value += BytesDownloaded
        'Nethifier.Update.FormatFileSize(BytesDowloadedSoFar)
        'Nethifier.Update.FormatFileSize(BytesToDownload)

        Dim MS As String = Msg.GetMessage("AGG_006")
        LBL_DOWNLOAD.Text = MS.Replace("#DOWNLOAD_SO_FAR#", Nethifier.Update.FormatFileSize(BytesDowloadedSoFar)).Replace("#TOTAL_BYTES#", Nethifier.Update.FormatFileSize(BytesToDownload))
    End Sub
End Class