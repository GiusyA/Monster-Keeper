
using Supabase.Gotrue.Exceptions;

public static class SB_Utils
{
    public static string GetCleanedGotrueExceptionMessage(GotrueException _goException)
    {
        string _message = _goException.Message;
        string _cleanedMessage = _message.Substring(_message.IndexOf("msg\":\"") + 6);
        _cleanedMessage = _cleanedMessage.Substring(0, _cleanedMessage.IndexOf("\""));
        return _cleanedMessage;
    }
}
