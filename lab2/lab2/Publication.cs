using System;
using System.Collections.Generic;

namespace lab2
{
    public class Publication
    {
        public string? Title { get; set; }
        public List<string> Authors { get; set; } = new List<string>();
        public int PublishedYear { get; set; }
        public string? Faculty { get; set; }
        public string? Department { get; set; }

        // Computed property for DataGrid display
        public string FormattedAuthors => string.Join(", ", Authors);

        public override string ToString()
        {
            return $"{Title} by {string.Join(", ", Authors)} ({PublishedYear})\nFaculty: {Faculty}, Department: {Department}";
        }
    }
}