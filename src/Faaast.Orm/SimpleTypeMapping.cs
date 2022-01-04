﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Faaast.Orm.Mapping;
using Faaast.Orm.Model;

namespace Faaast.Orm
{
    public class SimpleTypeMapping
    {
        public Type Type { get; set; }

        public TableMapping Table { get; set; }

        public SimpleTypeMapping(Type type)
        {
            this.Type = type;
            this.Table = new TableMapping()
            {
                Table = new Table()
            };
        }

        public void ToTable(string name, string schema = null)
        {
            this.Table.Table.Name = name;
            this.Table.Table.Schema = schema;
        }

        public void ToDatabase(string name) => this.Table.Database = name;
    }

    public class SimpleTypeMapping<TClass> : SimpleTypeMapping where TClass : class
    {
        public SimpleTypeMapping() : base(typeof(TClass))
        {
        }

        public Column Map<TProperty>(Expression<Func<TClass, TProperty>> member, string columnName) => this.Map(member, new Column(columnName));

        public Column Map<TProperty>(Expression<Func<TClass, TProperty>> member, Column column)
        {
            var exp = member.Body as MemberExpression ?? throw new ArgumentException("Must be a MemberExpression", nameof(member));
            var mappings = this.Table.ColumnMappings ?? new List<ColumnMapping>();
            mappings.Add(new ColumnMapping
            {
                Member = exp.Member,
                Column = column
            });
            this.Table.ColumnMappings = mappings;
            this.Table.Table.Columns.Add(column);
            return column;
        }
    }
}
