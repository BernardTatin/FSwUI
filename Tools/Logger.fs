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

(*
    TODO: must disappear when LogTypes.fs will have some classes
 *)

namespace LogTools

open LogTools.LogTypes
open LogTypes
open Tools.BasicStuff

module Logger =

    type private OLogger = LogBase option

    let mutable private log =
        OLogger.None


    let openLog () : bool =
        if not(isWindows()) then
            log <- Some (new LogConsole())
            // log <- Some (new LogUDP(2345))
        else
            log <- Some (new LogUDP(2345))
            // log <- Some (new LogConsole())
        log.Value.start()

    let closeLog () =
#if DEBUG
        log.Value.stop ()
#else
        true
#endif

    let doLog (message: string) : bool =
#if DEBUG
        log.Value.write message
#else
        true
#endif

    let doLogError message = doLog (sprintf "ERROR >>> %s\n" message)
