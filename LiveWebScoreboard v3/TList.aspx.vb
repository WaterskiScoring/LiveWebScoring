Public Class TList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request("YR") = Nothing Then
            Lbl_Errors.Text = "Missing Parameter"
            Exit Sub
        End If
        If Not IsPostBack Then
            Dim sSkiYr As String = Trim(Request("YR"))
            Dim sMsg As String = ""
            Dim sErrDetails As String = ""
            sMsg = ModDataAccess3.GetTournamentList2(sSkiYr)
            If Left(sMsg, 5) = "Error" Then
                Lbl_Errors.Text = "Check Internet Connection."
                sErrDetails = sMsg
                Exit Sub
            End If
            TList.InnerHtml = sMsg
        End If
    End Sub

    Private Sub Btn_Home_Click(sender As Object, e As EventArgs) Handles Btn_Home.Click
        Response.Redirect("Default.aspx")
    End Sub

End Class