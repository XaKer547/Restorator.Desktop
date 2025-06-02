namespace Restorator.Desktop.Models
{
    public class TemplateModel
    {
        public TemplateModel(string path)
        {
            Content = path;
        }

        public string Content { get; set; }
    }
}
