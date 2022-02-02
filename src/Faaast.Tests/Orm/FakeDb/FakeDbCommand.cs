﻿using System;
using System.Data;
using System.Data.Common;

namespace Faaast.Tests.Orm.FakeDb
{
    public class FakeDbCommand : DbCommand
    {
        public bool Prepared { get; set; }
        public FakeDbDataReader Reader { get; set; }
        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        protected override DbConnection DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection { get; } = new FakeDbParameters();

        protected override DbTransaction DbTransaction { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }

        public override int ExecuteNonQuery() => 18;

        public override object ExecuteScalar() => new();

        public override void Prepare() => this.Prepared = true;

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
            this.DbConnection.State != ConnectionState.Open ? throw new Exception("Connection is not open") : this.Reader;
        public override void Cancel() => throw new NotImplementedException();
        protected override DbParameter CreateDbParameter() => new FakeDbParameter();
    }
}
