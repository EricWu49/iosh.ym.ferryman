<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Body030.aspx.cs" Inherits="iosh.Body030" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
    <script type="text/javascript" src="js/form2json.js"></script>
    <script type="text/javascript" src="js/jquery.validate.min.js"></script>
    <script type="text/javascript" src="js/additional-methods.min.js"></script>
    <script type="text/javascript" src="js/messages_zh_TW.min.js"></script>
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
            <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                <HeaderTemplate>
                    <div class="page-header">
                        <h3 class="bg-primary">三、基礎動作篩檢測試</h3>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <p>
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <div class="bg-info">
                                <asp:Label ID="lblQuestion" runat="server" Text='<%# Eval("QTitle")%>'></asp:Label>
                            </div>
                        </div>
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <asp:RadioButtonList ID="rdbOption" runat="server" ClientIDMode="AutoID" CssClass="option-item" RepeatDirection="Horizontal" RepeatLayout="Flow" QuestionID='<%# Eval("QuestionID")%>'>
                                <asp:ListItem Text="是" Value="1"></asp:ListItem>
                                <asp:ListItem Text="否" Value="0"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </p>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <asp:Label ID="lblPageNo" runat="server" Text="4" Visible="false" ></asp:Label>
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6" style="text-align:center;">
            <button type="button" class="btn btn-success" style="width: 200px; height: 30px;" onclick="javascript: history.go(-1);">上一頁</button>
        </div>
        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6" style="text-align:center;">
            <asp:Button ID="btnSubmit" CssClass="btn btn-primary" Width="200" Height="30" ClientIDMode="Static" runat="server" Text="下一頁" OnClick="btnSubmit_Click" />
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
    </script>
</asp:Content>
