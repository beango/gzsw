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
	  LocalMapType.prototype.minZoom =6;    //地图显示最小级别
	 /* LocalMapType.prototype.name = '监控分布地图';
	  LocalMapType.prototype.alt = '显示监控分布地图';*/
/*    debugger;
	  LocalMapType.prototype.style.visibility = "hidden";*/
	  LocalMapType.prototype.getTile = function(coord, zoom, ownerDocument) {
	     
	      //alert($(ownerDocument.body).find("div[title='显示江西省-监控分布地图-监控分布地图']").length);

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
                zIndex: 10, 
                /*   icon:'/Content/lib/map/mapfiles/marker_sprite.png' */
            }); 
           
            if ($$Settings.level > 0 && $$Settings.level < 4) {

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
                }
                
                 
          
             google.maps.event.addListener(marker, 'click', function () {  
                 layer.closeAll();

                 var dialogW = ($(window).width() / 2)-50;
                 var dialogH = ($(window).height() / 2)-20;
                   gzsw.dialog.openHtml({
                                 title: "当前办税排队人数",
                                 html: "<div id=\"waitPersonsContent\" tag=\"chart\"  style=\"width:" + dialogW + "px;height:" + dialogH+250 + "px;\" ><p style=\"text-align: center;padding-top: " + (dialogH / 2) + "px;font-size: 14px;\">加载中....</p></div>",
                                 width: dialogW,
                                 height: dialogH,
                                 showType: 'right-top'  
                   }); 
                  gzsw.dialog.openHtml({
                      title: "当前等候时间",
                      html: "<div id=\"waitTimesContent\" tag=\"chart\"  style=\"width:" + dialogW + "px;height:" + dialogH + 250 + "px;\"  ><p style=\"text-align: center;padding-top:" + (dialogH / 2) + "px;font-size: 14px;\">加载中....</p></div>",
                      width: dialogW,
                      height: dialogH,
                      showType: 'right-bottom'  
                  });

                  gzsw.dialog.openHtml({
                      title: "当前饱和度和满意度",
                      html: "<div id=\"saturationContent\" tag=\"chart\" style=\"width:" + dialogW + "px;height:" + dialogH + 250 + "px;\"  ><p style=\"text-align: center;padding-top: " + (dialogH / 2) + "px;font-size: 14px;\">加载中....</p></div>",
                      width: dialogW,
                      height: dialogH,
                      showType: 'left-bottom' 
                  });

                  // 读取数据
                  $.ajax({
                      url: $$Settings.mapDataUrl.urlstamp(), success: function (result) {
                         
                          if (result != undefined && result != "") {
                              if (result.IsSuccess == true) {
                                  var organizeX = new Array();
                                  var waitPersonY = new Array();
                                  var waitPersonsTimeY = new Array();
                                  var baoHeY = new Array();
                                  var manYiY = new Array();
                                  for (var i = 0; i < result.Data.length; i++) {
                                      organizeX.push(result.Data[i].Name);
                                      waitPersonY.push(result.Data[i].WaitPersonsCount);
                                      waitPersonsTimeY.push(result.Data[i].WaitPersonsTime);
                                      baoHeY.push(result.Data[i].SaturationValue);
                                      manYiY.push(result.Data[i].SatisfactionValue);
                                  }
                                   
                                  // 加载图表
                                  // 等待人数图表
                                  var waitPersonsChart;
                                  // 等待时间图表
                                  var waitTimesChart;
                                  // 满意度&饱和度
                                  var saturationChart;
                                  
                                  // 等待人数图标配置
                                  var waitPersonsChartOption = {
                                      chart: {
                                          type: 'column',
                                          renderTo: 'waitPersonsContent',
                                          margin: [50, 50, 100, 80],
                                          height: dialogH
                                      },
                                      title: {
                                          text: '(当前办税排队人数)'
                                      },
                                      xAxis: {
                                          categories: organizeX,
                                          labels: {
                                              /*rotation: -45,
                                              align: 'right',*/
                                             /* style: {
                                                  fontSize: '13px',
                                                  fontFamily: 'Verdana, sans-serif'
                                              }*/
                                          }
                                      },
                                      yAxis: {
                                          min: 0,
                                          title: {
                                              text: '排队人数'
                                          },
                                          allowDecimals: false
                                      },
                                      legend: {
                                          enabled: false
                                      },
                                      tooltip: {
                                          pointFormat: '当前排队人数: <b>{point.y} 位</b>',
                                      },
                                      series: [
                                          {
                                              name: 'Population',
                                              data: waitPersonY,
                                             /* dataLabels: {
                                                  enabled: true,
                                                  rotation: -90,
                                                  color: '#FFFFFF',
                                                  align: 'right',
                                                  x: 4,
                                                  y: 10,
                                                  style: {
                                                      fontSize: '13px',
                                                      fontFamily: 'Verdana, sans-serif',
                                                      textShadow: '0 0 3px black'
                                                  }
                                              }*/
                                          }
                                      ]
                                  };

                                 var waitTimesChartOption = {
                                      chart: {
                                          type: 'column',
                                          renderTo: 'waitTimesContent',
                                          margin: [50, 50, 100, 80], 
                                          height: dialogH
                                      },
                                      title: { text: '(当前平均等候时间)' },
                                      xAxis: {
                                          categories: organizeX,
                                          labels: {
                                             /* rotation: -45,
                                              align: 'right',*/
                                             /* style: {
                                                  fontSize: '13px',
                                                  fontFamily: 'Verdana, sans-serif'
                                              }*/
                                          }
                                      },
                                      yAxis: {
                                          min: 0,
                                          title: {
                                              text: '平均等候时间（分钟）'
                                          },
                                          allowDecimals: false
                                      },
                                      legend: {
                                          enabled: false
                                      },
                                      tooltip: {
                                          pointFormat: '当前平均等候时间（分钟）: <b>{point.y} 分钟</b>',
                                      },
                                      series: [
                                          {
                                              name: 'Population',
                                              data: waitPersonsTimeY,
                                             /* dataLabels: {
                                                  enabled: true,
                                                  rotation: -90,
                                                  color: '#FFFFFF',
                                                  align: 'right',
                                                  x: 4,
                                                  y: 10,
                                                  style: {
                                                      fontSize: '13px',
                                                      fontFamily: 'Verdana, sans-serif',
                                                      textShadow: '0 0 3px black'
                                                  }
                                              }*/
                                          }
                                      ]
                                 }; 
                                  // 饱和和满意
                                 var saturationOption = {
                                     chart: {
                                         type: 'column',
                                         renderTo: 'saturationContent',
                                         margin: [50, 50, 100, 80],
                                         height: dialogH
                                     },
                                     title: { text: '(当前饱和度&满意度)' },
                                     xAxis: {
                                         categories: organizeX 
                                     },
                                     yAxis: {
                                         min: 0,
                                         title: {
                                             text: '前饱和度&满意度'
                                         }
                                      /*  ,
                                         allowDecimals: true */
                                     },
                                     tooltip: {
                                         headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                                         pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                                             '<td style="padding:0"><b>{point.y:.1f} </b></td></tr>',
                                         footerFormat: '</table>',
                                         shared: true,
                                         useHTML: true
                                     },
                                     legend: {
                                         enabled: false
                                     },
                                   /*  plotOptions: {
                                         column: { 
                                                 dataLabels: {
                                                     enabled: true, 
                                                     style: {
                                                         fontWeight: 'bold'
                                                     },
                                                     formatter: function() {
                                                         return this.y +'%';
                                                     }
                                                 }
                                         }
                                     },*/
                                     /* tooltip: {
                                         pointFormat: '当前平均等候时间（分钟）: <b>{point.y} 分钟</b>',
                                     },*/
                                     series: [
                                       {
                                           name: '饱和度',
                                           data: baoHeY 
                                       }, {
                                           name: '满意度',
                                           data: manYiY
                                         }
                                     ]
                                 };
                                   
                                  // 第一次加载数据
                                 waitPersonsChartOption.xAxis.categories = organizeX;
                                 //waitPersonsChartOption.series[0].data = waitPersonY;
                                 waitPersonsChart = new Highcharts.Chart(waitPersonsChartOption);

                                 waitTimesChartOption.xAxis.categories = organizeX;
                                 //waitTimesChartOption.series[0].data = waitPersonsTimeY;
                                 waitTimesChart = new Highcharts.Chart(waitTimesChartOption);


                                 saturationOption.xAxis.categories = organizeX;  
                                 saturationChart = new Highcharts.Chart(saturationOption);


                                 $("div[type='page']").addClass("filterBg");

                                  //定时刷新数据
                                window.setInterval(function () {
                                    $.ajax({ url: $$Settings.mapDataUrl.urlstamp(), success: function(resultObj) { 
                                        if (resultObj != undefined && resultObj != "") {
                                            if (resultObj.IsSuccess == true) {
                                                var organizeXObj = new Array();
                                                var waitPersonYObj = new Array();
                                                var waitPersonsTimeYObj = new Array();
                                                var baoHeYObj = new Array();
                                                var manYiYObj = new Array();
                                                for (var i = 0; i < resultObj.Data.length; i++) {
                                                    organizeXObj.push(resultObj.Data[i].Name);
                                                    waitPersonYObj.push(resultObj.Data[i].WaitPersonsCount);
                                                    waitPersonsTimeYObj.push(resultObj.Data[i].WaitPersonsTime);

                                                    baoHeYObj.push(resultObj.Data[i].SaturationValue);
                                                    manYiYObj.push(resultObj.Data[i].SatisfactionValue);
                                                } 
                                                waitPersonsChartOption.xAxis.categories = organizeXObj;
                                                waitPersonsChartOption.series[0].data = waitPersonYObj;
                                                waitPersonsChart = new Highcharts.Chart(waitPersonsChartOption);

                                                waitTimesChartOption.xAxis.categories = organizeXObj;
                                                waitTimesChartOption.series[0].data = waitPersonsTimeYObj;
                                                waitTimesChart = new Highcharts.Chart(waitTimesChartOption);
                                                
                                                saturationOption.xAxis.categories = organizeXObj;
                                                saturationOption.series[0].data = baoHeYObj;
                                                saturationOption.series[1].data = manYiYObj;
                                                saturationChart = new Highcharts.Chart(saturationOption);
                                                $("div[type='page']").addClass("filterBg");
                                            }
                                        }
                                    }}); 
                                    
                                }, showTime);
                                
                              }
                          } else {
                              $("div[tag='chart']").html("<p style=\"text-align: center;padding-top: 65px;font-size: 14px;\">暂无数据....</p>");
                          }
                        
                      }
                  });
 
                 $(".xubox_page").attr("style", ""); 
                 $(".xubox_main").css("background-color", "");
             });

            } else if ($$Settings.level == 4) {
                marker.icon = "/Content/cotide/img/level4.gif";
                // 服务厅级
                google.maps.event.addListener(marker, 'click', function () {
                    if ($$Settings.url != undefined && $$Settings.url != "") {
                        location.href = $$Settings.url;
                    }
                }); 
            }
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
           
            //localMapType.name =  $$Settings.txt ;
           // localMapType.alt = "显示" + $$Settings.txt + "-监控分布地图";
             
            map = new google.maps.Map(document.getElementById('map_canvas'), myOptions);
            map.mapTypes.set('locaMap', localMapType); //绑定本地地图类型 
            map.setMapTypeId('locaMap'); //指定显示本地地图
             
        }
    };
}();