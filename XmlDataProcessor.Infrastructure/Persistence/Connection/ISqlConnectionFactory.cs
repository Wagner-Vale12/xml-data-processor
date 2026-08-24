using System.Data;

namespace XmlDataProcessor.Infrastructure.Persistence.Connection;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}