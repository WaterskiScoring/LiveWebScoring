Public Class Recent
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Set the form level variables
        Dim sEM As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = ""
        Dim sYrPkd As String = "0"  'default is 0 = recent
        Dim sTournName As String = ""
        Dim sFormatCode As String = ""
        Dim sEventCodePkd As String = ""  'Could set to "A" as default
        Dim sDivisionCodePkd As String = "" 'Could set to "A" as default
        Dim sRndsPkd As Int16 = 0
        Dim sSlalomRounds As Int16 = 0
        Dim sTrickRounds As Int16 = 0
        Dim sJumpRounds As Int16 = 0
        Dim sEventCount As Int16 = 0
        Dim sUseNOPS As Boolean = False
        Dim sUseTeams As Boolean = False
        If Not IsPostBack Then


            ' VALIDATE ALL VALUES RECEIVED FROM QUERY STRING.
            If Request("SN") Is Nothing Or Request("SY") = Nothing Or Request("TN") = Nothing Or Request("FC") = Nothing Or Request("EV") = Nothing Or Request("DV") = Nothing Or Request("RP") = Nothing Or Request("UN") = Nothing Or Request("UT") = Nothing Then
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
                Exit Sub
            End If

            Try
                sSanctionID = Request("SN")
                sYrPkd = Request("SY")
                sTournName = CStr(Request("TN"))
                sFormatCode = Request("FC")
                sEventCodePkd = Request("EV")
                sDivisionCodePkd = Request("DV")
                sRndsPkd = CInt(Request("RP"))  'If not an integer will throw an esception Allowed alues 0,1,2,3,4
                '            sSlalomRounds = CInt(Request("SR"))
                '            sTrickRounds = CInt(Request("TR"))
                '            sJumpRounds = CInt(Request("JR"))
            Catch ex As Exception
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
                Exit Sub
            End Try
            If Not Regex.IsMatch(CStr(sRndsPkd), "^[01234]$") Then  '0 means all.  allowed values for rounds 0,1,2,3,4
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
                Exit Sub
            End If

            Try

                If Not Regex.IsMatch(sSanctionID, "^[0-9][0-9][CEMSWUX][0-9][0-9][0-9]$") Then  'Include X to include Dave's test tournament 24X017
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                    Exit Sub
                End If
                If CStr(sYrPkd) <> "0" Then   '0 = recent which is default
                    If Not Regex.IsMatch(sYrPkd, "^[2-9][0-9]$") Then
                        sEM = "Invalid Request.  You may try again."
                        Session.Abandon()
                        Response.Redirect("~/default.aspx?EM=" & sEM)
                        Exit Sub
                    End If
                End If
                If Not Regex.IsMatch(sEventCodePkd, "^[ASTJO]$") Then
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                End If
                If Not Regex.IsMatch(sDivisionCodePkd, "^[A-Za-z1-9]*$") Then
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                    Exit Sub
                End If
            Catch ex As Exception
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
                Exit Sub
            End Try
            If Not Regex.IsMatch(sFormatCode, "^[DVMR]*$") Then
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
            End If
            If sFormatCode <> "DV" And sFormatCode <> "MR" Then
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
            End If
            HF_FormatCode.Value = sFormatCode


            Try
                sUseNOPS = CBool(Request("UN"))
                sUseTeams = CBool(Request("UT"))
            Catch
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
            End Try
            HF_UseNOPS.Value = sUseNOPS
            HF_UseTeams.Value = sUseTeams
            TName.InnerHtml = "<h4>" & sTournName & " - " & sSanctionID & "</h4>"  'Tournament Name is just displayed so not validated
            HF_SanctionID.Value = sSanctionID
            HF_YearPkd.Value = sYrPkd  ' = ddl_YrPkd.selectedvalue
            HF_TournName.Value = sTournName

            'INITIALIZE THE ARRAYS
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
            'Get the data
            sArrSpecs = ModDataAccess.GetTournamentSpecs(sSanctionID)
            If Left(sArrSpecs(0, 0), 5) = "Error" Then
                lbl_Errors.Text = sArrSpecs(0, 0)  'friendly error message
                Exit Sub
            End If

            If sArrSpecs(6, 2) > 0 Then
                sSlalomRounds = sArrSpecs(6, 2)
                sEventCount += 1
            End If
            If sArrSpecs(7, 2) > 0 Then
                sTrickRounds = sArrSpecs(6, 2)
                sEventCount += 1
            End If
            If sArrSpecs(8, 2) > 0 Then
                sJumpRounds = sArrSpecs(6, 2)
                sEventCount += 1
            End If
            'Load the Events Drop List  and set selectedvalue to match selection on tournament page
            With DDL_EventsPkd
                .Items.Clear()
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
                If sEventCount = 3 Then
                    .Items.Add(New ListItem("Overall", "O"))
                End If
                .SelectedValue = sEventCodePkd
            End With
            HF_SlalomRnds.Value = sSlalomRounds
            HF_TrickRnds.Value = sTrickRounds
            HF_JumpRnds.Value = sJumpRounds

            ' Have query string variables validated.
            'display most recent performances in each event
            Div_MR_C1.InnerHtml = ModDataAccess.LoadOnWaterSlalom(sSanctionID)
            Div_MR_C2.InnerHtml = ModDataAccess.LoadOnWaterTrick(sSanctionID)
            Div_MR_C3.InnerHtml = ModDataAccess.LoadOnWaterJump(sSanctionID)
            'OVERALL NOT INCLUDED
            '           ColumnFour.InnerHtml = ModDataAccess.GetTeamScores(sSanctionID)
            'Ignore settings in event, round, and Dv droplists - set to include all
            sDivisionCodePkd = "A"
            sRndsPkd = 0
            sUseNOPS = False
            Select Case CB_ShowAll.Checked
                Case True
                    ColumnOne.InnerHtml = ModDataAccess.DisplayXRndandDv(sSanctionID, "S", sDivisionCodePkd, sRndsPkd, sUseNOPS, sUseTeams)
                    Panel_R1C1.Visible = True
                    ColumnTwo.InnerHtml = ModDataAccess.DisplayXRndandDv(sSanctionID, "T", sDivisionCodePkd, sRndsPkd, sUseNOPS, sUseTeams)
                    Panel_R1C2.Visible = True
                    ColumnThree.InnerHtml = ModDataAccess.DisplayXRndandDv(sSanctionID, "J", sDivisionCodePkd, sRndsPkd, sUseNOPS, sUseTeams)
                    Panel_R1C3.Visible = True
                    '                    ColumnFour.InnerHtml = ModDataAccess.DisplayXRndandDv(sSanctionID, "O", sDivisionCodePkd, sRndsPkd, sUseNOPS, sUseTeams)
                    Panel_R1C4.Visible = False
                Case False
                    Panel_R1C1.Visible = False
                    Panel_R1C2.Visible = False
                    Panel_R1C3.Visible = False
                    Panel_R1C4.Visible = False
            End Select 'display 
        End If

    End Sub

    Private Sub Btn_UpdateDisplay_Click(sender As Object, e As EventArgs) Handles Btn_UpdateDisplay.Click
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sFormatCode = HF_FormatCode.Value
        Dim sTournName As String = HF_TournName.Value
        Dim sEventCodePkd As String = "A"  'DDL_EventsPkd.SelectedValue
        Dim sDivisionCodePkd As String = "ALL" ' DDL_DvPkd.SelectedValue
        Dim sRndsPkd As Byte = 0  ' CByte(DDL_PkRnd.SelectedValue)
        Dim sYrPkd As String = HF_YearPkd.Value
        Dim sUseNOPS As Boolean = False    'CBool(cb_NOPS.Checked)
        Dim sUseTeams As Boolean = False
        Dim sMsg As String = ""
        Dim sErrMsg As String = ""
        'display most recent performances in each event
        Div_MR_C1.InnerHtml = ModDataAccess.LoadOnWaterSlalom(sSanctionID)
        Div_MR_C2.InnerHtml = ModDataAccess.LoadOnWaterTrick(sSanctionID)
        Div_MR_C3.InnerHtml = ModDataAccess.LoadOnWaterJump(sSanctionID)
        'OVERALL NOT INCLUDED
        '           ColumnFour.InnerHtml = ModDataAccess.GetTeamScores(sSanctionID)
        Select Case CB_ShowAll.Checked
            Case True
                ColumnOne.InnerHtml = ModDataAccess.DisplayXRndandDv(sSanctionID, "S", sDivisionCodePkd, sRndsPkd, sUseNOPS, sUseTeams)
                Panel_R1C1.Visible = True
                ColumnTwo.InnerHtml = ModDataAccess.DisplayXRndandDv(sSanctionID, "T", sDivisionCodePkd, sRndsPkd, sUseNOPS, sUseTeams)
                Panel_R1C2.Visible = True
                ColumnThree.InnerHtml = ModDataAccess.DisplayXRndandDv(sSanctionID, "J", sDivisionCodePkd, sRndsPkd, sUseNOPS, sUseTeams)
                Panel_R1C3.Visible = True
                '                    ColumnFour.InnerHtml = ModDataAccess.DisplayXRndandDv(sSanctionID, "O", sDivisionCodePkd, sRndsPkd, sUseNOPS, sUseTeams)
                Panel_R1C4.Visible = False
            Case False
                Panel_R1C1.Visible = False
                Panel_R1C2.Visible = False
                Panel_R1C3.Visible = False
                Panel_R1C4.Visible = False
        End Select '
    End Sub

    Private Sub Btn_2TList_Click(sender As Object, e As EventArgs) Handles Btn_2TList.Click
        Dim sYrPkd As String = HF_YearPkd.Value
        Response.Redirect("TList.aspx?YR=" & sYrPkd)
    End Sub

    Private Sub Btn_2Tournament_Click(sender As Object, e As EventArgs) Handles Btn_2Tournament.Click
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sSkiYr As String = HF_YearPkd.Value
        Dim sTournName As String = HF_TournName.Value
        Response.Redirect("Tournament.aspx?SN= " & sSanctionID & "&SY=" & sSkiYr & "&TN=" & sTournName & "")
    End Sub

    Private Sub Btn_Home_Click(sender As Object, e As EventArgs) Handles Btn_Home.Click
        Response.Redirect("default.aspx")
    End Sub

End Class