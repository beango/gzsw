/*
*	时分秒选择控件
*	@jclinag
*/
(function($){
    $.fn.timeSelect = function(options){
        var options = $.extend($.fn.timeSelect.defaults, options);

        $.fn.timeSelect.setPanel(options);

        this.each(function(){
        	//插件实现代码
        	$(this).click(function(){
        		var tt=this;
        		var $this=$(this);
        		var value=$this.val();
        		var ttop = tt.offsetTop;    //TT控件的定位点高 
				var thei = tt.clientHeight;    //TT控件本身的高 
				var tleft = tt.offsetLeft;    //TT控件的定位点宽 
				while (tt = tt.offsetParent) { 
					ttop += tt.offsetTop; 
					tleft += tt.offsetLeft; 
				} 

				var $panel=$("#"+options.defaultPanel);

				if(options.isShowHour && options.isShowMinute && options.isShowSecond){
					var hour="00",minute="00",second="00";
					if(value.length>0){
						hour=value.substring(0,2);
						minute=value.substring(3,5);
						//second=value.substring(4,6);
					}
					$("select[d-type='hour']",$panel).val(hour);
					$("select[d-type='minute']",$panel).val(minute);
					$("select[d-type='second']",$panel).val(second);
				}

				$panel.data("timeInput",this);
				$panel.css({ top: (ttop + thei + 4), left: tleft }).show();

	            return false;
	        });

        });
    };

    //参数默认值
    $.fn.timeSelect.defaults = {
        defaultPanel:"timeContentPanel", //时间面板
        isShowHour:true,	//是否显示小时
        isShowMinute:true,	//是否显示分
        isShowSecond:true,	//是否显示秒
        onSelect:null,		//确定选择事件
        onRemove:null		//清除事件
    };

    $.fn.timeSelect.setPanel=function(options){
    	var $panel = $("#"+options.defaultPanel);
    	if($panel.length<1){
    		$panel=$('<div id="'+ options.defaultPanel +'" style="padding:5px;background-color:#ECF7FE;font-size:12px;border:1px solid #CFD1D5;position:absolute;left:?px; top:?px; width:?px; height:?px;z-index:1;display:none;"></div>');

    		$("body").append($panel);
    	}
    	$.fn.timeSelect.setContent($panel, options);

        $panel.click(function() {
            return false;
        });
        $(document).click(function() {
            $panel.hide();
        });
    };

    $.fn.timeSelect.setContent=function(panel,options){
		//小时
		if(options.isShowHour){
			
    		var hour=$("<select d-type='hour'></select>");
    		for (var h=0;h<24; h++) {
    			var hstr=String(h);
    			if(h<10){
    				hstr="0"+h;
    			}
		        hour.append('<option value="' + hstr + '">' + hstr + '</option>');
    		};
    		
    		panel.append(hour);
    		panel.append("时");
		}

		//分
		if(options.isShowMinute){
			var minute=$("<select d-type='minute'></select>");
    		for (var i=0;i<60; i++) {
    			var str=String(i);
    			if(i<10){
    				str="0"+i;
    			}
		        minute.append('<option value="' + str + '">' + str + '</option>');
    		};
    		
    		panel.append(minute);
    		panel.append("分");
		}

		//秒
		if(options.isShowSecond){
			var second = $("<select d-type='second' style='display:none;'></select>");
    		for (var i=0;i<60; i++) {
    			var str=String(i);
    			if(i<10){
    				str="0"+i;
    			}
		        second.append('<option value="' + str + '">' + str + '</option>');
    		};
		    
    		panel.append(second);
		    //panel.append("秒");
		}

		var btnOk=$('<input type="button" style="margin: 0px 5px;border: 1px solid #ccc;"  value="确定" />');
		var btnClear = $('<input type="button" style="margin: 0px 5px;border: 1px solid #ccc;"  value="清除" />');
		
		btnOk.click(function(e){
			var $this=$(this);
			var $panel=$($this.parent());
			var $input=$($panel.data("timeInput"));

			var str = "";
		    var val = "";
			if(options.isShowHour){
				var $hour=$("select[d-type='hour']",$panel);
				str = $hour.val();
				val = $hour.val();
				str += "时";
			}
			if(options.isShowMinute){
			    var $minute = $("select[d-type='minute']", $panel);
			    str += $minute.val();
			    val += $minute.val();
			    str += "分";
			}
			if(options.isShowSecond){
				var $second=$("select[d-type='second']",$panel);
				//str += $second.val();
			    //str += "秒";
				val += $second.val();
			}

			$input.val(str);
			$input.prev().val(val);

			$panel.hide();

			if(typeof options.onSelect =='function'){
				options.onSelect(e,str);
			}
		});

		btnClear.click(function(e){
			var $this=$(this);
			var $panel=$($this.parent());
			var $input=$($panel.data("timeInput"));

			$input.val("");
			$panel.hide();

			if(typeof options.onRemove == 'function' ){
				options.onRemove(e);
			}
		});

		panel.append(btnOk);
		panel.append(btnClear);
    };

})(jQuery);