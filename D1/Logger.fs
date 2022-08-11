(*
    Project:    D1
    File name:  Logger.fs
    User:       berna
    Date:       2022-08-11

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

open System.Windows.Forms.VisualStyles

module Logger =

    open System.IO

    type private LogState =
        | Empty = 0
        | ReadyToOpen = 1
        | Open = 2
        | InError = 3

    type private LogFile =
        struct
            val mutable name : string;
            val mutable stream :StreamWriter;
            val mutable state : LogState;
        end

    let mutable private log = new LogFile()

    let isOpen() =
        log.state = LogState.Open

    let setName (fileName : string) : bool =
        match log.state with
        | LogState.Empty
        | LogState.ReadyToOpen ->
            log.name <- fileName
            log.state <- LogState.ReadyToOpen
            true
        | _ ->
            false

    let openLog (fileName : string) : bool =
        match log.state with
        | LogState.Empty
        | LogState.ReadyToOpen
        | LogState.InError ->
            if setName fileName then
                try
                    log.stream <- new StreamWriter(log.name, false)
                    log.state <- LogState.Open
                with
                | :? FileNotFoundException -> log.state <- LogState.InError
                | ex -> log.state <- LogState.InError
            isOpen()
        | _ ->
            isOpen()

    let closeLog () =
        if isOpen() then
            log.stream.Close()
            log.stream.Dispose()
            log.state <- LogState.ReadyToOpen
            true
        else
            false

    let doLog (message : string) : bool =
        if isOpen() then
            log.stream.WriteLine message
            true
        else
            false
