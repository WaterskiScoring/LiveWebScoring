Public Class _Default1

    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            With ddl_PkYear
                .Items.Add(New ListItem("Recent", "0"))
                .Items.Add(New ListItem("2023", "23"))
                .Items.Add(New ListItem("2022", "22"))
                .Items.Add(New ListItem("2021", "21"))
                .SelectedValue = "0"
            End With
        End If
    End Sub

    Private Sub Btn_LoadTList_Click(sender As Object, e As EventArgs) Handles Btn_LoadTList.Click
        Dim sYrPkd As String = ddl_PkYear.SelectedValue
        Response.Redirect("TList.aspx?YR=" & sYrPkd)
    End Sub
End Class