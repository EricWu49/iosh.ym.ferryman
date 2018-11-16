<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="ResultReport1.aspx.cs" Inherits="iosh.ResultReport1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="themes/icon.css" />
    <script src="js/easyui-common.js"></script>
    <script type="text/javascript" src="js/jquery.easyui.min.js"></script>
	<script src="js/highcharts.js"></script>
	<script src="js/highcharts-more.js"></script>
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
    <asp:Repeater ID="Repeater2" runat="server">
        <ItemTemplate>
            <div class="easyui-panel" title='' style="padding:10px; width: 600px;" data-options="href: 'SurveyReport.aspx?id=<%# Eval("ReportID")%>',noheader: true, border: false, closed: <%# Eval("Printable")%>">
            </div>
        </ItemTemplate>
        <FooterTemplate>
            <div style="padding: 10px; text-align: center">
                <a class="btn btn-info noprint" href="javascript: doprint();">列印結果</a>
                <a id="btnClose" style="margin-left:30px;" class="btn btn-info noprint" href="ResultPage1.aspx">關閉預覽</a>
<%--                <asp:Button ID="btnPrint" CssClass="btn btn-info noprint" ClientIDMode="Static" runat="server" Text="列印結果" OnClientClick="javascript: return doprint();" />
                <asp:Button ID="btnClose" CssClass="btn btn-info noprint" ClientIDMode="Static" runat="server" Text="關閉預覽" OnClick="btnClose_Click" />--%>
            </div>
        </FooterTemplate>
    </asp:Repeater>
    <script type="text/javascript">
        function doprint()
        {
            window.print();
            return false;
        }
    </script>
</asp:Content>
