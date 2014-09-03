function contains(a, obj) {
    for (var i = 0; i < a.length; i++) {
        if (a[i] == obj) {
            return i;
        }
    }
    return -1;
}

var map;
var geocoder;

var rootPath;
var filter;
var getGeocachesUrl;
var getWPInfoUrl;
var wpInfoNotAvailable = "Info wordt opgehaald...";

var markers = [];
var visibleMarkers = [];
var markerIds = [];
var minZoomLevelMarkersVisible = 8;
var markersAreVisible = true;
var infowindow = [];
var markerInfoRequested = null;
var markerClusterer = null;
var clusterOptions = { gridSize: 40, maxZoom: 11 };

var iconTable = {};


function markerThroughFilter(id, ct) {
    var hid;
    //todo
    hid = false;
    return !hid;
}

function markerFilterChanged() {
    if (markerClusterer != null) {
        markerClusterer.clearMarkers();
    }
    visibleMarkers.length = 0;
    for (var i = 0; i < markers.length; i++) {
        markers[i].setVisible(markersAreVisible && markerThroughFilter(markers[i].getTitle(), markers[i].getIcon()));
        if (markers[i].getVisible()) {
            visibleMarkers.push(markers[i]);
        }
    }
    markerClusterer = new MarkerClusterer(map, visibleMarkers, clusterOptions);
}

function zoomChanged() {
    var isvis = (map.getZoom() >= minZoomLevelMarkersVisible);
    if (isvis != markersAreVisible) {
        markersAreVisible = isvis;
        markerFilterChanged();
    }
}

function loadPoints() {
    if (markersAreVisible) {
        var bnds = map.getBounds();
        getPoints(bnds.getSouthWest().lat(), bnds.getSouthWest().lng(), bnds.getNorthEast().lat(), bnds.getNorthEast().lng(), map.getZoom());
    }
}

function showMarkerInfo(index, showinfowindow) {
    if (showinfowindow) {
        for (var i = 0; i < infowindow.length; i++) {
            infowindow[i].close();
        }
    }
    if (infowindow[index].getContent() != wpInfoNotAvailable) {
        if (showinfowindow) {
            infowindow[index].open(map, markers[index]);
        }
        else {
            if (this.document.getElementById("WPInfo") != null) {
                this.document.getElementById("WPInfo").innerHTML = infowindow[index].getContent();
            }
        }
    }
    else {
        if (showinfowindow) {
            infowindow[index].open(map, markers[index]);
        }
        else {
            if (this.document.getElementById("WPInfo") != null) {
                this.document.getElementById("WPInfo").innerHTML = wpInfoNotAvailable;
            }
        }
        getWaypointInfo(index, showinfowindow);
    }
}

function getWaypointInfo(index, showinfowindow) {
    $.ajax({
        type: "POST",
        url: getWPInfoUrl,
        data: {code : markers[index].getTitle()},
        success: function (response) {
            var wpinf = '' + response.Code;
            infowindow[index].setContent(wpinf);
            showMarkerInfo(index, showinfowindow);
        },
        error: function (data, errorText) {
            alert(errorText);
        }
    });
}

var prevCall;
function getPoints(minLat, minLon, maxLat, maxLon, zooml) {
    if (prevCall != null)
    {
        prevCall.abort();
        prevCall = null;
    }
    prevCall = $.ajax({
        type: "POST",
        url: getGeocachesUrl + '/' + minLat + '/' + minLon + '/' + maxLat + '/' + maxLon + '/' + zooml,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(filter),
        success: function (response) {
            prevCall = null;
            if (markerClusterer != null) {
                markerClusterer.clearMarkers();
            }
            for (var i = 0; i < response.Items.length; i++) {
                index = contains(markerIds, response.Items[i].c);
                if (index < 0) {
                    addMarker(response.Items[i]);
                }
                else {
                    //overwrite personalized values (might not be available before)
                    if (response.Items[i].i == "f") {
                        markers[index].setIcon(iconTable['f']);
                    }
                    else if (response.Items[i].i == "o") {
                        markers[index].setIcon(iconTable['o']);
                    }
                }
            }
            markerClusterer = new MarkerClusterer(map, visibleMarkers, clusterOptions);
            if (markerInfoRequested != null) {
                for (var i = 0; i < markerIds.length; i++) {
                    if (markerIds[i] == markerInfoRequested) {
                        showMarkerInfo(i, true);
                        break;
                    }
                }
                markerInfoRequested = null;
            }
        },
        error: function (data, errorText) {
            prevCall = null;
            alert(errorText);
        }
    });
}


function addMarker(wp) {
    markerIds.push(wp.c);
    var latlng = new google.maps.LatLng(wp.a, wp.o);
    var ic = iconTable[wp.i];
    if (ic == null) {
        ic = iconTable[0];
    }
    var marker = new google.maps.Marker({ 'title': wp.c, icon: ic, position: latlng, visible: markersAreVisible && markerThroughFilter(wp.c, ic) });
    var iw = new google.maps.InfoWindow();
    iw.setContent(wpInfoNotAvailable);
    infowindow.push(iw);
    markers.push(marker);
    var index = markers.length - 1;
    google.maps.event.addListener(marker, 'mouseover', function () {
        showMarkerInfo(index, false);
    });
    google.maps.event.addListener(marker, 'click', function () {
        showMarkerInfo(index, true);
    });
    if (marker.getVisible()) {
        visibleMarkers.push(marker);
    }
}

function initializeMap(centerLat, centerLon, zoom, rPath, gcFilter, getgcUrl, getwpUrl) {
    rootPath = rPath;
    filter = gcFilter;
    getGeocachesUrl = getgcUrl;
    getWPInfoUrl = getwpUrl;

    iconTable[0] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/0.png");
    iconTable[2] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/2.png");
    iconTable[3] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/3.png");
    iconTable[5] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/5.png");
    iconTable[6] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/6.png");
    iconTable[8] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/8.png");
    iconTable[11] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/11.png");
    iconTable[12] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/12.png");
    iconTable[137] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/137.png");
    iconTable['f'] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/found.png");
    iconTable['o'] = new google.maps.MarkerImage(rootPath + "Modules/Globalcaching/Media/WptTypes/map/myown.png");


    document.getElementById('map_canvas').style.height = '700px';
    document.getElementById('map_canvas').style.width = '700px';

    var mapTypeIds = [];
    for (var type in google.maps.MapTypeId) {
        mapTypeIds.push(google.maps.MapTypeId[type]);
    }
    mapTypeIds.push("OSM");

    var latlng = new google.maps.LatLng(centerLat, centerLon);
    var myOptions = {
        zoom: zoom,
        center: latlng,
        scaleControl: true,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapTypeControlOptions: { mapTypeIds: mapTypeIds }
    };
    map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);

    map.mapTypes.set("OSM", new google.maps.ImageMapType({
        getTileUrl: function (coord, zoom) {
            return "http://tile.openstreetmap.org/" + zoom + "/" + coord.x + "/" + coord.y + ".png";
        },
        tileSize: new google.maps.Size(256, 256),
        name: "OpenStreetMap",
        maxZoom: 18
    }));

    geocoder = new google.maps.Geocoder();

    google.maps.event.addListener(map, 'idle', function () {
        loadPoints();
    });
    google.maps.event.addListener(map, 'zoom_changed', function () {
        zoomChanged();
    });

}

