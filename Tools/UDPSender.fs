(*
    Project:    Tools
    File name:  UDPSender.fs
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

open System.Net
open System.Text
open UDPTools.UDPBase

module UDPSenderTools =

    type UDPSender(newPort: int) as self =
        inherit UDPBase(IPAddress.Loopback, newPort, UDPDirection.Receiver)

        member this.send(message: string) =
            let sClient = self.Client
            let sAddress = self.EndPoint

            let (sendBytes: byte array) =
                Encoding.ASCII.GetBytes message

            try
                sClient.Send (sendBytes, sendBytes.Length, sAddress)
                |> ignore
            with
                | error -> eprintfn "ERROR UDPSender: %s" error.Message

    let mutable private sender =
        new UDPSender (1)

    let setPort (port: int) = sender <- new UDPSender (port)

    let send (message: string) = sender.send message
