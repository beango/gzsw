/*
* 播放视频JS  
* @jcling
*/

var video = function () {

    //播放的窗口
    var iWndIndex = 0;
    //云台是否自动
    var bPTZAuto = false;
    //云台转动速度
    var iPTZSpeed = 4;

    /*
    *   默认初始化参数
    */
    var _defaultInit = {
        downloadUrl:"",
        screen: "divPlugin", //要播放的DIV的Id，
        width: 500,
        height: 300,
        iWndowType: 1 //分屏类型：1- 1*1，2- 2*2，3- 3*3，4- 4*4，默认值为1，单画面
    };

    /*
    *   默认登录参数
    **/
    var _defaultLogin = {
        szIP: "",       //设备的IP地址或者普通域名(比如花生壳域名)
        iPrototocol: 1,//http协议，1表示http协议 2表示https协议
        iPort: 80,//端口号
        szUserName: "",//登录用户名称
        szPassword: "",//用户密码
        async: null,//http交互方式，true表示异步，false表示同步
        cgi: null,   //CGI协议选择，1表示ISAPI，2表示PSIA，如果不传这个参数，会自动选择一种设备支持的协议.
        success: null,//成功回调函数，有一个参数，表示返回的XML内容。
        error: null //失败回调函数，有两个参数，第一个是http状态码，第二个是设备返回的XML(可能为空)
    };

    /*
    *   默认播放参数
    */
    var _defaultPlay = {
        iWndIndex: 0, //播放窗口，如果不传，则默认使用当前选择窗口播放（默认选中窗口0）
        iStreamType: 1,//码流类型1-主码流，2-子码流，默认使用主码流预览
        iChannelID: 1,//播放通道号，默认通道1
        bZeroChannel: false,//是否播放零通道，默认为false
        iPort: null //RTSP端口号，可以选择传入，如果不传，开发包会自动判断设备的RTSP端口
    };

    var _init = function (options) {
        _defaultInit = $.extend(_defaultInit, options);

        // 检查插件是否已经安装过
        if (-1 == WebVideoCtrl.I_CheckPluginInstall()) {
            var ret = confirm("您还未安装过插件,是否下载安装?");
            if (ret) {
                window.open(_defaultInit.downloadUrl);
            }
            //alert("您还未安装过插件，双击开发包目录里的WebComponents.exe安装！");
            return;
        }

        // 初始化插件参数及插入插件
        WebVideoCtrl.I_InitPlugin(_defaultInit.width, _defaultInit.height, {
            iWndowType: _defaultInit.iWndowType,
            cbSelWnd: function (xmlDoc) {
                //选择窗口
            }
        });

        //嵌入播放插件
        WebVideoCtrl.I_InsertOBJECTPlugin(_defaultInit.screen);
    };

    /*
    *   登录
    */
    var _login = function (options) {
        _defaultLogin = $.extend(_defaultLogin, options);
        var iRet = WebVideoCtrl.I_Login(_defaultLogin.szIP,
                                        _defaultLogin.iPrototocol,
                                        _defaultLogin.iPort,
                                        _defaultLogin.szUserName,
                                        _defaultLogin.szPassword, {
                                            cgi:_defaultLogin.cgi,
                                            success: function (xmlDoc) {
                                                if (typeof _defaultLogin.success == 'function') {
                                                    _defaultLogin.success(xmlDoc);
                                                }
                                            },
                                            error: function () {
                                                alert("初始化视频设置失败,请确认配置是否正确.");
                                                if (typeof _defaultLogin.error == 'function') {
                                                    _defaultLogin.error();
                                                }
                                            }
        });
    };

    /*
    *   播放
    */
    var _startRealPlay = function (options) {
        _defaultPlay = $.extend(_defaultPlay, options);

        iWndIndex = _defaultPlay.iWndIndex;

        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) {
            // 已经在播放了，先停止
            WebVideoCtrl.I_Stop();
        }

        var opt = {
            iStreamType: _defaultPlay.iStreamType,
            iChannelID: _defaultPlay.iChannelID,
            bZeroChannel: _defaultPlay.bZeroChannel
        };

        if (_defaultPlay.iPort) {
            opt = {
                iStreamType: _defaultPlay.iStreamType,
                iChannelID: _defaultPlay.iChannelID,
                bZeroChannel: _defaultPlay.bZeroChannel,
                iPort: _defaultPlay.iPort
            };
        }

        var iRet = WebVideoCtrl.I_StartRealPlay(_defaultLogin.szIP, opt);
        //alert(iRet);
    };

    /*
    *   恢复播放
    */
    var _resumeRealPlay = function() {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) {
            WebVideoCtrl.I_Resume();
        }
    };

    //停止播放
    var _stopRealPlay = function() {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) {
            WebVideoCtrl.I_Stop();
        }
    };

    //全屏
    var _fullScreen = function() {
        WebVideoCtrl.I_FullScreen(true);
    };

    //打开声音
    var _openSound = function() {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) {
            var allWndInfo = WebVideoCtrl.I_GetWindowStatus();

            // 循环遍历所有窗口，如果有窗口打开了声音，先关闭
            for (var i = 0, iLen = allWndInfo.length; i < iLen; i++) {
                oWndInfo = allWndInfo[i];
                if (oWndInfo.bSound) {
                    WebVideoCtrl.I_CloseSound(oWndInfo.iIndex);
                    break;
                }
            }

            //打开声音
            var iRet = WebVideoCtrl.I_OpenSound();
        }
    };

    //关闭声音
    var _closeSound = function() {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) {
            var iRet = WebVideoCtrl.I_CloseSound();
        }
    };

    //设置音量
    var _setVolume = function (iVolume) {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) {
            var iRet = WebVideoCtrl.I_SetVolume(iVolume);
        }
    };

    /*
    *   云台控制
    */

    //设置云台转动速度
    var _setPTZSpeed= function(speed) {
        iPTZSpeed = speed;
    }

    //开始设置云台方向, PTZ控制 9为自动，1,2,3,4,5,6,7,8为方向PTZ
    //1-上，2-下，3-左，4-右，5-左上，6-左下，7-右上，8-右下，9-自转，10-调焦+， 11-调焦-, 12-F聚焦+, 13-聚焦-, 14-光圈+, 15-光圈-）
    var _setBeginPTZControl = function(iPTZIndex) {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        var bStop = false;
        console.log(iPTZIndex);
        if (oWndInfo != null) {
            if (9 == iPTZIndex && bPTZAuto) {
                iPTZSpeed = 0;// 自动开启后，速度置为0可以关闭自动
                bStop = true;
            } else {
                bPTZAuto = false;// 点击其他方向，自动肯定会被关闭
                bStop = false;
            }

            WebVideoCtrl.I_PTZControl(iPTZIndex, bStop, {
                iPTZSpeed: iPTZSpeed,
                success: function (xmlDoc) {
                    if (9 == iPTZIndex) {
                        bPTZAuto = !bPTZAuto;
                    }
                    //开启云台成功！
                },
                error: function () {
                    // 开启云台失败！
                }
            });
        }
    };

    //结束设置云台方向
    var _setEndPTZControl = function() {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);

        if (oWndInfo != null) {
            WebVideoCtrl.I_PTZControl(1, true, {
                success: function (xmlDoc) {
                    //停止云台成功！;
                },
                error: function () {
                    //停止云台失败！;
                }
            });
        }
    };

    //增加焦距
    // iPTZIndex 10-调焦+， 11-调焦- 12-F聚焦+, 13-聚焦-, 14-光圈+, 15-光圈-
    // bStop 是否停止iPTZIndex指定的操作
    var _setPTZZoomFoucusIris = function (iPTZIndex, bStop) {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);

        if (oWndInfo != null) {
            WebVideoCtrl.I_PTZControl(iPTZIndex, bStop, {
                iWndIndex: iWndIndex,
                success: function (xmlDoc) {
                    //调焦+成功！
                },
                error: function () {
                   //调焦+失败！
                }
            });
        }
    };


    /*
    *   回放
    **/
    var startPlayback = function(iChannelID, szStartTime, szEndTime) {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) { // 已经在播放了，先停止
            WebVideoCtrl.I_Stop();
        }

        var date = new Date();
        if (!szStartTime) {
            szStartTime = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate() + " 00:00:00";
        }
        if (!szEndTime) {
            szEndTime = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate() + " 23:59:59";
        }

        WebVideoCtrl.I_StartPlayback(_defaultLogin.szIP, {
            iChannelID: iChannelID,
            szStartTime: szStartTime,
            szEndTime: szEndTime
        });
    };
    //倒放
    var reversePlayback = function (iChannelID, szStartTime, szEndTime) {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) {// 已经在播放了，先停止
            WebVideoCtrl.I_Stop();
        }

        WebVideoCtrl.I_ReversePlayback(szIP, {
            iChannelID: iChannelID,
            szStartTime: szStartTime,
            szEndTime: szEndTime
        });
    };

    //暂停
    var pausePlay = function() {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) { // 已经在播放了，先停止
            WebVideoCtrl.I_Pause();
        }
    };
    
    //慢放
    var playSlow = function() {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) {
            WebVideoCtrl.I_PlaySlow();
        }
    };

    //快放
    var playFast = function () {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        if (oWndInfo != null) {
            WebVideoCtrl.I_PlayFast();
        }
    };
    //是否播放
    var isInPlay = function() {
        var oWndInfo = WebVideoCtrl.I_GetWindowStatus(iWndIndex);
        return oWndInfo != null;
    };
    return {
        init: function (options) {
            _init(options);
        },
        login: function (options) {
            _login(options);
        },
        //是否播放
        isInPlay: function() {
            return isInPlay();
        },
        startRealPlay: function (options) {
            setTimeout(function() {
                _startRealPlay(options);
            }, 200);
        },
        stopRealPlay: function() {
            _stopRealPlay();
        },
        resumeRealPlay: function() {
            _resumeRealPlay();
        },
        fullScreen: function () {
            _fullScreen();
        },
        openSound:function() {
            _openSound();
        },
        closeSound: function() {
            _closeSound();
        },
        setVolume: function (iVolume) {
            _setVolume(iVolume);
        },
        /*
        *   下面是云台的控制
        */
        setPTZSpeed: function(speed) {
            _setPTZSpeed(speed);
        },
        setBeginPTZControl: function(index) {
            _setBeginPTZControl(index);
        },
        setEndPTZControl: function() {
            _setEndPTZControl();
        },
        setPTZZoomIn:function() {
            _setPTZZoomFoucusIris(10, false);
        },
        setPTZZoomOut:function() {
            _setPTZZoomFoucusIris(11, false);
        },
        setPTZZoomStop: function() {
            _setPTZZoomFoucusIris(11, true);
        },
        setPTZFoucusIn: function() {
            _setPTZZoomFoucusIris(12, false);
        },
        setPTZFoucusOut: function () {
            _setPTZZoomFoucusIris(13, false);
        },
        setPTZFoucusStop: function () {
            _setPTZZoomFoucusIris(12, true);
        },
        setPTZIrisIn: function() {
            _setPTZZoomFoucusIris(14, false);
        },
        setPTZIrisOut: function () {
            _setPTZZoomFoucusIris(15, false);
        },
        setPTZIrisStop: function () {
            _setPTZZoomFoucusIris(14, true);
        },
        /*
        *   回放
        */
        startPlayback: function (iChannelID, szStartTime, szEndTime) {
            startPlayback(iChannelID, szStartTime, szEndTime);
        },
        reversePlayback: function (iChannelID, szStartTime, szEndTime) {
            reversePlayback(iChannelID, szStartTime, szEndTime);
        },
        pausePlay: function() {
            pausePlay();
        },
        playSlow: function() {
            playSlow();
        },
        playFast: function() {
            playFast();
        }
    };
}();