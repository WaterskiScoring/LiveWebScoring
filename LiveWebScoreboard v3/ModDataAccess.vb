Imports System.Data.Common
Imports System.Data.OleDb

Module ModDataAccess
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
        Dim sSQL As String = "SELECT SR.LastUpdateDate, SR.SanctionID, TR.SkierName, SR.MemberID, SR.AgeGroup, "
        sSQL += " SR.[round], SR.SkierRunNum As Pass, SR.Score, SR.Note, SR.Reride, SR.RerideReason, SR.ProtectedScore "
        sSQL += " From LiveWebScoreboard.dbo.SlalomRecap SR "
        sSQL += " Left Join (Select distinct SkierName, SanctionID, MemberID from LiveWebScoreboard.dbo.TourReg where sanctionID = '" & sSanctionID & "') As TR "
        sSQL += " On TR.sanctionID = SR.SanctionID And SR.MemberID = TR.MemberID "
        sSQL += " Where SR.SanctionId = '" & sSanctionID & "' "
        sSQL += " and LastUpdateDate > DateAdd(Minute, 10, GetDate())"
        sSQL += " order by SR.MemberID, LastUpdateDate desc "

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
                                '                           If sTmpMemberID <> sMemberID Then  'first record - display name
                                sTmpMemberID = sMemberID
                                sText += "<div Class=""row"">"
                                sText += "    <div Class=""col-12 bg-info text-black text-center"">"
                                sText += " " & sSkierName & " &nbsp; " & sDV & " &nbsp; Round: " & sRound & " "
                                sText += "   </div>"
                                sText += " </div>"
                            End If
                            If sTmpMemberID = sMemberID Then
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
        SQL = "SELECT MemberID, Insertdate as time, SanctionID, SkierName, AgeGroup,EventClass, EventScore, Round, EventScoreDesc from LiveWebScoreboard.dbo.vTrickResults "
        SQL += " Where SanctionID ='" & sSanctionID & "' "
        SQL += " and LastUpdateDate > DateAdd(Minute, 5, GetDate())"
        SQL += "  ORDER BY MemberID, insertdate desc"

        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
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
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sEventClass = CStr(MyDataReader.Item("EventClass"))
                            sEventScore = CStr(MyDataReader.Item("EventScore"))
                            sRound = CStr(MyDataReader.Item("Round"))
                            sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                            sTSB.Append("<Table Class=""table table-striped border-1 "">")
                            sTSB.Append("<thead><tr Class=""table-primary""><td colspan=""5"">Most Recent <span class=""bg-danger text-white"">UNOFFICIAL</span> Trick Results for " & sSkierName & "</td></tr>")
                            sTSB.Append("<tr>><th>DV</th><th>Class</th><th>Rnd</th><th>Points</th><th>Detail</th></tr></thead>")
                            sTSB.Append("<tr><td>" & sAgeGroup & "&nbsp;</td></td><td>" & sEventClass & "&nbsp;;</td><td>" & sRound & "&nbsp;</td><td>" & sEventScore & "&nbsp;</td><td>" & sEventScoreDesc & "</td></tr>")

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
            Finally
                sText = sTSB.ToString() & "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
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


        SQLsb.Append("SELECT JR.LastUpdateDate, JR.SanctionID, TR.SkierName, JR.MemberID, JR.AgeGroup, ")
        SQLsb.Append(" JR.[round], JR.PassNum As Pass, JR.Results, JR.ScoreFeet, JR.ScoreMeters, JR.Note, JR.Reride, JR.RerideReason, JR.ScoreProt ")
        SQLsb.Append(", JR.RerideIfBest, JR.RerideCanImprove ")
        SQLsb.Append(" From LiveWebScoreboard.dbo.JumpRecap JR ")
        SQLsb.Append(" Left Join (Select distinct SkierName, SanctionID, MemberID from LiveWebScoreboard.dbo.TourReg where sanctionID = '" & sSanctionID & "') ")
        SQLsb.Append(" as TR On JR.sanctionID = TR.SanctionID And JR.MemberID = TR.MemberID ")
        SQLsb.Append(" Where TR.SanctionId = '" & sSanctionID & "' ") ' following has reride  and [round] <> 25 and TR.MemberID = '200149011' "
        SQLsb.Append(" and LastUpdateDate > DateAdd(Minute, 10, GetDate())")
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
                                '                               If sTmpMemberID <> sMemberID Then  'first record - display name
                                sTmpMemberID = sMemberID
                                myStringBuilder.Append("<div Class=""row"">")
                                myStringBuilder.Append("<div Class=""col-12 bg-info text-black text-center"">")
                                myStringBuilder.Append(sSkierName & " &nbsp; " & sDV & " &nbsp; Round: " & sRound & " ")
                                myStringBuilder.Append("   </div>")
                                myStringBuilder.Append(" </div>")
                            End If
                            If sTmpMemberID = sMemberID Then
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
    Public Function ScoresXRunOrdHoriz(ByVal SanctionID As String, ByVal selEvent As String, ByVal selDv As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal TName As String) As String
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sPREventCode As String = selEvent
        Dim sSelDV As String = selDv
        Dim sSelRnd As String = selRnd
        Dim sTName As String = TName
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sEventScoreDesc As String = ""
        Dim stmpMemberID As String = ""
        Dim i As Integer = 0
        Dim sDv As String = ""
        Dim sEventClass As String = ""
        Dim sEventClassIcon As String = ""
        Dim sEventGroup As String = ""
        Dim sRnd As String = ""
        Dim sRunOrd As New StringBuilder
        Dim sSlalomHeader As String = ""
        Dim sTrickHeader As String = ""
        Dim sJumpHeader As String = ""
        Dim sLine As New StringBuilder
        Dim sRndsSlalomOffered As String = RndsSlalomOffered
        Dim sRndsTrickOffered As String = RndsTrickOffered
        Dim sRndsJumpOffered As String = RndsJumpOffered


        Dim sSql As String = ""
        Select Case selEvent
            Case "S"
                sPREventCode = "Slalom"
                sSql = "PrSlalomScoresByRunOrder"
                If sSelRnd = 0 Then
                    sLine.Append("<Table Class=""table  border-1 "">") '& sSlalomHeader) '& sSlalomDVHeader)
                    sSlalomHeader = "<thead><tr><th class=""table-success"">SLALOM<br>Running Order</th><th class=""table-primary"">Group / Div </th>"
                    For i = 1 To RndsSlalomOffered
                        sSlalomHeader += "<th class=""table-primary"">Round " & i & "</th>"
                    Next
                    sLine.Append(sSlalomHeader & "</tr></thead>")
                Else
                    sSlalomHeader = "<thead><tr><th class=""table-success"">SLALOM<br>Running Order</th><th class=""table-primary"">Group / Div </th><th>Score</th></thead>"
                End If
            Case "T"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresByRunOrder"
                If sSelRnd = 0 Then
                    sLine.Append(" <Table Class=""table  border-1 "">") '& sTrickHeader) '& sTrickDVHeader)
                    sTrickHeader = "<thead><tr><th class=""table-success"">TRICK<br>Running Order</th><th class=""table-primary"">Group / Div </th>"
                    For i = 1 To RndsTrickOffered
                        sTrickHeader += "<th class=""table-primary"">Round " & i & "</th>"
                    Next
                    sLine.Append(sTrickHeader & "</tr></thead>")
                Else
                    sSlalomHeader = "<thead><tr><th class=""table-success"">SLALOM<br>Running Order</th><th class=""table-primary"">Group / Div </th><th>Score</th></thead>"
                End If
            Case "J"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresByRunOrder"
                If sSelRnd = 0 Then
                    sLine.Append("<Table Class=""table  border-1 "">") '& sJumpHeader)  ' & sJumpDVHeader)
                    sJumpHeader = "<thead><tr><th class=""table-success"">JUMP<br>Running Order</th><th class=""table-primary"">Group / Div </th>"
                    For i = 1 To sRndsJumpOffered
                        sJumpHeader += "<th class=""table-primary"">Round " & i & "</th>"
                    Next
                    sLine.Append(sJumpHeader & "</tr></thead>")

                Else
                    sSlalomHeader = "<thead><tr><th class=""table-success"">SLALOM<br>Running Order</th><th class=""table-primary"">Group / Div </th><th>Score</th></thead>"
                End If
            Case Else  'Load all by default
                sPREventCode = "ALL"
        End Select
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
        cmdRead.Parameters("@InRnd").Value = sSelRnd    '0 = All Rounds    sSelRnd
        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sSelDV   'sDv
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InGroup", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InGroup").Size = 3
        cmdRead.Parameters("@InGroup").Value = "ALL"   'sEventGroup
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

                            If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEventScoreDesc = MyDataReader.Item("EventScoreDesc")
                            Else
                                sEventScoreDesc = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("Round")) Then
                                sRnd = MyDataReader.Item("Round")
                            Else
                                sRnd = 0
                            End If

                            If stmpMemberID = "" Then
                                stmpMemberID = sMemberID ' first record in first pass
                                Select Case sEventClass
                                    Case "C"
                                        sEventClassIcon = "<img src=""Images/C.png"" alt=""Class C"" title=""Class C"" />"
                                    Case "E"
                                        sEventClassIcon = "<img src=""Images/E.png"" alt=""Class E"" title=""Class E"" />"
                                    Case "L"
                                        sEventClassIcon = "<img src=""Images/L.png"" alt=""Class L"" title=""Class L"" />"
                                    Case "R"
                                        sEventClassIcon = "<img src=""Images/R.png"" alt=""Class R"" title=""Class R"" />"
                                End Select

                                sLine.Append("<tr><td class=""table-success""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sDv & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a></td>")
                                sLine.Append("<td>" & sEventGroup & " /<b> " & sDv & "</b></td>")
                            End If

                            If stmpMemberID = sMemberID Then
                                If selRnd = 0 Then
                                    i += 1
                                    If sRnd > i Then  'If first score is in round 2 or greater - fill in earlier rounds as blanks
                                        Do Until sRnd = i
                                            sLine.Append("<td></td>")
                                            i += 1
                                        Loop
                                    End If
                                    Select Case sRnd
                                        Case 1
                                            sLine.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")
                                      '                           sLine.Append("<tr><td class=""table-success"">" & sSkierName & "</td><td>" & sEventGroup & " / " & sDv & " / " & sEventClass & "</td><td>" & sEventScoreDesc & "</td>")
                                        Case 2
                                            sLine.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")
                                        Case 3
                                            sLine.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")
                                        Case 4
                                            sLine.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")
                                        Case 5
                                            sLine.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")
                                        Case 6
                                            sLine.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")
                                        Case 0  'error
                                            sLine.Append("<td>Rnd Error</td>")
                                    End Select

                                Else 'only need score for selected round
                                    sLine.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sDv & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td>")
                                    sLine.Append("<td>" & sEventGroup & " / <b>" & sDv & "</b> </td><td>" & sEventClassIcon & " " & sEventScoreDesc & "</td></tr>")
                                End If
                            Else 'New skier
                                'fill in any empty <td></td> if skier did not ski all rounds
                                If i < RndsSlalomOffered Then
                                    For i = i To RndsSlalomOffered
                                        i += 1
                                        Select Case i
                                            Case 2
                                                sLine.Append("<td></td>")
                                            Case 3
                                                sLine.Append("<td></td>")
                                            Case 4
                                                sLine.Append("<td></td>")
                                            Case 5
                                                sLine.Append("<td></td>")
                                        End Select
                                    Next
                                    'Close out the line
                                    sLine.Append("</tr>")
                                Else 'No extra columns needed - close out the line
                                    sLine.Append("</tr>")
                                End If

                                'Finished previous line - start new
                                stmpMemberID = sMemberID

                                If selRnd = 0 Then
                                    i = 1
                                    Select Case sEventClass
                                        Case "C"
                                            sEventClassIcon = "<img src=""Images/C.png"" alt=""Class C"" title=""Class C"" />"
                                        Case "E"
                                            sEventClassIcon = "<img src=""Images/E.png"" alt=""Class E"" title=""Class E"" />"
                                        Case "L"
                                            sEventClassIcon = "<img src=""Images/L.png"" alt=""Class L"" title=""Class L"" />"
                                        Case "R"
                                            sEventClassIcon = "<img src=""Images/R.png"" alt=""Class R"" title=""Class R"" />"
                                    End Select
                                    sLine.Append("<tr><td class=""table-success""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sDv & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a></td>")
                                    sLine.Append("<td>" & sEventGroup & " / <b>" & sDv & "</b></td>")
                                    If sRnd > i Then  'If first score is in round 2 or greater - fill in earlier rounds as blanks
                                        Do Until sRnd = i
                                            sLine.Append("<td></td>")
                                            i += 1
                                        Loop
                                    End If
                                    sLine.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")
                                Else 'only need score for selected round
                                    sLine.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sDv & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td>")
                                    sLine.Append("<td>" & sEventGroup & " / <b>" & sDv & "</b></td><td>" & sEventClassIcon & " " & sEventScoreDesc & "</td></tr")
                                End If
                            End If
                        Loop
                        sLine.Append("</tr></table>")
                    Else 'No data
                        sLine.Append("No Skiers Found")
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
        Return sLine.ToString()
    End Function
    Public Function BestRncXEvDv(ByVal SanctionID As String, ByVal selEvent As String, ByVal selDv As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String) As String
        ' Use ModDataAccess2.LeaderBoard instead
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sPREventCode As String = selEvent
        Dim sSelDV As String = selDv
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sScoreBest As String = ""
        Dim stmpMemberID As String = ""
        Dim i As Integer = 0
        Dim sDv As String = ""
        Dim sTmpDv As String = ""
        Dim sEventClass As String = ""
        Dim sEventGroup As String = ""
        Dim sSlalomHeader As String = ""
        Dim sTrickHeader As String = ""
        Dim sJumpHeader As String = ""
        Dim sLine As New StringBuilder
        Dim sSql As String = ""
        Select Case selEvent
            Case "S"
                sPREventCode = "Slalom"
                sSql = " PrSlalomScoresBestByDiv"
                sLine.Append("<Table Class=""table  border-1 "">") '& sSlalomHeader) '& sSlalomDVHeader)
                sSlalomHeader = "<thead><tr class=""table-primary"" ><th> Name </th><th>Buoys</th>"
                sLine.Append(sSlalomHeader & "</tr></thead>")
            Case "T"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresBestByDiv"
                sLine.Append(" <Table Class=""table  border-1 "">") '& sTrickHeader) '& sTrickDVHeader)
                sTrickHeader = "<thead><tr class=""table-primary"" ><th> Name </th><th>Points</th>"
                sLine.Append(sTrickHeader & "</tr></thead>")
            Case "J"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresBestByDiv"
                sLine.Append("<Table Class=""table  border-1 "">") '& sJumpHeader)  ' & sJumpDVHeader)
                sJumpHeader = "<thead><tr class=""table-primary"" ><th> Name </th><th>Feet</th>"
                sLine.Append(sJumpHeader & "</tr></thead>")

            Case Else  'Load all by default
                sPREventCode = "ALL"
        End Select
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
        cmdRead.Parameters("@InDV").Value = "ALL"   'sDv
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
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            If Not IsDBNull(MyDataReader.Item("DiV")) Then
                                sDv = CStr(MyDataReader.Item("DiV"))
                            Else
                                sDv = ""
                            End If
                            sMemberID = MyDataReader.Item("MemberID")
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
                            If sTmpDv <> sDv Then
                                'Make a division header
                                sLine.Append("<tr><td colspan=""2""><span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span> " & sDv & " Scores</td></tr>")
                                sTmpDv = sDv
                            Else
                                'Add the data line
                                sLine.Append("<tr><td> " & sSkierName & "</td><td>" & sScoreBest & "</td></tr>")
                            End If
                        Loop
                        sLine.Append("</table>")
                    Else
                        sLine.Append("No Scores Found ")
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetRunOrder"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try

        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
        Else
            Return sLine.ToString()
        End If
    End Function
    Friend Function LoadDvList(ByVal sanctionID As String, ByVal EventCodePkd As String, ByVal DVPkd As String, ByVal RndPkd As String, ByRef DDL_Division As DropDownList, ByVal SRnds As String, ByVal TRnds As String, ByVal JRnds As String, ByRef DDL_PkRnd As DropDownList) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sAgeGroup As String = ""
        Dim sSanctionID As String = sanctionID
        Dim sEventCode As String = EventCodePkd
        Dim sPREventCode As String = ""
        Dim sDVPkd As String = DVPkd
        Dim sRndPkd As String = RndPkd
        Dim sSB As New StringBuilder
        Dim sSQL As String = ""

        sSQL = "PrGetUsedAgeGroups"
        Select Case sEventCode
            Case "A"
                sPREventCode = "ALL"
            Case "S"
                sPREventCode = "Slalom"
            Case "T"
                sPREventCode = "Trick"
            Case "J"
                sPREventCode = "Jump"
            Case "O"
                sPREventCode = "Overall"
            Case Else  'Load all by default
                sPREventCode = "ALL"
        End Select

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

        cmdRead.CommandType = CommandType.StoredProcedure
        cmdRead.CommandText = sSQL
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InEvCode", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InEvCode").Size = 12
        cmdRead.Parameters("@InEvCode").Value = sPREventCode
        cmdRead.Parameters("@InEvCode").Direction = ParameterDirection.Input



        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                With DDL_Division
                    .Items.Clear()
                    .Items.Add(New ListItem("ALL", "ALL"))
                    Using cmdRead

                        cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
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
                            .Items.Add(New ListItem("None Found", "0"))
                        End If 'end of has rows
                    End Using
                End With
            Catch ex As Exception
                sMsg += "Error: Can't retrieve Divisions Entered. "
                sErrDetails = " LoadDvList Caught: " & ex.Message & " " & ex.StackTrace & "<br>SQL= " & sSQL
                DDL_Division.Items.Clear()
            End Try
        End Using

        'LIMIT ROUNDS BASED ON ROUNDS OFFERED IN SELECTED EVENT
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

        With DDL_PkRnd
            .Items.Clear()
            .Items.Add(New ListItem("ALL", "0"))
            Select Case sEventCode
                Case "A"   'Include all rounds if overall is picked - or include overall and use minimum number of rounds offered in S, T, or J
                    For i = 1 To sMaxRounds
                        .Items.Add(New ListItem(i, i))
                    Next
                Case "S"
                    For i = 1 To sSlalomRnds
                        .Items.Add(New ListItem(i, i))
                    Next
                Case "T"
                    For i = 1 To sTrickRnds
                        .Items.Add(New ListItem(i, i))
                    Next
                Case "J"
                    For i = 1 To sJumpRnds
                        .Items.Add(New ListItem(i, i))
                    Next
                Case "O"
                    For i = 1 To sMinRounds
                        .Items.Add(New ListItem(i, i))
                    Next
            End Select
        End With
        If Len(sMsg) > 2 Then
            Return sMsg
        End If
        Return "Success"
    End Function
    Friend Function GetOfficials(ByVal SanctionID As String) As Array
        Dim sSanctionID As String = SanctionID
        Dim arrOfficials(0 To 20, 0 To 2)
        Dim i As Integer = 1
        Dim sJudgeChief As String = ""
        Dim sDriverChief As String = ""
        Dim sScoreChief As String = ""
        Dim sSafetyChief As String = ""
        Dim sJudgeAppointed As String = ""
        Dim sTechChief As String = ""
        Dim sSQL As String = ""
        Dim sUsePR As Boolean = CBool(ConfigurationManager.ConnectionStrings("Use_PR").ConnectionString)
        Dim sAssignment As String = ""
        If sUsePR = False Then
            sSQL = "select distinct TR.MemberID, TR.SkierName, TR.Notes, OW.JudgeChief, OW.DriverChief, OW.ScoreChief, OW.SafetyChief, OW.TechChief, OW.JudgeAppointed "
            sSQL += " From LiveWebScoreboard.dbo.TourReg TR left Join LiveWebScoreboard.dbo.OfficialWork OW on TR.MemberID = OW.MemberId "
            sSQL += " Where OW.SanctionId = '" & sSanctionID & "' and  (OW.JudgeAppointed = 'Y' or OW.JudgeChief = 'Y' or OW.DriverChief = 'Y' or OW.ScoreChief = 'Y' or OW.SafetyChief = 'Y' or OW.TechChief = 'Y'  )"
            sSQL += " And TR.Notes = 'Appointed Official'"
        Else
            sSQL = "PrGetOfficialsPanel"   '24E017, 24U269, 23E998, 23S999
        End If
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSkierName As String = ""
        Dim sPosition As String = ""

        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: GetTournamentSpecs could not retrieve connection string. "
            sErrDetails = "At GetOfficials Conn" & ex.Message & "  " & ex.StackTrace
            arrOfficials(0, 0) = sMsg
            Return arrOfficials
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        '        Dim sTableWidth As String = "100%"
        '        Dim sText As String = "<table width=""" & sTableWidth & """>"
        Dim cmdRead As New OleDb.OleDbCommand
        If sUsePR = False Then
            cmdRead.CommandType = CommandType.Text
            cmdRead.CommandText = sSQL

        Else  'sUsePR = true
            cmdRead.CommandType = CommandType.StoredProcedure
            cmdRead.CommandText = sSQL
            cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
            cmdRead.Parameters("@InSanctionID").Size = 6
            cmdRead.Parameters("@InSanctionID").Value = sSanctionID
            cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

        End If




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


                            sSkierName = MyDataReader.Item("SkierName") 'this field is in both queries

                            If sUsePR = True Then

                                sAssignment = MyDataReader.Item("Assignment")
                                arrOfficials(i, 1) = sAssignment
                                arrOfficials(i, 2) = sSkierName
                                ' IF using officials ratings need to test for null unless driver and scorer ratings are included.
                                'JudgeSlalomRating
                                'JudgeTrickRating
                                'JudgeJumpRating

                            ElseIf sUsePR = False Then
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
                            End If
                            i += 1
                        Loop
                    Else
                        arrOfficials(0, 0) = "No Officials Data is Available."
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: at GetOfficials"
                sErrDetails = ex.Message & " " & ex.StackTrace
                arrOfficials(0, 0) = sMsg
            End Try
        End Using
        Return arrOfficials
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
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If

        Catch ex As Exception
            sMsg = "Error: Can not access data"
            sErrDetails = "Error: GetTournamentSpecs could not retrieve connection string. " & ex.Message & "  " & ex.StackTrace
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
                sErrDetails += "Error: GetTournamentSpecs Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br> SQL= " & SQL
                arrSpecs(0, 0) = "Error retrieving tournament information."
            End Try
        End Using
        Return arrSpecs
    End Function
    Friend Function GetTournamentList2(ByVal SkiYr As String) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSkiYr As String = Trim(SkiYr)
        Dim sHasVideo As String = ""
        Dim sVCount As Int32 = 0
        Dim curCalYr As String = Right(Year(Now()), 2)
        Dim lastYear As String = curCalYr - 2  'limit recordset to this calendar year and previous calendar year.  Consider using datediff back 3 months. instead of lastyear  
        Dim SQL As String = ""
        Dim sSB As New StringBuilder
        If sSkiYr = "0" Then
            '           SQL = "Select Top 20 SanctionID, Name, Class, Format(cast(EventDates as date), 'yyyyMMdd') AS FormattedDate, EventDates, EventLocation, Rules, SlalomRounds, TrickRounds, JumpRounds "
            '           SQL += " from LiveWebScoreboard.dbo.Tournament WHERE left(SanctionID,2) > " & lastYear & " and ISDATE(EventDates) = 1 "
            sSB.Append("  Select top 20 T.SanctionId, Name, Class, Format(cast(EventDates As Date), 'yyyyMMdd') AS FormattedDate, ")
            sSB.Append("  EventDates, EventLocation, Rules, SlalomRounds, TrickRounds, JumpRounds, COALESCE(VideoCount, 0) as VCount ")
            sSB.Append(" From Tournament T ")
            sSB.Append(" Left Outer Join ( Select TV.SanctionId, Count(Pass1VideoUrl) + Count(Pass1VideoUrl) as VideoCount From TrickVideo TV Group by SanctionId) as V ON V.SanctionId = T.SanctionId ")
            sSB.Append(" Where Left(T.SanctionId, 2) > " & lastYear & " and ISDATE(EventDates) = 1")
            sSB.Append("  Order by  FormattedDate desc ")

        Else
            '           SQL = "Select SanctionID, Name, Class, Format(cast(EventDates as date), 'yyyyMMdd') AS FormattedDate, EventDates, EventLocation, Rules, SlalomRounds, TrickRounds, JumpRounds from LiveWebScoreboard.dbo.Tournament "
            '           SQL += " WHERE left(SanctionID, 2) = '" & sSkiYr & "' AND ISDATE(EventDates) = 1 "
            sSB.Append("  Select T.SanctionId, Name, Class, Format(cast(EventDates As Date), 'yyyyMMdd') AS FormattedDate, ")
            sSB.Append("  EventDates, EventLocation, Rules, SlalomRounds, TrickRounds, JumpRounds, COALESCE(VideoCount, 0) as VCount ")
            sSB.Append(" From Tournament T ")
            sSB.Append(" Left Outer Join ( Select TV.SanctionId, Count(Pass1VideoUrl) + Count(Pass1VideoUrl) as VideoCount From TrickVideo TV Group by SanctionId) as V ON V.SanctionId = T.SanctionId ")
            sSB.Append(" Where Left(T.SanctionId, 2)  = '" & sSkiYr & "' AND ISDATE(T.EventDates) = 1 ")
            sSB.Append("  Order by FormattedDate desc ")

        End If
        SQL = sSB.ToString()



        '  Dave's solution - incorporate other Tournament fields from above.
        '        Select Case T.SanctionId, name, COALESCE(VideoCount, 0)
        'From Tournament T
        'Left Outer Join ( Select TV.SanctionId, Count(Pass1VideoUrl) + Count(Pass1VideoUrl) as VideoCount From TrickVideo TV Group by SanctionId
        ') as V ON V.SanctionId = T.SanctionId
        'Where Left(T.SanctionId, 2) in ('23', '24')
        '            Order by T.SanctionId


        'Query from wfwShowTourList.php
        '        $TourneyQry = "Select SanctionId, Name, Class, EventLocation"
        '        . ", SlalomRounds, TrickRounds, JumpRounds"
        '			. ", STR_TO_DATE(EventDates, '%m/%d/%Y') as EventDate "
        '			. ", (Select count(*) from TrickVideo V Where V.SanctionId = T.SanctionId "
        '			. "And (V.Pass1VideoUrl is not null or V.Pass2VideoUrl is not null)) as TrickVideoCount "
        '			. "from Tournament T "
        ''			. "Where SanctionId like '" . $SkiYear . "%'"
        '			. "Order By STR_TO_DATE(EventDates, '%m/%d/%Y') DESC";

        Dim sqlTV As String = ""

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
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Can Not access data "
            sErrDetails = "<br />" & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)

        Dim sHTML As New StringBuilder("")
        sHTML.Append("<table class=""table table-striped border-1"">")
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Using cmdRead
                Try
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
                            sVCount = MyDataReader.Item("VCount")
                            If sVCount > 0 Then
                                sHasVideo = "<img src=""Images/Flag-green16.png"" alt=""Trick Video Available"" title=""Trick Video Available, Select skier on Entry List"" />"
                            End If
                            sLblText = sSanctionID & "</b> " & sEventLocation
                            sHTML.Append("<tr><td>" & sHasVideo & "</td><td><a runat=""server"" href=""Tournament.aspx?SN=" & sSanctionID & "&FM=1&SY=" & sSkiYr & """><b>" & sName & "</b></a><b> " & sEventDates & " " & sLblText & "</td></tr>")
                            '
                            sHasVideo = ""
                        Loop
                    Else
                        sHTML.Append("<tr><td>No Tournaments Results on file in " & curCalYr & "</td></tr>")
                    End If 'end of has rows

                Catch ex As Exception
                    sMsg += "Error: getting tournament list  "
                    sErrDetails = ex.Message & " " & ex.StackTrace & "<br>SQL= " & SQL
                    sMsg += sErrDetails
                Finally

                End Try
            End Using
        End Using
        sHTML.Append("</table>")

        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sHTML.ToString
    End Function
    Friend Function GetEntryList(ByVal SanctionID As String, ByVal TournName As String, ByVal EventPkd As String, ByVal AgeDvPkd As String, YrPkd As String) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
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
        sSQL = "Select * from LivewebScoreBoard.dbo.vSkiersEntered "

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
        '        sOrderBy = " Order By MemberID, Event, EventClass, AgeGroup "
        sOrderBy = " Order By SkierName "
        sSQL += sWhere & sOrderBy
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If

        Catch ex As Exception
            sMsg = "Can not access data"
            sErrDetails = "GetEntryList at Conn " & ex.Message & "  " & ex.StackTrace
        End Try

        Dim FirstPass As Boolean = True
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sLblText As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sTableWidth As String = "100%"
        Dim sTSB As New StringBuilder
        Dim sText As String = ""
        sTSB.Append("<table class=""table table-striped border-1"" width=" & sTableWidth & " cellpadding=""5"">")
        sTSB.Append("<tr><td colspan=""4""><h4>Select a skier</h4></td></tr>")
        sTSB.Append("<tr><td><b>Skier Name</b></td><td><b>Age Group</b></td><td><b>Team</b></td><td><b>OK2Ski</b></td></tr>")
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
                                sLine = "<tr><td><a runat=""server"" href=""TIndScores.aspx?SN=" & sSanctionID & "&SID= " & sMemberID & "&SY= " & sYrPkd & "&TN=" & sTournName & "&EV=" & sEventPkd & "&N=" & sSkierName & "&DV=" & sAgeDvPkd & """>" & sSkierName & "</a> </td><td>" & sAgeGroup & "</td><td>" & sShoTeam & "</td><td>" & sFlag & "</td></tr>"
                                sTSB.Append(sLine)
                                '                                sTmpEvent = sEvent
                                sTmpSkierID = sMemberID
                                '                                sTmpEventClass = sEventClass
                                '                                sTmpAgeGroup = sAgeGroup
                                sTmpReadyToSki = sReadyToSki
                                '                            sEnteredIn = sEvent & " Cls " & sEventClass
                                sEnteredIn = sEvent & " "
                                sLine = ""
                                sFlag = ""
                                sTmpTeam = sTeam
                                sShoTeam = ""
                            End If
                            '                            If sTmpEvent <> sEvent Then  'Event changed make new string section with current record
                            '                                '      sEnteredIn += ": " & sEvent & " Cls " & sEventClass & " "
                            '                                sEnteredIn += ": " & sEvent & " "
                            '                                sTmpEvent = sEvent
                            '                                '           sTmpEventClass = sEventClass
                            '                            End If
                            '                           '       If sTmpEventClass <> sEventClass Then
                            '                            '           sEnteredIn += "," & sEventClass
                            '                            '           sTmpEventClass = sEventClass
                            '                            '       End If
                        Loop
                    Else
                        sTSB.Append("<tr><td colspan=""4"">No Skiers Found.</td></tr>")
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: at GetEntryList"
                sErrDetails = "GetTournamentList Caught: " & ex.Message & " " & ex.StackTrace & "<br>SQL= " & sSQL

            Finally
                sText = sTSB.ToString() & "</table>"
            End Try

        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function
    Friend Function IndivSlalomResults(ByVal SanctionID As String, ByVal MemberID As String, ByVal AgeGroup As String, ByVal SkierName As String) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID

        Dim SQL As String = ""
        SQL = "SELECT * from dbo.vSlalomResults Where SanctionID ='" & sSanctionID & "' And MemberID = '" & Trim(sMemberID) & "' ORDER BY AgeGroup, MemberID, Round "
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSkierName As String = SkierName
        Dim sAgeGroup As String = AgeGroup
        Dim sEvent As String = ""
        Dim sEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sErrDetails = " IndivSlalomResults could not get connection string." & ex.Message & "  " & ex.StackTrace
            sMsg = "Error: No connection string"
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sText As String = ""
        Dim sTSB As New StringBuilder
        sTSB.Append("<Table Class=""table table-striped border-1 "">")
        sTSB.Append("<thead><tr class=""table-primary""><th colspan=""5""><span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span> Slalom Results for " & sSkierName & " </th></tr>")
        sTSB.Append("<tr><th>DV</th><th>Class</th><th>Rnd</th><th>Buoys</th><th>Detail</th></tr></thead>")
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

                            sTSB.Append("<tr><td>" & sAgeGroup & "&nbsp;</td><td>" & sEventClass & "&nbsp;</td><td>" & sRound & "&nbsp;</td><td>" & sEventScore & "&nbsp; </td><td>" & sEventScoreDesc & "</td></tr>")

                        Loop
                    Else
                        sTSB.Append("<tr><td colspan=""5"">No Scores Found for " & sSkierName & ".</td></tr>")
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg = "Error: at IndivSlalomResults "
                sErrDetails += ex.Message & " " & ex.StackTrace & "SQL= " & SQL
            Finally
                sText = sTSB.ToString() & "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function

    Friend Function IndivTrickResults(ByVal SanctionID As String, ByVal MemberID As String, ByVal AgeGroup As String, ByVal SkierName As String) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID
        Dim SQL As String = ""
        SQL = "SELECT * from LiveWebScoreboard.dbo.vTrickResults "
        SQL += " Where SanctionID ='" & sSanctionID & "' And MemberID = '" & Trim(sMemberID) & "'"
        SQL += "  ORDER BY AgeGroup, MemberID, Round "
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSkierName As String = SkierName
        Dim sAgeGroup As String = AgeGroup
        Dim sPass1URL As String = ""
        Dim sPass2URL As String = ""
        Dim sEvent As String = ""
        Dim sEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sPassNum As String = "0"
        Dim sConn As String = ""
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
        sTSB.Append("<Table Class=""table table-striped border-1 "">")
        sTSB.Append("<thead><tr Class=""table-primary""><td colspan=""5""><span class=""bg-danger text-white"">UNOFFICIAL</span> Trick Results for " & sSkierName & "</td></tr>")
        sTSB.Append("<tr>><th>DV</th><th>Class</th><th>Rnd</th><th>Points</th><th>Detail</th></tr></thead>")
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
                            If IsDBNull(MyDataReader.Item("Pass1VideoURL")) Then
                                sPass1URL = "Round " & sRound & " Pass 1 Video Not available."
                            Else
                                sPass1URL = MyDataReader.Item("Pass1VideoURL")
                            End If
                            If IsDBNull(MyDataReader.Item("Pass2VideoURL")) Then
                                sPass2URL = "Round " & sRound & " Pass 2 Video Is Not available."
                            Else
                                sPass2URL = MyDataReader.Item("Pass2VideoURL")
                            End If
                            sTSB.Append("<tr><td>" & sAgeGroup & "&nbsp;</td></td><td>" & sEventClass & "&nbsp;;</td><td>" & sRound & "&nbsp;</td><td>" & sEventScore & "&nbsp;</td><td>" & sEventScoreDesc & "</td></tr>")
                            sTSB.Append("<tr><td colspan=""5"">" & sPass1URL & "</td></tr>")
                            sTSB.Append("<tr><td colspan=""5"">" & sPass2URL & "</td></tr>")

                        Loop
                    Else
                        sTSB.Append("<tr><td colspan=""5"">No Scores Found For selected skier.</td></tr>")
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: Can't retrieve Trick Scores"
                sErrDetails = ex.Message & " " & ex.StackTrace & "<br>SQL = " & SQL
            Finally
                sText = sTSB.ToString() & "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function



    Friend Function IndivJumpResults(ByVal SanctionID As String, ByVal MemberID As String, ByVal AgeGroup As String, ByVal SkierName As String) As String
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID


        Dim SQL As String = ""
        SQL = "SELECT * from dbo.vJumpResults "
        SQL += " Where SanctionID ='" & sSanctionID & "' And MemberID = '" & Trim(sMemberID) & "'"
        SQL += "  ORDER BY AgeGroup, MemberID, Round "
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSkierName As String = SkierName
        Dim sAgeGroup As String = AgeGroup
        Dim sEvent As String = ""
        Dim sEventScore As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: IndivJumpResults could not get connection string. "
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sText As String = ""
        Dim sTSB As New StringBuilder
        sTSB.Append("<Table Class=""table table-striped border-1 "">")
        sTSB.Append("<thead><tr Class=""table-primary""><td colspan=""5""><span class=""bg-danger text-white"">UNOFFICIAL</span>Jump Results for " & sSkierName & "</td></tr>")
        sTSB.Append("<tr><th>DV</th><th>Class</th><th>Rnd</th><th>Distance</th><th>Detail</th></tr></thead>")

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

                            sTSB.Append("<tr><td>" & sAgeGroup & "&nbsp;</td><td>" & sEventClass & "&nbsp;</td><td>" & sRound & "&nbsp;</td><td>" & sEventScore & "&nbsp;</td><td>" & sEventScoreDesc & "</td></tr>")

                        Loop
                    Else
                        sTSB.Append("<tr><td colspan=""5"">No Scores Found for selected skier.</td></tr>")
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: Can't retrieve Jump Scores. "
                sErrDetails = ex.Message & " " & ex.StackTrace & "<br>SQL = " & SQL
            Finally
                sText = sTSB.ToString() & "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText
    End Function

    Friend Function PopulateTeams(ByVal SanctionID As String) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sSQL As String = "select distinct TeamCode from EventReg where sanctionID = '" & sSanctionID & "'"
        Dim sTeamCode As String = ""
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: LoadDVList could not get connection string. "
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Dim i As Integer = 0
        Dim sTeamList As String = ""
        Using Cnnt
            Try
                Using cmdRead
                    cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                    cmdRead.CommandText = sSQL
                    cmdRead.Connection.Open()
                    MyDataReader = cmdRead.ExecuteReader
                    If MyDataReader.HasRows = True Then
                        Do While MyDataReader.Read()
                            If IsDBNull(MyDataReader.Item("TeamCode")) Then
                                sTeamCode = "N/A"
                            Else
                                i += 1
                                sTeamCode = CStr(MyDataReader.Item("TeamCode"))
                                If i = 1 Then
                                    sTeamList += sTeamCode
                                Else
                                    sTeamList += ", " & sTeamCode
                                End If
                            End If
                        Loop
                    Else
                        sTeamList = "Error: No Teams Found for selected tournament. " 'prefix error prevents display of Panel_Teams
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: Can't retrieve Teams. "
                sErrDetails = " At Populate Teams " & ex.Message & " " & ex.StackTrace & " <br />SQL= " & sSQL
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
        Else
            Return sTeamList
        End If
    End Function

    Friend Function HasVideo(ByVal SanctionID As String) As String
        Dim sSanctionID As String = SanctionID
        Dim VCount As Int32 = 0
        Dim sHasTVideo As String = ""
        Dim sVExists As Boolean = False
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: LoadDVList could not get connection string. "
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdTV As New OleDb.OleDbCommand
        Using Cnnt
            Using cmdTV

                Try
                    With cmdTV
                        .Connection = Cnnt
                        .CommandText = "Select Count(*) from LiveWebScoreboard.dbo.vTrickResults where sanctionID = ' & sSanctionID & ' and Pass1VideoUrl is not null "
                        .Connection.Open()
                    End With
                    VCount = CInt(cmdTV.ExecuteScalar)
                    sVExists = CInt(cmdTV.ExecuteScalar) > 0

                Catch ex As Exception
                    sMsg = "Error at HasVideo"
                    sErrDetails += ex.Message & " " & ex.StackTrace
                End Try
            End Using
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return CStr(sVExists)
    End Function
    Friend Function IndivScoresXRound(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal Rnd As Byte, ByVal UseNOPS As Boolean, ByVal UseTeams As Boolean, ByVal DisplayFormat As String) As String
        'ByRound sorts by unofficial placement in each round.  Theory is that WSTIMS only allows correct group of skiers to advance to next round.  Not necessary to redo the calculations.
        'Need to create Team Placement for NCWSA
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = EventCode
        Dim sRnd As Byte = Rnd  'default is all rounds.  May add ability to select round
        'Use zero 0 for fields not in use
        Dim sDVCode As String = DivisionCode
        Dim sSql As String = BuildSQL(sSanctionID, sEventCode, sDVCode, sRnd, False, False, "IND")  'Make sure Select works with , separated list
        If Left(sSql, 5) = "Error" Then
            Return sSql
            Exit Function
        End If
        'Process the data by class, division, score desc
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
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
        Dim sFirstLoop As Boolean = True
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim sSkipPlace As Int16 = 0
        Dim sStopHere As Boolean = False
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If

        Catch ex As Exception
            sMsg = "Error IndivScoresXRound could Not get connection string."
            sErrDetails = "EventCode = " & sEventCode & " " & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sText As String = "<Table class= ""table table-striped border-1 "">"
        Select Case sEventCode
            Case "S"
                sText += "<thead>"
                sText += "<tr Class=""table-primary"" ><th colspan=""5"">Slalom Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
            Case "T"
                sText += "<thead>"
                sText += "<tr Class=""table-primary"" ><th colspan=""5"">Trick Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
            Case "J"
                sText += "<thead>"
                sText += "<tr Class=""table-primary"" ><th colspan=""5"">Jump Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Class</th><th>Rnd</th><th>NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
            Case "O"
                sText += "<thead>"
                sText += "<tr Class=""table-primary"" ><th colspan=""5"">Overall Results</th></tr>"
                sText += "<tr><th>Place</th><th>Name</th><th>DV</th><th>Rnd</th><th></th>Overall Score<th>Slalom NOPS</th><th>Trick NOPS</th><th>Jump NOPS</th><th>Detail</th></tr>"
                sText += "</thead>"
                sSql = "Select SanctionID, SkierName, MemberID, AgeGroup, [round] SlalomNopsScore, TrickNopsScore, JumpNOPSScore, SlalomScore, TrickScore, JumpFeet, JumpMeters "

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
                sMsg += "Error: at PlacementXRound "
                sErrDetails = "EventCode = " & sEventCode & " " & ex.Message & " " & ex.StackTrace & "<br>Sql = " & sSql

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
    Friend Function BuildSQL(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal RndNum As Byte, ByVal UseNops As Boolean, ByVal UseTeams As Boolean, ByVal FormatCode As String) As String
        'Builds the sql query string based on the parameters passed in.
        'called once for each event
        '  vSlalomResults, vTrickResults, vJumpResults, vOverallResults Incude all necessary performance information for each event. 
        ' Add button to change sort to display by EventClass, AgeGroup,  EventScore DESC or Gender, NopsScore DESC.  Maybe omit EventClass in sort and just list.

        Dim sSanctionID As String = Trim(SanctionID)
        Dim sEventCode As String = EventCode
        Dim sDvCodePkd As String = DivisionCode
        Dim sRound As Byte = RndNum    '1, 2 3, or 4.  If 0 include all
        Dim sFormatCode As String = FormatCode 'allowable values DV, IND, MR, ROBest, LB (sort by division, individual skier, Most Recent)
        Dim sDvText As String = ""
        If sDvCodePkd <> "ALL" Then
            sDvText = " and AgeGroup = '" & sDvCodePkd & "' "
        End If

        Dim sRoundText As String = ""
        Select Case sRound
            Case 0 ' leave out to include all rounds
            Case 1
                sRoundText = " and Round = 1 "
            Case 2
                sRoundText = " and Round = 2 "
            Case 3
                sRoundText = " and Round = 3 "
            Case 4
                sRoundText = " and Round = 4 "
            Case 5
                sRoundText = " and Round = 5 "
            Case 6
                sRoundText = " and Round = 6 "
            Case Else  'Include all rounds
        End Select
        Dim sViewName As String = ""
        Dim sSQL As String = ""
        Dim sUseNops As Boolean = UseNops
        Dim sUseTeams As Boolean = UseTeams  'Use to build team format display.  Probably order by gender, nopsscore
        'Add second display select top 5 nops in each event where gender = M, select top 5 nops in each event where genter = F.  Calculate team scores

        Dim sOrderBy As String = ""
        Select Case sFormatCode
            Case "LB"
                sOrderBy = "Order by AgeGroup, Round asc"
            Case "DV", "IND"
                sOrderBy = "Order By Round asc, AgeGroup,  EventScore DESC"
                If sUseNops = True Then
                    sOrderBy = " Order By Round asc, Gender, NopsScore DESC"
                End If
            Case "MR"
                sOrderBy = " Order By Time desc"
        End Select
        'What to display is determined in TXDivisionPro by calling this once for each event to be included in the display.
        'Scores appear as they are posted and displayed by round, division, and score
        'Display is determined in moddataaccess.DisplayXRndandDv which calls this function.
        Select Case sEventCode
            Case "S"
                sViewName = " LiveWebScoreBoard.dbo.vSlalomResults "
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, NopsScore, EventScore, EventClass, TeamCode, Round, Event, EventScoreDesc,  FORMAT(InsertDate, 'MM/dd/yy HH:mm') as Time "
                sSQL += " From " & sViewName & " where SanctionID = '" & sSanctionID & "' " & sRoundText & sDvText & sOrderBy
            Case "T"
                sViewName = " LiveWebScoreBoard.dbo.vTrickResults "
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, NopsScore, EventScore, EventClass, TeamCode, Round, Event, EventScoreDesc,   FORMAT(InsertDate, 'MM/dd/yy HH:mm') as Time   "
                sSQL += " From " & sViewName & " where SanctionID = '" & sSanctionID & "' " & sRoundText & sDvText & sOrderBy
            Case "J"
                sViewName = " LiveWebScoreBoard.dbo.vJumpResults "
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, NopsScore, EventScore, EventClass, TeamCode, Round, Event, EventScoreDesc,   FORMAT(InsertDate, 'MM/dd/yy HH:mm') as Time   "
                sSQL += " From " & sViewName & " where SanctionID = '" & sSanctionID & "' " & sRoundText & sDvText & sOrderBy
            Case "O"
                sViewName = " LiveWebScoreBoard.dbo.vOverallResults "
                sOrderBy = " Order By AgeGroup, EventScore Desc"
                sSQL = "Select SanctionID, SkierName, MemberID, AgeGroup, [round], OverallScore as EventScore, SlalomNopsScore, TrickNopsScore, JumpNOPSScore  "
                sSQL += " From " & sViewName & " where SanctionID = '" & sSanctionID & "' " & sRoundText & sDvText & sOrderBy
        End Select
        Return sSQL
    End Function
    Friend Function ChkRnd4Scores(ByVal SanctionID As String, ByVal EventCode As String, ByVal Rnd As Byte) As String
        'Checks to see if any scores have been posted in the speified event and round

        'THIS COULD BE IMPROVED BY RUNNING THE ROUND LOOP HERE INSTEAD OF IN THE CALLING PROCEDURE
        'EVALUATE THE ROUND HERE AND RETURN THE DESIRED ONE TO THE CALLING PROCEDURE
        'REPEAT CALL FOR EACH EVENT.
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
            Case "O"
                sView2Use = "vOverallResults"
        End Select
        Dim sSQL As String = "Select EventScore from " & sView2Use & " where SanctionID = '" & sSanctionID & "' and Round = " & sRound & " and EventScore > 0 "
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
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
                sErrDetails = "ChkRnd4Scores caught " & ex.Message & " " & ex.StackTrace & "SQL= " & sSQL
            End Try
        End Using
        If sMsg = Nothing Then
            sMsg = "0"
        End If
        Return sMsg
    End Function
    Friend Function DisplayXRndandDv(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal RoundNum As Byte, ByVal UseNops As Boolean, ByVal UseTeams As Boolean) As String

        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = EventCode
        Dim sDivisionCode As String = DivisionCode
        Dim sUseNops As Boolean = UseNops
        Dim sUseTeams As Boolean = UseTeams
        Dim sRndNum As Byte = RoundNum
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sFormatCode As String = "DV" 'By Round always is FormatCode DV
        Dim sSql As String = BuildSQL(sSanctionID, sEventCode, sDivisionCode, sRndNum, sUseNops, sUseTeams, sFormatCode)  'Make sure Select works with , separated list
        If Left(sSql, 5) = "Error" Then
            Return "Error at BuildSQL"
            sErrDetails = sSql
            Exit Function
        End If
        'Process the data by class, division, score desc
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
        Dim sNOPS As String = False ' database value for nops
        Dim sSNops As String = ""
        Dim sTNops As String = ""
        Dim sJNops As String = ""
        Dim sRound As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sFirstLoop As Boolean = True
        Dim sFirstPlace As String = ""
        Dim sTime As String = ""
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim sShowWhat As String = ""



        Dim sSkipPlace As Int16 = 0
        '        Dim sStopHere As Boolean = False
        Dim sSlalomHeader As String = "<thead><tr class=""table-primary"" ><th colspan=""5""><span class=""bg-danger text-white"" >  UNOFFICIAL </span> Slalom Results</th></tr></thead>"
        Dim sTrickHeader As String = "<thead><tr class=""table-primary""><th colspan=""5""><span class=""bg-danger text-white"" >  UNOFFICIAL </span> Trick Results</th></tr></thead>"
        Dim sJumpHeader As String = "<thead><tr class=""table-primary""><th colspan=""5""><span class=""bg-danger text-white"" >  UNOFFICIAL </span> Jump Results</th></tr></thead>"

        Dim sSlalomDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>BUOYS</th><th>NOPS</th><th>Detail</th><th>Time</th></tr></thead>"
        Dim sTrickDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>Points</th><th>NOPS</th><th>Detail</th><th>Time</th></tr></thead>"
        Dim sJumpDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>Dist</th><th>NOPS</th><th>Detail</th><th>Time</th></tr></thead>"

        Dim sOverallHeader As String = "<thead><tr class=""table-primary""><th colspan=""5""><span class=""bg-danger text-white"" >  UNOFFICIAL </span> Overall Results </th></tr></thead>"
        Dim sOverallDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>Score</th><th colspan=""2"">Ind Event NOPS</th></tr></thead>"

        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If

        Catch ex As Exception
            sMsg = "Error: Connection Failure Try again."
            sErrDetails = sEventCode & "DisplayXRndandDvRound could Not get connection string." & ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sTableWidth As String = "100%"
        Dim sText As String = "<Table Class=""table  border-1 "">"
        Select Case sEventCode
            Case "S"
                sText += sSlalomHeader
                sEvent = "Slalom"
            Case "T"
                sText += sTrickHeader
                sEvent = "Trick"
            Case "J"
                sText += sJumpHeader
                sEvent = "Jump"
            Case "O"
                sText += sOverallHeader
                sEvent = "Overall"
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

                            If Not IsDBNull(MyDataReader.Item("Round")) Then
                                sRound = CStr(MyDataReader.Item("Round"))
                            Else
                                sRound = "N/A"
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                                sEventScore = CStr(MyDataReader.Item("EventScore"))
                            Else
                                sEventScore = "N/A"
                            End If
                            If sEventCode <> "O" Then 'S,T,J fields

                                If Not IsDBNull(MyDataReader.Item("Time")) Then
                                    sTime = CStr(MyDataReader.Item("Time"))
                                Else
                                    sTime = "N/A"
                                End If
                                If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                    sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                                Else
                                    sEventScoreDesc = "N/A"
                                End If
                                If Not IsDBNull(MyDataReader.Item("EventClass")) Then
                                    sEventClass = CStr(MyDataReader.Item("EventClass"))
                                Else
                                    sEventClass = "N/A"
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
                            'First record is always first place in current age division
                            If sFirstLoop = True Then
                                sTmpAgeGroup = sAgeGroup
                                sFirstLoop = False
                                j = 0
                                '        sText += "<trClass=""table-primary""><td  colspan=""6"">Class " & sEventClass & "</td></tr>"
                                sText += "<tr><td class=""table-warning"" colspan=""5"" ><b>" & sAgeGroup & "</b> - " & sEvent & " Round " & sRound & " Class " & sEventClass & "</td></tr>"
                                Select Case sEventCode
                                    Case "S"
                                        sText += sSlalomDVHeader
                                    Case "T"
                                        sText += sTrickDVHeader
                                    Case "J"
                                        sText += sJumpDVHeader
                                    Case "O"
                                        sText += sOverallDVHeader
                                End Select
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
                                    If sEventCode <> "O" Then

                                        sText += "<tr><td><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=" & sEventCode & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScore & "</td><td>" & sNOPS & "</td><td>" & sEventScoreDesc & "</td><td><span class=""text-nowrap""> " & sTime & "</span></td></tr>"
                                    Else
                                        sText += "<tr><td><a runat=""server"" href=""Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=" & sEventCode & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td> " & sEventScore & "</td><td colspan=""2"">SNops:&nbsp;" & sSNops & "&nbsp;&nbsp;TNops:" & sTNops & "&nbsp;&nbsp;JNops: &nbsp;" & sJNops & "</td></tr>"
                                    End If

                                End If
                            Else  'Division changed.  Reset the variables. Put first skier in this division in first place
                                sStringMemberIDs = ""
                                sStringMemberIDs += sMemberID & ","
                                sSkipPlace = 0
                                sTmpAgeGroup = sAgeGroup
                                sText += "<tr><td class=""table-warning"" colspan=""5"" ><b>" & sAgeGroup & "</b> - " & sEvent & " Round " & sRound & " Class " & sEventClass & "</td></tr>"
                                Select Case sEventCode
                                    Case "S"
                                        sText += sSlalomDVHeader
                                    Case "T"
                                        sText += sTrickDVHeader
                                    Case "J"
                                        sText += sJumpDVHeader
                                    Case "O"
                                        sText += sOverallDVHeader
                                End Select
                                sTmpEventScore = sEventScore
                                j = 1
                                sFirstPlace = ""
                                If j = 1 Then sFirstPlace = ""  ' Class=""table-warning"" "
                                '            sText += "<tr><td" & sFirstPlace & ">" & j & "</td><td><<a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=" & sEventCode & "&SN=" & sSkierName & """ target=""_blank"">" & sSkierName & "</a></td><td>" & sEventClass & "&nbsp; &nbsp;</td><td> " & sRound & " &nbsp; &nbsp;</td><td>" & sEventScore & "&nbsp; &nbsp;</td><td>" & sEventScoreDesc & "&nbsp; &nbsp;</td></tr>"

                                If sEventCode <> "O" Then 'Overall has different format
                                    ' sText += "<tr><td><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=" & sEventCode & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventClass & "</td><td>" & sNOPS & "</td><td>" & sEventScore & "</td><td>" & sEventScoreDesc & "</td><td>" & sTime & "</td></tr>"
                                    If sUseNops = True Then
                                        '                 sText += "<tr><td><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=" & sEventCode & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventClass & "</td><td>" & sNOPS & "</td><td>" & sEventScore & "</td><td>" & sEventScoreDesc & "</td></tr>"
                                        sText += "<tr><td><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=" & sEventCode & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScore & "</td><td>" & sNOPS & "</td><td>" & sEventScoreDesc & "</td><td><span class=""text-nowrap""> " & sTime & "</span></td></tr>"
                                    Else
                                        sText += "<tr><td><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=" & sEventCode & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScore & "</td><td>" & sNOPS & "</td><td>" & sEventScoreDesc & "</td><td><span class=""text-nowrap""> " & sTime & "</span></td></tr>"
                                    End If
                                Else
                                    sText += "<tr><td><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=" & sEventCode & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td> " & sEventScore & " &nbsp;</td><td colspan=""2"">SNops:&nbsp;" & sSNops & "&nbsp;&nbsp;TNops:" & sTNops & "&nbsp;&nbsp;JNops: &nbsp;" & sJNops & "&nbsp;</td></tr>"
                                End If

                            End If
                        Loop
                    Else
                        sText += "<tr><td colspan=""6"">No Scores Found.</td></tr>"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error at XRndandDv"
                sErrDetails = "DisplayXRndandDv:" & ex.Message & " " & ex.StackTrace & "<br>SQL= " & sSql & "<br>EventCode= " & sEventCode & ""
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

    Friend Function RecapSlalom(ByVal SanctionID As String, ByVal MemberID As String, ByVal ageGroup As String, ByVal SkierName As String) As String
        'Pulled from wfwShowScoreRecap.php
        ' 
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sText As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID
        Dim sAgeGroup As String = ageGroup
        Dim sSkierName As String = SkierName
        Dim sCity As String = ""
        Dim sState As String = ""
        Dim sFederation As String = ""
        Dim sRankingScore As String = ""
        Dim sSkierRound As String = ""
        Dim sSkierEvent As String = ""
        Dim sSQL As String = ""
        Dim sRound As Int16 = 0
        Dim sTmpRound As Int16 = 0
        Dim sTmpName As String = ""
        Dim sScore As String = ""
        Dim sEventClass As String = ""
        Dim sBuoys As String = ""
        Dim sPsLnLngth As String = ""
        Dim sNote As String = ""
        Dim sReride As String = ""
        Dim sProtected As String = ""
        Dim sRerideReason As String = ""
        Dim sHighlightRerideReason As String = ""
        ' 3 event scores in available data.
        ' 23S108, James Bryans,             OM, 000107150,  at least 1 round overall
        ' 23S108, Tristan Duplan-Fribourg, JM, 000181068, at least 1 round overall
        '       If sSkierEvent = "Slalom" Then
        sSQL = "Select SR.[Round], SR.Score, SR.PassLineLength, SR.Note, SR.Reride, SR.ScoreProt, SR.RerideReason, SS.Score as Buoys, SS.EventClass,  "
        sSQL += " TR.Federation, TR.City, Tr.State, ER.RankingScore "
        sSQL += " From LiveWebScoreboard.dbo.SlalomRecap SR "
        sSQL += " left join LiveWebScoreboard.dbo.SlalomScore SS On SR.SanctionID = SS.SanctionID And SR.MemberID = SS.MemberID "
        sSQL += " Left Join LiveWebScoreboard.dbo.TourReg TR on SR.SanctionID = TR.SanctionId And SR.MemberID = TR.MemberId "
        sSQL += " left join LiveWebScoreboard.dbo.EventReg ER on SR.sanctionID = ER.SanctionID and SR.MemberId = ER.MemberID "
        sSQL += "Where SR.SanctionId ='" & sSanctionID & "' AND  SR.MemberId='" & sMemberID & "' And  SR.AgeGroup='" & sAgeGroup & "' and SR.[Round] = SS.[Round] and ER.Event = 'Slalom'"
        sSQL += " Order By SR.[Round], SR.SkierRunNum ASC "


        '       End If
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: RecapSlalom could not get connection string. "
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try

        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        sText = "<Table class=""table "">"
        '        sText += "<thead clase=""table-primary""><tr><td colspan=""6""><b>Slalom Recap for " & sSkierName & "</b></td></tr></head>"
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



                            If IsDBNull(MyDataReader.Item("RankingScore")) Then
                                sRankingScore = " "
                            Else
                                sRankingScore = CStr(MyDataReader.Item("RankingScore"))
                            End If

                            If IsDBNull(MyDataReader.Item("City")) Then
                                sCity = " "
                            Else
                                sCity = CStr(MyDataReader.Item("City"))
                            End If

                            If IsDBNull(MyDataReader.Item("State")) Then
                                sState = " "
                            Else
                                sState = CStr(MyDataReader.Item("State"))
                            End If
                            If IsDBNull(MyDataReader.Item("Federation")) Then
                                sFederation = " "
                            Else
                                sFederation = UCase(MyDataReader.Item("Federation"))
                            End If
                            If IsDBNull(MyDataReader.Item("Buoys")) Then
                                sBuoys = "0"
                            Else
                                sBuoys = CStr(MyDataReader.Item("Buoys"))
                            End If
                            If IsDBNull(MyDataReader.Item("Score")) Then
                                sScore = "N/A"
                            Else
                                sScore = CStr(MyDataReader.Item("Score"))
                            End If

                            If IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = "N/A"
                            Else
                                sEventClass = CStr(MyDataReader.Item("EventClass"))
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

                            If sTmpRound <> sRound Then  '


                                '      sText += "<tr><td  class=""table-info"" colspan=""6""><b>Round " & sRound & " Slalom Recap " & sSkierName & "  " & sAgeGroup & "&nbsp; Class " & sEventClass & " &nbsp; <span class=""bg-danger text-white"" >  UNOFFICIAL Score: " & sBuoys & " Buoys</span></b></td></tr>"
                                sText += "<tr><td  class=""table-info"" colspan=""6""><b> " & sSkierName & "  " & sAgeGroup & "   " & sCity & "   " & sState & "   " & sFederation & "</span></b></td></tr>"
                                sText += "<tr><td class=""table-info"" colspan=""6""><b>Round " & sRound & " Slalom Recap " & " &nbsp; Class " & sEventClass & " &nbsp;&nbsp; <span Class=""bg-danger text-white"" >  UNOFFICIAL Score: " & sBuoys & " Buoys</span></b></td></tr>"

                                sText += "<tr><th>Score</th><th>Pass Detail</th><th>Reride</th><th>Protected</th><th>Ranking Score</th></tr></thead>"
                                sTmpRound = sRound
                            End If
                            sText += "<tr><td>" & sScore & "</td><td>" & sNote & "</td><td>" & sReride & "</td><td>" & sProtected & "</td><td>" & sRankingScore & "</td></tr>"
                            If sReride = "Y" Then
                                sHighlightRerideReason = " class = ""table-danger"" "
                                sText += "<tr><td " & sHighlightRerideReason & " colspan=""5"">At " & sPsLnLngth & "M Pass, Reride Reason " & sRerideReason & "</td></tr>"
                                sHighlightRerideReason = ""
                            End If
                        Loop
                    Else
                        sText += "<tr><td class=""table-warning"" colspan=""5"">No Slalom results Found For selected skier.</td></tr>"
                    End If 'end of has rows
                End Using

            Catch ex As Exception
                sMsg += "Error Can 't retrieve Slalom Scores. "
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
        Dim sPassTable As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID
        Dim sAgeGroup As String = ageGroup
        Dim sSkierName As String = SkierName
        Dim sSkierRound As String = ""
        Dim sSkierEvent As String = ""
        Dim sSQL As String = ""
        Dim sRound As Int16 = 0
        Dim sTmpRound As Int16 = 0
        Dim sPass As String = ""
        Dim sTmpPass As String = ""
        Dim sP1SubTotal As Int16 = 0
        Dim sP2SubTotal As Int16 = 0
        Dim sP1Score As String = ""
        Dim sRoundScore As String = ""
        Dim sP2Score As String = ""

        Dim sTotalScore As Int16 = 0
        Dim sTrkScore As String = ""
        Dim sSkis As String = ""
        Dim sCode As String = ""
        Dim sResults As String = ""
        Dim sPass1URL As String = ""
        Dim sPass2URL As String = ""
        Dim sTmpPass1Url As String = ""
        Dim sTmpPass2URL As String = ""
        Dim sNextPass1URL As String = ""
        Dim sNextPass2URL As String = ""
        Dim MyStringBuilder As New StringBuilder
        'This query gives pass by pass detail, score per trick, pass score and round score
        MyStringBuilder.Append(" Select TP.[SanctionID], TP.[MemberId], TR.SkierName,TP.[AgeGroup],TP.[Round], ")
        MyStringBuilder.Append(" TP.[PassNum] as Pass, TP.Seq, TP.[Skis], TP.[Score] As TrkScore, TP.[Code], TP.[Results], ")
        MyStringBuilder.Append(" TS.ScorePass1 As P1Score, TS.ScorePass2 As P2Score, TS.Score As FScore, TP.[LastUpdateDate], ")
        MyStringBuilder.Append(" TS.EventClass, TV.Pass1VideoUrl, TV.Pass2VideoUrl ")
        MyStringBuilder.Append(" From [LiveWebScoreboard].[dbo].[TrickPass] TP  ")
        MyStringBuilder.Append(" Left Join(Select distinct SkierName, SanctionID, MemberID from LiveWebScoreboard.dbo.TourReg where sanctionID = '" & sSanctionID & "') as TR  ")
        MyStringBuilder.Append("  On TR.sanctionID = TP.SanctionID And TR.MemberID = TP.MemberID  ")
        MyStringBuilder.Append(" Left Join [LiveWebScoreboard].[dbo].TrickScore TS on TS.sanctionID = TP.SanctionID And TS.MemberID = TP.MemberID And TS.[round] = TP.[Round]  ")
        MyStringBuilder.Append(" LEFT OUTER JOIN [LiveWebScoreboard].dbo.TrickVideo AS TV ON TV.SanctionId = TR.SanctionId AND TV.MemberId = TR.MemberId ")
        MyStringBuilder.Append(" And TV.AgeGroup = TP.AgeGroup AND TV.Round = TP.Round ")

        MyStringBuilder.Append(" Where TP.SanctionId = '" & sSanctionID & "' and TP.MemberID = '" & sMemberID & "' ")
        MyStringBuilder.Append(" order by Round asc, Pass Asc, seq asc  ")
        sSQL = MyStringBuilder.ToString()

        '  sSQL = "Select * from vTrickResults  Where TP.SanctionId = '" & sSanctionID & "' and TP.MemberID = '" & sMemberID & "' order by Round asc, PassNum Asc  "

        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: RecapTrick could not get connection string."
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim sTableMstr As String = "<Table class=""table"">" 'Table row holding first round information
        Dim sTableRound As String = ""
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
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            sSkis = CStr(MyDataReader.Item("Skis"))
                            sCode = CStr(MyDataReader.Item("Code"))
                            sPass = CStr(MyDataReader.Item("Pass"))
                            sResults = CStr(MyDataReader.Item("Results"))
                            sTrkScore = CStr(MyDataReader.Item("TrkScore"))
                            If IsDBNull(MyDataReader.Item("FScore")) Then
                                sRoundScore = "N/A"
                            Else
                                sRoundScore = CStr(MyDataReader.Item("FScore"))
                            End If

                            If IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = "N/A"
                            Else
                                sEventClass = CStr(MyDataReader.Item("EventClass"))
                            End If

                            If IsDBNull(MyDataReader.Item("P1Score")) Then
                                sP1Score = "N/A"
                            Else
                                sP1Score = CStr(MyDataReader.Item("P1Score"))
                            End If

                            If IsDBNull(MyDataReader.Item("P2Score")) Then
                                sP2Score = "N/A"
                            Else
                                sP2Score = CStr(MyDataReader.Item("P2Score"))
                            End If

                            sRound = CStr(MyDataReader.Item("Round"))

                            If IsDBNull(MyDataReader.Item("Pass1VideoURL")) Then
                                sPass1URL = "Round " & sRound & " Pass 1 Video Not available."
                            Else  'Have a Url
                                sPass1URL = MyDataReader.Item("Pass1VideoURL")
                            End If
                            If IsDBNull(MyDataReader.Item("Pass2VideoURL")) Then
                                sPass2URL = "Round " & sRound & " Pass 2 Video Not available."
                            Else
                                sPass2URL = MyDataReader.Item("Pass2VideoURL")
                            End If


                            If sTmpRound = 0 Then 'This is the first round. set up row in master table and round Table that holds both passes
                                sTmpRound = sRound
                                sTableMstr += "<tr><td>" 'row that will hold Rounds tables
                                sTableRound = "<Table class=""table"">"
                                sTableRound += "<tr><td Class=""table-primary""><b>Trick Recap: " & sSkierName & " &nbsp;" & sAgeGroup & " Class " & sEventClass & "</b></td><td class=""table-primary""><span class=""bg-danger text-white"" >  UNOFFICIAL </span><b> Round: " & sRound & "&nbsp;Score " & sRoundScore & "</b></td></tr>"
                                sTableRound += "<tr><td>" 'row containing left and right tables
                                sPassTable = "<Table Class=""table table-warning"">"  'Pass 1 table
                                sPassTable += "<thead><tr Class=""table-info""><th colspan=""4""> Pass: " & sPass & "&nbsp; Score " & sP1Score & "</th></tr>"
                                sPassTable += "<tr><th>Skis</th><th>Code</th><th>Results</th><th>Score</th></tr></head>"
                                'reset for new round
                                sTmpPass = sPass
                                sTmpPass1Url = sPass1URL
                                sTmpPass2URL = sPass2URL
                                sP1SubTotal = 0
                                sP2SubTotal = 0
                                sTotalScore = 0
                            End If
                            If sRound = sTmpRound Then 'Beginning pass 1
                                If sTmpPass = sPass Then  'still in same round and pass
                                    'display next trick.  subtotal points?
                                    sPassTable += "<tr bncolor=""#FFB6C1""><td>" & sSkis & "</td><td>" & sCode & "</td><td>" & sResults & "</td><td>" & sTrkScore & "</td></tr>"
                                End If
                                If sTmpPass <> sPass Then  'Could use if sPass = 2
                                    'close out pass 1 
                                    sPassTable += "<tr><td colspan=""4"">End Pass " & sTmpPass & "</td></tr>"
                                    sPassTable += "</table>"
                                    sTableRound += sPassTable & "</td><td>" 'close column 1 and start second column in Round table prepare for pass 2
                                    sPassTable = ""
                                    ' set up pass 2
                                    sTmpPass = sPass  'should be 2
                                    sPassTable = "<Table Class=""table table-warning"">"
                                    sPassTable += "<thead><tr class=""table-info""><th colspan=""4"">  Pass: " & sPass & "&nbsp;Score " & sP2Score & "</th></tr>"
                                    sPassTable += "<tr><th>Skis</th><th>Code</th><th>Results</th><th>Score</th></tr></thead>"
                                    'display next trick. subtotal points?
                                    sPassTable += "<tr bncolor=""#FFB6C1""><td>" & sSkis & "</td><td>" & sCode & "</td><td>" & sResults & "</td><td>" & sTrkScore & "</td></tr>"
                                End If
                            End If
                            ' COULD USE ELSE
                            If sRound <> sTmpRound Then
                                'starting a new round.  Close out previous round
                                sTotalScore += sP2SubTotal
                                sPassTable += "<tr><td colspan=""4"">End Pass " & sTmpPass & "</td></tr>"
                                sPassTable += "</table>"  ' close PassTable
                                sTableRound += sPassTable & "</td></tr>" 'close column 2
                                'Add in the video links if available
                                sTableRound += "<tr><td colspan=""2"">" & sTmpPass1Url & "</td></tr>"
                                sTableRound += "<tr><td colspan=""2"">" & sTmpPass2URL & "</td></tr>"
                                sTableRound += "</table>"
                                'Add round data and close out First Round row in master table
                                sTableMstr += sTableRound & "</td><td>"
                                sTableMstr += "<tr><td>"
                                'reset for new round
                                sTmpRound = sRound
                                sTmpPass1Url = sPass1URL
                                sTmpPass2URL = sPass2URL
                                sTableRound = "<Table class=""table"">"
                                sTableRound += "<tr><td Class=""table-primary""><b>Trick Recap: " & sSkierName & " &nbsp;" & sAgeGroup & " Class " & sEventClass & "</b></td><td class=""table-primary""><span class=""bg-danger text-white"" >  UNOFFICIAL </span><b> Round: " & sRound & "&nbsp;Score " & sRoundScore & "</b></td></tr>"
                                sTableRound += "<tr><td>" 'row containing left and right tables
                                sPassTable = "<Table Class=""table table-warning"">"  'Pass 1 table
                                sPassTable += "<thead><tr Class=""table-info""><th colspan=""4""> Pass: " & sPass & "&nbsp; Score " & sP1Score & "</th></tr>"
                                sPassTable += "<tr><th>Skis</th><th>Code</th><th>Results</th><th>Score</th></tr></head>"
                                'reset for new round
                                sTmpPass = sPass  'pass number of next round
                                sP1SubTotal = 0
                                sP2SubTotal = 0
                                sTotalScore = 0
                                'Add first row of new round
                                sPassTable += "<tr bncolor=""#FFB6C1""><td>" & sSkis & "</td><td>" & sCode & "</td><td>" & sResults & "</td><td>" & sTrkScore & "</td></tr>"
                            End If

                        Loop
                    Else
                        sTableMstr += "<tr class=""table-warning"" colspan=""2""><td>No Trick results Found For selected skier.</td></tr>"  'close row here and table in Finally
                    End If 'end of has rows
                End Using

                Select Case sPass ' have processed all records.  Close table - hits here if pass ends with no additional rounds present

                    Case "1" 'if only one pass 

                        sTotalScore += sP2SubTotal
                        sPassTable += "<tr><td colspan=""4"">End Pass " & sTmpPass & "</td></tr>"
                        sPassTable += "</table>"  ' close PassTable
                        sTableRound += sPassTable & "</td><td></td></tr>" 'close column 2 empty
                        'Add in the video links if available
                        sTableRound += "<tr><td colspan=""2"">" & sPass1URL & "</td></tr>"
                        sTableRound += "</table>"
                        sTableMstr += sTableRound & "</td></tr>"
                    Case "2" 'if two passses but no next round present.
                        sPassTable += "<tr><td colspan=""4"">End Pass " & sTmpPass & "</td></tr>"
                        sPassTable += "</table>"
                        sTableRound += sPassTable & "</td></tr>" 'close row containing both pass tables
                        sTableRound += "<tr><td colspan=""2"">" & sPass1URL & "</td></tr>"
                        sTableRound += "<tr><td colspan=""2"">" & sPass2URL & "</td></tr>"
                        sTableRound += "</table>"
                        sTableMstr += sTableRound & "</td></tr>"
                    Case "0" ' no records found = dp nothing
                End Select

            Catch ex As Exception
                sMsg += "Error Can't retrieve Trick Scores. "
                sErrDetails = "error at RecapTrick " & ex.Message & " " & ex.StackTrace & "<br>SQL= " & sSQL
            Finally
                sTableMstr += "</td></tr></Table>" 'Close round container
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sTableMstr
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
        Dim sEventClass As String = ""
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
        Dim sResults As String = ""
        Dim sHighlightRerideReason As String = ""

        ' 3 event scores in available data.
        ' 23S108, James Bryans,             OM, 000107150,  at least 1 round overall
        ' 23S108, Tristan Duplan-Fribourg, JM, 000181068, at least 1 round overall
        sSQL = "Select JR.SanctionID, JR.AgeGroup, JR.[round], JR.ScoreFeet, JR.ScoreMeters, JR.PassNum, "
        sSQL += " JR.Results, Jr.BoatSpeed, Jr.RampHeight, Jr.ScoreProt, JR.Reride, Jr.RerideReason, JS.EventClass "
        sSQL += " From JumpRecap JR "
        sSQL += " left join jumpscore JS on JR.Sanctionid = JS.SanctionID and JR.MemberID = JS.MemberID and JR.[Round] = JS.[Round]"
        sSQL += " Where JR.SanctionId ='" & sSanctionID & "' AND JR.MemberId='" & sMemberID & "' And JR.AgeGroup='" & sAgeGroup & "' "
        sSQL += " Order By JR.[round], JR.PassNum ASC , ScoreFeet "

        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: JumpRecap could not get connection string. "
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        sText = "<Table class=""table table-striped border-1 "">"
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

                            If IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = "N/A"
                            Else
                                sEventClass = CStr(MyDataReader.Item("EventClass"))
                            End If

                            If IsDBNull(MyDataReader.Item("Results")) Then
                                sResults = "N/A"
                            Else
                                sResults = MyDataReader.Item("Results")
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
                            If sTmpRound <> sRound Then
                                sText += "<tr class=""table-primary""><td colspan=""7""><span class=""bg-danger text-white"">  UNOFFICIAL </span> <b>Round " & sRound & " Distance for " & sSkierName & " " & sAgeGroup & "  Class " & sEventClass & " </b></td></tr>"
                                sText += "<tr><th>Result<th>Pass</th><th> Ft  Mtr </th><th>Speed</th><th>RmpHt</th><th>Reride</th><th>Score Protect</th></tr>"
                                sTmpRound = sRound
                            End If
                            sText += "<tr><td>" & sResults & "</td><td>" & sPass & "</td><td>" & sFeet & "&nbsp;" & sMeters & "</td><td>" & sBSpeed & "</td><td>" & sRmpHt & "</td><td>" & sReride & "</td><td>" & sProtected & "</td></tr>"
                            If sReride = "Y" Then
                                sHighlightRerideReason = " class = ""table-danger"" "
                                sText += "<tr><td " & sHighlightRerideReason & " colspan=""7"">Pass# " & sPass & " " & sRerideReason & "</td></tr>"
                                sHighlightRerideReason = ""
                            End If

                        Loop
                    Else
                        sText += "<tr Class=""table-warning""><td colspan=""6"">No Jump results Found For selected skier.</td></tr>"
                    End If 'end of has rows
                End Using

            Catch ex As Exception
                sMsg += "Error Can 't retrieve Jump Recap. "
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
        Dim sTitleLine As String = ""
        Dim sDataLine As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID
        Dim sAgeGroup As String = AgeGroup
        Dim sSkierName As String = SkierName
        '       Replace(sSkierName, "'", "''")
        '        Replace(sSkierName, ",", ",,")
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
        Dim sNotOverall As String = ""

        ' 3 event scores in available data.
        ' 23S108, James Bryans,             OM, 000107150,  at least 1 round overall
        ' 23S108, Tristan Duplan-Fribourg, JM, 000181068, at least 1 round overall

        'NOTE:  IS AGEGROUP REQUIRED?  MUST ALL PERFORMANCES BE IN THE SAME AGE GROUP?
        'Can modification of this be used to put overall in with other events by division?
        Dim sSSB As New StringBuilder
        sSSB.Append("SELECT TR.SkierName, TR.MemberId, TR.AgeGroup, 'Overall' as Event ")
        sSSB.Append(", COALESCE(SS.NopsScore,0) + COALESCE(TS.NopsScore,0) + COALESCE(JS.NopsScore,0) as OverallScore ")
        sSSB.Append(", SS.NopsScore as SlalomNopsScore, TS.NopsScore as TrickNopsScore, JS.NopsScore as JumpNopsScore ")
        sSSB.Append(", COALESCE(SS.Round,COALESCE(TS.Round,COALESCE(JS.Round,0))) as Round ")
        sSSB.Append(", SS.Score as SlalomScore, FinalPassScore, FinalSpeedMph, FinalSpeedKph, FinalLen, FinalLenOff ")
        sSSB.Append(" , TS.Score as TrickScore, ScorePass1, ScorePass2 ")
        sSSB.Append(", JS.ScoreFeet, JS.ScoreMeters  ")
        sSSB.Append("From LiveWebScoreBoard.dbo.TourReg TR 	")
        sSSB.Append("Left OUTER JOIN LiveWebScoreBoard.dbo.SlalomScore SS on SS.MemberId=TR.MemberId And SS.SanctionId=TR.SanctionId And SS.AgeGroup = TR.AgeGroup ")
        sSSB.Append("Left OUTER JOIN LiveWebScoreBoard.dbo.TrickScore TS on TS.MemberId=TR.MemberId And TS.SanctionId=TR.SanctionId And TS.AgeGroup = TR.AgeGroup And TS.Round = SS.Round ")
        sSSB.Append(" Left OUTER JOIN LiveWebScoreBoard.dbo.JumpScore JS on JS.MemberId=TR.MemberId And JS.SanctionId=TR.SanctionId And JS.AgeGroup = TR.AgeGroup And JS.Round = SS.Round ")
        sSSB.Append("WHERE TR.SanctionID ='" & sSanctionID & "' AND TR.MemberId='" & sMemberID & "' And TR.AgeGroup='" & sAgeGroup & "'  ")
        sSSB.Append(" And COALESCE(SS.Round, COALESCE(TS.Round, COALESCE(JS.Round, 0))) > 0 ")
        sSSB.Append(" Order By TR.AgeGroup, TR.SkierName, OverallScore DESC ")
        sSQL = sSSB.ToString()
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: RecapOverall could not get connection string. "
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        sText = "<Table class=""table table-striped border-1 "">"
        sText += "<thead><tr Class=""table-primary""><td colspan=""6""><span class=""bg-danger text-white"">UNOFFICIAL</span><b>Overall Scores for " & sSkierName & "</b></td></tr>"
        sText += "<tr><th>Age Group</th><th>Rnd</th><th>Overall Score</th><th>Slalom Nops</th><th>Trick Nops</th><th>Jump Nops</th></tr></thead>"
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
                            If sSlalomNopsScore = "N/A" Or sTrickNopsScore = "N/A" Or sJumpNopsScore = "N/A" Then
                                sNotOverall += "<tr><td colspan=""6"">No overall results in Round " & sRound & ".</td></tr>"
                            Else
                                sDataLine += "<tr><td>" & sAgeGroup & "</td><td>" & sRound & "</td><td>" & sOverallScore & "</td><td>" & sSlalomNopsScore & "</td><td>" & sTrickNopsScore & "</td><td>" & sJumpNopsScore & "</td>"
                            End If '                            
                        Loop
                        If sDataLine <> "" Then
                            sText += sDataLine
                        Else
                            sText = "<table class=""table""><tr Class=""table-warning""><td colspan=""6"">No Overall results Found For selected skier.</td></tr>"
                        End If
                    Else
                        sText += "<tr Class=""table-warning""><td colspan=""6"">No Overall results Found For selected skier.</td></tr>"
                    End If 'end of has rows
                End Using

            Catch ex As Exception
                sMsg += "Error Can't retrieve Overall Scores. " 'SQL= " & SQL & "<br>IndivJumpResults Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br>"
                sErrDetails = ex.Message & " " & ex.StackTrace & "<br>error at RecapOverall:  SQL= " & sSQL
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

    Friend Function GetTeamScores(ByVal SanctionID)
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sAgeGroup As String = ""
        Dim sTeamCode As String = ""
        Dim sName As String = ""
        Dim sOverallPlcmtTeam As String = ""
        Dim sOverallScoreTeam As String = ""
        Dim sSlalomPlcmtTeam As String = ""
        Dim sSlalomScoreTeam As String = ""
        Dim sTrickPlcmtTeam As String = ""
        Dim sTrickScoreTeam As String = ""
        Dim sJumpPlcmtTeam As String = ""
        Dim sJumpScoreTeam As String = ""
        Dim sTmpAgeGroup As String = ""
        Dim sText As String = ""

        Dim sSQL As String = "Select S.SanctionId, S.TeamCode, S.AgeGroup, Name, ReportFormat 
		 , S.OverallPlcmt AS OverallPlcmtTeam, S.SlalomPlcmt AS SlalomPlcmtTeam, S.TrickPlcmt AS TrickPlcmtTeam, S.JumpPlcmt AS JumpPlcmtTeam 
		 , S.OverallScore AS OverallScoreTeam, S.SlalomScore AS SlalomScoreTeam, S.TrickScore AS TrickScoreTeam, S.JumpScore AS JumpScoreTeam 
 From TeamScore S 
 Where S.SanctionId = '" & sSanctionID & "' 
 Order by S.AgeGroup, S.OverallPlcmt "

        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: GetTeamScores could not get connection string." &
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim sEventClass As String = ""
        Dim sLine As String = ""
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        sText = "<Table class=""table table-striped border-1 "">"
        sText += "<thead><tr Class=""table-primary""><td colspan=""9""><span class=""bg-danger text-white"">UNOFFICIAL</span><b>Team Scores </b></td></tr>"
        sText += "<tr><th>Team Code</th><th>Team Name</th><th>Overall Place/Score<th>Slalom Place / Score</th><th>Trick Place / Score</th><th>Jump Place / Score</th></tr></thead>"
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
                            If IsDBNull(MyDataReader.Item("AgeGroup")) Then
                                sAgeGroup = ""
                            Else
                                sAgeGroup = CStr(MyDataReader.Item("AgeGroup"))
                            End If
                            If IsDBNull(MyDataReader.Item("TeamCode")) Then
                                sTeamCode = ""
                            Else
                                sTeamCode = CStr(MyDataReader.Item("TeamCode"))
                            End If
                            If IsDBNull(MyDataReader.Item("Name")) Then
                                sName = ""
                            Else
                                sName = CStr(MyDataReader.Item("Name"))
                            End If
                            If IsDBNull(MyDataReader.Item("OverallPlcmtTeam")) Then
                                sOverallPlcmtTeam = ""
                            Else
                                sOverallPlcmtTeam = CStr(MyDataReader.Item("OverallPlcmtTeam"))
                            End If
                            If IsDBNull(MyDataReader.Item("OverallScoreTeam")) Then
                                sOverallScoreTeam = ""
                            Else
                                sOverallScoreTeam = CStr(MyDataReader.Item("OverallScoreTeam"))
                            End If
                            If IsDBNull(MyDataReader.Item("SlalomPlcmtTeam")) Then
                                sSlalomPlcmtTeam = ""
                            Else
                                sSlalomPlcmtTeam = CStr(MyDataReader.Item("SlalomPlcmtTeam"))
                            End If
                            If IsDBNull(MyDataReader.Item("SlalomScoreTeam")) Then
                                sSlalomScoreTeam = ""
                            Else
                                sSlalomScoreTeam = CStr(MyDataReader.Item("SlalomScoreTeam"))
                            End If
                            If IsDBNull(MyDataReader.Item("TrickPlcmtTeam")) Then
                                sTrickPlcmtTeam = ""
                            Else
                                sTrickPlcmtTeam = CStr(MyDataReader.Item("TrickPlcmtTeam"))
                            End If
                            If IsDBNull(MyDataReader.Item("TrickScoreTeam")) Then
                                sTrickScoreTeam = ""
                            Else
                                sTrickScoreTeam = CStr(MyDataReader.Item("TrickScoreTeam"))
                            End If

                            If IsDBNull(MyDataReader.Item("JumpPlcmtTeam")) Then
                                sJumpPlcmtTeam = ""
                            Else
                                sJumpPlcmtTeam = CStr(MyDataReader.Item("JumpPlcmtTeam"))
                            End If
                            If IsDBNull(MyDataReader.Item("JumpScoreTeam")) Then
                                sJumpScoreTeam = ""
                            Else
                                sJumpScoreTeam = CStr(MyDataReader.Item("JumpScoreTeam"))
                            End If


                            If sTmpAgeGroup <> sAgeGroup Then 'New AgeGroup
                                sText += "<tr><td class=""table-info"" colspan="" 11"">" & sAgeGroup & "</td></tr>"
                                sTmpAgeGroup = sAgeGroup
                            End If
                            sText += "<tr><td>" & sTeamCode & "</td><td>" & sName & "</td><td><b>" & sOverallPlcmtTeam & "</b> / " & sOverallScoreTeam & "</td><td><b>"
                            sText += sSlalomPlcmtTeam & "</b> / " & sSlalomScoreTeam & "</td><td><b>" & sTrickPlcmtTeam & "</b> / " & sTrickScoreTeam & "</td><td><b>"
                            sText += sJumpPlcmtTeam & "</b> / " & sJumpScoreTeam & "</td><td></tr>"
                            '                            
                        Loop

                    Else
                        sText += "<tr Class=""table-warning""><td colspan=""9"">No Team Scores Found</td></tr>"
                    End If 'end of has rows
                End Using

            Catch ex As Exception
                sMsg += "Error at GetTeamScores. "
                sErrDetails = "<br />" & ex.Message & " " & ex.StackTrace & "<br>SQL= " & sSQL
            Finally
                sText += "</table>"
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sText.ToString

    End Function
    Friend Function PopulateTeams(ByVal SanctionID As String, ByRef ddl_Teams As DropDownList) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sSQL As String = "select distinct TeamCode from EventReg where sanctionID = '" & sSanctionID & "'"
        Dim sTeamCode As String = ""
        Dim sConn As String = ""
        Try
            If ConfigurationManager.ConnectionStrings("S_UseLocal_Scoreboard").ConnectionString = 0 Then
                sConn = ConfigurationManager.ConnectionStrings("LWS_Prod").ConnectionString
            Else
                sConn = ConfigurationManager.ConnectionStrings("Local_SS_WP23").ConnectionString
            End If
        Catch ex As Exception
            sMsg = "Error: GetTeamScores could not get connection string. "
            sErrDetails = ex.Message & "  " & ex.StackTrace
            Return sMsg
            Exit Function
        End Try
        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        Dim MyDataReader As OleDb.OleDbDataReader = Nothing
        Dim sCkRows As Boolean = False
        Using Cnnt
            Try
                With ddl_Teams
                    .Items.Clear()
                    Using cmdRead
                        cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                        cmdRead.CommandText = sSQL
                        cmdRead.Connection.Open()
                        MyDataReader = cmdRead.ExecuteReader
                        If MyDataReader.HasRows = True Then
                            Do While MyDataReader.Read()
                                If IsDBNull(MyDataReader.Item("TeamCode")) Then
                                    sTeamCode = "N/A"
                                Else
                                    sTeamCode = CStr(MyDataReader.Item("TeamCode"))
                                End If

                                .Items.Add(New ListItem(sTeamCode, sTeamCode))
                            Loop
                        Else
                            sMsg = " No Teams Found for selected tournament. "
                            .Items.Clear()
                            .Items.Add(New ListItem(sMsg, "0"))
                        End If 'end of has rows
                    End Using
                End With
            Catch ex As Exception
                sMsg += "Error: Can't retrieve Teams. "
                sErrDetails = " At Populate Teams " & ex.Message & " " & ex.StackTrace & " <br />SQL= " & sSQL
                ddl_Teams.Items.Clear()
                ddl_Teams.Items.Add(New ListItem(sMsg, "0"))
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
        End If
        Return "Success"
    End Function
    Public Function GetScoresXRunOrder(ByVal SanctionID As String, ByVal selEvent As String, ByVal selDV As String, ByVal selRnd As Char) As String
        'Use ModDataAccess2.ScoresXRunOrderHoriz
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sPREventCode As String = selEvent
        Dim sDV As String = selDV
        Dim sSelRnd As Char = selRnd
        Dim sList As New StringBuilder
        Dim sFirstLoop As Boolean = True
        Dim sName As String = ""
        Dim sSkrDv As String = ""
        Dim sCurRound As String = "" 'Allows for All Rounds

        Dim sEventClass As String = ""
        Dim sRankingScore As String = ""
        Dim sStatus As String = ""
        Dim sNopsScore As String = ""
        Dim sEventScore As String = ""
        Dim sScoreRunoff As String = ""
        Dim sEvScoreDesc As String = ""
        Dim sInsertDate As String = ""
        Dim sLastUpdateDate As String = ""
        Dim sBuoys As String = ""
        Dim sPass1VideoUrl As String = ""
        Dim sPass2VideoUrl As String = ""
        Dim sScoreFeet As String = ""
        Dim sScoreMeters As String = ""
        Dim sSlalomHeader As String = "<thead><tr class=""table-primary"" ><th colspan=""5""> Slalom Results &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   <span class=""bg-danger text-white"" >! UNOFFICIAL !</span></th></tr></thead>"
        Dim sTrickHeader As String = "<thead><tr class=""table-primary""><th colspan=""5""> Trick Results &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   <span class=""bg-danger text-white"" >! UNOFFICIAL !</span></th></tr></thead>"
        Dim sJumpHeader As String = "<thead><tr class=""table-primary""><th colspan=""5""> Jump Results &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   <span class=""bg-danger text-white"" >! UNOFFICIAL !</span></th></tr></thead>"

        Dim sSlalomDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>DV</th><th>NOPS</th><th>Buoys</th><th>Detail</th></tr></thead>"
        Dim sTrickDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>DV</th><th>NOPS</th><th>Points</th><th>Detail</th></tr></thead>"
        Dim sJumpDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>DV</th><th>NOPS</th><th>Feet / Meters</th></tr></thead>"


        Dim sSql As String = ""
        Select Case selEvent
            Case "S"
                sPREventCode = "Slalom"
                sSql = "PrSlalomScoresByRunOrder"
                sList.Append("<Table Class=""table  border-1 "">" & sSlalomHeader & sSlalomDVHeader)

            Case "T"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresByRunOrder"
                sList.Append(" <Table Class=""table  border-1 "">" & sTrickHeader & sTrickDVHeader)

            Case "J"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresByRunOrder"
                sList.Append("<Table Class=""table  border-1 "">" & sJumpHeader & sJumpDVHeader)

                '            Case "Overall"
                '                sPREventCode = "Overall"
            Case Else  'Load all by default
                sPREventCode = "ALL"
        End Select
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
        cmdRead.Parameters("@InRnd").Value = sSelRnd
        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sDV
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input
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
                            sName = CStr(MyDataReader.Item("SkierName"))
                            If Not IsDBNull(MyDataReader.Item("DiV")) Then
                                sSkrDv = CStr(MyDataReader.Item("DiV"))
                            Else
                                sSkrDv = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = MyDataReader.Item("EventClass")
                            Else
                                sEventClass = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("RankingScore")) Then
                                sRankingScore = CStr(MyDataReader.Item("RankingScore"))
                            Else
                                sRankingScore = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("Status")) Then
                                sStatus = MyDataReader.Item("Status")
                            Else
                                sStatus = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("NopsScore")) Then
                                sNopsScore = CStr(MyDataReader.Item("NopsScore"))
                            Else
                                sNopsScore = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                                sEventScore = CStr(MyDataReader.Item("EventScore"))
                            Else
                                sEventScore = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("ScoreRunoff")) Then
                                sScoreRunoff = MyDataReader.Item("ScoreRunoff")
                            Else
                                sScoreRunoff = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEvScoreDesc = MyDataReader.Item("EventScoreDesc")
                            Else
                                sEvScoreDesc = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("InsertDate")) Then
                                sInsertDate = CStr(MyDataReader.Item("InsertDate"))
                            Else
                                sInsertDate = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("LastUpdateDate")) Then
                                sLastUpdateDate = CStr(MyDataReader.Item("LastUpdateDate"))
                            Else
                                sLastUpdateDate = ""
                            End If
                            Select Case sPREventCode
                                Case "Slalom"
                                    If Not IsDBNull(MyDataReader.Item("Buoys")) Then
                                        sBuoys = CStr(MyDataReader.Item("Buoys"))
                                    Else
                                        sBuoys = ""
                                    End If
                                    sList.Append("<td>" & sName & "</td><td>" & sSkrDv & "</td><td>" & sNopsScore & "</td><td>" & sEventScore & "</td><td>" & sEvScoreDesc & " <span class=""bg-danger text-white"" >" & sScoreRunoff & "</span></td></tr>")

                                Case "Trick"
                                    If Not IsDBNull(MyDataReader.Item("Pass1VideoUrl")) Then
                                        sPass1VideoUrl = CStr(MyDataReader.Item("Pass1VideoUrl"))
                                    Else
                                        sPass1VideoUrl = ""
                                    End If
                                    If Not IsDBNull(MyDataReader.Item("Pass2VideoUrl")) Then
                                        sPass2VideoUrl = CStr(MyDataReader.Item("Pass2VideoUrl"))
                                    Else
                                        sPass2VideoUrl = ""
                                    End If

                                    sList.Append("<td>" & sName & "</td><td>" & sSkrDv & "</td><td>" & sNopsScore & "</td><td>" & sEventScore & "</td><td>" & sEvScoreDesc & " <span class=""bg-danger text-white"" >" & sScoreRunoff & "</span></td></tr>")
                                Case "Jump"
                                    If Not IsDBNull(MyDataReader.Item("ScoreFeet")) Then
                                        sScoreFeet = CStr(MyDataReader.Item("ScoreFeet"))
                                    Else
                                        sScoreFeet = ""
                                    End If
                                    If Not IsDBNull(MyDataReader.Item("ScoreMeters")) Then
                                        sScoreFeet = CStr(MyDataReader.Item("ScoreMeters"))
                                    Else
                                        sScoreMeters = ""
                                    End If
                                    sList.Append("<td>" & sName & "</td><td>" & sSkrDv & "</td><td>" & sNopsScore & "</td><td>" & sScoreFeet & " / " & "</td><td>" & sEvScoreDesc & " <span class=""bg-danger text-white"" >" & sScoreRunoff & "</span></td></tr>")
                            End Select
                        Loop
                        sList.Append("</table>")
                        sMsg = sList.ToString()
                    Else
                        sMsg = " No Entries Found for selected tournament. "
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetRunOrder"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try

        End Using

        Return sMsg
    End Function

    Public Function GetScoresXRunOrder1line(ByVal SanctionID As String, ByVal selEvent As String, ByVal selDV As String, ByVal selRnd As Char) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sPREventCode As String = selEvent
        Dim sDV As String = selDV
        Dim sSelRnd As Char = selRnd
        Dim sList As New StringBuilder
        Dim sFirstLoop As Boolean = True
        Dim sName As String = ""
        Dim sSkrDv As String = ""
        Dim sCurRound As String = "" 'Allows for All Rounds

        Dim sEventClass As String = ""
        Dim sRankingScore As String = ""
        Dim sStatus As String = ""
        Dim sNopsScore As String = ""
        Dim sEventScore As String = ""
        Dim sScoreRunoff As String = ""
        Dim sEvScoreDesc As String = ""
        Dim sInsertDate As String = ""
        Dim sLastUpdateDate As String = ""
        Dim sBuoys As String = ""
        Dim sPass1VideoUrl As String = ""
        Dim sPass2VideoUrl As String = ""
        Dim sScoreFeet As String = ""
        Dim sScoreMeters As String = ""
        Dim sSlalomHeader As String = "<thead><tr class=""table-primary"" ><th colspan=""5""><span class=""bg-danger text-white"" >! UNOFFICIAL !</span>  Slalom Results </th></tr></thead>"
        Dim sTrickHeader As String = "<thead><tr class=""table-primary""><th colspan=""5""> Trick Results &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   <span class=""bg-danger text-white"" >! UNOFFICIAL !</span></th></tr></thead>"
        Dim sJumpHeader As String = "<thead><tr class=""table-primary""><th colspan=""5""> Jump Results &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   <span class=""bg-danger text-white"" >! UNOFFICIAL !</span></th></tr></thead>"

        Dim sSlalomDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>DV</th><th>NOPS</th><th>Buoys</th><th>Detail</th></tr></thead>"
        Dim sTrickDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>DV</th><th>NOPS</th><th>Points</th><th>Detail</th></tr></thead>"
        Dim sJumpDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>DV</th><th>NOPS</th><th>Feet / Meters</th></tr></thead>"


        Dim sSql As String = ""
        Select Case selEvent
            Case "S"
                sPREventCode = "Slalom"
                sSql = "PrSlalomScoresByRunOrder"
                sList.Append("<Table Class=""table  border-1 "">" & sSlalomHeader & sSlalomDVHeader)

            Case "T"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresByRunOrder"
                sList.Append(" <Table Class=""table  border-1 "">" & sTrickHeader & sTrickDVHeader)

            Case "J"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresByRunOrder"
                sList.Append("<Table Class=""table  border-1 "">" & sJumpHeader & sJumpDVHeader)

                '            Case "Overall"
                '                sPREventCode = "Overall"
            Case Else  'Load all by default
                sPREventCode = "ALL"
        End Select
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
        cmdRead.Parameters("@InRnd").Value = sSelRnd
        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sDV
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input
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
                            sName = CStr(MyDataReader.Item("SkierName"))
                            If Not IsDBNull(MyDataReader.Item("DiV")) Then
                                sSkrDv = CStr(MyDataReader.Item("DiV"))
                            Else
                                sSkrDv = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = MyDataReader.Item("EventClass")
                            Else
                                sEventClass = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("RankingScore")) Then
                                sRankingScore = CStr(MyDataReader.Item("RankingScore"))
                            Else
                                sRankingScore = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("Status")) Then
                                sStatus = MyDataReader.Item("Status")
                            Else
                                sStatus = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("NopsScore")) Then
                                sNopsScore = CStr(MyDataReader.Item("NopsScore"))
                            Else
                                sNopsScore = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                                sEventScore = CStr(MyDataReader.Item("EventScore"))
                            Else
                                sEventScore = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("ScoreRunoff")) Then
                                sScoreRunoff = MyDataReader.Item("ScoreRunoff")
                            Else
                                sScoreRunoff = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEvScoreDesc = MyDataReader.Item("EventScoreDesc")
                            Else
                                sEvScoreDesc = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("InsertDate")) Then
                                sInsertDate = CStr(MyDataReader.Item("InsertDate"))
                            Else
                                sInsertDate = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("LastUpdateDate")) Then
                                sLastUpdateDate = CStr(MyDataReader.Item("LastUpdateDate"))
                            Else
                                sLastUpdateDate = ""
                            End If
                            Select Case sPREventCode
                                Case "Slalom"
                                    If Not IsDBNull(MyDataReader.Item("Buoys")) Then
                                        sBuoys = CStr(MyDataReader.Item("Buoys"))
                                    Else
                                        sBuoys = ""
                                    End If
                                    sList.Append("<td>" & sName & "</td><td>" & sSkrDv & "</td><td>" & sNopsScore & "</td><td>" & sEventScore & "</td><td>" & sEvScoreDesc & " <span class=""bg-danger text-white"" >" & sScoreRunoff & "</span></td></tr>")

                                Case "Trick"
                                    If Not IsDBNull(MyDataReader.Item("Pass1VideoUrl")) Then
                                        sPass1VideoUrl = CStr(MyDataReader.Item("Pass1VideoUrl"))
                                    Else
                                        sPass1VideoUrl = ""
                                    End If
                                    If Not IsDBNull(MyDataReader.Item("Pass2VideoUrl")) Then
                                        sPass2VideoUrl = CStr(MyDataReader.Item("Pass2VideoUrl"))
                                    Else
                                        sPass2VideoUrl = ""
                                    End If

                                    sList.Append("<td>" & sName & "</td><td>" & sSkrDv & "</td><td>" & sNopsScore & "</td><td>" & sEventScore & "</td><td>" & sEvScoreDesc & " <span class=""bg-danger text-white"" >" & sScoreRunoff & "</span></td></tr>")
                                Case "Jump"
                                    If Not IsDBNull(MyDataReader.Item("ScoreFeet")) Then
                                        sScoreFeet = CStr(MyDataReader.Item("ScoreFeet"))
                                    Else
                                        sScoreFeet = ""
                                    End If
                                    If Not IsDBNull(MyDataReader.Item("ScoreMeters")) Then
                                        sScoreFeet = CStr(MyDataReader.Item("ScoreMeters"))
                                    Else
                                        sScoreMeters = ""
                                    End If
                                    sList.Append("<td>" & sName & "</td><td>" & sSkrDv & "</td><td>" & sNopsScore & "</td><td>" & sScoreFeet & " / " & "</td><td>" & sEvScoreDesc & " <span class=""bg-danger text-white"" >" & sScoreRunoff & "</span></td></tr>")
                            End Select
                        Loop
                        sList.Append("</table>")
                        sMsg = sList.ToString()
                    Else
                        sMsg = " No Entries Found for selected tournament. "
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetRunOrder"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try

        End Using

        Return sMsg
    End Function
    Public Function GetNamesXRunOrder(ByVal SanctionID As String, ByVal selEvent As String, ByVal selDV As String, ByVal selRnd As Char) As String
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sPREventCode As String = selEvent
        Dim sDV As String = selDV
        Dim sSelRnd As Char = selRnd
        Dim sList As New StringBuilder
        Dim sFirstLoop As Boolean = True
        Dim sName As String = ""
        Dim sSkrDv As String = ""
        Dim sCurRound As String = "" 'Allows for All Rounds

        Dim sEventClass As String = ""
        Dim sRankingScore As String = ""
        Dim sStatus As String = ""
        Dim sNopsScore As String = ""
        Dim sEventScore As String = ""
        Dim sScoreRunoff As String = ""
        Dim sEvScoreDesc As String = ""
        Dim sInsertDate As String = ""
        Dim sLastUpdateDate As String = ""
        Dim sBuoys As String = ""
        Dim sPass1VideoUrl As String = ""
        Dim sPass2VideoUrl As String = ""
        Dim sScoreFeet As String = ""
        Dim sScoreMeters As String = ""
        Dim sSlalomHeader As String = "<thead><tr class=""table-primary"" ><th colspan=""5"">Slalom Running Order   DV: " & sDV & " Round : " & sCurRound & "</th></tr></thead>"
        Dim sTrickHeader As String = "<thead><tr class=""table-primary""><th colspan=""5"">Trick  Running Order   DV: " & sDV & " Round : " & sCurRound & "</th></tr></thead>"
        Dim sJumpHeader As String = "<thead><tr class=""table-primary""><th colspan=""5"">Jump  Running Order   DV: " & sDV & " Round : " & sCurRound & "</th></tr></thead>"

        '        '        Dim sSlalomDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>DV</th><th>NOPS</th><th>Buoys</th><th>Detail</th></tr></thead>"
        '       '       Dim sTrickDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>DV</th><th>NOPS</th><th>Points</th><th>Detail</th></tr></thead>"
        '       '       Dim sJumpDVHeader As String = "<thead><tr class=""table-warning""><th>Name</th><th>DV</th><th>NOPS</th><th>Feet / Meters</th></tr></thead>"


        Dim sSql As String = ""
        Select Case selEvent
            Case "S"
                sPREventCode = "Slalom"
                sSql = "PrSlalomScoresByRunOrder"
                sList.Append("<Table Class=""table  border-1 "">Run Ord") '& sSlalomHeader) '& sSlalomDVHeader)

            Case "T"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresByRunOrder"
                sList.Append(" <Table Class=""table  border-1 "">Run Ord") '& sTrickHeader) '& sTrickDVHeader)

            Case "J"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresByRunOrder"
                sList.Append("<Table Class=""table  border-1 "">Run Ord") '& sJumpHeader)  ' & sJumpDVHeader)

                '            Case "Overall"
                '                sPREventCode = "Overall"
            Case Else  'Load all by default
                sPREventCode = "ALL"
        End Select
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
        cmdRead.Parameters("@InRnd").Value = sSelRnd
        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sDV
        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input
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
                            sName = CStr(MyDataReader.Item("SkierName"))
                            If Not IsDBNull(MyDataReader.Item("DiV")) Then
                                sSkrDv = CStr(MyDataReader.Item("DiV"))
                            Else
                                sSkrDv = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = MyDataReader.Item("EventClass")
                            Else
                                sEventClass = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("RankingScore")) Then
                                sRankingScore = CStr(MyDataReader.Item("RankingScore"))
                            Else
                                sRankingScore = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("Status")) Then
                                sStatus = MyDataReader.Item("Status")
                            Else
                                sStatus = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("NopsScore")) Then
                                sNopsScore = CStr(MyDataReader.Item("NopsScore"))
                            Else
                                sNopsScore = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                                sEventScore = CStr(MyDataReader.Item("EventScore"))
                            Else
                                sEventScore = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("ScoreRunoff")) Then
                                sScoreRunoff = MyDataReader.Item("ScoreRunoff")
                            Else
                                sScoreRunoff = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEvScoreDesc = MyDataReader.Item("EventScoreDesc")
                            Else
                                sEvScoreDesc = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("InsertDate")) Then
                                sInsertDate = CStr(MyDataReader.Item("InsertDate"))
                            Else
                                sInsertDate = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("LastUpdateDate")) Then
                                sLastUpdateDate = CStr(MyDataReader.Item("LastUpdateDate"))
                            Else
                                sLastUpdateDate = ""
                            End If
                            '                           Select Case sPREventCode
                            '                           Case "Slalom"
                            '                           If Not IsDBNull(MyDataReader.Item("Buoys")) Then
                            '                           sBuoys = CStr(MyDataReader.Item("Buoys"))
                            '                           Else
                            '                           sBuoys = ""
                            '                   End If
                            sList.Append("<td>" & sName & "</td><tr>")  '<td>" & sSkrDv & "</td><td>" & sNopsScore & "</td><td>" & sEventScore & "</td><td>" & sEvScoreDesc & " <span class=""bg-danger text-white"" >" & sScoreRunoff & "</span></td></tr>")

                            '                                Case "Trick"
                            '                                    If Not IsDBNull(MyDataReader.Item("Pass1VideoUrl")) Then
                            '                                        sPass1VideoUrl = CStr(MyDataReader.Item("Pass1VideoUrl"))
                            '                                    Else
                            '                                        sPass1VideoUrl = ""
                            '                                    End If
                            '                                    If Not IsDBNull(MyDataReader.Item("Pass2VideoUrl")) Then
                            '                                        sPass2VideoUrl = CStr(MyDataReader.Item("Pass2VideoUrl"))
                            '                                    Else
                            '                                        sPass2VideoUrl = ""
                            '                                    End If
                            '
                            '                                    sList.Append("<td>" & sName & "</td><td>" & sSkrDv & "</td><td>" & sNopsScore & "</td><td>" & sEventScore & "</td><td>" & sEvScoreDesc & " <span class=""bg-danger text-white"" >" & sScoreRunoff & "</span></td></tr>")
                            '                                Case "Jump"
                            '                                    If Not IsDBNull(MyDataReader.Item("ScoreFeet")) Then
                            '                                        sScoreFeet = CStr(MyDataReader.Item("ScoreFeet"))
                            '                                    Else
                            '                                        sScoreFeet = ""
                            '                                    End If
                            '                                    If Not IsDBNull(MyDataReader.Item("ScoreMeters")) Then
                            '                                        sScoreFeet = CStr(MyDataReader.Item("ScoreMeters"))
                            '                                    Else
                            '                                        sScoreMeters = ""
                            '                                    End If
                            '                                   sList.Append("<td>" & sName & "</td></tr>")  '<td>" & sSkrDv & "</td><td>" & sNopsScore & "</td><td>" & sScoreFeet & " / " & "</td><td>" & sEvScoreDesc & " <span class=""bg-danger text-white"" >" & sScoreRunoff & "</span></td></tr>")
                            '                            End Select
                        Loop
                        sList.Append("</table>")
                        sMsg = sList.ToString()
                    Else
                        sMsg = " No Entries Found for selected tournament. "
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at GetRunOrder"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try

        End Using

        Return sMsg
    End Function


End Module
