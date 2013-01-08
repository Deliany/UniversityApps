using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adastra.RichEditorLibrary
{
    public interface IRichTextEditor
    {
        string HTML { get; set; }
        string DocumentBody { get; set; }
        Dictionary<string, string> InsertCustomTextItems { get; set; }
    }
}
