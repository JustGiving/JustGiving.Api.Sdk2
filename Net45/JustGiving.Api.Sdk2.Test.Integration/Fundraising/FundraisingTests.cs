﻿using System;
using System.Linq;
using System.Net;
using System.Reflection;
using JustGiving.Api.Sdk2.Model.Fundraising.Request;
using NUnit.Framework;
using RestSharp.Extensions;

namespace JustGiving.Api.Sdk2.Test.Integration.Fundraising
{
    [TestFixture]
    public class FundraisingTests
    {
        [Test]
        public async void CanCheckExistingPageName()
        {
            const string pageName = "YouCanTestPage5";
            var client = TestContext.CreateAnonymousClient();
            var response = await client.Fundraising.FundraisingPageUrlCheck(pageName);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async void CanCheckNonExistingPageName()
        {
            var pageName = Guid.NewGuid().ToString();
            var client = TestContext.CreateAnonymousClient();
            var response = await client.Fundraising.FundraisingPageUrlCheck(pageName);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async void CanSuggestNewNameWhenNameExists()
        {
            const string pageName = "YouCanTestPage5";
            var client = TestContext.CreateAnonymousClient();
            var response = await client.Fundraising.SuggestPageShortNames(pageName);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Names.Count, Is.GreaterThan(0));
            Assert.That(response.Data.Names.Any(name => name.Contains(pageName)), Is.True);
        }

        [Test]
        public async void CanSuggestNewNameWhenNameDoesNotExist()
        {
            var pageName = Guid.NewGuid().ToString();
            var client = TestContext.CreateAnonymousClient();
            var response = await client.Fundraising.SuggestPageShortNames(pageName);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Names.Count, Is.GreaterThan(0));
            Assert.That(response.Data.Names.Any(name => name == pageName), Is.True);
        }

        [Test]
        public async void CanRegisterFundraisingPageForActivity()
        {
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var requst = new FundraisingPageRegistration
            {
                ActivityType = ActivityType.Birthday,
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventName = "My Birthday",
                EventDate = DateTime.Now.AddMonths(1).Date
            };

            var response = await client.Fundraising.RegisterFundraisingPage(requst);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async void CanRegisterFundraisingPageForEvent()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var requst = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            var response = await client.Fundraising.RegisterFundraisingPage(requst);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async void CanGetFundraisingPageDetails()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var requst = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(requst);
            var response = await client.Fundraising.GetFundraisingPageDetails(pageName);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.PageShortName, Is.EqualTo(pageName));
        }

