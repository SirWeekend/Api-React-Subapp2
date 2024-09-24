// Initialize the map and set its view to the default location
var map = L.map('map').setView([51.505, -0.09], 13); // Default coordinates

// Add OpenStreetMap tiles to the map
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

// Function to load existing points and display them on the map
function loadPoints() {
    fetch('/api/pinpoints')
        .then(response => response.json())
        .then(data => {
            data.forEach(point => {
                L.marker([point.latitude, point.longitude])
                    .addTo(map)
                    .bindPopup(`<b>${point.name}</b><br>${point.description}`)
                    .openPopup();
            });
        })
        .catch(error => console.error('Error loading points:', error));
}

// Call the function to load points when the map is ready
loadPoints();

// Event listener for map clicks
map.on('click', function (e) {
    const lat = e.latlng.lat;
    const lng = e.latlng.lng;

    // Create a popup form at the clicked location
    const popup = L.popup()
        .setLatLng(e.latlng)
        .setContent(`
            <form id="popupForm">
                <label for="name">Name:</label><br>
                <input type="text" id="name" name="name" required><br>
                <label for="description">Description:</label><br>
                <textarea id="description" name="description" required></textarea><br>
                <button type="submit">Create Pinpoint</button>
            </form>
        `)
        .openOn(map);

    // Handle form submission within the popup
    document.getElementById('popupForm').addEventListener('submit', function (event) {
        event.preventDefault();

        const name = document.getElementById('name').value;
        const description = document.getElementById('description').value;

        const pinpoint = {
            name: name,
            description: description,
            latitude: lat,
            longitude: lng
        };

        // Send the data to the API to create a new pinpoint
        fetch('/api/pinpoints', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(pinpoint)
        })
        .then(response => {
            if (response.ok) {
                alert('Pinpoint created successfully!');
                // Reload the points to show the new one
                loadPoints();
                map.closePopup();  // Close the popup after successful submission
            } else {
                alert('Error creating pinpoint. Please try again.');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Error creating pinpoint. Please try again.');
        });
    });
});


