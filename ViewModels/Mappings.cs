using Storage.Models;
using AutoMapper;

namespace Storage.ViewModels.Mappings
{
    public class ViewModelsToModelsMappingProfile : Profile
    {
        public ViewModelsToModelsMappingProfile()
        {
            CreateMap<RegistrationViewModel, AppUser>();
        }
    }
}