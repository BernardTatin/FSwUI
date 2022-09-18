(*
    Project:    GUITools
    File name:  BasicForm.fs
    User:       berna
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

module GUITools.BasicForm

open System.Windows.Forms
open GUITools.Fonts
open GUITools.BaseControls
open GUITools.Controls
open LogTools.Logger

let DEFAULT_WIDTH = 640
let DEFAULT_HEIGHT = 480


type BasicForm(width: int, height: int, title: string, panel: Panel) as self =
    inherit Form(Width = width, Height = height, Text = title)
    let mutable backPanel: Panel = panel
    let bottomTips = new BottomTips(self)

    let changeBackPanel(panel: Panel) =
        backPanel.Hide()
        panel.Dock <- DockStyle.Fill
        self.Controls.Add panel
        backPanel <- panel

    do
        self.Controls.Add panel
        self.Controls.Add bottomTips

    /// back panel getter
    /// <remarks>must disappear</remarks>
    member this.ThePanel
        with get () = backPanel
        and set panel =  changeBackPanel panel

    member this.Tips
        with get() = bottomTips
