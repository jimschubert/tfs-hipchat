﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Xml.Serialization;
using System.IO;
using System.Collections;

namespace TfsHipChat.Tests
{
    public class CheckinEventServiceTests
    {
        [Fact]
        public void Notify_ShouldThrowException_WhenInvalidXmlData()
        {
            var notifier = Substitute.For<INotifier>();
            var checkinEventService = new CheckinEventService(notifier);
            const string eventXml = "invalid_xml";
            const string tfsIdentityXml = "invalid_xml";

            Assert.Throws<InvalidOperationException>(() => {
                checkinEventService.Notify(eventXml, tfsIdentityXml);
            });
        }

        [Fact]
        public void Notify_ShouldSendCheckinNotification_WhenValidCheckinEvent()
        {
            var notifier = Substitute.For<INotifier>();
            var checkinEventService = new CheckinEventService(notifier);
            string eventXml = GenerateValidCheckinEvent();
            const string tfsIdentityXml = "";

            checkinEventService.Notify(eventXml, tfsIdentityXml);

            notifier.ReceivedWithAnyArgs().SendCheckinNotification(null);
        }

        private string GenerateValidCheckinEvent()
        {
            var checkinEvent = new CheckinEvent(1000, new DateTime(), "owner", "commiter", "some comment");
            checkinEvent.Artifacts = new ArrayList();  // serialization fails without this

            var serializer = new XmlSerializer(typeof(CheckinEvent));
            var sw = new StringWriter();
            serializer.Serialize(sw, checkinEvent);

            return sw.ToString();
        }
    }
}