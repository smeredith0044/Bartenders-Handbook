using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

public class Cocktail
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<string> Ingredients { get; set; }
    public string FlavorProfile { get; set; }

    public static List<Cocktail> GetAllCocktails()
    {
        List<Cocktail> cocktails = new List<Cocktail>();

        try
        {
            // Connect to the PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;User Id=sydney;" +
            "Password=Elliesue2*;Database=bartenders_handbook;");
            conn.Open();

            // Define a SELECT statement to retrieve all cocktail records
            string sql = "SELECT * FROM ”cocktails” ORDER BY name";

            // Execute the SELECT statement and obtain a result set
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader dr = command.ExecuteReader();

            // Iterate over the result set, creating a Cocktail object for each row
            while (dr.Read())
            {
                Cocktail cocktail = new Cocktail();
                cocktail.Id = dr.GetInt32(0);
                cocktail.Name = dr.GetString(1);
                cocktail.Ingredients = new List<string>((string[])dr.GetValue(2));
                cocktail.FlavorProfile = dr.GetString(3);
                cocktails.Add(cocktail);
            }

            conn.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }

        return cocktails;
    }

    public static void AddCocktail(Cocktail cocktail)
    {
        try
        {
            // Connect to the PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;User Id=sydney;" +
            "Password=Elliesue2*;Database=bartenders_handbook;");
            conn.Open();

            // Define an INSERT statement to add the new cocktail record
            string sql = "INSERT INTO ”cocktails” (name, ingredients, flavor_profile) " +
                "VALUES (@name, @ingredients, @flavor_profile)";

            // Execute the INSERT statement, passing in the cocktail object's properties as parameters
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            command.Parameters.AddWithValue("@name", cocktail.Name);
            command.Parameters.AddWithValue("@ingredients", cocktail.Ingredients.ToArray());
            command.Parameters.AddWithValue("@flavor_profile", cocktail.FlavorProfile);
            command.ExecuteNonQuery();

            conn.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    public static void UpdateCocktail(Cocktail cocktail)
    {
        try
        {
            // Connect to the PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;User Id=sydney;" +
            "Password=Elliesue2*;Database=bartenders_handbook;");
            conn.Open();

            // Define an UPDATE statement to modify the existing cocktail record
            string sql = "UPDATE ”cocktails” SET name = @name, ingredients = @ingredients, " +
                "flavor_profile = @flavor_profile WHERE id = @id";

            // Execute the UPDATE statement, passing in the cocktail object's properties as parameters
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            command.Parameters.AddWithValue("@id", cocktail.Id);
            command.Parameters.AddWithValue("@name", cocktail.Name);
            command.Parameters.AddWithValue("@ingredients", cocktail.Ingredients.ToArray());
            command.Parameters.AddWithValue("@flavor_profile", cocktail.FlavorProfile);
            command.ExecuteNonQuery();

            conn.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}