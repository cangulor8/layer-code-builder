using System.Configuration;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace AppWeb.App_Biz
{
    public class RepositoryMySql
    {
        public static void GetDbTables(DropDownList ddl, string connectionString)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = new StringBuilder();
                query.Append("select TABLE_NAME from information_schema.TABLES WHERE TABLE_SCHEMA='"+connection.Database+"' order by TABLE_NAME");

                var dataAdapter = new MySqlDataAdapter(query.ToString(), connection);
                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                ddl.DataSource = dataSet.Tables[0];

                ddl.DataTextField = "TABLE_NAME";
                ddl.DataValueField = "TABLE_NAME";
                ddl.DataBind();

                ddl.Items.Insert(0, new ListItem("[Table]", "0"));
            }
        }

        public static void GetDbColumns(GridView grv, string tableName, string connectionString)
        {
            
            using (var connection = new MySqlConnection(connectionString))
            {
                var query = new StringBuilder();
                query.Append("select ");
                query.Append("COLUMN_NAME,");
                query.Append("ORDINAL_POSITION,");
                query.Append("COLUMN_DEFAULT,");
                query.Append("IS_NULLABLE,");
                query.Append("DATA_TYPE,");
                query.Append("CHARACTER_MAXIMUM_LENGTH,");
                query.Append("COLUMN_COMMENT AS DESCRIPTION");
                query.Append(" FROM information_schema.columns where TABLE_NAME = '" + tableName + "' AND TABLE_SCHEMA ='" + connection.Database + "'");

                var dataAdapter = new MySqlDataAdapter(query.ToString(), connection);
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
