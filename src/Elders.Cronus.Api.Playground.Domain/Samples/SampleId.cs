﻿using System;
using System.Runtime.Serialization;

namespace Elders.Cronus.Api.Playground.Domain.Samples
{
    [DataContract(Name = "b16a81fb-9b3f-4225-bdee-aa14d6a725f8")]
    public class SampleId : CronusApiId<SampleId>
    {
        protected SampleId() { }

        public SampleId(string id, string tenant) : base(id, "sample", tenant) { }

        public SampleId(string tenant) : this(Guid.NewGuid().ToString(), tenant) { }

        protected override SampleId Construct(string id, string tenant)
        {
            return new SampleId(id, tenant);
        }
    }

    public abstract class CronusApiId<T> : StringTenantId
        where T : CronusApiId<T>
    {
        static UberUrnFormatProvider urnFormatProvider = new UberUrnFormatProvider();

        protected CronusApiId() { }

        protected CronusApiId(string id, string rootName, string tenant) : base(id, rootName, tenant) { }

        public static T Parse(string id)
        {
            var instance = (T)Activator.CreateInstance(typeof(T), true);

            var stringTenantUrn = StringTenantUrn.Parse(id, urnFormatProvider);
            var newId = instance.Construct(stringTenantUrn.Id, stringTenantUrn.Tenant);
            if (stringTenantUrn.ArName == newId.AggregateRootName)
                return newId;
            else
                throw new Exception("bum");
            //todo check if ar name mateches..
        }

        public static bool TryParse(string id, out T result)
        {
            try
            {
                var instance = (T)Activator.CreateInstance(typeof(T), true);

                var stringTenantUrn = StringTenantUrn.Parse(id, urnFormatProvider);
                var newId = instance.Construct(stringTenantUrn.Id, stringTenantUrn.Tenant);
                if (stringTenantUrn.ArName == newId.AggregateRootName)
                    result = newId;
                else
                    throw new Exception("bum");
                //todo check if ar name mateches..

                return true;
            }
            catch (Exception ex)
            {
                result = null;
                return false;
            }
        }

        public static T New(string tenant)
        {
            var instance = (T)Activator.CreateInstance(typeof(T), true);
            return instance.Construct(Guid.NewGuid().ToString(), tenant);
        }

        public static T New(string tenant, string id)
        {
            var instance = (T)Activator.CreateInstance(typeof(T), true);
            return instance.Construct(id, tenant);
        }

        protected abstract T Construct(string id, string tenant);

        public override string ToString()
        {
            return urnFormatProvider.Format(Urn);
        }
    }
}
