# Unity Assetto Corsa data visualization

![Funny cat](assets/2025-05-2714-50-38_Clip_smallest-ezgif.com-video-to-gif-converter.gif)

This project enables you to visualize live data from Assetto Corsa using shared memory.
Data from the shared memory is exposed from a socket-io thats running with flask.

The data is read in unity, and displayed in accordingly, with a simple UI.
The unity app exposes an api to control camera behaviour, with example requests in the /Backend/camera_movement_mazda.py file 


Map models are from game files, and the E30 model is a free model from sketchfab.