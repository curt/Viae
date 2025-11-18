// Initialize the map
const map = L.map('map').setView([0, 0], 2);

// Add OpenStreetMap tiles
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
    maxZoom: 18
}).addTo(map);

// Define custom red marker icon
const redIcon = L.icon({
    iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
    shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41]
});

// Fetch GeoJSON data and add to map
// The `?geojson` is to prevent browsers from replacing cached HTML with JSON.
fetch('?geojson', {
    headers: {
        'Accept': 'application/geo+json'
    }
})
    .then(response => response.json())
    .then(data => {
        const geoJsonLayer = L.geoJSON(data, {
            pointToLayer: function (feature, latlng) {
                // Use red marker for center features
                if (feature.properties.center) {
                    return L.marker(latlng, { icon: redIcon, zIndexOffset: 1000 });
                }
                // Return default blue marker for non-center features
                return L.marker(latlng);
            },
            onEachFeature: function (feature, layer) {
                const props = feature.properties;
                let popupContent = `<strong>${props.name}</strong>`;
                popupContent += `<br><a href="${props.path}">Details</a>`;
                layer.bindPopup(popupContent);
            }
        }).addTo(map);

        // Fit map bounds to show all markers
        if (data.features.length > 0) {
            map.fitBounds(geoJsonLayer.getBounds(), { padding: [50, 50] });
        }
    })
    .catch(error => console.error('Error loading GeoJSON:', error));
