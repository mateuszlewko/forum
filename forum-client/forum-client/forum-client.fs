module ForumClient

open System
open System.Net.Sockets
open System.Text
open System.Threading
open Forum.Tokens

let write (stream : NetworkStream) (text : string) =
    let input = Encoding.UTF8.GetBytes text

    if stream.CanWrite then
        stream.Write(input, 0, input.Length)
    else
        printfn "Can't write"

let sendLine stream = write stream <| Console.ReadLine()

let parseAction (stream : NetworkStream) = 
    function
    | "1" | "l" -> 
        write stream LOGIN
        printfn "--- Login ---\nname: "
        sendLine stream
        printfn "pass: "
        sendLine stream
        write stream CommandEnd
    | "2" | "r" -> 
        ()
    | "3" | "out" ->
        ()
    | "4" | "ths" ->
        ()
    | "5" | "psts" ->
        ()
    | "6" | "nthr" ->
        ()
    | "7" | "npst" ->
        ()
    | cmd -> printfn "Unknown command: %s" cmd


let rec asyncSendInput (stream : NetworkStream) =
    async {
        let input = 
            Console.ReadLine ()
            |> Encoding.UTF8.GetBytes

        if stream.CanWrite then
            stream.Write(input, 0, input.Length)
            return! asyncSendInput stream
        else
            printfn "Can't write"
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
            printfn "Can't read."
    }

[<EntryPoint>]
let main args =
    let client = new System.Net.Sockets.TcpClient()
    client.Connect(args.[0], Int32.Parse(args.[1]))
    printfn "Connected to %A %A..." args.[0] args.[1]
    printfn "\nType number corresponding to action."
    let stream = client.GetStream()

    asyncSendInput stream |> Async.Start
    asyncPrintResponse stream |> Async.RunSynchronously
    0