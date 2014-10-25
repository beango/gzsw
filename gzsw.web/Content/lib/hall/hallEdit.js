
var util = function () {
    //包装参数
    var packagData = function (params) {
        if (null == params) {
            params = {};
        }
        params.timestamp = (new Date()).getTime();

        return params;
    }

    return {
        packagData: function(params) {
            return packagData(params);
        }
    };
}();

var virtualConfig = function () {

    var _data;
    var $Panel;
    var _removeData=new Array();//用于保存所有给删除的数据
    var dialogIndex;

    var _init = function () {

        $Panel = $("#"+_data.webHallId);

        _setHall(_data.image);
        _saveData();
        _setDrag();
        _setConfig();
    }

    var _setHall = function (imgUrl) {
        if (imgUrl) {
            _data.image = imgUrl;
            var $hall = $("#hall_" + _data.hallNo);
            if ($hall.length > 0) {
                $hall.attr("src", imgUrl);
            } else {
                $hall = $("<img class='web-hall-bg' id='hall_" + _data.hallNo + "' src='" + imgUrl + "' ondragstart='return false;' />");
                $Panel.append($hall);
            }
        }
    }

    var _setDrag = function () {
        var $newUser;

        //选中要添加的设备
        $("div.setting span[d-event='event']").on("click", function () {
            if ($(this).hasClass('sel')) {
                $(this).removeClass("sel");
            } else {
                $("div.setting span").removeClass("sel");
                $(this).addClass("sel");
            }
            if ($newUser) {
                $newUser.remove();
                $newUser = null;
            }
            return false;
        });
        $(document).on("click", function () {
            $("div.setting span").removeClass("sel");
        });

        //鼠标移进去生成图标
        $Panel.mousemove(function (e) {
            var objs = $("div.setting span[d-event='event'].sel");
            if (objs.length > 0) {
                var _parentOffset = $(this).offset();

                var dType = objs.attr("d-type");
                var type = 0;
                if (dType == 'tab') {
                    type = 1;
                }

                var item = {
                    Id: 0,
                    HallNo: _data.hallNo,
                    X: (e.pageX - _parentOffset.left) + "px",
                    Y: (e.pageY - _parentOffset.top) + "px",
                    DirType: 1,
                    Type: type,
                    IconUrl: objs.find("img").attr("src"),
                    CameraType: objs.attr("d-cameratype"),
                    Iport:80
                };

                if (type == 1) {
                    if (!$newUser) {

                        var maxId = 0;
                        var items = _getTabDatas();
                        $.each(items, function (i, d) {
                            if (d.Id > maxId) {
                                maxId = Number(d.Id);
                            }
                        });
                        item.Id = Number(maxId) + 1;

                        $newUser = $('<span d-type="device" id="tab_' + item.Id + '"></span>');
                        $newUser.append('<dt class="hallNum">' + item.Id + '</dt>');
                        var $img = $('<img class="imgSize" />');
                        $img.attr("src", item.IconUrl);
                        $newUser.append($img);
                        $newUser.addClass("hall-icon");
                        $newUser.css({ left: item.X, top: item.Y });
                        $newUser.data("data", item);
                        $Panel.append($newUser);
                    } else {
                        item = $newUser.data("data");
                        item.X = (e.pageX - _parentOffset.left) + "px";
                        item.Y = (e.pageY - _parentOffset.top) + "px";
                        $newUser.css({ left: item.X, top: item.Y });
                        $newUser.data("data", item);
                    }
                } else {

                    if (!$newUser) {
                        $newUser = $('<em d-type="device" id="device_' + item.Id + '"></em>');
                        var $img = $('<img class="imgSize" />');
                        $img.attr("src", item.IconUrl);
                        $newUser.append($img);
                        $newUser.attr("d-cameraType", item.CameraType);
                        $newUser.addClass("hall-icon");
                        $newUser.css({ left: item.X, top: item.Y });
                        $newUser.attr("index", 1);
                        $newUser.data("data", item);
                        $Panel.append($newUser);
                    } else {
                        $newUser.css({ left: item.X, top: item.Y });
                        $newUser.data("data", item);
                    }
                }
            }
        });

        //点击面板，让设备放在面板上
        $Panel.click(function (e) {
            if ($newUser) {
                
                $("div.setting span[d-event='event'].sel").removeClass("sel");

                var item = $newUser.data("data");
                if (item.Type == 1) {
                    _setTabAttribute($newUser);
                } else {
                    //摄像的属性设置
                    _setCameraAttribute($newUser);
                }

                $newUser = null;
                return false;
            }
        });
        $Panel.mousedown(function() {
            $Panel.find(".sel").removeClass("sel");
        });
        $(document).click(function (e) {
            if ($newUser) {
                $newUser.remove();
                $newUser = null;
                $("div.setting span[d-event='event'].sel").removeClass("sel");
            }
        });
        $(document).mousedown(function(e) {
            $Panel.find(".sel").removeClass("sel");
        });
        $(document).mousemove(function (e) {
            if (!!this.move) {
                var posix = !document.move_target ? { 'x': 0, 'y': 0 } : document.move_target.posix,
                    callback = function() {
                        $(this.move_target).css({
                            'top': e.pageY - posix.y,
                            'left': e.pageX - posix.x
                        });
                        document.documentElement.style.cursor = 'move';
                    };

                callback.call(this, e, posix);


            }

        }).mouseup(function(e) {
            if (!!this.move) {
                var posix = !document.move_target ? { 'x': 0, 'y': 0 } : document.move_target.posix,
                    callback = function() {
                        document.documentElement.style.cursor = 'move';
                        //这里是重新设置更新data里面的数据
                        document.documentElement.style.cursor = '';

                        var that = $(this.move_target);
                        var data = that.data("data");
                        data.X = e.pageX - posix.x + 'px';
                        data.Y = e.pageY - posix.y + 'px';
                        that.data("data", data);

                    };
                callback.call(this, e, 2);
                $.extend(this, {
                    'move': false,
                    'move_target': null,
                    'call_down': false,
                    'call_up': false
                });
            }

        }).dblclick(function(e) {
            if (!!this.dblclick) {
            }
        }).keydown(function (e) {
            if (!!this.dblclick) {
                callback = function () {
                    var $obj = $(this.dblclick_target);
                    var n = parseInt($obj.attr('index'));
                    var data = $obj.data("data");

                    //左右键
                    if (e.keyCode == 40) {
                        //下
                        mobileDevice($obj, 'down');
                    }
                    if (e.keyCode == 37) {
                        //左
                        mobileDevice($obj, 'left');
                    } else if (e.keyCode == 38) {
                        //上
                        mobileDevice($obj, 'up');
                    } else if (e.keyCode == 39) {
                        //右
                        mobileDevice($obj, 'right');
                    } else if (e.keyCode == 46) {
                        //删除键
                        if ($obj.attr("d-state") == 1) {
                            _removeData.push($obj.data('data'));
                        }
                        $obj.remove();
                    }
                }
                callback.call(this, e);
            }
        });

        $Panel.delegate('span', 'mousedown', function (e) {
            var offset = $(this).offset();
            var _parentOffset = $(this).parent().offset();

            $Panel.find(".sel").removeClass("sel");
            $(this).addClass("sel");

            this.posix = { 'x': e.pageX - offset.left + _parentOffset.left, 'y': e.pageY - offset.top + _parentOffset.top };
            $.extend(document, { 'move': true, 'move_target': this });
            return false;
        });
        $Panel.delegate('span', 'click', function (e) {
            $.extend(document, { 'dblclick': true, 'dblclick_target': this });
        });
        $Panel.delegate('span', 'dblclick', function (e) {
            _setTabAttribute(this);
            $.extend(document, { 'dblclick': true, 'dblclick_target': this });
        });
        $Panel.delegate('span', 'keydown', function (e) {
            $.extend(document, { 'dblclick': true, 'dblclick_target': this });
        });

        $Panel.delegate('em', 'mousedown', function (e) {
            var offset = $(this).offset();
            var _parentOffset = $(this).parent().offset();

            $Panel.find(".sel").removeClass("sel");
            $(this).addClass("sel");

            this.posix = { 'x': e.pageX - offset.left + _parentOffset.left, 'y': e.pageY - offset.top + _parentOffset.top };
            $.extend(document, { 'move': true, 'move_target': this });
            return false;
        });
        $Panel.delegate('em', 'click', function (e) {
            $.extend(document, { 'dblclick': true, 'dblclick_target': this });
        });
        $Panel.delegate('em', 'dblclick', function (e) {
            _setCameraAttribute(this);
            $.extend(document, { 'dblclick': true, 'dblclick_target': this });
        });
        $Panel.delegate('em', 'keydown', function (e) {
            $.extend(document, { 'dblclick': true, 'dblclick_target': this });
        });
    };

    /**
    *   移动设备
    *   @device 设置备
    *   @direction 方向
    */
    var mobileDevice = function (device, direction) {
        
        var $that = $(device);
        var data = $that.data("data");
        if (data) {
            var left = Number(data.X.replace("px",""));
            var top = Number(data.Y.replace("px", ""));


            if (direction == 'left') {
                left -=1;
            } else if (direction == 'right') {
                left +=1;
            } else if (direction == 'up') {
                top -= 1;
            } else if (direction == 'down') {
                top += 1;
            }
            data.X = left + "px";
            data.Y = top + "px";
            $that.css({ left: left, top: top });
            $that.data("data", data);
        }
    };

    //设置窗口和摄像
    var _setConfig = function () {
        $.ajax({
            type: "GET",
            url: _data.configUrl,
            data: util.packagData({ hallNo: _data.hallNo }),
            success: function (data) {
                $.each(data, function (i, item) {
                    
                    if (item.Type == 1) {
                        var $user = $("#tab_" + item.Id);
                        if ($user.length < 1) {
                            $user = $('<span d-type="device" d-state="1" id="tab_' + item.Id + '"></span>');
                            $user.append('<dt class="hallNum">'+ item.Id +'</dt>');
                            var $img = $('<img class="imgSize" />');
                            $img.attr("src", item.IconUrl);
                            $user.append($img);
                            $user.addClass("hall-icon");
                            $user.css({ left: item.X, top: item.Y });
                            $user.data("data", item);
                            $Panel.append($user);
                        } else {
                            $('img', $user).attr("src", item.IconUrl);
                        }
                        $user.css({ left: item.X, top: item.Y });
                        $user.data("data", item);

                        $user.mousedown(function(e) {
                            var offset = $(this).offset();
                            var s = offset.left;
                        });

                    } else {
                        var $device = $("#camera_" + item.Id);
                        if ($device.length < 1) {
                            $device = $('<em d-type="device" d-state="1"  id="camera_' + item.Id + '"></em>');
                            var $img = $('<img class="imgSize" />');
                            $img.attr("src", item.IconUrl);
                            $device.attr("d-cameraType", item.CameraType);
                            $device.append($img);
                            $device.addClass("hall-icon");
                            $Panel.append($device);
                        } else {
                            $('img', $device).attr("src", item.IconUrl);
                        }
                        $device.css({ left: item.X, top: item.Y });
                        $device.attr('index', item.DirType);
                        $device.data("data", item);
                    }
                });
            }
        });
    };

    //获取文件名称
    var _getFileData = function (filepath) {
        var re = /(\\+)/g;
        var filename = filepath.replace(re, "#");
        var one = filename.split("#");
        var name = one[one.length - 1];
        var nameStr= name.split(".");
        var ext = nameStr[nameStr.length - 1];
        var names = nameStr[0].split('_');
        return { name: names[0], ext: ext,filename:name };
    }

    //获取所有窗口的数据信息
    var _getTabDatas = function () {
        var items = [];
        var l = $("span[d-type='device']", $("#" + _data.webHallId)).length;
        $("span[d-type='device']", $("#" + _data.webHallId)).each(function (i, item) {
            items.push($(item).data("data"));
        });
        return items;
    };

    //保存数据
    var _saveData = function() {
        $("#save").click(function() {
            var hall = {
                HallNo:_data.hallNo,
                ImageUrl: _data.image,
                HallTabConfigs: [],
                HallCameraConfigs:[],
                RemoveHallConfigs: _removeData
            };
            $("span[d-type='device']", $Panel).each(function (i, t) {
                var data = $(t).data("data");
                if (data) {
                    hall.HallTabConfigs.push(data);
                }
            });
            $("em[d-type='device']", $Panel).each(function (i, t) {
                var data = $(t).data("data");
                if (data) {
                    hall.HallCameraConfigs.push(data);
                }
            });

            $.ajax({
                type: 'POST',
                url: _data.saveUrl,
                data: JSON.stringify(hall),
                contentType: 'application/json',
                success: function (data) {
                    if (data.success == 0) {
                        gzsw.dialog.alert('保存成功.', 1);

                        //让他重新加载
                        _setConfig();
                    } else {
                        gzsw.dialog.alert('保存失败.',5);
                    }
                },
                error: function (dd, a) {
                    gzsw.dialog.alert('保存失败.',5);
                }
            });

        });
    };

    //设置窗口属性
    var _setTabAttribute= function(that) {
        var $this = $(that);
        var item = $this.data("data");
        var $panel = $("#tabAttribute");
        if ($panel.length < 1) {
            $panel = $("<div id='tabAttribute' style='width:300px;'></div>");
            var table = $("<table class='formTable' cellpadding='0' cellspacing='0'>").appendTo($panel);
            var tr1 = $("<tr>").appendTo(table);
            tr1.append("<td class='tdLeft'> 窗口号: </td>");
            tr1.append("<td> <input type='text' d-type='num' class='text-box' /> </td>");

            var tr2 = $("<div style='text-align: center;'><input type='button' class='l-button hall_btn' d-type='button' value='确定' /></div>");
            $panel.append(tr2);

            $('body').append($panel);
        }

        $panel.find("input[d-type='num']").val(item.Id);

        var $btn = $panel.find("input[d-type='button']");
        $btn.data("target", $this);
        $btn.on("click", function() {
            var thisBtn = $(this);
            var thisInput = thisBtn.parent().parent().find("input[d-type='num']");
            var value = thisInput.val();
            var re = /^[0-9]*[1-9][0-9]*$/;
            if (!re.test(value)) {
                gzsw.dialog.alert('请输入大于0的正整数.', 4, function (index) {
                    thisInput.focus();
                });
                return false;
            }

            var $target = $(thisBtn.data("target"));
            var data = $target.data("data");

            var items = _getTabDatas();
            var isExist = false;
            $.each(items, function(i, tab) {
                if (tab.Id == value) {
                    isExist = true;
                }
            });
            if (isExist && data.Id != value) {
                gzsw.dialog.alert('窗口号已存在.', 4, function (index) {
                    thisInput.focus();
                });
                return false;
            }

            data.Id = value;
            $target.data("data", data);

            //更新页面上的窗口数值
            $target.find("dt.hallNum").text(data.Id);

            layer.close(dialogIndex);
        });

        dialogIndex = gzsw.dialog.openDiv({
            Div: "#tabAttribute",
            //标题
            title: "设置窗口属性",
            // 左面板ID
            width: 300,
            // 内容面板ID
            height: 200
        });
    }

    //设置摄像的属性
    var _setCameraAttribute = function(that) {
        var $this = $(that);
        var item = $this.data("data");
        var $panel = $("#cameraAttribute");
        if ($panel.length < 1) {
            $panel = $("<div id='cameraAttribute' style='width:400px;'></div>");
            var table = $("<table class='formTable' cellpadding='0' cellspacing='0'>").appendTo($panel);
            var tr1 = $("<tr>").appendTo(table);
            tr1.append("<td class='tdLeft'> IP地址: </td>");
            tr1.append("<td> <input type='text' d-type='IP' class='text-box' /> </td>");

            var tr2 = $("<tr>").appendTo(table);
            tr2.append("<td class='tdLeft'> HTTP协议: </td>");
            tr2.append("<td> <select d-type='HTTP_PROTOCOL' class='text-box' ><option value='1'>http</option><option value='2'>https</option></select> </td>");//http协议，1表示http协议 2表示https协议 

            var tr3 = $("<tr>").appendTo(table);
            tr3.append("<td class='tdLeft'> 端口号: </td>");
            tr3.append("<td> <input type='text' d-type='IPORT' class='text-box' value='80' /> 默认为80</td>");

            var tr4 = $("<tr>").appendTo(table);
            tr4.append("<td class='tdLeft'> 用户名: </td>");
            tr4.append("<td> <input type='text' d-type='USER_NAME' class='text-box' /> </td>");


            var tr5 = $("<tr>").appendTo(table);
            tr5.append("<td class='tdLeft'> 密码: </td>");
            tr5.append("<td> <input type='text' d-type='USER_PASSWORD' class='text-box' /> </td>");

            var tr6 = $("<tr style='display:none;'>").appendTo(table);
            tr6.append("<td class='tdLeft'> CGI协议类型: </td>");
            tr6.append("<td> <select d-type='CGI_PROTOCOL' class='text-box' ><option value=''>请选择</option><option value='1'>ISAPI</option><option value='2'>PSIA</option></select> </td>");

            var tr7 = $("<tr style='display:none;'>").appendTo(table);
            tr7.append("<td class='tdLeft'> 码流类型: </td>");
            tr7.append("<td> <select d-type='STRING_TYP' class='text-box' ><option value='1'>主码流</option><option value='2'>子码流</option></select> </td>");

            var tr8 = $("<tr>").appendTo(table);
            tr8.append("<td class='tdLeft'> 播放通道号: </td>");
            tr8.append("<td> <input type='text' d-type='CHANNEL_ID' class='text-box'  /> </td>");

            var tr9 = $("<tr style='display:none;'>").appendTo(table);
            tr9.append("<td class='tdLeft'> 是否播放零通道: </td>");
            tr9.append("<td> <input type='checkbox' d-type='ZERO_CHANNEL_IND' class='text-box' /> </td>");

            var tr10 = $("<tr style='display:none;'>").appendTo(table);
            tr10.append("<td class='tdLeft'> RTSP端口号: </td>");
            tr10.append("<td> <input type='text' id='RTSP_PORT' d-type='RTSP_PORT' class='text-box' /> </td>");

            var tr11 = $("<tr>").appendTo(table);
            tr11.append("<td class='tdLeft'> 监控的窗口号: </td>");
            tr11.append("<td> <input type='text' d-type='MON_COUNTER' class='text-box' /> 多个以|分隔 </td>");

            var tr12 = $("<tr>").appendTo(table);
            tr12.append("<td class='tdLeft'> 是否隐藏: </td>");
            tr12.append("<td> <input type='checkbox' d-type='MON_SHOW_IND' class='text-box' />  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;用于在虚拟大厅中显示</td>");

            var input1 = $("<div style='text-align: center;'><input type='button' class='l-button hall_btn' d-type='button' value='确定' /></div>");
            $panel.append(input1);

            $('body').append($panel);
        }

        //设置参数
        $panel.find("input[d-type='IP']").val(item.Ip);
        $panel.find("select[d-type='HTTP_PROTOCOL']").val(item.HttpProtocol);
        $panel.find("input[d-type='IPORT']").val(item.Iport);
        $panel.find("input[d-type='USER_NAME']").val(item.UserName);
        $panel.find("input[d-type='USER_PASSWORD']").val(item.Password);
        $panel.find("select[d-type='CGI_PROTOCOL']").val(item.CgiProtpcpl);
        $panel.find("select[d-type='STRING_TYP']").val(item.StringType);
        $panel.find("input[d-type='CHANNEL_ID']").val(item.ChannelId);
        if (item.ZeroChannelInd) {
            $panel.find("input[d-type='ZERO_CHANNEL_IND']").prop("checked", true);
        } else {
            $panel.find("input[d-type='ZERO_CHANNEL_IND']").prop("checked", false);
        }
        if (item.MonShowing) {
            $panel.find("input[d-type='MON_SHOW_IND']").prop("checked", true);
        } else {
            $panel.find("input[d-type='MON_SHOW_IND']").prop("checked", false);
        }
        $panel.find("input[d-type='RTSP_PORT']").val(item.RtspPort);
        $panel.find("input[d-type='MON_COUNTER']").val(item.MonCounter);

        //打开弹层
        dialogIndex=gzsw.dialog.openDiv({
                                        Div: "#cameraAttribute",
                                        //标题
                                        title: "设置摄像头属性",
                                        // 左面板ID
                                        width: 400,
                                        // 内容面板ID
                                        height: 400
                                    });


       var $btn = $panel.find("input[type='button']");
       $btn.data("target", $this);
       $btn.on("click", function () {
           var thisBtn = $(this);
           var panelCamera = thisBtn.parent().parent();

           var thisInput2 = panelCamera.find("#RTSP_PORT");
           var rtspPort = thisInput2.val();
           var re = /^[0-9]*[1-9][0-9]*$/;
           if (!thisInput2 && rtspPort.length > 0 && !re.test(rtspPort)) {
               gzsw.dialog.alert('RTSP端口号应输入大于0的正整数.', 4, function (index) {
                   thisInput2.focus();
               });
               return false;
           }

           var mon_counterValue = panelCamera.find("input[d-type='MON_COUNTER']").val();
           if (mon_counterValue.length > 0) {
               var isSuccess = true;
               $.each(mon_counterValue.split('|'), function(i, s) {
                   if (s.length > 0 && !re.test(s)) {
                       isSuccess = false;
                   }
               });
               if (!isSuccess) {
                   gzsw.dialog.alert('请输入正确的监控窗口号.', 4);
                   return false;
               }
           }


           var $target = $(thisBtn.data("target"));
           var data = $target.data("data");

           data.Ip = panelCamera.find("input[d-type='IP']").val();
           data.HttpProtocol = panelCamera.find("select[d-type='HTTP_PROTOCOL']").val();
           data.Iport = panelCamera.find("input[d-type='IPORT']").val();
           data.UserName = panelCamera.find("input[d-type='USER_NAME']").val();
           data.Password = panelCamera.find("input[d-type='USER_PASSWORD']").val();
           data.CgiProtpcpl = panelCamera.find("select[d-type='CGI_PROTOCOL']").val();
           data.StringType = panelCamera.find("select[d-type='STRING_TYP']").val();
           data.ChannelId = panelCamera.find("input[d-type='CHANNEL_ID']").val();
           data.ZeroChannelInd = panelCamera.find("input[d-type='ZERO_CHANNEL_IND']").prop("checked");
           data.RtspPort = rtspPort;
           data.MonCounter = panelCamera.find("input[d-type='MON_COUNTER']").val();
           data.MonShowing = panelCamera.find("input[d-type='MON_SHOW_IND']").prop("checked");
           

           $target.data("data", data);

           layer.close(dialogIndex);
       });
    };

    return {
        init: function(data) {
            _data = data;
            _init();
        },
        setHallImg: function(imgUrl) {
            _setHall(imgUrl);
        }
    }
}();
