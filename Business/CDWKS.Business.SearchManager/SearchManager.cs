using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CDWKS.Model.Poco.Content;

namespace CDWKS.Business.SearchManager
{
    public class SearchManager :ISearchManager
    {


        public List<Item> Search(string keyword)
        {
            throw new NotImplementedException();
        }

        public List<Item> Search(Dictionary<string, string> criteria)
        {
            var file1 = new AutodeskFile {Name = "Test file", Version = 1, Id = 5};
            var item1 = new Item {Name = "Test type 1", AutodeskFile = file1};
            var parameters = new Collection<Parameter>();
            var p = new Parameter
                        {
                            Hidden = false,
                            Featured = true,
                            SearchName = new SearchName {Name = "first"},
                            SearchValue = new SearchValue {Value = "param"}
                        };
            parameters.Add(new Parameter());
            item1.Parameters = parameters;

            var items = new List<Item> {item1};
            return items;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
