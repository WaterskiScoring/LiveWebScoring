Module ModDataDisplay

    Friend Function DisplayOrganizer(ByVal SanctionID As String, ByVal TourName As String, ByVal EventCode As String, ByVal FormatCode As String) As String

        'This controls the display of tournaments run with Preliminary round and Final(no tie break previous round)
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
        Dim sFormatCode As String = FormatCode
        Dim sEventScore As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sText2Insert As String = ""
        'Check for any scores in round 2
        Dim sLastRoundWithScores As Byte = 0
        Dim i As Byte = 4
        'FIND OUT HOW MANY ROUNDS ALREADY HAVE SCORES.  START WITH LATEST ROUND
        Do Until i = 0
            sEventScore = ModDataAccess.ChkRnd4Scores(sSanctionID, "S", i)
            If Left(sReturn, 5) = "Error" Then
                Return sEventScore
                Exit Function
            End If
            Select Case sEventScore
                Case "0"
                Case Else
                    sLastRoundWithScores = i

            End Select
            If sLastRoundWithScores > 0 Then Exit Do
            i -= 1
        Loop
        Select Case sLastRoundWithScores
            Case 0 '  no scores found
                If i = 1 Then
                    ' load col 1 with entry list and stop.
                    ModDataAccess.GetEntryListPro(sSanctionID, sTournName, sEventCode, "ALL", 0)
                End If
            Case 1 ' load col 2 with skier list and col 1 with empty placements round 1
                'NOTE: Col 1 holds 3 events - if not offered hide or just display no scores.
                sText2Insert = Display1col(SanctionID, sEventCode, sFormatCode)

            Case 2 ' load round 2 placements in col 1 and round 1 results in column 2
                sText2Insert = Display2col(SanctionID, sEventCode, sFormatCode)

            Case 3 ' load round 3 placements in col 1, round 2 results in col 2, round 1 results in col3 
                sText2Insert = Display3col(SanctionID, sEventCode, sFormatCode)

            Case 4 'load round 4 placements in col 1, round 3 results in col 2, round 2 results in col 3, round 1 results in col 4
                sText2Insert = Display4col(SanctionID, sEventCode, sFormatCode)

            Case Else
                sText2Insert = "Error retrieving data. Invalid number of rounds."
        End Select
        Return sText2Insert
        Return sReturn
    End Function
    Friend Function Display1col(ByVal SanctionID As String, ByVal EventCode As String, ByVal FormatCode As String) As String 'As Array
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sTournName As String = ""  'Try passing nothing.  Not needed for this function
        Dim sEventCode As String = EventCode
        Dim sFormatCode As String = FormatCode
        Dim sRnd As Byte = 1
        ModDataAccess.GetEntryListPro(sSanctionID, sTournName, sEventCode, "ALL", 0)
        ModDataAccess.PlacementXRound(sSanctionID, sFormatCode, sEventCode, sRnd)
        Return sReturn
    End Function
    Friend Function Display2col(ByVal SanctionID As String, ByVal EventCode As String, ByVal FormatCode As String) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = EventCode
        Dim sRnd As Byte = 2
        Dim sFormatCode As String = FormatCode
        ModDataAccess.PlacementXRound(sSanctionID, sFormatCode, sEventCode, sRnd)
        Return sReturn
    End Function
    Friend Function Display3col(ByVal SanctionID As String, ByVal EventCode As String, ByVal FormatCode As String) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = EventCode
        Dim sRnd As Byte = 3
        Dim sFormatCode As String = FormatCode
        ModDataAccess.PlacementXRound(sSanctionID, sFormatCode, sEventCode, sRnd)
        Return sReturn
    End Function
    Friend Function Display4col(ByVal SanctionID As String, ByVal EventCode As String, ByVal FormatCode As String) As String
        Dim sReturn As String = ""
        'Need different Column setup
        Return sReturn
    End Function
End Module
