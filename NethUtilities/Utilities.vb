
Imports System.IO
Imports System.Net.Mime
Imports System.Text

Public Class ExceptionManager
    Inherits TextFileManager

    Sub New()
        Try
            Dim Path As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & My.Application.Info.ProductName
            If Not IO.Directory.Exists(Path) Then
                IO.Directory.CreateDirectory(Path)
            End If
            Me.Path = Path & "\exception.ini"

        Catch ex As Exception
            'MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Me.Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\exception.ini"
        End Try
    End Sub

    Overrides Sub Write(ByVal Ex As Exception)
        Dim Msg As String = "" &
        "************************************************************************************************" & vbCrLf &
        Now.ToString & vbCrLf &
        "************************************************************************************************" & vbCrLf &
        Ex.Message & vbCrLf
        If Not IsNothing(Ex.StackTrace) Then
            Msg += "Stack Trace:" & vbCrLf &
            Ex.StackTrace & vbCrLf
        End If
        If Not IsNothing(Ex.Source) Then
            Msg += "Source:" & vbCrLf &
            Ex.Source & vbCrLf
        End If
        Msg += "************************************************************************************************" & vbCrLf
        MyBase.Write(New Exception(Msg))
    End Sub
End Class


Public Class UninstallBatchFile
    Public Shared ReadOnly Property Path As String
        Get
            Return IO.Path.GetTempPath & "\Uninstall_" & My.Application.Info.ProductName & ".cmd"
        End Get
    End Property
    Public Shared Sub CreateBatch(FolderPath As String)
        Dim Str As String = "@echo off" & vbCrLf &
            "set dir=""" & FolderPath & """" & vbCrLf &
            "del %dir%\* /F /Q " & vbCrLf &
            "for /d %%p in (%dir%\*) Do rd /Q /S ""%%p""" & vbCrLf &
            "rd /Q /S %dir%" & vbCrLf &
            "" & vbCrLf

        Using writer As StreamWriter = New StreamWriter(Path, True)
            With writer
                .Write(vbCrLf & Str)
            End With
        End Using
    End Sub
End Class

Public MustInherit Class TextFileManager
    Private _Path As String
    'Private Stream As StreamWriter

    Overridable Property Path As String
        Get
            Return _Path
        End Get
        Set(value As String)
            Try
                _Path = value
                'Dim MYINFO As FileInfo = New FileInfo(Path)
                'If MYINFO.IsReadOnly Then
                Using Stream As StreamReader = New StreamReader(Path)
                End Using
                'Else
                'Using Stream As StreamWriter = New StreamWriter(Path, True)
                'End Using
                'End If

                'Just create text file / read
                'End Using
            Catch ex As Exception
                Throw New Exception(ex.Message, ex.InnerException)
            End Try
        End Set
    End Property
    Overridable Sub Delete()
        Using Stream As StreamWriter = New StreamWriter(Path)
            'Just create text file / read
        End Using
        'File.Delete(Path)
    End Sub
    Overridable Sub Write(Ex As Exception)
        Write(Ex.Message, True)
    End Sub
    Overridable Sub Write(Msg As String, Optional NewLine As Boolean = True)
        Using writer As StreamWriter = New StreamWriter(Me.Path, True)
            With writer
                If NewLine Then
                    .Write(vbCrLf & Msg)
                Else
                    .Write(Msg)
                End If
            End With
        End Using
    End Sub
    Overridable Function Read() As String
        Dim ST As String = ""
        Using reader As StreamReader = New StreamReader(Me.Path)
            With reader
                ST = .ReadToEnd()
                .Close()
                .Dispose()
            End With
        End Using
        Return ST
    End Function
    Overridable Function ReadAllLines() As String()
        Dim Lines As String() = System.IO.File.ReadAllLines(Me.Path, Encoding.Default)
        Return Lines
    End Function
End Class