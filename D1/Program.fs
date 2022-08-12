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

open d1.StartStateMachines
open d1.SystemTools

module main =
    open System
    open System.Reflection
    open System.Windows.Forms
    open StartStateMachines
    open ExitStateMachine
    open Logger
    open SystemTools


    let newButton text : Control =
        let button = new Button ()
        button.Text <- text
        button

    let newLabel (text: string) : Control =
        let label = new Label (Text = text)
        label

    let setUIStyleAndShow (panel: FlowLayoutPanel) (element: Control) : bool =
        let onClick (arg: MouseEventArgs) =
            // show MouseEventArgs but the type of arg can't be MouseEventArgs
            let tCont = arg.GetType().FullName
            let x = arg.X
            let y = arg.Y
            element.Text <- sprintf "%A (%4d %4d)" tCont x y

        element.MouseClick.Add (onClick)
        element.AutoSize <- true
        panel.Controls.Add (element)

        true

    let createPanel (form: Form) : FlowLayoutPanel =
        let panel = new FlowLayoutPanel ()

        // why = and not <- ?
        panel.Dock = DockStyle.Fill |> ignore
        panel.WrapContents <- false
        panel.FlowDirection <- FlowDirection.TopDown
        panel.AutoSize <- true
        form.Controls.Add (panel)
        panel

    [<EntryPoint>]
    let main argv =
        openLog "theLog.log" |> ignore

        let form =
            new Form (Width = 400, Height = 300, Visible = true, Text = "D1 is my name")

        let panel = createPanel form

        let allControls =
            [
                newLabel ("Ohhhh!")
                newButton ("Un bouton")
                newLabel ("Ohhhh!")
                newButton ("Encore un bouton")
                newLabel ("Ohhhh!")
            ]

        List.forall (fun e -> setUIStyleAndShow panel e) allControls
        |> ignore

        onStart () |> ignore
        Application.Run (form)
        closeLog () |> ignore

        if onExit () then
            (int SYSExit.Success)
        else
            (int SYSExit.Failure)
