(*
    Project:    FSImage
    File name:  FSImage.fs
    User:       bernard
    Date:       2022-09-15

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
open FSImage.FImMenus
open FSImage.ImageLoad
open FSImage.helpers

module main =

    let resizePicture (form: BasicForm) (pic: PictureBox) =
        let delta1 = 16
        let delta2 = 2 * delta1
        pic.Width <- form.Width - delta2 - 200
        pic.Height <- form.Height - delta2 - (4 * form.Tips.Height)
        pic.Top <- delta1 + form.Tips.Height
        pic.Left <- delta1

    let createPictureBox (form: BasicForm) : PictureBox =
        let pic = new PictureBox()
        pic.SizeMode <- PictureBoxSizeMode.Zoom     // CenterImage
        // pic.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        // pic.Dock <- DockStyle.Fill
        pic.BorderStyle <- BorderStyle.Fixed3D
        resizePicture form pic
        pic.Image <- new System.Drawing.Bitmap(pic.Width, pic.Height)
        pic

    // [<EntryPoint>]
    let main argv =
        let form = new BasicForm(appName, new TableLayoutPanel3D (2, 2))
        let mutable currentImage = ""
        let imageProps = new Label3D (currentImage)
        let loadShowImage (pic: PictureBox) =
            let (filePath: string, ok: bool) = loadImage ()

            if ok then
                let fileName = (getBaseName filePath)
                let fi: FileInfo = new FileInfo(filePath)
                let bmp = new System.Drawing.Bitmap(filePath)
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipNone)
                pic.Image.Dispose()
                pic.Image <- bmp
                form.Text <- (sprintf "%s - %s" appName fileName)
                currentImage <- filePath
                imageProps.Text <- (sprintf "%s - %d Ko %d x %d pixels"
                                        fileName (fi.Length / 1024L)
                                        (bmp.Width) (bmp.Height) )
            ()

        try
            openLog () |> ignore

            // form.Font <- smallFont
            imageProps.Font <- smallFont
            imageProps.AutoSize <- true
            let pic = createPictureBox form
            createMenu form (fun () -> loadShowImage pic)
            form.addControl pic
            form.addControl (new StdLabel("Filtering"))
            form.addControl imageProps

            form.Resize.Add (fun _ -> resizePicture form pic)
            Application.Run form
            (int SYSExit.Success)
        with
        | :? System.InvalidOperationException as ex ->
            doLog (sprintf "unexpected exception %s" ex.Message) |> ignore
            (int SYSExit.Failure)

    [<STAThread>]
    do main("<nothing>") |> ignore
