using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CDWKS.Model.Poco.Content;
using CDWKS.Shared.ObjectFactory;
using Ninject;

namespace CDWKS.Utility.Revit.Common
{
    public class AutodeskFileRepositoryRevit : IAutodeskFileRepositoryRevit
    {

        #region Properties

// ReSharper disable InconsistentNaming
        private IRevitFamilyIndexer _revitFamilyIndexer { get; set; }
// ReSharper restore InconsistentNaming

        protected IRevitFamilyIndexer RevitFamilyIndexer
        {
           get { return _revitFamilyIndexer = _revitFamilyIndexer ?? Construction.StandardKernel.Get<IRevitFamilyIndexer>();}
        }

        #endregion

        public AutodeskFile GetRevitFamily(string path)
        {
            var file = RevitFamilyIndexer.GetFile(path);
            var catalogLines = GetTypeCatalogLines(path);
            if (catalogLines.Count > 0)
            {
                #region Type Catalog

                //Revit Family has Type Catalog
                var topLine = catalogLines.First();
                file.TypeCatalogHeader = topLine;
                //First Line Contains Parameter Names
                var paramNames = GetTypeCatalogParameterNames(topLine);
                //Remove Top Line to leave just Types
                catalogLines.RemoveAt(0);
                var familyParameters = RevitFamilyIndexer.GetRevitFamilyParameters();
                foreach (var line in catalogLines)
                {
                    var values = line.Split(',');
                    var currentItem = new Item();
                    foreach (var familyParameter in familyParameters)
                    {
                        var index = Array.IndexOf(paramNames, familyParameter.SearchName.Name);
                        if (index > -1)
                        {
                            familyParameter.SearchValue.Value = values[index];
                        }
                        currentItem.Parameters.Add(familyParameter);
                    }
                    file.Items.Add(currentItem);
                }

                #endregion
            }
            else
            {
                file.Items = (RevitFamilyIndexer.GetFamilyTypes(false,null));
            }
            

            return file;
        }


        #region Helper Methods
        public List<string> GetTypeCatalogLines(string path)
        {
            var lines = new List<string>();
            using (var r = new StreamReader(path.Replace("rfa", "txt")))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        public string[] GetTypeCatalogParameterNames(string line)
        {
            var tempParamNames = line.Split(',');
            var paramNames = new string[tempParamNames.Count()];
            var counter = 0;
            foreach (var p in tempParamNames)
            {
                paramNames[counter] = p.Split('#')[0];
                counter++;
            }
            return paramNames;
        }
        #endregion
    }
}
