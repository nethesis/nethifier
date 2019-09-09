Imports Newtonsoft.Json

Public Class CLS_ACTIONS

    Private _Action As String
    Property Action As String
        Get
            Return _Action
        End Get
        Set(value As String)
            _Action = value
        End Set
    End Property

    Private _Type As String
    Property Type As String
        Get
            Return _Type
        End Get
        Set(value As String)
            _Type = value
        End Set
    End Property

    'Private _Commands As Commands = New Commands
    'Property Commands As Commands
    '    Get
    '        Return _Commands
    '    End Get
    '    Set(value As Commands)
    '        _Commands = value
    '    End Set
    'End Property

    Private _Commands As New Hashtable
    Property Commands As Hashtable
        Get
            Return _Commands
        End Get
        Set(value As Hashtable)
            _Commands = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return JsonConvert.SerializeObject(Me).ToLower
    End Function

End Class
