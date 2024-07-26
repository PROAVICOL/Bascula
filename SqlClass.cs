using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace NamePlt
{

    public class SqlClass
    {
        SqlConnection con;

        public string txtDataSource { get; set; }
        public string txtInitialCatalog { get; set; }
        public string txtUserId { get; set; }
        public string txtPassword { get; set; }

        public bool conectar()
        {
            con = new SqlConnection();
            con.ConnectionString = @"Data Source=" + txtDataSource +
                    ";Initial Catalog=" + txtInitialCatalog +
                    ";User Id=" + txtUserId +
                    ";Password=" + txtPassword + "";

            try
            {
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            //return true;

        }

        // wsKioskoP.ServiceClient myKioskoP = new wsKioskoP.ServiceClient();
        SqlDataAdapter da;
        public respuestaCompuesta SqlConsulta(string consulta, ref DataTable dt)
        {
            conectar();
            respuestaCompuesta miRC = new respuestaCompuesta();
            miRC.error = false;
            miRC.query = consulta;
            dt.TableName = "temp";
            //return myKioskoP.SqlConsultaDT(consulta,ref dt);
            try
            {
                da = new SqlDataAdapter(consulta, con);
                miRC.RegistrosTabla = da.Fill(dt);


            }
            catch (Exception ex)
            {
                miRC.error = true;
                miRC.strError = ex.Message;
                //throw;
            }
            finally
            {
                con.Close();
            }
            return miRC;
        }

        public respuestaCompuesta SqlConsulta(string consulta)
        {
            //return myKioskoP.SqlConsultaSRT(consulta);
            respuestaCompuesta miRC = new respuestaCompuesta();
            miRC.query = consulta;
            miRC.error = false;

            int rowAffected = 0;
            conectar();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                rowAffected = cmd.ExecuteNonQuery();
                miRC.columnasAfectadas = rowAffected;
                //return rowAffected;

            }
            catch (Exception ex)
            {
                miRC.error = true;
                miRC.strError = ex.Message;
                // return rowAffected;
                //throw;
            }
            finally
            {
                con.Close();
            }


            return miRC;

        }

        public int SqlConsulta(string[] consulta)
        {
            //return myKioskoP.SqlConsultaARR(consulta);
            int rowAffected = 0;

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }


            for (int i = 0; i < consulta.Length; i++)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(consulta[i], con);
                    cmd.CommandType = CommandType.Text;
                    rowAffected += cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    //hrow;
                }

            }


            // con.Close();
            //return rowAffected;


            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            return rowAffected;

        }

        public int storeProc(String StoreProcedureName, List<parametro> parametros)
        {
            //return myKioskoP.storeProc(StoreProcedureName, parametros.ToArray());
            int rowAffected = 0;
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(StoreProcedureName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < parametros.Count; i++)
                {
                    cmd.Parameters.AddWithValue(parametros[i].nombre, parametros[i].valor);
                }
                rowAffected = cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {

                //throw;
            }
            finally
            {
                con.Close();
            }
            return rowAffected;
        }

    }
    public class parametro
    {
        List<parametro> parametros = new List<parametro>();
        private String nombre_;
        public String nombre
        {
            get { return nombre_; }
            set { nombre_ = "@" + value; }
        }
        public String valor;

    }

    public class respuestaCompuesta
    {

        public int RegistrosTabla { get; set; }
        public int columnasAfectadas { get; set; }
        public bool error { get; set; }
        public string strError { get; set; }

        public string query { get; set; }
    }

  
}
