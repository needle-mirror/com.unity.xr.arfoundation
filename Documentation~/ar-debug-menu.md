---
uid: arfoundation-debug-menu
---

# AR Foundation Debug Menu

The AR Foundation Debug Menu is a menu used to surface tracking information and visualize trackables. Menu functionality can be attached to any scene with an `ARSession`, `XROrigin`, and `Camera` using the `ARDebugMenu` script.

Currently, the menu provides information about the following:
- XR Origin
- FPS
- Tracking Mode
- Planes
- Configurations
- Anchors
- Point Clouds

# Setting up the Menu

In order to create an out-of-the-box fully configured debug menu, you should right-click anywhere in the scene inspector and select **XR** &gt; **AR Debug Menu**.

Right-click to open Scene Inspector Menu       |  Select XR
:-------------------------:|:-------------------------:
![Scene Inspector Menu](images/ar-debug-menu-xr.png "Select XR")  |  ![XR Submenu](images/ar-debug-menu-scene-inspector.png "Select ARDebugMenu")

The AR Debug Menu appears as a toolbar that is anchored either to the left or bottom side of the screen.
