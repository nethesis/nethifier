Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text

Public Class Form1
    Private ExceptionManager As ExceptionManager

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ExceptionManager = New ExceptionManager
        'MessageBox.Show(ExceptionManager.Path)

        Me.Opacity = 0
        DoCall()

        Application.Exit()
    End Sub

    Public Function DoCall() As Boolean

        Dim Url As String = ""
        'Gestione Expired HTTPS certificate
        System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf CertificateHandler
        Try
            Dim s As Net.WebRequest  'HttpWebRequest
            Dim enc As UTF8Encoding
            Dim postdata As String
            Dim postdatabytes As Byte()

            Dim Username As String = ""
            Dim Token As String = ""
            Dim Phone As String = ""

            For Each Arg As String In My.Application.CommandLineArgs
                If Arg.ToLower.StartsWith("-username=") Then
                    Username = Arg.Remove(0, "-username=".Length)
                ElseIf Arg.ToLower.StartsWith("-token=") Then
                    Token = Arg.Remove(0, "-token=".Length)
                ElseIf Arg.ToLower.StartsWith("-url=") Then
                    Url = Arg.Remove(0, "-url=".Length)
                ElseIf Arg.ToLower.StartsWith("-phone=") Then
                    Phone = Arg.Remove(0, "-phone=".Length)
                End If
            Next

            If Trim(Phone) = "" OrElse Trim(Username) = "" OrElse Trim(Token) = "" OrElse Trim(Url) = "" Then
                Return False
            End If

            'MessageBox.Show("U=" & Username)
            'MessageBox.Show("T=" & Token)
            'MessageBox.Show("Url=" & Url)
            'MessageBox.Show("P=" & Phone)

            With Me
                .Location = New Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width - .Width, Screen.PrimaryScreen.WorkingArea.Size.Height - .Height)
                .Opacity = 1
            End With

            ''Dim Url As String = "https://" & TXT_SERVER.Text & "/webrest/authentication/login" 'TXT_AUTHEN.Text
            LBL_CHIAMATA.Text = "Connessione al server in corso..."
            Application.DoEvents()

            'ExceptionManager.Write("Dialing number " & PhoneNumber)

            'Url = "https://"
            'With NethDebug
            '    If .IsActive Then
            '        Url += .DEBUG_HTTP_SERVER_ADDRESS
            '        If .DEBUG_HTTP_SERVER_PORT <> "" Then
            '            Url += ":" & .DEBUG_HTTP_SERVER_PORT
            '        End If
            '    Else
            '        Url += Config.SERVER
            '    End If
            'End With
            'Url += "/webrest/astproxy/call"

            'ExceptionManager.Write("Creating request...")

            s = WebRequest.Create(Url)

            'ExceptionManager.Write("Request created...[" & Url & "]")

            Phone = Replace(Replace(Replace(Phone, "-", ""), "/", ""), " ", "")

            enc = New System.Text.UTF8Encoding()
            postdata = "number=" & WebUtility.UrlEncode(Phone) & "&cached=" & WebUtility.UrlEncode(Now.ToString("yyyyMMdd_HHmmss"))
            postdatabytes = enc.GetBytes(postdata)

            'ExceptionManager.Write("Generating data...[" & postdata & "]")

            s.Method = "POST"
            's.Timeout = 30
            s.ContentType = "application/x-www-form-urlencoded"
            s.ContentLength = postdatabytes.Length

            s.Headers.Add("Authorization", Username & ":" & Token.ToLower)
            Using Stream = s.GetRequestStream()
                Stream.Write(postdatabytes, 0, postdatabytes.Length)
            End Using
            Application.DoEvents()

            LBL_CHIAMATA.Text = "Chiamata in corso " & Phone
            Application.DoEvents()

            Dim Respo As String = Trim("" & s.GetResponse.Headers("www-authenticate"))

            Threading.Thread.Sleep(5000)
        Catch ex As Exception
            Opacity = 0
            Application.DoEvents()
            ex = New Exception("Impossibile effettuare la chiamata." & vbCrLf & "REASON:" & ex.Message & vbCrLf & "URL:" & Url)

            ExceptionManager.Write(ex)
            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return True
    End Function

    Private Shared Function CertificateHandler(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal SSLerror As SslPolicyErrors) As Boolean
        Return True
    End Function
End Class
