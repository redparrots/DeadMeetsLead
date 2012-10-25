Dead Meets Lead
===============
Dead Meets Lead is a top down, single player, zombie shooter for PC/Windows developed by former studio Keldyn Interactive during 2010 - 2011. The game, engine and editor are all written in C# using DirectX 9 for graphics.

Getting started
---------------

### Prerequisites:
* Visual Studio 2010 or 2012 (works with Express and upwards, for instance [Visual Studio Express 2012 for Windows Desktop](http://www.microsoft.com/visualstudio/eng/downloads#d-2012-express)).
* [SlimDX developer SDK January 2012](http://slimdx.org/download.php)
* [DirectX SDK Jun 2010](http://www.microsoft.com/en-us/download/details.aspx?id=6812)

### Building
* Set up PATH to point to the directory where the fxc.exe program resides (C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64). May require a reboot.

### Running
In order to run most project you need to set their working directory to WorkingDirectory (which is located in the root of the DeadMeetsLead repository). Right click a project, select Properties and then go to the Debug tab and there you can set the working directory.

Tips & Tricks
-------------
* The test projects (located in the Test folder) are a pretty good start to getting to know the code. Most of them are fairly small and show how a specific part of the code is used.
* There's a bunch of command line options available for the client, for instance -DisplaySettingsForm=true which will display a form with different options configurable. Take a look in Client/Settings.cs for all available options.
* We used a modified version of Chad Vernons cvXporter exporter, which can be found in the Tools directory.
* The graphics system is built around the idea of having one part that describes what should be shown (MetaModels & Entity/Scene stuff) and one part that actually renders.

License
-------
Copyright (C) 2010 - 2012 Keldyn Interactive

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.