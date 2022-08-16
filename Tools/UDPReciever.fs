(*
    Project:    ShTTY
    File name:  UDPReciver.fs
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

namespace UDPTools

module UDPRecieverTools =
    open System
    open System.Net
    open System.Net.Sockets
    open System.Text

    type private UDPReceiver =
        struct
            val mutable port: int
            val mutable client: UdpClient
            val mutable address: IPEndPoint
        end

    let mutable private receiver = new UDPReceiver ()

    let setPort(port : int) =
        receiver.port <- port
        receiver.client <- new UdpClient(port)
        receiver.address <- new IPEndPoint(IPAddress.Any, port)

    let receive () =
        let receivingClient = receiver.client
        let mutable receivingIpEndPoint = receiver.address
        try
            let receiveBytes = receivingClient.Receive(&receivingIpEndPoint)
            let returnData = Encoding.ASCII.GetString(receiveBytes)
            printfn "%s" returnData
        with
            | error -> eprintfn "%s" error.Message
