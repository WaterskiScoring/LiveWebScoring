<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TSkierListPro.aspx.vb" Inherits="LiveWebScoreBoard.TSkierListPro" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Entry List</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
   <!--   <script src="js/bootstrap.bundle.min.js"></script>  -->
    <style>
      .down75{
          margin-left: 0;
          margin-right: 0;
          min-height: 75px;
      }
      .down125{
    margin-left: 0;
    margin-right: 0;
    min-height: 125px;
}
            .down100{
    margin-left: 0;
    margin-right: 0;
    min-height: 100px;
}
    </style>
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
       <div class="fixed-top">
        <div id="TName" class="text-bg-dark text-center" runat="Server">
           <asp:Label ID="lbl_Errors" runat="server"  ForeColor="White" Font-Bold="true"/>
        
       </div>
        <!-- Display droplist and button controls -->
        <div class="text-center">
            <asp:Button ID="Btn_2Tournament" runat="server" Text="Tourn Home" />
                
            <asp:Button ID="Btn_2TList" runat="server" Text="Tournament List" />
                <asp:Button ID="Btn_Home" runat="server" Text="Home Page" />
                
        </div>
</div>
        <div class="down125"></div>
        <!-- Display One column.   -->
         <div id="InsertHere" runat="server">

         </div>
               
    </form>
</body>
</html>
