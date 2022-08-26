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
open System.Drawing
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

    let setUIStyleAndShow (panel: FlowLayoutPanel) (element: Control) : bool =
        element.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        panel.Controls.Add element
        true


    [<EntryPoint>]
    let main argv =
        try
            openLog () |> ignore
            // Windows: first argument is not the name of the program !!
            for a in argv do
                doLog a |> ignore

            let form =
                new Form (Width = 400, Height = 600, Visible = true)

            form.Text <- "D1 is my name"

            let panel = createPanel form
            let formFont = form.Font
            let bigFontSize = formFont.Size + 2.0F
            let smallFontSize = formFont.Size
            let bigFont = new Font("Fira Sans", bigFontSize, FontStyle.Italic)
            let smallFont = new Font("Fira Sans", smallFontSize, FontStyle.Regular)
            form.Font <- smallFont

            let mkNameLabel (name : string) : Label =
                let nameLabel = newLabel name
                nameLabel.Anchor <- AnchorStyles.Left
                nameLabel.Font <- bigFont
                // casting, cf
                //   https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/casting-and-conversions
                nameLabel :?> Label

            let mkValueLabel (value: string) : Label =
                let valueLabel = makeLabel value true
                valueLabel.Anchor <- AnchorStyles.Right
                valueLabel.Font <- smallFont
                valueLabel :?> Label

            let showValue (name: string) (value: string) =

                let nameLabel = mkNameLabel name
                let valueLabel = mkValueLabel value
                let lpanel = getTabPanel 2 1
                lpanel.Controls.Add nameLabel
                lpanel.Controls.Add valueLabel
                panel.Controls.Add lpanel
                ()

            let showValues (name: string) (values: string[]) =
                let nameLabel = mkNameLabel name
                panel.Controls.Add nameLabel
                let makeValueLabel (value: string) =
                    let valueLabel = mkValueLabel value
                    panel.Controls.Add valueLabel

                for v in values do
                    makeValueLabel v

                ()


            showValue "Font" (sprintf "%s - %s" bigFont.Name (bigFont.Style.ToString()))
            showValue "Operating System" (sprintf "%s (%s bits)"
                                              (Environment.OSVersion.ToString())
                                              (if Environment.Is64BitOperatingSystem then "64" else "32"))
            showValue "Machine" Environment.MachineName
            showValue "Processors" (sprintf "%d" Environment.ProcessorCount)
            showValue "Page size" (sprintf "%d" Environment.SystemPageSize)

            showValue "CLI Version" (Environment.Version.ToString())

            showValue "Memory used by this process" (sprintf "%d" Environment.WorkingSet)
            showValue "64 bits process" (if Environment.Is64BitProcess then "yes" else "no")
            showValue "Directory" Environment.CurrentDirectory

            showValue "System Directory" Environment.SystemDirectory
            showValues "Logical drives/Mount points" (Environment.GetLogicalDrives())

            showValue "User name" Environment.UserName
            showValue "User domain" Environment.UserDomainName


            createMenu form

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
