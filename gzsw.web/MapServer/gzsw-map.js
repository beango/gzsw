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
    // 图标刷新时间 默认1分钟
    var showTime = 3000;

   // 创建地图对象
    function CreateMap() {
        var settings = { 
            id: "",
            txt:"",
           cityName:"南昌",
           defaultX:"28.695406",
           defaultY:"115.859649"
        };
        return settings;
    } 


  // 创建坐标点
  function CreateMarker() {
      var settings = {
          title: "",
          id: getGuid(),
          x: null,
          y: null,
          text: "",
          // 跳转URL
          url: "",
          isUseDialog: false, 
          // 等级 	1:省级，2：市级，3：区级，4：服务厅级
          level: 0,
          // 地图数据地址
          mapDataUrl: null,
          showTime : showTime
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
      function LocalMapType() {
       
      } 
      LocalMapType.prototype.tileSize = new google.maps.Size(256, 256);
	  LocalMapType.prototype.maxZoom = 15;   //地图显示最大级别
	  LocalMapType.prototype.minZoom = 6;    //地图显示最小级别 
	  LocalMapType.prototype.name = '监控分布地图';
	  LocalMapType.prototype.alt = '显示监控分布地图'; 
	  LocalMapType.prototype.getTile = function(coord, zoom, ownerDocument) {
	      
	      var img = ownerDocument.createElement('img');
	      img.style.width = this.tileSize.width + 'px';
	      img.style.height = this.tileSize.height + 'px';
	      var strURL = '/MapServer/maptile/mapabc/';
	      strURL += zoom + '/' + coord.x + '/'+ coord.y + '.PNG';
	      img.src = strURL;
	      return img;
  }; 
   
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
                title: $$Settings.text,//$$Settings.title,
                zIndex: 10
                /*   icon:'/Content/lib/map/mapfiles/marker_sprite.png' */
            }); 
           
        
                if ($$Settings.level == 1) {
                       // 省级 
                    map.setZoom(7);
                    marker.icon = "/Content/cotide/img/level1.gif";
                } else if ($$Settings.level == 2) {
                    // 市级
                    map.setZoom(12);
                    marker.icon = "/Content/cotide/img/level2.gif";
                } else if ($$Settings.level == 3) {
                    // 区级 
                    map.setZoom(14);
                    marker.icon = "/Content/cotide/img/level3.gif";
                } else   {
                    marker.icon = "/Content/cotide/img/level4.gif";
                }
                
                // 服务厅级
                google.maps.event.addListener(marker, 'click', function () {
                    if ($$Settings.url != undefined && $$Settings.url != "") {
                        location.href = $$Settings.url;
                    }
                });  
            map.setCenter(marker.getPosition());
            markerArray.push(marker);   
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
                zoom: 16,
                streetViewControl: false,
              /*  position: google.maps.ControlPosition.TOP_RIGHT,*/
                mapTypeControlOptions: {
                    mapTypeIds: [
                       /*  google.maps.MapTypeId.ROADMAP,
                         google.maps.MapTypeId.HYBRID,*/
                        /* google.maps.MapTypeId.SATELLITE,*/
                        /*google.maps.MapTypeId.TERRAIN,*/
                        'locaMap'
                    ] //定义地图类型
                }
            };
            var localMapType = new LocalMapType();
           
            localMapType.name = "本地数据" ;
            localMapType.alt = "本地数据";
             
            map = new google.maps.Map(document.getElementById('map_canvas'), myOptions); 
            map.mapTypes.set('locaMap', localMapType); //绑定本地地图类型 
            map.setMapTypeId('locaMap'); //指定显示本地地图
              
        }
    };
}();