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

(*
 From Windows, I tried to write on the equivalent of <stdout>,
 which would be great for debug, but it's really too shitty,
 I made an UDP ShTTY server for that.
 *)
namespace Tools

module Logger =
    open System
    open System.IO
    open UDPTools.UDPSenderTools

    type DeviceName =
        | Nothing = 0
        | ConsoleT = 1

    type LogName =
        | FileName of string
        | UDPort of int
        | DeviceType of DeviceName

    type private LogState =
        | Start = 0
        | ReadyToOpen = 1
        | Opened = 2
        | InError = 3

    type private LogFile(name: LogName, stream: TextWriter, state: LogState) =
        member val logName = name with get, set
        member val stream = stream with get, set
        member val state = state with get, set
        member this.isOpen() : bool = this.state = LogState.Opened
        member this.setName (fileName: LogName) : bool =
            match this.state with
            | LogState.InError
            | LogState.Start
            | LogState.ReadyToOpen ->
                this.logName <- fileName
                this.state <- LogState.ReadyToOpen
                true
            | LogState.Opened -> false
            | _ ->
                this.logName <- DeviceType DeviceName.Nothing
                this.state <- LogState.InError
                false
        member this.openLog (fileName: LogName) : bool =
            match this.state with
            | LogState.Start
            | LogState.ReadyToOpen
            | LogState.InError ->
                if this.setName fileName then
                    try
                        this.state <- LogState.Opened

                        match fileName with
                        | FileName name -> this.stream <- new StreamWriter (name, false)
                        | UDPort port -> setPort port
                        | DeviceType t ->
                            match t with
                            | DeviceName.ConsoleT -> this.stream <- Console.Out
                            | _ -> this.state <- LogState.InError
                    with
                        | :? FileNotFoundException -> this.state <- LogState.InError
                        | ex -> this.state <- LogState.InError

                this.isOpen ()
            | _ -> this.isOpen ()
        member this.closeLog () =
            if this.isOpen () then
                match this.logName with
                | FileName _ ->
                    this.stream.Close ()
                    this.stream.Dispose ()
                    this.state <- LogState.ReadyToOpen
                | UDPort _ -> this.state <- LogState.ReadyToOpen
                | DeviceType t ->
                    match t with
                    | DeviceName.ConsoleT ->
                        this.stream.Flush ()
                        this.state <- LogState.ReadyToOpen
                    | _ -> this.state <- LogState.InError

                this.state = LogState.ReadyToOpen
            else
                false
        member this.doLog (message: string) : bool =
            let tm = DateTime.Now

            let message =
                sprintf "%02d:%02d:%02d: %s" tm.Hour tm.Minute tm.Second message

            if this.isOpen () then
                match this.logName with
                | FileName _ ->
                    this.stream.WriteLine message
                    this.stream.Flush ()
                | UDPort _ -> send message
                | DeviceType t ->
                    match t with
                    | DeviceName.ConsoleT ->
                        this.stream.WriteLine message
                        this.stream.Flush ()
                    | _ -> this.state <- LogState.InError

                true
            else
                false



    let mutable private log: LogFile =
        LogFile (DeviceType DeviceName.Nothing, Console.Out, LogState.Start)

    let openLog (fileName: LogName) : bool =
        log.openLog fileName

    let closeLog () =
        log.closeLog ()

    let doLog (message: string) : bool =
        log.doLog message
