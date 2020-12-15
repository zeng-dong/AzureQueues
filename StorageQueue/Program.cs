using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;

namespace StorageQueue
{
    class Program
    {
        readonly string ConnectionString = "replace me";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new Program().InsertMessage("myfirstq", "my first message");
        }


        private QueueClient CreateQueueClient(string queueName)
        {
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(ConnectionString, queueName);
            return queueClient;
        }

        private bool CreateQueue(string queueName)
        {
            try
            {
                QueueClient queueClient = new QueueClient(ConnectionString, queueName);

                // Create the queue
                queueClient.CreateIfNotExists();

                if (queueClient.Exists())
                {
                    Console.WriteLine($"Queue created: '{queueClient.Name}'");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n\n");
                Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
                return false;
            }
        }

        private void InsertMessage(string queueName, string message)
        {
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(ConnectionString, queueName);

            // Create the queue if it doesn't already exist
            queueClient.CreateIfNotExists();

            if (queueClient.Exists())
            {
                // Send a message to the queue
                queueClient.SendMessage(message);
            }

            Console.WriteLine($"Inserted: {message}");
        }

        //-------------------------------------------------
        // Peek at a message in the queue
        //-------------------------------------------------
        public void PeekMessage(string queueName)
        {
            // Instantiate a QueueClient which will be used to manipulate the queue
            QueueClient queueClient = new QueueClient(ConnectionString, queueName);

            if (queueClient.Exists())
            {
                // Peek at the next message
                PeekedMessage[] peekedMessage = queueClient.PeekMessages();

                // Display the message
                Console.WriteLine($"Peeked message: '{peekedMessage[0].MessageText}'");
            }
        }

        //-------------------------------------------------
        // Update an existing message in the queue
        //-------------------------------------------------
        public void UpdateMessage(string queueName)
        {
            // Instantiate a QueueClient which will be used to manipulate the queue
            QueueClient queueClient = new QueueClient(ConnectionString, queueName);

            if (queueClient.Exists())
            {
                // Get the message from the queue
                QueueMessage[] message = queueClient.ReceiveMessages();

                // Update the message contents
                queueClient.UpdateMessage(message[0].MessageId,
                        message[0].PopReceipt,
                        "Updated contents",
                        TimeSpan.FromSeconds(60.0)  // Make it invisible for another 60 seconds
                    );
            }
        }

        //-------------------------------------------------
        // Process and remove a message from the queue
        //-------------------------------------------------
        public void DequeueMessage(string queueName)
        {

            // Instantiate a QueueClient which will be used to manipulate the queue
            QueueClient queueClient = new QueueClient(ConnectionString, queueName);

            if (queueClient.Exists())
            {
                // Get the next message
                QueueMessage[] retrievedMessage = queueClient.ReceiveMessages();

                // Process (i.e. print) the message in less than 30 seconds
                Console.WriteLine($"Dequeued message: '{retrievedMessage[0].MessageText}'");

                // Delete the message
                queueClient.DeleteMessage(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
            }
        }
    }


}
