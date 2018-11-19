<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BodyD.aspx.cs" Inherits="iosh.BodyD" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/jquery-ui.min.css" />
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
    <script type="text/javascript" src="js/form2json.js"></script>
    <script type="text/javascript" src="js/jquery.validate.min.js"></script>
    <script type="text/javascript" src="js/additional-methods.min.js"></script>
    <script type="text/javascript" src="js/messages_zh_TW.min.js"></script>
    <link rel="stylesheet" type="text/css" href="css/body03.css"/>
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
                <div class="title2 page-header">
                    <h3 class="bg-primary">三、基礎動作控制篩檢</h3>
                </div>
                <p class="text-hint text-info">接下來請您參考圖片及文字說明進行下列動作的檢測</p>
                <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                    <ItemTemplate>
                        <p>
                            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                <div class="title3 bg-info">
                                    <asp:Label ID="lblQuestion" runat="server" Text='<%# Eval("QTitle")%>'></asp:Label>
                                </div>
                            </div>
                        <div class="row box-border-btm">
                            <div class="col-md-3 col-sm-3  col-xs-12" style="text-align:center;">
                                <asp:Image ID="imgMotion" runat="server" ImageUrl='<%# Eval("MotionPhoto", "~/object/images/motion/{0}") %>' />
                            </div>
                            <div class="col-md-8 col-sm-9">
                                <div class="row">
                                    <div class="test_wording col-xs-12" style="text-align:left;">
	                                    <span class="test_title">測試姿勢：</span><asp:Label ID="Label1" runat="server" Text='<%# Eval("Posture")%>'></asp:Label><br/>
	                                    <span class="test_title">測試動作：</span><asp:Label ID="Label2" runat="server" Text='<%# Eval("Action")%>'></asp:Label><br/>
	                                    <span class="test_title">動作標準：</span><asp:Label ID="Label3" runat="server" Text='<%# Eval("Standard")%>'></asp:Label><br />
                                        <span class="test_title">請選擇測試結果：</span>
                                    </div>
                                </div>
                                <div class="col-xs-12">
                                    <asp:RadioButtonList ID="rdbOption" runat="server" ClientIDMode="AutoID" CssClass="option-item" RepeatDirection="Vertical"  RepeatLayout="Flow" QuestionID='<%# Eval("QuestionID")%>'>
                                        <asp:ListItem Value="10">動作達標準，不會痠痛</asp:ListItem>
                                        <asp:ListItem Value="01">動作達標準，會痠痛</asp:ListItem>
                                        <asp:ListItem Value="02">動作未達標準，不會痠痛</asp:ListItem>
                                        <asp:ListItem Value="03">動作未達標準，會痠痛</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                        </div>
                        </p>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    <asp:Label ID="lblPageNo" runat="server" Text="5" Visible="false" ></asp:Label>
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
    </script>
</asp:Content>
