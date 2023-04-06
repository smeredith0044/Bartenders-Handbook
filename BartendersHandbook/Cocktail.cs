using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

public class Cocktail
{
    public Cocktail(int id, string name, string[] ingredientsArr, string flavorProfile)
    {
        Id = id;
        Name = name;
        Ingredients = new List<string>(ingredientsArr);
        FlavorProfile = flavorProfile;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public List<string> Ingredients { get; set; }
    public string FlavorProfile { get; set; }

    public static List<Cocktail> GetAllCocktails()
    {
        List<Cocktail> cocktails = new List<Cocktail>();

        try
        {
            string connString = "Server=ec2-34-197-91-131.compute-1.amazonaws.com;Port=5432;Database=ddab6aknfp5lq5;User Id=ltkkuxigptlbec;Password=f7d1b5f66e29dd77cb68cfb31740424d36a9127edd5c5fdfe49ea14e12ad23b1;SSL Mode=Require;Trust Server Certificate=true;";

            NpgsqlConnection conn = new NpgsqlConnection(connString);
            conn.Open();

            string sql = "SELECT * FROM cocktails ORDER BY name";

            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader dr = command.ExecuteReader();

            while (dr.Read())
            {
                int id = dr.GetInt32(0);
                string name = dr.GetString(1);
                string[] ingredients = (string[])dr.GetValue(2);
                string flavorProfile = dr.GetString(3);

                cocktails.Add(new Cocktail(id, name, ingredients, flavorProfile));
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
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;User Id=sydney;" +
            "Password=Elliesue2*;Database=bartenders_handbook;");
            conn.Open();

            string sql = "INSERT INTO cocktails (name, ingredients, flavorprofile) " +
                "VALUES (@name, @ingredients, @flavorprofile)";

            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            command.Parameters.AddWithValue("@name", cocktail.Name);
            command.Parameters.AddWithValue("@ingredients", cocktail.Ingredients.ToArray());
            command.Parameters.AddWithValue("@flavorprofile", cocktail.FlavorProfile);
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
            string connString = "Server=ec2-34-197-91-131.compute-1.amazonaws.com;Port=5432;Database=ddab6aknfp5lq5;User Id=ltkkuxigptlbec;Password=f7d1b5f66e29dd77cb68cfb31740424d36a9127edd5c5fdfe49ea14e12ad23b1;SSL Mode=Require;Trust Server Certificate=true;";

            NpgsqlConnection conn = new NpgsqlConnection(connString);
            conn.Open();
            string sql = "UPDATE cocktails SET name = @name, ingredients = @ingredients, flavorprofile = @flavorprofile WHERE id = @id";


            // Execute the UPDATE statement, passing in the cocktail object's properties as parameters
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            command.Parameters.AddWithValue("@id", cocktail.Id);
            command.Parameters.AddWithValue("@name", cocktail.Name);
            command.Parameters.AddWithValue("@ingredients", cocktail.Ingredients.ToArray());
            command.Parameters.AddWithValue("@flavorprofile", cocktail.FlavorProfile);
            command.ExecuteNonQuery();

            conn.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}
