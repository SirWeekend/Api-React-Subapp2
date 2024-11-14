// Handle form submission for creating a new pinpoint
document.getElementById('pinpointForm').addEventListener('submit', function (event) {
    event.preventDefault();

    // Collect form data
    const name = document.getElementById('name').value;
    const description = document.getElementById('description').value;
    const latitude = parseFloat(document.getElementById('latitude').value);
    const longitude = parseFloat(document.getElementById('longitude').value);

    // Construct the payload
    const pinpoint = {
        name: name,
        description: description,
        latitude: latitude,
        longitude: longitude
    };

    // Send the data to the API using fetch
    fetch('/api/pinpoint', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(pinpoint)
    })
    .then(response => {
        if (response.ok) {
            alert('Pinpoint created successfully!');
            // Optionally, reload the page or add the new pinpoint to the map dynamically
            loadPoints();  // Re-load the points to show the newly created one
        } else {
            alert('Error creating pinpoint. Please try again.');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error creating pinpoint. Please try again.');
    });
});
