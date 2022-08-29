(*
    Project:    D1
    File name:  FontTools.fs
    User:       berna
    Date:       2022-08-27

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

open System.Drawing
open Tools.BasicStuff
module FontTools =

    let smallFontSize = 9.0F
    let defaultFontSize = 10.0F
    let biggerFontSize = 12.0F

    let defaultFontName(defaultName: string) : string =
        match getOSFamily() with
        | OSFamily.Windows ->
            "Calibri"
        | OSFamily.Unix ->
            "Fira Sans"
        | _ ->
            defaultName

    let newFont(fontName: string) (size: float32) (style: FontStyle) : Font =
        let font = new Font(fontName, size, style)
        if fontName <> font.Name then
            new Font(defaultFontName(fontName), size, style)
        else
            font

    let defaultFont (size: float32) (style: FontStyle) : Font =
        newFont (defaultFontName "") size style

    let smallerFont (style: FontStyle) : Font =
        defaultFont smallFontSize style

    let normalFont (style: FontStyle) : Font =
        defaultFont defaultFontSize style

    let biggerFont (style: FontStyle) : Font =
        defaultFont biggerFontSize style
