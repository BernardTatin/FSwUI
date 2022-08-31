(*
    Project:    D1
    File name:  D1Controls.fs
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

namespace GUITools

open System
open System.Drawing
open System.Windows.Forms
open Tools.BasicStuff
open GUITools.FontTools
open GUITools.BaseControls

module Controls =

    type BottomTips(form: Form) as self =
        inherit TableLayoutPanel3D(3, 1)
        let labTime = new Label3D ()
        let labSize = new Label3D ()
        let labMemory = new Label3D ()
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
            // the order is very important: Anchor first, Dock second!
            self.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
            self.Dock <- DockStyle.Bottom

            labTime.DoAnchor LabelAnchor.Right
            labSize.DoAnchor LabelAnchor.Left

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
