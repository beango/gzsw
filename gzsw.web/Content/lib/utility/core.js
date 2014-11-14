/************************************************
* 功能说明: Javascript Code 1.0
* 创建时间: 2013-7-24
*   创建人: lhc
*     描述: Javascript 扩展库
/************************************************/
/*------------------------------------------------String 命名空间扩展-----------------------------------------*/
//分析url
function parseURL(url) { 
    var a = document.createElement('a');
    a.href = url;  
    return {
        source: url, 
        host: window.location.host,
        port: window.location.port,
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
   
    var result = "";
    result = "http://" + myUrl.host + myUrl.path + "?";

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

/*替换URL*/
String.prototype.getUrl = function (option) { 
    var obj = parseURL(this);
    return replaceUrlParams(obj, option);
};

/** 
 * 生成完整Url
 * @param data 参数
 **/
String.prototype.toFullUrl = function (data) {
    var that = this;
    if (!data) {
        return that;
    }
    var list = new Array();
    for (var name in data) {
        list.push(name + "=" + escape(data[name]));
    }
    var exstring = that.indexOf("#") >= 0 ? "&" : "?";
    if (list.length > 0) {
        that = that + exstring + list.join("&");
    }
    return that;
}

/*获取GUID*/
String.guid = function() {
    var S4 = function() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
};


/*字符串format*/
String.format = function(fmt) {
    var params = arguments;
    var pattern = /{{|{[1-9][0-9]*}|\x7B0\x7D/g;
    return fmt.replace(
        pattern,
        function(p) {
            if (p == "{{") return "{";
            return params[parseInt(p.substr(1, p.length - 2), 10) + 1]
        }
    );
};

/*判断是否是一个String类型*/
String.prototype.isString = function() {
    return typeof this == "string" || (this && (typeof this.substr == 'function'));
};

/*添加时间戳*/
String.prototype.urlstamp = function() {
    var template = "{0}?_t={1}";
    if (this.indexOf("?") != -1) {
        template = "{0}&_t={1}";
    }
    return String.format(template, this, Date.parse(new Date()));
};

/*添加url参数*/
String.prototype.addurlpara = function(name, value) {
    var template = "{0}?{1}={2}";
    if (this.indexOf("?") != -1) {
        template = "{0}&{1}={2}";
    }
    return String.format(template, this, name, value);
};
  