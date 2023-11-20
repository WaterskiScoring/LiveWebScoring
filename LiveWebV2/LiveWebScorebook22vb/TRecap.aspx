<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TRecap.aspx.vb" Inherits="LiveWebScorebook22vb.TRecap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Individual Scores</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <script src="js/bootstrap.bundle.min.js"></script>
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
       

        <!-- Display title bar and error label -->
        <div id="TName" class="text-bg-dark text-center" runat="Server">
           <asp:Label ID="lbl_Errors" runat="server" ForeColor="Red"/>
        
       </div>
        
        
        <!-- Display 4 boxes - one for each event + Overall.  
            Put in skier's performances by round and pass as they become available -->
        
      <div class="container-fluid">
        <div class="text-center">
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
