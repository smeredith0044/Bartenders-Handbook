using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

class Program
{
    static void Main(string[] args)
    {
        // Connect to a PostgreSQL database
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;User Id=sydney; " +
            "Password=Elliesue2*;Database=bartenders_handbook;");
        conn.Open();

        // Set the console text color to yellow
        Console.ForegroundColor = ConsoleColor.Yellow;

        // Display the main menu and prompt the user for a choice
        int choice = -1;
        while (choice != 0)
        {
            Console.WriteLine("\nWelcome to the Bartender's Handbook!");
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. Search for a cocktail");
            Console.WriteLine("2. Add a cocktail");
            Console.WriteLine("3. Delete a cocktail");
            Console.WriteLine("0. Exit");

            Console.Write("\nEnter your choice: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out choice))
            {
                Console.WriteLine("Invalid choice. Please enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    // Prompt the user for a search term
                    Console.Write("\nEnter a cocktail name or ingredient: ");
                    string searchTerm = Console.ReadLine().ToLower();

                    // Define a parameterized query with a WHERE clause to filter results based on the search term
                    NpgsqlCommand searchCommand = new NpgsqlCommand("SELECT id, name, ingredients, flavorprofile FROM ”cocktails” WHERE LOWER(name) LIKE '%' || @searchTerm || '%' OR @searchTerm = ANY(ingredients)", conn);
                    searchCommand.Parameters.AddWithValue("@searchTerm", searchTerm);

                    // Execute the query and obtain a result set
                    NpgsqlDataReader dr = searchCommand.ExecuteReader();

                    // Check if any results were returned
                    if (dr.HasRows)
                    {
                        // Set the console text color to green
                        Console.ForegroundColor = ConsoleColor.Green;

                        // Output the column headers
                        Console.WriteLine("\nID\tName\tIngredients\tflavorprofile");

                        // Output the search results
                        while (dr.Read())
                        {
                            int id = BitConverter.ToInt32(((Guid)dr["id"]).ToByteArray(), 0);
                            string name = dr["name"].ToString();
                            string[] ingredientsArr = (string[])dr["ingredients"];
                            string ingredients = string.Join(", ", ingredientsArr);
                            string flavorprofile = dr["flavorprofile"].ToString();
                            Console.WriteLine("{0}\t{1}\t{2}\t{3}", id, name, ingredients, flavorprofile);
                        }

                        Console.ResetColor();
                    }
                    else
                    {
                        // Set the console text color to red
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nNo results found for '{0}'", searchTerm);
                        Console.ResetColor();
                    }

                    dr.Close();
                    break;

                case 2:
                    // Prompt the user for information about the new cocktail
                    Console.Write("\nEnter the name of the new cocktail: ");
                    string newName = Console.ReadLine();
                    Console.Write("Enter a comma-separated list of ingredients: ");
                    string newIngredientsStr = Console.ReadLine();
                    string[] newIngredients = newIngredientsStr.Split(',');
                    Console.Write("Enter the flavor profile: ");
                    string newFlavorProfile = Console.ReadLine();

                    try
                    {
                        // Build a parameterized INSERT statement to add the new cocktail to the database
                        NpgsqlCommand insertCommand = new NpgsqlCommand("INSERT INTO ”cocktails” (id, name, ingredients, flavorprofile) VALUES (@id, @name, @ingredients, @flavorprofile)", conn);
                        insertCommand.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, Guid.NewGuid());
                        insertCommand.Parameters.AddWithValue("@name", newName);
                        insertCommand.Parameters.AddWithValue("@ingredients", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Varchar, newIngredients);
                        insertCommand.Parameters.AddWithValue("@flavorprofile", newFlavorProfile);

                        // Execute the INSERT statement and display the number of rows affected
                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        // Display a message indicating the success or failure of the operation
                        if (rowsAffected > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n{0} cocktail added successfully!", newName);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nFailed to add {0} cocktail.", newName);
                        }
                    }
                    catch (Npgsql.PostgresException ex)
                    {
                        // Handle the case where the cocktail already exists in the database
                        if (ex.SqlState == "23505")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n{0} cocktail already exists in the database.", newName);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nFailed to add {0} cocktail.", newName);
                            Console.WriteLine("Error message: {0}", ex.MessageText);
                        }
                    }
                    finally
                    {
                        Console.ResetColor();
                    }
                    break;


                case 3:
                    // Prompt the user for the name of the cocktail to delete
                    Console.Write("\nEnter the name of the cocktail to delete: ");
                    string deleteName = Console.ReadLine();

                    // Define a parameterized query to delete a row from the "cocktails" table by name
                    NpgsqlCommand deleteCommand = new NpgsqlCommand("DELETE FROM ”cocktails” WHERE name = @name", conn);
                    deleteCommand.Parameters.AddWithValue("@name", deleteName);

                    try
                    {
                        // Execute the query and obtain the number of rows affected
                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        // Display a message indicating the success or failure of the operation
                        if (rowsAffected > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\nCocktail '{0}' deleted successfully!", deleteName);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nFailed to delete cocktail '{0}'. Cocktail does not exist.", deleteName);
                            Console.ResetColor();
                        }
                    }
                    catch (Npgsql.PostgresException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nFailed to delete cocktail '{0}'.", deleteName);
                        Console.WriteLine("Error message: {0}", ex.MessageText);
                        Console.ResetColor();
                    }
                    break;





                case 0:
                    // Exit the program
                    Console.WriteLine("\nThank you for using the Bartender's Handbook!");
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }
        }

        conn.Close();
    }
}