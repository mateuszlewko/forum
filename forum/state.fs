namespace Forum

open Microsoft.FSharp.Collections
open System

type User = {
        name : string
        pass : string
    }

type Session = {
        user : User option
    } with 

    static member Create () = { user = None }
     

type Post = Post of string

type Thread = Thread of Post List

type State = {
        mutable users   : Map<string, User>
        mutable threads : Thread List
    }
    with 

        static member Load () = 
            raise <| NotImplementedException ()

            
        static member Create () = {
                users   = Map.empty 
                threads = List.empty
            }

        member this.Store filePath = 
            raise <| NotImplementedException ()

