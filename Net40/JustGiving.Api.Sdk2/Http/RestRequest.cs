﻿using System;
using JustGiving.Api.Sdk2.Serialization;
using RestSharp;

namespace JustGiving.Api.Sdk2.Http
{
    /// <summary>
    /// Looks like we have to implement our own IRestRequest if we want control over message serialization :-/
    /// </summary>
    public class RestRequest : RestSharp.RestRequest
    {
        public RestRequest()
        {
            JsonSerializer = new JustGivingJsonSerializer();
        }

        public RestRequest(Method method) : base(method)
        {
            JsonSerializer = new JustGivingJsonSerializer();
        }

        public RestRequest(string resource) : base(resource)
        {
            JsonSerializer = new JustGivingJsonSerializer();
        }

        public RestRequest(string resource, Method method) : base(resource, method)
        {
            JsonSerializer = new JustGivingJsonSerializer();
        }

        public RestRequest(Uri resource) : base(resource)
        {
            JsonSerializer = new JustGivingJsonSerializer();
        }

        public RestRequest(Uri resource, Method method) : base(resource, method)
        {
            JsonSerializer = new JustGivingJsonSerializer();
        }
    }
}
