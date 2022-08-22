(*
    Project:    Tools
    File name:  LoTypes.fs
    User:       berna
    Date:       2022-08-20

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

namespace LogTools

open System
open System.IO
open UDPTools.UDPSenderTools

module LogTypes =

    type LogState =
        | Start = 0
        | Opened = 2
        | InError = 3

    type ILogger =
        abstract member currentState : LogState
        abstract member start : unit -> bool
        abstract member stop : unit -> bool
        abstract member write : string -> bool

    type LogNothing() =
        interface ILogger with
            member this.currentState = LogState.Opened
            member this.start() = false
            member this.stop() = false
            member this.write(message : string) = false

    let formatMessage (message: string) (withEOL: bool): string =
        let tm = DateTime.Now
        if withEOL then
            sprintf "%02d:%02d:%02d: %s\n" tm.Hour tm.Minute tm.Second message
        else
            sprintf "%02d:%02d:%02d: %s" tm.Hour tm.Minute tm.Second message

    type LogConsole() =
        let stream: TextWriter = Console.Out
        interface ILogger with
            member this.currentState = LogState.Opened
            member this.start() = true
            member this.stop() = true
            member this.write(message : string) =
                stream.Write (formatMessage message true)
                stream.Flush()
                true

    type LogTextFile(fileName: string) =
        let stream: TextWriter =  new StreamWriter (fileName, false)
        interface ILogger with
            member this.currentState = LogState.Opened
            member this.start() = true
            member this.stop() =
                stream.Close ()
                stream.Dispose ()
                true
            member this.write(message : string) =
                stream.Write (formatMessage message true)
                stream.Flush()
                true

    type LogUDP(port: int) =
        let sender = new UDPSender(port)
        interface ILogger with
            member this.currentState = LogState.Opened
            member this.start() = true
            member this.stop() =
                true
            member this.write(message : string) =
                sender.send (formatMessage message true)
                true
