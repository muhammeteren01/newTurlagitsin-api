using Core.Entities;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWork;

namespace Service.Service
{
    public class UserSavedTripService : Service<UserSavedTrip>, IUserSavedTripService
    {
        public UserSavedTripService(IGenericRepository<UserSavedTrip> repository, IUnitOfWork unitOfWork) 
            : base(repository, unitOfWork)
        {
        }
    }
}
