﻿using System;
using System.Net.Http;
using JustGiving.Api.Sdk2.Clients.Account;
using JustGiving.Api.Sdk2.Clients.Charity;
using JustGiving.Api.Sdk2.Clients.Countries;
using JustGiving.Api.Sdk2.Clients.Currency;
using JustGiving.Api.Sdk2.Clients.Donation;
using JustGiving.Api.Sdk2.Clients.Event;
using JustGiving.Api.Sdk2.Clients.Fundraising;
using JustGiving.Api.Sdk2.Clients.OneSearch;
using JustGiving.Api.Sdk2.Configuration;
using JustGiving.Api.Sdk2.Configuration.Logging;
using JustGiving.Api.Sdk2.Http;
using JustGiving.Api.Sdk2.Logging;
using JustGiving.Api.Sdk2.Security;
using JustGiving.Api.Sdk2.Security.Basic;
using JustGiving.Api.Sdk2.Security.OAuth;
using RestSharp;

namespace JustGiving.Api.Sdk2
{
    /// <summary>
    /// Main entry point into the SDK. Instantiate this class using one of the constructors to begin using the JustGiving API.
    /// </summary>
    public class JustGivingApiClient2
    {
        private AccountClient _accounts;
        private CharityClient _charities;
        private CountryClient _countries;
        private CurrencyClient _currency;
        private DonationClient _donation;
        private FundraisingClient _fundraising;
        private EventClient _event;
        private OneSearchClient _oneSearch;
        private IRestClient _restClient;
        private HttpClient _httpClient;
        private ApiRequestLogger _logger;
        private readonly ClientOptions _options;
        private ILogProvider _logProvider;

        /// <summary>
        /// Creates an Api client which uses an OAuth2 access token for authorization. This is the preferred method of authenticating, as it offers the best security and will work with all JustGiving user accounts.
        /// </summary>
        /// <param name="appId">The Application ID you received when you registered your app on the JustGiving developer portal.</param>
        /// <param name="applicationKey">The Application Key you created for your app on the JustGiving developer portal. You must include it here for your app to work.</param>
        /// <param name="oauthAccessToken">A valid OAuth2 access token issued by identity.justgiving.com</param>
        public JustGivingApiClient2(string appId, string applicationKey, OAuthAccessToken oauthAccessToken)
        {
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentException("You must provide an appId");
            }

            if (string.IsNullOrWhiteSpace(applicationKey))
            {
                throw new ArgumentException("You must provide a Application Key when using OAuth. To generate an Application Key for your app, log in to the JustGiving developer portal.");
            }

            _options = new ClientOptions(appId, applicationKey)
            {
                AuthorizationMode = AuthorizationMode.OAuth,
                Endpoint = Endpoints.Production,
                OAuthAccessToken = oauthAccessToken,
                LoggingOptions = LoggingOptions.Default
            };
        }

        /// <summary>
        /// Creates an Api client which uses a username and password for Basic authorization. This method carries security risks and will not be available to JustGiving users with different user account types (such as Facebook).
        /// </summary>
        /// <param name="appId">The Application ID you received when you registered your app on the JustGiving developer portal.</param>
        /// <param name="basicAuthCredential">The username and password of a registered JustGiving user.</param>
        public JustGivingApiClient2(string appId, BasicCredential basicAuthCredential)
        {
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentException("You must provide an appId");
            }

            if (basicAuthCredential == null)
            {
                throw new ArgumentException("You must provide a credential");
            }

