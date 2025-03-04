Imports System.Security.Policy

Module ModDataAccessPro
    Friend Function LoadProEventList(ByVal sanctionID As String, ByRef DDL_Event_Div As DropDownList) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sAgeGroup As String = ""
        Dim sEvent As String = ""
        Dim sSanctionID As String = sanctionID
        Dim sPREventCode As String = ""
        Dim sSQL As String = ""
        Dim sValue As String = ""
        sSQL = "select distinct agegroup, Event from LiveWebScoreboard.dbo.EventReg where sanctionid = ? order by event, Agegroup"


        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: LoadDVList could not get connection string."
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



        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                With DDL_Event_Div
                    .Items.Clear()
                    .Items.Add(New ListItem("Please Select an Event/Division", "0"))
                    Using cmdRead

                        cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                        cmdRead.Connection.Open()
                        MyDataReader = cmdRead.ExecuteReader
                        If MyDataReader.HasRows = True Then
                            Do While MyDataReader.Read()
                                sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                                sEvent = CStr(MyDataReader.Item("Event"))
                                sValue = Left(sEvent, 1) & sAgeGroup
                                .Items.Add(New ListItem(sAgeGroup & " " & sEvent, sValue))

                            Loop
                        Else
                            sMsg = " No Entries Found for selected tournament. "
                            .Items.Clear()
                            .Items.Add(New ListItem("None Found", "0"))
                        End If 'end of has rows
                    End Using
                End With
            Catch ex As Exception
                sMsg += "Error: Can't retrieve Divisions Entered. "
                sErrDetails = " LoadDvList Caught: " & ex.Message & " " & ex.StackTrace & "<br>SQL= " & sSQL
                DDL_Event_Div.Items.Clear()
            End Try
        End Using

        If Len(sMsg) > 2 Then
            Return sMsg
        End If
        Return "Success"
    End Function
    Friend Sub LoadProRndDDL(ByVal EventPkd As String, ByVal SRnds As String, ByVal TRnds As String, ByVal JRnds As String, ByRef ddl_PkRnd As DropDownList)
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sEventCode As String = EventPkd
        'LIMIT ROUNDS BASED ON ROUNDS OFFERED IN SELECTED EVENT.  If All events is selected max rounds for event with most rounds is used.
        Dim i As Integer = 0
        Dim sSlalomRnds As String = SRnds 'should be 0 if not offered
        Dim sTrickRnds As String = TRnds
        Dim sJumpRnds As String = JRnds
        Dim sMaxRounds = sSlalomRnds
        If sTrickRnds > sMaxRounds Then sMaxRounds = sTrickRnds
        If sJumpRnds > sMaxRounds Then sMaxRounds = sJumpRnds
        Dim sMinRounds = sSlalomRnds
        If sTrickRnds > 0 And sTrickRnds < sMinRounds Then sMinRounds = sTrickRnds
        If sJumpRnds > 0 And sJumpRnds < sMinRounds Then sMinRounds = sJumpRnds
        With ddl_PkRnd
            .Items.Clear()
            .Items.Add(New ListItem("Current Rnd", 0))
            Select Case sEventCode
