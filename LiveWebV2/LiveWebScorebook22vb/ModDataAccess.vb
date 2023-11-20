
Imports System.Data.OleDb

Module ModDataAccess
    Friend Function GetTournamentList(ByVal SkiYr As String) As String
        Dim sMsg As String = ""
        Dim sSkiYr As String = SkiYr

        Dim SQL As String = ""
        If sSkiYr = "0" Then
            SQL = "Select Top 20 SanctionID, Name, Class, EventDates, EventLocation, Rules, SlalomRounds, TrickRounds, JumpRounds from Tournament "
        Else
            SQL = "Select SanctionID, Name, Class, EventDates, EventLocation, Rules, SlalomRounds, TrickRounds, JumpRounds from Tournament "
            SQL += " WHERE left(SanctionID, 2) = '" & sSkiYr & "'"
        End If
        SQL += " order by EventDates desc"

        'Query from wfwShowTourList.php
        '        $TourneyQry = "Select SanctionId, Name, Class, EventLocation"
        '        . ", SlalomRounds, TrickRounds, JumpRounds"
        '			. ", STR_TO_DATE(EventDates, '%m/%d/%Y') as EventDate "
        '			. ", (Select count(*) from TrickVideo V Where V.SanctionId = T.SanctionId "
        '			. "And (V.Pass1VideoUrl is not null or V.Pass2VideoUrl is not null)) as TrickVideoCount "
        '			. "from Tournament T "
        ''			. "Where SanctionId like '" . $SkiYear . "%'"
        '			. "Order By STR_TO_DATE(EventDates, '%m/%d/%Y') DESC";



        Dim sSanctionID As String = ""
        Dim sName As String = ""
        Dim sClass As String = ""
        Dim sEventDates As String = ""
        Dim sEventLocation As String = ""
        Dim sRules As String = ""
        Dim sLblText As String = ""
        Dim sBtnText As String = ""
        Dim sLine As String = ""
        Dim sConn As String = ""
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
        Catch ex As Exception
            sMsg = ex.Message & "  " & ex.StackTrace
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sTableWidth As String = "100%"
        Dim sText As String = "<ul>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = SQL
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sName = CStr(MyDataReader.Item("Name"))
                            sClass = CStr(MyDataReader.Item("Class"))
                            sEventDates = MyDataReader.Item("EventDates")
                            sEventLocation = MyDataReader.Item("EventLocation")
                            sRules = MyDataReader.Item("Rules")
                            sLblText = sSanctionID & " " & sEventLocation
                            sLine = "<li><a runat=""server"" href=""Tournament.aspx?SN=" & sSanctionID & "&FM=1&SY=" & sSkiYr & """>" & sName & " " & sEventDates & " </a>" & sLblText & "</li>"
                            sText += sLine
                        Loop
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: SQL= " & SQL & "<br>GetTournamentList Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"

            Finally
                If Len(sMsg) < 2 Then
                    sText += "</ul>"
                End If
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function GetTournamentSpecs(ByVal SanctionID As String) As Array
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim SQL As String = ""
        Dim sSanctionID As String = Trim(SanctionID)
        SQL = "Select SanctionID, Name, Class, EventDates, EventLocation, Rules, SlalomRounds, TrickRounds, JumpRounds from Tournament WHERE SanctionID = '" & sSanctionID & "'"
        Dim sName As String = ""
        Dim sClass As String = ""
        Dim sEventDates As String = ""
        Dim sEventLocation As String = ""
        Dim sRules As String = ""
        Dim sSlalomRounds As Int16 = 0
        Dim sTrickRounds As Int16 = 0
        Dim sJumpRounds As Int16 = 0
        Dim arrSpecs(0 To 9, 0 To 2)
        Dim sConn As String = ""
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: GetTournamentSpecs could not retrieve connection string. " & ex.Message & "  " & ex.StackTrace
            arrSpecs(0, 0) = sMsg
            Return arrSpecs
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        '        Dim sTableWidth As String = "100%"
        '        Dim sText As String = "<table width=""" & sTableWidth & """>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = SQL
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sName = CStr(MyDataReader.Item("Name"))
                            sClass = CStr(MyDataReader.Item("Class"))
                            sEventDates = MyDataReader.Item("EventDates")
                            sEventLocation = MyDataReader.Item("EventLocation")
                            sRules = MyDataReader.Item("Rules")
                            sSlalomRounds = MyDataReader.Item("SlalomRounds")
                            sTrickRounds = MyDataReader.Item("TrickRounds")
                            sJumpRounds = MyDataReader.Item("JumpRounds")

                            arrSpecs(1, 1) = "SanctionID:"
                            arrSpecs(1, 2) = sSanctionID.ToString
                            arrSpecs(2, 1) = "Name:"
                            arrSpecs(2, 2) = sName.ToString
                            arrSpecs(3, 1) = "Start Date:"
                            arrSpecs(3, 2) = sEventDates
                            arrSpecs(4, 1) = "Location:"
                            arrSpecs(4, 2) = sEventLocation
                            arrSpecs(5, 1) = "Rules:"
                            arrSpecs(5, 2) = sRules
                            arrSpecs(6, 1) = "Slalom Rounds"
                            arrSpecs(6, 2) = sSlalomRounds
                            arrSpecs(7, 1) = "Trick Rounds"
                            arrSpecs(7, 2) = sTrickRounds
                            arrSpecs(8, 1) = "Jump Rounds"
                            arrSpecs(8, 2) = sJumpRounds

                        Loop
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sErrDetails += "Error: SQL= " & SQL & "<br>GetTournamentSpecs Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"
                arrSpecs(0, 0) = "Error retrieving tournament information."
            End Try
        End Using
        Return arrSpecs
    End Function
    Friend Function GetOfficials(ByVal SanctionID As String) As Array
        Dim sSanctionID As String = SanctionID
        Dim arrOfficials(0 To 12, 0 To 2)
        Dim i As Integer = 1
        Dim sJudgeChief As String = ""
        Dim sDriverChief As String = ""
        Dim sScoreChief As String = ""
        Dim sSafetyChief As String = ""
        Dim sJudgeAppointed As String = ""
        Dim sTechChief As String = ""
        Dim SQL As String = "select distinct TR.MemberID, TR.SkierName, TR.Notes, OW.JudgeChief, OW.DriverChief, OW.ScoreChief, OW.SafetyChief, OW.TechChief, OW.JudgeAppointed 
FROM waterskiprod23.dbo.TourReg TR left join waterskiprod23.dbo.OfficialWork OW on TR.MemberID = OW.MemberId
Where OW.SanctionId = '" & sSanctionID & "' and  (OW.JudgeAppointed = 'Y' or OW.JudgeChief = 'Y' or OW.DriverChief = 'Y' or OW.ScoreChief = 'Y' or OW.SafetyChief = 'Y' )
and TR.Notes = 'Appointed Official'"

        Dim sMsg As String = ""
        Dim sSkierName As String = ""
        Dim sPosition As String = ""

        Dim sConn As String = ""
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: GetTournamentSpecs could not retrieve connection string. " & ex.Message & "  " & ex.StackTrace
            arrOfficials(0, 0) = sMsg
            Return arrOfficials
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        '        Dim sTableWidth As String = "100%"
        '        Dim sText As String = "<table width=""" & sTableWidth & """>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = SQL
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()


                            sSkierName = MyDataReader.Item("SkierName")

                            If Not IsDBNull(MyDataReader.Item("JudgeChief")) Then
                                If MyDataReader.Item("JudgeChief") = "Y" Then
                                    arrOfficials(i, 1) = "Chief Judge: "
                                    arrOfficials(i, 2) = sSkierName
                                End If
                            End If

                            If Not IsDBNull(MyDataReader.Item("DriverChief")) Then
                                If MyDataReader.Item("DriverChief") = "Y" Then
                                    arrOfficials(i, 1) = "Chief Driver: "
                                    arrOfficials(i, 2) = sSkierName
                                End If
                            End If

                            If Not IsDBNull(MyDataReader.Item("ScoreChief")) Then
                                If MyDataReader.Item("ScoreChief") = "Y" Then
                                    arrOfficials(i, 1) = "Chief Scorer: "
                                    arrOfficials(i, 2) = sSkierName
                                End If
                            End If

                            If Not IsDBNull(MyDataReader.Item("SafetyChief")) Then
                                If MyDataReader.Item("SafetyChief") = "Y" Then
                                    arrOfficials(i, 1) = "Chief Safety: "
                                    arrOfficials(i, 2) = sSkierName
                                End If
                            End If
                            If Not IsDBNull(MyDataReader.Item("TechChief")) Then
                                If MyDataReader.Item("TechChief") = "Y" Then
                                    arrOfficials(i, 1) = "Tech Controller: "
                                    arrOfficials(i, 2) = sSkierName
                                End If
                            End If

                            If Not IsDBNull(MyDataReader.Item("JudgeAppointed")) Then
                                If MyDataReader.Item("JudgeAppointed") = "Y" Then
                                    arrOfficials(i, 1) = "Appointed Judge: "
                                    arrOfficials(i, 2) = sSkierName
                                End If
                            End If

                            i += 1
                        Loop
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: SQL= " & SQL & "<br>GetOfficials Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"
                arrOfficials(0, 0) = sMsg
            End Try
        End Using
        Return arrOfficials
    End Function

    Friend Function GetEntryList(ByVal SanctionID As String, ByVal TournName As String, ByVal EventPkd As String, ByVal AgeDvPkd As String, YrPkd As String) As String
        Dim sMsg As String = ""
        Dim sSanctionID As String = Trim(SanctionID)
        Dim sTournName As String = TournName
        Dim sEventPkd As String = EventPkd
        Dim sAgeDvPkd As String = AgeDvPkd
        Dim sAgeGroup As String = ""
        Dim sTmpSkierID As String = ""
        Dim sTmpAgeGroup As String = ""
        Dim sTmpEvent As String = ""
        Dim sTmpEventClass As String = ""
        Dim sYrPkd As String = YrPkd
        Dim sEvent As String = ""
        Dim sEventClass As String = ""
        Dim sTeam As String = ""
        Dim sTmpTeam As String = ""
        Dim sShoTeam As String = ""
        Dim sEnteredIn As String = ""
        Dim sConn As String = ""
        Dim sSQL As String = ""
        Dim sWhere As String = ""
        Dim sOR As String = ""
        Dim sOrderBy As String = ""
        Dim sTmpReadyToSki As String = ""
        Dim sReadyToSki As String = "N"
        Dim sFlag As String = ""
        Dim sCaption As String = " Cls "
        Dim sCollegiate As Boolean = False
        If Mid(sSanctionID, 3, 1) = "U" Then
            sCollegiate = True
            sCaption = " Team "
        End If
        sSQL = "Select * from waterskiProd23.dbo.vSkiersEntered "

        Select Case sEventPkd
            Case "S"
                sWhere = " Where SanctionID = '" & sSanctionID & "' and Event = 'Slalom' "
            Case "T"
                sWhere = " Where SanctionID = '" & sSanctionID & "' and Event = 'Trick' "
            Case "J"
                sWhere = " Where SanctionID = '" & sSanctionID & "' and Event = 'Jump' "
            Case "A"
                sWhere = " Where SanctionID = '" & sSanctionID & "' "
        End Select
        If sAgeDvPkd <> "ALL" Then
            sWhere += " And AgeGroup = '" & sAgeDvPkd & "' "
        End If
        sOrderBy = " Order By MemberID, Event, EventClass, AgeGroup "
        sSQL += sWhere & sOrderBy

        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = ex.Message & "  " & ex.StackTrace
        End Try

        Dim FirstPass As Boolean = True
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sLblText As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sTableWidth As String = "100%"
        Dim sText As String = "<table width=" & sTableWidth & " cellpadding=""5"">"
        sText += "<tr><td colspan=""4""><h4>Select a skier</h4></td></tr>"
        sText += "<tr><td><b>Skier Name</b></td><td><b>Events Entered</b></td><td><b>Team</b></td><td><b>OK2Ski</b></td></tr>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = sSQL
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            sMemberID = CStr(MyDataReader.Item("MemberID"))
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEvent = Left(CStr(MyDataReader.Item("Event")), 1)
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sTeam = "Not Set"
                            If Not IsDBNull(MyDataReader.Item("Team")) Then
                                If Len(Trim(MyDataReader.Item("Team"))) > 1 Then  'Make sure Team is not NULL or empty string
                                    sTeam = MyDataReader.Item("Team")
                                End If
                            End If
                            sReadyToSki = MyDataReader.Item("ReadyToSki")  ' holds Y or N
                            If FirstPass = True Then  'Include the first record
                                sTmpSkierID = sMemberID
                                sTmpTeam = sTeam
                                FirstPass = False
                                sTmpReadyToSki = sReadyToSki
                            End If
                            If sTmpSkierID <> sMemberID Then 'combine all events entered for same skiers.  May be entered in more than one AgeGroup
                                If sTmpReadyToSki = "N" Then
                                    sFlag = " &nbsp; ! SEE REGISTRAR ! &nbsp;"
                                Else
                                    sFlag = "&nbsp; &nbsp;" & sTmpReadyToSki
                                End If
                                sShoTeam = sTmpTeam
                                sLine = "<tr><td><a runat=""server"" href=""TIndScores.aspx?SN=" & sSanctionID & "&SID= " & sMemberID & "&SY= " & sYrPkd & "&TN=" & sTournName & "&EV=" & sEventPkd & "&AG=" & sAgeGroup & """>" & sSkierName & "</a> </td><td>" & sEnteredIn & "</td><td>" & sShoTeam & "</td><td>" & sFlag & "</td></tr>"
                                sText += sLine
                                sTmpEvent = sEvent
                                sTmpSkierID = sMemberID
                                sTmpEventClass = sEventClass
                                sTmpAgeGroup = sAgeGroup
                                sTmpReadyToSki = sReadyToSki
                                sEnteredIn = sEvent & " Cls " & sEventClass
                                sLine = ""
                                sFlag = ""
                                sTmpTeam = sTeam
                                sShoTeam = ""
                            End If
                            If sTmpEvent <> sEvent Then  'Event changed make new string section with current record
                                sEnteredIn += ": " & sEvent & " Cls " & sEventClass & " "
                                sTmpEvent = sEvent
                                sTmpEventClass = sEventClass
                            End If
                            If sTmpEventClass <> sEventClass Then
                                sEnteredIn += "," & sEventClass
                                sTmpEventClass = sEventClass
                            End If
                        Loop
                    Else
                        sText += "<tr><td colspan=""4"">No Skiers Found.</td></tr>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: SQL= " & sSQL & "<br>GetTournamentList Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"

            Finally
                sText += "</table>"
            End Try

        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function GetEntryListPro(ByVal SanctionID As String, ByVal TournName As String, ByVal EventPkd As String, ByVal AgeDvPkd As String, YrPkd As String) As String
        Dim sMsg As String = ""
        Dim sSanctionID As String = Trim(SanctionID)
        Dim sTournName As String = TournName
        Dim sEventPkd As String = EventPkd
        Dim sAgeDvPkd As String = AgeDvPkd
        Dim sAgeGroup As String = ""
        Dim sTmpSkierID As String = ""
        Dim sTmpAgeGroup As String = ""
        Dim sTmpEvent As String = ""
        Dim sTmpEventClass As String = ""
        Dim sYrPkd As String = YrPkd
        Dim sEvent As String = ""
        Dim sEventClass As String = ""
        Dim sTeam As String = ""
        Dim sTmpTeam As String = ""
        Dim sShoTeam As String = ""
        Dim sEnteredIn As String = ""
        Dim sConn As String = ""
        Dim sSQL As String = ""
        Dim sWhere As String = ""
        Dim sOR As String = ""
        Dim sOrderBy As String = ""
        Dim sTmpReadyToSki As String = ""
        Dim sReadyToSki As String = "N"
        Dim sFlag As String = ""
        Dim sCaption As String = " Cls "
        Dim sCollegiate As Boolean = False
        If Mid(sSanctionID, 3, 1) = "U" Then
            sCollegiate = True
            sCaption = " Team "
        End If
        sSQL = "Select * from waterskiProd23.dbo.vSkiersEntered "

        'Entry list will typically present skier name with all events and classes entered.
        'Single event list will be presented when FormatCode is ByRnd and display is of those entered a single event
        'Lists will typically include all age and international divisions unless user is allow to and specifies a single division.  Not yet implemented.
        Select Case sEventPkd
            Case "S"
                sWhere = " Where SanctionID = '" & sSanctionID & "' and Event = 'Slalom' "
                'Use 
            Case "T"
                sWhere = " Where SanctionID = '" & sSanctionID & "' and Event = 'Trick' "
            Case "J"
                sWhere = " Where SanctionID = '" & sSanctionID & "' and Event = 'Jump' "
            Case "A"
                sWhere = " Where SanctionID = '" & sSanctionID & "' "
        End Select
        If sAgeDvPkd <> "ALL" Then
            sWhere += " And AgeGroup = '" & sAgeDvPkd & "' "
        End If
        sOrderBy = " Order By MemberID, Event, EventClass, AgeGroup "
        sSQL += sWhere & sOrderBy

        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = ex.Message & "  " & ex.StackTrace
        End Try

        Dim FirstPass As Boolean = True
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sLblText As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sTableWidth As String = "100%"
        Dim sText As String = "<div class=""row"">
                    <div class=""col-sm-3"">
                        <div>"
        sText += "<table width=" & sTableWidth & " cellpadding=""5"">"

        sText += "<tr><td colspan=""4""><h4>Select a skier</h4></td></tr>"
        sText += "<tr><td><b>Skier Name</b></td><td><b>Events Entered</b></td><td><b>Team</b></td><td><b>OK2Ski</b></td></tr>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = sSQL
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            sMemberID = CStr(MyDataReader.Item("MemberID"))
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEvent = Left(CStr(MyDataReader.Item("Event")), 1)
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sTeam = "Not Set"
                            If Not IsDBNull(MyDataReader.Item("Team")) Then
                                If Len(Trim(MyDataReader.Item("Team"))) > 1 Then  'Make sure Team is not NULL or empty string
                                    sTeam = MyDataReader.Item("Team")
                                End If
                            End If
                            sReadyToSki = MyDataReader.Item("ReadyToSki")  ' holds Y or N
                            If FirstPass = True Then  'Include the first record
                                sTmpSkierID = sMemberID
                                sTmpTeam = sTeam
                                FirstPass = False
                                sTmpReadyToSki = sReadyToSki
                            End If
                            If sTmpSkierID <> sMemberID Then 'combine all events entered for same skiers.  May be entered in more than one AgeGroup
                                If sTmpReadyToSki = "N" Then
                                    sFlag = " &nbsp; ! SEE REGISTRAR ! &nbsp;"
                                Else
                                    sFlag = "&nbsp; &nbsp;" & sTmpReadyToSki
                                End If
                                sShoTeam = sTmpTeam
                                sLine = "<tr><td><a runat=""server"" href=""TIndScores.aspx?SN=" & sSanctionID & "&SID= " & sMemberID & "&SY= " & sYrPkd & "&TN=" & sTournName & "&EV=" & sEventPkd & "&AG=" & sAgeGroup & """>" & sSkierName & "</a> </td><td>" & sEnteredIn & "</td><td>" & sShoTeam & "</td><td>" & sFlag & "</td></tr>"
                                sText += sLine
                                sTmpEvent = sEvent
                                sTmpSkierID = sMemberID
                                sTmpEventClass = sEventClass
                                sTmpAgeGroup = sAgeGroup
                                sTmpReadyToSki = sReadyToSki
                                sEnteredIn = sEvent & " Cls " & sEventClass
                                sLine = ""
                                sFlag = ""
                                sTmpTeam = sTeam
                                sShoTeam = ""
                            End If
                            If sTmpEvent <> sEvent Then  'Event changed make new string section with current record
                                sEnteredIn += ": " & sEvent & " Cls " & sEventClass & " "
                                sTmpEvent = sEvent
                                sTmpEventClass = sEventClass
                            End If
                            If sTmpEventClass <> sEventClass Then
                                sEnteredIn += "," & sEventClass
                                sTmpEventClass = sEventClass
                            End If
                        Loop
                    Else
                        sText += "<tr><td colspan=""4"">No Skiers Found.</td></tr>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: SQL= " & sSQL & "<br>GetEntryListPro Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"

            Finally
                sText += "</table></div>"
                sText += "    </div>"
                sText += "    <div class=""col-sm-3"">"
                sText += "        <div id=""RoundTwo"" runat=""server"">"
                sText += "            Round2 Names and DV"
                sText += "        </div>"
                sText += "     </div>"
                sText += "     <div class=""col-sm-3"">"
                sText += "         <div id=""Round3"" runat=""server"">"
                sText += "             Round3 Names and DV"
                sText += "        </div>"
                sText += "     </div>"
                sText += "     <div class=""col-sm-3"">"
                sText += "         <div id=""RoundFour"" runat=""server"">"
                sText += "             Round4 Names and DV"
                sText += "        </div>"
                sText += "     </div>"
                sText += " </div>"
            End Try

        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function PlacementBestRound(ByVal SanctionID As String, ByVal CompetitionFormatCode As String, ByVal EventCode As String) As String
        'Competition Style is not yet defined.  Will be a code like EMS code for best of 3 rounds, 1 round qualifier and final, etc.
        'Will probably have to add round number, number of qualifiers, and how ties are handled as optional parameters
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = EventCode
        Dim sErrDetails As String = ""
        Dim sRnd As Byte = 0
        'Use zero 0 for fields not in use
        Dim sFormatCode As String = CompetitionFormatCode
        Dim sSql As String = PlacementSQL(sSanctionID, sEventCode, sFormatCode, 0, False)  'Make sure Select works with , separated list
        If Left(sSql, 5) = "Error" Then
            Return "Error at PlacementSQL"
            sErrDetails = sSql
            Exit Function
        End If
        'Process the data by class, division, score desc
        Dim sMsg As String = ""
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sTmpMemberID As String = ""
        Dim sStringMemberIDs As String = ""
        Dim sAgeGroup As String = ""
        Dim sTmpAgeGroup As String = ""
        Dim sEvent As String = ""
        Dim sEventClass As String = ""
        Dim sTmpEventClass As String = ""
        Dim sTmpEvent As String = ""
        Dim sEventScore As String = ""
        Dim sTmpEventScore As String = ""
        Dim TmpMemberID As String = ""
        Dim TmpEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sConn As String = ""
        Dim sFirstLoop As Boolean = True
        Dim sFirstPlace As String = ""
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim sSkipPlace As Int16 = 0
        Dim sStopHere As Boolean = False
        Dim sSlalomHeader As String = "<thead><tr class=""table-primary""><th colspan=""6"">Slalom Results</th></tr>"
        sSlalomHeader += "<tr><th>Place</th><th>Name</th><th>Class</th><th>Rnd</th><th>Buoys</th><th>Detail</th></tr></thead>"
        Dim sTrickHeader As String = "<tr class=""table-primary""><th colspan=""6"">Trick Results</th></tr>"
        sTrickHeader += "<tr><th>Place</th><th>Name</th><th>Class</th><th>Rnd</th><th>Points</th><th>Detail</th></tr>"
        Dim sJumpHeader As String = "<tr class=""table-primary""><th colspan=""6"">Jump Results</th></tr>"
        sJumpHeader += "<tr><th>Place</th><th>Name</th><th>Class</th><th>Rnd</th><th>Length</th><th>Detail</th></tr>"
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: Connection Failure Try again."
            sErrDetails = sEventCode & "PlacementBestRound could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sTableWidth As String = "100%"
        Dim sText As String = "<Table Class=""table table-striped border-1 "">"
        Select Case sEventCode
            Case "S"
                sText += sSlalomHeader
            Case "T"
                sText += sTrickHeader
            Case "J"
                sText += sJumpHeader
        End Select

        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = sSql
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()
                            sMemberID = CStr(MyDataReader.Item("MemberID"))
                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            Replace(sSkierName, "'", "")
                            Replace(sSkierName, ",", "")
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sEventScore = CStr(MyDataReader.Item("EventScore"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                            'First record is always first place in current age division
                            If sFirstLoop = True Then
                                sTmpAgeGroup = sAgeGroup
                                sFirstLoop = False
                                j = 0
                                '        sText += "<trClass=""table-primary""><td  colspan=""6"">Class " & sEventClass & "</td></tr>"
                                sText += "<tr><td class=""table-warning"" colspan=""3"" ><b>" & sAgeGroup & "</b></td><td colspan=""3"" class=""table-light"">&nbsp;</td></tr>"
                                sTmpEventClass = sEventClass
                            End If
                            If sTmpEventClass <> sEventClass Then
                                '  sText += "<tr><td colspan=""6"">Class " & sEventClass & "</td></tr>"
                                sTmpEventClass = sEventClass
                            End If
                            If sTmpAgeGroup = sAgeGroup Then  'In same division - continue
                                i = InStr(sStringMemberIDs, sMemberID)
                                If i < 1 Then ' If MemberID not in list add skier score.
                                    sStringMemberIDs += sMemberID & "," ' Add MemberID to list
                                    If sTmpEventScore = sEventScore Then 'Calculate ties
                                        sSkipPlace += 1
                                        'don't increment j
                                    Else
                                        sTmpEventScore = sEventScore 'reset to new performance level
                                        If sSkipPlace > 0 Then  'counts number of ties
                                            j += 1 + sSkipPlace
                                            sSkipPlace = 0 'reset tie counter
                                        Else
                                            j += 1  'Normal increment
                                        End If
                                        sFirstPlace = ""
                                        If j = 1 Then sFirstPlace = " Class=""table-warning"" "
                                        sStringMemberIDs += sMemberID & ","
                                    End If
                                    sFirstPlace = "" 'don't emphasize placement per David


                                    '                                   sText += "<tr><td" & sFirstPlace & ">" & j & "</td><td><a asp-page=""/Trecap.aspx?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&SN=" & sSkierName & " target=""_blank"" >" & sSkierName & "</a>&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td> " & sRound & " &nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"
                                    '                                   sText += "<tr><td" & sFirstPlace & ">" & j & "</td><td>" & sSkierName & "&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td> " & sRound & " &nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"
                                    sText += "<tr><td" & sFirstPlace & ">" & j & "</td><td><a href=""http://localhost:53474/Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&SN=" & sSkierName & """ target=""_blank"" >" & sSkierName & "</a>&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td> " & sRound & " &nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"

                                End If
                            Else  'Division changed.  Reset the variables. Put first skier in this division in first place
                                sStringMemberIDs = ""
                                sStringMemberIDs += sMemberID & ","
                                sSkipPlace = 0
                                sTmpAgeGroup = sAgeGroup
                                sText += "<tr><td class=""table-warning"" colspan=""3"" ><b>" & sAgeGroup & "</b></td><td colspan=""3"" class=""table-light"">&nbsp;</td></tr>"
                                sTmpEventScore = sEventScore
                                j = 1
                                sFirstPlace = ""
                                If j = 1 Then sFirstPlace = ""  ' Class=""table-warning"" "

                                sText += "<tr><td" & sFirstPlace & ">" & j & "</td><td><a href=""http://localhost:53474/Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&SN=" & sSkierName & "  >" & sSkierName & "</a>&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td> " & sRound & " &nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"

                                '                               sText += "<tr><td" & sFirstPlace & ">" & j & "</td><td>" & sSkierName & " &nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td>" & sRound & "&nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"
                                '                              sText += "<tr><td" & sFirstPlace & ">" & j & "</td><td><a asp-page=""/Trecap.aspx?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&SN=" & sSkierName & " target=""_blank"" >" & sSkierName & "</a>&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td>" & sRound & "&nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"
                            End If
                        Loop
                    Else
                        sText += "<tr><td colspan=""6"">No Scores Found.</td></tr>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error at BestRound"
                sErrDetails = "SQL= " & sSql & "<br>" & sEventCode & "PlacementBestRound Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"
            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText

    End Function
    Friend Function PlacementBestRoundNOPS(ByVal SanctionID As String, ByVal CompetitionFormatCode As String, ByVal EventCode As String) As String
        'Competition Style is not yet defined.  Will be a code like EMS code for best of 3 rounds, 1 round qualifier and final, etc.
        'Will probably have to add round number, number of qualifiers, and how ties are handled as optional parameters
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = EventCode
        Dim sRnd As Byte = 0  '0 means include all
        'Use zero 0 for fields not in use
        Dim sFormatCode As String = CompetitionFormatCode
        Dim sSql As String = PlacementSQL(sSanctionID, sEventCode, sFormatCode, 0, True)  'Make sure Select works with , separated list
        If Left(sSql, 5) = "Error" Then
            Return sSql
            Exit Function
        End If
        'Process the data by class, division, score desc
        Dim sMsg As String = ""
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sTmpMemberID As String = ""
        Dim sStringMemberIDs As String = ""
        Dim sAgeGroup As String = ""
        Dim sTmpAgeGroup As String = ""
        Dim sEvent As String = ""
        Dim sEventClass As String = ""
        Dim sTmpEvent As String = ""
        Dim sEventScore As String = ""
        Dim sNopsScore As Decimal = 0
        Dim sTmpNopsScore As Decimal = 0
        Dim sGender As String = ""
        Dim sTmpGender As String = ""
        Dim sTmpEventScore As String = ""
        Dim TmpMemberID As String = ""
        Dim TmpEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sFirstPlace As String = ""
        Dim sConn As String = ""
        Dim sFirstLoop As Boolean = True
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim sSkipPlace As Int16 = 0
        Dim sStopHere As Boolean = False
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: " & sEventCode & "PlacementBestRoundNOPS could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sText As String = "<Table class=""table table-striped border-1 "">"
        Select Case sEventCode
            Case "S"
                sText += "<thead>"
                sText += "<tr class=""table-primary"" ><th colspan=""5"">Slalom Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
            Case "T"
                sText += "<thead>"
                sText += "<tr class=""table-primary"" ><th colspan=""5"">Trick Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
            Case "J"
                sText += "<thead>"
                sText += "<tr class=""table-primary"" ><th colspan=""5"">Jump Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
        End Select

        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = sSql
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sMemberID = CStr(MyDataReader.Item("MemberID"))
                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            sGender = CStr(MyDataReader.Item("Gender"))
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sEventScore = CStr(MyDataReader.Item("EventScore"))
                            sNopsScore = CDec(MyDataReader.Item("NopsScore"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                            'First record is always first place in current age division
                            If sFirstLoop = True Then
                                sTmpGender = sGender
                                sFirstLoop = False
                                sFirstPlace = ""
                                j = 0
                            End If
                            If sTmpGender = sGender Then  'In same division - continue
                                i = InStr(sStringMemberIDs, sMemberID)
                                If i < 1 Then ' If MemberID not in list add skier score.
                                    sStringMemberIDs += sMemberID & "," ' Add MemberID to list
                                    If sTmpNopsScore = sNopsScore Then 'Calculate ties
                                        sSkipPlace += 1
                                        'don't increment j
                                    Else
                                        sTmpNopsScore = sNopsScore 'reset to new performance level
                                        If sSkipPlace > 0 Then  'counts number of ties
                                            j += 1 + sSkipPlace
                                            sSkipPlace = 0 'reset tie counter
                                        Else
                                            j += 1  'Normal increment
                                        End If
                                        sStringMemberIDs += sMemberID & ","
                                    End If
                                    sFirstPlace = ""
                                    If j = 1 Then
                                        sFirstPlace = " Class=""table-warning"" "
                                    End If

                                    sText += "<tr" & sFirstPlace & "> <td> " & j & "</td><td>" & sSkierName & "&nbsp; &nbsp;</td><td>" & sAgeGroup & "&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td> " & sRound & " &nbsp; &nbsp;</td><td>" & sNopsScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"

                                End If
                            Else  'Division changed.  Reset the variables. Put first skier in this division in first place
                                sStringMemberIDs = ""
                                sStringMemberIDs += sMemberID & ","
                                sSkipPlace = 0
                                sTmpGender = sGender
                                sTmpAgeGroup = sAgeGroup
                                sTmpEventScore = sEventScore
                                sTmpNopsScore = sNopsScore
                                sFirstPlace = ""
                                j = 1
                                If j = 1 Then
                                    sFirstPlace = " Class=""table-warning"" "
                                End If
                                sText += "<tr " & sFirstPlace & " ><td>" & j & "</td><td>" & sSkierName & "&nbsp; &nbsp;</td><td>" & sAgeGroup & "&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td>" & sRound & "&nbsp; &nbsp;</td><td>" & sNopsScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"
                            End If
                        Loop
                    Else
                        sText += "<tr><td colspan=""5"">No Scores Found.</td></tr>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: SQL= " & sSql & "<br>" & sEventCode & "PlacementBestRoundNOPS Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"

            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function PlacementXRound(ByVal SanctionID As String, ByVal CompetitionFormatCode As String, ByVal EventCode As String, ByVal Rnd As Byte) As String
        'CompetitionFormatCode is ByRound or BestRound.
        'BestRound produces placement list based on all performances by Class, AgeGroup.
        'ByRound gives placement in each round.  Theory is that WSTIMS only allows correct group of skiers to advance to next round.  Not necessary to redo the calculations.
        'Need to create Team Placement for NCWSA
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = EventCode
        Dim sRnd As Byte = Rnd
        'Use zero 0 for fields not in use
        Dim sFormatCode As String = CompetitionFormatCode
        Dim sSql As String = PlacementSQL(sSanctionID, sEventCode, sFormatCode, sRnd, False)  'Make sure Select works with , separated list
        If Left(sSql, 5) = "Error" Then
            Return sSql
            Exit Function
        End If
        'Process the data by class, division, score desc
        Dim sMsg As String = ""
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sTmpMemberID As String = ""
        Dim sStringMemberIDs As String = ""
        Dim sAgeGroup As String = ""
        Dim sTmpAgeGroup As String = ""
        Dim sEvent As String = ""
        Dim sEventClass As String = ""
        Dim sTmpEventClass As String = ""
        Dim sTmpEvent As String = ""
        Dim sEventScore As String = ""
        Dim sTmpEventScore As String = ""
        Dim TmpMemberID As String = ""
        Dim TmpEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sConn As String = ""
        Dim sFirstLoop As Boolean = True
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim sSkipPlace As Int16 = 0
        Dim sStopHere As Boolean = False
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: " & sEventCode & "PlacementXRound could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sText As String = "<Table class=""table table-striped border-1 "">"
        Select Case sEventCode
            Case "S"
                sText += "<thead>"
                sText += "<tr class=""table-primary"" ><th colspan=""5"">Slalom Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
            Case "T"
                sText += "<thead>"
                sText += "<tr class=""table-primary"" ><th colspan=""5"">Trick Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
            Case "J"
                sText += "<thead>"
                sText += "<tr class=""table-primary"" ><th colspan=""5"">Jump Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
        End Select

        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = sSql
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sMemberID = CStr(MyDataReader.Item("MemberID"))
                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sEventScore = CStr(MyDataReader.Item("EventScore"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                            'First record is always first place in current age division
                            If sFirstLoop = True Then
                                sTmpAgeGroup = sAgeGroup
                                sFirstLoop = False
                                j = 0
                                sText += "<tr><td colspan=""7"">Class " & sEventClass & "</td></tr>"
                                sTmpEventClass = sEventClass
                            End If
                            If sTmpEventClass <> sEventClass Then
                                sText += "<tr><td colspan=""7"">Class " & sEventClass & "</td></tr>"
                                sTmpEventClass = sEventClass
                            End If
                            If sTmpAgeGroup = sAgeGroup Then  'In same division - continue
                                i = InStr(sStringMemberIDs, sMemberID)
                                If i < 1 Then ' If MemberID not in list add skier score.
                                    sStringMemberIDs += sMemberID & "," ' Add MemberID to list
                                    If sTmpEventScore = sEventScore Then 'Calculate ties
                                        sSkipPlace += 1
                                        'don't increment j
                                    Else
                                        sTmpEventScore = sEventScore 'reset to new performance level
                                        If sSkipPlace > 0 Then  'counts number of ties
                                            j += 1 + sSkipPlace
                                            sSkipPlace = 0 'reset tie counter
                                        Else
                                            j += 1  'Normal increment
                                        End If
                                        sStringMemberIDs += sMemberID & ","
                                    End If
                                    sText += "<tr><td>" & j & "</td><td>" & sSkierName & "&nbsp; &nbsp;</td><td>" & sAgeGroup & "&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td> " & sRound & " &nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"

                                End If
                            Else  'Division changed.  Reset the variables. Put first skier in this division in first place
                                sStringMemberIDs = ""
                                sStringMemberIDs += sMemberID & ","
                                sSkipPlace = 0
                                sTmpAgeGroup = sAgeGroup
                                sTmpEventScore = sEventScore
                                j = 1
                                sText += "<tr><td>" & j & "</td><td>" & sSkierName & "&nbsp; &nbsp;</td><td>" & sAgeGroup & "&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td>" & sRound & "&nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"
                            End If
                        Loop
                    Else
                        sText += "<tr><td colspan=""7"">No Scores Found.</td></tr>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: SQL= " & sSql & "<br>" & sEventCode & "PlacementXRound Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"

            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText

    End Function
    Friend Function PlacementXRoundNOPS(ByVal SanctionID As String, ByVal CompetitionFormatCode As String, ByVal EventCode As String, ByVal Rnd As Byte) As String
        'Competition Style is not yet defined.  Will be a code like EMS code for best of 3 rounds, 1 round qualifier and final, etc.
        'Will probably have to add round number, number of qualifiers, and how ties are handled as optional parameters
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = EventCode
        Dim sRnd As Byte = Rnd
        'Use zero 0 for fields not in use
        Dim sFormatCode As String = CompetitionFormatCode
        Dim sSql As String = PlacementSQL(sSanctionID, sEventCode, sFormatCode, sRnd, True)  'Make sure Select works with , separated list
        If Left(sSql, 5) = "Error" Then
            Return sSql
            Exit Function
        End If
        'Process the data by class, division, score desc
        Dim sMsg As String = ""
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sTmpMemberID As String = ""
        Dim sStringMemberIDs As String = ""
        Dim sAgeGroup As String = ""
        Dim sTmpAgeGroup As String = ""
        Dim sEvent As String = ""
        Dim sEventClass As String = ""
        Dim sTmpEvent As String = ""
        Dim sEventScore As String = ""
        Dim sNopsScore As Decimal = 0
        Dim sTmpNopsScore As Decimal = 0
        Dim sGender As String = ""
        Dim sTmpGender As String = ""
        Dim sTmpEventScore As String = ""
        Dim TmpMemberID As String = ""
        Dim TmpEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sConn As String = ""
        Dim sFirstLoop As Boolean = True
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim sSkipPlace As Int16 = 0
        Dim sStopHere As Boolean = False
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: " & sEventCode & "PlacementXRoundNOPS could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sText As String = "<Table class=""table table-striped border-1 "">"
        Select Case sEventCode
            Case "S"
                sText += "<thead>"
                sText += "<tr class=""table-primary"" ><th colspan=""5"">Slalom Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
            Case "T"
                sText += "<thead>"
                sText += "<tr class=""table-primary"" ><th colspan=""5"">Trick Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
            Case "J"
                sText += "<thead>"
                sText += "<tr class=""table-primary"" ><th colspan=""5"">Jump Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
        End Select

        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = sSql
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sMemberID = CStr(MyDataReader.Item("MemberID"))
                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            sGender = CStr(MyDataReader.Item("Gender"))
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sEventScore = CStr(MyDataReader.Item("EventScore"))
                            sNopsScore = CDec(MyDataReader.Item("NopsScore"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                            'First record is always first place in current age division
                            If sFirstLoop = True Then
                                sTmpGender = sGender
                                sFirstLoop = False
                                j = 0
                            End If
                            If sTmpGender = sGender Then  'In same division - continue
                                i = InStr(sStringMemberIDs, sMemberID)
                                If i < 1 Then ' If MemberID not in list add skier score.
                                    sStringMemberIDs += sMemberID & "," ' Add MemberID to list
                                    If sTmpNopsScore = sNopsScore Then 'Calculate ties
                                        sSkipPlace += 1
                                        'don't increment j
                                    Else
                                        sTmpNopsScore = sNopsScore 'reset to new performance level
                                        If sSkipPlace > 0 Then  'counts number of ties
                                            j += 1 + sSkipPlace
                                            sSkipPlace = 0 'reset tie counter
                                        Else
                                            j += 1  'Normal increment
                                        End If
                                        sStringMemberIDs += sMemberID & ","
                                    End If
                                    sText += "<tr><td>" & j & "</td><td>" & sSkierName & "&nbsp; &nbsp;</td><td>" & sAgeGroup & "&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td> " & sRound & " &nbsp; &nbsp;</td><td>" & sNopsScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"

                                End If
                            Else  'Division changed.  Reset the variables. Put first skier in this division in first place
                                sStringMemberIDs = ""
                                sStringMemberIDs += sMemberID & ","
                                sSkipPlace = 0
                                sTmpGender = sGender
                                sTmpAgeGroup = sAgeGroup
                                sTmpEventScore = sEventScore
                                sTmpNopsScore = sNopsScore
                                j = 1
                                sText += "<tr><td>" & j & "</td><td>" & sSkierName & "&nbsp; &nbsp;</td><td>" & sAgeGroup & "&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td>" & sRound & "&nbsp; &nbsp;</td><td>" & sNopsScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"
                            End If
                        Loop
                    Else
                        sText += "<tr><td colspan=""5"">No Scores Found.</td></tr>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: SQL= " & sSql & "<br>" & sEventCode & "PlacementXRoundNOPS Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"

            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function PlacementSQL(ByVal SanctionID As String, ByVal EventCode As String, ByVal CompetitionFormatCode As String, ByVal RndNum As Byte, ByVal UseNops As Boolean) As String
        Dim sReturn As String
        '  vSlalomPlacement, vTrickPlacement, vJumpPlacement Incudes all necessary performance information for each event. 
        '    Separate View for each event. Perhaps one with all performances for overall.  
        'May have to expand view to include NOPS or other handicapping system.  Use standard format until WSTIMS can pass more detailed information.

        Dim sResults As String = ""
        Dim sMsg As String = ""
        Dim sSanctionID As String = Trim(SanctionID)
        Dim sEvent As String = EventCode
        Dim sFormatCode As String = CompetitionFormatCode
        Dim sRound As Byte = RndNum    '1, 2 3, or 4.  If 0 include all
        Dim sRoundText As String = ""
        Select Case sRound
            Case 0 ' leave out
            Case 1
                sRoundText = " and Round = 1 "
            Case 2
                sRoundText = " and Round = 2 "
            Case 3
                sRoundText = " and Round = 3 "
            Case 4
                sRoundText = " and Round = 4 "
            Case Else  'Include all
        End Select
        Dim sViewName As String = ""
        Dim sSQL As String = ""
        Dim sRound1List As String = ""
        Dim sRound2List As String = ""
        Dim sRound3List As String = ""
        Dim sRound4List As String = ""
        Dim sUseNops As Boolean = UseNops
        Dim sOrderBy As String = " Order By EventClass, AgeGroup,  EventScore DESC"
        If sUseNops = True Then
            sOrderBy = " Order By  Gender, NopsScore DESC"
        End If
        'USES [EVENT]ResultsView.  At start displays only skier names in seeded order based on WSTIMS registration data.
        'As results are scored, the view is automatically updated.
        'Each time the display refreshes the new scores appear in the running order display, and placement display is updated.
        ' Running order and placement lists are displayed separately

        Select Case sEvent
            Case "S"
                sViewName = " waterskiProd23.dbo.vSlalomResults "
            Case "T"
                sViewName = " waterskiProd23.dbo.vTrickResults "
            Case "J"
                sViewName = " waterskiProd23.dbo.vJumpResults "
        End Select
        Select Case sFormatCode
            Case "CO101", "CO102", "CO103", "CO104" 'Best of 1,2,3, or 4 rounds.  2 columns - one for skier entry list.  Second for placements
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, NopsScore, EventScore, EventClass, TeamCode, Round, Event, EventScoreDesc "
                sSQL += " From " & sViewName & " where SanctionID = '" & sSanctionID & "' " & sRoundText & sOrderBy

                Return sSQL
                Exit Function
'           
            Case "CO105" 'Preliminary round and Final(no tie break previous round)
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, NopsScore, EventScore, EventClass, TeamCode, Round, Event, EventScoreDesc "
                sSQL += " From " & sViewName & " where SanctionID = '" & sSanctionID & "' " & sRoundText & sOrderBy

            Case "CO106" 'Preliminary,Semi-final and Final(no tie break previous rounds)

            Case "CO107" 'Preliminary,Quarter,Semi-final and Final(no tie break previous rounds)

                'How are ties handled?

                'How are ties handled?
            Case "CO108" 'Qualification on best of 2 rounds and Final


            Case "CO109" 'Qualification on best of 2 rounds,Semi-final and Final

            Case "CO110" 'Qualification on best of 2 rounds,Quarter,Semi-final and Final

            Case "CO111" 'Qualification on best of 3 rounds and Final

            Case "CO112" 'Preliminary round and Final(tie break with previous round)

            Case "CO113" 'Preliminary round,Semi-final and Final(tie break with previous rounds)

            Case "CO114" 'Preliminary round,Quarter,Semi-final and Final(tie break with previous rounds)

            Case "CO115" 'Total of 2 best rounds

            Case "CO116" 'Total of 3 best rounds

            Case "CO117" 'Total of 4 best rounds

            Case "CO118" 'Performances

            Case "CO199" 'Overall on best score per event(2 rounds)

            Case "CO120" 'Overall on best score per event(3 rounds)

            Case "CO121" 'Overall on best score per event(4 rounds)

            Case "CO122" 'Overall Preliminary and Overall Final as round 3

        End Select

        Return sResults
        Return sReturn
    End Function
    Friend Function IndivSlalomResults(ByVal SanctionID As String, ByVal MemberID As String) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID

        Dim SQL As String = ""
        SQL = "SELECT * from dbo.vSlalomResults "
        SQL += " Where SanctionID ='" & sSanctionID & "' And MemberID = '" & Trim(sMemberID) & "'"
        SQL += "  ORDER BY AgeGroup, MemberID, Round "
        Dim sMsg As String = ""
        Dim sSkierName As String = ""
        Dim sAgeGroup As String = ""
        Dim sEvent As String = ""
        Dim sEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sConn As String = ""


        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: IndivSlalomResults could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sText As String = "<Table class=""table table-striped border-1 "">"
        sText += "<thead><tr class=""table-primary""><th colspan=""5"">Slalom Results</th></tr>"
        sText += "<tr><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>Buoys</th><th>Detail</th></tr></thead>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = SQL
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEventScore = CStr(MyDataReader.Item("EventScore"))
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))

                            sText += "<tr><td>" & sSkierName & "&nbsp; &nbsp;</td><td>" & sAgeGroup & "&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td>" & sRound & "&nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"

                        Loop
                    Else
                        sText += "<tr><td colspan=""5"">No Scores Found for selected skier.</td></tr>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: SQL= " & SQL & "<br>IndivSlalomResults Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"

            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function

    Friend Function IndivTrickResults(ByVal SanctionID As String, ByVal MemberID As String) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID

        'Below comments are history.  Have created a view which supplies the required data.  Below refers to sql code in LiveScorebook project.
        '       From Dave's PHP code -  Made it work in sql server with modifications listd below
        '       SQLite doesn't like fully qualified names, has no Concat function and probably has other complaints.
        '       This query needs to be reworked into SQLite syntax
        '       Added fully qualified database table name.
        '       Removed date formatting at DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, put bogus date in ssi.LastUpdateDate >= '1/1/2022'.
        '       Generates list of all performances in all events by all skiers.  Need to fix the formatting and figure out what CURDATE() is in ssi.LastUpdateDate >= CURDATE() "
        'NOW() didn't work.  Maybe CreateDAte()?
        Dim SQL As String = ""
        SQL = "SELECT * from dbo.vTrickResults "
        SQL += " Where SanctionID ='" & sSanctionID & "' And MemberID = '" & Trim(sMemberID) & "'"
        SQL += "  ORDER BY AgeGroup, MemberID, Round "
        Dim sMsg As String = ""
        Dim sSkierName As String = ""
        Dim sAgeGroup As String = ""
        Dim sEvent As String = ""
        Dim sEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sConn As String = ""


        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: IndivTrickResults could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sTableWidth As String = "100%"
        Dim sText As String = "<Table class=""table table-striped border-1 "">"
        sText += "<thead><tr><td colspan=""4"">Trick Results</td></tr>"
        sText += "<tr Class=""table-primary""><th colspan=""5"">Trick Results</th></tr>"
        sText += "<tr><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>Points</th><th>Detail</th></tr></thead>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = SQL
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sEventScore = CStr(MyDataReader.Item("EventScore"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))

                            sText += "<tr><td>" & sSkierName & "&nbsp; &nbsp;</td><td>" & sAgeGroup & "&nbsp; &nbsp;</td></td><td>" & sEventClass & "&nbsp; &nbsp;</td><td>" & sRound & "&nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"

                        Loop
                    Else
                        sText += "<tr><td colspan=""5"">No Scores Found For selected skier.</td></tr>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: Can't retrieve Trick Scores" ' " SQL= " & SQL & "<br>IndivTrickResults Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"

            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function IndivJumpResults(ByVal SanctionID As String, ByVal MemberID As String) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID


        Dim SQL As String = ""
        SQL = "SELECT * from dbo.vJumpResults "
        SQL += " Where SanctionID ='" & sSanctionID & "' And MemberID = '" & Trim(sMemberID) & "'"
        SQL += "  ORDER BY AgeGroup, MemberID, Round "
        Dim sMsg As String = ""
        Dim sSkierName As String = ""
        Dim sAgeGroup As String = ""
        Dim sEvent As String = ""
        Dim sEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sConn As String = ""
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: IndivJumpResults could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sText As String = "<Table class=""table table-striped border-1 "">"
        sText += "<thead><tr><td colspan=""4"">Jump Results</td></tr>"
        sText += "<tr><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>Distance</th><th>Detail</th></tr></thead>"

        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = SQL
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sEventScore = CStr(MyDataReader.Item("EventScore"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))

                            sText += "<tr><td>" & sSkierName & "&nbsp; &nbsp;</td><td>" & sAgeGroup & "&nbsp; &nbsp;</td><td>" & sEventClass & "&nbsp; &nbsp;</td><td>" & sRound & "&nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"

                        Loop
                    Else
                        sText += "<tr><td colspan=""5"">No Scores Found for selected skier.</td></tr>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: Can't retrieve Jump Scores. " 'SQL= " & SQL & "<br>IndivJumpResults Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"

            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function ChkRnd4Scores(ByVal SanctionID As String, ByVal EventCode As String, ByVal Rnd As Byte) As String
        'Checks to see if any scores have been posted in the speified event and round
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = EventCode
        Dim sRound As Byte = Rnd
        Dim sErrDetails As String = ""
        Dim sMsg As String = ""
        Dim sView2Use As String = ""
        Select Case sEventCode
            Case "S"
                sView2Use = "vSlalomResults"
            Case "T"
                sView2Use = "vTrickResults"
            Case "J"
                sView2Use = "vJumpResults"
        End Select
        Dim sSQL As String = "Select EventScore from " & sView2Use & " where SanctionID = '" & sSanctionID & "' and Round = " & sRound & " and EventScore > 0 "
        Dim sConn As String = ""
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error - Close App and Try Again"
            sErrDetails = "Error: ChkRnd4Scores could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmd As New OleDb.OleDbCommand
        Using cmd
            Try
                cmd.Connection = Cnnt 'New OleDbConnection(sConn)
                cmd.CommandText = sSQL
                cmd.Connection.Open()
                sMsg = CStr(cmd.ExecuteScalar)
            Catch ex As Exception
                sMsg = "Error at ChkRnd4Scores"
                sErrDetails = "ChkRnd4Scores caught SQL= " & sSQL & " " & ex.Message & " " & ex.StackTrace
            End Try
        End Using
        If sMsg = Nothing Then
            sMsg = "0"
        End If
        Return sMsg
    End Function
    Friend Function RecapSlalom(ByVal SanctionID As String, ByVal MemberID As String, ByVal ageGroup As String, ByVal SkierName As String) As String
        'Pulled from wfwShowScoreRecap.php
        ' 
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sText As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID
        Dim sAgeGroup As String = ageGroup
        Dim sSkierName As String = ""
        Dim sSkierRound As String = ""
        Dim sSkierEvent As String = ""
        Dim sSQL As String = ""
        Dim sRound As Int16 = 0
        Dim sTmpRound As Int16 = 0
        Dim sTmpName As String = ""
        Dim sScore As String = ""
        Dim sPsLnLngth As String = ""
        Dim sNote As String = ""
        Dim sReride As String = ""
        Dim sProtected As String = ""
        Dim sRerideReason As String = ""

        ' 3 event scores in available data.
        ' 23S108, James Bryans,             OM, 000107150,  at least 1 round overall
        ' 23S108, Tristan Duplan-Fribourg, JM, 000181068, at least 1 round overall
        '       If sSkierEvent = "Slalom" Then
        sSQL = "Select [Round], Score, PassLineLength, Note, Reride, ScoreProt, RerideReason "
        sSQL += " From WaterskiProd23.dbo.SlalomRecap "
        sSQL += "Where SanctionId ='" & sSanctionID & "' AND MemberId='" & sMemberID & "' And AgeGroup='" & sAgeGroup & "' "
        sSQL += " Order By [Round], SkierRunNum ASC "
        '       End If
        Dim sConn As String = ""
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: RecapSlalom could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        sText = "<Table class=""table table-striped border-1 "">"
        sText += "<thead><tr><td colspan=""6"" clase=""table-warning""><b>Slalom Recap</b></td></tr>"
        sText += "<tr><th>Score</th><th>Pass/ Line Length</th><th>Note</th><th>Reride</th><th>Protest</th><th>Reride Reason</th></tr></thead>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    cmdRead.CommandText = sSQL
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()
                            If IsDBNull(MyDataReader.Item("Score")) Then
                                sScore = "N/A"
                            Else
                                sScore = CStr(MyDataReader.Item("Score"))
                            End If
                            If IsDBNull(MyDataReader.Item("PassLineLength")) Then
                                sPsLnLngth = "N/A"
                            Else
                                sPsLnLngth = CStr(MyDataReader.Item("PassLineLength"))
                            End If

                            If IsDBNull(MyDataReader.Item("Note")) Then
                                sNote = "N/A"
                            Else
                                sNote = CStr(MyDataReader.Item("Note"))

                            End If
                            If IsDBNull(MyDataReader.Item("Reride")) Then
                                sReride = "N/A"
                            Else
                                sReride = CStr(MyDataReader.Item("Reride"))
                            End If
                            If IsDBNull(MyDataReader.Item("ScoreProt")) Then
                                sProtected = "N/A"
                            Else
                                sProtected = CStr(MyDataReader.Item("ScoreProt"))
                            End If
                            If IsDBNull(MyDataReader.Item("RerideReason")) Then
                                sRerideReason = ""
                            Else
                                sRerideReason = CStr(MyDataReader.Item("RerideReason"))
                            End If
                            If IsDBNull(MyDataReader.Item("Round")) Then
                                sRound = ""
                            Else
                                sRound = CStr(MyDataReader.Item("Round"))
                            End If
                            If sTmpName <> sSkierName Then  'display skier name once
                                sText += "<tr><td colspan="" 6"">Recap for " & sSkierName & "</td></tr>"
                                sTmpName = sSkierName
                            End If
                            If sTmpRound <> sRound Then  '
                                sText += "<tr><td colspan=""2""><b>Slalom Round " & sRound & "</b></td><td colspan=""4"">&nbsp;</td></tr>"
                                sTmpRound = sRound
                            End If
                            sText += "<tr><td>" & sScore & "&nbsp; &nbsp;</td><td>" & sPsLnLngth & "&nbsp; &nbsp;</td><td>" & sNote & "&nbsp; &nbsp;</td><td>" & sReride & "&nbsp; &nbsp;</td><td>" & sProtected & "&nbsp; &nbsp;</td><td>" & sRerideReason & "&nbsp; &nbsp;</td></tr>"
                        Loop
                    Else
                        sText += "<tr><td colspan=""6"">No Slalom results Found For selected skier.</td></tr>"
                    End If 'end of has rows
                End Using

            Catch ex As Exception
                sMsg += "Error Can 't retrieve Slalom Scores. " 'SQL= " & SQL & "<br>IndivJumpResults Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"
                sErrDetails = ex.Message & "<br> " & ex.StackTrace & "<br>""error at SRecapQry:SQL= " & sSQL
            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function RecapTrick(ByVal SanctionID As String, ByVal MemberID As String, ByVal ageGroup As String, ByVal SkierName As String) As String
        'Pulled from wfwShowScoreRecap.php
        ' 
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sText As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID
        Dim sAgeGroup As String = ageGroup
        Dim sSkierName As String = SkierName
        Dim sSkierRound As String = ""
        Dim sSkierEvent As String = ""
        Dim sSQL As String = ""
        Dim sRound As Int16 = 0
        Dim sTmpRound As Int16 = 0
        Dim sScore As String = ""
        Dim sPsLnLngth As String = ""
        Dim sNote As String = ""
        Dim sReride As String = ""
        Dim sProtected As String = ""
        Dim sRerideReason As String = ""

        ' 3 event scores in available data.
        ' 23S108, James Bryans,             OM, 000107150,  at least 1 round overall
        ' 23S108, Tristan Duplan-Fribourg, JM, 000181068, at least 1 round overall
        '       If sSkierEvent = "Slalom" Then
        sSQL = "Select P.PassNum, P.Seq, P.Skis, P.Score, P.Code, P.Results, S.ScorePass1, S.ScorePass2, S.Score As TotalScore "
        sSQL += " , V.Pass1VideoUrl, V.Pass2VideoUrl, S.Pass1VideoUrl as Pass1VideoUrlX, S.Pass2VideoUrl as Pass2VideoUrlX  "
        sSQL += " From TrickPass P "
        sSQL += " Join TrickScore S on S.SanctionId = P.SanctionId And S.MemberId = P.MemberId And S.AgeGroup = P.AgeGroup And S.Round = P.Round "
        sSQL += " Left Outer Join TrickVideo V ON V.SanctionId = P.SanctionId And V.MemberId = P.MemberId And V.AgeGroup = P.AgeGroup And V.Round = P.Round "
        sSQL += "  WHERE P.SanctionId ='" & sSanctionID & "' AND P.MemberId='" & sMemberID & "' And P.AgeGroup='" & sAgeGroup & "' "
        sSQL += " Order By P.PassNum ASC, P.Seq ASC "
        '       End If
        Dim sConn As String = ""
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: IndivJumpResults could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        sText = "<Table class=""table table-striped border-1 "">"
        sText += "<thead><tr><td colspan=""6"">Slalom Recap</td></tr>"
        sText += "<tr><th>Name</th><th>Score</th><th>Pass</th><th>Feet</th><th>Meters</th><th>Speed</th><th>RmpHt</th><th>Score Protect</th><th>Reride Reason</th></tr></thead>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    cmdRead.CommandText = sSQL
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()

                            sScore = CStr(MyDataReader.Item("ScoreFeet"))
                            sPsLnLngth = CStr(MyDataReader.Item("SkierName"))
                            sNote = CStr(MyDataReader.Item("AgeGroup"))
                            sReride = CStr(MyDataReader.Item("EventClass"))
                            sProtected = CStr(MyDataReader.Item("EventScore"))
                            If IsDBNull(MyDataReader.Item("RerideReason")) Then
                                sRerideReason = ""
                            Else
                                sRerideReason = CStr(MyDataReader.Item("RerideReason"))
                            End If
                            sRound = CStr(MyDataReader.Item("Round"))
                            If sTmpRound <> sRound Then
                                sText += "<tr><td colspan="" 6"">Jump Round " & sRound & "</td></tr>"
                                sTmpRound = sRound
                            End If
                            sText += "<tr><td>" & sScore & "&nbsp; &nbsp;</td><td>" & sPsLnLngth & "&nbsp; &nbsp;</td><td>" & sNote & "&nbsp; &nbsp;</td><td>" & sReride & "&nbsp; &nbsp;</td><td>" & sProtected & "&nbsp; &nbsp;</td><td>" & sRerideReason & "&nbsp; &nbsp;</td></tr>"
                        Loop
                    Else
                        sText += "<tr><td colspan=""6"">No Slalom results Found For selected skier.</td></tr>"
                    End If 'end of has rows
                End Using

            Catch ex As Exception
                sMsg += "Error Can 't retrieve Trick Scores. " 'SQL= " & SQL & "<br>IndivJumpResults Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"
                sErrDetails = "error at RecapTrick:SQL= " & sSQL & "<br>" & ex.Message & " " & ex.StackTrace & "<br>"
            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function RecapJump(ByVal SanctionID As String, ByVal MemberID As String, ByVal AgeGroup As String, ByVal SkierName As String) As String
        'Pulled from wfwShowScoreRecap.php
        ' 
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sText As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID
        Dim sAgeGroup As String = AgeGroup
        Dim sSkierName As String = SkierName
        Dim sSkierRound As String = ""
        Dim sSkierEvent As String = ""
        Dim sSQL As String = ""
        Dim sRound As Int16 = 0
        Dim sTmpRound As Int16 = 0
        Dim sTmpName As String = ""
        Dim sFeet As String = ""
        Dim sMeters As String = ""
        Dim sPass As String = ""
        Dim sBSpeed As String = ""
        Dim sRmpHt As String = ""
        Dim sNote As String = ""
        Dim sReride As String = ""
        Dim sProtected As String = ""
        Dim sRerideReason As String = ""

        ' 3 event scores in available data.
        ' 23S108, James Bryans,             OM, 000107150,  at least 1 round overall
        ' 23S108, Tristan Duplan-Fribourg, JM, 000181068, at least 1 round overall
        sSQL = "Select SanctionID, AgeGroup, [round], ScoreFeet, ScoreMeters, PassNum, "
        sSQL += " Results, BoatSpeed, RampHeight, ScoreProt, Reride, RerideReason "
        sSQL += " From JumpRecap "
        sSQL += " Where SanctionId ='" & sSanctionID & "' AND MemberId='" & sMemberID & "' And AgeGroup='" & sAgeGroup & "' "
        sSQL += " Order By [round], PassNum ASC , ScoreFeet "

        Dim sConn As String = ""
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: JumpRecap could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        sText = "<Table class=""table table-striped border-1 "">"
        sText += "<thead><tr><td colspan=""6"">Jump Recap</td></tr>"
        sText += "<tr><th>Age Grp<th>Pass</th><th> Ft  Mtr </th><th>Speed</th><th>RmpHt</th>Reride<th>Reride</th><th>Score Protect</th><th>Reride Reason</th></tr></thead>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    cmdRead.CommandText = sSQL
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()
                            If IsDBNull(MyDataReader.Item("PassNum")) Then
                                sPass = "N/A"
                            Else
                                sPass = CStr(MyDataReader.Item("PassNum"))
                            End If

                            If IsDBNull(MyDataReader.Item("ScoreFeet")) Then
                                sFeet = "N/A"
                            Else
                                sFeet = CStr(MyDataReader.Item("ScoreFeet"))
                            End If
                            If IsDBNull(MyDataReader.Item("ScoreMeters")) Then
                                sMeters = "N/A"
                            Else
                                sMeters = CStr(MyDataReader.Item("ScoreMeters"))
                            End If
                            If IsDBNull(MyDataReader.Item("AgeGroup")) Then
                                sAgeGroup = "N/A"
                            Else
                                sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            End If
                            If IsDBNull(MyDataReader.Item("BoatSpeed")) Then
                                sBSpeed = "N/A"
                            Else
                                sBSpeed = CStr(MyDataReader.Item("BoatSpeed"))
                            End If
                            If IsDBNull(MyDataReader.Item("RampHeight")) Then
                                sRmpHt = "N/A"
                            Else
                                sRmpHt = CStr(MyDataReader.Item("RampHeight"))
                            End If
                            If IsDBNull(MyDataReader.Item("Reride")) Then
                                sReride = "N"
                            Else
                                sReride = CStr(MyDataReader.Item("Reride"))
                            End If
                            If IsDBNull(MyDataReader.Item("ScoreProt")) Then
                                sProtected = "N/A"
                            Else
                                sProtected = CStr(MyDataReader.Item("ScoreProt"))
                            End If
                            If IsDBNull(MyDataReader.Item("RerideReason")) Then
                                sRerideReason = ""
                            Else
                                sRerideReason = CStr(MyDataReader.Item("RerideReason"))
                            End If
                            If IsDBNull(MyDataReader.Item("Round")) Then
                                sRound = "N/A"
                            Else
                                sRound = CStr(MyDataReader.Item("Round"))
                            End If
                            If sTmpName <> sSkierName Then
                                sText += "<tr><td colspan=""11"">Recap for " & sSkierName & "</td></tr>"
                            End If
                            If sTmpRound <> sRound Then
                                sText += "<tr><td colspan="" 2""><b>Round " & sRound & "</b></td><td colspan="" 9""></td></tr>"
                                sTmpRound = sRound
                            End If
                            sText += "<tr><td>" & sAgeGroup & "&nbsp; &nbsp;</td><td>" & sPass & "&nbsp; &nbsp;</td><td>" & sFeet & "&nbsp;" & sMeters & "</td><td>" & sBSpeed & "&nbsp; &nbsp;</td><td>" & sRmpHt & "&nbsp; &nbsp;</td><td>" & sReride & "&nbsp; &nbsp;</td><td>" & sProtected & "&nbsp; &nbsp;</td><td>" & sRerideReason & "&nbsp; &nbsp;</td></tr>"
                        Loop
                    Else
                        sText += "<tr><td colspan=""6"">No Jump results Found For selected skier.</td></tr>"
                    End If 'end of has rows
                End Using

            Catch ex As Exception
                sMsg += "Error Can 't retrieve Jump Scores. "
                sErrDetails = ex.Message & " " & ex.StackTrace & "error at RecapJump:SQL= " & sSQL
            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function RecapOverall(ByVal SanctionID As String, ByVal MemberID As String, ByVal AgeGroup As String, ByVal SkierName As String) As String
        'Pulled from wfwShowScoreRecap.php
        ' 
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sText As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID
        Dim sAgeGroup As String = AgeGroup
        Dim sSkierName As String = SkierName
        Replace(sSkierName, "'", "")
        Replace(sSkierName, ",", "")
        Dim sSkierRound As String = ""
        Dim sSkierEvent As String = ""
        Dim sSQL As String = ""
        Dim sRound As Int16 = 0
        Dim sTmpRound As Int16 = 0
        Dim sEvent As String = ""
        Dim sOverallScore As String = ""
        Dim sSlalomNopsScore As String = ""
        Dim sTrickNopsScore As String = ""
        Dim sJumpNopsScore As String = ""
        Dim sSlalomScore As String = ""
        Dim sFinalPassScore As String = ""
        Dim sFinalSpeedMPH As String = ""
        Dim sFinalspeedKPH As String = ""
        Dim sFinalLen As String = ""
        Dim sFinalLenOff As String = ""
        Dim sTrickScore As String = ""
        Dim sScorePass1 As String = ""
        Dim sScorePass2 As String = ""
        Dim sScoreFeet As String = ""
        Dim sScoreMeters As String = ""

        ' 3 event scores in available data.
        ' 23S108, James Bryans,             OM, 000107150,  at least 1 round overall
        ' 23S108, Tristan Duplan-Fribourg, JM, 000181068, at least 1 round overall

        'NOTE:  IS AGEGROUP REQUIRED?  MUST ALL PERFORMANCES BE IN THE SAME AGE GROUP?
        'Can modification of this be used to put overall in with other events by division?

        sSQL = "SELECT TR.SkierName, TR.MemberId, TR.AgeGroup, 'Overall' as Event "
        sSQL += ", COALESCE(SS.NopsScore,0) + COALESCE(TS.NopsScore,0) + COALESCE(JS.NopsScore,0) as OverallScore "
        sSQL += ", SS.NopsScore as SlalomNopsScore, TS.NopsScore as TrickNopsScore, JS.NopsScore as JumpNopsScore "
        sSQL += ", COALESCE(SS.Round,COALESCE(TS.Round,COALESCE(JS.Round,0))) as Round "
        sSQL += ", SS.Score as SlalomScore, FinalPassScore, FinalSpeedMph, FinalSpeedKph, FinalLen, FinalLenOff "
        sSQL += " , TS.Score as TrickScore, ScorePass1, ScorePass2 "
        sSQL += ", JS.ScoreFeet, JS.ScoreMeters  "
        sSQL += "From waterskiProd23.dbo.TourReg TR 	"
        sSQL += "Left OUTER JOIN waterskiProd23.dbo.SlalomScore SS on SS.MemberId=TR.MemberId And SS.SanctionId=TR.SanctionId And SS.AgeGroup = TR.AgeGroup "
        sSQL += "Left OUTER JOIN waterskiProd23.dbo.TrickScore TS on TS.MemberId=TR.MemberId And TS.SanctionId=TR.SanctionId And TS.AgeGroup = TR.AgeGroup And TS.Round = SS.Round "
        sSQL += " Left OUTER JOIN waterskiProd23.dbo.JumpScore JS on JS.MemberId=TR.MemberId And JS.SanctionId=TR.SanctionId And JS.AgeGroup = TR.AgeGroup And JS.Round = SS.Round "
        sSQL += "WHERE TR.SanctionID ='" & sSanctionID & "' AND TR.MemberId='" & sMemberID & "' And TR.AgeGroup='" & sAgeGroup & "'  "
        sSQL += " And COALESCE(SS.Round, COALESCE(TS.Round, COALESCE(JS.Round, 0))) > 0 "
        sSQL += " Order By TR.AgeGroup, TR.SkierName, OverallScore DESC "

        Dim sConn As String = ""
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: RecapOverall could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        sText = "<Table class=""table table-striped border-1 "">"
        sText += "<thead><tr><td colspan=""6""><b>Overall Recap</b></td></tr>"
        sText += "<tr><th>Age Group</th><th>Round</th><th>OverallScore</th><th>SlalomNopsScore</th><th>TrickNopsScore</th><th>JumpNopsScore</th>"
        sText += "<th>SlalomScore</th><th>FinalPassScore</th><th>FinalLen</th><th>FinalLenOff</th><th>TrickScore</th>"
        sText += "<th>ScorePass1</th><th>ScorePass2</th><th>ScoreFeet</th><th>ScoreMeters</th></tr></thead>"
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    cmdRead.CommandText = sSQL
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()
                            If IsDBNull(MyDataReader.Item("Round")) Then
                                sRound = "N/A"
                            Else
                                sRound = CStr(MyDataReader.Item("Round"))
                            End If
                            If IsDBNull(MyDataReader.Item("Event")) Then
                                sEvent = "N/A"
                            Else
                                sEvent = CStr(MyDataReader.Item("Event"))
                            End If
                            If IsDBNull(MyDataReader.Item("OverallScore")) Then
                                sEvent = "N/A"
                            Else
                                sOverallScore = CStr(MyDataReader.Item("OverallScore"))
                            End If
                            If IsDBNull(MyDataReader.Item("SlalomNopsScore")) Then
                                sSlalomNopsScore = "N/A"
                            Else
                                sSlalomNopsScore = CStr(MyDataReader.Item("SlalomNopsScore"))
                            End If
                            If IsDBNull(MyDataReader.Item("TrickNopsScore")) Then
                                sTrickNopsScore = "N/A"
                            Else
                                sTrickNopsScore = CStr(MyDataReader.Item("TrickNopsScore"))
                            End If
                            If IsDBNull(MyDataReader.Item("JumpNopsScore")) Then
                                sJumpNopsScore = "N/A"
                            Else
                                sJumpNopsScore = CStr(MyDataReader.Item("JumpNopsScore"))
                            End If
                            If IsDBNull(MyDataReader.Item("SlalomScore")) Then
                                sSlalomScore = "N/A"
                            Else
                                sSlalomScore = CStr(MyDataReader.Item("SlalomScore"))
                            End If
                            If IsDBNull(MyDataReader.Item("FinalPassScore")) Then
                                sFinalPassScore = "N/A"
                            Else
                                sFinalPassScore = CStr(MyDataReader.Item("FinalPassScore"))
                            End If
                            If IsDBNull(MyDataReader.Item("FinalSpeedMPH")) Then
                                sFinalSpeedMPH = "N/A"
                            Else
                                sFinalSpeedMPH = CStr(MyDataReader.Item("FinalSpeedMPH"))
                            End If
                            If IsDBNull(MyDataReader.Item("FinalSpeedKPH")) Then
                                sFinalspeedKPH = "N/A"
                            Else
                                sFinalspeedKPH = CStr(MyDataReader.Item("FinalspeedKPH"))
                            End If
                            If IsDBNull(MyDataReader.Item("FinalLen")) Then
                                sFinalLen = "N/A"
                            Else
                                sFinalLen = CStr(MyDataReader.Item("FinalLen"))
                            End If
                            If IsDBNull(MyDataReader.Item("FinalLenOff")) Then
                                sFinalLenOff = "N/A"
                            Else
                                sFinalLenOff = CStr(MyDataReader.Item("FinalLenOff"))
                            End If
                            If IsDBNull(MyDataReader.Item("TrickScore")) Then
                                sTrickScore = "N/A"
                            Else
                                sTrickScore = CStr(MyDataReader.Item("TrickScore"))
                            End If
                            If IsDBNull(MyDataReader.Item("ScorePass1")) Then
                                sScorePass1 = "N/A"
                            Else
                                sScorePass1 = CStr(MyDataReader.Item("ScorePass1"))
                            End If
                            If IsDBNull(MyDataReader.Item("ScorePass2")) Then
                                sScorePass2 = "N/A"
                            Else
                                sScorePass2 = CStr(MyDataReader.Item("ScorePass2"))
                            End If
                            If IsDBNull(MyDataReader.Item("ScoreFeet")) Then
                                sScoreFeet = "N/A"
                            Else
                                sScoreFeet = CStr(MyDataReader.Item("ScoreFeet"))
                            End If
                            If IsDBNull(MyDataReader.Item("ScoreMeters")) Then
                                sScoreMeters = "N/A"
                            Else
                                sScoreMeters = CStr(MyDataReader.Item("ScoreMeters"))
                            End If

                            sText += "<tr><td>" & sAgeGroup & "</td><td>" & sRound & "</td><td>" & sOverallScore & "</td><td>" & sSlalomNopsScore & "</td><td>" & sTrickNopsScore & "</td><td>" & sJumpNopsScore & "</td>"
                            sText += "<td>" & sSlalomScore & "</td><td>" & sFinalPassScore & "</td><td>" & sFinalLen & "</td><td>" & sFinalLenOff & "</td><td>" & sTrickScore & "</td>"
                            sText += "<td>" & sScorePass1 & "</td><td>" & sScorePass2 & "</td><td>" & sScoreFeet & "</td><td>" & sScoreMeters & "</td></tr>"

                        Loop
                    Else
                        sText += "<tr><td colspan=""15"">No Overall results Found For selected skier.</td></tr>"
                    End If 'end of has rows
                End Using

            Catch ex As Exception
                sMsg += "Error Can 't retrieve Overall Scores. " 'SQL= " & SQL & "<br>IndivJumpResults Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"
                sErrDetails = ex.Message & " " & ex.StackTrace & "<br>error at RecapOverall:  SQL= " & sSQL
            Finally
                sText += "<tr><td colspan=""15""><b>Close browser window when done</b></td></tr></table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function LoadDvList(ByVal sanctionID As String, ByVal EventCode As String, ByRef DDL_Division As DropDownList) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sAgeGroup As String = ""
        Dim sSanctionID As String = sanctionID
        Dim sEventCode As String = EventCode
        Dim sEVWhere As String = ""
        Select Case sEventCode
            Case "A"
                sEVWhere = ""
            Case "S"
                sEVWhere = " and Event = 'Slalom' "
            Case "T"
                sEVWhere = " and Event = 'Trick' "
            Case "J"
                sEVWhere = " and Event = 'Jump' "
            Case Else
        End Select
        Dim sSQL As String = "Select distinct AgeGroup from EventReg where SanctionID = '" & sSanctionID & "' " & sEVWhere
        Dim sConn As String = ""
        Try
            sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString

        Catch ex As Exception
            sMsg = "Error: LoadDVList could not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                With DDL_Division
                    .Items.Clear()
                    .Items.Add(New ListItem("ALL", "A"))
                    Using cmdRead
                        cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                        cmdRead.CommandText = sSQL
                        cmdRead.Connection.Open()
                        MyDataReader = cmdRead.ExecuteReader
                        If MyDataReader.HasRows = True Then
                            Do While MyDataReader.Read()
                                sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                                .Items.Add(New ListItem(sAgeGroup, sAgeGroup))
                            Loop
                        Else
                            sMsg = " No Entries Found for selected tournament. "
                            .Items.Clear()
                        End If 'end of has rows
                    End Using
                End With
            Catch ex As Exception
                sMsg += "Error: Can't retrieve Divisions Entered. "
                sErrDetails = "'SQL= " & sSQL & "<br>LoadDvList Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"
                DDL_Division.Items.Clear()
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
        End If
        Return "Success"
    End Function
End Module
