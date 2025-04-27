## Get Started 
1. Add varible in .env file and name it MSSQL_PASSWORD and add a secure password as value
2. add packege.json file with your client id to google
   {
  "GoogleAuth": {
    "ClientId": "",
    "ClientSecret": ""
  },
  "AllowedHosts": "*",
}
3. Make sure Docker desktop is runing
4. I recommed using Visual Studio and press Docker Compose to run the app but you can use the command run docker compose up --build
6. It should now run at the url https://localhost:8081/
7. When you log in for the first time you become an Admin and all the others normal users.
8. The admin can change oher users role.
