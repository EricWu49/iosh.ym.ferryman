<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BodyE.aspx.cs" Inherits="iosh.BodyE" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
    <script type="text/javascript" src="js/form2json.js"></script>
    <script type="text/javascript" src="js/jquery.validate.min.js"></script>
    <script type="text/javascript" src="js/additional-methods.min.js"></script>
    <script type="text/javascript" src="js/messages_zh_TW.min.js"></script>
    <script type="text/javascript" src="js/dotnet.js"></script>
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
    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <asp:Label ID="lblQuestion" runat="server" Text=""></asp:Label>
            <asp:Label ID="lblQID" runat="server" Text="" Visible="false"></asp:Label>
        </div>
        <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
            <ItemTemplate>
                <div class="col-lg-3 col-md-4 col-sm-6 col-xs-12">
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
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6" style="text-align:center;">
            <button type="button" class="btn btn-success" style="width: 200px; height: 30px;" onclick="javascript: history.go(-1);">上一頁</button>
        </div>
        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6" style="text-align:center;">
            <asp:Button ID="btnSubmit" CssClass="btn btn-primary" Width="200" Height="30" ClientIDMode="Static" runat="server" Text="下一頁" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JScriptPlaceHolder" runat="server">
</asp:Content>
