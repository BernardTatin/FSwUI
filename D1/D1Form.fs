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

open System
open System.Drawing
open System.Windows.Forms
open Tools.BasicStuff
open D1Fonts
open FormsTools
open FontTools
open LogTools.Logger


module D1Form =

    (*
    I CANNOT do that
*)
    type BackPanel(form: Form) as self =
        inherit FlowLayoutPanel()
        do
            // Works on Linux, not sure on Windows
            self.Dock <- DockStyle.Fill
            self.WrapContents <- false
            self.FlowDirection <- FlowDirection.TopDown
            // for debug purpose
            // self.BackColor <- Color.Crimson
            form.Controls.Add self

    type BottomTips(form: Form) as self =
        inherit TableLayoutPanel()
        let labTime = new Label ()
        let labSize = new Label ()
        let labMemory = new Label ()
        let timer = new Timer ()
        let refreshTips () =
            let now = DateTime.Now

            if (isUnix ()) then
                labMemory.Text <- ""
            else
                labMemory.Text <- (sprintf "%d Ko" (Environment.WorkingSet / 1024L))

            labSize.Text <- (sprintf "%4d x %4d" (form.Width) (form.Height))
            labTime.Text <- (sprintf "%02d:%02d:%02d" now.Hour now.Minute now.Second)
        let onTimerClick _ =
            timer.Stop ()
            refreshTips ()
            timer.Enabled <- true

        do
            self.AutoSize <- true
            self.ColumnCount <- 3
            self.RowCount <- 1
            // the order is very important: Anchor first, Dock second!
            self.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
            self.Dock <- DockStyle.Bottom
            self.BorderStyle <- BorderStyle.Fixed3D
            self.BackColor <- Color.White

            labTime.Dock <- DockStyle.Right
            labTime.Anchor <- AnchorStyles.Right
            labSize.Dock <- DockStyle.Left
            labSize.Anchor <- AnchorStyles.Left

            self.Controls.Add labSize
            self.Controls.Add labMemory
            self.Controls.Add labTime

            labTime.Font <- smallerFont FontStyle.Bold
            labSize.Font <- smallerFont (FontStyle.Bold ||| FontStyle.Italic)
            labMemory.Font <- smallerFont (FontStyle.Bold ||| FontStyle.Italic)

            refreshTips ()

            timer.Interval <- 1000
            timer.Tick.Add onTimerClick
            timer.Start ()

        member this.RefreshText() =
            refreshTips()

    type DForm(width: int, height: int, title: string) as self =
        inherit Form(Width = width, Height = height, Text = title)

        let bottomTips = new BottomTips(self)

        /// the back panel which receive all the controls
        let backPanel = new BackPanel (self)


        /// a resize callback to ensure the back panel is always of the good size
        /// <remarks>not sure it's useful</remarks>
        let basicResize () =
            bottomTips.RefreshText ()
            doLog (sprintf "basic resize of %s %d %d" title self.Width self.Height)
            |> ignore


        do
            self.Font <- smallFont
            self.Controls.Add bottomTips
            self.Resize.Add (fun _ -> basicResize ())

        /// add a control to the back panel
        /// <param name="control">the control to add</param>
        member this.addControl(control: Control) = backPanel.Controls.Add control

        /// add a menu to the form
        /// <remarks>bad stuff</remarks>
        member this.addMenu(menu) = self.Controls.Add menu

        /// back panel getter
        /// <remarks>must disappear</remarks>
        member this.Panel = backPanel

        /// set the form as a dialog box
        member this.setToDialog() =
            // Define the border style of the form to a dialog box.
            self.FormBorderStyle <- FormBorderStyle.FixedDialog
            // Set the MaximizeBox to false to remove the maximize box.
            self.MaximizeBox <- false
            // Set the MinimizeBox to false to remove the minimize box.
            self.MinimizeBox <- false
