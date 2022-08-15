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

module mainTest =
    open System
    open System.Net
    open System.Net.Sockets
    open System.Text

    let on_error message =
        match message with
            | "" -> eprintfn "FATAL ERROR!!"
            | str -> eprintfn "ERROR %s!!" message
        exit 1

    let str2int (str: string) =
        let mutable result = 0
        if Int32.TryParse(str, &result) then
            result
        else
            on_error (sprintf "Unable to transform '%s' as an integer" str) |> ignore
            0

    let mutable sendPort = 8080

    let send(message : string) =
        let sendingClient = new UdpClient()
        let sendingIpEndPoint = new IPEndPoint(IPAddress.Loopback, sendPort)
        let (sendBytes: byte array) = Encoding.ASCII.GetBytes(message)
        printfn "Send <%s>" message
        try
            sendingClient.Send(sendBytes, sendBytes.Length, sendingIpEndPoint) |> ignore
        with
            | error -> eprintfn "%s" error.Message

    let rec loop(k : int) =
        let message = sprintf "%5d message" k
        send message
        // System.Threading.Thread.Sleep 250
        if k<300 then
            loop (k + 1)
        else
            ()

    [<EntryPoint>]
    let main argv =
        if argv.Length = 0 then
            on_error "You must specify a port number"
        sendPort <- str2int argv[0]
        printfn "Listening on port %d, bordel!!" sendPort
        loop 1
        0 // return an integer exit code
