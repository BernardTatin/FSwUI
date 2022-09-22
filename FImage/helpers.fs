(*
    Project:    FImage
    File name:  helpers.fs
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

module FSImage.helpers

open System

open LogTools.Logger
open Tools.BasicStuff

let dirSep = if isWindows() then
                '\\'
             else
                 '/'
let getDirName (str: string) =
    let parts = str.Split  dirSep
    let mutable l = parts.Length
    if l > 1 then
        let mutable result = ""
        for i in 0..l-2 do
            if i = 0 then
                result <- parts[i]
            else
                result <- (sprintf "%s%c%s" result dirSep parts[i])
        result
    else
        "."

let getHome () =
    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
