open System
open Forum.Server
open Forum.Connection
open Forum

[<EntryPoint>]
let main args = 
    let filePath = args.[0]
    let state = State.Load filePath

    let disposable = 
        if args.Length >= 2
        then Server.Start (handleConnection state, args.[1], int args.[2])
        else Server.Start (handleConnection state)

    let rec waitForKill () =
        if Console.ReadLine () <> "c" 
        then waitForKill ()

    waitForKill ()
    state.Save ()

    printfn "bye!"
    disposable.Dispose()
    0
