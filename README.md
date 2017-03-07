Notes:
======

- Stripped version to one service
- Upgraded VS solution to VS2015 
- compatible with Win10


EcoTUIODriver
=============

Driver to convert tuio touch events into windows touch events. Supports both Windows 7 & 8. Started as GSoC 2012 project.
EcoTUIODriver uses the [vmulti](http://code.google.com/p/vmulti/) project.

Original Authors: Rajat Goyal(gyl.rajat@gmail.com) and Bill Hamilton(luin.uial@gmail.com)
Liscense: [LGPL](https://github.com/ecologylab/EcoTUIODriver/blob/master/LISCENSE.txt)

##INSTALL 
1. Download the [source](https://github.com/almighty-bungholio/EcoTUIODriver/archive/master.zip).

2. Run the installer (EcoTUIODriver.exe in top level directory).

##Configuration Utility 

If you used the installer, the configuration utility will automatically be added in your startmenu and desktop. Otherwise you can manually locate it at $INSALL_DIR/executables/Configuration/Configuration_Utility.exe

Start the service to convert TUIO events to windows touch events. 

Sensors may be mapped to displays by hitting the "ASSIGN DISPLAYS TO SENSORS" button, which runs a native windows mapping utility.

EcoTUIODriver in action:
========================
EcoTUIODriver is being used with CCV, Touch Frames, LeapMotion and Wii. 

[ZeroTouch Windows 8](http://www.youtube.com/watch?v=VnawSjc28CI)

[LEAP Motion demo: Visualizer, Windows 8](http://www.youtube.com/watch?feature=fvwp&v=5lFZeej-PvI&NR=1)

["The Leap" Windows 8 touchless tests](http://www.youtube.com/watch?v=gcPFoBn_O0g)

[Windows 8 + CCV 1.5 + multitouch](http://www.youtube.com/watch?v=MjLbiNKgtKs)

[Touchmote](http://touchmote.codeplex.com/)
