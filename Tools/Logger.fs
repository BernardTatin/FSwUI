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
 I give up for today
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
        | DeviceType of DeviceName
        | UDPort of int

    type private LogState =
        | Start = 0
        | ReadyToOpen = 1
        | Opened = 2
        | InError = 3

    type private LogFile =
        struct
            val mutable logName: LogName
            val mutable stream: TextWriter
            val mutable state: LogState
            new (name, stream, state) =
                {
                    logName = name
                    stream = stream
                    state = state
                }
        end

    let mutable private log : LogFile = LogFile (DeviceType DeviceName.Nothing,
                                                 Console.Out,
                                                 LogState.Start)

    let isOpen () = log.state = LogState.Opened

    let private setName (fileName: LogName) : bool =
        match log.state with
        | LogState.Start
        | LogState.ReadyToOpen ->
            log.logName <- fileName
            log.state <- LogState.ReadyToOpen
            true
        | _ ->
            log.logName <- DeviceType DeviceName.Nothing
            log.state <- LogState.InError
            false

    let openLog (fileName: LogName) : bool =
        match log.state with
        | LogState.Start
        | LogState.ReadyToOpen
        | LogState.InError ->
            if setName fileName then
                try
                    log.state <- LogState.Opened
                    match fileName with
                    | FileName name ->
                        log.stream <- new StreamWriter (name, false)
                    | UDPort port ->
                        setPort port
                    | DeviceType t ->
                        match t with
                        | DeviceName.Nothing ->
                            log.state <- LogState.InError
                        | DeviceName.ConsoleT ->
                            log.stream <- Console.Out
                    | _ ->
                        log.state <- LogState.InError
                with
                    | :? FileNotFoundException -> log.state <- LogState.InError
                    | ex -> log.state <- LogState.InError

            isOpen ()
        | _ -> isOpen ()

    let closeLog () =
        if isOpen () then
            match log.logName with
            | FileName _ ->
                log.stream.Close ()
                log.stream.Dispose ()
                log.state <- LogState.ReadyToOpen
            | UDPort _ ->
                log.state <- LogState.ReadyToOpen
            | DeviceType t ->
                match t with
                | DeviceName.ConsoleT ->
                    log.stream.Flush()
                    log.state <- LogState.ReadyToOpen
                | DeviceName.Nothing ->
                    log.state <- LogState.InError
            log.state = LogState.ReadyToOpen
        else
            false

    let doLog (message: string) : bool =
        if isOpen () then
            match log.logName with
            | FileName _ ->
                log.stream.WriteLine message
                log.stream.Flush()
            | UDPort _ ->
                send (sprintf "%s\n" message)
            | DeviceType t ->
                match t with
                | DeviceName.ConsoleT ->
                    log.stream.WriteLine message
                    log.stream.Flush()
                | DeviceName.Nothing ->
                    log.state <- LogState.InError
            true
        else
            false
