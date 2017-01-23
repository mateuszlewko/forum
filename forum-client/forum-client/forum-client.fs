module ForumClient

open System
open System.Net.Sockets
open System.Text
open System.Threading

let rec asyncSendInput (stream : NetworkStream) =
    async {
        let input = 
            Console.ReadLine ()
            |> Encoding.UTF8.GetBytes

        if stream.CanWrite then
            input |> Array.iter stream.WriteByte
            return! asyncSendInput stream
        else
            printfn "Can't write"
    }

let rec asyncPrintResponse (stream : NetworkStream) =
    async {
        if stream.CanRead then 
            let response = stream.ReadByte () |> Convert.ToChar
            Console.Write response
            Thread.Sleep 2000
            return! asyncPrintResponse stream
        else
            printfn "Can't read"
    }

[<EntryPoint>]
let main args =
    let client = new System.Net.Sockets.TcpClient()
    client.Connect(args.[0], Int32.Parse(args.[1]))
    printfn "Connected to %A %A..." args.[0] args.[1]
    let stream = client.GetStream()
    printfn "Got stream, starting two way asynchronous communication."
    asyncSendInput stream |> Async.Start
    asyncPrintResponse stream |> Async.RunSynchronously
    0