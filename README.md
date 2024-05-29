# VKB-Mobiflight-Definer
Utility for creating Mobiflight joystick definitions from VKB devices

This is a simple console application to create a MobiFlight joystick definition for connected VKB devices.

Currently, it can create entries for buttons and LEDs on all NXT and STECS modules. Grip support is still limited.

Usage: Have VKBDevCfg ready and be familiar with both the "Global/External Devices" and "Test/Buttons" views. You will need the program to determine both the LED base index and the first button number for each module.

Note: The tool assumes that the module configuration has only had minimal changes from Autoconfig in VKBDevCfg. If you have performed more involved changes, like changing button numbering, manual editing of the JSON file may be necessary.

After completing the instructions provided by the tool, there will be a JSON file in the output folder. Copy this file to your Mobiflight Joysticks folder.