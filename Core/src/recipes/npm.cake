#addin nuget:?package=Cake.Npm&version=0.8.0

// This is a locally published package which is nuget flavored official release.
#tool "nuget:http://packages.mk6.local/api/?package=mk6-nodejs&version=6.8.1"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

// I deleted the stuff from the Cake example 

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("npm-clean")
    .Does(() => {
        Npm.FromPath(config.NodeJsDirectoryPath)
            .RunScript("clean");
    });

Task("npm-install")
    .Does(() => {
        var npm = Npm.FromPath(config.NodeJsDirectoryPath);
        
        // Install the latest version of NPM.. the nodejs nuget package should
        // know how to reference this...
        npm.Install(settings => settings.Package("npm", "3.9.5"));
        
        npm.Install();
    });

Task("npm-build")
    .IsDependentOn("npm-install")
    .Does(() => {
        Npm.FromPath(config.NodeJsDirectoryPath).RunScript("build");
    });

Task("npm-test")
    .IsDependentOn("npm-build")
    .Does(() => {
        Npm.FromPath(config.NodeJsDirectoryPath).RunScript("test");
    });

Task("npm-ci-publish")
    .IsDependentOn("npm-test")
    .Does(() => {
        Npm.FromPath(config.NodeJsDirectoryPath).RunScript("ci-publish");
    });