<%@ Page Title="" Language="C#" MasterPageFile="~/Xchange.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CDWKS.BXC.Web.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="Main" runat="server">
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function ShowTypeCatalog(id) {
            var owin = radopen("TypeCatalog.aspx?id=" + id, "windowType");
        }

        function ShowHelp() {
            var owin = radopen("http://www.ENGworks.net/content", "windowHelp");
        }

        function ShowEula() {
            var owin = radopen("Eula.aspx", "windowEula");
        }

        function ShowChangeLibrary() {
            var owin = radopen("Library.aspx", "windowLibrary");
        }

        function LoadFile(Id) {
            PageMethods.LoadFile(Id, onSucceeded, onFailed);
        }

        function onSucceeded(result, userContext, methodName) {
            document.title = "dl=" + result;
        }

        function onFailed(error, userContext, methodName) {
            alert("An error occurred");
        }

        function OnClientClose(oWnd, args) {
            //get the transferred arguments
            var arg = args.get_argument();
            if (arg) {
                var package = arg.Package;
                document.title = "dl=" + package;
            }
        }

        function LoginClose() {
            window.location.reload();
        }

        function LibraryClose() {
            window.location.reload();
        }

        function HideFilterMessage() {
            document.getElementById("divMaxReached").className = 'ErrorOff';
        }

        function isTypeCatalog(value) {
            if (value == string.empty) {
                return false;
            } else {
                return true;
            }
        }

        function onSucceed(results) {
        }

        function onError(results) {
            alert("Could not load user library");
        }

        var loadingPanel = "";
        var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
        var postBackElement = "";
        pageRequestManager.add_initializeRequest(initializeRequest);
        pageRequestManager.add_endRequest(endRequest);

        function initializeRequest(sender, eventArgs) {
            loadingPanel = $find('<% =RadAjaxLoadingPanel1.ClientID%>');
            postBackElement = "<%=grdItems.ClientID%>";
            loadingPanel.show(postBackElement);
        }

        function endRequest(sender, eventArgs) {
            loadingPanel = $find('<% =RadAjaxLoadingPanel1.ClientID%>');
            loadingPanel.hide(postBackElement);
        }

      </script>
    <div id = "leftcolumn">
        <div id="LibraryImage">
            <center>
                <asp:Image CssClass="image" Height="75" Width="225" ID="imgLibrary" ImageUrl="~/Images/Libraries/Default.png"  runat="server" />
            </center>
        </div>
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <div id="Tabs" style="height: 29px;">
                    <asp:ImageButton ID="btntabTree" Height="30px" ImageUrl="~/Images/btnTreePage.png" OnClick="BtnTreeTabClick" runat="server" />
                    <asp:ImageButton ID="btntabFilter" Height="30px" CssClass="marginminus" ImageUrl="~/Images/btnFilterPage.png" OnClick="BtnFilterTabClick" runat="server" />
                    <asp:ImageButton ID="btnChangeLibrary" Height="30px"  CssClass="marginminus" 
                                     ImageUrl="~/Images/btnChangeLibrary.png" 
                                     OnClientClick="ShowChangeLibrary(); return false;"  runat="server"/>
                </div>
                <telerik:RadMultiPage ID="rmpNavigation" SelectedIndex="0" runat="server" 
                                      style="margin-top: 1px">
                    <telerik:RadPageView ID="pgTree" CssClass="tree-page tab-page" runat="server">
                        <asp:Panel ID="pnlTree" Height="500px" Width="235px" runat="server" ScrollBars="Auto">
                            <telerik:RadTreeView ID="tvDirectory" OnNodeExpand="TvDirectoryNodeExpand" OnNodeClick="TvDirectoryNodeClick" 
                                                 CausesValidation="false" runat="server" Skin="Default">
                            </telerik:RadTreeView>
                        </asp:Panel>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="pgFilter" CssClass="filter-page tab-page" runat="server">
                        <div id="divFilter" style="overflow: hidden; width: 235px;">
                        <asp:Panel ID="pnlSelect" runat="server">
                            <asp:RadioButtonList ID="rdbtnFilter" RepeatDirection="Horizontal" BorderWidth="0px" OnSelectedIndexChanged="RdFilterChanged" AutoPostBack="true" runat="server">
                                <asp:ListItem Selected="True" Value="Simple" Text="Simple"></asp:ListItem><asp:ListItem Text="Advanced" Value="Advanced"></asp:ListItem></asp:RadioButtonList>
                        </asp:Panel>
                        <asp:Panel ID="pnlAdvancedFilter" Visible="false" runat="server">
                            <telerik:RadComboBox ID="cbFields" Width="200" Height="150px"
                                                 EmptyMessage="Select a Field" EnableLoadOnDemand="true" ShowMoreResultsBox="true"
                                                 EnableVirtualScrolling="true" CssClass="TextInput" runat="server">
                                <WebServiceSettings Method="GetFieldNames" Path="default.aspx" />
                            </telerik:RadComboBox>
                            <asp:DropDownList ID="ddOperators" CssClass="TextInput" runat="server"><asp:ListItem>Contains</asp:ListItem><asp:ListItem>Equal To</asp:ListItem></asp:DropDownList>
                        </asp:Panel>
                        <asp:TextBox ID="txtValue" CssClass="TextInput" Width="160px" runat="server"></asp:TextBox>
                        <asp:ImageButton ID="btnAddFilter" CssClass="TextInput" ImageUrl="~/images/btnAddFilter.png" Height="30px" OnClick="BtnAddFilterClick" runat="server" />
                        <asp:Panel ID="pnlMaxReahed" visible="false" runat="server">
                            <div id="divMaxReached" runat="server" style="visibility: hidden; color: Red;"> 
                                <asp:Literal ID="litFilterMax" Text="Maximum of 5 filters allowed" Visible="true" runat="server"></asp:Literal>
                            </div>
                        </asp:Panel>
                        <telerik:RadListBox ID="lbFilters" BorderWidth="0px" Width="225px" runat="server">
                            <ItemTemplate>
                                <div class="FilterBox">
                                    <div id="removeBox" class="FilterBoxleft">
                                        <asp:ImageButton Id="btnRemove" CssClass="RemoveImage" CommandArgument='<%#Eval("Id")%>' OnClick="BtnRemoveClick" Height="25px" Width="25px" ImageUrl="~/images/remove.png" runat="server" />
                                    </div>
                                    <div id="box2" class="FilterBoxright">
                                        <%#Eval("Field")%><br /> <%#Eval("Op")%> <%#Eval("Value")%>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </telerik:RadListBox>
                        <asp:Panel Id="pnlRemoveFilters" CssClass="padleft6" runat="server" Visible="false">
                            <div class="FilterBox">
                                <div id="removeBox2" class="FilterBoxleft">
                                    <asp:ImageButton Id="btnRemove" CommandArgument="All" CssClass="RemoveImage" OnClick="BtnRemoveClick" Height="25px" Width="25px" ImageUrl="~/images/remove.png" runat="server" />
                                </div>
                                <div id="box22" class="FilterBoxright">
                                    Remove All Filters
                                </div>
                            </div>
                        </asp:Panel>
                    </telerik:RadPageView>
                </telerik:RadMultiPage>
            </ContentTemplate></asp:UpdatePanel>
    </div>   
