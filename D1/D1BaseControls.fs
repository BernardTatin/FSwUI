(*
    Project:    D1
    File name:  D1BaseControls.fs
    User:       berna
    Date:       2022-08-30

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

open System
open System.Drawing
open System.Windows.Forms
open Tools.BasicStuff
open FontTools

module D1BaseControls =
    type LabelAnchor =
        | None = 0
        | Left = 1
        | Right = 2

    type StdLabel(text: string) as self =
        inherit Label()
        do
            self.Text <- text
            self.AutoSize <- true
        new () = new StdLabel("")

        member this.DoAnchor(anchor: LabelAnchor) =
            match anchor with
            | LabelAnchor.Left ->
                self.Anchor <- AnchorStyles.Left
                self.Dock <- DockStyle.Left
            | LabelAnchor.Right ->
                self.Anchor <- AnchorStyles.Right
                self.Dock <- DockStyle.Right
            | _ ->
                ()

    type Label3D(text: string) as self =
        inherit StdLabel(text)
        do
            self.BorderStyle <- BorderStyle.Fixed3D
            self.BackColor <- Color.White
        new () = new Label3D("")
