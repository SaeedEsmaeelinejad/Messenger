using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FarinTalkApi.Models;
using Microsoft.AspNet.SignalR;

namespace FarinTalkApi.Hubs
{
    public class ServerHub : Hub
    {
        public void SendMessage(string fromUser, string toUser, string messageText, string messageId)
        {
            fromUser = Utility.GetClearPhoneNumber(fromUser);
            toUser = Utility.GetClearPhoneNumber(toUser);
            var connectionId = Context.ConnectionId;
            var dt = DateTime.Now;

            Clients.Client(connectionId).confirmGetMsgFromServer(toUser, dt, messageId);

            DbContext.Messages.Add(new Message() { CreationDate = dt, FromUser = fromUser, ToUser = toUser, MessageId = messageId, MessageText = messageText });
            var userConnectionIds = DbContext.UserConnections.Where(x => (x.UserName == toUser || x.UserName == fromUser) && x.ConnectionId != connectionId).ToList();

            foreach (var item in userConnectionIds)
            {
                Clients.Client(item.ConnectionId).recieveMessage(fromUser, messageText, dt, messageId);
            }

        }

        public void SendMessageToAll(string fromUser, string messageText, string messageId)
        {
            Clients.All.recieveMessage(fromUser, messageText, messageId);
        }
        public void ConfirmGetMessageFromClient(string fromUser, string toUser, string messageId)
        {
            fromUser = Utility.GetClearPhoneNumber(fromUser);
            toUser = Utility.GetClearPhoneNumber(toUser);
            var msgList = DbContext.Messages.Where(x =>
                x.ToUser == fromUser && x.FromUser == toUser && x.MessageId == messageId).ToList();
            msgList.ForEach(x =>
            {
                x.Delivered = true;
                DbContext.UserConnections.Where(z => z.UserName == toUser).ToList().ForEach(y =>
                    {
                        DbContext.ConfirmReadMessages.Add(new ConfirmReadMessage(){ FromUser = toUser, ToUser = fromUser, MessageId = messageId,CreationDate = DateTime.Now});
                        Clients.Client(y.ConnectionId).confirmGetMsgByOtherClient(fromUser, DateTime.Now, messageId);
                    });
            });

        }
        public void ConfirmGetAckMessageFromClient(string fromUser, string toUser, string messageId)
        {
            fromUser = Utility.GetClearPhoneNumber(fromUser);
            toUser = Utility.GetClearPhoneNumber(toUser);
            var msgList = DbContext.Messages.Where(x =>
                x.ToUser == toUser && x.FromUser == fromUser && x.MessageId == messageId).ToList();
            msgList.ForEach(x => { x.ConfirmGetMessageByServer = true; x.GetConfirmServerDateTime = DateTime.Now; });

        }
        public void ConfirmGetReadMessageFromClient(string fromUser, string toUser, string messageId)
        {
            fromUser = Utility.GetClearPhoneNumber(fromUser);
            toUser = Utility.GetClearPhoneNumber(toUser);
            var msgList = DbContext.ConfirmReadMessages.Where(x =>
                x.ToUser == toUser && x.FromUser == fromUser && x.MessageId == messageId).ToList();
            foreach (var confirmReadMessage in msgList)
            {
                DbContext.ConfirmReadMessages.Remove(confirmReadMessage);
            }
        }
        public void GetUnReadMessage(string userName)
        {
            userName = Utility.GetClearPhoneNumber(userName);
            var unreadMessage = DbContext.Messages.Where(x => x.ToUser == userName && x.Delivered == false).ToList();
            foreach (var item in unreadMessage)
            {
                var userConnectionIds = DbContext.UserConnections.Where(x => x.UserName == userName).ToList();
                foreach (var con in userConnectionIds)
                { 
                    Clients.Client(con.ConnectionId).recieveMessage(item.FromUser, item.MessageText, item.CreationDate, item.MessageId);
                }
            }

            var confirmGetMessages = DbContext.ConfirmReadMessages.Where(x => x.FromUser == userName).ToList();
            foreach (var confirmReadMessage in confirmGetMessages)
            {
                var userConnectionIds = DbContext.UserConnections.Where(x => x.UserName == userName).ToList();
                foreach (var con in userConnectionIds)
                {
                    Clients.Client(con.ConnectionId).confirmGetMsgByOtherClient(confirmReadMessage.ToUser, confirmReadMessage.CreationDate, confirmReadMessage.MessageId);
                }
            }
        }
        public override async Task OnConnected()
        {
            await base.OnConnected();
        }


        public override Task OnDisconnected(bool stopCalled)
        {

            RemoveConnection(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        private void RemoveConnection(string connectionId)
        {

            var list = DbContext.UserConnections.Where(x => x.ConnectionId == connectionId).ToList();
            foreach (var item in list)
            {
                DbContext.UserConnections.Remove(item);
                if (DbContext.UserConnections.All(x => x.UserName != item.UserName))
                {
                    DbContext.Users.Where(x => x.UserName == item.UserName).ToList().ForEach(x =>
                    {
                        x.IsOnline = false;
                        x.LastStatus=DateTime.Now;
                    });
                }
            }

        }


    }
}