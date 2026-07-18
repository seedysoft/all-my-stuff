namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.UILayers;

/// <summary>
/// Used to open popups in certain places of the map.
/// Use Map.openPopup to open popups while making sure that only one popup is open at one time (recommended for usability), or use Map.addLayer to open as many as you want.
/// </summary>
public sealed record class Popup : DivOverlay
{
    public Popup() : base()
    {
        Pane = Map.Panes.PopupPane;
        Offset = new Basic.Point(0, 7);
    }

    /// <summary>
    /// Max width of the popup, in pixels.
    /// </summary>
    /// <remarks>Default: <c>300</c></remarks>
    [J("maxWidth")] public double? MaxWidth { get; set; } = 300;

    /// <summary>
    /// Min width of the popup, in pixels.
    /// </summary>
    /// <remarks>Default: <c>50</c></remarks>
    [J("minWidth")] public double? MinWidth { get; set; } = 50;

    /// <summary>
    /// If set, creates a scrollable container of the given height inside a popup if its content exceeds it.
    /// The scrollable container can be styled using the leaflet-popup-scrolled CSS class selector.
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("maxHeight")] public double? MaxHeight { get; set; } = null;

    /// <summary>
    /// Set it to <c>false</c> if you don't want the map to do panning animation to fit the opened popup.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("autoPan")] public bool? AutoPan { get; set; } = true;

    /// <summary>
    /// The margin between the popup and the top left corner of the map view after autopanning was performed.
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("autoPanPaddingTopLeft")] public Basic.Point? AutoPanPaddingTopLeft { get; set; } = null;

    /// <summary>
    /// The margin between the popup and the bottom right corner of the map view after autopanning was performed.
    /// </summary>
    /// <remarks>Default: <c>null</c></remarks>
    [J("autoPanPaddingBottomRight")] public Basic.Point? AutoPanPaddingBottomRight { get; set; } = null;

    /// <summary>
    /// Equivalent of setting both top left and bottom right autopan padding to the same value.
    /// </summary>
    /// <remarks>Default: <c>Point(5, 5)</c></remarks>
    [J("autoPanPadding")] public Basic.Point? AutoPanPadding { get; set; } = new Basic.Point(5);

    /// <summary>
    /// Set it to <c>true</c> if you want to prevent users from panning the popup off of the screen while it is open.
    /// </summary>
    /// <remarks>Default: <c>false</c></remarks>
    [J("keepInView")] public bool? KeepInView { get; set; } = false;

    /// <summary>
    /// Controls the presence of a close button in the popup.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("closeButton")] public bool? CloseButton { get; set; } = true;

    /// <summary>
    /// Specifies the 'aria-label' attribute of the close button.
    /// </summary>
    /// <remarks>Default: <c>'Close popup'</c></remarks>
    [J("closeButtonLabel")] public string? CloseButtonLabel { get; set; } = "Close popup";

    /// <summary>
    /// Set it to <c>false</c> if you want to override the default behavior of the popup closing when another popup is opened.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("autoClose")] public bool? AutoClose { get; set; } = true;

    /// <summary>
    /// Set it to <c>false</c> if you want to override the default behavior of the ESC key for closing of the popup.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("closeOnEscapeKey")] public bool? CloseOnEscapeKey { get; set; } = true;

    /// <summary>
    /// Set it if you want to override the default behavior of the popup closing when user clicks on the map.
    /// Defaults to the map's <see cref="Map.ClosePopupOnClick"/> option.
    /// </summary>
    /// <remarks>Default: <c>*</c></remarks>
    [J("closeOnClick")] public bool? CloseOnClick { get; set; }

    /// <summary>
    /// Whether the popup shall react to changes in the size of its contents (e.g. when an image inside the popup loads) and reposition itself.
    /// </summary>
    /// <remarks>Default: <c>true</c></remarks>
    [J("trackResize")] public bool? TrackResize { get; set; } = true;
}
