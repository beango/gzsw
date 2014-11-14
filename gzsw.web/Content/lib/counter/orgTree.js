/*
*   组织架构树
*   @jcliang
*/

var orgTree = function () {

    var _options = {
        //document元素ID值  显示值
        documentId: "HallName",
        //隐藏保存值的ID
        hidValueId: "HallNo",
        //面板ID
        panelID: "orgTreeContent",
        //树的Id
        treeULId: "treeOrg",
        //加数据的地址
        loadDataUrl: '',
        //
        enableHide:true,
        //树的结构
        ztreeSetting: {
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
                            dblClickExpand: false,
                            showIcon: false
                        },
                        callback: {
                            onClick: onClick
                        }
        },
        //选择事件
        onSelect:null
    };

    var _init = function() {
        var $doc = $("#" + _options.documentId);

        //创建层
        var $panel = $("#" + _options.panelID);
        var $treeUl = $("#" + _options.treeULId);
        if ($panel.length < 1) {
            $panel = $("<div id='" + _options.panelID + "' style='display:none; position: absolute;'></div>");
            $panel.append('<ul id="' + _options.treeULId + '" class="ztree" style="clear:both;"></ul>');

            $("body").append($panel);
        } else {
            if ($treeUl < 1) {
                $('<ul id="' + _options.treeULId + '" class="ztree" style="clear:both;"></ul>').appendTo($panel);
            }
        }

        $doc.on("mousedown", function () {
            _timeout && clearTimeout(_timeout);

            var orgObj = $(this);
            var orgOffset = orgObj.offset();
            var $panel = $("#" + _options.panelID).css({ left: orgOffset.left + "px", top: orgOffset.top + orgObj.outerHeight() + "px" }).slideDown("fast");
            $panel.data("show", true);

            $("body").bind("mousedown", orgTree.onBodyDown);

            return false;
        }).on("mouseout", function() {
            _timeout = setTimeout(function () {
                _hidPanel();
            }, 800);
        });

        $("#" + _options.panelID).on("mouseover", function() {
            _timeout && clearTimeout(_timeout);
        }).on("mouseout", function() {
            _timeout = setTimeout(function () {
                _hidPanel();
            }, 800);
        });

        _ininTree();
    };

    var _timeout;

    //初始树
    var _ininTree = function () {
        $(function () {
            $.ajax({
                async: false,
                cache: false,
                type: 'POST',
                dataType: 'json',
                url: _options.loadDataUrl,
                success: function (data) {
                    $.fn.zTree.init($("#" + _options.treeULId), _options.ztreeSetting, data);
                }
            });
        });
    };

    var _hidPanel = function() {
        $("#" + _options.panelID).fadeOut("fast");
        $("body").unbind("mousedown", orgTree.onBodyDown);
    };

    return {
        init: function (options) {
            if (options) {
                $.extend(_options, options);
            }
            _init();
        },
        onBodyDown: function(event) {
            if (!(event.target.id == "orgTreeContent" || $(event.target).parents("#orgTreeContent").length > 0)) {
                _hidPanel();
            }
        },
        hidPanel: function() {
            _hidPanel();
        },
        getOption: function() {
            return _options;
        }
    }
}();

function onClick(e, treeId, treeNode) {
    var option = orgTree.getOption();

    if (option.enableHide) {
        orgTree.hidPanel();
    }
    
    if (treeNode.enable) {
        $("#" + option.hidValueId).val(treeNode.id);
        $("#" + option.documentId).val(treeNode.name);
        orgTree.hidPanel();
    }

    if (typeof option.onSelect == 'function') {
        option.onSelect(treeId, treeNode);
    }
}