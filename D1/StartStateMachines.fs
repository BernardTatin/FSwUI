(*
    Project:    D1
    File name:  StateMachines.fs
    User:       berna
    Date:       2022-08-10

    The MIT License (MIT)

    Copyright (c) 2022 Bernard TATIN

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

 *)

namespace d1

module StartStateMachines =
    open System
    open Tools.Logger

    type private StartingStates =
        | Start
        | Error
        | ConfigSearch
        | ConfigWriteDefault
        | ConfigLoad
        | ConfigLoaded
        | StartEnd

    // bad stuff
    let private printState (state: StartingStates) =
        let i_print_State (stateName: string) = doLog stateName

        match state with
        | Start -> i_print_State (sprintf "-> %s" "Start")
        | Error -> i_print_State (sprintf "-> %s" "Error")
        | ConfigSearch -> i_print_State (sprintf "-> %s" "ConfigSearch")
        | ConfigWriteDefault -> i_print_State (sprintf "-> %s" "ConfigWriteDefault")
        | ConfigLoad -> i_print_State (sprintf "-> %s" "ConfigLoad")
        | ConfigLoaded -> i_print_State (sprintf "-> %s" "ConfigLoaded")
        | StartEnd -> i_print_State (sprintf "-> %s" "StartEnd")

    let onStart () : bool =
        let rec run newState isOK : bool =
            printState newState |> ignore

            match newState with
            | Start -> run ConfigSearch isOK
            | ConfigSearch -> run ConfigWriteDefault isOK
            | ConfigWriteDefault -> run ConfigLoaded isOK
            | ConfigLoad -> run ConfigLoaded isOK
            | ConfigLoaded -> run StartEnd isOK
            | Error -> run StartEnd false
            | StartEnd -> isOK

        doLog "C'est parti" |> ignore
        run Start true
