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
open StartStateMachines
open ExitStateMachine
open LogTools.Logger
open Tools
open FormsTools

module aboutForm =
    let showAboutForm () =
        try
            let form = new Form(Text = "About D1")
            let panel = createPanel form
            let addControl (control: Control) : bool =
                control.AutoSize <- true
                control.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
                panel.Controls.Add control
                true
            // because of the form.AcceptButton, had to do this...
            let okButton = new Button()
            okButton.Text <- "OK"
            okButton.Click.Add (fun _ ->
                doLog "about Form: click OK button" |> ignore
                form.Close())
            let controls =
                [
                    newLabel "D1, for the fun of Windows Forms in F#"
                    okButton
                ]
            doLog "Start of showAboutForm" |> ignore
            List.forall addControl controls |> ignore
            form.AcceptButton <- okButton
            form.ShowDialog () |> ignore
        finally
            doLog "End of showAboutForm" |> ignore
