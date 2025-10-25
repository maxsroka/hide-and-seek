using System;
using Sandbox;
using Sandbox.Diagnostics;

public static class HideAndSeekLogger
{
    static Logger logger = new Logger( "Hide&Seek" );

    public static void Info(object message) => logger.Info(message);
    public static void Info(FormattableString message) => logger.Info(message);
    public static void Warning(object message) => logger.Warning(message);
    public static void Warning(FormattableString message) => logger.Warning(message);
    public static void Warning(Exception exception, FormattableString message) => logger.Warning(exception, message);
    public static void Error(object message) => logger.Error(message);
    public static void Error(FormattableString message) => logger.Error(message);
    public static void Error(Exception exception, FormattableString message) => logger.Error(exception, message);
    public static void Error(Exception exception, object message) => logger.Error(exception, message);
}
