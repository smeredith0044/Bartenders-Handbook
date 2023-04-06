# Bartender's Handbook Console Application

![Bartender's Handbook](https://cdn.diffords.com/contrib/bundle-assets/1121/header.jpg)

This console application was developed by Sydney Meredith as part of the Code Louisville program's Software Development 2 course. The application allows users to create, edit, and delete records for various drinks, containing information such as the drink name, ingredients list, and flavor profile.

## Table of Contents
1. [Installation](#installation)
2. [Usage](#usage)
3. [Features](#features)
4. [Technologies](#technologies)
5. [Contributing](#contributing)

## Installation
To install this application, please follow these steps:

- Clone this repository to your local machine
- Install the required packages by running `dotnet restore`
- This application uses a remote Heroku server to store the database, so there is no need to configure a local or remote database connection string
- Run the application using `dotnet run` command
-Install visual studio to run on your local machine

## Usage
Once the application is running, the user will be presented with a menu containing several options, including creating, editing, and deleting drinks records. The user can also search for drinks by name, and sort the list in ascending or descending order.

## Features
- Regular Expression (regex) validation to ensure a phone number or email address is stored and displayed in a consistent format.
- Error handling with logging to record errors, invalid inputs, or other important events.
- Implementation of SOLID principles:
  - Single Responsibility Principle: The code is organized in a way that each class and method has only one responsibility.
  - Dependency Inversion Principle: The application's classes depend on abstractions instead of concretions.
- Use of dictionary to store and retrieve values in the program.

## Technologies
- C#
- .NET Core 3.1
- Entity Framework Core 3.1
- SQL Server
- Heroku
- Visual Studio

## Contributing
Contributions are welcome! Please feel free to submit a pull request with any changes or improvements you would like to make.
