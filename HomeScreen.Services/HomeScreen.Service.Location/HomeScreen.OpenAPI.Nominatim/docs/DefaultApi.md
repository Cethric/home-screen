# HomeScreen.OpenAPI.Nominatim.Api.DefaultApi

All URIs are relative to *https://nominatim.openstreetmap.org*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReverseGet**](DefaultApi.md#reverseget) | **GET** /reverse | Reverse geocoding generates an address from a coordinate given as latitude and longitude. |

<a id="reverseget"></a>
# **ReverseGet**
> ReverseOutputJson ReverseGet (double lat, double lon, OutputFormat? format = null, string? jsonCallback = null, NumberBoolean? addressdetails = null, NumberBoolean? extratags = null, NumberBoolean? namedetails = null, NumberBoolean? entrances = null, string? acceptLanguage = null, int? zoom = null, Layer? layer = null, decimal? polygonGeojson = null, decimal? polygonKml = null, decimal? polygonSvg = null, decimal? polygonText = null, float? polygonThreshold = null, string? email = null, decimal? debug = null)

Reverse geocoding generates an address from a coordinate given as latitude and longitude.

# How it works The reverse geocoding API does not exactly compute the address for the coordinate it receives. It works by finding the closest suitable OSM object and returning its address information. This may occasionally lead to unexpected results.  First of all, Nominatim only includes OSM objects in its index that are suitable for searching. Small, unnamed paths for example are missing from the database and can therefore not be used for reverse geocoding either.  The other issue to be aware of is that the closest OSM object may not always have a similar enough address to the coordinate you were requesting. For example, in dense city areas it may belong to a completely different street. 

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using HomeScreen.OpenAPI.Nominatim.Api;
using HomeScreen.OpenAPI.Nominatim.Client;
using HomeScreen.OpenAPI.Nominatim.Model;

namespace Example
{
    public class ReverseGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://nominatim.openstreetmap.org";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new DefaultApi(httpClient, config, httpClientHandler);
            var lat = 1.2D;  // double | Latitude of a coordinate in WGS84 projection
            var lon = 1.2D;  // double | Longitude of a coordinate in WGS84 projection
            var format = new OutputFormat?(); // OutputFormat? |  (optional) 
            var jsonCallback = "jsonCallback_example";  // string? | When given, then JSON output will be wrapped in a callback function with the given name. See JSONP for more  information.    Only has an effect for JSON output formats.  (optional) 
            var addressdetails = new NumberBoolean?(); // NumberBoolean? | When set to 1, include a breakdown of the address into elements. The exact content of the address breakdown  depends on the output format.  (optional) 
            var extratags = new NumberBoolean?(); // NumberBoolean? | When set to 1, the response include any additional information in the result that is available in the database, e.g. wikipedia link, opening hours.  (optional) 
            var namedetails = new NumberBoolean?(); // NumberBoolean? | When set to 1, include a full list of names for the result. These may include language variants, older names, references and brand.  (optional) 
            var entrances = new NumberBoolean?(); // NumberBoolean? | When set to 1, include the tagged entrances in the result.  (optional) 
            var acceptLanguage = "\"en\"";  // string? | Preferred language order for showing search results. This may either be a simple comma-separated list of language codes or have the same format as the [\"Accept-Language\" HTTP header.](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Accept-Language)  (optional)  (default to "en")
            var zoom = 56;  // int? | Level of detail required for the address. This is a number that corresponds roughly to the zoom level used in XYZ tile sources in frameworks like Leaflet.js, Openlayers etc.  (optional) 
            var layer = new Layer?(); // Layer? |  (optional) 
            var polygonGeojson = 0MD;  // decimal? |  (optional)  (default to 0M)
            var polygonKml = 0MD;  // decimal? |  (optional)  (default to 0M)
            var polygonSvg = 0MD;  // decimal? |  (optional)  (default to 0M)
            var polygonText = 0MD;  // decimal? |  (optional)  (default to 0M)
            var polygonThreshold = 0F;  // float? | When one of the polygon_* outputs is chosen, return a simplified version of the output geometry. The parameter describes the tolerance in degrees with which the geometry may differ from the original geometry. Topology is preserved in the geometry.  (optional)  (default to 0F)
            var email = "email_example";  // string? | If you are making large numbers of request please include an appropriate email address to identify your requests. See Nominatim's Usage Policy for more details.  (optional) 
            var debug = 0MD;  // decimal? | Output assorted developer debug information. Data on internals of Nominatim's \"search loop\" logic, and SQL queries. The output is HTML format. This overrides the specified machine readable format.  (optional)  (default to 0M)

