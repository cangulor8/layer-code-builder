using System;

namespace AppWeb.App_Biz
{
    public static class Helper
    {
	    public static string GetVarType(string dataType)
	    {
		    switch (dataType)
		    {
			    case "char":
			    case "nchar":
			    case "text":
			    case "ntext":
			    case "varchar":
			    case "nvarchar": return "string";

			    case "varbinary":
			    case "sql_variant":
			    case "image":
			    case "binary": return "variant";
			    case "bit": return "bool";

			    case "datetime":
			    case "smalldatetime":
			    case "timestamp":
			    case "date": return "DateTime";

			    case "smallmoney":
			    case "decimal":
			    case "money":
			    case "numeric": return "decimal";

			    case "int": return "int";
			    case "bigint": return "long";
			    case "float": return "float";
			    case "double": return "double";
			    case "real": return "float";
			    case "smallint": return "short";
			    case "tinyint": return "byte";
			    case "uniqueidentifier": return "Guid";
		    }

		    return "";
	    }

		public static string ReplaceFirst(string text, string search, string replace)
        {
            var pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string FirstToLower(string columnName)
        {
            return char.ToLowerInvariant(columnName[0]) + columnName.Substring(1);
        }

	    public static string First(string columnName)
	    {
		    return char.ToLowerInvariant(columnName[0]) + columnName.Substring(1);
	    }

		public static string FirstToUpper(string columnName)
        {

            return char.ToUpperInvariant(columnName[0]) + columnName.Substring(1);
        }

        public static string GetColumnNameRubyStyleForEntity(string columnName)
        {
            try
            {
                var column = "";

                foreach (var part in columnName.Split('_'))
                {
                    column += char.ToUpperInvariant(part[0]) + part.Substring(1);
                }

                return column;
            }
            catch (Exception)
            {
                throw new Exception("GetColumnNameRubyStyleForEntity " + columnName);
            }
        }

        public static string GetColumnNameRubyStyleForFactoryIndexes(string columnName)
        {
            try
            {
                var column = "";

                foreach (var part in columnName.Split('_'))
                {
                    column += char.ToUpperInvariant(part[0]) + part.Substring(1);
                }

                return "_" + FirstToLower(column);
            }
            catch (Exception ex)
            {
                throw new Exception("GetColumnNameRubyStyleForFactoryIndexes " + columnName);
            }
        }

        public static string GetColumnNameRubyStyleForFactorySpParameter(string columnName)
        {
            var column = "";

            foreach (var part in columnName.Split('_'))
            {
                column += char.ToUpperInvariant(part[0]) + part.Substring(1);
            }

            return "_" + FirstToUpper(column);
        }
    }
}