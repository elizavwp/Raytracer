Bart Broekman: 5657679
Eliza van Wulfften Palthe: 5617561

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Camera controls:
- W/S: Increase/Decrease Z
- A/D: Increase/Decrease X
- Space/Left Shift: Increase/Decrease Y

- Q/E: Rotate Left/Right
- R/F: Rotate Up/Down

- Numpad +/-: Increase Decrease FOV

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Extra assignments:
- Triangles
- A HDR textured skydome: The implementation is at the bottom of the ShootRay() method, if there is no
			  intersection found with a primitive we draw the skybox.
- Refraction and absorption: The implementation can be found in the ShootRay() method, between the
			     if-statement for a reflective surface and a diffuse surface, just as
			     with a reflective surface you give a value between 0...1 for the
			     percantage of how dielectric a primitive is.

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Material used:
- Slides from the lectures
- The code for intersections with triangles: https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm

