(*
    Project:    FSImage
    File name:  FSImage.fs
    User:       bernard
    Date:       2022-09-15

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



open System
open System.Drawing
open System.Windows.Forms
open System.Drawing.Text
open System.IO
open System.Text

open LogTools.Logger
open Tools.BasicStuff
open GUITools.Fonts
open GUITools.BaseControls
open GUITools.Menus
open GUITools.BasicForm

open FSImage.FImNames
open FSImage.FImMenus
open FSImage.ImageLoad
open FSImage.ThePicture
open FSImage.helpers

module main =

    // [<EntryPoint>]
    let main argv =
        let form = new BasicForm(appName, new TableLayoutPanel3D (1, 2))
        let image = new ThePicture (form)

        try
#if LOGGER
            openLog () |> ignore
            doLog $"Running {appName}" |> ignore
#endif
            // form.Font <- smallFont
            createMenu form image

            form.Resize.Add (fun _ -> image.Resize())
            Application.Run form
            (int SYSExit.Success)
        with
        | :? System.InvalidOperationException as ex ->
#if LOGGER
            doLog (sprintf "unexpected exception %s" ex.Message) |> ignore
#endif
            (int SYSExit.Failure)

    [<STAThread>]
    do main("<nothing>") |> ignore
