Public Class TIndScores
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'Get the sanction number from the query string.
            'SN and FM and SY must always be present.  Check for other variables based on FM
            If Request("SN") Is Nothing Or Request("SY") = Nothing Or Request("SID") = Nothing Or Request("TN") = Nothing Then
                lbl_Errors.Text = "Missing Parameter"
                Exit Sub
            End If
            Dim sSanctionID As String = Trim(Request("SN"))
            Dim sFormMode As Int16 = Request("FM")
            Dim sYrPkd As String = Request("SY")
            Dim sMemberID As String = Trim(Request("SID"))
            Dim sTournName As String = Request("TN")
            TName.InnerHtml = "<h3>" & sTournName & " - " & sSanctionID & "</h3>"
            HF_SanctionID.Value = sSanctionID
            HF_YearPkd.Value = sYrPkd  ' = ddl_YrPkd.selectedvalue
            HF_TournName.Value = sTournName
            SlalomScore.InnerHtml = ModDataAccess.IndivSlalomResults(sSanctionID, sMemberID)
            TrickScore.InnerHtml = ModDataAccess.IndivTrickResults(sSanctionID, sMemberID)
            JumpScore.InnerHtml = ModDataAccess.IndivJumpResults(sSanctionID, sMemberID)
        End If

    End Sub

    Private Sub Btn_2TList_Click(sender As Object, e As EventArgs) Handles Btn_2TList.Click
        Dim sYrPkd As String = HF_YearPkd.Value
        Response.Redirect("TList.aspx?YR=" & sYrPkd)
    End Sub

    Private Sub Btn_ToSkierList_Click(sender As Object, e As EventArgs) Handles Btn_ToSkierList.Click
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sSkiYr As String = HF_YearPkd.Value
        Dim sTournName As String = HF_TournName.Value
        Response.Redirect("TSkierList.aspx?SN= " & sSanctionID & "&SY=" & sSkiYr & "&TN=" & sTournName & "")
    End Sub

    Private Sub Btn_Home_Click(sender As Object, e As EventArgs) Handles Btn_Home.Click
        Response.Redirect("Default.aspx")
    End Sub

    Private Sub Btn_2TournHome_Click(sender As Object, e As EventArgs) Handles Btn_2TournHome.Click
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sSkiYr As String = HF_YearPkd.Value
        Dim sTournName As String = HF_TournName.Value
        Response.Redirect("Tournament.aspx?SN= " & sSanctionID & "&SY=" & sSkiYr & "&TN=" & sTournName & "")
    End Sub
End Class