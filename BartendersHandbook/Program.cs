using System;
using System.Data;
using Npgsql;

class sample
{
    static void Main(string[] args)
    {
        try
        {
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;User Id=sydney; " +
            "Password=Elliesue2*;Database=bartenders_handbook;");
            conn.Open();

            // Prompt the user for a search term
            Console.WriteLine("Enter a cocktail name or ingredient:");
            string searchTerm = Console.ReadLine().ToLower();

            // Define a parameterized query with a WHERE clause to filter results based on the search term
            NpgsqlCommand command = new NpgsqlCommand("SELECT id, name, ingredients, flavor_profile FROM ”cocktails” WHERE LOWER(name) LIKE '%' || @searchTerm || '%' OR @searchTerm = ANY(ingredients)", conn);
            command.Parameters.AddWithValue("@searchTerm", searchTerm);

            // Execute the query and obtain a result set
            NpgsqlDataReader dr = command.ExecuteReader();

            // Check if any results were returned
            if (dr.HasRows)
            {
                // Output the column headers
                Console.WriteLine("ID\tName\tIngredients\tFlavor Profile");

                // Output the search results
                while (dr.Read())
                {
                    int id = dr.GetInt32(0);
                    string name = dr.GetString(1);
                    string[] ingredientsArr = (string[])dr.GetValue(2);
                    string ingredients = string.Join(", ", ingredientsArr);
                    string flavorProfile = dr.GetString(3);
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}", id, name, ingredients, flavorProfile);
                }
            }
            else
            {
                Console.WriteLine("No results found for '{0}'", searchTerm);
            }

            conn.Close();
        }
        catch (Exception e)
        {
            //Catch errors and write to error.txt file in the debug folder 
            string fileName = "error.txt";
            string destPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            Console.WriteLine("Oops, an error occurred. Please see " + destPath);
            System.IO.File.WriteAllText(destPath, e.ToString());
        }

        // Wait for user input before closing the console window
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
