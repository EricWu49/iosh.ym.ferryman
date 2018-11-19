<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Survey_Body.aspx.cs" Inherits="iosh.Survey_Body" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="css/Survey_body.css"/>
    <style>
        input button{
            margin-right: 20px;
        }

        /*#btnGo {
            background: url("images/btn_icon_write.svg") 
            left 3px top 5px no-repeat #563d7c;
            width:100%;
        }

        #btnQuery {
            background: url("images/btn_icon_srch.svg") 
            left 3px top 5px no-repeat #563d7c;
            width:100%;
        }*/
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FormContentPlaceHolder" runat="server">
    <div style="font-size: 16px">
        <div class="title1 row">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <h2>肌肉骨骼健康評估</h2>
                <p>親愛的朋友，您好！<br />
                    您身體上是否有痠痛影響到您的工作表現嗎？
                    肌肉骨格健康評估將協助您了解自己肌肉骨骼的健康風險程度。</p>
            </div>
        </div>
        <div class="box1 row">
            <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                <img class="col-sm-1 col-sm-offset-2" src="images/survey_body_img_muscle.svg" />
                <ul class="col-sm-7">
                    <li>若您之前已經填寫過，可以按「查詢報告」按鈕，於下一個查詢畫面輸入系統提供給您的「評估報告序號」，即可查詢上次的報告內容。</li>
                    <li>若您還未填寫過，或想要重新評估目前最新的健康風險程度，請按「開始填寫」按鈕開始。</li>
                </ul>
                
            </div>
        </div>
        <div class="button_ilosh row">
            <button class="col-sm-4 col-sm-offset-2 col-xs-12" runat="server" onserverclick="btnGo_Click">
                <input type="submit" name="ctl00$FormContentPlaceHolder$btnGo" value="開始填寫" id="FormContentPlaceHolder_btnGo" class="btn btn-primary" />
                <img src="images/btn_icon_write.svg" class="btn_icon" />
           </button>
            <button class="col-sm-4 col-xs-12" runat="server" onserverclick="btnQuery_Click">
                <input type="submit" name="ctl00$FormContentPlaceHolder$btnQuery" value="查詢報告" id="FormContentPlaceHolder_btnQuery" class="btn btn-info" />
                <img src="images/btn_icon_write.svg" class="btn_icon" />
            </button>
            <asp:Label ID="lblSurveyPage" runat="server" Text="" Visible="false"></asp:Label>
        </div>
    </div>
</asp:Content>
