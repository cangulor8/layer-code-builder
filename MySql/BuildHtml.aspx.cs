using System;
using System.Text;
using AppWeb.App_Biz;

namespace AppWeb.MySql
{
    public partial class BuildHtml : System.Web.UI.Page
    {
	    private readonly StringBuilder _sb = new StringBuilder();

	    protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RepositoryMySql.GetDb(Ddl_Db);
            }
        }

        protected void Ddl_Table_SelectedIndexChanged(object sender, EventArgs e)
        {
            RepositoryMySql.GetDbColumns(Grv_Data, Ddl_Table.SelectedItem.Text, Ddl_Db.SelectedValue);
        }

        protected void Lnb_Create_Click(object sender, EventArgs e)
        {
            try
            {
                var entityName = Ddl_Table.SelectedItem.Text.Substring(1);

                var columsNotAllowed = new StringBuilder();
                columsNotAllowed.Append(entityName + "UpdateUser,");
                columsNotAllowed.Append(entityName + "Update,");
                columsNotAllowed.Append(entityName + "CreateUser,");
                columsNotAllowed.Append(entityName + "Create,");
                columsNotAllowed.Append(entityName + "CompanyKey,");
                columsNotAllowed.Append(entityName + "Key");

                _sb.AppendLine();

                _sb.AppendLine("<ol class=\"breadcrumb\">");
                _sb.AppendLine("<li><a href=\"/\" title=\"<%: Resources.Title.Desk %>\"><i class=\"fa fa-desktop\"></i></a></li>");
                _sb.AppendLine("<li><a href=\"/WebForm/CF/\" title=\"<%: Resources.Title.Setting %>\"><i class=\"fa fa-wrench\"></i></a></li>");
                _sb.AppendLine("<li><a href=\"Default\"><%: Resources.Title.Branch %></a></li>");
                _sb.AppendLine("<li class=\"active\"><%: Resources.Action.Create %></li>");
                _sb.AppendLine("</ol>");


                _sb.AppendLine("<asp:UpdatePanel ID=\"Upp_Default\" runat=\"server\">");
                _sb.AppendLine("<ContentTemplate>");
                _sb.AppendLine("<asp:Literal runat=\"server\" ID=\"Ltr_Message\" EnableViewState=\"false\"></asp:Literal>");
                _sb.AppendLine();

                _sb.AppendLine("<div class=\"panel panel-default\">");
                _sb.AppendLine("<div class=\"panel-heading\">");
                _sb.AppendLine("<i class=\"fa fa-plus\"></i>&nbsp;<%: Resources.Action.Create %> / " + entityName);
                _sb.AppendLine("</div>");
                _sb.AppendLine("<div class=\"panel-body\">");
                _sb.AppendLine("<div class=\"row\">");
                _sb.AppendLine("<div class=\"col-sm-6\">");

                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;
                    //DATA_TYPE
                    var dataType = Grv_Data.Rows[i].Cells[4].Text;
                    //CHARACTER_MAXIMUM_LENGTH
                    var lenght = Grv_Data.Rows[i].Cells[5].Text;
                    //DESCRIPTION
                    var label = Grv_Data.Rows[i].Cells[6].Text;

                    if (!columsNotAllowed.ToString().Contains(columName))
                    {
                        _sb.AppendLine("<div class=\"form-group\">");

                        if (!string.IsNullOrWhiteSpace(label))
                            _sb.AppendLine("<label>" + columName + "</label>");
                        else
                            _sb.AppendLine("<label>" + label + "</label>");
                        if (dataType == "bit")
                        {
                            _sb.AppendLine("<asp:DropDownList runat=\"server\" ID=\"Ddl_" + columName + "\" CssClass=\"form-control input-sm\">");
                            _sb.AppendLine("<asp:ListItem Value=\"False\" Text=\"<%: Resources.Label.Not %>\"></asp:ListItem>");
                            _sb.AppendLine("<asp:ListItem Value=\"True\" Text=\"<%: Resources.Label.Yes %>\"></asp:ListItem>");
                            _sb.AppendLine("</asp:DropDownList>");

                            //_sb.AppendLine("<label class=\"switch\">");
                            //_sb.AppendLine("<asp:CheckBox runat=\"server\" ID=\"Chb_" + columName + "\" Checked=\"true\" />");
                            //_sb.AppendLine("<span></span>");
                            //_sb.AppendLine("</label>");
                        }
                        else if (dataType == "varchar" || dataType == "char")
                        {
                            _sb.AppendLine("<asp:TextBox runat=\"server\" MaxLength=\"" + lenght + "\" ID=\"Txb_" + columName + "\" CssClass=\"form-control input-sm\"></asp:TextBox>");
                        }
                        else
                        {
                            _sb.AppendLine("<asp:TextBox runat=\"server\" ID=\"Txb_" + columName + "\" CssClass=\"form-control input-sm\"></asp:TextBox>");
                        }
                        _sb.AppendLine("</div>");
                    }

                }

                _sb.AppendLine("</div>");
                _sb.AppendLine("</div>");
                _sb.AppendLine("</div>");

                _sb.AppendLine(" <div class=\"panel-footer\">");
                _sb.AppendLine("<asp:LinkButton ID=\"Lnb_Save\" runat=\"server\" EnableViewState=\"False\" OnClick=\"Lnb_Save_Click\" CssClass=\"btn btn-sm btn-success\"><%: Resources.Action.Create %></asp:LinkButton>");
                _sb.AppendLine("</div>");
                _sb.AppendLine("</div>");
                _sb.AppendLine("</ContentTemplate>");
                _sb.AppendLine("</asp:UpdatePanel>");


                Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());

                #region Crea el archivo

                //StreamWriter sw = new StreamWriter(Server.MapPath("Class") + "\\" + nombre + ".cs", false, Encoding.ASCII);
                //sw.Write(taResults.Value);
                //sw.Close();

                #endregion
            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Lnb_Create_Behind_Click(object sender, EventArgs e)
        {
            try
            {
                var entityName = Ddl_Table.SelectedItem.Text.Substring(1);
                var objName = Helper.FirstToLower(entityName);

                var columsNotAllowed = new StringBuilder();
                columsNotAllowed.Append(entityName + "UpdateUser,");
                columsNotAllowed.Append(entityName + "Update,");
                columsNotAllowed.Append(entityName + "CreateUser,");
                columsNotAllowed.Append(entityName + "Create,");
                columsNotAllowed.Append(entityName + "CompanyKey,");
                columsNotAllowed.Append(entityName + "Key");

                _sb.AppendLine();
                _sb.AppendLine("protected void Lnb_Save_Click(object sender, EventArgs e)");
                _sb.AppendLine("{");
                _sb.AppendLine("try");
                _sb.AppendLine("{");
                _sb.AppendLine("var " + objName + " = new " + entityName + "();");
                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;
                    //DATA_TYPE
                    var dataType = Grv_Data.Rows[i].Cells[4].Text;
                    //CHARACTER_MAXIMUM_LENGTH
                    var lenght = Grv_Data.Rows[i].Cells[5].Text;
                    //DESCRIPTION
                    var label = Grv_Data.Rows[i].Cells[6].Text;

                    if (!columsNotAllowed.ToString().Contains(columName))
                    {
                        if (dataType == "bit")
                        {
                            _sb.AppendLine(objName + "." + columName + " = bool.Parse(" + "Ddl_" + columName + ".SelectedValue);");
                        }
                        else if (dataType == "varchar" || dataType == "char")
                        {
                            _sb.AppendLine(objName + "." + columName + " = " + "Txb_" + columName + ".Text;");
                        }
                        else if (dataType == "bigint")
                        {
                            _sb.AppendLine(objName + "." + columName + " = long.Parse(" + "Txb_" + columName + ".Text);");
                        }
                        else if (dataType == "int")
                        {
                            _sb.AppendLine(objName + "." + columName + " = int.Parse(" + "Txb_" + columName + ".Text);");
                        }
                    }

                }

                _sb.AppendLine("_Samba.SqlServerCompany." + entityName + ".Add(" + objName + ");");
                _sb.AppendLine("AlertSuccessSave();");
                _sb.AppendLine("}");
                _sb.AppendLine("catch (Exception ex)");
                _sb.AppendLine("{");
                _sb.AppendLine("Ltr_Message.Text = _SmbAlert.Error(ex);");

                _sb.AppendLine("AlertError(ex);");
                _sb.AppendLine("}");
                _sb.AppendLine("}");

                Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());

                #region Crea el archivo

                //StreamWriter sw = new StreamWriter(Server.MapPath("Class") + "\\" + nombre + ".cs", false, Encoding.ASCII);
                //sw.Write(taResults.Value);
                //sw.Close();

                #endregion
            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }

        }
        protected void Lnb_Read_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Lnb_Update_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Lnb_Save_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Ddl_Db_SelectedIndexChanged(object sender, EventArgs e)
        {
            RepositoryMySql.GetDbTables(Ddl_Table, Ddl_Db.SelectedValue);
        }


    }
}



