Imports System.Threading
Imports System.Net
Imports System.Net.Sockets
Imports System.Net.Security
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
'Imports System.IO

Friend Class TCP_CLIENT
    Public errMsg As String
    Public ForcedTLS As Boolean

    ' Define the delegate type
    Public Delegate Sub ClientCallbackDelegate(ByVal bytes() As Byte)
    ' Create Delegate pointer
    Public ClientCallbackObject As ClientCallbackDelegate
    ' Monitor CPU usage
    'Private CPUutil As CpuMonitor

    Private continue_running As Boolean = False
    Private bytes() As Byte
    Private blockSize As UInt16
    Private IP As System.Net.IPAddress
    Private Port As Integer
    Private localAddr As IPAddress
    Private Client As TcpClient
    Private Const PingInterval As Integer = 45000 '45 secondi

    Private Stream As NetworkStream
    Private sslStream As SslStream
    ''Private fileWriter As clsAsyncUnbuffWriter
    'Private fileReader As FileStream
    'Private FileBeingSentPath As String
    'Private weHaveThePuck As Boolean = False
    Private isRunning As Boolean = False
    'Private UserBytesToBeSentAvailable As Boolean = False
    'Private UserBytesToBeSent As New MemoryStream
    'Private UserOutputChannel As Byte
    'Private SystemBytesToBeSentAvailable As Boolean = False
    'Private SystemBytesToBeSent() As Byte
    'Private SystemOutputChannel As Byte
    'Private SendingFile As Boolean = False
    'Private ReceivingFile As Boolean = False
    'Private IncomingFileName As String
    'Private IncomingFileSize As Int64 = 0
    'Private outgoingFileSize As UInt64 = 0
    'Private outgoingFileName As String
    'Private fileBytesRecieved As Int64 = 0
    'Private filebytesSent As Int64 = 0
    'Private bytesSentThisSecond As Int32 = 0
    'Private bytesReceivedThisSecond As Int32 = 0
    'Private mbpsOneSecondAverage() As Int32
    'Private ReceivedFilesFolder As String = Application.StartupPath & "\ReceivedFiles"
    'Private userName As String
    'Private password As String

    'Private mbpsSyncObject As New AutoResetEvent(False)

    Private Msg As MessageHelper '= New MessageHelper

    Private Function StrToByteArray(ByVal text As String) As Byte()
        Dim encoding As New System.Text.UTF8Encoding()
        StrToByteArray = encoding.GetBytes(text)
    End Function

    Private Function BytesToString(ByVal data() As Byte) As String
        Dim enc As New System.Text.UTF8Encoding()
        BytesToString = enc.GetString(data)
    End Function

    Public Function isClientRunning() As Boolean
        Return isRunning
    End Function

    Public Sub New(ByRef callbackMethod As ClientCallbackDelegate)
        blockSize = 10024
        Msg = New MessageHelper(New Config)
        ' Initialize the delegate variable to point to the user's callback method.
        ClientCallbackObject = callbackMethod
    End Sub

    Public Sub Connect(ByVal IP_Address As String, ByVal prt As Integer, Mode As String)
        Try
            Dim IP_Add As IPAddress '= Dns.GetHostEntry(IP_Address).AddressList(0)
            Try
                Dim addresses() As System.Net.IPAddress = Dns.GetHostEntry(IP_Address).AddressList

                If addresses.Length > 0 Then
                    IP_Add = addresses(0)
                Else
                    IP_Add = IPAddress.Parse(IP_Address)
                End If
                If Not IsNothing(IP_Add) AndAlso IP_Add.ToString.IndexOf(":", StringComparison.Ordinal) >= 0 Then
                    'IPV6
                    ' Find an IpV4 address
                    Dim Found As Boolean = False
                    'Dim addresses() As System.Net.IPAddress = Dns.GetHostEntry(IP_Address).AddressList '
                    For Each IP_Add In addresses
                        If IP_Add.ToString.Contains(".") Then
                            Found = True
                            Exit For
                        End If
                    Next

                    If Not Found Then
                        MessageBox.Show("No such IP found.")
                        Exit Sub
                    End If
                End If
            Catch
                IP_Add = IPAddress.Parse(IP_Address)
            End Try

            Dim m_Remote As IPEndPoint = New IPEndPoint(IP_Add, prt)

            Port = m_Remote.Port
            IP = m_Remote.Address  'System.Net.IPAddress.Parse(IP_Address)
            continue_running = True

            If (Mode = "TLS") Then
                'TLS
                Dim clientCommunicationThread As New Thread(AddressOf RunTLS)
                clientCommunicationThread.Name = "ClientCommunication"
                clientCommunicationThread.Start()
            Else
                'PLAIN
                Dim clientCommunicationThread As New Thread(AddressOf Run)
                clientCommunicationThread.Name = "ClientCommunication"
                clientCommunicationThread.Start()
            End If
        Catch ex As Exception
            WriteError(ex)
        End Try
    End Sub

    Public Sub Disconnect()
        If Not IsNothing(Client) Then
            If Client.Client.Connected Then
                Client.Client.Close()
            End If
        End If
    End Sub

    Public Function SendBytes(ByVal OutGoingMessage() As Byte) As Boolean

        If Not (sslStream Is Nothing) Then
            sslStream.Write(OutGoingMessage, 0, OutGoingMessage.Length)
            sslStream.Flush()
        ElseIf Client.Client.Connected Then
            Stream.Write(OutGoingMessage, 0, OutGoingMessage.Length)
            Stream.Flush()
        End If

        Application.DoEvents()

    End Function

    Public Function SendBytes(ByVal Message As String) As Boolean
        SendBytes(StrToByteArray(Message))
    End Function

    Private Function RcvBytes(ByVal data() As Byte) As Boolean

        Try
            ClientCallbackObject(data)
        Catch ex As Exception
            RcvBytes = False
            ' An unexpected error.
            'Debug.WriteLine("Unexpected error in Client\RcvBytes: " & ex.Message)
        End Try

    End Function

    Private Sub SystemMessage(ByVal MsgText As String)
        RcvBytes(StrToByteArray(MsgText))
    End Sub

    Private WithEvents PingServer As System.Windows.Forms.Timer
    Private WithEvents WaitingForServerReply As System.Windows.Forms.Timer
    Private ServerResponseExpired As Boolean

    Private Sub Run()

        Dim puck(1) As Byte : puck(0) = 0
        Dim theBuffer(blockSize - 1) As Byte
        Dim tmp(1) As Byte
        Dim dataChannel As Integer = 0
        Dim packetSize As UShort = 0
        'Dim bytesread As Integer
        Dim userOrSystemSwitcher As Integer = 0
        Dim PercentUsage As Short = -1

        WaitingForServerReply = New System.Windows.Forms.Timer
        With WaitingForServerReply
            .Interval = 5000 '5 seconds '''60000 '
            .Enabled = False
        End With

        Try

            Client = New TcpClient

            Client.Connect(IP, Port)

            Stream = Client.GetStream()
            Stream.ReadTimeout = PingInterval

            Client.NoDelay = True

            ' Pass a message up to the user about our status.
            SystemMessage("SYS:" & Msg.GetMessage("STAT_001"))
            isRunning = True

            ' Start the communication loop
            Do

                'Dim Buffer(10024) As Byte 'WIN7
                Dim Buffer(65537) As Byte 'WIN8
                Dim BufferSize As Integer

                'If theClientIsStopping() Then Exit Do
                BufferSize = Client.ReceiveBufferSize

                'MessageBox.Show(BufferSize.ToString)
                'MessageBox.Show(Buffer.Length.ToString)

                Stream.Read(Buffer, 0, BufferSize)

                If CLng(Buffer.GetValue(0)) > 0 Then
                    RcvBytes(Buffer)

                Else

                    If ServerResponseExpired Then

                        'Client.Close()
                        ServerResponseExpired = False
                        WaitingForServerReply.Enabled = False

                        Throw New System.Net.Sockets.SocketException(System.Net.Sockets.SocketError.NoData)
                        'SystemMessage("SYS:" & vbCrLf & "ERR_MSG: " & vbCrLf & "ERR_NUM: 10054")
                        'Exit Do
                    End If
                    WaitingForServerReply.Enabled = True

                End If

                Application.DoEvents()
            Loop
        Catch ex As Exception

            isRunning = False

            If Not IsNothing(ex.InnerException) Then
                WriteError(ex.InnerException)
            Else
                WriteError(ex)
            End If
        End Try

        Try
            'CPUutil.StopWatcher()
            Client.Client.Close()
            SystemMessage("SYS:" & Msg.GetMessage("STAT_002"))
        Catch ex As Exception
            If Not IsNothing(ex.InnerException) Then
                WriteError(ex.InnerException)
            Else
                WriteError(ex)
            End If
        End Try

        isRunning = False

    End Sub
    Private Sub RunTLS()
        Dim Conf As Config = New Config

        Dim puck(1) As Byte : puck(0) = 0
        Dim theBuffer(blockSize - 1) As Byte
        Dim tmp(1) As Byte
        Dim dataChannel As Integer = 0
        Dim packetSize As UShort = 0
        'Dim bytesread As Integer
        Dim userOrSystemSwitcher As Integer = 0
        Dim PercentUsage As Short = -1

        WaitingForServerReply = New System.Windows.Forms.Timer
        With WaitingForServerReply
            .Interval = 5000 '5 seconds '''60000 '
            .Enabled = False
        End With

        Try

            Client = New TcpClient

            Client.Connect(IP, Port)
            Client.NoDelay = True
            ' Connection Accepted.
            ' PLAIN
            'Stream = Client.GetStream()
            'Stream.ReadTimeout = PingInterval

            'TLS
            Dim callback As RemoteCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CertificateHandler)
            Dim mysslStream As SslStream = New SslStream(Client.GetStream(), False, callback)
            mysslStream.AuthenticateAsClient("Nethifier")

            If mysslStream.IsAuthenticated Then
                SystemMessage("SYS:" & Msg.GetMessage("SSL_AUTHENTICATED"))
                sslStream = mysslStream
            End If

                ' Pass a message up to the user about our status.
                SystemMessage("SYS:" & Msg.GetMessage("STAT_001"))
            isRunning = True

            ' Start the communication loop
            Do

                'Dim Buffer(10024) As Byte 'WIN7
                Dim Buffer(65537) As Byte 'WIN8
                Dim BufferSize As Integer

                'If theClientIsStopping() Then Exit Do
                BufferSize = Client.ReceiveBufferSize

                'MessageBox.Show(BufferSize.ToString)
                'MessageBox.Show(Buffer.Length.ToString)

                sslStream.Read(Buffer, 0, BufferSize)

                If CLng(Buffer.GetValue(0)) > 0 Then
                    RcvBytes(Buffer)
                    Debug.WriteLine(Replace(System.Text.Encoding.UTF8.GetString(Buffer), Chr(0), ""))
                Else

                    If ServerResponseExpired Then

                        'Client.Close()
                        ServerResponseExpired = False
                        WaitingForServerReply.Enabled = False

                        Throw New System.Net.Sockets.SocketException(System.Net.Sockets.SocketError.NoData)
                        'SystemMessage("SYS:" & vbCrLf & "ERR_MSG: " & vbCrLf & "ERR_NUM: 10054")
                        'Exit Do
                    End If
                    WaitingForServerReply.Enabled = True

                End If
                Application.DoEvents()
            Loop
        Catch ex As Exception

            isRunning = False

            If Not IsNothing(ex.InnerException) Then
                WriteError(ex.InnerException)
            Else
                WriteError(ex)
            End If
        End Try

        Try
            'CPUutil.StopWatcher()
            Client.Client.Close()
            SystemMessage("SYS:" & Msg.GetMessage("STAT_002"))
        Catch ex As Exception
            If Not IsNothing(ex.InnerException) Then
                WriteError(ex.InnerException)
            Else
                WriteError(ex)
            End If
        End Try

        isRunning = False

    End Sub
    Private Shared Function CertificateHandler(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal SSLerror As SslPolicyErrors) As Boolean
        Return True
    End Function

    Private Sub WriteError(Ex As Exception)
        Dim Message As String = Ex.Message
        Dim Number As Integer = 0

        Select Case Ex.GetType.ToString
            Case Is = "System.Net.Sockets.SocketException"
                With DirectCast(Ex, SocketException)
                    Message = .Message & " (" & .ErrorCode & ")"
                    Number = .ErrorCode
                End With
        End Select
        SystemMessage("SYS:" & vbCrLf & "ERR_MSG: " & Message & vbCrLf & "ERR_NUM: " & Number)
    End Sub

    Private Sub WaitingForServerReply_Tick(sender As Object, e As EventArgs) Handles WaitingForServerReply.Tick
        ServerResponseExpired = True
    End Sub
End Class