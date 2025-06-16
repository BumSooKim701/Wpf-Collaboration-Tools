using System.Data;
using MySqlConnector;

namespace CollaborationTools.database;

public class ConnectionPool : IDisposable
{
    private const int MAX_POOL_SIZE = 10; //최대 생성 가능한 커넥션 수
    private const int MIN_POOL_SIZE = 3; //처음에 생성될 최소 커넥션 수
    private static ConnectionPool _instance;
    private static readonly object _instanceLock = new(); //인스턴스 처리용 락
    private readonly MySqlConnectionStringBuilder _builder;
    private readonly Queue<MySqlConnection> _connectionPool;
    private readonly object _poolLock = new(); //풀 커넥션 처리용 락

    private int _currentConnectionCount; //현재 총 커넥션 수
    private bool _disposed;

    //생성자
    private ConnectionPool()
    {
        _builder = new MySqlConnectionStringBuilder
        {
            Server = "collaboration-tool-db.cx0wksqqo8cm.ap-northeast-2.rds.amazonaws.com",
            UserID = "root",
            Password = "rlaqjatn1026",
            Database = "collaboration-tools",
            MinimumPoolSize = MIN_POOL_SIZE,
            MaximumPoolSize = MAX_POOL_SIZE
        };

        _connectionPool = new Queue<MySqlConnection>();
        InitializePool();
    }
    
    //앱 종료 시 커넥션 Dispose
    public void Dispose()
    {
        if (!_disposed)
        {
            lock (_poolLock)
            {
                while (_connectionPool.Count > 0) _connectionPool.Dequeue()?.Dispose();
                _currentConnectionCount = 0;
            }

            _disposed = true;
        }
    }

    //풀 초기화
    private void InitializePool()
    {
        for (var i = 0; i < MIN_POOL_SIZE; i++, _currentConnectionCount++)
            _connectionPool.Enqueue(CreateNewConnection());
    }

    //인스턴스 생성 및 반환
    public static ConnectionPool GetInstance()
    {
        if (_instance == null)
            lock (_instanceLock)
            {
                //더블 체크
                if (_instance == null) _instance = new ConnectionPool();
            }

        return _instance;
    }

    //커넥션 관리 및 사용처에 리턴
    public MySqlConnection GetConnection()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ConnectionPool));

        lock (_poolLock)
        {
            if (_connectionPool.Count > 0)
            {
                _currentConnectionCount++;
                return _connectionPool.Dequeue();
            }

            if (_currentConnectionCount < MAX_POOL_SIZE)
                try
                {
                    _currentConnectionCount++;
                    return CreateNewConnection();
                }
                catch (Exception e)
                {
                    _currentConnectionCount--;
                    throw new Exception($"Failed to create database connection: {e.Message}", e);
                }
        }

        throw new Exception("Connection pool is full.");
    }

    //사용이 끝난 커넥션을 풀에 반환
    public void ReleaseConnection(MySqlConnection connection)
    {
        if (connection == null) return;

        lock (_poolLock)
        {
            if (connection.State == ConnectionState.Open)
                //커넥션 풀에 다시 추가
                _connectionPool.Enqueue(connection);
            else
                connection.Dispose();
        }
    }

    //커넥션 생성
    private MySqlConnection CreateNewConnection()
    {
        var connection = new MySqlConnection(_builder.ConnectionString);
        connection.Open();
        return connection;
    }
}