﻿using MMBot.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMBot.Discord.Services.Interfaces
{
    public interface IJsonService
    {
        Task<IDictionary<string, string>> ExportDBToJson();
        Task<bool> ImportJsonToDB(IDictionary<string, string> importJson, Context context = null);
    }
}
