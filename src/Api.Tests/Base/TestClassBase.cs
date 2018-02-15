using System;
using System.Net;
using ComposerCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Fields;
using Tesseract.ApiModel.Tags;
using Tesseract.Client;
using Tesseract.Client.Utils;
using Tesseract.Common.Extensions;
using Tesseract.Common.Results;
using Tesseract.Core.Connection;
using Tesseract.Core.Queue;
using Tesseract.Core.Queue.Implementation;

namespace Appson.Tesseract.Tests.Api.Base
{
    public class TestClassBase
    {
        protected const string TestAccount = "test-account";
        protected const string TestNs = "test-ns";
        protected const string TestTag = "test-tag";
        protected const string TestField = "test-field";
        protected const string TestTenantId = null;

//        protected TestServer Server { get; private set; }
        protected TesseractClient Client { get; private set; }
        protected IComponentContext Composer { get; private set; }

        [TestInitialize]
        public void TestInit()
        {
//            Server = TestServer.Create(appBuilder =>
//            {
//                var startup = new TestStartup();
//                startup.Configuration(appBuilder);
//
//                Composer = startup.GetComposer();
//            });

//            Client = new TesseractClient(Server.HttpClient);
            
            ApplyCompositionForTest();
            CleanupAllData();
        }

        [TestCleanup]
        public void TestCleanup()
        {
//            Server.Dispose();
//            Server = null;
        }

        protected virtual void ApplyCompositionForTest()
        {
            Composer.UnregisterFamily(typeof(IJobQueue<>));
            Composer.Register(typeof(InlineJobQueue<>));
        }

        protected virtual void CleanupAllData()
        {
            // Remove any data from previous tests and recreate index
            Composer.GetComponent<IEsManager>().DeleteTenantIndex(TestTenantId).GetAwaiter().GetResult();
            Composer.GetComponent<IEsManager>().CreateTenantIndex(TestTenantId).GetAwaiter().GetResult();
            Composer.GetComponent<IMongoManager>().DeleteTenantData(TestTenantId);

            // Define required namespaces
            Client.Tags.PutTagNsDefinition(TestNs, new PutTagNsDefinitionRequest()).GetAwaiter().GetResult();
            Client.Fields.PutFieldDefinition(TestField, new PutFieldDefinitionRequest()).GetAwaiter().GetResult();
        }
        
        protected void CheckSuccess(ApiValidationResult result)
        {
            Assert.IsTrue(result.Success, result.ToDebugMessage());
        }

        protected TResult CheckSuccess<TResult>(ApiValidatedResult<TResult> result)
        {
            Assert.IsTrue(result.Success, result.ToDebugMessage());
            return result.Result;
        }

        protected void CheckFailure(ApiValidationResult result, HttpStatusCode statusCode)
        {
            Assert.IsFalse(result.Success, "Result is supposed to be an error, but request returned a success result code.");
            Assert.IsNotNull(result.Errors, "Result is supposed to contain errors, but the errors collection on the result is null.");
            Assert.IsTrue(result.Errors.Count > 0, "Result is supposed to contain errors, but the errors collection on the result is empty.");
            Assert.AreEqual(statusCode.ToString(), result.Errors[0].ErrorKey, "The returned error status is not what it is supposed to be.");
        }

        protected string BuildUniqueString()
        {
            return Guid.NewGuid().ToUrlFriendly();
        }

        protected void RefreshIndex()
        {
            var esManager = Composer.GetComponent<IEsManager>();
            esManager.Client.Refresh(esManager.GetTenantIndexName(TestTenantId));
        }
    }
}