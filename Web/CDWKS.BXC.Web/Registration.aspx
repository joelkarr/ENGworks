<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="CDWKS.BXC.Web.Registration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title></title>
        <link rel = "Stylesheet" type = "text/css" href="styles/contentstyle.css" />
        <link rel = "Stylesheet" type = "text/css" href="styles/jQuery.css" />
    </head>
    <body style="background: white; height: 300px; width: 400px;">
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="true" runat="server">
            </asp:ScriptManager>
            <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function returnToParent() {

            //create the argument that will be returned to the parent page
            var oArg = new Object();

            //get text values
            oArg.userName = document.getElementById("txtEmailAddress").value;
            oArg.password = document.getElementById("txtPassword").value;
            oArg.first = document.getElementById("txtFirstName").value;
            oArg.last = document.getElementById("txtLastName").value;
            oArg.promo = document.getElementById("txtPromo").value;
            oArg.company = document.getElementById("txtCompany").value;
            oArg.phone = document.getElementById("txtPhoneNumber").value;
            if (PageMethods.isPromoCodeValid(oArg.promo)) {
                //Close the RadWindow and send the argument to the parent page
                PageMethods.RegisterLogin(oArg.userName, oArg.password, oArg.first, oArg.last, oArg.promo, oArg.company, oArg.phone, onSucceed, onError);
            } else {
                document.getElementById("divPromoNotRecognized").style.visibility = 'visible';
            }

        }

        function onSucceed(results) {
            if (results) {
                //create the argument that will be returned to the parent page
                var oArg = new Object();
                //get the city's name
                oArg.userName = document.getElementById("txtEmailAddress").text;
                oArg.password = document.getElementById("txtPassword").text;
                //get a reference to the current RadWindow
                var oWnd = GetRadWindow();
                oWnd.close(oArg);
            } else {
                document.getElementById("divAlreadyUsed").style.visibility = 'visible';
            }

        }

        function onError(results) {
            alert("Error:" + results);
        }

