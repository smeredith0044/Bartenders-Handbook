using System;
using System.Data;
using Npgsql;

class Sample
{
    static void Main(string[] args)
    {
        // Connect to a PostgreSQL database
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;User Id=sydney; " +
            "Password=Elliesue2*;Database=bartenders_handbook;");
        conn.Open();

        // Define a query
        NpgsqlCommand command = new NpgsqlCommand("select id, name, array_to_string(ingredients, ',') as ingredients, flavor_profile from ”cocktails”", conn);

        // Execute the query and obtain a result set
        NpgsqlDataReader dr = command.ExecuteReader();

        // Output rows
        while (dr.Read())
            Console.Write("{0}\t{1}\t{2}\t{3} \n", dr[0], dr[1], dr[2], dr[3]); 

        conn.Close();

    }
}