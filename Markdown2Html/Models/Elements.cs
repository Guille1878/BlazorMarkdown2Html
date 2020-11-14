using System.Collections.Generic;
using System.Linq;

namespace BlazorMarkdown2Html.Models
{
    internal class Elements 
    {
        public int Count { get; set; }
        public string Current { get; set; } = null;

        private HashSet<string> elements = new HashSet<string>();

        internal void Open(string element)
        {
            elements.Add(element);
            Current = element;
            Count++;
        }

        internal void Close()
        {
            if (elements.Any())
            {
                elements.RemoveWhere(e => e == elements.Last());
                Current = (Count == 1) ? null : elements.Last();
                Count--;
            }
        }

        internal void CloseIfCurrent(string currentWord)
        {
            if (currentWord == Current)
                Close();
        }
    }
}