'                Case "A" 'have to use highest number of rounds offered in any event.  Will result in empty spaces if all events do not have the same number of rounds
'                    For i = 1 To sMaxRounds
'                        .Items.Add(New ListItem("Rnd " & i, i))
'                    Next
                Case "S"
                    .Items.Add(New ListItem("Current Rnd", 0))
                    For i = 1 To sSlalomRnds
                        .Items.Add(New ListItem("Rnd " & i, i))
                    Next
                Case "T"
                    .Items.Add(New ListItem("Current Rnd", 0))
                    For i = 1 To sTrickRnds
                        .Items.Add(New ListItem("Rnd " & i, i))
                    Next
                Case "J"
                    .Items.Add(New ListItem("Current Rnd", 0))
                    For i = 1 To sJumpRnds
                        .Items.Add(New ListItem("Rnd " & i, i))
                    Next
                Case "O"  'Use least number of rounds offered in any of the 3 events
                    For i = 1 To sMinRounds
                        .Items.Add(New ListItem("Rnd " & i, i))
                    Next
            End Select
        End With
    End Sub
    Friend Function LoadOnWaterSlalom(ByVal SanctionID As String) As String
        'AJAX partial page refresh - Automatic refresh based on timer set to 1.75 minutes (est slalom pass at 2 min.)
        'Allow user to set refresh timer from 1.75 to 3 minutes.
        Dim sText As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sSkierName As String = ""
        Dim sDV As String = ""
        Dim sEventClass As String = ""
        Dim sMemberID As String = ""
        Dim sTmpMemberID As String = ""
        Dim sTmpRound As String = ""
        Dim sRound As String = ""
        Dim sPass As String = ""
        Dim sScore As String = ""
        Dim sBoatTime As String = ""
        Dim sNote As String = ""
        Dim sReride As String = "N"
        Dim sRerideReason As String = ""
        Dim sProtectedScore As String = ""
        Dim sBackColor As String = ""
        '        Dim sSQL As String = "Select top 5 SR.LastUpdateDate, SR.*, TR.SkierName "
        '        sSQL += " from SlalomRecap SR left join TourReg TR ON SR.MemberID = TR.MemberID where sanctionID = '" & sSanctionID & "'"
        '        sSQL += " order by sr.lastupdatedate desc "
        Dim sSQL As String = "SELECT top 10 SR.MemberID, SR.LastUpdateDate, SR.SanctionID, TR.SkierName,  SR.AgeGroup, "
        sSQL += " SR.[round], SR.SkierRunNum As Pass, SR.Score, SR.Note, SR.Reride, SR.RerideReason, SR.ProtectedScore "
        sSQL += " From LiveWebScoreboard.dbo.SlalomRecap SR "
        sSQL += " Left Join (Select distinct SkierName, SanctionID, MemberID from LiveWebScoreboard.dbo.TourReg where sanctionID = '" & sSanctionID & "') As TR "
        sSQL += " On TR.sanctionID = SR.SanctionID And SR.MemberID = TR.MemberID "
        sSQL += " Where SR.SanctionId = '" & sSanctionID & "' "
        '       sSQL += " and LastUpdateDate > DateAdd(Minute, 10, GetDate())"
        sSQL += " order by SR.LastUpdateDate desc, SR.MemberID  "

        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: OnWaterSlalom could not get connection string."
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try

        'Get the data - loop through and build the display using <div>s
        'If reride - put reride details on indented second line.
        'Loop until memberID changes.
        'Can computer sending new information be identified.  
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        sText = "    <div Class=""container"">"
        sText += "<div Class=""row"">"
        sText += "    <div Class=""col-12 bg-primary text-white text-center"">"
        sText += "Most Recent <span class=""bg-danger text-white"">UNOFFICIAL</span> SLALOM performance details."
        sText += "   </div>"
        sText += " </div>"
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
                            sSkierName = Replace(sSkierName, ",", ",,")
                            sSkierName = Replace(sSkierName, "'", "''")
                            sMemberID = CStr(MyDataReader.Item("MemberID"))
                            sDV = CStr(MyDataReader.Item("AgeGroup"))
                            sScore = CStr(MyDataReader.Item("Score"))
                            '    sScoreProtected = CStr(MyDataReader.Item("EventClass"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sPass = CStr(MyDataReader.Item("Pass"))
                            sNote = CStr(MyDataReader.Item("Note"))
                            sReride = CStr(MyDataReader.Item("Reride"))
                            sBackColor = ""
                            If IsDBNull(MyDataReader.Item("ProtectedScore")) Then
                                sProtectedScore = "N/A"
                            Else
                                sProtectedScore = CStr(MyDataReader.Item("ProtectedScore"))
                            End If
                            If sReride = "Y" Then
                                If sProtectedScore = "0.0" Then
                                    sBackColor = "bg-danger text-dark"
                                Else
                                    sBackColor = "bg-warning text-dark"
                                End If
                            End If
                            If IsDBNull(MyDataReader.Item("RerideReason")) Then
                                sRerideReason = "N/A"
                            Else
                                sRerideReason = CStr(MyDataReader.Item("RerideReason"))
                            End If

                            'Have data - create display
                            If sTmpMemberID = "" Then
                                sTmpMemberID = sMemberID
                                sTmpRound = sRound
                                sText += "<div Class=""row"">"
                                sText += "    <div Class=""col-12 bg-info text-black text-center"">"
                                sText += " " & sSkierName & " &nbsp; " & sDV & " &nbsp; Round: " & sRound & " "
                                sText += "   </div>"
                                sText += " </div>"
                            End If

                            If sTmpMemberID = sMemberID And sTmpRound = sRound Then
                                sText += "<div Class=""row"">"
                                sText += "<div Class=""col-1 " & sBackColor & """>"
                                sText += "Pass:&nbsp;" & sPass & "&nbsp;"
                                sText += "</div>"
                                sText += "<div Class=""col-2"">"
                                sText += "&nbsp;" & sScore & "&nbsp;buoys"
                                sText += "</div>"
                                sText += "<div Class=""col-9"">"
                                sText += sNote
                                sText += "</div>"
                                sText += "        </div>"
                            Else
                                'Original skier performances listed. Don't display next skier
                                Exit Do
                            End If
                            If sReride = "Y" Then
                                sText += "            <div Class=""row"">"
                                sText += "        <div Class=""col-12 " & sBackColor & " text-center "">"
                                sText += "Pass:&nbsp;" & sPass & "&nbsp;Protected Score: " & sProtectedScore & " &nbsp; Reride Reason: " & sRerideReason
                                sText += "        </div>"
                                sText += "        </div>"
                            End If
                        Loop
                    Else
                        '                       sText += "<div Class=""row"">"
                        '                       sText += "    <div Class=""col-12 text-center"">"
                        '                       sText += " No Recent Slalom Scores "
                        '                       sText += "   </div>"
                        '                       sText += " </div>"
                        sMsg = "No Recent Slalom Scores<br>"

                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "<div Class=""row"">"
                sMsg += "    <div Class=""col-12 text-center"">"
                sMsg += "<b> Error at LoadOnWaterSlalom </b>"
                sErrDetails = ex.Message & " " & ex.StackTrace & " <br>SQL= " & sSQL & "</br>" & sText
                ' IF DEBUG IS ON
                '   sMsg += "<br />" & sErrDetails
                'End If
                sMsg += "   </div>"
                sMsg += " </div>"

            Finally
                sText += "</div>"  'end of container
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        'sText += "</div>"  'End of container - if Finally is hit on successful record don't need this
        Return sText
    End Function
    Friend Function LoadOnWaterTrick(ByVal SanctionID As String) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim SQL As String = ""
        SQL = "SELECT top 4 MemberID, Insertdate as time, SanctionID, SkierName, AgeGroup,EventClass, EventScore, Round, EventScoreDesc from LiveWebScoreboard.dbo.vTrickResults "
        SQL += " Where SanctionID ='" & sSanctionID & "' "
        '        SQL += " and LastUpdateDate > DateAdd(Minute, 5, GetDate())"
        SQL += "  ORDER BY MemberID, insertdate desc"

        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sMemberID As String = ""
        Dim sTmpMemberID As String = ""
        Dim sTmpRound As String = ""
        Dim sPass1URL As String = ""
        Dim sPass2URL As String = ""
        Dim sEvent As String = ""
        Dim sEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sPassNum As String = "0"
        Dim sConn As String = ""
        Dim sSkierName As String = ""
        Dim sAgeGroup As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: IndivTrickResults could not get connection string."
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sTableWidth As String = "100%"
        Dim sText As String = ""
        Dim sTSB As New StringBuilder

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
                            sMemberID = CStr(MyDataReader.Item("MemberID"))
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sEventScore = CStr(MyDataReader.Item("EventScore"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                            If sTmpMemberID = "" Then
                                sTmpMemberID = sMemberID
                                sTmpRound = sRound
                                sTSB.Append("<Table class=""table"">")
                                sTSB.Append("<thead><tr class=""bg-primary text-white""><td colspan=""5"">MOST RECENT <span class=""bg-danger text-white"">UNOFFICIAL</span> Trick Performance</td></tr>")
                            End If
                            If sTmpMemberID = sMemberID And sTmpRound = sRound Then
                                sTSB.Append("<tr class=""bg-light text-primary""><th widtn=""25%"">" & sSkierName & "</th><th widtn=""15%"">DV: " & sAgeGroup & "</th><th widtn=""15%"">Rnd: " & sRound & "</th><th>" & sEventScoreDesc & "</th></tr></thead>")
                                '                           sTSB.Append("<tr><td><td>&nbsp;</td>" & sEventScore & "&nbsp;</td></tr>")
                            Else
                                Exit Do
                            End If
                        Loop
                    Else
                        '                        sTSB.Append("<thead><tr Class=""table-primary bg-primary""><td colspan=""5"">Most Recent <span class=""bg-danger text-white"">UNOFFICIAL</span> Performance Details</td></tr>")
                        '                        sTSB.Append("<tr><td colspan=""5"">No Recent Trick Scores</td></tr>")
                        sMsg = "No Recent Trick Scores<br>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: Can't retrieve Trick Scores"
                sErrDetails = ex.Message & " " & ex.StackTrace & "<br>SQL = " & SQL
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        sText = sTSB.ToString() & "</table>"
        Return sText

    End Function
    Friend Function LoadOnWaterJump(ByVal SanctionID As String) As String
        Dim myStringBuilder As New StringBuilder("")
        Dim sText As String = ""
        Dim SQLsb As New StringBuilder("")
        Dim sSQL As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sSkierName As String = ""
        Dim sDV As String = ""
        Dim sEventClass As String = ""
        Dim sMemberID As String = ""
        Dim sTmpMemberID As String = ""
        Dim sTmpRound As String = ""
        Dim sRound As String = ""
        Dim sPass As String = ""
        Dim sResults As String = ""
        Dim sScoreFeet As String = ""
        Dim sScoreMeters As String = ""
        Dim sNote As String = ""
        Dim sReride As String = ""
        Dim sRerideReason As String = ""
        Dim sProtectedScore As String = ""
        Dim sBackColor As String = ""


        SQLsb.Append("SELECT top 10 JR.MemberID, JR.LastUpdateDate, JR.SanctionID, TR.SkierName, JR.AgeGroup, ")
        SQLsb.Append(" JR.[round], JR.PassNum As Pass, JR.Results, JR.ScoreFeet, JR.ScoreMeters, JR.Note, JR.Reride, JR.RerideReason, JR.ScoreProt ")
        SQLsb.Append(", JR.RerideIfBest, JR.RerideCanImprove ")
        SQLsb.Append(" From LiveWebScoreboard.dbo.JumpRecap JR ")
        SQLsb.Append(" Left Join (Select distinct SkierName, SanctionID, MemberID from LiveWebScoreboard.dbo.TourReg where sanctionID = '" & sSanctionID & "') ")
        SQLsb.Append(" as TR On JR.sanctionID = TR.SanctionID And JR.MemberID = TR.MemberID ")
        SQLsb.Append(" Where TR.SanctionId = '" & sSanctionID & "' ") ' following has reride  and [round] <> 25 and TR.MemberID = '200149011' "
        '       SQLsb.Append(" and LastUpdateDate > DateAdd(Minute, 10, GetDate())")
        SQLsb.Append(" order by JR.MemberID, LastUpdateDate desc ")
        sSQL = SQLsb.ToString

        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: OnWaterJump could not get connection string. " &
                sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try

        'Get the data - loop through and build the display using <div>s
        'If reride - put reride details on  second line.
        'Loop until memberID changes.
        'Can computer sending new information be identified.  
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        myStringBuilder.Append(" <div Class=""container"">")
        myStringBuilder.Append(" <div Class=""row"">")
        myStringBuilder.Append(" <div Class=""col-12 bg-primary text-white text-center"">")
        myStringBuilder.Append("Most Recent <span class=""bg-danger text-white"">UNOFFICIAL</span> JUMP performance details.")
        myStringBuilder.Append("   </div>")
        myStringBuilder.Append(" </div>")
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
                            sSkierName = Replace(sSkierName, ", ", ",,")
                            sSkierName = Replace(sSkierName, "'", "''")
                            sMemberID = CStr(MyDataReader.Item("MemberID"))
                            sDV = CStr(MyDataReader.Item("AgeGroup"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sPass = CStr(MyDataReader.Item("Pass"))
                            sResults = MyDataReader.Item("Results")
                            If IsDBNull(MyDataReader.Item("ScoreFeet")) Then
                                sScoreFeet = "0"
                            Else
                                sScoreFeet = CStr(MyDataReader.Item("ScoreFeet"))
                            End If
                            If IsDBNull(MyDataReader.Item("ScoreMeters")) Then
                                sScoreMeters = "0"
                            Else
                                sScoreMeters = CStr(MyDataReader.Item("ScoreMeters"))
                            End If
                            If IsDBNull(MyDataReader.Item("Note")) Then
                                sNote = ""
                            Else
                                sNote = CStr(MyDataReader.Item("Note"))
                            End If

                            sReride = CStr(MyDataReader.Item("Reride"))
                            sBackColor = ""
                            If IsDBNull(MyDataReader.Item("ScoreProt")) Then
                                sProtectedScore = "N/A"
                            Else
                                sProtectedScore = CStr(MyDataReader.Item("ScoreProt"))
                            End If
                            If sReride = "Y" Then
                                If sProtectedScore = "0.0" Then
                                    sBackColor = "bg-danger text-dark"
                                Else
                                    sBackColor = "bg-warning text-dark"
                                End If
                            End If
                            If IsDBNull(MyDataReader.Item("RerideReason")) Then
                                sRerideReason = "N/A"
                            Else
                                sRerideReason = CStr(MyDataReader.Item("RerideReason"))
                            End If
                            'Have data
                            If sTmpMemberID = "" Then  'first record - display name
                                sTmpRound = sRound
                                sTmpMemberID = sMemberID
                                myStringBuilder.Append("<div Class=""row"">")
                                myStringBuilder.Append("<div Class=""col-12 bg-info text-black text-center"">")
                                myStringBuilder.Append(sSkierName & " &nbsp; " & sDV & " &nbsp; Round: " & sRound & " ")
                                myStringBuilder.Append("   </div>")
                                myStringBuilder.Append(" </div>")
                            End If
                            If sTmpMemberID = sMemberID And sTmpRound = sRound Then
                                myStringBuilder.Append("<div Class=""row"">")
                                myStringBuilder.Append("<div Class=""col-1 " & sBackColor & """>")
                                myStringBuilder.Append("Pass:&nbsp;" & sPass & "&nbsp;")
                                myStringBuilder.Append("</div>")
                                myStringBuilder.Append("<div Class=""col-2"">")
                                myStringBuilder.Append("&nbsp;" & sResults & "&nbsp;")
                                myStringBuilder.Append("</div>")
                                myStringBuilder.Append("<div Class=""col-2"">")
                                myStringBuilder.Append("&nbsp;" & sScoreFeet & "F&nbsp;" & sScoreMeters & "M")
                                myStringBuilder.Append("</div>")
                                myStringBuilder.Append("<div Class=""col-7"">")
                                myStringBuilder.Append(sNote)
                                myStringBuilder.Append("</div>")
                                myStringBuilder.Append(" </div>")
                            Else  'New skier don't display
                                Exit Do
                            End If
                            If sReride = "Y" Then
                                myStringBuilder.Append(" <div Class=""row"">")
                                myStringBuilder.Append("<div Class=""col-12 " & sBackColor & " text-center "">")
                                myStringBuilder.Append("Pass:&nbsp;" & sPass & "&nbsp;Protected Score: " & sProtectedScore & " &nbsp; Reride Reason: " & sRerideReason)
                                myStringBuilder.Append(" </div>")
                                myStringBuilder.Append(" </div>")
                            End If
                        Loop
                    Else
                        '                        myStringBuilder.Append("<div Class=""row"">")
                        '                        myStringBuilder.Append("    <div Class=""col-12 text-center"">")
                        '                        myStringBuilder.Append(" No Recent Jump Scores ")
                        '                        myStringBuilder.Append("   </div>")
                        '                        myStringBuilder.Append(" </div>")
                        sMsg = "No Recent Jump Scores<br>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "<div Class=""row"">"
                sMsg += "    <div Class=""col-12 text-center"">"
                sMsg += "<b> Error at LoadOnWaterSlalom </b>"
                sErrDetails = "</br>" & ex.Message & " " & ex.StackTrace & " <br>SQL= " & sSQL & "</br>" & sText
                'IF DEBUG IS ON THEN
                '      sMsg += sErrDetails
                '      End if
                sMsg += "   </div>"
                sMsg += " </div>"
            Finally
                myStringBuilder.Append("</div>")  'end of container
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        sText = myStringBuilder.ToString
        Return sText
    End Function
    Public Function GetRunOrdercountPro(ByVal SanctionID As String, ByVal SlalomRound As String, ByVal TrickRounds As String, ByVal JumpRnds As String) As Array
        'Check for multiple running orders by event
        'Return an array that provides the events with multiple running orders.
        'For each event - Use that info to display either the single running order for all rounds or a separate running order for each round
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sSlalomRnds As String = SlalomRound
        Dim sTrickRnds As String = TrickRounds
        Dim sJumpRnds As String = JumpRnds
        Dim sRnd As String = ""
        Dim sEventGrp As String = ""
        Dim sReturnArray(0 To 4)
        Dim sSQLS As String = ""
        Dim sSQLT As String = ""
        Dim sSQLJ As String = ""
        Dim sMultiS As String = "0"
        Dim sMultiT As String = "0"
        Dim sMultiJ As String = "0"

        If sSlalomRnds > 0 Then
            '            sSQLS = "Select distinct EventGroup, [round] From livewebscoreboard.dbo.EventRunOrder Where SanctionId = '" & sSanctionID & "' AND Event = 'Slalom' "
            sSQLS = " Select distinct ERO.EventGroup, ERO.[round], Upper(TP.PropValue) From livewebscoreboard.dbo.EventRunOrder ERO "
            sSQLS += " Left Join livewebscoreboard.dbo.TourProperties TP On ERO.SanctionID = TP.SanctionID "
            sSQLS += " where ERO.SanctionID = '" & sSanctionID & "' and ERO.Event = 'Slalom' and TP.PropKey = 'SlalomSummaryDataType' "
        End If
        If sTrickRnds > 0 Then
            sSQLT = " Select distinct ERO.EventGroup, ERO.[round], Upper(TP.PropValue) From livewebscoreboard.dbo.EventRunOrder ERO "
            sSQLT += " Left Join livewebscoreboard.dbo.TourProperties TP On ERO.SanctionID = TP.SanctionID "
            sSQLT += " where ERO.SanctionID = '" & sSanctionID & "' and ERO.Event = 'Trick' and TP.PropKey = 'TrickSummaryDataType' "

        End If
        If sJumpRnds > 0 Then
            sSQLJ = " Select distinct ERO.EventGroup, ERO.[round], Upper(TP.PropValue) From livewebscoreboard.dbo.EventRunOrder ERO "
            sSQLJ += " Left Join livewebscoreboard.dbo.TourProperties TP On ERO.SanctionID = TP.SanctionID "
            sSQLJ += " where ERO.SanctionID = '" & sSanctionID & "' and ERO.Event = 'Jump' and TP.PropKey = 'JumpSummaryDataType' "
        End If

        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: GetReportList could not get connection string."
            sErrDetails = ex.Message & "  " & ex.StackTrace
            sReturnArray(0) = sMsg
            Return sReturnArray
            Exit Function
        End Try

        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        cmdRead.CommandType = CommandType.Text
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try

                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    If Len(sSQLS) > 2 Then
                        cmdRead.CommandText = sSQLS
                        MyDataReader = cmdRead.ExecuteReader
                        If MyDataReader.HasRows = True Then
                            sMultiS = "1"
                        End If
                        MyDataReader.Close()
                    End If

                    If Len(sSQLT) > 2 Then
                        cmdRead.CommandText = sSQLT
                        MyDataReader = cmdRead.ExecuteReader
                        If MyDataReader.HasRows = True Then
                            sMultiT = "1"
                        End If
                        MyDataReader.Close()
                    End If

                    If Len(sSQLJ) > 2 Then
                        cmdRead.CommandText = sSQLJ
                        MyDataReader = cmdRead.ExecuteReader
                        If MyDataReader.HasRows = True Then
                            sMultiJ = "1"
                        End If
                        MyDataReader.Close()
                    End If

                End Using

            Catch ex As Exception
                sMsg = "Error at GetBestRndXEvDv"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
                sReturnArray(0) = sMsg
            End Try
        End Using
        sReturnArray(1) = sMultiS
        sReturnArray(2) = sMultiT
        sReturnArray(3) = sMultiJ
        Return sReturnArray
    End Function
    Friend Function GetPlcmtFormat(ByVal SanctionID As String, EventPkd As String) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sEvent As String = EventPkd
        Dim sPlcmntFormat As String = ""

        Dim sSQL As String = ""
        sSQL = "Select * From LiveWebScoreboard.dbo.TourProperties Where sanctionID = '" & sSanctionID & "' And PropKey = '" & sEvent & "SummaryDataType'"
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

        cmdRead.CommandType = CommandType.Text
        cmdRead.CommandText = sSQL



        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        MyDataReader.Read()

                        If Not IsDBNull(MyDataReader.Item("PropValue")) Then
                            sPlcmntFormat = MyDataReader.Item("PropValue")
                        Else
                            sPlcmntFormat = "BEST"
                        End If

                    Else 'No data
                        sPlcmntFormat = "BEST"
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetPlcmntFormat"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sPlcmntFormat

    End Function
    Friend Function GetRunOrderPro(ByVal SanctionID As String, ByVal YrPkd As String, ByVal TournName As String, ByVal EventCode As String, ByVal DivisionCodePkd As String, ByVal RndsPkd As String, sSlalomrounds As String, ByVal TrickRounds As String, ByVal JumpRounds As String, ByVal UseNops As Int16, ByVal UseTeams As Int16, ByVal FormatCode As String, ByVal DisplayMetric As Int16) As String

        'adapted from moddataaccess3.getRunOrdHoriz.
        'need to change to sql which gets correct running order by round and division
        'display skier name with link to recap, State, Federation, Team



        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sYrPkd As String = YrPkd
        Dim sTName As String = TournName
        Dim sEvent As String = ""
        Dim sEventCode As String = EventCode
        Select Case sEventCode
            Case "S"
                sEvent = "Slalom"
            Case "T"
                sEvent = "Trick"
            Case "J"
                sEvent = "Jump"
        End Select
        Dim sSelRnd As String = RndsPkd
        Dim sSelDv As String = DivisionCodePkd

        Dim sDv As String = ""
        Dim sEventClass As String = ""

        Dim sTmpMemberID As String = ""
        Dim sMemberID As String = ""
        Dim sReturn As String = ""
        Dim sTmpRound As String = ""
        Dim sRound As String = ""
        Dim sFederation As String = ""
        Dim sState As String = ""
        Dim sCity As String = ""
        Dim sSkierName As String = ""
        Dim sSkierLink As String = ""
        Dim sText As New StringBuilder
        Dim sSkiOrderNumber As Int16 = 0
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
        Dim sSql As String = "PrGetRunOrderByEvent"
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
        cmdRead.Parameters("@InEvCode").Value = sEvent
        cmdRead.Parameters("@InEvCode").Direction = ParameterDirection.Input


        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sSelDv   'sDv
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InGroup", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InGroup").Size = 3
        cmdRead.Parameters("@InGroup").Value = "ALL"   'sEventGroup
        cmdRead.Parameters("@InGroup").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InRnd", OleDb.OleDbType.Char)
        cmdRead.Parameters("@InRnd").Size = 1
        cmdRead.Parameters("@InRnd").Value = sSelRnd    '0 = All Rounds    sSelRnd
        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input



        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read




                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            If Not IsDBNull(MyDataReader.Item("MemberID")) Then
                                sMemberID = MyDataReader.Item("MemberID")
                            Else
                                sMemberID = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("City")) Then
                                sCity = CStr(MyDataReader.Item("City"))
                            Else
                                sCity = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("State")) Then
                                sState = CStr(MyDataReader.Item("State"))
                            Else
                                sState = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("Federation")) Then
                                sFederation = CStr(MyDataReader.Item("Federation"))
                            Else
                                sFederation = ""
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

                            'Have data - create display

                            sSkierLink = "<a runat = ""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sDv & "&EV=" & sEventCode & "&TN=" & sTName & ""
                            sSkierLink += "&FC=PRO&FT=0&RP=" & sSelRnd & "&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>"
                            If sTmpMemberID = "" Then
                                sSkiOrderNumber = 1
                                sTmpMemberID = sMemberID
                                sTmpRound = sRound
                                sText.Append("<div>")
                                sText.Append("<b>Running Order Rnd " & sSelRnd & " " & sDv & "</b><br> ")
                                '               sText.Append("<div Class=""row"">")
                                sText.Append(" <b>" & sSkiOrderNumber & ". &nbsp; " & sSkierLink & " &nbsp; " & sFederation & " &nbsp; " & sState & "</b> ")

                            End If

                            If sTmpMemberID <> sMemberID And sTmpRound = sRound Then
                                sSkiOrderNumber += 1
                                sText.Append("<br> <b>" & sSkiOrderNumber & ". &nbsp; " & sSkierLink & " &nbsp; " & sFederation & " &nbsp; " & sState & "</b>")
                                sTmpMemberID = sMemberID
                            End If
                        Loop
                        sText.Append("</div>")
                    Else 'No data
                        sMsg = "No Skiers Found"
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetRunOrderPro"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText.ToString()


    End Function

    Friend Function GetTrickResults(ByVal SanctionID As String, ByVal YrPkd As String, ByVal TournName As String, ByVal EventCode As String, ByVal DivisionCodePkd As String, ByVal RndsPkd As String, sSlalomrounds As String, ByVal TrickRounds As String, ByVal JumpRounds As String, ByVal UseNops As Int16, ByVal UseTeams As Int16, ByVal FormatCode As String, ByVal DisplayMetric As Int16) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sYrPkd As String = YrPkd
        Dim sTName As String = TournName

        Dim sSelRnd As String = RndsPkd
        Dim sSelDv As String = DivisionCodePkd 'division code passed in
        Dim sDv As String = ""  'division in skier record
        Dim sEventClass As String = ""

        Dim sTmpMemberID As String = ""
        Dim sMemberID As String = ""
        Dim sReturn As String = ""
        Dim sTmpRound As String = ""
        Dim sRound As String = ""
        Dim sFederation As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sEventScore As String = ""
        Dim sSkierName As String = ""
        Dim sSkierLink As String = ""
        Dim sText As New StringBuilder
        Dim sTmpEventScore As String = ""
        Dim sTie As String = ""

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

        Dim sSQL As String = "SELECT [SkierName], [SanctionId], [MemberId], [AgeGroup], [Div], [City], [State], [Federation], [Event], "
        sSQL += " [EventClass], [TeamCode],[Round],[EventScore], [EventScoreDesc],[LastUpdateDate] "
        sSQL += " From [LiveWebScoreboard].[dbo].[vTrickResults] "
        sSQL += " Where SanctionID = ? and DiV = ? and Round = ? "
        sSQL += " order by EventScore desc "

        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand

        cmdRead.CommandType = CommandType.Text
        cmdRead.CommandText = sSQL
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input


        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sSelDv   'sDv
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input


        cmdRead.Parameters.Add("@InRnd", OleDb.OleDbType.Char)
        cmdRead.Parameters("@InRnd").Size = 1
        cmdRead.Parameters("@InRnd").Value = sSelRnd    '0 = All Rounds    sSelRnd
        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input



        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read




                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            If Not IsDBNull(MyDataReader.Item("MemberID")) Then
                                sMemberID = MyDataReader.Item("MemberID")
                            Else
                                sMemberID = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                                sEventScore = CStr(MyDataReader.Item("EventScore"))
                            Else
                                sEventScore = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                            Else
                                sEventScoreDesc = ""
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

                            'Have data - create display

                            sSkierLink = "<a runat = ""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sDv & "&EV=T&TN=" & sTName & ""
                            sSkierLink += "&FC=PRO&FT=0&RP=" & sSelRnd & "&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>"
                            If sTmpMemberID = "" Then
                                sTmpMemberID = sMemberID
                                sTmpRound = sRound
                                sText.Append("<div>")
                                sText.Append("<b>Leader Board Rnd " & sSelRnd & " " & sDv & "Class " & sEventClass & "</b><br> ")
                                '               sText.Append("<div Class=""row"">")
                                sText.Append(" <b>" & sSkierLink & " &nbsp; " & sEventScoreDesc & "</b> ")
                                sTmpEventScore = sEventScore
                            End If

                            If sTmpMemberID <> sMemberID Then
                                If sTmpEventScore = sEventScore Then
                                    sTie = "<span Class=""red-text"">T</span>"
                                Else
                                    sTie = ""
                                End If
                                sText.Append("<br> <b>" & sSkierLink & " &nbsp;" & sTie & " " & sEventScoreDesc & "</b>")
                                sTmpMemberID = sMemberID
                            End If
                        Loop
                        sText.Append("</div>")
                    Else 'No data
                        sMsg = "No Skiers Found"
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetTrickResults"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText.ToString()
    End Function
    Friend Function GetJumpResults(ByVal SanctionID As String, ByVal YrPkd As String, ByVal TournName As String, ByVal EventCode As String, ByVal DivisionCodePkd As String, ByVal RndsPkd As String, sSlalomrounds As String, ByVal TrickRounds As String, ByVal JumpRounds As String, ByVal UseNops As Int16, ByVal UseTeams As Int16, ByVal FormatCode As String, ByVal DisplayMetric As Int16) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sYrPkd As String = YrPkd
        Dim sTName As String = TournName

        Dim sSelRnd As String = RndsPkd
        Dim sSelDv As String = DivisionCodePkd 'division code passed in
        Dim sDv As String = ""  'division in skier record
        Dim sEventClass As String = ""

        Dim sTmpMemberID As String = ""
        Dim sMemberID As String = ""
        Dim sReturn As String = ""
        Dim sTmpRound As String = ""
        Dim sRound As String = ""
        Dim sFederation As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sEventScore As String = ""
        Dim sSkierName As String = ""
        Dim sSkierLink As String = ""
        Dim sText As New StringBuilder
        Dim sTmpEventScore As String = ""
        Dim sTie As String = ""

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

        Dim sSQL As String = "SELECT [SkierName], [SanctionId], [MemberId], [AgeGroup], [Div], [City], [State], [Federation], [Event], "
        sSQL += " [EventClass], [TeamCode],[Round],[EventScore], [EventScoreDesc],[LastUpdateDate] "
        sSQL += " From [LiveWebScoreboard].[dbo].[vJumpResults] "
        sSQL += " Where SanctionID = ? and DiV = ? and Round = ? "
        sSQL += " order by EventScore desc "


        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand

        cmdRead.CommandType = CommandType.Text
        cmdRead.CommandText = sSQL
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input


        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sSelDv   'sDv
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input


        cmdRead.Parameters.Add("@InRnd", OleDb.OleDbType.Char)
        cmdRead.Parameters("@InRnd").Size = 1
        cmdRead.Parameters("@InRnd").Value = sSelRnd    '0 = All Rounds    sSelRnd
        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input



        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read

                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            If Not IsDBNull(MyDataReader.Item("MemberID")) Then
                                sMemberID = MyDataReader.Item("MemberID")
                            Else
                                sMemberID = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                                sEventScore = CStr(MyDataReader.Item("EventScore"))
                            Else
                                sEventScore = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                            Else
                                sEventScoreDesc = ""
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

                            'Have data - create display

                            sSkierLink = "<a runat = ""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sDv & "&EV=J&TN=" & sTName & ""
                            sSkierLink += "&FC=PRO&FT=0&RP=" & sSelRnd & "&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>"
                            If sTmpMemberID = "" Then
                                sTmpMemberID = sMemberID
                                sTmpRound = sRound
                                sText.Append("<div>")
                                sText.Append("<b>Leader Board Rnd " & sSelRnd & " " & sDv & "Class " & sEventClass & "</b><br> ")
                                '               sText.Append("<div Class=""row"">")
                                sText.Append(" <b>" & sSkierLink & " &nbsp; " & sEventScoreDesc & "</b> ")
                                sTmpEventScore = sEventScore
                            End If

                            If sTmpMemberID <> sMemberID Then
                                If sTmpEventScore = sEventScore Then
                                    sTie = "<span Class=""red-text"">T</span>"
                                Else
                                    sTie = ""
                                End If
                                sText.Append("<br> <b>" & sSkierLink & " &nbsp;" & sTie & " " & sEventScoreDesc & "</b>")
                                sTmpMemberID = sMemberID
                            End If
                        Loop
                        sText.Append("</div>")
                    Else 'No data
                        sMsg = "No Skiers Found"
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetJumpResults"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText.ToString()
    End Function
    Friend Function GetCurrentRound(ByVal SanctionID As String, ByVal EventPkd As String, ByVal DV As String) As String
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID = SanctionID
        Dim sEventPkd As String = EventPkd
        Dim sDv As String = DV
        Dim sTable2Use As String = ""
        Dim sESFieldName As String = ""
        Dim sRnd As String = ""
        sESFieldName = "EventScore"
        Select Case sEventPkd
            Case "S"
                sTable2Use = "vSlalomResults"
            Case "T"
                sTable2Use = "vTrickResults"
            Case "J"
                sTable2Use = "vJumpResults"
                sESFieldName = "ScoreFeet"
        End Select
        Dim sSQL As String = "Select Top 1 [Round] from " & sTable2Use & " where SanctionID = '" & sSanctionID & "' and Div = '" & sDv & "' and " & sESFieldName & " <> 0 order by [Round] desc"
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: GetReportList could not get connection string."
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try

        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        cmdRead.CommandType = CommandType.Text
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    Using MyDataReader
                        cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                        cmdRead.Connection.Open()
                        cmdRead.CommandText = sSQL
                        MyDataReader = cmdRead.ExecuteReader
                        If MyDataReader.HasRows = True Then
                            MyDataReader.Read()
                            sRnd = MyDataReader.Item("Round")
                        Else
                            sRnd = "0"
                        End If
                    End Using
                End Using
            Catch ex As Exception
                sMsg = "Error at GetBestRndXEvDv"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace

            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sRnd
    End Function
    Friend Function GetSlalomResults(ByVal SanctionID As String, ByVal YrPkd As String, ByVal TournName As String, ByVal EventCode As String, ByVal DivisionCodePkd As String, ByVal RndsPkd As String, sSlalomrounds As String, ByVal TrickRounds As String, ByVal JumpRounds As String, ByVal UseNops As Int16, ByVal UseTeams As Int16, ByVal FormatCode As String, ByVal DisplayMetric As Int16) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sYrPkd As String = YrPkd
        Dim sTName As String = TournName

        Dim sSelRnd As String = RndsPkd
        Dim sSelDv As String = DivisionCodePkd 'division code passed in
        Dim sDv As String = ""  'division in skier record
        Dim sEventClass As String = ""

        Dim sTmpMemberID As String = ""
        Dim sMemberID As String = ""
        Dim sReturn As String = ""
        Dim sTmpRound As String = ""
        Dim sRound As String = ""
        Dim sFederation As String = ""
        Dim sEventScoreDescMeteric As String = ""
        Dim sTmpEventScoreDescMeteric As String = ""
        Dim sEventScore As String = ""
        Dim sTmpEventScore As String = ""
        Dim sTie As String = ""
        Dim sFirstTie As Int16 = 1
        Dim sNoTieWithPrevious As Int16 = 1
        Dim sSkierName As String = ""
        Dim stmpSkierName As String = ""
        Dim sSkierLink As String = ""
        Dim sTmpSkierLink As String = ""
        Dim sText As New StringBuilder
        Dim sArrTies(0 To 25)
        Dim sTieNumber As Int16 = 0
        Dim sScoreRunoff As String = ""
        Dim sTmpScoreRunoff As String = ""
        Dim sRunOffSection As String = ""
        Dim sSSPK As String = ""
        Dim sTmpSSPK As String = ""
        Dim sWhere As String = ""
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

        Dim sSQL As String = "SELECT [SkierName], [SanctionId], [MemberId], [AgeGroup], [Div], [City], [State], [Federation], [Event], "
        sSQL += " [EventClass], [TeamCode],[Round],[EventScore], [EventScoreDescMeteric],[ScoreRunoff],[LastUpdateDate], SSPK "
        sSQL += " From [LiveWebScoreboard].[dbo].[vSlalomResults] "
        sSQL += " Where SanctionID = ? and DiV = ? and Round = ? "
        sSQL += " order by EventScore desc "


        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand

        cmdRead.CommandType = CommandType.Text
        cmdRead.CommandText = sSQL
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input


        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sSelDv   'sDv
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input


        cmdRead.Parameters.Add("@InRnd", OleDb.OleDbType.Char)
        cmdRead.Parameters("@InRnd").Size = 1
        cmdRead.Parameters("@InRnd").Value = sSelRnd    '0 = All Rounds    sSelRnd
        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input



        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read




                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            If Not IsDBNull(MyDataReader.Item("MemberID")) Then
                                sMemberID = MyDataReader.Item("MemberID")
                            Else
                                sMemberID = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                                sEventScore = CStr(MyDataReader.Item("EventScore"))
                            Else
                                sEventScore = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventScoreDescMeteric")) Then
                                sEventScoreDescMeteric = CStr(MyDataReader.Item("EventScoreDescMeteric"))
                            Else
                                sEventScoreDescMeteric = ""
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

                            If Not IsDBNull(MyDataReader.Item("ScoreRunoff")) Then
                                sScoreRunoff = MyDataReader.Item("ScoreRunoff")
                            Else
                                sScoreRunoff = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("SSPK")) Then
                                sSSPK = MyDataReader.Item("SSPK")
                            Else
                                sSSPK = ""
                            End If

                            'Have data - create display

                            sSkierLink = "<a runat = ""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sDv & "&EV=S&TN=" & sTName & ""
                            sSkierLink += "&FC=PRO&FT=0&RP=" & sSelRnd & "&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>"
                            If sTmpMemberID = "" Then
                                'Make the header
                                sText.Append("<div>")
                                sText.Append("<b>Leader Board Rnd " & sSelRnd & " " & sDv & "Class " & sEventClass & "</b><br> ")
                                'Store values for first skier
                                sTmpMemberID = sMemberID
                                sTmpSkierLink = sSkierLink
                                stmpSkierName = sSkierName
                                sTmpEventScoreDescMeteric = sEventScoreDescMeteric
                                sTmpEventScore = sEventScore
                                sTmpScoreRunoff = sScoreRunoff
                                sTmpSSPK = sSSPK
                                sNoTieWithPrevious = 1
                            End If

                            If sTmpMemberID <> sMemberID Then
                                If sTmpEventScore = sEventScore Then 'is a tie
                                    sTie = "<span Class=""red-text""><b>T</b></span>"
                                    sNoTieWithPrevious = 0  'reset no tie variable
                                    Try
                                        If sFirstTie = 1 Then 'Mark original record and current record as tie
                                            sText.Append("<br> <b>" & sTmpSkierLink & " &nbsp;" & sTie & " " & sTmpEventScoreDescMeteric & "</b>")
                                            sText.Append("<br> <b>" & sSkierLink & " &nbsp;" & sTie & " " & sEventScoreDescMeteric & "</b>")
                                            sFirstTie = 0
                                            '                                            If sTmpScoreRunoff <> "" Or sScoreRunoff <> "" Then 'DON'T MAKE A RUNNOFF SCORE SECTION UNLESS THERE ARE RUNOFF SCORES
                                            '
                                            '                                                sWhere = " WHERE SanctionID = '" & sSanctionID & "' and SSPK = " & sTmpSSPK & " or SSPK = " & sSSPK
                                            '                                                sTieNumber += 1
                                            '                                                sArrTies(sTieNumber) = (sWhere)
                                            '
                                            '                                            End If

                                        Else 'mark only current record as tie
                                            sText.Append("<br> <b>" & sSkierLink & " &nbsp;" & sTie & " " & sEventScoreDescMeteric & "</b>")
                                            '                                          If sTmpScoreRunoff <> "" Then 'DON'T MAKE A RUNNOFF SCORE SECTION UNLESS THERE ARE RUNOFF SCORES
                                            '                                              sArrTies(sTieNumber) += " OR SSPK = " & sSSPK
                                            '                                          End If
                                        End If
                                    Catch ex As Exception
                                        'skip this set of ties 
                                        Dim sStopHere As String = ""
                                    End Try


                                Else 'Not a tie
                                    'This record is not tied with previous or current record.  tmp values need to be published. .
                                    sTie = ""
                                    If sNoTieWithPrevious = 1 Then
                                        sText.Append("<br> <b>" & sTmpSkierLink & " &nbsp;" & sTie & " " & sTmpEventScoreDescMeteric & "</b>")
                                    End If
                                    'current record is not tied to previous but may be tied to next.  Save as temp
                                    sTmpSkierLink = sSkierLink
                                        sTmpEventScoreDescMeteric = sEventScoreDescMeteric
                                        sTmpMemberID = sMemberID
                                        sNoTieWithPrevious = 0
                                        sTmpEventScore = sEventScore
                                        sTmpScoreRunoff = sScoreRunoff
                                        sTmpSSPK = sSSPK
                                    sFirstTie = 1 'reset tie variable
                                    sNoTieWithPrevious = 1
                                End If

                                End If
                        Loop
                        'complete the last skier
                        sText.Append("<br> <b>" & sTmpSkierLink & " &nbsp;" & sTie & " " & sTmpEventScoreDescMeteric & "</b>")
                        sText.Append("</div>")
                        '                       If sTieNumber > 0 Then 'have ties and runoff scores
                        '                           sRunOffSection = ModDataAccessPro.GetRunoffSection(sSanctionID, "Slalom", sArrTies)
                        '                       End If
                    Else 'No data
                        sMsg = "No Skiers Found"
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetSlalomResults"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        '        Dim sTestString = sRunOffSection & "<br>" & sText.ToString()
        '        Return sRunOffSection & "<br>" & sText.ToString()
        Return sText.ToString()
    End Function
    Friend Function GetRunoffSectionOld(ByVal SanctionID As String, ByVal selEvent As String, ByVal ArrTies As Array) As String
        'Consider using Event parameter to make this work for all events
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sEvent As String = selEvent
        Dim sRnd As String = Rnd()
        Dim sTieSection As New StringBuilder
        Dim i As Int16 = 0
        Dim a1 As Int16 = 0
        Dim sSkierName As String = ""
        Dim sScoreRunoff As String = ""
        Dim sWhere As String = "WHERE "
        Dim sSQL As String = ""

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
        cmdRead.CommandType = CommandType.Text

        'note arrTies(0) is intentionally left empty
        Using Cnnt
            For i = 1 To ArrTies.GetUpperBound(0)
                If Len(ArrTies(i)) > 2 Then
                    sWhere = ArrTies(i)
                    sTieSection.Append("<b> Tie " & i & "Runoff Scores</b><br>")
                    sSQL = "SELECT SkierName, ScoreRunoff from livewebscoreboard.dbo.vSlalomResults " & sWhere & " Order by ScoreRunoff desc"
                    cmdRead.CommandText = sSQL
                    Dim MyDataReader As OleDb.OleDbDataReader = Nothing
                    Dim sCkRows As Boolean = False
                    Try
                        Using cmdRead
                            cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                            cmdRead.Connection.Open()
                            MyDataReader = cmdRead.ExecuteReader
                            If MyDataReader.HasRows = True Then
                                Do While MyDataReader.Read
                                    sSkierName = CStr(MyDataReader.Item("SkierName"))
                                    If Not IsDBNull(MyDataReader.Item("ScoreRunoff")) Then
                                        sScoreRunoff = MyDataReader.Item("ScoreRunoff")
                                    Else
                                        sScoreRunoff = ""
                                    End If

                                    'Have data - create display

                                    sTieSection.Append("<b>" & sSkierName & "  " & sScoreRunoff & "</b><br>")

                                Loop
                                cmdRead.Connection.Close()
                            Else 'No data
                                sMsg = "No RunOffs Found"
                            End If
                        End Using
                    Catch ex As Exception
                        sMsg = "Error at GetRunOff Section"
                        sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
                    End Try
                End If
            Next

        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If

        Return sTieSection.ToString()
    End Function


    Friend Function GetRunoffSection(ByVal SanctionID As String, ByVal selEvent As String, ByVal Division As String) As String
        'Consider using Event parameter to make this work for all events
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sEvent As String = selEvent
        Dim sTieSection As New StringBuilder
        Dim sSkierName As String = ""
        Dim sDV As String = Division
        Dim sTmpDV As String = ""
        Dim sScoreRunoff As String = ""
        Dim sWhere As String = "WHERE "
        Dim sSQL As String = ""

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
        cmdRead.CommandType = CommandType.Text

        sSQL = "select distinct skiername, Div, scorerunoff from vSlalomResults "
        sSQL += " where SanctionId = '" & SanctionID & "' and ScoreRunoff IS NOT NULL and Div = '" & sDv & "' "
        sSQL += " order by div, ScoreRunoff desc"
        Using Cnnt

            cmdRead.CommandText = sSQL
            Dim MyDataReader As OleDb.OleDbDataReader = Nothing
            Dim sCkRows As Boolean = False
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            sDV = CStr(MyDataReader.Item("Div"))
                            If Not IsDBNull(MyDataReader.Item("ScoreRunoff")) Then
                                sScoreRunoff = MyDataReader.Item("ScoreRunoff")
                            Else
                                sScoreRunoff = ""
                            End If

                            'Have data - create display
                            If sTmpDV = "" Then
                                sTmpDV = sDV
                                sTieSection.Append("<b>" & sDV & " Run Off Scores</b><br>")
                            End If
                            If sDV = sTmpDV Then
                                sTieSection.Append("<b>" & sSkierName & "  " & sScoreRunoff & "</b><br>")
                            Else
                                sTieSection.Append("<br><b>" & sDV & " Run Off Scores</b><br>")
                                sTieSection.Append("<b>" & sSkierName & "  " & sScoreRunoff & "</b><br>")
                            End If
                        Loop
                    Else 'No data
                        sMsg = "No RunOffs Found"
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetRunOff Section"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If

        Return sTieSection.ToString()
    End Function
End Module

