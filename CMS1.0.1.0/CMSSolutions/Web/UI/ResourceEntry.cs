namespace CMSSolutions.Web.UI
{
    public class ResourceEntry
    {
        public ResourceEntry(string type, string path)
        {
            Type = type;
            Path = path;
            Location = ResourceLocation.Foot;
            Order = 9999;
        }

        public string Type { get; private set; }

        public string Path { get; private set; }

        public int Order { get; set; }

        public ResourceLocation Location { get; set; }

        public ResourceEntry HasOrder(int order)
        {
            Order = order;
            return this;
        }

        public ResourceEntry AtHead()
        {
            Location = ResourceLocation.Head;
            return this;
        }

        public ResourceEntry AtFoot()
        {
            Location = ResourceLocation.Foot;
            return this;
        }
    }
}
