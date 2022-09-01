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
open LogTools.LogTypes

let write v = printfn "%s" v
let defaultName: string = "<uninitialized>"
let defaultValue: int = 0
/// a class
type ClassBase(newName, newValue) =
    let name: string = newName
    let mutable value: int = newValue

    new () = ClassBase(defaultName, defaultValue)
    new ((s: string)) = ClassBase(s, defaultValue)
    new ((i: int)) = ClassBase(defaultName, i)

    member this.Value with get() = value and set(v)= value <- v

    member this.show (className: string) =
        printfn "%s(%s, %d)"  className name value
    abstract member showMe: unit -> unit
    default this.showMe() =
        this.show "ClassBase"

type Class1(name, value) as self =
    inherit ClassBase(name, value)
    new () = Class1(defaultName, defaultValue)
    new ((s: string)) = Class1(s, defaultValue)
    new ((i: int)) = Class1(defaultName, i)
    override this.showMe() =
        self.Value <- self.Value + 100
        this.show "Class1"


let testMe() =
    let all = [
        new ClassBase()
        new ClassBase ("b1", 1)
        new Class1()
        new Class1("c1", 11)
    ]
    for v in all do
        v.showMe()


type LogSample(name: string, log: LogBase, loops: int) =
    do
        log.start() |> ignore
    let rec test (k: int) =
        log.write (sprintf "%3d:%s" k name) |> ignore
        if k > 0 then
            test (k - 1)
        else
            ()
    member this.run() =
        write (sprintf "runnin test %3d:%s" loops name)
        test loops


[<EntryPoint>]
let main argv =
    // let write v = System.IO.File.AppendAllText("test.txt", v + "\n")
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
        // let r = System.Console.In.ReadLine()
        // write (sprintf "J'ai lu: <%A>" r)
        testMe()

        let t = new  LogSample("Console", new LogConsole(), 5)
        t.run()
        let t = new  LogSample("File t.txt", new LogTextFile("t.txt"), 5)
        t.run()
        let t = new  LogSample("UDP 2345", new LogUDP(2345), 5)
        t.run()

    finally
        write "Try Finally"
    0
