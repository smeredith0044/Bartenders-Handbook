using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

// Single responsibility principle is followed by separating out the code for database operations
// into a separate class
class DbConnector
{
    private NpgsqlConnection _conn;

    // Constructor initializes a new NpgsqlConnection object
    public DbConnector(string connString)
    {
        _conn = new NpgsqlConnection(connString);
    }

    // Open connection to the database
    public void Open()
    {
        _conn.Open();
    }

    // Close connection to the database
    public void Close()
    {
        _conn.Close();
    }

    // Execute a query and return a NpgsqlDataReader object
    public NpgsqlDataReader ExecuteQuery(string query, Dictionary<string, object> parameters)
    {
        NpgsqlCommand command = new NpgsqlCommand(query, _conn);
        foreach (KeyValuePair<string, object> parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
        return command.ExecuteReader();
    }

    // Execute a non-query and return the number of rows affected
    public int ExecuteNonQuery(string query, Dictionary<string, object> parameters)
    {
        NpgsqlCommand command = new NpgsqlCommand(query, _conn);
        foreach (KeyValuePair<string, object> parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
        return command.ExecuteNonQuery();
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Open-closed principle is followed by using a connection string stored in an environment
        // variable, allowing the code to be easily deployed to different environments without modifying
        // the source code
        string connString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

        // Connect to the database
        DbConnector dbConnector = new DbConnector(connString);
        dbConnector.Open();

        // Set the console text color to yellow
        Console.ForegroundColor = ConsoleColor.Yellow;

        // Prompt the user for a phone number
        Console.Write("\nEnter your phone number: ");
        string phone = Console.ReadLine();

        // Display the main menu and prompt the user for a choice
        int choice = -1;
        while (choice != 0)
        {

            Console.WriteLine("\nWelcome to the Bartender's Handbook!");
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. Search for a cocktail");
            Console.WriteLine("2. Add a cocktail");
            Console.WriteLine("3. Delete a cocktail");
            Console.WriteLine("   ()      ()    /");
            Console.WriteLine("  ()      ()  /  ");
            Console.WriteLine("   ______________/___");
            Console.WriteLine("   \\            /   /");
            Console.WriteLine("    \\^^^^^^^^^^/^^^/");
            Console.WriteLine("     \\     ___/   /");
            Console.WriteLine("      \\   (   )  /");
            Console.WriteLine("       \\  (___) /");
            Console.WriteLine("        \\ /    /");
            Console.WriteLine("         \\    /");
            Console.WriteLine("          \\  /");
            Console.WriteLine("           \\/");
            Console.WriteLine("           ||");
            Console.WriteLine("           ||");
            Console.WriteLine("           ||");
            Console.WriteLine("           ||");
            Console.WriteLine("           ||");
            Console.WriteLine("           /\\");
            Console.WriteLine("          /;;\\");
            Console.WriteLine("======================");
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
                    NpgsqlCommand searchCommand = new NpgsqlCommand("SELECT id, name, ingredients, flavorprofile FROM cocktails WHERE LOWER(name) LIKE '%' || @searchTerm || '%' OR @searchTerm = ANY(ingredients)", conn);
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
    int id = int.Parse(dr["id"].ToString());
    string name = dr["name"].ToString();
    string[] ingredientsArr = (string[])dr["ingredients"];
    string ingredients = string.Join(", ", ingredientsArr);
    string flavorprofile = dr["flavorprofile"].ToString();
    Console.WriteLine("{0,-5} {1,-20} {2,-40} {3}", id, name, ingredients, flavorprofile);
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
                    string[] newIngredientsArr = newIngredientsStr.Split(',');

         
                    // Prompt the user for the flavor profile of the new cocktail
                    Console.Write("Enter the flavor profile of the new cocktail: ");
                    string newFlavorProfile = Console.ReadLine();

                    // Define an INSERT statement with parameter placeholders for the new cocktail
                    NpgsqlCommand insertCommand = new NpgsqlCommand("INSERT INTO cocktails (name, ingredients, flavorprofile) VALUES (@name, @ingredients, @flavorprofile)", conn);
                    insertCommand.Parameters.AddWithValue("@name", newName);
                    insertCommand.Parameters.AddWithValue("@ingredients", newIngredientsArr);
                    insertCommand.Parameters.AddWithValue("@flavorprofile", newFlavorProfile);

                    // Execute the INSERT statement and obtain the number of rows affected
                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    // Check if the operation was successful
                    if (rowsAffected == 1)
                    {
                        Console.WriteLine("\nNew cocktail '{0}' added successfully!", newName);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nFailed to add new cocktail '{0}'.", newName);
                        Console.ResetColor();
                    }

                    break;


                case 3:
                    // Prompt the user for the name of the cocktail to be deleted
                    Console.Write("\nEnter the name of the cocktail to be deleted: ");
                    string deleteName = Console.ReadLine();

                    // Check if the cocktail exists in the database
                    NpgsqlCommand checkCommand = new NpgsqlCommand("SELECT id FROM cocktails WHERE LOWER(name) = LOWER(@name)", conn);
                    checkCommand.Parameters.AddWithValue("@name", deleteName.ToLower());

                    object result = checkCommand.ExecuteScalar();
                    if (result == null)
                    {
                        // The cocktail does not exist in the database, display an error message
                        Console.WriteLine("\nCocktail '{0}' does not exist.", deleteName);
                    }
                    else
                    {
                        // The cocktail exists in the database, execute the DELETE statement
                        NpgsqlCommand deleteCommand = new NpgsqlCommand("DELETE FROM cocktails WHERE LOWER(name) = LOWER(@name)", conn);
                        deleteCommand.Parameters.AddWithValue("@name", deleteName.ToLower());
                        int numRowsDeleted = deleteCommand.ExecuteNonQuery();

                        if (numRowsDeleted > 0)
                        {
                            Console.WriteLine("\nCocktail '{0}' has been deleted.", deleteName);
                        }
                        else
                        {
                            Console.WriteLine("\nFailed to delete cocktail '{0}'.", deleteName);
                        }
                    }
                    break;





                case 0:
                    Console.WriteLine("\nGoodbye!");
                    break;

                default:
                    Console.WriteLine("\nInvalid choice. Please enter a number between 0 and 3.");
                    break;
            }
            try
            {
             
            }
            catch (Exception e)
            {
                string fileName = "error.txt";
                string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug", fileName);
                Console.WriteLine("Oops there were some errors, please see: " + destPath);
                File.WriteAllText(destPath, e.ToString());
            }

        }

        // Close the database connection
        conn.Close();
    }
}
