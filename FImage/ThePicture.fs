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

open System.Drawing
open System.Windows.Forms
open System.Drawing.Imaging
open System.IO

open LogTools.Logger
open GUITools.Fonts
open GUITools.BaseControls
open GUITools.BasicForm

open FSImage.FImNames
open FSImage.ImageLoad
open FSImage.helpers
open FSImage.BMPStates
open FSImage.BitmapTools

type ThePicture(form: BasicForm) =
    let mutable bmpState: BMPState =
        BMPState.NothingToSee

    let mutable currentImageFile = ""

    let mutable bmp =
        new Bitmap (20, 20)

    let pic = new PictureBox ()
    let imageProps = new Label3D ("")
    let setBitmap (newBMP: Bitmap) =
        bmp.Dispose ()
        bmp <- newBMP
        bmp.RotateFlip RotateFlipType.RotateNoneFlipNone
        pic.Image <- bmp

    let bmpToPixels () =
        ()

    let pixelsToBMP () =
        ()

    let isReady() = bmpState = BMPState.Ready
    let doFilter (f: byte*byte*byte -> byte*byte*byte) (message: string) =
        if isReady() then
#if DEBUG
            doLog $"{message}..." |> ignore
#endif
            let pix = new LockContext(bmp)
            pix.ForEach f
            pix.Unlock()
            pic.Image <- bmp
#if DEBUG
            doLog $"{message} OK" |> ignore
#endif
            ()
        else
#if DEBUG
            doLog $"Cannot {message}..." |> ignore
#endif
            ()

    let rec changeState (newState: BMPState) : bool =
        let result =
            match newState with
            | NothingToSee ->
                if bmpState <> BMPState.NothingToSee then
                    setBitmap (new Bitmap (20, 20))
                bmpState <- newState
                true
            | NewBMPFromFile ->
                setBitmap (new Bitmap (currentImageFile))
                bmpState <- newState
                changeState NewPixFromBMP
            | NewPixFromBMP ->
                bmpToPixels ()
                bmpState <- newState
                changeState Ready
            | NewBMPFromPix ->
                pixelsToBMP()
                bmpState <- newState
                changeState Ready
            | Ready ->
                bmpState <- newState
                true
            | DirtyBMP ->
                if bmpState = Ready then
                    bmpState <- newState
                    changeState NewPixFromBMP
                else
                    false
            | DirtyPixels ->
                if bmpState = Ready then
                    bmpState <- newState
                    changeState NewBMPFromPix
                else
                    false

        result


    let resizePicture () =
        let delta1 = 16
        let delta2 = 2 * delta1
        pic.Width <- form.Width - delta2 - 200
        pic.Height <- form.Height - delta2 - (4 * form.Tips.Height)
        pic.Top <- delta1 + form.Tips.Height
        pic.Left <- delta1

    let time f =
        let sw = System.Diagnostics.Stopwatch()
        sw.Start()
        let res = f()
        sw.Stop()
        (res, sw.Elapsed.TotalMilliseconds)
    let onNewImage(filePath: string) =
        let fileName = (getBaseName filePath)
        let fi: FileInfo = FileInfo filePath
        currentImageFile <- filePath
        changeState BMPState.NewBMPFromFile |> ignore
        form.Text <- (sprintf "%s - %s" appName fileName)

        imageProps.Text <-
            (sprintf
                "%s - %d Ko %d x %d pixels %s"
                fileName
                (fi.Length / 1024L)
                bmp.Width
                bmp.Height
                (bmp.PixelFormat.ToString ()))

        ()

    let reloadImage() =
        changeState BMPState.NewBMPFromFile |> ignore

    do
        pic.SizeMode <- PictureBoxSizeMode.Zoom // CenterImage
        pic.BorderStyle <- BorderStyle.Fixed3D
        resizePicture ()
        setBitmap (new Bitmap (pic.Width, pic.Height))
        imageProps.Font <- smallFont
        imageProps.AutoSize <- true

        form.addControl pic
        form.addControl imageProps



    member this.GetPicture() = pic
    member this.GetBMP() = pic.Image
    member this.ImageProps() = imageProps


    member this.LoadImage() =
        let (filePath: string, ok: bool) =
            loadImage ()

        if ok then onNewImage filePath
        ()

    member this.ReLoadImage() =
#if DEBUG
        let _, t0 = time reloadImage
        doLog $"ReLoadImage {t0}" |> ignore
#else
        reloadImage()
#endif

    member this.Resize() = resizePicture ()

    member this.Rotate() =
        if isReady() then
#if DEBUG
            doLog "Rotate..." |> ignore
#endif
            bmp.RotateFlip RotateFlipType.Rotate90FlipNone
            pic.Image <- bmp
            changeState BMPState.DirtyBMP |> ignore
#if DEBUG
            doLog "Rotate OK" |> ignore
        else
            doLog (sprintf "Cannot Rotate... state: %A" bmpState) |> ignore
#endif
        ()

    member this.ShiftColorsLeft() =
#if DEBUG
        let f() = doFilter (fun (r, g, b) -> (g, b, r)) "ShiftColorsRight"
        let _, t = time f
        doLog $"ShiftColorsLeft {t}" |> ignore
#else
        doFilter (fun (r, g, b) -> (g, b, r)) "ShiftColorsRight"
#endif

    member this.ShiftColorsRight() =
#if DEBUG
        let f() = doFilter (fun (r, g, b) -> (b, r, g)) "ShiftColorsRight"
        let _, t = time f
        doLog $"ShiftColorsRight {t}" |> ignore
#else
        doFilter (fun (r, g, b) -> (b, r, g)) "ShiftColorsRight"
#endif

    member this.RawBW(limit: byte) =
        let white = (byte 255, byte 255, byte 255)
        let black = (byte 0, byte 0, byte 0)
        let cutCol(r: byte, g: byte, b: byte) =
            if r>limit then
                white
            else if g>limit then
                white
            else if b>limit then
                white
            else
                black

#if DEBUG
        let _, t0 = time (fun() -> doFilter cutCol "RawBW")
        doLog $"RawBW: {t0}" |> ignore
#else
        doFilter cutCol "RawBW"
#endif

    member this.CutColors(limit: byte) =
        let white = (byte 255, byte 255, byte 255)
        let black = (byte 0, byte 0, byte 0)
        let cutCol(r: byte, g: byte, b: byte) =
            if r>limit && g>limit && b >limit then
                white
            else if r>limit && g<=r && b<=r then
                (r, byte 0, byte 0)
            else if g>limit && r<=g && b<=g then
                (byte 0, g, byte 0)
            else if b>limit && r<=b && g<=b then
                (byte 0, byte 0, b)
            else
                black
#if DEBUG
        let _, t0 = time (fun() -> doFilter cutCol "CutColors")
        doLog $"CutColors: {t0}" |> ignore
#else
        doFilter cutCol "CutColors"
#endif
