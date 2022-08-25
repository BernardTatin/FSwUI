(*
    Project:    D1
    File name:  FormsTools.fs
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

module FormsTools =

    let newButton (text: string) (onClick: Button -> MouseEventArgs -> unit) : Control =
        let button = new Button ()
        button.Text <- text
        button.MouseClick.Add (fun (event: MouseEventArgs) -> onClick button event)
        button

    let newLabel (text: string) : Control =
        let label = new Label (Text = text)
        label

    let createPanel (form: Form) : FlowLayoutPanel =
        let panel = new FlowLayoutPanel ()

        // Works on Linux, not sure on Windows
        panel.Dock <- DockStyle.Fill
        panel.WrapContents <- false
        panel.FlowDirection <- FlowDirection.TopDown
        // panel.AutoSize <- true
        // panel.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right ||| AnchorStyles.Top ||| AnchorStyles.Bottom)
        form.Controls.Add panel
        panel

    let newMenuEntry (text: string) onClick =
        let menuEntry = new ToolStripButton (text)
        menuEntry.Click.Add (fun arg -> onClick menuEntry arg)
        menuEntry
