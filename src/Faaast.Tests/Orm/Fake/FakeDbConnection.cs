﻿using System;
using System.Data;

namespace Faaast.Tests.Orm.FakeConnection
{
    public class FakeDbConnection : IDbConnection
    {
        public string ConnectionString { get; set; }

        public int ConnectionTimeout { get; set; }

        public string Database { get; set; }

        public FakeCommand Command { get; set; }

        public ConnectionState State { get; set; }

        public IDbTransaction BeginTransaction() => throw new NotImplementedException();

        public IDbTransaction BeginTransaction(IsolationLevel il) => throw new NotImplementedException();

        public void ChangeDatabase(string databaseName)
        {
            //Do nothing
        }

        public void Close() => this.State = ConnectionState.Closed;

        public IDbCommand CreateCommand() => this.Command ?? new FakeCommand();

        public void Dispose() => GC.SuppressFinalize(this);

        public void Open() => this.State = ConnectionState.Open;
    }
}