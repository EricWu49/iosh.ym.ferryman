<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Suggest.aspx.cs" Inherits="iosh.Suggest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="themes/icon.css" />
    <script src="js/easyui-common.js"></script>
    <script type="text/javascript" src="js/jquery.easyui.min.js"></script>
    <style>
        .clear_list
        {
            list-style:none;
        }

        .situation_list
        {
            list-style-image:url("images/red-dot.gif");
        }

        .strategy_list
        {
            list-style-image:url("images/blue-dot.gif");
        }

        .url_link
        {
            list-style-image:url("images/url.png");
        }

        .video_link{
            list-style-image:url("images/video.png");
        }

        .file_link{
            list-style-image:url("images/pdf.gif");
        }

        .image_link{
            list-style-image:url("images/Image.png");
        }
    </style>
    <style type="text/css" media="print">
        .noprint
        {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <div class="alert alert-warning"  style="width: 100%;" role="alert">一、分析結果</div>
    <asp:Panel ID="Panel_Situation" runat="server"></asp:Panel>
    <div class="alert alert-info" style="width: 100%; margin-top: 15px;" role="alert">二、建議對策與相關衛教資訊</div>
    <asp:Repeater ID="rptSuggest" runat="server"  OnItemDataBound="rptSuggest_ItemDataBound">
        <ItemTemplate>
            <div class="row">
                <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                    <asp:Label ID="lblSuggest" runat="server" Text='<%# Eval("Suggest")%>'></asp:Label>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                    <asp:Literal ID="litStrategy" runat="server"></asp:Literal>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Panel ID="Panel_Good" Visible="false" runat="server">
        <h4>請繼續保持！</h4>
    </asp:Panel>
    <div class="row" style="margin-top: 10px; margin-bottom: 10px;">
        <div class="col-lg-3 col-lg-offset-3 col-md-4 col-md-offset-2 col-sm-6 col-xs-6" style="text-align: center;">
            <a class="btn btn-info noprint" style="width: 160px;" href="javascript: doprint();">列印建議報告</a>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 col-xs-6" style="text-align: center;">
            <asp:Button ID="btnReturn" CssClass="btn btn-info noprint" Width="160" runat="server" Text="返回構面選項頁" OnClick="btnReturn_Click" />
        </div>
    </div>

    <script type="text/javascript">
        function doprint()
        {
            window.print();
        }
    </script>
</asp:Content>
