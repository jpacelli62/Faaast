﻿using Faaast.DatabaseModel;
using Faaast.Metadata;
using Faaast.Orm.Resolver;
using System;
using System.Collections.Generic;
using System.Data;

namespace Faaast.Orm.Reader
{
    public struct ObjectReader
    {
        public DtoClass Model;
        public ColumnReader[] ColumnsReaders;
        public string[] ColumnsNames;
        public Func<object> Activator;

        public ObjectReader(ICollection<Column> columns, DtoClass model)
        {
            this.Model = model;
            this.Activator = model.Activator;
            this.ColumnsReaders = null;
            this.ColumnsNames = null;
            if (columns != null)
            {
                this.ColumnsReaders = new ColumnReader[columns.Count];
                this.ColumnsNames = new string[columns.Count];
                int index = 0;
                foreach (var column in columns)
                {
                    this.ColumnsReaders[index] = new ColumnReader(column.Get(Meta.PocoProperty), column);
                    this.ColumnsNames[index] = column.Name;
                    index++;
                }
            }
        }

        public object Read(IDataReader reader, int[] indexes, int starting)
        {
            object instance = this.Activator();
            for (int i = 0; i < ColumnsReaders.Length; i++)
            {
                int index = indexes[i + starting];
                if (index != -1)
                    ColumnsReaders[i].Read(reader, index, instance);
            }

            return instance;
        }
    }
}
