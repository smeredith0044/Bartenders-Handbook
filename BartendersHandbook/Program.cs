using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Npgsql;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Single Responsibility Principle: The Program class is responsible for handling user input and database operations.

        string connString = "Server=ec2-34-197-91-131.compute-1.amazonaws.com;Port=5432;Database=ddab6aknfp5lq5;User Id=ltkkuxigptlbec;Password=f7d1b5f66e29dd77cb68cfb31740424d36a9127edd5c5fdfe49ea14e12ad23b1;SSL Mode=Require;Trust Server Certificate=true;";

        // Connect to a PostgreSQL database
        NpgsqlConnection conn = new NpgsqlConnection(connString);
        conn.Open();

        // Set the console text color to yellow
        Console.ForegroundColor = ConsoleColor.Yellow;

        // Prompt the user for their phone number
        Console.Write("Enter your phone number (format: xxx-xxx-xxxx): ");
        string phoneNumber = Console.ReadLine();

        // Validate the phone number using a regular expression
        Regex phoneRegex = new Regex(@"^\d{3}-\d{3}-\d{4}$");
        if (!phoneRegex.IsMatch(phoneNumber))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nInvalid phone number format. Please enter a phone number in the format xxx-xxx-xxxx.");
            Console.ResetColor();

        }

        // Display the main menu and prompt the user for a choice
        int choice = -1;
        while (choice != 0)
        {
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
            // Open/Closed Principle: The switch statement can be extended to support new features without modifying the existing code.

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
                    // Prompt the user for the name of the cocktail to delete
                    Console.Write("\nEnter the name of the cocktail to delete: ");
                    string deleteName = Console.ReadLine();

                    // Define a DELETE statement with a WHERE clause to delete the specified cocktail by name
                    NpgsqlCommand deleteCommand = new NpgsqlCommand("DELETE FROM cocktails WHERE name = @name", conn);
                    deleteCommand.Parameters.AddWithValue("@name", deleteName);

                    // Execute the DELETE statement and obtain the number of rows affected
                    rowsAffected = deleteCommand.ExecuteNonQuery();

                    // Check if the operation was successful
                    if (rowsAffected == 1)
                    {
                        Console.WriteLine("\nCocktail '{0}' deleted successfully!", deleteName);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nFailed to delete cocktail '{0}'.", deleteName);
                        Console.ResetColor();
                    }

                    break;

                case 0:
                    // Exit the program
                    Console.WriteLine("\nGoodbye!");
                    break;

                default:
                    // If the user enters an invalid option, display an error message
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid option. Please choose a valid option from the menu.");
                    Console.ResetColor();
                    break;
            }
        }

        // Close the database connection
        conn.Close();

        // Copy the "cocktail.txt" file to the "backup" folder and overwrite if it already exists
        try
        {
            string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cocktail.txt");
            string destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup", "cocktail.txt");
            File.Copy(sourcePath, destinationPath, true);
        }
        catch (Exception ex)
        {
            // Write error details to the "whoops.txt" file in the "debug" folder
            string debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug", "whoops.txt");
            File.WriteAllText(debugPath, ex.ToString());
        }
    }
}
