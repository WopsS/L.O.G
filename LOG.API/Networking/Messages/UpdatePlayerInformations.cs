using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.Networking.Messages
{
    public class UpdatePlayerInformations : IGameMessage
    {
        public UpdatePlayerInformations()
        {

        }

        public UpdatePlayerInformations(string Message)
        {
            this.Message = Message;
        }

        public UpdatePlayerInformations(NetIncomingMessage Message)
        {
            DecodeMessage(Message);
        }

        public string Message { get; set; }

        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.UpdatePlayer; }
        }

        public void DecodeMessage(NetIncomingMessage Message)
        {
            this.Message = Message.ReadString();
        }

        public void EncodeMessage(NetOutgoingMessage Message)
        {
            Message.Write(this.Message);
        }
    }
}
