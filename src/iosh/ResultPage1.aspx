<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" EnableEventValidation="false"  CodeBehind="ResultPage1.aspx.cs" Inherits="iosh.ResultPage1" %>
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
    <asp:Repeater ID="Repeater1" runat="server">
        <HeaderTemplate>
            <div id="tt" class="easyui-tabs" style="width:100%;" data-options="plain: true, narrow: true">
        </HeaderTemplate>
        <ItemTemplate>
            <div title='<%# Eval("ReportTitle")%>' href='SurveyReport.aspx?id=<%# Eval("ReportID")%>' style="padding:10px">
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
            <div style="padding: 10px; text-align: center">
                <a class="btn btn-info noprint" href="ResultReport1.aspx">預覽列印</a>
                <%--<asp:Button ID="btnPreview" CssClass="btn btn-info noprint" ClientIDMode="Static" runat="server" Text="預覽列印" OnClick="btnPreview_Click" />--%>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>
