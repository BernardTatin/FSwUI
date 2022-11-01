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
open System.Drawing
open System.Windows
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
    new (title: string, panel: Panel) = new BasicForm(DEFAULT_WIDTH, DEFAULT_HEIGHT, title, panel)
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

type SimpleOKForm(width: int, height: int, title: string, panel: Panel) as self =
    inherit BasicForm(width, height, title, panel)
    let okButtonOnClick =
        (fun _ _ ->
            self.Close ())
    let okButton = new StdButton ("Ok", okButtonOnClick)
    member this.GetOKButton() = okButton


type FullOKForm(width: int, height: int, title: string, panel: Panel) as self =
    inherit BasicForm(width, height, title, panel)
    let mutable result = DialogResult.OK
    let cancelButtonOnClick =
        (fun _ _ ->
            result <- DialogResult.Cancel
            self.Close ())
    let okButtonOnClick =
        (fun _ _ ->
            result <- DialogResult.OK
            self.Close ())
    let cancelButton = new StdButton ("Cancel", cancelButtonOnClick)
    let okButton = new StdButton ("OK", okButtonOnClick)

    do
        self.CancelButton <- cancelButton
        self.AcceptButton <- okButton

    member this.GetOKButton() = okButton
    member this.GetCancelButton() = cancelButton
    member this.Result = result


type AboutForm(width: int, height: int, appName: string, text: string) as self =
    inherit SimpleOKForm(width, height, $"About {appName}", new StdTableLayoutPanel (1, 5))

    let createAddControl (control: Control) : bool =
        control.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        self.ThePanel.Controls.Add control
        true

    let year = DateTime.Now.Year

    let controls =
        [
            (new Label3D (text) :> Control)
            (new StdLabel ())
            new Label3D $"(c) {year}"
            new StdLabel ()
            self.GetOKButton()
        ]

    do
        self.ThePanel.Dock <- DockStyle.Fill
        List.forall createAddControl controls |> ignore

type HelpForm(width: int, height: int, appName: string, text: string) as self =
    inherit SimpleOKForm(width, height, $"A little help for {appName}", new StdTableLayoutPanel (1, 2))

    let createAddControl (control: Control) : bool =
        control.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        self.ThePanel.Controls.Add control
        true

    let helpBox = new Label3D (text)

    let controls =
        [
            (helpBox :> Control)
            new StdLabel ()
            self.GetOKButton()
        ]

    do
        self.ThePanel.Dock <- DockStyle.Fill
        // helpBox.text <- TextWrapping.WrapWithOverflow
        List.forall createAddControl controls |> ignore
