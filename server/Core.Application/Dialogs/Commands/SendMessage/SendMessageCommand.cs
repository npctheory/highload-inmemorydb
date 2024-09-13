using Core.Application.Dialogs.DTO;
using MediatR;
using System;

namespace Core.Application.Dialogs.Commands.SendMessage;

public record SendMessageCommand(string SenderId, string ReceiverId, string Text) : IRequest<DialogMessageDTO>;