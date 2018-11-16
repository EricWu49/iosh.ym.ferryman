function myformatter(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
}
function myparser(s) {
    if (!s) return new Date();
    var ss = (s.split('-'));
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    var d = parseInt(ss[2], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return new Date(y, m - 1, d);
    } else {
        return new Date();
    }
}

// 取得第一筆選取資料
function getSelected() {

    var row = $('#dg').datagrid('getSelected');
    if (row) {
        return row.datakey;
    }
}

// 取得所有選取的資料
function getSelections() {

    var ss = [];
    var rows = $('#dg').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        ss.push(row.datakey);
    }
    return ss.join(';');
}

function submitForm() {
    return $('#form1').form('validate');
}

function clearForm() {
    $('#form1').form('clear');
}
