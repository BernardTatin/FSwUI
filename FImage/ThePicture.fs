(*
    Project:    FImage
    File name:  ThePicture.fs
    User:       berna
    Date:       2022-09-23

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

module FSImage.ThePicture

open System
open System.Drawing
open System.Windows.Forms
open System.Drawing.Text
open System.IO
open System.Text

open LogTools.Logger
open Tools.BasicStuff
open GUITools.Fonts
open GUITools.BaseControls
open GUITools.Menus
open GUITools.BasicForm

open FSImage.FImNames
open FSImage.ImageLoad
open FSImage.helpers

type ThePicture(form: BasicForm) as self =
    let mutable currentImageFile = ""
    let mutable imageLoaded = false
    let mutable bmp = new System.Drawing.Bitmap(20, 20)
    let pic = new PictureBox()
    let imageProps = new Label3D ("")
    let resizePicture () =
        let delta1 = 16
        let delta2 = 2 * delta1
        pic.Width <- form.Width - delta2 - 200
        pic.Height <- form.Height - delta2 - (4 * form.Tips.Height)
        pic.Top <- delta1 + form.Tips.Height
        pic.Left <- delta1

    do
        pic.SizeMode <- PictureBoxSizeMode.Zoom     // CenterImage
        // pic.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        // pic.Dock <- DockStyle.Fill
        pic.BorderStyle <- BorderStyle.Fixed3D
        resizePicture ()
        pic.Image <- new System.Drawing.Bitmap(pic.Width, pic.Height)
        imageProps.Font <- smallFont
        imageProps.AutoSize <- true

        form.addControl pic
        form.addControl imageProps

    member this.GetPicture() = pic
    member this.GetBMP() = pic.Image
    member this.ImageProps() = imageProps
    member this.OnNewImage(filePath: string) =
        let fileName = (getBaseName filePath)
        let fi: FileInfo = new FileInfo(filePath)
        bmp <- new System.Drawing.Bitmap(filePath)
        bmp.RotateFlip(RotateFlipType.RotateNoneFlipNone)
        pic.Image.Dispose()
        pic.Image <- bmp
        form.Text <- (sprintf "%s - %s" appName fileName)
        currentImageFile <- filePath
        imageProps.Text <- (sprintf "%s - %d Ko %d x %d pixels %s"
                                fileName (fi.Length / 1024L)
                                (bmp.Width) (bmp.Height)
                                (bmp.PixelFormat.ToString()))
        imageLoaded <- true
        ()
    member this.LoadImage() =
        let (filePath: string, ok: bool) = loadImage ()

        if ok then
            self.OnNewImage  filePath
        ()

    member this.Resize() = resizePicture()
    member this.Rotate () =
        if imageLoaded then
           bmp.RotateFlip(RotateFlipType.Rotate90FlipNone)
           pic.Image <- bmp
        ()

    member this.ShiftColorsLeft() =
        if imageLoaded then
            doLog "ShiftColorsLeft..." |> ignore
            for x in 0..(bmp.Width-1) do
                for y in 0..(bmp.Height-1) do
                    let c: Color = bmp.GetPixel (x, y)
                    let nc = Color.FromArgb(int c.A, int c.B, int c.R, int c.G)
                    bmp.SetPixel (x, y, nc)
            pic.Image <- bmp
            doLog "ShiftColorsLeft OK" |> ignore
            ()
        else
            ()
