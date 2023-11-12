Public Class TList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request("YR") = Nothing Then
            Lbl_Errors.Text = "Missing Parameter"
            Exit Sub
        End If
        Dim sSkiYr As String = Request("YR")
        Dim sMsg As String = ""

        'Only need SQL Server for this web application
        '       Dim sWhichDatabase As String = ConfigurationManager.ConnectionStrings("S_UseSS1_SL2").ConnectionString
        '       If sWhichDatabase = 1 Then 'Use Sql Server Local
        sMsg = ModDataAccess.GetTournamentList(sSkiYr)
        '      ElseIf sWhichDatabase = 2 Then 'Use SQLite
        '      '      sMsg = ModDataAccessSQLite.GetTournamentList(sSkiYr)
        '      End If
        If Left(sMsg, 5) = "Error" Then
            Lbl_Errors.Text = sMsg
            Exit Sub
        End If
        TList.InnerHtml = sMsg
    End Sub

    Private Sub Btn_Home_Click(sender As Object, e As EventArgs) Handles Btn_Home.Click
        Response.Redirect("Default.aspx")
    End Sub
End Class