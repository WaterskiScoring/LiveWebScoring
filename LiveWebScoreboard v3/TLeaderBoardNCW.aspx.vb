Public Class TLeaderBoardNCW
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
        Dim sFromTournament As String = "0"
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
                    Case "0" ' Returning from TRecap - Set droplist values and reload page
                        If Request("SID") Is Nothing Or Request("SY") = Nothing Or Request("TN") = Nothing Or Request("FC") = Nothing Or Request("EV") = Nothing Or Request("DV") = Nothing Or Request("UN") = Nothing Or Request("UT") = Nothing Then
                            sEM = "Invalid Request.  You may try again."
                            Session.Abandon()
                            Response.Redirect("~/default.aspx?EM=" & sEM)
                            Exit Sub
                        End If
                        sEventCodePkd = Request("EV")
                        sDivisionCodePkd = Request("DV")
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

                If Not Regex.IsMatch(sFormatCode, "^[NCWROL]*$") Then
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                End If
                If sFormatCode <> "NCWL" Then  'Make sure this is the correct display form for the format
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
            TName.InnerHtml = "<h4>" & sTournName & " - " & sSanctionID & "</h4> <span class=""bg-danger text-white"" ><b>!UNOFFICIAL!</b> </span> "   'Tournament Name is just displayed so not validated
            HF_SanctionID.Value = sSanctionID
            HF_YearPkd.Value = sYrPkd  ' = ddl_YrPkd.selectedvalue
            HF_TournName.Value = sTournName

            'INITIALIZE THE ARRAYS
            Dim ArrAgeGroup(0 To 43)
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
                HF_SlalomRnds.Value = sSlalomRounds
                sEventCount += 1
            End If
            If sArrSpecs(7, 2) > 0 Then
                sTrickRounds = sArrSpecs(7, 2)
                HF_TrickRnds.Value = sTrickRounds
                sEventCount += 1
            End If
            If sArrSpecs(8, 2) > 0 Then
                sJumpRounds = sArrSpecs(8, 2)
                HF_JumpRnds.Value = sJumpRounds
                sEventCount += 1
            End If

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
            'Load the Events Drop List  
            With DDL_EventsPkd
                .Items.Clear()
                .Items.Add(New ListItem("Select Event", "0"))
                If sSlalomRounds > 0 Then
                    .Items.Add(New ListItem("Slalom", "S"))
                    k += 1
                End If
                If sTrickRounds > 0 Then
                    .Items.Add(New ListItem("Trick", "T"))
                    k += 1
                End If
                If sJumpRounds > 0 Then
                    .Items.Add(New ListItem("Jump", "J"))
                    k += 1
                End If
                If k > 1 Then
                    .Items.Add(New ListItem("ALL", "A"))
                End If
            End With
            HF_SlalomRnds.Value = sSlalomRounds
            HF_TrickRnds.Value = sTrickRounds
            HF_JumpRnds.Value = sJumpRounds


            'can add round selecter here - have the rounds for each event.
            'Load the Divisions Drop List and 
            sMsg = ModDataAccess3.LoadDvList(sSanctionID, sEventCodePkd, sDivisionCodePkd, sRndsPkd, DDL_DvPkd, sSlalomRounds, sTrickRounds, sJumpRounds, DDL_PkRnd)
            If sMsg <> "Success" Then
                lbl_Errors.Text = sMsg  'friendly error message
                Exit Sub
            End If
            Dim ShowOnwater As Int16 = 0 = False
            If (sEventCodePkd = "S" Or sEventCodePkd = "A") Then
                sOWSlalom = ModDataAccess.LoadOnWaterSlalom(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
                If Left(sOWSlalom, 2) <> "No" Then
                    OnWater.InnerHtml = sOWSlalom
                    ShowOnwater += 1
                End If
            End If
            If (sEventCodePkd = "T" Or sEventCodePkd = "A") Then
                sOWTrick = ModDataAccess.LoadOnWaterTrick(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
                If Left(sOWTrick, 2) <> "No" Then
                    OnWater.InnerHtml += sOWTrick
                    ShowOnwater += 2
                End If
            End If
            If (sEventCodePkd = "J" Or sEventCodePkd = "A") Then
                sOWJump = ModDataAccess.LoadOnWaterJump(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
                If Left(sOWJump, 2) <> "No" Then
                    OnWater.InnerHtml += sOWJump
                    ShowOnwater += 3
                End If
            End If
            If ShowOnwater > 0 Then
                Panel_OnWater.Visible = True
            Else
                Panel_OnWater.Visible = False
            End If

            If sFromTournament = "0" Then
                DDL_EventsPkd.SelectedValue = sEventCodePkd
                DDL_DvPkd.SelectedValue = sDivisionCodePkd
                '                DDL_PkRnd.SelectedValue = sRndsPkd
                UpdateLeaderboard()
            End If


        End If 'end of if not ispostback
    End Sub

    Private Sub Btn_UpdateDisplay_Click(sender As Object, e As EventArgs) Handles Btn_UpdateDisplay.Click
        UpdateLeaderboard()

    End Sub
    Private Sub Btn_Home_Click(sender As Object, e As EventArgs) Handles Btn_Home.Click
        Response.Redirect("default.aspx")
    End Sub

    Private Sub Btn_2Tournament_Click(sender As Object, e As EventArgs) Handles Btn_2Tournament.Click
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sSkiYr As String = HF_YearPkd.Value
        Dim sTournName As String = HF_TournName.Value
        Response.Redirect("Tournament.aspx?SN= " & sSanctionID & "&SY=" & sSkiYr & "&TN=" & sTournName & "")
    End Sub

    Private Sub Btn_2TList_Click(sender As Object, e As EventArgs) Handles Btn_2TList.Click
        Dim sYrPkd As String = HF_YearPkd.Value
        Response.Redirect("TList.aspx?YR=" & sYrPkd)
    End Sub

    Private Sub DDL_EventsPkd_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_EventsPkd.SelectedIndexChanged
        Dim sMsg As String = ""
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sEventCodePkd As String = DDL_EventsPkd.SelectedValue
        Dim sDivisionCodePkd As String = DDL_DvPkd.SelectedValue
        Dim sRndsPkd As String = DDL_PkRnd.SelectedValue
        Dim sJumpRounds As String = HF_JumpRnds.Value
        Dim sSlalomrounds As String = HF_SlalomRnds.Value
        Dim sTrickRounds As String = HF_TrickRnds.Value

        sMsg = ModDataAccess3.LoadDvList(sSanctionID, sEventCodePkd, sDivisionCodePkd, sRndsPkd, DDL_DvPkd, sSlalomrounds, sTrickRounds, sJumpRounds, DDL_PkRnd)
        If sMsg <> "Success" Then
            lbl_Errors.Text = sMsg  'friendly error message
            Exit Sub
        End If
        UpdateLeaderboard()
    End Sub

    Private Sub DDL_DvPkd_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_DvPkd.SelectedIndexChanged
        Dim sMsg As String = ""
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sEventCodePkd As String = DDL_EventsPkd.SelectedValue
        Dim sDivisionCodePkd As String = DDL_DvPkd.SelectedValue
        Dim sRndsPkd As String = DDL_PkRnd.SelectedValue
        Dim sJumpRounds As String = HF_JumpRnds.Value
        Dim sSlalomrounds As String = HF_SlalomRnds.Value
        Dim sTrickRounds As String = HF_TrickRnds.Value

        sMsg = ModDataAccess3.LoadDvList(sSanctionID, sEventCodePkd, sDivisionCodePkd, sRndsPkd, DDL_DvPkd, sSlalomrounds, sTrickRounds, sJumpRounds, DDL_PkRnd)
        If sMsg <> "Success" Then
            lbl_Errors.Text = sMsg  'friendly error message
            Exit Sub
        End If
        UpdateLeaderboard()
    End Sub

    Private Sub UpdateLeaderboard()
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sYrPkd As String = HF_YearPkd.Value
        '        Dim sDivisionCodePkd As String = DDL_DvPkd.SelectedValue
        'Division droplist is not available for collegiate so set to ALL
        Dim sDivisionCodePkd As String = DDL_DvPkd.SelectedValue  '"ALL"
        Dim sEventCodePkd As String = DDL_EventsPkd.SelectedValue
        Dim sRndsPkd As String = DDL_PkRnd.SelectedValue
        Dim sJumpRounds As String = HF_JumpRnds.Value
        Dim sSlalomrounds As String = HF_SlalomRnds.Value
        Dim sTrickRounds As String = HF_TrickRnds.Value
        Dim sTournName As String = HF_TournName.Value
        Dim sUseTeams As Int16 = CInt(HF_UseTeams.Value)
        Dim sUseNops As Int16 = CInt(HF_UseNOPS.Value)
        Dim sFormatCode As String = HF_FormatCode.Value
        Dim sOWSlalom As String = ""
        Dim sOWTrick As String = ""
        Dim sOWJump As String = ""
        Dim sDisplayMetric As Int16 = CInt(HF_DisplayMetric.Value)
        Dim ShowOnwater As Int16 = 0 = False
        If sSlalomrounds > 0 Then
            sOWSlalom = ModDataAccess.LoadOnWaterSlalom(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
            If Left(sOWSlalom, 2) <> "No" Then OnWater.InnerHtml = sOWSlalom
            ShowOnwater += 1
        End If
        If sTrickRounds > 0 Then
            sOWTrick = ModDataAccess.LoadOnWaterTrick(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
            If Left(sOWTrick, 2) <> "No" Then OnWater.InnerHtml += sOWTrick
            ShowOnwater += 2
        End If
        If sJumpRounds > 0 Then
            sOWJump = ModDataAccess.LoadOnWaterJump(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
            If Left(sOWJump, 2) <> "No" Then OnWater.InnerHtml += sOWJump
            ShowOnwater += 3
        End If
        If ShowOnwater > 0 Then
            Panel_OnWater.Visible = True
        Else
            Panel_OnWater.Visible = False
        End If
        Select Case sEventCodePkd
            Case "0"
                ColumnOne.InnerHtml = "Please Select an Event"
            Case "A"
                ColumnOne.InnerHtml = ModDataAccessTeams.LeaderBoardBestRndLeft(sSanctionID, sYrPkd, sTournName, "S", sDivisionCodePkd, sRndsPkd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                ColumnTwo.InnerHtml = ModDataAccessTeams.LeaderBoardBestRndLeft(sSanctionID, sYrPkd, sTournName, "T", sDivisionCodePkd, sRndsPkd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                ColumnThree.InnerHtml = ModDataAccessTeams.LeaderBoardBestRndLeft(sSanctionID, sYrPkd, sTournName, "J", sDivisionCodePkd, sRndsPkd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)

            Case "S"
                ColumnOne.InnerHtml = ModDataAccessTeams.LeaderBoardBestRndLeft(sSanctionID, sYrPkd, sTournName, "S", sDivisionCodePkd, sRndsPkd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)


            Case "T"
                ColumnOne.InnerHtml = ModDataAccessTeams.LeaderBoardBestRndLeft(sSanctionID, sYrPkd, sTournName, "T", sDivisionCodePkd, sRndsPkd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)

            Case "J"
                ColumnOne.InnerHtml = ModDataAccessTeams.LeaderBoardBestRndLeft(sSanctionID, sYrPkd, sTournName, "J", sDivisionCodePkd, sRndsPkd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)


        End Select
    End Sub
End Class