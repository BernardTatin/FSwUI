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
open System.Drawing
open LogTools.Logger
open D1Form
open d1.D1BaseControls

module aboutForm =

    let showAboutForm () =
        try
            let form = new DForm (330, 200, "About D1")

            let panel = new StdTableLayoutPanel (1, 5)
            panel.Dock <- DockStyle.Fill
            // Controls are here (ok button  activated by <Space> or <Enter>
            // but they are not visible
            // form.Panel <- panel
            form.addControl panel

            let addControl (control: Control) : bool =
                control.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
                // control.Dock <- DockStyle.Fill
                control.Size <- Size (panel.Size.Width, control.MaximumSize.Height)
                // form.addControl control
                panel.Controls.Add control
                true
            // because of the form.AcceptButton, had to do this...
            let okButtonOnClick =
                (fun _ _ ->
                    doLog "about Form: click OK button" |> ignore
                    form.Close ())

            let controls =
                [
                    (new Label3D ("D1, for the fun of Windows Forms in F#") :> Control)
                    (new StdLabel ())
                    new Label3D ("(c) 2022")
                    new StdLabel ()
                    (new StdButton ("Ok", okButtonOnClick))
                ]

            doLog "Start of showAboutForm" |> ignore

            List.forall addControl controls |> ignore

            form.ShowDialog () |> ignore
        finally
            doLog "End of showAboutForm" |> ignore