        [Test]
        public async void CanGetFundraisingPagesForUser()
        {
            const int eventId = 756550;
            var pageName1 = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var request = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName1,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(request);
            var pageName2 = "Sdk2-test-" + Guid.NewGuid().ToString("N");

            request = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName2,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(request);

            var response = await client.Fundraising.GetFundraisingPages();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Count, Is.EqualTo(2));
            Assert.That(response.Data.Any(frp => frp.PageShortName == pageName1));
            Assert.That(response.Data.Any(frp => frp.PageShortName == pageName2));
        }

        [Test]
        public async void CanGetDonationsForPage()
        {
            var client = TestContext.CreateAnonymousClient();
            var response = await client.Fundraising.GetFundraisingPageDonations(TestContext.PageWithDonations);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Donations.Count, Is.GreaterThan(0));
            Assert.That(response.Data.Pagination.TotalResults, Is.GreaterThan(0));
        }

        [Test]
        public async void CanUpdateFundraisingPage()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var storySupplement = "Hello, world " + DateTime.Now;
            var update = new FundraisingPageUpdate() {StorySupplement = storySupplement};
            var response = await client.Fundraising.UpdateFundraisingPage(pageName, update);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async void CanPostMicroblogUpdates()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var update = new Model.Fundraising.Request.MicroblogUpdate()
            {
                CreatedDate = DateTime.Now,
                Message = "Hello world " + DateTime.Now
            };

            var response = await client.Fundraising.PageUpdatesAddPost(pageName, update);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async void CanGetMicroblogUpdates()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var update = new Model.Fundraising.Request.MicroblogUpdate()
            {
                CreatedDate = DateTime.Now,
                Message = "Hello world " + DateTime.Now
            };

            await client.Fundraising.PageUpdatesAddPost(pageName, update);

            var response = await client.Fundraising.PageUpdates(pageName);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Count, Is.EqualTo(1));
        }

        [Test]
        public async void CanGetMicroblogUpdatesById()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var update = new Model.Fundraising.Request.MicroblogUpdate()
            {
                CreatedDate = DateTime.Now,
                Message = "Hello world " + DateTime.Now
            };

            await client.Fundraising.PageUpdatesAddPost(pageName, update);

            var getAllResponse = await client.Fundraising.PageUpdates(pageName);
            var postId = getAllResponse.Data.First().Id;

            var response = await client.Fundraising.PageUpdateById(pageName, postId);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Id, Is.EqualTo(postId));
        }

        [Test]
        public async void CanDeleteMicroblogUpdatesById()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var update = new Model.Fundraising.Request.MicroblogUpdate()
            {
                CreatedDate = DateTime.Now,
                Message = "Hello world " + DateTime.Now
            };

            await client.Fundraising.PageUpdatesAddPost(pageName, update);

            var getAllResponse = await client.Fundraising.PageUpdates(pageName);
            var postId = getAllResponse.Data.First().Id;

            var deleteResponse = await client.Fundraising.DeleteFundraisingPageUpdate(pageName, postId);
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // TODO: Fix bug in API (error 500 instead of 404)
            /*
            var response = await client.Fundraising.PageUpdateById(pageName, postId);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Data.Id, Is.EqualTo(postId));
            */
        }

        [Test]
        public async void CanAppendFundraisingPageAttribution()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var attrib = new FundraisingPageAttribution()
            {
                Attribution = Guid.NewGuid().ToString()
            };

            var response = await client.Fundraising.AppendToFundraisingPageAttribution(pageName, attrib);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async void CanReplaceFundraisingPageAttribution()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var attrib = new FundraisingPageAttribution()
            {
                Attribution = Guid.NewGuid().ToString()
            };

            var response = await client.Fundraising.UpdateFundraisingPageAttribution(pageName, attrib);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async void CanGetFundraisingPageAttribution()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var attrib = new FundraisingPageAttribution()
            {
                Attribution = Guid.NewGuid().ToString()
            };

            await client.Fundraising.UpdateFundraisingPageAttribution(pageName, attrib);

            var response = await client.Fundraising.GetFundraisingPageAttribution(pageName);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Attribution, Is.EqualTo(attrib.Attribution));
        }

        [Test]
        public async void CanDeleteFundraisingPageAttribution()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var attrib = new FundraisingPageAttribution()
            {
                Attribution = Guid.NewGuid().ToString()
            };

            await client.Fundraising.UpdateFundraisingPageAttribution(pageName, attrib);
            await client.Fundraising.DeleteFundraisingPageAttribution(pageName);

            var response = await client.Fundraising.GetFundraisingPageAttribution(pageName);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Attribution, Is.Not.EqualTo(attrib.Attribution));
        }

        [Test]
        public async void CanUploadImage()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var image = GetEmbeddedTestImage();
            var caption = "Test caption " + Guid.NewGuid();
            var result = await client.Fundraising.UploadImage(pageName, image, "image/png", caption);
            var frpInfo = await client.Fundraising.GetFundraisingPageDetails(pageName);
            
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(frpInfo.Data.Media.Images.Any(im => im.Caption == caption));
        }

        [Test]
        public async void CanUploadDefaultImage()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            var image = GetEmbeddedTestImage();
            var caption = "Test caption " + Guid.NewGuid();
            var result = await client.Fundraising.UploadDefaultImage(pageName, image, "image/png", caption);
            var frpInfo = await client.Fundraising.GetFundraisingPageDetails(pageName);

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(frpInfo.Data.Image.Caption, Is.EqualTo(caption));
        }

        [Test]
        public void CanLoadEmbeddedTestImage()
        {
            var image = GetEmbeddedTestImage();
            Assert.That(image, Is.Not.Null);
            Assert.That(image.Length, Is.GreaterThan(0));
        }

        [Test]
        public async void CanAddImageToFundraisingPage()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            const string imageUrl = "https://images.justgiving.com/image/b02dcadb-2897-4083-855d-c43b4fa18256.jpg?template=size200x200";
            var caption = "Test caption " + Guid.NewGuid();
            var result = await client.Fundraising.AddImageToFundraisingPage(pageName, new AddImageToFundraisingPageRequest{Caption = caption, IsDefault=false, Url = imageUrl});
            var frpInfo = await client.Fundraising.GetFundraisingPageDetails(pageName);
            
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(frpInfo.Data.Media.Images.Any(im => im.Caption == caption));
        }

        [Test]
        public async void CanAddDefaultImageToFundraisingPage()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            const string imageUrl = "https://images.justgiving.com/image/b02dcadb-2897-4083-855d-c43b4fa18256.jpg?template=size200x200";
            var caption = "Test caption " + Guid.NewGuid();
            var result = await client.Fundraising.AddImageToFundraisingPage(pageName, new AddImageToFundraisingPageRequest { Caption = caption, IsDefault = true, Url = imageUrl });
            var frpInfo = await client.Fundraising.GetFundraisingPageDetails(pageName);

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(frpInfo.Data.Image.Caption, Is.EqualTo(caption));
        }

        [Test]
        public async void CanDeleteFundraisingPageImage()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            const string imageUrl = "https://images.justgiving.com/image/b02dcadb-2897-4083-855d-c43b4fa18256.jpg?template=size200x200";
            var caption = "Test caption " + Guid.NewGuid();
            await client.Fundraising.AddImageToFundraisingPage(pageName, new AddImageToFundraisingPageRequest { Caption = caption, IsDefault = false, Url = imageUrl });
            var frpInfo = await client.Fundraising.GetFundraisingPageDetails(pageName);
            var imageUri = new Uri(frpInfo.Data.Media.Images.First(im => im.Caption == caption).Url);
            var imageName = imageUri.Segments[imageUri.Segments.Length - 1];
            var response = await client.Fundraising.DeleteFundraisingPageImage(pageName, imageName, false);
            frpInfo = await client.Fundraising.GetFundraisingPageDetails(pageName);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(frpInfo.Data.Media.Images.Any(im => im.Caption == caption), Is.False);
        }

        [Test]
        public async void CanDeleteFundraisingPageStockImage()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);
            var frpInfo = await client.Fundraising.GetFundraisingPageDetails(pageName);
            var otherCaption = "Test caption " + Guid.NewGuid();
            const string otherImageUrl = "https://images.justgiving.com/image/b02dcadb-2897-4083-855d-c43b4fa18256.jpg?template=size200x200";
            await client.Fundraising.AddImageToFundraisingPage(pageName, new AddImageToFundraisingPageRequest { Caption = otherCaption, IsDefault = false, Url = otherImageUrl });

            var caption = frpInfo.Data.Image.Caption;
            var imageUri = new Uri(frpInfo.Data.Image.Url);
            var imageName = imageUri.Segments[imageUri.Segments.Length - 1];
            var response = await client.Fundraising.DeleteFundraisingPageImage(pageName, imageName, true);
            frpInfo = await client.Fundraising.GetFundraisingPageDetails(pageName);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(frpInfo.Data.Media.Images.Any(im => im.Caption == caption), Is.False);
        }

        [Test]
        public async void CanGetFundraisingPageImages()
        {
            const int eventId = 756550;
            var pageName = "Sdk2-test-" + Guid.NewGuid().ToString("N");
            var client = await TestContext.CreateBasicAuthClientAndUser();
            var frpRequest = new FundraisingPageRegistration
            {
                CharityId = TestContext.DemoCharityId,
                PageShortName = pageName,
                PageTitle = "Sdk2 Test Page",
                EventId = eventId
            };

            await client.Fundraising.RegisterFundraisingPage(frpRequest);

            const string imageUrl = "https://images.justgiving.com/image/b02dcadb-2897-4083-855d-c43b4fa18256.jpg?template=size200x200";
            var caption = "Test caption " + Guid.NewGuid();
            await client.Fundraising.AddImageToFundraisingPage(pageName, new AddImageToFundraisingPageRequest { Caption = caption, IsDefault = false, Url = imageUrl });
            var result = await client.Fundraising.GetImagesForFundraisingPage(pageName);

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Data.Any(im => im.Caption == caption));
            Assert.That(result.Data.Count, Is.EqualTo(2));
        }


        private byte[] GetEmbeddedTestImage()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("JustGiving.Api.Sdk2.Test.Integration.Resources.slim-it-cube.png").ReadAsBytes();
        }
    }    

}
