Imports System.Data.Common
Imports System.Data.OleDb
Imports System.Security.Cryptography.X509Certificates

Module ModDataAccess3
    Public Function ScoresXRunOrdHoriz(ByVal SanctionID As String, ByVal YearPkd As String, ByVal TName As String, ByVal selEvent As String, ByVal selDv As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNOPS As Int16, ByVal UseTeams As Int16, ByVal FormatCode As String, ByVal DisplayMetric As Int16) As String
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sYrPkd As String = YearPkd
        Dim sPREventCode As String = ""
        Dim sSelEvent As String = selEvent
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
        Dim sTmpEventGroup As String = ""
        Dim sRnd As String = ""
        Dim sRunOrd As New StringBuilder
        Dim sSlalomHeader As String = ""
        Dim sTrickHeader As String = ""
        Dim sJumpHeader As String = ""
        Dim sLine As New StringBuilder
        Dim sRndsSlalomOffered As String = RndsSlalomOffered
        Dim sRndsTrickOffered As String = RndsTrickOffered
        Dim sRndsJumpOffered As String = RndsJumpOffered
        Dim sRndsOffered As String = ""
        Dim sHasVideo As String = ""
        Dim sTVidAvail As String = ""

        Dim sSql As String = ""
        Select Case selEvent
            Case "S"
                sPREventCode = "Slalom"
                sSql = "PrSlalomScoresByRunOrder"
                If sSelRnd = 0 Then
                    sRndsOffered = sRndsSlalomOffered
                    sLine.Append("<Table Class=""table  border-1 "">") '& sSlalomHeader) '& sSlalomDVHeader)
                    sSlalomHeader = "<thead><tr><th class=""table-success"">SLALOM</th><th class=""table-primary""> Div </th>"
                    For i = 1 To RndsSlalomOffered
                        sSlalomHeader += "<th class=""table-primary"">Round " & i & "</th>"
                    Next
                    sLine.Append(sSlalomHeader & "</tr></thead>")
                Else
                    sLine.Append("<Table Class=""table  border-1 "">") '& sSlalomHeader) '& sSlalomDVHeader)
                    sLine.Append("<thead><tr><th class=""table-success"">SLALOM</th><th class=""table-primary""> Div </th><th class=""table-primary"">Score Round " & sSelRnd & "</th></thead>")
                End If
            Case "T"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresByRunOrder"
                If sSelRnd = 0 Then
                    sRndsOffered = sRndsTrickOffered
                    sLine.Append(" <Table Class=""table  border-1 "">") '& sTrickHeader) '& sTrickDVHeader)
                    sTrickHeader = "<thead><tr><th class=""table-success"">TRICK</th><th class=""table-primary""> Div </th>"
                    For i = 1 To RndsTrickOffered
                        sTrickHeader += "<th class=""table-primary"">Round " & i & "</th>"
                    Next
                    sLine.Append(sTrickHeader & "</tr></thead>")
                Else
                    sLine.Append(" <Table Class=""table  border-1 "">") '& sTrickHeader) '& sTrickDVHeader)
                    sLine.Append("<thead><tr><th class=""table-success"">TRICK</th><th class=""table-primary""> Div </th><th class=""table-primary"">Score Round " & sSelRnd & "</th></thead>")
                End If
            Case "J"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresByRunOrder"
                If sSelRnd = 0 Then
                    sRndsOffered = sRndsJumpOffered
                    sLine.Append("<Table Class=""table  border-1 "">") '& sJumpHeader)  ' & sJumpDVHeader)
                    sJumpHeader = "<thead><tr><th class=""table-success"">JUMP</th><th class=""table-primary""> Div </th>"
                    For i = 1 To sRndsJumpOffered
                        sJumpHeader += "<th class=""table-primary"">Round " & i & "</th>"
                    Next
                    sLine.Append(sJumpHeader & "</tr></thead>")

                Else
                    sLine.Append("<Table Class=""table  border-1 "">") '& sJumpHeader)  ' & sJumpDVHeader)
                    sLine.Append("<thead><tr><th class=""table-success"">JUMP</th><th class=""table-primary""> Div </th><th class=""table-primary"">Score Round " & sSelRnd & "</th></thead>")
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
                            sHasVideo = ""
                            'ONLY for Trick - Show trick video flag if available- 
                            If selEvent = "T" Then
                                If Not IsDBNull(MyDataReader.Item("Pass1VideoURL")) Then
                                    sTVidAvail = "Y"
                                End If

                                If Not IsDBNull(MyDataReader.Item("Pass2VideoURL")) Then
                                    sTVidAvail = "Y"
                                End If
                                If sTVidAvail = "Y" Then
                                    sHasVideo = "<img src=""Images/Flag-green16.png"" alt=""Trick Video Available"" title=""Trick Video Available, Select skier on Entry List"" />"
                                End If
                            End If


                            If selRnd = 0 Then
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
                                If sTmpEventGroup = "" Then
                                    sTmpEventGroup = sEventGroup
                                    sLine.Append("<tr><td class=""table-success""><b>Run Order</b></td><td class=""table-primary"" colspan = " & sRndsOffered + 1 & "><b> Event Group = " & sEventGroup & "</b></td></tr>")
                                End If

                                If stmpMemberID = "" Then
                                    stmpMemberID = sMemberID ' first record in first pass
                                    sLine.Append("<tr><td class=""table-success""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & stmpMemberID & "&DV=" & sDv & "&EV=" & sSelEvent & "&TN=" & sTName & "")
                                    sLine.Append("&FC=RO&FT=0&RP=" & sRnd & "&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>" & sHasVideo & "</td>")
                                    sLine.Append("<td><b> " & sDv & "</b></td>")
                                End If
                                If stmpMemberID = sMemberID Then
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
                                            sLine.Append("<td>No Score</td>")
                                    End Select


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
                                    If sTmpEventGroup <> sEventGroup Then
                                        sLine.Append("<tr><td class=""table-success""><b>Run Order</b></td><td class=""table-primary"" colspan = " & sRndsOffered + 1 & "><b> Event Group = " & sEventGroup & "</b></td></tr>")
                                        sTmpEventGroup = sEventGroup
                                    End If
                                    stmpMemberID = sMemberID
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
                                    sLine.Append("<tr><td class=""table-success""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & stmpMemberID & "&DV=" & sDv & "&EV=" & sSelEvent & "&TN=" & sTName & "")
                                    sLine.Append("&FC=RO&FT=0&RP=" & sRnd & "&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>" & sHasVideo & "</td>")
                                    sLine.Append("<td><b>" & sDv & "</b></td>")
                                    If sRnd > i Then  'If first score is in round 2 or greater - fill in earlier rounds as blanks
                                        Do Until sRnd = i
                                            sLine.Append("<td></td>")
                                            i += 1
                                        Loop
                                    End If
                                    sLine.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")
                                End If
                            Else 'only need score for selected round
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
                                sLine.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & stmpMemberID & "&DV=" & sDv & "&EV=" & sSelEvent & "&TN=" & sTName & "")
                                sLine.Append("&FC=RO&FT=0&RP=" & sRnd & "&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>" & sHasVideo & "</td>")
                                sLine.Append("<td>" & sEventGroup & " / <b>" & sDv & "</b> </td><td>" & sEventClassIcon & " " & sEventScoreDesc & "</td></tr>")
                            End If

                        Loop
                        sLine.Append("</tr></table>")
                    Else 'No data
                        sMsg = "No Skiers Found"
                    End If
                End Using
            Catch ex As Exception
                sMsg = "Error at ScoresXRunOrdHoriz"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sLine.ToString()
    End Function

    Friend Function LoadRecent(ByVal SanctionID As String) As String
        '   , ByVal YearPkd As String, ByVal TName As String, ByVal selEvent As String, ByVal selDv As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNOPS As Int16, ByVal UseTeams As Int16, ByVal FormatCode As String, ByVal DisplayMetric As Int16
        Dim sText As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sSkierName As String = ""
        Dim sSkierInsert As String = ""
        Dim sDV As String = ""
        Dim sEvent As String = ""
        Dim sEventClass As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sEventGroup As String = ""
        Dim sNOPS As String = ""
        Dim sMemberID As String = ""
        Dim sInsertDate As String = ""
        Dim sInsertTime As String = ""
        Dim sInsertTimeSpan As TimeSpan = Nothing
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
        Dim sSQL As String = "PrGetRecentScores"
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
        cmdRead.CommandText = sSQL
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InUseLastActive", OleDb.OleDbType.Boolean)
        cmdRead.Parameters("@InUseLastActive").Value = 0
        cmdRead.Parameters("@InUseLastActive").Direction = ParameterDirection.Input



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
                            sMemberID = CStr(MyDataReader.Item("MemberID"))
                            sDV = CStr(MyDataReader.Item("AgeGroup"))
                            If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                            Else
                                sEventScoreDesc = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("Event")) Then
                                sEvent = CStr(MyDataReader.Item("Event"))
                            Else
                                sEvent = ""
                            End If
                            Select Case sEvent
                                Case "Slalom"
                                    sEvent = "S"
                                Case "Trick"
                                    sEvent = "T"
                                Case "Jump"
                                    sEvent = "J"
                                Case Else
                                    sEvent = ""
                            End Select
                            If Not IsDBNull(MyDataReader.Item("EventClass")) Then
                                sEventClass = CStr(MyDataReader.Item("EventClass"))
                            Else
                                sEventClass = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventGroup")) Then
                                sEventGroup = CStr(MyDataReader.Item("EventGroup"))
                            Else
                                sEventGroup = ""
                            End If
                            sRound = CStr(MyDataReader.Item("Round"))
                            If Not IsDBNull(MyDataReader.Item("NOPSScore")) Then
                                sNOPS = CStr(MyDataReader.Item("NOPSScore"))
                            Else
                                sNOPS = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("InsertDate")) Then
                                sInsertDate = CStr(MyDataReader.Item("InsertDate").ToShortDateString())
                                sInsertTime = MyDataReader.Item("InsertDate").TimeOfDay.ToString()
                            Else
                                sInsertDate = ""
                            End If
                            'Have data - create display

                            If sText = "" Then
                                sText = "    <div Class=""container"">"
                                sText += "<div Class=""row"">"
                                sText += "    <div Class=""col-12 bg-primary text-white text-center"">"
                                sText += "Most Recent <span class=""bg-danger text-white"">UNOFFICIAL</span> Performances." & sInsertDate
                                sText += "   </div>"
                                sText += " </div>"
                                sText += "<div Class=""row"">"
                                sText += " <div class=""col-2"">Name</div><div class=""col-1"">Div</div><div class=""col-1"">Event</div><div class=""col-1"">Group</div><div class=""col-1"">Class</div><div class=""col-1"">Rnd</div><div class=""col-3"">Score</div><div class=""col-1"">Time</div>"
                            End If

                            sText += " <div class=""col-2"">" & sSkierName & "</div><div class=""col-1"">" & sDV & "</div><div class=""col-1"">" & sEvent & "</div><div class=""col-1"">" & sEventGroup & "</div><div class=""col-1"">" & sEventClass & "</div><div class=""col-1"">" & sRound & "</div><div class=""col-3"">" & sEventScoreDesc & "</div><div class=""col-1"">" & sInsertTime & "</div>"

                        Loop
                    Else
                        sMsg = "No Recent Scores<br>"

                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "<div Class=""row"">"
                sMsg += "    <div Class=""col-12 text-center"">"
                sMsg += "<b> Error at Load Recent </b>"
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
        Dim sInsertDate As String = ""
        Dim sInsertTime As String = ""
        Dim sBackColor As String = ""
        '        Dim sSQL As String = "Select top 5 SR.LastUpdateDate, SR.*, TR.SkierName "
        '        sSQL += " from SlalomRecap SR left join TourReg TR ON SR.MemberID = TR.MemberID where sanctionID = '" & sSanctionID & "'"
        '        sSQL += " order by sr.lastupdatedate desc "
        Dim sSQL As String = "SELECT top 10 SR.MemberID, SR.LastUpdateDate, SR.SanctionID, TR.SkierName,  SR.AgeGroup, "
        sSQL += " SR.[round], SR.SkierRunNum As Pass, SR.Score, SR.Note, SR.Reride, SR.RerideReason, SR.ProtectedScore, SR.InsertDate "
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
                            If Not IsDBNull(MyDataReader.Item("InsertDate")) Then
                                sInsertDate = CStr(MyDataReader.Item("InsertDate").ToShortDateString())
                                sInsertTime = MyDataReader.Item("InsertDate").TimeOfDay.ToString()
                            Else
                                sInsertDate = ""
                                sInsertTime = ""
                            End If
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
                            If sTmpMemberID = "" Then 'do this once on first record
                                sText = "    <div Class=""container"">"
                                sText += "<div Class=""row"">"
                                sText += "    <div Class=""col-12 bg-primary text-white text-center"">"
                                sText += "Most Recent <span class=""bg-danger text-white"">UNOFFICIAL</span> SLALOM performance on " & sInsertDate
                                sText += "   </div>"
                                sText += " </div>"

                                sTmpMemberID = sMemberID
                                sTmpRound = sRound
                                sText += "<div Class=""row"">"
                                sText += "    <div Class=""col-12 bg-info text-black "">"
                                sText += " <b>" & sSkierName & "</b> &nbsp; " & sDV & " &nbsp; Round: " & sRound
                                sText += "   </div>"
                                sText += " </div>"
                            End If

                            If sTmpMemberID = sMemberID And sTmpRound = sRound Then
                                sText += "<div Class=""row"">"
                                sText += "<div Class=""col-1 " & sBackColor & """>"
                                sText += "Pass:&nbsp;" & sPass & "&nbsp;"
                                sText += "</div>"
                                sText += "<div Class=""col-2"">"
                                sText += "&nbsp;&nbsp;" & sScore & "&nbsp;"
                                sText += "</div>"
                                sText += "<div Class=""col-9"">"
                                sText += sNote & "  " & sInsertTime
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
        SQL = "SELECT top 4 MemberID, Insertdate, SanctionID, SkierName, AgeGroup,EventClass, EventScore, Round, EventScoreDesc from LiveWebScoreboard.dbo.vTrickResults "
        SQL += " Where SanctionID ='" & sSanctionID & "' "
        '        SQL += " and LastUpdateDate > DateAdd(Minute, 5, GetDate())"
        SQL += "  ORDER BY insertdate desc"

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
        Dim sInsertDate As String = ""
        Dim sInsertTime As String = ""
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


                            If Not IsDBNull(MyDataReader.Item("InsertDate")) Then
                                sInsertDate = CStr(MyDataReader.Item("InsertDate").ToShortDateString())
                                sInsertTime = MyDataReader.Item("InsertDate").TimeOfDay.ToString()
                            Else
                                sInsertDate = ""
                                sInsertTime = ""
                            End If

                            If sTmpMemberID = "" Then
                                sTmpMemberID = sMemberID
                                sTmpRound = sRound
                                sTSB.Append("<Table class=""table"">")
                                sTSB.Append("<thead><tr class=""bg-primary text-white""><td colspan=""5"">MOST RECENT <span class=""bg-danger text-white"">UNOFFICIAL</span> Trick Performance on " & sInsertDate & "</td></tr>")
                            End If
                            If sTmpMemberID = sMemberID And sTmpRound = sRound Then
                                sTSB.Append("<tr class=""bg-light text-primary""><th widtn=""25%""><b>" & sSkierName & "</b></th><th widtn=""15%"">DV: " & sAgeGroup & "</th><th widtn=""15%"">Rnd: " & sRound & "</th><th>" & sEventScoreDesc & " @ " & sInsertTime & "</th></tr></thead>")
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
        Dim sInsertDate As String = ""
        Dim sInsertTime As String = ""
        Dim sBackColor As String = ""


        SQLsb.Append("SELECT top 10 JR.MemberID, JR.LastUpdateDate, JR.SanctionID, TR.SkierName, JR.AgeGroup, ")
        SQLsb.Append(" JR.[round], JR.PassNum As Pass, JR.Results, JR.ScoreFeet, JR.ScoreMeters, JR.Note, JR.Reride, JR.RerideReason, JR.ScoreProt ")
        SQLsb.Append(", JR.RerideIfBest, JR.RerideCanImprove, JR.InsertDate ")
        SQLsb.Append(" From LiveWebScoreboard.dbo.JumpRecap JR ")
        SQLsb.Append(" Left Join (Select distinct SkierName, SanctionID, MemberID from LiveWebScoreboard.dbo.TourReg where sanctionID = '" & sSanctionID & "') ")
        SQLsb.Append(" as TR On JR.sanctionID = TR.SanctionID And JR.MemberID = TR.MemberID ")
        SQLsb.Append(" Where TR.SanctionId = '" & sSanctionID & "' ") ' following has reride  and [round] <> 25 and TR.MemberID = '200149011' "
        '       SQLsb.Append(" and LastUpdateDate > DateAdd(Minute, 10, GetDate())")
        SQLsb.Append(" order by LastUpdateDate desc ")
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
                            If Not IsDBNull(MyDataReader.Item("InsertDate")) Then
                                sInsertDate = CStr(MyDataReader.Item("InsertDate").ToShortDateString())
                                sInsertTime = MyDataReader.Item("InsertDate").TimeOfDay.ToString()
                            Else
                                sInsertDate = ""
                                sInsertTime = ""
                            End If
                            'Have data
                            If sTmpMemberID = "" Then  'first record - build header, display name
                                myStringBuilder.Append(" <div Class=""container"">")
                                myStringBuilder.Append(" <div Class=""row"">")
                                myStringBuilder.Append(" <div Class=""col-12 bg-primary text-white text-center"">")
                                myStringBuilder.Append("Most Recent <span class=""bg-danger text-white"">UNOFFICIAL</span> JUMP performance " & sInsertDate)
                                myStringBuilder.Append("   </div>")
                                myStringBuilder.Append(" </div>")
                                sTmpRound = sRound
                                sTmpMemberID = sMemberID
                                myStringBuilder.Append("<div Class=""row"">")
                                myStringBuilder.Append("<div Class=""col-12 bg-info text-black ""><b>")
                                myStringBuilder.Append(sSkierName & "</b> &nbsp; " & sDV & " &nbsp; Round: " & sRound)
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
                                myStringBuilder.Append(sNote & "  " & sInsertTime)
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
        Dim sEventClassIcon As String = ""
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
        Dim sEventScoreDescMetric As String = ""
        Dim sEventScoreDescImperial As String = ""
        Dim sEventScoreToShow As String = ""
        Dim sIWWFGroups As String = ""
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
                sSBSql.Append(" TRIM(CAST(FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(FinalLen) + 'm' AS EventScoreDescMetric,")
                sSBSql.Append(" TRIM(CAST(FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(FinalSpeedMph AS CHAR)) + 'mph ' + TRIM(FinalLenOff) AS EventScoreDescImperial ")
                sSBSql.Append(" FROM LiveWebScoreboard.dbo.SlalomScore where sanctionID = ? and MemberID = ? and AgeGroup = ?  and [Round] <> 25")
                If sSelRnd <> 0 Then sSBSql.Append(" and Round = ? ")

            Case "Trick"

                sSBSql.Append(" Select SanctionID, MemberID,AgeGroup as Div, EventClass,  [Round], NopsScore, Score AS EventScore, ")
                sSBSql.Append(" Trim(CAST(Score As Char)) + ' POINTS (P1:' + TRIM(CAST(ScorePass1 AS CHAR)) + ' P2:' + TRIM(CAST(ScorePass2 AS CHAR)) + ')' AS EventScoreDesc ")
                sSBSql.Append(" From LiveWebScoreboard.dbo.TrickScore Where SanctionID = ? and MemberID = ? and AgeGroup = ?  and [Round] <> 25")
                If sSelRnd <> 0 Then sSBSql.Append(" and Round = ? ")

            Case "Jump"
                sSBSql.Append("Select SanctionID, memberid, AgeGroup as Div, EventClass,  [Round], NopsScore, TRIM(cast(ScoreFeet As Char) + 'F ' + Cast(ScoreMeters AS CHAR) + 'M') AS EventScoreDesc, ")
                sSBSql.Append(" ScoreFeet, ScoreMeters, InsertDate from LiveWebScoreboard.dbo.JumpScore where SanctionID = ? and MemberID = ? and AgeGroup = ? and [Round] <> 25")
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
                            'FOR SLALOM EVENT ONLY - DISPLAY IMPERIAL OR METRIC BASED ON AGE OR IWWF DIVISION
                            If sSelEvent = "Slalom" Then

                                If Not IsDBNull(MyDataReader.Item("EventScoreDescMetric")) Then
                                    sEventScoreDescMetric = MyDataReader.Item("EventScoreDescMetric")
                                Else
                                    sEventScoreDescMetric = "N/A"
                                End If
                                If Not IsDBNull(MyDataReader.Item("EventScoreDescImperial")) Then
                                    sEventScoreDescImperial = MyDataReader.Item("EventScoreDescImperial")
                                Else
                                    sEventScoreDescImperial = "N/A"
                                End If

                                sEventScoreToShow = ""
                                Select Case sDV
                                    Case "OM", "OW", "MM", "MW"
                                        sEventScoreToShow = sEventScoreDescMetric
                                End Select
                                sIWWFGroups = Left(sDV, 1)
                                Select Case sIWWFGroups
                                    Case "Y", "J", "I", "S", "L"
                                        sEventScoreToShow = sEventScoreDescMetric
                                End Select
                                If sEventScoreToShow = "" Then
                                    sEventScoreToShow = sEventScoreDescImperial
                                End If
                                sEventScoreDesc = sEventScoreToShow
                            End If

                            If sSelRnd = 0 Then 'including all rounds
                                i += 1 'Round 1 column
                                If sRnd > i Then  'If first score is in round 2 or greater - fill in earlier rounds as blanks
                                    Do Until sRnd = i
                                        sTmpRow.Append("<td></td>")
                                        i += 1
                                    Loop
                                End If
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
                                Select Case sRnd 'get the data available for the specified event, Group, DV, and skier
                                    Case 1
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td>" & sEventClassIcon & " <b>" & sEventScoreDesc & "</b></td>")
                                            '     sTmpRow.Append("<td class=""table-warning""><b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 2
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td>" & sEventClassIcon & " <b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 3
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td>" & sEventClassIcon & " <b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 4
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td>" & sEventClassIcon & " <b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 5
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td>" & sEventClassIcon & " <b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 6
                                        If sBestRnd = sRnd Then 'highlight the best round
                                            sTmpRow.Append("<td>" & sEventClassIcon & " <b>" & sEventScoreDesc & "</b></td>")
                                        Else
                                            sTmpRow.Append("<td>" & sEventClassIcon & " " & sEventScoreDesc & "</td>")   'sRnd1Score = sEventScoreDesc  'sR1=true
                                        End If
                                    Case 25 ' Runoff Round
                                        sTmpRow.Append("<td forecolor=""red" > "" & sEventClassIcon & " " & sEventScoreDesc & "</td>")
                                    Case 0  'error
                                        sTmpRow.Append("<td>No Score</td>")
                                End Select

                            Else 'only need score for selected round Expand format
                                'Header from calling function  <td>Rnd " & sSelRnd & "</td><td>NOPS</td><td>Details</td><td>Time</td>
                                sTmpRow.Append("<td><b>" & sEventClass & "</b></td><td><b> " & sNOPS & "</b></td><td colspan=""2""><b> " & sEventScoreDesc & "</b></td></tr>")
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
                        sTmpRow.Append("<td colspan=""" & sCols2Make & """>  </td></tr>") 'Per Dave leave blan
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
            Case "0"
                sMsg = "Please select an Event"
 '               sPREventCode = "ALL"   'Default to all so that DV and Rnd droplists are populated
            Case "A"
                '                sPREventCode = "ALL"
                sMsg = "Please select an Event"
            Case "S"
                sPREventCode = "Slalom"
            Case "T"
                sPREventCode = "Trick"
            Case "J"
                sPREventCode = "Jump"
            Case "O"
                sPREventCode = "Overall"
            Case Else  'If no event selected kick out
                sMsg = "Please select an Event"
                '               sPREventCode = "ALL"  'Default to all so that DV and Rnd droplists are populated
        End Select
        If Len(sMsg) > 2 Then
            DDL_Division.Items.Clear()
            DDL_PkRnd.Items.Clear()
            Return sMsg
            Exit Function
        End If
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
                    .Items.Add(New ListItem("ALL DV", "ALL"))
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
        If sDVPkd <> "" Then
            DDL_Division.SelectedValue = sDVPkd
        End If
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

        With DDL_PkRnd
            .Items.Clear()
            .Items.Add(New ListItem("ALL Rnds", "0"))  'Defaults to all rounds if sEventCode is not in range or rounds not used (NCWSA)
            Select Case sEventCode
                Case "A"   'Include all rounds if overall is picked - or include overall and use minimum number of rounds offered in S, T, or J
                    For i = 1 To sMaxRounds
                        .Items.Add(New ListItem("Rnd " & i, i))
                    Next
                Case "S"
                    For i = 1 To sSlalomRnds
                        .Items.Add(New ListItem("Rnd " & i, i))
                    Next
                Case "T"
                    For i = 1 To sTrickRnds
                        .Items.Add(New ListItem("Rnd " & i, i))
                    Next
                Case "J"
                    For i = 1 To sJumpRnds
                        .Items.Add(New ListItem("Rnd " & i, i))
                    Next
                Case "O"
                    For i = 1 To sMinRounds
                        .Items.Add(New ListItem("Rnd " & i, i))
                    Next
            End Select
        End With
        If Len(sMsg) > 2 Then
            Return sMsg
        End If
        Return "Success"
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
    Friend Function GetCurrentEvent(ByVal SanctionID As String, ByVal minutes As Int16) As String
        'Use PRGetMostRecentScores with minutes set to - (negative) number of minutes since performance.  
        'For Tournament.aspx want distinct event in last 30 minutes. Display Event, Div, Round of first record.
        '   if record 2 is different event and within 5 minutes of record 1, display both.  continue until all 3 events or more than 5 minutes
        'For recent scores use -30 as switch to show hide droplist option for Most Recent page.  If no records hide the option.
        '
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sReturnArray(0 To 3)
        Dim sSanctionID As String = SanctionID
        Dim sMinutes As Int16 = minutes
        Dim sConn As String = ""
        Dim sSQL As String = "PRGetMostRecentScores"
        Dim sEvent As String = ""
        Dim sTmpEvent As String = ""
        Dim sDV As String = ""
        Dim sTmpDv As String = ""
        Dim sRnd As String = ""
        Dim sReturnString1 = ""
        'NEED sTmpTime and sTime (as datetime?)
        Dim sCurEvent As String = ""
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

        cmdRead.Parameters.Add("@InLastMinuteCheck", OleDb.OleDbType.Integer)
        cmdRead.Parameters("@InLastMinuteCheck").Value = sMinutes
        cmdRead.Parameters("@InLastMinuteCheck").Direction = ParameterDirection.Input

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

                            'If this is first time
                            '   add event, division, round to string
                            '   stmpTime = sTime
                            '   sTmpEvent = sEvent
                            'if another time exists and sTmpEvent <> sEvent and tmpTime within 5 minutes of sTime then
                            '   Add Event, Div, Rnd

                            sEvent = CStr(MyDataReader.Item("Event"))
                            sRnd = CStr(MyDataReader.Item("Round"))
                            sDV = CStr(MyDataReader.Item("Div"))

                            If sTmpEvent = "" Then
                                sTmpEvent = sEvent
                                sTmpDv = sDV
                                sReturnString1 = "Active Event(s)  "
                                sReturnString1 += sDV & " " & sEvent & " Round " & sRnd
                            Else

                                If sEvent <> sTmpEvent Or sDV <> sTmpDv Then
                                    sReturnString1 += "<br>" & sDV & " " & sEvent & " Round " & sRnd
                                End If
                            End If
                        Loop
                    Else
                        sReturnString1 = ""
                    End If 'end of has rows
                End Using

            Catch ex As Exception
                sMsg += "Error: Can't retrieve Current Event. "
                sErrDetails = " GetCurrentEvent Caught: " & ex.Message & " " & ex.StackTrace & "<br>SQL= " & sSQL

            End Try
        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Return sReturnString1
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
                    Else
                        arrSpecs(0, 0) = "Data for " & sSanctionID & " is not available"
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sErrDetails += "Error: GetTournamentSpecs Caught: <br />" & ex.Message & " " & ex.StackTrace & "<br> SQL= " & SQL
                arrSpecs(0, 0) = "Error retrieving tournament information."
            End Try
        End Using
        Return arrSpecs
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

        Dim sAssignment As String = ""

        sSQL = "PrGetOfficialsPanel"   '24E017, 24U269, 23E998, 23S999
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
        Dim cmdRead As New OleDb.OleDbCommand
        'sUsePR = true
        cmdRead.CommandType = CommandType.StoredProcedure
        cmdRead.CommandText = sSQL
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

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
                            sAssignment = MyDataReader.Item("Assignment")
                            arrOfficials(i, 1) = sAssignment
                            arrOfficials(i, 2) = sSkierName
                            ' IF using officials ratings need to test for null unless driver and scorer ratings are included.
                            'JudgeSlalomRating
                            'JudgeTrickRating
                            'JudgeJumpRating
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
    Friend Function GetTeams(ByVal SanctionID As String) As String
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



    Public Function LeaderBoardBestRndLeftSP(ByVal SanctionID As String, ByVal SkiYr As String, ByVal TName As String, ByVal selEvent As String, ByVal selDv As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNOPS As Int16, ByVal UseTeams As Int16, ByVal selFormat As String, ByVal DisplayMetric As Int16) As String
        'This function is run for each event selected based on code in TLeaderBoard_Load and Btn_Update
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sStopHere As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sYrPkd As String = SkiYr
        Dim sTName As String = TName
        Dim sSelEvent As String = ""  'Event Selected as Slalom Trick, Jump
        Dim sEventPkd As String = selEvent 'Event as S, T, or J
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
        Dim sScoreBest As String = ""
        Dim sEventScore As String = ""
        Dim stmpMemberID As String = ""
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim sDv As String = ""
        Dim sTmpDv As String = ""
        Dim sRunOffDv As String = ""
        Dim sEventClass As String = ""
        Dim sEventGroup As String = ""
        Dim sCity As String = ""
        Dim sState As String = ""
        Dim sFederation As String = ""
        Dim sRankingScore As String = ""
        Dim sNopsScore As String = ""
        Dim sUseNOPS As Int16 = UseNOPS
        Dim suUseTeams As Int16 = UseTeams
        Dim sSlalomHeader As String = ""
        Dim sTrickHeader As String = ""
        Dim sUnit As String = ""
        Dim sJumpHeader As String = ""
        Dim sLine As New StringBuilder
        Dim sDVScoresSection As New StringBuilder
        Dim sDVHeader As String = ""
        Dim sMultiRndScores As String = ""
        Dim sSql As String = ""
        Dim sShowBuoys As String = ""
        Dim sTVidAvail As String = ""
        Dim sHasVideo As String = ""
        Dim sHasRunoff As Boolean = False
        Dim sRunoffSection As String = ""
        Dim sScoreRunoff As String = ""
        Dim sPlcmtFormat As String = ""
        Dim sReadyForPlcmt As String = ""

        Select Case selEvent
            Case "S"
                sSelEvent = "Slalom"
                sSql = " PrLeaderBoard"
                sRndsThisEvent = sRndsSlalomOffered
                sUnit = " Buoys"
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsSlalomOffered) + 1)
                    For j = 1 To RndsSlalomOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd&nbsp;" & j & "</td>" 'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Class</td><td>NOPS</td><td colspan=""2"">Details</td>"
                    sRndCols = "6"
                End If


            Case "T"
                sSelEvent = "Trick"
                sSql = "PrLeaderBoard"
                sRndsThisEvent = sRndsTrickOffered
                sUnit = " Points"
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsTrickOffered) + 1)  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsTrickOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd&nbsp;" & j & "</td>"  'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td> Points </td><td>NOPS</td><td>Details</td><td>Time</td>"
                    sRndCols = "6"
                End If

            Case "J"
                sSelEvent = "Jump"
                sSql = "PrLeaderBoard"
                sRndsThisEvent = sRndsJumpOffered
                sUnit = " Feet"
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsJumpOffered) + 1)  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsJumpOffered
                        sRoundsHTML += "<td  Class=""table-primary"">Rnd&nbsp;" & j & "</td>"    'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td>Class</td><td> Ft/M </td><td>NOPS</td><td>Details</td><td>Time</td></tr>"
                    sRndCols = "6"
                End If
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
        cmdRead.Parameters("@InFormat").Value = "Best"    '0 = All Rounds    sSelRnd
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
                            If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                                sScoreBest = MyDataReader.Item("EventScore")
                            Else
                                sScoreBest = ""
                            End If
                            'Show best score only for B1 and G1 slalom divisions
                            If (sDv <> "OM" And sDv <> "OW") And selEvent = "S" Then
                                sShowBuoys = sScoreBest & " Buoys"
                            Else
                                sShowBuoys = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("ScoreRunoff")) Then
                                sScoreRunoff = MyDataReader.Item("ScoreRunoff")
                            Else
                                sScoreRunoff = ""
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
                            sReadyForPlcmt = ""
                            If Not IsDBNull(MyDataReader.Item("ReadyForPlcmt")) Then
                                If MyDataReader.Item("ReadyForPlcmt") <> "Y" Then
                                    sReadyForPlcmt = "<img src=""Images/CriticalError.bmp"" alt=""NOT for Placement"" title=""NOT for placeent"" />"
                                End If
                            End If

                                sHasVideo = ""
                            'ONLY for Trick - Show trick video flag if available- 
                            If selEvent = "T" Then
                                If Not IsDBNull(MyDataReader.Item("Pass1VideoURL")) Then
                                    sTVidAvail = "Y"
                                End If

                                If Not IsDBNull(MyDataReader.Item("Pass2VideoURL")) Then
                                    sTVidAvail = "Y"
                                End If
                                If sTVidAvail = "Y" Then
                                    sHasVideo = "<img src=""Images/Flag-green16.png"" alt=""Trick Video Available"" title=""Trick Video Available, Select skier on Entry List"" />"
                                End If
                            End If
                            sPlcmtFormat = UCase(MyDataReader.Item("plcmtformat"))

                            If sTmpDv = "" Then
                                'Get the division header for first division
                                sDVHeader = "<table class=""table""><tr class=""table-info""><td colspan=""" & sRndCols & """>Placement Format = " & sPlcmtFormat & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span><b> &nbsp;" & sSelEvent & " " & sDv & "</b></td></tr>"
                                'Start the first Dataline
                                sDVScoresSection.Append("<tr><td Class=""table-warning"" width=""25%""><b> Leader Board </b></td>" & sRoundsHTML)
                                sTmpDv = sDv
                            End If
                            'Get the first MemberID' first record in first pass through data
                            If stmpMemberID = "" Then stmpMemberID = sMemberID

                            If sTmpDv = sDv Then 'Continue in same Division
                                'Add the data line
                                sDVScoresSection.Append("<tr><td Class=""table-warning""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sTmpDv & "&EV=" & sEventPkd & "&TN=" & sTName & "")
                                sDVScoresSection.Append("&FC=LBSP&FT=0&RP=1&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a><b> " & sShowBuoys & "</b>" & sHasVideo & sReadyForPlcmt & "</td>")   '   
                                sMultiRndScores = ModDataAccess3.LBGetRndScores(sSanctionID, sMemberID, sSelEvent, sDv, sSelRnd, sRound, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sNopsScore)
                                If sMultiRndScores <> "Error" Then
                                    sDVScoresSection.Append(sMultiRndScores)
                                    sMultiRndScores = ""
                                Else
                                    'FIX THIS ERROR TRAP
                                End If
                                If sScoreRunoff <> "" Then
                                    sHasRunoff = True
                                    sRunOffDv = sDv
                                End If
                            Else 'Division changed.
                                'Add the Header line
                                sLine.Append(sDVHeader)
                                'Get RunOff scores if appropriate
                                If sHasRunoff = True Then 'Only add runnoff scores if this division has runoff scores
                                    sRunoffSection = ModDataAccessPro.GetRunoffSection(sSanctionID, sEventPkd, sRunOffDv)
                                    '
                                    If Left(sRunoffSection, 3) = "<b>" Then
                                        sLine.Append("<tr><td Class=""table-info"" colspan=""" & sRndCols - 2 & """><b>RunOff Scores</td><td colspan=""2"">" & sRunoffSection & "</td></tr>")
                                    End If
                                End If
                                'Close division table
                                sDVScoresSection.Append("</table>")
                                'Add Division scores to sLine
                                sLine.Append(sDVScoresSection.ToString())
                                'reset the division variables
                                sDVScoresSection.Clear()
                                sRunoffSection = ""
                                sDVHeader = ""
                                sHasRunoff = False
                                'Previous division is closed - start new Division
                                stmpMemberID = sMemberID
                                sTmpDv = sDv
                                'start new division header
                                sDVHeader = "<table Class=""table""><tr Class=""table-info""><td  colspan=""" & sRndCols & """>Placement Format = " & sPlcmtFormat & " <span Class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span><b> &nbsp;" & sSelEvent & " " & sDv & "</b></td></tr>"
                                sDVScoresSection.Append("<tr><td Class=""table-warning"" width=""25%""><b> Leader Board </b></td>" & sRoundsHTML)
                                'Add the data line
                                sDVScoresSection.Append("<tr><td Class=""table-warning""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & stmpMemberID & "&DV=" & sTmpDv & "&EV=" & sEventPkd & "&TN=" & sTName & "")
                                sDVScoresSection.Append("&FC=LBSP&FT=0&RP=1&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a><b> " & sShowBuoys & "</b>" & sHasVideo & sReadyForPlcmt & "</td>")   '   
                                sMultiRndScores = ModDataAccess3.LBGetRndScores(sSanctionID, sMemberID, sSelEvent, sDv, sSelRnd, sRound, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sNopsScore)
                                If sMultiRndScores <> "Error" Then
                                    sDVScoresSection.Append(sMultiRndScores)
                                Else
                                    'FIX THIS ERROR TRAP
                                End If
                            End If

                        Loop
                        'Add the Header line
                        sLine.Append(sDVHeader)
                        '                       End If
                        'Get RunOff scores if appropriate
                        If sHasRunoff = True Then 'Only add runnoff scores if this division has runoff scores
                            sRunoffSection = ModDataAccessPro.GetRunoffSection(sSanctionID, sEventPkd, sRunOffDv)

                            If Left(sRunoffSection, 3) = "<b>" Then
                                sLine.Append("<tr><td Class=""table-info"" colspan=""" & sRndCols - 2 & """><b>RunOff Scores</td><td colspan=""2"">" & sRunoffSection & "</td></tr>")
                            End If
                        End If
                        'Close division table
                        sDVScoresSection.Append("</table>")
                        'Add Division scores to sLine
                        sLine.Append(sDVScoresSection.ToString())
                    Else
                        '      sLine.Append("<tr  Class=""table-info""><td> " & sSkierName & "</td><td>No Scores</td></tr></table>")
                        sMsg = "No Scores"
                    End If

                End Using
            Catch ex As Exception
                sMsg = "Error at LeaderBoardBestRndLeftSP"
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

    Public Function LeaderBoardROUND(ByVal SanctionID As String, ByVal SkiYr As String, ByVal TName As String, ByVal selEvent As String, ByVal selDv As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNOPS As Int16, ByVal UseTeams As Int16, ByVal selFormat As String, ByVal DisplayMetric As Int16) As String
        'Called from LeaderBoardSP when plcmntFormat = ROUND
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sStopHere As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sYrPkd As String = SkiYr
        Dim sTName As String = TName
        Dim sSelEvent As String = ""  'Event Selected as Slalom Trick, Jump
        Dim sSelRnd As String = selRnd   'Round selected as a filter
        Dim sSelDV As String = selDv    'Div selected as a filter
        Dim sRndsSlalomOffered = RndsSlalomOffered
        Dim sRndsTrickOffered = RndsTrickOffered
        Dim sRndsJumpOffered = RndsJumpOffered
        Dim sRndsThisEvent As String = ""  'generic form of RndsSSlalomOffered, etc.
        Dim sRoundsHTML As String = ""  'string of <td>Rnd " & i & "</td> one for each round in an event
        Dim sRndCols As Int16 = 0  'Number of <td></td> sections in the table based on rounds offered + one column for Name and best score
        '        Dim sRound As String = ""  'Round in which best score was achieved
        Dim sSkierName As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sEventScoreDescMetric As String = ""
        Dim sEventScoreDescImperial As String = ""
        Dim sEventScore As String = ""
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim sMemberID As String = ""
        Dim stmpMemberID As String = ""
        Dim sDv As String = ""
        Dim sTmpDv As String = ""
        Dim sCurRnd As String = ""
        Dim sTmpRnd As String = ""
        Dim sEventClass As String = ""
        Dim sEventGroup As String = ""
        Dim sCity As String = ""
        Dim sState As String = ""
        Dim sFederation As String = ""
        Dim sRankingScore As String = ""
        Dim sNopsScore As String = ""
        Dim sUseNOPS As Int16 = UseNOPS
        Dim suUseTeams As Int16 = UseTeams
        Dim sSlalomHeader As String = ""
        Dim sTrickHeader As String = ""
        Dim sUnit As String = ""
        Dim sJumpHeader As String = ""
        Dim sLine As New StringBuilder
        Dim sMultiRndScores As String = ""
        Dim sSql As String = ""
        Dim sShowBuoys As String = ""
        Dim sTVidAvail As String = ""
        Dim sHasVideo As String = ""
        Dim sScoreFeet As String = ""
        Dim sScoreMeters As String = ""
        Dim sMasterTable As New StringBuilder
        Dim sDvTable As New StringBuilder
        Dim sSkierLink As String = ""


        Select Case selEvent
            Case "S"
                sSelEvent = "Slalom"
                sSql = "Select [SanctionID],[SkierName],[MemberId],[Div],[EventClass],[TeamCode],[Round],[NopsScore],[EventScore],[ScoreRunoff],[Buoys] "
                sSql += " ,[EventScoreDesc],[EventScoreDescMeteric],[EventScoreDescImperial],[InsertDate] "
                sSql += " From [LiveWebScoreboard].[dbo].[vSlalomResults] Where SanctionID = ? and Event = ? "
                If sSelRnd <> "0" Then
                    sSql += " And [Round] = ? "
                End If
                If UCase(sSelDV) <> "ALL" Then
                    sSql += " And [Div] = ? "
                End If
                sSql += " Order By [Round], [Div], [EventScore] desc"
                sUnit = " Buoys"
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsSlalomOffered))
                    For j = 1 To RndsSlalomOffered
                        sRoundsHTML += "<td  Class=""table-primary"">Rnd&nbsp;" & j & "</td>" 'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Class</td><td>NOPS</td><td colspan=""2"">Details</td>"
                    sRndCols = "6"
                End If


            Case "T"
                sSelEvent = "Trick"
                sSql = "Select [SanctionID],[SkierName],[MemberId],[Div],[EventClass],[TeamCode],[Round],[NopsScore],[EventScore],[ScoreRunoff] "
                sSql += " ,[EventScoreDesc],[Pass1VideoUrl],[Pass2VideoUrl],[InsertDate] "
                sSql += " From [LiveWebScoreboard].[dbo].[vTrickResults]  Where SanctionID = ? and Event = ? "
                If sSelRnd <> "0" Then
                    sSql += " And [Round] = ? "
                End If
                If UCase(sSelDV) <> "ALL" Then
                    sSql += " And [Div] = ? "
                End If

                sSql += " Order By [Round], [Div], [EventScore] desc"
                sRndsThisEvent = sRndsTrickOffered
                sUnit = " Points"
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsTrickOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsTrickOffered
                        sRoundsHTML += "<td  Class=""table-primary"">Rnd&nbsp;" & j & "</td>"  'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td> Points </td><td>NOPS</td><td>Details</td><td>Time</td>"
                    sRndCols = "6"
                End If

            Case "J"
                sSelEvent = "Jump"
                sSql = "Select [SanctionID],[SkierName],[MemberId],[Div],[EventClass],[TeamCode],[Round],[NopsScore],[EventScore],[ScoreRunoff] "
                sSql += " ,[EventScoreDesc],[ScoreFeet],[ScoreMeters],[InsertDate] "
                sSql += " From [LiveWebScoreboard].[dbo].[vJumpResults]  Where SanctionID = ? and Event = ? "
                If sSelRnd <> "0" Then
                    sSql += " And [Round] = ? "
                End If
                If UCase(sSelDV) <> "ALL" Then
                    sSql += " And [Div] = ? "
                End If
                sSql += " Order By [Round], [Div], [EventScore] desc"
                sRndsThisEvent = sRndsJumpOffered
                sUnit = " Feet"
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsTrickOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsJumpOffered
                        sRoundsHTML += "<td  Class=""table-primary"">Rnd&nbsp;" & j & "</td>"    'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td>Class</td><td> Ft/M </td><td>NOPS</td><td>Details</td><td>Time</td>"
                    sRndCols = "6"
                End If
            Case Else  'Load all by default
                sMsg = "<td>Event Code out Of range</td></tr>"
                Return sMsg
                Exit Function
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
        cmdRead.CommandType = CommandType.Text
        cmdRead.CommandText = sSql
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InEvCode", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InEvCode").Size = 12
        cmdRead.Parameters("@InEvCode").Value = sSelEvent
        cmdRead.Parameters("@InEvCode").Direction = ParameterDirection.Input

        If sSelRnd <> "0" Then

            cmdRead.Parameters.Add("@InRnd", OleDb.OleDbType.Char)
            cmdRead.Parameters("@InRnd").Size = 1
            cmdRead.Parameters("@InRnd").Value = sSelRnd    '0 = All Rounds    sSelRnd
            cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input
        End If

        If UCase(sSelDV) <> "ALL" Then
            cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
            cmdRead.Parameters("@InDV").Size = 3
            cmdRead.Parameters("@InDV").Value = sSelDV   'This is the division selected for display.  sDv is the division in which the skier is performing.
            cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input
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
                            sSanctionID = CStr(MyDataReader.Item("SanctionID"))
                            sSkierName = CStr(MyDataReader.Item("SkierName"))

                            sSkierName = MyDataReader.Item("SkierName")

                            If Not IsDBNull(MyDataReader.Item("MemberID")) Then
                                sMemberID = MyDataReader.Item("MemberID")
                            Else
                                sMemberID = ""
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
                            'team
                            If Not IsDBNull(MyDataReader.Item("Round")) Then
                                sCurRnd = CStr(MyDataReader.Item("Round"))
                            Else
                                sCurRnd = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventScore")) Then
                                sEventScore = MyDataReader.Item("EventScore")
                            Else
                                sEventScore = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("NOPSScore")) Then
                                sNopsScore = MyDataReader.Item("NOPSScore")
                            Else
                                sNopsScore = ""
                            End If

                            If Not IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEventScoreDesc = MyDataReader.Item("EventScoreDesc")
                            Else
                                sEventScoreDesc = ""
                            End If


                            If selEvent = "S" Then  'fields specific to slalom event

                                If Not IsDBNull(MyDataReader.Item("EventScoreDescMeteric")) Then
                                    sEventScoreDescMetric = MyDataReader.Item("EventScoreDescMeteric")
                                Else
                                    sEventScoreDescMetric = ""
                                End If
                                If Not IsDBNull(MyDataReader.Item("EventScoreDescImperial")) Then
                                    sEventScoreDescImperial = MyDataReader.Item("EventScoreDescImperial")
                                Else
                                    sEventScoreDescImperial = ""
                                End If
                                'Show best score only for B1 and G1 slalom divisions
                                '                           If (sDv = "B1" Or sDv = "G1")  Then
                                '                               sShowBuoys = sScoreBest & " Buoys"
                                '                           Else
                                '                               sShowBuoys = ""
                                '                           End If
                            End If 'end slalom specific fields

                            sHasVideo = ""

                            If selEvent = "T" Then 'ONLY for Trick - Show trick video flag if available- 
                                If Not IsDBNull(MyDataReader.Item("Pass1VideoURL")) Then
                                    sTVidAvail = "Y"
                                End If

                                If Not IsDBNull(MyDataReader.Item("Pass2VideoURL")) Then
                                    sTVidAvail = "Y"
                                End If
                                If sTVidAvail = "Y" Then
                                    sHasVideo = "<img src=""Images/Flag-green16.png"" alt=""Trick Video Available"" title=""Trick Video Available"" />"
                                End If
                            End If 'end of trick specific fields

                            If selEvent = "J" Then 'Jump Specific fields

                                If Not IsDBNull(MyDataReader.Item("ScoreFeet")) Then
                                    sScoreFeet = MyDataReader.Item("ScoreFeet")
                                Else
                                    sScoreFeet = ""
                                End If

                                If Not IsDBNull(MyDataReader.Item("ScoreMeters")) Then
                                    sScoreMeters = MyDataReader.Item("ScoreMeters")
                                Else
                                    sScoreMeters = ""
                                End If
                            End If 'End jump specific fields

                            'Make skier name link
                            sSkierLink = "<a runat = ""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sDv & "&EV=S&TN=" & sTName & ""
                            sSkierLink += "&FC=LBSP&FT=0&RP=" & sSelRnd & "&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>"


                            If sTmpRnd = "" Then
                                'Make master table
                                sMasterTable.Append("<table class=""table""><tr class=""table-primary""><td colspan=""" & sRndCols & """>")
                                sMasterTable.Append("<span Class=""bg-danger text-white"" ><b>!UNOFFICIAL!</b>&nbsp; &nbsp; </span> ")
                                sMasterTable.Append(" <b> " & UCase(sSelEvent) & " Results - Placement Format = ROUND</b></td></tr>")
                                sMasterTable.Append("<tr><td>")
                                'Make first division table
                                sDvTable.Append("<table Class=""table""><tr Class=""table-info""><td colspan=""2""><b>" & sDv & " &nbsp; &nbsp; Round " & sCurRnd & "</b></td></tr>")
                                sTmpRnd = sCurRnd
                                sTmpDv = sDv
                            End If
                            If sTmpRnd = sCurRnd Then  'Rnd header already in place.  Continue

                                If sTmpDv = sDv Then 'Div header in place, just list skier
                                    sDvTable.Append("<tr><td>" & sSkierLink & "</td><td><b>" & sEventScoreDesc & "</b></td></tr>")

                                Else 'division changed  close out division table. Add to Mastertable, and start new division table 
                                    sDvTable.Append("</table>")
                                    sMasterTable.Append(sDvTable.ToString())
                                    sDvTable.Clear()
                                    sDvTable.Append("<table Class=""table""><tr Class=""table-info""><td colspan=""2""><b>" & sDv & " &nbsp; &nbsp; Round " & sCurRnd & "</b></td></tr>")
                                    sDvTable.Append("<tr><td>" & sSkierLink & "</td><td><b>" & sEventScoreDesc & "</b></td></tr>")
                                    sTmpDv = sDv
                                End If

                            Else 'Round changed close out existing division and round and start a new round and division
                                sDvTable.Append("</table>")
                                sMasterTable.Append(sDvTable.ToString() & "</td><td valign=""top"">")
                                sDvTable.Clear()
                                sDvTable.Append("<table Class=""table""><tr Class=""table-info""><td colspan=""2""><b>" & sDv & " &nbsp; &nbsp; Round " & sCurRnd & "</b></td></tr>")
                                sDvTable.Append("<tr><td>" & sSkierLink & "</td><td><b>" & sEventScoreDesc & "</b></td></tr>")
                                sTmpDv = sDv
                                sTmpRnd = sCurRnd

                            End If

                        Loop
                        'end of scores  close existing division and master tables
                        sDvTable.Append("</table>")
                        sMasterTable.Append(sDvTable.ToString & "</td></tr></table>")
                    Else  'No records
                        sMsg = "NO " & UCase(sSelEvent) & " SCORES FOUND FOR " & sSanctionID

                    End If

                End Using
            Catch ex As Exception
                sMsg = "Error at LeaderBoardRound"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try

        End Using

        If Len(sMsg) > 2 Then
            Return sMsg
        Else
            '          sMsg = sMasterTable.ToString()
            Return sMasterTable.ToString()
        End If
    End Function
    Public Function LeaderBoardBestRndLeft(ByVal SanctionID As String, ByVal SkiYr As String, ByVal TName As String, ByVal selEvent As String, ByVal selDv As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNOPS As Int16, ByVal UseTeams As Int16, ByVal selFormat As String, ByVal DisplayMetric As Int16) As String
        'This function is run for each event selected based on code in TLeaderBoard_Load and Btn_Update
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sStopHere As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sYrPkd As String = SkiYr
        Dim sTName As String = TName
        Dim sSelEvent As String = ""  'Event Selected as Slalom Trick, Jump
        Dim sEventPkd As String = selEvent 'Event as S, T, or J
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
        Dim suUseTeams As Int16 = UseTeams
        Dim sSlalomHeader As String = ""
        Dim sTrickHeader As String = ""
        Dim sUnit As String = ""
        Dim sJumpHeader As String = ""
        Dim sLine As New StringBuilder
        Dim sMultiRndScores As String = ""
        Dim sSql As String = ""
        Dim sShowBuoys As String = ""
        Dim sTVidAvail As String = ""
        Dim sHasVideo As String = ""
        Select Case selEvent
            Case "S"
                sSelEvent = "Slalom"
                sSql = " PrSlalomScoresBestByDiv"
                sUnit = " Buoys"
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsSlalomOffered) + 1)
                    For j = 1 To RndsSlalomOffered
                        sRoundsHTML += "<td  Class=""table-primary"">Rnd&nbsp;" & j & "</td>" 'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Class</td><td>NOPS</td><td colspan=""2"">Details</td>"
                    sRndCols = "6"
                End If


            Case "T"
                sSelEvent = "Trick"
                sSql = "PrTrickScoresBestByDiv"
                sRndsThisEvent = sRndsTrickOffered
                sUnit = " Points"
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsTrickOffered) + 1)  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsTrickOffered
                        sRoundsHTML += "<td  Class=""table-primary"">Rnd&nbsp;" & j & "</td>"  'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td> Points </td><td>NOPS</td><td>Details</td><td>Time</td>"
                    sRndCols = "6"
                End If

            Case "J"
                sSelEvent = "Jump"
                sSql = "PrJumpScoresBestByDiv"
                sRndsThisEvent = sRndsJumpOffered
                sUnit = " Feet"
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsTrickOffered) + 1)  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsJumpOffered
                        sRoundsHTML += "<td  Class=""table-primary"">Rnd&nbsp;" & j & "</td>"    'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td>Class</td><td> Ft/M </td><td>NOPS</td><td>Details</td><td>Time</td>"
                    sRndCols = "6"
                End If
            Case Else  'Load all by default
                sMsg = "<td>Event Code out Of range</td></tr>"
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
                            'Show best score only for B1 and G1 slalom divisions
                            If (sDv = "B1" Or sDv = "G1") And selEvent = "S" Then
                                sShowBuoys = sScoreBest & " Buoys"
                            Else
                                sShowBuoys = ""
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

                            sHasVideo = ""
                            'ONLY for Trick - Show trick video flag if available- 
                            If selEvent = "T" Then
                                If Not IsDBNull(MyDataReader.Item("Pass1VideoURL")) Then
                                    sTVidAvail = "Y"
                                End If

                                If Not IsDBNull(MyDataReader.Item("Pass2VideoURL")) Then
                                    sTVidAvail = "Y"
                                End If
                                If sTVidAvail = "Y" Then
                                    sHasVideo = "<img src=""Images/Flag-green16.png"" alt=""Trick Video Available"" title=""Trick Video Available, Select skier on Entry List"" />"
                                End If
                            End If

                            If sTmpDv = "" Then
                                'Add the division header for first division
                                sLine.Append("<table class=""table""><tr class=""table-info""><td colspan=""" & sRndCols & """><span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span><b> &nbsp;" & sSelEvent & " " & sDv & "</b></td></tr>")
                                sLine.Append("<table class=""table""><tr><td class=""table-warning"" width=""25%""><b> Leader Board </b></td>" & sRoundsHTML)
                                sTmpDv = sDv
                            End If
                            'Get the first MemberID' first record in first pass through data
                            If stmpMemberID = "" Then stmpMemberID = sMemberID

                            If sTmpDv = sDv Then 'Continue in same Division
                                'Add the data line
                                sLine.Append("<tr><td class=""table-warning""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sTmpDv & "&EV=" & sEventPkd & "&TN=" & sTName & "")
                                sLine.Append("&FC=LB&FT=0&RP=1&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a><b> " & sShowBuoys & "</b>" & sHasVideo & "</td>")   '   
                                sMultiRndScores = ModDataAccess3.LBGetRndScores(sSanctionID, sMemberID, sSelEvent, sDv, sSelRnd, sRound, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sNopsScore)
                                If sMultiRndScores <> "Error" Then
                                    sLine.Append(sMultiRndScores)
                                    sMultiRndScores = ""
                                Else
                                    'FIX THIS ERROR TRAP
                                End If
                            Else 'Division changed.

                                sLine.Append("</table>")
                                stmpMemberID = sMemberID
                                sTmpDv = sDv
                                'start new division header
                                sLine.Append("<table class=""table""><tr class=""table-info""><td  colspan=""" & sRndCols & """><span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span><b> &nbsp;" & sSelEvent & " " & sDv & "</b></td></tr")
                                sLine.Append("<table class=""table""><tr><td class=""table-warning""  width=""25%""><b> Leader Board </b></td>" & sRoundsHTML)
                                'Add the data line
                                sLine.Append("<tr><td class=""table-warning""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & stmpMemberID & "&DV=" & sTmpDv & "&EV=" & sEventPkd & "&TN=" & sTName & "")
                                sLine.Append("&FC=LB&FT=0&RP=1&UN=0&UT=0&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a><b> " & sShowBuoys & "</b>" & sHasVideo & "</td>")   '   
                                sMultiRndScores = ModDataAccess3.LBGetRndScores(sSanctionID, sMemberID, sSelEvent, sDv, sSelRnd, sRound, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sNopsScore)
                                If sMultiRndScores <> "Error" Then
                                    sLine.Append(sMultiRndScores)
                                Else
                                    'FIX THIS ERROR TRAP
                                End If
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
                sMsg = "Error at LeaderBoardBestRndLeft"
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
    Friend Function GetEntryList(ByVal SanctionID As String, ByVal TournName As String, YrPkd As String) As String
        'As of v3.1.5  uses TRecap.aspx instead of TIndScores.aspx to display skier score detail.
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = Trim(SanctionID)
        Dim sTName As String = TournName
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

        Dim sTmpMemberID As String = ""
        Dim sEventsEntered As String = ""
        Dim sShowVidLink As String = ""

        Dim sSkierRow As New StringBuilder

        Dim sSkierLink As String = ""
        If Mid(sSanctionID, 3, 1) = "U" Then
            sCollegiate = True
            sCaption = " Team "
        End If
        Dim sSQLBuilder As New StringBuilder
        sSQLBuilder.Append("SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.AgeGroup, TR.AgeGroup AS Div, TR.City, TR.State, TR.Federation, ER.Event, ")
        sSQLBuilder.Append("TR.ReadyToSki, ER.TeamCode AS Team, CASE WHEN COALESCE  ")
                      sSQLBuilder.Append("((SELECT MIN(PK) AS TVPK  ")
                        sSQLBuilder.Append("FROM      TrickVideo AS TV  ")
                        sSQLBuilder.Append("WHERE   TV.SanctionId = TR.SanctionId AND TV.MemberId = TR.MemberId AND TV.AgeGroup = TR.AgeGroup AND (TV.Pass1VideoUrl IS NOT NULL OR  ")
        sSQLBuilder.Append("TV.Pass2VideoUrl IS NOT NULL)), 0) > 0 THEN 'Y' ELSE 'N' END AS TrickVideoAvailable  ")
        sSQLBuilder.Append("FROM     dbo.TourReg AS TR INNER JOIN  ")
                  sSQLBuilder.Append("dbo.EventReg AS ER ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup  ")

        sSQLBuilder.Append("WHERE  tr.SanctionID = '" & sSanctionID & "' and ((TR.Withdrawn IS NULL) OR(TR.Withdrawn = 'N') OR(TR.Withdrawn = ''))  ")
        sSQLBuilder.Append("order by tr.SkierName  ")
        sSQL = sSQLBuilder.ToString()
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
        Dim sTVidAvail As String = "N"
        Dim sHasVideo As String = ""
        Dim sTableWidth As String = "100%"
        Dim sTSB As New StringBuilder
        Dim sText As String = ""
        sTSB.Append("<table class=""table table-striped border-1"" width=" & sTableWidth & " cellpadding=""5"">")
        'space down from under header div
        sTSB.Append("<tr><td colspan=""4""><h4>&nbsp;</h4></td></tr>")
        sTSB.Append("<tr><td><b>Select Skier</b></td><td><b>Events Entered</b></td><td><b>Team</b></td><td><b>OK2Ski</b></td></tr>")
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
                            '                           sEventClass = CStr(MyDataReader.Item("EventClass"))
                            If UCase(sEvent) = "TRICK" Then
                                sTVidAvail = CStr(MyDataReader.Item("TrickVideoAvailable"))
                                sHasVideo = ""

                            End If
                            sTeam = "Not Set"
                            If Not IsDBNull(MyDataReader.Item("Team")) Then
                                If Len(Trim(MyDataReader.Item("Team"))) > 1 Then  'Make sure Team is not NULL or empty string
                                    sTeam = MyDataReader.Item("Team")
                                End If
                            End If
                            sReadyToSki = MyDataReader.Item("ReadyToSki")  ' holds Y or N


                            'set up first pass


                            If sTmpMemberID = "" Then  'First record 

                                sSkierLink = "<a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=A"
                                sSkierLink += "&FC=EL&FT=1&RP=0&UN=0&UT=0&TN=" & sTName & "&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>"
                                sFlag = "&nbsp; &nbsp;" & sReadyToSki

                                sTmpMemberID = sMemberID
                            End If
                            If sTmpMemberID = sMemberID Then
                                'Store other data in variables
                                sEventsEntered += ", " & sAgeGroup & " " & sEvent
                                If sReadyToSki = "N" Then
                                    sFlag = " &nbsp; ! SEE REGISTRAR ! &nbsp;"
                                End If
                                sShoTeam = sTeam
                                If UCase(sEvent) = "TRICK" Then
                                    If sTVidAvail = "Y" Then
                                        sHasVideo = "<img src=""Images/Flag-green16.png"" alt=""Trick Video Available"" title=""Trick Video Available, Select skier on Entry List"" />"
                                    End If
                                End If
                            Else  'new skier
                                'Close out previous skier
                                sLine = "<tr><td>" & sSkierLink & " " & sHasVideo & " </td><td>" & sEventsEntered & "</td><td>" & sShoTeam & "</td><td>" & sFlag & "</td></tr>"
                                sTSB.Append(sLine)
                                'Start new skier
                                sTmpMemberID = sMemberID
                                sSkierLink = "<a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=A"
                                sSkierLink += "&FC=EL&FT=1&RP=0&UN=0&UT=0&TN=" & sTName & "&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>"
                                sEventsEntered = ""

                                'Store other data in variables
                                sEventsEntered += sAgeGroup & " " & sEvent & " "
                                sFlag = "&nbsp; &nbsp;" & sReadyToSki
                                If sReadyToSki = "N" Then
                                    sFlag = " &nbsp; ! SEE REGISTRAR ! &nbsp;"
                                End If
                                sShoTeam = sTeam
                                If UCase(sEvent) = "TRICK" Then
                                    If sTVidAvail = "Y" Then
                                        sHasVideo = "<img src=""Images/Flag-green16.png"" alt=""Trick Video Available"" title=""Trick Video Available, Select skier on Entry List"" />"
                                    End If
                                End If
                            End If

                        Loop
                        sLine = "<tr><td>" & sSkierLink & " " & sHasVideo & " </td><td>" & sEventsEntered & "</td><td>" & sShoTeam & "</td><td>" & sFlag & "</td></tr>"
                        sTSB.Append(sLine)


                    Else
                        sTSB.Append("<tr><td colspan=""4"">No Skiers Found.</td></tr>")
                    End If 'end of has rows
                End Using
            Catch ex As Exception
                sMsg += "Error: at GetEntryList"
                sErrDetails = "GetEntryList Caught: " & ex.Message & " " & ex.StackTrace & "<br>SQL= " & sSQL

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
    Friend Function GetEntryListOLD(ByVal SanctionID As String, ByVal TournName As String, YrPkd As String) As String
        'As of v3.1.5  uses TRecap.aspx instead of TIndScores.aspx to display skier score detail.
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = Trim(SanctionID)
        Dim sTName As String = TournName
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
        Dim sSkierLink As String = ""
        If Mid(sSanctionID, 3, 1) = "U" Then
            sCollegiate = True
            sCaption = " Team "
        End If
        sSQL = "Select * from LivewebScoreBoard.dbo.vSkiersEntered Where SanctionID = '" & sSanctionID & "'  Order By SkierName "

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
        Dim sTVidAvail As String = "N"
        Dim sHasVideo As String = ""
        Dim sTableWidth As String = "100%"
        Dim sTSB As New StringBuilder
        Dim sText As String = ""
        sTSB.Append("<table class=""table table-striped border-1"" width=" & sTableWidth & " cellpadding=""5"">")
        '       sTSB.Append("<tr><td colspan=""4""><h4>Select a skier</h4></td></tr>")
        sTSB.Append("<tr><td><b>Select Skier</b></td><td><b>Age Group</b></td><td><b>Team</b></td><td><b>OK2Ski</b></td></tr>")
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
                            sTVidAvail = CStr(MyDataReader.Item("TrickVideoAvailable"))
                            sHasVideo = ""
                            If sTVidAvail = "Y" Then
                                sHasVideo = "<img src=""Images/Flag-green16.png"" alt=""Trick Video Available"" title=""Trick Video Available, Select skier on Entry List"" />"
                            End If
                            sTeam = "Not Set"
                            If Not IsDBNull(MyDataReader.Item("Team")) Then
                                If Len(Trim(MyDataReader.Item("Team"))) > 1 Then  'Make sure Team is not NULL or empty string
                                    sTeam = MyDataReader.Item("Team")
                                End If
                            End If
                            sReadyToSki = MyDataReader.Item("ReadyToSki")  ' holds Y or N

                            sSkierLink = "<a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sAgeGroup & "&EV=A"
                            sSkierLink += "&FC=EL&FT=1&RP=0&UN=0&UT=0&TN=" & sTName & "&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>"


                            '                            If FirstPass = True Then  'Include the first record
                            '                                sTmpSkierID = sMemberID
                            '                                sTmpTeam = sTeam
                            '                                FirstPass = False
                            '                                sTmpReadyToSki = sReadyToSki
                            '                            End If
                            '                            If sTmpSkierID <> sMemberID Then 'combine all events entered for same skiers.  May be entered in more than one AgeGroup
                            If sReadyToSki = "N" Then
                                sFlag = " &nbsp; ! SEE REGISTRAR ! &nbsp;"
                            Else
                                sFlag = "&nbsp; &nbsp;" & sReadyToSki
                            End If
                            sShoTeam = sTeam
                            sLine = "<tr><td>" & sSkierLink & " " & sHasVideo & " </td><td>" & sAgeGroup & "</td><td>" & sShoTeam & "</td><td>" & sFlag & "</td></tr>"
                            '                              sLine = "<tr><td><a runat=""server"" href=""TIndScores.aspx?SID=" & sSanctionID & "&MID= " & sMemberID & "&SY= " & sYrPkd & "&TN=" & sTournName & "&EV=" & sEvent & "&N=" & sSkierName & "&DV=" & sAgeGroup & """>" & sSkierName & "</a>" & sHasVideo & " </td><td>" & sAgeGroup & "</td><td>" & sShoTeam & "</td><td>" & sFlag & "</td></tr>"
                            sTSB.Append(sLine)
                            '                                sTmpEvent = sEvent
                            '                                sTmpSkierID = sMemberID
                            '                                sTmpEventClass = sEventClass
                            '                                sTmpAgeGroup = sAgeGroup
                            '                            sTmpReadyToSki = sReadyToSki
                            '                            sEnteredIn = sEvent & " Cls " & sEventClass
                            sEnteredIn = sEvent & " "
                            sLine = ""
                            sFlag = ""
                            '                               sTmpTeam = sTeam
                            sShoTeam = ""
                            '                            End If

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
        Dim sLastUpdateDate As String = ""
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
        sSQL += " TR.Federation, TR.City, Tr.State, ER.RankingScore, SR.LastUpdateDate "
        sSQL += " From LiveWebScoreboard.dbo.SlalomRecap SR "
        sSQL += " left join LiveWebScoreboard.dbo.SlalomScore SS On SR.SanctionID = SS.SanctionID And SR.MemberID = SS.MemberID "
        sSQL += " Left Join LiveWebScoreboard.dbo.TourReg TR on SR.SanctionID = TR.SanctionId And SR.MemberID = TR.MemberId "
        sSQL += " left join LiveWebScoreboard.dbo.EventReg ER on SR.sanctionID = ER.SanctionID and SR.MemberId = ER.MemberID "
        sSQL += "Where SR.SanctionId ='" & sSanctionID & "' AND  SR.MemberId='" & sMemberID & "' and SR.[Round] = SS.[Round] and ER.Event = 'Slalom'"
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
                            If IsDBNull(MyDataReader.Item("LastUpdateDate")) Then
                                sLastUpdateDate = ""
                            Else
                                sLastUpdateDate = CStr(MyDataReader.Item("LastUpdateDate"))
                            End If
                            If sTmpRound <> sRound Then  '


                                sText += "<tr class=""table-info""><td  colspan=""3"" class=""text-start""><b>" & sSkierName & "  " & sAgeGroup & " Ranking Score: " & sRankingScore & "</td><td colspan=""3""><b>&nbsp; &nbsp; &nbsp;  " & sCity & ",   " & sState & "   " & sFederation & "  </b></td></tr>"
                                sText += "<tr Class=""table-info""><td  colspan=""6""><b>Round " & sRound & " Slalom Recap " & " &nbsp; Class " & sEventClass & " &nbsp;&nbsp; <span Class=""bg-danger text-white"" >  UNOFFICIAL Score: " & sBuoys & " Buoys</span></b> as of " & sLastUpdateDate & "</td></tr>"

                                sText += "<tr><th>Score</th><th>Pass Detail</th><th>Reride</th><th>Protected</th><th>Class</th></tr></thead>"
                                sTmpRound = sRound
                            End If
                            sText += "<tr><td>" & sScore & "</td><td>" & sNote & "</td><td>" & sReride & "</td><td>" & sProtected & "</td><td>" & sEventClass & "</td></tr>"
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
        Dim sLastUpdateDate As String = ""
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
                            If IsDBNull(MyDataReader.Item("LastUpdateDate")) Then
                                sLastUpdateDate = ""
                            Else
                                sLastUpdateDate = CStr(MyDataReader.Item("LastUpdateDate"))
                            End If

                            If sTmpRound = 0 Then 'This is the first round. set up row in master table and round Table that holds both passes
                                sTmpRound = sRound
                                sTableMstr += "<tr><td>" 'row that will hold Rounds tables
                                sTableRound = "<Table class=""table"">"
                                sTableRound += "<tr><td Class=""table-primary""><b>Trick Recap: " & sSkierName & " &nbsp;" & sAgeGroup & " Class " & sEventClass & " Round " & sRound & "</b></td><td class=""table-primary""><span class=""bg-danger text-white"" >  UNOFFICIAL </span><b> &nbsp;Score " & sRoundScore & "</b> as of " & sLastUpdateDate & "</td></tr>"
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
                                sTableRound += "<tr><td Class=""table-primary""><b>Trick Recap: " & sSkierName & " &nbsp;" & sAgeGroup & " Class " & sEventClass & " Round " & sRound & "</b></td><td class=""table-primary""><span class=""bg-danger text-white"" >  UNOFFICIAL </span><b> &nbsp;Score " & sRoundScore & "</b> as of " & sLastUpdateDate & "</td></tr>"
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
        Dim sLastUpdateDate As String = ""

        ' 3 event scores in available data.
        ' 23S108, James Bryans,             OM, 000107150,  at least 1 round overall
        ' 23S108, Tristan Duplan-Fribourg, JM, 000181068, at least 1 round overall
        sSQL = "Select JR.SanctionID, JR.AgeGroup, JR.[round], JR.ScoreFeet, JR.ScoreMeters, JR.PassNum, JR.LastUpdateDate, "
        sSQL += " JR.Results, Jr.BoatSpeed, Jr.RampHeight, Jr.ScoreProt, JR.Reride, Jr.RerideReason, JS.EventClass "
        sSQL += " From JumpRecap JR "
        sSQL += " left join jumpscore JS on JR.Sanctionid = JS.SanctionID and JR.MemberID = JS.MemberID and JR.[Round] = JS.[Round]"
        sSQL += " Where JR.SanctionId ='" & sSanctionID & "' AND JR.MemberId='" & sMemberID & "' "
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
                            If IsDBNull(MyDataReader.Item("LastUpdateDate")) Then
                                sLastUpdateDate = ""
                            Else
                                sLastUpdateDate = CStr(MyDataReader.Item("LastUpdateDate"))
                            End If
                            If sTmpRound <> sRound Then
                                sText += "<tr class=""table-primary""><td colspan=""7""><span class=""bg-danger text-white"">  UNOFFICIAL </span> <b>Round " & sRound & " Distance for " & sSkierName & " " & sAgeGroup & "  Class " & sEventClass & " </b> as of " & sLastUpdateDate & "</td></tr>"
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

    Friend Function RecapOverall(ByVal SanctionID As String, ByVal MemberID As String, ByVal SkierName As String) As String
        'Pulled from wfwShowScoreRecap.php
        ' 
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sText As String = ""
        Dim sTitleLine As String = ""
        Dim sDataLine As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID
        Dim sAgeGroup As String = ""
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


        sSQL = "select * from vOverallResults where SanctionID ='" & sSanctionID & "'  and OverallScore > 0 and MemberID = '" & sMemberID & "'"

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



    Friend Function IndivSlalomResults(ByVal SanctionID As String, ByVal MemberID As String, ByVal SkierName As String) As String
        'Not used after v3.1.5 - switched to use Recaps
        Dim sReturn As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sMemberID As String = MemberID

        Dim SQL As String = ""
        SQL = "SELECT * from dbo.vSlalomResults Where SanctionID ='" & sSanctionID & "' And MemberID = '" & Trim(sMemberID) & "' ORDER BY AgeGroup, MemberID, Round "
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSkierName As String = SkierName
        Dim sAgeGroup As String = ""
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
                            If IsDBNull(MyDataReader.Item("EventScore")) Then
                                sEventScore = "0"
                            Else
                                sEventScore = CStr(MyDataReader.Item("EventScore"))
                            End If
                            sEventClass = CStr(MyDataReader.Item("EventClass"))

                            If IsDBNull(MyDataReader.Item("Round")) Then
                                sRound = "N/A"
                            Else
                                sRound = CStr(MyDataReader.Item("Round"))
                            End If

                            If IsDBNull(MyDataReader.Item("EventScoreDesc")) Then
                                sEventScoreDesc = "N/A"
                            Else
                                sEventScoreDesc = CStr(MyDataReader.Item("EventScoreDesc"))
                            End If


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

    Friend Function IndivJumpResults(ByVal SanctionID As String, ByVal MemberID As String, ByVal SkierName As String) As String
        'Not used after v3.1.5 - switched to use Recaps
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
        Dim sAgeGroup As String = ""
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
    Friend Function IndivTrickResults(ByVal SanctionID As String, ByVal MemberID As String, ByVal SkierName As String) As String
        'Not used after v3.1.5 - switched to use Recaps
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
        Dim sAgeGroup As String = ""
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

    Public Function GetReportList(ByVal SanctionID As String) As String
        'Produce comma separated list of divisions included by selected Event and round
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sReportTitle As String = ""
        Dim sReportFileUri As String = ""
        Dim sSql As String = "PrGetPublishReports"
        Dim sReportType As String = ""
        Dim sTmpReportType As String = ""
        Dim sEvent As String = ""
        Dim sTmpEvent As String = ""

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

        Dim sList As New StringBuilder
        sList.Append("<table class=""table"">")
        sList.Append("<thead><tr class=""table-primary"" ><th colspan=""2"">Official Published Reports</th></tr></thead>")

        Dim Cnnt As New OleDb.OleDbConnection(sConn)
        Dim cmdRead As New OleDb.OleDbCommand
        cmdRead.CommandType = CommandType.StoredProcedure
        cmdRead.CommandText = sSql
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InReportType", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InReportType").Size = 12
        cmdRead.Parameters("@InReportType").Value = "PDF"
        cmdRead.Parameters("@InReportType").Direction = ParameterDirection.Input

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
                            If Not IsDBNull(MyDataReader.Item("ReportTitle")) Then
                                sReportTitle = CStr(MyDataReader.Item("ReportTitle"))
                            Else
                                sReportTitle = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("ReportFileUri")) Then
                                sReportFileUri = CStr(MyDataReader.Item("ReportFileUri"))
                            Else
                                sReportFileUri = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("ReportType")) Then
                                sReportType = CStr(MyDataReader.Item("ReportType"))
                            Else
                                sReportFileUri = "N/A"
                            End If
                            If Not IsDBNull(MyDataReader.Item("Event")) Then
                                sEvent = CStr(MyDataReader.Item("Event"))
                            Else
                                sEvent = "N/A"
                            End If
                            'Build the Header
                            If sTmpReportType = "" Then  'Only occurs on first pass. set first header
                                sTmpReportType = sReportType
                                sTmpEvent = sEvent
                                sList.Append("<tr class=""table-info""><td colspan=""2"">Report Type: " & sReportType & " - Event:" & sEvent & " </td></tr>")
                            End If
                            If sTmpEvent <> sEvent Or sTmpReportType <> sTmpReportType Then 'If report type or event changes make a new header
                                sList.Append("<tr class=""table-info""><td colspan=""2"">Report Type: " & sReportType & " - Event: " & sEvent & " </td></tr>")
                                sTmpReportType = sReportType
                                sTmpEvent = sEvent
                            End If
                            'Build the list
                            sList.Append("<tr><td width=""5%"">&nbsp;</td><td><a href=""" & sReportFileUri & """ target=""_blank"" >" & sReportTitle & "</a></td></tr>")
                        Loop

                    Else 'No reports found
                        sList.Append("<tr><td>No Reports found for " & sSanctionID)
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
            sList.Append("</table>")
            sReturn = sList.ToString()
            Return sReturn
        End If
    End Function


    Public Function GetRunOrdercount(ByVal SanctionID As String, ByVal SlalomRound As String, ByVal TrickRounds As String, ByVal JumpRnds As String) As Array
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

    Public Function GetRunOrderXRnd(ByVal SanctionID As String, ByVal YrPkd As String, ByVal TName As String, ByVal selEvent As String, ByVal selDivision As String, ByVal RndPkd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNOPS As Int16, ByVal UseTeams As Int16, ByVal FormatCode As String, ByVal DisplayMetric As Int16) As String
        ' Called from TROxRnd.aspx when One or more rounds use different running orders and ALL rounds is selected.
        ' Calls appropriate functions to create tables to be passed back up.
        ' 'Produces Master table which includes separate tables for each round with name, dv, and score
        ' 'If A single round is specified  - Produces One table with specified round and enhanced horizontal display
        'Division, Event, and EventGroup can be individually specified
        'Provisiion included but not implemented to specify NOPS (0 or 1), TEAMS (0 or 1), Display Metric (0 = no, 1 = yes, 2 = display imperial and metric)
        Dim sMsg As String = ""
        Dim sErrMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sYrPkd As String = YrPkd
        Dim sTournName As String = TName
        Dim sDivisionCodePkd As String = selDivision
        Dim sSlalomRounds As String = RndsSlalomOffered
        Dim sTrickRounds As String = RndsTrickOffered
        Dim sJumpRounds As String = RndsJumpOffered
        Dim sEventPkd As String = selEvent
        Dim sSlalomRnds As String = RndsSlalomOffered
        Dim sTrickRnds As String = RndsTrickOffered
        Dim sJumpRnds As String = RndsJumpOffered
        Dim sUseNops As Int16 = UseNOPS
        Dim sUseTeams As Int16 = UseTeams
        Dim sFormatCode As String = FormatCode
        Dim sDisplayMetric As Int16 = DisplayMetric
        Dim sSelRnd As Int16 = 0
        Dim sRndPkd As Int16 = RndPkd
        Dim sMaxRnds As Int16 = 0
        Dim sSQL As String = ""
        Dim sColInnerHTML As New StringBuilder
        sColInnerHTML.Append("<table Class=""table"">")
        'MAKE PAGE LEVEL TABLE WITH A COLUMN FOR EACH ROW.  GENERATE A TABLE FOR EACH ROUND AND PUT IN MASTER TABLE COLUMNS.  LIKE TRICK SHEETS
        Select Case sEventPkd
            Case "S"
                If sRndPkd = 0 Then
                    For sSelRnd = 1 To sSlalomRnds
                        sMsg = ModDataAccess3.ScoresXMultiRunOrdHoriz(sSanctionID, sYrPkd, sTournName, "S", sDivisionCodePkd, sSelRnd, sSlalomRounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                        If Left(sMsg, 5) <> "Error" Then
                            sColInnerHTML.Append(sMsg)
                        Else
                            sErrMsg += sMsg
                        End If
                    Next
                Else
                    sMsg = ModDataAccess3.ScoresXMultiRunOrdHoriz(sSanctionID, sYrPkd, sTournName, "S", sDivisionCodePkd, sRndPkd, sSlalomrounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                    If Left(sMsg, 5) <> "Error" Then
                        sColInnerHTML.Append(sMsg)
                    End If
                End If
            Case "T"

                If sRndPkd = 0 Then
                    For sSelRnd = 1 To sSlalomRnds
                        sMsg = ModDataAccess3.ScoresXMultiRunOrdHoriz(sSanctionID, sYrPkd, sTournName, "T", sDivisionCodePkd, sSelRnd, sSlalomRounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                        If Left(sMsg, 5) <> "Error" Then
                            sColInnerHTML.Append(sMsg)
                        Else
                            sErrMsg += sMsg
                        End If
                    Next
                Else
                    sMsg = ModDataAccess3.ScoresXMultiRunOrdHoriz(sSanctionID, sYrPkd, sTournName, "T", sDivisionCodePkd, sRndPkd, sSlalomRounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                    If Left(sMsg, 5) <> "Error" Then
                        sColInnerHTML.Append(sMsg)
                    End If
                End If
            Case "J"
                If sRndPkd = 0 Then
                    For sSelRnd = 1 To sSlalomRnds
                        sMsg = ModDataAccess3.ScoresXMultiRunOrdHoriz(sSanctionID, sYrPkd, sTournName, "J", sDivisionCodePkd, sSelRnd, sSlalomRounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                        If Left(sMsg, 5) <> "Error" Then
                            sColInnerHTML.Append(sMsg)
                        Else
                            sErrMsg += sMsg
                        End If
                    Next
                Else
                    sMsg = ModDataAccess3.ScoresXMultiRunOrdHoriz(sSanctionID, sYrPkd, sTournName, "J", sDivisionCodePkd, sRndPkd, sSlalomRounds, sTrickRounds, sJumpRounds, sUseNops, sUseTeams, sFormatCode, sDisplayMetric)
                    If Left(sMsg, 5) <> "Error" Then
                        sColInnerHTML.Append(sMsg)
                    End If
                End If
        End Select

        sColInnerHTML.Append("</table>")


        If Len(sErrMsg) > 2 Then
            Return sErrMsg
            Exit Function
        End If
        Return sColInnerHTML.ToString()
    End Function
    Public Function ScoresXMultiRunOrdHoriz(ByVal SanctionID As String, ByVal YrPkd As String, ByVal TName As String, ByVal selEvent As String, ByVal selDivision As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNOPS As Int16, ByVal UseTeams As Int16, ByVal FormatCode As String, ByVal DisplayMetric As Int16) As String
        'Call this function event where multiple running orders are used and all rounds are to be displayed
        ' Sanction number and event are used to produce the page.
        ' The other variables passed in are used to provide navigation information when skier name is clicked and TRecap is opened.
        ' Code to handle indivisual round is left in but should never be reached.

        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sYrPkd As String = YrPkd
        Dim sPREventCode As String = selEvent
        Dim sSelRnd As String = selRnd
        Dim sSelDivision As String = selDivision
        Dim sDisplayMetric As Int16 = DisplayMetric
        Dim sSelFormat As String = "Best"
        '       Dim sUseNOPS As Int16 = 0
        '       Dim sShowTeams As Int16 = 0
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sEventScoreDescMetric As String = ""
        Dim sEventScoreDescImperial As String = ""
        Dim stmpMemberID As String = ""
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim k As Int16 = 0
        Dim sDv As String = ""
        Dim sTmpDV As String = ""
        Dim sEventClass As String = ""
        Dim sEventGroup As String = ""
        Dim sTmpEventGroup As String = ""
        Dim sEventScore As String = ""
        Dim sRnd As String = ""
        Dim sRunOrd As New StringBuilder
        Dim sTName As String = TName
        Dim sRoundsHTML As String = ""
        Dim sLine As New StringBuilder
        Dim sRndsSlalomOffered As String = RndsSlalomOffered
        Dim sRndsTrickOffered As String = RndsTrickOffered
        Dim sRndsJumpOffered As String = RndsJumpOffered
        Dim sRndsThisEvent As String = ""
        Dim sRndCols As String = 0
        Dim sSkierLink As String = ""
        Dim sRndCount As Int16 = 0
        Dim sPassCount As Int16 = 1


        Dim sSql As String = ""

        Select Case selEvent
            Case "S"
                sPREventCode = "Slalom"
                sSql = "PrSlalomScoresByRunOrder"
                sRndsThisEvent = sRndsSlalomOffered
                If sSelRnd = "0" Then   ' All Rounds
                    sRndCols = CStr(CInt(sRndsSlalomOffered))
                    j = 1
                    k = CInt(sRndsSlalomOffered)
                    '                    For j = 1 To RndsSlalomOffered
                    '                        sRoundsHTML += "<td  Class=""table-primary"">Rnd " & j & "</td>"
                    '                    Next
                Else 'This should never be reached.
                    j = selRnd
                    k = selRnd
                    sRoundsHTML += "<td  Class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
            Case "T"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresByRunOrder"
                sRndsThisEvent = sRndsTrickOffered
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsTrickOffered))  'Rnds + name + 2 for BestRnd
                    j = 1
                    k = CInt(sRndsTrickOffered)
                    '                For j = 1 To RndsTrickOffered
                    '                    sRoundsHTML += "<td  Class=""table-primary"">Rnd " & j & "</td>"
                    '                Next
                Else
                    j = selRnd
                    k = selRnd
                    sRoundsHTML += "<td  Class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
            Case "J"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresByRunOrder"
                sRndsThisEvent = sRndsJumpOffered

                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsJumpOffered))  'Rnds + name + 2 for BestRnd
                    j = 1
                    k = CInt(sRndsJumpOffered)
                    '                    For j = 1 To RndsJumpOffered
                    '                        sRoundsHTML += "<td  Class=""table-primary"">Rnd " & j & "</td>"
                    '                    Next
                Else
                    j = selRnd
                    k = selRnd
                    sRoundsHTML += "<td  Class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
            Case Else  'Load all by default
                sMsg += "Overall not available"
                Return sMsg
                Exit Function
        End Select
        ' Create Event Level table with rows for headers and enough columns to provide a subtable for each round
        Dim sEventTable As New StringBuilder
        Dim sRoundTable As New StringBuilder
        sEventTable.Append("<table class=""table"" width=""100%"" cellpadding=""0"" cellspacing=""0"">")
        sEventTable.Append("<tr class=""table-primary""><td colspan=""" & sRndCols & """>" & sPREventCode & " Running Orders X Round </td></tr>")
        sEventTable.Append("<tr><td>") 'first column in master table will contain round 1 table
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
        Using Cnnt
            '           For sRndCount = 1 To sRndCols
            For sRndCount = j To k
                cmdRead.CommandType = CommandType.StoredProcedure
                cmdRead.CommandText = sSql
                cmdRead.Parameters.Clear()
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
                cmdRead.Parameters("@InRnd").Value = sRndCount    '0 = All Rounds    sSelRnd
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
                Try
                    Using cmdRead
                        cmdRead.Connection = Cnnt 'New OleDbConnection(sConn)
                        If sPassCount = 1 Then 'Only open connection on first pass through
                            cmdRead.Connection.Open()
                            sPassCount = 0
                        End If
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
                                            If Not IsDBNull(MyDataReader.Item("EventScoreDescMeteric")) Then
                                                sEventScoreDesc = MyDataReader.Item("EventScoreDescMeteric")
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

                                ' Have data line
                                sSkierLink = "<a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&SY=" & sYrPkd & "&MID=" & sMemberID & "&DV=" & sDv & "&EV=" & selEvent & ""
                                sSkierLink += "&FC=RO&FT=1&RP=0&UN=0&UT=0&TN=" & sTName & "&SN=" & sSkierName & """ ><b>" & sSkierName & "</b></a>"

                                If sTmpEventGroup = "" Then 'set up first event group
                                    sTmpEventGroup = sEventGroup
                                    'Make event group header
                                    sRoundTable.Append("<table class = ""table"" width=""100%"" cellpadding=""0"" cellspacing=""0"">")
                                    sRoundTable.Append("<tr class=""table-info""><td colspan=""2""><b>Event Group: " & sEventGroup & " Round: " & sRndCount & "</b></td></tr>")
                                End If

                                If sTmpDV = "" Then
                                    sTmpDV = sDv
                                End If
                                'First skier in first event group or other skiers in current event group
                                If sTmpEventGroup = sEventGroup Then 'Same Event Group continue with skier lines
                                    sRoundTable.Append("<tr><td width=""25%"">" & sSkierLink & "</td><td><b>" & sDv & " - " & sEventScoreDesc & "</b></td></tr>")

                                Else 'Make new event group header
                                    sRoundTable.Append("<tr class=""table-info""><td colspan=""2""><b>Event Group: " & sEventGroup & " Round: " & sRndCount & "</b></td></tr>")
                                    'Add current skier
                                    sRoundTable.Append("<tr><td width=""25%"">" & sSkierLink & "</td><td><b>" & sDv & " - " & sEventScoreDesc & "</b></td></tr>")
                                    'Reset the variables
                                    sTmpEventGroup = sEventGroup
                                End If
                            Loop
                            'End of round  close out round table and add to Event table`
                            sRoundTable.Append("</table>")

                            sEventTable.Append(sRoundTable.ToString() & "</td>") 'End column for this round table in the event table
                            If CInt(sRndCols) > sRndCount Then
                                sEventTable.Append("<td>")
                            End If
                            sRoundTable.Clear()

                        Else 'No data
                            sLine.Append("No " & sPREventCode & "Skiers Found")
                        End If
                        MyDataReader.Close()  'have to close datareader before reusing with same command name
                    End Using

                Catch ex As Exception
                    sMsg = "Error at GetRunOrder"
                    sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
                End Try
                'Reset variables for new pass
                sMemberID = ""
                sTmpEventGroup = ""
                sTmpDV = ""
            Next
            'finished the round loop - close out last round table and event table
            sRoundTable.Append("</table>")
            sEventTable.Append(sRoundTable.ToString() & "</td></tr></table>")
        End Using  'End CnnT
        If Len(sMsg) > 2 Then
            Return sMsg
            Exit Function
        End If
        Dim sdebug As String = sEventTable.ToString()
        Return sEventTable.ToString()
    End Function


End Module
