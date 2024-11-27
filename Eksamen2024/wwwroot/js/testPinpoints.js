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
                    <button onclick="editPinpoint(${point.pinpointId})">Edit Pinpoint</button>
                    <button onclick="toggleComments(${point.pinpointId})">View Comments</button>
                    <div id="comments-container-${point.pinpointId}" style="display: none; margin-top: 10px;">
                        <div id="comments-${point.pinpointId}">Loading comments...</div>
                        <textarea id="comment-input-${point.pinpointId}" placeholder="Write your comment here..."></textarea><br>
                        <button onclick="submitComment(${point.pinpointId})">Post Comment</button>
                    </div>
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

function loadComments(pinpointId) {
    fetch(`/api/pinpoint/${pinpointId}/comments`)
        .then(response => {
            if (!response.ok) {
                console.error(`Failed to fetch comments: ${response.statusText}`);
                throw new Error(`API returned status ${response.status}`);
            }
            return response.json();
        })
        .then(comments => {
            // Normalize response if it has $values
            const normalizedComments = comments.$values || comments;

            if (!Array.isArray(normalizedComments)) {
                console.error("Invalid API response:", comments);
                throw new Error("API response is not an array");
            }

            const commentsDiv = document.getElementById(`comments-${pinpointId}`);
            commentsDiv.innerHTML = ''; // Clear existing comments

            if (normalizedComments.length === 0) {
                commentsDiv.textContent = 'No comments yet.';
            } else {
                normalizedComments.forEach(comment => {
                    const commentDiv = document.createElement('div');
                    commentDiv.textContent = `${comment.username}: ${comment.text}`;
                    commentsDiv.appendChild(commentDiv);
                });
            }
        })
        .catch(error => {
            console.error("Error loading comments:", error);
            const commentsDiv = document.getElementById(`comments-${pinpointId}`);
            commentsDiv.textContent = 'Failed to load comments.';
        });
}

    

// Function to submit a new comment
function submitComment(pinpointId) {
    const commentText = document.getElementById(`comment-input-${pinpointId}`).value;

    if (!commentText.trim()) {
        alert("Comment cannot be empty.");
        return;
    }

    fetch(`/api/pinpoint/${pinpointId}/comments`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(commentText), // Send raw string instead of object
    })
        .then(response => {
            if (response.ok) {
                alert("Comment added successfully!");
                loadComments(pinpointId); // Reload comments after posting
            } else {
                return response.text().then(text => {
                    throw new Error(text);
                });
            }
        })
        .catch(error => {
            console.error("Error submitting comment:", error);
            alert("Failed to add comment.");
        });
}


function toggleComments(pinpointId) {
    const commentsContainer = document.getElementById(`comments-container-${pinpointId}`);

    // Toggle visibility
    if (commentsContainer.style.display === "none") {
        commentsContainer.style.display = "block";
        loadComments(pinpointId); // Load comments when expanding the section
    } else {
        commentsContainer.style.display = "none";
    }
}
window.toggleComments = toggleComments;
console.log("toggleComments assigned to window");
window.submitComment = submitComment;
console.log("submitComment assigned to window");
window.loadComments = loadComments;
console.log("loadComments assigned to window");

function editPinpoint(pinpointId) {
    const newName = prompt("Enter new name for the pinpoint:");
    const newDescription = prompt("Enter new description for the pinpoint:");

    if (!newName || !newDescription) {
        alert("Name and description cannot be empty!");
        return;
    }

    const updatedPinpoint = {
        pinpointId, // Make sure the ID matches
        name: newName,
        description: newDescription,
        // Optional: Add other properties if you allow editing them
    };

    fetch(`/api/pinpoint/${pinpointId}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedPinpoint),
    })
        .then(response => {
            if (response.ok) {
                alert("Pinpoint updated successfully!");
                loadPoints(); // Reload points to reflect changes
            } else {
                return response.text().then(text => {
                    throw new Error(text);
                });
            }
        })
        .catch(error => {
            console.error("Error updating pinpoint:", error);
            alert("Failed to update pinpoint.");
        });
}




    


