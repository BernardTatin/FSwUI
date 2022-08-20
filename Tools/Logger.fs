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
 This code is more about to learn F# types than anything else. To get something
 really usable, some more works must be done. Perhaps in the next days.

 From Windows application, I tried to write on the equivalent of <stdout>,
 which would be great for debug, but it's really too shitty,
 I made an UDP ShTTY server for that.
 *)

namespace LogTools

open LogTools.LogTypes

module Logger =
    open System
    open System.IO
    open LogTypes
    open UDPTools.UDPSenderTools

    // type DeviceName =
    //     | Nothing = 0
    //     | ConsoleT = 1
    //
    // type LogName =
    //     | FileName of string
    //     | UDPort of int
    //     | DeviceType of DeviceName
    //
    //
    // type private LogState =
    //     | Start = 0
    //     | ReadyToOpen = 1
    //     | Opened = 2
    //     | InError = 3
    //
    // type private LogFile() =
    //     let mutable logName = DeviceType DeviceName.Nothing
    //     let mutable stream = Console.Out
    //     let mutable state = LogState.Start
    //     let isOpen() : bool = state = LogState.Opened
    //
    //     let setName(fileName: LogName) : bool =
    //         match state with
    //         | LogState.InError
    //         | LogState.Start
    //         | LogState.ReadyToOpen ->
    //             logName <- fileName
    //             state <- LogState.ReadyToOpen
    //             true
    //         | LogState.Opened -> false
    //         | _ ->
    //             logName <- DeviceType DeviceName.Nothing
    //             state <- LogState.InError
    //             false
    //
    //     member this.openLog(fileName: LogName) : bool =
    //         match state with
    //         | LogState.Start
    //         | LogState.ReadyToOpen
    //         | LogState.InError ->
    //             if setName fileName then
    //                 try
    //                     state <- LogState.Opened
    //
    //                     match fileName with
    //                     | FileName name -> stream <- new StreamWriter (name, false)
    //                     | UDPort port -> setPort port
    //                     | DeviceType t ->
    //                         match t with
    //                         | DeviceName.ConsoleT -> stream <- Console.Out
    //                         | _ -> state <- LogState.InError
    //                 with
    //                     | :? FileNotFoundException -> state <- LogState.InError
    //                     | ex -> state <- LogState.InError
    //
    //             isOpen ()
    //         | _ -> isOpen ()
    //
    //     member this.closeLog() =
    //         if isOpen () then
    //             match logName with
    //             | FileName _ ->
    //                 stream.Close ()
    //                 stream.Dispose ()
    //                 state <- LogState.ReadyToOpen
    //             | UDPort _ -> state <- LogState.ReadyToOpen
    //             | DeviceType t ->
    //                 match t with
    //                 | DeviceName.ConsoleT ->
    //                     stream.Flush ()
    //                     state <- LogState.ReadyToOpen
    //                 | _ -> state <- LogState.InError
    //
    //             logName <- DeviceType DeviceName.Nothing
    //             state = LogState.ReadyToOpen
    //         else
    //             false
    //
    //     member this.doLog(message: string) : bool =
    //         let tm = DateTime.Now
    //
    //         let message =
    //             sprintf "%02d:%02d:%02d: %s" tm.Hour tm.Minute tm.Second message
    //
    //         if isOpen () then
    //             match logName with
    //             | FileName _ ->
    //                 stream.WriteLine message
    //                 stream.Flush ()
    //             | UDPort _ -> send message
    //             | DeviceType t ->
    //                 match t with
    //                 | DeviceName.ConsoleT ->
    //                     stream.WriteLine message
    //                     stream.Flush ()
    //                 | _ -> state <- LogState.InError
    //
    //             true
    //         else
    //             false

    let mutable private log: ILogger =
        LogNothing ()

    let openLog (fileName: LogName) : bool = log.openLog fileName

    let closeLog () = log.closeLog ()

    let doLog (message: string) : bool = log.doLog message
