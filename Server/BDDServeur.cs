
using System.Data.SqlClient;
namespace Server
{
    public class BDDServeur
    {
        public static SqlConnection GetDBConnection( )
        {
            string datasource = @"DESKTOP-J55CLP8";
            string database = "Personne";
            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                        + database + ";Integrated Security = True;";
            SqlConnection conn = new SqlConnection(connString);
            return conn;
        }
    }
}
