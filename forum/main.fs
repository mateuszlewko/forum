open System
open Forum.Server
open Forum.Connection
open Forum

[<EntryPoint>]
let main args = 
    let state = State.Empty ()

    let disposable = 
        if args.Length >= 2
        then Server.Start (handleConnection state, args.[0], int args.[1])
        else Server.Start (handleConnection state)

    let rec waitForKill () =
        if Console.ReadLine () <> "c" 
        then waitForKill ()

    waitForKill ()

    printfn "bye!"
    disposable.Dispose()
    0
