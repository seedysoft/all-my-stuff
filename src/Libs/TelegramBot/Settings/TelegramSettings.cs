﻿namespace Seedysoft.Libs.TelegramBot.Settings;

public record TelegramSettings : BackgroundServices.ScheduleConfig
{
    public required Users Users { get; init; }

    public TelegramBotUser CurrentBot => System.Diagnostics.Debugger.IsAttached ? Users.BotTest : Users.BotProd;
}