</div>      
    <div id = "maincolumn">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <telerik:RadGrid ID="grdItems"  CssClass="image" runat="server" AllowPaging="True" AllowCustomPaging="true" 
                                 AutoGenerateColumns="False" GridLines="None" OnItemDataBound="GrdItemsItemBound" ShowHeader="false" PageSize="5" Skin="Outlook" OnPageIndexChanged="GrdItemsPageIndexChanged">
                    <PagerStyle Mode="NextPrevAndNumeric" />
                    <MasterTableView TableLayout="Fixed" AutoGenerateColumns="True" DataKeyNames="Id">
                        <NoRecordsTemplate>
                            <br />
                            <br />
                            <br />
                            <center><asp:Image ImageUrl="Images/BIMXchange-logo.png" Height="75px" runat="server" />
                                <br />
                                <br />
                                <h2>No Items To Display</h2></center>
                        </NoRecordsTemplate>
                        <ItemTemplate>
                            <asp:Image ID="Image1" Style="float: left; padding: 10px; margin-top: 10px;" Width="80px" ImageUrl='<%#"Images/FileImages/" + Eval("AutodeskFile.Name") + ".gif"%>'
                                       runat="server" AlternateText="Stock Image" />
                            <div><div style="overflow: hidden; height: 14px; width: 430px;"><span style="color: Blue; height: 16px; font-weight: bold;">
                                                                                                <%#Eval("AutodeskFile.Name")%></span></div>
                                <br />
                                <span style="font-weight: bold;">Type:</span>
                                <%#Eval("Name")%>
                                <br />
                                <div id="divProps">
                                    <telerik:RadListView Height="30px" ID="lvParams" DataSource='<%#Eval("Parameters")%>' DataMember="Parameters" DataKeyNames="SearchName.Name,SearchValue.Value" runat="server">
                                        <ItemTemplate>
                                            <span style="font-weight: bold;"><%#Eval("SearchName.Name")%>:</span> <%#Eval("SearchValue.Value")%><br />
                                        </ItemTemplate></telerik:RadListView>
                                </div>
                                <asp:ImageButton ID="btnShowTC" ImageUrl="Images/TypeCatalogButton.png" Height="30px" Visible= '<%#!String.IsNullOrEmpty(Eval("TypeCatalogEntry").ToString())%>' OnClientClick='<%#"ShowTypeCatalog(" + Eval("AutodeskFile.Id") + "); return false;"%>' runat="server" />
                                <asp:ImageButton ID="btnDownload" ImageUrl="Images/LoadFileButton.png" Height="30px"  CommandName="LoadFile" CommandArgument='<%#Eval("Id")%>' OnClientClick='<%#"LoadFile(" + Eval("Id") + "); return false;"%>' runat="server" />
                            </div>
                        </ItemTemplate>
                        <PagerStyle Mode="NumericPages"/>
                    </MasterTableView>
                    <FilterMenu EnableImageSprites="False"></FilterMenu>

                    <HeaderContextMenu CssClass="GridContextMenu GridContextMenu_Default"></HeaderContextMenu>
                </telerik:RadGrid>  
                <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Default">
                </telerik:RadAjaxLoadingPanel>
            </ContentTemplate></asp:UpdatePanel>     
    </div>
    <%--Modals--%>

    <telerik:RadWindowManager EnableShadow="true" 
                              Behaviors="Close, Move, Resize,Maximize" ID="RadWindowManager" DestroyOnClose="true"
                              RestrictionZoneID="RestrictionZone" Opacity="99" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowLogin" Modal="true" Opacity="100" CssClass="LoginModal" VisibleStatusbar="false" Height="500px" DestroyOnClose="false" Width="450px" VisibleTitlebar="false" OnClientClose="LoginClose" Title="Login" NavigateUrl="~/login.aspx" Behaviors="None" runat="server">
            </telerik:RadWindow>
            <telerik:RadWindow ID="windowLibrary" Modal="true" Opacity="100" VisibleStatusbar="false" Height="370px" Width="400px" DestroyOnClose="false" Behaviors="Close, Move, Resize" Title="Change Library" OnClientClose="LibraryClose" NavigateUrl="~/library.aspx" runat="server">
            </telerik:RadWindow>
            <telerik:RadWindow ID="windowHelp" Title="Help" NavigateUrl="http://www.ENGworks.net/content" Height="600px" Width="800px" DestroyOnClose="false" runat="server">
            </telerik:RadWindow>
            <telerik:RadWindow ID="windowEula" Title="EULA" NavigateUrl="~/Eula.aspx" Modal="true" Height="600px" Width="600px" DestroyOnClose="false" Behaviors="Close, Move, Resize" runat="server">
            </telerik:RadWindow>
            <telerik:RadWindow ID="windowType" Modal="true" Opacity="100" VisibleStatusbar="false" Height="650px" Width="820px" DestroyOnClose="false" Behaviors="Close, Move, Resize" Title="Type Catalog" OnClientClose="OnClientClose" NavigateUrl="~/typecatalog.aspx" runat="server">
            </telerik:RadWindow>
            <telerik:RadWindow ID="windowRegister" Modal="true" Opacity="100" VisibleStatusbar="false" Height="600px" Width="950px" DestroyOnClose="false" Behaviors="Close, Move, Resize" Title="Register" NavigateUrl="~/registration.aspx" runat="server">
            </telerik:RadWindow>
            <telerik:RadWindow ID="windowNotRevit" Modal="true" Opacity="100" VisibleStatusbar="false" Height="200px" Width="400px" DestroyOnClose="false" Behaviors="Close, Move, Resize" Title="Oops!" NavigateUrl="~/NotRevit.aspx" runat="server">
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>