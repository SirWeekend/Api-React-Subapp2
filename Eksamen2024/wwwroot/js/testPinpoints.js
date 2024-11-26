// Initialize the map and set its view to the default location
//var map = L.map('map').setView([59.9139, 10.7522], 13); // Coordinates for Oslo


// Add OpenStreetMap tiles to the map
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

// Function to load existing points and display them on the map
function loadPoints() {
    fetch('/api/pinpoint')
        .then(response => response.json())
        .then(data => {
            const points = data.$values || []; // Use $values if it's there
            points.forEach(point => {
                // Construct the popup content dynamically with new attributes
                const popupContent = `
                    <b>${point.name}</b><br>
                    <p>${point.description || 'No description available.'}</p>
                    ${point.imageUrl ? `<img src="${point.imageUrl}" alt="${point.name}" style="width:100px;height:auto;">` : ''}
                    <p><i>Username: ${point.username || 'Anonymous'}</i></p>
                `;
                
                L.marker([point.latitude, point.longitude])
                    .addTo(map)
                    .bindPopup(popupContent)
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
        console.log("Create Pinpoint button clicked"); // Debugging

        const name = document.getElementById('name').value;
        const description = document.getElementById('description').value || null;
        //const imageUrl = document.getElementById('imageUrl').value || null; //Can be null
        //const userId = document.getElementById('userId').value || null; //Can be null

        const pinpoint = {
            name: name,
            description: description,
            latitude: lat,
            longitude: lng,
            //imageUrl: imageUrl,
            //userId: userId
        };

        console.log("Payload to be sent:", pinpoint); // Log for debugging

        // Send the data to the API to create a new pinpoint
        fetch('/api/pinpoint', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(pinpoint)
            //credentials: "include", // Include cookies in the request
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
    


