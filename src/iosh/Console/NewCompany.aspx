<%@ Page Title="" Language="C#" MasterPageFile="~/Console/Console.Master" AutoEventWireup="true" CodeBehind="NewCompany.aspx.cs" Inherits="iosh.Console.NewCompany" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../js/form2js.js"></script>
    <script src="../js/js2form.js"></script>
    <script src="../js/json2.js"></script>
    <script src="../js/jQuery.BlockUI.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="p" class="easyui-panel" title="新公司註冊" style="width:600px;height:500px;padding:20px;">
            <div style="margin-bottom:20px">
                <input class="easyui-textbox" id="txtCompany" name="company_companyname" prompt="請輸入公司完整名稱" style="width:100%" data-options="label:'公司名稱 : ',required:true" />
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-textbox" id="txtTaxID" name="company_taxid" prompt="請輸入公司統一編號" style="width:100%" data-options="label:'公司統編 : ',required:true" />
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-textbox" id="txtUserName" name="useraccount_username" prompt="請輸入您的姓名" style="width:100%" data-options="label:'姓名 : ',required:true" />
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-textbox" id="txtEmail" name="useraccount_email" prompt="請輸入您的Email address，" style="width:100%" data-options="label:'Email:',required:true,validType:'email'" />
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-textbox" id="txtAccount" name="useraccount_useraccount" prompt="請設定登入系統的帳號" style="width:100%" data-options="label:'申請帳號 : ',required:true" />
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-passwordbox" id="txtPassword" name="useraccount_userpass" prompt="請設定登入密碼" style="width:100%" data-options="label:'密碼設定 : ',required:true" />
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-passwordbox" id="txtConfirm" name="dummy_userpass" prompt="請再輸入一次登入密碼" style="width:100%" validType="confirmPass['#txtPassword']" data-options="label:'密碼確認 : ',required:true" />
            </div>
            <div style="text-align:center;padding:5px 0">
                <a href="javascript:void(0)" class="easyui-linkbutton" onclick="formSubmit()" style="width:80px">確認申請</a>
                <a href="javascript:void(0)" class="easyui-linkbutton" onclick="clearForm()" style="width:80px">清除重填</a>
            </div>
    </div>
    <script type="text/javascript">
        // unblock when ajax activity stops 
        $(document).ajaxStop($.unblockUI);

        $.extend($.fn.validatebox.defaults.rules, {
            confirmPass: {
                validator: function(value, param){
                    var pass = $(param[0]).passwordbox('getValue');
                    return value == pass;
                },
                message: 'Password does not match confirmation.'
            }
        })

        function formSubmit() {
            if (submitForm()) {
                if (isValidGUI($("#txtTaxID").textbox("getValue"))) {
                    $.blockUI({ message: '<h3><img src="../images/loading.gif" /> 資料處理中，請稍候...</h3>' });

                    var formData = form2js('form1', '_', true,
                      function (node) {
                          if (node.id && node.id.match(/callbackTest/)) {
                              return { name: node.id, value: node.innerHTML };
                          }
                      });
                    var jsObject = JSON.stringify(formData, null, '\t');
                    window.alert(jsObject);
                    $.ajax({
                        type: "POST",
                        url: "ajax/CompanySave.ashx",
                        data: { "action":"insert", "formdata": jsObject },
                        contentType: "application/json; charset=utf-8",
                        datatype: "json",
                        cache: false,
                        success: function (result) {
                            if (result != "ERROR") {
                                return true;
                            } else {
                                window.alert(result);
                                return false;
                            }
                        },
                        error: function () {
                            alert('Error!');
                            return false;
                        }
                    });
                }
                else {
                    window.alert("統編錯誤。");
                }
            }
            else {
                window.alert("請修正錯誤的資料。");
            }
        }

        // 檢查公司統編
        function isValidGUI(taxId) {
            var invalidList = "00000000,11111111";
            if (/^\d{8}$/.test(taxId) == false || invalidList.indexOf(taxId) != -1) {
                return false;
            }

            var validateOperator = [1, 2, 1, 2, 1, 2, 4, 1],
                sum = 0,
                calculate = function (product) { // 個位數 + 十位數
                    var ones = product % 10,
                        tens = (product - ones) / 10;
                    return ones + tens;
                };
            for (var i = 0; i < validateOperator.length; i++) {
                sum += calculate(taxId[i] * validateOperator[i]);
            }

            return sum % 10 == 0 || (taxId[6] == "7" && (sum + 1) % 10 == 0);
        }
    </script>
</asp:Content>
