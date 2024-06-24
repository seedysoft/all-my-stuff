// LEAVE FOR SAMPLE

//namespace Seedysoft.Libs.Infrastructure.Migrations.Views;

//public static class WebDataView
//{
//    // View version numbers
//    public enum VersionNumbers
//    {
//        // Uncoment when next version
//        //Version20230222,
//        Version20231112,
//        VersionFirst
//    }

//    private const string ViewName = nameof(WebDataView);

//    // Get the previous view create statement for a version number
//    public static string GetCreateSqlPrevious(VersionNumbers versionNumber)
//    {
//        return versionNumber switch
//        {
//            // Uncoment when next version
//            //VersionNumber.Version20230222 => Version20230222,
//            VersionNumbers.Version20231112 => Version20231112,
//            VersionNumbers.VersionFirst => VersionFirst,
//            _ => throw Shared.Constants.UnknownVersionNumberApplicationException
//        };
//    }

//    // Get the view create statement for the version number
//    public static string GetCreateSql(VersionNumbers versionNumber)
//    {
//        return versionNumber switch
//        {
//            // Uncoment when next version
//            //VersionNumber.Version20230222 => Version20230222,
//            VersionNumbers.Version20231112 => Version20231112,
//            VersionNumbers.VersionFirst => VersionFirst,
//            _ => throw Shared.Constants.UnknownVersionNumberApplicationException
//        };
//    }

//    public static string GetDropSql() => $@"DROP VIEW IF EXISTS {ViewName};";

//    // Uncoment when next version
//    //private static string Version20221230 => $@"CREATE VIEW {ViewName} AS ......";

//    private static string Version20231112 => $@"CREATE VIEW {ViewName} AS 
//        SELECT
//            {nameof(CoreLib.Entities.WebDataBase.SubscriptionId)}
//          , {nameof(CoreLib.Entities.WebDataBase.WebUrl)}
//          , {nameof(CoreLib.Entities.WebDataBase.Description)}
//          , {nameof(CoreLib.Entities.WebDataBase.CurrentWebContent)}
//          , {nameof(CoreLib.Entities.WebDataBase.SeenAtDateTimeOffset)}
//          , {nameof(CoreLib.Entities.WebDataBase.UpdatedAtDateTimeOffset)}
//          , {nameof(CoreLib.Entities.WebDataBase.IgnoreChangeWhen)}
//          , {nameof(CoreLib.Entities.WebDataBase.CssSelector)}
//          , {nameof(CoreLib.Entities.WebDataBase.TakeAboveBelowLines)}
//          , {nameof(CoreLib.Entities.WebDataBase.UseHttpClient)}
//          , (strftime('%s', datetime({nameof(CoreLib.Entities.WebDataBase.SeenAtDateTimeOffset)}))) AS {nameof(CoreLib.Entities.WebDataView.SeenAtDateTimeUnix)}
//          , (strftime('%s', datetime({nameof(CoreLib.Entities.WebDataBase.UpdatedAtDateTimeOffset)}))) AS {nameof(CoreLib.Entities.WebDataView.UpdatedAtDateTimeUnix)}
//        FROM {nameof(CoreLib.Entities.WebData)}
//        ";

//    private static string VersionFirst => $@"CREATE VIEW {ViewName} AS 
//        SELECT
//            {nameof(CoreLib.Entities.WebDataBase.SubscriptionId)}
//          , {nameof(CoreLib.Entities.WebDataBase.WebUrl)}
//          , {nameof(CoreLib.Entities.WebDataBase.Description)}
//          , {nameof(CoreLib.Entities.WebDataBase.CurrentWebContent)}
//          , {nameof(CoreLib.Entities.WebDataBase.SeenAtDateTimeOffset)}
//          , {nameof(CoreLib.Entities.WebDataBase.UpdatedAtDateTimeOffset)}
//          , {nameof(CoreLib.Entities.WebDataBase.IgnoreChangeWhen)}
//          , {nameof(CoreLib.Entities.WebDataBase.CssSelector)}
//          , {nameof(CoreLib.Entities.WebDataBase.TakeAboveBelowLines)}
//          , (strftime('%s', datetime({nameof(CoreLib.Entities.WebDataBase.SeenAtDateTimeOffset)}))) AS {nameof(CoreLib.Entities.WebDataView.SeenAtDateTimeUnix)}
//          , (strftime('%s', datetime({nameof(CoreLib.Entities.WebDataBase.UpdatedAtDateTimeOffset)}))) AS {nameof(CoreLib.Entities.WebDataView.UpdatedAtDateTimeUnix)}
//        FROM {nameof(CoreLib.Entities.WebData)}
//        ";
//}
