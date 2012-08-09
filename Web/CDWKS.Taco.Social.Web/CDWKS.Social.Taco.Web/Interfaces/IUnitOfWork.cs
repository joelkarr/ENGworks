
using CDWKS.Social.Taco.Models;

namespace CDWKS.Social.Taco.Interfaces
{
    /// <summary>
    /// Generic interface that can be used across all unit of works.
    /// </summary>
    public interface IUnitOfWork
    {
        IRepository<SocialFeedbackForm> SocialFeedbackFormRespository { get; }

        void Save();
        void CloseConnection();
    }
}
