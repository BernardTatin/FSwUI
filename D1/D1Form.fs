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
    type DForm(width: int, height: int, title: string) as self =
        inherit Form(Width = width, Height = height, Text = title)

        let bottomTips =
            getTabPanel (if isUnix () then 2 else 3) 1

        let labTime = new Label ()
        let labSize = new Label ()
        let labMemory = new Label ()
        let timer = new Timer ()

        /// the back panel which receive all the controls
        let backPanel = createPanel (self)

        let refreshTips () =
            let now = DateTime.Now

            if (not (isUnix ())) then
                labMemory.Text <- (sprintf "%d Ko" (Environment.WorkingSet / 1024L))

            labSize.Text <- (sprintf "%4d x %4d" (self.Width) (self.Height))
            labTime.Text <- (sprintf "%02d:%02d:%02d" now.Hour now.Minute now.Second)

        /// a resize callback to ensure the back panel is always of the good size
        /// <remarks>not sure it's useful</remarks>
        let basicResize () =
            refreshTips ()

            doLog (sprintf "basic resize of %s %d %d" title self.Width self.Height)
            |> ignore

        let onTimerClick _ =
            timer.Stop ()
            refreshTips ()
            timer.Enabled <- true

        do
            self.Font <- smallFont
            self.Controls.Add bottomTips
            // bottomTips.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right ||| AnchorStyles.Bottom)
            bottomTips.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
            bottomTips.Dock <- DockStyle.Bottom
            bottomTips.BorderStyle <- BorderStyle.Fixed3D
            bottomTips.BackColor <- Color.White

            labTime.Dock <- DockStyle.Right
            labTime.Anchor <- AnchorStyles.Right
            labSize.Dock <- DockStyle.Left
            labSize.Anchor <- AnchorStyles.Left
            bottomTips.Controls.Add labSize

            if not (isUnix ()) then
                bottomTips.Controls.Add labMemory

            bottomTips.Controls.Add labTime

            labTime.Font <- smallerFont FontStyle.Bold
            labSize.Font <- smallerFont (FontStyle.Bold ||| FontStyle.Italic)

            if not (isUnix ()) then
                labMemory.Font <- smallerFont (FontStyle.Bold ||| FontStyle.Italic)


            refreshTips ()

            timer.Interval <- 1000
            timer.Tick.Add onTimerClick
            timer.Start ()

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
