namespace Sentinel.Domain.Models.Authentication
{
    public enum RequestEncoding
    {
        // application/x-www-form-urlencoded
        FormUrlEncoding,
        // multipart/form-data
        MultiPartFormData,
        Json
    }
}