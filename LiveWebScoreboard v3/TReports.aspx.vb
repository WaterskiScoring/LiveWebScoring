Public Class TReports
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sSanctionID As String = ""  'Trim(Request("SN"))
        Dim sMsg As String = ""
        Dim sErrDetails As String = ""
        Dim sEvent As String = ""
        If Not IsPostBack Then
            'Get the sanction number from the query string.

            If Request("SID") = Nothing Then
                lbl_Errors.Text = "Missing Parameter"
                Exit Sub
            End If

            Try
                sSanctionID = Trim(Request("SID"))  'SanctionID

            Catch ex As Exception
                lbl_Errors.Text = "Invalid Parameter"

                Exit Sub
            End Try
        End If
        sMsg = ModDataAccess3.GetReportList(sSanctionID)
        ReportList.InnerHtml = sMsg
    End Sub

    Private Sub Btn_Back_Click(sender As Object, e As EventArgs) Handles Btn_Back.Click
        'Nothing to do here. back works clientside with javascript
    End Sub
End Class