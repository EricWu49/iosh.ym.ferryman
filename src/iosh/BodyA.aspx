﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BodyA.aspx.cs" Inherits="iosh.BodyA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <link rel="stylesheet" type="text/css" href="css/body01.css"/>	<!-- UI/UX Design -->
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
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="title1 row">
                <div class=" col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <h2>開始填寫</h2>
                </div>
            </div>
            <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                <HeaderTemplate>
                   <div class="box1">
                        <div class="title2 page-header">
                            <h3 class="bg-primary">一、最近12個月，您身體各部位有沒有發生痠痛？請依據痠痛情況在適當的等級進行勾選。</h3>
                        </div>
                        <div>身體部位圖：</div>
                        <asp:Image ID="Image1" ImageUrl="~/object/images/24639.jpg" runat="server" AlternateText="身體部位圖" />
                </HeaderTemplate>
                <ItemTemplate>
                    <p>
                        <div class="title3 bg-info">
                            <asp:Label ID="lblQuestion" runat="server" Text='<%# Eval("QTitle")%>'></asp:Label>
                        </div>
                        <p class="text-hint text-info">(0表示沒有痠痛，5表示劇烈疼痛到完全無法忍受，需要立即到急診就醫；如果兩邊都痛，請以最痛邊來判斷痠痛分數)</p>
                        <div class="box-border-btm">
                            <asp:RadioButtonList ID="rdbOption" runat="server" ClientIDMode="AutoID" CssClass="option-item" RepeatDirection="Horizontal" RepeatLayout="Flow" QuestionID='<%# Eval("QuestionID")%>'>
                                <asp:ListItem Value="0" class="input_box">0</asp:ListItem>
                                <asp:ListItem Value="1" class="input_box">1</asp:ListItem>
                                <asp:ListItem Value="2" class="input_box">2</asp:ListItem>
                                <asp:ListItem Value="3" class="input_box">3</asp:ListItem>
                                <asp:ListItem Value="4" class="input_box">4</asp:ListItem>
                                <asp:ListItem Value="5" class="input_box">5</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <asp:Panel ID="Panel1" runat="server">
                            <div id='<%# Eval("QuestionID", "position-select-{0}")%>' style="display: none;">
                                <span>痠痛位置：</span>
                                <asp:RadioButtonList ID="rdbPosition" runat="server" CssClass="option-item" RepeatDirection="Horizontal" RepeatLayout="Flow" QuestionID='<%# Eval("QuestionID")%>'>
                                    <asp:ListItem Value="1" class="input_box">左側</asp:ListItem>
                                    <asp:ListItem Value="2" class="input_box">右側</asp:ListItem>
                                    <asp:ListItem Value="3" class="input_box">兩側皆有</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </asp:Panel>
                    </p>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div class="row" style="margin-top: 10px;">
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
    <script type="text/javascript" >
        var validator = $("#form1").validate({
            rules: {
                <asp:Literal ID="litRule" runat="server"></asp:Literal>
            }
        });

        $("#form1").submit(function () {
            if ($("#form1").valid()) {
                return true;
            } else {
                window.alert("資料不完整，請檢查。");
                return false;
            };
        });

        function showit(obj, id, show)
        {
            if (show)
            {
                $("#position-select-" + id).css("display", "block");
            }
            else
            {
                $("#position-select-" + id).css("display", "none");
            }
            item_checked(obj);
        }
    </script>
</asp:Content>
