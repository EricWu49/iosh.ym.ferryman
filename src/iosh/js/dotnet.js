// 解決Repeater中的Radio Button無法視為同一個群組的問題，需搭配Repeater的ItemDatBound事件使用
function setExclusiveRadioButton(name, current) {
    regex = new RegExp(name);

    for (i = 0; i < document.forms[0].elements.length; i++) {
        var elem = document.forms[0].elements[i];
        if (elem.type == 'radio') {
            elem.checked = false;
        }
    }
    current.checked = true;
}