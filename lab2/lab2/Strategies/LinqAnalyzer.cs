using System.Xml.Linq;

namespace lab2
{

    public class LinqAnalyzer : IAnalyzer
    {
        public List<Publication> Analyze(string filePath,
                                         string? titleFilter,
                                         List<string>? authorsFilter,
                                         int? publishedYearFilter,
                                         string? facultyFilter,
                                         string? departmentFilter)
        {
            var publications = new List<Publication>();
            var filterService = new FilterService();

            XDocument xmlDoc = XDocument.Load(filePath);

            var publicationNodes = xmlDoc.Descendants("Publication");

            foreach (var publicationNode in publicationNodes)
            {
                var publication = new Publication
                {
                    Title = GetElementValue(publicationNode, "Title"),
                    PublishedYear = ParseNullableInt(GetElementValue(publicationNode, "PublishedYear")) ?? 0, 
                    Faculty = GetElementValue(publicationNode, "Faculty"),
                    Department = GetElementValue(publicationNode, "Department"),
                    Authors = publicationNode.Element("Authors")?
                         .Elements("Author")
                         .Select(a => a.Value.Trim())
                         .ToList() ?? new List<string>()
                };

                if (filterService.IsMatchingFilters(publication, titleFilter, authorsFilter, publishedYearFilter, facultyFilter, departmentFilter))
                {
                    publications.Add(publication);
                }
            }

            return publications;
        }

        private string GetElementValue(XElement parentElement, string childElementName)
        {
            return parentElement.Element(childElementName)?.Value?.Trim() ?? string.Empty;
        }

        private int? ParseNullableInt(string input)
        {
            if (int.TryParse(input, out int result))
            {
                return result;
            }
            return null;
        }
    }
}
