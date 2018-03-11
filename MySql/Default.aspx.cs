using System;
using System.Text;
using System.Web.UI;
using AppWeb.App_Biz;

namespace AppWeb.MySql
{
    public partial class BuildCodeMysql : Page
    {
        private readonly StringBuilder _sb = new StringBuilder();
        protected const char QuoteCh = '"';
        protected string Quote = QuoteCh.ToString();

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

        protected void Lnb_Entity_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Txb_Namespace.Text))
                {
                    var nombre = Helper.GetColumnNameRubyStyleForEntity(Ddl_Table.SelectedItem.Text);

                    _sb.AppendLine();
                    _sb.AppendLine("using System;");
                    _sb.AppendLine("using System.Data;");
                    _sb.AppendLine();

                    _sb.AppendLine("namespace " + Txb_Namespace.Text.Trim());
                    _sb.AppendLine("{");
                    _sb.AppendLine("\tpublic sealed class " + nombre);
                    _sb.AppendLine("\t{");

                    for (var i = 0; i < Grv_Data.Rows.Count; i++)
                    {
                        var columName = Grv_Data.Rows[i].Cells[0].Text;
                        var dataType = Grv_Data.Rows[i].Cells[4].Text;
                        var isNullable = Grv_Data.Rows[i].Cells[3].Text;

                        try
                        {
                            var maxLenght = "";

                            if (Grv_Data.Rows[i].Cells.Count >= 5)
                            {
                                maxLenght = Grv_Data.Rows[i].Cells[5].Text;
                            }

                            if (!string.IsNullOrWhiteSpace(maxLenght) && maxLenght == "36" && dataType == "char" && isNullable == "NO")
                            {
                                _sb.AppendLine("\t\tpublic Guid " + Helper.GetColumnNameRubyStyleForEntity(columName) + " { get; set; }");
                            }
                            else if (!string.IsNullOrWhiteSpace(maxLenght) && maxLenght == "36" && dataType == "char" && isNullable == "YES")
                            {
                                _sb.AppendLine("\t\tpublic Guid? " + Helper.GetColumnNameRubyStyleForEntity(columName) + " { get; set; }");
                            }
                            else
                            {
                                if (dataType == "tinyint")
                                {
                                    _sb.AppendLine("\t\tpublic bool " + Helper.GetColumnNameRubyStyleForEntity(columName) + " { get; set; }");
                                }
                                else
                                {
                                    if ((dataType == "bigint" || dataType == "int" || dataType == "decimal") && isNullable == "YES")
                                    {
                                        _sb.AppendLine("\t\tpublic " + Helper.GetVarType(dataType) + "? " + Helper.GetColumnNameRubyStyleForEntity(columName) + " { get; set; }");
                                    }
                                    else
                                    {
                                        _sb.AppendLine("\t\tpublic " + Helper.GetVarType(dataType) + " " + Helper.GetColumnNameRubyStyleForEntity(columName) + " { get; set; }");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(columName + ": " + ex.Message);
                        }
                    }

                    _sb.AppendLine("\t}");
                    _sb.AppendLine("}");

                    _sb.Replace("'", Quote); // Replace ['] character with ["] character

                    Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());

                    #region Crea el archivo

                    //StreamWriter sw = new StreamWriter(Server.MapPath("Class") + "\\" + nombre + ".cs", false, Encoding.ASCII);
                    //sw.Write(taResults.Value);
                    //sw.Close();

                    #endregion
                }
                else
                {
                    Ltr_Message.Text = "<div class='alert alert-danger'>Ingresa un namespace</div>";
                }
            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Lnb_Factory_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Txb_Namespace.Text))
                {
                    var entityName = Helper.GetColumnNameRubyStyleForEntity(Ddl_Table.SelectedItem.Text);

                    _sb.AppendLine();
                    _sb.AppendLine("using AppBiz._Generic;");
                    _sb.AppendLine("using System;");
                    _sb.AppendLine("using System.Data;");
                    _sb.AppendLine();
                    _sb.AppendLine("namespace " + Txb_Namespace.Text);
                    _sb.AppendLine("{");
                    _sb.AppendLine("\tpublic sealed class " + entityName + "Factory  : Builder");
                    _sb.AppendLine("\t{  ");
                    _sb.AppendLine();

                    //_sb.AppendLine("\t\tpublic bool _isInitialized { get; private set; }");
                    //_sb.AppendLine("\t\tpublic object[] _values { get; private set; }");

                    _sb.AppendLine();

                    #region Indices

                    _sb.AppendLine("\t\t#region Indices");
                    _sb.AppendLine();
                    for (var i = 0; i < Grv_Data.Rows.Count; i++)
                    {
                        var columName = Helper.GetColumnNameRubyStyleForFactoryIndexes(Grv_Data.Rows[i].Cells[0].Text);
                        _sb.AppendLine("\t\tprivate int " + columName + ";");
                    }
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t#endregion");
                    _sb.AppendLine();

                    #endregion

                    //_sb.AppendLine("\t\tpublic void InitializeMapper(IDataRecord dataRecord)");
                    //_sb.AppendLine("\t\t{");
                    //_sb.AppendLine("\t\t\tBuildIndex(dataRecord);");
                    //_sb.AppendLine("\t\t\t_isInitialized = true;");
                    //_sb.AppendLine("\t\t\t_values = new object[dataRecord.FieldCount];");
                    //_sb.AppendLine("\t\t}");
                    //_sb.AppendLine();

                    #region Construir Indices

                    _sb.AppendLine("\t\t#region Construir Indices");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\tprotected override void BuildIndex(IDataRecord dataRecord)");
                    _sb.AppendLine("\t\t{");

                    for (var i = 0; i < Grv_Data.Rows.Count; i++)
                    {
                        var columName = Grv_Data.Rows[i].Cells[0].Text;
                        _sb.AppendLine("\t\t\t" + Helper.GetColumnNameRubyStyleForFactoryIndexes(columName) + " = dataRecord.GetOrdinal(\"" + columName + "\");");
                    }

                    _sb.AppendLine("\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t#endregion");
                    _sb.AppendLine();

                    #endregion

                    #region Funcion Build

                    _sb.AppendLine("\t\tinternal override object Build(IDataRecord dataRecord)");
                    _sb.AppendLine("\t\t{");

                    _sb.AppendLine("\t\t\tif (!_isInitialized) { InitializeMapper(dataRecord); }");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\tvar " + Helper.First(entityName) + " = new " + entityName + "();");
                    _sb.AppendLine("\t\t\tdataRecord.GetValues(_values);");

                    _sb.AppendLine();

                    for (var i = 0; i < Grv_Data.Rows.Count; i++)
                    {
                        var columName = Grv_Data.Rows[i].Cells[0].Text;
                        var dataType = Grv_Data.Rows[i].Cells[4].Text;
                        var isNullable = Grv_Data.Rows[i].Cells[3].Text;
                        //MAX
                        var maxLenght = Grv_Data.Rows[i].Cells[5].Text;

                        if (dataType != "uniqueidentifier")
                        {
                            if (isNullable == "YES")
                            {
                                if (dataType == "decimal" || dataType == "int" || dataType == "bigint")
                                {
                                    _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." + Helper.GetColumnNameRubyStyleForEntity(columName) + " = GetValue<" + Helper.GetVarType(dataType).Trim() + "?>(_values[" + Helper.GetColumnNameRubyStyleForFactoryIndexes(columName) + "]);");
                                }
                                else if (dataType == "tinyint")
                                {
                                    _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." + Helper.GetColumnNameRubyStyleForEntity(columName) + " = GetValue<bool>(_values[" + Helper.GetColumnNameRubyStyleForFactoryIndexes(columName) + "]);");
                                }
                                else if (maxLenght == "36" && dataType == "char")
                                {
                                    _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." + Helper.GetColumnNameRubyStyleForEntity(columName) + " = GetValue<Guid?>(_values[" + Helper.GetColumnNameRubyStyleForFactoryIndexes(columName) + "]);");
                                }
                                else
                                {
                                    _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." +
                                                   Helper.GetColumnNameRubyStyleForEntity(columName) + " = GetValue<" +
                                                   Helper.GetVarType(dataType).Trim() + ">(_values[" +
                                                   Helper.GetColumnNameRubyStyleForFactoryIndexes(columName) + "]);");
                                }
                            }
                            else if (dataType == "tinyint")
                            {
                                _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." + Helper.GetColumnNameRubyStyleForEntity(columName) + " = GetValue<bool>(_values[" + Helper.GetColumnNameRubyStyleForFactoryIndexes(columName) + "]);");
                            }
                            else if (maxLenght == "36" && dataType == "char")
                            {
                                _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." + Helper.GetColumnNameRubyStyleForEntity(columName) + " = GetValue<Guid>(_values[" + Helper.GetColumnNameRubyStyleForFactoryIndexes(columName) + "]);");
                            }
                            else
                            {
                                _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." +
                                               Helper.GetColumnNameRubyStyleForEntity(columName) + " = (" +
                                               Helper.GetVarType(dataType).Trim() + ")_values[" +
                                               Helper.GetColumnNameRubyStyleForFactoryIndexes(columName) + "];");
                            }
                        }
                        else
                        {
                            _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." + Helper.GetColumnNameRubyStyleForEntity(columName) + " = Guid.Parse(_values[" + Helper.GetColumnNameRubyStyleForFactoryIndexes(columName) + "].ToString());");
                        }
                    }

                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\treturn " + Helper.First(entityName) + ";");
                    _sb.AppendLine("\t\t}");
                    _sb.AppendLine();

                    #endregion

                    _sb.AppendLine("\t}");
                    _sb.AppendLine("}");

                    _sb.Replace("'", Quote);

                    Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());


                    #region Crea el archivo

                    //StreamWriter sw = new StreamWriter(Server.MapPath("Class") + "\\" + _entityName + ".cs", false, Encoding.ASCII);
                    //sw.Write(taResults.Value);
                    //sw.Close();

                    #endregion
                }
                else
                {
                    Ltr_Message.Text = "<div class='alert alert-danger'>Ingresa un namespace</div>";
                }
            }
            catch (Exception ex)
            {
                Ltr_Message.Text = "<div class='alert alert-danger'>" + ex.Message + "</div>";
            }
        }

        protected void Lnb_Repository_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Txb_Namespace.Text))
                {
                    var entityName = Ddl_Table.SelectedItem.Text;
                    var entityNameOriginal = Ddl_Table.SelectedItem.Text;
                    entityName = Helper.GetColumnNameRubyStyleForEntity(entityName);

                    _sb.AppendLine();
                    _sb.AppendLine("using System;");
                    _sb.AppendLine("using System.Collections.Generic;");
                    _sb.AppendLine("using MySql.Data.MySqlClient;");
                    _sb.AppendLine("using AppBiz._Generic;");
                    _sb.AppendLine("using System.Threading.Tasks;");
                    _sb.AppendLine();

                    _sb.AppendLine("namespace " + Txb_Namespace.Text);
                    _sb.AppendLine("{");
                    _sb.AppendLine("\tpublic sealed class " + entityName + "Repository : Repository<" + entityName + "," + entityName + "Factory>");
                    _sb.AppendLine("\t{  ");

                    _sb.AppendLine();

                    _sb.AppendLine("\t\t#region Constructor");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\tpublic " + entityName + "Repository(int companyKey, string companyTenant): base(companyKey, companyTenant) { }");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t#endregion");
                    _sb.AppendLine();

                    #region CREATE

                    _sb.AppendLine("\t\tpublic async Task<int> InsertAsync(" + entityName + " " + Helper.First(entityName) + ")");
                    _sb.AppendLine("\t\t{");
                    _sb.AppendLine("\t\t\tusing(var command = new MySqlCommand(\"" + entityNameOriginal + "_insert\"))");
                    _sb.AppendLine("\t\t\t{");
                    _sb.AppendLine();

                    var columsNotAllowedCreate = new StringBuilder();
                    columsNotAllowedCreate.Append(entityNameOriginal + "_updated_by,");
                    columsNotAllowedCreate.Append(entityNameOriginal + "_updated_by_tag,");
                    columsNotAllowedCreate.Append(entityNameOriginal + "_updated_at,");
                    columsNotAllowedCreate.Append(entityNameOriginal + "_created_at");

                    for (var i = 0; i < Grv_Data.Rows.Count; i++)
                    {
                        //COLUMN_NAME
                        var columName = Grv_Data.Rows[i].Cells[0].Text;

                        if (!columsNotAllowedCreate.ToString().Contains(columName))
                        {
                            if (columName == "company_key")
                                _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"_company_key\", CompanyKey);");
                            else
                            {
                                _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"_" + columName + "\", " + Helper.First(entityName) + "." + Helper.GetColumnNameRubyStyleForEntity(columName) + ");");

                            }
                        }
                    }
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\treturn await MyExecuteNonQueryAsync(command);");
                    //_sb.AppendLine("\t\t\tCacheClear();");
                    _sb.AppendLine("\t\t\t}");//End create
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t}");//End create
                    _sb.AppendLine();

                    #endregion

                    #region DELETE

                    _sb.AppendLine("\t\tpublic async Task<int> DeleteAsync(Guid id)");
                    _sb.AppendLine("\t\t{");
                    _sb.AppendLine("\t\t\tusing(var command = new MySqlCommand(\"" + entityNameOriginal + "_delete\"))");
                    _sb.AppendLine("\t\t\t{");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"_id\", id);");
                    _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"_company_key\", CompanyKey);");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\treturn await MyExecuteNonQueryAsync(command);");
                    //_sb.AppendLine("\t\t\tCacheClear();");
                    _sb.AppendLine("\t\t\t}");//End delete
                    _sb.AppendLine("\t\t}");//End delete
                    _sb.AppendLine();

                    #endregion

                    #region DELETE

                    //_sb.AppendLine("\t\tpublic bool Delete(Guid id)");
                    //_sb.AppendLine("\t\t{");

                    //_sb.AppendLine("\t\t\tvar command = new MySqlCommand(\"" + entityNameOriginal + "_delete\");");
                    //_sb.AppendLine();

                    //_sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"_id\", id);");
                    //_sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"_company_key\", _CompanyKey);");
                    //_sb.AppendLine();
                    //_sb.AppendLine("\t\t\tbool confirm = Convert.ToBoolean(MyExecuteScalar(command));");
                    //_sb.AppendLine();
                    //_sb.AppendLine("\t\t\tif (confirm)");
                    //_sb.AppendLine("\t\t\t{");
                    //_sb.AppendLine("\t\t\t\tCacheClear();");
                    //_sb.AppendLine("\t\t\t}");
                    //_sb.AppendLine();
                    //_sb.AppendLine("\t\t\treturn confirm;");

                    //_sb.AppendLine("\t\t}");//End delete
                    //_sb.AppendLine();

                    #endregion

                    #region UPDATE

                    _sb.AppendLine("\t\tpublic async Task<int> UpdateAsync(" + entityName + " " + Helper.First(entityName) + ")");
                    _sb.AppendLine("\t\t{");

                    _sb.AppendLine("\t\t\tusing(var command = new MySqlCommand(\"" + entityNameOriginal + "_update\"))");
                    _sb.AppendLine("\t\t\t{");
                    _sb.AppendLine();

                    var columsNotAllowedUpdate = new StringBuilder();
                    columsNotAllowedUpdate.Append(entityNameOriginal + "_created_by,");
                    columsNotAllowedUpdate.Append(entityNameOriginal + "_created_at,");
                    columsNotAllowedUpdate.Append(entityNameOriginal + "_created_by_tag,");
                    columsNotAllowedUpdate.Append(entityNameOriginal + "_updated_at,");
                    columsNotAllowedUpdate.Append("company_key");

                    for (var i = 0; i < Grv_Data.Rows.Count; i++)
                    {
                        //COLUMN_NAME
                        var columName = Grv_Data.Rows[i].Cells[0].Text;
                        //DATA_TYPE
                        var dataType = Grv_Data.Rows[i].Cells[4].Text;

                        if (!columsNotAllowedUpdate.ToString().Contains(columName))
                        {
                            _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"_" + columName + "\", " + Helper.First(entityName) + "." + Helper.GetColumnNameRubyStyleForEntity(columName) + ");");
                        }
                    }
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\treturn await MyExecuteNonQueryAsync(command);");
                    //_sb.AppendLine("\t\t\tCacheClear();");
                    _sb.AppendLine("\t\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t}");//End create
                    _sb.AppendLine();

                    #endregion

                    //#region GET

                    //_sb.AppendLine("\t\tpublic " + entityName + " Get(Guid id)");
                    //_sb.AppendLine("\t\t{");

                    //_sb.AppendLine("\t\t\treturn GetByCompany().Find(i => i.Id == id);");
                    //_sb.AppendLine("\t\t}");
                    //_sb.AppendLine();

                    //#endregion

                    #region GetByCompany

                    _sb.AppendLine("\t\tpublic async Task<List<" + entityName + ">> GetByCompanyAsync()");
                    _sb.AppendLine("\t\t{");
                    _sb.AppendLine("\t\t\tvar command = new MySqlCommand(\"" + entityNameOriginal + "_get_by_company\");");
                    _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"_company_key\", CompanyKey);");
                    _sb.AppendLine("\t\t\treturn await BuildManyAsync(command);");
                    _sb.AppendLine("\t\t}");
                    _sb.AppendLine();

                    #endregion

                    #region GETALL

                    _sb.AppendLine("\t\tpublic async Task<" + entityName + "> GetAsync(Guid id)");
                    _sb.AppendLine("\t\t{");
                    _sb.AppendLine("\t\t\tvar command = new MySqlCommand(\"" + entityNameOriginal + "_get\");");
                    _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"_company_key\", CompanyKey);");
                    _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"_id\", id);");
                    _sb.AppendLine("\t\t\treturn await BuildOneAsync(command);");
                    _sb.AppendLine("\t\t}");
                    _sb.AppendLine();

                    #endregion

                    _sb.AppendLine("\t}");//End class
                    _sb.AppendLine("}");//End namespace

                    _sb.Replace("'", Quote); // Replace ['] character with ["] character

                    Ltr_Code.Text = System.Web.HttpUtility.HtmlEncode(_sb.ToString());

                    #region Crea el archivo

                    //StreamWriter sw = new StreamWriter(Server.MapPath("Class") + "\\" + _entityName + ".cs", false, Encoding.ASCII);
                    //sw.Write(taResults.Value);
                    //sw.Close();

                    #endregion
                }
                else
                {
                    Ltr_Message.Text = "<div class='alert alert-danger'>Ingresa un namespace</div>";
                }
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



