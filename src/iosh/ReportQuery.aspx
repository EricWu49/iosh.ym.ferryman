<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="ReportQuery.aspx.cs" Inherits="iosh.ReportQuery" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <div class="control-group">
        <label class="control-label" for="txtPaperSN">請輸入您的評估報告序號：</label>
        <div class="controls">
            <asp:TextBox ID="txtPaperSN" ClientIDMode="Static" runat="server"></asp:TextBox>
        </div>
    </div>
    <div class="control-group">
        <div class="controls">
            <asp:Button ID="btnQuery" CssClass="btn btn-primary" runat="server" Text="查詢報告" OnClick="btnQuery_Click" />
        </div>
    </div>
</asp:Content>
