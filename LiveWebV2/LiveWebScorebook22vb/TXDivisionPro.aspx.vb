Imports System.Text.RegularExpressions
Public Class TXDivisionPro
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
        Dim sEventCode As String = ""  'Could set to "A" as default
        Dim sDivisionCode As String = "" 'Could set to "A" as default
        Dim sUseNOPS As Boolean = False
        Dim sSlalomRounds As Int16 = 0
        Dim sTrickRounds As Int16 = 0
        Dim sJumpRounds As Int16 = 0
        Dim sEventCount As Int16 = 0


        ' VALIDATE ALL VALUES RECEIVED FROM QUERY STRING.
        If Request("SN") Is Nothing Or Request("SY") = Nothing Or Request("TN") = Nothing Or Request("FC") = Nothing Or Request("EV") = Nothing Or Request("DV") = Nothing Or Request("UN") = Nothing Then
            lbl_Errors.Text = "Missing Parameter"
            sEM = "Invalid Request.  You may try again."
            Session.Abandon()
            Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
            Exit Sub
        End If

        Try
            sUseNOPS = CBool(Request("UN"))
            sSanctionID = Request("SN")
            sYrPkd = Request("SY")
            sTournName = CStr(Request("TN"))
            sFormatCode = Request("FC")
            sEventCode = Request("EV")
            sDivisionCode = Request("DV")
        Catch ex As Exception
            lbl_Errors.Text = "Invalid Parameter"
            sEM = "Invalid Request.  You may try again."
            Session.Abandon()
            Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
            Exit Sub
        End Try

        Try


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
            If Not Regex.IsMatch(sEventCode, "^[ASTJ]$") Then
                lbl_Errors.Text = "Invalid Parameter"
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
                Exit Sub
            End If
            If Not Regex.IsMatch(sDivisionCode, "^[A-Za-z1-9]*$") Then
                lbl_Errors.Text = "Invalid Parameter"
                sEM = "Invalid Request.  You may try again."
                Session.Abandon()
                Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
                Exit Sub
            End If
        Catch ex As Exception
            sErrDetails = ex.Message & "<br>" & ex.StackTrace
            sEM = "Invalid Request.  You may try again."
            Session.Abandon()
            Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
        End Try

        TName.InnerHtml = "<h3>" & sTournName & " - " & sSanctionID & "</h3>"
        HF_SanctionID.Value = sSanctionID
        HF_YearPkd.Value = sYrPkd  ' = ddl_YrPkd.selectedvalue
        HF_TournName.Value = sTournName
        HF_FormatCode.Value = sFormatCode
        HF_UseNOPS.Value = sUseNOPS
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
        With DDL_Events
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
            .SelectedValue = sEventCode
        End With
        'can add round selecter here - have the rounds for each event.
        'Load the Divisions Drop List and set selectedvalue to match selection on tournament page
        sMsg = ModDataAccess.LoadDvList(sSanctionID, sEventCode, DDL_Divisions)
        If sMsg <> "Success" Then
            lbl_Errors.Text = sMsg  'friendly error message
            Exit Sub
        End If
        DDL_Divisions.SelectedValue = sDivisionCode
        'Display Results

        'NOW HAVE EVENT AND DIVISION SELECTION TO FILTER RECORDS ACCESSED BELOW. 11/18/2023

        Select Case sFormatCode
            Case "0" 'Display entry list only  NEED ONE COLUMN LAYOUT
                ColumnOne.InnerHtml = ModDataAccess.GetEntryListPro(sSanctionID, sTournName, "A", "ALL", sYrPkd)

            Case "BestRnd", "CO102", "CO103", "CO104"  'THESE WILL NEVER BE REACHED UNLESS FormatCode is accurate.  Need 2 codes  BestRound and ByRound.  Both look for scores
                If sUseNOPS = False Then
                    ColumnOne.InnerHtml = ModDataAccess.PlacementBestRound(sSanctionID, "CO103", "S")
                    ColumnTwo.InnerHtml = ModDataAccess.PlacementBestRound(sSanctionID, "CO102", "T")
                    ColumnThree.InnerHtml = ModDataAccess.PlacementBestRound(sSanctionID, "CO101", "J")
                Else
                    ColumnOne.InnerHtml = ModDataAccess.PlacementBestRoundNOPS(sSanctionID, "CO103", "S")
                    ColumnTwo.InnerHtml = ModDataAccess.PlacementBestRoundNOPS(sSanctionID, "CO102", "T")
                    ColumnThree.InnerHtml = ModDataAccess.PlacementBestRoundNOPS(sSanctionID, "CO101", "J")
                End If

            Case "ByRound"  'NEED TO SPECIFY ROUNDS AND SWITCH COLUMNS AS ROUNDS PROCEED
                ColumnOne.InnerHtml = ModDataDisplay.DisplayOrganizer(sSanctionID, sTournName, sEventCode, sFormatCode)
                ColumnTwo.InnerHtml = ModDataDisplay.DisplayOrganizer(sSanctionID, sTournName, sEventCode, sFormatCode)
                ColumnThree.InnerHtml = ModDataDisplay.DisplayOrganizer(sSanctionID, sTournName, sEventCode, sFormatCode)
            Case "RndsCombo"
                'Need more information - How many rounds combined?  what follows?
            Case Else 'unexpected value - kick out

        End Select

        'FOR DEBUG - DELETE HERE AND ON ASPX WHEN DONE
        Dim sTID As String = "23S108"
        Dim sSID As String = "000181068"
        Dim sAgeGroup As String = "JM"
        Dim sSkrName As String = "Tristan Duplan-Fribourg"
        Dim slink As String = "<a asp-page=""Trecap.aspx?SID=" & sTID & "&MID=" & sSID & "&DV=" & sAgeGroup & "&SN=" & sSkrName & """>HelperLink</a>"
        HyperLink2.NavigateUrl = "Trecap.aspx?SID=" & sTID & "&MID=" & sSID & "&DV=" & sAgeGroup & "&SN=" & sSkrName
        sSID = "000107150"
        sAgeGroup = "OM"
        sSkrName = "James Bryans"
        HyperLink1.NavigateUrl = "Trecap.aspx?SID=" & sTID & "&MID=" & sSID & "&DV=" & sAgeGroup & "&SN=" & sSkrName
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

    Private Sub Btn_UpdateDisplay_Click(sender As Object, e As EventArgs) Handles Btn_UpdateDisplay.Click
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sFormatCode = HF_FormatCode.Value
        Dim sTournName As String = HF_TournName.Value
        Dim sYrPkd As String = HF_YearPkd.Value
        Dim sUseNOPS As String = HF_UseNOPS.Value
        Dim sEventCode As String = "S"
        Select Case sFormatCode
            Case "0" 'Display entry list only  NEED ONE COLUMN LAYOUT
                ColumnOne.InnerHtml = ModDataAccess.GetEntryListPro(sSanctionID, sTournName, "A", "ALL", sYrPkd)

            Case "BestRnd", "CO102", "CO103", "CO104"  'THESE WILL NEVER BE REACHED UNLESS FormatCode is accurate.  Need 2 codes  BestRound and ByRound.  Both look for scores
                If sUseNOPS = False Then
                    ColumnOne.InnerHtml = ModDataAccess.PlacementBestRound(sSanctionID, "CO103", "S")
                    ColumnTwo.InnerHtml = ModDataAccess.PlacementBestRound(sSanctionID, "CO102", "T")
                    ColumnThree.InnerHtml = ModDataAccess.PlacementBestRound(sSanctionID, "CO101", "J")
                Else
                    ColumnOne.InnerHtml = ModDataAccess.PlacementBestRoundNOPS(sSanctionID, "CO103", "S")
                    ColumnTwo.InnerHtml = ModDataAccess.PlacementBestRoundNOPS(sSanctionID, "CO102", "T")
                    ColumnThree.InnerHtml = ModDataAccess.PlacementBestRoundNOPS(sSanctionID, "CO101", "J")
                End If

            Case "ByRound"  'NEED TO SPECIFY ROUNDS AND SWITCH COLUMNS AS ROUNDS PROCEED
                'Check which rounds have scores
                ColumnOne.InnerHtml = ModDataDisplay.DisplayOrganizer(sSanctionID, sTournName, sEventCode, sFormatCode)
                ColumnTwo.InnerHtml = ModDataDisplay.DisplayOrganizer(sSanctionID, sTournName, sEventCode, sFormatCode)
                ColumnThree.InnerHtml = ModDataDisplay.DisplayOrganizer(sSanctionID, sTournName, sEventCode, sFormatCode)
        End Select
    End Sub

    Private Sub DDL_Events_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Events.SelectedIndexChanged
        Dim sEventCode As String = DDL_Events.SelectedValue
        Dim sSanctionID As String = HF_SanctionID.Value
        Dim sMsg As String = ""
        sMsg = ModDataAccess.LoadDvList(sSanctionID, sEventCode, DDL_Divisions)
        If sMsg <> "Success" Then
            lbl_Errors.Text = sMsg
        End If
    End Sub
End Class