
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Microsoft.VisualBasic;
using NamePlt;
using System.Threading;
using static TestPrint.localDB;
using Com.SharpZebra;
using Com.SharpZebra.Printing;
using Com.SharpZebra.Commands;
using Com.SharpZebra.Commands.Codes;


using CT.ControlCT;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;

namespace TestPrint
{
    public partial class Form1 : Form
    {
        private string TEMPLATE_DIRECTORY = Program.TEMPLATE_DIRECTORY; // Template file path
        private string TEMPLATE_SIMPLE = Program.TEMPLATE_SIMPLE;// "Proavicol.LBX";	// Template file name
		private const string TEMPLATE_FRAME = "NamePlate2.LBX";		// Template file name

        public Form1()
        {
            InitializeComponent();
		}

        SqlClass mySqlClass = new SqlClass()
        {
            txtDataSource = Program.txtDataSource,
            txtInitialCatalog = Program.txtInitialCatalog,
            txtUserId = Program.txtUserId,
            txtPassword = Program.txtPassword
        };
        bool enProcesoActivacion;
        PC_INFO pc = new PC_INFO();

        int intentosActivacion = 0;

      

        private void Form1_Load(object sender, EventArgs e)
		{
           // validacionOperacion();


            timer1.Interval = Program.timerSparaSubir;

            DataTable dtTHUEVO = new DataTable();
            mySqlClass.SqlConsulta("select * from VARValor where VAVClave='THUEVO'",ref dtTHUEVO);

            if (dtTHUEVO.Rows.Count>0)
            {
                miLocalDB.eliminarTHuevo();
                for (int i = 0; i < dtTHUEVO.Rows.Count; i++)
                {
                    // cmbdtTHUEVO.Items.Add(dtTHUEVO.Rows[i]["Descripcion"].ToString());
                    miLocalDB.nuevaTHuevo(dtTHUEVO.Rows[i]["VARClave"].ToString(), dtTHUEVO.Rows[i]["Descripcion"].ToString());

                }
            }

            List<thuevo> lstTHuevos = miLocalDB.consultaTHuevo();
            for (int i = 0; i < lstTHuevos.Count; i++)
            {
                Button miBoton = new Button()
                {
                    Text = lstTHuevos[i].Descripcion,
                    AccessibleName = lstTHuevos[i].VARClave.ToString(),
                    BackColor = System.Drawing.Color.Gainsboro,
                    ForeColor = System.Drawing.Color.Black,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Size = new System.Drawing.Size(220, 40),
                    UseVisualStyleBackColor = false
                };
                miBoton.Click += MiBoton_Click1;
                contenedorTipos.Controls.Add(miBoton);
            }


            DataTable dtCacetas = new DataTable();

            if (dtTHUEVO.Rows.Count > 0)//si no hay conexion auqi ya se sabe
            {
               
                mySqlClass.SqlConsulta("select * from cacetas", ref dtCacetas);
            }
         

            if (dtCacetas.Rows.Count > 0)
            {
                miLocalDB.eliminarCasetas();
                for (int i = 0; i < dtCacetas.Rows.Count; i++)
                {
                    //cmbCaceta.Items.Add(dtCacetas.Rows[i]["nombre"].ToString());
                    miLocalDB.nuevaCaseta(dtCacetas.Rows[i]["id"].ToString(), dtCacetas.Rows[i]["nombre"].ToString());
                }
            }

            List<tCaseta> lstCasestas = miLocalDB.consultaCaseta();
            for (int i = 0; i < lstCasestas.Count; i++)
            {
                Button miBoton = new Button()
                {
                    Text =lstCasestas[i].nombre,
                    AccessibleName = lstCasestas[i].id_granja.ToString(),
                    BackColor = System.Drawing.Color.Gainsboro,
                    ForeColor = System.Drawing.Color.Black,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Size = new System.Drawing.Size(200, 40),
                    UseVisualStyleBackColor = false
                };
                miBoton.Click += MiBoton_Click;
                contenedorCasetas.Controls.Add(miBoton);
            }

           

            cargarResultados();           

            this.cmbTemplate.SelectedIndex = 0;
		}

