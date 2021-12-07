﻿using SqlKata;

namespace Faaast.Orm
{
    public abstract class FaaastQueryBase<T> where T : FaaastQueryBase<T>
    {
        public FaaastQueryDb Db { get; set; }

        public Query Query { get; set; } = new Query();

        public FaaastQueryBase(FaaastQueryDb db) => this.Db = db;

        public abstract T NewQuery();

        public virtual T Clone()
        {
            var clone = this.NewQuery();
            clone.Query = this.Query.Clone();
            clone.Db = this.Db;
            return clone;
        }
    }
}