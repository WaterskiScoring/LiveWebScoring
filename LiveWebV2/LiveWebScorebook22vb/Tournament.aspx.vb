Public Class Tournament
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'Get the sanction number from the query string.
            'SN and FM and SY must always be present.  Check for other variables based on FM
            If Request("SN") Is Nothing Or Request("SY") = Nothing Then
                lbl_Errors.Text = "Missing Parameter"
                Exit Sub
            End If
            Dim sSanctionID As String = Trim(Request("SN"))
            Dim sYrPkd As String = Trim(Request("SY"))
            Dim sTournName As String = Request("TN")
            HF_SanctionID.Value = sSanctionID
            HF_YearPkd.Value = sYrPkd  ' = ddl_YrPkd.selectedvalue

            'INITIALIZE THE ARRAYS
            Dim sArrOfficials(0 To 12, 0 To 2)
            Dim j As Int16 = 0
            For j = 0 To 12
                sArrOfficials(j, 0) = ""
                sArrOfficials(j, 1) = ""
                sArrOfficials(j, 2) = ""
            Next
            Dim ArrAgeGroup(0 To 43)  'gets divisions offered only
            For j = 0 To 43
                ArrAgeGroup(j) = ""
            Next
            Dim sArrSpecs(0 To 5, 0 To 3)
            For j = 0 To 5
                sArrSpecs(j, 0) = ""
                sArrSpecs(j, 1) = ""
                sArrSpecs(j, 2) = ""
            Next
            'Not filtering here by event at this point Probably more appropriate for TXDivision or TIndScores
            'Look for code for pulling out events and divisions Tournament.aspx in LiveScorebook project

            'Get the Tournament Specifications
            sArrSpecs = ModDataAccess.GetTournamentSpecs(sSanctionID)
            If Left(sArrSpecs(0, 0), 5) = "Error" Then
                lbl_Errors.Text = sArrSpecs(0, 0)
                Exit Sub
            End If
            ' Populate the title div
            TName.InnerHtml = "<h3>" & sArrSpecs(2, 2) & " - " & sSanctionID & "</h3>"
            HF_TournName.Value = sArrSpecs(2, 2)
            'Displah Tournament Specs
            Dim sTableWidth As String = "100%"
            Dim sDisplayText As String = "<table width=""" & sTableWidth & """>"
            sDisplayText += "<tr><td><hr /></td></tr>"
            sDisplayText += "<tr><td><h4>Tournament Details<h4></td></tr>"
            sDisplayText += "<tr><td>" & sArrSpecs(1, 1) & " &nbsp; " & sArrSpecs(1, 2) & "</td></tr>"
            sDisplayText += "<tr><td>" & sArrSpecs(2, 1) & " &nbsp; " & sArrSpecs(2, 2) & "</td></tr>"
            sDisplayText += "<tr><td>" & sArrSpecs(3, 1) & " &nbsp; " & sArrSpecs(3, 2) & "</td></tr>"
            sDisplayText += "<tr><td>" & sArrSpecs(4, 1) & " &nbsp; " & sArrSpecs(4, 2) & "</td></tr>"
            sDisplayText += "<tr><td>" & sArrSpecs(5, 1) & " &nbsp; " & UCase(sArrSpecs(5, 2)) & "</td></tr>"
            sDisplayText += "</table>"
            'OFFICIALS
            Dim sOfficialsText As String = "<table width=""" & sTableWidth & """>"
            sOfficialsText += "<tr><td><hr /></td></tr>"
            sOfficialsText += "<tr><td><h4>Officials Panel</h4></td></tr>"
            sArrOfficials = ModDataAccess.GetOfficials(sSanctionID)
            If Left(sArrOfficials(0, 0), 5) = "Error" Then
                lbl_Errors.Text = sArrOfficials(0, 0)
                Exit Sub
            End If
            'Have array of officials
            For i = 1 To 12
                If sArrOfficials(i, 1) <> "" Then
                    sOfficialsText += "<tr><td>" & sArrOfficials(i, 1) & " &nbsp; " & sArrOfficials(i, 2) & "</td></tr>"
                Else
                    Exit For
                End If
            Next
            sOfficialsText += "</table>"
            DisplayText.InnerHtml = sDisplayText
            Officials.InnerHtml = sOfficialsText

        End If
    End Sub

    Private Sub Btn_ScoreXSkier_Click(sender As Object, e As EventArgs) Handles Btn_ScoreXSkier.Click
        Dim sYrPkd As String = HF_YearPkd.Value
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sTournName As String = HF_TournName.Value
        Dim sFormToUse As String = "TSkierList.aspx"
        '        If ddl_SelectFeatures.SelectedValue = "Test" Then
        '            sFormToUse = "TSkierListPro.aspx"
        '        End If
        Response.Redirect(sFormToUse & "?SY=" & sYrPkd & "&SN=" & sSanctionID & "&TN=" & sTournName & "")
    End Sub

    Private Sub Btn_PlacementXDv_Click(sender As Object, e As EventArgs) Handles Btn_PlacementXDv.Click
        Dim sUseNops As Boolean = cb_UseNOPS.Checked
        Dim sYrPkd As String = HF_YearPkd.Value
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sTournName As String = HF_TournName.Value
        Dim SelValue As String = ddl_SelectFeatures.SelectedValue
        Dim sFormatCode As String = "BestRnd"
        Dim sFormToUse As String = "TXDivisionPro.aspx"
        If SelValue <> "0" Then
            sFormatCode = SelValue
        End If
        Response.Redirect(sFormToUse & "?SY=" & sYrPkd & "&SN=" & sSanctionID & "&TN=" & sTournName & "&UN=" & sUseNops & "&FC=" & sFormatCode & "")
    End Sub

    Private Sub Btn_Back2TList_Click(sender As Object, e As EventArgs) Handles Btn_Back2TList.Click
        Dim sYrPkd As String = HF_YearPkd.Value
        Response.Redirect("TList.aspx?YR=" & sYrPkd & "")
    End Sub

    Private Sub Btn_Home_Click(sender As Object, e As EventArgs) Handles Btn_Home.Click
        Response.Redirect("default.aspx")
    End Sub
End Class