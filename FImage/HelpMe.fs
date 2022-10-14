(*
    Project:    FImage
    File name:  HelpMe.fs
    User:       berna
    Date:       2022-10-02

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

module FSImage.HelpMe

open GUITools.BasicForm

let internal helpText = """
Un peu d'aide

Ctrl+O: ouvrir un fichier image (jpeg et png)
Ctrl+S: sauver un fichier; si l'image s'appelle 'image.jpg',
     sauvegarde sous le nom de 'image-001.jpg'
Ctrl+Q: quitter l'appliication

Le menu 'Edit' propose plusieurs filtres amusants.
"""

let showHelp (appName: string) =
    let form = new HelpForm(DEFAULT_WIDTH, DEFAULT_HEIGHT,
                            appName, helpText)
    form.ShowDialog() |> ignore
