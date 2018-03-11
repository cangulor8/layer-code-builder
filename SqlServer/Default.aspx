<%@ Page Language="c#" MasterPageFile="~/Site.Master" Inherits="AppWeb.SqlServer.BuildCodeSqlServer" CodeBehind="Default.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset>
        <legend>Generador de Clases</legend>
        <asp:Literal runat="server" EnableViewState="false" ID="Ltr_Message"></asp:Literal>

        <nav class="navbar navbar-default" role="navigation">
            <div class="collapse navbar-collapse navbar-ex1-collapse">
                <div class="navbar-form navbar-left" role="search">
                    Db
                    <div class="form-group">
                        <asp:DropDownList ID="Ddl_Db" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Ddl_Db_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    Table
                    <div class="form-group">
                        <asp:DropDownList ID="Ddl_Table" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Ddl_Table_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    Namespace
                    <div class="form-group">
                        <asp:TextBox ID="Txb_Namespace" runat="server" CssClass="form-control" placeholder="Namespace"></asp:TextBox>
                    </div>
                    Prefix
                    <div class="form-group">
                        <asp:TextBox ID="Txb_Prefix" runat="server" CssClass="form-control" placeholder="Prefijo"></asp:TextBox>
                    </div>
                   <%-- <div class="checkbox">
    <label>
      <asp:CheckBox runat="server" ID="Chb_MultiTenant" Checked="true" /> Multi Tenant
    </label>
  </div>

                    TenantKey
                    <div class="form-group">
                        <asp:TextBox ID="Txb_Tenant" runat="server" CssClass="form-control" placeholder="Prefijo"></asp:TextBox>
                    </div>--%>

                </div>
            </div>
            <!-- /.navbar-collapse -->
        </nav>

        <nav class="navbar navbar-default" role="navigation">
            <div class="collapse navbar-collapse navbar-ex1-collapse">
                <div class="navbar-form navbar-left" role="search">
                    <asp:LinkButton CssClass="btn btn-primary" runat="server" ID="Lnb_Entity" OnClick="Lnb_Entity_Click">Entity</asp:LinkButton>
                    <asp:LinkButton CssClass="btn btn-success" runat="server" ID="Lnb_Factory" OnClick="Lnb_Factory_Click">Factory</asp:LinkButton>
                    <asp:LinkButton CssClass="btn btn-info" runat="server" ID="Lnb_Repository" OnClick="Lnb_Repository_Click">Repository</asp:LinkButton>
                </div>
            </div>
            <!-- /.navbar-collapse -->
        </nav>

        <div class="row">

            <div class="col-md-12">
                <pre class="brush: csharp; bloggerMode: true;">
                 <asp:Literal ID="Ltr_Code" runat="server"></asp:Literal>
              </pre>
            </div>

            <div class="col-md-12">
                <asp:GridView AutoGenerateColumns="false" ID="Grv_Data" runat="server" CssClass="table table-hover">
                    <Columns>
                        <asp:BoundField HeaderText="COLUMN_NAME" DataField="COLUMN_NAME" />
                        <asp:BoundField HeaderText="ORDINAL_POSITION" DataField="ORDINAL_POSITION" />
                        <asp:BoundField HeaderText="COLUMN_DEFAULT" DataField="COLUMN_DEFAULT" />
                        <asp:BoundField HeaderText="IS_NULLABLE" DataField="IS_NULLABLE" />
                        <asp:BoundField HeaderText="DATA_TYPE" DataField="DATA_TYPE" />
                        <asp:BoundField HeaderText="CHARACTER_MAXIMUM_LENGTH" DataField="CHARACTER_MAXIMUM_LENGTH" />
                        <asp:BoundField HeaderText="DESCRIPTION" DataField="DESCRIPTION" />
                        <asp:TemplateField HeaderText="Read">
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="Chb_Read" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="ReadAll">
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="Chb_ReadAll" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

        </div>


    </fieldset>
</asp:Content>





