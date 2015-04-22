/**
 * JG - JustGiving API SDK
 * @version v2.0.0
 * @link https://api.justgiving.com/
 * @license MIT
 */
(function (global, factory) {
  if (typeof define === 'function' && define.amd) {
    define('JustGiving', ['exports'], factory);
  } else if (typeof exports !== 'undefined') {
    factory(exports);
  } else {
    var mod = {
      exports: {}
    };
    factory(mod.exports);
    global.JustGiving = mod.exports;
  }
})(this, function (exports) {
  'use strict';

  var _classCallCheck = function (instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } };

  Object.defineProperty(exports, '__esModule', {
    value: true
  });

  var ApiClient = (function () {
    function ApiClient(url, appId, accessToken) {
      _classCallCheck(this, ApiClient);

      if (typeof url !== 'string') throw new Error('URL is required');
      this._url = url;
      this._appId = appId;
      this._accessToken = accessToken;
      this._version = 'v1';
    }

    ApiClient.prototype._getOptions = function _getOptions(payload, method) {
      var options = { method: method || 'GET', headers: new Headers({ 'x-app-id': this._appId, Accept: 'application/json' }) };
      if (this._accessToken) {
        options.headers.append('Authorization', this._accessToken);
      }
      if (payload || method === 'POST') {
        options.method = method || 'POST';
        options.body = JSON.stringify(payload);
        options.headers.append('Content-Type', 'application/json');
      }
      return options;
    };

    ApiClient.prototype._handleResponse = function _handleResponse(response) {
      if (response.status >= 400) {
        var contentType = response.headers.get('content-type');

        if (contentType && contentType.indexOf('application/json') === 0) {
          return response.json().then(function (json) {
            if (json[0]) {
              throw new Error('' + response.status + ' ' + response.statusText + '. ' + json[0].id + ' : ' + json[0].desc);
            }
          });
        }

        throw new Error('' + response.status + ' ' + response.statusText);
      }

      return response.json();
    };

    ApiClient.prototype._fetch = function _fetch(resource, payload, method) {
      return fetch('' + this._url + '/' + this._version + '/' + resource, this._getOptions(payload, method)).then(this._handleResponse);
    };

    // Account resource

    ApiClient.prototype.validateAccount = function validateAccount(email, password) {
      return this._fetch('account/validate', { email: email, password: password });
    };

    ApiClient.prototype.getFundraisingPagesForUser = function getFundraisingPagesForUser(email, charityId) {
      var charityRestriction = charityId ? '?charityId=' + charityId : '';
      return this._fetch('account/' + email + '/pages' + charityRestriction);
    };

    ApiClient.prototype.getDonationsForUser = function getDonationsForUser(pageSize, pageNum, charityId) {
      var charityRestriction = charityId ? 'charityId=' + charityId + '&' : '';
      var pageSizeRestriction = pageSize ? 'pageSize=' + pageSize + '&' : '';
      var pageNumRestriction = pageNum ? 'pageNum=' + pageNum + '&' : '';
      return this._fetch('account/donations?' + pageSizeRestriction + '' + pageNumRestriction + '' + charityRestriction);
    };

    ApiClient.prototype.checkAccountAvailability = function checkAccountAvailability(email) {
      return this._fetch('account/' + encodeURIComponent(email));
    };

    ApiClient.prototype.getContentFeed = function getContentFeed() {
      return this._fetch('account/feed');
    };

    ApiClient.prototype.getAccountRating = function getAccountRating(pageSize, pageNum) {
      var pageSizeRestriction = pageSize ? 'pageSize=' + pageSize + '&' : '';
      var pageNumRestriction = pageNum ? 'page=' + pageNum + '&' : '';
      return this._fetch('account/rating?' + pageSizeRestriction + '' + pageNumRestriction);
    };

    ApiClient.prototype.getAccount = function getAccount() {
      return this._fetch('account');
    };

    ApiClient.prototype.getInterests = function getInterests() {
      return this._fetch('account/interest');
    };

    ApiClient.prototype.addInterest = function addInterest(interest) {
      return this._fetch('account/interest', { interest: interest });
    };

    ApiClient.prototype.replaceInterests = function replaceInterests() {
      for (var _len = arguments.length, interests = Array(_len), _key = 0; _key < _len; _key++) {
        interests[_key] = arguments[_key];
      }

      return this._fetch('account/interest', interests, 'PUT');
    };

    ApiClient.prototype.requestPasswordReminder = function requestPasswordReminder(email) {
      return this._fetch('account/' + email + '/requestpasswordreminder');
    };

    ApiClient.prototype.changePassword = function changePassword(email, currentPassword, newPassword) {
      if (!email || !currentPassword || !newPassword) throw new Error('All parameters are required');
      return this._fetch('account/changePassword?emailaddress=' + email + '&currentpassword=' + encodeURIComponent(currentPassword) + '&newpassword=' + encodeURIComponent(newPassword), undefined, 'POST');
    };

    // Countries resource

    ApiClient.prototype.getCountries = function getCountries() {
      return this._fetch('countries');
    };

    // Currency resource

    ApiClient.prototype.getCurrencies = function getCurrencies() {
      return this._fetch('fundraising/currencies');
    };

    // Charity resource

    ApiClient.prototype.getCharityCategories = function getCharityCategories() {
      return this._fetch('charity/categories');
    };

    ApiClient.prototype.getCharity = function getCharity(charityId) {
      return this._fetch('charity/' + charityId);
    };

    ApiClient.prototype.getEventsByCharity = function getEventsByCharity(charityId, pageSize, pageNum) {
      var pageSizeRestriction = pageSize ? 'pageSize=' + pageSize + '&' : '';
      var pageNumRestriction = pageNum ? 'pageNum=' + pageNum + '&' : '';

      return this._fetch('charity/' + charityId + '/events?' + pageSizeRestriction + '' + pageNumRestriction);
    };

    // Donation resource

    ApiClient.prototype.getDonation = function getDonation(donationId) {
      return this._fetch('donation/' + donationId);
    };

    ApiClient.prototype.getDonationByReference = function getDonationByReference(thirdPartyReference) {
      return this._fetch('donation/ref/' + encodeURIComponent(thirdPartyReference));
    };

    ApiClient.prototype.getDonationStatus = function getDonationStatus(donationId) {
      return this._fetch('donation/' + donationId + '/status');
    };

    // Event resource

    ApiClient.prototype.getEvent = function getEvent(eventId) {
      return this._fetch('event/' + eventId);
    };

    ApiClient.prototype.getEventPages = function getEventPages(eventId, pageSize, pageNum) {
      var pageSizeRestriction = pageSize ? 'pageSize=' + pageSize + '&' : '';
      var pageNumRestriction = pageNum ? 'page=' + pageNum + '&' : '';

      return this._fetch('event/' + eventId + '/pages?' + pageSizeRestriction + '' + pageNumRestriction);
    };

    ApiClient.prototype.registerEvent = function registerEvent(eventDetails) {
      return this._fetch('event', eventDetails);
    };

    // Fundraising resource

    ApiClient.prototype.getFundraisingPages = function getFundraisingPages() {
      return this._fetch('fundraising/pages');
    };

    ApiClient.prototype.getFundraisingPage = function getFundraisingPage(pageShortName) {
      return this._fetch('fundraising/pages/' + encodeURIComponent(pageShortName));
    };

    ApiClient.prototype.suggestPageShortName = function suggestPageShortName(preferredName) {
      return this._fetch('fundraising/pages/suggest?preferredName=' + encodeURIComponent(preferredName));
    };

    // OneSearch resource

    ApiClient.prototype.onesearch = function onesearch(searchTerm, grouping, index, pageSize, pageNum, country) {
      return this._fetch('onesearch?q=' + encodeURIComponent(searchTerm) + '&g=' + encodeURIComponent(grouping) + '&i=' + encodeURIComponent(index) + '&limit=' + pageSize + '&offset=' + pageNum + '&country=' + country);
    };

    // Project resource

    ApiClient.prototype.getProject = function getProject(projectId) {
      return this._fetch('project/global/' + projectId);
    };

    // Search resource

    ApiClient.prototype.searchCharities = function searchCharities(searchTerm, charityId, categoryId, pageNum, pageSize) {
      var pageSizeRestriction = pageSize ? 'pageSize=' + pageSize + '&' : '';
      var pageNumRestriction = pageNum ? 'page=' + pageNum + '&' : '';
      var charityIdRestriction = charityId.length ? charityId.map(function (id) {
        return 'charityId=' + id + '&';
      }).join() : 'charityId=' + charityId + '&';
      var categoryIdRestriction = categoryId.length ? categoryId.map(function (id) {
        return 'categoryId=' + id + '&';
      }).join() : 'categoryId=' + categoryId + '&';
      return this._fetch('charity/search?q=' + encodeURIComponent(searchTerm) + '&' + categoryIdRestriction + '' + charityIdRestriction + '' + pageSizeRestriction + '' + pageNumRestriction);
    };

    ApiClient.prototype.searchEvents = function searchEvents(searchTerm, pageNum, pageSize) {
      var pageSizeRestriction = pageSize ? 'pageSize=' + pageSize + '&' : '';
      var pageNumRestriction = pageNum ? 'page=' + pageNum + '&' : '';
      return this._fetch('event/search?q=' + encodeURIComponent(searchTerm) + '&' + pageSizeRestriction + '' + pageNumRestriction);
    };

    // Team resource

    ApiClient.prototype.getTeam = function getTeam(shortName) {
      return this._fetch('team/' + encodeURIComponent(shortName));
    };

    ApiClient.prototype.checkTeamExists = function checkTeamExists(shortName) {
      return this._fetch('team/' + encodeURIComponent(shortName), undefined, 'HEAD');
    };

    ApiClient.prototype.createOrUpdateTeam = function createOrUpdateTeam(shortName, details) {
      return this._fetch('team/' + encodeURIComponent(shortName), details, 'PUT');
    };

    ApiClient.prototype.joinTeam = function joinTeam(teamShortName, pageShortName) {
      return this._fetch('team/join/' + encodeURIComponent(teamShortName), { pageShortName: pageShortName }, 'PUT');
    };

    return ApiClient;
  })();

  exports.ApiClient = ApiClient;
});
//# sourceMappingURL=justgiving-apiclient.js.map