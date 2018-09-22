namespace CMSSolutions.Localization.Models
{
    public class ComparitiveLocalizableString
    {
        public int SequenceId { get; set; }

        public string Key { get; set; }

        public string InvariantValue { get; set; }

        public string LocalizedValue { get; set; }
    }
}