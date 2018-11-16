<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BodyInput.aspx.cs" Inherits="Admin.BodyInput" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <link rel="stylesheet" type="text/css" href="css/easyui-form.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
	<script src="js/highcharts.js"></script>
	<script src="js/highcharts-more.js"></script>
    <script type="text/javascript" src="js/form2object.js" ></script>
    <style type="text/css" media="print">
        .noprint
        {
            display: none;
        }
    </style>
    <style type="text/css">
        .level1
        {
            list-style-image:url("images/blue.png");
        }
        .level2
        {
            list-style-image:url("images/green.png");
        }
        .level3
        {
            list-style-image:url("images/yellow.png");
        }
        .level4
        {
            list-style-image:url("images/red.png");
        }
        .form_input_element {
            width: 100%;
        }
        .left_chart {
            float: left;
            width: 46%;
            text-align:right;
            padding-left: 20px;
        }
        .right_chart {
            float: right;
            width: 46%;
            text-align:left;
            padding-left: 10px;
            padding-right: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>肌肉骨骼健康評估 資料輸入</h2>
    <div id="Page1" class="easyui-panel" title="輸入評估報告序號" style="width:100%;height:auto;padding:10px; margin-bottom: 10px;">
        <div class="form_input_content">
                <asp:Label ID="Label1" Text="評估報告序號" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="txtReportSN" CssClass="easyui-textbox" data-options="required:true" runat="server"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnQuery" CssClass="easyui-linkbutton" Width="120" Height="25" data-options="iconCls:'icon-search'" runat="server" Text="查詢" OnClick="btnQuery_Click" />
        </div>
    </div>
    <div style="text-align: left;">
    <asp:Panel ID="pnlReport" runat="server" >
        <div>評估報告序號：<b><asp:Label ID="lblPaperSN" runat="server" Text=""></asp:Label></b></div>
        <div>
            <asp:Label ID="Label2" runat="server" Text="肌肉骨骼健康風險值："></asp:Label>
            <asp:Label ID="lblIndex" runat="server" Text=""></asp:Label>
            <asp:HiddenField ID="hidRiskID" ClientIDMode="Static" runat="server" />
            <asp:HiddenField ID="hidPaperID" ClientIDMode="Static" runat="server" />
        </div>
        <div class="form_input_content">
            <div class="left_chart">
                <div id="myChart_Radar" style="min-width: 400px; max-width: 600px; height: 300px; margin: 0 auto"></div>
            </div>
            <div class="right_chart">
                <asp:Literal ID="litResultList" runat="server"></asp:Literal>
            </div>
        </div>
        <div style="clear: both;"></div>
        <div id="tabs">
            <asp:Repeater ID="Repeater1" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                        <li><a href='<%#  Eval("ResourceCode", "#tab-{0}") %>'><%# Eval("QTitle") %></a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="Panel1" runat="server"></asp:Panel>
        </div>
    </asp:Panel>
    </div>
    <script type="text/javascript">
        $(function () {
            $("#tabs").tabs({
                beforeLoad: function (event, ui) {
                    ui.jqXHR.fail(function () {
                        ui.panel.html("無法載入結果。");
                    });
                }
            });
        });

        function submitForm(code) {
            var formData = form2object('resource-' + code, '$', false);
            var id = $("#hidPaperID").val();

            $.ajax({
                type: "GET",
                url: "ajax/InsertPaperResource.ashx",
                data: { "id": id, "code": code, "formdata": JSON.stringify(formData) },
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                cache: false,
                success: function (result) {
                    if (result == "") {
                        window.alert("已儲存");
                        return true;
                    } else {
                        window.alert(result);
                        return false;
                    }
                },
                error: function () {
                    alert('Error!');
                    return false;
                }
            });
        }
    </script>
</asp:Content>
