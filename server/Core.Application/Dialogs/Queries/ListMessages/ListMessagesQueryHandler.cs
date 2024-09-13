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

namespace Core.Application.Users.Queries.ListMessages;

public class ListMessagesQueryHandler : IRequestHandler<ListMessagesQuery, List<DialogMessageDTO>>
{
    private readonly IDialogRepository _dialogRepository;
    private readonly IMapper _mapper;

    public ListMessagesQueryHandler(IDialogRepository dialogRepository, IMapper mapper)
    {
        _dialogRepository = dialogRepository;
        _mapper = mapper;
    }

    public async Task<List<DialogMessageDTO>> Handle(ListMessagesQuery request, CancellationToken cancellationToken)
    {
        List<DialogMessage> messages = await _dialogRepository.ListMessages(request.userId, request.agentId);
        return _mapper.Map<List<DialogMessageDTO>>(messages);
    }
}