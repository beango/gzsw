/************************************************
* 功能说明: JQuery dialog plug-in 1.0
* 创建时间: 2014-10-12
*   创建人: hcli 
/************************************************/
if (typeof gzsw == "undefined") {
    gzsw = new Object();
}
gzsw.map = function () {
    // 地图对象
    var map =null;
    // 坐标对象数组
    var markerArray = new Array();
    // 弹出窗口数组
    var infowindowArray = new Array();

   // 创建地图对象
    function CreateMap() {
        var settings = { 
           id:"",
           cityName:"南昌",
           defaultX:"28.695406",
           defaultY:"115.859649"
        };
        return settings;
    } 


  // 创建坐标点
  function CreateMarker()
  {
       var settings = { 
       	   title:"",
       	   id:getGuid(),
           x:null,
           y:null,
           text: "",
           url:"",
           isUseDialog: false,
           isUseLink:false
        };
        return settings;
  }
 
   // 获取唯一标示
  function getGuid(){ 
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4()); 
  }

  /* 创建地图对象 */
  function LocalMapType() {} 
	  LocalMapType.prototype.tileSize = new google.maps.Size(256, 256);
	  LocalMapType.prototype.maxZoom = 15;   //地图显示最大级别
	  LocalMapType.prototype.minZoom = 9;    //地图显示最小级别
	  LocalMapType.prototype.name = '本地数据';
	  LocalMapType.prototype.alt = '显示本地地图数据';
	  LocalMapType.prototype.getTile = function(coord, zoom, ownerDocument) {

	      var img = ownerDocument.createElement('img');
	      img.style.width = this.tileSize.width + 'px';
	      img.style.height = this.tileSize.height + 'px';
	      var strURL = '/Content/lib/map/maptile/mapabc/';
	      strURL += zoom + '/' + coord.x + '/'+ coord.y + '.PNG';
	      img.src = strURL;
	      return img;
  }; 
  
    // 关闭所有弹出框
 	function closeAllInfoWindow(){
	 
	    for (var i=0;i<infowindowArray.length;i++)
		{ 
		   infowindowArray[i].close();
		}
	}

    return {
        /* 新增坐标 */
        addMarker: function(options) {
            /* 初始化配置 */
            var $$Settings = CreateMarker();
            if (options) {
                $.extend($$Settings, options);
            }

            var marker = new google.maps.Marker({
                position: new google.maps.LatLng($$Settings.x, $$Settings.y),
                map: map,
                draggable: false, 
                title: $$Settings.title,
                /*   icon:'/Content/lib/map/mapfiles/marker_sprite.png' */
            }); 

            if ($$Settings.isUseLink == true) {
                google.maps.event.addListener(marker, 'click', function() {
                    if ($$Settings.url != undefined && $$Settings.url != "") {
                        location.href = $$Settings.url;
                    }
                });
                map.setCenter(marker.getPosition()); 
                markerArray.push(marker);
            } else {

                var infowindow = new google.maps.InfoWindow(
                {
                    content: 'latlng:' + marker.getPosition().toString(),
                    size: new google.maps.Size(50, 50),
                    /*   backgroundImage : 'mapfiles/iphone-dialog-bg.png'*/
                });

                google.maps.event.addListener(marker, 'click', function() {

                    // 关闭所有弹出窗
                    closeAllInfoWindow();
                    infowindow.setContent($$Settings.text);
                    infowindow.open(map, marker);
                });

                markerArray.push(marker);
                infowindowArray.push(infowindow);
            }

        },
        /* 初始化 */
        init: function(options) {
            /* 初始化配置 */
            var $$Settings = CreateMap();
            if (options) {
                $.extend($$Settings, options);
            }
            /* 设置默认坐标 和 配置 */
            var myLatlng = new google.maps.LatLng($$Settings.defaultX, $$Settings.defaultY);
            var myOptions = {
                center: myLatlng,
                zoom: 15,
                streetViewControl: false,
                mapTypeControlOptions: {
                    mapTypeIds: [
                        google.maps.MapTypeId.ROADMAP,
                        /*  google.maps.MapTypeId.HYBRID,*/
                        /* google.maps.MapTypeId.SATELLITE,*/
                        /*google.maps.MapTypeId.TERRAIN,*/
                        'locaMap'
                    ] //定义地图类型
                }
            };
            var localMapType = new LocalMapType();
            map = new google.maps.Map(document.getElementById('map_canvas'), myOptions);
            map.mapTypes.set('locaMap', localMapType); //绑定本地地图类型
            map.setMapTypeId('locaMap'); //指定显示本地地图
        }
    };
}();