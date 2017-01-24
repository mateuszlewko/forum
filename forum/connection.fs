namespace Forum

module Connection = 
    open Forum
    open Server
    open System.Net.Sockets
    open LoginHandlers
    open ThreadsHandlers
    open Tokens

    /// Skips incoming messages until CommandEnd token appears 
    let rec skipToEnd (socket : Socket) =
        if socket.Receive () <> Tokens.CommandEnd
        then
            printfn "skip" 
            skipToEnd socket

    let handleConnection (state : State) (socket : Socket) =
        let mutable session = Session.empty
        let sss f = f session socket state

        let auth f = 
            match session.user with 
            | Some user -> f
            | None -> 
                socket.Send "Login first."
                fun s _ _ -> s

        let rec handleRequest () = 
            session <- 
                match socket.Receive () with
                | LOGIN           -> sss loginUser 
                | LOGOUT          -> sss logout
                | REGISTER        -> sss registerUser
                | ``ADD_THREAD``  -> sss <| auth addNewThread
                | ``ADD_POST``    -> sss <| auth addNewPost
                | ``GET_POSTS``   -> sss <| auth getPosts 
                | ``GET_THREADS`` -> sss <| auth getThreads
                // | CommandEnd      -> session
                | cmd             -> 
                    printfn "Unknown command: %s" cmd
                    session 

            skipToEnd socket
            handleRequest ()

        handleRequest ()