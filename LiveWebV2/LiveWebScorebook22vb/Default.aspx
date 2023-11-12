<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="LiveWebScorebook22vb._Default1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Live Web Scorebook Home Page</title>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <script src="js/bootstrap.bundle.min.js"></script>
</head>
<body>
   <form id="form1" runat="server">
       <div class="text-bg-dark text-center">
<h3>Live Web Scorebook</h3>
           See scores as soon as they happen
           <br />powered by WSTIMS
</div>
       
    <!--   <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
    <div class="container-fluid" >
        <button type="button" class="btn">Basic</button>
            <button type="button" class="btn btn-default">Default</button>
  <button type="button" class="btn btn-primary">Primary</button>
  <button type="button" class="btn btn-success">Success</button>
  <button type="button" class="btn btn-info">Info</button>
  <button type="button" class="btn btn-warning">Warning</button>
  <button type="button" class="btn btn-danger">Danger</button> -->
  <!-- <button type="button" class="btn btn-link">Link</button>
    </div>
</nav> -->
 <div class="container-fluid">
    <div class="row">
        <div class="col-">
            <div class="text-center">
            <asp:DropDownList ID="ddl_PkYear" runat="server"></asp:DropDownList> &nbsp; &nbsp;
            <asp:Button ID="Btn_LoadTList" runat="server" Text="Load Tournaments"  />
      <!--      <button type="button" class="btn btn-primary">Primary</button>
            <button type="button" class="btn btn-success">Success</button>  -->
            </div>
        </div>
        <div class="row">
            <div class =" col-xs-1"> </div>
            <div class =" col-xs-10">
                <div class="p-5 my-4 bg-light rounded-3">
                    <h4>Welcome,</h4>
                    This portal provides access to Waterski Scores from in progress and archived competitions.
                    The WSTIMS scoring program allows scorers to publish results to be displayed here, virtually in real time.
                    Trick video can also be uploaded and viewed here, typically for archived tournaments.  
                    If trick video is available, the ICON icon will be displayed in the tournament list.
                    Click on the icon to display trick score sheets (pink slips) and streaming video.
                    <h4>Directions</h4>
                    <ol><li>Start by selecting a range of tournament dates from the droplist<br />
                    Recent provides a list of competitons held in the last 20 days, including competitions currently in progress.
                    <br /><b>IMPORTANT:</b> Results from competitions in progress are <b>UNOFFICIAL</b>.</li>
                    <li>Click the Tournament List button to display a list of tournaments in the range you have selected.</li>
                    </ol>                                                                                   
                    
                </div>
            </div>

            <div class =" col-xs-1"></div>
        </div> <!-- End Div class row -->
        
    
    </div> <!-- end of row -->
            
       <!--  Leave this in case it becomes helpful     
      <div class="row">
        <div class="col-md-4">
            <h2>Box 1</h2>
            <p>This is the first box in row of 3</p>
        </div>
        <div class="col-md-4">
            <h2>Box 2</h2>
            <p>this is the middle row</p>
        </div>
        <div class="col-md-4">
            <h2>Box 3</h2>
            <p>This is the box on the right</p>
        </div>
    </div>  -->
    <hr />
    <footer>
        <div class="row">
            <div class="col-md-6">
                This is the left half of the footer<br />
                <p>Copyright &copy; 2023 Just Us Volunteers</p>
            </div>
            <div class="col-md-6 text-md-end">
                This is the right half of the footer<br />
                <a href="#" class="text-dark">Terms of Use</a> 
                <span class="text-muted mx-2">|</span> 
                <a href="#" class="text-dark">Privacy Policy</a>
            </div>
        </div> <!-- end of row -->
    </footer>
</div> <!-- End of container -->
    </form> 
</body>
</html>
