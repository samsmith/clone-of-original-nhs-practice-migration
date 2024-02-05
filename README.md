# GP Connect Patient Migrator

This is a Proof of concept for using GP Connect as a means of migrating data between suppliers. It is also being used to demonstrate that GP connect can be used for maintaining a central database for GP data and contains a first draft database schema for storing GP Connect data for research purposes

## Requirements
```
* .net 6
* MS SQL server v18
```
## Running the code

Ensure the requirements are installed, example sql create scripts are provided and need to be run on an MS SQL database, once the database is setup, configure the connection strings to point to your local database 

❗️ Do not check in any connection strings to source control

Run the code in an IDE of your choosing

There are currently no unit tests as the aim is to grab the data and prove it is sufficient for our needs but these will come later