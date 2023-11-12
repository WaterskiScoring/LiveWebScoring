<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TSkierListPro.aspx.vb" Inherits="LiveWebScorebook22vb.TSkierListPro" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Entry List</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <script src="js/bootstrap.bundle.min.js"></script>
    
</head>
<body>
    <form id="form1" runat="server">
   <!-- Store user selections -->
        <asp:HiddenField ID="HF_SanctionID" runat="server" />
        <asp:HiddenField ID="HF_SkierID" runat="server" />
        <asp:HiddenField ID="HF_Event" runat="server" />
        <asp:HiddenField ID="HF_AgeGroup" runat="server" />
        <asp:HiddenField ID="HF_YearPkd" runat="server" /> <!-- Stores ddl_PkYear.selected value on home page.  Recent = 0 -->
        <asp:HiddenField ID="HF_TournName" runat="server" />
        <!-- Display title bar and error label -->
        <div id="TName" class="text-bg-dark text-center" runat="Server">
           <asp:Label ID="lbl_Errors" runat="server" Text="Here I am" ForeColor="Red"/>
        
       </div>
        <!-- Display droplist and button controls -->
        <div class="text-center">
            <asp:Button ID="Btn_2Tournament" runat="server" Text="This Tournament" />
           <asp:Button ID="Btn_2TList" runat="server" Text="New Tournament" />
            <asp:Button ID="Btn_Home" runat="server" Text="Home Page" />
            <asp:DropDownList ID="ddl_ColumnSelector" runat="server"></asp:DropDownList>
        </div>

        <!-- Display up to 3 columns - One for each Event.   -->
        <div class="container-fluid">
            <div class="row">
                <div class="col-sm-4">
                    <div id="InsertHere" runat="server"></div>
                </div>
                <div class="col-sm-4">
                    <div id="InsertHere2" runat="server">Column2</div>
                </div>
                <div class="col-sm-4">
                    <div id="InsertHere3" runat="server">Column 3</div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>