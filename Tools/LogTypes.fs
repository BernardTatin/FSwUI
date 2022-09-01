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

(*
    TODO: rewrite all with classes
 *)
namespace LogTools

open System
open System.IO
open UDPTools.UDPSenderTools
open Tools.BasicStuff

module LogTypes =

    // TODO: c'est l'bordel, la didans!

    type LogState =
        | Start = 0
        | Opened = 2
        | InError = 3

    let formatMessage (message: string) (withEOL: bool): string =
        let tm = DateTime.Now
        if withEOL then
            sprintf "%02d:%02d:%02d: %s\n" tm.Hour tm.Minute tm.Second message
        else
            sprintf "%02d:%02d:%02d: %s" tm.Hour tm.Minute tm.Second message


    [<AbstractClass>]
    type LogBase() =
        let mutable state: LogState = LogState.Start
        member this.isOpen() : bool = state = LogState.Opened
        member this.isIdle() : bool = state = LogState.Start
        member this.State with get() = state and set(s: LogState) = state <- s

        abstract member start : unit -> bool
        default this.start() =
            printfn "LogBase.start()"
            false

        abstract member stop: unit -> bool
        default this.stop() = false

        abstract member write: string -> bool
        default this.write message : bool =
            printfn "LogBase.write %s" message
            false

    type LogWithStream() as self =
        inherit LogBase()
        let mutable stream: TextWriter = Console.Out

        member this.Stream with get() = stream and set(s: TextWriter) = stream <- s
        override this.stop() =
            if self.isOpen() then
                stream.Close ()
                stream.Dispose ()
                self.State <- LogState.Start
                true
            else
                false

        override this.write(message : string) : bool =
            if self.isOpen() then
                stream.WriteLine (formatMessage message false)
                stream.Flush()
                true
            else
                false

    type LogConsole() as self =
        inherit LogWithStream()
        override this.start() =
            self.State <- LogState.Opened
            true

    type LogTextFile(fileName: string) as self =
        inherit LogWithStream()
        override this.start() =
            if self.isIdle() then
                self.Stream <- new StreamWriter (fileName, false)
                self.State <- LogState.Opened
            self.isOpen()


    type LogUDP(port: int) as self =
        inherit LogBase()
        let sender = new UDPSender(port)
        override this.start() =
            self.State <- LogState.Opened
            true
        override this.stop() =
            if self.isOpen() then
                sender.Close()
                self.State <- LogState.Start
            self.isIdle()

        override this.write(message : string) =
            if self.isOpen() then
                sender.send (formatMessage message (isUnix()))
                true
            else
                false
