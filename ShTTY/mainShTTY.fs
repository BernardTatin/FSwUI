(*
    Project:    ShTTY
    File name:  mainShTTY.fs
    User:       berna
    Date:       2022-08-14

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

namespace main

module mainShTTY =
    open System
    open System.Threading
    open UDPTools.UDPRecieverTools
    open Tools.BasicStuff

    let run (receiver: UDPReceiver) =
        use cancellation = new CancellationTokenSource()
        let onCtrlC () =
            cancellation.Cancel();
            Console.WriteLine "Closing application..."
            receiver.close ()
            Console.WriteLine "exiting..."
            exit (int SYSExit.Success)

        let rec loop (receiver: UDPReceiver) =
            receiver.receive ()
            loop (receiver)

        printfn "Prepare Ctrl+C handler"
        Console.CancelKeyPress.Add  (fun _ -> onCtrlC ())
        // why I had to put () around  receiver.getPort() ?
        // perhaps membership is done by a sort of a preprocessor
        // strange and disturbing and sort of 'not natural'
        printfn "Listening on port %d" (receiver.getPort())
        printfn "Run loop"
        loop receiver


    [<EntryPoint>]
    let main argv =
        if argv.Length = 0 then
            on_error "You must specify a port number"

        let receivePort = str2int argv[0]
        // setPort receivePort
        let receiver = new UDPReceiver(receivePort)
        run receiver

        (int SYSExit.Success)
