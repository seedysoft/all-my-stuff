//using System.Diagnostics;

//namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Events;

//[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
//public abstract class Event
//{
//    /// <summary>
//    /// The event type (e.g. 'click').
//    /// </summary>
//    /*[J("type")]*/
//    public required string Type { get; init; }

//    /// <summary>
//    /// The object that fired the event. For propagated events, the last object in the propagation chain that fired the event.
//    /// </summary>
//    /*[J("target")]*/
//    public object? Target { get; init; }

//    /// <summary>
//    /// The object that originally fired the event. For non-propagated events, this will be the same as the target.
//    /// </summary>
//    /*[J("sourceTarget")]*/
//    public object? SourceTarget { get; init; }

//    /// <summary>
//    /// For propagated events, the last object that propagated the event to its event parent.
//    /// </summary>
//    /*[J("propagatedFrom")]*/
//    public object? PropagatedFromtarget { get; init; }

//    private string GetDebuggerDisplay() => Type;
//}
