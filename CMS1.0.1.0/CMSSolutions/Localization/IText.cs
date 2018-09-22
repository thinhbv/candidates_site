namespace CMSSolutions.Localization
{
    public interface IText
    {
        LocalizedString Get(string textHint, params object[] args);
    }
}