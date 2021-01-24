using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MVCTest.Data;
using MVCTest.Models;
using System;
using System.Linq;

namespace MvcMovie.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MVCTestContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MVCTestContext>>()))
            {
                // Look for any movies.
                if (context.Campaign.Any())
                {
                    return;   // DB has been seeded
                }

                Campaign c = new Campaign { Name = "Test Campaign" };
                Log l1 = new Log { Campaign = c, date = DateTime.Now, Title = "Test Log 1" };
                Log l2 = new Log { Campaign = c, date = DateTime.Now, Title = "Test Log 2" };
                Message m1 = new Message { log = l1, date = DateTime.Now, speaker = "Streak", htmlClass = "general",
                                           message = "This is a test message! Wow!" };
                Message m2 = new Message { log = l1, date = DateTime.Now, speaker = "Drufle", htmlClass = "general",
                                           message = "Uh are you sure?" };
                Message m3 = new Message { log = l1, date = DateTime.Now, speaker = "Streak", htmlClass = "general",
                                           message = "Of course!!" };
                Message m4 = new Message { log = l2, date = DateTime.Now, speaker = "Man O'Barksmasher", htmlClass = "general",
                                           message = "\"Bwahaha! I've made a new canoe!\"" };
                Message m5 = new Message { log = l2, date = DateTime.Now, speaker = "Aerlin", htmlClass = "general",
                                           message = "\"Wait...where am I? Who are you?\"" };
                Message m6 = new Message { log = l2, date = DateTime.Now, speaker = "Ezra", htmlClass = "general",
                                           message = "\"I could ask the same thing...\"" };
                Message m7 = new Message { log = l2, date = DateTime.Now, speaker = "Man O'Barksmasher", htmlClass = "emote",
                                           message = "Man O'Barksmasher breaks a nearby tree with his new canoe." };


                context.Campaign.AddRange(c);
                context.Log.AddRange(l1, l2);
                context.Message.AddRange(m1, m2, m3, m4, m5, m6, m7);
                context.SaveChanges();
            }
        }
    }
}