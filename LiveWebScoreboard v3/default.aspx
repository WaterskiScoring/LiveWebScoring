<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="default.aspx.vb" Inherits="LiveWebScoreBoard._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Live Web Scorebook Home Page</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />

</head>
<body>
   <form id="form1" runat="server">
       <div class="text-bg-dark text-center">
<h3>Live Web Scoreboard v3.2.4</h3>
           See scores as soon as they happen.
           <br />powered by WSTIMS
</div>
       
 <div class="container-fluid">
    <div class="row">
        <div class="col-">
            <div class="text-center">
            <asp:DropDownList ID="ddl_PkYear" runat="server" AutoPostBack="true"></asp:DropDownList> &nbsp; &nbsp;
           
            <br />    <asp:Label ID="lbl_Errors" runat="server" forecolor="Red" Font-Bold="true"></asp:Label>
     <br />OR <asp:TextBox ID="TB_SanctionID" runat="server" /> <asp:Button ID="Btn_SanctionID" runat="server" Text="Submit Sanction Number" />
            </div>
        </div>
        <div class="row">
            <div class =" col-xs-1"> </div>
            <div class =" col-xs-10">
                <div class="p-1 mr-4 bg-light rounded-3">
                    <h4>Welcome,</h4>
                    The WSTIMS scoring program enables scorers to post <span class="bg-danger text-white" >  UNOFFICIAL </span> results, as they happen.
                    Trick video is available if the <img src="images/Flag-green16.png" /> icon is displayed in the tournament list.
                    For Official results click the Reports button on the Tournament Home Page.
                    
                    <h4>Directions</h4>
                    <ol><li>Select a range of tournaments and pick from the list<br />or enter a sanction number</li>
                    <li>On the Tournament Home Page select a Display Style<br />
                    If the tournament is in progress the event(s) currently being scored are listed<br />
                    <b>WARNING TO SKIERS:</b> DO NOT rely on the running orders displayed for your start times, especially if multiple lakes are in use.
                    </li>
                    <li>On the selected page, Select an Event<br />optional - select Division, Group or Round</li>
                    <li>Click on a skier's name to display recap results for all events entered.</li>    
                    <li>Click the Update button to refresh the data</li>
                    </ol>                                                                                   
                </div>
            </div>
        </div> <!-- End Div class row -->
    </div> <!-- end of row -->


        <div class="row">
            <div class="col-md-6">
                Live Web Scoreboard is a free volunteer created resource intended to advance the sport of waterskiing.
                <br />&copy; 2024 All rights reserved

            </div>
            <div class="col-md-6 text-md-end">
                <br />
                <asp:Button ID="Btn_Privacy_TermsOfUse" runat="server" Text="Privacy / Terms of Use" />
                <asp:Panel ID="Panel_Priv_TofUse" runat="server" visible="false">
             No Yada Yada<br />
            <b>Privacy Policy</b><br />
            This site does not collect or store any information from site users.
            Information displayed is in the public domain and freely available elsewhere, albeit not quite as conveniently.<br />
            
            <b>Terms of Use</b><br />
            Enjoy the content. Do not make any modifications. You may place a link to scores.waterskiresults.com on your website.
                    Republication by any other means is prohibited without written consent.
                </asp:Panel>
            </div>
        </div> <!-- end of row -->

</div> <!-- End of container -->
    </form> 
</body>
</html>
