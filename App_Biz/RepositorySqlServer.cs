using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI.WebControls;

namespace AppWeb.App_Biz
{
    public class RepositorySqlServer
    {
        //static string _connectionString = "";

        public static void GetDbTables(DropDownList ddl, string connectionString)
        {
            var query = new StringBuilder();
            query.Append("select name, Id from sysobjects  ");
            query.Append("where type='U' and name <> 'dtproperties' ");
            query.Append("order by name ");

            using (var connection = new SqlConnection(connectionString))
            {
                var dataAdapter = new SqlDataAdapter(query.ToString(), connection);
                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                ddl.DataSource = dataSet.Tables[0];

                ddl.DataTextField = "name";
                ddl.DataValueField = "Id";
                ddl.DataBind();

                ddl.Items.Insert(0, new ListItem("[Table]", "0"));
            }
        }

        public static void GetDbColumns(GridView grv, string tableName, string connectionString)
        {
            var query = new StringBuilder();

            //query.AppendLine("SELECT COLUMN_NAME, ORDINAL_POSITION, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH ");
            //query.AppendLine("FROM     INFORMATION_SCHEMA.COLUMNS ");
            //query.AppendLine("WHERE  (TABLE_NAME = '" + tableName + "')");
            query.Append("SELECT ColumnInfo.COLUMN_NAME, ");
            query.Append("ColumnInfo.ORDINAL_POSITION, ");
            query.Append("ColumnInfo.COLUMN_DEFAULT, ");
            query.Append("ColumnInfo.IS_NULLABLE, ");
            query.Append("ColumnInfo.DATA_TYPE, ");
            query.Append("ColumnInfo.CHARACTER_MAXIMUM_LENGTH, ");
            query.Append("Properties.value AS DESCRIPTION ");
            query.Append("FROM INFORMATION_SCHEMA.COLUMNS ColumnInfo ");
            query.Append("LEFT OUTER JOIN sys.extended_properties Properties ");
            query.Append("ON Properties.major_id = OBJECT_ID(ColumnInfo.TABLE_SCHEMA+'.'+ColumnInfo.TABLE_NAME) ");
            query.Append("AND Properties.minor_id = ColumnInfo.ORDINAL_POSITION ");
            query.Append("AND Properties.name = 'MS_Description' ");
            query.Append("WHERE OBJECTPROPERTY(OBJECT_ID(ColumnInfo.TABLE_SCHEMA+'.'+ColumnInfo.TABLE_NAME), 'IsMSShipped')=0 AND ");
            query.Append("ColumnInfo.TABLE_NAME='" + tableName + "'");

            using (var connection = new SqlConnection(connectionString))
            {
                var dataAdapter = new SqlDataAdapter(query.ToString(), connection);
                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                grv.DataSource = dataSet.Tables[0];
                grv.DataBind();
            }
        }

        public static void GetDb(DropDownList ddl) 
        {
            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings) 
            {
                ddl.Items.Add(new ListItem(item.Name,item.ConnectionString));
            }
        }
    }
}
