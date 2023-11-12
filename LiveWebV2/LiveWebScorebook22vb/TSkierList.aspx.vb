Public Class TSkierList
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'Get the sanction number from the query string.
            'SN and FM and SY must always be present.  Check for other variables based on FM
            If Request("SN") Is Nothing Or Request("SY") = Nothing Or Request("TN") = Nothing Then
                lbl_Errors.Text = "Missing Parameter"
                Exit Sub
            End If
            Dim sSanctionID As String = Request("SN")
            Dim sFormMode As Int16 = Request("FM")
            Dim sYrPkd As String = Request("SY")
            Dim sTournName As String = Request("TN")
            TName.InnerHtml = "<h3>" & sTournName & " - " & sSanctionID & "</h3>"

            HF_SanctionID.Value = sSanctionID
            HF_YearPkd.Value = sYrPkd  ' = ddl_YrPkd.selectedvalue
            HF_TournName.Value = sTournName
            'Parameters below - EventPkd is not yet implemented.  Use A for all, S T or J for indivisual event list.
            'AgeDvPkd Not yet implemented.  Use "ALL" for entire registration list.

            Dim sSkierList As String = ModDataAccess.GetEntryList(sSanctionID, sTournName, "A", "ALL", sYrPkd)
            SkierList.InnerHtml = sSkierList
        End If
    End Sub

    Private Sub Btn_2TList_Click(sender As Object, e As EventArgs) Handles Btn_2TList.Click
        Dim sYrPkd As String = HF_YearPkd.Value
        Response.Redirect("TList.aspx?YR=" & sYrPkd)
    End Sub

    Private Sub Btn_2Tournament_Click(sender As Object, e As EventArgs) Handles Btn_2Tournament.Click
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sSkiYr As String = HF_YearPkd.Value
        Response.Redirect("Tournament.aspx?SN= " & sSanctionID & " &SY=" & sSkiYr & "")
    End Sub

    Private Sub Btn_Home_Click(sender As Object, e As EventArgs) Handles Btn_Home.Click
        Response.Redirect("Default.aspx")
    End Sub

End Class