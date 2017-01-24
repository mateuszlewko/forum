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

    static member empty = { user = None }
     

type Post = Post of string

type Thread = {
        name  : string
        posts : Post List
    }

type State = {
        mutable users   : Map<string, User>
        mutable threads : Map<string, Thread>
    }
    with 

        static member Load () = 
            raise <| NotImplementedException ()

            
        static member Empty () = {
                users   = Map.empty 
                threads = Map.empty
            }

        member this.Store filePath = 
            raise <| NotImplementedException ()

