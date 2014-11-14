/*
 FusionCharts Export Component JavaScript Library
 Copyright FusionCharts Technologies LLP
 License Information at <http://www.fusioncharts.com/license>

 @version fusioncharts/1.0.4-release
*/
if (typeof FusionCharts === "undefined") throw alert("FusionChartsExport: object::FusionCharts was not found. Verify script inclusions.", 10901271400), "FusionChartsExportComponentFatalError::10901271400"; var _FCEO = function () { _FCEO.initializeComponent(this, arguments) }, FusionChartsExportObject = _FCEO; if (FusionCharts.version && FusionCharts.version[0] >= 3) _FCEO.__global = FusionCharts(["private", "LegacyExportComponent"]); _FCEO.version = [1, 0, 3, "release", 362];
_FCEO.defaultParameters = { id: void 0, swfLocation: void 0, componentAttributes: { width: "120", height: "40", bgColor: "", strictTrigger: "0" }, exportAttributes: {}, sourceCharts: [], debugMode: !1, outerHTML: "" }; _FCEO.checkFlashVersion = !1; _FCEO.debugMode = !1; var FC_ExportComponentReady = function (a) { setTimeout(function () { _FCEO.flushRack(a) }, 0); return !0 }, FC_ExportDataReady = function (a) { setTimeout(function () { return _FCEO.relayStream(a) }, 0) }; _FCEO.prototype.Render = function (a) { return this.render(a) };
_FCEO.prototype.BeginExport = function () { return _FCEO.batchExport(this, this.sourceCharts) }; _FCEO.prototype.BeginExportAll = function () { return _FCEO.batchExport(this, _FCEO.probeCharts()) }; _FCEO.prototype.GetOuterHTML = function () { this.update(); return this.outerHTML }; _FCEO.prototype.render = function (a) { a = _FCEO.L.get(a); if (!a || !_FCEO.L.isObj(a.innerHTML, "string")) return this.trace(_FCEO.R.errContainer, 10902091233), !1; this.update(); try { this.srcObj.render(a) } catch (b) { this.trace(_FCEO.R.errFusion + "\n" + b, 10902101448) } return !0 };
_FCEO.prototype.update = function () { _FCEO.updateComponent(this) }; _FCEO.prototype.trace = function (a, b) { _FCEO.trace(a, b, this.debugMode) }; _FCEO.prototype.toString = function () { return "[object " + _FCEO.R.resSig + " #" + this.id + " ]" }; _FCEO.streamRack = {}; _FCEO.flags = { c: 0 }; _FCEO.playerVersion = { major: 0, minor: 0, rev: 0 };
try { _FCEO.playerVersion = typeof _FCEO.__global === "undefined" ? infosoftglobal.FusionChartsUtil.getPlayerVersion() : _FCEO.__global.swfobject.getFlashPlayerVersion() } catch (err$$1) { _FCEO.trace("FusionChartsExport: object::Flash Player DetectionError.\n" + err$$1, 10902182119) }
_FCEO.relayStream = function (a) {
    var b = _FCEO.probeHandler(a); b == null && _FCEO.trace(_FCEO.R.errStream, 10902041759); if (!_FCEO.L.isObj(a.parameters.exportParameters)) a.parameters.exportParameters = {}; if (!_FCEO.L.isObj(a.parameters.exportParameters.triggerOrdinal, "number")) a.parameters.exportParameters.triggerOrdinal = _FCEO.flags.c++; _FCEO.__global && _FCEO.__global.raiseEvent("ExportReady", a.parameters, _FCEO.__global.core.items[a.meta.DOMId] || _FCEO.__global.core); setTimeout(function () {
        typeof window.FC_ExportReady ===
        "function" && FC_ExportReady(a.meta.DOMId)
    }, 0); if (_FCEO.probeComponent(b)) return _FCEO.pushStream(b, [a]), !0; _FCEO.streamRack[b] || (_FCEO.streamRack[b] = []); _FCEO.streamRack[b].push(a); return !0
}; _FCEO.flushRack = function (a) { a = _FCEO.probeHandler(a); a == null && _FCEO.trace(_FCEO.R.errComponent, 10902041939); _FCEO.probeComponent(a) || _FCEO.trace(_FCEO.R.errComponent, 10902041917); _FCEO.streamRack[a] && (_FCEO.pushStream(a, _FCEO.streamRack[a]), _FCEO.streamRack[a] = []) }; _FCEO.pushStream = function (a, b) { return _FCEO.L.get(a).pushData(b) };
_FCEO.probeComponent = function (a) { if (!a) return !1; return (a = _FCEO.L.get(a)) ? a.isReady != null && a.isReady() == !0 : !1 }; _FCEO.probeHandler = function (a) { if (!a) return null; return _FCEO.L.isString(a) ? a : a.parameters != null && _FCEO.L.isString(a.parameters.exportHandler) ? a.parameters.exportHandler : null };
_FCEO.probeCharts = function () { var a, b, d = []; b = _FCEO.L.tags("object"); for (var e = 0; e < b.length; e++) a = b[e], _FCEO.probeChart(a) && d.push(a); b = _FCEO.L.tags("embed"); for (e = 0; e < b.length; e++) a = b[e], a.parentNode != void 0 && a.parentNode.tagName == "object" || _FCEO.probeChart(a) && d.push(a); return d }; _FCEO.probeChart = function (a) { if (a.signature == void 0) return !1; a = a.signature(); return _FCEO.L.isString(a) && a.indexOf(_FCEO.R.resChartSig) == 0 };
_FCEO.initializeComponent = function (a, b) {
    (!b || !b.length || b.length < 1) && _FCEO.trace(_FCEO.R.errArgs, 10902061902); var d = !_FCEO.L.isString(b[0]) && b.length == 1; !d && b.length < 2 && _FCEO.trace(_FCEO.R.errArgs, 10902091732); var e = 0, f; for (f in _FCEO.defaultParameters) a[f] = (d ? b[0][f] : b[e++]) || _FCEO.defaultParameters[f]; (!_FCEO.L.isString(a.id) || !_FCEO.L.isString(a.swfLocation)) && a.trace(_FCEO.R.errArgs, 10902091820); _FCEO.playerVersion.major < 10 && (_FCEO.checkFlashVersion && !a.debugMode && !_FCEO.debugMode && alert(_FCEO.R.msgNoPlayer),
    a.trace(_FCEO.R.errPlayer, 10902182131)); a.srcObj = new FusionCharts(a.swfLocation, a.id, 0, 0, a.debugMode, _FCEO.__global && !_FCEO.__global.jsBatchExportSupported && { renderer: "flash" })
};
_FCEO.updateComponent = function (a) {
    a.srcObj || a.trace(_FCEO.R.errFusion, 10902220400); _FCEO.L.sync(a.componentAttributes, a.srcObj.variables || a.srcObj.flashVars); var b, d = function (b) { return !_FCEO.L.isObj((a.srcObj.variables || a.srcObj.flashVars)[b], "undefined") ? (a.srcObj.variables || a.srcObj.flashVars)[b] : _FCEO.defaultParameters.componentAttributes[b] }; try {
        (a.srcObj.variables || a.srcObj.flashVars).generator = _FCEO.R.resSig; b = d("width"); if (b != void 0) _FCEO.__global ? (a.srcObj.width = b.toString(), a.srcObj.flashVars.width =
        b.toString()) : (a.srcObj.setAttribute("width", b), a.srcObj.addVariable("width", b)); b = d("height"); if (b != void 0) _FCEO.__global ? (a.srcObj.height = b.toString(), a.srcObj.flashVars.height = b.toString()) : (a.srcObj.setAttribute("height", b), a.srcObj.addVariable("height", b)); b = d("bgColor"); b != void 0 && (a.srcObj.params ? a.srcObj.params.bgColor = b : a.srcObj.addParam("bgColor", b)); a.outerHTML = a.srcObj.getSWFHTML()
    } catch (e) { this.trace(_FCEO.R.errFusion + "" + e, 10902091318) }
};
_FCEO.batchExport = function (a, b) {
    if (!(b instanceof Array)) return a.trace(_FCEO.R.errArgs, 10902091853), []; var d = [], e = _FCEO.flags.c; _FCEO.flags.c += b.length; for (var f = parseInt(a.componentAttributes.strictTrigger || 0) == 0 ? function (a) { return a } : function (b) { b.handlerId = a.id; return b }, h = 0; h < b.length; h++) {
        var g = _FCEO.L.get(b[h]); if (g == null) a.trace(_FCEO.R.errFusion, 10902092026); else {
            if (!_FCEO.L.isObj(a.exportAttributes.exportParameters)) a.exportAttributes.exportParameters = {}; a.exportAttributes.exportParameters.triggerOrdinal =
            e++; a.exportAttributes = f(a.exportAttributes); g && g.exportChart ? (g.exportChart(a.exportAttributes), d.push(g.id)) : a.trace(_FCEO.R.errFusion + "\nRef: " + g.id, 10902092036)
        }
    } return d
}; _FCEO.toString = function () { return "[object " + _FCEO.R.resSig + "]" }; _FCEO.trace = function (a, b, d) { (d || _FCEO.debugMode) && _FCEO.L.raise(a, b) };
_FCEO.R = {
    errStream: "Inbound stream missing or stream integrity failure.", errComponent: "Export component missing or authentication failure.", errArgs: "Invalid arguments or parameters out of bounds exception.", errContainer: "Invalid container id or HTMLNode missing from DOM.", errFusion: "Error with internal FusionCharts object. Review parameters.", errPlayer: "Incompatible Flash player or Flash player not installed. Flash Player 10 (or above) is needed for Export Component.", msgNoPlayer: "This page contains elements that require the latest version of Flash Player plugin.",
    resSig: "FusionChartsExportComponent", resChartSig: "FusionCharts/"
};
_FCEO.L = {
    isIE: navigator.appName == "Microsoft Internet Explorer", isFF: document.getElementById && !document.all, tags: function (a, b) { if (!_FCEO.L.isString(a)) return []; return (b || document).getElementsByTagName(a) }, get: function (a, b) { if (!_FCEO.L.isString(a)) return a; return (b || document).getElementById(a) }, getNew: function (a) { for (var b = document.createElement(a), d, e = 1; c < arguments.length; e++) d = arguments[e].split("="), b.setAttribute(d[0], d[1]); return b }, sync: function (a, b) { for (var d in a) b[d] = a[d] }, isString: function (a) {
        return typeof a ==
        "string"
    }, isObj: function (a, b) { return typeof a == (b ? b : "object") }, raise: function (a, b, d) { a = "[object " + _FCEO.R.resSig + (d ? "# " + d : "") + "]\n\nError " + b + ".\n" + a + "\n\nRefer documentation."; alert(a); throw a; }
};