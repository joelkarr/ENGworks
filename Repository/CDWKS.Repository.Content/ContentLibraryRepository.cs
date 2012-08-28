using System;
using System.Linq;
using System.Linq.Expressions;
using CDWKS.Model.EF.BIMXchange;

namespace CDWKS.Repository.Content
{
    public interface ILibraryRepository
    {
        Library GetByID(int id);
        Library GetByNameAndOwner(string libraryName, string ownerId);
        Library AddLibrary(string libraryName);
    }

    public class LibraryRepository : ILibraryRepository
    {
        private readonly BXCModelEntities _context;

        public LibraryRepository(BXCModelEntities context)
        {
            _context = context;
        }

        public Library GetByID(int id)
        {
            return _context.Libraries.FirstOrDefault(l => l.Id == id);
        }

        public Library GetByNameAndOwner(string libraryName, string ownerId)
        {
            Expression<Func<Library, bool>> expr = l => l.Name == libraryName;
            return _context.Libraries.Where(expr).FirstOrDefault();
        }

        public Library AddLibrary(string libraryName)
        {
            var lib = new Library { Name = libraryName };
            _context.AddObject("Libraries", lib);
            _context.SaveChanges();
            return lib;
        }

    }
}
