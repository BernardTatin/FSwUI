(*
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
open Microsoft.FSharp.NativeInterop

#if LOGGER
open LogTools.Logger
#endif

type private OData = BitmapData option

type LockContext(bitmap:Bitmap) =
     let mutable data = OData.None

     let mutable isLocked = false

     let mutable bitmapLenInBytes = 0

     let unlockTheBits() =
#if LOGGER
        doLog $"Unlock bits isLocked = {isLocked}" |> ignore
#endif
        if isLocked then
           bitmap.UnlockBits(data.Value)
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
     let lockTheBits() =
        if not isLocked then
           data <- Some(bitmap.LockBits(Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                ImageLockMode.ReadOnly,
                                bitmap.PixelFormat))
           isLocked <- true
           bitmapLenInBytes <- bitmap.Width * bitmap.Height
           match data.Value.PixelFormat with
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

#if RECURSEBM
     let forEachPixels (f: byte*byte*byte -> byte*byte*byte) =
        let rec loop idx k =
          if k < bitmapLenInBytes then
             let address = NativePtr.add<byte> (NativePtr.ofNativeInt data.Value.Scan0) idx
             setRGB address (f (getRGB address))
             loop (idx + sizeofColor) (k + 1)
          else
             ()
        loop 0 0

     let meanTone() =
        let rec loop (acc: int64) idx k =
          if k < bitmapLenInBytes then
             let address = NativePtr.add<byte> (NativePtr.ofNativeInt data.Value.Scan0) idx
             let r, g, b = getRGB address
             // let m:int64 = (int64 r) + (int64 g) + (int64 b)
             loop (acc + (int64 r) + (int64 g) + (int64 b)) (idx + sizeofColor) (k + 1)
          else
             acc / (3L * (int64 bitmapLenInBytes))
        loop 0L 0 0
#else
     let forEachPixels (f: byte*byte*byte -> byte*byte*byte) =
        let mutable idx = 0
        for k = 0 to bitmapLenInBytes - 1 do
          let address = NativePtr.add<byte> (NativePtr.ofNativeInt data.Value.Scan0) idx
          setRGB address (f (getRGB address))
          idx <- idx + sizeofColor
        ()

     let meanTone() =
        let mutable idx = 0
        let mutable acc = 0L
        for k = 0 to bitmapLenInBytes - 1 do
             let address = NativePtr.add<byte> (NativePtr.ofNativeInt data.Value.Scan0) idx
             let r, g, b = getRGB address
             acc <- acc + (int64 r) + (int64 g) + (int64 b)
        acc / (3L * (int64 bitmapLenInBytes))
#endif

     member this.ForEach f =
        forEachPixels f

     member this.With f =
         lockTheBits()
         f this
         unlockTheBits()

     member this.getMeanTone() =
        meanTone()

     member this.Unlock() =
        unlockTheBits()

     interface IDisposable with
        member this.Dispose() =
            unlockTheBits()
