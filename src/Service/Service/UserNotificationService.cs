using Core.Entities;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWork;

namespace Service.Service
{
    public class UserNotificationService : Service<UserNotification>, IUserNotificationService
    {
        public UserNotificationService(IGenericRepository<UserNotification> repository, IUnitOfWork unitOfWork) 
            : base(repository, unitOfWork)
        {
        }
    }
}
