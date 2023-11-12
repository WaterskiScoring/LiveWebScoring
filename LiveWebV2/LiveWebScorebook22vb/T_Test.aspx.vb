Public Class T_Test
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim something As String = ""
        If Not IsPostBack Then

        End If
    End Sub

    Private Sub Btn_UpdateDisplay_Click(sender As Object, e As EventArgs) Handles Btn_UpdateDisplay.Click
        ColumnOne.InnerHtml = "I am column 1"
        ColumnTwo.InnerHtml = "I am Column 2"
        ColumnThree.InnerHtml = "I am Column 3"

    End Sub
End Class