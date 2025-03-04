Imports System.Security.Cryptography.X509Certificates

Public Class TRecap
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sEM As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = ""
        Dim sMemberID As String = ""
        Dim sSkierName As String = ""
        Dim sYrPkd As String = "0"  'default is 0 = recent
        Dim sFormatCode As String = ""
        Dim sTournName As String = ""
        Dim sEventCodePkd As String = ""  'Could set to "A" as default
        Dim sEvent As String = ""
        Dim sDivisionCodePkd As String = "" 'Could set to "A" as default
        Dim sAgeGroup As String = ""
        Dim sRndsPkd As Int16 = 0
        Dim sSlalomRounds As String = "0"
        Dim sTrickRounds As String = "0"
        Dim sJumpRounds As String = "0"
        Dim sEventCount As Int16 = 0
        Dim sUseNOPS As Boolean = False
        Dim sUseTeams As Boolean = False
        Dim sDisplay As String = ""
        If Not IsPostBack Then


            ' VALIDATE ALL VALUES RECEIVED FROM QUERY STRING.
            If Request("SID") Is Nothing Or Request("SN") Is Nothing Or Request("SY") = Nothing Or Request("MID") = Nothing Or Request("TN") = Nothing Or Request("FC") = Nothing Or Request("FT") = Nothing Or Request("EV") = Nothing Or Request("DV") = Nothing Or Request("RP") = Nothing Or Request("UN") = Nothing Or Request("UT") = Nothing Then
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
                Exit Sub
            End If

            Try
                sSanctionID = Request("SID")
                sSkierName = Request("SN")
                sYrPkd = Request("SY")
                sTournName = CStr(Request("TN"))
                HF_TournName.Value = Request("TN") 'not used in sql - just pass through
                sFormatCode = Request("FC")
                sEventCodePkd = Request("EV")
                sDivisionCodePkd = Request("DV")
                sRndsPkd = CInt(Request("RP"))  'If not an integer will throw an esception Allowed alues 0,1,2,3,4
                sMemberID = Request("MID")
                If Not Regex.IsMatch(CStr(sRndsPkd), "^[01234]$") Then  '0 means all.  allowed values for rounds 0,1,2,3,4
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                    Exit Sub
                End If
                HF_RndPkd.Value = sRndsPkd
                If Not Regex.IsMatch(sSanctionID, "^[0-9][0-9][CEMSWUX][0-9][0-9][0-9]$") Then  'Include X to include Dave's test tournament 24X017
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                    Exit Sub
                End If
                HF_SanctionID.Value = sSanctionID
                If CStr(sYrPkd) <> "0" Then   '0 = recent which is default
                    If Not Regex.IsMatch(sYrPkd, "^[2-9][0-9]$") Then
                        sEM = "Invalid Request.  You may try again."
                        Session.Abandon()
                        Response.Redirect("~/default.aspx?EM=" & sEM)
                        Exit Sub
                    End If
                End If
                HF_YearPkd.Value = sYrPkd
                If Not Regex.IsMatch(sMemberID, "^[0-9]*$") Then
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                    Exit Sub
                End If
                If Not Regex.IsMatch(sEventCodePkd, "^[ASTJO]$") Then
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                End If
                sEvent = sEventCodePkd
                HF_Event.Value = sEvent
                If Not Regex.IsMatch(sDivisionCodePkd, "^[A-Za-z1-9]*$") Then
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                    Exit Sub
                End If
                HF_AgeGroup.Value = sDivisionCodePkd
                sAgeGroup = sDivisionCodePkd
                If Not Regex.IsMatch(sFormatCode, "^[DVMROPNCWELBS]*$") Then
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                End If
                If sFormatCode <> "LB" And sFormatCode <> "LBSP" And sFormatCode <> "DV" And sFormatCode <> "MR" And sFormatCode <> "RO" And sFormatCode <> "NCWL" And sFormatCode <> "NCWRO" And sFormatCode <> "ROPro" And sFormatCode <> "PRO" And sFormatCode <> "EL" Then
                    sEM = "Invalid Request.  You may try again."
                    Session.Abandon()
                    Response.Redirect("~/default.aspx?EM=" & sEM)
                End If
                HF_FormatCode.Value = sFormatCode
                If sFormatCode = "EL" Then Btn_Back.Text = "Back to Entry List"

                sUseNOPS = CBool(Request("UN"))
                sUseTeams = CBool(Request("UT"))
                HF_UseNOPS.Value = sUseNOPS
                HF_UseTeams.Value = sUseTeams
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
                    sTrickRounds = sArrSpecs(7, 2)
                    sEventCount += 1
                End If
                If sArrSpecs(8, 2) > 0 Then
                    sJumpRounds = sArrSpecs(8, 2)
                    sEventCount += 1
                End If
            Catch ex As Exception
                sEM = "Invalid Request.  You may try again."
                sErrDetails = ex.Message & "<br>" & ex.StackTrace
                Session.Abandon()
                Response.Redirect("~/default.aspx?EM=" & sEM)
            End Try

            'Not used in sql statement so just display what is there
            TName.InnerHtml = "<h4>" & sTournName & " - " & sSanctionID & "</h4>"
            'For recap display all events offered.
            If sSlalomRounds > 0 Then
                SlalomRecap.InnerHtml = ModDataAccess3.RecapSlalom(sSanctionID, sMemberID, sAgeGroup, sSkierName)
            End If
            If sTrickRounds > 0 Then
                TrickRecap.InnerHtml = ModDataAccess3.RecapTrick(sSanctionID, sMemberID, sAgeGroup, sSkierName)
            End If
            If sJumpRounds > 0 Then
                JumpRecap.InnerHtml = ModDataAccess3.RecapJump(sSanctionID, sMemberID, sAgeGroup, sSkierName)
            End If
            If sEventCount > 2 Then
                OverallRecap.InnerHtml = ModDataAccess3.RecapOverall(sSanctionID, sMemberID, sSkierName)
            End If
        End If

    End Sub

    Private Sub Btn_Back_Click(sender As Object, e As EventArgs) Handles Btn_Back.Click
        Dim sFormatCode As String = HF_FormatCode.Value
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sYrPkd As String = HF_YearPkd.Value
        Dim sEventCode As String = HF_Event.Value
        Dim sDV As String = HF_AgeGroup.Value
        Dim sRndPkd As String = HF_RndPkd.Value
        Dim sTournName As String = HF_TournName.Value
        Dim sUseNops As String = HF_UseNOPS.Value
        Dim sUseTeams As String = HF_UseTeams.Value

        Dim sFormToUse As String = ""
        'Add the other fields
        'Need parameter to show single run order or multiples
        'Add a parameter that tells calling form to set Event, DV and Rnd drop lists to specified selected index.
        Select Case sFormatCode
            Case "LB"
                sFormToUse = "TLeaderBoard.aspx"
            Case "LBSP"
                sFormToUse = "TLeaderBoardSP.aspx"
            Case "RO"
                sFormToUse = "TROxRnd.aspx"
            Case "NCWL"
                sFormToUse = "TLeaderBoardNCW.aspx"
            Case "NCWRO"
                sFormToUse = "TROandBrNCW.aspx"
            Case "PRO"
                sFormToUse = "TPro.aspx"
            Case "EL"
                sFormToUse = "TSkierListPro"
        End Select
        'parameter FT (from Tournament.aspx) is always 0 false
        Response.Redirect(sFormToUse & "?SY=" & sYrPkd & "&SID=" & sSanctionID & "&TN=" & sTournName & "&FC=" & sFormatCode & "&FT=0&EV=" & sEventCode & "&DV=" & sDV & "&RP=" & sRndPkd & "&UN=" & sUseNops & "&UT=" & sUseTeams & "")
    End Sub
End Class