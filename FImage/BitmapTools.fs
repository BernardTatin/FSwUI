﻿(*
    Project:    FImage
    File name:  BitmapTools.fs
    User:       berna
    Date:       2022-09-27

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

(*
   From:
      http://www.mikeobrien.net/blog/high-performance-image-fun-with-f
 *)
module FSImage.BitmapTools

open System
open System.Drawing
open System.Drawing.Imaging
 open System.Windows.Media
open Microsoft.FSharp.NativeInterop

 open LogTools.Logger

type LockContext(bitmap:Bitmap) =
     let data = bitmap.LockBits(Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                ImageLockMode.ReadOnly,
                                bitmap.PixelFormat)

     let mutable isLocked = true

     let unlockTheBits() =
#if DEBUG
        doLog $"Unlock bits isLocked = {isLocked}" |> ignore
#endif
        if isLocked then
           bitmap.UnlockBits(data)
           isLocked <- false
        else
           ()

     let formatNotSupportedMessage = "BitmapTools: Pixel format not supported."
     let getRGB24 address =
       (NativePtr.get address 2,
                      NativePtr.get address 1,
                      NativePtr.read address)
     let setRGB24 address (r, g, b) =
        NativePtr.set address 2 r
        NativePtr.set address 1 g
        NativePtr.write address b

     let getRGB32 address =
       (NativePtr.get address 2,
                      NativePtr.get address 1,
                      NativePtr.read address)
     let setRGB32 address (r, g, b) =
        NativePtr.set address 2 r
        NativePtr.set address 1 g
        NativePtr.write address b
     let mutable sizeofColor = 3
     let mutable getRGB = getRGB24
     let mutable setRGB = setRGB24
     do
        match data.PixelFormat with
        | PixelFormat.Format24bppRgb ->
           sizeofColor <- 3
           setRGB <- setRGB24
           getRGB <- getRGB24
        | PixelFormat.Format32bppArgb
        | PixelFormat.Format32bppPArgb
        | PixelFormat.Format32bppRgb ->
           sizeofColor <- 4
           setRGB <- setRGB32
           getRGB <- getRGB32
        | _ -> failwith formatNotSupportedMessage


     let forEachPixels (f: byte*byte*byte -> byte*byte*byte) =
        let len = ((bitmap.Width * bitmap.Height) - 1)
        let rec loop idx k =
          let address = NativePtr.add<byte> (NativePtr.ofNativeInt data.Scan0) idx
          setRGB address (f (getRGB address))
          if k = len then
             ()
          else
             loop (idx + sizeofColor) (k + 1)
        loop 0 0

     member this.ForEach f =
        forEachPixels f

     member this.Unlock() =
         unlockTheBits()

     interface IDisposable with
        member this.Dispose() =
            unlockTheBits()
