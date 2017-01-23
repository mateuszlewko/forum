namespace Forum

module Session = 
    open Server
    open System.Net.Sockets

    type Session = {
        user : User option
    }

    let create () = { user = None }

    module Tokens = 
        [<Literal>]
        let CommandEnd = "$end$"   

    let rec skipToEnd (socket : Socket) =
        if socket.Receive() <> Tokens.CommandEnd
        then skipToEnd socket
    let tryLogin session socket = 
        match session.user with 
        | Some user ->
            skipToEnd socket
            session
        | None -> 
            let login, pass = socket.Receive (), socket.Receive ()
            


    let handleConnection (socket : Socket) =
        let mutable session = create ()

        let rec handleRequest () = 
            match socket.Receive () with
            | "#login" -> session <- tryLogin session
            | cmd      -> printfn "Unknown command : %s" cmd

            handleRequest ()

        handleRequest ()