(*
    Project:    FImage
    File name:  DlgRawBW.fs
    User:       berna
    Date:       2022-11-01

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

module FSImage.DlgRawBW

open System.Windows.Forms
open GUITools.BaseControls
open GUITools.BasicForm
open FSImage.ThePicture

type ByteLevelConfiguration(title, onChange: byte -> unit) as self =
    inherit FullOKForm(DEFAULT_WIDTH / 2,
                       DEFAULT_HEIGHT / 2,
                       title,
                       new StdTableLayoutPanel(1, 3))

    let slider = new TrackBar()
    let sliderValue = new LabeledValue("Level", "127")
    let createAddControl (control: Control) : bool =
        control.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        self.ThePanel.Controls.Add control
        true
    let onSliderChange() =
        let v = slider.Value
        onChange (byte v)
        sliderValue.Value <- $"{v}"

    let controls =
        [
            sliderValue :> Control
            slider :> Control
            self.GetOKBar()
        ]
    do
        slider.Minimum <- 0
        slider.Maximum <- 255
        slider.TickFrequency <- 16
        slider.SmallChange <- 2
        slider.LargeChange <- 10
        slider.Value <- 120
        onSliderChange()
        slider.Scroll.Add (fun _ -> onSliderChange())

        self.ThePanel.Dock <- DockStyle.Fill
        List.forall createAddControl controls |> ignore

        self.setToDialog()

type RawBWConfiguration(image: ThePicture) =
    inherit ByteLevelConfiguration( "Raw Black and white",
                                    (fun v -> image.RawBW v))

type CutColorsConfiguration(image: ThePicture) =
    inherit ByteLevelConfiguration( "Raw Black and white",
                                    (fun v -> image.CutColors v))


let private showByteLC (image: ThePicture) (form: ByteLevelConfiguration) =
    form.ShowDialog() |> ignore
    if form.Result <> DialogResult.OK then
        image.ReLoadImage()
    else
        image.AcceptBitmap()
    ()


let ShowRawBWDlg(image: ThePicture) =
    let form = new RawBWConfiguration(image)
    showByteLC image form

let ShowRawCutColorsDlg(image: ThePicture) =
    let form = new CutColorsConfiguration(image)
    showByteLC image form
