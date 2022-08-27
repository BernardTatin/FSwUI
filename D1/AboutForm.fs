(*
    Project:    D1
    File name:  AboutForm.fs
    User:       bernard
    Date:       2022-08-24

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
open LogTools.Logger
open D1Fonts
open FormsTools

module aboutForm =

    let showAboutForm () =
        try
            let form = new Form (Width = 330, Height = 200, Text = "About D1")

            let width = form.Size.Width

            form.Font <- smallFont
            let panel = createPanel form
            panel.Width <- width
            let addControl (control: Control) : bool =
                control.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
                control.Dock <- DockStyle.Fill
                panel.Controls.Add control
                true
            // because of the form.AcceptButton, had to do this...
            let okButtonOnClick =
                (fun _ _ ->
                    doLog "about Form: click OK button" |> ignore
                    form.Close ())

            let controls =
                [
                    makeLabel "D1, for the fun of Windows Forms in F#" true
                    makeLabel "" false
                    makeLabel "(c) 2022" true
                    makeLabel "" false
                    newButton "Ok" okButtonOnClick
                ]

            doLog "Start of showAboutForm" |> ignore
            // Define the border style of the form to a dialog box.
            form.FormBorderStyle <- FormBorderStyle.FixedDialog
            // Set the MaximizeBox to false to remove the maximize box.
            form.MaximizeBox <- false
            // Set the MinimizeBox to false to remove the minimize box.
            form.MinimizeBox <- false

            List.forall addControl controls |> ignore
            form.ShowDialog () |> ignore
        finally
            doLog "End of showAboutForm" |> ignore
