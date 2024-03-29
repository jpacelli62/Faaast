﻿using System;
using Faaast.Orm.Converters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Faaast.Tests.Orm.Fixture
{
    public class FaaastOrmFixture
    {
        public IServiceProvider Services { get; set; }

        public FakeDB Db { get; set; }

        public FaaastOrmFixture()
        {
            var services = new ServiceCollection();
            services.AddFaaastOrm();
            services.AddSingleton<FakeDB>();
            services.AddSingleton<EnumToIntValueConverter<TestState>>();
            this.Services = services.BuildServiceProvider();
            this.Db = this.Services.GetRequiredService<FakeDB>();
            Assert.NotNull(this.Db.Mapper);
            Assert.NotNull(this.Db.Mappings.Value);
        }
    }
}