        public static DialogResult InputBox(string title, string promptText, ref string value, bool espassword)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            if (espassword)
            {
                textBox.PasswordChar = '*';
            }

            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void MiBoton_Click1(object sender, EventArgs e)
        {
            string value = "";
            if (Program.UsarPassword)
            {

                if (InputBox("Desea cambiar la configuraciom, tenga cuidado!!", "Introduzca el password:", ref value, true) == DialogResult.OK)
                {

                }
                else {
                    value = "";
                }
            }
           

            if (value == Program.Password||!Program.UsarPassword)
            {
                for (int i = 0; i < contenedorTipos.Controls.Count; i++)
                {
                    ((Button)contenedorTipos.Controls[i]).BackColor = System.Drawing.Color.Gainsboro;
                    ((Button)contenedorTipos.Controls[i]).ForeColor = System.Drawing.Color.Black;
                }

           ((Button)sender).BackColor = System.Drawing.Color.SteelBlue;
                ((Button)sender).ForeColor = System.Drawing.Color.White;
                txtTipoId.Text = ((Button)sender).AccessibleName;
                txtTipo.Text = ((Button)sender).Text;
                //throw new NotImplementedException();
                // throw new NotImplementedException();
            }
            else
            {
                MessageBox.Show("Contraseña Incorrecta");
            }


        }

        private void MiBoton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < contenedorCasetas.Controls.Count; i++)
            {
                ((Button)contenedorCasetas.Controls[i]).BackColor = System.Drawing.Color.Gainsboro;
                ((Button)contenedorCasetas.Controls[i]).ForeColor = System.Drawing.Color.Black;
            }

