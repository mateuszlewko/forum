namespace Forum

open System.Collections.Generic
open System

type User = {
    name : string
    pass : string
}

type Post = Post of string

type Thread = Thread of Post List

type State = {
        mutable users   : Dictionary<string, User>
        mutable threads : Thread List
    }
    with 
    
        static member Load () = 
            raise <| NotImplementedException ()

        static member Create () = {
                users   = new Dictionary<_, _> ()
                threads = new List<_> ()
            }

