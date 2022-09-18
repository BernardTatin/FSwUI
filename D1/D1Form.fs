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
open GUITools.BasicForm
open LogTools.Logger


module D1Form =

    type DForm(width: int, height: int, title: string) as self =
        inherit BasicForm(width, height, title, new BackPanel())
        // inherit Form(Width = width, Height = height, Text = title)


        /// a resize callback to ensure the back panel is always of the good size
        /// <remarks>not sure it's useful</remarks>
        let basicResize () =
            self.Tips.RefreshText ()

            self.ThePanel.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
            self.ThePanel.Dock <- DockStyle.Fill
            doLog (sprintf "basic resize of %s %d %d" title self.Width self.Height)
            |> ignore

        do
            self.Font <- smallFont
            self.Resize.Add (fun _ -> basicResize ())


        new () = new DForm(DEFAULT_WIDTH, DEFAULT_HEIGHT, "<no tile!>")
        new (title: string) = new DForm(DEFAULT_WIDTH, DEFAULT_HEIGHT, title)

        /// add a control to the back panel
        /// <param name="control">the control to add</param>
        member this.addControl(control: Control) = self.ThePanel.Controls.Add control

        /// add a menu to the form
        /// <remarks>bad stuff</remarks>
        member this.addMenu(menu) = self.Controls.Add menu

        /// set the form as a dialog box
        member this.setToDialog() =
            // Define the border style of the form to a dialog box.
            self.FormBorderStyle <- FormBorderStyle.FixedDialog
            // Set the MaximizeBox to false to remove the maximize box.
            self.MaximizeBox <- false
            // Set the MinimizeBox to false to remove the minimize box.
            self.MinimizeBox <- false
