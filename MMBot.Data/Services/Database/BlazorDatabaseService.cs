﻿using System;
using System.Collections.Generic;
using System.Linq;
using MMBot.Data;

namespace MMBot.Services.Database
{
    public class BlazorDatabaseService
    {
        private readonly Context _ctx;

        public BlazorDatabaseService(Context dataContext) => _ctx = dataContext;

        public IList<Tuple<ulong, string>> GetAllGuilds()
            => _ctx.GuildSettings.AsQueryable()
                .Select(x => Tuple.Create(x.GuildId, x.GuildName))
                .Distinct()
                .ToList();
    }
}
