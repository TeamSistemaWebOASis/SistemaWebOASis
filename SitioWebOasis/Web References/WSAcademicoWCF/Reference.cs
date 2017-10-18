﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Microsoft.VSDesigner generó automáticamente este código fuente, versión=4.0.30319.42000.
// 
#pragma warning disable 1591

namespace SitioWebOasis.WSAcademicoWCF {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2046.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="AcademicoSoap", Namespace="http://tempuri.org/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Rol))]
    public partial class Academico : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback BuscarApellidoOperationCompleted;
        
        private System.Threading.SendOrPostCallback EnviarEmailOperationCompleted;
        
        private System.Threading.SendOrPostCallback EnviarEmailHtmlOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Academico() {
            this.Url = global::SitioWebOasis.Properties.Settings.Default.SitioWebOasis_WSAcademicoWCF_Academico;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event BuscarApellidoCompletedEventHandler BuscarApellidoCompleted;
        
        /// <remarks/>
        public event EnviarEmailCompletedEventHandler EnviarEmailCompleted;
        
        /// <remarks/>
        public event EnviarEmailHtmlCompletedEventHandler EnviarEmailHtmlCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/BuscarApellido", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Persona[] BuscarApellido(string apellido) {
            object[] results = this.Invoke("BuscarApellido", new object[] {
                        apellido});
            return ((Persona[])(results[0]));
        }
        
        /// <remarks/>
        public void BuscarApellidoAsync(string apellido) {
            this.BuscarApellidoAsync(apellido, null);
        }
        
        /// <remarks/>
        public void BuscarApellidoAsync(string apellido, object userState) {
            if ((this.BuscarApellidoOperationCompleted == null)) {
                this.BuscarApellidoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnBuscarApellidoOperationCompleted);
            }
            this.InvokeAsync("BuscarApellido", new object[] {
                        apellido}, this.BuscarApellidoOperationCompleted, userState);
        }
        
        private void OnBuscarApellidoOperationCompleted(object arg) {
            if ((this.BuscarApellidoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.BuscarApellidoCompleted(this, new BuscarApellidoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/EnviarEmail", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool EnviarEmail(string emailfrom, string namemailfrom, string emailto, string subject, string texto, string usuario, string password) {
            object[] results = this.Invoke("EnviarEmail", new object[] {
                        emailfrom,
                        namemailfrom,
                        emailto,
                        subject,
                        texto,
                        usuario,
                        password});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void EnviarEmailAsync(string emailfrom, string namemailfrom, string emailto, string subject, string texto, string usuario, string password) {
            this.EnviarEmailAsync(emailfrom, namemailfrom, emailto, subject, texto, usuario, password, null);
        }
        
        /// <remarks/>
        public void EnviarEmailAsync(string emailfrom, string namemailfrom, string emailto, string subject, string texto, string usuario, string password, object userState) {
            if ((this.EnviarEmailOperationCompleted == null)) {
                this.EnviarEmailOperationCompleted = new System.Threading.SendOrPostCallback(this.OnEnviarEmailOperationCompleted);
            }
            this.InvokeAsync("EnviarEmail", new object[] {
                        emailfrom,
                        namemailfrom,
                        emailto,
                        subject,
                        texto,
                        usuario,
                        password}, this.EnviarEmailOperationCompleted, userState);
        }
        
        private void OnEnviarEmailOperationCompleted(object arg) {
            if ((this.EnviarEmailCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.EnviarEmailCompleted(this, new EnviarEmailCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/EnviarEmailHtml", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool EnviarEmailHtml(string emailfrom, string namemailfrom, string emailto, string subject, string texto, string usuario, string password) {
            object[] results = this.Invoke("EnviarEmailHtml", new object[] {
                        emailfrom,
                        namemailfrom,
                        emailto,
                        subject,
                        texto,
                        usuario,
                        password});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void EnviarEmailHtmlAsync(string emailfrom, string namemailfrom, string emailto, string subject, string texto, string usuario, string password) {
            this.EnviarEmailHtmlAsync(emailfrom, namemailfrom, emailto, subject, texto, usuario, password, null);
        }
        
        /// <remarks/>
        public void EnviarEmailHtmlAsync(string emailfrom, string namemailfrom, string emailto, string subject, string texto, string usuario, string password, object userState) {
            if ((this.EnviarEmailHtmlOperationCompleted == null)) {
                this.EnviarEmailHtmlOperationCompleted = new System.Threading.SendOrPostCallback(this.OnEnviarEmailHtmlOperationCompleted);
            }
            this.InvokeAsync("EnviarEmailHtml", new object[] {
                        emailfrom,
                        namemailfrom,
                        emailto,
                        subject,
                        texto,
                        usuario,
                        password}, this.EnviarEmailHtmlOperationCompleted, userState);
        }
        
        private void OnEnviarEmailHtmlOperationCompleted(object arg) {
            if ((this.EnviarEmailHtmlCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.EnviarEmailHtmlCompleted(this, new EnviarEmailHtmlCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Persona : Rol {
        
        private string strcedulaField;
        
        private string strapellidosField;
        
        private string strnombresField;
        
        /// <comentarios/>
        public string Strcedula {
            get {
                return this.strcedulaField;
            }
            set {
                this.strcedulaField = value;
            }
        }
        
        /// <comentarios/>
        public string Strapellidos {
            get {
                return this.strapellidosField;
            }
            set {
                this.strapellidosField = value;
            }
        }
        
        /// <comentarios/>
        public string Strnombres {
            get {
                return this.strnombresField;
            }
            set {
                this.strnombresField = value;
            }
        }
    }
    
    /// <comentarios/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Persona))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2102.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Rol {
        
        private string nombreRolField;
        
        private string codigoCarreraField;
        
        /// <comentarios/>
        public string NombreRol {
            get {
                return this.nombreRolField;
            }
            set {
                this.nombreRolField = value;
            }
        }
        
        /// <comentarios/>
        public string CodigoCarrera {
            get {
                return this.codigoCarreraField;
            }
            set {
                this.codigoCarreraField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2046.0")]
    public delegate void BuscarApellidoCompletedEventHandler(object sender, BuscarApellidoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2046.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class BuscarApellidoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal BuscarApellidoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Persona[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Persona[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2046.0")]
    public delegate void EnviarEmailCompletedEventHandler(object sender, EnviarEmailCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2046.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class EnviarEmailCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal EnviarEmailCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2046.0")]
    public delegate void EnviarEmailHtmlCompletedEventHandler(object sender, EnviarEmailHtmlCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2046.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class EnviarEmailHtmlCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal EnviarEmailHtmlCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591