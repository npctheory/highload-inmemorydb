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

namespace Core.Application.Dialogs.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, DialogMessageDTO>
{
    private readonly IDialogRepository _dialogRepository;
    private readonly IMapper _mapper;

    public SendMessageCommandHandler(IDialogRepository dialogRepository, IMapper mapper)
    {
        _dialogRepository = dialogRepository;
        _mapper = mapper;
    }

    public async Task<DialogMessageDTO> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        DialogMessage message = new DialogMessage{
            SenderId = request.SenderId,
            ReceiverId = request.ReceiverId,
            Text = request.Text
        };

        DialogMessage message_sent = await _dialogRepository.SendMessage(message);
        return _mapper.Map<DialogMessageDTO>(message_sent);
    }
}