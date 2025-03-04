<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TList.aspx.vb" Inherits="LiveWebScoreBoard.TList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Tournament List</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
   <!--   <script src="js/bootstrap.bundle.min.js"></script>  -->

</head>
<body>
    <form id="form1" runat="server">
        <div class="text-bg-dark text-center">
                <h3>Select a Tournament</h3>
                <asp:Label ID="Lbl_Errors" runat="server" ForeColor="Red" Font-Bold="true" ></asp:Label>
</div>
        <div class="text-center">
            <asp:Button ID="Btn_Home" runat="server" Text="Home Page" />
        </div>
        <div class="container-fluid">
            <div class="row">
                <div class =" col-xs-1"> </div>
                <div class =" col-xs-10">
                    <div id="TList" runat="server" >

                    </div>
                </div>
                <div class =" col-xs-1"></div>
            </div> <!-- End Div class row -->
        </div><!-- End div Container-->
    </form>
</body>
</html>
