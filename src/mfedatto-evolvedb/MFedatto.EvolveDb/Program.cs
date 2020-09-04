using Serilog;
using Serilog.Sinks.SystemConsole;
using System;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace MFedatto.EvolveDb
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                     .WriteTo.ColoredConsole(Serilog.Events.LogEventLevel.Debug)
                     .MinimumLevel.Debug()
                     .Enrich.FromLogContext()
                     .CreateLogger();
                string dbConnectionString = "Server=MAUMAU-LENOVOZ4;Database=EVOLVE_DB;Trusted_Connection=True;";
                IDbConnection dbConnection = new SqlConnectionFactory()
                    .CreateConnection(dbConnectionString);
                Evolve.Evolve evolve = new Evolve.Evolve(dbConnection, msg => Log.Information(msg))
                {
                    MetadataTableSchema = "dbo",
                    MetadataTableName = "changelog",
                    Locations = new[]
                    {
                        "db/datasets",
                        "db/migrations/versioned",
                        "db/migrations/repeatable"
                    },
                    IsEraseDisabled = true
                };

                evolve.Migrate();
            }
            catch (Exception ex)
            {
                Log.Fatal("Database migration failed.", ex);
            }
        }
    }
}
