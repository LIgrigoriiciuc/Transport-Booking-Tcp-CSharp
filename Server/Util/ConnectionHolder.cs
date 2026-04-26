using Microsoft.Data.Sqlite;

namespace Server.Util;

public class ConnectionHolder : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly bool _owned;
    public ConnectionHolder(SqliteConnection connection, bool owned)
    {
        _connection = connection;
        _owned = owned;
    }

    public SqliteConnection Connection => _connection;

    public void Dispose()
    {
        if (_owned)
            {
            _connection.Dispose();
            }
    }
}