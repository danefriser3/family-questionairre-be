# Family questionairre backend

The backend for the Family questionairre app is developed with the "ASP.NET Core Web API" option in Visual Studio.

The dataset is saved as Sql Server data by adding "Server=(localdb)\\mssqllocaldb;Database=FamilyQuestionnaireDb;Trusted_Connection=True;MultipleActiveResultSets=true" connection string in the appsettings.json file.

The only model is the Child.cs one that will store the single Child data passed by the front end application.

All the operations are managed by the ChildrenController class.

The routes that are managed here are
### `HttpGet("childrenList")` that queries the saved children from the DB
### `HttpDelete("deleteAll")` that deletes all the data from the DB
### `HttpDelete("deleteById/{id:int}")` that deletes one single entry from the DB
### `HttpPost("submit")` that saves an entry in the DB
### `HttpPost("export")` that handles the data export to CSV

All the validation operations are managed in this controller.

## Running the Application
In order to run the application, go into the root folder and execute

`dotnet run`

This command will also show the command prompt for test purpose.

