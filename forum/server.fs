namespace Forum

module Server = 
    // Module based on: https://gist.github.com/panesofglass/765088

    open System
    open System.IO
    open System.Net
    open System.Net.Sockets
    open System.Threading
    open System.Text

    type Socket with
    
        member socket.AsyncAccept() = 
            Async.FromBeginEnd(socket.BeginAccept, socket.EndAccept)

        member socket.AsyncReceive(buffer : byte[], ?offset, ?count) =
            let offset = defaultArg offset 0
            let count = defaultArg count buffer.Length
            let beginReceive (b, o, c, cb, s) = socket.BeginReceive(b, o, c, SocketFlags.None, cb, s)
            Async.FromBeginEnd(buffer, offset, count, beginReceive, socket.EndReceive)
        
        member socket.AsyncSend(buffer : byte[], ?offset, ?count) =
            let offset = defaultArg offset 0
            let count = defaultArg count buffer.Length
            let beginSend(b, o, c, cb, s) = socket.BeginSend(b, o, c, SocketFlags.None, cb, s)
            Async.FromBeginEnd(buffer, offset, count, beginSend, socket.EndSend)

        member socket.AsyncSend (text : string) =
            Encoding.UTF8.GetBytes text |> socket.AsyncSend

        member socket.Send (texts : string seq) =
            texts
            |> String.concat "\n" 
            |> socket.AsyncSend 
            |> Async.RunSynchronously 
            |> ignore 

        member socket.Send (text : string) =
            printfn "sent: %s" text

            socket.AsyncSend text 
            |> Async.RunSynchronously 
            |> ignore 

        member socket.Receive ?maxSize =
            let data : byte array = 
                Array.zeroCreate <| defaultArg maxSize 1024 * 64
            socket.Receive data |> ignore
            let s = 
                data
                |> Array.takeWhile ((<>) 0uy)
                |> Encoding.UTF8.GetString 

            printfn "got: %s" s
            s

    type Server() =

        static member Start(handleConnection, hostname : string, ?port) =
            let ipAddress = Dns.GetHostEntry(hostname).AddressList.[0]
            Server.Start(handleConnection, ipAddress, ?port = port)

        static member Start(handleConnection : Socket -> unit, ?ipAddress, ?port) =
            let ipAddress = defaultArg ipAddress <| IPAddress.Parse "127.0.0.1"
            let port = defaultArg port 4000
            let endpoint = IPEndPoint(ipAddress, port)
            let cts = new CancellationTokenSource()
            let listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            
            listener.Bind(endpoint)
            listener.Listen(int SocketOptionName.MaxConnections)
            
            printfn "Started listening on port %d" port
            
            let rec acceptRequest () = async {
                printfn "Waiting for request..."
                let! socket = listener.AsyncAccept ()
                printfn "Received request"
                
                try
                    async { handleConnection socket } |> Async.Start
                with e -> printfn "An error occurred: %s" e.Message

                return! acceptRequest ()
            }

            Async.Start (acceptRequest (), cancellationToken = cts.Token)
            
            { new IDisposable with 
                member x.Dispose() = 
                    cts.Cancel()
                    listener.Close() 
            }