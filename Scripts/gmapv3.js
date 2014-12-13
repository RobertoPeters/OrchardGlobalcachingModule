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
var overlay;

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
    var bnds = map.getBounds();
    $('#minLat').val(bnds.getSouthWest().lat());
    $('#minLon').val(bnds.getSouthWest().lng());
    $('#maxLat').val(bnds.getNorthEast().lat());
    $('#maxLon').val(bnds.getNorthEast().lng());
    if (markersAreVisible) {
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

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function getWaypointInfo(index, showinfowindow) {
    $.ajax({
        type: "POST",
        url: getWPInfoUrl,
        data: {code : markers[index].getTitle()},
        success: function (response) {
            var wpinf = '<img src="' + rootPath + 'Modules/Globalcaching/Media/WptTypes/' + response.GeocacheTypeId + '.gif" /> ';
            wpinf += '<a href="' + rootPath + 'Geocache/' + response.Code + '" target="_blank" >' + response.Code + '</a><br />';
            wpinf += '<a href="' + response.Url + '" target="_blank" >' + htmlEncode(response.Url) + '</a><br />';
            wpinf += htmlEncode(response.Name) + '<br />';
            wpinf += 'Door: <a href="http://www.geocaching.com/profile/?guid=' + response.PublicGuid + '" target="_blank" >' + htmlEncode(response.UserName) + '</a><br />';
            var d = eval('new' + response.UTCPlaceDate.replace(/\//g, ' '));
            wpinf += 'Datum: ' + d.toLocaleDateString() + '<br />';
            wpinf += 'Container: <img src="' + rootPath + 'Modules/Globalcaching/Media/container/' + response.ContainerTypeId + '.gif" /> <br />';
            wpinf += 'Moelijkheid: ' + response.Difficulty + '<br />';
            wpinf += 'Terrein:' + response.Terrain + '<br />';
            if (response.Distance != null) {
                wpinf += 'Afstand: ' + response.Distance + '<br />';
            }
            if (response.Municipality != null) {
                wpinf += 'Gemeente: ' + htmlEncode(response.Municipality) + '<br />';
            }
            wpinf += 'Favorites: ' + response.FavoritePoints + '<br />';
            infowindow[index].setContent(wpinf);
            showMarkerInfo(index, showinfowindow);
        },
        error: function (data, errorText) {
            //alert(errorText);
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
    overlay.show();
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
            overlay.hide();
        },
        error: function (data, errorText) {
            prevCall = null;
            overlay.hide();
            //alert(errorText);
        }
    });
}

function showAddress(address) {
    geocoder.geocode({ 'address': address },
        function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);
                map.setZoom(13);
            }
            else {
                alert("Kan locatie van het adres niet bepalen.");
            }
        }
    );
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


    document.getElementById('map_canvas').style.height = '800px';
    document.getElementById('map_canvas').style.width = '800px';

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
    overlay = new USGSOverlay(rootPath + 'Modules/Globalcaching/Media/gmap/reqpoints.png', map);
    google.maps.event.addListener(map, 'idle', function () {
        loadPoints();
    });
    google.maps.event.addListener(map, 'zoom_changed', function () {
        zoomChanged();
    });

}

function USGSOverlay(image, map) {

    // Now initialize all properties.
    this.image_ = image;
    this.map_ = map;
    this.showRequests_ = 0;

    // We define a property to hold the image's
    // div. We'll actually create this div
    // upon receipt of the add() method so we'll
    // leave it null for now.
    this.div_ = null;

    // Explicitly call setMap() on this overlay
    this.setMap(map);
}

USGSOverlay.prototype = new google.maps.OverlayView();

USGSOverlay.prototype.onAdd = function () {

    // Note: an overlay's receipt of onAdd() indicates that
    // the map's panes are now available for attaching
    // the overlay to the map via the DOM.

    // Create the DIV and set some basic attributes.
    var div = document.createElement('DIV');
    div.style.border = "none";
    div.style.borderWidth = "0px";
    div.style.position = "absolute";
    div.style.visibility = "hidden";

    // Create an IMG element and attach it to the DIV.
    var img = document.createElement("img");
    img.src = this.image_;
    img.style.width = "100%";
    img.style.height = "100%";
    div.appendChild(img);

    // Set the overlay's div_ property to this DIV
    this.div_ = div;

    // We add an overlay to a map via one of the map's panes.
    // We'll add this overlay to the overlayImage pane.
    var panes = this.getPanes();
    panes.overlayLayer.appendChild(div);
}

USGSOverlay.prototype.draw = function () {

    var sw = this.getProjection().fromLatLngToDivPixel(this.map_.getBounds().getSouthWest());
    var ne = this.getProjection().fromLatLngToDivPixel(this.map_.getBounds().getNorthEast());
    sw.x += 30;
    ne.y += 30;
    this.div_.style.left = sw.x + 'px';
    this.div_.style.top = ne.y + 'px';
    this.div_.style.width = '150px';
    this.div_.style.height = '50px';
}

USGSOverlay.prototype.hide = function () {
    if (this.div_) {
        this.showRequests_--;
        if (this.showRequests_ <= 0) {
            this.div_.style.visibility = "hidden";
        }
    }
}

USGSOverlay.prototype.show = function () {
    if (this.div_) {
        this.showRequests_++;
        var sw = this.getProjection().fromLatLngToDivPixel(this.map_.getBounds().getSouthWest());
        var ne = this.getProjection().fromLatLngToDivPixel(this.map_.getBounds().getNorthEast());
        sw.x += 30;
        ne.y += 30;
        this.div_.style.left = sw.x + 'px';
        this.div_.style.top = ne.y + 'px';
        this.div_.style.visibility = "visible";
    }
}

USGSOverlay.prototype.toggle = function () {
    if (this.div_) {
        if (this.div_.style.visibility == "hidden") {
            this.show();
        } else {
            this.hide();
        }
    }
}

USGSOverlay.prototype.toggleDOM = function () {
    if (this.getMap()) {
        this.setMap(null);
    } else {
        this.setMap(this.map_);
    }
}


