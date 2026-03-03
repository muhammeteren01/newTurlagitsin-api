using Core.Entities;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWork;

namespace Service.Service
{
    public class CompanyReviewService : Service<CompanyReview>, ICompanyReviewService
    {
        public CompanyReviewService(IGenericRepository<CompanyReview> repository, IUnitOfWork unitOfWork) 
            : base(repository, unitOfWork)
        {
        }
    }
}
