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
open System.Drawing
open System.Windows.Forms
open System.Drawing.Text
open StartStateMachines
open ExitStateMachine
open LogTools.Logger
open Tools
open BasicStuff
open D1Fonts
open D1Form
open FormsTools
open aboutForm

module main =
    let form =
        new DForm ( 500,  700, "D1 is my name")

    let createMenu (form: DForm) =
        let menu = new MenuStrip ()

        let menuAbout =
            newMenuEntry "&About" (fun _ _ -> showAboutForm ())

        let menuQuit =
            newMenuEntry "&Quit" (fun _ _ -> form.Close ())

        let fullMenu = new ToolStripDropDown ()
        fullMenu.Text <- "Menu"
        fullMenu.Items.Add menuAbout |> ignore
        fullMenu.Items.Add menuQuit |> ignore
        // bien compliqué, tout ça, hein? JE VEUX un truc SIMPLE pour faire un MENU!
        // does not work
        // menu.Items.Add fullMenu
        menu.Items.Add menuQuit |> ignore
        menu.Items.Add menuAbout |> ignore
        // form.MainMenuStrip <- menu
        form.addMenu menu
        ()

    let setUIStyleAndShow (panel: FlowLayoutPanel) (element: Control) : bool =
        element.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        panel.Controls.Add element
        true

    let mkNameLabel (name: string) : Label =
        let nameLabel = newLabel name :?> Label
        nameLabel.Anchor <- AnchorStyles.Left
        nameLabel.Font <- bigFont
        // casting, cf
        //   https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/casting-and-conversions
        nameLabel

    let mkValueLabel (value: string) : Label =
        let valueLabel =
            makeLabel value true :?> Label

        valueLabel.Anchor <- AnchorStyles.Right
        valueLabel.Font <- smallFont
        valueLabel

    let showValueR (name: string) (value: string) (panel: FlowLayoutPanel) =

        let nameLabel = mkNameLabel name
        let valueLabel = mkValueLabel value
        let lPanel = getTabPanel 2 1
        if isWindows() then
            lPanel.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        lPanel.Controls.Add nameLabel
        lPanel.Controls.Add valueLabel
        panel.Controls.Add lPanel
        (nameLabel, valueLabel)
    let showValue (name: string) (value: string) (panel: FlowLayoutPanel) =
        showValueR name value panel |> ignore
        ()

    let showValues (name: string) (values: string []) (panel: FlowLayoutPanel) =
        let nameLabel = mkNameLabel name
        panel.Controls.Add nameLabel

        let makeValueLabel (value: string) =
            let valueLabel = mkValueLabel value
            panel.Controls.Add valueLabel

        for v in values do
            makeValueLabel v

        ()

    let logFonts() =
        let iFontCollection = new InstalledFontCollection()
        let fontCollection = iFontCollection.Families
        for c in fontCollection do
            doLog (sprintf "%s %A" c.Name (c.IsStyleAvailable(FontStyle.Italic))) |> ignore
        ()


    [<EntryPoint>]
    let main argv =
        try
            openLog () |> ignore
            logFonts()
            // Windows: first argument is not the name of the program !!
            for a in argv do
                doLog a |> ignore
            form.initialize()
            let panel = form.getPanel()
            let getWinDim() = (sprintf "%d x %d" (form.Width()) (form.Height()))
            let _, yLab = showValueR "Windows dimension" (getWinDim()) panel
            // font: Name != OriginalName sie la font Name  n'existe pas
            //       il y a peut-être d'autres cas
            showValue "Font" (sprintf "%s - %s" smallFont.Name (smallFont.Style.ToString ())) panel
            showValue "" smallFont.OriginalFontName panel
            if smallFont.IsSystemFont then
                showValue "" smallFont.SystemFontName panel
            showValue "Font size/unit" (sprintf "%f / %s" smallFont.Size (smallFont.Unit.ToString())) panel

            showValue
                "Operating System"
                (sprintf
                    "%s (%s bits)"
                    (Environment.OSVersion.ToString ())
                    (if Environment.Is64BitOperatingSystem then
                         "64"
                     else
                         "32"))
                panel

            showValue "Machine" Environment.MachineName panel
            showValue "Processors" (sprintf "%d" Environment.ProcessorCount) panel
            showValue "Page size" (sprintf "%d" Environment.SystemPageSize) panel

            showValue "CLI Version" (Environment.Version.ToString ()) panel

            let getMemoryValue() = (sprintf "%d Ko" (Environment.WorkingSet / 1024L))
            let _, memLab = showValueR "Memory used by this process" (getMemoryValue()) panel

            showValue
                "64 bits process"
                (if Environment.Is64BitProcess then
                     "yes"
                 else
                     "no")
                panel

            showValue "Directory" Environment.CurrentDirectory panel

            showValue "System Directory" Environment.SystemDirectory panel
            showValues "Logical drives/Mount points" (Environment.GetLogicalDrives ()) panel

            showValue "User name" Environment.UserName panel
            showValue "User domain" Environment.UserDomainName panel

            let timer = new Timer()
            timer.Tick.Add (fun _ ->
                timer.Stop()
                memLab.Text <- (getMemoryValue())
                timer.Enabled <- true)
            timer.Interval <- 1000
            timer.Start()

            createMenu form
            form.addResize (fun _ ->
                memLab.Text <- (getMemoryValue())
                yLab.Text <- (getWinDim()))
            let onAppExit1 _ =
                doLog "onExit1" |> ignore
                ()

            Application.ApplicationExit.Add (fun args ->
                doLog (sprintf "Application.ApplicationExit %A" args)
                |> ignore
                onAppExit1 args)

            form.addClosed (fun args ->
                doLog (sprintf "Form.Closed %A" args) |> ignore
                onAppExit1 args)

            onStart () |> ignore
            form.run()

            if onExit () then
                (int SYSExit.Success)
            else
                (int SYSExit.Failure)
        finally
            doLog "Finally, exit!" |> ignore
            closeLog () |> ignore
