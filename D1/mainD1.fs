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
open Tools.BasicStuff
open GUITools.Fonts
open GUITools.BaseControls
open GUITools.Menus
open D1Form
open aboutForm

module main =
    let form =
        new DForm (500, 700, "D1 is my name")

    let createMenu (form: DForm) =
        let menu = new MenuBar ()
        menu.Font <- smallFont

        let menuHelp = new MenuHead ("&Help")

        let menuAbout =
            new MenuEntry ("&About", (fun _ _ -> showAboutForm ()))

        menuHelp.DropDownItems.Add menuAbout |> ignore

        let menuFile = new MenuHead ("&File")

        let menuQuit =
            new MenuEntryWithK ("&Quit",
                                (fun _ _ -> form.Close ()),
                                Keys.Control ||| Keys.Q)

        menuFile.DropDownItems.Add menuQuit |> ignore

        menu.AddHead menuFile |> ignore
        menu.AddHead menuHelp |> ignore
        menu.Attach form
        ()

    let mkNameLabel (name: string) : Label =
        // casting, cf
        //   https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/casting-and-conversions
        let nameLabel = new StdLabel (name)
        nameLabel.DoAnchor LabelAnchor.Left
        nameLabel.Font <- bigFont
        nameLabel :> Label

    let mkValueLabel (value: string) : Label =
        let valueLabel = new Label3D(value)
        valueLabel.DoAnchor LabelAnchor.Right
        valueLabel.Font <- smallFont
        valueLabel :> Label

    let showValueR (name: string) (value: string) (panel: Panel) =

        let nameLabel = mkNameLabel name
        let valueLabel = mkValueLabel value
        let lPanel = new StdTableLayoutPanel (2, 1)

        if isWindows () then
            lPanel.Anchor <- (AnchorStyles.Left ||| AnchorStyles.Right)
        lPanel.Dock <- DockStyle.None
        lPanel.AutoSize <- true
        lPanel.Controls.Add nameLabel
        lPanel.Controls.Add valueLabel
        panel.Controls.Add lPanel
        (nameLabel, valueLabel)

    let showValue (name: string) (value: string) (panel: Panel) =
        showValueR name value panel |> ignore
        ()

    let showValues (name: string) (values: string []) (panel: Panel) =
        let nameLabel = mkNameLabel name
        panel.Controls.Add nameLabel

        let makeValueLabel (value: string) =
            let valueLabel = mkValueLabel value
            panel.Controls.Add valueLabel

        for v in values do
            makeValueLabel v

        ()

    let logFonts () =
        let iFontCollection =
            new InstalledFontCollection ()

        let fontCollection =
            iFontCollection.Families

        for c in fontCollection do
            doLog (sprintf "%s %A" c.Name (c.IsStyleAvailable (FontStyle.Italic)))
            |> ignore

        ()


    [<EntryPoint>]
    let main argv =
        try
            try
                openLog () |> ignore


                // first argument is not the name of the program !!
                // IT'S NOT UNIX!
                for a in argv do
                    doLog a |> ignore

                // logFonts ()
                let panel = form.ThePanel

                let getWinDim () =
                    (sprintf "%d x %d" (form.Width) (form.Height))

                let _, yLab =
                    showValueR "Windows dimension" (getWinDim ()) panel
                // font: Name != OriginalName sie la font Name  n'existe pas
                //       il y a peut-être d'autres cas
                showValue "Font" (sprintf "%s - %s" smallFont.Name (smallFont.Style.ToString ())) panel
                showValue "" smallFont.OriginalFontName panel

                if smallFont.IsSystemFont then
                    showValue "" smallFont.SystemFontName panel

                showValue "Font size/unit" (sprintf "%f / %s" smallFont.Size (smallFont.Unit.ToString ())) panel

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

                let getMemoryValue () =
                    (sprintf "%d Ko" (Environment.WorkingSet / 1024L))

                let _, memLab =
                    showValueR "Memory used by this process" (getMemoryValue ()) panel

                showValue
                    "64 bits process"
                    (if Environment.Is64BitProcess then
                         "yes"
                     else
                         "no")
                    panel

                showValue "Directory" Environment.CurrentDirectory panel

                showValue "System Directory" Environment.SystemDirectory panel
                showValue "System dir" (Environment.GetFolderPath(Environment.SpecialFolder.System)) panel
                showValue "LocalApplicationData" (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) panel
                showValue "ApplicationData" (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) panel
                showValue "MyPictures" (Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)) panel

                showValues "Logical drives/Mount points" (Environment.GetLogicalDrives ()) panel

                showValue "User name" Environment.UserName panel
                showValue "User domain" Environment.UserDomainName panel

                let timer = new Timer ()

                timer.Tick.Add (fun _ ->
                    timer.Stop ()
                    memLab.Text <- (getMemoryValue ())
                    timer.Enabled <- true)

                timer.Interval <- 1000
                timer.Start ()

                createMenu form

                form.Resize.Add (fun _ ->
                    memLab.Text <- (getMemoryValue ())
                    yLab.Text <- (getWinDim ()))

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
            with
            | :? System.InvalidOperationException as ex ->
                doLog (sprintf "unexpected exception %s" ex.Message) |> ignore
                (int SYSExit.Failure)
        finally
            doLog "Finally, exit!\n\n" |> ignore
            closeLog () |> ignore
