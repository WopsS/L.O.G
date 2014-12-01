using Lidgren.Network;
using LOG.API.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.Networking.Messages
{
    public class VesselMessage : IGameMessage
    {
        public VesselMessage()
        {

        }

        public VesselMessage(NetIncomingMessage im)
        {
            DecodeMessage(im);
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public string Data { get; set; }

        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.UpdateVesseState; }
        }

        public void DecodeMessage(NetIncomingMessage Message)
        {
            ID = Message.ReadString();
            Name = Message.ReadString();
            Data = Message.ReadString();
        }

        public void EncodeMessage(NetOutgoingMessage Message)
        {
            Message.Write(ID);
            Message.Write(Name);
            Message.Write(Data);
        }
    }
}
