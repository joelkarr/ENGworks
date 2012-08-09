using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CDWKS.Model.Poco.Content;
using CDWKS.Utility.Revit.Common;
using Parameter = CDWKS.Model.Poco.Content.Parameter;


namespace CDWKS.Utility.Revit2011
{
    public class RevitFamilyIndexer :IRevitFamilyIndexer
    {
        private Document Document { get; set; }
        private ExternalCommandData CommandData { get; set; }
        public RevitFamilyIndexer(ExternalCommandData commandData)
        {
            CommandData = commandData;
        }

        public AutodeskFile GetFile(string path)
        {
            try
            {
                if (FileReadAllowed(path))
                {
                    
                    Document = CommandData.Application.Application.OpenDocumentFile(path);
                    var isTypeCatalog = IsTypeCatalog(path);
                    var typeCatalogLines = isTypeCatalog ? GetTypeCatalogLines(path) : new List<string>();
                    var file = new AutodeskFile
                                   {
                                       Name = Document.Title,
                                       MC_OwnerId = "5",
                                       TypeCatalogHeader = isTypeCatalog ? GetTypeCatalogHeader(path) : String.Empty,
                                       Items = GetFamilyTypes(isTypeCatalog, typeCatalogLines),
                                       Version = GetVersion()
                                   };
                    Document.Close(false);
                    return file;
                }
                return null;
            }
            catch (Exception)
            {

                return null;
            }

        }

        private bool FileReadAllowed(string path)
        {
            var fp = new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Write, path);
            try
            {
                fp.Assert();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GetTypeCatalogHeader(string path)
        {
            return GetTypeCatalogLines(path).FirstOrDefault();
        }

        private bool IsTypeCatalog(string path)
        {
            return File.Exists(path.Replace("rfa", "txt"));
        }

        private int GetVersion()
        {
            try
            {
                var type = (from FamilyType t in Document.FamilyManager.Types select t).FirstOrDefault();

                var p = (from FamilyParameter param in Document.FamilyManager.Parameters
                         where param.Definition.Name == "Version" & param.StorageType == StorageType.Integer
                         select param).FirstOrDefault();

                if (type != null)
                {
                    var v = Int32.Parse(type.AsValueString(p));
                    return v;
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return 0;
        }

        public List<Item> GetFamilyTypes(bool isTypeCatalog, List<string> typeCatalogLines = null )
        {
            var items = new List<Item>();
            if (Document == null)
                throw new NullReferenceException("Autodesk Document is null: Must Get File before Getting Types");
            if(isTypeCatalog & typeCatalogLines != null)
            {

                #region Get Parameter Names

                //First Line Contains Parameter Names
                var tempParamNames = typeCatalogLines.First().Split(',');
                var paramNames = new string[tempParamNames.Count()];
                var counter = 0;
                foreach (var p in tempParamNames)
                {
                    paramNames[counter] = ProcessString(p.Split('#')[0]);
                    counter++;
                }
                //Remove Line after Values
                typeCatalogLines.RemoveAt(0);

                #endregion

                var familyParameters = GetRevitFamilyParameters();
                foreach(var typeCatalogEntry in typeCatalogLines)
                {
                    if (String.IsNullOrEmpty(typeCatalogEntry.Trim())) continue;
                    var indexedItem = new Item();
                    var paramValues = typeCatalogEntry.Split(',');
                    indexedItem.Name = paramValues[0];
                    indexedItem.TypeCatalogEntry = typeCatalogEntry;
                    foreach (var parameter in familyParameters)
                    {
                        if (!paramNames.Contains(parameter.SearchName.Name)) continue;
                        //Find index of ParamNames
                        var findCounter = 0;
                        var found = false;
                        while (!found & findCounter < paramNames.Count())
                        {
                            if (paramNames[findCounter] == parameter.SearchName.Name)
                            {
                                found = true;
                            }
                            else
                            {
                                findCounter++;
                            }
                        }
                        if (found)
                        {
                            parameter.SearchValue.Value = ProcessString(paramValues[findCounter]);
                        }

                    }
                    indexedItem.Parameters = familyParameters;
                    items.Add(indexedItem);
                }
            }
            else
            {
                if (Document.FamilyManager.Types.IsEmpty)
                {
                    #region No Types

                    items.Add(new Item{Name="Standard"});

                    #endregion
                }
                else if(Document.FamilyManager.Types.Size == 1)
                {
                    items.AddRange(Document.FamilyManager.Types.Cast<FamilyType>().Select(IndexRevitSingleType).Where(a => a.Name.Trim() != String.Empty));
                }
                else
                {
                    items.AddRange(Document.FamilyManager.Types.Cast<FamilyType>().Select(IndexRevitMultipleTypes).Where(a=>a.Name.Trim() != String.Empty));

                }
            }
            return items;
        }

        public List<Parameter> GetRevitFamilyParameters()
        {
           return GetRevitFamilyParameters(Document.FamilyManager.CurrentType);
        }

        public List<Parameter> GetRevitFamilyParameters(FamilyType type)
        {
            if (Document == null)
                throw new NullReferenceException("Autodesk Document is null: Must Get File before Getting Types");
            return (from FamilyParameter param in Document.FamilyManager.Parameters
                    where ParamSwitch(param, type) != null
                    let searchName = new SearchName {Name = param.Definition.Name}
                    let searchValue = new SearchValue {Value = ParamSwitch(param, type).ToString()}
                    select new Parameter {SearchValue = searchValue, SearchName = searchName}).ToList();
        }

        private Item IndexRevitType(FamilyType type, string nameOverride = "")
        {
            var currentItem = new Item
                                  {
                                      Name = type.Name.Trim() != String.Empty ? type.Name : nameOverride,
                                      TypeCatalogEntry = string.Empty,
                                      Parameters = GetRevitFamilyParameters(type)
                                  };
            return currentItem;
        }

        private Item IndexRevitSingleType(FamilyType type)
        {
            return IndexRevitType(type, "Standard");
        }
        private Item IndexRevitMultipleTypes(FamilyType type)
        {
            return IndexRevitType(type);
        }
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

        public string ProcessString(string value)
        {
            var tempValue = value;

            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                tempValue = tempValue.Remove(0, 1);
                tempValue = tempValue.Remove(tempValue.Length - 1, 1);
            }
            tempValue = tempValue.Replace("\"\"", "\"");
            return tempValue;
        }

        private object ParamSwitch(FamilyParameter param, FamilyType type)
        {
            object value = String.Empty;
            try
            {
                var paramType = param.StorageType;
                switch (paramType)
                {
                    case StorageType.String:
                        value = type.AsString(param);
                        if (value != null)
                        {
                            value = value as String;
                        }
                        break;
                    case StorageType.Integer:
                        value = type.AsValueString(param);
                        break;
                    case StorageType.Double:
                        value = type.AsValueString(param);
                        break;
                    case StorageType.ElementId:
                        value = String.Empty;
                        break;
                }
            }
            catch (Exception)
            {

            }
            return value;
        }

    }
}
