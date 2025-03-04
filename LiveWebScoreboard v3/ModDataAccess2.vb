Imports System.Data.Common
Imports System.Data.OleDb
Imports System.Security.Policy

Module ModDataAccess2

    Public Function ScoresXDvMemRndHoriz(ByVal SanctionID As String, ByVal TName As String, ByVal selEvent As String, ByVal selDivision As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNops As Int16, ByVal UseTeams As Int16, ByVal FormatCode As String, Optional ByVal DisplayMetric As Int16 = 0) As String
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = selEvent
        Dim sDivisionCode As String = selDivision
        Dim sUseNops As Int16 = CInt(UseNops)
        Dim sDisplayMetric As Int16 = DisplayMetric
        Dim sUseTeams As Int16 = CInt(UseTeams)
        Dim sRndNum As Byte = selRnd
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sFormatCode As String = FormatCode
        Dim sSql As String = ModDataAccess2.BuildSQLpq(sSanctionID, sEventCode, sDivisionCode, sRndNum, sUseNops, sUseTeams, sFormatCode)  'Make sure Select works with , separated list
        If Left(sSql, 5) = "Error" Then
            Return "Error at BuildSQL"
            sErrDetails = sSql
            Exit Function
        End If

        Dim sReturn As String = ""
        Dim sPREventCode As String = selEvent
        Dim sSelRnd As String = selRnd
        Dim sSelDivision As String = selDivision
        Dim sSelFormat As String = "Best"
        Dim sShowTeams As Int16 = 0
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sEventScoreDesc As String = ""
        Dim sEventScoreDescMetric As String = ""
        Dim sEventScoreDescImperial As String = ""
        Dim stmpMemberID As String = ""
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim sDv As String = ""
        Dim sTmpDV As String = ""
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
        Dim sRndsThisEvent As String = ""
        Dim sRndCols As String = 0
        Dim sRowCount As Int16 = 0
        Dim sTmpRow As New StringBuilder
        sTmpRow.Clear()
        '       Dim sBestRndScore As String = ""
        '       Dim sBoldThis As String = ""
        '       Dim sBoldThisEnd As String = "</b>"
        '       Dim sRnd1Best As String = ""
        '       Dim sRnd2Best As String = ""
        '       Dim sRnd3Best As String = ""
        '       Dim sRnd4Best As String = ""
        '       Dim sRnd5Best As String = ""
        '       Dim sRnd6Best As String = ""
        Dim sEventScore As String = ""
        '       Dim sTmpEventScore As String = ""
        Dim sBestRndTable As String = "" 'Holds the best round table for this division
        Dim sFirstRowInDv As String = ""  'First line display for each division.  eventually holds <td rowspan="[skiers in DV]">
        Dim sFirstSkierInDV As Boolean = False
        Dim sMoreLines As String = ""   ' Second and any additional lines in that division

        Dim sOrderBy As String = ""
        sOrderBy = "Order By Round asc, AgeGroup,  EventScore DESC"
        If sUseNops <> 0 Then
            sOrderBy = " Order By Round asc, Gender, NopsScore DESC"
        End If

        Select Case selEvent
            Case "S"
                sPREventCode = "Slalom"
                '               sSql = "PrSlalomScoresByRunOrder"
                sRndsThisEvent = sRndsSlalomOffered
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsSlalomOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsSlalomOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"
                    Next
                Else
                    sRoundsHTML += "<td  class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
            Case "T"
                sPREventCode = "Trick"
                '                sSql = "PrTrickScoresByRunOrder"
                sRndsThisEvent = sRndsTrickOffered
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsTrickOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsTrickOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"
                    Next
                Else
                    sRoundsHTML += "<td  class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
            Case "J"
                sPREventCode = "Jump"
                '                sSql = "PrJumpScoresByRunOrder"
                sRndsThisEvent = sRndsJumpOffered
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsJumpOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsJumpOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"
                    Next
                Else
                    sRoundsHTML += "<td  class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
            Case "O"
                sMsg += "OVERALL IS NOT READY FOR PRIME TIME"
                Return sMsg
                Exit Function
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
        cmdRead.CommandType = CommandType.Text
        cmdRead.CommandText = sSql
        cmdRead.Parameters.Add("@InSanctionID", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InSanctionID").Size = 6
        cmdRead.Parameters("@InSanctionID").Value = sSanctionID
        cmdRead.Parameters("@InSanctionID").Direction = ParameterDirection.Input

        '       cmdRead.Parameters.Add("@InEvCode", OleDb.OleDbType.VarChar)
        '       cmdRead.Parameters("@InEvCode").Size = 12
        '       cmdRead.Parameters("@InEvCode").Value = sPREventCode
        '       cmdRead.Parameters("@InEvCode").Direction = ParameterDirection.Input

        '        cmdRead.Parameters.Add("@InRnd", OleDb.OleDbType.Char)
        '        cmdRead.Parameters("@InRnd").Size = 1
        '        cmdRead.Parameters("@InRnd").Value = sSelRnd    '0 = All Rounds    sSelRnd
        '        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input
        '
        '        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        '        cmdRead.Parameters("@InDV").Size = 3
        '        cmdRead.Parameters("@InDV").Value = sSelDivision  'sDv
        '        cmdRead.Parameters("@InDV").Direction = ParameterDirection.Input

        '       cmdRead.Parameters.Add("@InGroup", OleDb.OleDbType.VarChar)
        '       cmdRead.Parameters("@InGroup").Size = 3
        '       cmdRead.Parameters("@InGroup").Value = "ALL"   'sEventGroup  Don't have a droplist for Event Group so always all
        '       cmdRead.Parameters("@InGroup").Direction = ParameterDirection.Input

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
                            If sTmpEventGroup = "" Then sTmpEventGroup = sEventGroup
                            'this form works only if same running order is used for all rounds.
                            'Check this at Tournament.aspx level and adjust options accordingly
                            'First EventGroup - get list of included divisions

                            If stmpMemberID = "" Then stmpMemberID = sMemberID ' first record in first pass through data
                            If sTmpDV = "" Then sTmpDV = sDv
                            If sTmpEventGroup = sEventGroup Then
                                If sTmpDV = sDv Then ' Split divisions here

                                    If stmpMemberID = sMemberID Then 'Continuing in same division 
                                        If sSelRnd = 0 Then
                                            i += 1
                                            If sRnd > i Then  'If first score is in round 2 or greater - fill in earlier rounds as blanks
                                                Do Until sRnd = i
                                                    If i = 1 Then
                                                        sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td></td>")
                                                    Else
                                                        sTmpRow.Append("<td></td>")
                                                    End If
                                                    i += 1
                                                Loop
                                            End If
                                            Select Case sRnd 'get the data available for the specified event, Group, DV, and skier
                                                Case 1
                                                    sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                                Case 2
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 3
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 4
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 5
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 0  'error
                                                    sTmpRow.Append("<td>Rnd Error</td>")
                                            End Select
                                        Else 'only need score for selected round
                                            sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                        End If
                                    Else 'New skier
                                        If sSelRnd = 0 Then 'don't add columns if a round has been selected
                                            'fill in any empty <td></td> if skier did not ski all rounds
                                            If i <> sRndsThisEvent Then
                                                i += 1
                                                For i = i To RndsSlalomOffered
                                                    Select Case i
                                                        Case 2
                                                            sTmpRow.Append("<td></td>")
                                                        Case 3
                                                            sTmpRow.Append("<td></td>")
                                                        Case 4
                                                            sTmpRow.Append("<td></td>")
                                                        Case 5
                                                            sTmpRow.Append("<td></td>")
                                                    End Select
                                                Next
                                            End If
                                        End If
                                        sMoreLines += sTmpRow.ToString
                                        sTmpRow.Clear()
                                        'Finished previous row - start next skier in same division
                                        stmpMemberID = sMemberID
                                        sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                        i = 1  'first round is in sTmpRow
                                    End If
                                    sRowCount += 1
                                Else 'division changed 
                                    'close out previous Division
                                    If i <> sRndsThisEvent Then
                                        i += 1
                                        For i = i To RndsSlalomOffered
                                            Select Case i
                                                Case 2
                                                    sTmpRow.Append("<td></td>")
                                                Case 3
                                                    sTmpRow.Append("<td></td>")
                                                Case 4
                                                    sTmpRow.Append("<td></td>")
                                                Case 5
                                                    sTmpRow.Append("<td></td>")
                                            End Select
                                        Next
                                    End If
                                    sMoreLines += sTmpRow.ToString() 'Get last skier in previous division
                                    ' Assemble the division row in the Full Page Table  
                                    sLine.Append("<tr><td><Table Class=""table  border-1 "">")  'Make row for Division and Start Division table
                                    'Division header
                                    sLine.Append("<tr><td  class=""table-success"" width=""13%"" ><b>Run Order</b></td><td colspan=""" & (sRndCols) & """ class=""text-bg-primary"" >" & sPREventCode & " " & sTmpEventGroup & ", " & sTmpDV & ", " & sEventClass & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td><td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                                    sLine.Append("<tr><td  class=""table-success""  width=""13%"">Name</td>" & sRoundsHTML) 'Leave 2 cols for Best Rnd
                                    If sSelRnd = 0 Then
                                        '                                       sBestRndTable = ModDataAccess2.BestRndXEvDv(sSanctionID, sPREventCode, sTmpDV, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered)
                                        sBestRndTable = ModDataAccess2.LeaderBoard(sSanctionID, sPREventCode, sTmpDV, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sUseNops, sUseTeams, sSelFormat, sDisplayMetric)
                                        If sBestRndTable = "No Scores" Then sBestRndTable = " "
                                    Else
                                        sBestRndTable = ""
                                    End If
                                    'Error or table will be put in column
                                    sLine.Append("<td class=""align-top table-info"" colspan""2"" rowspan=""" & (sRowCount + 2) & """>" & sBestRndTable & "</td></tr>")
                                    'Skier details
                                    sLine.Append(sMoreLines) 'Adds the rest of the skiers in that division
                                    'Close the division table the row in the Full Page table for the division
                                    sLine.Append("</table></td></tr>")
                                    '                               sLine.Append("</table>")  Uncomment to debug one division
                                    '                                Return sLine.ToString()
                                    '
                                    ' Reset variables for next division
                                    sTmpDV = sDv
                                    stmpMemberID = sMemberID
                                    sMoreLines = ""
                                    sTmpRow.Clear()
                                    sRowCount = 0 'row count is implemented after row is complete
                                    'Start new row in Page level table
                                    sLine.Append("<tr><td>")
                                    'Use current data to start next division
                                    sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                    i = 1 'Row 1 is in sTmpRow
                                End If
                            Else 'EventGroupChanged

                                'close out previous Division
                                sMoreLines += sTmpRow.ToString() 'Get last skier in previous division
                                ' Assemble the division row in the Full Page Table  
                                sLine.Append("<tr><td><Table Class=""table  border-1 "">")  'Make row for Division and Start Division table
                                'Division header
                                sLine.Append("<tr><td  class=""table-success"" width=""13%"" ><b>Run Order</b></td><td class=""text-bg-primary""  colspan=""" & (sRndCols) & """>" & sPREventCode & " " & sTmpEventGroup & ", " & sTmpDV & ", " & sEventClass & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td><td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                                sLine.Append("<tr><td  class=""table-success"" width=""13%"">Name</td>" & sRoundsHTML) 'Leave 2 cols for Best Rnd
                                If sSelRnd = 0 Then
                                    sBestRndTable = ModDataAccess2.BestRndXEvDv(sSanctionID, sPREventCode, sTmpDV, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered)
                                    If sBestRndTable = "No Scores" Then sBestRndTable = " "
                                Else
                                    sBestRndTable = ""
                                End If
                                'Error or table will be put in column
                                sLine.Append("<td class=""align-top table-info"" colspan""2"" rowspan=""" & (sRowCount + 2) & """>" & sBestRndTable & "</td></tr>")
                                'Skier details
                                sLine.Append(sMoreLines) 'Adds the rest of the skiers in that division
                                'Close the division table the row in the Full Page table for the division
                                sLine.Append("</table></td></tr>")
                                ' Reset variables for next division
                                sTmpDV = sDv
                                stmpMemberID = sMemberID
                                sMoreLines = ""
                                sTmpRow.Clear()
                                sRowCount = 0 'row count is implemented after row is complete
                                'Start new row in Page level table
                                sLine.Append("<tr><td>")
                                'Use current data to start next division
                                sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                i = 1 'Row 1 is in sTmpRow
                                sTmpEventGroup = sEventGroup
                            End If
                        Loop
                        'END OF SINGLE DIVISION OR LAST OF MULTIPLE DIVISIONS 
                        'Last skier data is in tmpXXXX
                        'May have had 1 or moure rounds already collected in sMoreLines
                        If sSelRnd = 0 Then
                            'fill in any empty <td></td> if skier did not ski all rounds
                            If sRnd < sRndsThisEvent Then
                                i = sRnd + 1
                                For i = i To RndsSlalomOffered
                                    Select Case i
                                        Case 2
                                            sTmpRow.Append("<td></td>")
                                        Case 3
                                            sTmpRow.Append("<td></td>")
                                        Case 4
                                            sTmpRow.Append("<td></td>")
                                        Case 5
                                            sTmpRow.Append("<td></td>")
                                    End Select
                                Next
                            End If
                        End If
                        sMoreLines += sTmpRow.ToString
                        'Have last skier's performance by round
                        'close out previous Division'
                        ' Assemble the division row in the Full Page Table  
                        sLine.Append("<tr><td><Table Class=""table  border-1 "">")  'Make row for Division and Start Division table
                        'Division header
                        sLine.Append("<tr><td  class=""table-success""  width=""13%""><b>Run Order</b></td><td class=""text-bg-primary""  colspan=""" & (sRndCols) & """>" & sPREventCode & " " & sTmpEventGroup & ", " & sTmpDV & ", " & sEventClass & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td><td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                        sLine.Append("<tr><td  class=""table-primary"" width=""13%"">Name</td>" & sRoundsHTML) 'Leave 2 cols for Best Rnd
                        If sSelRnd = 0 Then
                            sBestRndTable = ModDataAccess2.BestRndXEvDv(sSanctionID, sPREventCode, sTmpDV, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered)
                            If sBestRndTable = "No Scores" Then sBestRndTable = ""
                        Else
                            sBestRndTable = ""
                        End If
                        'Error or table will be put in column
                        sLine.Append("<td class=""align-top table-info"" colspan""2"" rowspan=""" & (sRowCount + 2) & """>" & sBestRndTable & "</td></tr>")
                        'Skier details
                        sLine.Append(sMoreLines) 'Adds the rest of the skiers in that division
                        'Close the division table the row in the Full Page table for the division
                        sLine.Append("</table></td></tr>")
                        sLine.Append("</table>") 'Close out the full page table

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
        Return sLine.ToString()
    End Function

    Public Function BestRndXEvDv(ByVal SanctionID As String, ByVal selEvent As String, ByVal selDv As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String) As String
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
        Dim sUnit As String = ""
        Dim sJumpHeader As String = ""
        Dim sLine As New StringBuilder
        Dim sSql As String = ""
        Select Case selEvent
            Case "Slalom"
                sPREventCode = "Slalom"
                sSql = " PrSlalomScoresBestByDiv"
                sLine.Append("<Table Class=""table table-bordered border-start "">")
                sUnit = " Buoys"
            Case "Trick"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresBestByDiv"
                sLine.Append(" <Table Class=""table  table-bordered border-primary "">") '& sTrickHeader) 
                sUnit = " Points"
            Case "Jump"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresBestByDiv"
                sLine.Append("<Table Class=""table  table-bordered border-primary "">") '& sJumpHeader)  
                sUnit = " Feet"
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
        cmdRead.Parameters("@InDV").Value = sSelDV   'sDv
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
                                If sDv = "M1" Then
                                    sDv = sDv
                                End If
                                'Make a division header
                                sLine.Append("<tr class=""table-info""><td  colspan=""2""><b> " & sDv & "</b></td></tr>")
                                'Add the data line
                                sLine.Append("<tr class=""table-info""><td> " & sSkierName & "</td><td>" & sScoreBest & sUnit & "</td></tr>")
                                sTmpDv = sDv
                            Else
                                'Add the data line
                                sLine.Append("<tr  class=""table-info""><td> " & sSkierName & "</td><td>" & sScoreBest & sUnit & "</td></tr>")
                            End If
                        Loop
                        'Close the Leader board table for the specified division
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
            Return sLine.ToString()
        End If
    End Function


    Public Function GetDVsXEventGroup(ByVal SanctionID As String, ByVal EventCode As String, ByVal Rnd As String) As String
        'Produce comma separated list of divisions included by selected Event and round
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = EventCode
        Dim sSql As String = "PrCheckForCustomRunorder"
        'Accepts SanctionID and EventCode
        'Returns "" if event groups for this sanction number and event are the same for each round
        'Returns the Divisions included in each event group by round if they are different
        Dim sDV As String = ""
        Dim sRnd As String = ""
        Dim sEventGroup As String = ""

        Dim sDvList As String = ""
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
        cmdRead.Parameters("@InEvCode").Value = sEventCode
        cmdRead.Parameters("@InEvCode").Direction = ParameterDirection.Input

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
                            If Not IsDBNull(MyDataReader.Item("AgeGroup")) Then
                                sDV = CStr(MyDataReader.Item("AgeGroup"))
                            Else
                                sDV = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("Round")) Then
                                sRnd = CStr(MyDataReader.Item("Round"))
                            Else
                                sRnd = ""
                            End If
                            If Not IsDBNull(MyDataReader.Item("EventGroup")) Then
                                sEventGroup = CStr(MyDataReader.Item("EventGroup"))
                            Else
                                sEventGroup = ""
                            End If

                            'NEED CODE TO PRODUCE LIST OF DIVISIONS INCLUDED BY ROUND AND EVENTGROUP

                        Loop

                    Else
                        sDvList = ""
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
            Return sDvList
        End If
    End Function
    Public Function LeaderBoard(ByVal SanctionID As String, ByVal selEvent As String, ByVal selDv As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNops As Int16, ByVal UseTeams As Int16, ByVal selFormat As String, ByVal DisplayMetric As Int16) As String
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sPREventCode As String = selEvent
        Dim sSelDV As String = selDv
        Dim sSelFormat As String = selFormat
        Dim sDisplayMetric As Int16 = DisplayMetric
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
        Dim sUnit As String = ""
        Dim sJumpHeader As String = ""
        Dim sLine As New StringBuilder
        Dim sSql As String = ""
        Select Case selEvent
            Case "Slalom"
                sPREventCode = "Slalom"
                sSql = " PrLeaderBoard"
                sLine.Append("<Table Class=""table table-bordered border-start "">")
                sUnit = ""
            Case "Trick"
                sPREventCode = "Trick"
                sSql = "PrLeaderBoard"
                sLine.Append(" <Table Class=""table  table-bordered border-primary "">") '& sTrickHeader) 
                sUnit = " Points"
            Case "Jump"
                sPREventCode = "Jump"
                sSql = "PrLeaderBoard"
                sLine.Append("<Table Class=""table  table-bordered border-primary "">") '& sJumpHeader)  
                sUnit = " Feet"
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

        cmdRead.Parameters.Add("@InEvCode", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InEvCode").Size = 12
        cmdRead.Parameters("@InEvCode").Value = sPREventCode
        cmdRead.Parameters("@InEvCode").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InFormat", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InFormat").Size = 12
        cmdRead.Parameters("@InFormat").Value = sSelFormat   'sEventGroup
        cmdRead.Parameters("@InFormat").Direction = ParameterDirection.Input

        'If a specific round is selected this function is ignored or returns ""
        '       cmdRead.Parameters.Add("@InRnd", OleDb.OleDbType.Char)
        '       cmdRead.Parameters("@InRnd").Size = 1
        '       cmdRead.Parameters("@InRnd").Value = "0"    '0 = All Rounds    sSelRnd
        ''        cmdRead.Parameters("@InRnd").Direction = ParameterDirection.Input

        cmdRead.Parameters.Add("@InDV", OleDb.OleDbType.VarChar)
        cmdRead.Parameters("@InDV").Size = 3
        cmdRead.Parameters("@InDV").Value = sSelDV   'sDv
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
                            sSkierName = CStr(MyDataReader.Item("SkierName"))
                            If Not IsDBNull(MyDataReader.Item("DiV")) Then
                                sDv = CStr(MyDataReader.Item("DiV"))
                            Else
                                sDv = ""
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
                            If sTmpDv <> sDv Then

                                'Make a division header
                                sLine.Append("<tr class=""table-info""><td  colspan=""2""><b> " & sDv & "</b></td></tr>")
                                'Add the data line
                                sLine.Append("<tr class=""table-info""><td> " & sSkierName & "</td><td>" & sEventScoreDesc & "</td></tr>")
                                sTmpDv = sDv
                            Else
                                'Add the data line
                                sLine.Append("<tr  class=""table-info""><td> " & sSkierName & "</td><td>" & sEventScoreDesc & "</td></tr>")
                            End If
                        Loop
                        'Close the Leader board table for the specified division
                        sLine.Append("</table>")
                    Else
                        '      sLine.Append("<tr  class=""table-info""><td> " & sSkierName & "</td><td>No Scores</td></tr></table>")
                        sMsg = "No Scores"
                    End If

                End Using
            Catch ex As Exception
                sMsg = "Error at GetLeaderBoard"
                sErrDetails = sMsg & " " & ex.Message & " " & ex.StackTrace
            End Try

        End Using
        If Len(sMsg) > 2 Then
            Return sMsg
        Else
            Return sLine.ToString()
        End If

    End Function


    Private Function BuildSQLpq(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal RndNum As Byte, ByVal UseNops As Boolean, ByVal UseTeams As Boolean, ByVal FormatCode As String) As String
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
                sOrderBy = "Order by AgeGroup, MemberID, Round asc"
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
                '               sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, NopsScore, EventScore, EventClass, TeamCode, Round, Event, EventScoreDesc, CONVERT(VARCHAR(10), cast(InsertDate As Time),0) AS Time  "
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, Div,[EventGroup], NopsScore, EventScore, [EventScoreDescMeteric],[EventScoreDescImperial] "
                sSQL += "  ,EventScoreDesc,EventClass, TeamCode, [Round], Event,  FORMAT(InsertDate, 'MM/dd/yy HH:mm') as Time "
                sSQL += " From " & sViewName & " where SanctionID = ? " & sRoundText & sDvText & sOrderBy
            Case "T"
                sViewName = " LiveWebScoreBoard.dbo.vTrickResults "
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup,  Div,[EventGroup],  NopsScore, EventScore, EventScoreDesc, EventClass, TeamCode, Round, Event,   FORMAT(InsertDate, 'MM/dd/yy HH:mm') as Time   "
                sSQL += " From " & sViewName & " where SanctionID = ? " & sRoundText & sDvText & sOrderBy
            Case "J"
                sViewName = " LiveWebScoreBoard.dbo.vJumpResults "
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, Div,[EventGroup],  NopsScore, EventScore, EventScoreDesc, EventClass, TeamCode, Round, Event,   FORMAT(InsertDate, 'MM/dd/yy HH:mm') as Time   "
                sSQL += " From " & sViewName & " where SanctionID = ? " & sRoundText & sDvText & sOrderBy
            Case "O"
                sViewName = " LiveWebScoreBoard.dbo.vOverallResults "
                sOrderBy = " Order By AgeGroup, EventScore Desc"
                sSQL = "Select SanctionID, SkierName, MemberID, AgeGroup, [round], OverallScore as EventScore, SlalomNopsScore, TrickNopsScore, JumpNOPSScore  "
                sSQL += " From " & sViewName & " where SanctionID = ? " ' " & sRoundText & sDvText & " Order By Round asc, AgeGroup,  EventScore DESC"
        End Select
        Return sSQL
    End Function
    Private Function BuildSQL2(ByVal SanctionID As String, ByVal EventCode As String, ByVal DivisionCode As String, ByVal RndNum As Byte, ByVal UseNops As Boolean, ByVal UseTeams As Boolean, ByVal FormatCode As String) As String
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
                sOrderBy = "Order by AgeGroup, MemberID, Round asc"
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
                '               sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, NopsScore, EventScore, EventClass, TeamCode, Round, Event, EventScoreDesc, CONVERT(VARCHAR(10), cast(InsertDate As Time),0) AS Time  "
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, Div,[EventGroup], NopsScore, EventScore, [EventScoreDescMeteric],[EventScoreDescImperial] "
                sSQL += "  ,EventScoreDesc,EventClass, TeamCode, [Round], Event,  FORMAT(InsertDate, 'MM/dd/yy HH:mm') as Time "
                sSQL += " From " & sViewName & " where SanctionID = '" & sSanctionID & "' " & sRoundText & sDvText & sOrderBy
            Case "T"
                sViewName = " LiveWebScoreBoard.dbo.vTrickResults "
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup,  Div,[EventGroup],  NopsScore, EventScore, EventScoreDesc, EventClass, TeamCode, Round, Event,   FORMAT(InsertDate, 'MM/dd/yy HH:mm') as Time   "
                sSQL += " From " & sViewName & " where SanctionID = '" & sSanctionID & "' " & sRoundText & sDvText & sOrderBy
            Case "J"
                sViewName = " LiveWebScoreBoard.dbo.vJumpResults "
                sSQL = "Select SanctionID, SkierName, MemberID, Gender, AgeGroup, Div,[EventGroup],  NopsScore, EventScore, EventScoreDesc, EventClass, TeamCode, Round, Event,   FORMAT(InsertDate, 'MM/dd/yy HH:mm') as Time   "
                sSQL += " From " & sViewName & " where SanctionID = '" & sSanctionID & "' " & sRoundText & sDvText & sOrderBy
            Case "O"
                sViewName = " LiveWebScoreBoard.dbo.vOverallResults "
                sOrderBy = " Order By AgeGroup, EventScore Desc"
                sSQL = "Select SanctionID, SkierName, MemberID, AgeGroup, [round], OverallScore as EventScore, SlalomNopsScore, TrickNopsScore, JumpNOPSScore  "
                sSQL += " From " & sViewName & " where SanctionID = '" & sSanctionID & "' " & sRoundText & sDvText & " Order By Round asc, AgeGroup,  EventScore DESC"
        End Select
        Return sSQL
    End Function
    Public Function ScoresXDvRnd(ByVal SanctionID As String, ByVal TName As String, ByVal selEvent As String, ByVal selDivision As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNops As Int16, ByVal UseTeams As Int16, ByVal FormatCode As String, Optional ByVal DisplayMetric As Int16 = 0) As String

        'NOTE - NOT USED AS OF 3/8/2024.  BUILDS OLD DIVISION LIST FORMAT WITH ADDED FIELDS AVAILABLE. REPLACED BY LEADERBOARD FORMAT
        Dim sSanctionID As String = SanctionID
        Dim sEventCode As String = selEvent
        Dim sDivisionCode As String = selDivision
        Dim sUseNops As Int16 = CInt(UseNops)
        Dim sDisplayMetric As Int16 = DisplayMetric
        Dim sUseTeams As Int16 = CInt(UseTeams)
        Dim sRndNum As Byte = selRnd
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sFormatCode As String = FormatCode
        Dim sSql As String = ModDataAccess2.BuildSQL2(sSanctionID, sEventCode, sDivisionCode, sRndNum, sUseNops, sUseTeams, sFormatCode)  'Make sure Select works with , separated list
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
    Public Function ScoresXRunOrdHorizCLB(ByVal SanctionID As String, ByVal TName As String, ByVal selEvent As String, ByVal selDivision As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, Optional ByVal DisplayMetric As Int16 = 0) As String
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sStopHere As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sPREventCode As String = selEvent
        Dim sSelRnd As String = selRnd
        Dim sSelDivision As String = selDivision
        Dim sSelFormat As String = "Best"
        Dim sUseNOPS As Int16 = 0
        Dim sUseTeams As Int16 = 0
        Dim sShowTeams As Int16 = 0
        Dim sSkierName As String = ""
        Dim sMemberID As String = ""
        Dim sDisplayMetric As Int16 = DisplayMetric
        Dim sEventScoreDesc As String = ""
        Dim sEventScoreDescMetric As String = ""
        Dim sEventScoreDescImperial As String = ""
        Dim stmpMemberID As String = ""
        Dim i As Int16 = 0
        Dim j As Int16 = 0
        Dim sDv As String = ""
        Dim sTmpDV As String = ""
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
        Dim sRndsThisEvent As String = ""
        Dim sRndCols As String = 0
        Dim sRowCount As Int16 = 0
        Dim sTmpRow As New StringBuilder
        sTmpRow.Clear()
        '       Dim sBestRndScore As String = ""
        '       Dim sBoldThis As String = ""
        '       Dim sBoldThisEnd As String = "</b>"
        '       Dim sRnd1Best As String = ""
        '       Dim sRnd2Best As String = ""
        '       Dim sRnd3Best As String = ""
        '       Dim sRnd4Best As String = ""
        '       Dim sRnd5Best As String = ""
        '       Dim sRnd6Best As String = ""
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

        Select Case selEvent
            Case "S"
                sPREventCode = "Slalom"
                sSql = "PrSlalomScoresByRunOrder"
                sRndsThisEvent = sRndsSlalomOffered
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsSlalomOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsSlalomOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"
                    Next
                Else
                    sRoundsHTML += "<td  class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
            Case "T"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresByRunOrder"
                sRndsThisEvent = sRndsTrickOffered
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsTrickOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsTrickOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"
                    Next
                Else
                    sRoundsHTML += "<td  class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
            Case "J"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresByRunOrder"
                sRndsThisEvent = sRndsJumpOffered
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsJumpOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsJumpOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"
                    Next
                Else
                    sRoundsHTML += "<td  class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
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
        cmdRead.Parameters("@InRnd").Value = sSelRnd    '0 = All Rounds    sSelRnd
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
                            If sTmpEventGroup = "" Then sTmpEventGroup = sEventGroup
                            'this form works only if same running order is used for all rounds.
                            'Check this at Tournament.aspx level and adjust options accordingly
                            'First EventGroup - get list of included divisions

                            If stmpMemberID = "" Then stmpMemberID = sMemberID ' first record in first pass through data
                            If sTmpDV = "" Then sTmpDV = sDv
                            If sTmpEventGroup = sEventGroup Then
                                If sTmpDV = sDv Then ' Split divisions here

                                    If stmpMemberID = sMemberID Then 'Continuing in same division 
                                        If sSelRnd = 0 Then
                                            i += 1
                                            If sRnd > i Then  'If first score is in round 2 or greater - fill in earlier rounds as blanks
                                                Do Until sRnd = i
                                                    If i = 1 Then
                                                        sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                                    Else
                                                        sTmpRow.Append("<td></td>")
                                                    End If
                                                    i += 1
                                                Loop
                                            End If
                                            Select Case sRnd 'get the data available for the specified event, Group, DV, and skier
                                                Case 1
                                                    sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                                Case 2
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 3
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 4
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 5
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 0  'error
                                                    sTmpRow.Append("<td>Rnd Error</td>")
                                            End Select

                                        Else 'only need score for selected round
                                            sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                        End If
                                    Else 'New skier
                                        If sSelRnd = 0 Then 'don't add columns if a round has been selected
                                            'fill in any empty <td></td> if skier did not ski all rounds
                                            If i <> sRndsThisEvent Then
                                                i += 1
                                                For i = i To RndsSlalomOffered
                                                    Select Case i
                                                        Case 2
                                                            sTmpRow.Append("<td></td>")
                                                        Case 3
                                                            sTmpRow.Append("<td></td>")
                                                        Case 4
                                                            sTmpRow.Append("<td></td>")
                                                        Case 5
                                                            sTmpRow.Append("<td></td>")
                                                    End Select
                                                Next
                                            End If
                                        End If
                                        sMoreLines += sTmpRow.ToString
                                        sTmpRow.Clear()
                                        'Finished previous row - start next skier in same division
                                        stmpMemberID = sMemberID
                                        sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                        i = 1  'first round is in sTmpRow
                                    End If
                                    sRowCount += 1
                                Else 'division changed 
                                    'close out previous Division
                                    If i <> sRndsThisEvent Then
                                        i += 1
                                        For i = i To RndsSlalomOffered
                                            Select Case i
                                                Case 2
                                                    sTmpRow.Append("<td></td>")
                                                Case 3
                                                    sTmpRow.Append("<td></td>")
                                                Case 4
                                                    sTmpRow.Append("<td></td>")
                                                Case 5
                                                    sTmpRow.Append("<td></td>")
                                            End Select
                                        Next
                                    End If
                                    sMoreLines += sTmpRow.ToString() 'Get last skier in previous division
                                    ' Assemble the division row in the Full Page Table  
                                    sLine.Append("<tr><td><Table Class=""table  border-1 "">")  'Make row for Division and Start Division table
                                    'Division header
                                    sLine.Append("<tr><td  class=""table-success"" width=""13%"" ><b>Run Order</b></td><td colspan=""" & (sRndCols) & """ class=""text-bg-primary"" >" & sPREventCode & " " & sTmpEventGroup & ", " & sTmpDV & ", " & sEventClass & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td><td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                                    sLine.Append("<tr><td  class=""table-success""  width=""13%"">Name</td>" & sRoundsHTML) 'Leave 2 cols for Best Rnd
                                    If sSelRnd = 0 Then
                                        '                                       sBestRndTable = ModDataAccess2.BestRndXEvDv(sSanctionID, sPREventCode, sTmpDV, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered)
                                        sBestRndTable = ModDataAccess2.LeaderBoard(sSanctionID, sPREventCode, sTmpDV, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sUseNOPS, suseteams, sSelFormat, sDisplayMetric)
                                        If sBestRndTable = "No Scores" Then sBestRndTable = " "
                                    Else
                                        sBestRndTable = ""
                                    End If
                                    'Error or table will be put in column
                                    sLine.Append("<td class=""align-top table-info"" colspan""2"" rowspan=""" & (sRowCount + 2) & """>" & sBestRndTable & "</td></tr>")
                                    'Skier details
                                    sLine.Append(sMoreLines) 'Adds the rest of the skiers in that division
                                    'Close the division table the row in the Full Page table for the division
                                    sLine.Append("</table></td></tr>")
                                    '                               sLine.Append("</table>")  Uncomment to debug one division
                                    '                                Return sLine.ToString()
                                    '
                                    ' Reset variables for next division
                                    sTmpDV = sDv
                                    stmpMemberID = sMemberID
                                    sMoreLines = ""
                                    sTmpRow.Clear()
                                    sRowCount = 0 'row count is implemented after row is complete
                                    'Start new row in Page level table
                                    sLine.Append("<tr><td>")
                                    'Use current data to start next division
                                    sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                    i = 1 'Row 1 is in sTmpRow
                                End If
                            Else 'EventGroupChanged

                                'close out previous Division
                                sMoreLines += sTmpRow.ToString() 'Get last skier in previous division
                                ' Assemble the division row in the Full Page Table  
                                sLine.Append("<tr><td><Table Class=""table  border-1 "">")  'Make row for Division and Start Division table
                                'Division header
                                sLine.Append("<tr><td  class=""table-success"" width=""13%"" ><b>Run Order</b></td><td class=""text-bg-primary""  colspan=""" & (sRndCols) & """>" & sPREventCode & " " & sTmpEventGroup & ", " & sTmpDV & ", " & sEventClass & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td><td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                                sLine.Append("<tr><td  class=""table-success"" width=""13%"">Name</td>" & sRoundsHTML) 'Leave 2 cols for Best Rnd
                                If sSelRnd = 0 Then
                                    sBestRndTable = ModDataAccess2.BestRndXEvDv(sSanctionID, sPREventCode, sTmpDV, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered)
                                    If sBestRndTable = "No Scores" Then sBestRndTable = " "
                                Else
                                    sBestRndTable = ""
                                End If
                                'Error or table will be put in column
                                sLine.Append("<td class=""align-top table-info"" colspan""2"" rowspan=""" & (sRowCount + 2) & """>" & sBestRndTable & "</td></tr>")
                                'Skier details
                                sLine.Append(sMoreLines) 'Adds the rest of the skiers in that division
                                'Close the division table the row in the Full Page table for the division
                                sLine.Append("</table></td></tr>")
                                ' Reset variables for next division
                                sTmpDV = sDv
                                stmpMemberID = sMemberID
                                sMoreLines = ""
                                sTmpRow.Clear()
                                sRowCount = 0 'row count is implemented after row is complete
                                'Start new row in Page level table
                                sLine.Append("<tr><td>")
                                'Use current data to start next division
                                sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                i = 1 'Row 1 is in sTmpRow
                                sTmpEventGroup = sEventGroup
                            End If
                        Loop
                        'END OF SINGLE DIVISION OR LAST OF MULTIPLE DIVISIONS 
                        'Last skier data is in tmpXXXX
                        'May have had 1 or moure rounds already collected in sMoreLines
                        If sSelRnd = 0 Then
                            'fill in any empty <td></td> if skier did not ski all rounds
                            If sRnd < sRndsThisEvent Then
                                i = sRnd + 1
                                For i = i To RndsSlalomOffered
                                    Select Case i
                                        Case 2
                                            sTmpRow.Append("<td></td>")
                                        Case 3
                                            sTmpRow.Append("<td></td>")
                                        Case 4
                                            sTmpRow.Append("<td></td>")
                                        Case 5
                                            sTmpRow.Append("<td></td>")
                                    End Select
                                Next
                            End If
                        End If
                        sMoreLines += sTmpRow.ToString
                        'Have last skier's performance by round
                        'close out previous Division'
                        ' Assemble the division row in the Full Page Table  
                        sLine.Append("<tr><td><Table Class=""table  border-1 "">")  'Make row for Division and Start Division table
                        'Division header
                        sLine.Append("<tr><td  class=""table-success""  width=""13%""><b>Run Order</b></td><td class=""text-bg-primary""  colspan=""" & (sRndCols) & """>" & sPREventCode & " " & sTmpEventGroup & ", " & sTmpDV & ", " & sEventClass & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td><td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                        sLine.Append("<tr><td  class=""table-primary"" width=""13%"">Name</td>" & sRoundsHTML) 'Leave 2 cols for Best Rnd
                        If sSelRnd = 0 Then
                            sBestRndTable = ModDataAccess2.BestRndXEvDv(sSanctionID, sPREventCode, sTmpDV, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered)
                            If sBestRndTable = "No Scores" Then sBestRndTable = ""
                        Else
                            sBestRndTable = ""
                        End If
                        'Error or table will be put in column
                        sLine.Append("<td class=""align-top table-info"" colspan""2"" rowspan=""" & (sRowCount + 2) & """>" & sBestRndTable & "</td></tr>")
                        'Skier details
                        sLine.Append(sMoreLines) 'Adds the rest of the skiers in that division
                        'Close the division table the row in the Full Page table for the division
                        sLine.Append("</table></td></tr>")
                        sLine.Append("</table>") 'Close out the full page table

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
        Return sLine.ToString()
    End Function
    Public Function LeaderBoardBestRndLeft(ByVal SanctionID As String, ByVal TName As String, ByVal selEvent As String, ByVal selDv As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, ByVal UseNOPS As Int16, ByVal UseTeams As Int16, ByVal selFormat As String, ByVal DisplayMetric As Int16) As String
        'This function is run for each event selected based on code in TLeaderBoard_Load and Btn_Update
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sStopHere As String = ""
        Dim sSanctionID As String = SanctionID
        Dim sTName As String = TName
        Dim sSelEvent As String = selEvent  'Event Selected
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
        Select Case selEvent
            Case "S"
                sSelEvent = "Slalom"
                sSql = " PrSlalomScoresBestByDiv"
                sUnit = " Buoys"
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsSlalomOffered) + 1)
                    For j = 1 To RndsSlalomOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>" 'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td>NOPS</td><td>Details</td><td>Time</td>"
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
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"  'completes the event header with appropriate number of rounds columns
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
                    For j = 1 To RndsTrickOffered
                        sRoundsHTML += "<td  Class=""table-primary"">Rnd " & j & "</td>"    'completes the event header with appropriate number of rounds columns
                    Next
                    sRoundsHTML += "</tr>"
                Else
                    sRoundsHTML += "<td>Rnd " & sSelRnd & "</td><td>Class</td><td> Ft/M </td><td>NOPS</td><td>Details</td><td>Time</td>"
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
                                sNOPSScore = MyDataReader.Item("NOPSScore")
                            Else
                                sNOPSScore = ""
                            End If
                            If sTmpDv = "" Then
                                'Add the division header for first division
                                sLine.Append("<table class=""table""><tr class=""table-info""><td colspan=""" & sRndCols & """><span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span><b> &nbsp;" & sSelEvent & " " & sDv & "</b></td></tr>")
                                '     sLine.Append("<table class=""table""><tr><td class=""table-warning"" colspan=""2"" width=""15%""><b> Leader Board </b></td>" & sRoundsHTML)
                                sLine.Append("<table class=""table""><tr><td class=""table-warning"" width=""25%""><b> Leader Board </b></td>" & sRoundsHTML)
                                sTmpDv = sDv
                            End If
                            'Get the first MemberID' first record in first pass through data
                            If stmpMemberID = "" Then stmpMemberID = sMemberID

                            If sTmpDv = sDv Then 'Continue in same Division
                                'Add the data line
                                sLine.Append("<tr><td class=""table-warning""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDv & "&EV=" & sSelEvent & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a><b> " & sScoreBest & sUnit & "</b></td>")   '   
                                sMultiRndScores = ModDataAccess2.LBGetRndScores(sSanctionID, sMemberID, sSelEvent, sDv, sSelRnd, sRound, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sNopsScore)
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
                                '      sLine.Append("<table class=""table""><tr><td class=""table-warning"" colspan=""2"" width=""15%""><b> Leader Board </b></td>" & sRoundsHTML)
                                sLine.Append("<table class=""table""><tr><td class=""table-warning""  width=""25%""><b> Leader Board </b></td>" & sRoundsHTML)
                                'Add the data line
                                sLine.Append("<tr><td class=""table-warning""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDv & "&EV=" & sSelEvent & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a><b> " & sScoreBest & sUnit & "</b></td>")   '   
                                sMultiRndScores = ModDataAccess2.LBGetRndScores(sSanctionID, sMemberID, sSelEvent, sDv, sSelRnd, sRound, sRndsSlalomOffered, sRndsTrickOffered, sRndsJumpOffered, sNopsScore)
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

    Public Function ScoresXRunOrdHoriz2(ByVal SanctionID As String, ByVal TName As String, ByVal selEvent As String, ByVal selDivision As String, ByVal selRnd As String, ByVal RndsSlalomOffered As String, ByVal RndsTrickOffered As String, ByVal RndsJumpOffered As String, Optional ByVal DisplayMetric As Int16 = 0) As String
        'NO LeaderBoard column  3/9/2024 has space for more data
        Dim sReturn As String = ""
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sSanctionID As String = SanctionID
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
        Dim sRowCount As Int16 = 0
        Dim sTmpRow As New StringBuilder
        sTmpRow.Clear()
        Dim sMoreLines As String = ""   ' Second and any additional lines in that division

        Dim sSql As String = ""

        Select Case selEvent
            Case "S"
                sPREventCode = "Slalom"
                sSql = "PrSlalomScoresByRunOrder"
                sRndsThisEvent = sRndsSlalomOffered
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsSlalomOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsSlalomOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"
                    Next
                Else
                    sRoundsHTML += "<td  class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
            Case "T"
                sPREventCode = "Trick"
                sSql = "PrTrickScoresByRunOrder"
                sRndsThisEvent = sRndsTrickOffered
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsTrickOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsTrickOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"
                    Next
                Else
                    sRoundsHTML += "<td  class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
            Case "J"
                sPREventCode = "Jump"
                sSql = "PrJumpScoresByRunOrder"
                sRndsThisEvent = sRndsJumpOffered
                If sSelRnd = "0" Then
                    sRndCols = CStr(CInt(sRndsJumpOffered))  'Rnds + name + 2 for BestRnd
                    For j = 1 To RndsJumpOffered
                        sRoundsHTML += "<td  class=""table-primary"">Rnd " & j & "</td>"
                    Next
                Else
                    sRoundsHTML += "<td  class=""table-primary"">Rnd " & sSelRnd & "</td>"
                    sRndCols = "1"
                End If
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
        cmdRead.Parameters("@InRnd").Value = sSelRnd    '0 = All Rounds    sSelRnd
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
                            If sTmpEventGroup = "" Then sTmpEventGroup = sEventGroup
                            'this form works only if same running order is used for all rounds.
                            'Check this at Tournament.aspx level and adjust options accordingly
                            'First EventGroup - get list of included divisions

                            If stmpMemberID = "" Then stmpMemberID = sMemberID ' first record in first pass through data
                            If sTmpDV = "" Then sTmpDV = sDv

                            If sTmpDV = sDv Then ' Split divisions here

                                    If stmpMemberID = sMemberID Then 'Continuing in same division 
                                        If sSelRnd = 0 Then
                                            i += 1
                                            If sRnd > i Then  'If first score is in round 2 or greater - fill in earlier rounds as blanks
                                                Do Until sRnd = i
                                                    sTmpRow.Append("<td></td>")
                                                    i += 1
                                                Loop
                                            End If
                                            Select Case sRnd 'get the data available for the specified event, Group, DV, and skier
                                                Case 1
                                                    sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                                Case 2
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 3
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 4
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 5
                                                    sTmpRow.Append("<td>" & sEventScoreDesc & "</td>")
                                                Case 0  'error
                                                    sTmpRow.Append("<td>Rnd Error</td>")
                                            End Select

                                        Else 'only need score for selected round
                                            sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td><td>" & sEventScoreDesc & "</td>")
                                        End If
                                    Else 'New skier
                                        If sSelRnd = 0 Then 'don't add columns if a round has been selected
                                            'fill in any empty <td></td> if skier did not ski all rounds
                                            If i < sRndsThisEvent Then
                                                Do Until i = sRndsThisEvent
                                                    sTmpRow.Append("<td></td>")
                                                    i += 1
                                                Loop
                                                sTmpRow.Append("</tr>") 'close out the current row
                                            End If
                                        End If
                                        sMoreLines += sTmpRow.ToString
                                        sTmpRow.Clear()
                                        'Finished previous row - start next skier in same division
                                        stmpMemberID = sMemberID
                                        'comlete first round of new skier
                                        sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td>")
                                        If sRnd = 1 Then
                                            sTmpRow.Append("<td>" & sEventScoreDesc & "</td></tr>")
                                        Else
                                            sTmpRow.Append("<td> </td></tr>")
                                        End If
                                        i = 1  'first round is in sTmpRow
                                    End If
                                    sRowCount += 1
                                Else 'division changed 
                                    'close out previous skier
                                    If selRnd = 0 Then 'including all rounds - finish out row
                                        If i < sRndsThisEvent Then
                                            i += 1
                                            For i = i To RndsSlalomOffered
                                                Select Case i
                                                    Case 2
                                                        sTmpRow.Append("<td></td>")
                                                    Case 3
                                                        sTmpRow.Append("<td></td>")
                                                    Case 4
                                                        sTmpRow.Append("<td></td>")
                                                    Case 5
                                                        sTmpRow.Append("<td></td>")
                                                End Select
                                            Next
                                            sTmpRow.Append("</tr>") 'close out the current row
                                        End If
                                    End If
                                    sMoreLines += sTmpRow.ToString() 'Collect data for last skier in previous division
                                    ' Assemble the division row in the Full Page Table  
                                    sLine.Append("<tr><td><Table Class=""table  border-1 "">")  'Make row for Division and Start Division table
                                    'Division header
                                    '                                    sLine.Append("<tr><td  class=""table-success"" width=""13%"" ><b>Run Order</b></td><td colspan=""" & (sRndCols) & """ class=""text-bg-primary"" >" & sPREventCode & " " & sTmpEventGroup & ", " & sTmpDV & ", " & sEventClass & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td><td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                                    sLine.Append("<tr><td  class=""table-success"" width=""13%"" ><b>Run Order</b></td><td colspan=""" & (sRndCols) & """ class=""text-bg-primary"" >" & sPREventCode & " " & sTmpEventGroup & ", " & sTmpDV & ", " & sEventClass & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td></tr>")
                                    sLine.Append("<tr><td  class=""table-success""  width=""13%"">Name</td>" & sRoundsHTML)
                                    'Skier details
                                    sLine.Append(sMoreLines) 'Adds the skier name and rounds in that division
                                    'Close the division table the row in the Full Page table for the division
                                    sLine.Append("</table></td></tr>") '

                                    ' Reset variables for next division
                                    sTmpDV = sDv
                                    stmpMemberID = sMemberID
                                    sMoreLines = ""
                                    sTmpRow.Clear()
                                    'Start new row in Page level table
                                    sLine.Append("<tr><td>")
                                    'First round of first skier in new division
                                    sTmpRow.Append("<tr><td class = ""table-success"" width=""13%""><a runat=""server""  href=""Trecap?SID=" & sSanctionID & "&MID=" & stmpMemberID & "&DV=" & sTmpDV & "&EV=" & sPREventCode & "&TN=" & sTName & "&SN=" & sSkierName & """ >" & sSkierName & "</a></td>")
                                    If sRnd = 1 Then
                                        sTmpRow.Append("<td>" & sEventScoreDesc & "</td></tr>")
                                    Else
                                        sTmpRow.Append("<td> </td></tr>")
                                    End If
                                    i = 1 'Row 1 is in sTmpRow
                                End If

                        Loop
                        'END OF SINGLE DIVISION OR LAST OF MULTIPLE DIVISIONS 
                        'close out previous skier
                        If selRnd = 0 Then 'including all rounds - finish out row
                            If i < sRndsThisEvent Then
                                i += 1
                                For i = i To RndsSlalomOffered
                                    Select Case i
                                        Case 2
                                            sTmpRow.Append("<td></td>")
                                        Case 3
                                            sTmpRow.Append("<td></td>")
                                        Case 4
                                            sTmpRow.Append("<td></td>")
                                        Case 5
                                            sTmpRow.Append("<td></td>")
                                    End Select
                                Next
                                sTmpRow.Append("</tr>") 'close out the current row
                            End If
                        End If
                        sMoreLines += sTmpRow.ToString() 'Collect data for last skier in previous division
                        ' Assemble the division row in the Full Page Table  
                        sLine.Append("<tr><td><Table Class=""table  border-1 "">")  'Make row for Division and Start Division table
                        'Division header
                        '                                    sLine.Append("<tr><td  class=""table-success"" width=""13%"" ><b>Run Order</b></td><td colspan=""" & (sRndCols) & """ class=""text-bg-primary"" >" & sPREventCode & " " & sTmpEventGroup & ", " & sTmpDV & ", " & sEventClass & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td><td  class=""table-info"" colspan=""2""><b>Leader Board</b></td></tr>")
                        sLine.Append("<tr><td  class=""table-success"" width=""13%"" ><b>Run Order</b></td><td colspan=""" & (sRndCols) & """ class=""text-bg-primary"" >" & sPREventCode & " " & sTmpEventGroup & ", " & sTmpDV & ", " & sEventClass & " <span class=""bg-danger text-white"" > <b>! UNOFFICIAL !</b></span></td></tr>")
                        sLine.Append("<tr><td  class=""table-success""  width=""13%"">Name</td>" & sRoundsHTML)
                        'Skier details
                        sLine.Append(sMoreLines) 'Adds the skier name and rounds in that division
                        'Close the division table the row in the Full Page table for the division, and the full page table
                        sLine.Append("</table></td></tr></table>") '


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
        Return sLine.ToString()
    End Function

End Module
