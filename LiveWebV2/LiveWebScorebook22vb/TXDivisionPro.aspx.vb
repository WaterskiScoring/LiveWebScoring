Public Class TXDivisionPro
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Get the sanction number from the query string.
        'SN and TN and SY must always be present.  Check for other variables based on FM
        If Request("SN") Is Nothing Or Request("SY") = Nothing Or Request("TN") = Nothing Or Request("FC") = Nothing Then
            lbl_Errors.Text = "Missing Parameter"
            Exit Sub
        End If
        Dim sUseNOPS As Boolean = False
        If Request("UN") = Nothing Then
            sUseNOPS = False
        Else
            sUseNOPS = CBool(Request("UN"))
        End If
        Dim sSanctionID As String = Request("SN")
        Dim sYrPkd As String = Request("SY")
        Dim sTournName As String = Request("TN")
        Dim sFormatCode As String = Request("FC")
        Dim sEventCode As String = "S"
        TName.InnerHtml = "<h3>" & sTournName & " - " & sSanctionID & "</h3>"
        HF_SanctionID.Value = sSanctionID
        HF_YearPkd.Value = sYrPkd  ' = ddl_YrPkd.selectedvalue
        HF_TournName.Value = sTournName
        HF_FormatCode.Value = sFormatCode
        HF_UseNOPS.Value = sUseNOPS
        'Display Results
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
        End Select
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
End Class