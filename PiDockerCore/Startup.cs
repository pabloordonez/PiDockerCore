using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Paradigm.Core.Logging;
using Paradigm.ORM.Data.Database.Schema;
using Paradigm.ORM.Data.Database.Schema.Structure;
using Paradigm.ORM.Data.MySql;

namespace PiDockerCore
{
    public class Startup
    {
        #region Public Methods

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ILogging, ConsoleLogging>();
        }

        public async Task Run(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogging>();
            var customMessage = $"{{3}}{Environment.NewLine}";
            logger.SetMinimumLevel(LogType.Trace);
            logger.SetCustomMessage(LogType.Trace, customMessage);
            logger.SetCustomMessage(LogType.Debug, customMessage);
            logger.SetCustomMessage(LogType.Information, customMessage);
            logger.SetCustomMessage(LogType.Warning, customMessage);
            logger.SetCustomMessage(LogType.Error, customMessage);
            logger.SetCustomMessage(LogType.Critical, customMessage);

            try
            {
                var arguments = serviceProvider.GetRequiredService<ApplicationArguments>();

                logger.Log($"Trying to connect to '{arguments.Host}' to read database '{arguments.Database}' with user '{arguments.UserName}':", LogType.Information);

                var connection = new MySqlDatabaseConnector($"SERVER={arguments.Host};DATABASE={arguments.Database};UID={arguments.UserName};PASSWORD={arguments.Pepe};Allow User Variables=True;POOLING=true;Connection Timeout=120;Default Command Timeout=120");

                await connection.OpenAsync();

                if (!connection.IsOpen())
                {
                    logger.Log("Couldn't stablish a connection with the server.", LogType.Error);
                    return;
                }

                var schemaProvider = connection.GetSchemaProvider();
                var tables = await schemaProvider.GetTablesAsync(arguments.Database);
                var views = await schemaProvider.GetViewsAsync(arguments.Database);
                var procedures = await schemaProvider.GetStoredProceduresAsync(arguments.Database);

                foreach (var table in tables)
                {
                    await PrintTablesAsync(logger, arguments.Database, schemaProvider, table);
                }

                foreach (var view in views)
                {
                    await PrintViewsAsync(logger, arguments.Database, schemaProvider, view);
                }

                foreach (var procedure in procedures)
                {
                    await PrintProceduresAsync(logger, arguments.Database, schemaProvider, procedure);
                }
            }
            catch (Exception e)
            {
                logger.Log(e.Message, LogType.Error);
            }

        }

        #endregion

        #region Private Methods

        private static async Task PrintTablesAsync(ILogging logger, string database, ISchemaProvider schemaProvider, ITable table)
        {
            logger.Log($"TABLE {table.Name}:", LogType.Information);

            var columns = await schemaProvider.GetColumnsAsync(database, table.Name);

            foreach (var column in columns)
            {
                logger.Log($"   COLUMN {column.Name} ({column.DataType}){(column.IsIdentity ? " IDENTITY" : "")}{(column.IsNullable ? " NULLABLE" : "")}");
            }

            var constraints = await schemaProvider.GetConstraintsAsync(database, table.Name);

            foreach (var constraint in constraints)
            {
                logger.Log($"   CONSTRAINT {constraint.Name} ({constraint.Type})");
            }
        }

        private static async Task PrintViewsAsync(ILogging logger, string database, ISchemaProvider schemaProvider, IView view)
        {
            logger.Log($"TABLE {view.Name}:", LogType.Information);

            var columns = await schemaProvider.GetColumnsAsync(database, view.Name);

            foreach (var column in columns)
            {
                logger.Log($"   COLUMN {column.Name} ({column.DataType}){(column.IsIdentity ? " IDENTITY" : "")}{(column.IsNullable ? " NULLABLE" : "")}");
            }
        }

        private static async Task PrintProceduresAsync(ILogging logger, string database, ISchemaProvider schemaProvider, IStoredProcedure procedure)
        {
            logger.Log($"TABLE {procedure.Name}:", LogType.Information);

            var parameters = await schemaProvider.GetParametersAsync(database, procedure.Name);

            foreach (var parameter in parameters)
            {
                logger.Log($"   {(parameter.IsInput ? "IN" : "OUT")} PARAMETER {parameter.Name} ({parameter.DataType})");
            }
        }

        #endregion
    }
}