using System.Xml;

namespace lab2
{
    public class DomAnalyzer : IAnalyzer
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

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath); 

            // XPath query to get all publications
            XmlNodeList? publicationNodes = xmlDoc.SelectNodes("//Publication");

            if(publicationNodes != null)
            foreach (XmlNode publicationNode in publicationNodes)
            {
                var publication = new Publication
                {
                    Title = GetNodeValue(publicationNode, "Title"),
                    PublishedYear = ParseNullableInt(GetNodeValue(publicationNode, "PublishedYear")) ?? 0, 
                    Faculty = GetNodeValue(publicationNode, "Faculty"),
                    Department = GetNodeValue(publicationNode, "Department"),
                    Authors = publicationNode["Authors"]?.GetElementsByTagName("Author")
                                    .Cast<XmlElement>()
                                    .Select(a => a.InnerText.Trim())
                                    .ToList() ?? new List<string>()
                };

                // for filters
                if (filterService.IsMatchingFilters(publication, titleFilter, authorsFilter, publishedYearFilter, facultyFilter, departmentFilter))
                {
                    publications.Add(publication);
                }
            }

            return publications;
        }

        private string GetNodeValue(XmlNode parentNode, string childNodeName)
        {
            var childNode = parentNode.SelectSingleNode(childNodeName);
            return childNode?.InnerText.Trim() ?? string.Empty;
        }

        private List<string> GetAuthors(XmlNode publicationNode)
        {
            var authors = new List<string>();
            var authorNodes = publicationNode.SelectNodes("Author");
            if(authorNodes != null) 
            foreach (XmlNode authorNode in authorNodes)
            {
                if (!string.IsNullOrEmpty(authorNode.InnerText))
                {
                    authors.Add(authorNode.InnerText.Trim());
                }
            }

            return authors;
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
