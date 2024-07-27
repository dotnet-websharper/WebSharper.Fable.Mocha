#r "nuget: FAKE.Core"
#r "nuget: Fake.Core.Target"
#r "nuget: Fake.IO.FileSystem"
#r "nuget: Fake.Tools.Git"
#r "nuget: Fake.DotNet.Cli"
#r "nuget: Fake.DotNet.AssemblyInfoFile"
#r "nuget: Fake.DotNet.Paket"

open Fake.Core
System.Environment.GetCommandLineArgs()
|> Array.skip 2 // skip fsi.exe; build.fsx
|> Array.toList
|> Fake.Core.Context.FakeExecutionContext.Create false __SOURCE_FILE__
|> Fake.Core.Context.RuntimeContext.Fake
|> Fake.Core.Context.setExecutionContext

#load "paket-files/wsbuild/github.com/dotnet-websharper/build-script/WebSharper.Fake.fsx"
open WebSharper.Fake
open Fake.DotNet
open Fake.Core
open Fake.Core.TargetOperators

let WithProjects projects args =
    { args with BuildAction = Projects projects }

Target.create "PrePackaging" <| fun _ ->

    let template = """type file
id WebSharper.Fable.Mocha
authors IntelliFactory
projectUrl https://websharper.com/
repositoryType git
repositoryUrl https://github.com/dotnet-websharper/WebSharper.Fable.Mocha/
licenseUrl https://github.com/dotnet-websharper/WebSharper.Fable.Mocha/blob/master/LICENSE.md
iconUrl https://github.com/dotnet-websharper/core/raw/websharper50/tools/WebSharper.png
description
    WebSharper Proxy for Fable's Browser.%TEMP%
tags
    WebSharper Fable FSharp CSharp JavaScript WebAPI %TEMP%
dependencies
    framework: netstandard2.0
        WebSharper ~> LOCKEDVERSION:[3]
        Fable.Mocha == 2.17.0
files
    ../websharper/WebSharper.Fable.Mocha/bin/Release/netstandard2.0/WebSharper.Fable.Mocha.dll ==> lib/netstandard2.0

references
    WebSharper.Fable.Mocha.dll

"""

    System.IO.Directory.CreateDirectory "nuget"
    System.IO.File.WriteAllText(sprintf "nuget/WebSharper.Fable.Mocha.paket.template", template)

let targets =
    LazyVersionFrom "WebSharper" |> WSTargets.Default
    |> fun args ->
        { args with
            Attributes = [
                AssemblyInfo.Company "IntelliFactory"
                AssemblyInfo.Copyright "(c) IntelliFactory 2023"
                AssemblyInfo.Title "https://github.com/dotnet-websharper/WebSharper.Fable.Mocha"
                AssemblyInfo.Product "WebSharper Browser"
            ]
        }
    |> WithProjects [
        "WebSharper.sln"
    ]
    |> MakeTargets

"PrePackaging" ==> "WS-Package"

targets
|> RunTargets