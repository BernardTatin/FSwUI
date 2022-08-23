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

open Tools


module main =
    open System
    open System.Reflection
    open System.Windows.Forms
    open StartStateMachines
    open ExitStateMachine
    open LogTools.Logger
    open BasicStuff


    let newButton text : Control =
        let button = new Button ()
        button.Text <- text
        button

    let newLabel (text: string) : Control =
        let label = new Label (Text = text)
        label

    let setUIStyleAndShow (panel: FlowLayoutPanel) (element: Control) : bool =
        let onClick (arg: MouseEventArgs) =
            let x = arg.X
            let y = arg.Y
            element.Text <- sprintf "(%4d %4d)" x y

        element.MouseClick.Add (onClick)
        element.AutoSize <- true
        // element.Dock <- DockStyle.Fill
        element.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        panel.Controls.Add (element)

        true

    let createPanel (form: Form) : FlowLayoutPanel =
        let panel = new FlowLayoutPanel ()

        // Works on Linux, not sure on Windows
        panel.Dock <- DockStyle.Fill
        panel.WrapContents <- false
        panel.FlowDirection <- FlowDirection.TopDown
        // panel.AutoSize <- true
        // panel.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right ||| AnchorStyles.Top ||| AnchorStyles.Bottom)
        form.Controls.Add (panel)
        panel

    [<EntryPoint>]
    let main argv =
        try
            openLog () |> ignore
            // Windows: first argument is not the name of the program !!
            for a in argv do
                doLog a |> ignore

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

            let onAppExit1 _ =
                doLog "onExit1"
                ()
            Application.ApplicationExit.Add (fun args ->
                doLog (sprintf "Application.ApplicationExit %A" args) |> ignore
                onAppExit1 args)
            form.Closed.Add (fun args ->
                doLog (sprintf "Form.Closed %A" args) |> ignore
                onAppExit1 args)
            onStart () |> ignore
            Application.Run (form)

            if onExit () then
                (int SYSExit.Success)
            else
                (int SYSExit.Failure)
        finally
            doLog "Finaly, exit!"
            closeLog () |> ignore
