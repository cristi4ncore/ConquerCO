// * This Project Created by Mostafa Desha
// * Copyright MostafaDesha © 2025


namespace AccServer.Interfaces
{
    public unsafe interface IPacket
    {
        byte[] ToArray();
        void Deserialize(byte[] buffer);
    }
}