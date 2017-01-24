namespace Forum

    module Tokens = 

        [<Literal>]
        let CommandEnd = "$end$"
        
        [<Literal>] 
        let LOGIN = "#login"
        
        [<Literal>]
        let LOGOUT = "#logout"
        
        [<Literal>]
        let REGISTER = "#register"

        [<Literal>]
        let ``ADD_THREAD`` = "#add-thread"

        [<Literal>]
        let ``ADD_POST`` = "#add-post"

        [<Literal>]
        let ``GET_POSTS`` = "#get-posts"
        
        [<Literal>]
        let ``GET_THREADS`` = "#get-threads"