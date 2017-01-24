namespace Forum

module Connection = 
    open Forum
    open Server
    open System.Net.Sockets
    open LoginHandlers
    open ThreadsHandlers

    [<AutoOpen>]
    module Tokens = 

        [<Literal>]
        let CommandEnd = "$end$"
        
        [<Literal>] 
        let LOGIN = "#login"
        
        [<Literal>]
        let LOGOUT = "#logout"
        
        [<Literal>]
        let REGISTER = "#register"

        [<Literal>]
        let ``ADD_THREAD`` = "#add-thread"

        [<Literal>]
        let ``ADD_POST`` = "#add-post"

        [<Literal>]
        let ``GET_POSTS`` = "#get-posts"

    /// Skips incoming messages until CommandEnd token appears 
    let rec skipToEnd (socket : Socket) =
        if socket.Receive () <> Tokens.CommandEnd
        then
            printfn "skip" 
            skipToEnd socket

    let handleConnection (state : State) (socket : Socket) =
        let mutable session = Session.empty
        let sss f = f session socket state

        let rec handleRequest () = 
            session <- 
                match socket.Receive () with
                | LOGIN          -> sss loginUser 
                | LOGOUT         -> sss logout
                | REGISTER       -> sss registerUser
                | ``ADD_THREAD`` -> sss addNewThread
                | ``ADD_POST``   -> sss addNewPost
                | cmd            -> 
                    printfn "Unknown command : %s" cmd
                    session 

            skipToEnd socket
            handleRequest ()

        handleRequest ()