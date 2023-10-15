// See https://aka.ms/new-console-template for more information
using System;
using System.Data.Odbc;
using System.Globalization;
using MySqlConnector;

String? lastDate = null;

string? convertDate(string value)
{
    PersianCalendar pc = new PersianCalendar();

    var persianDateSplitedParts = value.Split('/');
    DateTime dateTime = new DateTime(int.Parse(persianDateSplitedParts[0]), int.Parse(persianDateSplitedParts[1]), int.Parse(persianDateSplitedParts[2]), pc);
    try
    {
         lastDate = DateTime.Parse(dateTime.ToString(CultureInfo.CreateSpecificCulture("en-US"))).ToString("yyyy/MM/dd");
    }
    catch (System.Exception)
    {
        Console.WriteLine("time format error" + value);
    }
    return lastDate;
}

Console.WriteLine("Hello, World!");

// https://mysqlconnector.net/tutorials/basic-api/
var connString = "Server=127.0.0.1;User ID=root;Port=3306;Database=letter_app";

await using var mySqlConnection = new MySqlConnection(connString);
await mySqlConnection.OpenAsync();

// Insert some data
// using (var cmd = new MySqlCommand())
// {
//     cmd.Connection = mySqlConnection;
//     cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
//     cmd.Parameters.AddWithValue("p", "Hello world");
//     await cmd.ExecuteNonQueryAsync();
// }

// Retrieve all rows
// using var command = new MySqlCommand("SELECT * FROM letters", connection);
// using var reader = await command.ExecuteReaderAsync();
// while (await reader.ReadAsync())
//     Console.WriteLine(reader.GetString(1));



string ConnectionString = "Driver={Microsoft Access Driver (*.mdb, *.accdb)}; Dbq=D:\\Adnicatorbook1.mdb; Uid = Admin; Pwd =; ";

OdbcCommand command = new OdbcCommand("SELECT * FROM daftare_andicator");

using (OdbcConnection connection = new OdbcConnection(ConnectionString))
{
    command.Connection = connection;
    connection.Open();
    using (var reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = mySqlConnection;
                cmd.CommandText = "INSERT INTO letters (id,subject,type_id,user_id,created_at,updated_at) VALUES (@id,@subject,@type_id,@user_id,@created_at,@updated_at)";
                cmd.Parameters.AddWithValue("id", reader.GetInt32(0));
                cmd.Parameters.AddWithValue("subject", reader.GetString(4));
                cmd.Parameters.AddWithValue("type_id", 1);
                cmd.Parameters.AddWithValue("user_id", 1);

                var date = convertDate(reader.GetString(2));
                // Console.WriteLine(date);
                cmd.Parameters.AddWithValue("created_at", date);
                cmd.Parameters.AddWithValue("updated_at", date);
                cmd.ExecuteNonQuery();
            }
        }
    };
}

Console.ReadKey();