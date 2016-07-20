#tool "nuget:?package=Grpc.Tools"

var grpcToolsRootPath = (DirectoryPath)Directory("./tools/Grpc.Tools/tools/windows_x64");
var protocPath = MakeAbsolute(grpcToolsRootPath.Combine("protoc.exe")).FullPath;
var csharpPluginPath = MakeAbsolute(grpcToolsRootPath.Combine("grpc_csharp_plugin.exe")).FullPath;

var protoImportPath = Argument<string>("protoImportPath"); //"C:/src/kroger.microservices/protos/reporting.metadata";
var csharpOutPath = Argument<string>("csharpOutPath"); //"C:/src/MK6.Tools.CakeBuild/BuildScripts";
var protoInputPath = Argument<string>("protoInputPath"); //"C:/src/kroger.microservices/protos/reporting.metadata/messages.proto";

Task("ProtoGen")
    .Does(() => {
      foreach(var protoFile in System.IO.Directory.GetFiles(protoInputPath, "*.proto"))
      {
        var p = StartAndReturnProcess(protocPath, new ProcessSettings
        {
          Arguments = String.Format("--proto_path={0} --csharp_out {1} --grpc_out {1} --plugin=protoc-gen-grpc={2} {3}", 
                      protoImportPath,
                      csharpOutPath,
                      csharpPluginPath,
                      protoFile)
        });
        p.WaitForExit();
        var lines = p.GetStandardOutput();
        foreach(var l in lines)
        {
          Information(l);
        }
      }
    });

RunTarget("ProtoGen");