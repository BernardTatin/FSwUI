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

namespace d1

module main =
    open System
    open System.Reflection
    open System.Windows.Forms

    let onClickForm (arg : EventArgs) (leControl : Control) =
        // show MouseEventArgs but the type of arg can't be MouseEventArgs
        let tArg = arg.GetType().FullName
        let tCont = leControl.GetType().FullName
        leControl.Text <- sprintf "%A %A" tArg tCont

    let setUIStyleAndShow (panel : FlowLayoutPanel) (element : Control) : bool =
        element.AutoSize <- true
        element.Click.Add (fun arg -> onClickForm arg element)
        panel.Controls.Add(element)
        true

    let createPanel (form : Form) : FlowLayoutPanel =
        let panel = new FlowLayoutPanel()
        form.Controls.Add(panel)
        // why = and not <- ?
        panel.Dock = DockStyle.Fill |> ignore 
        panel.WrapContents <- false 
        panel.FlowDirection <- FlowDirection.TopDown
        panel
        
    [<EntryPoint>]
    let main argv =
        let form = new Form(Width= 400, Height = 300, 
            Visible = true, 
            Text = "D1 is my name")
        let leHello = new Label(Text = "B'jour, toi!")
        let leTexte = new Label(Text="Oh!!")
        let leTPanel = new Label(Text="Aaaaaaaaaaaaah")
        let panel = createPanel form
        let uiElements = [ leHello; leTexte; leTPanel]

        // form.Click.Add (fun arg -> onClickForm arg leTexte)
        

        List.forall (fun e -> setUIStyleAndShow panel e) uiElements |> ignore

        Application.Run(form)

        0

