using System;
using System.Collections.Generic;
using System.Xml;

namespace  lab2
{
    public class SaxAnalyzer : IAnalyzer
    {
        public List<Publication> Analyze(string filePath,
                                         string? titleFilter,
                                         List<string>? authorsFilter,
                                         int? publishedYearFilter,
                                         string? facultyFilter,
                                         string? departmentFilter)
        {
            var publications = new List<Publication>();
            Publication? currentPublication = null;
            string currentElement = string.Empty;

            var filterService = new FilterService();

            bool applyFilters = !string.IsNullOrEmpty(titleFilter) ||
                                (authorsFilter != null && authorsFilter.Count > 0) ||
                                publishedYearFilter.HasValue ||
                                !string.IsNullOrEmpty(facultyFilter) ||
                                !string.IsNullOrEmpty(departmentFilter);

            using (var reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            currentElement = reader.Name;
                            if (currentElement == "Publication")
                            {
                                currentPublication = new Publication();
                            }
                            else if (currentElement == "Author" && currentPublication != null)
                            {
                                reader.Read(); 
                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    if (currentPublication.Authors == null)
                                    {
                                        currentPublication.Authors = new List<string>();
                                    }
                                    currentPublication.Authors.Add(reader.Value.Trim());
                                }
                            }
                            break;

                        case XmlNodeType.Text:
                            if (currentPublication != null)
                            {
                                switch (currentElement)
                                {
                                    case "Title":
                                        currentPublication.Title = reader.Value;
                                        break;
                                    case "PublishedYear":
                                        currentPublication.PublishedYear = int.Parse(reader.Value);
                                        break;
                                    case "Faculty":
                                        currentPublication.Faculty = reader.Value;
                                        break;
                                    case "Department":
                                        currentPublication.Department = reader.Value;
                                        break;
                                }
                            }
                            break;

                        case XmlNodeType.EndElement:
                            if (reader.Name == "Publication" && currentPublication != null)
                            {
                                if (!applyFilters || filterService.IsMatchingFilters(currentPublication, titleFilter, authorsFilter, publishedYearFilter, facultyFilter, departmentFilter))
                                {
                                    publications.Add(currentPublication);
                                }
                                currentPublication = null; 
                            }
                            break;
                    }
                }
            }

            return publications;
        }
    }
}