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

    let addNewPost session (socket : Socket) (state : State) = 
        let threadName, post = socket.Receive (), socket.Receive ()
        match state.threads.TryFind threadName with
        | Some thread -> 
            state.threads <-
                { thread with posts = (Post post)::thread.posts }
                |> curry state.threads.Add threadName

            socket.Send "Post added."
        | None -> 
            sprintf "No thread with name: %s." threadName
            |> socket.Send 
        
        session

    let getThreads session (socket : Socket) (state : State) = 
        state.threads
        |> Map.toSeq
        |> Seq.map fst
        |> socket.Send

        session

    let getPosts session (socket : Socket) (state : State) = 
        let threadName = socket.Receive ()

        match state.threads.TryFind threadName with
        | Some thread -> 
            try 
                let num = int <| socket.Receive () 

                thread.posts
                |> List.take num
                |> List.map (fun (Post p) -> p)
                |> socket.Send
            with 
            | :? System.FormatException ->
                socket.Send "Invalid number of threads."
        | None -> 
            sprintf "No thread with name: %s." threadName
            |> socket.Send 
            
        session