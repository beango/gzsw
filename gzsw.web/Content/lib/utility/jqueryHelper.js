
/* JQUERY 扩展弹出框 */
(function ($) {


    /* 字符串TrimStart处理 */
    String.prototype.trimStart = function (trimStr) {
        if (!trimStr) { return this; }
        var temp = this;
        while (true) {
            if (temp.substr(0, trimStr.length) != trimStr) {
                break;
            }
            temp = temp.substr(trimStr.length);
        }
        return temp;
    };
    /* 字符串TrimEnd处理 */
    String.prototype.trimEnd = function (trimStr) {
        if (!trimStr) { return this; }
        var temp = this;
        while (true) {
            if (temp.substr(temp.length - trimStr.length, trimStr.length) != trimStr) {
                break;
            }
            temp = temp.substr(0, temp.length - trimStr.length);
        }
        return temp;
    };
    /* 字符串Trim处理 */
    String.prototype.trim = function (trimStr) {
        var temp = trimStr;
        if (!trimStr) { temp = " "; }
        return this.trimStart(temp).trimEnd(temp);
    };


    //分析url
    function parseURL(url) {
        var a = document.createElement('a');
        a.href = url;
        return {
            source: url,
            protocol: a.protocol.replace(':', ''),
            host: a.hostname,
            port: a.port,
            query: a.search,
            params: (function () {
                var ret = {},
                seg = a.search.replace(/^\?/, '').split('&'),
                len = seg.length, i = 0, s;
                for (; i < len; i++) {
                    if (!seg[i]) { continue; }
                    s = seg[i].split('=');
                    ret[s[0]] = s[1];
                }
                return ret;

            })(),
            file: (a.pathname.match(/\/([^\/?#]+)$/i) || [, ''])[1],
            hash: a.hash.replace('#', ''),
            path: a.pathname.replace(/^([^\/])/, '/$1'),
            relative: (a.href.match(/tps?:\/\/[^\/]+(.+)/) || [, ''])[1],
            segments: a.pathname.replace(/^\//, '').split('/')
        };
    }

    //替换myUrl中的同名参数值
    function replaceUrlParams(myUrl, newParams) {
        
        for (var x in newParams) {
            var hasInMyUrlParams = false;
            for (var y in myUrl.params) {
                if (x.toLowerCase() == y.toLowerCase()) {
                    myUrl.params[y] = newParams[x];
                    hasInMyUrlParams = true;
                    break;
                }
            }
            //原来没有的参数则追加
         
            if (!hasInMyUrlParams) {
                myUrl.params[x] = newParams[x];
            }
        }
        var result = myUrl.protocol + "://" + myUrl.host + ":" + myUrl.port + myUrl.path + "?";

        for (var p in myUrl.params) {
            result += (p + "=" + myUrl.params[p] + "&");
        }

        if (result.substr(result.length - 1) == "&") {
            result = result.substr(0, result.length - 1);
        }

        if (myUrl.hash != "") {
            result += "#" + myUrl.hash;
        }
        return result;
    }

    //  判断正整数
    function isNumber(str) {
        var re = /^\d*$/;
        return re.test(str);
    }

    $.fn.dialog = function () {
        $(this).click(function () {
            // 获取地址
            var url = $(this).attr("ref");
            var width = $(this).attr("dialogW");
            var height = $(this).attr("dialogH");
            if (width == undefined || width <= 0) {
                width = 600;
            }
            if (height == undefined || height <= 0) {
                height = 300;
            }

            if (url != undefined && url != "") {
                gzsw.dialog.open({
                    url: url,
                    width: width,
                    height: height
                });
            }
        });
    };
 

    // 获取选中数  
    $.fn.getSelCount = function () {
        return $(this).find("input[type='checkbox']:checked").not("[name='chkAll']").length;
    }; 
    // 获取选中值
    $.fn.getSelValue = function () {
        var selObj = $(this).find("input[type='checkbox']:checked").not("[name='chkAll']");
        if (selObj.length == 1) {
            return selObj.val();
        } else {
            var value = ""; 
            for (var i=0;i<selObj.length;i++) { 
                var val = $(selObj[i]).val(); 
                value = value+ (val + ",");
            } 
            return value.trimEnd(',');
        } 
    };


    //获取选中值里面的属性值
    $.fn.getSelAttr = function(attr) {
        var selObj = $(this).find("input[type='checkbox']:checked").not("[name='chkAll']");
        if (selObj.length == 1) {
            return selObj.attr(attr);
        } else {
            var ary = new Array();
            selObj.each(function(i, item) {
                ary.push($(item).attr(attr));
            });
            return ary.join(",");
        }
    };
    // 初始化全选/取消按钮
    $.fn.selInit = function () {
        $(this).live("click", function () {
            $("input[type='checkbox']").attr("checked", $(this).is(":checked"));
        });
    };

    $.fn.pager = function () {
        var selectObj = $(this).find("select[name='rp']");
        var defaultValue = $(this).find("input[tag='num']").val(); 
        var maxValue = parseInt($(this).find("input[tag='num']").attr("maxvalue"));
        var urlObj = parseURL(location.href);
        //  当前页
        var nowIndex = 1;
        if (urlObj.params["pageIndex"]!=undefined) {
            nowIndex = parseInt(urlObj.params["pageIndex"]);
        }
        // 分页数据绑定 
        var pageSizeObj = urlObj.params["pageSize"];
        if (pageSizeObj != undefined) { 
            selectObj.find("option[value='" + pageSizeObj + "']").attr("selected", true);
        }

        // 刷新按钮
        $("span[tag='refresh']").click(function() {
            location.href = replaceUrlParams(urlObj, {
                _t: Date.parse(new Date())
            });
        });

        // 控制输入框为正整数
        $(this).find("input[tag='num']").keyup(function () {
            var value = $(this).val(); 
            if (!isNumber(value)) { 
                $(this).val(value.substring(0, value.length - 1));
                if ($(this).val() == "") {
                    $(this).val("1");
                }
            } 
            if (value == "") {
                $(this).val("1");
            } else if (parseInt(value) >parseInt(maxValue)) {
                $(this).val(maxValue);
            }
            // 重置URL
            if ($(this).val() != defaultValue) {
                var url = replaceUrlParams(urlObj, {
                    "pageIndex": $(this).val(),
                    _t: Date.parse(new Date())
                });
                location.href = url;
            }
        }).click(function() {
            $(this).select();
        });
        
        // 页面控制
        // 首页
        $("span[tag='firstBtn']").click(function() {
            var url = replaceUrlParams(urlObj, {
                "pageIndex": 1,
                _t: Date.parse(new Date())
            });
            location.href = url;
        });

        // 上一页
        $("span[tag='upBtn']").click(function () {
            if (nowIndex - 1 > 0) {
                var url = replaceUrlParams(urlObj, {
                    "pageIndex": nowIndex - 1,
                    _t: Date.parse(new Date())
                });
                location.href = url;
            }  
        });
        
        // 上一页
        $("span[tag='nextBtn']").click(function () {
            if (nowIndex + 1 <= maxValue) {
                var url = replaceUrlParams(urlObj, {
                    "pageIndex": nowIndex + 1,
                    _t: Date.parse(new Date())
                });
                location.href = url;
            }
        });

        // 尾页
        $("span[tag='endBtn']").click(function () {
            var url = replaceUrlParams(urlObj, {
                "pageIndex": maxValue,
                _t: Date.parse(new Date())
            });
            location.href = url;
        });

        // 处理下拉框事件
        selectObj.change(function () {
            var url = replaceUrlParams(urlObj, {
                "pageSize": $(this).val(),
                _t: Date.parse(new Date())
            }); 
            location.href = url;
        });
    };

})(jQuery)