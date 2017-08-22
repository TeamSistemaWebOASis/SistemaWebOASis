using GestorErrores;
using System;
using System.Web.Helpers;

namespace SitioWebOasis.Library
{
    public class Persona
    {
        public Int32 per_id { get; set; }
        public DynamicJsonArray carnetDiscapacidad { get; set; }
        public DynamicJsonArray cuenta { get; set; }
        public DynamicJsonArray direccion { get; set; }
        public DynamicJsonArray documentoPersonal { get; set; }
        public Int32 eci_id { get; set; }
        public string estadoCivil { get; set; }
        public int etn_id { get; set; }
        public string etnia { get; set; }
        public Int32 gen_id { get; set; }
        public string genero { get; set; }
        public byte[] imagen { get; set; }
        public DynamicJsonArray instruccionFormal { get; set; }
        public Int32 lugarprocedencia_id { get; set; }
        public DynamicJsonArray nacionalidadPersona { get; set; }
        public string parroquia { get; set; }
        public string per_afiliacionIESS { get; set; }
        public Int32 per_creadoPor { get; set; }
        public string per_email { get; set; }
        public string per_emailAlternativo { get; set; }
        public DateTime per_fechaCreacion { get; set; }
        public DateTime per_fechaModificacion { get; set; }
        public DateTime per_fechaNacimiento { get; set; }
        public Int32 per_modificadoPor { get; set; }
        public string per_nombres { get; set; }
        public string per_primerApellido { get; set; }
        public string per_segundoApellido { get; set; }
        public string per_telefonoCasa { get; set; }
        public string per_telefonoCelular { get; set; }
        public string per_telefonoOficina { get; set; }
        public DynamicJsonArray personaPlurinacionalidad { get; set; }
        public string tipoSangre { get; set; }

        public string sex_id { get; set; }

        public string sexo { get; set; }

        public string defaultImage { get; set; }

        public Int32 tsa_id { get; set; }

        public Persona()
        {
            
        }

        public Persona( string dtaJsonPersona )
        {
            try
            {
                var dtaPersona = Json.Decode(dtaJsonPersona);

                if ( dtaPersona != null ) {
                    this.per_id = dtaPersona.per_id;
                    this.carnetDiscapacidad = dtaPersona.carnetDiscapacidad;
                    this.cuenta = dtaPersona.cuenta;
                    this.direccion = dtaPersona.direccion;
                    this.documentoPersonal = dtaPersona.documentoPersonal;
                    this.eci_id = dtaPersona.eci_id;
                    this.estadoCivil = dtaPersona.estadoCivil;
                    this.etn_id = dtaPersona.etn_id;
                    this.etnia = dtaPersona.etnia;
                    this.gen_id = dtaPersona.gen_id;
                    this.genero = dtaPersona.genero;
                    this.imagen = dtaPersona.imagen;
                    this.instruccionFormal = dtaPersona.instruccionFormal;
                    this.lugarprocedencia_id = dtaPersona.lugarprocedencia_id;
                    this.nacionalidadPersona = dtaPersona.nacionalidadPersona;
                    this.parroquia = dtaPersona.parroquia;
                    this.per_afiliacionIESS = dtaPersona.per_afiliacionIESS;
                    this.per_creadoPor = dtaPersona.per_creadoPor;
                    this.per_email = dtaPersona.per_email;
                    this.per_emailAlternativo = dtaPersona.per_emailAlternativo;

                    this.per_fechaCreacion = (!string.IsNullOrEmpty(Convert.ToString(dtaPersona.per_fechaCreacion)))
                                                ? Convert.ToDateTime(dtaPersona.per_fechaCreacion)
                                                : default(DateTime);

                    this.per_fechaModificacion = (!string.IsNullOrEmpty(Convert.ToString(dtaPersona.per_fechaModificacion)))
                                                    ? Convert.ToDateTime(dtaPersona.per_fechaModificacion)
                                                    : default(DateTime);

                    this.per_fechaNacimiento = (!string.IsNullOrEmpty(Convert.ToString(dtaPersona.per_fechaNacimiento)))
                                                ? Convert.ToDateTime(dtaPersona.per_fechaNacimiento)
                                                : default(DateTime);

                    this.per_modificadoPor = dtaPersona.per_modificadoPor;
                    this.per_nombres = dtaPersona.per_nombres;
                    this.per_primerApellido = dtaPersona.per_primerApellido;
                    this.per_segundoApellido = dtaPersona.per_segundoApellido;
                    this.per_telefonoCasa = dtaPersona.per_telefonoCasa;
                    this.per_telefonoCelular = dtaPersona.per_telefonoCelular;
                    this.per_telefonoOficina = dtaPersona.per_telefonoOficina;
                    this.personaPlurinacionalidad = dtaPersona.personaPlurinacionalidad;
                    this.tipoSangre = dtaPersona.tipoSangre;
                    this.tsa_id = dtaPersona.tsa_id;
                    this.sex_id = dtaPersona.sex_id;
                    this.sexo = dtaPersona.sexo;
                }
                else {
                    this.per_id = default(Int32);
                    this.carnetDiscapacidad = default(DynamicJsonArray);
                    this.cuenta = default(DynamicJsonArray);
                    this.direccion = default(DynamicJsonArray);
                    this.documentoPersonal = default(DynamicJsonArray);
                    this.eci_id = default(Int32);
                    this.estadoCivil = string.Empty;
                    this.etn_id = default(Int32);
                    this.etnia = string.Empty;
                    this.gen_id = default(Int32);
                    this.genero = string.Empty;
                    this.imagen = default(byte[]);
                    this.instruccionFormal = default(DynamicJsonArray);
                    this.lugarprocedencia_id = default(Int32);
                    this.nacionalidadPersona = default(DynamicJsonArray);
                    this.parroquia = string.Empty;
                    this.per_afiliacionIESS = string.Empty;
                    this.per_creadoPor = default(Int32);
                    this.per_email = string.Empty;
                    this.per_emailAlternativo = string.Empty;
                    this.per_fechaCreacion = default(DateTime);
                    this.per_fechaModificacion = default(DateTime);
                    this.per_fechaNacimiento = default(DateTime);
                    this.per_modificadoPor = default(Int32);
                    this.per_nombres = string.Empty;
                    this.per_primerApellido = string.Empty;
                    this.per_segundoApellido = string.Empty;
                    this.per_telefonoCasa = string.Empty;
                    this.per_telefonoCelular = string.Empty;
                    this.per_telefonoOficina = string.Empty;
                    this.personaPlurinacionalidad = default(DynamicJsonArray);
                    this.tipoSangre = string.Empty;
                    this.tsa_id = default(Int32);
                    //  this.sex_id = ;
                    //  this.sexo = ;
                }

                this.defaultImage = (this.imagen == null) 
                                        ? (this.sex_id == "M")
                                                ? "~/Content/img/EstudianteMasculinoDefault.png"
                                                : (this.sex_id == "F") ? "~/Content/img/EstudianteFemeninoDefault.png"
                                                                        : "~/Content/img/userDefault.png"
                                        : "";
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "Persona");
            }
        }

    }
}