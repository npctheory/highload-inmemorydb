using AutoMapper;
using Core.Domain.Entities;
using Core.Infrastructure.Snapshots;

namespace Core.Infrastructure.Mapping
{
    public class InfrastructureProfile : Profile
    {
        public InfrastructureProfile()
        {
            CreateMap<UserSnapshot, User>();
            CreateMap<FriendshipSnapshot, Friendship>();
            CreateMap<PostSnapshot, Post>();
        }
    }
}