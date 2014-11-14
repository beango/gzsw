/************************************************
* 功能说明: JQuery dialog plug-in 1.0
* 创建时间: 2014-10-12
*   创建人: hcli 
/************************************************/
if (typeof gzsw == "undefined") {
    gzsw = new Object();
}
gzsw.dialog = function () {

    function CreateOpenDivDialog() {
        
        var sttings = {
            //要弹出的Div
            Div: null,
            //标题
            title: "对话框",
            // 左面板ID
            width: 600,
            // 内容面板ID
            height: 300,
            // 关闭事件
            closeFun: null,
            // 显示
            showType: "",
            // 位置
            offset: null
        };
        return sttings;
    }

    function CreateOpenHtmlDialog() {
        var settings = {
            // 页面地址
            html: "",
            // 标题
            title: "对话框",
            // 左面板ID
            width: 600,
            // 内容面板ID
            height: 300, 
            // 关闭事件
            closeFun: null,
            // 显示
            showType: "",
            // 位置
            offset:  null
        };
        return settings;
    }

    // 创建Open窗体
    function CreateOpenDialog() {
        var settings = {
            // 页面地址
            url: null,
            // 标题
            title: "对话框",
            // 左面板ID
            width: 600,
            // 内容面板ID
            height: 300,
            // 关闭是否刷新页面
            isReload: true,
            // 关闭事件
            closeFun: null,
        };
        return settings;
    }

    // 创建询问窗体
    function CreateConfirmDialog() {
        var settings = { 
            // 标题
            title: "对话框",
            // 提示消息
            msg: "",
            // 左面板ID
            width: 200,
            // 内容面板ID
            height: 130,
            // 关闭是否刷新页面
            isReload: true,
            // 关闭事件
            closeFun: null,
            // 确定事件
            yesFun: null,
            // 取消事件
            noFun:null
        };
        return settings;
    }

    return {
        alert: function(msg, imgType, fn) {
            if (imgType == undefined || imgType < 0) {
                imgType = 0;
            }
            //if (top != self) {
            //    top.gzsw.dialog.alert(msg, imgType, fn);
            //    return false;
            //}
            /*layer.border = [3, 0.3, '#BED5F3'];
            layer.alert(msg, imgType, function(index) {
                layer.close(index);
                fn && fn(index);
            });*/

            $.layer({
                area: ['auto', 'auto'],
                title: "对话框",
                border: [1, 0.3, '#BED5F3'],
                dialog: {
                    msg: msg,
                    btns: 1,
                    type: imgType,
                    btn: ['确定'],
                    yes: function(index) {
                        layer.close(index);

                        if (typeof fn == 'function') {
                            fn(index);
                        }
                    }
                }
            });
        },
        confirm: function(options) {
            //if (top != self) {
            //    top.gzsw.dialog.confirm(options);
            //    return false;
            //}
            var $$Settings = CreateConfirmDialog();
            if (options) {
                $.extend($$Settings, options);
            }

            $.layer({
                area: ['auto', 'auto'],
                title: $$Settings.title,
                border: [3, 0.3, '#BED5F3'],
                area: [$$Settings.width, $$Settings.height],
                dialog: {
                    msg: $$Settings.msg,
                    btns: 2,
                    type: 4,
                    btn: ['确定', '取消'],
                    yes: function(index) {
                        if ($.isFunction($$Settings.yesFun)) {
                            $$Settings.yesFun(index);
                        }
                    },
                    no: function(index) {
                        if ($.isFunction($$Settings.noFun)) {
                            $$Settings.noFun(index);
                        }
                        layer.close(index);
                    }
                }
            });
        },
        openDiv: function(options) {
            var $$Settings = CreateOpenDivDialog();
            if (options) {
                $.extend($$Settings, options);
            }
            var layerObj = $.layer({
                type: 1, //0-4的选择,
                title: $$Settings.title,
                border: [2, 0.3, '#000'],
                shade: [0],
                shadeClose: false,
                maxmin: false,
                area: [$$Settings.width, $$Settings.height],
                page: {
                    dom: $$Settings.Div
                }
            });
            return layerObj;
        },
        openHtml: function(options) {
            var $$Settings = CreateOpenHtmlDialog();
            if (options) {
                $.extend($$Settings, options);
            }
            var layerObj = $.layer({
                type: 1, //0-4的选择,
                title: $$Settings.title,
                border: [3, 0.3, '#BED5F3'],
                shade: [0],
                shadeClose: false,
                maxmin: false,
                area: [$$Settings.width, $$Settings.height],
                page: {
                    html: $$Settings.html
                },
                shift: $$Settings.showType
                /*, offset: $$Settings.offset,*/
/*                success: function (othis) {
                    layer.setTop(othis);
                }*/
            });
            return layerObj;
        },
        open: function(options) {

            //if (top != self) {
            //    top.gzsw.dialog.open(options);
            //    return false;
            //}
            var $$Settings = CreateOpenDialog();
            if (options) {
                $.extend($$Settings, options);
            }

            $.layer({
                type: 2,
                title: $$Settings.title,
                shadeClose: true,
                maxmin: false,
                fix: false,
                border: [3, 0.3, '#BED5F3'],
                area: [$$Settings.width, $$Settings.height],
                iframe: {
                    src: $$Settings.url
                },
                end: function() {
                    if ($.isFunction($$Settings.closeFun)) {
                        $$Settings.closeFun();
                    }
                    if ($$Settings.isReload) {
                        location.reload();
                    }
                },
                close: function(index) {
                    $$Settings.isReload = false;
                }
            });
        },
        //顶级父类中打开
        openFull: function(options) {
            if (top != self) {
                top.gzsw.dialog.open(options);
                return false;
            }
            gzsw.dialog.open(options);
        }
    };
}();
