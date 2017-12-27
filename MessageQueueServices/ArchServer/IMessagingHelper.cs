using System;
using System.Collections.Generic;
using System.Messaging;
using ServiceHelper;

namespace ArchServer
{
    public interface IMessagingHelper
    {
        MessageQueue GetQueue(string queueName);
        List<SequanceMessage> CreateBatchFileMessages(List<byte[]> listBytes, string fileName, Guid clientId);
        void SendStatus(MessageQueue queue, string status, Guid clientId);
        void SendSettings(MessageQueue queue, int settingValue, Guid clientId);
        void SendMessagesUsingTransactions(MessageQueue queue, List<SequanceMessage> meassges);
        List<SequanceMessage> ReceiveMessagesUsingEnumerator(MessageQueue queue);
        List<SequanceMessage> ReceiveMessagesUsingPeek(MessageQueue queue, Guid clientId);
    }
}
