<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="HealthSetting.aspx.cs" Inherits="Admin.HealthSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="layout-content" class="easyui-layout" style="width:100%; height:500px">
        <div data-options="region:'west',title:'1.題目',split:false,collapsible:false" style="width:600px;height:200px">
            <div id="list-question"></div>
        </div>
        <div data-options="region:'center',title:'2.選項'" style="padding:0px;background:#eee;">
            <div id="list-option"></div>
        </div>
        <div data-options="region:'south',title:'3.衛教資源',split:false,collapsible:false" style="height:300px;">
            <table id="dg" class="easyui-datagrid" style="width:95%;height:280px;margin:0px;" 
                data-options="singleSelect:true,collapsible:true,toolbar:'#tb',nowrap:false,striped:true,cache:false">
                <thead>
                    <tr>
                        <th data-options="field:'uniqueid',hidden:true"></th>
                        <th data-options="field:'strategyid',hidden:true"></th>
                        <th data-options="field:'strategyname',width:300,align:'left',halign:'center'">資源名稱</th>
                        <th data-options="field:'strategytype',width:100,align:'center',halign:'center'">資源類型</th>
                        <th data-options="field:'strategysource',width:500,align:'left',halign:'center'">資源位置</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
    <!-- Datagrid Toolbar -->
    <div id="tb" style="height:auto">
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="append()">新增</a>
        <%--<a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="modify2()">修改</a>--%>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="removeit()">刪除</a>
    </div>
    <!-- 衛教資源視窗 -->
    <div id="win-strategy" class="easyui-window" title="衛教資源維護" style="width:700px;height:350px;padding:10px;"
         data-options="modal:true,closed:true,collapsible:false,minimizable:false,maximizable:false">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'center'" style="padding:10px;">
                <div style="margin-bottom:20px">
                    <label>資源名稱：</label><br />
                    <input id="input-resource" class="easyui-combogrid" value="" style="width:650px"
                        data-options="panelWidth: 600,idField: 'strategyid', fitColumns: true,textField: 'strategyname', url: 'ajax/GetStrategyDataList.ashx', method: 'get',
                                columns: [[
                                            {field:'strategyid',hidden:true},
                                            {field:'strategyname',title:'資源名稱',width:150},
                                            {field:'strategytype',title:'資源類型',width:80},
                                            {field:'strategysource',title:'資源位置',width:350}
                                        ]]
                                " />
                </div>
                <input type="hidden" id="hid-uniqueid" />
                <input type="hidden" id="hid-operation" value="A" />
            </div>
            <div data-options="region:'south',border:false" style="text-align:right;padding:5px 0 0;">
                <a class="easyui-linkbutton" data-options="iconCls:'icon-ok'" href="javascript:void(0)" onclick="javascript:insert();" style="width:80px">儲存</a>
                <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" href="javascript:void(0)" onclick="javascript:$('#win-strategy').window('close');" style="width:80px">取消</a>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hidSurveyID" ClientIDMode="Static" runat="server" />
    <script type="text/javascript">
        $(function () {
            $("#list-question").datalist({
                url: "ajax/GetQuestionDataList.ashx?id=" + $("#hidSurveyID").val(),
                valueField: "QuestionID",
                textField: "QTitle",
                line: true
            });
        });

        $('#list-question').datalist({
            onSelect: function (index, row) {
                var id = row.QuestionID;
                $("#list-option").datalist({
                    url: "ajax/GetQuestionOption.ashx?id=" + id,
                    valueField: "SelectValue",
                    textField: "SelectOption",
                    line: true
                });
                $('#list-option').datalist('unselectAll');
                $('#dg').datagrid('reload', {
                    id: -1
                });
            }
        });

        $('#list-option').datalist({
            onSelect: function (index, row) {
                var qrow = $("#list-question").datalist('getSelected');
                var id = qrow.QuestionID;
                var value = row.SelectValue;
                $('#dg').datagrid({
                    url: 'ajax/GetHealthSuggest.ashx',
                    method: 'post',
                    cache: false,
                    queryParams: {
                        id: id,
                        value: value
                    }
                });
            }
        });

        function append() {
            $("#hid-uniqueid").val("");
            $("#input-resource").combogrid('clear');
            $("#hid-operation").val("A");
            $("#win-strategy").window('open');
        }

        function removeit() {
            var row = $('#dg').datagrid('getSelected');
            if (row) {
                var id = row.uniqueid;
                if (window.confirm("您確定要移除這一項衛教資源嗎？")) {
                    $.ajax({
                        url: "ajax/DeleteHealthSuggest.aspx",
                        data: "uid=" + id,
                        type: "POST",
                        dataType: 'text',
                        success: function (msg) {
                            if (msg != "") {
                                alert(msg);
                            }
                            else {
                                var row1=$("#list-question").datalist('getSelected');
                                var row2=$("#list-option").datalist('getSelected')
                                var qid=row1.QuestionID;
                                var value=row2.SelectValue;
                                $("#dg").datagrid('reload', {
                                    id: qid,
                                    value: value
                                });
                            }
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.status);
                            alert(thrownError);
                        }
                    });
                }
            }
            else {
                window.alert("請選擇要移除的衛教資源。");
            }
        }

        function insert() {
            var row1=$("#list-question").datalist('getSelected');
            var row2=$("#list-option").datalist('getSelected')
            var qid=row1.QuestionID;
            var value=row2.SelectValue;
            $.ajax({
                url: "ajax/CreateHealthSuggest.aspx",
                data: "question=" + qid + "&value=" + value +
                        "&strategy=" + $("#input-resource").combogrid('getValue') + "&operation=" + $("#hid-operation").val(),
                type: "POST",
                dataType: 'text',
                success: function (msg) {
                    if (msg != "") {
                        alert(msg);
                    }
                    else {
                        $("#dg").datagrid('reload', {
                            id: qid,
                            value: value
                        });
                        $("#win-strategy").window("close");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    </script>
</asp:Content>
