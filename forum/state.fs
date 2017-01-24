namespace Forum

open Microsoft.FSharp.Collections
open System
open System.IO
open Newtonsoft.Json

type User = {
        name : string
        pass : string
    }

type Session = {
        user : User option
    } 
    with 
    static member empty = { user = None }
     

type Post = Post of string * User

type Thread = {
        name  : string
        posts : Post List
    }

type State = {
        mutable users   : Map<string, User>
        mutable threads : Map<string, Thread>
        filePath        : string
    }
    with 

        static member Load filePath =
            try  
                let s = 
                    File.ReadAllText filePath
                    |> JsonConvert.DeserializeObject<State>

                { s with filePath = filePath }
            with 
            | :? FileNotFoundException -> State.Empty ()
            
        static member Empty () = {
                users    = Map.empty 
                threads  = Map.empty
                filePath = "store.txt"
            }

        member this.Save () = 
            JsonConvert.SerializeObject this
            |> curry File.WriteAllText this.filePath

