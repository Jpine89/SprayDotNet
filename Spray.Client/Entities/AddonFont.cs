namespace Spray.Client.Entities
{
    internal class AddonFont
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Category { get; set; }
        public int Id { get; set; }

        public AddonFont(string fileName, string name, string fontCategory)
        {
            Name = name;
            FileName = fileName;
            Category = fontCategory;
        }
    }
}
