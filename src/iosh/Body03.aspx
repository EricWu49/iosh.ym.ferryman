<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Body03.aspx.cs" Inherits="iosh.Body03" %>
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
        <h3 class="bg-primary">三、基礎動作控制篩檢</h3>
        <p class="text-info">接下來請您參考圖片及文字說明進行下列動作的檢測</p>
    </div>
    <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
        <ItemTemplate>
            <div class="row" style="margin-top: 10px;">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="bg-info">
                        <asp:Label ID="lblQuestion" runat="server" Text='<%# Eval("QTitle")%>'></asp:Label>
                    </div>
                </div>
            </div>
            <div class="row" style="margin-top: 10px;">
                <div class="col-lg-4 col-md-6 col-sm-12 col-xs-12" style="text-align:center;">
                    <asp:Image ID="imgMotion" runat="server" ImageUrl='<%# Eval("MotionPhoto", "~/object/images/motion/{0}") %>' />
                </div>
                <div class="col-lg-8 col-md-6 col-sm-12 col-xs-12" style="text-align:left;">
                    <div class="row" style="margin-top: 10px;">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 bg-success">
	                        <span>測試姿勢：</span><asp:Label ID="Label1" runat="server" Text='<%# Eval("Posture")%>'></asp:Label><br/>
	                        <span>測試動作：</span><asp:Label ID="Label2" runat="server" Text='<%# Eval("Action")%>'></asp:Label><br/>
	                        <span>動作標準：</span><asp:Label ID="Label3" runat="server" Text='<%# Eval("Standard")%>'></asp:Label>
                        </div>
                    </div>
                    <div class="row" style="margin-top: 10px;">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 bg-info">
                            <span>請選擇測試結果：</span><br />
                            <asp:RadioButtonList ID="rdbOption" runat="server" ClientIDMode="AutoID" CssClass="option-item" RepeatDirection="Vertical"  RepeatLayout="Flow" QuestionID='<%# Eval("QuestionID")%>'>
                                <asp:ListItem Value="10">通過測試</asp:ListItem>
                                <asp:ListItem Value="02">動作未達標準，不會痛</asp:ListItem>
                                <asp:ListItem Value="03">動作未達標準且會痛</asp:ListItem>
                                <asp:ListItem Value="01">動作達標準但會痛</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblPageNo" runat="server" Text="5" Visible="false" ></asp:Label>
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
