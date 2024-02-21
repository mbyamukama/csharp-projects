using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

namespace EntityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("inserting data");
           // InsertData();
            Console.WriteLine("reading data");
            PrintData();
        }

        private static void InsertData()
        {
            using (var context = new MessageContext())
            {
                // Creates the database if not exists
                context.Database.EnsureCreated();

                var msgCollection1 = new MessageCollection
                {
                    UserID="Maximus"
                };
                var msgCollection2 = new MessageCollection
                {
                    UserID = "Byamukama"
                };
                context.MessageCollections.Add(msgCollection1);
                context.MessageCollections.Add(msgCollection2);

                // Adds some books
                context.Messages.Add(new Message
                {
                    MsgID = 10001,
                    Text = "This is message 1",
                    DateReceived = DateTime.Now,
                    MessageCollection = msgCollection1
                }) ;
                context.Messages.Add(new Message
                {
                    MsgID = 10002,
                    Text = "This is message 2",
                    DateReceived = new DateTime(2022, 02, 01),
                    MessageCollection = msgCollection1
                }) ;

                // Saves changes
                context.SaveChanges();
            }
        }

        private static void PrintData()
        {
            // Gets and prints all books in database
            using (var context = new MessageContext())
            {
                var msgs = context.Messages.Include(p => p.MessageCollection);
                foreach (var msg in msgs)
                {
                    var data = new StringBuilder();
                    data.AppendLine($"MsgID: {msg.MsgID}");
                    data.AppendLine($"Title: {msg.Text}");
                    data.AppendLine($"User: {msg.MessageCollection.UserID}");
                    Console.WriteLine(data.ToString());
                }
            }
        }
    }
}