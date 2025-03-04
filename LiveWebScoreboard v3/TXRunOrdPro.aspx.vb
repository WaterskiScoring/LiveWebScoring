Public Class TXRunOrdPro
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load





        OnWater.InnerHtml = ""
        ColumnLeft.InnerHtml = "Running Order List"
        ColumnRight.InnerHtml = "By DV, EV, Rnd in Placement Order"

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
        Dim sOWSlalom As String = ""
        Dim sOWTrick As String = ""
        Dim sOWJump As String = ""

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
            'Don't need these passed in - get from array below which is needed for Events

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
            If Not Regex.IsMatch(sFormatCode, "^[DVMROPro]*$") Then
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
            End If
            If sFormatCode <> "DV" And sFormatCode <> "MR" And sFormatCode <> "RO" And sFormatCode <> "ROPro" Then
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
            TName.InnerHtml = "<h3>" & sTournName & " - " & sSanctionID & "</h3> <span class=""bg-danger text-white"" >  UNOFFICIAL </span> "   'Tournament Name is just displayed so not validated
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
                HF_SlalomRnds.Value = sSlalomRounds
                sEventCount += 1
            End If
            If sArrSpecs(7, 2) > 0 Then
                sTrickRounds = sArrSpecs(6, 2)
                HF_TrickRnds.Value = sTrickRounds
                sEventCount += 1
            End If
            If sArrSpecs(8, 2) > 0 Then
                sJumpRounds = sArrSpecs(6, 2)
                HF_JumpRnds.Value = sJumpRounds
                sEventCount += 1
            End If
            'Load the Events Drop List  and set selectedvalue to match selection on tournament page
            With DDL_EventsPkd
                .Items.Clear()
                If sSlalomRounds > 0 Then
                    .Items.Add(New ListItem("Slalom", "S"))
                End If
                If sTrickRounds > 0 Then
                    .Items.Add(New ListItem("Trick", "T"))
                End If
                If sJumpRounds > 0 Then
                    .Items.Add(New ListItem("Jump", "J"))
                End If
                .SelectedValue = sEventCodePkd
            End With
            HF_SlalomRnds.Value = sSlalomRounds
            HF_TrickRnds.Value = sTrickRounds
            HF_JumpRnds.Value = sJumpRounds
            'can add round selecter here - have the rounds for each event.
            'Load the Divisions Drop List and set selectedvalue to match selection on tournament page
            sMsg = ModDataAccess.LoadDvList(sSanctionID, sEventCodePkd, sDivisionCodePkd, sRndsPkd, DDL_DvPkd, sSlalomRounds, sTrickRounds, sJumpRounds, DDL_PkRnd)
            If sMsg <> "Success" Then
                lbl_Errors.Text = sMsg  'friendly error message
                Exit Sub
            End If
            sEventCodePkd = DDL_EventsPkd.SelectedValue
            sFormatCode = "ROPro"  'is always ROPro in form TXRnOrdPro.aspx
            If sSlalomRounds > 0 Then
                sOWSlalom = ModDataAccess.LoadOnWaterSlalom(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
                If Left(sOWSlalom, 2) <> "No" Then OnWater.InnerHtml = sOWSlalom
            End If
            If sTrickRounds > 0 Then
                sOWTrick = ModDataAccess.LoadOnWaterTrick(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
                If Left(sOWTrick, 2) <> "No" Then OnWater.InnerHtml += sOWTrick
            End If
            If sJumpRounds > 0 Then
                sOWJump = ModDataAccess.LoadOnWaterJump(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
                If Left(sOWJump, 2) <> "No" Then OnWater.InnerHtml += sOWJump
            End If

            ColumnLeft.InnerHtml = ModDataAccess.GetNamesXRunOrder(sSanctionID, sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
            ColumnRight.InnerHtml = ModDataAccess.DisplayXRndandDv(sSanctionID, sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd), False, False)


                'TO DO - IN TOURNAMENTS ADD SELECTEDINDEXCHANGED - FORCE SELECTION OF EVENT DIVISION AND ROUND
                'IN MODDATAACCESS - MAKE RUN ORDER DISPLAY BY EVENT DV ROUND - INCLUDE NAMES (AND MAYBE FEDERATION) 
                'IN MODDATAACCESS - MAKE PLACEMENTxDIVISION EVENT ROUND

            End If 'end of if not ispostback
    End Sub

    Private Sub Btn_UpdateDisplay_Click(sender As Object, e As EventArgs) Handles Btn_UpdateDisplay.Click
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sDivisionCodePkd As String = DDL_DvPkd.SelectedValue
        Dim sEventCodePkd As String = DDL_EventsPkd.SelectedValue
        Dim sRndsPkd As String = DDL_PkRnd.SelectedValue
        Dim sJumpRounds As String = HF_JumpRnds.Value
        Dim sSlalomrounds As String = HF_SlalomRnds.Value
        Dim sTrickRounds As String = HF_TrickRnds.Value
        Dim sOWSlalom As String = ""
        Dim sOWTrick As String = ""
        Dim sOWJump As String = ""
        If sSlalomRounds > 0 Then
            sOWSlalom = ModDataAccess.LoadOnWaterSlalom(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
            If Left(sOWSlalom, 4) <> "Hide" Then OnWater.InnerHtml += sOWSlalom
        End If
        If sTrickRounds > 0 Then
            sOWTrick = ModDataAccess.LoadOnWaterTrick(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
            If Left(sOWTrick, 4) <> "Hide" Then OnWater.InnerHtml += sOWTrick
        End If
        If sJumpRounds > 0 Then
            sOWJump = ModDataAccess.LoadOnWaterJump(sSanctionID)  ', sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
            If Left(sOWJump, 4) <> "Hide" Then OnWater.InnerHtml += sOWJump
        End If

        ColumnLeft.InnerHtml = ModDataAccess.GetNamesXRunOrder(sSanctionID, sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd))
        ColumnRight.InnerHtml = ModDataAccess.DisplayXRndandDv(sSanctionID, sEventCodePkd, sDivisionCodePkd, CStr(sRndsPkd), False, False)

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
End Class