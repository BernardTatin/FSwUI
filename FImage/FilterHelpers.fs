(*
    Project:    FImage
    File name:  FilterHelpers.fs
    User:       berna
    Date:       2022-11-03

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

module FSImage.FilterHelpers

let inline mMMin x y z = min (min x y) z
let inline mMMax x y z = max (max x y) z

let white = (byte 255, byte 255, byte 255)

let black = (byte 0, byte 0, byte 0)

let inline cutColLow (limit: byte) (r: byte, g: byte, b: byte) =
    let M = mMMax r g b
    let m = mMMin r g b

    if M <= limit then
        black
    else if r > limit && g <= r && b <= r then
        (r, m, m)
    else if g > limit && r <= g && b <= g then
        (m, g, m)
    else
        (m, m, b)

let inline cutColHigh (limit: byte) (r: byte, g: byte, b: byte) =
    let m = mMMax r g b

    if m <= limit then
        black
    else if r > limit && g <= r && b <= r then
        let n = max g b
        (r, n, n)
    else if g > limit && r <= g && b <= g then
        let n = max r b
        (n, g, n)
    else
        let n = max r g
        (n, n, b)
