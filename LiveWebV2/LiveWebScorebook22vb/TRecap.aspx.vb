Imports System.Security.Cryptography.X509Certificates
Imports System.Text.RegularExpressions

Public Class TRecap
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sSanctionID As String = ""  'Trim(Request("SN"))
        Dim sMemberID As String = ""
        Dim sAgeGroup As String = ""
        Dim sSkierName As String = ""
        Dim sErrDetails As String = ""
        If Not IsPostBack Then
            'Get the sanction number from the query string.
            'SN and FM and SY must always be present.  Check for other variables based on FM
            If Request("SID") Is Nothing Or Request("MID") = Nothing Or Request("DV") = Nothing Or Request("SN") = Nothing Then
                lbl_Errors.Text = "Missing Parameter"
                Exit Sub
            End If

            Try
                sSanctionID = Trim(Request("SID"))  'SanctionID
                sAgeGroup = Request("DV")
                sMemberID = CStr(Request("MID"))  'MemberID   specify string or lose leading 000
                sSkierName = CStr(Request("SN"))    'SkierName
                sSkierName = Replace(sSkierName, "'", "")
                sSkierName = Replace(sSkierName, ",", "")
            Catch ex As Exception
                lbl_Errors.Text = "Invalid Parameter"
                '                Session.Abandon()
                '               Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
                Exit Sub
            End Try

            Try
                'Validate query string values
                If Not Regex.IsMatch(sSanctionID, "^[0-9][0-9][CEMSWU][0-9][0-9][0-9]$") Then
                    lbl_Errors.Text = "Invalid Parameter"
                    '                   Session.Abandon()
                    '                    Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
                    Exit Sub
                End If

                If Not Regex.IsMatch(sAgeGroup, "^[A-Za-z1-9]*$") Then
                    lbl_Errors.Text = "Invalid Parameter"
                    '                   Session.Abandon()
                    '                   Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
                    Exit Sub
                End If

                If Not Regex.IsMatch(sSkierName, "^[A-Za-z /-]*$") Then
                    lbl_Errors.Text = "Invalid Parameter"
                    '                   Session.Abandon()
                    '                    Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
                    '   Exit Sub
                End If
            Catch ex As Exception
                sErrDetails = ex.Message & "<br>" & ex.StackTrace
                '               Session.Abandon()
                '                Response.Redirect("http://www.WaterskiResults.com/default.aspx?EM=sEM")
            End Try

            '            Dim sSanctionID As String = "23S108"  'Trim(Request("SN"))
            '           Dim sFormMode As Int16 = Request("FM")
            '            Dim sMemberID As String = "000107150"   'Trim(Request("SID"))
            TName.InnerHtml = "<h3>" & sSanctionID & " &nbsp;-&nbsp; Recap For " & sSkierName & " </h3>"
            SlalomRecap.InnerHtml = ModDataAccess.RecapSlalom(sSanctionID, sMemberID, sAgeGroup, sSkierName)
            '           TrickRecap.InnerHtml = ModDataAccess.RecapTrick(sSanctionID, sMemberID, sAgeGroup, sSkierName)
            JumpRecap.InnerHtml = ModDataAccess.RecapJump(sSanctionID, sMemberID, sAgeGroup, sSkierName)
            OverallRecap.InnerHtml = ModDataAccess.RecapOverall(sSanctionID, sMemberID, sAgeGroup, sSkierName)
        End If

    End Sub


End Class