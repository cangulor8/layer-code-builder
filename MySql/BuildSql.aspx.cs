using System;
using System.Text;
using AppWeb.App_Biz;

namespace AppWeb.MySql
{
    public partial class BuildSql : System.Web.UI.Page
    {
        private StringBuilder _sb = new StringBuilder();

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

        protected void Lnb_CREATE_Click(object sender, EventArgs e)
        {
            try
            {
                var tableName = Ddl_Table.SelectedItem.Text;

                var spName = Ddl_Table.SelectedItem.Text;

                if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text) && spName.StartsWith(Txb_Prefix.Text.Trim()) && Chb_RemovePrefix.Checked)
                    spName = spName.Remove(0, Txb_Prefix.Text.Trim().Length);

               // var entityName = Ddl_Table.SelectedItem.Text;

                var entityNameOriginal = Ddl_Table.SelectedItem.Text;

                var columsNotAllowedCreate = new StringBuilder();
                columsNotAllowedCreate.Append(entityNameOriginal + "_updated_by,");
                columsNotAllowedCreate.Append(entityNameOriginal + "_updated_by_tag,");
                columsNotAllowedCreate.Append(entityNameOriginal + "_updated_at,");
	            columsNotAllowedCreate.Append(entityNameOriginal + "_enabled,");
				//columsNotAllowedCreate.Append(entityNameOriginal + "_created_at");
				_sb.AppendLine();
                _sb.AppendLine("DELIMITER $$");
                _sb.AppendLine("CREATE PROCEDURE `" + spName + "_insert`(");
                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;
                    //DATA_TYPE
                    var dataType = Grv_Data.Rows[i].Cells[4].Text;
                    //CHARACTER_MAXIMUM_LENGTH
                    var dataLength = "";
                    if (Grv_Data.Rows[i].Cells.Count > 5)
                    {
                        dataLength = Grv_Data.Rows[i].Cells[5].Text;
                    }

