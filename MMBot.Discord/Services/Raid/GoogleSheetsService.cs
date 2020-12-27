using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Services.Raid
{
    public class GoogleSheetsService : MMBotService<GoogleSheetsService>, IGoogleSheetsService
    {
        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private static readonly string ApplicationName = "MMBot";

        private readonly IDatabaseService _databaseService;
        private readonly IGuildSettingsService _guildSettings;
        private readonly IConfiguration _configuration;

        private SheetsService _sheetsService;

        public GoogleSheetsService(IDatabaseService databaseService, IGuildSettingsService guildSettings, ILogger<GoogleSheetsService> logger, IConfiguration configuration) : base(logger)
        {
            _databaseService = databaseService;
            _guildSettings = guildSettings;
            _configuration = configuration;
        }

        public async Task ConnectAsync()
        {
            var gs = new GoogleClientSecrets();
            gs.Secrets.ClientId = _configuration.GetValue<string>("ClientId");
            gs.Secrets.ClientSecret =  _configuration.GetValue<string>("ClientSecret");

            var credPath = "token.json";
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(gs.Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true));

            // Create Google Sheets API service.
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
                
            });
        }

        public async Task<byte[]> GetTacticPictureAsync()
        {
             // Define request parameters.
            var spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
            var range = "Class Data!A2:E";
            var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            var response = await request.ExecuteAsync();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[4]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            return new byte[1];
        }
    }
}
