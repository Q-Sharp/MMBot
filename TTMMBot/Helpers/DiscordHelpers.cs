﻿using Discord;

namespace TTMMBot.Helpers
{
    public static class DiscordHelpers
    {
        public static string GetUserAndDiscriminator(this IUser sgu) => $"{sgu.Username}#{sgu.Discriminator}";
    }
}
