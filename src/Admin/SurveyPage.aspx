<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="SurveyPage.aspx.cs" Inherits="Admin.SurveyPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- for CKEditor -->
    <!--<script type="text/javascript" src="ckeditor/ckeditor.js"></script>-->
    <!-- for CKEditor -->
    <link type="text/css" rel="stylesheet" href="css/easyui-form.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="page-data-content">
        <asp:FormView ID="FormView1" runat="server" Width="100%">
            <ItemTemplate>
                <div class="form_input_content">
                    <span class="input_header">問卷名稱：</span>
                    <span class="input_content">
                    <asp:Label ID="lblSurveyName" runat="server" Text='<%# Eval("SurveyName")%>' Width="500" ClientIDMode="Static"></asp:Label>
                    </span>
               </div>
                <div class="form_input_content">
                    <span class="input_header">問卷說明：</span>
                    <span class="input_content" style="border: 1px solid #cfcfcf; padding: 3px; display: inline-block;">
                        <asp:TextBox ID="txtInstruction" runat="server" TextMode="MultiLine" ClientIDMode="Static" Rows="10" Width="500px" Height="300px"
                            Text='<%# Eval("Instruction")%>' ReadOnly="true"  ></asp:TextBox>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">問卷備註：</span>
                    <span class="input_content" style="border: 1px solid #cfcfcf; padding: 3px; display: inline-block;">
                        <asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" ClientIDMode="Static" Rows="10" Width="500px"
                            Text='<%# Eval("Remark")%>' ReadOnly="true"  ></asp:TextBox>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">問卷狀態：</span>
                    <span class="input_content">
                    <asp:Label ID="lblClosed" runat="server" Text='<%# Eval("ClosedName")%>' Width="500" ClientIDMode="Static"></asp:Label>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">報告類型：</span>
                    <span class="input_content">
                    <asp:Label ID="lblPageType" runat="server" Text='<%# Eval("PageType")%>' Width="500" ClientIDMode="Static"></asp:Label>
                    </span>
                </div>
                <div class="form_input_content" style="text-align: center;">
                    <asp:Button ID="btnInsert" CommandName="Edit" runat="server" Text="編輯" CssClass="form_action_button" OnClientClick="javascript: return submitForm();" />
                    <input id="btnBack" type="button" value="取消" onclick="javascript: window.location = 'CaseList.aspx'; return false;" />
                </div>
            </ItemTemplate>
            <InsertItemTemplate>
                <div class="form_input_content">
                    <span class="input_header">問卷名稱：</span>
                    <span class="input_content">
                    <asp:TextBox ID="txtSurveyName" runat="server" Text='' Width="500" ClientIDMode="Static" CssClass="easyui-textbox" 
                        data-options="required:true,missingMessage:'問卷名稱必填'"></asp:TextBox>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">問卷說明：</span>
                    <span class="input_content">
                    <asp:TextBox ID="txtInstruction" runat="server" Text='' Width="800" ClientIDMode="Static" CssClass="easyui-textbox" 
                        data-options="required:true,missingMessage:'問卷說明必填',height:150,multiline:true"></asp:TextBox>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">問卷備註：</span>
                    <span class="input_content">
                    <asp:TextBox ID="txtRemark" Text='' runat="server" ClientIDMode="Static" Width="800px"  CssClass="easyui-textbox" 
                        data-options="height:150,multiline:true"></asp:TextBox>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">問卷狀態：</span>
                    <span class="input_content">
                    <asp:Textbox ID="txtClosed" runat="server" Text='' Width="80" class="easyui-combobox" ClientIDMode="Static"
                         data-options="
                            panelHeight: 70,
		                    valueField: 'name',
		                    textField: 'code',
		                    data: [{
			                    code: '開放',
			                    name: 'N'
		                    },{
			                    code: '關閉',
			                    name: 'Y'
		                    }]" >
                    </asp:Textbox>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">報告類型：</span>
                    <span class="input_content">
                    <asp:TextBox ID="txtPageID" Text='' runat="server" Width="300" ClientIDMode="Static" CssClass="easyui-combobox" 
                        data-options="url:'ajax/GetPageList.ashx',valueField:'PageID',textField:'PageType',required:true,missingMessage:'報告類型必填',editable:true"></asp:TextBox>
                    </span>
                </div>
                <div class="form_input_content" style="text-align: center;">
                    <asp:Button ID="btnInsert" CommandName="Insert" runat="server" Text="新增" CssClass="form_action_button" OnClientClick="javascript: return submitForm();" />
                    <input id="btnBack" type="button" value="取消" onclick="javascript: window.location = 'SurveyList.aspx'; return false;" />
                </div>
            </InsertItemTemplate>
            <EditItemTemplate>
                <div class="form_input_content">
                    <span class="input_header">問卷名稱：</span>
                    <span class="input_content">
                    <asp:TextBox ID="txtSurveyName" runat="server" Text='<%# Eval("SurveyName")%>' Width="500" ClientIDMode="Static" CssClass="easyui-textbox" 
                        data-options="required:true,missingMessage:'問卷名稱必填'"></asp:TextBox>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">問卷說明：</span>
                    <span class="input_content">
                    <asp:TextBox ID="txtInstruction" runat="server" Text='<%# Eval("Instruction")%>' Width="800" ClientIDMode="Static" CssClass="easyui-textbox" 
                        data-options="required:true,missingMessage:'問卷說明必填',height:150,multiline:true"></asp:TextBox>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">問卷備註：</span>
                    <span class="input_content">
                    <asp:TextBox ID="txtRemark" Text='<%# Eval("Remark")%>' runat="server" ClientIDMode="Static" Width="800px"  CssClass="easyui-textbox" 
                        data-options="height:150,multiline:true"></asp:TextBox>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">問卷狀態：</span>
                    <span class="input_content">
                    <asp:Textbox ID="txtClosed" runat="server" Text='<%# Eval("Closed")%>' Width="80" class="easyui-combobox" ClientIDMode="Static"
                         data-options="
                            panelHeight: 70,
		                    valueField: 'name',
		                    textField: 'code',
		                    data: [{
			                    code: '開放',
			                    name: 'N'
		                    },{
			                    code: '關閉',
			                    name: 'Y'
		                    }]" >
                    </asp:Textbox>
                    </span>
                </div>
                <div class="form_input_content">
                    <span class="input_header">報告類型：</span>
                    <span class="input_content">
                    <asp:TextBox ID="txtPageID" Text='<%# Eval("PageID")%>' runat="server" Width="300" ClientIDMode="Static" CssClass="easyui-combobox" 
                        data-options="url:'ajax/GetPageList.ashx',valueField:'PageID',textField:'PageType',required:true,missingMessage:'報告類型必填',editable:true"></asp:TextBox>
                    </span>
                </div>
                <div class="form_input_content" style="text-align: center;">
                    <asp:Button ID="btnInsert" CommandName="Update" runat="server" Text="更新" CssClass="form_action_button" OnClientClick="javascript: return submitForm();" />
                    <input id="btnBack" type="button" value="取消" onclick="javascript: window.location = 'SurveyList.aspx'; return false;" />
                </div>
            </EditItemTemplate>
        </asp:FormView>
        <asp:HiddenField ID="hidSurveyID" ClientIDMode="Static" Value='' runat="server" />
    </div>
    <script type="text/javascript">
        //if (document.forms[0].txtDescription) {
        //    CKEDITOR.replace('txtDescription',
        //        {
        //            filebrowserImageUploadUrl: 'ajax/UploadImage.ashx'
        //        });
        //    CKEDITOR.config = {
        //        width: '850',
        //        height: '300'
        //    };
        //};

    </script>
</asp:Content>
