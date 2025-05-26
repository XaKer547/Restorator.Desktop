namespace Restorator.Desktop.Models
{
    public class TemplateModel
    {
        public TemplateModel(byte[] content)
        {
            Content = content;
        }

        public byte[] Content { get; set; }
    }
}
