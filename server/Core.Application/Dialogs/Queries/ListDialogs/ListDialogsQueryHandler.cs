using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Users.Queries.SearchUsers;
using Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Core.Domain.Interfaces;
using Core.Application.Users.DTO;
using Core.Application.Dialogs.DTO;
using Core.Application.Dialogs.Queries.ListMessages;
using Core.Domain.Aggregates;
using Core.Application.Dialogs.Queries.ListDialogs;

namespace Core.Application.Users.Queries.ListDialogs;

public class ListDialogsQueryHandler : IRequestHandler<ListDialogsQuery, List<AgentDTO>>
{
    private readonly IDialogRepository _dialogRepository;
    private readonly IMapper _mapper;

    public ListDialogsQueryHandler(IDialogRepository dialogRepository, IMapper mapper)
    {
        _dialogRepository = dialogRepository;
        _mapper = mapper;
    }

    public async Task<List<AgentDTO>> Handle(ListDialogsQuery request, CancellationToken cancellationToken)
    {
        List<Dialog> agents = await _dialogRepository.ListDialogs(request.userId);
        return _mapper.Map<List<AgentDTO>>(agents);
    }
}