open System
open Forum.Server
open Forum.Session

[<EntryPoint>]
let main args = 
    let disposable = 
        if args.Length >= 2
        then Server.Start (handleConnection, args.[0], int args.[1])
        else Server.Start (handleConnection)

    let rec waitForKill () =
        if Console.ReadLine () <> "c" 
        then waitForKill ()

    waitForKill ()

    printfn "bye!"
    disposable.Dispose()
    0
