using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phases.UmbracoGenie.Models
{

    public class PromptModel
    {
        public string Prompt { get; set; }
    }
    public class EditPromptModel
    {
        public string Prompt { get; set; }
        public string EditedText { get; set; }
    }
}
