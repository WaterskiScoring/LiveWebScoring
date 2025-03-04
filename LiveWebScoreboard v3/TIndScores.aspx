<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TIndScores.aspx.vb" Inherits="LiveWebScoreBoard.TIndScores" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Individual Scores</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <!--   <script src="js/bootstrap.bundle.min.js"></script>  -->
    <style>
        .box{
           
            border: 1px solid blue;
            min-height: 50px;
            font-size: .6em;
            
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Store user selections -->
        <asp:Panel ID="Panel1" runat="server" visible="false">
        <asp:HiddenField ID="HF_SanctionID" runat="server" />
        <asp:HiddenField ID="HF_SkierID" runat="server" />
        <asp:HiddenField ID="HF_Event" runat="server" />
        <asp:HiddenField ID="HF_AgeGroup" runat="server" />
        <asp:HiddenField ID="HF_YearPkd" runat="server" /> <!-- Stores ddl_PkYear.selected value on home page.  Recent = 0 -->
        <asp:HiddenField ID="HF_TournName" runat="server" />
        </asp:Panel>
        <div class="top-fixed">
        <!-- Display title bar and error label -->
        <div id="TName" class="text-bg-dark text-center" runat="Server">
           <asp:Label ID="lbl_Errors" runat="server" ForeColor="white" Font-Bold="true"/>
        
       </div>
        <!-- Display droplist and button controls -->
        <div class="text-center">
           <asp:Button ID="Btn_ToSkierList" runat="server" Text="New Skier" />
           <asp:Button ID="Btn_2TournHome" runat="server" Text="Tourn Home" />
            <asp:Button ID="Btn_2TList" runat="server" Text="Tournament List" />
            <asp:Button ID="Btn_Home" runat="server" Text="Home Page" />
            
        </div>
     </div>   
        <!-- Display 3 boxes - one for each event.  Put in skier's performances by round as they become available -->
        
      <div class="container-fluid">
        <div class="text-center">
            <div class="row">
                <div class="col-xxl">

                   <div id="SlalomScore" runat="server" class="box"></div> 
                    
                </div>
        
                <div class="col-xxl">

                    <div id="TrickScore" runat="server" class="box"></div>
                    
                </div>
        
                <div class="col-xxl">

                    <div id="JumpScore" runat="server" class="box"></div>
                    
                </div>

               <div class="col-xxl">

                    <div id="OverallScore" runat="server" class="box"></div>
                    
                </div>

           </div>
       </div>
          
   </div>
    </form>
</body>
</html>