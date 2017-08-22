<%@ OutputCache Duration="1200" VaryByControl="SubSite"%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Header.ascx.cs" Inherits="OAS_SitioWeb.CommonControls.C_Header" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<link href="../oasis.css" rel="stylesheet" type="text/css">
<table cellSpacing="0" cellPadding="0" width="100%" border="0" class="clsTableHeader">
	<!--DWLayoutTable-->
	<tr>
		<td vAlign="top" width="100%">
			<table height="71" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<!--DWLayoutTable-->
				<tr>
					<td width="200" rowspan="2" vAlign="top"><img src="/OAS_SitioWeb/images/OASisLogo.gif" width="200" height="100">
					</td>
					<td width="100%" height="79" align="middle" vAlign="center">
						<asp:Label id="lblSite" runat="server" CssClass="clsHeaderSiteTitle"></asp:Label><br>
						<asp:Label id="lblSubSite" CssClass="clsHeaderSubSiteTitle" runat="server" Height="10px"></asp:Label>
					</td>
				</tr>
				<tr>
					<td height="21" vAlign="top"><table width="100%" border="0" cellpadding="0" cellspacing="0">
							<!--DWLayoutTable-->
							<tr>
								<td width="100%" height="21">
									<table cellSpacing="0" cellPadding="0" width="100%" border="0">
										<!--DWLayoutTable-->
										<tr>
											<td width="70%" align="left" vAlign="bottom" nowrap>
												<asp:DataList id="dtlstTabs" runat="server" RepeatDirection="Horizontal" SelectedIndex="0">
													<SelectedItemStyle BorderWidth="1px" ForeColor="Black" CssClass="SelTab"></SelectedItemStyle>
													<SelectedItemTemplate>
														&nbsp;
														<asp:HyperLink id=HyperLink2 runat="server" NavigateUrl="" Text='<%# DataBinder.Eval(Container.DataItem, "LinkName") %>'>HyperLink</asp:HyperLink>&nbsp;&nbsp;
													</SelectedItemTemplate>
													<ItemStyle BorderWidth="1px" ForeColor="White" CssClass="UnselTab"></ItemStyle>
													<ItemTemplate>
														&nbsp;
														<asp:HyperLink id=HyperLink1 runat="server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "FullURL") %>' Text='<%# DataBinder.Eval(Container.DataItem, "LinkName") %>'>
														</asp:HyperLink>&nbsp;&nbsp;
													</ItemTemplate>
												</asp:DataList>
											</td>
											<TD vAlign="bottom" align="right" width="100%">
												<TABLE class="clsRowDate" cellSpacing="0" cellPadding="0" width="100%">
													<!--DWLayoutTable-->
													<TR>
														<TD vAlign="bottom" width="100%">
															<asp:Label id="lblFecha" runat="server" ForeColor="White"> 
                    Fecha</asp:Label>&nbsp;&nbsp;
														</TD>
													</TR> <!--DWLayoutTable--></TABLE>
											</TD>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
