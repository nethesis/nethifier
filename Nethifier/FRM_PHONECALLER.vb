Imports System.Linq
Imports Microsoft.Win32
Imports System.Net
Imports System.Web
Imports System.Text
Imports System.IO
Imports System.Net.Security
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports Newtonsoft.Json
Imports IWshRuntimeLibrary
Imports Nethifier.Helper

Friend NotInheritable Class FRM_PHONECALLER

    Private ExceptionManager As ExceptionManager
    Private UserLogin As Login
    Private Config As Nethifier.Config
    Private NethDebug As NethDebugger
    Private Msg As MessageHelper

    Sub New(ByVal UserLogin As Login, ByVal Config As Nethifier.Config, NethDebug As NethDebugger)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.Config = Config
        Me.UserLogin = UserLogin
        Me.NethDebug = NethDebug

        TXT_PHONE_NUMBER.Text = ""

    End Sub

    Private Sub BUT_CALL_Click(sender As Object, e As EventArgs) Handles BUT_CALL.Click
        DoTelephoneCall()
    End Sub

    Public Sub DoTelephoneCall()

        If IsNumeric(TXT_PHONE_NUMBER.Text) Then
            Me.Opacity = 0
            Dim CallNotifier As FRM_CALL_NOTIFICATIONS = New FRM_CALL_NOTIFICATIONS(UserLogin, Config, NethDebug)
            CallNotifier.DoCall(TXT_PHONE_NUMBER.Text)

            'With CallNotifier
            '    .Opacity = 0
            '    .Show()
            '    .Location = New Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width - .Width, Screen.PrimaryScreen.WorkingArea.Size.Height - .Height)
            '    .Opacity = 1
            '    .DoCall(TXT_PHONE_NUMBER.Text)
            'End With
        End If

    End Sub

    Private Sub FRM_PHONECALLER_Activated(sender As Object, e As EventArgs) Handles Me.Activated

    End Sub

    Private Sub FRM_PHONECALLER_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not IsClosing Then
            e.Cancel = True
            Me.Opacity = 0
        End If
    End Sub

    Private IsClosing As Boolean
    Public Sub CloseDialer()
        IsClosing = True
        Me.Close()
    End Sub

    Private Sub TXT_PHONE_NUMBER_KeyDown(sender As Object, e As KeyEventArgs) Handles TXT_PHONE_NUMBER.KeyDown
        If (e.KeyCode >= Keys.D0 AndAlso e.KeyCode <= Keys.D9) OrElse (e.KeyCode >= Keys.NumPad0 AndAlso e.KeyCode <= Keys.NumPad9) OrElse
            e.KeyCode = Keys.Back OrElse
            e.KeyCode = Keys.Left OrElse
            e.KeyCode = Keys.Right OrElse e.KeyCode = Keys.Delete Then
        Else
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TXT_PHONE_NUMBER_TextChanged(sender As Object, e As EventArgs) Handles TXT_PHONE_NUMBER.TextChanged
        'BUT_CALL.Enabled = IsNumeric(TXT_PHONE_NUMBER.Text)
        BUT_CALL.Enabled = IsNumeric(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(TXT_PHONE_NUMBER.Text, "-", ""), "/", ""), " ", ""), "(", ""), ")", ""), "[", ""), "]", ""), "{", ""), "}", ""))
    End Sub

    Private Sub NUM_Click(sender As Object, e As EventArgs) Handles BUT_0.Click, BUT_1.Click, BUT_2.Click,
        BUT_3.Click, BUT_4.Click, BUT_5.Click,
        BUT_6.Click, BUT_7.Click, BUT_8.Click,
        BUT_9.Click

        TXT_PHONE_NUMBER.Text += DirectCast(sender, Button).Text

    End Sub

End Class