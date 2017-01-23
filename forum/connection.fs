namespace Forum

module Connection = 
    open Forum
    open Server
    open System.Net.Sockets
    open LoginHandlers

    module Tokens = 
        [<Literal>]
        let CommandEnd = "$end$"   

    /// Skips incoming messages until CommandEnd token appears 
    let rec skipToEnd (socket : Socket) =
        if socket.Receive() <> Tokens.CommandEnd
        then skipToEnd socket

    let handleConnection (state : State) (socket : Socket) =
        let mutable session = Session.Create ()
        let sss f = f session socket state

        let rec handleRequest () = 
            session <- 
                match socket.Receive () with
                | "#login"    -> sss loginUser 
                | "#logout"   -> sss logout
                | "#register" -> sss registerUser
                | cmd         -> 
                    printfn "Unknown command : %s" cmd
                    session 

            skipToEnd socket
            handleRequest ()

        handleRequest ()