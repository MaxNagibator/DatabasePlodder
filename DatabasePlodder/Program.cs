using System.Data.SqlClient;

string connectionString = "Server=localhost;Database=TrashPlodder;User Id=plodder;Password=plodder;";

string createTableQuery = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='heros' and xtype='U')
CREATE TABLE dbo.heros
(
    Id int NOT NULL,
    Name nvarchar(100) NOT NULL,
    Age int NULL,
    CONSTRAINT heros_pk PRIMARY KEY (Id)
);";

var sqlProvider = new SqlProvider(connectionString);
sqlProvider.ExecuteNonQuery(createTableQuery);

string deleteHeroQuery = @"DELETE FROM dbo.heros WHERE Id = @Id";
sqlProvider = new SqlProvider(connectionString);
sqlProvider.AddParameter("Id", 1);
sqlProvider.ExecuteNonQuery(deleteHeroQuery);

var random = new Random();
var age = random.Next(1, 217);
string insertHeroQuery = @"INSERT INTO dbo.heros(Id, Name, Age) VALUES(@Id, @Name, @Age)";
sqlProvider = new SqlProvider(connectionString);
sqlProvider.AddParameter("Id", 1);
sqlProvider.AddParameter("Name", "Бульбазавр");
sqlProvider.AddParameter("Age", age);
sqlProvider.ExecuteNonQuery(insertHeroQuery);

sqlProvider = new SqlProvider(connectionString);
sqlProvider.AddParameter("Id", 2);
sqlProvider.AddParameter("Name", "Чермондер");
sqlProvider.AddParameter("Age", age);
sqlProvider.ExecuteNonQuery(insertHeroQuery);

sqlProvider = new SqlProvider(connectionString);
sqlProvider.AddParameter("Id", 3);
sqlProvider.AddParameter("Name", "Пикачу");
sqlProvider.AddParameter("Age", age);
sqlProvider.ExecuteNonQuery(insertHeroQuery);


string updateHeroQuery = @"UPDATE dbo.heros SET Name = Name + ' обыкновенный' WHERE Id = @Id";
sqlProvider = new SqlProvider(connectionString);
sqlProvider.AddParameter("Id", 1);
sqlProvider.ExecuteNonQuery(updateHeroQuery);

string selectHeroQuery = @"SELECT * FROM dbo.heros WHERE Id < @Id";
sqlProvider = new SqlProvider(connectionString);
sqlProvider.AddParameter("Id", 10);
sqlProvider.ExecuteQuery(selectHeroQuery);
for (var i = 0; i < sqlProvider.Columns.Count; i++)
{
    if (i > 0)
    {
        Console.Write(" | ");
    }
    var value = sqlProvider.Columns[i];
    Console.Write(value.ToString());
}
Console.WriteLine();

for (var i = 0; i < sqlProvider.DataRows.Count; i++)
{
    for (int j = 0; j < sqlProvider.DataRows[i].Values.Count; j++)
    {
        if (j > 0)
        {
            Console.Write(" | ");
        }
        var value = sqlProvider.DataRows[i].Values[j];
        Console.Write(value.ToString());
    }
    Console.WriteLine();
}

public class SqlProvider
{
    private string _connectionString;

    public SqlProvider(string connectionString)
    {
        _connectionString = connectionString;
        _parameters = new Dictionary<string, object>();
    }

    private Dictionary<string, object> _parameters;

    public void AddParameter(string key, object value)
    {
        _parameters.Add(key, value);
    }

    public void ExecuteNonQuery(string query)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            foreach (var parameter in _parameters)
            {
                command.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
            }

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    public void ExecuteQuery(string query)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            foreach (var parameter in _parameters)
            {
                command.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
            }

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Columns.Add(reader.GetName(i));
                }
                while (reader.Read())
                {
                    var row = new DataRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Values.Add(reader[i]);
                    }
                    DataRows.Add(row);
                }
            }
            finally
            {
                // Always call Close when done reading.
                reader.Close();
            }
        }
    }

    public List<DataRow> DataRows { get; set; } = new List<DataRow>();
    public List<string> Columns { get; set; } = new List<string>();

    public class DataRow
    {
        public List<object> Values { get; set; } = new List<object>();
    }

}
