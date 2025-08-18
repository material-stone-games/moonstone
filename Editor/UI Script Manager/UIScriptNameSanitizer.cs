namespace Moonstone.UIScriptManagement
{
    public static class UIScriptNameSanitizer
    {
        public static string Sanitize(string text)
        {
            return text.Replace(" ", "");
        }
    }
}
