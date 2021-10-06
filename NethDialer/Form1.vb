Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Web
Imports System.IO
Imports System.Diagnostics.Debug
Imports System.Security.Cryptography

Public Class Form1
    Private ExceptionManager As ExceptionManager

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
        Return (0)
    End Function

    Public Class CookieAwareWebClient
        Inherits WebClient

        Private cc As New CookieContainer()
        Private lastPage As String

        Protected Overrides Function GetWebRequest(ByVal address As System.Uri) As System.Net.WebRequest
            Dim R = MyBase.GetWebRequest(address)
            If TypeOf R Is HttpWebRequest Then
                With DirectCast(R, HttpWebRequest)
                    .CookieContainer = cc
                    .AllowAutoRedirect = False
                    If Not lastPage Is Nothing Then
                        .Referer = lastPage
                    End If
                End With
            End If
            lastPage = address.ToString()
            Return R
        End Function
    End Class
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ExceptionManager = New ExceptionManager
        'MessageBox.Show(ExceptionManager.Path)
        Dim C2CURL As String = ""

        Me.Opacity = 0
        
        For Each Arg As String In My.Application.CommandLineArgs
            If Arg.ToLower.StartsWith("-cloud2call=") Then
                C2CURL = Arg.Remove(0, "-cloud2call=".Length)
            End If
        Next
        If C2CURL.Length Then
            Mylog("execute:NethDialer", "to cloud2call")
            DoCloud2call(C2CURL)
        Else
            Mylog("execute:NethDialer", "to DoCall")
            DoCall()
        End If

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

            Phone = Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Phone, "-", ""), "/", ""), " ", ""), "(", ""), ")", ""), "[", ""), "]", ""), "{", ""), "}", "")

            enc = New System.Text.UTF8Encoding()
            postdata = "number=" & WebUtility.UrlEncode(Phone) & "&cached=" & WebUtility.UrlEncode(Now.ToString("yyyyMMdd_HHmmss"))
            postdatabytes = enc.GetBytes(postdata)

            Dim DebugPath As String = IO.Path.Combine(Application.StartupPath, "debug.log")
            If IO.File.Exists(DebugPath) Then
                My.Computer.FileSystem.WriteAllText(DebugPath, Format(Date.Now(), "yyyy/MM/dd HH:mm:ss") & "- DoCall: " & postdata, True)
            End If

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
            Dim DebugPath As String = IO.Path.Combine(Application.StartupPath, "debug.log")
            If IO.File.Exists(DebugPath) Then
                My.Computer.FileSystem.WriteAllText(DebugPath, Format(Date.Now(), "yyyy/MM/dd HH:mm:ss") & "- DoCallEx: " & ex.ToString(), True)
            End If

            ExceptionManager.Write(ex)
            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return True
    End Function

    Public Function DoCloud2call(ByVal URL As String)
        Dim UserName As String
        Dim Password As String
        Dim UserPwd As String
        Dim SubUrl As String
        Dim SubLen As Int16

        SubLen = ((URL.StartsWith("http://") * 7) + (URL.StartsWith("https://") * 8)) * -1
        SubUrl = URL.Substring(SubLen, URL.LastIndexOf("/") - SubLen)

        UserPwd = SubUrl.Substring(0, SubUrl.LastIndexOf("@"))
        UserName = UserPwd.Substring(0, UserPwd.IndexOf(":"))
        Password = UserPwd.Substring(UserPwd.IndexOf(":") + 1)
        Mylog("execute:cloud2call", URL.Replace(Password, "****").ToString())

        'Dim webClient As New System.Net.WebClient
        Dim webClient As New CookieAwareWebClient
        ''webClient.Headers.Clear()
        ''webClient.CachePolicy = New System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)
        ' webClient.Credentials = New NetworkCredential(UserName, Password)
        Dim credentials As String

        credentials = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(UserName + ":" + Password))
        'webClient.Headers.Add("Authorization", String.Format("Basic {0}", credentials))
        webClient.Headers.Add("Accept-Encoding", "")
        webClient.Headers.Add("User-Agent", Guid.NewGuid().ToString)
        webClient.Encoding = System.Text.Encoding.UTF8
        If Not (webClient.IsBusy) Then
            Dim result As String
            Try
                'webClient.Headers.Add("authorization", String.Format("Basic {0}", credentials))
                'webClient.AllowAutoRedirect = False
                webClient.Headers.Add(HttpRequestHeader.Authorization, String.Format("Basic {0}", credentials))
                result = webClient.DownloadString(URL)
                Mylog("ResultCall", result.ToString())
            Catch e As WebException
                Try
                    If InStr(e.Message, "401", CompareMethod.Text) Or InStr(e.Message, "403", CompareMethod.Text) Then
                        'webClient.Headers.Add(HttpRequestHeader.Authorization, String.Format("Basic {0}", credentials))
                        result = webClient.DownloadString(URL)
                        Mylog("ResultCall", result.ToString())
                    Else
                        Mylog("ResultCall", e.Message.ToString())
                        MsgBox(e.Message.ToString(), 0, "ResultCall")
                    End If
                Catch ex As Exception
                    Mylog("ResultCall", ex.Message.ToString())
                    MsgBox(ex.Message.ToString(), 0, "ResultCall")
                End Try
            End Try
            '   'webClient.Dispose()
        Else
            Mylog("ResultCall", "Webclient Busy")
        End If
        Return(true)
    End Function
    Private Shared Function CertificateHandler(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal SSLerror As SslPolicyErrors) As Boolean
        Return True
    End Function

    Private Sub LBL_CHIAMATA_Click(sender As Object, e As EventArgs) Handles LBL_CHIAMATA.Click

    End Sub
End Class

