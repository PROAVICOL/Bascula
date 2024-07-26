using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamePlt
{
    public class datos
    {
        public int Id { get; set; }
        public string TokenUsr { get; set; }
        public string IDApp { get; set; }
        public string DatEquipo { get; set; }
        public int IdPantalla { get; set; }
        public int IdAccion { get; set; }
        public int Op { get; set; }
        public int IdJson { get; set; }
        public string DatJson { get; set; }
    }

    public class respuestaSpRequest
    {
        public string Err { get; set; }
        public string ErrDescripcion { get; set; }
        public string IdResp { get; set; }
        public string DatJsonResp { get; set; }
        public string json { get; set; }
    }
}
