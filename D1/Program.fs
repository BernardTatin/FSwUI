(*
 * D1/Program.fs
 * see:
 *      https://stackoverflow.com/questions/50373607/how-can-i-enable-a-winforms-or-wpf-project-in-form
 *
 * Steps:
 *  - Create a new console application in F#,
 *  - Select the solution properties,
 *      - change "Output type" in the "Application" page from "Console application" to "Windows Application"
 *  - Right click on "References" in the project
 *      - add reference to "System.Windows.Forms" and "System.Drawing" (in the "Framework" tab).
 *
 * Etapes
 *  - Créer une application console en form#,
 *  - Afficher les propriétés de la solutions
 *      - changer "Type de Sortie" de "Application Console" en "Application Windows"
 *  - Click droit sur les "Dépendances" de la solution,
 *      - ajouter les  "référence d'assembly" -> "Framework":
 *          - "System.Windows.Forms"
 *          - "System.Drawing"
 *)
open System
open System.Windows.Forms

let setUIStyleAndShow (panel : FlowLayoutPanel) (element : Control) : bool =
    element.AutoSize <- true
    panel.Controls.Add(element)
    true


let form = new Form(Width= 400, Height = 300, Visible = true, Text = "Hello World")
let leHello = new Label(Text = "Oh!")
let leTexte = new Label(Text="Hello world!")
let uiElements = [ leHello; leTexte ]

form.Click.Add (fun _ -> 
    leTexte.Text <- sprintf "form clicked at %i" DateTime.Now.Ticks)
let panel = new FlowLayoutPanel()
form.Controls.Add(panel)
// why = and not <- ?
panel.Dock = DockStyle.Fill |> ignore 
panel.WrapContents <- false 
panel.FlowDirection <- FlowDirection.TopDown

List.forall (fun e -> setUIStyleAndShow panel e) uiElements |> ignore


Application.Run(form)

