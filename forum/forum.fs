open System
open Forum.Server
open Forum.Session

[<EntryPoint>]
let main args = 
    let disposable = 
        if args.Length >= 2
        then Server.Start (startSession, args.[0], int args.[1])
        else Server.Start (startSession)

    let rec waitForKill () =
        if Console.ReadLine () <> "c" 
        then waitForKill ()

    waitForKill ()

    printfn "bye!"
    disposable.Dispose()
    0
