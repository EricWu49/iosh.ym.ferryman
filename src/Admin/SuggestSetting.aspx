<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="SuggestSetting.aspx.cs" Inherits="Admin.SuggestSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="layout-content" class="easyui-layout" style="width:100%; height:500px">
        <div data-options="region:'west',title:'1.診斷構面',split:false,collapsible:false" style="width:250px;height:200px">
            <div id="list-dimension"></div>
        </div>
        <div data-options="region:'center',title:'2.判斷條件'" style="padding:0px;background:#eee;">
            <table id="dg" class="easyui-datagrid" style="width:95%;height:180px;margin:0px;" 
                data-options="singleSelect:true,collapsible:true,toolbar:'#tb',nowrap:false,striped:true,
                                url: 'ajax/GetSituationRule.ashx'">
                <thead>
                    <tr>
                        <th data-options="field:'ruleid',hidden:true"></th>
                        <th data-options="field:'situationid',hidden:true"></th>
                        <th data-options="field:'minvalue',width:80,align:'center',halign:'center'">最小值</th>
                        <th data-options="field:'maxvalue',width:80,align:'center',halign:'center'">最大值</th>
                        <th data-options="field:'situationname',width:150,align:'left',halign:'center'">結果描述</th>
                        <th data-options="field:'suggest',width:400,align:'left',halign:'center'">建議</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'south',title:'3.衛教資源',split:false,collapsible:false" style="height:300px;">
            <table id="dg1" class="easyui-datagrid" style="width:95%;height:280px;margin:0px;" 
                data-options="singleSelect:true,collapsible:true,toolbar:'#tb2',nowrap:false,striped:true,
                                url: 'ajax/GetStrategy.ashx'">
                <thead>
                    <tr>
                        <th data-options="field:'situationid',hidden:true"></th>
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
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="modify()">修改</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="removeit()">刪除</a>
    </div>
    <div id="tb2" style="height:auto">
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="append2()">新增</a>
        <%--<a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="modify2()">修改</a>--%>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="removeit2()">刪除</a>
    </div>
    <!-- 判斷條件維護視窗 -->
    <div id="win-rule" class="easyui-window" title="判斷條件維護" style="width:700px;height:350px;padding:10px;"
         data-options="modal:true,closed:true,collapsible:false,minimizable:false,maximizable:false">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'center'" style="padding:10px;">
                <div style="margin-bottom:20px">
                    <label>診斷構面：</label>
                    <span id="win-dimension"></span>
                </div>
                <div style="margin-bottom:20px">
                    <label>最小值：</label>
                    <input id="win-minvalue" class="easyui-numberbox" data-options="precision:2" value="" style="width:80px;height:26px" />
                </div>
                <div style="margin-bottom:20px">
                    <label>最大值：</label>
                    <input id="win-maxvalue" class="easyui-numberbox" data-options="precision:2" value="" style="width:80px;height:26px" />
                </div>
                <div style="margin-bottom:20px">
                    <label>結果描述：</label>
                    <input id="win-situation" class="easyui-combobox" style="width:400px;height:26px" 
                        data-options="url: 'ajax/GetSituationList.ashx', valueField: 'situationid', textField: 'situationname'"/>
                </div>
                <div style="margin-bottom:20px">
                    <label>建議：</label>
                    <span id="win-suggest"></span>
                </div>
                <input type="hidden" id="win-uid" />
                </div>
            <div data-options="region:'south',border:false" style="text-align:right;padding:5px 0 0;">
                <a class="easyui-linkbutton" data-options="iconCls:'icon-ok'" href="javascript:void(0)" onclick="javascript:insert();" style="width:80px">儲存</a>
                <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" href="javascript:void(0)" onclick="javascript:$('#win-rule').window('close');" style="width:80px">取消</a>
            </div>
        </div>
    </div>
    <!-- 衛教資源視窗 -->
    <div id="win-strategy" class="easyui-window" title="衛教資源維護" style="width:700px;height:350px;padding:10px;"
         data-options="modal:true,closed:true,collapsible:false,minimizable:false,maximizable:false">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'center'" style="padding:10px;">
                <div style="margin-bottom:20px">
                    <label>結果描述：</label>
                    <span id="span-situation"></span>
                </div>
                <div style="margin-bottom:20px">
                    <label>資源名稱：</label><br />
                    <input id="input-resource" class="easyui-combogrid" value="" style="width:650px"
                        data-options="panelWidth: 600,idField: 'strategyid',textField: 'strategyname', url: 'ajax/GetStrategyDataList.ashx', method: 'get',
                                columns: [[
                                            {field:'strategyid',hidden:true},
                                            {field:'strategyname',title:'資源名稱',width:150},
                                            {field:'strategytype',title:'資源類型',width:80},
                                            {field:'strategysource',title:'資源位置',width:350}
                                        ]],
                                fitColumns: true" />
                </div>
                <input type="hidden" id="hid-situationid" />
                <input type="hidden" id="hid-operation" value="A" />
            </div>
            <div data-options="region:'south',border:false" style="text-align:right;padding:5px 0 0;">
                <a class="easyui-linkbutton" data-options="iconCls:'icon-ok'" href="javascript:void(0)" onclick="javascript:insert2();" style="width:80px">儲存</a>
                <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" href="javascript:void(0)" onclick="javascript:$('#win-strategy').window('close');" style="width:80px">取消</a>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hidSurveyID" ClientIDMode="Static" runat="server" />
    <script type="text/javascript">
        $(function () {
            $("#list-dimension").datalist({
                url: "ajax/GetSurveyDimension.ashx?id=" + $("#hidSurveyID").val(),
                valueField: "DimensionID",
                textField: "DimensionName",
                line: true
            });
        });

        $('#list-dimension').datalist({
            onSelect: function (index, row) {
                var id = row.DimensionID;
                $('#dg').datagrid('load', {
                    survey: $("#hidSurveyID").val(),
                    dimension: id
                });
                $('#dg').datagrid('unselectAll');
                $('#dg1').datagrid('load', {
                    id: -1
                });
            }
        });

        $('#dg').datagrid({
            onSelect: function (index, row) {
                var id = row.situationid;
                $('#dg1').datagrid('load', {
                    id: id
                });
            }
        })

        function getSelected() {
            var row = $('#dg').datagrid('getSelected');
            if (row) {
                return row.ruleid;
            }
        }

        function append() {
            $("#win-uid").val("0");
            $("#win-dimension").html($('#list-dimension').datalist('getSelected').DimensionName);
            $("#win-minvalue").numberbox('setValue', "0");
            $("#win-maxvalue").numberbox('setValue', "0");
            $("#win-situation").combobox('clear');
            $("#win-suggest").html("");
            $("#win-rule").window('open');
        }

        function removeit() {
            var row = $('#dg').datagrid('getSelected');
            if (row) {
                var id = row.ruleid;
                if (window.confirm("您確定要刪除這一筆資料嗎？")) {
                    $.ajax({
                        url: "ajax/DeleteSituationRule.aspx",
                        data: "id=" + id,
                        type: "POST",
                        dataType: 'text',
                        success: function (msg) {
                            if (msg != "") {
                                alert(msg);
                            }
                            else {
                                $("#dg").datagrid('reload');
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
                window.aler("請先選擇要刪除的資料。");
            }
        }

        function modify() {
            var row = $("#dg").datagrid('getSelected');
            $("#win-dimension").html($('#list-dimension').datalist('getSelected').DimensionName);
            $("#win-uid").val(row.ruleid);
            $("#win-minvalue").numberbox('setValue', row.minvalue);
            $("#win-maxvalue").numberbox('setValue', row.maxvalue);
            $("#win-situation").combobox('setValue', row.situationid);
            $("#win-suggest").html(row.suggest);
            $("#win-rule").window('open');
        }

        function insert() {
            $.ajax({
                url: "ajax/CreateSituationRule.aspx",
                data: "id=" + $("#win-uid").val() + "survey=" + $("#hidSurveyID").val() + "&dimension=" + $('#list-dimension').datalist('getSelected').DimensionID +
                    "&min=" + $("#win-minvalue").numberbox('getValue') + "&max=" + $("#win-maxvalue").numberbox('getValue') +
                    "&situation=" + $("#win-situation").combobox('getValue'),
                type: "POST",
                dataType: 'text',
                success: function (msg) {
                    if (msg != "") {
                        alert(msg);
                    }
                    else {
                        $("#dg").datagrid('reload');
                        $("#win-rule").window("close");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }

        function getSelected2() {
            var row = $('#dg1').datagrid('getSelected');
            if (row) {
                return row.strategyid;
            }
        }

        function append2() {
            $("#hid-situationid").val($('#dg').datagrid('getSelected').situationid);
            $("#span-situation").html($('#dg').datagrid('getSelected').situationname);
            $("#input-resource").combogrid('clear');
            $("#hid-operation").val("A");
            $("#win-strategy").window('open');
        }

        function removeit2() {
            var row = $('#dg1').datagrid('getSelected');
            if (row) {
                var id1 = row.situationid;
                var id2 = row.strategyid;
                if (window.confirm("您確定要移除這一項衛教資源嗎？")) {
                    $.ajax({
                        url: "ajax/DeleteResourceDB.aspx",
                        data: "situation=" + id1 + "&strategy=" + id2,
                        type: "POST",
                        dataType: 'text',
                        success: function (msg) {
                            if (msg != "") {
                                alert(msg);
                            }
                            else {
                                $("#dg1").datagrid('reload');
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

        //function modify2() {
        //    var row = $('#dg1').datagrid('getSelected');
        //    if (row) {
        //        $("#hid-situationid").val(row.situationid);
        //        $("#span-situation").html($('#dg').datagrid('getSelected').situationname);
        //        $("#input-resource").combogrid('setValue', row.strategyid);
        //        $("#hid-operation").val("U");
        //        $("#win-strategy").window('open');
        //    }
        //    else {
        //        window.alert("請選擇要修改的衛教資源。");
        //    }
        //}

        function insert2() {
            $.ajax({
                url: "ajax/CreateResourceDB.aspx",
                data: "situation=" + $("#hid-situationid").val() + "&strategy=" + $("#input-resource").combogrid('getValue') + "&operation=" + $("#hid-operation").val(),
                type: "POST",
                dataType: 'text',
                success: function (msg) {
                    if (msg != "") {
                        alert(msg);
                    }
                    else {
                        $("#dg1").datagrid('reload');
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
