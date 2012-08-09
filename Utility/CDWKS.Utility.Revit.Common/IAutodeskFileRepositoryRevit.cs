using CDWKS.Model.Poco.Content;

namespace CDWKS.Utility.Revit.Common
{
    public interface IAutodeskFileRepositoryRevit
    {
        AutodeskFile GetRevitFamily(string path);
    }
}
