(*
    Project:    D1
    File name:  D1Form.fs
    User:       bernard
    Date:       2022-08-28

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
open System.Windows.Forms
open D1Fonts
open FormsTools
open LogTools.Logger


module D1Form =

(*
    I CANNOT do that
*)
(*
    type DForm(width: int, height: int, title: string) =
        inherit Form()
        let backPanel = getHzPanel()
        let basicResize() =
            backPanel.Width <- base.Width
            backPanel.Height <- base.Height
        do
            base.Width <- width
            base.Height <- height
            base.Text <- title
            base.Font <- smallFont
            base.Resize.Add (fun _ ->
                basicResize())
*)
    type DForm(width: int, height: int, title: string) =
        let form = new Form(Width=width, Height=height, Text=title)

        let backPanel = createPanel(form)
        let basicResize() =
            backPanel.Width <- form.Width
            backPanel.Height <- form.Height
            doLog (sprintf "basic resize of %s %d %d" title form.Width form.Height) |> ignore
        member this.initialize() =
            form.Font <- smallFont
            form.Resize.Add (fun _ -> basicResize())
        member this.addControl (control: Control) =
            backPanel.Controls.Add control
        member this.Close() =
            form.Close()
        member this.addMenu(menu) = form.Controls.Add menu
        member this.getPanel() = backPanel
        member this.Width() = form.Width
        member this.Height() = form.Height
        member this.addResize f = form.Resize.Add f
        member this.addClosed f = form.Closed.Add f
        member this.run() = Application.Run form
        member this.setToDialog() =
            // Define the border style of the form to a dialog box.
            form.FormBorderStyle <- FormBorderStyle.FixedDialog
            // Set the MaximizeBox to false to remove the maximize box.
            form.MaximizeBox <- false
            // Set the MinimizeBox to false to remove the minimize box.
            form.MinimizeBox <- false
        member this.ShowDialog() =
            form.ShowDialog()
