Module ModDataDisplay
    Friend Function DisplayOrganizer(ByVal SanctionID As String, ByVal TourName As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal FormatCode As String, ByVal UseNOPS As Boolean, ByVal UseTeams As Boolean) As String

        'This controls the display of tournaments 
        'Code loads 1 column with an alphabetical list of all skiers entered in all events in [SanctionID].
        'Code loads second column with results from round 1 as they come in.  The cut off to proceed to Finals is clearly marked.
        'Code checks for scores in round 2 on every refresh.
        'If no scores in round 2 - continue to show placement in Round 1 in column 2.
        'When the first score is posted to round 2 - Put the original skier list in column 3. Put the round 1 results in column 2.
        'Put the names of skiers in the final round in column 1 and fill in their scores as they are posted.
        '.aspx page has the code for tournament names And navigation buttons .  
        '    It has a container divs that run at the server which allow information to be put into the form dynamically.
        '    <div> and a <div id= "PutGridHere" runat="server"></div>
        '   The code passed in contains the appropriate divs And bootstrap classes to build the grid with the scores data inside.
        '   Sorting and limiting numbers is done by the database query.
        '   Code here controls which columns are loaded, which data is presented etc. to meet the requirements of CO105 and other codes.
        Dim sTournName As String = ""
        Dim sReturn As String = ""
        Dim sEventCode As String = EventCode
        Dim sDivisionCode As String = DivisionCode
        Dim sFormatCode As String = FormatCode
        Dim sEventScore As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sText2Insert As String = ""
        Dim sUseNOPS As Boolean = UseNOPS
        Dim sUseTeams As Boolean = UseTeams
        'Check for any scores in round 2
        Dim sLastRoundWithScores As Byte = 0
        Dim i As Byte = 6
        'FIND OUT HOW MANY ROUNDS ALREADY HAVE SCORES.  START DISPLAY WITH LATEST ROUND
        Do Until i = 0
            sEventScore = ModDataAccess.ChkRnd4Scores(sSanctionID, "S", i)
            If Left(sReturn, 5) = "Error" Then
                Return sEventScore
                Exit Function
            End If
            Select Case sEventScore
                Case "0"
                Case Else   'capture the last round with scores here.
                    sLastRoundWithScores = i

            End Select
            If sLastRoundWithScores > 0 Then Exit Do
            i -= 1
        Loop

        '       Make a loop loading scores by round starting with the round found above.
        '       For j = i To 0

        '          If EventCode = "A" Then
        '              'build slalom trick jump and overall events in specified round
        '              Sql = ModDataAccess.BuildSQL(Rnd() = i, other variables)
        '              ? innerhtml += moddataaccess.
        '          End If
        '          sMsg = ModDataAccess.
        '          Loop
        '      Next
        Select Case sLastRoundWithScores
            Case 0 '  no scores found
                sText2Insert = "No Scores Found"

                'DISPLAY MESSAGE NO SCORES FOUND (YET)
                'DIV FOR ROUNDS 2 - 6 .VISIBLE = FALSE

             '   If i = 1 Then
             '       ' load col 1 with entry list and stop.
             '       ModDataAccess.GetEntryList(sSanctionID, sTournName, sEventCode, "ALL", 0)
             '   End If
            Case 1 ' load col 1 with round 1 scores  Just have to pass the round number
                'NOTE: Col 1 holds 3 events - if not offered hide or just display no scores.
                sText2Insert = Display1col(SanctionID, sEventCode, sDivisionCode, sFormatCode, sUseNOPS, sUseTeams)

            Case 2 ' load round 2 placements in col 1 and round 1 results in column 2
                sText2Insert = Display2col(SanctionID, sEventCode, sDivisionCode, sFormatCode, sUseNOPS, sUseTeams)

            Case 3 ' load round 3 placements in col 1, round 2 results in col 2, round 1 results in col3 
                sText2Insert = Display3col(SanctionID, sEventCode, sDivisionCode, sFormatCode, sUseNOPS, sUseTeams)

            Case 4 'load round 4 placements in col 1, round 3 results in col 2, round 2 results in col 3, round 1 results in col 4
                sText2Insert = Display4col(SanctionID, sEventCode, sDivisionCode, sFormatCode, sUseNOPS, sUseTeams)
            Case 5
                sText2Insert = Display5col(SanctionID, sEventCode, sDivisionCode, sFormatCode, sUseNOPS, sUseTeams)
            Case 6
                sText2Insert = Display6col(SanctionID, sEventCode, sDivisionCode, sFormatCode, sUseNOPS, sUseTeams)
            Case Else
                sText2Insert = "Error retrieving data. Invalid number of rounds."
        End Select
        Return sText2Insert

    End Function
    Friend Function Display1col(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal FormatCode As String, ByVal useNops As Boolean, ByVal useTeams As Boolean) As String 'As Array
        'DECIDE IF RETURN IS STRING OR ARRAY
        'COL 1 IS EMPTY UNTIL FIRST SKIER IS SCORED. Display no scores found (yet)
        'Entry list is a separate page

        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sTournName As String = ""  'Try passing nothing.  Not needed for this function
        Dim sRnd As Byte = 1
        Dim sEventCode As String = EventCode
        Dim sDvCode As String = DivisionCode
        Dim sFormatCode As String = FormatCode
        Dim sUseNOPS As Boolean = useNops
        Dim sUseTeams As Boolean = useTeams
        sReturn = ModDataAccess.IndivScoresXRound(sSanctionID, sEventCode, sDvCode, sRnd, sUseNOPS, sUseTeams, sFormatCode)
        Return sReturn
    End Function
    Friend Function Display2col(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal FormatCode As String, ByVal useNops As Boolean, ByVal useTeams As Boolean) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sRnd As Byte = 2
        Dim sEventCode As String = EventCode
        Dim sDvCode As String = DivisionCode
        Dim sFormatCode As String = FormatCode
        Dim sUseNOPS As Boolean = useNops
        Dim sUseTeams As Boolean = useTeams
        sReturn = ModDataAccess.IndivScoresXRound(sSanctionID, sEventCode, sDvCode, sRnd, sUseNOPS, sUseTeams, sFormatCode)
        Return sReturn
        Return sReturn
    End Function
    Friend Function Display3col(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal FormatCode As String, ByVal useNops As Boolean, ByVal useTeams As Boolean) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sRnd As Byte = 3
        Dim sEventCode As String = EventCode
        Dim sDvCode As String = DivisionCode
        Dim sFormatCode As String = FormatCode
         Dim sUseNOPS As Boolean = useNops
        Dim sUseTeams As Boolean = useTeams
        sReturn = ModDataAccess.IndivScoresXRound(sSanctionID, sEventCode, sDvCode, sRnd, sUseNOPS, sUseTeams, sFormatCode)
        Return sReturn
        '    Return sReturn
    End Function
    Friend Function Display4col(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal FormatCode As String, ByVal useNops As Boolean, ByVal useTeams As Boolean) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sRnd As Byte = 4
        Dim sEventCode As String = EventCode
        Dim sDvCode As String = DivisionCode
        Dim sFormatCode As String = FormatCode
         Dim sUseNOPS As Boolean = useNops
        Dim sUseTeams As Boolean = useTeams
        sReturn = ModDataAccess.IndivScoresXRound(sSanctionID, sEventCode, sDvCode, sRnd, sUseNOPS, sUseTeams, sFormatCode)
        Return sReturn
    End Function
    Friend Function Display5col(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal FormatCode As String, ByVal useNops As Boolean, ByVal useTeams As Boolean) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sRnd As Byte = 5
        Dim sEventCode As String = EventCode
        Dim sDvCode As String = DivisionCode
        Dim sFormatCode As String = FormatCode
         Dim sUseNOPS As Boolean = useNops
        Dim sUseTeams As Boolean = useTeams
        sReturn = ModDataAccess.IndivScoresXRound(sSanctionID, sEventCode, sDvCode, sRnd, sUseNOPS, sUseTeams, sFormatCode)
        Return sReturn
    End Function
    Friend Function Display6col(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal FormatCode As String, ByVal useNops As Boolean, ByVal useTeams As Boolean) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sRnd As Byte = 6
        Dim sEventCode As String = EventCode
        Dim sDvCode As String = DivisionCode
        Dim sFormatCode As String = FormatCode
        Dim sUseNOPS As Boolean = useNops
        Dim sUseTeams As Boolean = useTeams
        sReturn = ModDataAccess.IndivScoresXRound(sSanctionID, sEventCode, sDvCode, sRnd, sUseNOPS, sUseTeams, sFormatCode)
        Return sReturn
    End Function

End Module