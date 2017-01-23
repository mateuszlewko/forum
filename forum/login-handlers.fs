namespace Forum

module LoginHandlers =
    open Server
    open System.Net.Sockets

    let loginUser session (socket : Socket) (state : State) = 
        match session.user with 
        | Some user -> session
        | None -> 
            let login, pass = socket.Receive (), socket.Receive ()
            match state.users.TryFind login with
            | Some user -> 
                if user.pass = pass then
                    socket.Send "Logged in."
                    { session with user = Some user }
                else
                    socket.Send "Wrong password."
                    session
            | None -> 
                socket.Send "No such login."
                session

    let registerUser session (socket : Socket) (state : State) = 
        let login, pass = socket.Receive (), socket.Receive ()
        match state.users.TryFind login with
        | Some user -> 
            socket.Send "Username taken."
        | None -> 
            state.users <- 
                {     
                    User.name = login
                    pass = pass 
                }
                |> curry state.users.Add login

            sprintf "Registered new user: %s." login
            |> socket.Send 
        
        session

    let logout session (socket : Socket) (state : State) = 
        match session.user with 
        | Some user -> 
            socket.Send "Logged-out succesfully."
            { session with user = None }
        | None -> 
            socket.Send "You aren't logged-in."
            session