                    if (!columsNotAllowedCreate.ToString().Contains(columName))
                    {
                        if (dataType == "varchar" || dataType == "char" || dataType == "nvarchar" || dataType == "nchar")
                            _sb.AppendLine("IN `_" + columName + "` " + dataType + "(" + dataLength + "),");
                        else
                            _sb.AppendLine("IN `_" + columName + "` " + dataType + ",");
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);
                _sb.AppendLine(")");
                _sb.AppendLine("SQL SECURITY INVOKER");
                _sb.AppendLine("BEGIN");
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
                        _sb.AppendLine("_" + columName + ",");
                    }
                }
                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);
                _sb.AppendLine(");");
                _sb.AppendLine("END $$");
                _sb.AppendLine("DELIMITER ;");

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
                var entityNameOriginal = Ddl_Table.SelectedItem.Text;
                var spName = Ddl_Table.SelectedItem.Text;

                if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text) && spName.StartsWith(Txb_Prefix.Text.Trim()) && Chb_RemovePrefix.Checked)
                    spName = spName.Remove(0, Txb_Prefix.Text.Trim().Length);

                var columsNotAllowed = new StringBuilder();
                columsNotAllowed.Append(entityNameOriginal + "_created_by,");
                columsNotAllowed.Append(entityNameOriginal + "_created_at,");
                columsNotAllowed.Append(entityNameOriginal + "_created_by_tag,");
                //columsNotAllowed.Append(entityNameOriginal + "_updated_at,");
                columsNotAllowed.Append("company_key");

                _sb.AppendLine();

                _sb.AppendLine();
                _sb.AppendLine("DELIMITER $$");
                _sb.AppendLine("CREATE PROCEDURE `" + spName + "_update`");
                _sb.AppendLine("(");
                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;
                    //DATA_TYPE
                    var dataType = Grv_Data.Rows[i].Cells[4].Text;
                    //CHARACTER_MAXIMUM_LENGTH
                    var dataLength = Grv_Data.Rows[i].Cells[5].Text;

                    if (!columsNotAllowed.ToString().Contains(columName))
                    {
                        if (dataType == "varchar" || dataType == "char" || dataType == "nvarchar" || dataType == "nchar")
                            _sb.AppendLine("IN `_" + columName + "` " + dataType + "(" + dataLength + "),");
                        else
                            _sb.AppendLine("IN `_" + columName + "` " + dataType + ",");
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);

                _sb.AppendLine(")");
                _sb.AppendLine("SQL SECURITY INVOKER");
                _sb.AppendLine("BEGIN");
                _sb.AppendLine("UPDATE " + tableName + " SET");

                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;

                    if (!columsNotAllowed.ToString().Contains(columName))
                    {
                        if (columName != "id" && columName != "company_key")
                        {
                            _sb.AppendLine(columName + " = _" + columName + ",");
                        }
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);
                _sb.AppendLine("WHERE id = _id;");
                _sb.AppendLine("END $$");
                _sb.AppendLine("DELIMITER ;");

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

                var entityName = Ddl_Table.SelectedItem.Text;
                var spName = Ddl_Table.SelectedItem.Text;

                if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text) && spName.StartsWith(Txb_Prefix.Text.Trim()) && Chb_RemovePrefix.Checked)
                    spName = spName.Remove(0, Txb_Prefix.Text.Trim().Length);

                var columsAllowedDelete = new StringBuilder();
                columsAllowedDelete.Append("id");

                _sb.AppendLine();
                _sb.AppendLine("DELIMITER $$");
                _sb.AppendLine("CREATE PROCEDURE `" + spName + "_delete`(");
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
                            _sb.AppendLine("IN `_" + columName + "` " + dataType + "(" + dataLength + "),");
                        else
                            _sb.AppendLine("IN `_" + columName + "` " + dataType + ",");
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);
                _sb.AppendLine(")");
                _sb.AppendLine("BEGIN");
                _sb.AppendLine("DELETE FROM " + tableName);
                _sb.AppendLine("WHERE id = _id;");
                _sb.AppendLine("END $$");
                _sb.AppendLine("DELIMITER ;");

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

                var columsAllowedRead = new StringBuilder();
                columsAllowedRead.Append("id");

                _sb.AppendLine();
                _sb.AppendLine("DELIMITER $$");
                _sb.AppendLine("CREATE PROCEDURE `" + spName + "_get`(");
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
                            _sb.AppendLine("IN `_" + columName + "` " + dataType + "(" + dataLength + "),");
                        else
                            _sb.AppendLine("IN `_" + columName + "` " + dataType + ",");
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);
                _sb.AppendLine(")");
                _sb.AppendLine("SQL SECURITY INVOKER");
                _sb.AppendLine("BEGIN");
                _sb.AppendLine("SELECT");
                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;

                    _sb.AppendLine(columName + ",");
                }
                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);
                _sb.AppendLine("FROM " + tableName);
                _sb.AppendLine("WHERE id = _id;");
                _sb.AppendLine("END $$");
                _sb.AppendLine("DELIMITER ;");


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

                var columsAllowedRead = new StringBuilder();
                columsAllowedRead.Append("company_key");

                _sb.AppendLine();
                _sb.AppendLine("DELIMITER $$");
                _sb.AppendLine("CREATE PROCEDURE `" + spName + "_get_by_company`(");
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
                            _sb.AppendLine("IN `_" + columName + "` " + dataType + "(" + dataLength + "),");
                        else
                            _sb.AppendLine("IN `_" + columName + "` " + dataType + ",");
                    }
                }

                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);
                _sb.AppendLine(")");
                _sb.AppendLine("SQL SECURITY INVOKER");
                _sb.AppendLine("BEGIN");
                _sb.AppendLine("SELECT");
                for (var i = 0; i < Grv_Data.Rows.Count; i++)
                {
                    //COLUMN_NAME
                    var columName = Grv_Data.Rows[i].Cells[0].Text;

                    _sb.AppendLine(columName + ",");
                }
                _sb = _sb.Remove(_sb.ToString().LastIndexOf(','), 1);
                _sb.AppendLine("FROM " + tableName);
                _sb.AppendLine("WHERE company_key = _company_key;");
                _sb.AppendLine("END $$");
                _sb.AppendLine("DELIMITER ;");


                Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());
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



