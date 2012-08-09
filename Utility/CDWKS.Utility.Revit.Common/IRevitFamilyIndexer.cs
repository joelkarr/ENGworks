using System.Collections.Generic;
using Autodesk.Revit.DB;
using CDWKS.Model.Poco.Content;
using Parameter = CDWKS.Model.Poco.Content.Parameter;

namespace CDWKS.Utility.Revit.Common
{
    public interface IRevitFamilyIndexer
    {
        AutodeskFile GetFile(string path); 
        List<Item> GetFamilyTypes(bool isTypeCatalog, List<string> typeCatalogLines);
        List<Parameter> GetRevitFamilyParameters();
        List<Parameter> GetRevitFamilyParameters(FamilyType type);
    }
}
