using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phases.UmbracoGenie.Models.Dtos
{
    public class TextModelDto
    {
        public string? Name { get; set; }
        public string? ModelId { get; set; }
        public string? ApiKey { get; set; }
        public string? Endpoint { get; set; }
    }

    public class ImageModelDto
    {
        public string? Name { get; set; }
        public string? Provider { get; set; }
        public string? ApiKey { get; set; }
        public string? Model { get; set; }
        public string? ModelId { get; set; }
        public string? Endpoint { get; set; }
    }

    public class GenieConfigurationDto
    {
        public TextModelDto? SelectedTextModel { get; set; }
        public ImageModelDto? SelectedImageModel { get; set; }
        public IEnumerable<TextModelDto>? TextModels { get; set; }
        public IEnumerable<ImageModelDto>? ImageModels { get; set; }
        public bool? enableForTextBox { get; set; }
        public bool? enableForTextArea { get; set; }
    }
}
