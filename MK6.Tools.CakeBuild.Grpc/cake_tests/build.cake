#tool nuget:?package=Grpc.Tools&version=0.14.0

#r "../src/MK6.Tools.CakeBuild.Grpc/bin/Debug/MK6.Tools.CakeBuild.Grpc.dll"

var target = Argument("Target", "ProtoCompile-Test");

Task("ProtoCompile-Test")
  .Does(() => 
  {
    ProtoCompile("./protos/test.proto");
    if(!FileExists("Test.cs"))
      Error("Test failed--Output file Test.cs not found");
    DeleteFile("Test.cs");
  });

Task("ProtoCompile-Test2")
  .Does(() => 
  {
    ProtoCompile(new ProtoCompileSettings { ProtoInputFile = "./protos/test.proto"});
    if(!FileExists("Test.cs"))
      Error("Test failed--Output file Test.cs not found");
    DeleteFile("Test.cs");
  });
  
Task("ProtoCompile-Test3")
  .Does(() => 
  {
    ProtoCompile(new ProtoCompileSettings 
                      { 
                        ProtoInputFile = "./protos/test.proto", 
                        ProtoImportPath = "./protos",
                        CSharpOutputPath = "./out"
                      });
    if(!FileExists("./out/Test.cs"))
      Error("Test failed--Output file Test.cs not found");
    DeleteFile("./out/Test.cs");
  });  
  
Task("Default")
  .IsDependentOn("ProtoCompile-Test")
  .IsDependentOn("ProtoCompile-Test2")
  .IsDependentOn("ProtoCompile-Test3")
  ;

  
RunTarget(target);