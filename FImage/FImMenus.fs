(*
    Project:    FSImage
    File name:  FImMenus.fs
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

module FSImage.FImMenus


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

open FSImage.FImNames
open FSImage.helpers
open FSImage.ImageLoad
open FSImage.ThePicture
open FSImage.HelpMe
open FSImage.DlgRawBW

let showAboutForm () =
    try
        let form = new AboutForm (330, 200, appName, appText)

        form.ShowDialog () |> ignore
    finally
        doLog "End of showAboutForm" |> ignore

type TMenuHead (text)  =
    let mutable subs: MenuEntry List = []
    member this.AddEntry(entry) =
        subs <- subs @ [ entry ]
        ()
    member this.AttachToBar (bar: MenuBar) =
        let m = new MenuHead (text)
        for e in subs do
            m.DropDownItems.Add e |> ignore
        bar.AddHead m


let createMenu (form: BasicForm) (image: ThePicture) =


    let menu = new MenuBar ()
    menu.Font <- smallFont



    let menuEdit = new TMenuHead ("&Filters")
    menuEdit.AddEntry (new MenuEntryWithK ("&Rotate",
                                         (fun _ _ -> image.Rotate()),
                                         Keys.Control ||| Keys.R,
                                         image.IsReady))
    menuEdit.AddEntry (new MenuEntryWithK ("Shift Color Right",
                                         (fun _ _ -> image.ShiftColorsRight()),
                                         Keys.Control ||| Keys.T,
                                         image.IsReady))
    menuEdit.AddEntry (new MenuEntryWithK ("Shift Color Left",
                                         (fun _ _ -> image.ShiftColorsLeft()),
                                         Keys.Control ||| Keys.L,
                                         image.IsReady))
    menuEdit.AddEntry (new MenuEntryWithK ("Cut Colors low (63)",
                                         (fun _ _ -> ShowRawCutColorsDlg image),
                                         Keys.Control ||| Keys.X,
                                         image.IsReady))
    menuEdit.AddEntry (new MenuEntryWithK ("Raw Black and White",
                                         (fun _ _ -> ShowRawBWDlg image),
                                         Keys.Control ||| Keys.N,
                                         image.IsReady))

    let menuFile = new TMenuHead("&File")
    menuFile.AddEntry (new MenuEntryWithK ("&Open...",
                            (fun _ _ -> image.LoadImage()),
                            Keys.Control ||| Keys.O))
    menuFile.AddEntry (new MenuEntryWithK ("&Reload...",
                            (fun _ _ -> image.ReLoadImage()),
                            Keys.Control ||| Keys.D,
                            image.IsModified))
    menuFile.AddEntry (new MenuEntryWithK ("&Save...",
                            (fun _ _ -> image.SaveImage()),
                            Keys.Control ||| Keys.S,
                            image.IsModified))
    menuFile.AddEntry (new MenuEntryWithK ("&Quit",
                            (fun _ _ -> form.Close ()),
                            Keys.Control ||| Keys.Q))

    let menuHelp = new TMenuHead("&Help")
    menuHelp.AddEntry (new MenuEntryWithK ("&Help", (fun _ _ -> showHelp appName), Keys.F1))
    menuHelp.AddEntry (new MenuEntry ("&About", (fun _ _ -> showAboutForm ())))

    menuFile.AttachToBar menu
    menuEdit.AttachToBar menu
    menuHelp.AttachToBar menu

    menu.Attach form
    ()
