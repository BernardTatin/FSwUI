(*
    Project:    Tools
    File name:  BasicStuff.fs
    User:       berna
    Date:       2022-08-16

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

namespace Tools

module BasicStuff =
    open System

    type SYSExit =
        | Success = 0
        | Failure = 127

    let on_error message =
        match message with
        | "" -> eprintfn "FATAL ERROR!!"
        | str -> eprintfn "ERROR %s!!" message

        exit (int SYSExit.Failure)

    let str2int (str: string) =
        let mutable result = 0

        if Int32.TryParse (str, &result) then
            result
        else
            on_error (sprintf "Unable to transform '%s' as an integer" str)
            |> ignore

            0
