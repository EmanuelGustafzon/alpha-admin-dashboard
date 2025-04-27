# Get Started 

## Use Docker 
1. Add varible in .env file and name it MSSQL_PASSWORD and add a secure password as value
2. Add packege.json file with your client id to google
   {
  "GoogleAuth": {
    "ClientId": "",
    "ClientSecret": ""
  },
  "AllowedHosts": "*",
}
3. Make sure Docker desktop is runing
4. use connection string: Environment.GetEnvironmentVariable("mssqlConnectionString");
5. I recommed using Visual Studio and press Docker Compose to run the app but you can use the command run docker compose up --build
6. It should now run at the url https://localhost:8081/

## Use localDb
1. Add packege.json file with your client id to google and your conenction string.
```json
{
  "GoogleAuth": {
    "ClientId": "",
    "ClientSecret": ""
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "LocalDb": ""
  }
}
```
3. use connection string: builder.Configuration.GetConnectionString("LocalDb");
   
## When the application runs it is good to know
1. When you log in for the first time you become an Admin and all the others normal users.
2. The admin can change oher users role.
