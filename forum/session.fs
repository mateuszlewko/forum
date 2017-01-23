namespace Forum

module Session = 
    open Server
    open System.Net.Sockets

    let startSession (socket : Socket) =
        let rec handleRequest () = 
            printfn "%s" <| socket.Receive ()
            handleRequest ()

        handleRequest ()