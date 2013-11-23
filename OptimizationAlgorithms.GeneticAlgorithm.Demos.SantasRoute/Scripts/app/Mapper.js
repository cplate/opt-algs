var SantasRoute = SantasRoute || {};

// Class to encapsulate mapping provider-specific code (to some extent anyway)
SantasRoute.Mapper = (function () {
    function Mapper(hostDivName, centerLat, centerLng, initialZoom) {

        this.mqMap = new MQA.TileMap({
            elt: document.getElementById(hostDivName),
            zoom: initialZoom,
            latLng: new MQA.LatLng(centerLat, centerLng),
            mtype: 'osm'
        });

        this.addViewControl();
        this.addZoomControl();

        var ctx = this;
        MQA.withModule('mousewheel', function () {
            ctx.mqMap.enableMouseWheelZoom();
        });
        MQA.withModule('shapes', function () { }); // load this so its ready later, dont like how api does this
    }
    Mapper.prototype.addViewControl = function () {
        var ctx = this;
        MQA.withModule('viewoptions', function () {
            ctx.mqMap.addControl(new MQA.ViewOptions(), new MQA.MapCornerPlacement(MQA.MapCorner.TOP_RIGHT, new MQA.Size(0, 0)));
        });
    };
    Mapper.prototype.addZoomControl = function () {
        var ctx = this;
        MQA.withModule('largezoom', function () {
            ctx.mqMap.addControl(new MQA.LargeZoom(), new MQA.MapCornerPlacement(MQA.MapCorner.TOP_LEFT, new MQA.Size(0, 0)));
        });
    };
    Mapper.prototype.setSize = function (width, height) {
        this.mqMap.setSize(new MQA.Size(width, height));
    };
    Mapper.prototype.setCenterAndZoom = function (lat, lng, zoom) {
        this.mqMap.setCenter(new MQA.LatLng(lat, lng), zoom);
    };
    Mapper.prototype.bestFit = function () {
        this.mqMap.bestFit(false, 1, 18);
    };
    Mapper.prototype.createPoint = function (lat, lng, title, content) {
        var poi = new MQA.Poi({ lat: lat, lng: lng });
        poi.infoTitleHTML = title;
        poi.infoContentHTML = content;
        poi.setRolloverContent(poi.infoTitleHTML);
        return poi;
    };
    Mapper.prototype.createOverlay = function (latLngs, borderWidth, borderColor, colorAlpha) {
        var singleCoordArray = [];
        for (var idx = 0; idx < latLngs.length; idx++) {
            singleCoordArray.push(latLngs[idx].lat);
            singleCoordArray.push(latLngs[idx].lng);
        }
        var overlay = new MQA.LineOverlay();
        overlay.setShapePoints(singleCoordArray);
        overlay.borderWidth = borderWidth;
        overlay.color = borderColor;
        overlay.colorAlpha = colorAlpha;
        return overlay;
    };
    Mapper.prototype.addShapes = function (shapeGroupName, shapes) {
        var shapeColl = new MQA.ShapeCollection();
        shapeColl.setName(shapeGroupName);
        for (var idx = 0; idx < shapes.length; idx++) {
            var s = shapes[idx];
            shapeColl.add(s);
        }
        this.mqMap.addShapeCollection(shapeColl);
    };
    Mapper.prototype.removeShapes = function (shapeGroupName) {
        this.mqMap.removeShapeCollection(shapeGroupName);
    };
    Mapper.prototype.clearShapes = function () {
        this.mqMap.removeAllShapes();
    };
    return Mapper;
}());
