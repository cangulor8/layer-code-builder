<%@ Page Language="c#" MasterPageFile="~/Site.Master" Inherits="AppWeb.MySql.BuildHtml" CodeBehind="BuildHtml.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset>
        <legend>Generador de Formularios</legend>
        <asp:Literal runat="server" EnableViewState="false" ID="Ltr_Message"></asp:Literal>

        <nav class="navbar navbar-default" role="navigation">
            <div class="collapse navbar-collapse navbar-ex1-collapse">
                <div class="navbar-form navbar-left" role="search">
                       <div class="form-group">
                        <asp:DropDownList ID="Ddl_Db" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Ddl_Db_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <asp:DropDownList ID="Ddl_Table" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Ddl_Table_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                </div>
            </div>
            <!-- /.navbar-collapse -->
        </nav>

        <nav class="navbar navbar-default" role="navigation">
            <div class="collapse navbar-collapse navbar-ex1-collapse">
                <div class="navbar-form navbar-left" role="search">
                    <asp:LinkButton CssClass="btn btn-primary" runat="server" ID="Lnb_Create" OnClick="Lnb_Create_Click">Create</asp:LinkButton>
                    <asp:LinkButton CssClass="btn btn-primary" runat="server" ID="Lnb_Create_Behind" OnClick="Lnb_Create_Behind_Click">Create Behind</asp:LinkButton>
                    <asp:LinkButton CssClass="btn btn-primary" runat="server" OnClick="Lnb_Read_Click">Read</asp:LinkButton>
                    <asp:LinkButton CssClass="btn btn-success" runat="server" OnClick="Lnb_Update_Click">Update</asp:LinkButton>
                    <asp:LinkButton CssClass="btn btn-info" runat="server" OnClick="Lnb_Save_Click">Save</asp:LinkButton>
                </div>
            </div>
            <!-- /.navbar-collapse -->
        </nav>

        <div class="row">

            <div class="col-md-12">
                <pre class="brush: xml; bloggerMode: true;">
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





