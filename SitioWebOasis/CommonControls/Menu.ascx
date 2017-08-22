<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Menu.ascx.cs" Inherits="OAS_SitioWeb.CommonControls.C_Menu" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<ul class="nav side-menu">
    <li>
        <a>
            <i class="fa fa-home"></i> <%: this.Titulo %> 
            <span class="fa fa-chevron-down"></span>
        </a>
        <ul class="nav child_menu">
            <%  foreach( OAS_SitioWeb.CommonClasses.UI.LinkMenu item in this._ListaVinculos ) { %>
                <li class="active"> 
                    <% Response.Write(item.GetHTMLLink()); %>
                </li>
            <% } %>
        </ul>
    </li>
</ul>