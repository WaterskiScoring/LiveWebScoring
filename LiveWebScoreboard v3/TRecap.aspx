<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TRecap.aspx.vb" Inherits="LiveWebScoreBoard.TRecap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Skier Recap</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <!--   <script src="js/bootstrap.bundle.min.js"></script>  -->
    <style>
        .box2{
       border: 1px solid blue;
             }
        .box{
            min-height: 50px;
            font-size: .6em;
        }
.legend .row:nth-of-type(odd) div {
background-color:antiquewhite;
}
.legend .row:nth-of-type(even) div {
background: #FFFFFF;
}
.ms-0{
	margin-left: 0;
	margin-right: 0;
    padding-left: 0;
    padding-right: 0;
}

    </style>
</head>
<body>
    <form id="form1" runat="server">
       
         <!-- Store user selections -->
        <asp:Panel ID="Panel_HF" runat="server" Visible="false">
            <asp:HiddenField ID="HF_SanctionID" runat="server" />
            <asp:HiddenField ID="HF_SkierID" runat="server" />
            <asp:HiddenField ID="HF_Event" runat="server" />
            <asp:HiddenField ID="HF_AgeGroup" runat="server" />
            <asp:HiddenField ID="HF_RndPkd" runat="server" /> <!-- selectedvalue of ddl_PkRnd -->
            <asp:HiddenField ID="HF_YearPkd" runat="server" /> <!-- Stores ddl_PkYear.selected value on home page.  Recent = 0 -->
            <asp:HiddenField ID="HF_TournName" runat="server" />
            <asp:HiddenField ID="HF_FormatCode" runat="server" />
            <asp:HiddenField ID="HF_SlalomRnds" runat="server" />
            <asp:HiddenField ID="HF_TrickRnds" runat="server" />
            <asp:HiddenField ID="HF_JumpRnds" runat="server" />
            <asp:HiddenField ID="HF_UseNOPS" runat="server" />
         <asp:HiddenField ID="HF_UseTeams" runat="server" />
         <asp:HiddenField ID="HF_DisplayMetric" runat="server" />
         </asp:Panel>
        <!-- Display title bar and error label -->
        <!-- Display title bar and error label -->
        <div class="fixed-top">
            <div id="TName" class="text-bg-dark text-center " runat="Server">
                <br /><asp:Label ID="lbl_Errors" runat="server" ForeColor="White"/>
            </div>
            <div class="bg-aqua"><center><asp:Button ID="Btn_Back" runat="server" Text="Back To Scores" /></center>
            </div>

        </div>
        <!-- Display 4 boxes - one for each event + Overall.  
            Put in skier's performances by round and pass as they become available -->
        
      <div class="container-fluid ms-0">
        <div class="text-center" >
            <div class="row">
                <div class="col-xxl">
                    <h4>&nbsp;</h4>
                   <div id="SlalomRecap" runat="server" class="box"></div> 
                    
                </div>
        
                <div class="col-xxl">
                    <h4>&nbsp;</h4>
                    <div id="TrickRecap" runat="server" class="box"></div>
                    
                </div>
        
                <div class="col-xxl">
                    <h4>&nbsp;</h4>
                    <div id="JumpRecap" runat="server" class="box"></div>
                    
                </div>

                <div class="col-xxl">
                 <h4>&nbsp;</h4>
                 <div id="OverallRecap" runat="server" class="box"></div>
     
                </div>
           </div>
       </div>
          
   </div>
    </form>
</body>
</html>
