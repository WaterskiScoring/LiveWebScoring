Public Class Tournament
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sEM As String = ""
        Dim sMsg As String = ""
        Dim sTMsg As String = ""
        Dim sSlalomRounds As Int16 = 0
        Dim sTrickRounds As Int16 = 0
        Dim sJumpRounds As Int16 = 0
        Dim sEventCount As Int16 = 0
        Dim sSanctionID As String = ""
        Dim sYrPkd As String = ""
        Dim sTournName As String = ""
        Dim sRulesImg As String = ""
        Dim sDisplayMetric As Int16 = 0
        Dim sEventCode As String = ""
        Dim sDivisionCode As String = ""
        Dim sMultiS As String = "0"
        Dim sMultiT As String = "0"
        Dim sMultiJ As String = "0"
        Dim sShowMostRecent As Boolean = True
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
            If Not Regex.IsMatch(sSanctionID, "^[0-9][0-9][CEMSWUX][0-9][0-9][0-9]$") Then
                lbl_Errors.Text = "Invalid Parameter"
                Exit Sub
            End If
            If CStr(sYrPkd) <> "0" Then   '0 = recent which is default
                If Not Regex.IsMatch(sYrPkd, "^[2-9][0-9]$") Then
                    lbl_Errors.Text = "Invalid Parameter"
                    Exit Sub
                End If
            End If
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
            'Get the Tournament Specifications
            sArrSpecs = ModDataAccess3.GetTournamentSpecs(sSanctionID)
            If Len(sArrSpecs(0, 0)) > 3 Then
                lbl_Errors.Text = sArrSpecs(0, 0)
                Exit Sub
            End If
            ' Populate the title div
            TName.InnerHtml = "<h3>" & sArrSpecs(2, 2) & " - " & sSanctionID & "</h3>"
            HF_TournName.Value = sArrSpecs(2, 2)
            'Display active events if any or an error msg
            Dim sActiveEvent As String = ""
            sActiveEvent = ModDataAccess3.GetCurrentEvent(sSanctionID, -15)

            ' If Now() < sArrSpecs(3, 2) '= start date  -How to get end date??
            ' ' or now() > end date then
            '            sShowMostRecent = False
            '        End If
            If Left(sActiveEvent, 5) = "Error" Or sActiveEvent = "" Then
                sShowMostRecent = False
            End If
            Lbl_ActiveEvents.Text = sActiveEvent  'Shows friendly error, active event(s) or is empty

            'Displah Tournament Specs
            Dim sTableWidth As String = "100%"
                Dim sDisplayText As String = "<table width=""" & sTableWidth & """>"
                sDisplayText += "<tr><td><hr /></td></tr>"
                sDisplayText += "<tr><td><h4>Tournament Details<h4></td></tr>"
                sDisplayText += "<tr><td>" & sArrSpecs(1, 1) & " &nbsp; " & sArrSpecs(1, 2) & "</td></tr>"
                sDisplayText += "<tr><td>" & sArrSpecs(2, 1) & " &nbsp; " & sArrSpecs(2, 2) & "</td></tr>"
                sDisplayText += "<tr><td>" & sArrSpecs(3, 1) & " &nbsp; " & sArrSpecs(3, 2) & "</td></tr>"
                sDisplayText += "<tr><td>" & sArrSpecs(4, 1) & " &nbsp; " & sArrSpecs(4, 2) & "</td></tr>"
                Select Case UCase(sArrSpecs(5, 2))'rules
                    Case "AWSA"
                        sRulesImg = "<img src=""images/AWSALogo.jpg""/>"
                        sDisplayMetric = 0
                    Case "IWWF"
                        sRulesImg = "<img src=""images/iwwflogo.png""/>"
                        sDisplayMetric = 1
                    Case "NCWSA"
                        sRulesImg = UCase(sArrSpecs(5, 2))
                        sDisplayMetric = 0
                    Case Else
                        sRulesImg = sRulesImg = UCase(sArrSpecs(5, 2))
                        sDisplayMetric = 0
                End Select
                HF_DisplayMetric.Value = sDisplayMetric
                sDisplayText += "<tr><td>" & sArrSpecs(5, 1) & " &nbsp; " & sRulesImg & "</td></tr>"

                sTMsg += ModDataAccess3.GetTeams(sSanctionID)
                If Left(sTMsg, 5) = "Error" Then
                    lbl_Errors.Text = sTMsg
                    Panel_Teams.Visible = False
                Else
                    lbl_Teams.Text = sTMsg
                    Panel_Teams.Visible = True
                End If
                If sArrSpecs(6, 2) > 0 Then
                    sSlalomRounds = sArrSpecs(6, 2)
                    sEventCount += 1
                    sDisplayText += "<tr><td>" & sArrSpecs(6, 1) & " &nbsp; " & sSlalomRounds & "</td></tr>"
                End If
                If sArrSpecs(7, 2) > 0 Then
                    sTrickRounds = sArrSpecs(7, 2)
                    sEventCount += 1
                    sDisplayText += "<tr><td>" & sArrSpecs(7, 1) & " &nbsp; " & sTrickRounds & "</td></tr>"
                End If
                If sArrSpecs(8, 2) > 0 Then
                    sJumpRounds = sArrSpecs(8, 2)
                    sEventCount += 1
                    sDisplayText += "<tr><td>" & sArrSpecs(8, 1) & " &nbsp; " & sJumpRounds & "</td></tr>"
                End If
                sDisplayText += "</table>"

                HF_SlalomRnds.Value = sSlalomRounds 'should be 0 if not offered
                HF_TrickRnds.Value = sTrickRounds
                HF_JumpRnds.Value = sJumpRounds

                'Set up the droplists

                'Check - running orders different in various rounds?
                'if no add Summary
                'If yes - offer by division listing instead with rounds horizontal.
                Dim tmp As String = sSanctionID.Substring(2, 1)
                DDL_Format.Items.Clear()
                With DDL_Format
                    .Items.Add(New ListItem("Select Display Style", "0"))
                If sSanctionID.Substring(2, 1) <> "U" Then
                    .Items.Add(New ListItem("LeaderBoard", "LBSP"))
                    '                   .Items.Add(New ListItem("LeaderBoard", "LB"))
                    .Items.Add(New ListItem("Running Order", "RO"))
                    .Items.Add(New ListItem("Entry List", "EL"))

                    '                   .Items.Add(New ListItem("Summary x Dv Rnd", "PRO"))
                    '                    .Items.Add(New ListItem("Summary", "ROBst"))
                    '    .Items.Add(New ListItem("AgeDv", "DV"))
                Else  'Display NCWSA Formats
                    .Items.Add(New ListItem("LeaderBoard", "NCWL"))
                    .Items.Add(New ListItem("Running Order", "NCWRO"))
                    .Items.Add(New ListItem("Entry List", "EL"))
                End If
                If sShowMostRecent = True Then 'offer for AWS and NCW
                    .Items.Add(New ListItem("Most Recent", "MR"))
                End If
            End With

                'OFFICIALS
                Dim sOfficialsText As String = "<table width=""" & sTableWidth & """>"
                sOfficialsText += "<tr><td><hr /></td></tr>"
                sOfficialsText += "<tr><td><h4>Officials Panel</h4></td></tr>"
                sArrOfficials = ModDataAccess3.GetOfficials(sSanctionID)
                If Left(sArrOfficials(0, 0), 5) = "Error" Then
                    lbl_Errors.Text = sArrOfficials(0, 0)
                    Exit Sub
                End If
                If Left(sArrOfficials(0, 0), 2) = "No" Then
                    sOfficialsText += "<tr><td>" & sArrOfficials(0, 0) & "</td></tr></table>"
                Else
                    'Have array of officials
                    For i = 1 To 12
                        If sArrOfficials(i, 1) <> "" Then
                            sOfficialsText += "<tr><td>" & sArrOfficials(i, 1) & " &nbsp; " & sArrOfficials(i, 2) & "</td></tr>"
                        Else
                            Exit For
                        End If
                    Next

                    sOfficialsText += "</table>"
                End If
                DisplayText.InnerHtml = sDisplayText
                Officials.InnerHtml = sOfficialsText

            End If

    End Sub



    Private Sub Btn_Back2TList_Click(sender As Object, e As EventArgs) Handles Btn_Back2TList.Click
        Dim sYrPkd As String = HF_YearPkd.Value
        Response.Redirect("TList.aspx?YR=" & sYrPkd & "")
    End Sub

    Private Sub Btn_Home_Click(sender As Object, e As EventArgs) Handles Btn_Home.Click
        Response.Redirect("default.aspx")
    End Sub

    Private Sub Btn_Reports_Click(sender As Object, e As EventArgs) Handles Btn_Reports.Click
        Dim sSanctionID As String = HF_SanctionID.Value

        Response.Redirect("TReports.aspx?SID=" & sSanctionID)
    End Sub

    Private Sub DDL_Format_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Format.SelectedIndexChanged
        Dim sFormToUse As String = ""
        Dim sFormatCode As String = DDL_Format.SelectedValue
        Dim sSanctionID As String = HF_SanctionID.Value
        Select Case sFormatCode
 '           Case "LB"  'Leader Board
 '               sFormToUse = "TLeaderBoard.aspx"
            Case "LBSP"
                sFormToUse = "TLeaderBoardSP.aspx"
            Case "RO"       'Running Order
                sFormToUse = "TROxRnd.aspx"
            Case "EL"       'Entry List
                sFormToUse = "TSkierListPro.aspx"
 '           Case "PRO"
 '               sFormToUse = "TPro.aspx"
            Case "NCWL"
                sFormToUse = "TLeaderBoardNCW.aspx"
            Case "NCWRO"
                sFormToUse = "TROandBrNCW.aspx"
            Case "MR"
                sFormToUse = "TMostRecent.aspx"
        End Select
        Dim sUseNops As Int16 = 0 'set teams and NOPS to false pending development of teams page
        Dim sUseTeams As Int16 = 0
        Dim sYrPkd As String = HF_YearPkd.Value
        Dim sTournName As String = HF_TournName.Value
        'Parameter FT (From Tournament.aspx) always 1(true).  Is false returning from TRecap or TIndScores
        Response.Redirect(sFormToUse & "?SY=" & sYrPkd & "&SID=" & sSanctionID & "&TN=" & sTournName & "&UN=" & sUseNops & "&FC=" & sFormatCode & "&FT=1&UT=" & sUseTeams & "")
    End Sub
End Class