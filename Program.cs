
using JasperFx;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.SqlServer;
using Wolverine.Http;

public partial class Program
{
	private static async Task<int> Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddWolverineHttp();

        var connectionString = builder.Configuration.GetConnectionString("Data");

        builder.UseWolverine(opts =>
        {
            opts.Services.AddDbContextWithWolverineIntegration<ItemDbContext>(x =>
            {
                x.UseSqlServer(connectionString);
            });

            opts.Services.AddDbContextWithWolverineManagedMultiTenancy<ItemDbContext>((context, connectionString, tenantId) =>
            {
                context.UseSqlServer(connectionString.Value);
            });

            opts.PersistMessagesWithSqlServer(connectionString)
                .RegisterStaticTenants(x => { });

            opts.UseEntityFrameworkCoreTransactions();
            opts.Policies.UseDurableLocalQueues();
        });


        var app = builder.Build();

        app.MapWolverineEndpoints(opts =>
        {
            opts.TenantId.IsRequestHeaderValue("Tenant");
            opts.TenantId.DefaultIs(StorageConstants.DefaultTenantId);
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        };

        // hack to ensure the database is created
        using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ItemDbContext>();
            context.Database.EnsureCreated();
        }

        return await app.RunJasperFxCommands(args);
    }
}
