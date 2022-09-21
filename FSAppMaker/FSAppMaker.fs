(*
    Project:    FSAppMaker
    File name:  FSAppMaker.fs
    User:       bernard
    Date:       2022-09-16

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

namespace FSAppMaker

open System
open System.Drawing
open System.Windows.Forms
open System.Drawing.Text
open LogTools.Logger
open Tools.BasicStuff
open GUITools.Fonts
open GUITools.BaseControls
open GUITools.Menus
open GUITools.BasicForm

module main =
    let appName = "FSAppMaker"
    let showAboutForm () =
        try
            let form = new AboutForm (330, 200, appName, (sprintf "%s, creating F# projects" appName))

            form.ShowDialog () |> ignore
        finally
            doLog "End of showAboutForm" |> ignore
    let createMenu (form: BasicForm) =
        let menu = new MenuBar ()
        menu.Font <- smallFont

        let menuHelp = new MenuHead ("&Help")

        let menuAbout =
            new MenuEntry ("&About", (fun _ _ -> showAboutForm ()))

        menuHelp.DropDownItems.Add menuAbout |> ignore

        let menuFile = new MenuHead ("&File")

        let menuQuit =
            new MenuEntryWithK ("&Quit",
                                (fun _ _ -> form.Close ()),
                                Keys.Control ||| Keys.Q)

        menuFile.DropDownItems.Add menuQuit |> ignore

        menu.AddHead menuFile |> ignore
        menu.AddHead menuHelp |> ignore
        menu.Attach form
        ()

    [<EntryPoint>]
    let main argv =
        try
            openLog () |> ignore
            let form = new BasicForm(appName)
            createMenu form
            Application.Run form
            (int SYSExit.Success)
        with
        | :? System.InvalidOperationException as ex ->
            doLog (sprintf "unexpected exception %s" ex.Message) |> ignore
            (int SYSExit.Failure)
