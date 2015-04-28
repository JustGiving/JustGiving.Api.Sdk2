using System.Threading.Tasks;
using JustGiving.Api.Sdk2.Model.Donation.Response;
using RestSharp;

namespace JustGiving.Api.Sdk2.Clients.Donation
{
    public interface IDonationClient
    {
        Task<IRestResponse<DonationsByReference>> RetrieveDonationDetailsByReference(string thirdPartyReference, int pageNum = 1, int pageSize = 100);
        Task<IRestResponse<Model.Donation.Response.Donation>> RetrieveDonationDetails(int donationId);
        Task<IRestResponse<DonationStatus>> RetrieveDonationStatus(int donationId);
    }
}