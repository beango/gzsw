/* 初始化Ajax异步提交表单 */
function InitAjaxForm() {
    var options = {
        dataType: 'json',
        success: function (data) {
            if (!data.result)
            {
                var validmsg = data.validmsg;
                for (var i = 0; i < validmsg.length; i++) {
                    $(".field-validation-valid[data-valmsg-for='" + validmsg[i].Key + "']")
                        .html(validmsg[i].Value);
                    $(".field-validation-valid[data-valmsg-for='" + validmsg[i].Key + "']").attr("class", "field-validation-error");
                    if (validmsg[i].Key == "") {
                        layer.alert(validmsg[i].Value,1
                        , function (index) {
                            //parent.layer.closeAll(); 
                            layer.close(index);
                        });
                    }
                }
            }
            else {
                //alert(data.reload);
                layer.alert(data.desc, 2
                        , function (index) {
                            layer.close(index);
                            parent.layer.closeAll();
                        });
                if (data.reload == true || data.reload=='true')
                    parent.location.href = data.url;
                else {
                    //parent.layer.closeAll();
                    //layer.close();
                }
            }
        }
    };

    //bind to the form's submit event
    $('form').submit(function () {
        try {
            var evt = evt || window.event;
            var isValid = $('.formTable :input[data-val="true"]').valid();
            if (!isValid) {
                evt.preventDefault();
                return false;
            }
        } catch (e) {

        }
        $(".field-validation-valid").html("");
        $(".field-validation-error").html("");
        $(".field-validation-error").attr("class","field-validation-valid");
        $(this).ajaxSubmit(options);
        return false;
    });
}

/* ztree树 */
var ztree_setting_checkbox_YN2 = {
    check: {
        enable: true,
        nocheckInherit: false,
        chkboxType: { "Y": "ps", "N": "ps" }
    },
    async: {
        enable: false
    },
    data: {
        simpleData: {
            enable: true,
            idKey: "id",
            pIdKey: "pId",
            rootPId: 0
        }
    },
    view: {
        showIcon: showIconForTree
    }
};
var ztree_setting_radio = {
    check: {
        enable: true,
        chkStyle: "radio",
        radioType: "all"
    },
    async: {
        enable: false,
        autoParam: ["id", "name"]//异步加载时需要提交的参数，多个用逗号分隔
    },
    data: {
        simpleData: {
            enable: true,
            idKey: "id",
            pIdKey: "pId",
            rootPId: 0
        }
    },
    view: {
        showIcon: showIconForTree
    }
};
var ztree_setting = {
    async: {
        enable: false
    },
    data: {
        simpleData: {
            enable: true,
            idKey: "id",
            pIdKey: "pId",
            rootPId: 0
        }
    },
    view: {
        showIcon: showIconForTree
    }
};
function showIconForTree(treeId, treeNode) {
    return !treeNode;
};
function Tree(obj, zNodes) {
    $.fn.zTree.init($(obj), ztree_setting, zNodes);
}
function checkboxTreeYN2(obj, zNodes) {
    $.fn.zTree.init($(obj), ztree_setting_checkbox_YN2, zNodes);
}
function radioTree(obj, zNodes) {
    $.fn.zTree.init($(obj), ztree_setting_radio, zNodes);
}
/* ztree树 */