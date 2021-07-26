Imports System.Net
Imports System.IO
Imports Microsoft.Win32

Friend NotInheritable Class FRM_ABOUT
    Private ExceptionManager As ExceptionManager
    Private UserLogin As Login
    Private Config As Nethifier.Config
    Private Msg As MessageHelper

    Sub New(UserLogin As Login, Config As Nethifier.Config)

        ' This call is required by the designer.
        InitializeComponent()
        Me.Config = Config
        ' Add any initialization after the InitializeComponent() call.

        Msg = New MessageHelper(Config)
    End Sub

    Private Sub FRM_ABOUT_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("About {0}", ApplicationTitle)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        Me.LabelProductName.Text = My.Application.Info.ProductName

        'Dim VersionInfo As Version = New Version
        'With VersionInfo
        'Me.LabelVersion.Text = String.Format("Version {0}", .Major & "." & .Minor & "." & .Build)
        'Me.LabelCopyright.Text = .Copyright
        'Me.LabelCompanyName.Text = .CompanyName
        'End With

        Me.LabelVersion.Text = String.Format("Version {0}", My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build)
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.LabelCompanyName.Text = My.Application.Info.CompanyName

        Me.TextBoxDescription.Text = Msg.GetMessage("ABOUT") 'My.Application.Info.Description


    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

End Class
