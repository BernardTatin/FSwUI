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
open System.ComponentModel
open System.Drawing
open System.Drawing.Printing
open System.Drawing.Imaging

// [<EntryPoint>]
let main () =
 let loadimgform = new Form(Text="Printing Documents", Width=600, Height=800)
 loadimgform.BackColor<-Color.Cornsilk

 let picBox=new PictureBox(SizeMode=PictureBoxSizeMode.StretchImage,Top=40,Left=320,Height=200,Width=300,BorderStyle=BorderStyle.FixedSingle)
 let lblfilename=new Label(AutoSize=true,Top=240,Width=400,Left=320,BorderStyle=BorderStyle.FixedSingle)
 let printbtn=new Button(Text="Print",Top=270,Left=70)

 printbtn.BackColor<-Color.Ivory
 printbtn.ForeColor<-Color.Brown

 let loadbtn=new Button(Text="Load",Top=270,Left=150)
 loadbtn.BackColor<-Color.Ivory
 loadbtn.ForeColor<-Color.Brown

 let exitbtn=new Button(Text="Exit",Top=270,Left=230)
 exitbtn.BackColor<-Color.Ivory
 exitbtn.ForeColor<-Color.Red

 let opnfiledlg=new OpenFileDialog()
 let gr=loadimgform.CreateGraphics()
 let prndoc=new System.Drawing.Printing.PrintDocument()

 loadimgform .Controls.Add(picBox)
 loadimgform.Controls.Add(loadbtn)
 loadimgform.Controls.Add(lblfilename)
 loadimgform.Controls.Add(printbtn)
 loadimgform.Controls.Add(exitbtn)

 printbtn.Click.Add(fun printing->prndoc.Print())

 loadbtn.Click.Add(fun load->

 opnfiledlg.Filter <- "JPEG Images (*.jpg,*.jpeg)|*.jpg;*.jpeg|Image Files (*.gif)|*.gif"
 opnfiledlg.Title<-"Choose Image File"

 if opnfiledlg.ShowDialog()=DialogResult.OK then
  let bmp=new System.Drawing.Bitmap(opnfiledlg.FileName)
  bmp.RotateFlip(RotateFlipType.RotateNoneFlipNone)
  picBox.Image<-bmp
  lblfilename.Text<-"\t\tFilename:" + Convert.ToString(Convert.ToChar(32))+ (opnfiledlg.FileName))

 prndoc.PrintPage.Add(fun printdata -> (gr.DrawImage(picBox.Image,10,10)))

 exitbtn.Click.Add(fun quit->loadimgform.Close())

 Application.Run(loadimgform)
 // 0

[<STAThread>]
do main()
