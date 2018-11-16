<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="HumanFinish.aspx.cs" Inherits="iosh.HumanFinish" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <style type="text/css" media="print">
        .noprint
        {
            display: none;
        }

        #btnClose
        {
            margin-left: 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <asp:Panel ID="Panel1" runat="server"></asp:Panel>
    <asp:Literal ID="litScript" runat="server"></asp:Literal>
    <div class="row" style="margin-top: 10px; margin-bottom: 10px;">
        <div class="col-lg-3 col-lg-offset-3 col-md-4 col-md-offset-2 col-sm-6 col-xs-6" style="text-align: center;">
            <a class="btn btn-info noprint" style="width: 160px;" href="javascript: doprint();">列印建議報告</a>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 col-xs-6" style="text-align: center;">
            <asp:Button ID="btnReturn" CssClass="btn btn-info noprint" Width="160" runat="server" Text="返回" OnClick="btnReturn_Click" />
        </div>
    </div>

    <script type="text/javascript">
        function doprint()
        {
            window.print();
        }
    </script>
</asp:Content>
