using System;
using System.Text;
using AppWeb.App_Biz;

namespace AppWeb.SqlServer
{
    public partial class BuildCodeSqlServer : System.Web.UI.Page
    {
	    private readonly StringBuilder _sb = new StringBuilder();

        protected const char QuoteCh = '"';
        protected string Quote = QuoteCh.ToString();

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

        protected void Lnb_Entity_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Txb_Namespace.Text))
                {
                    var nombre = Ddl_Table.SelectedItem.Text;

                    if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text))
                    {
                        nombre = Ddl_Table.SelectedItem.Text.Substring(1, Ddl_Table.SelectedItem.Text.Length - 1);
                        //   nombre = Ddl_Table.SelectedItem.Text.Replace(Txb_Prefix.Text, "");
                    }
                    _sb.AppendLine();
                    //_sb.AppendLine("using System;");
                    //_sb.AppendLine("using System.Collections.Generic;");
                    _sb.AppendLine("using System;");
                    _sb.AppendLine();

                    _sb.AppendLine("namespace " + Txb_Namespace.Text.Trim());
                    _sb.AppendLine("{");
                    _sb.AppendLine("\tpublic sealed partial class " + nombre);
                    _sb.AppendLine("\t{");

                    for (var i = 0; i < Grv_Data.Rows.Count; i++)
                    {
                        //COLUMN_NAME
                        var columName = Grv_Data.Rows[i].Cells[0].Text;
                        //DATA_TYPE
                        var dataType = Grv_Data.Rows[i].Cells[4].Text;

                        //IS_NULLABLE
                        var isNullable = Grv_Data.Rows[i].Cells[3].Text;

                        if ((dataType == "bigint" || dataType == "int" || dataType == "decimal") && isNullable == "YES")
                            _sb.AppendLine("\t\tpublic " + Helper.GetVarType(dataType) + "? " + columName + " { get; set; }");
                        else
                            _sb.AppendLine("\t\tpublic " + Helper.GetVarType(dataType) + " " + columName + " { get; set; }");
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
                    var entityName = Ddl_Table.SelectedItem.Text;

                    #region Nombre de la entidad

                    if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text))
                        entityName = Helper.ReplaceFirst(entityName, Txb_Prefix.Text, "");

                    #endregion

                    _sb.AppendLine();
                    _sb.AppendLine("using System;");
                    _sb.AppendLine("using System.Data;");
                    _sb.AppendLine();
                    _sb.AppendLine("namespace " + Txb_Namespace.Text);
                    _sb.AppendLine("{");
                    _sb.AppendLine("\tinternal class " + entityName + "Factory  : Reader, IDataMapper");
                    _sb.AppendLine("\t{  ");
                    _sb.AppendLine();

                    #region Indices

                    _sb.AppendLine("\t\t#region Indices");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\tprivate bool _isInitialized = false;");
                    for (var i = 0; i < Grv_Data.Rows.Count; i++)
                    {
                        var columName = Grv_Data.Rows[i].Cells[0].Text;
                        var dataType = Grv_Data.Rows[i].Cells[4].Text;
                        _sb.AppendLine("\t\tint " + columName + ";");
                    }
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t#endregion");
                    _sb.AppendLine();

                    #endregion

                    _sb.AppendLine("\t\tprivate void InitializeMapper(IDataReader dataReader)");
                    _sb.AppendLine("\t\t{");
                    _sb.AppendLine("\t\t\tBuildIndex(dataReader);");
                    _sb.AppendLine("\t\t\t_isInitialized = true;");
                    _sb.AppendLine("\t\t}");
                    _sb.AppendLine();

                    #region Construir Indices

                    _sb.AppendLine("\t\t#region Construir Indices");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\tprivate void BuildIndex(IDataReader dataReader)");
                    _sb.AppendLine("\t\t{");

                    for (var i = 0; i < Grv_Data.Rows.Count; i++)
                    {
                        var columName = Grv_Data.Rows[i].Cells[0].Text;
                        var dataType = Grv_Data.Rows[i].Cells[4].Text;
                        _sb.AppendLine("\t\t\t" + columName + " = dataReader.GetOrdinal(\"" + columName + "\");");
                    }

                    _sb.AppendLine("\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t#endregion");
                    _sb.AppendLine();

                    #endregion

                    #region Funcion Build

                    _sb.AppendLine("\t\tpublic object Build(IDataReader dataReader)");
                    _sb.AppendLine("\t\t{");

                    _sb.AppendLine("\t\t\tif (!_isInitialized) { InitializeMapper(dataReader); }");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\tvar " + Helper.First(entityName) + " = new " + entityName + "();");
                    _sb.AppendLine("\t\t\tvar values = new object[dataReader.FieldCount];");
                    _sb.AppendLine("\t\t\tdataReader.GetValues(values);");

                    _sb.AppendLine();

                    for (var i = 0; i < Grv_Data.Rows.Count; i++)
                    {
                        var columName = Grv_Data.Rows[i].Cells[0].Text;
                        var dataType = Grv_Data.Rows[i].Cells[4].Text;
                        var isNullable = Grv_Data.Rows[i].Cells[3].Text;

                        if (dataType != "uniqueidentifier")
                        {
                            if (isNullable == "YES")
                            {
                                if (dataType == "decimal" || dataType == "int" || dataType == "bigint")
                                {
                                    _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." + columName + " = GetValue<" + Helper.GetVarType(dataType).Trim() + "?>(values[" + columName + "]);");
                                }
                                else
                                {
                                    _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." + columName + " = GetValue<" + Helper.GetVarType(dataType).Trim() + ">(values[" + columName + "]);");
                                }
                            }
                            else
                            {
                                _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." + columName + " = (" + Helper.GetVarType(dataType).Trim() + ")values[" + columName + "];");
                            }
                        }
                        else
                        {
                            _sb.AppendLine("\t\t\t" + Helper.First(entityName) + "." + columName + " = Guid.Parse(values[" + columName.ToLower() + "].ToString());");
                        }
                    }

                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\treturn " + Helper.First(entityName) + ";");
                    _sb.AppendLine("\t\t}");
                    _sb.AppendLine();

                    #endregion

                    #region Funcion BuildOne
                    /*
                    _sb.AppendLine("\t\t#region BuildOneWithoutCache");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\tinternal " + _entityName + " BuildOneWithoutCache(SqlCommand command)");
                    _sb.AppendLine("\t\t{");
                    _sb.AppendLine("\t\t\tvar " + sqlHelper.First(_entityName) + " = new " + _entityName + "();");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\t\t\tusing (var dataReader = MyExecuteReader(command))");
                    _sb.AppendLine("\t\t\t\t\t{");
                    _sb.AppendLine("\t\t\t\t\t\tBuildIndex(dataReader);");
                    _sb.AppendLine();
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\t\t\t\tif (dataReader.Read())");
                    _sb.AppendLine("\t\t\t\t\t\t\t " + sqlHelper.First(_entityName) + "= Build(dataReader);");
                    _sb.AppendLine("\t\t\t\t\t\telse");
                    _sb.AppendLine("\t\t\t\t\t\t\t" + sqlHelper.First(_entityName) + "= null;");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\treturn " + sqlHelper.First(_entityName) + ";");
                    _sb.AppendLine("\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t#endregion");
                    _sb.AppendLine();
                    */
                    #endregion

                    #region Funcion BuildMany
                    /*
                    _sb.AppendLine("\t\t#region BuildManyWithCache");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\tinternal List<" + _entityName + "> BuildManyWithCache(SqlCommand command)");
                    _sb.AppendLine("\t\t{");
                    _sb.AppendLine("\t\t\tif (CacheContent == null)");
                    _sb.AppendLine("\t\t\t{");
                    _sb.AppendLine("\t\t\t\t\tusing (var dataReader = MyExecuteReader(command))");
                    _sb.AppendLine("\t\t\t\t\t{");
                    _sb.AppendLine("\t\t\t\t\t\tBuildIndex(dataReader);");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\t\t\t\tvar list = new List<" + _entityName + ">();");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\t\t\t\twhile (dataReader.Read())");
                    _sb.AppendLine("\t\t\t\t\t\t{");
                    _sb.AppendLine("\t\t\t\t\t\t\tlist.Add(Build(dataReader));");
                    _sb.AppendLine("\t\t\t\t\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\t\t\t\tCacheAdd(list);");
                    _sb.AppendLine("\t\t\t\t}");
                    _sb.AppendLine("\t\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\treturn CacheContent;");
                    _sb.AppendLine("\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t#endregion");
                    */
                    #endregion

                    #region Funcion BuildManyWithoutcache
                    /*
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t#region BuildManyWithoutCache");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\tinternal List<" + _entityName + "> BuildManyWithoutCache(SqlCommand command)");
                    _sb.AppendLine("\t\t{");
                    _sb.AppendLine("\t\t\t\tusing (var dataReader = MyExecuteReader(command))");
                    _sb.AppendLine("\t\t\t\t{");
                    _sb.AppendLine("\t\t\t\t\tBuildIndex(dataReader);");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\t\t\tvar list = new List<" + _entityName + ">();");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\t\t\twhile (dataReader.Read())");
                    _sb.AppendLine("\t\t\t\t\t{");
                    _sb.AppendLine("\t\t\t\t\t\tlist.Add(Build(dataReader));");
                    _sb.AppendLine("\t\t\t\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t\t\t\treturn list;");
                    _sb.AppendLine("\t\t\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t}");
                    _sb.AppendLine();
                    _sb.AppendLine("\t\t#endregion");
                    */
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
                    if (Ddl_Table.SelectedValue != "0")
                    {

                        var entityName = Ddl_Table.SelectedItem.Text;

                        if (!string.IsNullOrWhiteSpace(Txb_Prefix.Text))
                        {
                            entityName = Helper.ReplaceFirst(entityName, Txb_Prefix.Text.Trim(), "");
                        }

                        _sb.AppendLine();
                        _sb.AppendLine("using System;");
                        _sb.AppendLine("using System.Collections.Generic;");
                        _sb.AppendLine("using System.Data;");
                        _sb.AppendLine("using System.Data.SqlClient;");
                        _sb.AppendLine("using System.Threading.Tasks;");
                        _sb.AppendLine();

                        _sb.AppendLine("namespace " + Txb_Namespace.Text);
                        _sb.AppendLine("{");
                        _sb.AppendLine("\tpublic sealed partial class " + entityName + "Repository : RepositorySqlServer<" + entityName + ">");
                        _sb.AppendLine("\t{  ");

                        _sb.AppendLine();

                        //if (Chb_MultiTenant.Checked)
                        //{
                        _sb.AppendLine("\t\t#region Constructor");
                        _sb.AppendLine();
                        _sb.AppendLine("\t\tpublic " + entityName + "Repository(int companyKey): base(companyKey) { }");
                        //_sb.AppendLine("\t\t{");
                        //_sb.AppendLine("\t\t\t_CompanyKey = companyKey;");
                        //_sb.AppendLine("\t\t}");
                        _sb.AppendLine();
                        _sb.AppendLine("\t\t#endregion");
                        _sb.AppendLine();
                        //}

                        #region CREATE

                        _sb.AppendLine("\t\tpublic void Add(" + entityName + " " + Helper.First(entityName) + ")");
                        //_sb.AppendLine("\t\tpublic void _Create(" + _entityName + " " + sqlHelper.First(_entityName) + ")");
                        _sb.AppendLine("\t\t{");

                        _sb.AppendLine("\t\t\tvar command = new SqlCommand(\"" + entityName + "_Add\");");
                        _sb.AppendLine();

                        var columsNotAllowedCreate = new StringBuilder();
                        columsNotAllowedCreate.Append(entityName + "UpdateUser,");
                        columsNotAllowedCreate.Append(entityName + "Update,");
                        columsNotAllowedCreate.Append(entityName + "Updated,");
                        columsNotAllowedCreate.Append(entityName + "UpdatedBy,");
                        columsNotAllowedCreate.Append(entityName + "Key");

                        for (var i = 0; i < Grv_Data.Rows.Count; i++)
                        {
                            //COLUMN_NAME
                            var columName = Grv_Data.Rows[i].Cells[0].Text;
                            //DATA_TYPE
                            var dataType = Grv_Data.Rows[i].Cells[4].Text;

                            if (!columsNotAllowedCreate.ToString().Contains(columName))
                            {
                                _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"" + columName + "\", " + Helper.First(entityName) + "." + columName + ");");
                            }
                        }
                        _sb.AppendLine();
                        _sb.AppendLine("\t\t\tMyExecuteNonQuery(command);");
                        _sb.AppendLine("\t\t\tCacheClear();");
                        _sb.AppendLine();
                        _sb.AppendLine("\t\t}");//End create
                        _sb.AppendLine();

                        #endregion

                        #region DELETE

                        //_sb.AppendLine("\t\tpublic void _Delete(int " + sqlHelper.First(_entityName) + "Key)");
                        _sb.AppendLine("\t\tpublic void Delete(int " + Helper.First(entityName) + "Key)");
                        _sb.AppendLine("\t\t{");

                        _sb.AppendLine("\t\t\tvar command = new SqlCommand(\"" + entityName + "_Delete\");");
                        _sb.AppendLine();

                        _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"" + entityName + "Key\", " + Helper.First(entityName) + "Key);");
                        _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"CompanyKey\", _CompanyKey);");
                        _sb.AppendLine();
                        _sb.AppendLine("\t\t\tMyExecuteNonQuery(command);");
                        _sb.AppendLine("\t\t\tCacheClear();");
                        _sb.AppendLine("\t\t}");//End delete
                        _sb.AppendLine();

                        #endregion

                        #region DELETE

                        _sb.AppendLine("\t\tpublic bool Delete(int " + Helper.First(entityName) + "Key)");
                        //_sb.AppendLine("\t\tpublic bool _Delete(int " + sqlHelper.First(_entityName) + "Key)");
                        _sb.AppendLine("\t\t{");

                        _sb.AppendLine("\t\t\tvar command = new SqlCommand(\"" + entityName + "_Delete\");");
                        _sb.AppendLine();

                        _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"" + entityName + "Key\", " + Helper.First(entityName) + "Key);");
                        _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"CompanyKey\", _CompanyKey);");
                        _sb.AppendLine();
                        _sb.AppendLine("\t\t\tbool confirm = Convert.ToBoolean(MyExecuteScalar(command));");
                        _sb.AppendLine();
                        _sb.AppendLine("\t\t\tif (confirm)");
                        _sb.AppendLine("\t\t\t{");
                        _sb.AppendLine("\t\t\t\tCacheClear();");
                        _sb.AppendLine("\t\t\t}");
                        _sb.AppendLine();
                        _sb.AppendLine("\t\t\treturn confirm;");

                        _sb.AppendLine("\t\t}");//End delete
                        _sb.AppendLine();

                        #endregion

                        #region EDIT

                        _sb.AppendLine("\t\tpublic void Update(" + entityName + " " + Helper.First(entityName) + ")");
                        //_sb.AppendLine("\t\tpublic void _Edit(" + _entityName + " " + sqlHelper.First(_entityName) + ")");
                        _sb.AppendLine("\t\t{");

                        _sb.AppendLine("\t\t\tvar command = new SqlCommand(\"" + entityName + "_Update\");");
                        _sb.AppendLine();

                        var columsNotAllowedUpdate = new StringBuilder();
                        columsNotAllowedUpdate.Append(entityName + "CreateUser,");
                        columsNotAllowedUpdate.Append(entityName + "Create");
                        columsNotAllowedUpdate.Append(entityName + "Created,");
                        columsNotAllowedUpdate.Append(entityName + "CreatedBy");

                        for (var i = 0; i < Grv_Data.Rows.Count; i++)
                        {
                            //COLUMN_NAME
                            var columName = Grv_Data.Rows[i].Cells[0].Text;
                            //DATA_TYPE
                            var dataType = Grv_Data.Rows[i].Cells[4].Text;

                            if (!columsNotAllowedUpdate.ToString().Contains(columName))
                            {
                                _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"" + columName + "\", " + Helper.First(entityName) + "." + columName + ");");
                            }
                        }
                        _sb.AppendLine();
                        _sb.AppendLine("\t\t\tMyExecuteNonQuery(command);");
                        _sb.AppendLine("\t\t\tCacheClear();");
                        _sb.AppendLine();
                        _sb.AppendLine("\t\t}");//End create
                        _sb.AppendLine();

                        #endregion

                        #region GET

                        //_sb.AppendLine("\t\tpublic " + _entityName + " _Get(int " + sqlHelper.First(_entityName) + "Key)");
                        //_sb.AppendLine("\t\t{");

                        //_sb.AppendLine("\t\t\tusing (var connection = new SqlConnection(ConnectionString))");
                        //_sb.AppendLine("\t\t\t{");
                        //_sb.AppendLine();

                        //_sb.AppendLine("\t\t\t\tvar command = new SqlCommand(\"" + _entityName + "_Read\", connection);");
                        //_sb.AppendLine();

                        //_sb.AppendLine("\t\t\t\tcommand.Parameters.AddWithValue(\"" + _entityName + "Key\", " + sqlHelper.First(_entityName) + "Key);");
                        //_sb.AppendLine("\t\t\t\tcommand.Parameters.AddWithValue(\"CompanyKey\", companyKey);");
                        //_sb.AppendLine();
                        //_sb.AppendLine("\t\t\t   return _Factory.BuildOne(MyExecuteReader(command, System.Data.CommandBehavior.SingleResult));");
                        //_sb.AppendLine("\t\t\t}");
                        //_sb.AppendLine("\t\t}");
                        //_sb.AppendLine();

                        _sb.AppendLine("\t\tpublic " + entityName + " Get(int " + Helper.First(entityName) + "Key)");
                        //_sb.AppendLine("\t\tpublic " + _entityName + " _Get(int " + sqlHelper.First(_entityName) + "Key)");
                        _sb.AppendLine("\t\t{");
                        _sb.AppendLine("\t\t\treturn GetAllCompany().Find(i => i." + entityName + "Key ==" + Helper.First(entityName) + "Key);");
                        _sb.AppendLine("\t\t}");
                        _sb.AppendLine();

                        #endregion

                        #region GETALLCompany

                        //_sb.AppendLine("\t\tpublic List<" + _entityName + "> _GetAll()");
                        _sb.AppendLine("\t\tpublic List<" + entityName + "> GetAllCompany()");
                        _sb.AppendLine("\t\t{");
                        //_sb.AppendLine("\t\t\tget");
                        //_sb.AppendLine("\t\t\t{");
                        _sb.AppendLine("\t\t\tvar command = new SqlCommand(\"" + entityName + "_GetAllCompany\");");
                        _sb.AppendLine("\t\t\tcommand.Parameters.AddWithValue(\"CompanyKey\", _CompanyKey);");
                        _sb.AppendLine("\t\t\treturn GetListWithCache(command);");
                        //_sb.AppendLine("\t\t\t}");
                        _sb.AppendLine("\t\t}");
                        _sb.AppendLine();

                        #endregion

                        #region GETALL

                        //_sb.AppendLine("\t\tpublic List<" + _entityName + "> _GetAll()");
                        _sb.AppendLine("\t\tpublic List<" + entityName + "> GetAll");
                        _sb.AppendLine("\t\t{");
                        _sb.AppendLine("\t\t\tget");
                        _sb.AppendLine("\t\t\t{");
                        _sb.AppendLine("\t\t\t\tvar command = new SqlCommand(\"" + entityName + "_GetAll\");");
                        _sb.AppendLine("\t\t\t\treturn GetListWithCache(command);");
                        _sb.AppendLine("\t\t\t}");
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
                        Ltr_Message.Text = "<div class='alert alert-danger'>Selecciona una tabla</div>";
                    }
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
            RepositorySqlServer.GetDbTables(Ddl_Table, Ddl_Db.SelectedValue);
        }

    }
}



