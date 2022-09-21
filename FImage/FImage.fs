(*
    Project:    FImage
    File name:  FImage.fs
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

open LogTools.Logger
open Tools.BasicStuff
open GUITools.Fonts
open GUITools.BaseControls
open GUITools.Menus
open GUITools.BasicForm

module main =
    let appName = "FSImage"
    let showAboutForm () =
        try
            let form = new AboutForm (330, 200, appName, (sprintf "%s, creating F# projects" appName))

            form.ShowDialog () |> ignore
        finally
            doLog "End of showAboutForm" |> ignore

    let createMenu (form: BasicForm) (pic: PictureBox) =

        let loadImage () =
            let ofd = new OpenFileDialog()
            ofd.DefaultExt <- "*.jpg"
            try
                ofd.Filter <- "Image Files (*.jpg)|*.JPG;*.JPEG"
                if isWindows() then
                    ofd.InitialDirectory <- "c:\\"
                else
                    ofd.InitialDirectory <- "./"
            with
            | :? ArgumentOutOfRangeException as ex ->
                doLog (sprintf "unexpected ArgumentOutOfRangeException %s" ex.Message) |> ignore
            | :? ArgumentNullException as ex ->
                doLog (sprintf "unexpected ArgumentNullException %s" ex.Message) |> ignore
            | :? ArgumentException as ex ->
                doLog (sprintf "unexpected ArgumentException %s" ex.Message) |> ignore
            | ex ->
                doLog (sprintf "unexpected exception %s" ex.Message) |> ignore
            ofd.FilterIndex <- 1
            let result = ofd.ShowDialog()
            if result = DialogResult.OK then
                let bmp=new System.Drawing.Bitmap(ofd.FileName)
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipNone)
                pic.Image <- bmp
                // pic.ImageLocation <- ofd.FileName
            ()

        let menu = new MenuBar ()
        menu.Font <- smallFont

        let menuHelp = new MenuHead ("&Help")

        let menuAbout =
            new MenuEntry ("&About", (fun _ _ -> showAboutForm ()))

        menuHelp.DropDownItems.Add menuAbout |> ignore

        let menuFile = new MenuHead ("&File")

        let menuOpen =
            new MenuEntryWithK ("&Open...",
                                (fun _ _ -> loadImage()),
                                Keys.Control ||| Keys.O)
        menuFile.DropDownItems.Add menuOpen |> ignore

        let menuQuit =
            new MenuEntryWithK ("&Quit",
                                (fun _ _ -> form.Close ()),
                                Keys.Control ||| Keys.Q)

        menuFile.DropDownItems.Add menuQuit |> ignore

        menu.AddHead menuFile
        menu.AddHead menuHelp
        menu.Attach form
        ()

    let addPictureBox (form: BasicForm) : PictureBox =
        let pic = new PictureBox()
        pic.SizeMode <- PictureBoxSizeMode.StretchImage     // CenterImage
        // pic.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        pic.Dock <- DockStyle.Fill
        pic.BorderStyle <- BorderStyle.Fixed3D
        pic.Width <- DEFAULT_WIDTH - 8
        pic.Height <- DEFAULT_HEIGHT - 8
        pic.Top <- 4
        pic.Left <- 4
        pic.BackColor <- Color.Coral
        form.addControl pic
        pic

    // [<EntryPoint>]
    let main argv =

        try
            openLog () |> ignore

            let form = new BasicForm(appName, StdTableLayoutPanel (1, 1))
            let pic = addPictureBox form
            createMenu form pic
            Application.Run form
            (int SYSExit.Success)
        with
        | :? System.InvalidOperationException as ex ->
            doLog (sprintf "unexpected exception %s" ex.Message) |> ignore
            (int SYSExit.Failure)

    [<STAThread>]
    do main("<nothing>")
