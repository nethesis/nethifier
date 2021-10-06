Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Web
Imports System.Diagnostics.Debug
Imports Nethifier.Helper

Friend NotInheritable Class FRM_CALL_NOTIFICATIONS

    Private ExceptionManager As ExceptionManager
    Private UserLogin As Login
    Private Config As Nethifier.Config
    Private NethDebug As NethDebugger

    Sub New(ByVal UserLogin As Login, ByVal Config As Nethifier.Config, NethDebug As Nethifier.NethDebugger)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.Config = Config
        Me.UserLogin = UserLogin
        Me.NethDebug = NethDebug


        ExceptionManager = New ExceptionManager
    End Sub

    Private Sub LNK_CLEAR_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LNK_CLEAR.LinkClicked
        Me.Close()
    End Sub

    Private Sub CALL_TIMER_Tick(sender As Object, e As EventArgs) Handles CALL_TIMER.Tick
        Opacity = 0
        Me.Close()
    End Sub

    Private Shared Function CertificateHandler(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal SSLerror As SslPolicyErrors) As Boolean
        Return True
    End Function

    Public Function DoCall(PhoneNumber As String) As Boolean

        Dim Url As String = ""
        Url = "https://" & Config.SERVER & "/webrest/astproxy/call"

        Dim proc As New ProcessStartInfo
        With proc
            .UseShellExecute = True
            .WorkingDirectory = Environment.CurrentDirectory
            .FileName = "NethDialer.exe"
            .Arguments = "-username=" & UserLogin.Username & " -token=" & UserLogin.Token & " -phone=" & PhoneNumber & " -url=" & Url
            '.Verb = "runas"
        End With

        Try
            Process.Start(proc)
        Catch ex As Exception
            ' The user refused the elevation. 
            ' Do nothing and return directly ... 
            'MessageBox.Show(ex.Message)
        Finally
            CALL_TIMER.Enabled = True
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
            LBL_CHIAMATA.Text = "Connessione al server in corso..."
            Application.DoEvents()

            ExceptionManager.Write("Dialing number " & PhoneNumber)

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
            Url += "/webrest/astproxy/call"

            ExceptionManager.Write("Creating request...")

            s = WebRequest.Create(Url)

            ExceptionManager.Write("Request created...[" & Url & "]")

            enc = New System.Text.UTF8Encoding()
            postdata = "number=" & HttpUtility.UrlEncode(PhoneNumber) & "&cached=" & HttpUtility.UrlEncode(Now.ToString("yyyyMMdd_HHmmss"))
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

            LBL_CHIAMATA.Text = "Chiamata in corso " & PhoneNumber
            Application.DoEvents()

            ''Token
            ExceptionManager.Write("Recieving response...")
            Dim Respo As String = Trim("" & s.GetResponse.Headers("www-authenticate"))
            ExceptionManager.Write("Response recieved...[" & Respo & "]")
            ExceptionManager.Write("Telephone is ringing, please pick it up...")
        Catch ex As Exception
            Opacity = 0
            Application.DoEvents()
            ex = New Exception("Impossibile effettuare la chiamata." & vbCrLf & "REASON:" & ex.Message & vbCrLf & "URL:" & Url)
            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            DoShowError(ex)
            Dim DebugPath As String = IO.Path.Combine(Application.StartupPath, "debug.log")
            If IO.File.Exists(DebugPath) Then
                My.Computer.FileSystem.WriteAllText(DebugPath, Format(Date.Now(), "yyyy/MM/dd HH:mm:ss") & "- DoCallEx: " & ex.Message, True)
            End If

        End Try
        CALL_TIMER.Enabled = True

    End Function



    Private Sub DoShowError(Ex As Exception)

        ExceptionManager.Write(Ex)
        Dim DebugPath As String = IO.Path.Combine(Application.StartupPath, "debug.log")
        If IO.File.Exists(DebugPath) Then
            My.Computer.FileSystem.WriteAllText(DebugPath, Ex.ToString(), True)
        End If

    End Sub
End Class