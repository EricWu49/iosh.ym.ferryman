<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Page2.aspx.cs" Inherits="iosh.Page2" %>
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
    <div class="text-primary bg-info" style="margin-bottom: 0px;">
        <asp:Label ID="lblQuestion332" runat="server" Text="三、請勾選您現有或曾經經醫師確定診斷或治療的疾病或傷害。(可複選)"></asp:Label><br/>
        <small>若沒有確定診斷或治療的疾病或傷害，請直接點選「下一頁」進行填寫</small>
    </div>
    <div class="row" style="margin-top: 10px;">
        <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
            <ItemTemplate>
                <div class='col-lg-3 col-md-4 col-sm-6 col-xs-12'>
                    <asp:CheckBox ID="chkOption332" Text='<%# Eval("SelectOption")%>' Value='<%# Eval("SelectValue")%>' runat="server" QuestionID="332" OptionID='<%# Eval("OptionID")%>' />
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:HiddenField ID="hidCount" ClientIDMode="Static" Value="0" runat="server" />
    </div>
    <div class="row" style="margin-top: 10px;">
        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6" style="text-align:center;">
            <button type="button" class="btn btn-success" style="width: 200px; height: 30px;" onclick="javascript: history.go(-1);">上一頁</button>
        </div>
        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6" style="text-align:center;">
            <asp:Button ID="btnSubmit" CssClass="btn btn-primary" Width="200" Height="30" ClientIDMode="Static" runat="server" Text="下一頁" OnClick="btnSubmit_Click" />
        </div>
    </div>
    <script type="text/javascript" >
        //var validator = $("#form1").validate({
        //    rules: {
        //        <asp:Literal ID="litRule" runat="server"></asp:Literal>
        //}
        //});

        $("#form1").submit(function () {
            if ($("#form1").valid()) {
                //if (parseInt($("#hidCount").val())<1)
                //{
                //    window.alert("第一題至少需勾選一項。");
                //    return false;
                //}
                //else
                //{
                //    return true;
                //}
                return true;
            } else {
                window.alert("資料不完整，請檢查。");
                return false;
            };
        });

        function check_click(obj)
        {
            if (obj.checked)
            {
                $("#hidCount").val(parseInt($("#hidCount").val()) + 1);
            }
            else
            {
                $("#hidCount").val(parseInt($("#hidCount").val()) - 1);
            }
        }
    </script>
</asp:Content>
