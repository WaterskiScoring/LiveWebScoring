Public Class TSkierListPro
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sMsg As String = ""
        If Not IsPostBack Then
            'validate the query string variables
            If Request("SID") Is Nothing Or Request("SY") = Nothing Or Request("TN") = Nothing Then
                TName.InnerHtml = "<h3>Invalid Parameter</h3>"
                Exit Sub
            End If
            Dim sSanctionID As String = Trim(Request("SID"))
            If Not Regex.IsMatch(sSanctionID, "^[1-9][0-9][ESCMWU][0-9][0-9][0-9]+$") Then
                TName.InnerHtml = "<h3>Invalid Parameter</h3>"
                Exit Sub
            End If
            Dim sYrPkd As String = Trim(Request("SY"))
            If CStr(sYrPkd) <> "0" Then   '0 = recent which is default
                If Not Regex.IsMatch(sYrPkd, "^[2-9][0-9]$") Then
                    TName.InnerHtml = "<h3>Invalid Parameter</h3>"
                    Exit Sub
                End If
            End If

            Dim sTournName As String = Trim(Request("TN"))
            'Not used in sql statement so just display what is there
            TName.InnerHtml = "<h3>" & sTournName & " - " & sSanctionID & "</h3>! UNOFFICIAL !"

            HF_SanctionID.Value = sSanctionID
                HF_YearPkd.Value = sYrPkd  ' = ddl_YrPkd.selectedvalue
                HF_TournName.Value = sTournName
            Dim sSkierList As String = ModDataAccess3.GetEntryList(sSanctionID, sTournName, sYrPkd)
            InsertHere.InnerHtml = sSkierList

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