            try
            {
                // Reverse geocoding generates an address from a coordinate given as latitude and longitude.
                ReverseOutputJson result = apiInstance.ReverseGet(lat, lon, format, jsonCallback, addressdetails, extratags, namedetails, entrances, acceptLanguage, zoom, layer, polygonGeojson, polygonKml, polygonSvg, polygonText, polygonThreshold, email, debug);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DefaultApi.ReverseGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReverseGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Reverse geocoding generates an address from a coordinate given as latitude and longitude.
    ApiResponse<ReverseOutputJson> response = apiInstance.ReverseGetWithHttpInfo(lat, lon, format, jsonCallback, addressdetails, extratags, namedetails, entrances, acceptLanguage, zoom, layer, polygonGeojson, polygonKml, polygonSvg, polygonText, polygonThreshold, email, debug);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DefaultApi.ReverseGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **lat** | **double** | Latitude of a coordinate in WGS84 projection |  |
| **lon** | **double** | Longitude of a coordinate in WGS84 projection |  |
| **format** | [**OutputFormat?**](OutputFormat?.md) |  | [optional]  |
| **jsonCallback** | **string?** | When given, then JSON output will be wrapped in a callback function with the given name. See JSONP for more  information.    Only has an effect for JSON output formats.  | [optional]  |
| **addressdetails** | [**NumberBoolean?**](NumberBoolean?.md) | When set to 1, include a breakdown of the address into elements. The exact content of the address breakdown  depends on the output format.  | [optional]  |
| **extratags** | [**NumberBoolean?**](NumberBoolean?.md) | When set to 1, the response include any additional information in the result that is available in the database, e.g. wikipedia link, opening hours.  | [optional]  |
| **namedetails** | [**NumberBoolean?**](NumberBoolean?.md) | When set to 1, include a full list of names for the result. These may include language variants, older names, references and brand.  | [optional]  |
| **entrances** | [**NumberBoolean?**](NumberBoolean?.md) | When set to 1, include the tagged entrances in the result.  | [optional]  |
| **acceptLanguage** | **string?** | Preferred language order for showing search results. This may either be a simple comma-separated list of language codes or have the same format as the [\&quot;Accept-Language\&quot; HTTP header.](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Accept-Language)  | [optional] [default to &quot;en&quot;] |
| **zoom** | **int?** | Level of detail required for the address. This is a number that corresponds roughly to the zoom level used in XYZ tile sources in frameworks like Leaflet.js, Openlayers etc.  | [optional]  |
| **layer** | [**Layer?**](Layer?.md) |  | [optional]  |
| **polygonGeojson** | **decimal?** |  | [optional] [default to 0M] |
| **polygonKml** | **decimal?** |  | [optional] [default to 0M] |
| **polygonSvg** | **decimal?** |  | [optional] [default to 0M] |
| **polygonText** | **decimal?** |  | [optional] [default to 0M] |
| **polygonThreshold** | **float?** | When one of the polygon_* outputs is chosen, return a simplified version of the output geometry. The parameter describes the tolerance in degrees with which the geometry may differ from the original geometry. Topology is preserved in the geometry.  | [optional] [default to 0F] |
| **email** | **string?** | If you are making large numbers of request please include an appropriate email address to identify your requests. See Nominatim&#39;s Usage Policy for more details.  | [optional]  |
| **debug** | **decimal?** | Output assorted developer debug information. Data on internals of Nominatim&#39;s \&quot;search loop\&quot; logic, and SQL queries. The output is HTML format. This overrides the specified machine readable format.  | [optional] [default to 0M] |

### Return type

[**ReverseOutputJson**](ReverseOutputJson.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/xml


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successfully retrieve the current forecast |  -  |
| **0** | Anything other than success is an error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

