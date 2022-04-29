Imports Newtonsoft.Json

Public Class CLS_NOTIFICATION

    Private _Notification As Notification = New Notification
    Property Notification As Notification
        Get
            Return _Notification
        End Get
        Set(value As Notification)
            _Notification = value
        End Set
    End Property
    Public Overrides Function ToString() As String
        Return JsonConvert.SerializeObject(Me).ToLower
    End Function
End Class

Public Class Notification
    Private _ID As String
    Property ID As String
        Get
            Return _ID
        End Get
        Set(value As String)
            _ID = value
        End Set
    End Property

    Private _URL As String
    Property URL As String
        Get
            Return _URL
        End Get
        Set(value As String)
            _URL = value
        End Set
    End Property

    Private _Action As String
    Property Action As String
        Get
            Return _Action
        End Get
        Set(value As String)
            _Action = value
        End Set
    End Property
    Private _CALLED As String
    Property CALLED As String
        Get
            Return _CALLED
        End Get
        Set(value As String)
            _CALLED = value
        End Set
    End Property
    Private _UNIQUEID As String
    Property UNIQUEID As String
        Get
            Return _UNIQUEID
        End Get
        Set(value As String)
            _UNIQUEID = value
        End Set
    End Property
    Private _CloseTimeOut As Integer
    Property CloseTimeOut As Integer
        Get
            Return _CloseTimeOut
        End Get
        Set(value As Integer)
            _CloseTimeOut = value
        End Set
    End Property

    Private _Width As Integer
    Property Width As Integer
        Get
            Return _Width
        End Get
        Set(value As Integer)
            _Width = value
        End Set
    End Property

    Private _Height As Integer
    Property Height As Integer
        Get
            Return _Height
        End Get
        Set(value As Integer)
            _Height = value
        End Set
    End Property
End Class
