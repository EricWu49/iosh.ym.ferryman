<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="HealthFinish.aspx.cs" Inherits="iosh.HealthFinish" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
    <style>
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
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <div class="bg-info" style="width: 100%; height: 30px; vertical-align: middle; margin: 10px;">
            這些是您選取的健康問題，請點選各標籤頁查看我們提供給您的衛教資訊
        </div>
        <div id="tabs">
            <asp:Repeater ID="Repeater1" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                        <li><a href='<%#  Eval("QuestionID", "#tab-{0}") %>'><%# Eval("QTitle") %></a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="Panel1" runat="server"></asp:Panel>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="PlaceHolder2" Visible="false" runat="server">
        <div style='width: 100%; text-align: center; height: 40px; margin: 10px;' class='bg-primary '><h2 style='color: #ffffff;'>恭喜您，您的健康狀況沒有影響工作適能的問題。</h2></div>
    </asp:PlaceHolder>
    <div class="row" style="margin-top: 10px; margin-bottom: 10px;">
        <div class="col-lg-3 col-lg-offset-3 col-md-4 col-md-offset-2 col-sm-6 col-xs-6" style="text-align: center;">
            <a class="btn btn-info noprint" style="width: 160px;" href="javascript: doprint();">列印建議報告</a>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 col-xs-6" style="text-align: center;">
            <asp:Button ID="btnReturn" CssClass="btn btn-info noprint" Width="160" runat="server" Text="返回構面選項頁" OnClick="btnReturn_Click" />
        </div>
    </div>

    <script type="text/javascript">
        $(function () {
            $("#tabs").tabs({
                beforeLoad: function (event, ui) {
                    ui.jqXHR.fail(function () {
                        ui.panel.html("無法載入所需要的衛教資源。");
                    });
                }
            });
        });

        function doprint() {
            window.print();
        }
    </script>
</asp:Content>
