﻿module ProgramTokenReplacer

module SCL = 
    open System.CommandLine

    let packageOpt = Option<string>("--package", [|"-p"|], Description = "A package with a leading @ name")

    let action (parseResult: ParseResult) = 
        let package = parseResult.GetValue packageOpt
        printfn $"The value for --package is: %s{package}"

    let main (argv: string[]) = 
        let cfg = new CommandLineConfiguration(new RootCommand())
        cfg.ResponseFileTokenReplacer <- null // in beta5, you must set ResponseFileTokenReplacer to null to skip @ processing
        let cmd = cfg.RootCommand

        cmd.Description <- "My sample app"
        cmd.Add(packageOpt)
        cmd.SetAction(action)

        cmd.Parse(argv, cfg).Invoke()

    let run () = 
        "--package @shoelace-style/shoelace" |> Utils.args |> main


open FSharp.SystemCommandLine
open Input

let getCmd = 
    command "get" {
        description "Get a package by name"
        inputs (
            argument<string> "package" 
            |> desc "A package with a leading @ name"
        )
        setAction (fun (package: string) ->
            if package.StartsWith("@") then
                printfn $"package name: '{package}'"
                0 // success
            else
                eprintfn "The package name does not start with a leading @"
                1 // failure
        )
    }

let main argv =
    rootCommand argv {
        description "Can be called with a leading @ package"
        configure (fun cfg -> 
            // Skip @ processing
            cfg.ResponseFileTokenReplacer <- null
        )        
        noAction
        addCommand getCmd
    }

let run () = 
    "get @shoelace-style/shoelace" |> Utils.args |> main
