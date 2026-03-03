using Core.Entities;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWork;

namespace Service.Service
{
    public class TripGalleryService : Service<TripGallery>, ITripGalleryService
    {
        public TripGalleryService(IGenericRepository<TripGallery> repository, IUnitOfWork unitOfWork) 
            : base(repository, unitOfWork)
        {
        }
    }
}
