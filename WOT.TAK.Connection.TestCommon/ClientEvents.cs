using System.Globalization;
using System.Xml;
using WOT.TAK.Connection.DTOs;

namespace WOT.TAK.Connection.TestCommon;

public static class ClientEvents {

    private const decimal Latitude = 40.2100278338638m;
    private const decimal Longitude = -92.6243997054913m;
    private const decimal HeightAboveEllipsoid = 246.411508368538m;

    public static Event NewContactAnnouncement(Uid uid, string callsign)
    {
        var now = DateTime.UtcNow;
        var stale = now.AddMinutes(5);

        return new Event
        {
            Access = "Undefined",
            How = "h-e",
            Stale = stale,
            Start = now,
            Time = now,
            Type = "a-f-G-U-C-I",
            Uid = uid.AsString(),
            Version = 2,
            Point = new Point
            {
                Ce = 99.0m,
                Hae = HeightAboveEllipsoid,
                Lat = Latitude,
                Le = 99.0m,
                Lon = Longitude
            },
            Detail = new Detail
            {
                Group = new Group
                {
                    Name = "Cyan",
                    Role = "Team Member"
                },
                Contact = new Contact
                {
                    Callsign = callsign,
                    Endpoint = "*:-1:stcp"
                },
                Status = new Status
                {
                    Battery = "98"
                },
                Precisionlocation = new Precisionlocation
                {
                    Altsrc = "???",
                    Geopointsrc = "???"
                },
                Takv = new Takv
                {
                    Device = "HP HP ZBook 17 G6",
                    Os = "Microsoft Windows 10 Pro",
                    Platform = "TAKconn-java",
                    Version = "1.0"
                },
                Track = new Track
                {
                    Course = 0,
                    Speed = 0
                },
                Uid = new DTOs.Uid
                {
                    Droid = "TAKconn-java"
                },
                Usericon = new Usericon
                {
                    Iconsetpath = "COT_MAPPING_2525B/a-n/a-n-G"
                }
            }
        };
    }

    public static Event ChatPost(Uid uid, string callsign, string chatMessage)
    {
        var now = DateTime.UtcNow;
        var stale = now.AddMinutes(5);

        const string chatroomName = "All Chat Rooms";
        var messageId = Uid.Random().ToString();
        var geoChatId = $"GeoChat.{uid}.{chatroomName}.{messageId}";
        var remarkSource = $"BAO.F.WinTAK.{uid}";

        var chatPost = new Event
        {
            Access = "Undefined",
            How = "h-g-i-g-o",
            Stale = stale,
            Start = now,
            Time = now,
            Type = "b-t-f",
            Uid = geoChatId,
            Version = 2,
            Point = new Point
            {
                Ce = 99.0m,
                Hae = HeightAboveEllipsoid,
                Lat = Latitude,
                Le = 99.0m,
                Lon = Longitude
            },
            Detail = new Detail
            {
                Chat = new Chat
                {
                    Chatroom = chatroomName,
                    GroupOwner = false,
                    Id = chatroomName,
                    MessageId = messageId,
                    SenderCallsign = callsign,
                    Chatgrp = new Chatgrp
                    {
                        Id = chatroomName,
                        Uid0 = uid.ToString(),
                        Uid1 = chatroomName
                    }
                },
                Link = new Link
                {
                    Relation = "p-p",
                    Type = "a-f-G-U-C-I",
                    Uid = uid.ToString(),
                },
                Remarks = new Remarks
                {
                    Source = remarkSource,
                    SourceId = uid.ToString(),
                    Time = now,
                    To = chatroomName,
                    Text = [chatMessage]
                },
            }
        };

        var flowTags = new FlowTags();
        var xmlDoc = new XmlDocument();
        var attribute = xmlDoc.CreateAttribute($"TAK-Server-{Guid.NewGuid()}");
        attribute.Value = now.ToString("o", CultureInfo.InvariantCulture);
        flowTags.AnyAttribute.Add(attribute);

        chatPost.Detail.FlowTags = flowTags;

        return chatPost;
    }
}