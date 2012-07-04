using SignalR.Hubs;

namespace Tarneeb.Hubs
{
    public class Activity : Hub
    {
        public void Create(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
            Clients.broadcast(new { message = string.Format("Group {0} created.", groupName) });
        }

        public void Join(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
            Caller.broadcast(new { message = string.Format("you have joined group {0}.", groupName) });
            Clients[groupName].broadcast(new { message = string.Format("User {0} joined the group.", Context.ConnectionId) });
        }
    }
}