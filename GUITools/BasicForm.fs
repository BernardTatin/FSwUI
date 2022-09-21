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

open System
open System.Windows.Forms
open GUITools.BaseControls
open GUITools.Controls
open LogTools.Logger

let DEFAULT_WIDTH = 640
let DEFAULT_HEIGHT = 480


type BasicForm(width: int, height: int, title: string, panel: Panel) as self =
    inherit Form(Width = width, Height = height, Text = title)
    let mutable backPanel: Panel = panel
    let bottomTips = new BottomTips (self)

    let changeBackPanel (panel: Panel) =
        backPanel.Hide ()
        panel.Dock <- DockStyle.Fill
        self.Controls.Add panel
        backPanel <- panel
    do
        self.Controls.Add panel
        self.Controls.Add bottomTips

    new (width: int, height: int, title: string) = new BasicForm(width, height, title, new BackPanel())
    new (title: string) = new BasicForm(DEFAULT_WIDTH, DEFAULT_HEIGHT, title, new BackPanel())
    /// back panel getter
    /// <remarks>must disappear</remarks>
    member this.ThePanel
        with get () = backPanel
        and set panel = changeBackPanel panel

    member this.Tips = bottomTips


    /// add a control to the back panel
    /// <param name="control">the control to add</param>
    member this.addControl(control: Control) = self.ThePanel.Controls.Add control

    /// add a menu to the form
    /// <remarks>bad stuff</remarks>
    member this.addMenu(menu) = self.Controls.Add menu

    member this.setToDialog() =
        // Define the border style of the form to a dialog box.
        self.FormBorderStyle <- FormBorderStyle.FixedDialog
        // Set the MaximizeBox to false to remove the maximize box.
        self.MaximizeBox <- false
        // Set the MinimizeBox to false to remove the minimize box.
        self.MinimizeBox <- false

type AboutForm(width: int, height: int, appName: string, text: string) as self =
    inherit BasicForm(width, height, (sprintf "About %s" appName), new StdTableLayoutPanel (1, 5))

    let createAddControl (control: Control) : bool =
        control.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        // control.Dock <- DockStyle.Fill
        // control.Size <- Size (panel.Size.Width, control.MaximumSize.Height)
        self.ThePanel.Controls.Add control
        true

    let okButtonOnClick =
        (fun _ _ ->
            doLog "about Form: click OK button" |> ignore
            self.Close ())

    let year = DateTime.Now.Year

    let controls =
        [
            (new Label3D (text) :> Control)
            (new StdLabel ())
            new Label3D (sprintf "(c) %d" year)
            new StdLabel ()
            (new StdButton ("Ok", okButtonOnClick))
        ]

    do
        let bPanel = self.ThePanel
        bPanel.Dock <- DockStyle.Fill
        List.forall createAddControl controls |> ignore
