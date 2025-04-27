using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Data.Context;

public class DataContextFatory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(@"Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\Emanuel\\source\\repos\\Alpha\\Data\\Database\\LocalDatabase.mdf; Integrated Security = True; Connect Timeout = 30");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