            _options = new ClientOptions(appId)
            {
                AuthorizationMode = AuthorizationMode.Basic,
                Endpoint = Endpoints.Production,
                BasicAuthSettings = basicAuthCredential,
                LoggingOptions = LoggingOptions.Default
            };
        }

        /// <summary>
        /// Creates an Api client which uses a username and password for Basic authorization. This method carries security risks and will not be available to JustGiving users with different user account types (such as Facebook).
        /// </summary>
        /// <param name="appId">The Application ID you received when you registered your app on the JustGiving developer portal.</param>
        /// <param name="applicationKey">If you created a Application Key for this app in the Developer Portal, you must include it here for your app to work.</param>
        /// <param name="basicAuthCredential">The username and password of a registered JustGiving user.</param>
        public JustGivingApiClient2(string appId, string applicationKey, BasicCredential basicAuthCredential)
        {
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentException("You must provide an appId");
            }

            if (string.IsNullOrWhiteSpace(applicationKey))
            {
                throw new ArgumentException("Application Key cannot be empty. If you have not created an Application Key for your application, use the overloaded version of this constructor.");
            }

            if (basicAuthCredential == null)
            {
                throw new ArgumentException("You must provide a credential");
            }

            _options = new ClientOptions(appId, applicationKey)
            {
                AuthorizationMode = AuthorizationMode.Basic,
                Endpoint = Endpoints.Production,
                BasicAuthSettings = basicAuthCredential,
                LoggingOptions = LoggingOptions.Default
            };
        }

        /// <summary>
        /// Creates an anonymous Api client using common default settings. This client will not be able to access protected Api resources which require authentication.
        /// </summary>
        /// <param name="appId">The Application ID you received when you registered your app on the JustGiving developer portal.</param>
        public JustGivingApiClient2(string appId)
        {
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentException("You must provide an appId");
            }

            _options = new ClientOptions(appId)
            {
                AuthorizationMode = AuthorizationMode.Anonymous,
                Endpoint = Endpoints.Production,
                LoggingOptions = LoggingOptions.Default
            };
        }

        /// <summary>
        /// Creates an anonymous Api client using common default settings. This client will not be able to access protected Api resources which require authentication.
        /// </summary>
        /// <param name="appId">The Appliaction ID you received when you registered your app on the JustGiving developer portal.</param>
        /// <param name="applicationKey">If you created a Application Key for this app in the Developer Portal, you must include it here for your app to work.</param>
        public JustGivingApiClient2(string appId, string applicationKey)
        {
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentException("You must provide an appId");
            }

            if (string.IsNullOrWhiteSpace(applicationKey))
            {
                throw new ArgumentException("Application Key cannot be empty. If you have not created an Application Key for your application, use the overloaded version of this constructor.");
            }

            _options = new ClientOptions(appId, applicationKey)
            {
                AuthorizationMode = AuthorizationMode.Anonymous,
                Endpoint = Endpoints.Production,
                LoggingOptions = LoggingOptions.Default
            };
        }

        /// <summary>
        /// Creates an API client using custom configuration settings. For advanced use.
        /// </summary>
        /// <param name="options"></param>
        public JustGivingApiClient2(ClientOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Tells the API client to use the JustGiving sandbox environment, and turns on verbose logging of all HTTP requests and responses.
        /// </summary>
        public void UseSandbox()
        {
            _options.Endpoint = Endpoints.Sandbox;
        }

        /// <summary>
        /// Tells the API client to write extremely detailed log information about every API request and response. For development & troubleshooting scenarios.
        /// </summary>
        public void LogEverything()
        {
            _options.LoggingOptions.LogAllMessageContent = true;
            _options.LoggingOptions.LogFailedRequests = true;
            _options.LoggingOptions.LogSuccessfulRequests = true;
            _options.RequestDebuggingInfo = true;
        }

        /// <summary>
        /// Tells the API client the use the JustGiving production (live) environment, with some minimal error logging.
        /// </summary>
        public void UseProduction()
        {
            _options.Endpoint = Endpoints.Production;
        }
        
        /// <summary>
        /// API resources for working with user accounts and personalised information.
        /// </summary>
        public AccountClient Accounts
        {
            get
            {
                if (_accounts == null)
                {
                    _accounts = new AccountClient(RestClient, HttpClient, Logger);
                }

                return _accounts;
            }
        }

        /// <summary>
        /// API resources for working with Charity data.
        /// </summary>
        public CharityClient Charities
        {
            get
            {
                if (_charities == null)
                {
                    _charities = new CharityClient(RestClient, HttpClient, Logger);
                }

                return _charities;
            }
        }

        /// <summary>
        /// API resources for working with Country data.
        /// </summary>
        public CountryClient Countries
        {
            get
            {
                if (_countries == null)
                {
                    _countries = new CountryClient(RestClient, HttpClient, Logger);
                }

                return _countries;
            }
        }

        /// <summary>
        /// API resources for working with international currency data.
        /// </summary>
        public CurrencyClient Currency
        {
            get
            {
                if (_currency == null)
                {
                    _currency = new CurrencyClient(RestClient, HttpClient, Logger);
                }

                return _currency;
            }
        }

        /// <summary>
        /// API resources for retrieving donation information.
        /// </summary>
        public DonationClient Donation
        {
            get
            {
                if (_donation == null)
                {
                    _donation = new DonationClient(RestClient, HttpClient, Logger);
                }

                return _donation;
            }
        }

        /// <summary>
        /// API resources for retrieving fundraising event information.
        /// </summary>
        public EventClient Event
        {
            get
            {
                if (_event == null)
                {
                    _event = new EventClient(RestClient, HttpClient, Logger);
                }

                return _event;
            }
        }

        /// <summary>
        /// API resources for working with fundraising pages.
        /// </summary>
        public FundraisingClient Fundraising
        {
            get
            {
                if (_fundraising == null)
                {
                    _fundraising = new FundraisingClient(RestClient, HttpClient, Logger);
                }

                return _fundraising;
            }
        }

        /// <summary>
        /// API resources for working with OneSearch, our unified search API
        /// </summary>
        public OneSearchClient OneSearch
        {
            get
            {
                if (_oneSearch == null)
                {
                    _oneSearch = new OneSearchClient(RestClient, HttpClient, Logger);
                }

                return _oneSearch;
            }
        }

        /// <summary>
        /// Gets or sets the underlying log infrastructure. Defaults to the Log4NetLogProvider.
        /// </summary>
        public ILogProvider LogProvider
        {
            get
            {
                if (_logProvider == null)
                {
                    _logProvider = new Log4NetLogProvider();
                }

                return _logProvider;
            }
            set { _logProvider = value; }
        }
    

        private ApiRequestLogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new ApiRequestLogger(_options, LogProvider);
                }

                return _logger;
            }
        }

        private IRestClient RestClient
        {
            get
            {
                if (_restClient == null)
                {
                    var factory = new RestClientFactory();
                    _restClient = factory.CreateClient(_options);
                }

                return _restClient;
            }
        }

        private HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    var factory = new HttpClientFactory();
                    _httpClient = factory.CreateClient(_options);
                }

                return _httpClient;
            }
        }
    }
}