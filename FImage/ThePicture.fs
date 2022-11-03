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
open System.IO

#if LOGGER
open LogTools.Logger
#endif
open GUITools.Fonts
open GUITools.BaseControls
open GUITools.BasicForm

open FSImage.FImNames
open FSImage.ImageLoad
open FSImage.helpers
open FSImage.BMPStates
open FSImage.BitmapTools
open FSImage.FilterHelpers

#if LOGGER
let time f =
    let sw = System.Diagnostics.Stopwatch ()
    sw.Start ()
    let res = f ()
    sw.Stop ()
    (res, sw.Elapsed.TotalMilliseconds)
#endif


type ThePicture (form: BasicForm) =
    let mutable bmpState: BMPState =
        BMPState.NothingToSee

    let mutable currentImageFile = ""

    let mutable bmp = new Bitmap (20, 20)

    let mutable context = OLockContext.None

    let pic = new PictureBox ()
    let imageProps = new Label3D ("")

    let setBitmap (newBMP: Bitmap) =
        bmp.Dispose ()
        bmp <- newBMP
        bmp.RotateFlip RotateFlipType.RotateNoneFlipNone
        pic.Image <- bmp

    let isReady () = bmpState <> BMPState.NothingToSee
    let isModified () = bmpState = BMPState.Modified

    let rec changeState (newState: BMPState) : bool =
        match newState with
        | NothingToSee ->
            if bmpState <> BMPState.NothingToSee then
                setBitmap (new Bitmap (20, 20))

            bmpState <- NothingToSee
            true
        | NewBMPFromFile ->
            context <- None
            setBitmap (new Bitmap (currentImageFile))
            bmpState <- NewBMPFromFile
            changeState Loaded
        | Loaded ->
            bmpState <- Loaded
            true
        | Modified ->
            if bmpState = Loaded then
                bmpState <- Modified

            newState <> NothingToSee

    let getContext () =
        match context with
        | Some _ -> context.Value
        | None ->
            context <- Some (new LockContext (bmp))
            context.Value

    let openContext () =
        let ctx = getContext ()

        ctx.Lock ()
        ctx

    let closeContext (ctx: LockContext) = ctx.Unlock ()

    let resetRGBBuffer () =
        if context <> None then
            let ctx = openContext ()
            ctx.ResetRGBBuffer ()
            ctx.Unlock ()

    let withContext doIt =
        let ctx = openContext ()
        doIt ctx
        closeContext ctx

    let getMeanTone (pix: LockContext) = pix.getMeanTone ()

    let doFilter (pix: LockContext) (f: byte * byte * byte -> byte * byte * byte) =
        if isReady () then
            pix.ForEach f
            pic.Image <- bmp
            changeState BMPState.Modified |> ignore

        ()


    let resizePicture () =
        let delta1 = 16
        let delta2 = 2 * delta1
        pic.Width <- form.Width - delta2 - 200
        pic.Height <- form.Height - delta2 - (4 * form.Tips.Height)
        pic.Top <- delta1 + form.Tips.Height
        pic.Left <- delta1

    let onNewImage (filePath: string) =
        let fileName = (getBaseName filePath)
        let fi: FileInfo = FileInfo filePath
        currentImageFile <- filePath
        changeState BMPState.NewBMPFromFile |> ignore
        form.Text <- $"{appName} - {fileName}"

        imageProps.Text <-
            $"{fileName} - {fi.Length / 1024L} Ko {bmp.Width} x {bmp.Height} pixels {bmp.PixelFormat.ToString ()}"

        ()

    let onSaveImage (filePath: string) =
        resetRGBBuffer ()
        bmp.Save filePath
        changeState BMPState.Loaded |> ignore

    let reloadImage () =
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


    member this.ResetBitmap () =
        if context <> None then
            let ctx = openContext()
            ctx.ResetBitmap()
            closeContext ctx

    member this.ResetRGBBuffers () = resetRGBBuffer ()
    member this.IsReady () = isReady ()
    member this.IsModified () = isModified ()

    member this.LoadImage () =
        let (filePath: string, ok: bool) =
            loadImage ()

        if ok then onNewImage filePath
        ()

    member this.SaveImage () =
        if isReady () then
            let (filePath: string, ok: bool) =
                saveImage currentImageFile

            if ok then onSaveImage filePath

        ()

    member this.ReLoadImage () =
#if LOGGER
        let _, t0 = time reloadImage
        doLog $"ReLoadImage {t0}" |> ignore
#else
        reloadImage ()
#endif

    member this.Resize () = resizePicture ()

    member this.Rotate () =
        if isReady () then
            bmp.RotateFlip RotateFlipType.Rotate90FlipNone
            pic.Image <- bmp
            changeState BMPState.Modified |> ignore
            resetRGBBuffer ()

        ()

    member this.ShiftColorsLeft () =
        let doIt (context: LockContext) =
#if LOGGER
            let f () =
                doFilter context (fun (r, g, b) -> (g, b, r))

            let _, t = time f
            doLog $"ShiftColorsLeft {t}" |> ignore
#else
            doFilter context (fun (r, g, b) -> (g, b, r))
#endif
        withContext doIt

    member this.ShiftColorsRight () =
        let doIt (context: LockContext) =
#if LOGGER
            let f () =
                doFilter context (fun (r, g, b) -> (b, r, g))

            let _, t = time f
            doLog $"ShiftColorsRight {t}" |> ignore
#else
            doFilter context (fun (r, g, b) -> (b, r, g))
#endif
        withContext doIt

    member this.RawBW (limit: byte) =
        let cutCol (r: byte, g: byte, b: byte) =
            if r > limit then white
            else if g > limit then white
            else if b > limit then white
            else black

        let doIt (context: LockContext) =
#if LOGGER
            let _, t0 =
                time (fun () -> doFilter context cutCol)

            doLog $"RawBW {limit}: {t0}" |> ignore
#else
            doFilter context cutCol
#endif
        withContext doIt

    member this.CutColors (limit: byte) =
        let cutCol = cutColLow limit

        let doIt (context: LockContext) =
#if LOGGER
            let _, t0 =
                time (fun () -> doFilter context cutCol)

            doLog $"CutColors {limit}: {t0}" |> ignore
#else
            doFilter context cutCol
#endif
        withContext doIt

    member this.CutColorsMeanLow () =
        let doIt (context: LockContext) =
            let limit = getMeanTone context
            let cutCol = cutColLow (byte limit)
#if LOGGER
            let _, t0 =
                time (fun () -> doFilter context cutCol)

            doLog $"CutColorsMeanLow {limit}: {t0}" |> ignore
#else
            doFilter context cutCol
#endif
        withContext doIt

    member this.CutColorsMeanHigh () =
        let doIt (context: LockContext) =
            let limit = getMeanTone context
            let cutCol = cutColHigh (byte limit)
#if LOGGER
            let _, t0 =
                time (fun () -> doFilter context cutCol)

            doLog $"CutColorsMeanHigh {limit}: {t0}" |> ignore
#else
            doFilter context cutCol
#endif
        withContext doIt
