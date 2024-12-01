using System.Collections.Generic;

namespace lab2
{
    public class XmlAnalyzerContext
    {
        private IAnalyzer _analyzer;

        public XmlAnalyzerContext(IAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        public void SetAnalyzer(IAnalyzer analyzer)
        {
            if (_analyzer != analyzer)
            {
               _analyzer = analyzer;
            }   
        }

        public List<Publication> AnalyzeXml(string xmlFilePath)
        {
            return _analyzer.Analyze(xmlFilePath,
                                  null,
                                  null,
                                  null,
                                  null,
                                  null);
        }

        public List<Publication> ApplyFilters(string xmlFilePath,
                                  string titleFilter,
                                  List<string> authorsFilter,
                                  int? publishedYearFilter,
                                  string facultyFilter,
                                  string departmentFilter)
        {
            return _analyzer.Analyze(xmlFilePath,
                                  titleFilter,
                                  authorsFilter,
                                  publishedYearFilter,
                                  facultyFilter,
                                  departmentFilter);
        }
    }
}
