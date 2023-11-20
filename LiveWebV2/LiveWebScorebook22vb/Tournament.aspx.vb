Imports System.Runtime.Remoting.Metadata.W3cXsd2001

Public Class Tournament
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sEM As String = ""
        Dim sMsg As String = ""
        Dim sSlalomRounds As Int16 = 0
        Dim sTrickRounds As Int16 = 0
        Dim sJumpRounds As Int16 = 0
        Dim sEventCount As Int16 = 0
        Dim sSanctionID As String = ""
        Dim sYrPkd As String = ""
        Dim sTournName As String = ""
        Dim sEventCode As String = ""
        Dim sDivisionCode As String = ""
        If Not IsPostBack Then
            'Get the sanction number from the query string.
            'SN and FM and SY must always be present.  Check for other variables based on FM
            If Request("SN") Is Nothing Or Request("SY") = Nothing Then
                lbl_Errors.Text = "Missing Parameter"
                Exit Sub
            End If

            sSanctionID = Trim(Request("SN"))
            sYrPkd = Trim(Request("SY"))
            sTournName = Request("TN")

            'Validate query string values
            If Not Regex.IsMatch(sSanctionID, "^[0-9][0-9][CEMSWU][0-9][0-9][0-9]$") Then
                lbl_Errors.Text = "Invalid Parameter"
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
                Exit Sub
            End If
            If CStr(sYrPkd) <> "0" Then   '0 = recent which is default
                If Not Regex.IsMatch(sYrPkd, "^[2][0][0-9][0-9]$") Then
                    lbl_Errors.Text = "Invalid Parameter"
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
                    Exit Sub
                End If
            End If
            'sTournName is display only - don't care what characters are used
            '           sTournName = Replace(sTournName, "'", "")
            '           If Not Regex.IsMatch(sTournName, "^[0123]$") Then
            '               lbl_Errors.Text = "Invalid Parameter"
            '               Exit Sub
            '           End If
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
            Dim ArrAgeGroup(0 To 43)  'get only divisions offered, but array is large enough to accept all divisions
            For j = 0 To 43
                ArrAgeGroup(j) = ""
            Next
            Dim sArrSpecs(0 To 9, 0 To 3)
            For j = 0 To 9
                sArrSpecs(j, 0) = ""
                sArrSpecs(j, 1) = ""
                sArrSpecs(j, 2) = ""
            Next
            'Set up drop list for selecting events and divisions to display

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

            If sArrSpecs(6, 2) > 0 Then
                sSlalomRounds = sArrSpecs(6, 2)
                sEventCount += 1
                sDisplayText += "<tr><td>" & sArrSpecs(6, 1) & " &nbsp; " & sSlalomRounds & "</td></tr>"
            End If
            If sArrSpecs(7, 2) > 0 Then
                sTrickRounds = sArrSpecs(6, 2)
                sEventCount += 1
                sDisplayText += "<tr><td>" & sArrSpecs(7, 1) & " &nbsp; " & sTrickRounds & "</td></tr>"
            End If
            If sArrSpecs(8, 2) > 0 Then
                sJumpRounds = sArrSpecs(6, 2)
                sEventCount += 1
                sDisplayText += "<tr><td>" & sArrSpecs(8, 1) & " &nbsp; " & sJumpRounds & "</td></tr>"
            End If
            With DDL_Events
                If sEventCount > 1 Then
                    .Items.Add(New ListItem("ALL", "A"))
                End If
                If sSlalomRounds > 0 Then
                    .Items.Add(New ListItem("Slalom", "S"))
                End If
                If sTrickRounds > 0 Then
                    .Items.Add(New ListItem("Trick", "T"))
                End If
                If sJumpRounds > 0 Then
                    .Items.Add(New ListItem("Jump", "J"))
                End If
            End With

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
        If Not IsPostBack Then
            sMsg = ModDataAccess.LoadDvList(sSanctionID, sEventCode, DDL_Division)
            If sMsg <> "Success" Then
                lbl_Errors.Text = sMsg
                Btn_PlacementXDv.Enabled = False
                Btn_ScoreXSkier.Enabled = False
            End If
        End If
    End Sub

    Private Sub Btn_ScoreXSkier_Click(sender As Object, e As EventArgs) Handles Btn_ScoreXSkier.Click
        Dim sYrPkd As String = HF_YearPkd.Value
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sTournName As String = HF_TournName.Value
        Dim sEventCode As String = DDL_Events.SelectedValue.ToString
        Dim sDivisionCode As String = DDL_Events.SelectedValue.ToString
        Dim sFormToUse As String = "TSkierList.aspx"
        '        If ddl_SelectFeatures.SelectedValue = "Test" Then
        '            sFormToUse = "TSkierListPro.aspx"
        '        End If
        Response.Redirect(sFormToUse & "?SY=" & sYrPkd & "&SN=" & sSanctionID & "&EV=" & sEventCode & "&DV=" & sDivisionCode & "&TN=" & sTournName & "")
    End Sub

    Private Sub Btn_PlacementXDv_Click(sender As Object, e As EventArgs) Handles Btn_PlacementXDv.Click
        Dim sUseNops As Boolean = cb_UseNOPS.Checked
        Dim sYrPkd As String = HF_YearPkd.Value
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sTournName As String = HF_TournName.Value
        Dim SelValue As String = ddl_SelectFeatures.SelectedValue
        Dim sFormatCode As String = "BestRnd"
        Dim sFormToUse As String = "TXDivisionPro.aspx"
        Dim sEventCode As String = DDL_Events.SelectedValue.ToString
        Dim sDivisionCode As String = DDL_Division.SelectedValue.ToString
        If SelValue <> "0" Then
            sFormatCode = SelValue
        End If
        Response.Redirect(sFormToUse & "?SY=" & sYrPkd & "&SN=" & sSanctionID & "&TN=" & sTournName & "&EV=" & sEventCode & "&DV=" & sDivisionCode & "&UN=" & sUseNops & "&FC=" & sFormatCode & "")
    End Sub

    Private Sub Btn_Back2TList_Click(sender As Object, e As EventArgs) Handles Btn_Back2TList.Click
        Dim sYrPkd As String = HF_YearPkd.Value
        Response.Redirect("TList.aspx?YR=" & sYrPkd & "")
    End Sub

    Private Sub Btn_Home_Click(sender As Object, e As EventArgs) Handles Btn_Home.Click
        Response.Redirect("default.aspx")
    End Sub

    Private Sub DDL_Events_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Events.SelectedIndexChanged
        Dim sEventCode As String = DDL_Events.SelectedValue
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sMsg As String = ""
        sMsg = ModDataAccess.LoadDvList(sSanctionID, sEventCode, DDL_Division)
        If sMsg <> "Success" Then
            lbl_Errors.Text = sMsg
        End If
    End Sub
End Class