class JG {
  constructor(url, appId, accessToken) {
    if (typeof url !== 'string') throw new Error('URL is required');
    this._url = url;
    this._appId = appId;
    this._accessToken = accessToken;
    this._version = 'v1';
  }

  _getOptions(payload, method) {
    const options = {method: method || 'GET', headers: new Headers({'x-app-id': this._appId, Accept: 'application/json'})};
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

  _handleResponse(response) {
    if (response.status >= 400) {
      const contentType = response.headers.get('content-type');

      if(contentType && contentType.indexOf('application/json') === 0) {
        return response.json().then(json => {
          if (json[0]) {
            throw new Error(`${response.status} ${response.statusText}. ${json[0].id} : ${json[0].desc}`);
          }
        });
      }

      throw new Error(`${response.status} ${response.statusText}`);
    }

    return response.json();
  };

  _fetch(resource, payload, method) {
    return fetch(`${this._url}/${this._version}/${resource}`, this._getOptions(payload, method)).then(this._handleResponse);
  }

  // Account resource
  validateAccount(email, password) {
    return this._fetch('account/validate', {email: email, password: password});
  }

  getFundraisingPagesForUser(email, charityId) {
    const charityRestriction = (charityId?`?charityId=${charityId}`:'');
    return this._fetch(`account/${email}/pages${charityRestriction}`);
  }

  getDonationsForUser(pageSize, pageNum, charityId) {
    const charityRestriction = (charityId?`charityId=${charityId}&`:'');
    const pageSizeRestriction = (pageSize?`pageSize=${pageSize}&`:'');
    const pageNumRestriction = (pageNum?`pageNum=${pageNum}&`:'');
    return this._fetch(`account/donations?${pageSizeRestriction}${pageNumRestriction}${charityRestriction}`);
  }

  checkAccountAvailability(email) {
    return this._fetch(`account/${email}`);
  }

  getContentFeed() {
    return this._fetch('account/feed');
  }

  getAccountRating(pageSize, pageNum) {
    const pageSizeRestriction = (pageSize?`pageSize=${pageSize}&`:'');
    const pageNumRestriction = (pageNum?`page=${pageNum}&`:'');
    return this._fetch(`account/rating?${pageSizeRestriction}${pageNumRestriction}`);
  }

  getAccount() {
    return this._fetch('account');
  };

  getInterests() {
    return this._fetch('account/interest');
  }

  addInterest(interest) {
    return this._fetch('account/interest', {interest: interest});
  }

  replaceInterests(...interests) {
    return this._fetch('account/interest', interests, 'PUT');
  }

  requestPasswordReminder(email) {
    return this._fetch(`account/${email}/requestpasswordreminder`);
  }

  changePassword(email, currentPassword, newPassword) {
    if (!email || !currentPassword || !newPassword) throw new Error('All parameters are required');
    return this._fetch(`account/changePassword?emailaddress=${email}&currentpassword=${encodeURIComponent(currentPassword)}&newpassword=${encodeURIComponent(newPassword)}`, undefined, 'POST');
  }

  // Countries resource
  getCountries() {
    return this._fetch('countries');
  };

  // Currency resource
  getCurrencies() {
    return this._fetch('fundraising/currencies');
  };

  // Charity resource
  getCharityCategories() {
    return this._fetch('charity/categories');
  };

  getCharity(charityId) {
    return this._fetch(`charity/${charityId}`);
  }

  getEventsByCharity(charityId, pageSize, pageNum) {
    const pageSizeRestriction = (pageSize?`pageSize=${pageSize}&`:'');
    const pageNumRestriction = (pageNum?`pageNum=${pageNum}&`:'');

    return this._fetch(`charity/${charityId}/events?${pageSizeRestriction}${pageNumRestriction}`);
  }

  // Donation resource
  getDonation(donationId) {
    return this._fetch(`donation/${donationId}`);
  }

  getDonationByReference(thirdPartyReference) {
    return this._fetch(`donation/ref/${encodeURIComponent(thirdPartyReference)}`);
  }

  getDonationStatus(donationId) {
    return this._fetch(`donation/${donationId}/status`);
  }

  // Event resource
  getEvent(eventId) {
    return this._fetch(`event/${eventId}`);
  }

  getEventPages(eventId, pageSize, pageNum) {
    const pageSizeRestriction = (pageSize?`pageSize=${pageSize}&`:'');
    const pageNumRestriction = (pageNum?`page=${pageNum}&`:'');

    return this._fetch(`event/${eventId}/pages?${pageSizeRestriction}${pageNumRestriction}`);
  }

  registerEvent(eventDetails) {
    return this._fetch('event', eventDetails);
  }

  // Fundraising resource
  getFundraisingPages() {
    return this._fetch('fundraising/pages');
  }

  getFundraisingPage(pageShortName) {
    return this._fetch(`fundraising/pages/${encodeURIComponent(pageShortName)}`);
  }

  suggestPageShortName(preferredName) {
    return this._fetch(`fundraising/pages/suggest?preferredName=${encodeURIComponent(preferredName)}`);
  }

  // OneSearch resource
  onesearch(searchTerm, grouping, index, pageSize, pageNum, country) {
    return this._fetch(`onesearch?q=${encodeURIComponent(searchTerm)}&g=${encodeURIComponent(grouping)}&i=${encodeURIComponent(index)}&limit=${pageSize}&offset=${pageNum}&country=${country}`);
  }

  // Project resource
  getProject(projectId) {
    return this._fetch(`project/global/${projectId}`);
  }

  // Search resource
  searchCharities(searchTerm, charityId, categoryId, pageNum, pageSize) {
    const pageSizeRestriction = (pageSize?`pageSize=${pageSize}&`:'');
    const pageNumRestriction = (pageNum?`page=${pageNum}&`:'');
    const charityIdRestriction = charityId.length?charityId.map(id => `charityId=${id}&`).join():`charityId=${charityId}&`;
    const categoryIdRestriction = categoryId.length?categoryId.map(id => `categoryId=${id}&`).join():`categoryId=${categoryId}&`;
    return this._fetch(`charity/search?q=${encodeURIComponent(searchTerm)}&${categoryIdRestriction}${charityIdRestriction}${pageSizeRestriction}${pageNumRestriction}`);
  }

  searchEvents(searchTerm, pageNum, pageSize) {
    const pageSizeRestriction = (pageSize?`pageSize=${pageSize}&`:'');
    const pageNumRestriction = (pageNum?`page=${pageNum}&`:'');
    return this._fetch(`event/search?q=${encodeURIComponent(searchTerm)}&${pageSizeRestriction}${pageNumRestriction}`);
  }

  // Team resource
  getTeam(shortName) {
    return this._fetch(`team/${encodeURIComponent(shortName)}`);
  }

  checkTeamExists(shortName) {
    return this._fetch(`team/${encodeURIComponent(shortName)}`, undefined, 'HEAD');
  }

  createOrUpdateTeam(shortName, details) {
    return this._fetch(`team/${encodeURIComponent(shortName)}`, details, 'PUT');
  }

  joinTeam(teamShortName, pageShortName) {
    return this._fetch(`team/join/${encodeURIComponent(teamShortName)}`, {pageShortName: pageShortName}, 'PUT');
  }
}