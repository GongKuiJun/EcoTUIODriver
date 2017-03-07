\tools\devcon\i386\devcon.exe remove ecologylab\vmultia 
cd .. 
cmd /c buildme.bat 
cd bin 
cmd /c install_driver.bat 
echo After keypress, launching testvmultia 
pause 
testvmultia /joystick 
