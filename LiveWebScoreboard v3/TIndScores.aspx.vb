Imports System.Security.Cryptography.X509Certificates

Public Class TIndScores
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'THIS PAGE DEPRECIATED AS OF V 3.1.5.  SHOWING TRecap INSTEAD

        If Not IsPostBack Then
            'validate the query string variables

            If Request("SID") Is Nothing Or Request("SY") = Nothing Or Request("MID") = Nothing Or Request("N") = Nothing Or Request("TN") = Nothing Or Request("EV") = Nothing Or Request("DV") = Nothing Then
                TName.InnerHtml = "<h4>Invalid Parameter</h4>"
                Exit Sub
            End If
            Dim sSanctionID As String = Trim(Request("SID"))
            If Not Regex.IsMatch(sSanctionID, "^[1-9][0-9][ESCMWU][0-9][0-9][0-9]+$") Then
                TName.InnerHtml = "<h4>Invalid Parameter</h4>"
                Exit Sub
            End If
            '
            Dim sYrPkd As String = Trim(Request("SY"))
            If CStr(sYrPkd) <> "0" Then   '0 = recent which is default
                If Not Regex.IsMatch(sYrPkd, "^[2-9][0-9]$") Then
                    TName.InnerHtml = "<h4>Invalid Parameter</h4>"
                    Exit Sub
                End If
            End If
            Dim sMemberID As String = Trim(Request("MID"))
            If Not Regex.IsMatch(sMemberID, "^[0-9]*$") Then
                TName.InnerHtml = "<h4>Invalid Parameter</h4>"
                Exit Sub
            End If
            Dim sEventCode As String = Request("EV")
            If Not Regex.IsMatch(sEventCode, "^[ASTJ]$") Then
                TName.InnerHtml = "<h4>Invalid Parameter</h4>"
                Exit Sub
            End If
            Dim sDivisionCode As String = Request("DV")
            If Not Regex.IsMatch(sDivisionCode, "^[A-Za-z1-9]*$") Then
                TName.InnerHtml = "<h4>Invalid Parameter</h4>"
                Exit Sub
            End If
            Dim sSkierName As String = Request("N")   'Not used in sql statement so just display what is there
            Dim sTournName As String = Request("TN")
            'Not used in sql statement so just display what is there
            TName.InnerHtml = "<h4>" & sTournName & " - " & sSanctionID & "</h4><span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span>"

            HF_SanctionID.Value = sSanctionID
            HF_YearPkd.Value = sYrPkd  ' = ddl_YrPkd.selectedvalue
            HF_TournName.Value = sTournName
            HF_Event.Value = sEventCode
            HF_AgeGroup.Value = sDivisionCode


            SlalomScore.InnerHtml = ModDataAccess3.IndivSlalomResults(sSanctionID, sMemberID, sSkierName)
            TrickScore.InnerHtml = ModDataAccess3.IndivTrickResults(sSanctionID, sMemberID, sSkierName)
            JumpScore.InnerHtml = ModDataAccess3.IndivJumpResults(sSanctionID, sMemberID, sSkierName)
            OverallScore.InnerHtml = ModDataAccess3.RecapOverall(sSanctionID, sMemberID, sSkierName)
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
        Dim sEventCode As String = HF_Event.Value
        Dim sDivisionCode As String = HF_AgeGroup.Value
        Response.Redirect("TSkierListPro.aspx?SID=" & sSanctionID & "&SY=" & sSkiYr & "&TN=" & sTournName & "")
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