            ((Button)sender).BackColor = System.Drawing.Color.SteelBlue;
            ((Button)sender).ForeColor = System.Drawing.Color.White;
            txtCasetaID.Text = ((Button)sender).AccessibleName;
            txtCaseta.Text = ((Button)sender).Text;
            //throw new NotImplementedException();
        }

        localDB miLocalDB = new localDB();
        public void cargarResultados() {
                        
            var source = new BindingSource();            
            source.DataSource = miLocalDB.consultarNombres();
            this.Invoke(new Funcion(delegate ()
            {
                dataGridView1.DataSource = source;
            }));
          

        }

        public void Imprmir() {
            //string templatePath = TEMPLATE_DIRECTORY;
            //// None decoration frame
            //if (cmbTemplate.SelectedIndex == 0)
            //{
            //    templatePath += TEMPLATE_SIMPLE;
            //}
            //// Decoration frame
            //else
            //{
            //    templatePath += TEMPLATE_FRAME;
            //}

            //bpac.DocumentClass doc = new DocumentClass();
            //if (doc.Open(templatePath) != false)
            //{
            //    doc.GetObject("TextoSuperior").Text = txtCompany.Text;
            //    doc.GetObject("fecha").Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //    doc.GetObject("cbarras").Text = DateTime.Now.ToString("yyMMddHHmmss");

            //    //doc.SetMediaById(doc.Printer.GetMediaId(), true);
            //    doc.StartPrint("", PrintOptionConstants.bpoDefault);
            //    doc.PrintOut(1, PrintOptionConstants.bpoDefault);
            //    doc.EndPrint();
            //    doc.Close();
            //}
            //else
            //{
            //    MessageBox.Show("Open() Error: " + doc.ErrorCode);
            //}
        }
        
        

		private void btnPrint_Click(object sender, EventArgs e)	{
            Imprmir();			
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}


        delegate void Funcion();
        string pesoStr = "";

        private void sp1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string x = sp1.ReadExisting().Trim().Replace("\r", "").Replace("\0", "").Replace("\n", "");

                pesoStr = pesoStr + x;
                if (pesoStr.Contains("."))
                {
                    int indexPunto = pesoStr.IndexOf(".");
                    if (pesoStr.Substring(indexPunto, pesoStr.Length - indexPunto).Length > 2)
                    {
                        try
                        {
                            if (decimal.Parse(pesoStr) > Program.PesoMinImprimir && decimal.Parse(pesoStr) < Program.PesoMaxImprimir)
                            {
                                pesoStr = decimal.Parse(pesoStr).ToString("0#.#0").Replace(".", "");
                                this.Invoke(new Funcion(delegate ()
                                {
                                    AgregarRegistro(pesoStr);
                                }));

                            }
                        }
                        catch (Exception)
                        {

                            MessageBox.Show("No cumple requisito." + pesoStr);
                        }
                        finally {
                            pesoStr = "";
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }


        string ultimoPesoRegistrado = "";
        int cantRegsitros = 0;


        private void AgregarRegistro(String pesoARegistrar) {
            string cbarras = DateTime.Now.ToString("yyMMddHHmmss" +
                                                             pesoARegistrar +
                                                             int.Parse(txtCasetaID.Text).ToString("00") +
                                                             int.Parse(txtTipoId.Text).ToString("00")+Program.NumeroBascula);

            if (ultimoPesoRegistrado == pesoARegistrar)
            {
                cantRegsitros++;                
            }
            else {                
                cantRegsitros = 0;
            }
            ultimoPesoRegistrado = pesoARegistrar;

            if (cantRegsitros<Program.CantValidacionesRequeridas)
            {                
                new Thread(() => generar(1)).Start();
                return;
            }

           

            cbarras = cbarras.Replace("-", "");
            this.Invoke(new Funcion(delegate ()
            {
                txtPeso.Text = pesoARegistrar.Substring(0, 2) + "." + pesoARegistrar.Substring(2, 2) + " KG.";
                new Thread(habilitar_boton).Start();
                ultimoPesoRegistrado = "";
            }));

            

            try
            {
                if (Program.UsarBrother)
                {
                    ImprmirBrother(pesoARegistrar, DateTime.Now, cbarras);
                }
                if (Program.UsarZebra)
                {
                    ImprimirZebra(pesoARegistrar, DateTime.Now, cbarras,txtTipo.Text);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
           
            
            miLocalDB.nuevoProduccion(cbarras, "1", txtCaseta.Text, txtTipo.Text, pesoARegistrar);
            cargarResultados();
        }

        private void ImprimirZebra(String peso, DateTime fecha, String cbarras,String tipo) {


            Print(new BorderedLabel("Peso:"+peso.Substring(0,2)+"."+peso.Substring(2,2)+" KG.","Fecha:"+fecha.ToString("dd/MM/yyyy HH:mm:ss"),"PROAVICOL", cbarras,"Tipo:"+tipo));


            //MessageBox.Show(Program.ZebraLeft + " " + Program.ZebraTop + " " + Program.ZebraHeight + " " + Program.ZebraLeftLabelCBarras + " " +
            //          Program.ZebraTopLabelCBarras + " " + Program.ZebraLeftLabelPeso + " " + Program.ZebraTopLabelPeso + " " +
            //          Program.ZebraLeftLabelFecha + " " + Program.ZebraTopLabelFecha + " " + Program.ZebraLeftLabelMarca + " " + Program.ZebraTopLabelMarca);

            //PrinterSettings ps = new PrinterSettings();
            //ps.PrinterName = "ZDesigner GC420d (EPL)";
            //ps.Width = 203 * 2;
            //ps.Length = 203 * 1;
            //ps.Darkness = 30;

            //List<byte> page = new List<byte>();

            //Barcode b = new Barcode();
            //b.Type = BarcodeType.CODE128_AUTO;

            //page.AddRange(ZPLCommands.ClearPrinter(ps));
            //page.AddRange(ZPLCommands.BarCodeWrite(Program.ZebraLeft, Program.ZebraTop, Program.ZebraHeight, ElementDrawRotation.NO_ROTATION, b, false, cbarras,Program.BarWidthNarrowMin));
            //page.AddRange(ZPLCommands.TextWrite(Program.ZebraLeftLabelCBarras, Program.ZebraTopLabelCBarras, ElementDrawRotation.NO_ROTATION, ZebraFont.STANDARD_NORMAL, 15, 10, cbarras));
            //page.AddRange(ZPLCommands.TextWrite(Program.ZebraLeftLabelPeso, Program.ZebraTopLabelPeso, ElementDrawRotation.NO_ROTATION, ZebraFont.STANDARD_NORMAL, 15, 10, "Peso:" + peso.Substring(0, 2) + "." + peso.Substring(2, 2) + " KG. Tipo:" + txtTipo.Text));
            //page.AddRange(ZPLCommands.TextWrite(Program.ZebraLeftLabelFecha, Program.ZebraTopLabelFecha, ElementDrawRotation.NO_ROTATION, ZebraFont.STANDARD_NORMAL, 15, 10, "Fecha: " + fecha.ToString("dd/MM/yy")));
            //page.AddRange(ZPLCommands.TextWrite(Program.ZebraLeftLabelMarca, Program.ZebraTopLabelMarca, ElementDrawRotation.NO_ROTATION, ZebraFont.STANDARD_NORMAL, 15, 10, "PROAVICOL"));
            //page.AddRange(ZPLCommands.PrintBuffer(1));
            //new SpoolPrinter(ps).Print(page.ToArray());
        }

        private void Print(IZebraCommand command)
        {
            string zebraInstructions = command.ToZebraInstruction();
            string selectedPrinterName = Program.txtNombreZebra;// GetSelectedPrinterName();
            new ZebraPrinter(selectedPrinterName).Print(zebraInstructions);
        }

        private void ImprmirBrother(String peso, DateTime fecha, String cbarras) {
            

            //string templatePath = TEMPLATE_DIRECTORY;
            //// None decoration frame
            //if (cmbTemplate.SelectedIndex == 0)
            //{
            //    templatePath += TEMPLATE_SIMPLE;
            //}
            //// Decoration frame
            //else
            //{
            //    templatePath += TEMPLATE_FRAME;
            //}

            //bpac.DocumentClass doc = new DocumentClass();
            //if (doc.Open(templatePath) != false)
            //{

                  

            //    doc.GetObject("TextoSuperior").Text = "Peso:"+ peso.Substring(0, 2) + "." + peso.Substring(2, 2) + " KG. Tipo: " + txtTipo.Text;
            //    doc.GetObject("fecha").Text = "Fecha: "+  fecha.ToString("dd/MM/yyyy") + " Hora: "+ fecha.ToString("HH:mm:ss");
            //    doc.GetObject("cbarras").Text = cbarras;

            //    //doc.SetMediaById(doc.Printer.GetMediaId(), true);
            //    doc.StartPrint("", PrintOptionConstants.bpoDefault);
            //    doc.PrintOut(1, PrintOptionConstants.bpoDefault);
            //    doc.EndPrint();
            //    doc.Close();
            //}
            //else
            //{
            //    MessageBox.Show("Open() Error: " + doc.ErrorCode);
            //}
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Leer();           
        }

        public void Leer() {
            if (!sp1.IsOpen)
            {
                sp1.PortName = Program.ComPort; //"COM3";
                sp1.Open();
            }
            sp1.Write("P");
        }

        private void Button2_Click(object sender, EventArgs e)
        {

          //  miProduccion.nuevo(DateTime.Now.ToString("yyMMddHHmmss"));
            cargarResultados();

        }

        public void habilitar_boton() {
            Thread.Sleep(3000);
            this.Invoke(new Funcion(delegate ()
            {
                btnGenerar.Enabled = true;
            }));
        }

        


        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            btnGenerar.Enabled = false;
            generar();            
        }


        private void generar(int i=0) {

            Thread.Sleep(i * 3000);

          
            //new Thread(habilitar_boton).Start();

            if (txtTipo.Text != "" && txtCaseta.Text != "")
            {
                if (!Program.SimularLectura)
                {
                    Leer();
                }
                else
                {
                    Random r = new Random();
                    int aleatorio = 1234;// r.Next(1000, 9999);
                    AgregarRegistro(aleatorio.ToString());
                }

            }
            else
            {
                MessageBox.Show("Debe seleccionar correctamente las opciones");
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (contadorGral>0)
            {
                contadorGral--;
            }

            var t= miLocalDB.consultarNombres();

            if (t.Count > 0)
            {
                /*
              var x=  mySqlClass.SqlConsulta("INSERT INTO [dbo].[produccion_cajas]\r\n" +
                         "           ([id_granja]\r\n" +
                         "           ,[id_caceta]\r\n" +
                         "           ,[codigo]\r\n" +
                         "           ,[peso]\r\n" +
                         "           ,[tipo]\r\n" +
                         "           ,[status]\r\n" +
                         "           ,[creacion]\r\n" +
                         "           ,[actualizacion])\r\n" +
                         "     VALUES\r\n" +
                         "           ('1'," +
                         "           '" + miLocalDB.consultaCaseta(t[0].caseta) + "'," +
                         "           '" + t[0].codigo + "'," +
                         "           '" + t[0].peso + "'," +
                         "           '" + miLocalDB.consultaTHuevo(t[0].tipo) + "'," +
                         "           '1'," +
                         "            GETDATE()," +
                         "            GETDATE()" +
                         ")");
                string texto = "Contador: "+t.Count+ " Codigo: "+ t[0].codigo+ " --- ";
                // Serializing 'x' to JSON
                string jsonResult = JsonConvert.SerializeObject(x, Formatting.Indented);

                // Appending JSON to an existing text file
                string filePath = "result.txt";
                File.AppendAllText(filePath, texto + jsonResult + Environment.NewLine);


                if (x.columnasAfectadas>0 )
                {
                    miLocalDB.actualizar(t[0].codigo);
                }
                else if ( x.strError.Contains("UNIQUE KEY"))
                {
                    miLocalDB.actualizar2(t[0].codigo);
                }
                
                 cargarResultados();
                */
                /**/
                // Llamada a la api
                string Datos = "1|" + miLocalDB.consultaCaseta(t[0].caseta) + "|" + t[0].codigo + "|" + t[0].peso + "|" + miLocalDB.consultaTHuevo(t[0].tipo);

                await llamarApi(Datos, t[0].codigo); // Wait for the API call to complete

                cargarResultados();
            }
        }

        private async Task llamarApi(string Datos, string Codigo)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var data = new datos 
                    {
                        Id = Program.NumeroBascula,
                        TokenUsr = "",
                        IDApp = "1.0.0",
                        DatEquipo = "Bascula: " + Program.NumeroBascula + "|",
                        IdPantalla = 4,
                        IdAccion = 1,
                        Op = 1,
                        IdJson = 1,
                        DatJson = Datos,
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(data));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    httpClient.DefaultRequestHeaders.Add("ApiKey", "d2fc4d32-74f7-4199-ab99-7bf87268a9e7");

                    string path_server = Program.leerAPI;

                    //string path_server = "http://suhuevo.ddns.net:81/api/";

                    var response = await httpClient.PostAsync(path_server + "principal/Funcion", content);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var aux = JsonConvert.DeserializeObject<respuestaSpRequest>(responseContent);

                    if (aux.Err == "1") // respuesta exitosa
                    {
                        miLocalDB.actualizar(Codigo);
                    }
                    else if (aux.Err == "2") // codigo ya existe
                    {
                        miLocalDB.actualizar(Codigo);
                    }
                    else
                    {

                    }
                }

            }
            catch (Exception err)
            {
                //return Json(new { Respuesta = false, Mensaje = err.ToString() });
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            contadorGral++;
            if (contadorGral>3)
            {
                string value = "";
                if (InputBox("¿Realmente quieres eleminar la produccion?, este procedimiento no es reversible", "Introduzca el password:", ref value, true) == DialogResult.OK)
                {

                    if (value == Program.Password)
                    {
                        miLocalDB.eliminarProduccion("");
                        cargarResultados();
                        
                    }
                   
                }
             
            }
        }

        int contadorGral = 0;

        private void txtPeso_Click(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows[0].Cells[1].Value.ToString().Contains("-"))
            {
                miLocalDB.actualizar(dataGridView1.SelectedRows[0].Cells[0].Value.ToString(),
                    dataGridView1.SelectedRows[0].Cells[1].Value.ToString().Replace("-", ""));
                cargarResultados();
            }
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {

        }
    }

    public class BorderedLabel : IZebraCommand
    {
        private readonly string commandString;

        public BorderedLabel(string peso, string fecha, string marca, string barcode,string tipo)
        {
            commandString = new LabelBuilder()
                // .Border(5, 5, 400, 100)
                .Barcode(barcode).At(Program.ZebraLeft, Program.ZebraTop)
                .Text(barcode).At(Program.ZebraLeftLabelCBarras, Program.ZebraTopLabelCBarras)
                .Text(peso).At(Program.ZebraLeftLabelPeso, Program.ZebraTopLabelPeso)
                .Text(fecha).At(Program.ZebraLeftLabelFecha, Program.ZebraTopLabelFecha)
                .Text(tipo).At(Program.ZebraLeftLabelTipo, Program.ZebraTopLabelTipo)
                //.Image("descarga.pcx").At(450, 0)
                .ToZebraInstruction();
        }

        public string ToZebraInstruction()
        {
            return commandString;
        }
    }


    class LabelBuilder
    {
        private delegate IZebraCommand Command(string value, int x, int y);
        private string value;
        private Command commandToExecute;

        private ZebraCommands commands = new ZebraCommands();

        public LabelBuilder Barcode(string value)
        {
            return StoreValues(BarcodeCommand, value);
        }

        private LabelBuilder StoreValues(Command delegateCommand, string value)
        {
            this.value = value;
            commandToExecute = delegateCommand;
            return this;
        }

        public LabelBuilder At(int x, int y)
        {
            commands.Add(commandToExecute(value, x, y));
            commandToExecute = null;
            value = null;
            return this;
        }

        public LabelBuilder Text(string value)
        {
            return StoreValues(StandardText, value);
        }

        public LabelBuilder Image(string value)
        {
            return StoreValues(ImageCommand, value);
        }

        private IZebraCommand ImageCommand(string imageName, int x, int y)
        {
            return new GraphicZebraCommand(imageName, x, y);
        }

        private IZebraCommand BarcodeCommand(string barcode, int x, int y)
        {
            return ZebraCommands.BarCodeCommand(x, y, ElementRotation.NO_ROTATION, 1, 3, 0, Program.ZebraHeight, false, barcode);
        }

        private IZebraCommand StandardText(string text, int x, int y)
        {
            return ZebraCommands.TextCommand(x, y, ElementRotation.NO_ROTATION, StandardZebraFont.NORMAL, 1, 1, false, text);
        }


        public string ToZebraInstruction()
        {
            return commands.ToZebraInstruction();
        }

        public LabelBuilder Border(int startX, int startY, int endX, int endY)
        {
            commands.Add(ZebraCommands.DrawBox(startX, startY, 2, endX, endY));
            return this;
        }
    }



    public class data
    {
        public string connString()
        {
            string c = @"W:\Proyectos VS2015\Brother VCS\NamePlt\bin\x64\Debug\produccion.sdf";
            c = "Data Source=" + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\produccion.sdf;Persist Security Info = False; ";
            return c.Replace("file:\\","");
        }

        string path_ = "";
        string pass_ = "";


        public void upgrade()
        {
            //Directory.GetCurrentDirectory()
            SqlCeEngine engine = new SqlCeEngine(connString());
            engine.Upgrade();
        }




    }

    public class localDB
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public DateTime fecha { get; set; }
        public int sync { get; set; }
        public string granaja { get; set; }
        public string caseta { get; set; }
        public string tipo { get; set; }
        public string peso { get; set; }

        List<componentes> list;

        SqlCeConnection conn;
        public localDB()
        {
            data d = new data();

            try
            {
                conn = new SqlCeConnection(d.connString());
            }
            catch (Exception ex)
            {

               // throw;
            }

           
            id = 0;
            codigo = "";

        }

        #region Casetas
        public void eliminarCasetas()
        {
            try
            {
                string query = "DROP TABLE [t_caseta];";
                conn.Open();
               
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();

                query = "CREATE TABLE [t_caseta] (" +
                "  [Id] int IDENTITY (1,1) NOT NULL" +
                ", [id_granja] int NOT NULL\r\n" +
                ", [nombre] nvarchar(100) NOT NULL" +
                ");";

                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();

                query = "ALTER TABLE [t_caseta] ADD CONSTRAINT [PK_t_caseta] PRIMARY KEY ([Id]);\r\n";
                  
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                //throw;
            }
            finally
            {
                conn.Close();
            }

        }

        public void nuevaCaseta(String id, String Nombre)
        {
            try
            {
                string query = "INSERT INTO [t_caseta] ([id_granja],[nombre]) VALUES (" +
                    "'" + id + "'," +
                    "'" + Nombre + "'" +
                    "); ";
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                conn.Open();
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                //throw;
            }
            finally
            {
                conn.Close();
            }


        }

        public List<tCaseta> consultaCaseta()
        {
            string query = "SELECT * FROM [t_caseta] ;";
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            SqlCeDataReader rdr = cmd.ExecuteReader();
          
            List<tCaseta> lstCasetas = new List<tCaseta>();
            try
            {
                for (int i = 0; rdr.Read(); i++)
                {
                    try
                    {
                        lstCasetas.Add(new tCaseta() {id_granja  = rdr.GetInt32(1), nombre = rdr.GetString(2) });
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            finally
            {
                rdr.Close();
                conn.Close();
            }
            return lstCasetas;
        }


        public string consultaCaseta(string nombre)
        {
            string query = "SELECT * FROM [t_caseta] where nombre='"+nombre+"';";
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            SqlCeDataReader rdr = cmd.ExecuteReader();
            string respuesta = "";
            try
            {
                rdr.Read();
                respuesta = rdr.GetInt32(1).ToString();
            }
            catch (Exception ex)
            {

               
            }
            finally
            {
                rdr.Close();
                conn.Close();
            }
            return int.Parse(respuesta).ToString("00");
        }

        #endregion


        #region TiposHuevo
        public void eliminarTHuevo()
        {
            try
            {
                string query = "DROP TABLE [t_tipo];";
                conn.Open();

                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();

                query = "CREATE TABLE [t_tipo] " +
                    "([Id] int IDENTITY(1,1) NOT NULL, [VARClave] int NOT NULL, [Descripcion] nvarchar(100) NOT NULL);";

                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();

                query = "ALTER TABLE [t_tipo] ADD CONSTRAINT [PK_t_tipo] PRIMARY KEY ([Id]);";

                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();  

             
            }
            catch (Exception ex)
            {

                //throw;
            }
            finally
            {
                conn.Close();
            }

        }

        public void nuevaTHuevo(String id, String Nombre)
        {
            try
            {
                string query = "INSERT INTO [t_tipo] ([VARClave],[Descripcion]) VALUES (" +
                    "'" + id + "'," +
                    "'" + Nombre + "'" +
                    "); ";
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                conn.Open();
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                //throw;
            }
            finally
            {
                conn.Close();
            }


        }

        public class tCaseta
        {
            public int id_granja { get; set; }
            public string nombre { get; set; }

        }


        public class thuevo
        {
            public int VARClave { get; set; }
            public string Descripcion { get; set; }
   
        }

        public List<thuevo> consultaTHuevo()
        {
            string query = "SELECT * FROM [t_tipo] ORDER BY convert(int, VARClave) ";
            SqlCeCommand cmd = new SqlCeCommand(query, conn);
            List<thuevo> lstTHuevos = new List<thuevo>();
            conn.Open();
            SqlCeDataReader rdr = cmd.ExecuteReader();


            try
            {
                for (int i = 0; rdr.Read(); i++)
                {
                    try
                    {
                        lstTHuevos.Add(new thuevo() { VARClave = rdr.GetInt32(1), Descripcion = rdr.GetString(2) });
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            finally
            {
                rdr.Close();
                conn.Close();
            }

            return lstTHuevos;
        }



        public string consultaTHuevo(string nombre)
        {
            string query = "SELECT * FROM [t_tipo] where Descripcion='" + nombre + "';";
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            SqlCeDataReader rdr = cmd.ExecuteReader();
            string respuesta = "";
            try
            {
                rdr.Read();
                respuesta = rdr.GetInt32(1).ToString();
            }
            catch (Exception ex)
            {


            }
            finally
            {
                rdr.Close();
                conn.Close();
            }
            return int.Parse(respuesta).ToString("00");
        }

        #endregion



        public void nuevoProduccion(String codigo, String granja, String caseta, String tipo, String peso_)
        {

            try
            {
                string query = "Insert into t_produccion (codigo,fecha,sync,[granaja],[caceta],[tipo],[peso])" +
                    " values ('" + codigo + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss ") + "',0" +
                    ",'" + granja + "','" + caseta + "','" + tipo + "','" + peso_ + "')";
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                conn.Open();
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                //throw;
            }
            finally
            {
                conn.Close();
            }


        }

        public void actualizar(String id,string codigox)
        {
            try
            {
                string query = "UPDATE t_produccion set codigo = '"+codigox+"' where Id='" + id + "'";
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                conn.Open();
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                //throw;
            }
            finally
            {
                conn.Close();
            }
        }


        public void actualizar(String codigo) {
            try
            {
                string query = "UPDATE t_produccion set sync = 1 where codigo='" + codigo + "'";
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                conn.Open();
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                //throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public void eliminarProduccion(String codigo)
        {
            try
            {
                string query = "DELETE FROM [t_produccion]";
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                conn.Open();
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                //throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public void actualizar(String Nombre, List<componentes> misComponentes)
        {
            try
            {
                string query = "Delete from subproceso where nombre='" + Nombre + "'";
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();

                query = "Delete from componentes where nombre='" + Nombre + "'";
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();

                query = "Insert into subproceso (Nombre) values ('" + Nombre + "')";
                cmd = new SqlCeCommand(query, conn);
                cmd.ExecuteNonQuery();

                for (int i = 0; i < misComponentes.Count; i++)
                {
                    query = "Insert into componentes (setpoint, yaggeo, nombre, nodo) values ('" +
                        misComponentes[i].setpoint.ToString() + "','" +
                        misComponentes[i].yaggeo.ToString() + "','" +
                        misComponentes[i].nombre + "','" +
                        misComponentes[i].nodo.ToString() + "') ";

                    cmd = new SqlCeCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception)
            {

                //                throw;
            }
            finally
            {
                conn.Close();
            }



        }

        public List<localDB> consultarNombres()
        {
            string query = "SELECT [Id] ,[codigo],[fecha],[sync],[granaja],[caceta],[tipo],[peso] FROM [t_produccion] where sync in (0);";
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            SqlCeDataReader rdr = cmd.ExecuteReader();
            List<localDB> lstProduccion = new List<localDB>();
            try
            {
                for (int i = 0; rdr.Read(); i++)
                {
                    try
                    {
                       

                        lstProduccion.Add(new localDB()
                        {
                            id = rdr.GetInt32(0),
                            codigo = rdr.GetString(1),
                            fecha = rdr.GetDateTime(2),
                            sync = rdr.GetInt32(3),
                            granaja= rdr.GetString(4),
                            caseta= rdr.GetString(5),
                            tipo= rdr.GetString(6),
                            peso = rdr.GetString(7)

                        });
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            finally
            {
                rdr.Close();
                conn.Close();
            }
            return lstProduccion;
        }

        public List<componentes> conultarComponente(String Nombre)
        {
            return new componentes().consulta(Nombre);
        }


    }


    public class componentes
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int setpoint { get; set; }
        public int yaggeo { get; set; }
        public int nodo { get; set; }
        SqlCeConnection conn;
        public componentes()
        {
            data d = new data();
            conn = new SqlCeConnection(d.connString());
            id = 0;
            setpoint = 0;
            yaggeo = 0;
            nombre = "";

        }

        public void alta()
        {
        }

        public void baja()
        {
        }

        public List<componentes> consulta(string subrprocesoNombre)
        {

            string query = "SELECT id, setpoint, yaggeo, nombre, nodo FROM componentes where nombre='" + subrprocesoNombre + "'";
            SqlCeCommand cmd = new SqlCeCommand(query, conn);
            conn.Open();
            SqlCeDataReader rdr = cmd.ExecuteReader();
            List<componentes> lComponentes = new List<componentes>();
            try
            {
                for (int i = 0; rdr.Read(); i++)
                {
                    try
                    {
                        lComponentes.Add(new componentes()
                        {
                            id = rdr.GetInt32(0),
                            setpoint = rdr.GetInt32(1),
                            yaggeo = rdr.GetInt32(2),
                            nombre = rdr.GetString(3),
                            nodo = rdr.GetInt32(4)
                        });
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            finally
            {
                rdr.Close();
                conn.Close();
            }
            return lComponentes;


        }





    }


    public class HexConverter
    {
        private static Int32 value_;
        public string Data_Hex_Asc(ref string Data)
        {
            string Data1 = "";
            string sData = "";

            while (Data.Length > 0)

            //first take two hex value using substring.
            //then  convert Hex value into ascii.
            //then convert ascii value into character.
            {
                Data1 = System.Convert.ToChar(System.Convert.ToUInt32(Data.Substring(0, 2), 16)).ToString();
                sData = sData + Data1;
                Data = Data.Substring(2, Data.Length - 2);
            }
            return sData;
        }
        public string Data_Asc_Hex(char Datax)
        {
            //first take each charcter using substring.
            //then  convert character into ascii.
            //then convert ascii value into Hex Format
            string Data = Datax.ToString();
            string sValue;
            string sHex = "";
            while (Data.Length > 0)
            {
                sValue = Conversion.Hex(Strings.Asc(Data.Substring(0, 1).ToString()));
                Data = Data.Substring(1, Data.Length - 1);
                sHex = sHex + sValue;
            }
            return sHex;
        }

        public static string ToHexString(byte[] valor)
        {
            string tem = null;
            for (int i = 0; i < valor.Length; i++)
            {
                value_ = Convert.ToInt32(valor[i]);
                //var s = Convert.ToString(value_, 16);
                var x = String.Format("{0:X}", value_);
                if (x.Length == 1)
                    x = "0" + x;
                tem += x;

            }
            return tem;
        }
    }

}