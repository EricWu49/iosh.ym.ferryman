<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Human02.aspx.cs" Inherits="iosh.Human02" %>
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
                <ItemTemplate>
                    <p>
                        <asp:Panel ID="PanelTitle" runat="server">
                            <div class="page-header">
                                <h3 class="bg-primary">
                                    <asp:Label ID="lblQTitle" runat="server" Text='<%# Eval("QTitle")%>'></asp:Label></h3>
                                <p class="text-info">請參考每題的圖片，回答工作時是否有類似的情形</p>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="PanelQuestion" runat="server">
                            <div class="bg-info">
                                <asp:Label ID="lblQuestion" runat="server" Text='<%# Eval("QTitle")%>'></asp:Label>
                                <asp:Label ID="lblWarning" runat="server" Text="   *此題尚未作答" Font-Bold="true"  ForeColor="Red" Visible="false"></asp:Label>
                            </div>
                            <div>
                                <asp:Image ID="imgQuestion" ImageUrl='<%# Eval("QuestionID", "~/object/images/human/Q{0}.jpg") %>' runat="server" />
                            </div>
                            <div>
                                <asp:CheckBoxList ID="rdbOption" runat="server" CssClass="option-item" RepeatDirection="Horizontal" RepeatLayout="Flow" QuestionID='<%# Eval("QuestionID")%>'>
                                </asp:CheckBoxList>
                            </div>
                        </asp:Panel> 
                    </p>
                </ItemTemplate>
            </asp:Repeater>
            <asp:ListBox ID="lstQuestion" Visible="false" runat="server"></asp:ListBox>
        </div>
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
        //    }
        //});

        $("#form1").submit(function () {
            if ($("#form1").valid()) {
                return true;
            } else {
                window.alert("資料不完整，請檢查。");
                return false;
            };
        });

        // 選擇沒有此現象
        function none_check(obj)
        {
            var group = $(obj).parent().attr("group");
            if (obj.checked)
            {
                group=group.replace("_0", "_1");
                $("span[group='" + group + "'] > input[type='checkbox']").attr("checked", false);
            }
            else
            {

            }
        }

        // 選擇其他現象
        function other_check(obj)
        {
            var group = $(obj).parent().attr("group");
            if (obj.checked) {
                group=group.replace("_1", "_0");
                $("span[group='" + group + "'] > input[type='checkbox']").attr("checked", false);
            }
            else {

            }
        }
    </script>
</asp:Content>
