module ForumClient

open System
open System.Net.Sockets
open System.Text
open System.Threading
open Forum.Tokens

let write (stream : NetworkStream) (text : string) =
    let input = Encoding.UTF8.GetBytes text

    if stream.CanWrite then
        stream.Write (input, 0, input.Length)
        Thread.Sleep 1
    else
        printfn "Can't write stream"

let sendLine stream = write stream <| Console.ReadLine()

let commandsInfo () = 
    printfn "--- Commands ---\n1 - login, 2 - register, 3 - logout, 4 - get threads,\n5 - get #n posts, 6 - new thread, 7 - new post,\n0 | cmds - print this info\n--- * ---"

let parseAction (stream : NetworkStream) = 
    function
    | "1" | "l" -> 
        write stream LOGIN
        printf "--- Login ---\nname: "
        sendLine stream
        printf "pass: "
        sendLine stream
        write stream CommandEnd
    | "2" | "r" -> 
        write stream REGISTER
        printf "--- Register ---\nnew name: "
        sendLine stream
        printf "pass: "
        sendLine stream
        write stream CommandEnd
    | "3" | "out" ->
        write stream LOGOUT
        write stream CommandEnd
    | "4" | "ths" ->
        printfn "--- Threads --- "
        write stream GET_THREADS
        write stream CommandEnd
    | "5" | "psts" ->
        ()
    | "6" | "nthr" ->
        write stream ADD_THREAD
        printf "--- New threads ---\nTitle: "
        sendLine stream
        write stream CommandEnd
    | "7" | "npst" ->
        ()
    | "cmds" -> commandsInfo ()
    | cmd -> printfn "Unknown command: %s" cmd


let rec asyncSendInput (stream : NetworkStream) =
    async {
        if stream.CanWrite then
            Console.ReadLine ()
            |> parseAction stream

            // commandsInfo ()
            return! asyncSendInput stream
    }

let read (stream : NetworkStream) =
    let data : byte array = Array.zeroCreate <| 1024 * 64
    stream.Read(data, 0, data.Length) |> ignore

    data
    |> Array.takeWhile ((<>) 0uy)
    |> Encoding.UTF8.GetString 

let rec asyncPrintResponse (stream : NetworkStream) =
    async {
        if stream.CanRead then 
            printfn "%s" <| read stream
            return! asyncPrintResponse stream
        else
            printfn "Can't read stream."
    }

[<EntryPoint>]
let main args =
    let client = new System.Net.Sockets.TcpClient()
    client.Connect(args.[0], Int32.Parse(args.[1]))
    printfn "Connected to %A %A..." args.[0] args.[1]
    commandsInfo ()
    printfn "\nType number corresponding to action."
    let stream = client.GetStream()

    asyncSendInput stream |> Async.Start
    asyncPrintResponse stream |> Async.RunSynchronously
    0