namespace Forum

module ThreadsHandlers =
    open Server
    open System.Net.Sockets

    let addNewThread session (socket : Socket) (state : State) = 
        let name = socket.Receive ()
        match state.threads.TryFind name with
        | Some _ -> 
            socket.Send "Thread with this name already exists."
        | None -> 
            state.threads <- 
                {     
                    name  = name
                    posts = List.empty
                }
                |> curry state.threads.Add name

            sprintf "Added new thread: %s." name
            |> socket.Send 
        
        session

    let addNewPost (session : Session) (socket : Socket) (state : State) = 
        match session.user with
        | Some user ->
            try 
                let threadIndex, post = int <| socket.Receive (), socket.Receive ()
                let items = Map.toArray state.threads
                
                if  0 <= threadIndex && threadIndex < items.Length then
                    let thread = snd items.[threadIndex]
                    state.threads <-
                        { thread with posts = (Post (post, user))::thread.posts }
                        |> curry state.threads.Add thread.name

                    socket.Send "Post added."
                else
                    sprintf "Thread index out of bounds: %d" threadIndex
                    |> socket.Send 
            
            with 
                | :? System.FormatException ->
                    socket.Send "Invalid thread number."
        | None -> ()
        session

    let getThreads session (socket : Socket) (state : State) = 
        state.threads
        |> Map.toSeq
        |> Seq.mapi (fun i th -> sprintf "%d) %A" i (fst th))
        |> socket.Send

        session

    let getPosts session (socket : Socket) (state : State) = 
        try 
            let threadIndex = int <| socket.Receive ()
            let items = Map.toArray state.threads
            
            if  0 <= threadIndex && threadIndex < items.Length then
                let thread = snd items.[threadIndex]
                let num = int <| socket.Receive () 

                thread.posts
                |> List.take (min num thread.posts.Length)
                |> List.map (fun (Post (p, u)) -> sprintf "user: %s\npost: %s" u.name p)
                |> socket.Send
            else
                sprintf "Thread index out of bounds: %d" threadIndex
                |> socket.Send 
        
        with 
            | :? System.FormatException ->
                socket.Send "Invalid number."
        session