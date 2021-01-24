using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MVCTest.Data;
using System;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;

namespace MVCTest.Models
{
    public class LogInterpreter
    {
        public static async System.Threading.Tasks.Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var context = new MVCTestContext(
               serviceProvider.GetRequiredService<
                   DbContextOptions<MVCTestContext>>()))
            {
                //Use the default configuration for AngleSharp
                var config = Configuration.Default;

                //Create a new context for evaluating webpages with the given config
                var browsingContext = BrowsingContext.New(config);

                //Source to be parsed
                //TODO: Load from file?
                var source = "<div class=\"content\"><div class=\"message general\" data-messageid=\"-MRD0XaqBHEZnJDfSWNQ\"><div class=\"spacer\"></div><div class=\"avatar\" aria-hidden=\"true\"><img src=\"https://s3.amazonaws.com/files.d20.io/images/135342042/CbksPH8jVm0LAaCXhTxEmA/max.png?1589670299\"></div><span class=\"tstamp\" aria-hidden=\"true\">January 17, 2021 1:47AM</span><span class=\"by\">Tubrineros:</span>tubby shall fly up</div><div class=\"message general you\" data-messageid=\"-MRD0cYU_RfmkL-2B8DB\"><div class=\"spacer\"></div><div class=\"avatar\" aria-hidden=\"true\"><img src=\"https://s3.amazonaws.com/files.d20.io/images/146527777/Gb7hywPQ6VTIEPdNjdyfXA/max.png?1593285566\"></div><span class=\"tstamp\" aria-hidden=\"true\">January 17, 2021 1:47AM</span><span class=\"by\">Seifer:</span>\"Urgh...and what do you mean to do? Are you gonna kill the woman that raised us?\"</div><div class=\"message general\" data-messageid=\"-MRD0e4gNCfYGtIQCX5N\"><div class=\"spacer\"></div><div class=\"avatar\" aria-hidden=\"true\"><img src=\"https://s3.amazonaws.com/files.d20.io/images/135342042/CbksPH8jVm0LAaCXhTxEmA/max.png?1589670299\"></div><span class=\"tstamp\" aria-hidden=\"true\">January 17, 2021 1:47AM</span><span class=\"by\">Tubrineros:</span>the who know</div><div class=\"message general\" data-messageid=\"-MRD0fShNAV6vPSaML7S\"><div class=\"spacer\"></div><div class=\"avatar\" aria-hidden=\"true\"><img src=\"https://s3.amazonaws.com/files.d20.io/images/84613138/-Shy6M_ifsYrwkGEMWDstQ/max.png?1561214324\"></div><span class=\"tstamp\" aria-hidden=\"true\">January 17, 2021 1:47AM</span><span class=\"by\">Orme:</span>\"pardon?\"</div><div class=\"message general you\" data-messageid=\"-MRD0gYZr9rxJvYXGMge\"><div class=\"spacer\"></div><div class=\"avatar\" aria-hidden=\"true\"><img src=\"https://s3.amazonaws.com/files.d20.io/images/146527777/Gb7hywPQ6VTIEPdNjdyfXA/max.png?1593285566\"></div><span class=\"tstamp\" aria-hidden=\"true\">January 17, 2021 1:47AM</span><span class=\"by\">Seifer:</span>\"Heh...didn't think you were that cold-hearted, Squall.</div><div class=\"message emote\" data-messageid=\"-MRD0iOUVYTVhK5hx7vP\"><div class=\"avatar\" aria-hidden=\"true\"><img src=\"https://s3.amazonaws.com/files.d20.io/images/84615449/9pJv-nsIuya4L0tTpqjBlQ/max.png?1561216407\"></div><div class=\"spacer\"></div>Isabel nudges Orme</div><div class=\"message general you\" data-messageid=\"-MRD0jX2kmDjqOtegnrL\"><div class=\"spacer\"></div><div class=\"avatar\" aria-hidden=\"true\"><img src=\"/users/avatar/539117/30\"></div><span class=\"tstamp\" aria-hidden=\"true\">January 17, 2021 1:47AM</span><span class=\"by\">Streak (GM):</span>you might recall they all grew up in an orphanage</div></div>";

                //Create a virtual request to specify the document to load (here from our fixed string)
                var document = await browsingContext.OpenAsync(req => req.Content(source));

                //Get the first message root node.
                var messageRootNode = document.Children[0].Children[1].Children[0].Children[0];
                var numberOfMessages = document.Children[0].Children[1].Children[0].ChildElementCount;

                Campaign c = new Campaign { Name = "Testing" }; //TODO: Get the campaign from database

                Log log = new Log { Campaign = c, date = DateTime.Now, Title = "Testing Sesh" };
                Message[] messages = new Message[numberOfMessages];

                int messageCounter = 0;
                DateTime priorDateRecorded = DateTime.Now;
                //End when we reach the last message.
                while (messageRootNode != null)
                {
                    //Get the class of the root element.
                    string htmlClass = messageRootNode.ClassName;

                    //Find date in current message, and save it for the next run. If there is no date, the prior date used will be reused.
                    DateTime date = FindDate(messageRootNode, priorDateRecorded);
                    priorDateRecorded = date;

                    //Find name of the message's speaker.
                    string speaker = FindSpeaker(messageRootNode);

                    string message = FindMessage(messageRootNode);

                    //Create message and add it to collection.
                    messages[messageCounter] = new Message
                        {
                            htmlClass = htmlClass,
                            date = date,
                            speaker = speaker,
                            message = message,
                            log = log
                        };

                    messageCounter++;
                    //Move to the next message. Will be null when we've reached the end.
                    messageRootNode = messageRootNode.NextElementSibling;
                }

                //Add Log and Message entries to context and save.
                context.Campaign.AddRange(c);
                context.Log.AddRange(log);
                context.Message.AddRange(messages);
                context.SaveChanges();
            }
        }

        //Find the date in the message and turn it into a DateTime object. If date can't be found or parsed, then the backupDate given is returned.
        private static DateTime FindDate(IElement message, DateTime backupDate)
        {
            var dateNode = message.GetElementsByClassName("tstamp");
            string dateString;
            if (dateNode.Length > 0)
            {
                dateString = dateNode[0].TextContent;
            }
            else
            {
                return backupDate;
            }
            try
            {
                var date = DateTime.Parse(dateString);
                return date;
            }
            catch (Exception)
            {
                return backupDate;
            }
        }

        //Attempts to find the name of the speaker of this message.
        private static string FindSpeaker(IElement message)
        {
            var speakerNode = message.GetElementsByClassName("by");
            string speakerString;
            if (speakerNode.Length > 0)
            {
                speakerString = speakerNode[0].TextContent[0..^1];
            }
            else
            {
                //TODO: If class is emote, attempt to parse speaker from message.
                speakerString = null;
            }
            return speakerString;
        }

        //Attempts to find the spoken message. The method of obtaining this is dependant on the message's class, and if it can't find a valid class, this returns nothing.
        private static string FindMessage(IElement message)
        {
            if (message.ClassName.Contains("emote"))
            {
                return message.TextContent;
            }
            else if (message.ClassName.Contains("general"))
            {
                var dateNode = message.GetElementsByClassName("tstamp");
                var speakerNode = message.GetElementsByClassName("by");
                int dateStringLength = dateNode.Length > 0 ? dateNode[0].TextContent.Length : 0;
                int speakerStringLength = speakerNode.Length > 0 ? speakerNode[0].TextContent.Length : 0;
                int stringOffset = dateStringLength + speakerStringLength;
                return message.TextContent[stringOffset..];
            }
            else
            {
                return null;
            }
        }
    }
}
