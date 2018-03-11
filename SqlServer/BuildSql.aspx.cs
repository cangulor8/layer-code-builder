using System;
using System.Text;
using AppWeb.App_Biz;

namespace AppWeb.SqlServer
{
    public partial class BuildSql : System.Web.UI.Page
    {
	    private StringBuilder _sb = new StringBuilder();

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RepositorySqlServer.GetDb(Ddl_Db);
            }
        }
        
        protected void Ddl_Table_SelectedIndexChanged(object sender, EventArgs e)
        {
            RepositorySqlServer.GetDbColumns(Grv_Data, Ddl_Table.SelectedItem.Text, Ddl_Db.SelectedValue);
        }

        protected void Lnb_CREATE_Click(object sender, EventArgs e)
        {
            try
            {
                var tableName = Ddl_Table.SelectedItem.Text;
                var spName = Ddl_Table.SelectedItem.Text;

                if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text) && spName.StartsWith(Txb_Prefix.Text.Trim()) && Chb_RemovePrefix.Checked)
                    spName = spName.Remove(0, Txb_Prefix.Text.Trim().Length);

                var entityName = Ddl_Table.SelectedItem.Text.Substring(1);

                var columsNotAllowedCreate = new StringBuilder();
                columsNotAllowedCreate.Append(entityName + "UpdateUser");
                columsNotAllowedCreate.Append(entityName + "Update");
                columsNotAllowedCreate.Append(entityName + "Updated");
                columsNotAllowedCreate.Append(entityName + "UpdatedBy");
                columsNotAllowedCreate.Append(entityName + "Enabled");
                columsNotAllowedCreate.Append(entityName + "Key");

                _sb.AppendLine();

                _sb.AppendLine("SET ANSI_NULLS ON");
                _sb.AppendLine("GO");
                _sb.AppendLine("SET QUOTED_IDENTIFIER ON");
                _sb.AppendLine("GO");
                _sb.AppendLine();
                _sb.AppendLine("CREATE PROCEDURE " + spName + "_Add");
                _sb.AppendLine();
                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;
                    //DATA_TYPE
                    var dataType = Grv_Data.Rows[i].Cells[4].Text;
                    //CHARACTER_MAXIMUM_LENGTH
                    var dataLength = Grv_Data.Rows[i].Cells[5].Text;

                    if (!columsNotAllowedCreate.ToString().Contains(columName))
                    {
                        if (dataType == "varchar" || dataType == "char" || dataType == "nvarchar" || dataType == "nchar")
                            _sb.AppendLine("@" + columName + " " + dataType + "(" + dataLength + "),");
                        else
                            _sb.AppendLine("@" + columName + " " + dataType + ",");
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);

                _sb.AppendLine();
                _sb.AppendLine("AS");
                _sb.AppendLine("BEGIN");
                _sb.AppendLine("\tSET NOCOUNT ON;");
                _sb.AppendLine();

                _sb.AppendLine("INSERT INTO " + tableName + " (");

                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;

                    if (!columsNotAllowedCreate.ToString().Contains(columName))
                    {
                        _sb.AppendLine(columName + ",");
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);

                _sb.AppendLine(") VALUES (");

                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;
                    if (!columsNotAllowedCreate.ToString().Contains(columName))
                    {
                        _sb.AppendLine("@" + columName + ",");
                    }
                }
                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);

                _sb.AppendLine(");");
                _sb.AppendLine();
                _sb.AppendLine("END");
                _sb.AppendLine("GO");

                Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());
            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Lnb_UPDATE_Click(object sender, EventArgs e)
        {
            try
            {
                var tableName = Ddl_Table.SelectedItem.Text;

                var entityName = Ddl_Table.SelectedItem.Text.Substring(1);
                var spName = Ddl_Table.SelectedItem.Text;

                if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text) && spName.StartsWith(Txb_Prefix.Text.Trim()) && Chb_RemovePrefix.Checked)
                    spName = spName.Remove(0, Txb_Prefix.Text.Trim().Length);

                var columsNotAllowedCreate = new StringBuilder();
                columsNotAllowedCreate.Append(entityName + "CreateUser");
                columsNotAllowedCreate.Append(entityName + "Create");
                columsNotAllowedCreate.Append(entityName + "Created");
                columsNotAllowedCreate.Append(entityName + "CreatedBy");

                _sb.AppendLine();

                _sb.AppendLine("SET ANSI_NULLS ON");
                _sb.AppendLine("GO");
                _sb.AppendLine("SET QUOTED_IDENTIFIER ON");
                _sb.AppendLine("GO");
                _sb.AppendLine();
                _sb.AppendLine("CREATE PROCEDURE " + spName + "_Update");
                _sb.AppendLine();
                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;
                    //DATA_TYPE
                    var dataType = Grv_Data.Rows[i].Cells[4].Text;
                    //CHARACTER_MAXIMUM_LENGTH
                    var dataLength = Grv_Data.Rows[i].Cells[5].Text;

                    if (!columsNotAllowedCreate.ToString().Contains(columName))
                    {
                        if (dataType == "varchar" || dataType == "char" || dataType == "nvarchar" || dataType == "nchar")
                            _sb.AppendLine("@" + columName + " " + dataType + "(" + dataLength + "),");
                        else
                            _sb.AppendLine("@" + columName + " " + dataType + ",");
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);

                _sb.AppendLine();
                _sb.AppendLine("AS");
                _sb.AppendLine("BEGIN");
                _sb.AppendLine("\tSET NOCOUNT ON;");
                _sb.AppendLine();

                _sb.AppendLine("UPDATE " + tableName + " SET");

                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;

                    if (!columsNotAllowedCreate.ToString().Contains(columName))
                    {
                        if (columName != entityName + "Key" && columName != "CompanyKey")
                            _sb.AppendLine(columName + " = @" + columName + ",");
                    }
                }


                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);

                _sb.AppendLine("WHERE " + entityName + "Key = @" + entityName + "Key AND");
                _sb.AppendLine("CompanyKey = @CompanyKey;");
                _sb.AppendLine();
                _sb.AppendLine("END");
                _sb.AppendLine("GO");

                Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());

            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Lnb_DELETE_Click(object sender, EventArgs e)
        {
            try
            {
                var tableName = Ddl_Table.SelectedItem.Text;

                var entityName = Ddl_Table.SelectedItem.Text.Substring(1);
                var spName = Ddl_Table.SelectedItem.Text;

                if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text) && spName.StartsWith(Txb_Prefix.Text.Trim()) && Chb_RemovePrefix.Checked)
                    spName = spName.Remove(0, Txb_Prefix.Text.Trim().Length);

                var columsAllowedDelete = new StringBuilder();
                columsAllowedDelete.Append(entityName + "Key,");
                columsAllowedDelete.Append("CompanyKey");

                _sb.AppendLine();

                _sb.AppendLine("SET ANSI_NULLS ON");
                _sb.AppendLine("GO");
                _sb.AppendLine("SET QUOTED_IDENTIFIER ON");
                _sb.AppendLine("GO");
                _sb.AppendLine();
                _sb.AppendLine("CREATE PROCEDURE " + spName + "_Delete");
                _sb.AppendLine();

                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;
                    //DATA_TYPE
                    var dataType = Grv_Data.Rows[i].Cells[4].Text;
                    //CHARACTER_MAXIMUM_LENGTH
                    var dataLength = Grv_Data.Rows[i].Cells[5].Text;

                    if (columsAllowedDelete.ToString().Contains(columName))
                    {
                        if (dataType == "varchar" || dataType == "char")
                            _sb.AppendLine("@" + columName + " " + dataType + "(" + dataLength + "),");
                        else
                            _sb.AppendLine("@" + columName + " " + dataType + ",");
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);

                _sb.AppendLine();
                _sb.AppendLine("AS");
                _sb.AppendLine("BEGIN");
                _sb.AppendLine("\tSET NOCOUNT ON;");
                _sb.AppendLine();

                _sb.AppendLine("DELETE FROM " + tableName);
                _sb.AppendLine("WHERE " + entityName + "Key = @" + entityName + "Key AND");
                _sb.AppendLine("CompanyKey = @CompanyKey;");
                _sb.AppendLine();
                _sb.AppendLine("END");
                _sb.AppendLine("GO");

                Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());

            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Lnb_READ_Click(object sender, EventArgs e)
        {
            try
            {
                var tableName = Ddl_Table.SelectedItem.Text;
                var spName = Ddl_Table.SelectedItem.Text;

                if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text) && spName.StartsWith(Txb_Prefix.Text.Trim()) && Chb_RemovePrefix.Checked)
                    spName = spName.Remove(0, Txb_Prefix.Text.Trim().Length);

                var entityName = Ddl_Table.SelectedItem.Text.Substring(1);

                var columsAllowedRead = new StringBuilder();
                columsAllowedRead.Append(entityName + "Key,");
                columsAllowedRead.Append("CompanyKey");

                _sb.AppendLine();

                _sb.AppendLine("SET ANSI_NULLS ON");
                _sb.AppendLine("GO");
                _sb.AppendLine("SET QUOTED_IDENTIFIER ON");
                _sb.AppendLine("GO");
                _sb.AppendLine();
                _sb.AppendLine("CREATE PROCEDURE " + spName + "_Get");
                _sb.AppendLine();
                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;
                    //DATA_TYPE
                    var dataType = Grv_Data.Rows[i].Cells[4].Text;
                    //CHARACTER_MAXIMUM_LENGTH
                    var dataLength = Grv_Data.Rows[i].Cells[5].Text;

                    if (columsAllowedRead.ToString().Contains(columName))
                    {
                        if (dataType == "varchar" || dataType == "char")
                            _sb.AppendLine("@" + columName + " " + dataType + "(" + dataLength + "),");
                        else
                            _sb.AppendLine("@" + columName + " " + dataType + ",");
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);

                _sb.AppendLine();
                _sb.AppendLine("AS");
                _sb.AppendLine("BEGIN");
                _sb.AppendLine("\tSET NOCOUNT ON;");
                _sb.AppendLine();

                _sb.AppendLine("SELECT");

                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;

                    _sb.AppendLine(columName + ",");
                }


                _sb = _sb.Remove(_sb.ToString().Length - 1, 1);

                _sb.AppendLine("FROM " + tableName);
                _sb.AppendLine("WHERE " + entityName + "Key = @" + entityName + "Key AND");
                _sb.AppendLine("CompanyKey = @CompanyKey;");
                _sb.AppendLine();
                _sb.AppendLine("END");
                _sb.AppendLine("GO");

                Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());
            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Lnb_READALL_Click(object sender, EventArgs e)
        {
            try
            {
                var tableName = Ddl_Table.SelectedItem.Text;
                var spName = Ddl_Table.SelectedItem.Text;

                if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text) && spName.StartsWith(Txb_Prefix.Text.Trim()) && Chb_RemovePrefix.Checked)
                    spName = spName.Remove(0, Txb_Prefix.Text.Trim().Length);


                var entityName = Ddl_Table.SelectedItem.Text.Substring(1);

                var columsAllowedRead = new StringBuilder();
                columsAllowedRead.Append(entityName + "Key,");
                columsAllowedRead.Append("CompanyKey");

                _sb.AppendLine();

                _sb.AppendLine("SET ANSI_NULLS ON");
                _sb.AppendLine("GO");
                _sb.AppendLine("SET QUOTED_IDENTIFIER ON");
                _sb.AppendLine("GO");
                _sb.AppendLine();
                _sb.AppendLine("CREATE PROCEDURE " + spName + "_GetAllCompany");
                _sb.AppendLine();
                    
                _sb.AppendLine("@CompanyKey int");

                _sb.AppendLine();
                _sb.AppendLine("AS");
                _sb.AppendLine("BEGIN");
                _sb.AppendLine("\tSET NOCOUNT ON;");
                _sb.AppendLine();

                _sb.AppendLine("SELECT ");

                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;

                    _sb.AppendLine(columName + ",");
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);

                _sb.AppendLine("FROM " + tableName);
                _sb.AppendLine("WHERE CompanyKey = @CompanyKey;");
                _sb.AppendLine();
                _sb.AppendLine("END");
                _sb.AppendLine("GO");

                Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());
            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Ddl_Db_SelectedIndexChanged(object sender, EventArgs e)
        {
            RepositorySqlServer.GetDbTables(Ddl_Table, Ddl_Db.SelectedValue);
        }
    }
}



