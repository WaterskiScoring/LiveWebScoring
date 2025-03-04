Public Class TDashboard
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        OnWater.InnerHtml = ""
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
        Dim sFromTournament As String = ""
        Dim sRndsPkd As Int16 = 0
        Dim sSlalomRounds As Int16 = 0
        Dim sTrickRounds As Int16 = 0
        Dim sJumpRounds As Int16 = 0
        Dim sEventCount As Int16 = 0
        Dim sUseNOPS As Int16 = 0
        Dim sUseTeams As Int16 = 0
        Dim sOWSlalom As String = ""
        Dim sOWTrick As String = ""
        Dim sOWJump As String = ""
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim sDisplayMetric As Int16 = 0

        If Not IsPostBack Then

            ' VALIDATE ALL VALUES RECEIVED FROM QUERY STRING.
            Try
                ' VALIDATE ALL VALUES RECEIVED FROM QUERY STRING.
                If Request("FT") = Nothing Then
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                    Exit Sub
                End If
                sFromTournament = Request("FT")
                Select Case sFromTournament
                    Case "1" 'if request comes from Tournment don't set droplist values
                        If Request("SID") Is Nothing Or Request("SY") = Nothing Or Request("TN") = Nothing Or Request("FC") = Nothing Or Request("UN") = Nothing Or Request("UT") = Nothing Then
                            sEM = "Invalid Request.  You may try again."
                            Session.Abandon()
                            Response.Redirect("~/default.aspx?EM=" & sEM)
                            Exit Sub
                        End If
                        'Force event selection
                        DDL_PkRnd.Enabled = False
                        Btn_UpdateDisplay.Enabled = False
                    Case "0" ' Returning from TRecap - Set droplist values and reload page
                        If Request("SID") Is Nothing Or Request("SY") = Nothing Or Request("TN") = Nothing Or Request("FC") = Nothing Or Request("EV") = Nothing Or Request("DV") = Nothing Or Request("RP") = Nothing Or Request("UN") = Nothing Or Request("UT") = Nothing Then
                            sEM = "Invalid Request.  You may try again."
                            Session.Abandon()
                            Response.Redirect("~/default.aspx?EM=" & sEM)
                            Exit Sub
                        End If
                        sEventCodePkd = Request("EV")
                        sDivisionCodePkd = Request("DV")
                        sRndsPkd = Request("RP")
                        If Not Regex.IsMatch(CStr(sRndsPkd), "^[01234]$") Then  '0 means all.  allowed values for rounds 0,1,2,3,4
                            sEM = "Invalid Request.  You may try again."
                            Session.Abandon()
                            Response.Redirect("~/default.aspx?EM=" & sEM)
                            Exit Sub
                        End If
                        HF_RndPkd.Value = sRndsPkd
                        If Not Regex.IsMatch(sEventCodePkd, "^[ASTJO]$") Then
                            sEM = "Invalid Request.  You may try again."
                            Session.Abandon()
                            Response.Redirect("~/default.aspx?EM=" & sEM)
                        End If
                        HF_Event.Value = sEventCodePkd
                        If Not Regex.IsMatch(sDivisionCodePkd, "^[A-Za-z1-9]*$") Then
                            sEM = "Invalid Request.  You may try again."
                            Session.Abandon()
                            Response.Redirect("~/default.aspx?EM=" & sEM)
                            Exit Sub
                        End If
                        HF_AgeGroup.Value = sDivisionCodePkd
                    Case Else
                        sEM = "Invalid Request.  You may try again."
                        Session.Abandon()
                        Response.Redirect("~/default.aspx?EM=" & sEM)
                        Exit Sub
                End Select

                sSanctionID = Request("SID")
                sYrPkd = Request("SY")
                sTournName = CStr(Request("TN"))
                sFormatCode = Request("FC")

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

                If Not Regex.IsMatch(sFormatCode, "^[PRO]*$") Then
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                End If
                If sFormatCode <> "PRO" Then  'Make sure this is the correct display form for the format
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                End If
                HF_FormatCode.Value = sFormatCode

                sUseNOPS = CBool(Request("UN"))
                sUseTeams = CBool(Request("UT"))
            Catch
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
            End Try
            HF_UseNOPS.Value = sUseNOPS
            HF_UseTeams.Value = sUseTeams
            TName.InnerHtml = "<h4>" & sTournName & " - " & sSanctionID & "</h4> <span class=""bg-danger text-white"" ><b>!UNOFFICIAL!</b> </span> "
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
            sArrSpecs = ModDataAccess3.GetTournamentSpecs(sSanctionID)
            If Left(sArrSpecs(0, 0), 5) = "Error" Then
                lbl_Errors.Text = sArrSpecs(0, 0)  'friendly error message
                Exit Sub
            End If

            If sArrSpecs(6, 2) > 0 Then
                sSlalomRounds = sArrSpecs(6, 2)
                sEventCount += 1
            End If
            If sArrSpecs(7, 2) > 0 Then
                sTrickRounds = sArrSpecs(7, 2)
                sEventCount += 1
            End If
            If sArrSpecs(8, 2) > 0 Then
                sJumpRounds = sArrSpecs(8, 2)
                sEventCount += 1
            End If
            'Load the Events Drop List  and set selectedvalue to match selection on tournament page

            Select Case UCase(sArrSpecs(5, 2))'rules
                Case "AWSA"
                    sDisplayMetric = 0
                Case "IWWF"
                    sDisplayMetric = 1
                Case "NCWSA"
                    sDisplayMetric = 0
                Case Else
                    sDisplayMetric = 0
            End Select
            HF_DisplayMetric.Value = sDisplayMetric
            HF_SlalomRnds.Value = sSlalomRounds
            HF_TrickRnds.Value = sTrickRounds
            HF_JumpRnds.Value = sJumpRounds
            'can add round selecter here - have the rounds for each event.
            sMsg = ModDataAccessPro.LoadProEventList(sSanctionID, DDL_Event_Div)
            If sMsg <> "Success" Then
                lbl_Errors.Text = sMsg  'friendly error message
                Exit Sub
            End If

            If sFromTournament = "0" Then
                ModDataAccessPro.LoadProRndDDL(sEventCodePkd, sSlalomRounds, sTrickRounds, sJumpRounds, DDL_PkRnd)
                Dim sEDValue As String = sEventCodePkd & sDivisionCodePkd
                DDL_PkRnd.SelectedValue = sRndsPkd
                DDL_Event_Div.SelectedValue = sEDValue
                UpdateTPro()
            End If
        End If 'end of if not ispostback
    End Sub
    Private Sub UpdateTPro()
        'Updates the display using filter settings
        'Separate displays provided for various conditions - 1 running order for all rounds, different running order for one or more rounds, Individual round selected.
        Dim sMsg As String = ""
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sYrPkd As String = HF_YearPkd.Value
        Dim sEvDivSelVal As String = DDL_Event_Div.SelectedValue
        Dim sEventCodePkd As String = Left(sEvDivSelVal, 1)
        Dim sDivisionCodePkd As String = Right(Trim(sEvDivSelVal), 2)
        Dim sRndsPkd As String = DDL_PkRnd.SelectedValue
        Dim sSelRnd As String = ""
        Dim sJumpRounds As String = HF_JumpRnds.Value
        Dim sSlalomrounds As String = HF_SlalomRnds.Value
        Dim sTrickRounds As String = HF_TrickRnds.Value
        Dim sTournName As String = HF_TournName.Value
        Dim sUseTeams As Int16 = CInt(HF_UseTeams.Value)
        Dim sUseNops As Int16 = CInt(HF_UseNOPS.Value)
        Dim sFormatCode As String = HF_FormatCode.Value
        Dim sMultiS As String = "0"
        Dim sMultiT As String = "0"
        Dim sMultiJ As String = "0"
        Dim sOWSlalom As String = ""
        Dim sOWTrick As String = ""
        Dim sOWJump As String = ""
        Dim sDisplayMetric As Int16 = CInt(HF_DisplayMetric.Value)
        Dim ShowOnwater As Int16 = 0
        If sRndsPkd = "0" Then
            sSelRnd = ModDataAccessPro.GetCurrentRound(sSanctionID, sEventCodePkd, sDivisionCodePkd)
            HF_RndPkd.Value = sSelRnd
            If sSelRnd <> 25 Then
                DDL_PkRnd.SelectedValue = sSelRnd
            End If
        Else
            sSelRnd = sRndsPkd
            HF_RndPkd.Value = sSelRnd
        End If
        '        Dim sRunOrdCountArray(0 To 4)
        '        sRunOrdCountArray = ModDataAccessPro.GetRunOrdercountPro(sSanctionID, sSlalomrounds, sTrickRounds, sJumpRounds)
        '        If Left(sRunOrdCountArray(0), 5) = "Error" Then
        '            sMsg = sRunOrdCountArray(0)
        '        Else
        '            sMultiS = sRunOrdCountArray(1)
        '            sMultiT = sRunOrdCountArray(2)
        '            sMultiJ = sRunOrdCountArray(3)
        '        End If
        Dim sPlcmtFormat As String = ""
        Dim sActiveEvent As String = ""
        sActiveEvent = ModDataAccess3.GetCurrentEvent(sSanctionID, -15)
        If Left(sActiveEvent, 5) <> "Error" And sActiveEvent <> "" Then
            ShowOnwater = 0
            If sEventCodePkd = "S" And sSlalomrounds > 0 Then
                sOWSlalom = ModDataAccess3.LoadOnWaterSlalom(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
                If Left(sOWSlalom, 2) <> "No" Then OnWater.InnerHtml += sOWSlalom
                ShowOnwater += 1
            End If
            If sEventCodePkd = "T" And sTrickRounds > 0 Then
                sOWTrick = ModDataAccess3.LoadOnWaterTrick(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
                If Left(sOWTrick, 2) <> "No" Then OnWater.InnerHtml += sOWTrick
                ShowOnwater += 2
            End If
            If sEventCodePkd = "J" And sJumpRounds > 0 Then
                sOWJump = ModDataAccess3.LoadOnWaterJump(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
                If Left(sOWJump, 2) <> "No" Then OnWater.InnerHtml += sOWJump
                ShowOnwater += 3
            End If
        End If
        Select Case sEventCodePkd
            Case "0"
                RunOrd.InnerHtml = "Please select an event and Division"
            Case Else
                sMsg = ModDataAccessPro.GetRunOrderPro(sSanctionID, sYrPkd, sTournName, sEventCodePkd, sDivisionCodePkd, sSelRnd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                RunOrd.InnerHtml = sMsg
                Select Case sEventCodePkd
                    Case "S"
                        'start master table with column number = Round Number from Run Order above
                        'Run GetSlalomResults once for each round and put in column
                        'RunOff scores put in column in GetSlalomResults      list of scores in time order including round ? and Round 25 where runoff score non null.

                        sMsg = ModDataAccessPro.GetSlalomResults(sSanctionID, sYrPkd, sTournName, "S", sDivisionCodePkd, sSelRnd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                    Case "T"
                        sMsg = ModDataAccessPro.GetTrickResults(sSanctionID, sYrPkd, sTournName, "T", sDivisionCodePkd, sSelRnd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                    Case "J"
                        sMsg = ModDataAccessPro.GetJumpResults(sSanctionID, sYrPkd, sTournName, "J", sDivisionCodePkd, sSelRnd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                End Select
                Results.InnerHtml = sMsg
        End Select

    End Sub

    Private Sub Btn_2TList_Click(sender As Object, e As EventArgs) Handles Btn_2TList.Click
        Dim sYrPkd As String = HF_YearPkd.Value
        Response.Redirect("TList.aspx?YR=" & sYrPkd & "")
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

    Private Sub Btn_UpdateDisplay_Click(sender As Object, e As EventArgs) Handles Btn_UpdateDisplay.Click
        If DDL_Event_Div.SelectedValue = "0" Then
            lbl_Errors.Text = "Please Pick an Event"
            Exit Sub
        End If
        UpdateTPro()
    End Sub

    Private Sub DDL_Event_Div_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Event_Div.SelectedIndexChanged
        Dim sMsg As String = ""
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sEventCodePkd As String = Left(DDL_Event_Div.SelectedValue, 1)
        Dim sRndsPkd As String = DDL_PkRnd.SelectedValue
        If sRndsPkd = "" Then sRndsPkd = "0"
        Dim sJumpRounds As String = HF_JumpRnds.Value
        Dim sSlalomrounds As String = HF_SlalomRnds.Value
        Dim sTrickRounds As String = HF_TrickRnds.Value
        ModDataAccessPro.LoadProRndDDL(sEventCodePkd, sSlalomrounds, sTrickRounds, sJumpRounds, DDL_PkRnd)

        UpdateTPro()
        DDL_PkRnd.Enabled = True
        Btn_UpdateDisplay.Enabled = True
    End Sub

    Private Sub DDL_PkRnd_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_PkRnd.SelectedIndexChanged
        If DDL_PkRnd.SelectedValue <> HF_RndPkd.Value Then
            UpdateTPro()
        End If
    End Sub
End Class