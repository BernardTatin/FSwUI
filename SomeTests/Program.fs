(*
    Project:    SomeTests
    File name:  Program.fs
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

(*
    With Windows:
    It works with the 'Developer Command Prompt' but NOT with PowerShell!
    I don't know why!
 *)
open System


[<EntryPoint>]
let main argv =
    // let write v = System.IO.File.AppendAllText("test.txt", v + "\n")
    let write v = printfn "%s" v
    try
        write "Starting a Test 2"
        // not sure if the arg.Cancel has the same behavior and on Linux
        System.Console.CancelKeyPress.Add (fun arg ->
            write "CancelKeyPress"
            arg.Cancel <- false )
        System.AppDomain.CurrentDomain.ProcessExit.Add (fun _ -> write "ProcessExit" )
        System.AppDomain.CurrentDomain.DomainUnload.Add (fun _ -> write "DomainUnload" )
        // let onExit = new ConsoleCancelEventHandler(fun _ args -> Console.WriteLine("Exit"); closing.Set() |> ignore)
        // let onExit = new ConsoleCancelEventHandler(fun _ args -> write "\nExit")
        // Console.CancelKeyPress.AddHandler onExit
        write "Waiting for Ctrl+C !!!!!"
        // let r = Console.ReadKey()
        let r = System.Console.In.ReadLine()

        write (sprintf "J'ai lu: <%A>" r)
    finally
        write "Try Finally"
    0
