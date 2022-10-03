(*
    Project:    FImage
    File name:  ImageLoad.fs
    User:       bernard
    Date:       2022-09-22

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

module FSImage.ImageLoad

open System
open System.Windows.Forms

open LogTools.Logger

open FSImage.helpers

let mutable private startDir = getPicturesDir()

let loadImage () =
    let ofd = new OpenFileDialog()
    ofd.DefaultExt <- "*.jpg"
    try
        ofd.Filter <- "Image Files (*.jpg;*.png)|*.jpg;*.jpeg;*.JPG;*.JPEG;*.png;*.PNG|All files (*.*)|*.*"
        ofd.InitialDirectory <- startDir
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
        startDir <- getDirName (ofd.FileName)
        (ofd.FileName, true)
    else
        ("", false)

let saveImage (fileName: string) =
    let sfd = new SaveFileDialog()
    sfd.DefaultExt <- "*.jpg"
    sfd.Filter <- "Image Files (*.jpg;*.png)|*.jpg;*.jpeg;*.JPG;*.JPEG;*.png;*.PNG|All files (*.*)|*.*"
    sfd.InitialDirectory <- startDir
    let result = sfd.ShowDialog()
    if result = DialogResult.OK then
        (sfd.FileName, true)
    else
        ("", false)
