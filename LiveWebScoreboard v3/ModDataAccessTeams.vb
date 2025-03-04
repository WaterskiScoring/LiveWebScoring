
Imports System.Data.Common
Imports System.Data.OleDb
Imports System.Security.Policy
Module ModDataAccessTeams

    Public Function SQLstatements()

        'Just a place to put various useful queries
        Dim sSQL As String = ""

        sSQL = "Select Case SanctionID, TeamCode, OverallPlcmt  From livewebscoreboard.dbo.TeamScore Where SanctionId = '24U268' order by OverallPlcmt asc"
        sSQL = "Select Case SanctionID, TeamCode, SlalomPlcmt  From livewebscoreboard.dbo.TeamScore Where SanctionId = '24U268' order by SlalomPlcmt asc"
        sSQL = "Select Case SanctionID, TeamCode, TrickPlcmt  From livewebscoreboard.dbo.TeamScore Where SanctionId = '24U268' order by TrickPlcmt asc"
        sSQL = "Select Case SanctionID, TeamCode,  JumpPlcmt From livewebscoreboard.dbo.TeamScore Where SanctionId = '24U268' order by JumpPlcmt asc"

        sSQL = "Select * from livewebscoreboard.dbo.teamscoredetail where sanctionID = '24U268'"
        sSQL = "Select SanctionID, TeamCode, AgeGroup, Name, OverallPlcmt, SlalomPlcmt, TrickPlcmt, JumpPlcmt, OverallScore, SlalomScore,TrickScore,JumpScore "
        sSQL += "From livewebscoreboard.dbo.TeamScore Where sanctionID = '24U268' and AgeGroup = 'CM' Order By OverallPlcmt desc"

        sSQL = "Select * from TeamScoreDetil where sanctionID = '24U268'  Order by Event AgeGroup, TeamCode, LineNum"
        'DIFFERENT NAMES FOR EACH EVENT ACROSS ROW.  
        'SELECT SANCTIONID TEAMCODE, AGEGROUP,lINEnUMBER sLALOMsKIERnAME
        Return sSQL
    End Function

    Public Function LeaderBoardBestRndLeft(ByVal SanctionID As String, ByVal SkiYr As String, ByVal TName As String, ByVal selEvent As String, ByVal selDv As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNOPS As Int16, ByVal UseTeams As Int16, ByVal selFormat As String, ByVal DisplayMetric As Int16) As String
        'This function is run for each event selected based on code in TLeaderBoard_Load and Btn_Update
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sStopHere As String = ""
        Dim sFormatCode As String = "LB"
        Dim sSanctionID As String = SanctionID
        Dim sSkiYear As String = SkiYr
        Dim sTName As String = TName
        Dim sEventPkd As String = selEvent  'Event passed in as S,T,J,orA
        Dim sSelEvent As String = ""  'Event passed in as Slalom, Trick, Jump
        Dim sSelRnd As String = selRnd   'Round selected as a filter
        Dim sSelDV As String = selDv    'Div selected as a filter
        Dim sRndsSlalomOffered = RndsSlalomOffered
        Dim sRndsTrickOffered = RndsTrickOffered
        Dim sRndsJumpOffered = RndsJumpOffered
        Dim sRndsThisEvent As String = ""  'generic form of RndsSSlalomOffered, etc.
        Dim sRoundsHTML As String = ""  'string of <td>Rnd " & i & "</td> one for each round in an event
        Dim sRndCols As Int16 = 0  'Number of <td></td> sections in the table based on rounds offered + one column for Name and best score
        Dim sRound As String = ""  'Round in which best score was achieved
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sEvent As String = selEvent
        Dim sScoreBest As String = ""
        Dim sEventScore As String = ""
        Dim stmpMemberID As String = ""
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim sDv As String = ""
        Dim sTmpDv As String = ""
        Dim sEventClass As String = ""
        Dim sEventGroup As String = ""
        Dim sCity As String = ""
        Dim sState As String = ""
        Dim sFederation As String = ""
        Dim sRankingScore As String = ""
        Dim sNopsScore As String = ""
        Dim sUseNOPS As Int16 = UseNOPS
        Dim sUseTeams As Int16 = UseTeams
        Dim sTeamCode As String = ""
        Dim sSlalomHeader As String = ""
        Dim sTrickHeader As String = ""
        Dim sUnit As String = ""
        Dim sJumpHeader As String = ""
        Dim sLine As New StringBuilder
        Dim sMultiRndScores As String = ""
        Dim sSql As String = ""
        Select Case selEvent
            Case "S"
                sSelEvent = "Slalom"
                sSql = " PrLeaderboard"
 '               sUnit = " Buoys"
                'NCWSA ALWAYS 1 ROUND

            Case "T"
                sSelEvent = "Trick"
                sSql = "PrLeaderBoard"
                sRndsThisEvent = sRndsTrickOffered
 '               sUnit = " Points"

            Case "J"
                sSelEvent = "Jump"
                sSql = "PrLeaderBoard"

            Case Else  'Load all by default
                sMsg = "<td>Event Code out of range</td></tr>"
                Return sMsg
                Exit Function
        End Select
        '       sLine.Append("<Table Class=""table  table-bordered border-primary "">") '& sJumpHeader)  
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: GetRunOrder could not get connection string."
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        cmdRead.CommandType = CommandType.StoredProcedure
        cmdRead.CommandText = sSql
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InEvCode", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InEvCode").Size = 12
        cmdRead.Parameters("@InEvCode").Value = sSelEvent
        cmdRead.Parameters("@InEvCode").Direction = ParameterDirection.Input


        cmdRead.Parameters.Add("@InFormat", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InFormat").Size = 12
        cmdRead.Parameters("@InFormat").Value = "All"    '0 = All Rounds    sSelRnd
        cmdRead.Parameters("@InFormat").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sSelDV   'This is the division selected for display.  sDv is the division in which the skier is performing.
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input

        '    cmdRead.Parameters.Add("@InGroup", OleDb.OleDbType.VarChar)
        '    cmdRead.Parameters("@InGroup").Size = 3
        '    cmdRead.Parameters("@InGroup").Value = selDv   'sEventGroup
        '    cmdRead.Parameters("@InGroup").Direction = ParameterDirection.Input

        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()
                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))

                            If Not IsDBNull(MyDataReader.Item("DiV")) Then
                                sDv = CStr(MyDataReader.Item("DiV"))
                            Else
                                sDv = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("TeamCode")) Then
                                sTeamCode = CStr(MyDataReader.Item("TeamCode"))
                            Else
                                sTeamCode = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("Round")) Then
                                sRound = CStr(MyDataReader.Item("Round"))
                            Else
                                sRound = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("MemberID")) Then
                                sMemberID = MyDataReader.Item("MemberID")
                            Else
                                sMemberID = ""
                            End If
                            '                           If Not IsDBNull(MyDataReader.Item("ScoreBest")) Then
                            '                               sScoreBest = MyDataReader.Item("ScoreBest")
                            '                           Else
                            '                               sScoreBest = ""
                            '                           End If
                            If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEventScoreDesc = MyDataReader.Item("EventScoreDesc")
                            Else
                                sEventScoreDesc = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = MyDataReader.Item("EventClass")
                            Else
                                sEventClass = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("City")) Then
                                sCity = MyDataReader.Item("City")
                            Else
                                sCity = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("State")) Then
                                sState = MyDataReader.Item("State")
                            Else
                                sState = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("Federation")) Then
                                sFederation = MyDataReader.Item("Federation")
                            Else
                                sFederation = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("RankingScore")) Then
                                sRankingScore = MyDataReader.Item("RankingScore")
                            Else
                                sRankingScore = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("NOPSScore")) Then
                                sNopsScore = MyDataReader.Item("NOPSScore")
                            Else
                                sNopsScore = ""
                            End If
                            If sTmpDv = "" Then
                                'Add the division header for first division
                                sLine.Append("<table class=""table"" width=""100%"">")
                                sLine.Append("<tr><td class=""table-warning"" width=""25%""><b> Leader Board </b></td>")
                                '   sLine.Append("<td Class=""table-primary"" width=""5%""></td>")
                                sLine.Append("<td Class=""table-primary""><b>" & UCase(sSelEvent) & "</b></td>")
                                sLine.Append("<td Class=""table-primary"" colspan=""2""><b>Group: " & sDv & " &nbsp;</b><span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td></tr>")
                                sTmpDv = sDv
                            End If
                            'Get the first MemberID' first record in first pass through data
                            If stmpMemberID = "" Then stmpMemberID = sMemberID

                            If sTmpDv = sDv Then 'Continue in same Division
                                'Add the data line
                                sLine.Append("<tr><td class=""table-warning""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sSkiYear & "&MID=" & sMemberID & "&DV=" & sSelDV & "&EV=" & sEventPkd & "&TN=" & sTName & "")
                                sLine.Append("&FC=NCWL&FT=0&RP=1&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a></td>")   '   
                                sLine.Append("<td><b> " & sTeamCode & "</b></td>")
                                sLine.Append("<td><b> " & sScoreBest & " " & sUnit & "</b></td><td>" & sEventScoreDesc & "</td></tr>")
                                '   sMultiRndScores = ModDataAccessTeams.LBGetRndScores(sSanctionID, sMemberID, sSelEvent, sDv, sSelRnd, sRound, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sNopsScore)
                                '   If sMultiRndScores <> "Error" Then
                                '       sLine.Append(sMultiRndScores)
                                '       sMultiRndScores = ""
                                '   Else
                                '       'FIX THIS ERROR TRAP
                                '   End If
                            Else 'Division changed.

                                sLine.Append("</table>")
                                stmpMemberID = sMemberID
                                sTmpDv = sDv
                                'start new division header
                                sLine.Append("<table class=""table"" width=""100%"">")
                                sLine.Append("<tr><td class=""table-warning"" width=""25%""><b> Leader Board </b></td>")
                                '   sLine.Append("<td Class=""table-primary"" width=""5%""></td>")
                                sLine.Append("<td Class=""table-primary""><b>" & UCase(sSelEvent) & "</b></td>")
                                sLine.Append("<td Class=""table-primary"" colspan=""2""><b>Group: " & sDv & " &nbsp;</b><span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td></tr>")
                                'Add the data line

                                sLine.Append("<tr><td class=""table-warning""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sSkiYear & "&MID=" & sMemberID & "&DV=" & sSelDV & "&EV=" & sEventPkd & "&TN=" & sTName & "")
                                sLine.Append("&FC=NCWL&FT=0&RP=1&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a></td>")   '   
                                sLine.Append("<td><b> " & sTeamCode & "</b></td>")
                                sLine.Append("<td><b> " & sScoreBest & " " & sUnit & "</b></td><td>" & sEventScoreDesc & "</td></tr>")
                                '                                sMultiRndScores = ModDataAccessTeams.LBGetRndScores(sSanctionID, sMemberID, sSelEvent, sDv, sSelRnd, sRound, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sNopsScore)
                                '                                If sMultiRndScores <> "Error" Then
                                '                                    sLine.Append(sMultiRndScores)
                                '                                Else
                                '                                    'FIX THIS ERROR TRAP
                                '                                End If
                            End If

                        Loop
                        'Close the DV table for the specified division
                        sLine.Append("</table>")
                    Else
                        '      sLine.Append("<tr  class=""table-info""><td> " & sSkierName & "</td><td>No Scores</td></tr></table>")
                        sMsg = "No Scores"
                    End If

                End Using
            Catch ex As Exception
                sMsg = "Error at MDATeams.GetBestRndLeft"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try

        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
        Else
            sMsg = sLine.ToString()
            Return sLine.ToString()
        End If
    End Function
    Public Function LeaderBoardBestRndLeftOrig(ByVal SanctionID As String, ByVal SkiYr As String, ByVal TName As String, ByVal selEvent As String, ByVal selDv As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNOPS As Int16, ByVal UseTeams As Int16, ByVal selFormat As String, ByVal DisplayMetric As Int16) As String
        'This function is run for each event selected based on code in TLeaderBoard_Load and Btn_Update
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sStopHere As String = ""
        Dim sFormatCode As String = "LB"
        Dim sSanctionID As String = SanctionID
        Dim sSkiYear As String = SkiYr
        Dim sTName As String = TName
        Dim sEventPkd As String = selEvent  'Event passed in as S,T,J,orA
        Dim sSelEvent As String = ""  'Event passed in as Slalom, Trick, Jump
        Dim sSelRnd As String = selRnd   'Round selected as a filter
        Dim sSelDV As String = selDv    'Div selected as a filter
        Dim sRndsSlalomOffered = RndsSlalomOffered
        Dim sRndsTrickOffered = RndsTrickOffered
        Dim sRndsJumpOffered = RndsJumpOffered
        Dim sRndsThisEvent As String = ""  'generic form of RndsSSlalomOffered, etc.
        Dim sRoundsHTML As String = ""  'string of <td>Rnd " & i & "</td> one for each round in an event
        Dim sRndCols As Int16 = 0  'Number of <td></td> sections in the table based on rounds offered + one column for Name and best score
        Dim sRound As String = ""  'Round in which best score was achieved
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sEvent As String = selEvent
        Dim sScoreBest As String = ""
        Dim sEventScore As String = ""
        Dim stmpMemberID As String = ""
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim sDv As String = ""
        Dim sTmpDv As String = ""
        Dim sEventClass As String = ""
        Dim sEventGroup As String = ""
        Dim sCity As String = ""
        Dim sState As String = ""
        Dim sFederation As String = ""
        Dim sRankingScore As String = ""
        Dim sNopsScore As String = ""
        Dim sUseNOPS As Int16 = UseNOPS
        Dim sUseTeams As Int16 = UseTeams
        Dim sTeamCode As String = ""
        Dim sSlalomHeader As String = ""
        Dim sTrickHeader As String = ""
        Dim sUnit As String = ""
        Dim sJumpHeader As String = ""
        Dim sLine As New StringBuilder
        Dim sMultiRndScores As String = ""
        Dim sSql As String = ""
        Select Case selEvent
            Case "S"
                sSelEvent = "Slalom"
                sSql = " PrSlalomScoresBestByDiv"
                sUnit = " Buoys"
                'NCWSA ALWAYS 1 ROUND

'                If sSelRnd = "0" Then
'                    sRndCols = CStr(CInt(sRndsSlalomOffered) + 1)
'                    For j = 1 To RndsSlalomOffered
'                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>" 'completes the event header with appropriate number of rounds columns
'                    Next
'                    sRoundsHTML += "</tr>"
'                Else
'                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td>NOPS</td><td>Details</td><td>Time</td>"
'                    sRndCols = "6"
'                End If


            Case "T"
                sSelEvent = "Trick"
                sSql = "PrTrickScoresBestByDiv"
                sRndsThisEvent = sRndsTrickOffered
                sUnit = " Points"
'                If sSelRnd = "0" Then
'                    sRndCols = CStr(CInt(sRndsTrickOffered) + 1)  'Rnds + name + 2 for BestRnd
'                    For j = 1 To RndsTrickOffered
'                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"  'completes the event header with appropriate number of rounds columns
'                    Next
'                    sRoundsHTML += "</tr>"
'                Else
'                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td> Points </td><td>NOPS</td><td>Details</td><td>Time</td>"
'                    sRndCols = "6"
'                End If

            Case "J"
                sSelEvent = "Jump"
                sSql = "PrJumpScoresBestByDiv"
                sRndsThisEvent = sRndsJumpOffered
                sUnit = " Feet"
                '                If sSelRnd = "0" Then
                '                    sRndCols = CStr(CInt(sRndsTrickOffered) + 1)  'Rnds + name + 2 for BestRnd
                '                    For j = 1 To RndsTrickOffered
                '                        sRoundsHTML += "<td  Class=""table-primary"">Rnd " & j & "</td>"    'completes the event header with appropriate number of rounds columns
                '                    Next
                '                    sRoundsHTML += "</tr>"
                '                Else
                '                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td>Class</td><td> Ft/M </td><td>NOPS</td><td>Details</td><td>Time</td>"
                '                    sRndCols = "6"
                '                End If
            Case Else  'Load all by default
                sMsg = "<td>Event Code out of range</td></tr>"
                Return sMsg
                Exit Function
        End Select
        '       sLine.Append("<Table Class=""table  table-bordered border-primary "">") '& sJumpHeader)  
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: GetRunOrder could not get connection string."
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        cmdRead.CommandType = CommandType.StoredProcedure
        cmdRead.CommandText = sSql
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

        '       cmdRead.Parameters.Add("@InEvCode", OleDb.OleDbType.VarChar)
        '       cmdRead.Parameters("@InEvCode").Size = 12
        '       cmdRead.Parameters("@InEvCode").Value = sPREventCode
        '       cmdRead.Parameters("@InEvCode").Direction = ParameterDirection.Input

        '       cmdRead.Parameters.Add("@InRnd", OleDb.OleDbType.Char)
        '       cmdRead.Parameters("@InRnd").Size = 1
        '       cmdRead.Parameters("@InRnd").Value = "0"    '0 = All Rounds    sSelRnd
        ''        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sSelDV   'This is the division selected for display.  sDv is the division in which the skier is performing.
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input

        '    cmdRead.Parameters.Add("@InGroup", OleDb.OleDbType.VarChar)
        '    cmdRead.Parameters("@InGroup").Size = 3
        '    cmdRead.Parameters("@InGroup").Value = selDv   'sEventGroup
        '    cmdRead.Parameters("@InGroup").Direction = ParameterDirection.Input

        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()
                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))

                            If Not IsDBNull(MyDataReader.Item("DiV")) Then
                                sDv = CStr(MyDataReader.Item("DiV"))
                            Else
                                sDv = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("TeamCode")) Then
                                sTeamCode = CStr(MyDataReader.Item("TeamCode"))
                            Else
                                sTeamCode = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("Round")) Then
                                sRound = CStr(MyDataReader.Item("Round"))
                            Else
                                sRound = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("MemberID")) Then
                                sMemberID = MyDataReader.Item("MemberID")
                            Else
                                sMemberID = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("ScoreBest")) Then
                                sScoreBest = MyDataReader.Item("ScoreBest")
                            Else
                                sScoreBest = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEventScoreDesc = MyDataReader.Item("EventScoreDesc")
                            Else
                                sEventScoreDesc = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = MyDataReader.Item("EventClass")
                            Else
                                sEventClass = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("City")) Then
                                sCity = MyDataReader.Item("City")
                            Else
                                sCity = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("State")) Then
                                sState = MyDataReader.Item("State")
                            Else
                                sState = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("Federation")) Then
                                sFederation = MyDataReader.Item("Federation")
                            Else
                                sFederation = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("RankingScore")) Then
                                sRankingScore = MyDataReader.Item("RankingScore")
                            Else
                                sRankingScore = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("NOPSScore")) Then
                                sNopsScore = MyDataReader.Item("NOPSScore")
                            Else
                                sNopsScore = ""
                            End If
                            If sTmpDv = "" Then
                                'Add the division header for first division
                                sLine.Append("<table class=""table"" width=""100%"">")
                                sLine.Append("<tr><td class=""table-warning"" width=""25%""><b> Leader Board </b></td>")
                                '   sLine.Append("<td Class=""table-primary"" width=""5%""></td>")
                                sLine.Append("<td Class=""table-primary""><b>" & UCase(sSelEvent) & "</b></td>")
                                sLine.Append("<td Class=""table-primary"" colspan=""2""><b>Group: " & sDv & " &nbsp;</b><span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td></tr>")
                                sTmpDv = sDv
                            End If
                            'Get the first MemberID' first record in first pass through data
                            If stmpMemberID = "" Then stmpMemberID = sMemberID

                            If sTmpDv = sDv Then 'Continue in same Division
                                'Add the data line
                                sLine.Append("<tr><td class=""table-warning""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sSkiYear & "&MID=" & sMemberID & "&DV=" & sSelDV & "&EV=" & sEventPkd & "&TN=" & sTName & "")
                                sLine.Append("&FC=NCWL&FT=0&RP=1&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a></td>")   '   
                                sLine.Append("<td><b> " & sTeamCode & "</b></td>")
                                sLine.Append("<td><b> " & sScoreBest & " " & sUnit & "</b></td><td>" & sEventScoreDesc & "</td></tr>")
                                '   sMultiRndScores = ModDataAccessTeams.LBGetRndScores(sSanctionID, sMemberID, sSelEvent, sDv, sSelRnd, sRound, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sNopsScore)
                                '   If sMultiRndScores <> "Error" Then
                                '       sLine.Append(sMultiRndScores)
                                '       sMultiRndScores = ""
                                '   Else
                                '       'FIX THIS ERROR TRAP
                                '   End If
                            Else 'Division changed.

                                sLine.Append("</table>")
                                stmpMemberID = sMemberID
                                sTmpDv = sDv
                                'start new division header
                                sLine.Append("<table class=""table"" width=""100%"">")
                                sLine.Append("<tr><td class=""table-warning"" width=""25%""><b> Leader Board </b></td>")
                                '   sLine.Append("<td Class=""table-primary"" width=""5%""></td>")
                                sLine.Append("<td Class=""table-primary""><b>" & UCase(sSelEvent) & "</b></td>")
                                sLine.Append("<td Class=""table-primary"" colspan=""2""><b>Group: " & sDv & " &nbsp;</b><span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td></tr>")
                                'Add the data line

                                sLine.Append("<tr><td class=""table-warning""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sSkiYear & "&MID=" & sMemberID & "&DV=" & sSelDV & "&EV=" & sEventPkd & "&TN=" & sTName & "")
                                sLine.Append("&FC=NCWL&FT=0&RP=1&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a></td>")   '   
                                sLine.Append("<td><b> " & sTeamCode & "</b></td>")
                                sLine.Append("<td><b> " & sScoreBest & " " & sUnit & "</b></td><td>" & sEventScoreDesc & "</td></tr>")
                                '                                sMultiRndScores = ModDataAccessTeams.LBGetRndScores(sSanctionID, sMemberID, sSelEvent, sDv, sSelRnd, sRound, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sNopsScore)
                                '                                If sMultiRndScores <> "Error" Then
                                '                                    sLine.Append(sMultiRndScores)
                                '                                Else
                                '                                    'FIX THIS ERROR TRAP
                                '                                End If
                            End If

                        Loop
                        'Close the DV table for the specified division
                        sLine.Append("</table>")
                    Else
                        '      sLine.Append("<tr  class=""table-info""><td> " & sSkierName & "</td><td>No Scores</td></tr></table>")
                        sMsg = "No Scores"
                    End If

                End Using
            Catch ex As Exception
                sMsg = "Error at GetBestRndXEvDv"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try

        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
        Else
            sMsg = sLine.ToString()
            Return sLine.ToString()
        End If
    End Function
    Public Function LBGetRndScores(ByVal SanctionID As String, ByVal MemberID As String, ByVal selEvent As String, ByVal selDv As String, ByVal selRnd As String, ByVal BestRnd As String, ByVal rndsSlalomOffered As String, ByVal rndsTrickOffered As String, ByVal rndsJumpOffered As String, ByVal sNopsScore As String) As String
        'Get skiers by division in order of score desc
        'If all divisions are chosen display each round score in each event in horizontal row
        'If a specific round is selected - display that round details ignoring best round
        'if rnd = 0 then <td>sRnd1score</td><td>sRnd2Score</td> etc
        'if rnd > 0 then <td colspan="sRndsThisEvent"> Buoys NOPS EventScoreDetail Time
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID
        Dim sTmpMemberID As String = ""
        Dim sAgeGroup As String = ""
        Dim sDV As String = ""
        Dim sTmpDV As String = ""
        Dim sSelDv As String = selDv  'AgeGroup/Div selected as a filter
        Dim sTmpAgeGroup As String = ""
        Dim sSelRnd As String = selRnd      'Round selected as a filter.
        Dim sBestRnd As String = BestRnd  'Round in which skier scored his best performance
        Dim sSelEvent As String = selEvent
        Dim sEventClass As String = ""
        Dim sTmpEventClass As String = ""
        Dim sTmpEvent As String = ""
        Dim sEventScore As String = ""
        Dim sTmpEventScore As String = ""
        Dim TmpMemberID As String = ""
        Dim TmpEventScore As String = ""
        Dim sNOPS As String = False ' database value for nops
        Dim sSNops As String = ""
        Dim sTNops As String = ""
        Dim sJNops As String = ""
        Dim sRnd As String = ""
        Dim sRndsThisEvent As String = ""
        Dim sRndsSlalomOffered As String = rndsSlalomOffered
        Dim sRndsTrickOffered As String = rndsTrickOffered
        Dim sRndsJumpOffered As String = rndsJumpOffered
        Dim sSlalomHeader As String = ""
        Dim sTrickHeader As String = ""
        Dim sJumpHeader As String = ""
        Dim sCols2Make As String = ""
        If sSelRnd = 0 Then
            sCols2Make = rndsSlalomOffered
            If sRndsTrickOffered > sCols2Make Then sCols2Make = sRndsTrickOffered
            If sRndsJumpOffered > sCols2Make Then sCols2Make = sRndsJumpOffered
        Else
            sCols2Make = "4"  'ADJUST THIS TO MATCH ONE ROUND FORMAT
        End If
        Dim sEventScoreDesc As String = ""
        Dim sTime As String = ""
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim sTmpRow As New StringBuilder
        Dim sSQL As String = ""
        Dim sSBSql As New StringBuilder
        Select Case sSelEvent
            Case "Slalom"
                sSBSql.Append(" select Memberid, [Round], NopsScore, AgeGroup as Div, EventClass, CAST(Score AS CHAR) AS Buoys, TRIM(CAST(FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(FinalSpeedMph AS CHAR)) + 'mph '")
                sSBSql.Append(" + TRIM(FinalLenOff) + ' (' + TRIM(CAST(FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(FinalLen) + 'm)' AS EventScoreDesc, ")
                sSBSql.Append(" TRIM(CAST(FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(FinalLen) + 'm' AS EventScoreDescMeteric,")
                sSBSql.Append(" TRIM(CAST(FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(FinalSpeedMph AS CHAR)) + 'mph ' + TRIM(FinalLenOff) AS EventScoreDescImperial ")
                sSBSql.Append(" FROM LiveWebScoreboard.dbo.SlalomScore where sanctionID = ? and MemberID = ? and AgeGroup = ? ")
                If sSelRnd <> 0 Then sSBSql.Append(" and Round = ? ")

            Case "Trick"

                sSBSql.Append(" Select SanctionID, MemberID,AgeGroup as Div, EventClass,  [Round], NopsScore, Score AS EventScore, ")
                sSBSql.Append(" Trim(CAST(Score As Char)) + ' POINTS (P1:' + TRIM(CAST(ScorePass1 AS CHAR)) + ' P2:' + TRIM(CAST(ScorePass2 AS CHAR)) + ')' AS EventScoreDesc ")
                sSBSql.Append(" From LiveWebScoreboard.dbo.TrickScore Where SanctionID = ? and MemberID = ? and AgeGroup = ? ")
                If sSelRnd <> 0 Then sSBSql.Append(" and Round = ? ")

            Case "Jump"
                sSBSql.Append("Select SanctionID, memberid, AgeGroup as Div, EventClass,  [Round], NopsScore, TRIM(cast(ScoreFeet As Char) + 'Ft' + Cast(ScoreMeters AS CHAR) + 'M') AS EventScoreDesc, ")
                sSBSql.Append(" ScoreFeet, ScoreMeters from LiveWebScoreboard.dbo.JumpScore where SanctionID = ? and MemberID = ? and AgeGroup = ? ")
                If sSelRnd <> 0 Then sSBSql.Append(" and Round = ? ")

            Case "Overall"
                sMsg = "Overall not available"
                Return sMsg
                Exit Function
            Case Else
                sMsg = "Event out of range"
                sMsg = "Overall not available"
                Return sMsg
                Exit Function
        End Select
        sSQL = sSBSql.ToString()
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: LBGetRndScores could not get connection string."
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        cmdRead.CommandType = CommandType.Text
        cmdRead.CommandText = sSQL
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InMemID", OleDb.OleDbType.Char)
        cmdRead.Parameters("@InMemID").Size = 9
        cmdRead.Parameters("@InMemID").Value = sMemberID
        cmdRead.Parameters("@InMemID").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sSelDv   'skier's division from previous query.  NOT from drop list
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input

        If sSelRnd <> 0 Then
            cmdRead.Parameters.Add("@InRnd", OleDb.OleDbType.Char)
            cmdRead.Parameters("@InRnd").Size = 1
            cmdRead.Parameters("@InRnd").Value = sSelRnd
            cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input
        End If


        '        cmdRead.Parameters.Add("@InEvCode", OleDb.OleDbType.VarChar)
        '        cmdRead.Parameters("@InEvCode").Size = 12
        '        cmdRead.Parameters("@InEvCode").Value = sSelEvent
        '        cmdRead.Parameters("@InEvCode").Direction = ParameterDirection.Input

        '        cmdRead.Parameters.Add("@InGroup", OleDb.OleDbType.VarChar)
        '        cmdRead.Parameters("@InGroup").Size = 3
        '    cmdRead.Parameters("@InGroup").Value = selDv   'sEventGroup
        '    cmdRead.Parameters("@InGroup").Direction = ParameterDirection.Input

        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()
                            If Not IsDBNull(MyDataReader.Item("DiV")) Then
                                sDV = CStr(MyDataReader.Item("DiV"))
                            Else
                                sDV = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = MyDataReader.Item("EventClass")
                            Else
                                sEventClass = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("Round")) Then
                                sRnd = CStr(MyDataReader.Item("Round"))
                            Else
                                sRnd = "N/A"
                            End If

                            '                        If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                            '                            sEventScore = CStr(MyDataReader.Item("EventScore"))
                            '                        Else
                            '                            sEventScore = "N/A"
                            '                        End If
                            If sSelEvent <> "Overall" Then 'S,T,J fields

                                If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                    sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                                Else
                                    sEventScoreDesc = "N/A"
                                End If

                                If Not IsDBNull(MyDataReader.Item("NopsScore")) Then
                                    sNOPS = CStr(MyDataReader.Item("NopsScore"))
                                Else
                                    sNOPS = "N/A"
                                End If
                            Else 'Overall Fields
                                If Not IsDBNull(MyDataReader.Item("SlalomNopsScore")) Then
                                    sSNops = CStr(MyDataReader.Item("SlalomNopsScore"))
                                Else
                                    sSNops = "N/A"
                                End If
                                If Not IsDBNull(MyDataReader.Item("TrickNopsScore")) Then
                                    sTNops = CStr(MyDataReader.Item("TrickNopsScore"))
                                Else
                                    sTNops = "N/A"
                                End If
                                If Not IsDBNull(MyDataReader.Item("JumpNopsScore")) Then
                                    sJNops = CStr(MyDataReader.Item("JumpNopsScore"))
                                Else
                                    sJNops = "N/A"
                                End If
                            End If

                            If sSelRnd = 0 Then 'including all rounds
                                i += 1 'Round 1 column
                                If sRnd > i Then  'If first score is in round 2 or greater - fill in earlier rounds as blanks
                                    Do Until sRnd = i
                                        sTmpRow.Append("<td></td>")
                                        i += 1
                                    Loop
                                End If
                                Select Case sRnd 'get the data available for the specified event, Group, DV, and skier
                                    Case 1
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td><b>" & sEventScoreDesc & "</b></td>")
                                            '     sTmpRow.Append("<td class=""table-warning""><b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 2
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td><b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 3
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td><b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 4
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td><b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 5
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td><b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 6
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td><b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 0  'error
                                        sTmpRow.Append("<td>Rnd Error</td>")
                                End Select

                            Else 'only need score for selected round Expand format
                                sTmpRow.Append("<td>sRnd " & sSelRnd & "</td><td>" & sEventClass & "</td><td> " & sEventScore & "</td><td> " & sNOPS & "</td><td> " & sEventScoreDesc & "</td></tr>")
                            End If

                            '                           sLine.Append(sTmpRow.ToString())
                        Loop
                        'ended loop.  Add extra td if needed and close out row
                        If i < sCols2Make Then
                            Do Until i = sCols2Make
                                sTmpRow.Append("<td></td>")
                                i += 1
                            Loop
                        End If
                        sTmpRow.Append("</tr>")

                    Else 'No data
                        sTmpRow.Append("<td colspan=""" & sCols2Make & """> No scores found for this skier</td></tr>")
                    End If
                End Using
            Catch ex As Exception
                sMsg = "<td colspan=""" & sCols2Make & """>Error at LBGetRndScores</td></tr>"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
        Else
            Return sTmpRow.ToString()
        End If
    End Function

    Public Function ScoresXRunOrdHorizNCW(ByVal SanctionID As String, ByVal SkiYr As String, ByVal TName As String, ByVal selEvent As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, Optional ByVal selDivision As String = "ALL", Optional ByVal selRnd As String = "0", Optional ByVal DisplayMetric As Int16 = 0) As String
        'If optional parameter is provided - event if "" the default is not used.
        'Either leave it out of the calling code or provide an accurate value
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sStopHere As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sSkiYear As String = SkiYr
        Dim sPREventCode As String = ""
        Dim sSelEvent As String = selEvent
        Dim sSelRnd As String = selRnd
        Dim sSelDivision As String = selDivision
        Dim sSelFormat As String = "Best"
        Dim sUseNOPS As Int16 = 0
        Dim sShowTeams As Int16 = 0
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sTeamCode As String = ""
        Dim sDisplayMetric As Int16 = DisplayMetric
        Dim sEventScoreDesc As String = ""
        Dim sEventScoreDescMetric As String = ""
        Dim sEventScoreDescImperial As String = ""
        Dim stmpMemberID As String = ""
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim sDv As String = ""
        Dim sTmpDV As String = ""
        Dim sRunOrdTitle As String = ""
        Dim sEventClass As String = ""
        Dim sEventGroup As String = ""
        Dim sTmpEventGroup As String = ""
        Dim sRnd As String = ""
        Dim sRunOrd As New StringBuilder
        Dim sTName As String = TName
        Dim sRoundsHTML As String = ""
        Dim sLine As New StringBuilder
        Dim sRndsSlalomOffered As String = RndsSlalomOffered
        Dim sRndsTrickOffered As String = RndsTrickOffered
        Dim sRndsJumpOffered As String = RndsJumpOffered
        Dim sUnits As String = ""
        Dim sRndsThisEvent As String = ""
        Dim sRndCols As String = 0
        Dim sRowCount As Int16 = 0
        Dim sTmpRow As New StringBuilder
        sTmpRow.Clear()

        Dim sEventScore As String = ""
        '       Dim sTmpEventScore As String = ""
        Dim sBestRndTable As String = "" 'Holds the best round table for this division
        Dim sFirstRowInDv As String = ""  'First line display for each division.  eventually holds <td rowspan="[skiers in DV]">
        Dim sFirstSkierInDV As Boolean = False
        Dim sMoreLines As String = ""   ' Second and any additional lines in that division

        Dim sSql As String = ""
        Dim sOrderBy As String = ""
        sOrderBy = "Order By Round asc, AgeGroup,  EventScore DESC"
        If sUseNOPS <> 0 Then
            sOrderBy = " Order By Round asc, Gender, NopsScore DESC"
        End If

        Select Case sSelEvent
            Case "S"
                sPREventCode = "Slalom"
                sSql = "PrSlalomScoresByRunOrder"
                sUnits = "Buoys"
                sRndsThisEvent = sRndsSlalomOffered
                sRndCols = 3 'NCWSA ALWAYS 1 ROUND  Slalom uses exstra field
           '     If sSelRnd = "0" Then
            '        sRndCols = CStr(CInt(sRndsSlalomOffered))  'Name, Team, EventScore, EventScoreDesc
            '        For j = 1 To RndsSlalomOffered
            '            sRoundsHTML += "<td  class=""table-info"">Rnd " & j & "</td>"
            '        Next
            '    Else
            '        sRoundsHTML += "<td  class=""table-info"">Rnd " & sSelRnd & "</td>"
            '        sRndCols = "1"
            '    End If
            Case "T"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresByRunOrder"
                sUnits = "Points"
                sRndsThisEvent = sRndsTrickOffered
                sRndCols = 3 'NCWSA ALWAYS 1 ROUND  Trick doesn't display EventScore field so 3 instead of 4 like slalom
'                If sSelRnd = "0" Then
'                    sRndCols = CStr(CInt(sRndsTrickOffered))  'Rnds + name + 2 for BestRnd
'                    For j = 1 To RndsTrickOffered
'                        sRoundsHTML += "<td  class=""table-info"">Rnd " & j & "</td>"
'                    Next
'                Else
'                    sRoundsHTML += "<td  class=""table-info"">Rnd " & sSelRnd & "</td>"
'                    sRndCols = "1"
'                End If
            Case "J"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresByRunOrder"
                sUnits = "Ft M"
                sRndsThisEvent = sRndsJumpOffered
                sRndCols = 3  'NCWSA ALWAYS 1 ROUND  Jump  doesn't display EventScore field so 3 instead of 4 like slalom
                '               If sSelRnd = "0" Then

                '                   sRndCols = CStr(CInt(sRndsJumpOffered))  'Rnds + name + 2 for BestRnd
                '                   For j = 1 To RndsJumpOffered
                '                       sRoundsHTML += "<td  class=""table-info"">Rnd " & j & "</td>"
                '                   Next
                '               Else
                '                   sRoundsHTML += "<td  class=""table-info"">Rnd " & sSelRnd & "</td>"
                '                   sRndCols = "1"
                '               End If
            Case Else  'Load all by default
                sMsg += "Event out of Range"
                Return sMsg
                Exit Function
        End Select
        sLine.Append("<Table Class=""table  border-1 "">")  'Start full page table
        i = 0
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: GetRunOrder could not get connection string."
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        cmdRead.CommandType = CommandType.StoredProcedure
        cmdRead.CommandText = sSql
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

        '       cmdRead.Parameters.Add("@InEvCode", OleDb.OleDbType.VarChar)
        '       cmdRead.Parameters("@InEvCode").Size = 12
        '       cmdRead.Parameters("@InEvCode").Value = sPREventCode
        '       cmdRead.Parameters("@InEvCode").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InRnd", OleDb.OleDbType.Char)
        cmdRead.Parameters("@InRnd").Size = 1
        cmdRead.Parameters("@InRnd").Value = "1"    'NCWSA IS ALWAYS 1 ROUND
        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sSelDivision  'sDv
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InGroup", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InGroup").Size = 3
        cmdRead.Parameters("@InGroup").Value = "ALL"   'sEventGroup  Don't have a droplist for Event Group so always all
        cmdRead.Parameters("@InGroup").Direction = ParameterDirection.Input

        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()
                            sSkierName = CStr(MyDataReader.Item("SkierName"))


                            If Not IsDBNull(MyDataReader.Item("TeamCode")) Then
                                sTeamCode = CStr(MyDataReader.Item("TeamCode"))
                            Else
                                sTeamCode = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("DiV")) Then
                                sDv = CStr(MyDataReader.Item("DiV"))
                            Else
                                sDv = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = MyDataReader.Item("EventClass")
                            Else
                                sEventClass = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventGroup")) Then
                                sEventGroup = MyDataReader.Item("EventGroup")
                            Else
                                sEventGroup = ""
                            End If

                            sMemberID = MyDataReader.Item("MemberID")
                            If Not IsDBNull(MyDataReader.Item("MemberID")) Then
                                sMemberID = MyDataReader.Item("MemberID")
                            Else
                                sMemberID = ""
                            End If
                            If sPREventCode = "Slalom" Then
                                Select Case sDisplayMetric
                                    Case 0
                                        If Not IsDBNull(MyDataReader.Item("EventScoreDescImperial")) Then
                                            sEventScoreDesc = MyDataReader.Item("EventScoreDescImperial")
                                        Else
                                            sEventScoreDesc = ""
                                        End If
                                    Case 1
                                        If Not IsDBNull(MyDataReader.Item("EventScoreDescMetric")) Then
                                            sEventScoreDesc = MyDataReader.Item("EventScoreDescMetric")
                                        Else
                                            sEventScoreDesc = ""
                                        End If
                                    Case Else
                                        If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                            sEventScoreDesc = MyDataReader.Item("EventScoreDesc")
                                        Else
                                            sEventScoreDesc = ""
                                        End If
                                End Select
                            Else  'Jump and Trick do not have EventScoreDescImperial and EventScoreDescMetric fields
                                If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                    sEventScoreDesc = MyDataReader.Item("EventScoreDesc")
                                Else
                                    sEventScoreDesc = ""
                                End If
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                                sEventScore = MyDataReader.Item("EventScore")
                            Else
                                sEventScore = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("Round")) Then
                                sRnd = MyDataReader.Item("Round")
                            Else
                                sRnd = 0
                            End If

                            If sTmpEventGroup = "" Then
                                sTmpEventGroup = sEventGroup
                                sLine.Append("<tr><td  class=""table-success"" width=""20%"" ><b>Running Order</b></td><td class=""text-bg-info""  colspan=""" & (sRndCols) & """>EventGroup: <b>" & sTmpEventGroup & "</b>, Div: <b>" & sDv & "</b> &nbsp; <span class=""bg-danger text-white"" > <b>UNOFFICIAL !</b></span></td></tr>")  '<td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                                sTmpDV = sDv
                            End If

                            If sTmpEventGroup = sEventGroup Then 'For NCWSA split by Event Group and division
                                '                                If sTmpDV = sDv Then ' Split divisions here
                                If sTmpDV <> sDv Then 'add another header
                                    sLine.Append("<tr><td  class=""table-success"" width=""20%"" ><b>Running Order</b></td><td class=""text-bg-info""  colspan=""" & (sRndCols) & """>EventGroup: <b>" & sEventGroup & "</b>, Div: <b>" & sDv & "</b> &nbsp; <span class=""bg-danger text-white"" > <b>UNOFFICIAL !</b></span></td></tr>")  '<td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                                    sTmpDV = sDv
                                End If


                                'only need score for first round
                                sLine.Append("<tr><td Class = ""table-success"" width=""20%""><a runat=""server""  target=""_blank""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sSkiYear & "&MID=" & sMemberID & "&DV=" & sDv & "&EV=" & sSelEvent & "&TN=" & sTName & "&FC=NCWRO&FT=0&RP=1&UN=0&UT=0&SN=" & sSkierName & """ >" & sSkierName & "</a></td>")
                                sLine.Append("<td><b>" & sTeamCode & "</b></td>")
                                If sPREventCode = "Slalom" Then
                                    sLine.Append("<td><b>" & sEventScore & " " & sUnits & "</b></td>")
                                End If
                                sLine.Append("<td><b>" & sEventScoreDesc & "</b></td></tr>")

                            Else 'EventGroupChanged

                                'New Event Header
                                sLine.Append("<tr><td  class=""table-success"" width=""20%"" ><b>Running Order</b></td><td class=""text-bg-info""  colspan=""" & (sRndCols) & """>EventGroup: <b>" & sEventGroup & "</b>, Div: <b>" & sDv & "</b> &nbsp; <span class=""bg-danger text-white"" > <b>UNOFFICIAL !</b></span></td></tr>")  '<td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                                'Skier details
                                sLine.Append("<tr><td class = ""table-success"" width=""25%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sSkiYear & "&MID=" & sMemberID & "&DV=" & sDv & "&EV=" & sSelEvent & "&TN=" & sTName & "&FC=NCWRO&FT=0&RP=1&UN=0&UT=0&SN=" & sSkierName & """ >" & sSkierName & "</a></td>")
                                sLine.Append("<td><b>" & sTeamCode & "</b></td>")
                                If sPREventCode = "Slalom" Then
                                    sLine.Append("<td><b>" & sEventScore & " " & sUnits & "</b></td>")
                                End If
                                sLine.Append("<td><b>" & sEventScoreDesc & "</b></td></tr>")
                                sTmpEventGroup = sEventGroup
                                sTmpDV = sDv
                            End If
                        Loop
                        'END OF SINGLE DIVISION OR LAST OF MULTIPLE DIVISIONS 
                        'Last skier data is in tmpXXXX
                        'May have had 1 or moure rounds already collected in sMoreLines


                        sLine.Append("</table>") 'Close out the  table

                    Else 'No data
                        sLine.Append("No " & sPREventCode & "Skiers Found")
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetRunOrder"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        sMsg = sLine.ToString()
        Return sLine.ToString()
    End Function
End Module
