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


open System
open System.Windows.Forms
open StartStateMachines
open ExitStateMachine
open LogTools.Logger
open Tools
open BasicStuff
open FormsTools
open aboutForm

module main =

    let onButtonClick (button: Button) (arg: MouseEventArgs) =
        let x = arg.X
        let y = arg.Y
        let message = (sprintf "(%4d %4d)" x y)
        doLog message |> ignore
        button.Text <- message


    let setUIStyleAndShow (panel: FlowLayoutPanel) (element: Control) : bool =
        element.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        panel.Controls.Add element
        true

    let createMenu (form: Form) =
        let menu = new MenuStrip ()

        let menuAbout =
            newMenuEntry "&About" (fun _ _ -> showAboutForm ())

        let menuQuit =
            newMenuEntry "&Quit" (fun _ _ -> form.Close ())

        let fullMenu = new ToolStripDropDown()
        fullMenu.Text <- "Menu"
        fullMenu.Items.Add menuAbout |> ignore
        fullMenu.Items.Add menuQuit |> ignore
        // bien compliqué, tout ça, hein? JE VEUX un truc SIMPLE pour faire un MENU!
        // does not work
        // menu.Items.Add fullMenu
        menu.Items.Add menuQuit |> ignore
        menu.Items.Add menuAbout |> ignore
        form.MainMenuStrip <- menu
        form.Controls.Add menu
        ()

    [<EntryPoint>]
    let main argv =
        try
            openLog () |> ignore
            // Windows: first argument is not the name of the program !!
            for a in argv do
                doLog a |> ignore

            let form =
                new Form (Width = 400, Height = 300, Visible = true)

            form.Text <- "D1 is my name"

            let panel = createPanel form

            let allControls =
                [
                    makeLabel (sprintf "OS: %s" (Environment.OSVersion.ToString())) true
                    makeLabel (sprintf "Machine:") false
                    makeLabel (sprintf "%s" Environment.MachineName) true
                    makeLabel (sprintf "Directory:") false
                    makeLabel (sprintf "%s" Environment.CurrentDirectory) true
                    makeLabel (sprintf "System Directory:") false
                    makeLabel (sprintf "%s" Environment.SystemDirectory) true
                ]

            createMenu form

            List.forall (fun e -> setUIStyleAndShow panel e) allControls
            |> ignore

            let onAppExit1 _ =
                doLog "onExit1" |> ignore
                ()

            Application.ApplicationExit.Add (fun args ->
                doLog (sprintf "Application.ApplicationExit %A" args)
                |> ignore

                onAppExit1 args)

            form.Closed.Add (fun args ->
                doLog (sprintf "Form.Closed %A" args) |> ignore
                onAppExit1 args)

            onStart () |> ignore
            Application.Run form

            if onExit () then
                (int SYSExit.Success)
            else
                (int SYSExit.Failure)
        finally
            doLog "Finaly, exit!" |> ignore
            closeLog () |> ignore
