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

module Logger =

    open System.IO

    type private LogFile =
        struct
            val mutable name : string;
            val mutable stream :StreamWriter;
            val mutable isOpen : bool;
        end

    let mutable private log = new LogFile()

    let openLog (fileName : string) : bool =
        log.name <- fileName
        log.stream <- new StreamWriter(fileName, false)
        log.isOpen <- true
        log.isOpen

    let closeLog () =
        if log.isOpen then
            log.stream.Close()
            log.stream.Dispose()
            log.isOpen <- false
            true
        else
            false

    let doLog (message : string) : bool =
        if log.isOpen then
            log.stream.WriteLine message
            true
        else
            false
