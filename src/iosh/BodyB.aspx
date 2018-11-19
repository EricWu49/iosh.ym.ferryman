<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BodyB.aspx.cs" Inherits="iosh.BodyB" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
    <script type="text/javascript" src="js/form2json.js"></script>
    <script type="text/javascript" src="js/jquery.validate.min.js"></script>
    <script type="text/javascript" src="js/additional-methods.min.js"></script>
    <script type="text/javascript" src="js/messages_zh_TW.min.js"></script>
    <link rel="stylesheet" type="text/css" href="css/body02.css"/>
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
            <div class="title1 row">
                <div class=" col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <h2>繼續填寫</h2>
                </div>
            </div>
            <div class="box1">
                <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                    <HeaderTemplate>
                        <div class="title2 page-header">
                            <h3 class="bg-primary">二、痠痛對您工作的影響如何？</h3>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <p>
                            <div class="title3 bg-info">
                                <asp:Label ID="lblQuestion" runat="server" Text='<%# Eval("QTitle")%>'></asp:Label></div>
                            <div class="box-border-btm">
                                <asp:RadioButtonList ID="rdbOption" runat="server" ClientIDMode="AutoID" CssClass="option-item" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="3" QuestionID='<%# Eval("QuestionID")%>'>
                                    <asp:ListItem Value="2" class="input_box" onclick="javascript:item_checked(this);">無影響</asp:ListItem>
                                    <asp:ListItem Value="3" class="input_box" onclick="javascript:item_checked(this);">有影響，但仍可正常工作</asp:ListItem>
                                    <asp:ListItem Value="4" class="input_box" onclick="javascript:item_checked(this);">有蠻大的影響：工作速度變慢或動作時會痛</asp:ListItem>
                                    <asp:ListItem Value="5" class="input_box" onclick="javascript:item_checked(this);">有很大的影響，需請假休息或去看病</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </p>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <asp:ListBox ID="lstQuestion" Visible="false" runat="server"></asp:ListBox>
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JScriptPlaceHolder" runat="server">
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
    </script>
</asp:Content>
