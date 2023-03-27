﻿using System;
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
        NpgsqlCommand command = new NpgsqlCommand("SELECT name, ingredients, flavor_profile FROM ”cocktails”", conn);

        // Execute the query and obtain a result set
        NpgsqlDataReader dr = command.ExecuteReader();

        // Output rows
        while (dr.Read())
            Console.Write("{0}\t{1} \n", dr[0], dr[1]);

        conn.Close();

    }
}