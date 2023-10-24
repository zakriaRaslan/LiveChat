

using Api.Dtos;
using Api.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private readonly ChatService _chatService;

    public ChatHub(ChatService chatService)
    {
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "OnlineChat");
        await Clients.Caller.SendAsync("UserConnected");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "OnlineChat");
        var user = _chatService.GetUserByConnectionId(Context.ConnectionId);
        _chatService.RemoveUserFormList(user);
        await DisplayOnlineUsers();

        await base.OnDisconnectedAsync(exception);
    }

    public async Task AddUserConnectionId(string name)
    {
        _chatService.AddUserConnectionId(name, Context.ConnectionId);
        await DisplayOnlineUsers();
    }

    public async Task ReceiveMessage(MessageDto message)
    {
        await Clients.Group("OnlineChat").SendAsync("NewMessage", message);
    }

    public async Task CreatePrivateChat(MessageDto message)
    {
        string PrivateGroupName = GetPrivateGroupName(message.From, message.To);
        await Groups.AddToGroupAsync(Context.ConnectionId, PrivateGroupName);
        var toConnectionId = _chatService.GetConnectionIdByUser(message.To);
        await Groups.AddToGroupAsync(toConnectionId, PrivateGroupName);
        //opening private chatbox from the other end user
        await Clients.Client(toConnectionId).SendAsync("OpenPrivateChat", message);
    }

    public async Task ReceivePrivateMessage(MessageDto message)
    {
        string PrivateGroupName = GetPrivateGroupName(message.From, message.To);
        await Clients.Group(PrivateGroupName).SendAsync("NewPrivateMessage", message);
    }

    public async Task RemovePrivateChat(string from, string to)
    {
        string PrivateGroupName = GetPrivateGroupName(from, to);
        await Clients.Group(PrivateGroupName).SendAsync("ClosePrivateChat");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, PrivateGroupName);
        var toConnectionId = _chatService.GetConnectionIdByUser(to);
        await Groups.RemoveFromGroupAsync(toConnectionId, PrivateGroupName);

    }
    private async Task DisplayOnlineUsers()
    {
        var onlineUsers = _chatService.GetOnlineUsers();
        await Clients.Groups("OnlineChat").SendAsync("onlineUsers", onlineUsers);
    }

    private string GetPrivateGroupName(string from, string to)
    {
        // from: name1, to: name2 => this is the group name "name1-name2"

        var stringCompare = String.CompareOrdinal(from, to) < 0;
        return stringCompare ? $"{from}-{to}" : $"{to}-{from}";
    }
}

