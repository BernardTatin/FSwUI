(*
    Project:    TestShTTY
    File name:  mainTest.fs
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

open UDPTools

module mainTest =
    open System
    open UDPTools.UDPSenderTools
    open Tools.BasicStuff

    let rec loop (sender: UDPSender) (k: int) =
        let message = sprintf "%5d message" k
        printfn "Sending <%s>" message
        sender.send message
        // System.Threading.Thread.Sleep 250
        if k < 300 then loop sender (k + 1) else ()

    [<EntryPoint>]
    let main argv =
        if argv.Length = 0 then
            on_error "You must specify a port number"

        let sendPort = str2int argv[0]
        let sender = new UDPSender (sendPort)
        printfn "Sending on port %d" sendPort
        loop sender 1

        (int SYSExit.Success)
