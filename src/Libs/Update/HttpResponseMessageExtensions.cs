namespace Seedysoft.Libs.Update;

public static class HttpResponseMessageExtensions
{
    public static bool IsRedirect(this HttpResponseMessage httpResponseMessage) => httpResponseMessage.StatusCode is
        System.Net.HttpStatusCode.Redirect or
        System.Net.HttpStatusCode.RedirectKeepVerb or
        System.Net.HttpStatusCode.RedirectMethod or
        System.Net.HttpStatusCode.Found or
        System.Net.HttpStatusCode.MovedPermanently;
}
