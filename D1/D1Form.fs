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
open GUITools.Fonts
open GUITools.BaseControls
open GUITools.Controls
open LogTools.Logger


module D1Form =

    let private DEFAULT_WIDTH = 640
    let private DEFAULT_HEIGHT = 480

    type DForm(width: int, height: int, title: string) as self =
        inherit Form(Width = width, Height = height, Text = title)

        let bottomTips = new BottomTips (self)

        /// the back panel which receive all the controls
        let mutable backPanel: Panel = new BackPanel (self)


        /// a resize callback to ensure the back panel is always of the good size
        /// <remarks>not sure it's useful</remarks>
        let basicResize () =
            bottomTips.RefreshText ()

            backPanel.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
            backPanel.Dock <- DockStyle.Fill
            doLog (sprintf "basic resize of %s %d %d" title self.Width self.Height)
            |> ignore

        let changeBackPanel(panel: Panel) =
            backPanel.Hide()
            panel.Dock <- DockStyle.Fill
            self.Controls.Add panel
            backPanel <- panel


        do
            self.Font <- smallFont
            self.Controls.Add bottomTips
            self.Resize.Add (fun _ -> basicResize ())


        new () = new DForm(DEFAULT_WIDTH, DEFAULT_HEIGHT, "<no tile!>")
        new (title: string) = new DForm(DEFAULT_WIDTH, DEFAULT_HEIGHT, title)

        /// add a control to the back panel
        /// <param name="control">the control to add</param>
        member this.addControl(control: Control) = backPanel.Controls.Add control

        /// add a menu to the form
        /// <remarks>bad stuff</remarks>
        member this.addMenu(menu) = self.Controls.Add menu

        /// back panel getter
        /// <remarks>must disappear</remarks>
        member this.Panel
            with get () = backPanel
            and set panel =  changeBackPanel panel

        /// set the form as a dialog box
        member this.setToDialog() =
            // Define the border style of the form to a dialog box.
            self.FormBorderStyle <- FormBorderStyle.FixedDialog
            // Set the MaximizeBox to false to remove the maximize box.
            self.MaximizeBox <- false
            // Set the MinimizeBox to false to remove the minimize box.
            self.MinimizeBox <- false
