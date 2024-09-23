// TODO:            Remove all pragma warning disable

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System.ComponentModel;

namespace Seedysoft.Libs.TelegramBot.Enums;

/// <summary>
/// Comandos disponibles en el bot.
/// </summary>
/// <remarks>Text of the command; 1-32 characters. Can contain only lowercase English letters, digits and underscores.</remarks>
public enum BotActionName
{
    /// <summary>
    /// Este comando es el que se usa para interactuar con el bot y es obligatorio.
    /// </summary>
    start = 000,

    [Description("Muestra tu email.")]
    email_show = 110,
    [Description("[email] Se establece el valor proporcionado como email del usuario. Si no se proporciona, se elimina.")]
    email_edit = 120,

    [Description($"[{Core.Constants.Formats.YearMonthDay}] Obtiene los PVPC.")]
    pvpc_fill = 200,

    [Description("Iniciar una conversación de búsqueda de productos.")]
    amaz_find = 310,

    [Description("Muestra a qué estás suscrito.")]
    subs_list = 410,
    [Description("{IdSuscripcion} Te da de baja de la suscripción.")]
    subs_quit = 420,

    //[System.ComponentModel.Description("")]
    //baja = 500,
}
