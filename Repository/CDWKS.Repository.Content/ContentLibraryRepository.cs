using System;
using System.Linq;
using System.Linq.Expressions;
using CDWKS.Model.EF.Content;

namespace CDWKS.Repository.Content
{
    public interface IContentLibraryRepository
    {
        ContentLibrary GetByID(int id);
        ContentLibrary GetByNameAndOwner(string libraryName, string ownerId);
        ContentLibrary AddContentLibrary(string libraryName);
    }

    public class ContentLibraryRepository : IContentLibraryRepository
    {
        private readonly BXC_ContentModelEntities _context;

        public ContentLibraryRepository(BXC_ContentModelEntities context)
        {
            _context = context;
        }

        public ContentLibrary GetByID(int id)
        {
            return _context.ContentLibraries.FirstOrDefault(l => l.Id == id);
        }

        public ContentLibrary GetByNameAndOwner(string libraryName, string ownerId)
        {
            Expression<Func<ContentLibrary, bool>> expr = l => l.Name == libraryName;
            return _context.ContentLibraries.Where(expr).FirstOrDefault();
        }

        public ContentLibrary AddContentLibrary(string libraryName)
        {
            var lib = new ContentLibrary { Name = libraryName };
            _context.AddObject("ContentLibraries", lib);
            _context.SaveChanges();
            return lib;
        }

    }
}
