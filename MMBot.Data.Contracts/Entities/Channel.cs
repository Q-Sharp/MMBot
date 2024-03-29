﻿namespace MMBot.Data.Contracts.Entities;

public class Channel : IHaveId, IHaveGuildId
{
    public int Id { get; set; }

    public ulong GuildId { get; set; }

    public ulong TextChannelId { get; set; }

    public ulong AnswerTextChannelId { get; set; }

    public byte[] Version { get; set; }

    public object Update(object channel)
    {
        if (channel is Channel c && Id == c.Id)
        {
            GuildId = c.GuildId;
            TextChannelId = c.TextChannelId;
            AnswerTextChannelId = c.AnswerTextChannelId;
        }
        return this;
    }
}
