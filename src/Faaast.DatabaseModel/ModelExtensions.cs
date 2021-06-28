﻿using System.Collections.Generic;

namespace Faaast.DatabaseModel
{
    public static class ColumnExtensions
    {
        public static Column IsPrimaryKey(this Column column, bool isPromaryKey = true)
        {
            column.PrimaryKey = isPromaryKey;
            return column;
        }

        public static Column IsIdentity(this Column column, bool isIdentity = true)
        {
            column.Identity = isIdentity;
            return column;
        }

        public static Column IsNullable(this Column column, bool isNullable = true)
        {
            column.Set(DbMeta.Nullable, isNullable);
            return column;
        }

        public static Column Length(this Column column, int Length)
        {
            column.Set(DbMeta.Length, Length);
            return column;
        }

        public static Column References(this Column column, string schema, string table, string key)
        {
            column.Set(DbMeta.ForeignKey, !string.IsNullOrEmpty(key));
            column.Set(DbMeta.ReferenceSchema, schema);
            column.Set(DbMeta.ReferenceTable, table);
            column.Set(DbMeta.ReferenceColumn, key);
            return column;
        }

        public static Column[] PrimaryKeyColumns(this Table table)
        {
            var pk = table.Get(DbMeta.PrimaryKeyColumns);
            if (pk == null)
            {
                List<Column> columns = new List<Column>();
                foreach (var column in table.Columns)
                {
                    if (column.Value.PrimaryKey)
                        columns.Add(column.Value);
                }

                pk = columns.ToArray();
                table.Set(DbMeta.PrimaryKeyColumns, pk);
            }

            return pk;
        }
    }
}