//get a reference to the current RadWindow
        var oWnd = GetRadWindow();
    </script>
            <div id="register-fields" >
                <asp:HyperLink ID="hlBack" CssClass="right hyperlink" NavigateUrl="~/Login.aspx" Text="Back to Login" runat="server" />     
                <h2 id="create-title"><asp:Literal ID="litCreateUserTitle" Text="Registration" runat="server" /></h2>
                <div id="divAlreadyUsed" style="visibility: hidden; color: Red;"> 
                    <asp:Literal ID="litAlreadyUsed" Text="Email is already Registered." Visible="true" runat="server" /> 
                </div>
                <div id="divPromoNotRecognized" style="visibility: hidden; color: Red;"> 
                    <asp:Literal ID="Literal1" Text="Promo Code Not Recognized.  Please Try again." Visible="true" runat="server" /> 
                </div>
                <div id="account-pwords"> 
                    <div class="half">
                        <h3><span class="title"><asp:Literal ID="litAccountInformationTitle" Text="Account Info" runat="server" /></span></h3>
                        <div class="acc-info-field-div"><label><asp:Literal ID="litEmailAddress" Text="Email:" runat="server" />*</label><br />
                            <asp:TextBox ID="txtEmailAddress" CssClass="text" runat="server"/>
                            <p id="availability-checker-email" class="off" ></p>
                        </div>
                              
                        <br style="clear: left" />
                        <div class="acc-info-field-div">
                            <label><asp:Literal ID="litPassword" Text="Password" runat="server" />*</label><br />
                            <asp:TextBox autocomplete="off" ID="txtPassword" CssClass="text" runat="server" TextMode="Password"  MaxLength="16" />
                        </div>
                               
                        <br style="clear: left" />
                        <div class="acc-info-field-div">
                            <label><asp:Literal ID="litPasswordConfirm" Text="Confirm Password" runat="server" />*</label><br />
                            <asp:TextBox ID="txtPasswordConfirm" CssClass="text" runat="server" TextMode="Password"  MaxLength="16" />
                        </div>
                         
                        <br style="clear: left" />
                        <div class="acc-info-field-div">
                            <label><asp:Literal ID="litPromo" Text="Promo Code:" runat="server" /></label><br />
                            <asp:TextBox ID="txtPromo" CssClass="text" runat="server" />
                        </div>
                    </div><!--halfwithtip-->
                </div><!--account-pwords-->
                 
                <div id="personal-info-div" class="half"> 
                    <h3><span class="title"><asp:Literal ID="litPersonalInformationTitle" Text="Personal Info" runat="server" /></span></h3>
                    <div class="acc-info-field-div">
                        <label><asp:Literal ID="litFirstName" Text="First Name:" runat="server" />*</label><br />
                        <asp:TextBox ID="txtFirstName" CssClass="text" runat="server" />
                    </div>
                    <br style="clear: left" />
                    <div class="acc-info-field-div">
                        <label><asp:Literal ID="litLastName" Text="Last Name:" runat="server" />*</label><br />
                        <asp:TextBox ID="txtLastName" CssClass="text" runat="server" />
                    </div>
                    <br style="clear: left" />
                    <div class="acc-info-field-div">
                        <label><asp:Literal ID="litPhoneNumber" Text="Phone Number:" runat="server" />*</label><br />
                        <asp:TextBox ID="txtPhoneNumber" CssClass="text" runat="server" />
                    </div> 
                    <br style="clear: left" />
                    <div class="acc-info-field-div">
                        <label><asp:Literal ID="litCompany" Text="Company:" runat="server" />*</label><br />
                        <asp:TextBox ID="txtCompany" CssClass="text" runat="server" />
                    </div> 
                    <br style="clear: left" />
    
                </div><!--personal-info-div-->
                <center><asp:ImageButton ID="CompleteRegBttn" 
                        ImageUrl="~/images/RegisterButton.png" Height="40px" CssClass="button" 
                                         Text="Complete Registration"  runat="server" 
                                         OnClientClick="returnToParent(); return false;" /></center>
            </div><!--account-info-div-parent-->

            <div>
                <asp:RequiredFieldValidator ID="rfEmailAddress" Display="None" ControlToValidate="txtEmailAddress" ErrorMessage="Please enter email." runat="server"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="rfPassword" Display="None" ControlToValidate="txtPassword" ErrorMessage="Please enter password." runat="server"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revPassword" Display="None" Text="*" runat="server" ValidationExpression="^([a-z,A-z,0-9,\.,!]+){8}$" ErrorMessage="Password must be at least 8 characters." ControlToValidate="txtPassword" />
                <asp:RequiredFieldValidator ID="rfPasswordConfirm" Display="None" ControlToValidate="txtPasswordConfirm" ErrorMessage="Please confirm password." runat="server"></asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvPassword" Display="None" ErrorMessage="Confirm password must match." runat="server" ControlToValidate="txtPasswordConfirm" ControlToCompare="txtPassword" Text="*" />
                <asp:RequiredFieldValidator ID="rfFirstName" Display="None" ControlToValidate="txtFirstName" ErrorMessage="Please enter first name." runat="server"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="rfLastName" Display="None" ControlToValidate="txtLastName" ErrorMessage="Please enter last name." runat="server"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="revPhone" Display="None" ErrorMessage="Please enter phone number in format (555)555-5555" ControlToValidate="txtPhoneNumber" Text="*" ValidationExpression="^((\(?\+?[0-9]*\)?)?[0-9_\- \(\)]+){10}$" runat="server" />
                <asp:RequiredFieldValidator ID="rfCompany" Display="None" ControlToValidate="txtCompany" ErrorMessage="Please enter company." runat="server"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="rfPhoneNumber" Display="None" ControlToValidate="txtPhoneNumber" ErrorMessage="Please enter phone number." runat="server"></asp:RequiredFieldValidator>
            </div>
            <asp:ValidationSummary Id="ValidSum" DisplayMode="BulletList" ShowMessageBox="true" ShowSummary="false" runat="server" />
        </form>
    </body>
</html>