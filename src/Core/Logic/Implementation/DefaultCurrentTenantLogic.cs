﻿using System;
using System.Threading.Tasks;
using Tesseract.Core.Context;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Logic.Implementation
{
    [Component]
    public class DefaultCurrentTenantLogic : ICurrentTenantLogic
    {
        private const string TenantInfoContextKey = "fanap.identity.tenant-info";

        [ComponentPlug]
        public IComposer Composer { get; set; }

        public void InitializeInfo(IOwinRequest request)
        {
            // TODO: read tenant ID from request
            SetTenantInfo(null);
        }

        public Task PopulateInfo()
        {
            // no await, no async. Just pass the task from the inner call.
            return GetTenantInfo().Initialize(Composer);
        }

        public string Id => GetTenantInfo().Id;
        public string None => "_NONE_";

        public bool DoesTagNsExist(string ns)
        {
            return GetInitializedTenantInfo().TagNsDefinitions.ContainsKey(ns);
        }

        public TagNsDefinitionData GetTagNsDefinition(string ns)
        {
            var definitions = GetTenantInfo()?.TagNsDefinitions;
            TagNsDefinitionData result = null;
            definitions?.TryGetValue(ns, out result);
            return result;
        }

        public bool DoesFieldExist(string fieldName)
        {
            return GetInitializedTenantInfo().FieldDefinitions.ContainsKey(fieldName);
        }

        public FieldDefinitionData GetFieldDefinition(string fieldName)
        {
            var definitions = GetTenantInfo()?.FieldDefinitions;
            FieldDefinitionData result = null;
            definitions?.TryGetValue(fieldName, out result);
            return result;
        }

        #region Private helper methods

        private static TenantContextInfo GetTenantInfo()
        {
            var info = OwinRequestScopeContext.Current?.OwinContext?.Get<TenantContextInfo>(TenantInfoContextKey);
            if (info == null)
                throw new InvalidOperationException("Tenant info is not initialized on the owin context");

            return info;
        }

        private TenantContextInfo GetInitializedTenantInfo()
        {
            var info = GetTenantInfo();
            if (!info.Initialized)
                throw new InvalidOperationException("TenantContextInfo is not yet initialized / populated with the data.");

            return info;
        }

        private void SetTenantInfo(string tenantId)
        {
            OwinRequestScopeContext.Current?.OwinContext?.Set(TenantInfoContextKey, new TenantContextInfo(tenantId));
        }

        #endregion
    }
}