<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BodyE.aspx.cs" Inherits="iosh.BodyE" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
    <script type="text/javascript" src="js/form2json.js"></script>
    <script type="text/javascript" src="js/jquery.validate.min.js"></script>
    <script type="text/javascript" src="js/additional-methods.min.js"></script>
    <script type="text/javascript" src="js/messages_zh_TW.min.js"></script>
    <script type="text/javascript" src="js/dotnet.js"></script>
    <link rel="stylesheet" type="text/css" href="css/body031.css"/>
    <style type="text/css">
        .option-item label
        {
            margin-right: 20px;
        }

        .error
        {
            color: #ff0000;
            font-weight: bold;
            font-style:italic;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
        <div class="title1 row">
            <div class=" col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <h2>繼續填寫</h2>
            </div>
        </div>
   <div class="box1 row">
        <div class="title2 ppage-header">
            <h3 class="bg-primary"><asp:Label ID="lblQuestion" runat="server" Text=""></asp:Label></h3>
            <asp:Label ID="lblQID" runat="server" Text="" Visible="false"></asp:Label>
        </div>
        <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
            <ItemTemplate>
                <div class="col-md-3 col-xs-6" style="margin-top: 15px;">
                    <asp:RadioButton ID="rdbAnswer" runat="server" GroupName="answer-group" Width="200px" Text='<%# Eval("SelectOption")%>' value='<%# Eval("OptionID")%>' OptionID='<%# Eval("OptionID")%>' />
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Repeater ID="Repeater2" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
            <ItemTemplate>
                <div class="col-lg-3 col-md-4 col-sm-6 col-xs-12">
                    <asp:Checkbox ID="chkAnswer" runat="server" GroupName="answer-group" Width="200px" Text='<%# Eval("SelectOption")%>' value='<%# Eval("OptionID")%>' OptionID='<%# Eval("OptionID")%>' />
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div class="row" style="margin-top: 55px;">
        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6" style="text-align:center;">
            <div class="btn-pre">
                <button type="button" class="btn btn-success col-md-6 col-sm-6 col-xs-12" onclick="javascript: history.go(-1);">上一頁</button>
            </div>
        </div>
        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6" style="text-align:center;">
            <div class="btn-next ">
                <asp:Button ID="btnSubmit" CssClass="btn btn-primary col-md-6 col-sm-6 col-xs-12" ClientIDMode="Static" runat="server" Text="下一頁" OnClick="btnSubmit_Click" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JScriptPlaceHolder" runat="server">
</asp:Content>
