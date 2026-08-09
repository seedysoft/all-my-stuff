namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Misc;

/// <summary>
/// Panes are DOM elements used to control the ordering of layers on the map.
/// You can access panes with map.getPane or map.getPanes methods.
/// New panes can be created with the map.createPane method.
/// Every map has the following default panes that differ only in zIndex.
/// </summary>
[K(typeof(Core.Serialization.EnumMemberJsonConverter<Panes>))]
public enum Panes
{
#pragma warning disable format
    [System.Runtime.Serialization.EnumMember(Value = "tilePane")]       TilePane,
    [System.Runtime.Serialization.EnumMember(Value = "overlayPane")]    OverlayPane,
    [System.Runtime.Serialization.EnumMember(Value = "shadowPane")]     ShadowPane,
    [System.Runtime.Serialization.EnumMember(Value = "markerPane")]     MarkerPane,
    [System.Runtime.Serialization.EnumMember(Value = "tooltipPane")]    TooltipPane,
    [System.Runtime.Serialization.EnumMember(Value = "popupPane")]      PopupPane,
#pragma warning restore format
    }
