<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="SurveyList.aspx.cs" Inherits="Admin.SurveyList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="page-data-content">
        <table id="dg" class="easyui-datagrid" style="width:1096px; height: auto" pagination="false" 
            data-options="rownumbers:false,singleSelect:true,url:'ajax/GetSurveyList.ashx',method:'get',toolbar:'#tb',selectOnCheck:true,checkOnSelect:true">
            <thead>
                <tr>
                    <th data-options="field:'ck',checkbox:true"></th>
                    <th data-options="field:'SurveyID',hidden:true"></th>
                    <th data-options="field:'SurveyName',align:'left',halign:'center',width:300">問卷名稱</th>
                    <th data-options="field:'Instruction',align:'left',halign:'center',width:560">問卷說明</th>
                    <th data-options="field:'ClosedText',align:'center',halign:'center',width:70">停用</th>
                    <th data-options="field:'PageType',align:'left',halign:'center',width:120">報告類型</th>
                </tr>
            </thead>
        </table>   
        <!-- 列表工具列 -->
        <div id="tb" style="padding:2px 5px;">
            <div>
                <a href="javascript:do_add();" class="easyui-linkbutton" iconCls="icon-add">新增問卷</a>
                <a href="javascript:do_edit();" class="easyui-linkbutton" iconCls="icon-edit">編輯問卷</a>
                <a href="javascript:do_detail();" class="easyui-linkbutton" iconCls="icon-tools">設定題目</a>
           </div>
        </div>
    </div>
    <script type="text/javascript">
        //$('#dg').datagrid({
        //    onClickRow: function (index, row) {
        //        location.href = 'MemberPage.aspx?key=' + row.SurveyID;
        //    }
        //});

        // 關鍵字搜尋
        function do_search() {
            //var sYear;
            //var sPeriod;
            //sYear = $('#txtYear').combobox('getValue');
            //sPeriod = $('#txtPeriod').combobox('getValue');
            //$('#dg').datagrid('load', {
            //    year: sYear,
            //    period: sPeriod
            //});
        }

        function do_add()
        {
            location.href = "SurveyPage.aspx";
        }

        function do_edit()
        {
            var row = $('#dg').datagrid('getSelected');
            if (row) {
                var id=row.SurveyID;
                location.href = "SurveyPage.aspx?id="+id;
            }
        }

        function do_detail()
        {
            var row = $('#dg').datagrid('getSelected');
            if (row) {
                var id = row.SurveyID;
                location.href = "QuestionList.aspx?id=" + id;
            }
        }
    </script>
</asp:Content>
