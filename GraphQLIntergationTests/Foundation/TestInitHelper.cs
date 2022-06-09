using System.Reflection;
using AutoFixture.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GraphQLIntegrationTests.Foundation;

public class TestInitHelper
{
    public static AutoFixture.Fixture CreteFixture()
    {
        var fixture = new AutoFixture.Fixture();
        fixture.Customizations.Add(new FixtureIgnoreVirtualMembers());
        return fixture;
    }

    public static DbContextOptions<DbContext> GetOptions()
    {
        var dbName = Guid.NewGuid().ToString();
        
        return new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(dbName)
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }

    public static DbContext CreateDbContext(DbContextOptions<DbContext>  options = null)
    {
        if (options == null)
        {
            options = GetOptions();
        }

        var context = new DbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }
}

public class FixtureIgnoreVirtualMembers : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var info = request as PropertyInfo;
        if (info != null && info.GetGetMethod().IsVirtual)
        {
            return new OmitSpecimen();
        }

        return new NoSpecimen();
    }
}