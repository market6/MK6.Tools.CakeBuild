# nodejs tool nupkg

The goal of this script is to make a nupkg which contains node.exe and a custom npm.cmd for use as a CakeBuild tool. This will allow the build server to resolve node version easily.

It does NOT include npm because there is the maximum path length misnap that happens. Instead we can try installing npm with the system-preinstalled npm using cake.  This gets around the maximum path length problems.