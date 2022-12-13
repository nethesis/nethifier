Imports System.IO
Imports LuxaforSharp

Public Class Form1
    Private ExceptionManager As ExceptionManager
    Private WithEvents Watch As System.IO.FileSystemWatcher
    Private Device As IDevice


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ExceptionManager = New ExceptionManager
        Me.Opacity = 0
        StartWatching()
    End Sub

    Private Function GetLED() As IDevice
        Try

            If IsNothing(Device) Then
                Dim List As IDeviceList = New DeviceList
                List.Scan()
                Device = List.First

            End If

        Catch ex As Exception

        End Try

        If IsNothing(Device) Then
            Threading.Thread.Sleep(5000)
        End If

        Return Device
    End Function

    Private Sub RingBlinking()

        With GetLED()
            .Blink(LedTarget.All, New Color(255, 69, 0), 5, 0, 0)
            Threading.Thread.Sleep(1000)
            .SetColor(LedTarget.All, New Color(0, 0, 0))
            Threading.Thread.Sleep(1000)
        End With

    End Sub

    Private Sub OtherBlinking(RGB As String())
        With GetLED()
            .Blink(LedTarget.All, New Color(CInt(RGB(0)), CInt(RGB(1)), CInt(RGB(2))), 5, 0, 0)
            Threading.Thread.Sleep(1000)
            .SetColor(LedTarget.All, New Color(0, 0, 0))
            Threading.Thread.Sleep(1000)
        End With

    End Sub

    Private Sub Busy()
        GetLED.Blink(LedTarget.All, New Color(255, 0, 0), 30, 0, 0)
    End Sub

    Private Sub SwitchOffLED()
        With GetLED()
            Threading.Thread.Sleep(1)
            .SetColor(LedTarget.All, New Color(0, 0, 0))
        End With
    End Sub

    Private Sub Online()
        With GetLED()
            Threading.Thread.Sleep(1)
            .SetColor(LedTarget.All, New Color(0, 255, 0))
        End With
    End Sub

    Private Sub Available()
        With GetLED()
            Threading.Thread.Sleep(1)
            .SetColor(LedTarget.All, New Color(0, 255, 0))
        End With
    End Sub

    Private WithEvents LED As Timer = New Timer With {
        .Enabled = True,
        .Interval = 1
        }

    Private Sub LED_Tick(sender As Object, e As EventArgs) Handles LED.Tick
        LED.Stop()
        If LED_Type <> "" Then
            Try
                Dim F As String = IO.Path.Combine(Path, "led_" & LED_Type)
                Debug.WriteLine("NethLed Type: " & LED_Type)
                If IO.File.Exists(F) Then
                    IO.File.Delete(F)
                End If

                Select Case LED_Type
                    Case "incoming"
                        RingBlinking()
                    Case "busy"
                        Busy()
                        'LED_Type = ""
                    Case "off", "offline"
                        SwitchOffLED()
                        'LED_Type = ""
                    Case "online"
                        Online()
                        'LED_Type = ""
                    Case "available"
                        Available()
                        'LED_Type = ""

                    Case Else
                        Dim RGB As String() = Split(LED_Type, "_")
                        With GetLED()
                            Threading.Thread.Sleep(1)
                            If RGB.Contains("blink") Then
                                .Blink(LedTarget.All, New Color(CInt(RGB(0)), CInt(RGB(1)), CInt(RGB(2))), 30, 0, 0)
                            Else
                                .SetColor(LedTarget.All, New Color(CInt(RGB(0)), CInt(RGB(1)), CInt(RGB(2))))
                            End If
                            '.SetColor(LedTarget.All, New Color(0, 1, 2))
                        End With
                        'OtherBlinking(RGB)
                        'LED_Type = ""
                End Select
            Catch ex As Exception
                Debug.WriteLine("NethLed:" & LED_Type)
            End Try
        End If

        LED.Start()
    End Sub


    Private Sub OnError(sender As Object, e As ErrorEventArgs)
        WaitAndRecover(e)
        Debug.WriteLine("NethLed: OnError")
    End Sub

    Private Path As String = ""
    Private LED_Type As String = ""
    Private Sub OnCreated(sender As Object, e As FileSystemEventArgs)
        Debug.WriteLine("NethLed: OnCreated " & e.Name.ToLower)
        If e.Name.ToLower.StartsWith("led_") Then
            LED_Type = e.Name.Substring(4)
            Debug.WriteLine("LED_Type :" & LED_Type)
        End If
    End Sub
    Private Sub OnDeleted(sender As Object, e As FileSystemEventArgs)
        Debug.WriteLine("NethLed: OnDeleted")
        'If e.Name.ToLower.StartsWith("led_" & LED_Type) Then
        '    LED_Type = "off"
        'End If
    End Sub

    Private Sub StartWatching()

        Path = Application.StartupPath
        If My.Application.CommandLineArgs.Contains("-e") Then
            Path = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName)
        End If
        Debug.WriteLine("NethLed watcher on: " & Path)

        Watch = New FileSystemWatcher
        With Watch
            .Path = Path
            .IncludeSubdirectories = False

            .NotifyFilter = IO.NotifyFilters.DirectoryName
            .NotifyFilter = .NotifyFilter Or IO.NotifyFilters.FileName
            .NotifyFilter = .NotifyFilter Or IO.NotifyFilters.Attributes
            '.NotifyFilter = IO.NotifyFilters.FileName

            'add the handler to each event
            AddHandler .Created, AddressOf OnCreated
            AddHandler .Error, AddressOf OnError
            AddHandler .Deleted, AddressOf OnDeleted
            'AddHandler .Changed, AddressOf Changed
            '' add the rename handler as the signature is different
            'AddHandler .Renamed, AddressOf Renamed

            .EnableRaisingEvents = True
        End With
    End Sub

    Private Sub WaitAndRecover(e As ErrorEventArgs)
        Dim Err As Exception = Nothing
        Watch = New FileSystemWatcher
        While Not Watch.EnableRaisingEvents
            Try
                Debug.WriteLine("NethLed: Error ")
                StartWatching()

                If Not Watch.EnableRaisingEvents Then
                    System.Threading.Thread.Sleep(5000)
                End If
            Catch ex As Exception
                System.Threading.Thread.Sleep(5000)
            End Try
        End While
    End Sub

End Class
