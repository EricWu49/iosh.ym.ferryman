<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Place01.aspx.cs" Inherits="iosh.Place01" %>
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
    <div class="page-header">
        <h2 class="bg-primary">工作環境危害因子問卷</h2>
        <p class="text-info">
            此份問卷可幫助您檢視目前的工作環境安全衛生狀況。您可以根據填答的狀況向安全衛生主管人員反應。<br />
            若您使用工作單位提供之單機版系統，系統也會刪去可辨識個人身分的資訊。<br />
            本系統亦提供以統整方式呈現給您工作單位安全衛生主管人員之選項。
        </p>
    </div>
    <div class="text-primary bg-info" style="margin-bottom: 0px;">
        <asp:Label ID="lblQuestion186" runat="server" Text="一、您在工作環境中可能會遭遇下列哪些危險？(可複選)"></asp:Label><br />
        <small>若不確定有無這些危害，請選擇「16. 不清楚是否有這些危害」</small>
    </div>
    <div class="row" style="margin-top: 10px;">
        <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
            <ItemTemplate>
                <div class='col-lg-3 col-md-4 col-sm-6 col-xs-12'>
                    <asp:CheckBox ID="chkOption186" CssClass="dangous_item" Text='<%# Eval("SelectOption")%>' runat="server" QuestionID="186" OptionID='<%# Eval("OptionID")%>' group='<%# Eval("ColumnNo", "Group_{0}")%>' index='<%# Eval("SortID")%>'/>
                    <asp:TextBox ID="txtOption186" Visible="false" Width="120" placeholder="請說明..." runat="server"></asp:TextBox>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:HiddenField ID="hidCount" ClientIDMode="Static" Value="0" runat="server" />
    </div>
    <div class="text-primary bg-info" style="margin-bottom: 0px;">
        <asp:Label ID="lblQuestion258" runat="server" Text="二、前述您認為會遭遇的危險中，何者最可能發生？"></asp:Label>
        <br />
        <small>請填入前述01-15一個代碼</small>
    </div>
    <div>
        <asp:TextBox ID="txtQuestion258" ClientIDMode="Static" QuestionID="258" runat="server"></asp:TextBox>
    </div>
    <div class="text-primary bg-info" style="margin-bottom: 0px;">
        <span>三、您的工作環境有沒有下列情形？</span>
    </div>
    <div>
        <asp:Repeater ID="Repeater2" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
            <ItemTemplate>
                <div class="bg-info">
                    <asp:Label ID="lblQuestion187" runat="server" Text='<%# Eval("QTitle")%>'></asp:Label>
                </div>
                <div>
                    <asp:RadioButtonList ID="rdbOption187" CssClass="option-item" runat="server" QuestionID='<%# Eval("QuestionID")%>' RepeatDirection="Horizontal" RepeatLayout="Flow" ></asp:RadioButtonList>
                </div>
            </ItemTemplate>
        </asp:Repeater>
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
        var validator = $("#form1").validate({
            rules: {
                <asp:Literal ID="litRule" runat="server"></asp:Literal>
            }
        });

        $("#form1").submit(function () {
            if ($("#form1").valid()) {
                if (parseInt($("#hidCount").val())<1)
                {
                    window.alert("第一題至少需勾選一項。");
                    return false;
                }
                else
                {
                    if ($("#txtQuestion258").attr("disabled")=="disabled")
                    {
                        return true;
                    }
                    else
                    {
                        return input_check();
                    }
                }
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
                var group = $(obj).parent().attr("group");
                switch (group)
                {
                    case "Group_1":
                        // 選擇其他危害
                        $("span[group='Group_2'] > input[type='checkbox']").attr("checked", false);
                        $("span[group='Group_3'] > input[type='checkbox']").attr("checked", false);
                        $("#txtQuestion258").removeAttr("disabled");
                        break;
                    case "Group_2":
                        // 選擇不清楚
                        $("span[group='Group_1'] > input[type='checkbox']").attr("checked", false);
                        $("span[group='Group_3'] > input[type='checkbox']").attr("checked", false);
                        $("#txtQuestion258").attr("disabled", "disabled");
                        break;
                    case "Group_3":
                        // 選擇沒有危害
                        $("span[group='Group_1'] > input[type='checkbox']").attr("checked", false);
                        $("span[group='Group_2'] > input[type='checkbox']").attr("checked", false);
                        $("#txtQuestion258").attr("disabled", "disabled");
                        break;
                }
            }
            else
            {
                $("#hidCount").val(parseInt($("#hidCount").val()) - 1);
            }
        }

        function others_check(obj, target, require)
        {
            check_click(obj);
            if (require)
            {
                $("#"+target).removeAttr("disabled");
            }
            else
            {
                $("#"+target).attr("disabled", "disabled");
            }
        }

        $("#txtQuestion258").change(function(){
            return input_check();
        })

        function input_check()
        {
            var val=$("#txtQuestion258").val();
            if ($("span[class='dangous_item'][index='" + val + "'] > input[type='checkbox']")[0].checked)
            {
                return true;
            }
            else
            {
                window.alert("您所輸入的危害代碼並沒有在第一題中被選取，請確認。");
                return false;
            }
        }
    </script>
</asp:Content>
