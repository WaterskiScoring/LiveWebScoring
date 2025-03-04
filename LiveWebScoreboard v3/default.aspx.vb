Public Class _default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim sCurYear As Int16 = CInt(Year(Now))
            Dim sCurYY As Int16 = CInt(Right(sCurYear, 2))
            Dim sCurMM As Int16 = CInt(Month(Now))
            If sCurMM > 7 Then
                sCurYear += 1 'After July display sanctions from following ski year
                sCurYY += 1
            End If
            Dim sDescription As String = ""
            Dim sIndex As String = "999"
            Dim i As Int16 = 0
            With ddl_PkYear
                .Items.Add(New ListItem("Please Select a Range of Tournaments To Display", "999"))
                .Items.Add(New ListItem("Most Recent 20 Competitions", "0"))
                Do Until sCurYY = 20 Or i = 10
                    sDescription = "Ski Year " & CStr(sCurYear)
                    sIndex = CStr(sCurYY)
                    .Items.Add(New ListItem(sDescription, sIndex))
                    sCurYear -= 1
                    sCurYY -= 1
                    i += 1
                Loop
            End With


            If Not Request("EM") = Nothing Then
                lbl_Errors.Text = Request("EM")
            End If
        End If
    End Sub



    Private Sub Btn_Privacy_TermsOfUse_Click(sender As Object, e As EventArgs) Handles Btn_Privacy_TermsOfUse.Click
        If Panel_Priv_TofUse.Visible = True Then
            Panel_Priv_TofUse.Visible = False
        Else
            Panel_Priv_TofUse.Visible = True
        End If
    End Sub

    Private Sub ddl_PkYear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddl_PkYear.SelectedIndexChanged
        Dim sYrPkd As String = ddl_PkYear.SelectedValue
        If sYrPkd = "999" Then
            lbl_Errors.Text = "You must select a range of tournaments to display"
            Exit Sub
        End If
        Response.Redirect("TList.aspx?YR=" & sYrPkd)

    End Sub

    Private Sub Btn_SanctionID_Click(sender As Object, e As EventArgs) Handles Btn_SanctionID.Click
        'Validate the sanction number entered
        Dim sSanctionID As String = TB_SanctionID.Text
        Dim sEM As String = ""
        Dim sSkiYr As String = 0
        If Not Regex.IsMatch(sSanctionID, "^[0-9][0-9][CEMSWUX][0-9][0-9][0-9]$") Then  'Include X to include Dave's test tournament 24X017
            sEM = "Invalid Request.  You may try again."
            Session.Abandon()
            Response.Redirect("~/default.aspx?EM=" & sEM)
            Exit Sub
        End If
        Response.Redirect("Tournament.aspx?SN=" & sSanctionID & "&FM=1&SY=0")

    End Sub
End Class