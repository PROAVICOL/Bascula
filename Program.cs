using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TestPrint
{
    static class Program
    {

        public static CT.ControlCT.Registro Bitacora = new CT.ControlCT.Registro();
        public static CT.ControlCT.Configuracion Ini = new CT.ControlCT.Configuracion();


        //public static String DBPath = Ini.Obtener("SQLServer", "DBPath", ".");
        //public static String DBPass = Ini.Obtener("SQLServer", "DBPass", "Basculas");
        public static String ComPort = Ini.Obtener("Serial", "ComPort", "Basculas");
        public static String TEMPLATE_DIRECTORY = Ini.Obtener("Template", "TEMPLATE_DIRECTORY", @"C:\Users\ServerCositec2018\Dropbox\Proyectos VS2015\Brother VCS\Brother bPAC3 SDK\Templates\");
        public static String TEMPLATE_SIMPLE = Ini.Obtener("Template", "TEMPLATE_SIMPLE", "Proavicol.LBX");


        public static String txtDataSource = Ini.Obtener("SQLServer", "txtDataSource", "");
        public static String txtInitialCatalog = Ini.Obtener("SQLServer", "txtInitialCatalog", "");
        public static String txtUserId = Ini.Obtener("SQLServer", "txtUserId", "");
        public static String txtPassword = Ini.Obtener("SQLServer", "txtPassword", "");

        public static String leerAPI = Ini.Obtener("Template2", "leerAPI", @"http://suhuevo.ddns.net:81/api/");

        public static bool SimularLectura = Ini.Obtener("Pruebas", "SimularLectura", "NO").ToUpper() == "SI" ? true : false;
        public static int timerSparaSubir = int.Parse(Ini.Obtener("Pruebas", "timerSparaSubir", "5000"));

        public static bool UsarBrother = Ini.Obtener("Impresora", "UsarBrother", "NO").ToUpper()=="SI"?true:false;
        public static bool UsarZebra = Ini.Obtener("Impresora", "UsarZebra", "NO").ToUpper() == "SI" ? true : false;
        public static String txtNombreZebra = Ini.Obtener("Impresora", "txtNombreZebra", "ZDesigner GC420d (EPL)");


        public static int PesoMinImprimir = int.Parse(Ini.Obtener("Configuracion", "PesoMinImprimir", "13"));
        public static int PesoMaxImprimir = int.Parse(Ini.Obtener("Configuracion", "PesoMaxImprimir", "53"));
        public static int NumeroBascula = int.Parse(Ini.Obtener("Configuracion", "NumeroBascula", "1"));
        public static bool UsarPassword = Ini.Obtener("Configuracion", "UsarPassword", "SI").ToUpper() == "SI" ? true : false;
        public static int CantValidacionesRequeridas = int.Parse(Ini.Obtener("Configuracion", "CantValidacionesRequeridas", "3"));
        public static String Password =Ini.Obtener("Configuracion", "Password", "produccion2019");

        public static bool BarWidthNarrowMin = Ini.Obtener("ImpresoraZebra", "BarWidthNarrowMin", "SI").ToUpper() == "SI" ? true : false;
        public static int ZebraLeft = int.Parse(Ini.Obtener("ImpresoraZebra", "Zebraleft", "70"));
        public static int ZebraTop = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraTop", "10"));
        public static int ZebraHeight = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraHeight", "100"));

        public static int ZebraLeftLabelCBarras = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraLeftLabelCBarras", "75"));
        public static int ZebraTopLabelCBarras = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraTopLabelCBarras", "120"));

        public static int ZebraLeftLabelPeso = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraLeftLabelPeso", "50"));
        public static int ZebraTopLabelPeso = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraTopLabelPeso", "140"));

        public static int ZebraLeftLabelFecha = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraLeftLabelFecha", "100"));
        public static int ZebraTopLabelFecha = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraTopLabelFecha", "165"));

        public static int ZebraLeftLabelMarca = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraLeftLabelMarca", "125"));
        public static int ZebraTopLabelMarca = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraTopLabelMarca", "190"));

        public static int ZebraLeftLabelTipo = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraLeftLabelTipo", "0"));
        public static int ZebraTopLabelTipo = int.Parse(Ini.Obtener("ImpresoraZebra", "ZebraTopLabelTipo", "0"));

        #region Licencia        
        public static String NombreProducto = "BasculaProavicol";
        public static String Licencia = Ini.Obtener("Seguridad", "Licencia", "NO");
        #endregion

        /// <summary>
		/// Main entry point of application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}