(*
    Project:    GUITools
    File name:  Menus.fs
    User:       berna
    Date:       2022-09-19

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

module GUITools.Menus

open System
open System.Drawing
open System.Windows.Forms
open System.Drawing.Text
open LogTools.Logger
open GUITools.BaseControls

type MenuHead(text:string) as self =
    inherit ToolStripMenuItem (text)
    do
        self.Anchor <- AnchorStyles.Top

let internal defaultAmIEnabled() =
    true

type MenuEntry(text: string,
               onClick: ToolStripMenuItem -> EventArgs -> unit,
               amIEnabled: unit -> bool) as self =
    inherit MenuHead(text)
    do
        self.Click.Add (fun arg -> onClick self arg)
        self.Paint.Add (fun _ -> self.Enabled <- amIEnabled())
    new (text, onClick) = new MenuEntry(text, onClick, defaultAmIEnabled)

type MenuEntryWithK(text: string,
                    onClick: ToolStripMenuItem -> EventArgs -> unit,
                    shortCutK: Keys,
                    amIEnabled: unit -> bool) as self =
    inherit MenuEntry(text, onClick, amIEnabled)
    do
        self.ShortcutKeys <- shortCutK
    new (text, onClick, shortCutK) = new MenuEntryWithK(text, onClick, shortCutK, defaultAmIEnabled)


type MenuBar() as self =
    inherit MenuStrip()
    let mutable heads: MenuHead List  = []

    member this.AddHead head =
        heads <- heads @ [ head ]

    member this.Attach (form: Form) =
        let rec attachHeads (heads: MenuHead List) =
            match heads with
            | h :: t ->
                self.Items.Add h |> ignore
                attachHeads t
            | [] -> ()
        attachHeads heads
        form.Controls.Add self
